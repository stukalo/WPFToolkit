using Microsoft.Windows.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public abstract class LayoutFloatingWindowControl : Window, ILayoutControl
	{
		private ResourceDictionary currentThemeResourceDictionary;

		private ILayoutElement _model;

		private bool _attachDrag;

		private HwndSource _hwndSrc;

		private HwndSourceHook _hwndSrcHook;

		private readonly static DependencyPropertyKey IsDraggingPropertyKey;

		public readonly static DependencyProperty IsDraggingProperty;

		private DragService _dragService;

		private bool _internalCloseFlag;

		public readonly static DependencyProperty IsMaximizedProperty;

		protected bool CloseInitiatedByUser
		{
			get
			{
				return !this._internalCloseFlag;
			}
		}

		public bool IsDragging
		{
			get
			{
				return (bool)base.GetValue(LayoutFloatingWindowControl.IsDraggingProperty);
			}
		}

		public bool IsMaximized
		{
			get
			{
				return (bool)base.GetValue(LayoutFloatingWindowControl.IsMaximizedProperty);
			}
			private set
			{
				base.SetValue(LayoutFloatingWindowControl.IsMaximizedProperty, value);
				this.UpdatePositionAndSizeOfPanes();
			}
		}

		internal bool KeepContentVisibleOnClose
		{
			get;
			set;
		}

		public abstract ILayoutElement Model
		{
			get;
		}

		static LayoutFloatingWindowControl()
		{
			LayoutFloatingWindowControl.IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly("IsDragging", typeof(bool), typeof(LayoutFloatingWindowControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(LayoutFloatingWindowControl.OnIsDraggingChanged)));
			LayoutFloatingWindowControl.IsDraggingProperty = LayoutFloatingWindowControl.IsDraggingPropertyKey.DependencyProperty;
			LayoutFloatingWindowControl.IsMaximizedProperty = DependencyProperty.Register("IsMaximized", typeof(bool), typeof(LayoutFloatingWindowControl), new FrameworkPropertyMetadata(false));
			ContentControl.ContentProperty.OverrideMetadata(typeof(LayoutFloatingWindowControl), new FrameworkPropertyMetadata(null, null, new CoerceValueCallback(LayoutFloatingWindowControl.CoerceContentValue)));
			Window.AllowsTransparencyProperty.OverrideMetadata(typeof(LayoutFloatingWindowControl), new FrameworkPropertyMetadata(false));
			Window.ShowInTaskbarProperty.OverrideMetadata(typeof(LayoutFloatingWindowControl), new FrameworkPropertyMetadata(false));
		}

		protected LayoutFloatingWindowControl(ILayoutElement model)
		{
			base.Loaded += new RoutedEventHandler(this.OnLoaded);
			base.Unloaded += new RoutedEventHandler(this.OnUnloaded);
			this._model = model;
			this.UpdateThemeResources(null);
		}

		internal void AttachDrag(bool onActivated = true)
		{
			if (onActivated)
			{
				this._attachDrag = true;
				base.Activated += new EventHandler(this.OnActivated);
				return;
			}
			IntPtr handle = (new WindowInteropHelper(this)).Handle;
			IntPtr intPtr = new IntPtr((int)base.Left & 65535 | (int)base.Top << 16);
			Win32Helper.SendMessage(handle, 161, new IntPtr(2), intPtr);
		}

		private static object CoerceContentValue(DependencyObject sender, object content)
		{
			return new LayoutFloatingWindowControl.FloatingWindowContentHost(sender as LayoutFloatingWindowControl)
			{
				Content = content as UIElement
			};
		}

		protected virtual IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			bool flag;
			handled = false;
			if (msg <= 274)
			{
				if (msg != 6)
				{
					if (msg == 274)
					{
						int num = (int)wParam & 65520;
						if (num == 61488 || num == 61728)
						{
							this.UpdateMaximizedState(num == 61488);
						}
					}
				}
				else if (((int)wParam & 65535) == 0 && lParam == this.GetParentWindowHandle())
				{
					Win32Helper.SetActiveWindow(this._hwndSrc.Handle);
					handled = true;
				}
			}
			else if (msg == 514)
			{
				if (this._dragService != null && Mouse.LeftButton == MouseButtonState.Released)
				{
					this._dragService.Abort();
					this._dragService = null;
					this.SetIsDragging(false);
				}
			}
			else if (msg == 534)
			{
				this.UpdateDragPosition();
			}
			else if (msg == 562)
			{
				this.UpdatePositionAndSizeOfPanes();
				if (this._dragService != null)
				{
					Point deviceDPI = this.TransformToDeviceDPI(Win32Helper.GetMousePosition());
					this._dragService.Drop(deviceDPI, out flag);
					this._dragService = null;
					this.SetIsDragging(false);
					if (flag)
					{
						this.InternalClose();
					}
				}
			}
			return IntPtr.Zero;
		}

		internal void InternalClose()
		{
			this._internalCloseFlag = true;
			base.Close();
		}

		private void OnActivated(object sender, EventArgs e)
		{
			base.Activated -= new EventHandler(this.OnActivated);
			if (this._attachDrag && Mouse.LeftButton == MouseButtonState.Pressed)
			{
				IntPtr handle = (new WindowInteropHelper(this)).Handle;
				Point screenDPI = this.PointToScreenDPI(Mouse.GetPosition(this));
				Win32Helper.RECT clientRect = Win32Helper.GetClientRect(handle);
				Win32Helper.RECT windowRect = Win32Helper.GetWindowRect(handle);
				base.Left = screenDPI.X - (double)windowRect.Width / 2;
				base.Top = screenDPI.Y - (double)(windowRect.Height - clientRect.Height) / 2;
				this._attachDrag = false;
				IntPtr intPtr = new IntPtr((int)screenDPI.X & 65535 | (int)screenDPI.Y << 16);
				Win32Helper.SendMessage(handle, 161, new IntPtr(2), intPtr);
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			if (base.Content != null)
			{
				LayoutFloatingWindowControl.FloatingWindowContentHost content = base.Content as LayoutFloatingWindowControl.FloatingWindowContentHost;
				if (content != null)
				{
					content.Dispose();
				}
				if (this._hwndSrc != null)
				{
					this._hwndSrc.RemoveHook(this._hwndSrcHook);
					this._hwndSrc.Dispose();
					this._hwndSrc = null;
				}
			}
			base.OnClosed(e);
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.CloseWindowCommand, (object s, ExecutedRoutedEventArgs args) => Microsoft.Windows.Shell.SystemCommands.CloseWindow((Window)args.Parameter)));
			base.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MaximizeWindowCommand, (object s, ExecutedRoutedEventArgs args) => Microsoft.Windows.Shell.SystemCommands.MaximizeWindow((Window)args.Parameter)));
			base.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MinimizeWindowCommand, (object s, ExecutedRoutedEventArgs args) => Microsoft.Windows.Shell.SystemCommands.MinimizeWindow((Window)args.Parameter)));
			base.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.RestoreWindowCommand, (object s, ExecutedRoutedEventArgs args) => Microsoft.Windows.Shell.SystemCommands.RestoreWindow((Window)args.Parameter)));
			base.OnInitialized(e);
		}

		private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutFloatingWindowControl)d).OnIsDraggingChanged(e);
		}

		protected virtual void OnIsDraggingChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!(bool)e.NewValue)
			{
				base.ReleaseMouseCapture();
				return;
			}
			base.CaptureMouse();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			base.Loaded -= new RoutedEventHandler(this.OnLoaded);
			this.SetParentToMainWindowOf(this.Model.Root.Manager);
			this._hwndSrc = PresentationSource.FromDependencyObject(this) as HwndSource;
			LayoutFloatingWindowControl layoutFloatingWindowControl = this;
			this._hwndSrcHook = new HwndSourceHook(layoutFloatingWindowControl.FilterMessage);
			this._hwndSrc.AddHook(this._hwndSrcHook);
			this.UpdateMaximizedState(this.Model.Descendents().OfType<ILayoutElementForFloatingWindow>().Any<ILayoutElementForFloatingWindow>((ILayoutElementForFloatingWindow l) => l.IsMaximized));
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);
		}

		protected override void OnStateChanged(EventArgs e)
		{
			base.WindowState = (this.IsMaximized ? System.Windows.WindowState.Maximized : System.Windows.WindowState.Normal);
			base.OnStateChanged(e);
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			base.Unloaded -= new RoutedEventHandler(this.OnUnloaded);
			if (this._hwndSrc != null)
			{
				this._hwndSrc.RemoveHook(this._hwndSrcHook);
				this.InternalClose();
			}
		}

		protected void SetIsDragging(bool value)
		{
			base.SetValue(LayoutFloatingWindowControl.IsDraggingPropertyKey, value);
		}

		private void UpdateDragPosition()
		{
			if (this._dragService == null)
			{
				this._dragService = new DragService(this);
				this.SetIsDragging(true);
			}
			Point deviceDPI = this.TransformToDeviceDPI(Win32Helper.GetMousePosition());
			this._dragService.UpdateMouseLocation(deviceDPI);
		}

		private void UpdateMaximizedState(bool isMaximized)
		{
			foreach (ILayoutElementForFloatingWindow layoutElementForFloatingWindow in this.Model.Descendents().OfType<ILayoutElementForFloatingWindow>())
			{
				layoutElementForFloatingWindow.IsMaximized = isMaximized;
			}
			this.IsMaximized = isMaximized;
			base.WindowState = (isMaximized ? System.Windows.WindowState.Maximized : System.Windows.WindowState.Normal);
		}

		private void UpdatePositionAndSizeOfPanes()
		{
			foreach (ILayoutElementForFloatingWindow left in this.Model.Descendents().OfType<ILayoutElementForFloatingWindow>())
			{
				left.FloatingLeft = base.Left;
				left.FloatingTop = base.Top;
				left.FloatingWidth = base.Width;
				left.FloatingHeight = base.Height;
			}
		}

		internal virtual void UpdateThemeResources(Theme oldTheme = null)
		{
			if (oldTheme != null)
			{
				if (!(oldTheme is DictionaryTheme))
				{
					ResourceDictionary resourceDictionaries = base.Resources.MergedDictionaries.FirstOrDefault<ResourceDictionary>((ResourceDictionary r) => r.Source == oldTheme.GetResourceUri());
					if (resourceDictionaries != null)
					{
						base.Resources.MergedDictionaries.Remove(resourceDictionaries);
					}
				}
				else if (this.currentThemeResourceDictionary != null)
				{
					base.Resources.MergedDictionaries.Remove(this.currentThemeResourceDictionary);
					this.currentThemeResourceDictionary = null;
				}
			}
			DockingManager manager = this._model.Root.Manager;
			if (manager.Theme != null)
			{
				if (manager.Theme is DictionaryTheme)
				{
					this.currentThemeResourceDictionary = ((DictionaryTheme)manager.Theme).ThemeResourceDictionary;
					base.Resources.MergedDictionaries.Add(this.currentThemeResourceDictionary);
					return;
				}
				base.Resources.MergedDictionaries.Add(new ResourceDictionary()
				{
					Source = manager.Theme.GetResourceUri()
				});
			}
		}

		protected class FloatingWindowContentHost : HwndHost
		{
			private LayoutFloatingWindowControl _owner;

			private HwndSource _wpfContentHost;

			private Border _rootPresenter;

			private DockingManager _manager;

			public readonly static DependencyProperty ContentProperty;

			public UIElement Content
			{
				get
				{
					return (UIElement)base.GetValue(LayoutFloatingWindowControl.FloatingWindowContentHost.ContentProperty);
				}
				set
				{
					base.SetValue(LayoutFloatingWindowControl.FloatingWindowContentHost.ContentProperty, value);
				}
			}

			public Visual RootVisual
			{
				get
				{
					return this._rootPresenter;
				}
			}

			static FloatingWindowContentHost()
			{
				LayoutFloatingWindowControl.FloatingWindowContentHost.ContentProperty = DependencyProperty.Register("Content", typeof(UIElement), typeof(LayoutFloatingWindowControl.FloatingWindowContentHost), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutFloatingWindowControl.FloatingWindowContentHost.OnContentChanged)));
			}

			public FloatingWindowContentHost(LayoutFloatingWindowControl owner)
			{
				this._owner = owner;
				DockingManager manager = this._owner.Model.Root.Manager;
			}

			protected override HandleRef BuildWindowCore(HandleRef hwndParent)
			{
				HwndSourceParameters hwndSourceParameter = new HwndSourceParameters()
				{
					ParentWindow = hwndParent.Handle,
					WindowStyle = 1442840576,
					Width = 1,
					Height = 1
				};
				this._wpfContentHost = new HwndSource(hwndSourceParameter);
				this._rootPresenter = new Border()
				{
					Child = new AdornerDecorator()
					{
						Child = this.Content
					},
					Focusable = true
				};
				this._rootPresenter.SetBinding(Border.BackgroundProperty, new Binding("Background")
				{
					Source = this._owner
				});
				this._wpfContentHost.RootVisual = this._rootPresenter;
				this._wpfContentHost.SizeToContent = System.Windows.SizeToContent.Manual;
				this._manager = this._owner.Model.Root.Manager;
				this._manager.InternalAddLogicalChild(this._rootPresenter);
				return new HandleRef(this, this._wpfContentHost.Handle);
			}

			protected override void DestroyWindowCore(HandleRef hwnd)
			{
				this._manager.InternalRemoveLogicalChild(this._rootPresenter);
				if (this._wpfContentHost != null)
				{
					this._wpfContentHost.Dispose();
					this._wpfContentHost = null;
				}
			}

			protected override Size MeasureOverride(Size constraint)
			{
				if (this.Content == null)
				{
					return base.MeasureOverride(constraint);
				}
				this.Content.Measure(constraint);
				return this.Content.DesiredSize;
			}

			private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
			{
				((LayoutFloatingWindowControl.FloatingWindowContentHost)d).OnContentChanged(e);
			}

			protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
			{
				if (this._rootPresenter != null)
				{
					this._rootPresenter.Child = this.Content;
				}
			}
		}
	}
}