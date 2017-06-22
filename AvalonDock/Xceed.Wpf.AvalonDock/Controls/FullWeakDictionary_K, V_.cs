using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xceed.Wpf.AvalonDock;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal class FullWeakDictionary<K, V>
	where K : class
	{
		private List<WeakReference> _keys;

		private List<WeakReference> _values;

		public V this[K key]
		{
			get
			{
				V v;
				if (!this.GetValue(key, out v))
				{
					throw new ArgumentException();
				}
				return v;
			}
			set
			{
				this.SetValue(key, value);
			}
		}

		public FullWeakDictionary()
		{
		}

		private void CollectGarbage()
		{
			int num = 0;
			do
			{
				num = this._keys.FindIndex(num, (WeakReference k) => !k.IsAlive);
				if (num < 0)
				{
					continue;
				}
				this._keys.RemoveAt(num);
				this._values.RemoveAt(num);
			}
			while (num >= 0);
			num = 0;
			do
			{
				num = this._values.FindIndex(num, (WeakReference v) => !v.IsAlive);
				if (num < 0)
				{
					continue;
				}
				this._values.RemoveAt(num);
				this._keys.RemoveAt(num);
			}
			while (num >= 0);
		}

		public bool ContainsKey(K key)
		{
			this.CollectGarbage();
			return -1 != this._keys.FindIndex((WeakReference k) => (object)k.GetValueOrDefault<K>() == (object)key);
		}

		public bool GetValue(K key, out V value)
		{
			this.CollectGarbage();
			int num = this._keys.FindIndex((WeakReference k) => (object)k.GetValueOrDefault<K>() == (object)key);
			value = default(V);
			if (num == -1)
			{
				return false;
			}
			value = this._values[num].GetValueOrDefault<V>();
			return true;
		}

		public void SetValue(K key, V value)
		{
			this.CollectGarbage();
			int weakReference = this._keys.FindIndex((WeakReference k) => (object)k.GetValueOrDefault<K>() == (object)key);
			if (weakReference > -1)
			{
				this._values[weakReference] = new WeakReference((object)value);
				return;
			}
			this._values.Add(new WeakReference((object)value));
			this._keys.Add(new WeakReference((object)key));
		}
	}
}