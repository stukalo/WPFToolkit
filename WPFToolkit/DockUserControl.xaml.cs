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
using Xceed.Wpf.AvalonDock.Layout;

namespace WPFToolkit
{
    public partial class DockUserControl : UserControl
    {
        public CanvasControl CurrentCanvs
        {
            get { return (CanvasControl)GetValue(CurrentCanvsProperty); }
            set { SetValue(CurrentCanvsProperty, value); }
        }

        public static readonly DependencyProperty CurrentCanvsProperty =
            DependencyProperty.Register("CurrentCanvs", typeof(CanvasControl), typeof(DockUserControl), new PropertyMetadata(null));



        public DockUserControl()
        {
            InitializeComponent();
            var newDoc = CreateDocument();
            documentPane.Children.Add(newDoc);
            paletteControl.ColorClicked += OnColorClicked;
            widthScrollControl.DragCompletedEvent += OnDragCompleted;
        }

        private void OnDragCompleted(double value)
        {
            value = value <= 1 ? 1 : value; 
            CurrentCanvs.Canvas.DefaultDrawingAttributes.Width = value;
            CurrentCanvs.Canvas.DefaultDrawingAttributes.Height = value;
        }

        private void OnColorClicked(SolidColorBrush brush)
        {
            CurrentCanvs.Canvas.DefaultDrawingAttributes.Color = brush.Color;
        }

        private void AddDocument(object sender, RoutedEventArgs e)
        {
            var newDoc = CreateDocument();
            documentPane.Children.Add(newDoc);
        }

        private LayoutDocument CreateDocument()
        {
            var newDocument = new LayoutDocument();
            newDocument.ContentId = "document" + Guid.NewGuid();
            var cnvs = new CanvasControl();
            newDocument.Content = cnvs;
            widthScrollControl.slider.Value = cnvs.Canvas.DefaultDrawingAttributes.Width;
            newDocument.Title = "Document " + documentPane.ChildrenCount;
            newDocument.IsActiveChanged += OnActiveChanged;
            return newDocument;
        }

        private void OnActiveChanged(object sender, EventArgs e)
        {
            var doc = sender as LayoutDocument;
            if (doc != null && doc.IsActive)
            {
                CurrentCanvs = doc.Content as CanvasControl;
                if (CurrentCanvs != null)
                    widthScrollControl.slider.Value = CurrentCanvs.Canvas.DefaultDrawingAttributes.Width;
                Debug.WriteLine(doc.Title);
            }
        }

        private void ClearCurrentCanvas(object sender, RoutedEventArgs e)
        {
            CurrentCanvs.Canvas.Strokes.Clear();
            Debug.WriteLine("ClearCurrentCanvas");
        }
    }
}
