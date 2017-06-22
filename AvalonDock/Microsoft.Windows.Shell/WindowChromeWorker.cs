using Standard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Windows.Shell
{
	internal class WindowChromeWorker : DependencyObject
	{
		private const SWP _SwpFlags = SWP.DRAWFRAME | SWP.FRAMECHANGED | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.NOREPOSITION | SWP.NOSIZE | SWP.NOZORDER;

		private readonly List<KeyValuePair<WM, MessageHandler>> _messageTable;

		private Window _window;

		private IntPtr _hwnd;

		private HwndSource _hwndSource;

		private bool _isHooked;

		private bool _isFixedUp;

		private bool _isUserResizing;

		private bool _hasUserMovedWindow;

		private Point _windowPosAtStartOfUserMove;

		private int _blackGlassFixupAttemptCount;

		private WindowChrome _chromeInfo;

		private WindowState _lastRoundingState;

		private WindowState _lastMenuState;

		private bool _isGlassEnabled;

		public readonly static DependencyProperty WindowChromeWorkerProperty;

		private readonly static HT[,] _HitTestBorders;

		private bool _IsWindowDocked
		{
			get
			{
				if (this._window.WindowState != WindowState.Normal)
				{
					return false;
				}
				RECT rECT = new RECT()
				{
					Bottom = 100,
					Right = 100
				};
				RECT rECT1 = this._GetAdjustedWindowRect(rECT);
				Point point = new Point(this._window.Left, this._window.Top);
				point = point - (Vector)DpiHelper.DevicePixelsToLogical(new Point((double)rECT1.Left, (double)rECT1.Top));
				return this._window.RestoreBounds.Location != point;
			}
		}

		static WindowChromeWorker()
		{
			WindowChromeWorker.WindowChromeWorkerProperty = DependencyProperty.RegisterAttached("WindowChromeWorker", typeof(WindowChromeWorker), typeof(WindowChromeWorker), new PropertyMetadata(null, new PropertyChangedCallback(WindowChromeWorker._OnChromeWorkerChanged)));
			HT[,] hTArray = new HT[3, 3];
			hTArray[0, 0] = HT.TOPLEFT;
			hTArray[0, 1] = HT.TOP;
			hTArray[0, 2] = HT.TOPRIGHT;
			hTArray[1, 0] = HT.LEFT;
			hTArray[1, 1] = HT.CLIENT;
			hTArray[1, 2] = HT.RIGHT;
			hTArray[2, 0] = HT.BOTTOMLEFT;
			hTArray[2, 1] = HT.BOTTOM;
			hTArray[2, 2] = HT.BOTTOMRIGHT;
			WindowChromeWorker._HitTestBorders = hTArray;
		}

		public WindowChromeWorker()
		{
			this._messageTable = new List<KeyValuePair<WM, MessageHandler>>()
			{
				new KeyValuePair<WM, MessageHandler>(WM.SETTEXT, new MessageHandler(this._HandleSetTextOrIcon)),
				new KeyValuePair<WM, MessageHandler>(WM.SETICON, new MessageHandler(this._HandleSetTextOrIcon)),
				new KeyValuePair<WM, MessageHandler>(WM.NCACTIVATE, new MessageHandler(this._HandleNCActivate)),
				new KeyValuePair<WM, MessageHandler>(WM.NCCALCSIZE, new MessageHandler(this._HandleNCCalcSize)),
				new KeyValuePair<WM, MessageHandler>(WM.NCHITTEST, new MessageHandler(this._HandleNCHitTest)),
				new KeyValuePair<WM, MessageHandler>(WM.NCRBUTTONUP, new MessageHandler(this._HandleNCRButtonUp)),
				new KeyValuePair<WM, MessageHandler>(WM.SIZE, new MessageHandler(this._HandleSize)),
				new KeyValuePair<WM, MessageHandler>(WM.WINDOWPOSCHANGED, new MessageHandler(this._HandleWindowPosChanged)),
				new KeyValuePair<WM, MessageHandler>(WM.DWMCOMPOSITIONCHANGED, new MessageHandler(this._HandleDwmCompositionChanged))
			};
			if (Utility.IsPresentationFrameworkVersionLessThan4)
			{
				this._messageTable.AddRange((new KeyValuePair<WM, MessageHandler>[] { new KeyValuePair<WM, MessageHandler>(WM.WININICHANGE, new MessageHandler(this._HandleSettingChange)), new KeyValuePair<WM, MessageHandler>(WM.ENTERSIZEMOVE, new MessageHandler(this._HandleEnterSizeMove)), new KeyValuePair<WM, MessageHandler>(WM.EXITSIZEMOVE, new MessageHandler(this._HandleExitSizeMove)), new KeyValuePair<WM, MessageHandler>(WM.MOVE, new MessageHandler(this._HandleMove)) }));
			}
		}

		private void _ApplyNewCustomChrome()
		{
			if (this._hwnd == IntPtr.Zero)
			{
				return;
			}
			if (this._chromeInfo == null)
			{
				this._RestoreStandardChromeState(false);
				return;
			}
			if (!this._isHooked)
			{
				this._hwndSource.AddHook(new HwndSourceHook(this._WndProc));
				this._isHooked = true;
			}
			this._FixupFrameworkIssues();
			this._UpdateSystemMenu(new WindowState?(this._window.WindowState));
			this._UpdateFrameState(true);
			Standard.NativeMethods.SetWindowPos(this._hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP.DRAWFRAME | SWP.FRAMECHANGED | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.NOREPOSITION | SWP.NOSIZE | SWP.NOZORDER);
		}

		private void _ClearRoundingRegion()
		{
			Standard.NativeMethods.SetWindowRgn(this._hwnd, IntPtr.Zero, Standard.NativeMethods.IsWindowVisible(this._hwnd));
		}

		private static void _CreateAndCombineRoundRectRgn(IntPtr hrgnSource, Rect region, double radius)
		{
			IntPtr zero = IntPtr.Zero;
			try
			{
				zero = WindowChromeWorker._CreateRoundRectRgn(region, radius);
				if (Standard.NativeMethods.CombineRgn(hrgnSource, hrgnSource, zero, RGN.OR) == CombineRgnResult.ERROR)
				{
					throw new InvalidOperationException("Unable to combine two HRGNs.");
				}
			}
			catch
			{
				Utility.SafeDeleteObject(ref zero);
				throw;
			}
		}

		private static IntPtr _CreateRoundRectRgn(Rect region, double radius)
		{
			if (DoubleUtilities.AreClose(0, radius))
			{
				return Standard.NativeMethods.CreateRectRgn((int)Math.Floor(region.Left), (int)Math.Floor(region.Top), (int)Math.Ceiling(region.Right), (int)Math.Ceiling(region.Bottom));
			}
			return Standard.NativeMethods.CreateRoundRectRgn((int)Math.Floor(region.Left), (int)Math.Floor(region.Top), (int)Math.Ceiling(region.Right) + 1, (int)Math.Ceiling(region.Bottom) + 1, (int)Math.Ceiling(radius), (int)Math.Ceiling(radius));
		}

		private void _ExtendGlassFrame()
		{
			if (!Utility.IsOSVistaOrNewer)
			{
				return;
			}
			if (IntPtr.Zero == this._hwnd)
			{
				return;
			}
			if (!Standard.NativeMethods.DwmIsCompositionEnabled())
			{
				this._hwndSource.CompositionTarget.BackgroundColor = SystemColors.WindowColor;
				return;
			}
			this._hwndSource.CompositionTarget.BackgroundColor = Colors.Transparent;
			double left = this._chromeInfo.GlassFrameThickness.Left;
			Thickness glassFrameThickness = this._chromeInfo.GlassFrameThickness;
			Point device = DpiHelper.LogicalPixelsToDevice(new Point(left, glassFrameThickness.Top));
			double right = this._chromeInfo.GlassFrameThickness.Right;
			glassFrameThickness = this._chromeInfo.GlassFrameThickness;
			Point point = DpiHelper.LogicalPixelsToDevice(new Point(right, glassFrameThickness.Bottom));
			MARGINS mARGIN = new MARGINS()
			{
				cxLeftWidth = (int)Math.Ceiling(device.X),
				cxRightWidth = (int)Math.Ceiling(point.X),
				cyTopHeight = (int)Math.Ceiling(device.Y),
				cyBottomHeight = (int)Math.Ceiling(point.Y)
			};
			MARGINS mARGIN1 = mARGIN;
			Standard.NativeMethods.DwmExtendFrameIntoClientArea(this._hwnd, ref mARGIN1);
		}

		private void _FixupFrameworkIssues()
		{
			if (!Utility.IsPresentationFrameworkVersionLessThan4)
			{
				return;
			}
			if (this._window.Template == null)
			{
				return;
			}
			if (VisualTreeHelper.GetChildrenCount(this._window) == 0)
			{
				this._window.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new WindowChromeWorker._Action(this._FixupFrameworkIssues));
				return;
			}
			FrameworkElement child = (FrameworkElement)VisualTreeHelper.GetChild(this._window, 0);
			RECT windowRect = Standard.NativeMethods.GetWindowRect(this._hwnd);
			RECT rECT = this._GetAdjustedWindowRect(windowRect);
			Rect logical = DpiHelper.DeviceRectToLogical(new Rect((double)windowRect.Left, (double)windowRect.Top, (double)windowRect.Width, (double)windowRect.Height));
			Rect rect = DpiHelper.DeviceRectToLogical(new Rect((double)rECT.Left, (double)rECT.Top, (double)rECT.Width, (double)rECT.Height));
			Thickness thickness = new Thickness(logical.Left - rect.Left, logical.Top - rect.Top, rect.Right - logical.Right, rect.Bottom - logical.Bottom);
			if (child != null)
			{
				child.Margin = new Thickness(0, 0, -(thickness.Left + thickness.Right), -(thickness.Top + thickness.Bottom));
			}
			if (child != null)
			{
				if (this._window.FlowDirection != FlowDirection.RightToLeft)
				{
					child.RenderTransform = null;
				}
				else
				{
					child.RenderTransform = new MatrixTransform(1, 0, 0, 1, -(thickness.Left + thickness.Right), 0);
				}
			}
			if (!this._isFixedUp)
			{
				this._hasUserMovedWindow = false;
				this._window.StateChanged += new EventHandler(this._FixupRestoreBounds);
				this._isFixedUp = true;
			}
		}

		private void _FixupRestoreBounds(object sender, EventArgs e)
		{
			if ((this._window.WindowState == WindowState.Maximized || this._window.WindowState == WindowState.Minimized) && this._hasUserMovedWindow)
			{
				this._hasUserMovedWindow = false;
				WINDOWPLACEMENT windowPlacement = Standard.NativeMethods.GetWindowPlacement(this._hwnd);
				RECT rECT = new RECT()
				{
					Bottom = 100,
					Right = 100
				};
				RECT rECT1 = this._GetAdjustedWindowRect(rECT);
				Point logical = DpiHelper.DevicePixelsToLogical(new Point((double)(windowPlacement.rcNormalPosition.Left - rECT1.Left), (double)(windowPlacement.rcNormalPosition.Top - rECT1.Top)));
				this._window.Top = logical.Y;
				this._window.Left = logical.X;
			}
		}

		private void _FixupWindows7Issues()
		{
			if (this._blackGlassFixupAttemptCount > 5)
			{
				return;
			}
			if (Utility.IsOSWindows7OrNewer && Standard.NativeMethods.DwmIsCompositionEnabled())
			{
				this._blackGlassFixupAttemptCount = this._blackGlassFixupAttemptCount + 1;
				bool hasValue = false;
				try
				{
					hasValue = Standard.NativeMethods.DwmGetCompositionTimingInfo(this._hwnd).HasValue;
				}
				catch (Exception exception)
				{
				}
				if (!hasValue)
				{
					base.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new WindowChromeWorker._Action(this._FixupWindows7Issues));
					return;
				}
				this._blackGlassFixupAttemptCount = 0;
			}
		}

		private RECT _GetAdjustedWindowRect(RECT rcWindow)
		{
			WS windowLongPtr = (WS)((int)Standard.NativeMethods.GetWindowLongPtr(this._hwnd, GWL.STYLE));
			WS_EX wSEX = (WS_EX)((int)Standard.NativeMethods.GetWindowLongPtr(this._hwnd, GWL.EXSTYLE));
			return Standard.NativeMethods.AdjustWindowRectEx(rcWindow, windowLongPtr, false, wSEX);
		}

		private WindowState _GetHwndState()
		{
			SW windowPlacement = Standard.NativeMethods.GetWindowPlacement(this._hwnd).showCmd;
			if (windowPlacement == SW.SHOWMINIMIZED)
			{
				return WindowState.Minimized;
			}
			if (windowPlacement == SW.SHOWMAXIMIZED)
			{
				return WindowState.Maximized;
			}
			return WindowState.Normal;
		}

		private Rect _GetWindowRect()
		{
			RECT windowRect = Standard.NativeMethods.GetWindowRect(this._hwnd);
			return new Rect((double)windowRect.Left, (double)windowRect.Top, (double)windowRect.Width, (double)windowRect.Height);
		}

		private IntPtr _HandleDwmCompositionChanged(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			this._UpdateFrameState(false);
			handled = false;
			return IntPtr.Zero;
		}

		private IntPtr _HandleEnterSizeMove(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			this._isUserResizing = true;
			if (this._window.WindowState != WindowState.Maximized && !this._IsWindowDocked)
			{
				this._windowPosAtStartOfUserMove = new Point(this._window.Left, this._window.Top);
			}
			handled = false;
			return IntPtr.Zero;
		}

		private IntPtr _HandleExitSizeMove(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			this._isUserResizing = false;
			if (this._window.WindowState == WindowState.Maximized)
			{
				this._window.Top = this._windowPosAtStartOfUserMove.Y;
				this._window.Left = this._windowPosAtStartOfUserMove.X;
			}
			handled = false;
			return IntPtr.Zero;
		}

		private IntPtr _HandleMove(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			if (this._isUserResizing)
			{
				this._hasUserMovedWindow = true;
			}
			handled = false;
			return IntPtr.Zero;
		}

		private IntPtr _HandleNCActivate(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			handled = true;
			return Standard.NativeMethods.DefWindowProc(this._hwnd, WM.NCACTIVATE, wParam, new IntPtr(-1));
		}

		private IntPtr _HandleNCCalcSize(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			handled = true;
			return new IntPtr(768);
		}

		private IntPtr _HandleNCHitTest(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			IntPtr zero = IntPtr.Zero;
			handled = false;
			if (Utility.IsOSVistaOrNewer)
			{
				if (this._chromeInfo.GlassFrameThickness != new Thickness() && this._isGlassEnabled)
				{
					handled = Standard.NativeMethods.DwmDefWindowProc(this._hwnd, uMsg, wParam, lParam, out zero);
				}
			}
			if (IntPtr.Zero == zero)
			{
				Point point = new Point((double)Utility.GET_X_LPARAM(lParam), (double)Utility.GET_Y_LPARAM(lParam));
				Rect rect = this._GetWindowRect();
				HT hT = this._HitTestNca(DpiHelper.DeviceRectToLogical(rect), DpiHelper.DevicePixelsToLogical(point));
				if (hT != HT.CLIENT)
				{
					Point logical = point;
					logical.Offset(-rect.X, -rect.Y);
					logical = DpiHelper.DevicePixelsToLogical(logical);
					IInputElement inputElement = this._window.InputHitTest(logical);
					if (inputElement != null && WindowChrome.GetIsHitTestVisibleInChrome(inputElement))
					{
						hT = HT.CLIENT;
					}
				}
				handled = true;
				zero = new IntPtr((int)hT);
			}
			return zero;
		}

		private IntPtr _HandleNCRButtonUp(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			if (2 == wParam.ToInt32())
			{
				if (this._window.ContextMenu != null)
				{
					this._window.ContextMenu.Placement = PlacementMode.MousePoint;
					this._window.ContextMenu.IsOpen = true;
				}
				else if (WindowChrome.GetWindowChrome(this._window).ShowSystemMenu)
				{
					Microsoft.Windows.Shell.SystemCommands.ShowSystemMenuPhysicalCoordinates(this._window, new Point((double)Utility.GET_X_LPARAM(lParam), (double)Utility.GET_Y_LPARAM(lParam)));
				}
			}
			handled = false;
			return IntPtr.Zero;
		}

		private IntPtr _HandleSetTextOrIcon(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			bool flag = this._ModifyStyle(WS.VISIBLE, WS.OVERLAPPED);
			IntPtr intPtr = Standard.NativeMethods.DefWindowProc(this._hwnd, uMsg, wParam, lParam);
			if (flag)
			{
				this._ModifyStyle(WS.OVERLAPPED, WS.VISIBLE);
			}
			handled = true;
			return intPtr;
		}

		private IntPtr _HandleSettingChange(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			this._FixupFrameworkIssues();
			handled = false;
			return IntPtr.Zero;
		}

		private IntPtr _HandleSize(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			WindowState? nullable = null;
			if (wParam.ToInt32() == 2)
			{
				nullable = new WindowState?(WindowState.Maximized);
			}
			this._UpdateSystemMenu(nullable);
			handled = false;
			return IntPtr.Zero;
		}

		private IntPtr _HandleWindowPosChanged(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			this._UpdateSystemMenu(null);
			if (!this._isGlassEnabled)
			{
				WINDOWPOS structure = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
				this._SetRoundingRegion(new WINDOWPOS?(structure));
			}
			handled = false;
			return IntPtr.Zero;
		}

		private HT _HitTestNca(Rect windowPosition, Point mousePosition)
		{
			int num = 1;
			int num1 = 1;
			bool top = false;
			if (mousePosition.Y >= windowPosition.Top && mousePosition.Y < windowPosition.Top + this._chromeInfo.ResizeBorderThickness.Top + this._chromeInfo.CaptionHeight)
			{
				double y = mousePosition.Y;
				double top1 = windowPosition.Top;
				Thickness resizeBorderThickness = this._chromeInfo.ResizeBorderThickness;
				top = y < top1 + resizeBorderThickness.Top;
				num = 0;
			}
			else if (mousePosition.Y < windowPosition.Bottom && mousePosition.Y >= windowPosition.Bottom - (double)((int)this._chromeInfo.ResizeBorderThickness.Bottom))
			{
				num = 2;
			}
			if (mousePosition.X >= windowPosition.Left && mousePosition.X < windowPosition.Left + (double)((int)this._chromeInfo.ResizeBorderThickness.Left))
			{
				num1 = 0;
			}
			else if (mousePosition.X < windowPosition.Right && mousePosition.X >= windowPosition.Right - this._chromeInfo.ResizeBorderThickness.Right)
			{
				num1 = 2;
			}
			if (num == 0 && num1 != 1 && !top)
			{
				num = 1;
			}
			HT hT = WindowChromeWorker._HitTestBorders[num, num1];
			if (hT == HT.TOP && !top)
			{
				hT = HT.CAPTION;
			}
			return hT;
		}

		private static bool _IsUniform(CornerRadius cornerRadius)
		{
			if (!DoubleUtilities.AreClose(cornerRadius.BottomLeft, cornerRadius.BottomRight))
			{
				return false;
			}
			if (!DoubleUtilities.AreClose(cornerRadius.TopLeft, cornerRadius.TopRight))
			{
				return false;
			}
			if (!DoubleUtilities.AreClose(cornerRadius.BottomLeft, cornerRadius.TopRight))
			{
				return false;
			}
			return true;
		}

		private bool _ModifyStyle(WS removeStyle, WS addStyle)
		{
			IntPtr windowLongPtr = Standard.NativeMethods.GetWindowLongPtr(this._hwnd, GWL.STYLE);
			int num = windowLongPtr.ToInt32();
			WS w = (WS)(num & (int)(~removeStyle) | (int)addStyle);
			if (num == (int)w)
			{
				return false;
			}
			Standard.NativeMethods.SetWindowLongPtr(this._hwnd, GWL.STYLE, new IntPtr((int)w));
			return true;
		}

		private void _OnChromePropertyChangedThatRequiresRepaint(object sender, EventArgs e)
		{
			this._UpdateFrameState(true);
		}

		private static void _OnChromeWorkerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Window window = (Window)d;
			((WindowChromeWorker)e.NewValue)._SetWindow(window);
		}

		private void _OnWindowPropertyChangedThatRequiresTemplateFixup(object sender, EventArgs e)
		{
			if (this._chromeInfo != null && this._hwnd != IntPtr.Zero)
			{
				this._window.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new WindowChromeWorker._Action(this._FixupFrameworkIssues));
			}
		}

		private void _RestoreFrameworkIssueFixups()
		{
			if (Utility.IsPresentationFrameworkVersionLessThan4)
			{
				FrameworkElement child = (FrameworkElement)VisualTreeHelper.GetChild(this._window, 0);
				if (child != null)
				{
					child.Margin = new Thickness();
				}
				this._window.StateChanged -= new EventHandler(this._FixupRestoreBounds);
				this._isFixedUp = false;
			}
		}

		private void _RestoreGlassFrame()
		{
			if (!Utility.IsOSVistaOrNewer || this._hwnd == IntPtr.Zero)
			{
				return;
			}
			this._hwndSource.CompositionTarget.BackgroundColor = SystemColors.WindowColor;
			if (Standard.NativeMethods.DwmIsCompositionEnabled())
			{
				MARGINS mARGIN = new MARGINS();
				Standard.NativeMethods.DwmExtendFrameIntoClientArea(this._hwnd, ref mARGIN);
			}
		}

		private void _RestoreHrgn()
		{
			this._ClearRoundingRegion();
			Standard.NativeMethods.SetWindowPos(this._hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP.DRAWFRAME | SWP.FRAMECHANGED | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.NOREPOSITION | SWP.NOSIZE | SWP.NOZORDER);
		}

		private void _RestoreStandardChromeState(bool isClosing)
		{
			base.VerifyAccess();
			this._UnhookCustomChrome();
			if (!isClosing)
			{
				this._RestoreFrameworkIssueFixups();
				this._RestoreGlassFrame();
				this._RestoreHrgn();
				this._window.InvalidateMeasure();
			}
		}

		private void _SetRoundingRegion(WINDOWPOS? wp)
		{
			int left;
			int top;
			Size size;
			if (Standard.NativeMethods.GetWindowPlacement(this._hwnd).showCmd != SW.SHOWMAXIMIZED)
			{
				if (!wp.HasValue || Utility.IsFlagSet(wp.Value.flags, 1))
				{
					if (wp.HasValue && this._lastRoundingState == this._window.WindowState)
					{
						return;
					}
					size = this._GetWindowRect().Size;
				}
				else
				{
					size = new Size((double)wp.Value.cx, (double)wp.Value.cy);
				}
				this._lastRoundingState = this._window.WindowState;
				IntPtr zero = IntPtr.Zero;
				try
				{
					double num = Math.Min(size.Width, size.Height);
					CornerRadius cornerRadius = this._chromeInfo.CornerRadius;
					Point device = DpiHelper.LogicalPixelsToDevice(new Point(cornerRadius.TopLeft, 0));
					double x = device.X;
					x = Math.Min(x, num / 2);
					if (!WindowChromeWorker._IsUniform(this._chromeInfo.CornerRadius))
					{
						zero = WindowChromeWorker._CreateRoundRectRgn(new Rect(0, 0, size.Width / 2 + x, size.Height / 2 + x), x);
						cornerRadius = this._chromeInfo.CornerRadius;
						device = DpiHelper.LogicalPixelsToDevice(new Point(cornerRadius.TopRight, 0));
						double x1 = device.X;
						x1 = Math.Min(x1, num / 2);
						Rect rect = new Rect(0, 0, size.Width / 2 + x1, size.Height / 2 + x1);
						rect.Offset(size.Width / 2 - x1, 0);
						WindowChromeWorker._CreateAndCombineRoundRectRgn(zero, rect, x1);
						cornerRadius = this._chromeInfo.CornerRadius;
						device = DpiHelper.LogicalPixelsToDevice(new Point(cornerRadius.BottomLeft, 0));
						double num1 = device.X;
						num1 = Math.Min(num1, num / 2);
						Rect rect1 = new Rect(0, 0, size.Width / 2 + num1, size.Height / 2 + num1);
						rect1.Offset(0, size.Height / 2 - num1);
						WindowChromeWorker._CreateAndCombineRoundRectRgn(zero, rect1, num1);
						cornerRadius = this._chromeInfo.CornerRadius;
						device = DpiHelper.LogicalPixelsToDevice(new Point(cornerRadius.BottomRight, 0));
						double x2 = device.X;
						x2 = Math.Min(x2, num / 2);
						Rect rect2 = new Rect(0, 0, size.Width / 2 + x2, size.Height / 2 + x2);
						rect2.Offset(size.Width / 2 - x2, size.Height / 2 - x2);
						WindowChromeWorker._CreateAndCombineRoundRectRgn(zero, rect2, x2);
					}
					else
					{
						zero = WindowChromeWorker._CreateRoundRectRgn(new Rect(size), x);
					}
					Standard.NativeMethods.SetWindowRgn(this._hwnd, zero, Standard.NativeMethods.IsWindowVisible(this._hwnd));
					zero = IntPtr.Zero;
				}
				finally
				{
					Utility.SafeDeleteObject(ref zero);
				}
			}
			else
			{
				if (!wp.HasValue)
				{
					Rect rect3 = this._GetWindowRect();
					left = (int)rect3.Left;
					top = (int)rect3.Top;
				}
				else
				{
					left = wp.Value.x;
					top = wp.Value.y;
				}
				RECT monitorInfo = Standard.NativeMethods.GetMonitorInfo(Standard.NativeMethods.MonitorFromWindow(this._hwnd, 2)).rcWork;
				monitorInfo.Offset(-left, -top);
				IntPtr intPtr = IntPtr.Zero;
				try
				{
					intPtr = Standard.NativeMethods.CreateRectRgnIndirect(monitorInfo);
					Standard.NativeMethods.SetWindowRgn(this._hwnd, intPtr, Standard.NativeMethods.IsWindowVisible(this._hwnd));
					intPtr = IntPtr.Zero;
				}
				finally
				{
					Utility.SafeDeleteObject(ref intPtr);
				}
			}
		}

		private void _SetWindow(Window window)
		{
			this._window = window;
			this._hwnd = (new WindowInteropHelper(this._window)).Handle;
			if (Utility.IsPresentationFrameworkVersionLessThan4)
			{
				Utility.AddDependencyPropertyChangeListener(this._window, Control.TemplateProperty, new EventHandler(this._OnWindowPropertyChangedThatRequiresTemplateFixup));
				Utility.AddDependencyPropertyChangeListener(this._window, FrameworkElement.FlowDirectionProperty, new EventHandler(this._OnWindowPropertyChangedThatRequiresTemplateFixup));
			}
			this._window.Closed += new EventHandler(this._UnsetWindow);
			if (IntPtr.Zero == this._hwnd)
			{
				this._window.SourceInitialized += new EventHandler((object sender, EventArgs e) => {
					this._hwnd = (new WindowInteropHelper(this._window)).Handle;
					this._hwndSource = HwndSource.FromHwnd(this._hwnd);
					if (this._chromeInfo != null)
					{
						this._ApplyNewCustomChrome();
					}
				});
			}
			else
			{
				this._hwndSource = HwndSource.FromHwnd(this._hwnd);
				this._window.ApplyTemplate();
				if (this._chromeInfo != null)
				{
					this._ApplyNewCustomChrome();
					return;
				}
			}
		}

		private void _UnhookCustomChrome()
		{
			if (this._isHooked)
			{
				this._hwndSource.RemoveHook(new HwndSourceHook(this._WndProc));
				this._isHooked = false;
			}
		}

		private void _UnsetWindow(object sender, EventArgs e)
		{
			if (Utility.IsPresentationFrameworkVersionLessThan4)
			{
				Utility.RemoveDependencyPropertyChangeListener(this._window, Control.TemplateProperty, new EventHandler(this._OnWindowPropertyChangedThatRequiresTemplateFixup));
				Utility.RemoveDependencyPropertyChangeListener(this._window, FrameworkElement.FlowDirectionProperty, new EventHandler(this._OnWindowPropertyChangedThatRequiresTemplateFixup));
			}
			if (this._chromeInfo != null)
			{
				this._chromeInfo.PropertyChangedThatRequiresRepaint -= new EventHandler(this._OnChromePropertyChangedThatRequiresRepaint);
			}
			this._RestoreStandardChromeState(true);
		}

		private void _UpdateFrameState(bool force)
		{
			if (IntPtr.Zero == this._hwnd)
			{
				return;
			}
			bool flag = Standard.NativeMethods.DwmIsCompositionEnabled();
			if (force || flag != this._isGlassEnabled)
			{
				this._isGlassEnabled = (!flag ? false : this._chromeInfo.GlassFrameThickness != new Thickness());
				if (this._isGlassEnabled)
				{
					this._ClearRoundingRegion();
					this._ExtendGlassFrame();
					this._FixupWindows7Issues();
				}
				else
				{
					this._SetRoundingRegion(null);
				}
				Standard.NativeMethods.SetWindowPos(this._hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP.DRAWFRAME | SWP.FRAMECHANGED | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.NOREPOSITION | SWP.NOSIZE | SWP.NOZORDER);
			}
		}

		private void _UpdateSystemMenu(WindowState? assumeState)
		{
			WindowState? nullable = assumeState;
			WindowState windowState = (nullable.HasValue ? nullable.GetValueOrDefault() : this._GetHwndState());
			if (assumeState.HasValue || this._lastMenuState != windowState)
			{
				this._lastMenuState = windowState;
				bool flag = this._ModifyStyle(WS.VISIBLE, WS.OVERLAPPED);
				IntPtr systemMenu = Standard.NativeMethods.GetSystemMenu(this._hwnd, false);
				if (IntPtr.Zero != systemMenu)
				{
					IntPtr windowLongPtr = Standard.NativeMethods.GetWindowLongPtr(this._hwnd, GWL.STYLE);
					int num = windowLongPtr.ToInt32();
					bool flag1 = Utility.IsFlagSet(num, 131072);
					bool flag2 = Utility.IsFlagSet(num, 65536);
					bool flag3 = Utility.IsFlagSet(num, 262144);
					if (windowState == WindowState.Minimized)
					{
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.RESTORE, MF.ENABLED);
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.MOVE, MF.GRAYED | MF.DISABLED);
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.SIZE, MF.GRAYED | MF.DISABLED);
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.MINIMIZE, MF.GRAYED | MF.DISABLED);
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.MAXIMIZE, (flag2 ? MF.ENABLED : MF.GRAYED | MF.DISABLED));
					}
					else if (windowState != WindowState.Maximized)
					{
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.RESTORE, MF.GRAYED | MF.DISABLED);
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.MOVE, MF.ENABLED);
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.SIZE, (flag3 ? MF.ENABLED : MF.GRAYED | MF.DISABLED));
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.MINIMIZE, (flag1 ? MF.ENABLED : MF.GRAYED | MF.DISABLED));
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.MAXIMIZE, (flag2 ? MF.ENABLED : MF.GRAYED | MF.DISABLED));
					}
					else
					{
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.RESTORE, MF.ENABLED);
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.MOVE, MF.GRAYED | MF.DISABLED);
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.SIZE, MF.GRAYED | MF.DISABLED);
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.MINIMIZE, (flag1 ? MF.ENABLED : MF.GRAYED | MF.DISABLED));
						Standard.NativeMethods.EnableMenuItem(systemMenu, SC.MAXIMIZE, MF.GRAYED | MF.DISABLED);
					}
				}
				if (flag)
				{
					this._ModifyStyle(WS.OVERLAPPED, WS.VISIBLE);
				}
			}
		}

		private IntPtr _WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			IntPtr value;
			WM wM = (WM)msg;
			List<KeyValuePair<WM, MessageHandler>>.Enumerator enumerator = this._messageTable.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<WM, MessageHandler> current = enumerator.Current;
					if (current.Key != wM)
					{
						continue;
					}
					value = current.Value(wM, wParam, lParam, out handled);
					return value;
				}
				return IntPtr.Zero;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return value;
		}

		public static WindowChromeWorker GetWindowChromeWorker(Window window)
		{
			Verify.IsNotNull<Window>(window, "window");
			return (WindowChromeWorker)window.GetValue(WindowChromeWorker.WindowChromeWorkerProperty);
		}

		public void SetWindowChrome(WindowChrome newChrome)
		{
			base.VerifyAccess();
			if (newChrome == this._chromeInfo)
			{
				return;
			}
			if (this._chromeInfo != null)
			{
				this._chromeInfo.PropertyChangedThatRequiresRepaint -= new EventHandler(this._OnChromePropertyChangedThatRequiresRepaint);
			}
			this._chromeInfo = newChrome;
			if (this._chromeInfo != null)
			{
				this._chromeInfo.PropertyChangedThatRequiresRepaint += new EventHandler(this._OnChromePropertyChangedThatRequiresRepaint);
			}
			this._ApplyNewCustomChrome();
		}

		public static void SetWindowChromeWorker(Window window, WindowChromeWorker chrome)
		{
			Verify.IsNotNull<Window>(window, "window");
			window.SetValue(WindowChromeWorker.WindowChromeWorkerProperty, chrome);
		}

		private delegate void _Action();
	}
}