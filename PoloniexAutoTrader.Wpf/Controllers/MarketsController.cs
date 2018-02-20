using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using AutoMapper;

using PoloniexAutoTrader.Domain.Contracts;
using PoloniexAutoTrader.Domain.Models;
using PoloniexAutoTrader.Domain.Models.PublicApi;
using PoloniexAutoTrader.Domain.Models.PushApi;
using PoloniexAutoTrader.Wpf.Messages.Application;
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

    private bool areSettingsChanged;

    private DateTime lastSettingsSave;

    private DateTime lastHeartbeat;

    private List<Ticker> tickers;

    private List<Currency> currencies;

    private UserSettings settings;

    private readonly MarketsViewModel viewModel;

    private readonly IAsyncService asyncService;

    private readonly IMessenger messenger;

    private readonly IMapper mapper;

    private readonly IPublicApiService publicApiService;

    private readonly IPushApiService pushApiService;

    private readonly IUserSettingsRepository settingsRepository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public MarketsController(MarketsViewModel ViewModel, IAsyncService AsyncService, IMessenger Messenger, IMapper Mapper, IPublicApiService PublicApiService, IPushApiService PushApiService, IUserSettingsRepository SettingsRepository) {
      viewModel = ViewModel;

      asyncService = AsyncService;
      messenger    = Messenger;

      mapper = Mapper;

      publicApiService = PublicApiService;

      pushApiService = PushApiService;

      settingsRepository = SettingsRepository;
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

      markets.ForEach(market => market.PropertyChanged += onMarketTabViewModelPropertyChanged);

      markets.SelectMany(market => market.Currencies).ForEach(currency => {
        currency.FavoriteClick = new RelayCommand<MouseButtonEventArgs>(onFavoriteClick);

        currency.PropertyChanged += onCurrencyViewEntityPropertyChanged;
      });

      viewModel.MarketTabs = new ObservableCollection<MarketTabViewModel>(markets);

      await pushApiService.SubscribeToTickerEvents(onTickerEventCallback);

      lastHeartbeat = DateTime.Now;

      viewModel.PropertyChanged += onMarketsViewModelPropertyChanged;

      registerMessages();
    }

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
      messenger.Register<AppLoadedMessage>(this, onAppLoaded);

      messenger.RegisterAsync<StatusTickMessage>(this, onStatusTickAsync);

      messenger.Register<MarketSearchMessage>(this, onMarketSearch);

      messenger.Register<ApplicationClosingMessage>(this, onApplicationClosing);
    }

    private void onAppLoaded(AppLoadedMessage message) {
      settings = message.Settings;

      MarketTabViewModel market = viewModel.MarketTabs.SingleOrDefault(m => m.Name == settings.SelectedMarket);

      if (market != null) market.IsSelected = true;
      else viewModel.MarketTabs[0].IsSelected = true;

      List<CurrencyViewEntity> currencyEntities = viewModel.MarketTabs.SelectMany(m => m.Currencies).ToList();

      CurrencyViewEntity cve = currencyEntities.SingleOrDefault(currency => currency.Id == settings.SelectedCurrency);

      if (cve != null) cve.IsSelected = true;
      else {
        if (viewModel.ShowFavorites) cve = currencyEntities.FirstOrDefault(currency => currency.IsFavorite);

        if (cve == null) viewModel.MarketTabs[0].Currencies[0].IsSelected = true;
      }

      currencyEntities.ForEach(currency => currency.IsFavorite = settings.Favorites.Contains(currency.Id));

      viewModel.ShowFavorites = settings.ShowFavorites;
    }

    private async Task onStatusTickAsync(StatusTickMessage message) {
      if ((DateTime.Now - lastSettingsSave).TotalSeconds >= 3 && areSettingsChanged) {
        await settingsRepository.SaveUserSettingsAsync(settings);

        areSettingsChanged = false;

        lastSettingsSave = DateTime.Now;
      }

      if ((DateTime.Now - lastHeartbeat).TotalSeconds >= 60) {
        await pushApiService.SendHeartbeat();

        lastHeartbeat = DateTime.Now;
      }
    }

    private void onMarketSearch(MarketSearchMessage message) {
      viewModel.FocusSearch();
    }

    private void onApplicationClosing(ApplicationClosingMessage message) {
      Task.Run(() => pushApiService.ShutdownAsync()).Wait();
    }

    #endregion Messages

    #region Private Methods

    private void onMarketsViewModelPropertyChanged(object sender, PropertyChangedEventArgs args) {
      List<CurrencyViewEntity> currencyEntities = viewModel.MarketTabs.SelectMany(market => market.Currencies).ToList();

      switch(args.PropertyName) {
        case "ShowFavorites": {
          if (viewModel.ShowFavorites) {
            viewModel.Filter = String.Empty;

            currencyEntities.ForEach(currency => currency.IsVisible = currency.IsFavorite);
          }
          else currencyEntities.ForEach(currency => currency.IsVisible = true);

          settings.ShowFavorites = viewModel.ShowFavorites;

          areSettingsChanged = true;

          break;
        }
        case "Filter": {
          if (!String.IsNullOrEmpty(viewModel.Filter)) {
            viewModel.ShowFavorites = false;

            currencyEntities.ForEach(currency => currency.IsVisible = currency.Currency.ToLowerInvariant().Contains(viewModel.Filter.ToLowerInvariant())
                                                                   || currency.Description.ToLowerInvariant().Contains(viewModel.Filter.ToLowerInvariant()));
          }
          else currencyEntities.ForEach(currency => currency.IsVisible = true);

          break;
        }
      }
    }

    private void onMarketTabViewModelPropertyChanged(object sender, PropertyChangedEventArgs args) {
      if (!(sender is MarketTabViewModel market) || settings == null) return;

      switch(args.PropertyName) {
        case "IsSelected": {
          if (market.IsSelected && settings.SelectedMarket != market.Name) {
            settings.SelectedMarket = market.Name;

            areSettingsChanged = true;
          }

          break;
        }
      }
    }

    private void onCurrencyViewEntityPropertyChanged(object sender, PropertyChangedEventArgs args) {
      if (!(sender is CurrencyViewEntity cve)) return;

      switch(args.PropertyName) {
        case "IsSelected": {
          if (cve.IsSelected && settings.SelectedCurrency != cve.Id) {
            settings.SelectedCurrency = cve.Id;

            areSettingsChanged = true;
          }

          break;
        }
      }
    }

    private void onFavoriteClick(MouseButtonEventArgs args) {
      if (args.LeftButton == MouseButtonState.Pressed && args.Source is FrameworkElement fa && fa.DataContext is CurrencyViewEntity cve) {
        cve.IsFavorite = !cve.IsFavorite;

        if (settings.Favorites == null) settings.Favorites = new List<int>();

        if (cve.IsFavorite) settings.Favorites.Add(cve.Id);
        else settings.Favorites.Remove(cve.Id);

        areSettingsChanged = true;

        args.Handled = true;
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
