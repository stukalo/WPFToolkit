using System;
using System.Runtime.CompilerServices;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock
{
	internal class LayoutEventArgs : EventArgs
	{
		public Xceed.Wpf.AvalonDock.Layout.LayoutRoot LayoutRoot
		{
			get;
			private set;
		}

		public LayoutEventArgs(Xceed.Wpf.AvalonDock.Layout.LayoutRoot layoutRoot)
		{
			this.LayoutRoot = layoutRoot;
		}
	}
}