using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutDocumentPaneControl : TabControl, ILayoutControl
	{
		private List<object> _logicalChildren = new List<object>();

		private LayoutDocumentPane _model;

		protected override IEnumerator LogicalChildren
		{
			get
			{
				return this._logicalChildren.GetEnumerator();
			}
		}

		public ILayoutElement Model
		{
			get
			{
				return this._model;
			}
		}

		static LayoutDocumentPaneControl()
		{
			UIElement.FocusableProperty.OverrideMetadata(typeof(LayoutDocumentPaneControl), new FrameworkPropertyMetadata(false));
		}

		internal LayoutDocumentPaneControl(LayoutDocumentPane model)
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

		private void OnLayoutUpdated(object sender, EventArgs e)
		{
			LayoutDocumentPane actualWidth = this._model;
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

		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
			if (this._model.SelectedContent != null)
			{
				this._model.SelectedContent.IsActive = true;
			}
		}
	}
}