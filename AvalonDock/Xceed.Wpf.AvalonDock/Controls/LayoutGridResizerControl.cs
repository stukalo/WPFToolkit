using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutGridResizerControl : Thumb
	{
		public readonly static DependencyProperty BackgroundWhileDraggingProperty;

		public readonly static DependencyProperty OpacityWhileDraggingProperty;

		public Brush BackgroundWhileDragging
		{
			get
			{
				return (Brush)base.GetValue(LayoutGridResizerControl.BackgroundWhileDraggingProperty);
			}
			set
			{
				base.SetValue(LayoutGridResizerControl.BackgroundWhileDraggingProperty, value);
			}
		}

		public double OpacityWhileDragging
		{
			get
			{
				return (double)base.GetValue(LayoutGridResizerControl.OpacityWhileDraggingProperty);
			}
			set
			{
				base.SetValue(LayoutGridResizerControl.OpacityWhileDraggingProperty, value);
			}
		}

		static LayoutGridResizerControl()
		{
			LayoutGridResizerControl.BackgroundWhileDraggingProperty = DependencyProperty.Register("BackgroundWhileDragging", typeof(Brush), typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(Brushes.Black));
			LayoutGridResizerControl.OpacityWhileDraggingProperty = DependencyProperty.Register("OpacityWhileDragging", typeof(double), typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata((object)0.5));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(typeof(LayoutGridResizerControl)));
			FrameworkElement.HorizontalAlignmentProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata((object)System.Windows.HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
			FrameworkElement.VerticalAlignmentProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata((object)System.Windows.VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
			Control.BackgroundProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(Brushes.Transparent));
			UIElement.IsHitTestVisibleProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(true, null));
		}

		public LayoutGridResizerControl()
		{
		}
	}
}