using System;
using System.Windows;
using Xceed.Wpf.AvalonDock;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class DockingManagerOverlayArea : OverlayArea
	{
		private DockingManager _manager;

		internal DockingManagerOverlayArea(IOverlayWindow overlayWindow, DockingManager manager) : base(overlayWindow)
		{
			this._manager = manager;
			DockingManager dockingManager = this._manager;
			Point point = new Point();
			base.SetScreenDetectionArea(new Rect(dockingManager.PointToScreenDPI(point), this._manager.TransformActualSizeToAncestor()));
		}
	}
}