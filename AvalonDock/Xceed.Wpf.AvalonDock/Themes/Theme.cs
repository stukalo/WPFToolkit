using System;
using System.Windows;

namespace Xceed.Wpf.AvalonDock.Themes
{
	public abstract class Theme : DependencyObject
	{
		public Theme()
		{
		}

		public abstract Uri GetResourceUri();
	}
}