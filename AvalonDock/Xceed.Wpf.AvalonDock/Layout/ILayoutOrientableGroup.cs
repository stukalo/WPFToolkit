using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Xceed.Wpf.AvalonDock.Layout
{
	public interface ILayoutOrientableGroup : ILayoutGroup, ILayoutContainer, ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging
	{
		System.Windows.Controls.Orientation Orientation
		{
			get;
			set;
		}
	}
}