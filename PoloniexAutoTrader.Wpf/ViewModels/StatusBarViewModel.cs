using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

using STR.MvvmCommon;


namespace PoloniexAutoTrader.Wpf.ViewModels {

  [Export]
  [ViewModel(nameof(StatusBarViewModel))]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  public sealed class StatusBarViewModel : ObservableObject {

    #region Private Fields

    private double memory;

    #endregion Private Fields

    #region Properties

    public double Memory {
      get => memory;
      set { SetField(ref memory, value, () => Memory); }
    }

    #endregion Properties

  }

}
