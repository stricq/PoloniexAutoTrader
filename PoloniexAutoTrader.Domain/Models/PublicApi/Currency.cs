

namespace PoloniexAutoTrader.Domain.Models.PublicApi {

  public class Currency {

    #region Properties

    public string Name { get; set; }

    public int Id { get; set; }

    public string Description { get; set; }

    public double TransactionFee { get; set; }

    public int MinimumConf { get; set; }

    public string DepositAddress { get; set; }

    public bool IsDisabled { get; set; }

    public bool IsDelisted { get; set; }

    public bool IsFrozen { get; set; }

    #endregion Properties

    #region Overrides

    public override string ToString() {
      return $"{Name}; {Description}";
    }

    #endregion Overrides

  }

}
