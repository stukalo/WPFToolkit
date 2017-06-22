using System;
using System.Globalization;
using System.Windows.Data;

namespace Xceed.Wpf.AvalonDock.Converters
{
	public class NullToDoNothingConverter : IValueConverter
	{
		public NullToDoNothingConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return Binding.DoNothing;
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}