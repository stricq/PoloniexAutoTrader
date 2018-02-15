using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using System.Windows.Media;

using FontAwesome.WPF;

using STR.MvvmCommon;


namespace PoloniexAutoTrader.Wpf.ViewEntities {

  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  public class CurrencyViewEntity : ObservableObject {

    #region Private Fields

    private bool isSelected;

    private bool isFavorite;

    private string currency;

    private double price;

    private double volume;

    private double change;

    private string description;

    private RelayCommand<MouseButtonEventArgs> favoriteClick;

    #endregion Private Fields

    #region Properties

    public RelayCommand<MouseButtonEventArgs> FavoriteClick {
      get => favoriteClick;
      set { SetField(ref favoriteClick, value, () => FavoriteClick); }
    }

    public bool IsSelected {
      get => isSelected;
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public bool IsFavorite {
      get => isFavorite;
      set { SetField(ref isFavorite, value, () => IsFavorite, () => Icon); }
    }

    public FontAwesomeIcon Icon => IsFavorite ? FontAwesomeIcon.Star : FontAwesomeIcon.StarOutline;

    public string Currency {
      get => currency;
      set { SetField(ref currency, value, () => Currency); }
    }

    public double Price {
      get => price;
      set { SetField(ref price, value, () => Price); }
    }

    public double Volume {
      get => volume;
      set { SetField(ref volume, value, () => Volume); }
    }

    public double Change {
      get => change;
      set { SetField(ref change, value, () => Change, () => ChangeColor); }
    }

    public SolidColorBrush ChangeColor => new SolidColorBrush(Change >= 0 ? Color.FromRgb(0x1d, 0x74, 0x24) : Color.FromRgb(0xb7, 0x22, 0x19));

    public string Description {
      get => description;
      set { SetField(ref description, value, () => Description); }
    }

    #endregion Properties

  }

}
