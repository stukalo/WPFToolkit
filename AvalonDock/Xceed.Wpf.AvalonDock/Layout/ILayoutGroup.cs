using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xceed.Wpf.AvalonDock.Layout
{
	public interface ILayoutGroup : ILayoutContainer, ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging
	{
		int IndexOfChild(ILayoutElement element);

		void InsertChildAt(int index, ILayoutElement element);

		void RemoveChildAt(int index);

		void ReplaceChildAt(int index, ILayoutElement element);

		event EventHandler ChildrenCollectionChanged;
	}
}