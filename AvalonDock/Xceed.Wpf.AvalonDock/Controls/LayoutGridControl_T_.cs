using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public abstract class LayoutGridControl<T> : Grid, ILayoutControl
	where T : class, ILayoutPanelElement
	{
		private LayoutPositionableGroup<T> _model;

		private System.Windows.Controls.Orientation _orientation;

		private bool _initialized;

		private ChildrenTreeChange? _asyncRefreshCalled;

		private ReentrantFlag _fixingChildrenDockLengths;

		private Border _resizerGhost;

		private Window _resizerWindowHost;

		private Vector _initialStartPoint;

		private bool AsyncRefreshCalled
		{
			get
			{
				return this._asyncRefreshCalled.HasValue;
			}
		}

		public ILayoutElement Model
		{
			get
			{
				return this._model;
			}
		}

		public System.Windows.Controls.Orientation Orientation
		{
			get
			{
				return (this._model as ILayoutOrientableGroup).Orientation;
			}
		}

		static LayoutGridControl()
		{
		}

		internal LayoutGridControl(LayoutPositionableGroup<T> model, System.Windows.Controls.Orientation orientation)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			this._model = model;
			this._orientation = orientation;
			base.FlowDirection = System.Windows.FlowDirection.LeftToRight;
		}

		private void AttachNewSplitters()
		{
			foreach (LayoutGridResizerControl layoutGridResizerControl in base.Children.OfType<LayoutGridResizerControl>())
			{
				layoutGridResizerControl.DragStarted += new DragStartedEventHandler(this.OnSplitterDragStarted);
				layoutGridResizerControl.DragDelta += new DragDeltaEventHandler(this.OnSplitterDragDelta);
				layoutGridResizerControl.DragCompleted += new DragCompletedEventHandler(this.OnSplitterDragCompleted);
			}
		}

		private void AttachPropertyChangeHandler()
		{
			foreach (ILayoutControl layoutControl in base.InternalChildren.OfType<ILayoutControl>())
			{
				layoutControl.Model.PropertyChanged += new PropertyChangedEventHandler(this.OnChildModelPropertyChanged);
			}
		}

		private void CreateSplitters()
		{
			for (int i = 1; i < base.Children.Count; i++)
			{
				LayoutGridResizerControl layoutGridResizerControl = new LayoutGridResizerControl()
				{
					Cursor = (this.Orientation == System.Windows.Controls.Orientation.Horizontal ? Cursors.SizeWE : Cursors.SizeNS)
				};
				base.Children.Insert(i, layoutGridResizerControl);
				i++;
			}
		}

		private void DetachOldSplitters()
		{
			foreach (LayoutGridResizerControl layoutGridResizerControl in base.Children.OfType<LayoutGridResizerControl>())
			{
				layoutGridResizerControl.DragStarted -= new DragStartedEventHandler(this.OnSplitterDragStarted);
				layoutGridResizerControl.DragDelta -= new DragDeltaEventHandler(this.OnSplitterDragDelta);
				layoutGridResizerControl.DragCompleted -= new DragCompletedEventHandler(this.OnSplitterDragCompleted);
			}
		}

		private void DetachPropertChangeHandler()
		{
			foreach (ILayoutControl layoutControl in base.InternalChildren.OfType<ILayoutControl>())
			{
				layoutControl.Model.PropertyChanged -= new PropertyChangedEventHandler(this.OnChildModelPropertyChanged);
			}
		}

		protected void FixChildrenDockLengths()
		{
			using (ReentrantFlag._ReentrantFlagHandler __ReentrantFlagHandler = this._fixingChildrenDockLengths.Enter())
			{
				this.OnFixChildrenDockLengths();
			}
		}

		private FrameworkElement GetNextVisibleChild(int index)
		{
			for (int i = index + 1; i < base.InternalChildren.Count; i++)
			{
				if (!(base.InternalChildren[i] is LayoutGridResizerControl))
				{
					if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
					{
						if (base.ColumnDefinitions[i].Width.IsStar || base.ColumnDefinitions[i].Width.Value > 0)
						{
							return base.InternalChildren[i] as FrameworkElement;
						}
					}
					else if (base.RowDefinitions[i].Height.IsStar || base.RowDefinitions[i].Height.Value > 0)
					{
						return base.InternalChildren[i] as FrameworkElement;
					}
				}
			}
			return null;
		}

		private void HideResizerOverlayWindow()
		{
			if (this._resizerWindowHost != null)
			{
				this._resizerWindowHost.Close();
				this._resizerWindowHost = null;
			}
		}

		private void OnChildModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.AsyncRefreshCalled)
			{
				return;
			}
			if (this._fixingChildrenDockLengths.CanEnter && e.PropertyName == "DockWidth" && this.Orientation == System.Windows.Controls.Orientation.Horizontal)
			{
				if (base.ColumnDefinitions.Count == base.InternalChildren.Count)
				{
					ILayoutPositionableElement layoutPositionableElement = sender as ILayoutPositionableElement;
					UIElement uIElement = base.InternalChildren.OfType<ILayoutControl>().First<ILayoutControl>((ILayoutControl ch) => ch.Model == layoutPositionableElement) as UIElement;
					int dockWidth = base.InternalChildren.IndexOf(uIElement);
					base.ColumnDefinitions[dockWidth].Width = layoutPositionableElement.DockWidth;
					return;
				}
			}
			else if (this._fixingChildrenDockLengths.CanEnter && e.PropertyName == "DockHeight" && this.Orientation == System.Windows.Controls.Orientation.Vertical)
			{
				if (base.RowDefinitions.Count == base.InternalChildren.Count)
				{
					ILayoutPositionableElement layoutPositionableElement1 = sender as ILayoutPositionableElement;
					UIElement uIElement1 = base.InternalChildren.OfType<ILayoutControl>().First<ILayoutControl>((ILayoutControl ch) => ch.Model == layoutPositionableElement1) as UIElement;
					int dockHeight = base.InternalChildren.IndexOf(uIElement1);
					base.RowDefinitions[dockHeight].Height = layoutPositionableElement1.DockHeight;
					return;
				}
			}
			else if (e.PropertyName == "IsVisible")
			{
				this.UpdateRowColDefinitions();
			}
		}

		protected abstract void OnFixChildrenDockLengths();

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			this._model.ChildrenTreeChanged += new EventHandler<ChildrenTreeChangedEventArgs>((object s, ChildrenTreeChangedEventArgs args) => {
				if (this._asyncRefreshCalled.HasValue && this._asyncRefreshCalled.Value == args.Change)
				{
					return;
				}
				this._asyncRefreshCalled = new ChildrenTreeChange?(args.Change);
				base.Dispatcher.BeginInvoke(new Action(() => {
					this._asyncRefreshCalled = null;
					this.UpdateChildren();
				}), DispatcherPriority.Normal, null);
			});
			base.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
		}

		private void OnLayoutUpdated(object sender, EventArgs e)
		{
			LayoutPositionableGroup<T> actualWidth = this._model;
			((ILayoutPositionableElementWithActualSize)actualWidth).ActualWidth = base.ActualWidth;
			((ILayoutPositionableElementWithActualSize)actualWidth).ActualHeight = base.ActualHeight;
			if (!this._initialized)
			{
				this._initialized = true;
				this.UpdateChildren();
			}
		}

		private void OnSplitterDragCompleted(object sender, DragCompletedEventArgs e)
		{
			double num;
			GridLength dockHeight;
			LayoutGridResizerControl layoutGridResizerControl = sender as LayoutGridResizerControl;
			GeneralTransform ancestor = base.TransformToAncestor(this.FindVisualTreeRoot() as Visual);
			Vector vector = ancestor.Transform(new Point(e.HorizontalChange, e.VerticalChange)) - ancestor.Transform(new Point());
			num = (this.Orientation != System.Windows.Controls.Orientation.Horizontal ? Canvas.GetTop(this._resizerGhost) - this._initialStartPoint.Y : Canvas.GetLeft(this._resizerGhost) - this._initialStartPoint.X);
			int num1 = base.InternalChildren.IndexOf(layoutGridResizerControl);
			FrameworkElement item = base.InternalChildren[num1 - 1] as FrameworkElement;
			FrameworkElement nextVisibleChild = this.GetNextVisibleChild(num1);
			Size size = item.TransformActualSizeToAncestor();
			Size ancestor1 = nextVisibleChild.TransformActualSizeToAncestor();
			ILayoutPositionableElement model = (ILayoutPositionableElement)(item as ILayoutControl).Model;
			ILayoutPositionableElement gridLength = (ILayoutPositionableElement)(nextVisibleChild as ILayoutControl).Model;
			if (this.Orientation != System.Windows.Controls.Orientation.Horizontal)
			{
				if (!model.DockHeight.IsStar)
				{
					dockHeight = model.DockHeight;
					model.DockHeight = new GridLength(dockHeight.Value + num, GridUnitType.Pixel);
				}
				else
				{
					dockHeight = model.DockHeight;
					model.DockHeight = new GridLength(dockHeight.Value * (size.Height + num) / size.Height, GridUnitType.Star);
				}
				if (!gridLength.DockHeight.IsStar)
				{
					dockHeight = gridLength.DockHeight;
					gridLength.DockHeight = new GridLength(dockHeight.Value - num, GridUnitType.Pixel);
				}
				else
				{
					dockHeight = gridLength.DockHeight;
					gridLength.DockHeight = new GridLength(dockHeight.Value * (ancestor1.Height - num) / ancestor1.Height, GridUnitType.Star);
				}
			}
			else
			{
				if (!model.DockWidth.IsStar)
				{
					dockHeight = model.DockWidth;
					model.DockWidth = new GridLength(dockHeight.Value + num, GridUnitType.Pixel);
				}
				else
				{
					dockHeight = model.DockWidth;
					model.DockWidth = new GridLength(dockHeight.Value * (size.Width + num) / size.Width, GridUnitType.Star);
				}
				if (!gridLength.DockWidth.IsStar)
				{
					dockHeight = gridLength.DockWidth;
					gridLength.DockWidth = new GridLength(dockHeight.Value - num, GridUnitType.Pixel);
				}
				else
				{
					dockHeight = gridLength.DockWidth;
					gridLength.DockWidth = new GridLength(dockHeight.Value * (ancestor1.Width - num) / ancestor1.Width, GridUnitType.Star);
				}
			}
			this.HideResizerOverlayWindow();
		}

		private void OnSplitterDragDelta(object sender, DragDeltaEventArgs e)
		{
			GeneralTransform ancestor = base.TransformToAncestor(this.FindVisualTreeRoot() as Visual);
			Vector vector = ancestor.Transform(new Point(e.HorizontalChange, e.VerticalChange)) - ancestor.Transform(new Point());
			if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
			{
				Canvas.SetLeft(this._resizerGhost, MathHelper.MinMax(this._initialStartPoint.X + vector.X, 0, this._resizerWindowHost.Width - this._resizerGhost.Width));
				return;
			}
			Canvas.SetTop(this._resizerGhost, MathHelper.MinMax(this._initialStartPoint.Y + vector.Y, 0, this._resizerWindowHost.Height - this._resizerGhost.Height));
		}

		private void OnSplitterDragStarted(object sender, DragStartedEventArgs e)
		{
			this.ShowResizerOverlayWindow(sender as LayoutGridResizerControl);
		}

		private void ShowResizerOverlayWindow(LayoutGridResizerControl splitter)
		{
			Size size;
			this._resizerGhost = new Border()
			{
				Background = splitter.BackgroundWhileDragging,
				Opacity = splitter.OpacityWhileDragging
			};
			int num = base.InternalChildren.IndexOf(splitter);
			FrameworkElement item = base.InternalChildren[num - 1] as FrameworkElement;
			FrameworkElement nextVisibleChild = this.GetNextVisibleChild(num);
			Size ancestor = item.TransformActualSizeToAncestor();
			Size ancestor1 = nextVisibleChild.TransformActualSizeToAncestor();
			ILayoutPositionableElement model = (ILayoutPositionableElement)(item as ILayoutControl).Model;
			ILayoutPositionableElement layoutPositionableElement = (ILayoutPositionableElement)(nextVisibleChild as ILayoutControl).Model;
			Point point = new Point();
			Point screenDPIWithoutFlowDirection = item.PointToScreenDPIWithoutFlowDirection(point);
			if (this.Orientation != System.Windows.Controls.Orientation.Horizontal)
			{
				size = new Size(ancestor.Width, ancestor.Height - model.DockMinHeight + splitter.ActualHeight + ancestor1.Height - layoutPositionableElement.DockMinHeight);
				this._resizerGhost.Height = splitter.ActualHeight;
				this._resizerGhost.Width = size.Width;
				screenDPIWithoutFlowDirection.Offset(0, model.DockMinHeight);
			}
			else
			{
				size = new Size(ancestor.Width - model.DockMinWidth + splitter.ActualWidth + ancestor1.Width - layoutPositionableElement.DockMinWidth, ancestor1.Height);
				this._resizerGhost.Width = splitter.ActualWidth;
				this._resizerGhost.Height = size.Height;
				screenDPIWithoutFlowDirection.Offset(model.DockMinWidth, 0);
			}
			point = new Point();
			this._initialStartPoint = splitter.PointToScreenDPIWithoutFlowDirection(point) - screenDPIWithoutFlowDirection;
			if (this.Orientation != System.Windows.Controls.Orientation.Horizontal)
			{
				Canvas.SetTop(this._resizerGhost, this._initialStartPoint.Y);
			}
			else
			{
				Canvas.SetLeft(this._resizerGhost, this._initialStartPoint.X);
			}
			Canvas canva = new Canvas()
			{
				HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
				VerticalAlignment = System.Windows.VerticalAlignment.Stretch
			};
			canva.Children.Add(this._resizerGhost);
			this._resizerWindowHost = new Window()
			{
				SizeToContent = SizeToContent.Manual,
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
				Content = canva
			};
			this._resizerWindowHost.Loaded += new RoutedEventHandler((object s, RoutedEventArgs e) => this._resizerWindowHost.SetParentToMainWindowOf(this));
			this._resizerWindowHost.Show();
		}

		private void UpdateChildren()
		{
			ILayoutControl[] array = base.Children.OfType<ILayoutControl>().ToArray<ILayoutControl>();
			this.DetachOldSplitters();
			this.DetachPropertChangeHandler();
			base.Children.Clear();
			base.ColumnDefinitions.Clear();
			base.RowDefinitions.Clear();
			if (this._model == null || this._model.Root == null)
			{
				return;
			}
			DockingManager manager = this._model.Root.Manager;
			if (manager == null)
			{
				return;
			}
			foreach (T child in this._model.Children)
			{
				ILayoutElement layoutElement = child;
				ILayoutControl layoutControl = array.FirstOrDefault<ILayoutControl>((ILayoutControl chVM) => chVM.Model == layoutElement);
				if (layoutControl == null)
				{
					base.Children.Add(manager.CreateUIElementForModel(layoutElement));
				}
				else
				{
					base.Children.Add(layoutControl as UIElement);
				}
			}
			this.CreateSplitters();
			this.UpdateRowColDefinitions();
			this.AttachNewSplitters();
			this.AttachPropertyChangeHandler();
		}

		private void UpdateRowColDefinitions()
		{
			ILayoutRoot root = this._model.Root;
			if (root == null)
			{
				return;
			}
			DockingManager manager = root.Manager;
			if (manager == null)
			{
				return;
			}
			this.FixChildrenDockLengths();
			base.RowDefinitions.Clear();
			base.ColumnDefinitions.Clear();
			if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
			{
				int num = 0;
				int num1 = 0;
				int num2 = 0;
				while (num2 < this._model.Children.Count)
				{
					ILayoutPositionableElement item = (object)this._model.Children[num2] as ILayoutPositionableElement;
					base.ColumnDefinitions.Add(new ColumnDefinition()
					{
						Width = (item.IsVisible ? item.DockWidth : new GridLength(0, GridUnitType.Pixel)),
						MinWidth = (item.IsVisible ? item.DockMinWidth : 0)
					});
					Grid.SetColumn(base.InternalChildren[num1], num);
					if (num1 < base.InternalChildren.Count - 1)
					{
						num1++;
						num++;
						bool flag = false;
						int num3 = num2 + 1;
						while (num3 < this._model.Children.Count)
						{
							if (!((object)this._model.Children[num3] as ILayoutPositionableElement).IsVisible)
							{
								num3++;
							}
							else
							{
								flag = true;
								break;
							}
						}
						base.ColumnDefinitions.Add(new ColumnDefinition()
						{
							Width = (item.IsVisible & flag ? new GridLength(manager.GridSplitterWidth) : new GridLength(0, GridUnitType.Pixel))
						});
						Grid.SetColumn(base.InternalChildren[num1], num);
					}
					num2++;
					num++;
					num1++;
				}
				return;
			}
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			while (num6 < this._model.Children.Count)
			{
				ILayoutPositionableElement layoutPositionableElement = (object)this._model.Children[num6] as ILayoutPositionableElement;
				base.RowDefinitions.Add(new RowDefinition()
				{
					Height = (layoutPositionableElement.IsVisible ? layoutPositionableElement.DockHeight : new GridLength(0, GridUnitType.Pixel)),
					MinHeight = (layoutPositionableElement.IsVisible ? layoutPositionableElement.DockMinHeight : 0)
				});
				Grid.SetRow(base.InternalChildren[num5], num4);
				if (num5 < base.InternalChildren.Count - 1)
				{
					num5++;
					num4++;
					bool flag1 = false;
					int num7 = num6 + 1;
					while (num7 < this._model.Children.Count)
					{
						if (!((object)this._model.Children[num7] as ILayoutPositionableElement).IsVisible)
						{
							num7++;
						}
						else
						{
							flag1 = true;
							break;
						}
					}
					base.RowDefinitions.Add(new RowDefinition()
					{
						Height = (layoutPositionableElement.IsVisible & flag1 ? new GridLength(manager.GridSplitterHeight) : new GridLength(0, GridUnitType.Pixel))
					});
					Grid.SetRow(base.InternalChildren[num5], num4);
				}
				num6++;
				num4++;
				num5++;
			}
		}
	}
}