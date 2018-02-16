

namespace PoloniexAutoTrader.Domain.Models.PushApi {

  public class TickerMessage : PushMessageBase {

    #region Properties

    public int Id { get; set; }

    public double Last { get; set; }

    public double LowestAsk { get; set; }

    public double HighestBid { get; set; }

    public double PercentChange { get; set; }

    public double BaseVolume { get; set; }

    public double QuoteVolume { get; set; }

    public bool IsFrozen { get; set; }

    public double High24Hour { get; set; }

    public double Low24Hour { get; set; }

    #endregion Properties

  }

}
