using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace PoloniexAutoTrader.Wpf.ViewModels {

  [Export]
  [ViewModel(nameof(MarketsViewModel))]
  public sealed class MarketsViewModel : ObservableObject {

    #region Private Fields

    private bool showFavorites;

    private string filter;

    private Func<bool> focusSearch = () => true;

    private ObservableCollection<MarketTabViewModel> marketTabs;

    #endregion Private Fields

    #region Properties

    public bool ShowFavorites {
      get => showFavorites;
      set { SetField(ref showFavorites, value, () => ShowFavorites); }
    }

    public string Filter {
      get => filter;
      set { SetField(ref filter, value, () => Filter); }
    }

    public Func<bool> FocusSearch {
      get => focusSearch;
      set { SetField(ref focusSearch, value, () => FocusSearch); }
    }

    public ObservableCollection<MarketTabViewModel> MarketTabs {
      get => marketTabs;
      set { SetField(ref marketTabs, value, () => MarketTabs); }
    }

    #endregion Properties

  }

}
