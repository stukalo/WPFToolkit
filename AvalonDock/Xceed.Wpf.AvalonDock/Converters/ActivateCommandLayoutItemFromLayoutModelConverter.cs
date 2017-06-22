using System;
using System.Globalization;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Converters
{
	public class ActivateCommandLayoutItemFromLayoutModelConverter : IValueConverter
	{
		public ActivateCommandLayoutItemFromLayoutModelConverter()
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
			LayoutItem layoutItemFromModel = layoutContent.Root.Manager.GetLayoutItemFromModel(layoutContent);
			if (layoutItemFromModel == null)
			{
				return Binding.DoNothing;
			}
			return layoutItemFromModel.ActivateCommand;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}