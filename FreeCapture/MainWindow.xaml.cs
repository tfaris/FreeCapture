using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FreeCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CaptureManager _capMgr;
        string _lastSaveDir;

        public MainWindow()
        {
            InitializeComponent();
            _capMgr = new CaptureManager(captureSurface1);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _capMgr.BeginCapture();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter || e.Key == Key.F10 || e.Key == Key.System)
            {
                Visibility = System.Windows.Visibility.Hidden;
                // Use a timer so that the window will (hopefully) 
                // be hidden by the time we capture.
                System.Timers.Timer timer =
                    new System.Timers.Timer(50);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
                timer.Start();
            }
            else if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                // Clear current capture; start over
                _capMgr.BeginCapture();
                captureSurface1.InvalidateVisual();
            }
            else if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ((System.Timers.Timer)sender).Stop();
            Dispatcher.Invoke((Action)delegate
            {
                string filename =
                System.IO.Path.Combine(
                    _lastSaveDir ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    System.IO.Path.GetRandomFileName() + ".bmp");
                try
                {
                    using (System.Drawing.Image image = _capMgr.CreateImage())
                    {
                        CapturePreviewWindow prev = new CapturePreviewWindow();
                        prev.CaptureFilename = filename;
                        // Create a WPF-compatibale image source from a System.Drawing.Bitmap
                        BitmapSource source =
                        System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(((System.Drawing.Bitmap)image).GetHbitmap(),
                                                                                      IntPtr.Zero,
                                                                                      Int32Rect.Empty,
                                                                                      BitmapSizeOptions.FromEmptyOptions());                        
                        prev.image1.Source = source;
                        bool? dr = prev.ShowDialog();
                        if (dr.HasValue && dr.Value)
                        {
                            filename = prev.CaptureFilename;
                            _lastSaveDir = System.IO.Path.GetDirectoryName(filename);
                            image.Save(filename);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    Visibility = System.Windows.Visibility.Visible;
                    Focus();
                }
            });
        }
    }
}
