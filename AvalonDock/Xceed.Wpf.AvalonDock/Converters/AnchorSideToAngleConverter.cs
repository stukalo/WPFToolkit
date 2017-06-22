using System;
using System.Globalization;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Converters
{
	[ValueConversion(typeof(AnchorSide), typeof(double))]
	public class AnchorSideToAngleConverter : IValueConverter
	{
		public AnchorSideToAngleConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			AnchorSide anchorSide = (AnchorSide)value;
			if (anchorSide != AnchorSide.Left && anchorSide != AnchorSide.Right)
			{
				return Binding.DoNothing;
			}
			return 90;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}