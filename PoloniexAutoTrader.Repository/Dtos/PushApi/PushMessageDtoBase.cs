using Newtonsoft.Json;


namespace PoloniexAutoTrader.Repository.Dtos.PushApi {

  public class PushMessageDtoBase {

    [JsonIgnore]
    public int Channel { get; set; }

    [JsonIgnore]
    public long? Sequence { get; set; }

  }

}
