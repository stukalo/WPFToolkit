using System;
using System.Runtime.CompilerServices;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal class WindowActivateEventArgs : EventArgs
	{
		public IntPtr HwndActivating
		{
			get;
			private set;
		}

		public WindowActivateEventArgs(IntPtr hwndActivating)
		{
			this.HwndActivating = hwndActivating;
		}
	}
}