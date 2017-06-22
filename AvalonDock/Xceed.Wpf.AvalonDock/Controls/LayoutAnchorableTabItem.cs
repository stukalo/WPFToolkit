using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutAnchorableTabItem : Control
	{
		public readonly static DependencyProperty ModelProperty;

		private readonly static DependencyPropertyKey LayoutItemPropertyKey;

		public readonly static DependencyProperty LayoutItemProperty;

		private bool _isMouseDown;

		private static LayoutAnchorableTabItem _draggingItem;

		public Xceed.Wpf.AvalonDock.Controls.LayoutItem LayoutItem
		{
			get
			{
				return (Xceed.Wpf.AvalonDock.Controls.LayoutItem)base.GetValue(LayoutAnchorableTabItem.LayoutItemProperty);
			}
		}

		public LayoutContent Model
		{
			get
			{
				return (LayoutContent)base.GetValue(LayoutAnchorableTabItem.ModelProperty);
			}
			set
			{
				base.SetValue(LayoutAnchorableTabItem.ModelProperty, value);
			}
		}

		static LayoutAnchorableTabItem()
		{
			LayoutAnchorableTabItem.ModelProperty = DependencyProperty.Register("Model", typeof(LayoutContent), typeof(LayoutAnchorableTabItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutAnchorableTabItem.OnModelChanged)));
			LayoutAnchorableTabItem.LayoutItemPropertyKey = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(Xceed.Wpf.AvalonDock.Controls.LayoutItem), typeof(LayoutAnchorableTabItem), new FrameworkPropertyMetadata(null));
			LayoutAnchorableTabItem.LayoutItemProperty = LayoutAnchorableTabItem.LayoutItemPropertyKey.DependencyProperty;
			LayoutAnchorableTabItem._draggingItem = null;
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorableTabItem), new FrameworkPropertyMetadata(typeof(LayoutAnchorableTabItem)));
		}

		public LayoutAnchorableTabItem()
		{
		}

		internal static LayoutAnchorableTabItem GetDraggingItem()
		{
			return LayoutAnchorableTabItem._draggingItem;
		}

		internal static bool IsDraggingItem()
		{
			return LayoutAnchorableTabItem._draggingItem != null;
		}

		private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutAnchorableTabItem)d).OnModelChanged(e);
		}

		protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.Model == null)
			{
				this.SetLayoutItem(null);
				return;
			}
			this.SetLayoutItem(this.Model.Root.Manager.GetLayoutItemFromModel(this.Model));
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			if (LayoutAnchorableTabItem._draggingItem != null && LayoutAnchorableTabItem._draggingItem != this && e.LeftButton == MouseButtonState.Pressed)
			{
				LayoutContent model = this.Model;
				ILayoutContainer parent = model.Parent;
				ILayoutPane layoutPane = model.Parent as ILayoutPane;
				if (layoutPane is LayoutAnchorablePane && !((LayoutAnchorablePane)layoutPane).CanRepositionItems)
				{
					return;
				}
				if (layoutPane.Parent != null && layoutPane.Parent is LayoutAnchorablePaneGroup && !((LayoutAnchorablePaneGroup)layoutPane.Parent).CanRepositionItems)
				{
					return;
				}
				List<ILayoutElement> list = parent.Children.ToList<ILayoutElement>();
				layoutPane.MoveChild(list.IndexOf(LayoutAnchorableTabItem._draggingItem.Model), list.IndexOf(model));
			}
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			if (this._isMouseDown && e.LeftButton == MouseButtonState.Pressed)
			{
				LayoutAnchorableTabItem._draggingItem = this;
			}
			this._isMouseDown = false;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			this._isMouseDown = true;
			LayoutAnchorableTabItem._draggingItem = this;
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			this._isMouseDown = false;
			base.OnMouseLeftButtonUp(e);
			this.Model.IsActive = true;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				this._isMouseDown = false;
				LayoutAnchorableTabItem._draggingItem = null;
			}
		}

		protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnPreviewGotKeyboardFocus(e);
		}

		internal static void ResetDraggingItem()
		{
			LayoutAnchorableTabItem._draggingItem = null;
		}

		protected void SetLayoutItem(Xceed.Wpf.AvalonDock.Controls.LayoutItem value)
		{
			base.SetValue(LayoutAnchorableTabItem.LayoutItemPropertyKey, value);
		}
	}
}