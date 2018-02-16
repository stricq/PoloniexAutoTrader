using Newtonsoft.Json;

using PoloniexAutoTrader.Domain.Models.PushApi;

using PoloniexAutoTrader.Repository.Helpers;


namespace PoloniexAutoTrader.Repository.Dtos.PushApi {

  [JsonConverter(typeof(PushObjectConverter<TickerMessageDto>))]
  public sealed class TickerMessageDto : PushMessageDtoBase {

    [JsonProperty(Order = 1)]
    public int Id { get; set; }

    [JsonProperty(Order = 2)]
    public string Last { get; set; }

    [JsonProperty(Order = 3)]
    public string LowestAsk { get; set; }

    [JsonProperty(Order = 4)]
    public string HighestBid { get; set; }

    [JsonProperty(Order = 5)]
    public string PercentChange { get; set; }

    [JsonProperty(Order = 6)]
    public string BaseVolume { get; set; }

    [JsonProperty(Order = 7)]
    public string QuoteVolume { get; set; }

    [JsonProperty(Order = 8)]
    public int IsFrozen { get; set; }

    [JsonProperty(Order = 9)]
    public string High24hr { get; set; }

    [JsonProperty(Order = 10)]
    public string Low24hr { get; set; }

  }

}
