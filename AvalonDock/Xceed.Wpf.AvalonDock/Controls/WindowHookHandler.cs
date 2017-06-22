using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Xceed.Wpf.AvalonDock;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal class WindowHookHandler
	{
		private IntPtr _windowHook;

		private Win32Helper.HookProc _hookProc;

		private ReentrantFlag _insideActivateEvent = new ReentrantFlag();

		public WindowHookHandler()
		{
		}

		public void Attach()
		{
			this._hookProc = new Win32Helper.HookProc(this.HookProc);
			this._windowHook = Win32Helper.SetWindowsHookEx(Win32Helper.HookType.WH_CBT, this._hookProc, IntPtr.Zero, (int)Win32Helper.GetCurrentThreadId());
		}

		public void Detach()
		{
			Win32Helper.UnhookWindowsHookEx(this._windowHook);
		}

		public int HookProc(int code, IntPtr wParam, IntPtr lParam)
		{
			if (code == 9)
			{
				if (this.FocusChanged != null)
				{
					this.FocusChanged(this, new FocusChangeEventArgs(wParam, lParam));
				}
			}
			else if (code == 5 && this._insideActivateEvent.CanEnter)
			{
				using (ReentrantFlag._ReentrantFlagHandler __ReentrantFlagHandler = this._insideActivateEvent.Enter())
				{
				}
			}
			return Win32Helper.CallNextHookEx(this._windowHook, code, wParam, lParam);
		}

		public event EventHandler<FocusChangeEventArgs> FocusChanged;
	}
}