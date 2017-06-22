using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutAutoHideWindowControl : HwndHost, ILayoutControl
	{
		private LayoutAnchorControl _anchor;

		private LayoutAnchorable _model;

		private HwndSource _internalHwndSource;

		private IntPtr parentWindowHandle;

		private bool _internalHost_ContentRendered;

		private ContentPresenter _internalHostPresenter = new ContentPresenter();

		private Grid _internalGrid;

		private LayoutAnchorableControl _internalHost;

		private AnchorSide _side;

		private LayoutGridResizerControl _resizer;

		private DockingManager _manager;

		private Border _resizerGhost;

		private Window _resizerWindowHost;

		private Vector _initialStartPoint;

		public readonly static DependencyProperty BackgroundProperty;

		public readonly static DependencyProperty AnchorableStyleProperty;

		public System.Windows.Style AnchorableStyle
		{
			get
			{
				return (System.Windows.Style)base.GetValue(LayoutAutoHideWindowControl.AnchorableStyleProperty);
			}
			set
			{
				base.SetValue(LayoutAutoHideWindowControl.AnchorableStyleProperty, value);
			}
		}

		public Brush Background
		{
			get
			{
				return (Brush)base.GetValue(LayoutAutoHideWindowControl.BackgroundProperty);
			}
			set
			{
				base.SetValue(LayoutAutoHideWindowControl.BackgroundProperty, value);
			}
		}

		internal bool IsResizing
		{
			get;
			private set;
		}

		internal bool IsWin32MouseOver
		{
			get
			{
				Win32Helper.Win32Point win32Point = new Win32Helper.Win32Point();
				if (!Win32Helper.GetCursorPos(ref win32Point))
				{
					return false;
				}
				Point point = new Point();
				this.PointToScreenDPI(point);
				if (this.GetScreenArea().Contains(new Point((double)win32Point.X, (double)win32Point.Y)))
				{
					return true;
				}
				LayoutAnchorControl layoutAnchorControl = (
					from c in this.Model.Root.Manager.FindVisualChildren<LayoutAnchorControl>()
					where c.Model == this.Model
					select c).FirstOrDefault<LayoutAnchorControl>();
				if (layoutAnchorControl == null)
				{
					return false;
				}
				point = new Point();
				layoutAnchorControl.PointToScreenDPI(point);
				if (layoutAnchorControl.IsMouseOver)
				{
					return true;
				}
				return false;
			}
		}

		protected override IEnumerator LogicalChildren
		{
			get
			{
				if (this._internalHostPresenter == null)
				{
					return (new UIElement[0]).GetEnumerator();
				}
				return ((Array)(new UIElement[] { this._internalHostPresenter })).GetEnumerator();
			}
		}

		public ILayoutElement Model
		{
			get
			{
				return this._model;
			}
		}

		static LayoutAutoHideWindowControl()
		{
			LayoutAutoHideWindowControl.BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(LayoutAutoHideWindowControl), new FrameworkPropertyMetadata(null));
			LayoutAutoHideWindowControl.AnchorableStyleProperty = DependencyProperty.Register("AnchorableStyle", typeof(System.Windows.Style), typeof(LayoutAutoHideWindowControl), new FrameworkPropertyMetadata(null));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAutoHideWindowControl), new FrameworkPropertyMetadata(typeof(LayoutAutoHideWindowControl)));
			UIElement.FocusableProperty.OverrideMetadata(typeof(LayoutAutoHideWindowControl), new FrameworkPropertyMetadata(true));
			Control.IsTabStopProperty.OverrideMetadata(typeof(LayoutAutoHideWindowControl), new FrameworkPropertyMetadata(true));
			UIElement.VisibilityProperty.OverrideMetadata(typeof(LayoutAutoHideWindowControl), new FrameworkPropertyMetadata((object)System.Windows.Visibility.Hidden));
		}

		internal LayoutAutoHideWindowControl()
		{
		}

		private void _internalHwndSource_ContentRendered(object sender, EventArgs e)
		{
			this._internalHost_ContentRendered = true;
		}

		private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsAutoHidden" && !this._model.IsAutoHidden)
			{
				this._manager.HideAutoHideWindow(this._anchor);
			}
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (this._internalHostPresenter == null)
			{
				return base.ArrangeOverride(finalSize);
			}
			this._internalHostPresenter.Arrange(new Rect(finalSize));
			return base.ArrangeOverride(finalSize);
		}

		protected override HandleRef BuildWindowCore(HandleRef hwndParent)
		{
			this.parentWindowHandle = hwndParent.Handle;
			HwndSourceParameters hwndSourceParameter = new HwndSourceParameters()
			{
				ParentWindow = hwndParent.Handle,
				WindowStyle = 1442840576,
				Width = 0,
				Height = 0
			};
			this._internalHwndSource = new HwndSource(hwndSourceParameter);
			this._internalHost_ContentRendered = false;
			this._internalHwndSource.ContentRendered += new EventHandler(this._internalHwndSource_ContentRendered);
			this._internalHwndSource.RootVisual = this._internalHostPresenter;
			base.AddLogicalChild(this._internalHostPresenter);
			Win32Helper.BringWindowToTop(this._internalHwndSource.Handle);
			return new HandleRef(this, this._internalHwndSource.Handle);
		}

		private void CreateInternalGrid()
		{
			this._internalGrid = new Grid()
			{
				FlowDirection = System.Windows.FlowDirection.LeftToRight
			};
			this._internalGrid.SetBinding(Panel.BackgroundProperty, new Binding("Background")
			{
				Source = this
			});
			this._internalHost = new LayoutAnchorableControl()
			{
				Model = this._model,
				Style = this.AnchorableStyle
			};
			this._internalHost.SetBinding(FrameworkElement.FlowDirectionProperty, new Binding("Model.Root.Manager.FlowDirection")
			{
				Source = this
			});
			System.Windows.Input.KeyboardNavigation.SetTabNavigation(this._internalGrid, KeyboardNavigationMode.Cycle);
			this._resizer = new LayoutGridResizerControl();
			this._resizer.DragStarted += new DragStartedEventHandler(this.OnResizerDragStarted);
			this._resizer.DragDelta += new DragDeltaEventHandler(this.OnResizerDragDelta);
			this._resizer.DragCompleted += new DragCompletedEventHandler(this.OnResizerDragCompleted);
			if (this._side == AnchorSide.Right)
			{
				this._internalGrid.ColumnDefinitions.Add(new ColumnDefinition()
				{
					Width = new GridLength(this._manager.GridSplitterWidth)
				});
				this._internalGrid.ColumnDefinitions.Add(new ColumnDefinition()
				{
					Width = (this._model.AutoHideWidth == 0 ? new GridLength(this._model.AutoHideMinWidth) : new GridLength(this._model.AutoHideWidth, GridUnitType.Pixel))
				});
				Grid.SetColumn(this._resizer, 0);
				Grid.SetColumn(this._internalHost, 1);
				this._resizer.Cursor = Cursors.SizeWE;
				base.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
				base.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
			}
			else if (this._side == AnchorSide.Left)
			{
				this._internalGrid.ColumnDefinitions.Add(new ColumnDefinition()
				{
					Width = (this._model.AutoHideWidth == 0 ? new GridLength(this._model.AutoHideMinWidth) : new GridLength(this._model.AutoHideWidth, GridUnitType.Pixel))
				});
				this._internalGrid.ColumnDefinitions.Add(new ColumnDefinition()
				{
					Width = new GridLength(this._manager.GridSplitterWidth)
				});
				Grid.SetColumn(this._internalHost, 0);
				Grid.SetColumn(this._resizer, 1);
				this._resizer.Cursor = Cursors.SizeWE;
				base.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
				base.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
			}
			else if (this._side == AnchorSide.Top)
			{
				this._internalGrid.RowDefinitions.Add(new RowDefinition()
				{
					Height = (this._model.AutoHideHeight == 0 ? new GridLength(this._model.AutoHideMinHeight) : new GridLength(this._model.AutoHideHeight, GridUnitType.Pixel))
				});
				this._internalGrid.RowDefinitions.Add(new RowDefinition()
				{
					Height = new GridLength(this._manager.GridSplitterHeight)
				});
				Grid.SetRow(this._internalHost, 0);
				Grid.SetRow(this._resizer, 1);
				this._resizer.Cursor = Cursors.SizeNS;
				base.VerticalAlignment = System.Windows.VerticalAlignment.Top;
				base.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
			}
			else if (this._side == AnchorSide.Bottom)
			{
				this._internalGrid.RowDefinitions.Add(new RowDefinition()
				{
					Height = new GridLength(this._manager.GridSplitterHeight)
				});
				this._internalGrid.RowDefinitions.Add(new RowDefinition()
				{
					Height = (this._model.AutoHideHeight == 0 ? new GridLength(this._model.AutoHideMinHeight) : new GridLength(this._model.AutoHideHeight, GridUnitType.Pixel))
				});
				Grid.SetRow(this._resizer, 0);
				Grid.SetRow(this._internalHost, 1);
				this._resizer.Cursor = Cursors.SizeNS;
				base.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
				base.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
			}
			this._internalGrid.Children.Add(this._resizer);
			this._internalGrid.Children.Add(this._internalHost);
			this._internalHostPresenter.Content = this._internalGrid;
		}

		protected override void DestroyWindowCore(HandleRef hwnd)
		{
			if (this._internalHwndSource != null)
			{
				this._internalHwndSource.ContentRendered -= new EventHandler(this._internalHwndSource_ContentRendered);
				this._internalHwndSource.Dispose();
				this._internalHwndSource = null;
			}
		}

		protected override bool HasFocusWithinCore()
		{
			return false;
		}

		internal void Hide()
		{
			if (this._model == null)
			{
				return;
			}
			this._model.PropertyChanged -= new PropertyChangedEventHandler(this._model_PropertyChanged);
			this.RemoveInternalGrid();
			this._anchor = null;
			this._model = null;
			this._manager = null;
			base.Visibility = System.Windows.Visibility.Hidden;
		}

		private void HideResizerOverlayWindow()
		{
			if (this._resizerWindowHost != null)
			{
				this._resizerWindowHost.Close();
				this._resizerWindowHost = null;
			}
		}

		protected override Size MeasureOverride(Size constraint)
		{
			if (this._internalHostPresenter == null)
			{
				return base.MeasureOverride(constraint);
			}
			this._internalHostPresenter.Measure(constraint);
			return this._internalHostPresenter.DesiredSize;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
		}

		private void OnResizerDragCompleted(object sender, DragCompletedEventArgs e)
		{
			double num;
			GeneralTransform ancestor = base.TransformToAncestor(this.FindVisualTreeRoot() as Visual);
			Vector vector = ancestor.Transform(new Point(e.HorizontalChange, e.VerticalChange)) - ancestor.Transform(new Point());
			num = (this._side == AnchorSide.Right || this._side == AnchorSide.Left ? Canvas.GetLeft(this._resizerGhost) - this._initialStartPoint.X : Canvas.GetTop(this._resizerGhost) - this._initialStartPoint.Y);
			if (this._side == AnchorSide.Right)
			{
				if (this._model.AutoHideWidth != 0)
				{
					LayoutAnchorable autoHideWidth = this._model;
					autoHideWidth.AutoHideWidth = autoHideWidth.AutoHideWidth - num;
				}
				else
				{
					this._model.AutoHideWidth = this._internalHost.ActualWidth - num;
				}
				this._internalGrid.ColumnDefinitions[1].Width = new GridLength(this._model.AutoHideWidth, GridUnitType.Pixel);
			}
			else if (this._side == AnchorSide.Left)
			{
				if (this._model.AutoHideWidth != 0)
				{
					LayoutAnchorable layoutAnchorable = this._model;
					layoutAnchorable.AutoHideWidth = layoutAnchorable.AutoHideWidth + num;
				}
				else
				{
					this._model.AutoHideWidth = this._internalHost.ActualWidth + num;
				}
				this._internalGrid.ColumnDefinitions[0].Width = new GridLength(this._model.AutoHideWidth, GridUnitType.Pixel);
			}
			else if (this._side == AnchorSide.Top)
			{
				if (this._model.AutoHideHeight != 0)
				{
					LayoutAnchorable autoHideHeight = this._model;
					autoHideHeight.AutoHideHeight = autoHideHeight.AutoHideHeight + num;
				}
				else
				{
					this._model.AutoHideHeight = this._internalHost.ActualHeight + num;
				}
				this._internalGrid.RowDefinitions[0].Height = new GridLength(this._model.AutoHideHeight, GridUnitType.Pixel);
			}
			else if (this._side == AnchorSide.Bottom)
			{
				if (this._model.AutoHideHeight != 0)
				{
					LayoutAnchorable autoHideHeight1 = this._model;
					autoHideHeight1.AutoHideHeight = autoHideHeight1.AutoHideHeight - num;
				}
				else
				{
					this._model.AutoHideHeight = this._internalHost.ActualHeight - num;
				}
				this._internalGrid.RowDefinitions[1].Height = new GridLength(this._model.AutoHideHeight, GridUnitType.Pixel);
			}
			this.HideResizerOverlayWindow();
			this.IsResizing = false;
			base.InvalidateMeasure();
		}

		private void OnResizerDragDelta(object sender, DragDeltaEventArgs e)
		{
			GeneralTransform ancestor = base.TransformToAncestor(this.FindVisualTreeRoot() as Visual);
			Vector x = ancestor.Transform(new Point(e.HorizontalChange, e.VerticalChange)) - ancestor.Transform(new Point());
			if (this._side != AnchorSide.Right && this._side != AnchorSide.Left)
			{
				Canvas.SetTop(this._resizerGhost, MathHelper.MinMax(this._initialStartPoint.Y + x.Y, 0, this._resizerWindowHost.Height - this._resizerGhost.Height));
				return;
			}
			if (FrameworkElement.GetFlowDirection(this._internalHost) == System.Windows.FlowDirection.RightToLeft)
			{
				x.X = -x.X;
			}
			Canvas.SetLeft(this._resizerGhost, MathHelper.MinMax(this._initialStartPoint.X + x.X, 0, this._resizerWindowHost.Width - this._resizerGhost.Width));
		}

		private void OnResizerDragStarted(object sender, DragStartedEventArgs e)
		{
			this.ShowResizerOverlayWindow(sender as LayoutGridResizerControl);
			this.IsResizing = true;
		}

		private void RemoveInternalGrid()
		{
			this._resizer.DragStarted -= new DragStartedEventHandler(this.OnResizerDragStarted);
			this._resizer.DragDelta -= new DragDeltaEventHandler(this.OnResizerDragDelta);
			this._resizer.DragCompleted -= new DragCompletedEventHandler(this.OnResizerDragCompleted);
			this._internalHostPresenter.Content = null;
		}

		internal void Show(LayoutAnchorControl anchor)
		{
			if (this._model != null)
			{
				throw new InvalidOperationException();
			}
			this._anchor = anchor;
			this._model = anchor.Model as LayoutAnchorable;
			this._side = (anchor.Model.Parent.Parent as LayoutAnchorSide).Side;
			this._manager = this._model.Root.Manager;
			this.CreateInternalGrid();
			this._model.PropertyChanged += new PropertyChangedEventHandler(this._model_PropertyChanged);
			base.Visibility = System.Windows.Visibility.Visible;
			base.InvalidateMeasure();
			base.UpdateWindowPos();
		}

		private void ShowResizerOverlayWindow(LayoutGridResizerControl splitter)
		{
			Size size;
			this._resizerGhost = new Border()
			{
				Background = splitter.BackgroundWhileDragging,
				Opacity = splitter.OpacityWhileDragging
			};
			FrameworkElement autoHideAreaElement = this._manager.GetAutoHideAreaElement();
			this._internalHost.TransformActualSizeToAncestor();
			Point point = new Point();
			Point screenDPIWithoutFlowDirection = autoHideAreaElement.PointToScreenDPIWithoutFlowDirection(point);
			Size ancestor = autoHideAreaElement.TransformActualSizeToAncestor();
			if (this._side == AnchorSide.Right || this._side == AnchorSide.Left)
			{
				size = new Size(ancestor.Width - 25 + splitter.ActualWidth, ancestor.Height);
				this._resizerGhost.Width = splitter.ActualWidth;
				this._resizerGhost.Height = size.Height;
				screenDPIWithoutFlowDirection.Offset(25, 0);
			}
			else
			{
				size = new Size(ancestor.Width, ancestor.Height - this._model.AutoHideMinHeight - 25 + splitter.ActualHeight);
				this._resizerGhost.Height = splitter.ActualHeight;
				this._resizerGhost.Width = size.Width;
				screenDPIWithoutFlowDirection.Offset(0, 25);
			}
			point = new Point();
			this._initialStartPoint = splitter.PointToScreenDPIWithoutFlowDirection(point) - screenDPIWithoutFlowDirection;
			if (this._side == AnchorSide.Right || this._side == AnchorSide.Left)
			{
				Canvas.SetLeft(this._resizerGhost, this._initialStartPoint.X);
			}
			else
			{
				Canvas.SetTop(this._resizerGhost, this._initialStartPoint.Y);
			}
			Canvas canva = new Canvas()
			{
				HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
				VerticalAlignment = System.Windows.VerticalAlignment.Stretch
			};
			canva.Children.Add(this._resizerGhost);
			this._resizerWindowHost = new Window()
			{
				ResizeMode = ResizeMode.NoResize,
				WindowStyle = WindowStyle.None,
				ShowInTaskbar = false,
				AllowsTransparency = true,
				Background = null,
				Width = size.Width,
				Height = size.Height,
				Left = screenDPIWithoutFlowDirection.X,
				Top = screenDPIWithoutFlowDirection.Y,
				ShowActivated = false,
				Owner = Window.GetWindow(this),
				Content = canva
			};
			this._resizerWindowHost.Show();
		}

		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == 70 && this._internalHost_ContentRendered)
			{
				Win32Helper.SetWindowPos(this._internalHwndSource.Handle, Win32Helper.HWND_TOP, 0, 0, 0, 0, Win32Helper.SetWindowPosFlags.IgnoreMove | Win32Helper.SetWindowPosFlags.IgnoreResize);
			}
			return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}
	}
}