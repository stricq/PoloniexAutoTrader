

namespace PoloniexAutoTrader.Domain.Models.PublicApi {

  public sealed class Ticker {

    #region Properties

    public CurrencyPair CurrencyPair { get; set; }

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

    #region Domain Properties

    public Currency Currency { get; set; }

    #endregion Domain Properties

    #region Overrides

    public override string ToString() {
      return $"Id: {Id}; Currency: {CurrencyPair.Pair}";
    }

    #endregion Overrides

  }

}
