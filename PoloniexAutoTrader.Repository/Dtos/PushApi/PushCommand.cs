using Newtonsoft.Json;


namespace PoloniexAutoTrader.Repository.Dtos.PushApi {

  public sealed class PushCommand<T> {

    [JsonProperty("command")]
    public string Command { get; set; }

    [JsonProperty("channel")]
    public T Channel { get; set; }

  }

}
