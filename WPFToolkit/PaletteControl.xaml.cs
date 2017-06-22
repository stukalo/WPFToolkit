using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFToolkit
{
    /// <summary>
    /// Interaction logic for PaletteControl.xaml
    /// </summary>
    public partial class PaletteControl : UserControl
    {
        public PaletteControl()
        {
            InitializeComponent();
        }

        public delegate void ColorClickHandler(SolidColorBrush color);

        public event ColorClickHandler ColorClicked;

        private void ColorBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            SolidColorBrush brush = (SolidColorBrush)btn.Background;
            Debug.WriteLine("ColorBtn_Click");
            ColorClicked?.Invoke(brush);
        }
    }
}
