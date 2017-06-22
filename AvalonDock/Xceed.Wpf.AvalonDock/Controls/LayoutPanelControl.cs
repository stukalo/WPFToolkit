using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutPanelControl : LayoutGridControl<ILayoutPanelElement>, ILayoutControl
	{
		private LayoutPanel _model;

		internal LayoutPanelControl(LayoutPanel model) : base(model, model.Orientation)
		{
			this._model = model;
		}

		protected override void OnFixChildrenDockLengths()
		{
			if (base.ActualWidth == 0 || base.ActualHeight == 0)
			{
				return;
			}
			LayoutPanel layoutPanel = this._model;
			if (this._model.Orientation == System.Windows.Controls.Orientation.Horizontal)
			{
				if (!this._model.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>())
				{
					for (int i = 0; i < this._model.Children.Count; i++)
					{
						ILayoutPositionableElement item = this._model.Children[i] as ILayoutPositionableElement;
						if (!item.DockWidth.IsStar)
						{
							item.DockWidth = new GridLength(1, GridUnitType.Star);
						}
					}
					return;
				}
				for (int j = 0; j < this._model.Children.Count; j++)
				{
					ILayoutContainer layoutContainer = this._model.Children[j] as ILayoutContainer;
					ILayoutPositionableElement gridLength = this._model.Children[j] as ILayoutPositionableElement;
					if (layoutContainer != null && (layoutContainer.IsOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>() || layoutContainer.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>()))
					{
						gridLength.DockWidth = new GridLength(1, GridUnitType.Star);
					}
					else if (gridLength != null && gridLength.DockWidth.IsStar)
					{
						double num = Math.Max((gridLength as ILayoutPositionableElementWithActualSize).ActualWidth, gridLength.DockMinWidth);
						num = Math.Min(num, base.ActualWidth / 2);
						num = Math.Max(num, gridLength.DockMinWidth);
						gridLength.DockWidth = new GridLength(num, GridUnitType.Pixel);
					}
				}
				return;
			}
			if (!this._model.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>())
			{
				for (int k = 0; k < this._model.Children.Count; k++)
				{
					ILayoutPositionableElement layoutPositionableElement = this._model.Children[k] as ILayoutPositionableElement;
					if (!layoutPositionableElement.DockHeight.IsStar)
					{
						layoutPositionableElement.DockHeight = new GridLength(1, GridUnitType.Star);
					}
				}
				return;
			}
			for (int l = 0; l < this._model.Children.Count; l++)
			{
				ILayoutContainer item1 = this._model.Children[l] as ILayoutContainer;
				ILayoutPositionableElement gridLength1 = this._model.Children[l] as ILayoutPositionableElement;
				if (item1 != null && (item1.IsOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>() || item1.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>()))
				{
					gridLength1.DockHeight = new GridLength(1, GridUnitType.Star);
				}
				else if (gridLength1 != null && gridLength1.DockHeight.IsStar)
				{
					double num1 = Math.Max((gridLength1 as ILayoutPositionableElementWithActualSize).ActualHeight, gridLength1.DockMinHeight);
					num1 = Math.Min(num1, base.ActualHeight / 2);
					num1 = Math.Max(num1, gridLength1.DockMinHeight);
					gridLength1.DockHeight = new GridLength(num1, GridUnitType.Pixel);
				}
			}
		}
	}
}