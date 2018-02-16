

namespace PoloniexAutoTrader.Domain.Models.PushApi {

  public class PushMessageBase {

    public int Channel { get; set; }

    public long? Sequence { get; set; }

  }

}
