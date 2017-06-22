using Microsoft.Windows.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Commands;
using Xceed.Wpf.AvalonDock.Converters;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutAnchorableFloatingWindowControl : LayoutFloatingWindowControl, IOverlayWindowHost
	{
		private LayoutAnchorableFloatingWindow _model;

		public readonly static DependencyProperty SingleContentLayoutItemProperty;

		private OverlayWindow _overlayWindow;

		private List<IDropArea> _dropAreas;

		public ICommand HideWindowCommand
		{
			get;
			private set;
		}

		public override ILayoutElement Model
		{
			get
			{
				return this._model;
			}
		}

		public LayoutItem SingleContentLayoutItem
		{
			get
			{
				return (LayoutItem)base.GetValue(LayoutAnchorableFloatingWindowControl.SingleContentLayoutItemProperty);
			}
			set
			{
				base.SetValue(LayoutAnchorableFloatingWindowControl.SingleContentLayoutItemProperty, value);
			}
		}

		DockingManager Xceed.Wpf.AvalonDock.Controls.IOverlayWindowHost.Manager
		{
			get
			{
				return this._model.Root.Manager;
			}
		}

		static LayoutAnchorableFloatingWindowControl()
		{
			LayoutAnchorableFloatingWindowControl.SingleContentLayoutItemProperty = DependencyProperty.Register("SingleContentLayoutItem", typeof(LayoutItem), typeof(LayoutAnchorableFloatingWindowControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutAnchorableFloatingWindowControl.OnSingleContentLayoutItemChanged)));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorableFloatingWindowControl), new FrameworkPropertyMetadata(typeof(LayoutAnchorableFloatingWindowControl)));
		}

		internal LayoutAnchorableFloatingWindowControl(LayoutAnchorableFloatingWindow model) : base(model)
		{
			this._model = model;
			this.HideWindowCommand = new RelayCommand((object p) => this.OnExecuteHideWindowCommand(p), (object p) => this.CanExecuteHideWindowCommand(p));
		}

		private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "RootPanel" && this._model.RootPanel == null)
			{
				base.InternalClose();
			}
		}

		private bool CanExecuteHideWindowCommand(object parameter)
		{
			if (this.Model == null)
			{
				return false;
			}
			ILayoutRoot root = this.Model.Root;
			if (root == null)
			{
				return false;
			}
			DockingManager manager = root.Manager;
			if (manager == null)
			{
				return false;
			}
			bool flag = false;
			LayoutAnchorable[] array = this.Model.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>();
			int num = 0;
			while (num < (int)array.Length)
			{
				LayoutAnchorable layoutAnchorable = array[num];
				if (layoutAnchorable.CanHide)
				{
					LayoutAnchorableItem layoutItemFromModel = manager.GetLayoutItemFromModel(layoutAnchorable) as LayoutAnchorableItem;
					if (layoutItemFromModel == null || layoutItemFromModel.HideCommand == null || !layoutItemFromModel.HideCommand.CanExecute(parameter))
					{
						flag = false;
						break;
					}
					else
					{
						flag = true;
						num++;
					}
				}
				else
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		private void CreateOverlayWindow()
		{
			if (this._overlayWindow == null)
			{
				this._overlayWindow = new OverlayWindow(this);
			}
			Point point = new Point();
			Rect rect = new Rect(this.PointToScreenDPIWithoutFlowDirection(point), this.TransformActualSizeToAncestor());
			this._overlayWindow.Left = rect.Left;
			this._overlayWindow.Top = rect.Top;
			this._overlayWindow.Width = rect.Width;
			this._overlayWindow.Height = rect.Height;
		}

		protected override IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg != 161)
			{
				if (msg == 165)
				{
					if (wParam.ToInt32() == 2)
					{
						if (this.OpenContextMenu())
						{
							handled = true;
						}
						if (!this._model.Root.Manager.ShowSystemMenu)
						{
							WindowChrome.GetWindowChrome(this).ShowSystemMenu = false;
						}
						else
						{
							WindowChrome.GetWindowChrome(this).ShowSystemMenu = !handled;
						}
					}
				}
			}
			else if (wParam.ToInt32() == 2)
			{
				this._model.Descendents().OfType<LayoutAnchorablePane>().First<LayoutAnchorablePane>((LayoutAnchorablePane p) => {
					if (p.ChildrenCount <= 0)
					{
						return false;
					}
					return p.SelectedContent != null;
				}).SelectedContent.IsActive = true;
				handled = true;
			}
			return base.FilterMessage(hwnd, msg, wParam, lParam, ref handled);
		}

		private bool IsContextMenuOpen()
		{
			System.Windows.Controls.ContextMenu anchorableContextMenu = this._model.Root.Manager.AnchorableContextMenu;
			if (anchorableContextMenu == null || this.SingleContentLayoutItem == null)
			{
				return false;
			}
			return anchorableContextMenu.IsOpen;
		}

		protected override void OnClosed(EventArgs e)
		{
			ILayoutRoot root = this.Model.Root;
			root.Manager.RemoveFloatingWindow(this);
			root.CollectGarbage();
			if (this._overlayWindow != null)
			{
				this._overlayWindow.Close();
				this._overlayWindow = null;
			}
			base.OnClosed(e);
			if (!base.CloseInitiatedByUser)
			{
				root.FloatingWindows.Remove(this._model);
			}
			this._model.PropertyChanged -= new PropertyChangedEventHandler(this._model_PropertyChanged);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (base.CloseInitiatedByUser && !base.KeepContentVisibleOnClose)
			{
				e.Cancel = true;
				((IEnumerable<LayoutAnchorable>)this._model.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>()).ForEach<LayoutAnchorable>((LayoutAnchorable a) => a.Hide(true));
			}
			base.OnClosing(e);
		}

		private void OnExecuteHideWindowCommand(object parameter)
		{
			DockingManager manager = this.Model.Root.Manager;
			LayoutAnchorable[] array = this.Model.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				LayoutAnchorable layoutAnchorable = array[i];
				(manager.GetLayoutItemFromModel(layoutAnchorable) as LayoutAnchorableItem).HideCommand.Execute(parameter);
			}
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			DockingManager manager = this._model.Root.Manager;
			base.Content = manager.CreateUIElementForModel(this._model.RootPanel);
			base.IsVisibleChanged += new DependencyPropertyChangedEventHandler((object s, DependencyPropertyChangedEventArgs args) => {
				BindingExpression bindingExpression = base.GetBindingExpression(UIElement.VisibilityProperty);
				if (base.IsVisible && bindingExpression == null)
				{
					base.SetBinding(UIElement.VisibilityProperty, new Binding("IsVisible")
					{
						Source = this._model,
						Converter = new BoolToVisibilityConverter(),
						Mode = BindingMode.OneWay,
						ConverterParameter = System.Windows.Visibility.Hidden
					});
				}
			});
			base.SetBinding(LayoutAnchorableFloatingWindowControl.SingleContentLayoutItemProperty, new Binding("Model.SinglePane.SelectedContent")
			{
				Source = this,
				Converter = new LayoutItemFromLayoutModelConverter()
			});
			this._model.PropertyChanged += new PropertyChangedEventHandler(this._model_PropertyChanged);
		}

		private static void OnSingleContentLayoutItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutAnchorableFloatingWindowControl)d).OnSingleContentLayoutItemChanged(e);
		}

		protected virtual void OnSingleContentLayoutItemChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private bool OpenContextMenu()
		{
			System.Windows.Controls.ContextMenu anchorableContextMenu = this._model.Root.Manager.AnchorableContextMenu;
			if (anchorableContextMenu == null || this.SingleContentLayoutItem == null)
			{
				return false;
			}
			anchorableContextMenu.PlacementTarget = null;
			anchorableContextMenu.Placement = PlacementMode.MousePoint;
			anchorableContextMenu.DataContext = this.SingleContentLayoutItem;
			anchorableContextMenu.IsOpen = true;
			return true;
		}

		internal override void UpdateThemeResources(Theme oldTheme = null)
		{
			base.UpdateThemeResources(oldTheme);
			if (this._overlayWindow != null)
			{
				this._overlayWindow.UpdateThemeResources(oldTheme);
			}
		}

		IEnumerable<IDropArea> Xceed.Wpf.AvalonDock.Controls.IOverlayWindowHost.GetDropAreas(LayoutFloatingWindowControl draggingWindow)
		{
			if (this._dropAreas != null)
			{
				return this._dropAreas;
			}
			this._dropAreas = new List<IDropArea>();
			if (draggingWindow.Model is LayoutDocumentFloatingWindow)
			{
				return this._dropAreas;
			}
			Visual rootVisual = (base.Content as LayoutFloatingWindowControl.FloatingWindowContentHost).RootVisual;
			foreach (LayoutAnchorablePaneControl layoutAnchorablePaneControl in rootVisual.FindVisualChildren<LayoutAnchorablePaneControl>())
			{
				this._dropAreas.Add(new DropArea<LayoutAnchorablePaneControl>(layoutAnchorablePaneControl, DropAreaType.AnchorablePane));
			}
			foreach (LayoutDocumentPaneControl layoutDocumentPaneControl in rootVisual.FindVisualChildren<LayoutDocumentPaneControl>())
			{
				this._dropAreas.Add(new DropArea<LayoutDocumentPaneControl>(layoutDocumentPaneControl, DropAreaType.DocumentPane));
			}
			return this._dropAreas;
		}

		void Xceed.Wpf.AvalonDock.Controls.IOverlayWindowHost.HideOverlayWindow()
		{
			this._dropAreas = null;
			this._overlayWindow.Owner = null;
			this._overlayWindow.HideDropTargets();
		}

		bool Xceed.Wpf.AvalonDock.Controls.IOverlayWindowHost.HitTest(Point dragPoint)
		{
			Point point = new Point();
			Rect rect = new Rect(this.PointToScreenDPIWithoutFlowDirection(point), this.TransformActualSizeToAncestor());
			return rect.Contains(dragPoint);
		}

		IOverlayWindow Xceed.Wpf.AvalonDock.Controls.IOverlayWindowHost.ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow)
		{
			this.CreateOverlayWindow();
			this._overlayWindow.Owner = draggingWindow;
			this._overlayWindow.EnableDropTargets();
			this._overlayWindow.Show();
			return this._overlayWindow;
		}
	}
}