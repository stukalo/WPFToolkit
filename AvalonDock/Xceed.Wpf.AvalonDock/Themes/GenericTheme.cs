using System;

namespace Xceed.Wpf.AvalonDock.Themes
{
	public class GenericTheme : Theme
	{
		public GenericTheme()
		{
		}

		public override Uri GetResourceUri()
		{
			return new Uri("/Xceed.Wpf.AvalonDock;component/Themes/generic.xaml", UriKind.Relative);
		}
	}
}