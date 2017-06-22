using System;
using System.Runtime.CompilerServices;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal class FocusChangeEventArgs : EventArgs
	{
		public IntPtr GotFocusWinHandle
		{
			get;
			private set;
		}

		public IntPtr LostFocusWinHandle
		{
			get;
			private set;
		}

		public FocusChangeEventArgs(IntPtr gotFocusWinHandle, IntPtr lostFocusWinHandle)
		{
			this.GotFocusWinHandle = gotFocusWinHandle;
			this.LostFocusWinHandle = lostFocusWinHandle;
		}
	}
}