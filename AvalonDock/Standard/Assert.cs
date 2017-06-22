using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Standard
{
	internal static class Assert
	{
		private static void _Break()
		{
			Debugger.Break();
		}

		[Conditional("DEBUG")]
		public static void AreEqual<T>(T expected, T actual)
		{
			if (expected == null)
			{
				if (actual != null && !actual.Equals(expected))
				{
					Standard.Assert._Break();
					return;
				}
			}
			else if (!expected.Equals(actual))
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual<T>(T notExpected, T actual)
		{
			if (notExpected == null)
			{
				if (actual == null || actual.Equals(notExpected))
				{
					Standard.Assert._Break();
					return;
				}
			}
			else if (notExpected.Equals(actual))
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void BoundedDoubleInc(double lowerBoundInclusive, double value, double upperBoundInclusive)
		{
			if (value < lowerBoundInclusive || value > upperBoundInclusive)
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void BoundedInteger(int lowerBoundInclusive, int value, int upperBoundExclusive)
		{
			if (value < lowerBoundInclusive || value >= upperBoundExclusive)
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		[Obsolete("Use Assert.AreEqual instead of Assert.Equals", false)]
		public static void Equals<T>(T expected, T actual)
		{
		}

		[Conditional("DEBUG")]
		public static void Evaluate(Standard.Assert.EvaluateFunction argument)
		{
			argument();
		}

		[Conditional("DEBUG")]
		public static void Fail()
		{
			Standard.Assert._Break();
		}

		[Conditional("DEBUG")]
		public static void Fail(string message)
		{
			Standard.Assert._Break();
		}

		[Conditional("DEBUG")]
		public static void Implies(bool condition, bool result)
		{
			if (condition && !result)
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void Implies(bool condition, Standard.Assert.ImplicationFunction result)
		{
			if (condition && !result())
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void IsApartmentState(ApartmentState expectedState)
		{
			if (Thread.CurrentThread.GetApartmentState() != expectedState)
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void IsDefault<T>(T value)
		where T : struct
		{
			value.Equals(default(T));
		}

		[Conditional("DEBUG")]
		public static void IsFalse(bool condition)
		{
			if (condition)
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void IsFalse(bool condition, string message)
		{
			if (condition)
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void IsNeitherNullNorEmpty(string value)
		{
		}

		[Conditional("DEBUG")]
		public static void IsNeitherNullNorWhitespace(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				Standard.Assert._Break();
			}
			if (value.Trim().Length == 0)
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void IsNotDefault<T>(T value)
		where T : struct
		{
			value.Equals(default(T));
		}

		[Conditional("DEBUG")]
		public static void IsNotNull<T>(T value)
		where T : class
		{
			if (value == null)
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void IsNotOnMainThread()
		{
			if (Application.Current.Dispatcher.CheckAccess())
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void IsNull<T>(T item)
		where T : class
		{
			if (item != null)
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void IsTrue(bool condition)
		{
			if (!condition)
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void IsTrue(bool condition, string message)
		{
			if (!condition)
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void NullableIsNotNull<T>(Nullable<T> value)
		where T : struct
		{
			if (!value.HasValue)
			{
				Standard.Assert._Break();
			}
		}

		[Conditional("DEBUG")]
		public static void NullableIsNull<T>(Nullable<T> value)
		where T : struct
		{
			if (value.HasValue)
			{
				Standard.Assert._Break();
			}
		}

		public delegate void EvaluateFunction();

		public delegate bool ImplicationFunction();
	}
}