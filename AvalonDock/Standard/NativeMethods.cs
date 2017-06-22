using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Standard
{
	internal static class NativeMethods
	{
		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="AdjustWindowRectEx", ExactSpelling=false, SetLastError=true)]
		private static extern bool _AdjustWindowRectEx(ref RECT lpRect, WS dwStyle, bool bMenu, WS_EX dwExStyle);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="ChangeWindowMessageFilter", ExactSpelling=false, SetLastError=true)]
		private static extern bool _ChangeWindowMessageFilter(WM message, MSGFLT dwFlag);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="ChangeWindowMessageFilterEx", ExactSpelling=false, SetLastError=true)]
		private static extern bool _ChangeWindowMessageFilterEx(IntPtr hwnd, WM message, MSGFLT action, [In][Out] ref CHANGEFILTERSTRUCT pChangeFilterStruct = default(CHANGEFILTERSTRUCT));

		[DllImport("shell32.dll", CharSet=CharSet.Unicode, EntryPoint="CommandLineToArgvW", ExactSpelling=false)]
		private static extern IntPtr _CommandLineToArgvW(string cmdLine, out int numArgs);

		[DllImport("gdi32.dll", CharSet=CharSet.None, EntryPoint="CreateDIBSection", ExactSpelling=false, SetLastError=true)]
		private static extern SafeHBITMAP _CreateDIBSection(SafeDC hdc, [In] ref BITMAPINFO bitmapInfo, int iUsage, out IntPtr ppvBits, IntPtr hSection, int dwOffset);

		[DllImport("gdi32.dll", CharSet=CharSet.None, EntryPoint="CreateDIBSection", ExactSpelling=false, SetLastError=true)]
		private static extern SafeHBITMAP _CreateDIBSectionIntPtr(IntPtr hdc, [In] ref BITMAPINFO bitmapInfo, int iUsage, out IntPtr ppvBits, IntPtr hSection, int dwOffset);

		[DllImport("gdi32.dll", CharSet=CharSet.None, EntryPoint="CreateRectRgn", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr _CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

		[DllImport("gdi32.dll", CharSet=CharSet.None, EntryPoint="CreateRectRgnIndirect", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr _CreateRectRgnIndirect([In] ref RECT lprc);

		[DllImport("gdi32.dll", CharSet=CharSet.None, EntryPoint="CreateRoundRectRgn", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr _CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

		[DllImport("user32.dll", CharSet=CharSet.Unicode, EntryPoint="CreateWindowExW", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr _CreateWindowEx(WS_EX dwExStyle, string lpClassName, string lpWindowName, WS dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="DrawMenuBar", ExactSpelling=false, SetLastError=true)]
		private static extern bool _DrawMenuBar(IntPtr hWnd);

		[DllImport("dwmapi.dll", CharSet=CharSet.None, EntryPoint="DwmGetColorizationColor", ExactSpelling=false)]
		private static extern HRESULT _DwmGetColorizationColor(out uint pcrColorization, out bool pfOpaqueBlend);

		[DllImport("dwmapi.dll", CharSet=CharSet.None, EntryPoint="DwmGetCompositionTimingInfo", ExactSpelling=false)]
		private static extern HRESULT _DwmGetCompositionTimingInfo(IntPtr hwnd, ref DWM_TIMING_INFO pTimingInfo);

		[DllImport("dwmapi.dll", CharSet=CharSet.None, EntryPoint="DwmIsCompositionEnabled", ExactSpelling=false, PreserveSig=false)]
		private static extern bool _DwmIsCompositionEnabled();

		[DllImport("dwmapi.dll", CharSet=CharSet.None, EntryPoint="DwmSetWindowAttribute", ExactSpelling=false)]
		private static extern void _DwmSetWindowAttribute(IntPtr hwnd, DWMWA dwAttribute, ref int pvAttribute, int cbAttribute);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="EnableMenuItem", ExactSpelling=false)]
		private static extern int _EnableMenuItem(IntPtr hMenu, SC uIDEnableItem, MF uEnable);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="GetClientRect", ExactSpelling=false, SetLastError=true)]
		private static extern bool _GetClientRect(IntPtr hwnd, out RECT lpRect);

		[DllImport("uxtheme.dll", CharSet=CharSet.Unicode, EntryPoint="GetCurrentThemeName", ExactSpelling=false)]
		private static extern HRESULT _GetCurrentThemeName(StringBuilder pszThemeFileName, int dwMaxNameChars, StringBuilder pszColorBuff, int cchMaxColorChars, StringBuilder pszSizeBuff, int cchMaxSizeChars);

		[DllImport("kernel32.dll", CharSet=CharSet.Unicode, EntryPoint="GetModuleFileName", ExactSpelling=false, SetLastError=true)]
		private static extern int _GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);

		[DllImport("kernel32.dll", CharSet=CharSet.Unicode, EntryPoint="GetModuleHandleW", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr _GetModuleHandle(string lpModuleName);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="GetMonitorInfo", ExactSpelling=false, SetLastError=true)]
		private static extern bool _GetMonitorInfo(IntPtr hMonitor, [In][Out] MONITORINFO lpmi);

		[DllImport("gdi32.dll", CharSet=CharSet.None, EntryPoint="GetStockObject", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr _GetStockObject(StockObject fnObject);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="GetWindowRect", ExactSpelling=false, SetLastError=true)]
		private static extern bool _GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("kernel32.dll", CharSet=CharSet.None, EntryPoint="LocalFree", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr _LocalFree(IntPtr hMem);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="PostMessage", ExactSpelling=false, SetLastError=true)]
		private static extern bool _PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="RegisterClassExW", ExactSpelling=false, SetLastError=true)]
		private static extern short _RegisterClassEx([In] ref WNDCLASSEX lpwcx);

		[DllImport("user32.dll", CharSet=CharSet.Unicode, EntryPoint="RegisterWindowMessage", ExactSpelling=false, SetLastError=true)]
		private static extern uint _RegisterWindowMessage(string lpString);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="RemoveMenu", ExactSpelling=false, SetLastError=true)]
		private static extern bool _RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

		[DllImport("gdi32.dll", CharSet=CharSet.None, EntryPoint="SelectObject", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr _SelectObject(SafeDC hdc, IntPtr hgdiobj);

		[DllImport("gdi32.dll", CharSet=CharSet.None, EntryPoint="SelectObject", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr _SelectObjectSafeHBITMAP(SafeDC hdc, SafeHBITMAP hgdiobj);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="SetActiveWindow", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr _SetActiveWindow(IntPtr hWnd);

		[DllImport("kernel32.dll", CharSet=CharSet.None, EntryPoint="SetProcessWorkingSetSize", ExactSpelling=false, SetLastError=true)]
		private static extern bool _SetProcessWorkingSetSize(IntPtr hProcess, IntPtr dwMinimiumWorkingSetSize, IntPtr dwMaximumWorkingSetSize);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="SetWindowPos", ExactSpelling=false, SetLastError=true)]
		private static extern bool _SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP uFlags);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="SetWindowRgn", ExactSpelling=false, SetLastError=true)]
		private static extern int _SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

		[DllImport("shell32.dll", CharSet=CharSet.None, EntryPoint="SHAddToRecentDocs", ExactSpelling=false)]
		private static extern void _SHAddToRecentDocs_ShellLink(SHARD uFlags, IShellLinkW pv);

		[DllImport("shell32.dll", CharSet=CharSet.None, EntryPoint="SHAddToRecentDocs", ExactSpelling=false)]
		private static extern void _SHAddToRecentDocs_String(SHARD uFlags, string pv);

		[DllImport("user32.dll", CharSet=CharSet.Unicode, EntryPoint="SystemParametersInfoW", ExactSpelling=false, SetLastError=true)]
		private static extern bool _SystemParametersInfo_HIGHCONTRAST(SPI uiAction, int uiParam, [In][Out] ref HIGHCONTRAST pvParam, SPIF fWinIni);

		[DllImport("user32.dll", CharSet=CharSet.Unicode, EntryPoint="SystemParametersInfoW", ExactSpelling=false, SetLastError=true)]
		private static extern bool _SystemParametersInfo_NONCLIENTMETRICS(SPI uiAction, int uiParam, [In][Out] ref NONCLIENTMETRICS pvParam, SPIF fWinIni);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="SystemParametersInfoW", ExactSpelling=false, SetLastError=true)]
		private static extern bool _SystemParametersInfo_String(SPI uiAction, int uiParam, string pvParam, SPIF fWinIni);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="UnregisterClass", ExactSpelling=false, SetLastError=true)]
		private static extern bool _UnregisterClassAtom(IntPtr lpClassName, IntPtr hInstance);

		[DllImport("user32.dll", CharSet=CharSet.Unicode, EntryPoint="UnregisterClass", ExactSpelling=false, SetLastError=true)]
		private static extern bool _UnregisterClassName(string lpClassName, IntPtr hInstance);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="UpdateLayeredWindow", ExactSpelling=false, SetLastError=true)]
		private static extern bool _UpdateLayeredWindow(IntPtr hwnd, SafeDC hdcDst, [In] ref POINT pptDst, [In] ref SIZE psize, SafeDC hdcSrc, [In] ref POINT pptSrc, int crKey, ref BLENDFUNCTION pblend, ULW dwFlags);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="UpdateLayeredWindow", ExactSpelling=false, SetLastError=true)]
		private static extern bool _UpdateLayeredWindowIntPtr(IntPtr hwnd, IntPtr hdcDst, IntPtr pptDst, IntPtr psize, IntPtr hdcSrc, IntPtr pptSrc, int crKey, ref BLENDFUNCTION pblend, ULW dwFlags);

		public static RECT AdjustWindowRectEx(RECT lpRect, WS dwStyle, bool bMenu, WS_EX dwExStyle)
		{
			if (!Standard.NativeMethods._AdjustWindowRectEx(ref lpRect, dwStyle, bMenu, dwExStyle))
			{
				HRESULT.ThrowLastError();
			}
			return lpRect;
		}

		public static HRESULT ChangeWindowMessageFilterEx(IntPtr hwnd, WM message, MSGFLT action, out MSGFLTINFO filterInfo)
		{
			filterInfo = MSGFLTINFO.NONE;
			if (!Utility.IsOSVistaOrNewer)
			{
				return HRESULT.S_FALSE;
			}
			if (!Utility.IsOSWindows7OrNewer)
			{
				if (Standard.NativeMethods._ChangeWindowMessageFilter(message, action))
				{
					return HRESULT.S_OK;
				}
				return (HRESULT)Win32Error.GetLastError();
			}
			CHANGEFILTERSTRUCT cHANGEFILTERSTRUCT = new CHANGEFILTERSTRUCT()
			{
				cbSize = (uint)Marshal.SizeOf(typeof(CHANGEFILTERSTRUCT))
			};
			CHANGEFILTERSTRUCT cHANGEFILTERSTRUCT1 = cHANGEFILTERSTRUCT;
			if (!Standard.NativeMethods._ChangeWindowMessageFilterEx(hwnd, message, action, ref cHANGEFILTERSTRUCT1))
			{
				return (HRESULT)Win32Error.GetLastError();
			}
			filterInfo = cHANGEFILTERSTRUCT1.ExtStatus;
			return HRESULT.S_OK;
		}

		[DllImport("gdi32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern CombineRgnResult CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, RGN fnCombineMode);

		public static string[] CommandLineToArgvW(string cmdLine)
		{
			string[] strArrays;
			IntPtr zero = IntPtr.Zero;
			try
			{
				int num = 0;
				zero = Standard.NativeMethods._CommandLineToArgvW(cmdLine, out num);
				if (zero == IntPtr.Zero)
				{
					throw new Win32Exception();
				}
				string[] stringUni = new string[num];
				for (int i = 0; i < num; i++)
				{
					IntPtr intPtr = Marshal.ReadIntPtr(zero, i * Marshal.SizeOf(typeof(IntPtr)));
					stringUni[i] = Marshal.PtrToStringUni(intPtr);
				}
				strArrays = stringUni;
			}
			finally
			{
				Standard.NativeMethods._LocalFree(zero);
			}
			return strArrays;
		}

		public static SafeHBITMAP CreateDIBSection(SafeDC hdc, ref BITMAPINFO bitmapInfo, out IntPtr ppvBits, IntPtr hSection, int dwOffset)
		{
			SafeHBITMAP safeHBITMAP = null;
			safeHBITMAP = (hdc != null ? Standard.NativeMethods._CreateDIBSection(hdc, ref bitmapInfo, 0, out ppvBits, hSection, dwOffset) : Standard.NativeMethods._CreateDIBSectionIntPtr(IntPtr.Zero, ref bitmapInfo, 0, out ppvBits, hSection, dwOffset));
			if (safeHBITMAP.IsInvalid)
			{
				HRESULT.ThrowLastError();
			}
			return safeHBITMAP;
		}

		public static IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect)
		{
			IntPtr intPtr = Standard.NativeMethods._CreateRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect);
			if (IntPtr.Zero == intPtr)
			{
				throw new Win32Exception();
			}
			return intPtr;
		}

		public static IntPtr CreateRectRgnIndirect(RECT lprc)
		{
			IntPtr intPtr = Standard.NativeMethods._CreateRectRgnIndirect(ref lprc);
			if (IntPtr.Zero == intPtr)
			{
				throw new Win32Exception();
			}
			return intPtr;
		}

		public static IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse)
		{
			IntPtr intPtr = Standard.NativeMethods._CreateRoundRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect, nWidthEllipse, nHeightEllipse);
			if (IntPtr.Zero == intPtr)
			{
				throw new Win32Exception();
			}
			return intPtr;
		}

		[DllImport("gdi32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr CreateSolidBrush(int crColor);

		public static IntPtr CreateWindowEx(WS_EX dwExStyle, string lpClassName, string lpWindowName, WS dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam)
		{
			IntPtr intPtr = Standard.NativeMethods._CreateWindowEx(dwExStyle, lpClassName, lpWindowName, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
			if (IntPtr.Zero == intPtr)
			{
				HRESULT.ThrowLastError();
			}
			return intPtr;
		}

		[DllImport("user32.dll", CharSet=CharSet.Unicode, EntryPoint="DefWindowProcW", ExactSpelling=false)]
		public static extern IntPtr DefWindowProc(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("gdi32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool DestroyIcon(IntPtr handle);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern bool DestroyWindow(IntPtr hwnd);

		public static void DrawMenuBar(IntPtr hWnd)
		{
			if (!Standard.NativeMethods._DrawMenuBar(hWnd))
			{
				throw new Win32Exception();
			}
		}

		[DllImport("dwmapi.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool DwmDefWindowProc(IntPtr hwnd, WM msg, IntPtr wParam, IntPtr lParam, out IntPtr plResult);

		[DllImport("dwmapi.dll", CharSet=CharSet.None, ExactSpelling=false, PreserveSig=false)]
		public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

		public static bool DwmGetColorizationColor(out uint pcrColorization, out bool pfOpaqueBlend)
		{
			if (Utility.IsOSVistaOrNewer && Standard.NativeMethods.IsThemeActive() && Standard.NativeMethods._DwmGetColorizationColor(out pcrColorization, out pfOpaqueBlend).Succeeded)
			{
				return true;
			}
			pcrColorization = -16777216;
			pfOpaqueBlend = true;
			return false;
		}

		public static DWM_TIMING_INFO? DwmGetCompositionTimingInfo(IntPtr hwnd)
		{
			DWM_TIMING_INFO? nullable;
			if (!Utility.IsOSVistaOrNewer)
			{
				nullable = null;
				return nullable;
			}
			DWM_TIMING_INFO dWMTIMINGINFO = new DWM_TIMING_INFO()
			{
				cbSize = Marshal.SizeOf(typeof(DWM_TIMING_INFO))
			};
			DWM_TIMING_INFO dWMTIMINGINFO1 = dWMTIMINGINFO;
			HRESULT hRESULT = Standard.NativeMethods._DwmGetCompositionTimingInfo((Utility.IsOSWindows8OrNewer ? IntPtr.Zero : hwnd), ref dWMTIMINGINFO1);
			if (hRESULT == HRESULT.E_PENDING)
			{
				nullable = null;
				return nullable;
			}
			hRESULT.ThrowIfFailed();
			return new DWM_TIMING_INFO?(dWMTIMINGINFO1);
		}

		[DllImport("dwmapi.dll", CharSet=CharSet.None, ExactSpelling=false, PreserveSig=false)]
		public static extern void DwmInvalidateIconicBitmaps(IntPtr hwnd);

		public static bool DwmIsCompositionEnabled()
		{
			if (!Utility.IsOSVistaOrNewer)
			{
				return false;
			}
			return Standard.NativeMethods._DwmIsCompositionEnabled();
		}

		[DllImport("dwmapi.dll", CharSet=CharSet.None, ExactSpelling=false, PreserveSig=false)]
		public static extern void DwmSetIconicLivePreviewBitmap(IntPtr hwnd, IntPtr hbmp, RefPOINT pptClient, DWM_SIT dwSITFlags);

		[DllImport("dwmapi.dll", CharSet=CharSet.None, ExactSpelling=false, PreserveSig=false)]
		public static extern void DwmSetIconicThumbnail(IntPtr hwnd, IntPtr hbmp, DWM_SIT dwSITFlags);

		public static void DwmSetWindowAttributeDisallowPeek(IntPtr hwnd, bool disallowPeek)
		{
			int num = (disallowPeek ? 1 : 0);
			Standard.NativeMethods._DwmSetWindowAttribute(hwnd, DWMWA.DISALLOW_PEEK, ref num, 4);
		}

		public static void DwmSetWindowAttributeFlip3DPolicy(IntPtr hwnd, DWMFLIP3D flip3dPolicy)
		{
			int num = (int)flip3dPolicy;
			Standard.NativeMethods._DwmSetWindowAttribute(hwnd, DWMWA.FLIP3D_POLICY, ref num, 4);
		}

		public static MF EnableMenuItem(IntPtr hMenu, SC uIDEnableItem, MF uEnable)
		{
			return (MF)Standard.NativeMethods._EnableMenuItem(hMenu, uIDEnableItem, uEnable);
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern bool FindClose(IntPtr handle);

		[DllImport("kernel32.dll", CharSet=CharSet.Unicode, ExactSpelling=false, SetLastError=true)]
		public static extern Standard.SafeFindHandle FindFirstFileW(string lpFileName, [In][Out] WIN32_FIND_DATAW lpFindFileData);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern bool FindNextFileW(Standard.SafeFindHandle hndFindFile, [In][Out] WIN32_FIND_DATAW lpFindFileData);

		[DllImport("gdiplus.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern Status GdipCreateBitmapFromStream(IStream stream, out IntPtr bitmap);

		[DllImport("gdiplus.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern Status GdipCreateHBITMAPFromBitmap(IntPtr bitmap, out IntPtr hbmReturn, int background);

		[DllImport("gdiplus.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern Status GdipCreateHICONFromBitmap(IntPtr bitmap, out IntPtr hbmReturn);

		[DllImport("gdiplus.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern Status GdipDisposeImage(IntPtr image);

		[DllImport("gdiplus.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern Status GdipImageForceValidation(IntPtr image);

		[DllImport("gdiplus.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern Status GdiplusShutdown(IntPtr token);

		[DllImport("gdiplus.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern Status GdiplusStartup(out IntPtr token, StartupInput input, out StartupOutput output);

		public static RECT GetClientRect(IntPtr hwnd)
		{
			RECT rECT;
			if (!Standard.NativeMethods._GetClientRect(hwnd, out rECT))
			{
				HRESULT.ThrowLastError();
			}
			return rECT;
		}

		[DllImport("shell32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern HRESULT GetCurrentProcessExplicitAppUserModelID(out string AppID);

		public static void GetCurrentThemeName(out string themeFileName, out string color, out string size)
		{
			StringBuilder stringBuilder = new StringBuilder(260);
			StringBuilder stringBuilder1 = new StringBuilder(260);
			StringBuilder stringBuilder2 = new StringBuilder(260);
			HRESULT hRESULT = Standard.NativeMethods._GetCurrentThemeName(stringBuilder, stringBuilder.Capacity, stringBuilder1, stringBuilder1.Capacity, stringBuilder2, stringBuilder2.Capacity);
			hRESULT.ThrowIfFailed();
			themeFileName = stringBuilder.ToString();
			color = stringBuilder1.ToString();
			size = stringBuilder2.ToString();
		}

		[Obsolete("Use SafeDC.GetDC instead.", true)]
		public static void GetDC()
		{
		}

		[DllImport("gdi32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int GetDeviceCaps(SafeDC hdc, DeviceCap nIndex);

		public static string GetModuleFileName(IntPtr hModule)
		{
			StringBuilder stringBuilder = new StringBuilder(260);
			while (true)
			{
				int num = Standard.NativeMethods._GetModuleFileName(hModule, stringBuilder, stringBuilder.Capacity);
				if (num == 0)
				{
					HRESULT.ThrowLastError();
				}
				if (num != stringBuilder.Capacity)
				{
					break;
				}
				stringBuilder.EnsureCapacity(stringBuilder.Capacity * 2);
			}
			return stringBuilder.ToString();
		}

		public static IntPtr GetModuleHandle(string lpModuleName)
		{
			IntPtr intPtr = Standard.NativeMethods._GetModuleHandle(lpModuleName);
			if (intPtr == IntPtr.Zero)
			{
				HRESULT.ThrowLastError();
			}
			return intPtr;
		}

		public static MONITORINFO GetMonitorInfo(IntPtr hMonitor)
		{
			MONITORINFO mONITORINFO = new MONITORINFO();
			if (!Standard.NativeMethods._GetMonitorInfo(hMonitor, mONITORINFO))
			{
				throw new Win32Exception();
			}
			return mONITORINFO;
		}

		public static IntPtr GetStockObject(StockObject fnObject)
		{
			return Standard.NativeMethods._GetStockObject(fnObject);
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int GetSystemMetrics(SM nIndex);

		public static IntPtr GetWindowLongPtr(IntPtr hwnd, GWL nIndex)
		{
			IntPtr zero = IntPtr.Zero;
			zero = (8 != IntPtr.Size ? new IntPtr(Standard.NativeMethods.GetWindowLongPtr32(hwnd, nIndex)) : Standard.NativeMethods.GetWindowLongPtr64(hwnd, nIndex));
			if (IntPtr.Zero == zero)
			{
				throw new Win32Exception();
			}
			return zero;
		}

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="GetWindowLong", ExactSpelling=false, SetLastError=true)]
		private static extern int GetWindowLongPtr32(IntPtr hWnd, GWL nIndex);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="GetWindowLongPtr", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, GWL nIndex);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool GetWindowPlacement(IntPtr hwnd, WINDOWPLACEMENT lpwndpl);

		public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd)
		{
			WINDOWPLACEMENT wINDOWPLACEMENT = new WINDOWPLACEMENT();
			if (!Standard.NativeMethods.GetWindowPlacement(hwnd, wINDOWPLACEMENT))
			{
				throw new Win32Exception();
			}
			return wINDOWPLACEMENT;
		}

		public static RECT GetWindowRect(IntPtr hwnd)
		{
			RECT rECT;
			if (!Standard.NativeMethods._GetWindowRect(hwnd, out rECT))
			{
				HRESULT.ThrowLastError();
			}
			return rECT;
		}

		[DllImport("uxtheme.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool IsThemeActive();

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool IsWindow(IntPtr hwnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool IsWindowVisible(IntPtr hwnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

		public static void PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam)
		{
			if (!Standard.NativeMethods._PostMessage(hWnd, Msg, wParam, lParam))
			{
				throw new Win32Exception();
			}
		}

		public static short RegisterClassEx(ref WNDCLASSEX lpwcx)
		{
			short num = Standard.NativeMethods._RegisterClassEx(ref lpwcx);
			if (num == 0)
			{
				HRESULT.ThrowLastError();
			}
			return num;
		}

		public static WM RegisterWindowMessage(string lpString)
		{
			uint num = Standard.NativeMethods._RegisterWindowMessage(lpString);
			if (num == 0)
			{
				HRESULT.ThrowLastError();
			}
			return (WM)num;
		}

		public static void RemoveMenu(IntPtr hMenu, SC uPosition, MF uFlags)
		{
			if (!Standard.NativeMethods._RemoveMenu(hMenu, (uint)uPosition, (uint)uFlags))
			{
				throw new Win32Exception();
			}
		}

		public static IntPtr SelectObject(SafeDC hdc, IntPtr hgdiobj)
		{
			IntPtr intPtr = Standard.NativeMethods._SelectObject(hdc, hgdiobj);
			if (intPtr == IntPtr.Zero)
			{
				HRESULT.ThrowLastError();
			}
			return intPtr;
		}

		public static IntPtr SelectObject(SafeDC hdc, SafeHBITMAP hgdiobj)
		{
			IntPtr intPtr = Standard.NativeMethods._SelectObjectSafeHBITMAP(hdc, hgdiobj);
			if (intPtr == IntPtr.Zero)
			{
				HRESULT.ThrowLastError();
			}
			return intPtr;
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr SendMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

		public static IntPtr SetActiveWindow(IntPtr hwnd)
		{
			Verify.IsNotDefault<IntPtr>(hwnd, "hwnd");
			IntPtr intPtr = Standard.NativeMethods._SetActiveWindow(hwnd);
			if (intPtr == IntPtr.Zero)
			{
				HRESULT.ThrowLastError();
			}
			return intPtr;
		}

		public static IntPtr SetClassLongPtr(IntPtr hwnd, GCLP nIndex, IntPtr dwNewLong)
		{
			if (8 == IntPtr.Size)
			{
				return Standard.NativeMethods.SetClassLongPtr64(hwnd, nIndex, dwNewLong);
			}
			return new IntPtr(Standard.NativeMethods.SetClassLongPtr32(hwnd, nIndex, dwNewLong.ToInt32()));
		}

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="SetClassLong", ExactSpelling=false, SetLastError=true)]
		private static extern int SetClassLongPtr32(IntPtr hWnd, GCLP nIndex, int dwNewLong);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="SetClassLongPtr", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr SetClassLongPtr64(IntPtr hWnd, GCLP nIndex, IntPtr dwNewLong);

		[DllImport("shell32.dll", CharSet=CharSet.None, ExactSpelling=false, PreserveSig=false)]
		public static extern void SetCurrentProcessExplicitAppUserModelID(string AppID);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern ErrorModes SetErrorMode(ErrorModes newMode);

		public static void SetProcessWorkingSetSize(IntPtr hProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize)
		{
			if (!Standard.NativeMethods._SetProcessWorkingSetSize(hProcess, new IntPtr(dwMinimumWorkingSetSize), new IntPtr(dwMaximumWorkingSetSize)))
			{
				throw new Win32Exception();
			}
		}

		public static IntPtr SetWindowLongPtr(IntPtr hwnd, GWL nIndex, IntPtr dwNewLong)
		{
			if (8 == IntPtr.Size)
			{
				return Standard.NativeMethods.SetWindowLongPtr64(hwnd, nIndex, dwNewLong);
			}
			return new IntPtr(Standard.NativeMethods.SetWindowLongPtr32(hwnd, nIndex, dwNewLong.ToInt32()));
		}

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="SetWindowLong", ExactSpelling=false, SetLastError=true)]
		private static extern int SetWindowLongPtr32(IntPtr hWnd, GWL nIndex, int dwNewLong);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="SetWindowLongPtr", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

		public static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP uFlags)
		{
			if (!Standard.NativeMethods._SetWindowPos(hWnd, hWndInsertAfter, x, y, cx, cy, uFlags))
			{
				return false;
			}
			return true;
		}

		public static void SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw)
		{
			if (Standard.NativeMethods._SetWindowRgn(hWnd, hRgn, bRedraw) == 0)
			{
				throw new Win32Exception();
			}
		}

		[DllImport("uxtheme.dll", CharSet=CharSet.None, ExactSpelling=false, PreserveSig=false)]
		public static extern void SetWindowThemeAttribute([In] IntPtr hwnd, [In] WINDOWTHEMEATTRIBUTETYPE eAttribute, [In] ref WTA_OPTIONS pvAttribute, [In] uint cbAttribute);

		public static void SHAddToRecentDocs(string path)
		{
			Standard.NativeMethods._SHAddToRecentDocs_String(SHARD.PATHW, path);
		}

		public static void SHAddToRecentDocs(IShellLinkW shellLink)
		{
			Standard.NativeMethods._SHAddToRecentDocs_ShellLink(SHARD.LINK, shellLink);
		}

		[DllImport("shell32.dll", CharSet=CharSet.None, ExactSpelling=false, PreserveSig=false)]
		public static extern HRESULT SHCreateItemFromParsingName(string pszPath, IBindCtx pbc, [In] ref Guid riid, out object ppv);

		[DllImport("shell32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool Shell_NotifyIcon(NIM dwMessage, [In] NOTIFYICONDATA lpdata);

		[DllImport("shell32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern Win32Error SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);

		[DllImport("shell32.dll", CharSet=CharSet.None, ExactSpelling=false, PreserveSig=false)]
		public static extern void SHGetItemFromDataObject(IDataObject pdtobj, DOGIF dwFlags, [In] ref Guid riid, out object ppv);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool ShowWindow(IntPtr hwnd, SW nCmdShow);

		public static HIGHCONTRAST SystemParameterInfo_GetHIGHCONTRAST()
		{
			HIGHCONTRAST hIGHCONTRAST = new HIGHCONTRAST()
			{
				cbSize = Marshal.SizeOf(typeof(HIGHCONTRAST))
			};
			HIGHCONTRAST hIGHCONTRAST1 = hIGHCONTRAST;
			if (!Standard.NativeMethods._SystemParametersInfo_HIGHCONTRAST(SPI.GETHIGHCONTRAST, hIGHCONTRAST1.cbSize, ref hIGHCONTRAST1, SPIF.None))
			{
				HRESULT.ThrowLastError();
			}
			return hIGHCONTRAST1;
		}

		public static NONCLIENTMETRICS SystemParameterInfo_GetNONCLIENTMETRICS()
		{
			NONCLIENTMETRICS nONCLIENTMETRIC = (Utility.IsOSVistaOrNewer ? NONCLIENTMETRICS.VistaMetricsStruct : NONCLIENTMETRICS.XPMetricsStruct);
			if (!Standard.NativeMethods._SystemParametersInfo_NONCLIENTMETRICS(SPI.GETNONCLIENTMETRICS, nONCLIENTMETRIC.cbSize, ref nONCLIENTMETRIC, SPIF.None))
			{
				HRESULT.ThrowLastError();
			}
			return nONCLIENTMETRIC;
		}

		public static void SystemParametersInfo(SPI uiAction, int uiParam, string pvParam, SPIF fWinIni)
		{
			if (!Standard.NativeMethods._SystemParametersInfo_String(uiAction, uiParam, pvParam, fWinIni))
			{
				HRESULT.ThrowLastError();
			}
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern uint TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

		public static void UnregisterClass(short atom, IntPtr hinstance)
		{
			if (!Standard.NativeMethods._UnregisterClassAtom(new IntPtr(atom), hinstance))
			{
				HRESULT.ThrowLastError();
			}
		}

		public static void UnregisterClass(string lpClassName, IntPtr hInstance)
		{
			if (!Standard.NativeMethods._UnregisterClassName(lpClassName, hInstance))
			{
				HRESULT.ThrowLastError();
			}
		}

		public static void UpdateLayeredWindow(IntPtr hwnd, SafeDC hdcDst, ref POINT pptDst, ref SIZE psize, SafeDC hdcSrc, ref POINT pptSrc, int crKey, ref BLENDFUNCTION pblend, ULW dwFlags)
		{
			if (!Standard.NativeMethods._UpdateLayeredWindow(hwnd, hdcDst, ref pptDst, ref psize, hdcSrc, ref pptSrc, crKey, ref pblend, dwFlags))
			{
				HRESULT.ThrowLastError();
			}
		}

		public static void UpdateLayeredWindow(IntPtr hwnd, int crKey, ref BLENDFUNCTION pblend, ULW dwFlags)
		{
			if (!Standard.NativeMethods._UpdateLayeredWindowIntPtr(hwnd, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, crKey, ref pblend, dwFlags))
			{
				HRESULT.ThrowLastError();
			}
		}
	}
}