using System;

namespace Xceed.Wpf.AvalonDock.Layout
{
	internal interface ILayoutPreviousContainer
	{
		ILayoutContainer PreviousContainer
		{
			get;
			set;
		}

		string PreviousContainerId
		{
			get;
			set;
		}
	}
}