using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

using STR.MvvmCommon;


namespace PoloniexAutoTrader.Wpf.ViewModels {

  [Export]
  [ViewModel(nameof(MainMenuViewModel))]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  public sealed class MainMenuViewModel : ObservableObject {

    #region Private Fields

    private RelayCommand exit;

    private RelayCommand about;

    #endregion Private Fields

    #region Properties

    public RelayCommand Exit {
      get => exit;
      set { SetField(ref exit, value, () => Exit); }
    }

    public RelayCommand About {
      get => about;
      set { SetField(ref about, value, () => About); }
    }

    #endregion Properties

  }

}
