using System;
using System.ComponentModel;

namespace Xceed.Wpf.AvalonDock.Layout
{
	public interface ILayoutPanelElement : ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging
	{
		bool IsVisible
		{
			get;
		}
	}
}