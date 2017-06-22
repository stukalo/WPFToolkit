using System;
using System.Collections.ObjectModel;
using Xceed.Wpf.AvalonDock;

namespace Xceed.Wpf.AvalonDock.Layout
{
	public interface ILayoutRoot
	{
		LayoutContent ActiveContent
		{
			get;
			set;
		}

		LayoutAnchorSide BottomSide
		{
			get;
		}

		ObservableCollection<LayoutFloatingWindow> FloatingWindows
		{
			get;
		}

		ObservableCollection<LayoutAnchorable> Hidden
		{
			get;
		}

		LayoutAnchorSide LeftSide
		{
			get;
		}

		DockingManager Manager
		{
			get;
		}

		LayoutAnchorSide RightSide
		{
			get;
		}

		LayoutPanel RootPanel
		{
			get;
		}

		LayoutAnchorSide TopSide
		{
			get;
		}

		void CollectGarbage();
	}
}