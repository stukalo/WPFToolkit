using System;
using System.Collections.Generic;
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
    /// Interaction logic for CanvasControl.xaml
    /// </summary>
    public partial class CanvasControl : UserControl
    {
        public CanvasControl()
        {
            InitializeComponent();
            Canvas = canvas;
        }
        
        public InkCanvas Canvas
        {
            get { return (InkCanvas)GetValue(CanvasProperty); }
            set { SetValue(CanvasProperty, value); }
        }
        
        public static readonly DependencyProperty CanvasProperty =
            DependencyProperty.Register("Cavas", typeof(InkCanvas), typeof(CanvasControl), new PropertyMetadata(null));


    }
}
