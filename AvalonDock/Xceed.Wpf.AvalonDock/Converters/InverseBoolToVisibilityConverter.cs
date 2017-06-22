using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Xceed.Wpf.AvalonDock.Converters
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class InverseBoolToVisibilityConverter : IValueConverter
	{
		public InverseBoolToVisibilityConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool) || !(targetType == typeof(Visibility)))
			{
				throw new ArgumentException("Invalid argument/return type. Expected argument: bool and return type: Visibility");
			}
			if (!(bool)value)
			{
				return Visibility.Visible;
			}
			if (parameter != null && parameter is Visibility)
			{
				return parameter;
			}
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Visibility) || !(targetType == typeof(bool)))
			{
				throw new ArgumentException("Invalid argument/return type. Expected argument: Visibility and return type: bool");
			}
			if ((Visibility)value == Visibility.Visible)
			{
				return false;
			}
			return true;
		}
	}
}