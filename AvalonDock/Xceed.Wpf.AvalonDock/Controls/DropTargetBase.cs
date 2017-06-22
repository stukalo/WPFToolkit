using System;
using System.Windows;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal abstract class DropTargetBase : DependencyObject
	{
		public readonly static DependencyProperty IsDraggingOverProperty;

		static DropTargetBase()
		{
			DropTargetBase.IsDraggingOverProperty = DependencyProperty.RegisterAttached("IsDraggingOver", typeof(bool), typeof(DropTargetBase), new FrameworkPropertyMetadata(false));
		}

		protected DropTargetBase()
		{
		}

		public static bool GetIsDraggingOver(DependencyObject d)
		{
			return (bool)d.GetValue(DropTargetBase.IsDraggingOverProperty);
		}

		public static void SetIsDraggingOver(DependencyObject d, bool value)
		{
			d.SetValue(DropTargetBase.IsDraggingOverProperty, value);
		}
	}
}