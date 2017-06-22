using System;

namespace Standard
{
	internal static class DoubleUtilities
	{
		private const double Epsilon = 1.53E-06;

		public static bool AreClose(double value1, double value2)
		{
			if (value1 == value2)
			{
				return true;
			}
			double num = value1 - value2;
			if (num >= 1.53E-06)
			{
				return false;
			}
			return num > -1.53E-06;
		}

		public static bool GreaterThan(double value1, double value2)
		{
			if (value1 <= value2)
			{
				return false;
			}
			return !DoubleUtilities.AreClose(value1, value2);
		}

		public static bool GreaterThanOrClose(double value1, double value2)
		{
			if (value1 > value2)
			{
				return true;
			}
			return DoubleUtilities.AreClose(value1, value2);
		}

		public static bool IsFinite(double value)
		{
			if (double.IsNaN(value))
			{
				return false;
			}
			return !double.IsInfinity(value);
		}

		public static bool IsValidSize(double value)
		{
			if (!DoubleUtilities.IsFinite(value))
			{
				return false;
			}
			return DoubleUtilities.GreaterThanOrClose(value, 0);
		}

		public static bool LessThan(double value1, double value2)
		{
			if (value1 >= value2)
			{
				return false;
			}
			return !DoubleUtilities.AreClose(value1, value2);
		}

		public static bool LessThanOrClose(double value1, double value2)
		{
			if (value1 < value2)
			{
				return true;
			}
			return DoubleUtilities.AreClose(value1, value2);
		}
	}
}