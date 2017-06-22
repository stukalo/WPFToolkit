using System.Windows;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public interface IDropArea
	{
		Rect DetectionRect
		{
			get;
		}

		DropAreaType Type
		{
			get;
		}
	}
}