using System.ComponentModel.Composition;
using System.Windows;

using AutoMapper;

using PoloniexAutoTrader.Domain.Contracts;
using PoloniexAutoTrader.Domain.Models;
using PoloniexAutoTrader.Domain.Models.PublicApi;

using PoloniexAutoTrader.Wpf.ViewEntities;


namespace PoloniexAutoTrader.Wpf.Mappings {

  [Export(typeof(IAutoMapperConfiguration))]
  public sealed class ViewEntityMappingConfiguration : IAutoMapperConfiguration {

    #region IAutoMapperConfiguration Implementation

    public void RegisterMappings(IMapperConfigurationExpression config) {

      config.CreateMap<ApplicationSettings, ApplicationSettingsViewEntity>().ForMember(dest => dest.AreSettingsChanged, opt => opt.Ignore())
                                                                            .ForMember(dest => dest.MainWindowState,    opt => opt.ResolveUsing(src => src.Maximized ? WindowState.Maximized : WindowState.Normal));

      config.CreateMap<ApplicationSettingsViewEntity, ApplicationSettings>().ForMember(dest => dest.Maximized, opt => opt.ResolveUsing(src => src.MainWindowState == WindowState.Maximized));

      config.CreateMap<Ticker, CurrencyViewEntity>().ForMember(dest => dest.Currency,      opt => opt.MapFrom(src => src.CurrencyPair.Currency))
                                                    .ForMember(dest => dest.Price,         opt => opt.MapFrom(src => src.Last))
                                                    .ForMember(dest => dest.Volume,        opt => opt.MapFrom(src => src.BaseVolume))
                                                    .ForMember(dest => dest.Change,        opt => opt.MapFrom(src => src.PercentChange))
                                                    .ForMember(dest => dest.Description,   opt => opt.MapFrom(src => src.Currency.Description))
                                                    .ForMember(dest => dest.IsSelected,    opt => opt.Ignore())
                                                    .ForMember(dest => dest.IsFavorite,    opt => opt.Ignore())
                                                    .ForMember(dest => dest.FavoriteClick, opt => opt.Ignore());
    }

    #endregion IAutoMapperConfiguration Implementation

  }

}
