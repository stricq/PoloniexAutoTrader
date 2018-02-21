using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using STR.Common.Extensions;


namespace PoloniexAutoTrader.Wpf.Behaviors {

  internal static class GridViewBehaviors {

    #region AutoResizeColumns Property

    public static readonly DependencyProperty AutoResizeColumnsProperty = DependencyProperty.RegisterAttached("AutoResizeColumns", typeof(bool), typeof(GridViewBehaviors), new FrameworkPropertyMetadata(false, onResizeColumnsPropertyChanged));

    public static bool GetAutoResizeColumns(DependencyObject obj) {
      return (bool)obj.GetValue(AutoResizeColumnsProperty);
    }

    public static void SetAutoResizeColumns(DependencyObject obj, bool value) {
      obj.SetValue(AutoResizeColumnsProperty, value);
    }

    private static void onResizeColumnsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
      if (sender is ListView view) handleListView(view, args);
    }

    private static void handleListView(ListView list, DependencyPropertyChangedEventArgs args) {
      list.Loaded += (o, eventArgs) => {
        ListView listView = o as ListView;

        int count = VisualTreeHelper.GetChildrenCount(listView);

        Decorator border = null;

        if (count > 0) border = VisualTreeHelper.GetChild(listView, 0) as Decorator;

        if (!(border?.Child is ScrollViewer scroller)) return;

        if ((bool)args.NewValue) scroller.ScrollChanged += onListViewScrollChanged;
        else scroller.ScrollChanged -= onListViewScrollChanged;
      };
    }

    private static void onListViewScrollChanged(object sender, ScrollChangedEventArgs e) {
      if (!e.ViewportHeightChange.EqualInPercentRange(0.0) || !e.VerticalChange.EqualInPercentRange(0.0) || !e.ExtentHeightChange.EqualInPercentRange(0.0)) {

        if (!(sender is ScrollViewer scroller)) return;

        if (!(VisualTreeHelper.GetParent(scroller) is Decorator border)) return;

        ListView listView = VisualTreeHelper.GetParent(border) as ListView;

        if (!(listView?.View is GridView view)) return;

        foreach(GridViewColumn column in view.Columns) {
          if (Double.IsNaN(column.Width)) column.Width = column.ActualWidth;

          column.Width = Double.NaN;
        }
      }
    }

    #endregion AutoResizeColumns Property

  }

}
