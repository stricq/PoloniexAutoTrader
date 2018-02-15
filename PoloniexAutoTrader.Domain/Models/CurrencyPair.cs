

namespace PoloniexAutoTrader.Domain.Models {

  public class CurrencyPair {

    #region Constructor

    public CurrencyPair(string MarketCurrency) {
      string[] parts = MarketCurrency.Split('_');

      Market   = parts[0];
      Currency = parts[1];
    }

    #endregion Constructor

    #region Properties

    public string Market { get; set; }

    public string Currency { get; set; }

    #endregion Properties

    #region Domain Properties

    public string Pair => $"{Market}_{Currency}";

    #endregion Domain Properties

    #region Overrides

    public override string ToString() => Pair;

    #endregion Overrides

  }

}
