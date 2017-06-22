using System.ComponentModel;

namespace Xceed.Wpf.AvalonDock.Layout
{
	public interface ILayoutElement : INotifyPropertyChanged, INotifyPropertyChanging
	{
		ILayoutContainer Parent
		{
			get;
		}

		ILayoutRoot Root
		{
			get;
		}
	}
}