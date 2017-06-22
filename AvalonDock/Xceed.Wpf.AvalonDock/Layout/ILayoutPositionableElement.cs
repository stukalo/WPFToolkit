using System;
using System.ComponentModel;
using System.Windows;

namespace Xceed.Wpf.AvalonDock.Layout
{
	internal interface ILayoutPositionableElement : ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging, ILayoutElementForFloatingWindow
	{
		GridLength DockHeight
		{
			get;
			set;
		}

		double DockMinHeight
		{
			get;
			set;
		}

		double DockMinWidth
		{
			get;
			set;
		}

		GridLength DockWidth
		{
			get;
			set;
		}

		bool IsVisible
		{
			get;
		}
	}
}