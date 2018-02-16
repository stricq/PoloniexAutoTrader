using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


namespace PoloniexAutoTrader.Repository.Helpers {

  public sealed class PushObjectConverter<T> : JsonConverter {

    #region Overrides

    public override bool CanConvert(Type objectType) {
      return typeof(T) == objectType;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
      Type type = value.GetType();

      if (!(serializer.ContractResolver.ResolveContract(type) is JsonObjectContract contract)) throw new JsonSerializationException("invalid type " + type.FullName);

      IEnumerable<object> list = contract.Properties.Where(p => !shouldSkip(p)).Select(p => p.ValueProvider.GetValue(value));

      serializer.Serialize(writer, list);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
      if (reader.TokenType == JsonToken.Null) return null;

      JArray token = JArray.Load(reader);

      if (!(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract contract)) throw new JsonSerializationException("invalid type " + objectType.FullName);

      object value = existingValue ?? contract.DefaultCreator();

      foreach(var pair in contract.Properties.Where(p => !shouldSkip(p)).Zip(token, (p, v) => new { Value = v, Property = p })) {
        object propertyValue = pair.Value.ToObject(pair.Property.PropertyType, serializer);

        pair.Property.ValueProvider.SetValue(value, propertyValue);
      }

      return value;
    }

    #endregion Overrides

    #region Private Methods

    private static bool shouldSkip(JsonProperty property) {
      return property.Ignored || !property.Readable || !property.Writable;
    }

    #endregion Private Methods

  }

}
