using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace PoloniexAutoTrader.Wpf.ViewModels {

  [Export]
  [ViewModel(nameof(MarketsViewModel))]
  public sealed class MarketsViewModel : ObservableObject {

    #region Private Fields

    private ObservableCollection<MarketTabViewModel> marketTabs;

    #endregion Private Fields

    #region Properties

    public ObservableCollection<MarketTabViewModel> MarketTabs {
      get => marketTabs;
      set { SetField(ref marketTabs, value, () => MarketTabs); }
    }

    #endregion Properties

  }

}
