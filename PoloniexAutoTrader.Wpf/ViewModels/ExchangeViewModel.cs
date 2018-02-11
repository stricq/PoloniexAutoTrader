using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace PoloniexAutoTrader.Wpf.ViewModels {

  [Export]
  [ViewModel(nameof(ExchangeViewModel))]
  public sealed class ExchangeViewModel : ObservableObject {

  }

}
