using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using AutoMapper;

using PoloniexAutoTrader.Domain.Contracts;
using PoloniexAutoTrader.Domain.Models.PublicApi;
using PoloniexAutoTrader.Domain.Models.PushApi;
using PoloniexAutoTrader.Wpf.Messages.Status;
using PoloniexAutoTrader.Wpf.ViewEntities;
using PoloniexAutoTrader.Wpf.ViewModels;

using STR.Common.Contracts;
using STR.Common.Extensions;
using STR.Common.Messages;
using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;


namespace PoloniexAutoTrader.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class MarketsController : IController {

    #region Private Fields

    private DateTime lastHeartbeat;

    private List<Ticker> tickers;

    private List<Currency> currencies;

    private readonly MarketsViewModel viewModel;

    private readonly IAsyncService asyncService;

    private readonly IMessenger messenger;

    private readonly IMapper mapper;

    private readonly IPublicApiService publicApiService;

    private readonly IPushApiService pushApiService;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public MarketsController(MarketsViewModel ViewModel, IAsyncService AsyncService, IMessenger Messenger, IMapper Mapper, IPublicApiService PublicApiService, IPushApiService PushApiService) {
      viewModel = ViewModel;

      asyncService = AsyncService;
      messenger    = Messenger;

      mapper = Mapper;

      publicApiService = PublicApiService;

      pushApiService = PushApiService;
    }

    #endregion Constructor

    #region IController Implementation

    public int InitializePriority { get; } = 200;

    public async Task InitializeAsync() {
      tickers = await publicApiService.GetTickersAsync();

      currencies = await publicApiService.GetCurrencies();

      tickers.ForEach(ticker => {
        ticker.Currency = currencies.SingleOrDefault(currency => currency.Name == ticker.CurrencyPair.Currency);
      });

      List<MarketTabViewModel> markets = tickers.GroupBy(ticker => ticker.CurrencyPair.Market)
                                                .Select(g => new MarketTabViewModel { Name = g.Key, Currencies = new ObservableCollection<CurrencyViewEntity>(mapper.Map<List<CurrencyViewEntity>>(g.OrderBy(ticker => ticker.CurrencyPair.Currency))) })
                                                .OrderBy(mtvm => mtvm.Name)
                                                .ToList();

      markets.SelectMany(market => market.Currencies).ForEach(currency => currency.FavoriteClick = new RelayCommand<MouseButtonEventArgs>(onFavoriteClick));

      viewModel.MarketTabs = new ObservableCollection<MarketTabViewModel>(markets);

      await pushApiService.SubscribeToTickerEvents(onTickerEventCallback);

      lastHeartbeat = DateTime.Now;

      registerMessages();
    }

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
      messenger.RegisterAsync<StatusTickMessage>(this, onStatusTickAsync);

      messenger.Register<ApplicationClosingMessage>(this, onApplicationClosing);
    }

    private async Task onStatusTickAsync(StatusTickMessage message) {
      if ((DateTime.Now - lastHeartbeat).TotalSeconds >= 60) {
        await pushApiService.SendHeartbeat();

        lastHeartbeat = DateTime.Now;;
      }
    }

    private void onApplicationClosing(ApplicationClosingMessage message) {
      Task.Run(() => pushApiService.ShutdownAsync()).Wait();
    }

    #endregion Messages

    #region Private Methods

    private static void onFavoriteClick(MouseButtonEventArgs args) {
      if (args.LeftButton == MouseButtonState.Pressed && args.Source is FrameworkElement fa && fa.DataContext is CurrencyViewEntity cve) {
        cve.IsFavorite = !cve.IsFavorite;
      }
    }

    private void onTickerEventCallback(PushMessageBase message) {
      if (!(message is TickerMessage ticker)) return;

      CurrencyViewEntity currency = viewModel.MarketTabs.SelectMany(market => market.Currencies).SingleOrDefault(c => c.Id == ticker.Id);

      if (currency != null) asyncService.RunUiContext(async () => {
        double oldChange = currency.Price;
        double newChange = ticker.Last;

        mapper.Map(message, currency);

        if (oldChange.EqualInPercentRange(newChange)) return;

        if (newChange < oldChange) currency.IsChangeDown = true;
        else if (newChange > oldChange) currency.IsChangeUp = true;

        await Task.Delay(TimeSpan.FromMilliseconds(500));

        if (newChange < oldChange) currency.IsChangeDown = false;
        else if (newChange > oldChange) currency.IsChangeUp = false;
      }).FireAndForget();
    }

    #endregion Private Methods

  }

}
