using System;
using System.Windows;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class AnchorablePaneControlOverlayArea : OverlayArea
	{
		private LayoutAnchorablePaneControl _anchorablePaneControl;

		internal AnchorablePaneControlOverlayArea(IOverlayWindow overlayWindow, LayoutAnchorablePaneControl anchorablePaneControl) : base(overlayWindow)
		{
			this._anchorablePaneControl = anchorablePaneControl;
			LayoutAnchorablePaneControl layoutAnchorablePaneControl = this._anchorablePaneControl;
			Point point = new Point();
			base.SetScreenDetectionArea(new Rect(layoutAnchorablePaneControl.PointToScreenDPI(point), this._anchorablePaneControl.TransformActualSizeToAncestor()));
		}
	}
}