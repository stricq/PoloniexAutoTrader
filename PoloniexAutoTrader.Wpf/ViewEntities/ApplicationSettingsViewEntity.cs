﻿using System.Diagnostics.CodeAnalysis;
using System.Windows;

using STR.MvvmCommon;


namespace PoloniexAutoTrader.Wpf.ViewEntities {

  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  public class ApplicationSettingsViewEntity : ObservableObject {

    #region Private Fields

    private bool areSettingsChanged;

    private double windowW;
    private double windowH;

    private double windowX;
    private double windowY;

    private WindowState mainWindowState;

    private double splitterDistance;

    #endregion Private Fields

    #region Properties

    public bool AreSettingsChanged {
      get => areSettingsChanged;
      set { SetField(ref areSettingsChanged, value, () => AreSettingsChanged); }
    }

    public double WindowW {
      get => windowW;
      set { AreSettingsChanged |= SetField(ref windowW, value, () => WindowW); }
    }

    public double WindowH {
      get => windowH;
      set { AreSettingsChanged |= SetField(ref windowH, value, () => WindowH); }
    }

    public double WindowX {
      get => windowX;
      set { AreSettingsChanged |= SetField(ref windowX, value, () => WindowX); }
    }

    public double WindowY {
      get => windowY;
      set { AreSettingsChanged |= SetField(ref windowY, value, () => WindowY); }
    }

    public WindowState MainWindowState {
      get => mainWindowState;
      set { AreSettingsChanged |= SetField(ref mainWindowState, value, () => MainWindowState); }
    }

    public double SplitterDistance {
      get => splitterDistance;
      set { AreSettingsChanged |= SetField(ref splitterDistance, value, () => SplitterDistance); }
    }

    #endregion Properties

  }

}
