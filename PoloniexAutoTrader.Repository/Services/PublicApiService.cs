using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using AutoMapper;

using PoloniexAutoTrader.Domain.Contracts;
using PoloniexAutoTrader.Domain.Models.PublicApi;

using PoloniexAutoTrader.Repository.Dtos.PublicApi;

using RestSharp;


namespace PoloniexAutoTrader.Repository.Services {

  [Export(typeof(IPublicApiService))]
  public sealed class PublicApiService : IPublicApiService {

    #region Private Fields

    private readonly IMapper mapper;

    private readonly RestClient client;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public PublicApiService(IMapper Mapper) {
      mapper = Mapper;

      client = new RestClient(ConfigurationManager.AppSettings["PublicApiUrl"]);
    }

    #endregion Constructor

    #region IPublicApiService Implementation

    public async Task<List<Ticker>> GetTickersAsync() {
      RestRequest request = new RestRequest(Method.GET);

      request.AddQueryParameter("command", "returnTicker");

      IRestResponse<Dictionary<string, TickerDto>> response = await client.ExecuteTaskAsync<Dictionary<string, TickerDto>>(request);

      if (response.StatusCode != HttpStatusCode.OK) throw new Exception($"Error from returnTicker: {response.StatusDescription}");

      return await Task.Run(() => mapper.Map<List<Ticker>>(response.Data.ToList()));
    }

    public async Task<List<Currency>> GetCurrencies() {
      RestRequest request = new RestRequest(Method.GET);

      request.AddQueryParameter("command", "returnCurrencies");

      IRestResponse<Dictionary<string, CurrencyDto>> response = await client.ExecuteTaskAsync<Dictionary<string, CurrencyDto>>(request);

      if (response.StatusCode != HttpStatusCode.OK) throw new Exception($"Error from returnCurrency: {response.StatusDescription}");

      return await Task.Run(() => mapper.Map<List<Currency>>(response.Data.ToList()));
    }

    #endregion IPublicApiService Implementation

  }

}
