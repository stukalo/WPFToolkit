using System;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal interface IDropTarget
	{
		DropTargetType Type
		{
			get;
		}

		void DragEnter();

		void DragLeave();

		void Drop(LayoutFloatingWindow floatingWindow);

		Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindow);

		bool HitTest(Point dragPoint);
	}
}