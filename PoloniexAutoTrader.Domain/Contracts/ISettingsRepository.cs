using System.Threading.Tasks;

using PoloniexAutoTrader.Domain.Models;


namespace PoloniexAutoTrader.Domain.Contracts {

  public interface ISettingsRepository {

    Task<Settings> LoadSettingsAsync();

    Task SaveSettingsAsync(Settings Settings);

  }

}
