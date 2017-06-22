using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for WidthScrollControl.xaml
    /// </summary>
    public partial class WidthScrollControl : UserControl
    {
        public WidthScrollControl()
        {
            InitializeComponent();
        }

        public delegate void DragCompletedHandler(double value);

        public event DragCompletedHandler DragCompletedEvent;

        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var value = ((Slider)sender).Value;
            DragCompletedEvent?.Invoke(value);
            Debug.WriteLine("Slider_DragCompleted");
        }

        public void SetSliderValue(double value)
        {
            slider.Value = value;
        }
    }
}
