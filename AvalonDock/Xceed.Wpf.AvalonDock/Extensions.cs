using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Xceed.Wpf.AvalonDock
{
	internal static class Extensions
	{
		public static bool Contains(this IEnumerable collection, object item)
		{
			bool flag;
			IEnumerator enumerator = collection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != item)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}

		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			foreach (T t in collection)
			{
				action(t);
			}
		}

		public static V GetValueOrDefault<V>(this WeakReference wr)
		{
			if (wr != null && wr.IsAlive)
			{
				return (V)wr.Target;
			}
			return default(V);
		}

		public static int IndexOf<T>(this T[] array, T value)
		where T : class
		{
			for (int i = 0; i < (int)array.Length; i++)
			{
				if ((object)array[i] == (object)value)
				{
					return i;
				}
			}
			return -1;
		}
	}
}