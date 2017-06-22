using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Xceed.Wpf.AvalonDock.Converters
{
	public class UriSourceToBitmapImageConverter : IValueConverter
	{
		public UriSourceToBitmapImageConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return Binding.DoNothing;
			}
			return new Image()
			{
				Source = new BitmapImage((Uri)value)
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}