using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using AutoMapper;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PoloniexAutoTrader.Domain.Constants;
using PoloniexAutoTrader.Domain.Contracts;
using PoloniexAutoTrader.Domain.Models.PushApi;

using PoloniexAutoTrader.Repository.Dtos.PushApi;

using STR.Common.Extensions;

using WebsocketClientLite.PCL;


namespace PoloniexAutoTrader.Repository.Services {

  [Export(typeof(IPushApiService))]
  public sealed class PushApiService : IPushApiService {

    #region Private Fields

    private MessageWebSocketRx client;

    private IObservable<string> observer;

    private IDisposable subscribe;

    private Dictionary<int, Action<PushMessageBase>> messageCallbacks;

    private readonly IMapper mapper;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public PushApiService(IMapper Mapper) {
      mapper = Mapper;

      messageCallbacks = new Dictionary<int, Action<PushMessageBase>>();
    }

    #endregion Constructor

    #region IPushApiService Implementation

    public async Task SubscribeToTickerEvents(Action<PushMessageBase> TickerCallback) {
      if (client == null) await connectToServer();

      PushCommand<int> tickerCommand = new PushCommand<int> { Command = "subscribe", Channel = Channel.Ticker };

      messageCallbacks[Channel.Ticker] = TickerCallback;

      await client.SendTextAsync(JsonConvert.SerializeObject(tickerCommand));
    }

    public async Task SendHeartbeat() {
      if (client == null) return;

      await client.SendTextAsync(".");
    }

    public async Task ShutdownAsync() {
      if (client == null) return;

      await client.CloseAsync();

      subscribe.Dispose();

      client = null;

      messageCallbacks.Clear();
    }

    #endregion IPushApiService Implementation

    #region Private Methods

    private async Task connectToServer() {
      List<string> subProtocols = new List<string> { "json" };

      Dictionary<string, string> headers = new Dictionary<string, string> { { "Pragma", "no-cache" }, { "Cache-Control", "no-cache" } };

      client = new MessageWebSocketRx();

      observer = await client.CreateObservableMessageReceiver(new Uri("wss://api2.poloniex.com:443"), ignoreServerCertificateErrors: false, headers: headers, subProtocols: subProtocols);

      subscribe = observer.Subscribe(convert, ex => Console.WriteLine(ex.Message), () => Console.WriteLine("Subscription Closed"));
    }

    private void convert(string message) {
      JArray msg = JArray.Parse(message);

      if (msg.Count > 1 && msg[1].Type != JTokenType.Null && msg[1].Value<long>() == 1) return;

      switch(msg[0].Value<int>()) {
        case Channel.Ticker: {
          TickerMessageDto ticker = msg[2].ToObject<TickerMessageDto>();

          ticker.Channel = Channel.Ticker;

          if (messageCallbacks.ContainsKey(Channel.Ticker)) messageCallbacks[Channel.Ticker](mapper.Map<TickerMessage>(ticker));

          break;
        }
        case Channel.HeartBeat: {
          SendHeartbeat().FireAndForget();

          break;
        }
      }


    }

    #endregion Private Methods

  }

}
