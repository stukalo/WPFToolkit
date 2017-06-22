using System;

namespace Xceed.Wpf.AvalonDock
{
	internal static class MathHelper
	{
		public static void AssertIsPositiveOrZero(double value)
		{
			if (value < 0)
			{
				throw new ArgumentException("Invalid value, must be a positive number or equal to zero");
			}
		}

		public static double MinMax(double value, double min, double max)
		{
			if (min > max)
			{
				throw new ArgumentException("min>max");
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}
	}
}