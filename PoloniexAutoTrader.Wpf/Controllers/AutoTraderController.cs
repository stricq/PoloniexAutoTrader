using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using AutoMapper;

using PoloniexAutoTrader.Domain.Contracts;
using PoloniexAutoTrader.Domain.Models;

using PoloniexAutoTrader.Wpf.Messages.Application;
using PoloniexAutoTrader.Wpf.Messages.Status;
using PoloniexAutoTrader.Wpf.ViewEntities;
using PoloniexAutoTrader.Wpf.ViewModels;

using STR.Common.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;


namespace PoloniexAutoTrader.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class AutoTraderController : IController {

    #region Private Fields

    private bool isStartupComplete;

    private DateTime lastSave = DateTime.Now;

    private Settings settings;

    private readonly AutoTraderViewModel     viewModel;
    private readonly   MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;
    private readonly IMapper    mapper;

    private readonly ISettingsRepository settingsRepository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public AutoTraderController(AutoTraderViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IMapper Mapper, ISettingsRepository SettingsRepository) {
      if (Application.Current != null) Application.Current.DispatcherUnhandledException += onCurrentDispatcherUnhandledException;

      AppDomain.CurrentDomain.UnhandledException += onDomainUnhandledException;

      Dispatcher.CurrentDispatcher.UnhandledException += onCurrentDispatcherUnhandledException;

      TaskScheduler.UnobservedTaskException += onUnobservedTaskException;

      System.Windows.Forms.Application.ThreadException += onThreadException;

          viewModel =     ViewModel;
      menuViewModel = MenuViewModel;

      messenger = Messenger;
      mapper    = Mapper;

      settingsRepository = SettingsRepository;
    }

    #endregion Constructor

    #region IController Implementation

    public async Task InitializeAsync() {
      settings = await settingsRepository.LoadSettingsAsync();

      viewModel.Settings = mapper.Map<SettingsViewEntity>(settings);

      registerMessages();
      registerCommands();
    }

    public int InitializePriority { get; } = 1000;

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
      messenger.RegisterAsync<StatusTickMessage>(this, onStatusTick);
    }

    private async Task onStatusTick(StatusTickMessage message) {
      if ((DateTime.Now - lastSave).Seconds > 3 && viewModel.Settings.AreSettingsChanged) {
        mapper.Map(viewModel.Settings, settings);

        await settingsRepository.SaveSettingsAsync(settings);

        viewModel.Settings.AreSettingsChanged = false;

        lastSave = DateTime.Now;
      }
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      viewModel.Loaded = new RelayCommandAsync<RoutedEventArgs>(onLoadedExecuteAsync);

      viewModel.Closing = new RelayCommand<CancelEventArgs>(onClosingExecute);

      menuViewModel.Exit = new RelayCommand(onExitExecute);
    }

    private async Task onLoadedExecuteAsync(RoutedEventArgs args) {
      isStartupComplete = true;

      await messenger.SendAsync(new AppLoadedMessage { Settings = settings });
    }

    private void onClosingExecute(CancelEventArgs args) {
      ApplicationClosingMessage message = new ApplicationClosingMessage();

      messenger.Send(message);

      args.Cancel = message.Cancel;

      if (!args.Cancel && viewModel.Settings.AreSettingsChanged) {
        mapper.Map(viewModel.Settings, settings);

        Task.Run(() => settingsRepository.SaveSettingsAsync(settings)).Wait();
      }
    }

    private void onExitExecute() {
      ApplicationClosingMessage message = new ApplicationClosingMessage();

      messenger.Send(message);

      if (!message.Cancel) Application.Current.Shutdown();
    }

    #endregion Commands

    #region Private Methods

    private void onDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
      if (e.ExceptionObject is Exception ex) {
        if (e.IsTerminating) MessageBox.Show(ex.Message, "Fatal Domain Unhandled Exception");
        else messenger.SendUi(new ApplicationErrorMessage { ErrorMessage = "Domain Unhandled Exception", Exception = ex });
      }
    }

    private void onCurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
      if (e.Exception != null) {
        if (isStartupComplete) {
          messenger.SendUi(new ApplicationErrorMessage { ErrorMessage = "Dispatcher Unhandled Exception", Exception = e.Exception });

          e.Handled = true;
        }
        else {
          e.Handled = true;

          MessageBox.Show(e.Exception.Message, "Fatal Dispatcher Exception");

          Application.Current.Shutdown();
        }
      }
    }

    private void onUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
      if (e.Exception == null || e.Exception.InnerExceptions.Count == 0) return;

      foreach(Exception ex in e.Exception.InnerExceptions) {
        if (isStartupComplete) {
          messenger.SendUi(new ApplicationErrorMessage { ErrorMessage = "Unobserved Task Exception", Exception = ex });
        }
        else {
          MessageBox.Show(ex.Message, "Fatal Unobserved Task Exception");
        }
      }

      if (!isStartupComplete) Application.Current.Shutdown();

      e.SetObserved();
    }

    private void onThreadException(object sender, ThreadExceptionEventArgs e) {
      if (e.Exception == null) return;

      messenger.SendUi(new ApplicationErrorMessage { ErrorMessage = "Thread Exception", Exception = e.Exception });
    }

    #endregion Private Methods

  }

}
