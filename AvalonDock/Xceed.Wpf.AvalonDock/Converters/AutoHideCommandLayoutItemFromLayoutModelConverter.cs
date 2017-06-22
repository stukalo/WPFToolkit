using System;
using System.Globalization;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Converters
{
	public class AutoHideCommandLayoutItemFromLayoutModelConverter : IValueConverter
	{
		public AutoHideCommandLayoutItemFromLayoutModelConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			LayoutContent layoutContent = value as LayoutContent;
			if (layoutContent == null)
			{
				return null;
			}
			if (layoutContent.Root == null)
			{
				return null;
			}
			if (layoutContent.Root.Manager == null)
			{
				return null;
			}
			LayoutAnchorableItem layoutItemFromModel = layoutContent.Root.Manager.GetLayoutItemFromModel(layoutContent) as LayoutAnchorableItem;
			if (layoutItemFromModel == null)
			{
				return Binding.DoNothing;
			}
			return layoutItemFromModel.AutoHideCommand;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}