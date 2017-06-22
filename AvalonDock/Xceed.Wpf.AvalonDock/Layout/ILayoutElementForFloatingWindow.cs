using System;

namespace Xceed.Wpf.AvalonDock.Layout
{
	internal interface ILayoutElementForFloatingWindow
	{
		double FloatingHeight
		{
			get;
			set;
		}

		double FloatingLeft
		{
			get;
			set;
		}

		double FloatingTop
		{
			get;
			set;
		}

		double FloatingWidth
		{
			get;
			set;
		}

		bool IsMaximized
		{
			get;
			set;
		}
	}
}