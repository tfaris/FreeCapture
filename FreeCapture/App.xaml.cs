using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace FreeCapture
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mw = new MainWindow();
            mw.Show();
            // Strecth the window across all screens
            ScreenInfo si = ScreenInfo.AllScreenInfo;
            mw.Left = si.MinX;
            mw.Top = si.MinY;
            mw.Width = si.TotalWidth;
            mw.Height = si.TotalHeight;
        }
    }
}
