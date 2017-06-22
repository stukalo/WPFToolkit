using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutAnchorablePaneControl : TabControl, ILayoutControl
	{
		private LayoutAnchorablePane _model;

		public ILayoutElement Model
		{
			get
			{
				return this._model;
			}
		}

		static LayoutAnchorablePaneControl()
		{
			UIElement.FocusableProperty.OverrideMetadata(typeof(LayoutAnchorablePaneControl), new FrameworkPropertyMetadata(false));
		}

		public LayoutAnchorablePaneControl(LayoutAnchorablePane model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			this._model = model;
			base.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("Model.Children")
			{
				Source = this
			});
			base.SetBinding(FrameworkElement.FlowDirectionProperty, new Binding("Model.Root.Manager.FlowDirection")
			{
				Source = this
			});
			base.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
		}

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			this._model.SelectedContent.IsActive = true;
			base.OnGotKeyboardFocus(e);
		}

		private void OnLayoutUpdated(object sender, EventArgs e)
		{
			LayoutAnchorablePane actualWidth = this._model;
			((ILayoutPositionableElementWithActualSize)actualWidth).ActualWidth = base.ActualWidth;
			((ILayoutPositionableElementWithActualSize)actualWidth).ActualHeight = base.ActualHeight;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			if (!e.Handled && this._model.SelectedContent != null)
			{
				this._model.SelectedContent.IsActive = true;
			}
		}

		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseRightButtonDown(e);
			if (!e.Handled && this._model.SelectedContent != null)
			{
				this._model.SelectedContent.IsActive = true;
			}
		}
	}
}