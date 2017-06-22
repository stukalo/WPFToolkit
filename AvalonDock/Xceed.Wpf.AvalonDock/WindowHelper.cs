using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Xceed.Wpf.AvalonDock
{
	internal static class WindowHelper
	{
		public static IntPtr GetParentWindowHandle(this Window window)
		{
			if (window.Owner != null)
			{
				return (new WindowInteropHelper(window.Owner)).Handle;
			}
			return Win32Helper.GetOwner((new WindowInteropHelper(window)).Handle);
		}

		public static bool GetParentWindowHandle(this Visual element, out IntPtr hwnd)
		{
			hwnd = IntPtr.Zero;
			HwndSource hwndSource = PresentationSource.FromVisual(element) as HwndSource;
			if (hwndSource == null)
			{
				return false;
			}
			hwnd = Win32Helper.GetParent(hwndSource.Handle);
			if ((void*)hwnd == IntPtr.Zero)
			{
				hwnd = hwndSource.Handle;
			}
			return true;
		}

		public static bool IsAttachedToPresentationSource(this Visual element)
		{
			return PresentationSource.FromVisual(element) != null;
		}

		public static void SetParentToMainWindowOf(this Window window, Visual element)
		{
			IntPtr intPtr;
			Window window1 = Window.GetWindow(element);
			if (window1 != null)
			{
				window.Owner = window1;
				return;
			}
			if (element.GetParentWindowHandle(out intPtr))
			{
				Win32Helper.SetOwner((new WindowInteropHelper(window)).Handle, intPtr);
			}
		}

		public static void SetParentWindowToNull(this Window window)
		{
			if (window.Owner != null)
			{
				window.Owner = null;
				return;
			}
			Win32Helper.SetOwner((new WindowInteropHelper(window)).Handle, IntPtr.Zero);
		}
	}
}