using System;

namespace Xceed.Wpf.AvalonDock.Layout
{
	internal interface ILayoutPositionableElementWithActualSize
	{
		double ActualHeight
		{
			get;
			set;
		}

		double ActualWidth
		{
			get;
			set;
		}
	}
}