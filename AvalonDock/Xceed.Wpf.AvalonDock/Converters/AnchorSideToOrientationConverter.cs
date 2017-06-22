using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Converters
{
	[ValueConversion(typeof(AnchorSide), typeof(Orientation))]
	public class AnchorSideToOrientationConverter : IValueConverter
	{
		public AnchorSideToOrientationConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			AnchorSide anchorSide = (AnchorSide)value;
			if (anchorSide != AnchorSide.Left && anchorSide != AnchorSide.Right)
			{
				return Orientation.Horizontal;
			}
			return Orientation.Vertical;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}