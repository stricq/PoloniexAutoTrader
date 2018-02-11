using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

using PoloniexAutoTrader.Domain.Contracts;
using PoloniexAutoTrader.Domain.Models;

using STR.Common.Extensions;


namespace PoloniexAutoTrader.Repository.Services {

  [Export(typeof(ISettingsRepository))]
  public sealed class SettingsRepository : ISettingsRepository {

    #region ISettingsRepository Implementation

    public async Task<Settings> LoadSettingsAsync() {
      Settings settings;

      if (await Task.Run(() => File.Exists(Filename))) {
        settings = await Task.Run(() => JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Filename)));

        if (settings.WindowW.EqualInPercentRange(0)) {
          settings.WindowW = 1024;
          settings.WindowH = 768;

          settings.WindowX = 100;
          settings.WindowY = 100;
        }
      }
      else settings = new Settings {
        WindowW = 1024,
        WindowH = 768,

        WindowX = 100,
        WindowY = 100
      };

      return settings;
    }

    public async Task SaveSettingsAsync(Settings Settings) {
      string json = await Task.Run(() => JsonConvert.SerializeObject(Settings, Formatting.Indented));

      if (!await Task.Run(() => File.Exists(Filename))) await Task.Run(() => Directory.CreateDirectory(Path.GetDirectoryName(Filename)));

      await Task.Run(() => File.WriteAllText(Filename, json));
    }

    #endregion ISettingsRepository Implementation

    #region Private Properties

    private static string Filename => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"STR Programming Services\Poloniex Auto Trader\Settings.json");

    #endregion Private Properties

  }

}
