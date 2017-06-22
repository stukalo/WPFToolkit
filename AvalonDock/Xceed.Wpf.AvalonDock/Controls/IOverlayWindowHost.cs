using System;
using System.Collections.Generic;
using System.Windows;
using Xceed.Wpf.AvalonDock;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal interface IOverlayWindowHost
	{
		DockingManager Manager
		{
			get;
		}

		IEnumerable<IDropArea> GetDropAreas(LayoutFloatingWindowControl draggingWindow);

		void HideOverlayWindow();

		bool HitTest(Point dragPoint);

		IOverlayWindow ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow);
	}
}