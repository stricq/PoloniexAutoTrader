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

    private bool isChangeUp;

    private bool isChangeDown;

    private string currency;

    private double price;

    private double volume;

    private double change;

    private string description;

    private RelayCommand<MouseButtonEventArgs> favoriteClick;

    #endregion Private Fields

    #region Properties

    public int Id { get; set; }

    public RelayCommand<MouseButtonEventArgs> FavoriteClick {
      get => favoriteClick;
      set { SetField(ref favoriteClick, value, () => FavoriteClick); }
    }

    public bool IsSelected {
      get => isSelected;
      set { SetField(ref isSelected, value, () => IsSelected, () => IconBackground); }
    }

    public bool IsFavorite {
      get => isFavorite;
      set { SetField(ref isFavorite, value, () => IsFavorite, () => Icon); }
    }

    public bool IsChangeUp {
      get => isChangeUp;
      set { SetField(ref isChangeUp, value, () => IsChangeUp); }
    }

    public bool IsChangeDown {
      get => isChangeDown;
      set { SetField(ref isChangeDown, value, () => IsChangeDown); }
    }

    public FontAwesomeIcon Icon => IsFavorite ? FontAwesomeIcon.Star : FontAwesomeIcon.StarOutline;

    public SolidColorBrush IconBackground => new SolidColorBrush(IsSelected ? Color.FromRgb(0xa3, 0x75, 0x14) : Color.FromRgb(0x6f, 0x93, 0x97));

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
