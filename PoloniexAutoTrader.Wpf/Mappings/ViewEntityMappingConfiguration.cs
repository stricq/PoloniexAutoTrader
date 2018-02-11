using System.ComponentModel.Composition;
using System.Windows;

using AutoMapper;

using PoloniexAutoTrader.Domain.Contracts;
using PoloniexAutoTrader.Domain.Models;

using PoloniexAutoTrader.Wpf.ViewEntities;


namespace PoloniexAutoTrader.Wpf.Mappings {

  [Export(typeof(IAutoMapperConfiguration))]
  public sealed class ViewEntityMappingConfiguration : IAutoMapperConfiguration {

    #region IAutoMapperConfiguration Implementation

    public void RegisterMappings(IMapperConfigurationExpression config) {

      config.CreateMap<ApplicationSettings, ApplicationSettingsViewEntity>().ForMember(dest => dest.AreSettingsChanged, opt => opt.Ignore())
                                                      .ForMember(dest => dest.MainWindowState,    opt => opt.ResolveUsing(src => src.Maximized ? WindowState.Maximized : WindowState.Normal));

      config.CreateMap<ApplicationSettingsViewEntity, ApplicationSettings>().ForMember(dest => dest.Maximized, opt => opt.ResolveUsing(src => src.MainWindowState == WindowState.Maximized));
    }

    #endregion IAutoMapperConfiguration Implementation

  }

}
