using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Xml.Serialization;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[Serializable]
	public abstract class LayoutElement : DependencyObject, ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging
	{
		[NonSerialized]
		private ILayoutContainer _parent;

		[NonSerialized]
		private ILayoutRoot _root;

		[XmlIgnore]
		public ILayoutContainer Parent
		{
			get
			{
				return JustDecompileGenerated_get_Parent();
			}
			set
			{
				JustDecompileGenerated_set_Parent(value);
			}
		}

		public ILayoutContainer JustDecompileGenerated_get_Parent()
		{
			return this._parent;
		}

		public void JustDecompileGenerated_set_Parent(ILayoutContainer value)
		{
			if (this._parent != value)
			{
				ILayoutContainer layoutContainer = this._parent;
				ILayoutRoot layoutRoot = this._root;
				this.RaisePropertyChanging("Parent");
				this.OnParentChanging(layoutContainer, value);
				this._parent = value;
				this.OnParentChanged(layoutContainer, value);
				this._root = this.Root;
				if (layoutRoot != this._root)
				{
					this.OnRootChanged(layoutRoot, this._root);
				}
				this.RaisePropertyChanged("Parent");
				LayoutRoot root = this.Root as LayoutRoot;
				if (root != null)
				{
					root.FireLayoutUpdated();
				}
			}
		}

		public ILayoutRoot Root
		{
			get
			{
				ILayoutContainer parent = this.Parent;
				while (parent != null && !(parent is ILayoutRoot))
				{
					parent = parent.Parent;
				}
				return parent as ILayoutRoot;
			}
		}

		internal LayoutElement()
		{
		}

		public virtual void ConsoleDump(int tab)
		{
			Trace.Write(new string(' ', tab * 4));
			Trace.WriteLine(this.ToString());
		}

		protected virtual void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
		{
		}

		protected virtual void OnParentChanging(ILayoutContainer oldValue, ILayoutContainer newValue)
		{
		}

		protected virtual void OnRootChanged(ILayoutRoot oldRoot, ILayoutRoot newRoot)
		{
			if (oldRoot != null)
			{
				((LayoutRoot)oldRoot).OnLayoutElementRemoved(this);
			}
			if (newRoot != null)
			{
				((LayoutRoot)newRoot).OnLayoutElementAdded(this);
			}
		}

		protected virtual void RaisePropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected virtual void RaisePropertyChanging(string propertyName)
		{
			if (this.PropertyChanging != null)
			{
				this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public event PropertyChangingEventHandler PropertyChanging;
	}
}