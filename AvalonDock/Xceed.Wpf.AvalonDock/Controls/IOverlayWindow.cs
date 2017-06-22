using System;
using System.Collections.Generic;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal interface IOverlayWindow
	{
		void DragDrop(IDropTarget target);

		void DragEnter(LayoutFloatingWindowControl floatingWindow);

		void DragEnter(IDropArea area);

		void DragEnter(IDropTarget target);

		void DragLeave(LayoutFloatingWindowControl floatingWindow);

		void DragLeave(IDropArea area);

		void DragLeave(IDropTarget target);

		IEnumerable<IDropTarget> GetTargets();
	}
}