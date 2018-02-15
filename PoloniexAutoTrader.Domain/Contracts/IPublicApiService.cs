using System.Collections.Generic;
using System.Threading.Tasks;

using PoloniexAutoTrader.Domain.Models.PublicApi;


namespace PoloniexAutoTrader.Domain.Contracts {

  public interface IPublicApiService {

    Task<List<Ticker>> GetTickersAsync();

    Task<List<Currency>> GetCurrencies();

  }

}
