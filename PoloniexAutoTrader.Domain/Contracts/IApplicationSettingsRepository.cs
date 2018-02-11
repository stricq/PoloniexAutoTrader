using System.Threading.Tasks;

using PoloniexAutoTrader.Domain.Models;


namespace PoloniexAutoTrader.Domain.Contracts {

  public interface IApplicationSettingsRepository {

    Task<ApplicationSettings> LoadApplicationSettingsAsync();

    Task SaveApplicationSettingsAsync(ApplicationSettings Settings);

  }

}
