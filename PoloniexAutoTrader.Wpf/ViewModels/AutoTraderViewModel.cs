using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

using PoloniexAutoTrader.Wpf.ViewEntities;

using STR.MvvmCommon;


namespace PoloniexAutoTrader.Wpf.ViewModels {

  [Export]
  [ViewModel(nameof(AutoTraderViewModel))]
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  public sealed class AutoTraderViewModel : ObservableObject {

    #region Private Fields

    private RelayCommandAsync<RoutedEventArgs> loaded;

    private RelayCommand<CancelEventArgs> closing;

    private ApplicationSettingsViewEntity settings;

    #endregion Private Fields

    #region Properties

    public RelayCommandAsync<RoutedEventArgs> Loaded {
      get => loaded;
      set { SetField(ref loaded, value, () => Loaded); }
    }

    public RelayCommand<CancelEventArgs> Closing {
      get => closing;
      set { SetField(ref closing, value, () => Closing); }
    }

    public ApplicationSettingsViewEntity Settings {
      get => settings;
      set { SetField(ref settings, value, () => Settings); }
    }

    #endregion Properties

  }

}
