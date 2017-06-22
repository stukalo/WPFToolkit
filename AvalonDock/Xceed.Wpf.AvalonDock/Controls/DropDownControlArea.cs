using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class DropDownControlArea : UserControl
	{
		public readonly static DependencyProperty DropDownContextMenuProperty;

		public readonly static DependencyProperty DropDownContextMenuDataContextProperty;

		public System.Windows.Controls.ContextMenu DropDownContextMenu
		{
			get
			{
				return (System.Windows.Controls.ContextMenu)base.GetValue(DropDownControlArea.DropDownContextMenuProperty);
			}
			set
			{
				base.SetValue(DropDownControlArea.DropDownContextMenuProperty, value);
			}
		}

		public object DropDownContextMenuDataContext
		{
			get
			{
				return base.GetValue(DropDownControlArea.DropDownContextMenuDataContextProperty);
			}
			set
			{
				base.SetValue(DropDownControlArea.DropDownContextMenuDataContextProperty, value);
			}
		}

		static DropDownControlArea()
		{
			DropDownControlArea.DropDownContextMenuProperty = DependencyProperty.Register("DropDownContextMenu", typeof(System.Windows.Controls.ContextMenu), typeof(DropDownControlArea), new FrameworkPropertyMetadata(null));
			DropDownControlArea.DropDownContextMenuDataContextProperty = DependencyProperty.Register("DropDownContextMenuDataContext", typeof(object), typeof(DropDownControlArea), new FrameworkPropertyMetadata(null));
		}

		public DropDownControlArea()
		{
		}

		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseRightButtonDown(e);
		}

		protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseRightButtonUp(e);
			if (!e.Handled && this.DropDownContextMenu != null)
			{
				this.DropDownContextMenu.PlacementTarget = null;
				this.DropDownContextMenu.Placement = PlacementMode.MousePoint;
				this.DropDownContextMenu.DataContext = this.DropDownContextMenuDataContext;
				this.DropDownContextMenu.IsOpen = true;
			}
		}
	}
}