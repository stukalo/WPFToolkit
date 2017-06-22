using System;
using System.Windows;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class OverlayWindowDropTarget : IOverlayWindowDropTarget
	{
		private IOverlayWindowArea _overlayArea;

		private Rect _screenDetectionArea;

		private OverlayWindowDropTargetType _type;

		Rect Xceed.Wpf.AvalonDock.Controls.IOverlayWindowDropTarget.ScreenDetectionArea
		{
			get
			{
				return this._screenDetectionArea;
			}
		}

		OverlayWindowDropTargetType Xceed.Wpf.AvalonDock.Controls.IOverlayWindowDropTarget.Type
		{
			get
			{
				return this._type;
			}
		}

		internal OverlayWindowDropTarget(IOverlayWindowArea overlayArea, OverlayWindowDropTargetType targetType, FrameworkElement element)
		{
			this._overlayArea = overlayArea;
			this._type = targetType;
			Point point = new Point();
			this._screenDetectionArea = new Rect(element.TransformToDeviceDPI(point), element.TransformActualSizeToAncestor());
		}
	}
}