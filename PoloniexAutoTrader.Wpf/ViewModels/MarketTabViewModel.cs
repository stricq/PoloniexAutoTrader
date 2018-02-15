using System.Collections.ObjectModel;

using PoloniexAutoTrader.Wpf.ViewEntities;

using STR.MvvmCommon;


namespace PoloniexAutoTrader.Wpf.ViewModels {

  public sealed class MarketTabViewModel : ObservableObject {

    #region Private Fields

    private bool isSelected;

    private string name;

    private ObservableCollection<CurrencyViewEntity> currencies;

    #endregion Private Fields

    #region Properties

    public bool IsSelected {
      get => isSelected;
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public string Name {
      get => name;
      set { SetField(ref name, value, () => Name); }
    }

    public ObservableCollection<CurrencyViewEntity> Currencies {
      get => currencies;
      set { SetField(ref currencies, value, () => Currencies); }
    }

    #endregion Properties

  }

}
