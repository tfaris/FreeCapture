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
using System.Windows.Shapes;

namespace FreeCapture
{
    /// <summary>
    /// Interaction logic for CapturePreviewWindow.xaml
    /// </summary>
    public partial class CapturePreviewWindow : Window
    {
        string _capFilename;

        public string CaptureFilename
        {
            get { return _capFilename; }
            set { _capFilename = value; }
        }

        public CapturePreviewWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = CaptureFilename;
            if (!string.IsNullOrEmpty(CaptureFilename))
            {
                dlg.InitialDirectory = System.IO.Path.GetDirectoryName(CaptureFilename);
            }
            dlg.Filter = "Bitmap Image (.bmp)|*.bmp";
            this.DialogResult = dlg.ShowDialog();
            if (this.DialogResult.HasValue && this.DialogResult.Value)
            {
                CaptureFilename = dlg.FileName;
                Close();
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = null;
                Close();
            }
            else if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                button1_Click(button1, new RoutedEventArgs());
            }
        }
    }
}
