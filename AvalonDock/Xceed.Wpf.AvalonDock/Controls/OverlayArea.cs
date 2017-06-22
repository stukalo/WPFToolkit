using System;
using System.Windows;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public abstract class OverlayArea : IOverlayWindowArea
	{
		private IOverlayWindow _overlayWindow;

		private Rect? _screenDetectionArea;

		Rect Xceed.Wpf.AvalonDock.Controls.IOverlayWindowArea.ScreenDetectionArea
		{
			get
			{
				return this._screenDetectionArea.Value;
			}
		}

		internal OverlayArea(IOverlayWindow overlayWindow)
		{
			this._overlayWindow = overlayWindow;
		}

		protected void SetScreenDetectionArea(Rect rect)
		{
			this._screenDetectionArea = new Rect?(rect);
		}
	}
}