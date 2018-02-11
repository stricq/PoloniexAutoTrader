

using System.Threading.Tasks;

using PoloniexAutoTrader.Domain.Models;


namespace PoloniexAutoTrader.Domain.Contracts {

  public interface IUserSettingsRepository {

    Task<UserSettings> LoadUserSettingsAsync();

    Task SaveUserSettingsAsync(UserSettings Settings);

  }

}
