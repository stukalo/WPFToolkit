using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xceed.Wpf.AvalonDock.Layout
{
	public interface ILayoutContainer : ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging
	{
		IEnumerable<ILayoutElement> Children
		{
			get;
		}

		int ChildrenCount
		{
			get;
		}

		void RemoveChild(ILayoutElement element);

		void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement);
	}
}