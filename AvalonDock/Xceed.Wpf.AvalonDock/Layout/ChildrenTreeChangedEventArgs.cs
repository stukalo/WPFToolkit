using System;
using System.Runtime.CompilerServices;

namespace Xceed.Wpf.AvalonDock.Layout
{
	public class ChildrenTreeChangedEventArgs : EventArgs
	{
		public ChildrenTreeChange Change
		{
			get;
			private set;
		}

		public ChildrenTreeChangedEventArgs(ChildrenTreeChange change)
		{
			this.Change = change;
		}
	}
}