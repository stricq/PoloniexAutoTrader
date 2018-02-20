using System.Collections.Generic;


namespace PoloniexAutoTrader.Domain.Models {

  public sealed class UserSettings {

    public bool ShowFavorites { get; set; }

    public List<int> Favorites { get; set; }

    public string SelectedMarket { get; set; }

    public int SelectedCurrency { get; set; }

    public string ApiKey { get; set; }

    public string ApiSecret { get; set; }

  }

}
