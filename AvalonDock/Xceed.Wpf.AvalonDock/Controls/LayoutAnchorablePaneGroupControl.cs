using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutAnchorablePaneGroupControl : LayoutGridControl<ILayoutAnchorablePane>, ILayoutControl
	{
		private LayoutAnchorablePaneGroup _model;

		internal LayoutAnchorablePaneGroupControl(LayoutAnchorablePaneGroup model) : base(model, model.Orientation)
		{
			this._model = model;
		}

		protected override void OnFixChildrenDockLengths()
		{
			if (this._model.Orientation == System.Windows.Controls.Orientation.Horizontal)
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
				ILayoutPositionableElement gridLength = this._model.Children[j] as ILayoutPositionableElement;
				if (!gridLength.DockHeight.IsStar)
				{
					gridLength.DockHeight = new GridLength(1, GridUnitType.Star);
				}
			}
		}
	}
}