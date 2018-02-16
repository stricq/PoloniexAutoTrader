using System;
using System.Threading.Tasks;

using PoloniexAutoTrader.Domain.Models.PushApi;


namespace PoloniexAutoTrader.Domain.Contracts {

  public interface IPushApiService {

    Task SubscribeToTickerEvents(Action<PushMessageBase> TickerCallback);

    Task SendHeartbeat();

    Task ShutdownAsync();

  }

}
