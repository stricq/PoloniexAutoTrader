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

using PoloniexAutoTrader.Wpf.ViewEntities;
using PoloniexAutoTrader.Wpf.ViewModels;

using STR.Common.Extensions;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;


namespace PoloniexAutoTrader.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class MarketsController : IController {

    #region Private Fields

    private List<Ticker> tickers;

    private List<Currency> currencies;

    private readonly MarketsViewModel viewModel;

    private readonly IMapper mapper;

    private readonly IPublicApiService publicApiService;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public MarketsController(MarketsViewModel ViewModel, IMapper Mapper, IPublicApiService PublicApiService) {
      viewModel = ViewModel;

      mapper = Mapper;

      publicApiService = PublicApiService;
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
    }

    #endregion IController Implementation

    #region Private Methods

    private static void onFavoriteClick(MouseButtonEventArgs args) {
      if (args.LeftButton == MouseButtonState.Pressed && args.Source is FrameworkElement fa && fa.DataContext is CurrencyViewEntity cve) {
        cve.IsFavorite = !cve.IsFavorite;
      }
    }

    #endregion Private Methods

  }

}
