using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;

using AutoMapper;

using PoloniexAutoTrader.Wpf.Messages.Status;
using PoloniexAutoTrader.Wpf.ViewModels;

using STR.MvvmCommon.Contracts;


namespace PoloniexAutoTrader.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class StatusBarController : IController {

    #region Private Fields

    private readonly IMessenger messenger;
    private readonly IMapper    mapper;

    private readonly DispatcherTimer timer;

    private readonly StatusBarViewModel viewModel;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public StatusBarController(StatusBarViewModel ViewModel, IMessenger Messenger, IMapper Mapper) {
      viewModel = ViewModel;

      messenger = Messenger;
      mapper    = Mapper;

      timer = new DispatcherTimer();
    }

    #endregion Constructor

    #region IController Implementation

    public async Task InitializeAsync() {
      timer.Tick    += onTimerTick;
      timer.Interval = TimeSpan.FromSeconds(1);

      timer.Start();

      registerMessages();

      await Task.CompletedTask;
    }

    public int InitializePriority { get; } = 500;

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
    }

    #endregion Messages

    #region Private Methods

    private async void onTimerTick(object sender, EventArgs args) {
      await messenger.SendAsync(new StatusTickMessage());

      using(Process process = Process.GetCurrentProcess()) {
        viewModel.Memory = process.WorkingSet64 / 1024.0 / 1024.0;
      }
    }

    #endregion Private Methods

  }

}
