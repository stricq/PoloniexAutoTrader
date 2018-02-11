using AutoMapper;


namespace PoloniexAutoTrader.Domain.Contracts {

  public interface IAutoMapperConfiguration {

    void RegisterMappings(IMapperConfigurationExpression config);

  }

}
