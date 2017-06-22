using System;

namespace Xceed.Wpf.AvalonDock.Layout
{
	public interface ILayoutUpdateStrategy
	{
		void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown);

		void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown);

		bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer);

		bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer);
	}
}