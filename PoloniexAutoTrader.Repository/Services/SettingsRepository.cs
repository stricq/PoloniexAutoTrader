using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

using PoloniexAutoTrader.Domain.Contracts;
using PoloniexAutoTrader.Domain.Models;

using STR.Common.Extensions;


namespace PoloniexAutoTrader.Repository.Services {

  [Export(typeof(IApplicationSettingsRepository))]
  [Export(typeof(IUserSettingsRepository))]
  public sealed class SettingsRepository : IApplicationSettingsRepository, IUserSettingsRepository {

    #region IApplicationSettingsRepository Implementation

    public async Task<ApplicationSettings> LoadApplicationSettingsAsync() {
      ApplicationSettings settings = await loadSettingsAsync<ApplicationSettings>(ApplicationSettingsFilename);

      if (settings.WindowW.EqualInPercentRange(0)) {
        settings.WindowW = 1024;
        settings.WindowH = 768;

        settings.WindowX = 100;
        settings.WindowY = 100;

        settings.SplitterDistance = 225;
      }

      return settings;
    }

    public async Task SaveApplicationSettingsAsync(ApplicationSettings Settings) {
      await saveSettingsAsync(Settings, ApplicationSettingsFilename);
    }

    #endregion IApplicationSettingsRepository Implementation

    #region IUserSettingsRepository Implementation

    public async Task<UserSettings> LoadUserSettingsAsync() {
      return await loadSettingsAsync<UserSettings>(UserSettingsFilename);
    }

    public async Task SaveUserSettingsAsync(UserSettings Settings) {
      await saveSettingsAsync(Settings, UserSettingsFilename);
    }

    #endregion IUserSettingsRepository Implementation

    #region Private Properties

    private static string ApplicationSettingsFilename => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"STR Programming Services\Poloniex Auto Trader\ApplicationSettings.json");

    private static string UserSettingsFilename => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"STR Programming Services\Poloniex Auto Trader\UserSettings.json");

    #endregion Private Properties

    #region Private Methods

    private static async Task<T> loadSettingsAsync<T>(string filename) where T : new() {
      T settings;

      if (await Task.Run(() => File.Exists(filename))) {
        settings = await Task.Run(() => JsonConvert.DeserializeObject<T>(File.ReadAllText(filename)));
      }
      else settings = new T();

      return settings;
    }

    private static async Task saveSettingsAsync<T>(T Settings, string filename) {
      string json = await Task.Run(() => JsonConvert.SerializeObject(Settings, Formatting.Indented));

      if (!await Task.Run(() => File.Exists(filename))) await Task.Run(() => Directory.CreateDirectory(Path.GetDirectoryName(filename)));

      await Task.Run(() => File.WriteAllText(filename, json));
    }

    #endregion Private Methods

  }

}
