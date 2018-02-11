using PoloniexAutoTrader.Domain.Models;

using STR.Common.Messages;


namespace PoloniexAutoTrader.Wpf.Messages.Application {

  internal sealed class AppLoadedMessage : ApplicationLoadedMessage {

    public Settings Settings { get; set; }

  }

}
