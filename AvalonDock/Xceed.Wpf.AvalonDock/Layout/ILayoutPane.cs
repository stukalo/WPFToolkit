using System;
using System.ComponentModel;

namespace Xceed.Wpf.AvalonDock.Layout
{
	public interface ILayoutPane : ILayoutContainer, ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging, ILayoutElementWithVisibility
	{
		void MoveChild(int oldIndex, int newIndex);

		void RemoveChildAt(int childIndex);
	}
}