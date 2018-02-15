

namespace PoloniexAutoTrader.Repository.Dtos.PublicApi {

  public class CurrencyDto {

    public int Id { get; set; }

    public string Name { get; set; }

    public string TxFee { get; set; }

    public int MinConf { get; set; }

    public string DepositAddress { get; set; }

    public int Disabled { get; set; }

    public int Delisted { get; set; }

    public int Frozen { get; set; }

  }

}
