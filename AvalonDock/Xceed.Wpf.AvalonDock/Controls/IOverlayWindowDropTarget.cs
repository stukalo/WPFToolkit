using System.Windows;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal interface IOverlayWindowDropTarget
	{
		Rect ScreenDetectionArea
		{
			get;
		}

		OverlayWindowDropTargetType Type
		{
			get;
		}
	}
}