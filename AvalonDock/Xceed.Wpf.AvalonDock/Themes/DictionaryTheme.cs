using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Xceed.Wpf.AvalonDock.Themes
{
	public abstract class DictionaryTheme : Theme
	{
		public ResourceDictionary ThemeResourceDictionary
		{
			get;
			private set;
		}

		public DictionaryTheme(ResourceDictionary themeResourceDictionary)
		{
			this.ThemeResourceDictionary = themeResourceDictionary;
		}

		public override Uri GetResourceUri()
		{
			return null;
		}
	}
}