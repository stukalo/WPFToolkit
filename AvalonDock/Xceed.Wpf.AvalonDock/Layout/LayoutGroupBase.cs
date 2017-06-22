using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[Serializable]
	public abstract class LayoutGroupBase : LayoutElement
	{
		protected LayoutGroupBase()
		{
		}

		protected void NotifyChildrenTreeChanged(ChildrenTreeChange change)
		{
			this.OnChildrenTreeChanged(change);
			LayoutGroupBase parent = base.Parent as LayoutGroupBase;
			if (parent != null)
			{
				parent.NotifyChildrenTreeChanged(ChildrenTreeChange.TreeChanged);
			}
		}

		protected virtual void OnChildrenCollectionChanged()
		{
			if (this.ChildrenCollectionChanged != null)
			{
				this.ChildrenCollectionChanged(this, EventArgs.Empty);
			}
		}

		protected virtual void OnChildrenTreeChanged(ChildrenTreeChange change)
		{
			if (this.ChildrenTreeChanged != null)
			{
				this.ChildrenTreeChanged(this, new ChildrenTreeChangedEventArgs(change));
			}
		}

		public event EventHandler ChildrenCollectionChanged;

		public event EventHandler<ChildrenTreeChangedEventArgs> ChildrenTreeChanged;
	}
}