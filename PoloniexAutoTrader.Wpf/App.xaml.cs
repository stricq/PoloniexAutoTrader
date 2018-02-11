using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using AutoMapper;

using PoloniexAutoTrader.Domain.Contracts;

using STR.Common.Extensions;

using STR.MvvmCommon.Contracts;
using STR.MvvmCommon.Mef;


namespace PoloniexAutoTrader.Wpf {

  internal sealed partial class App : Application {

    #region Private Fields

    private readonly IMvvmContainer container;

    #endregion Private Fields

    #region Constructor

    public App() {
      InitializeComponent();

      container = new MvvmContainer();

      container.Initialize(() => new AggregateCatalog(new DirectoryCatalog(Directory.GetCurrentDirectory(), "PoloniexAutoTrader.exe"),
                                                      new DirectoryCatalog(Directory.GetCurrentDirectory(), "PoloniexAutoTrader.*.dll"),
                                                      new DirectoryCatalog(Directory.GetCurrentDirectory(), "Str.*.dll")));
    }

    #endregion Constructor

    #region Overrides

    protected override void OnStartup(StartupEventArgs e) {
      base.OnStartup(e);

      try {
        IEnumerable<IAutoMapperConfiguration> configurations = container.GetAll<IAutoMapperConfiguration>();

        MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg => configurations.ForEach(configuration => configuration.RegisterMappings(cfg)));

        try {
          mapperConfiguration.AssertConfigurationIsValid();
        }
        catch(Exception ex) {
          MessageBox.Show(ex.Message, "Mapping Validation Error");
        }

        container.RegisterInstance(mapperConfiguration.CreateMapper());

        IEnumerable<IController> controllers = container.GetAll<IController>();

        IOrderedEnumerable<IGrouping<int, IController>> groups = controllers.GroupBy(c => c.InitializePriority).OrderBy(g => g.Key);

        foreach(IGrouping<int, IController> group in groups) {
          Task.Run(() => group.ForEachAsync(controller => controller.InitializeAsync())).Wait();
        }
      }
      catch(Exception ex) {
        while(ex.InnerException != null) ex = ex.InnerException;

        MessageBox.Show(ex.Message, "MEF Error");
      }
    }

    #endregion Overrides

  }

}
