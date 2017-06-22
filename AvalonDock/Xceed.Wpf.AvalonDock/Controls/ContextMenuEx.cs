using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class ContextMenuEx : System.Windows.Controls.ContextMenu
	{
		static ContextMenuEx()
		{
		}

		public ContextMenuEx()
		{
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new MenuItemEx();
		}

		protected override void OnOpened(RoutedEventArgs e)
		{
			BindingOperations.GetBindingExpression(this, ItemsControl.ItemsSourceProperty).UpdateTarget();
			base.OnOpened(e);
		}
	}
}