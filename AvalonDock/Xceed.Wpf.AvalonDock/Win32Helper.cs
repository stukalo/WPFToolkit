using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Xceed.Wpf.AvalonDock
{
	internal static class Win32Helper
	{
		internal const int WS_CHILD = 1073741824;

		internal const int WS_VISIBLE = 268435456;

		internal const int WS_VSCROLL = 2097152;

		internal const int WS_BORDER = 8388608;

		internal const int WS_CLIPSIBLINGS = 67108864;

		internal const int WS_CLIPCHILDREN = 33554432;

		internal const int WS_TABSTOP = 65536;

		internal const int WS_GROUP = 131072;

		internal readonly static IntPtr HWND_TOPMOST;

		internal readonly static IntPtr HWND_NOTOPMOST;

		internal readonly static IntPtr HWND_TOP;

		internal readonly static IntPtr HWND_BOTTOM;

		internal const int WM_WINDOWPOSCHANGED = 71;

		internal const int WM_WINDOWPOSCHANGING = 70;

		internal const int WM_NCMOUSEMOVE = 160;

		internal const int WM_NCLBUTTONDOWN = 161;

		internal const int WM_NCLBUTTONUP = 162;

		internal const int WM_NCLBUTTONDBLCLK = 163;

		internal const int WM_NCRBUTTONDOWN = 164;

		internal const int WM_NCRBUTTONUP = 165;

		internal const int WM_CAPTURECHANGED = 533;

		internal const int WM_EXITSIZEMOVE = 562;

		internal const int WM_ENTERSIZEMOVE = 561;

		internal const int WM_MOVE = 3;

		internal const int WM_MOVING = 534;

		internal const int WM_KILLFOCUS = 8;

		internal const int WM_SETFOCUS = 7;

		internal const int WM_ACTIVATE = 6;

		internal const int WM_NCHITTEST = 132;

		internal const int WM_INITMENUPOPUP = 279;

		internal const int WM_KEYDOWN = 256;

		internal const int WM_KEYUP = 257;

		internal const int WA_INACTIVE = 0;

		internal const int WM_SYSCOMMAND = 274;

		internal const int SC_MAXIMIZE = 61488;

		internal const int SC_RESTORE = 61728;

		internal const int WM_CREATE = 1;

		internal const int HT_CAPTION = 2;

		public const int HCBT_SETFOCUS = 9;

		public const int HCBT_ACTIVATE = 5;

		internal const uint GW_HWNDNEXT = 2;

		internal const uint GW_HWNDPREV = 3;

		internal const int WM_MOUSEMOVE = 512;

		internal const int WM_LBUTTONDOWN = 513;

		internal const int WM_LBUTTONUP = 514;

		internal const int WM_LBUTTONDBLCLK = 515;

		internal const int WM_RBUTTONDOWN = 516;

		internal const int WM_RBUTTONUP = 517;

		internal const int WM_RBUTTONDBLCLK = 518;

		internal const int WM_MBUTTONDOWN = 519;

		internal const int WM_MBUTTONUP = 520;

		internal const int WM_MBUTTONDBLCLK = 521;

		internal const int WM_MOUSEWHEEL = 522;

		internal const int WM_MOUSEHWHEEL = 526;

		static Win32Helper()
		{
			Win32Helper.HWND_TOPMOST = new IntPtr(-1);
			Win32Helper.HWND_NOTOPMOST = new IntPtr(-2);
			Win32Helper.HWND_TOP = new IntPtr(0);
			Win32Helper.HWND_BOTTOM = new IntPtr(1);
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern bool BringWindowToTop(IntPtr hWnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int CallNextHookEx(IntPtr hhook, int code, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet=CharSet.Unicode, ExactSpelling=false)]
		internal static extern IntPtr CreateWindowEx(int dwExStyle, string lpszClassName, string lpszWindowName, int style, int x, int y, int width, int height, IntPtr hwndParent, IntPtr hMenu, IntPtr hInst, object pvParam);

		[DllImport("user32.dll", CharSet=CharSet.Unicode, ExactSpelling=false)]
		internal static extern bool DestroyWindow(IntPtr hwnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool GetClientRect(IntPtr hWnd, out Win32Helper.RECT lpRect);

		internal static Win32Helper.RECT GetClientRect(IntPtr hWnd)
		{
			Win32Helper.RECT rECT = new Win32Helper.RECT();
			Win32Helper.GetClientRect(hWnd, out rECT);
			return rECT;
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern uint GetCurrentThreadId();

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern bool GetCursorPos(ref Win32Helper.Win32Point pt);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern IntPtr GetFocus();

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool GetMonitorInfo(IntPtr hMonitor, [In][Out] Win32Helper.MonitorInfo lpmi);

		internal static Point GetMousePosition()
		{
			Win32Helper.Win32Point win32Point = new Win32Helper.Win32Point();
			Win32Helper.GetCursorPos(ref win32Point);
			return new Point((double)win32Point.X, (double)win32Point.Y);
		}

		public static IntPtr GetOwner(IntPtr childHandle)
		{
			return new IntPtr(Win32Helper.GetWindowLong(childHandle, -8));
		}

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
		internal static extern IntPtr GetParent(IntPtr hWnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern IntPtr GetTopWindow(IntPtr hWnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern bool GetWindowRect(IntPtr hWnd, out Win32Helper.RECT lpRect);

		internal static Win32Helper.RECT GetWindowRect(IntPtr hWnd)
		{
			Win32Helper.RECT rECT = new Win32Helper.RECT();
			Win32Helper.GetWindowRect(hWnd, out rECT);
			return rECT;
		}

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
		internal static extern bool IsChild(IntPtr hWndParent, IntPtr hwnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern bool IsWindowEnabled(IntPtr hWnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern bool IsWindowVisible(IntPtr hWnd);

		internal static int MakeLParam(int LoWord, int HiWord)
		{
			return HiWord << 16 | LoWord & 65535;
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr MonitorFromRect([In] ref Win32Helper.RECT lprc, uint dwFlags);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern int PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr SetActiveWindow(IntPtr hWnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern IntPtr SetFocus(IntPtr hWnd);

		public static void SetOwner(IntPtr childHandle, IntPtr ownerHandle)
		{
			Win32Helper.SetWindowLong(childHandle, -8, ownerHandle.ToInt32());
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, Win32Helper.SetWindowPosFlags uFlags);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr SetWindowsHookEx(Win32Helper.HookType code, Win32Helper.HookProc func, IntPtr hInstance, int threadID);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int UnhookWindowsHookEx(IntPtr hhook);

		internal enum GetWindow_Cmd : uint
		{
			GW_HWNDFIRST,
			GW_HWNDLAST,
			GW_HWNDNEXT,
			GW_HWNDPREV,
			GW_OWNER,
			GW_CHILD,
			GW_ENABLEDPOPUP
		}

		public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

		public enum HookType
		{
			WH_JOURNALRECORD,
			WH_JOURNALPLAYBACK,
			WH_KEYBOARD,
			WH_GETMESSAGE,
			WH_CALLWNDPROC,
			WH_CBT,
			WH_SYSMSGFILTER,
			WH_MOUSE,
			WH_HARDWARE,
			WH_DEBUG,
			WH_SHELL,
			WH_FOREGROUNDIDLE,
			WH_CALLWNDPROCRET,
			WH_KEYBOARD_LL,
			WH_MOUSE_LL
		}

		public class MonitorInfo
		{
			public int Size;

			public Win32Helper.RECT Monitor;

			public Win32Helper.RECT Work;

			public uint Flags;

			public MonitorInfo()
			{
			}
		}

		[Serializable]
		internal struct RECT
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;

			public int Height
			{
				get
				{
					return this.Bottom - this.Top;
				}
			}

			public Point Location
			{
				get
				{
					return new Point((double)this.Left, (double)this.Top);
				}
			}

			public Size Size
			{
				get
				{
					return new Size((double)this.Width, (double)this.Height);
				}
			}

			public int Width
			{
				get
				{
					return this.Right - this.Left;
				}
			}

			public RECT(int left_, int top_, int right_, int bottom_)
			{
				this.Left = left_;
				this.Top = top_;
				this.Right = right_;
				this.Bottom = bottom_;
			}

			public static Win32Helper.RECT FromRectangle(Rect rectangle)
			{
				return new Rect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
			}

			public override int GetHashCode()
			{
				return this.Left ^ (this.Top << 13 | this.Top >> 19) ^ (this.Width << 26 | this.Width >> 6) ^ (this.Height << 7 | this.Height >> 25);
			}

			public static implicit operator Rect(Win32Helper.RECT rect)
			{
				return rect.ToRectangle();
			}

			public static implicit operator RECT(Rect rect)
			{
				return Win32Helper.RECT.FromRectangle(rect);
			}

			public Rect ToRectangle()
			{
				return new Rect((double)this.Left, (double)this.Top, (double)this.Right, (double)this.Bottom);
			}
		}

		[Flags]
		internal enum SetWindowPosFlags : uint
		{
			IgnoreResize = 1,
			IgnoreMove = 2,
			IgnoreZOrder = 4,
			DoNotRedraw = 8,
			DoNotActivate = 16,
			DrawFrame = 32,
			FrameChanged = 32,
			ShowWindow = 64,
			HideWindow = 128,
			DoNotCopyBits = 256,
			DoNotChangeOwnerZOrder = 512,
			DoNotReposition = 512,
			DoNotSendChangingEvent = 1024,
			DeferErase = 8192,
			SynchronousWindowPosition = 16384
		}

		internal struct Win32Point
		{
			public int X;

			public int Y;
		}

		internal class WINDOWPOS
		{
			public IntPtr hwnd;

			public IntPtr hwndInsertAfter;

			public int x;

			public int y;

			public int cx;

			public int cy;

			public int flags;

			public WINDOWPOS()
			{
			}
		}
	}
}