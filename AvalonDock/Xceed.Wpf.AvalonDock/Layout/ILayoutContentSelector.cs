using System;

namespace Xceed.Wpf.AvalonDock.Layout
{
	public interface ILayoutContentSelector
	{
		LayoutContent SelectedContent
		{
			get;
		}

		int SelectedContentIndex
		{
			get;
			set;
		}

		int IndexOf(LayoutContent content);
	}
}