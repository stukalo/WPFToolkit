using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal static class FocusElementManager
	{
		private static List<DockingManager> _managers;

		private static FullWeakDictionary<ILayoutElement, IInputElement> _modelFocusedElement;

		private static WeakDictionary<ILayoutElement, IntPtr> _modelFocusedWindowHandle;

		private static WeakReference _lastFocusedElement;

		private static WindowHookHandler _windowHandler;

		private static DispatcherOperation _setFocusAsyncOperation;

		private static WeakReference _lastFocusedElementBeforeEnterMenuMode;

		static FocusElementManager()
		{
			FocusElementManager._managers = new List<DockingManager>();
			FocusElementManager._modelFocusedElement = new FullWeakDictionary<ILayoutElement, IInputElement>();
			FocusElementManager._modelFocusedWindowHandle = new WeakDictionary<ILayoutElement, IntPtr>();
			FocusElementManager._windowHandler = null;
			FocusElementManager._lastFocusedElementBeforeEnterMenuMode = null;
		}

		private static void Current_Exit(object sender, ExitEventArgs e)
		{
			Application.Current.Exit -= new ExitEventHandler(FocusElementManager.Current_Exit);
			if (FocusElementManager._windowHandler != null)
			{
				FocusElementManager._windowHandler.FocusChanged -= new EventHandler<FocusChangeEventArgs>(FocusElementManager.WindowFocusChanging);
				FocusElementManager._windowHandler.Detach();
				FocusElementManager._windowHandler = null;
			}
		}

		internal static void FinalizeFocusManagement(DockingManager manager)
		{
			manager.PreviewGotKeyboardFocus -= new KeyboardFocusChangedEventHandler(FocusElementManager.manager_PreviewGotKeyboardFocus);
			FocusElementManager._managers.Remove(manager);
			if (FocusElementManager._managers.Count == 0 && FocusElementManager._windowHandler != null)
			{
				FocusElementManager._windowHandler.FocusChanged -= new EventHandler<FocusChangeEventArgs>(FocusElementManager.WindowFocusChanging);
				FocusElementManager._windowHandler.Detach();
				FocusElementManager._windowHandler = null;
			}
		}

		internal static IInputElement GetLastFocusedElement(ILayoutElement model)
		{
			IInputElement inputElement;
			if (FocusElementManager._modelFocusedElement.GetValue(model, out inputElement))
			{
				return inputElement;
			}
			return null;
		}

		internal static IntPtr GetLastWindowHandle(ILayoutElement model)
		{
			IntPtr intPtr;
			if (FocusElementManager._modelFocusedWindowHandle.GetValue(model, out intPtr))
			{
				return intPtr;
			}
			return IntPtr.Zero;
		}

		private static void InputManager_EnterMenuMode(object sender, EventArgs e)
		{
			if (Keyboard.FocusedElement == null)
			{
				return;
			}
			if ((Keyboard.FocusedElement as DependencyObject).FindLogicalAncestor<DockingManager>() == null)
			{
				FocusElementManager._lastFocusedElementBeforeEnterMenuMode = null;
				return;
			}
			FocusElementManager._lastFocusedElementBeforeEnterMenuMode = new WeakReference(Keyboard.FocusedElement);
		}

		private static void InputManager_LeaveMenuMode(object sender, EventArgs e)
		{
			if (FocusElementManager._lastFocusedElementBeforeEnterMenuMode != null && FocusElementManager._lastFocusedElementBeforeEnterMenuMode.IsAlive)
			{
				UIElement valueOrDefault = FocusElementManager._lastFocusedElementBeforeEnterMenuMode.GetValueOrDefault<UIElement>();
				if (valueOrDefault != null)
				{
					Keyboard.Focus(valueOrDefault);
				}
			}
		}

		private static void manager_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			Visual newFocus = e.NewFocus as Visual;
			if (newFocus != null && !(newFocus is LayoutAnchorableTabItem) && !(newFocus is LayoutDocumentTabItem))
			{
				LayoutAnchorableControl layoutAnchorableControl = newFocus.FindVisualAncestor<LayoutAnchorableControl>();
				if (layoutAnchorableControl != null)
				{
					FocusElementManager._modelFocusedElement[layoutAnchorableControl.Model] = e.NewFocus;
					return;
				}
				LayoutDocumentControl layoutDocumentControl = newFocus.FindVisualAncestor<LayoutDocumentControl>();
				if (layoutDocumentControl != null)
				{
					FocusElementManager._modelFocusedElement[layoutDocumentControl.Model] = e.NewFocus;
				}
			}
		}

		internal static void SetFocusOnLastElement(ILayoutElement model)
		{
			IInputElement inputElement;
			IntPtr intPtr;
			bool zero = false;
			if (FocusElementManager._modelFocusedElement.GetValue(model, out inputElement))
			{
				zero = inputElement == Keyboard.Focus(inputElement);
			}
			if (FocusElementManager._modelFocusedWindowHandle.GetValue(model, out intPtr))
			{
				zero = IntPtr.Zero != Win32Helper.SetFocus(intPtr);
			}
			if (zero)
			{
				FocusElementManager._lastFocusedElement = new WeakReference(model);
			}
		}

		internal static void SetupFocusManagement(DockingManager manager)
		{
			if (FocusElementManager._managers.Count == 0)
			{
				FocusElementManager._windowHandler = new WindowHookHandler();
				FocusElementManager._windowHandler.FocusChanged += new EventHandler<FocusChangeEventArgs>(FocusElementManager.WindowFocusChanging);
				FocusElementManager._windowHandler.Attach();
				if (Application.Current != null)
				{
					Application.Current.Exit += new ExitEventHandler(FocusElementManager.Current_Exit);
				}
			}
			manager.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(FocusElementManager.manager_PreviewGotKeyboardFocus);
			FocusElementManager._managers.Add(manager);
		}

		private static void WindowActivating(object sender, WindowActivateEventArgs e)
		{
			IntPtr intPtr;
			if (Keyboard.FocusedElement == null && FocusElementManager._lastFocusedElement != null && FocusElementManager._lastFocusedElement.IsAlive)
			{
				ILayoutElement target = FocusElementManager._lastFocusedElement.Target as ILayoutElement;
				if (target != null)
				{
					DockingManager manager = target.Root.Manager;
					if (manager == null)
					{
						return;
					}
					if (!manager.GetParentWindowHandle(out intPtr))
					{
						return;
					}
					if (e.HwndActivating != intPtr)
					{
						return;
					}
					FocusElementManager._setFocusAsyncOperation = Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => {
						try
						{
							FocusElementManager.SetFocusOnLastElement(target);
						}
						finally
						{
							FocusElementManager._setFocusAsyncOperation = null;
						}
					}), DispatcherPriority.Input, new object[0]);
				}
			}
		}

		private static void WindowFocusChanging(object sender, FocusChangeEventArgs e)
		{
			Func<HwndHost, bool> func = null;
			foreach (DockingManager _manager in FocusElementManager._managers)
			{
				IEnumerable<HwndHost> hwndHosts = _manager.FindLogicalChildren<HwndHost>();
				Func<HwndHost, bool> func1 = func;
				if (func1 == null)
				{
					Func<HwndHost, bool> func2 = (HwndHost hw) => Win32Helper.IsChild(hw.Handle, e.GotFocusWinHandle);
					Func<HwndHost, bool> func3 = func2;
					func = func2;
					func1 = func3;
				}
				HwndHost hwndHost = hwndHosts.FirstOrDefault<HwndHost>(func1);
				if (hwndHost == null)
				{
					continue;
				}
				LayoutAnchorableControl gotFocusWinHandle = hwndHost.FindVisualAncestor<LayoutAnchorableControl>();
				if (gotFocusWinHandle == null)
				{
					LayoutDocumentControl layoutDocumentControl = hwndHost.FindVisualAncestor<LayoutDocumentControl>();
					if (layoutDocumentControl == null)
					{
						continue;
					}
					FocusElementManager._modelFocusedWindowHandle[layoutDocumentControl.Model] = e.GotFocusWinHandle;
					if (layoutDocumentControl.Model == null)
					{
						continue;
					}
					layoutDocumentControl.Model.IsActive = true;
				}
				else
				{
					FocusElementManager._modelFocusedWindowHandle[gotFocusWinHandle.Model] = e.GotFocusWinHandle;
					if (gotFocusWinHandle.Model == null)
					{
						continue;
					}
					gotFocusWinHandle.Model.IsActive = true;
				}
			}
		}
	}
}