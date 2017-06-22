using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class DropDownButton : ToggleButton
	{
		public readonly static DependencyProperty DropDownContextMenuProperty;

		public readonly static DependencyProperty DropDownContextMenuDataContextProperty;

		public System.Windows.Controls.ContextMenu DropDownContextMenu
		{
			get
			{
				return (System.Windows.Controls.ContextMenu)base.GetValue(DropDownButton.DropDownContextMenuProperty);
			}
			set
			{
				base.SetValue(DropDownButton.DropDownContextMenuProperty, value);
			}
		}

		public object DropDownContextMenuDataContext
		{
			get
			{
				return base.GetValue(DropDownButton.DropDownContextMenuDataContextProperty);
			}
			set
			{
				base.SetValue(DropDownButton.DropDownContextMenuDataContextProperty, value);
			}
		}

		static DropDownButton()
		{
			DropDownButton.DropDownContextMenuProperty = DependencyProperty.Register("DropDownContextMenu", typeof(System.Windows.Controls.ContextMenu), typeof(DropDownButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DropDownButton.OnDropDownContextMenuChanged)));
			DropDownButton.DropDownContextMenuDataContextProperty = DependencyProperty.Register("DropDownContextMenuDataContext", typeof(object), typeof(DropDownButton), new FrameworkPropertyMetadata(null));
		}

		public DropDownButton()
		{
			base.Unloaded += new RoutedEventHandler(this.DropDownButton_Unloaded);
		}

		private void DropDownButton_Unloaded(object sender, RoutedEventArgs e)
		{
			if (base.IsLoaded)
			{
				this.DropDownContextMenu = null;
			}
		}

		protected override void OnClick()
		{
			if (this.DropDownContextMenu != null)
			{
				this.DropDownContextMenu.PlacementTarget = this;
				this.DropDownContextMenu.Placement = PlacementMode.Bottom;
				this.DropDownContextMenu.DataContext = this.DropDownContextMenuDataContext;
				this.DropDownContextMenu.Closed += new RoutedEventHandler(this.OnContextMenuClosed);
				this.DropDownContextMenu.IsOpen = true;
			}
			base.OnClick();
		}

		private void OnContextMenuClosed(object sender, RoutedEventArgs e)
		{
			(sender as System.Windows.Controls.ContextMenu).Closed -= new RoutedEventHandler(this.OnContextMenuClosed);
			base.IsChecked = new bool?(false);
		}

		private static void OnDropDownContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DropDownButton)d).OnDropDownContextMenuChanged(e);
		}

		protected virtual void OnDropDownContextMenuChanged(DependencyPropertyChangedEventArgs e)
		{
			System.Windows.Controls.ContextMenu oldValue = e.OldValue as System.Windows.Controls.ContextMenu;
			if (oldValue != null && base.IsChecked.GetValueOrDefault())
			{
				oldValue.Closed -= new RoutedEventHandler(this.OnContextMenuClosed);
			}
		}
	}
}