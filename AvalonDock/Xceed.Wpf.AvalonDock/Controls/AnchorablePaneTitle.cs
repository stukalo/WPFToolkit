using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class AnchorablePaneTitle : Control
	{
		public readonly static DependencyProperty ModelProperty;

		private readonly static DependencyPropertyKey LayoutItemPropertyKey;

		public readonly static DependencyProperty LayoutItemProperty;

		private bool _isMouseDown;

		public Xceed.Wpf.AvalonDock.Controls.LayoutItem LayoutItem
		{
			get
			{
				return (Xceed.Wpf.AvalonDock.Controls.LayoutItem)base.GetValue(AnchorablePaneTitle.LayoutItemProperty);
			}
		}

		public LayoutAnchorable Model
		{
			get
			{
				return (LayoutAnchorable)base.GetValue(AnchorablePaneTitle.ModelProperty);
			}
			set
			{
				base.SetValue(AnchorablePaneTitle.ModelProperty, value);
			}
		}

		static AnchorablePaneTitle()
		{
			AnchorablePaneTitle.ModelProperty = DependencyProperty.Register("Model", typeof(LayoutAnchorable), typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(AnchorablePaneTitle._OnModelChanged)));
			AnchorablePaneTitle.LayoutItemPropertyKey = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(Xceed.Wpf.AvalonDock.Controls.LayoutItem), typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(null));
			AnchorablePaneTitle.LayoutItemProperty = AnchorablePaneTitle.LayoutItemPropertyKey.DependencyProperty;
			UIElement.IsHitTestVisibleProperty.OverrideMetadata(typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(true));
			UIElement.FocusableProperty.OverrideMetadata(typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(false));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(typeof(AnchorablePaneTitle)));
		}

		public AnchorablePaneTitle()
		{
		}

		private static void _OnModelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((AnchorablePaneTitle)sender).OnModelChanged(e);
		}

		private void OnHide()
		{
			this.Model.Hide(true);
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

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			if (this._isMouseDown && e.LeftButton == MouseButtonState.Pressed)
			{
				LayoutAnchorablePaneControl layoutAnchorablePaneControl = this.FindVisualAncestor<LayoutAnchorablePaneControl>();
				if (layoutAnchorablePaneControl != null)
				{
					LayoutAnchorablePane model = layoutAnchorablePaneControl.Model as LayoutAnchorablePane;
					model.Root.Manager.StartDraggingFloatingWindowForPane(model);
				}
			}
			this._isMouseDown = false;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			if (!e.Handled)
			{
				bool flag = false;
				LayoutAnchorableFloatingWindow layoutAnchorableFloatingWindow = this.Model.FindParent<LayoutAnchorableFloatingWindow>();
				if (layoutAnchorableFloatingWindow != null)
				{
					flag = layoutAnchorableFloatingWindow.Descendents().OfType<LayoutAnchorablePane>().Count<LayoutAnchorablePane>() == 1;
				}
				if (flag)
				{
					this.Model.Root.Manager.FloatingWindows.Single<LayoutFloatingWindowControl>((LayoutFloatingWindowControl fwc) => fwc.Model == layoutAnchorableFloatingWindow).AttachDrag(false);
					return;
				}
				this._isMouseDown = true;
			}
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			this._isMouseDown = false;
			base.OnMouseLeftButtonUp(e);
			if (this.Model != null)
			{
				this.Model.IsActive = true;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				this._isMouseDown = false;
			}
			base.OnMouseMove(e);
		}

		private void OnToggleAutoHide()
		{
			this.Model.ToggleAutoHide();
		}

		protected void SetLayoutItem(Xceed.Wpf.AvalonDock.Controls.LayoutItem value)
		{
			base.SetValue(AnchorablePaneTitle.LayoutItemPropertyKey, value);
		}
	}
}