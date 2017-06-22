using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal class DockingManagerDropTarget : DropTarget<DockingManager>
	{
		private DockingManager _manager;

		internal DockingManagerDropTarget(DockingManager manager, Rect detectionRect, DropTargetType type) : base(manager, detectionRect, type)
		{
			this._manager = manager;
		}

		protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
		{
			switch (base.Type)
			{
				case DropTargetType.DockingManagerDockLeft:
				{
					if (this._manager.Layout.RootPanel.Orientation != Orientation.Horizontal && this._manager.Layout.RootPanel.Children.Count == 1)
					{
						this._manager.Layout.RootPanel.Orientation = Orientation.Horizontal;
					}
					if (this._manager.Layout.RootPanel.Orientation != Orientation.Horizontal)
					{
						LayoutPanel layoutPanel = new LayoutPanel()
						{
							Orientation = Orientation.Horizontal
						};
						layoutPanel.Children.Add(floatingWindow.RootPanel);
						layoutPanel.Children.Add(this._manager.Layout.RootPanel);
						this._manager.Layout.RootPanel = layoutPanel;
						break;
					}
					else
					{
						LayoutAnchorablePaneGroup rootPanel = floatingWindow.RootPanel;
						if (rootPanel == null || rootPanel.Orientation != Orientation.Horizontal)
						{
							this._manager.Layout.RootPanel.Children.Insert(0, floatingWindow.RootPanel);
							break;
						}
						else
						{
							ILayoutAnchorablePane[] array = rootPanel.Children.ToArray<ILayoutAnchorablePane>();
							for (int i = 0; i < (int)array.Length; i++)
							{
								this._manager.Layout.RootPanel.Children.Insert(i, array[i]);
							}
							break;
						}
					}
				}
				case DropTargetType.DockingManagerDockTop:
				{
					if (this._manager.Layout.RootPanel.Orientation != Orientation.Vertical && this._manager.Layout.RootPanel.Children.Count == 1)
					{
						this._manager.Layout.RootPanel.Orientation = Orientation.Vertical;
					}
					if (this._manager.Layout.RootPanel.Orientation != Orientation.Vertical)
					{
						LayoutPanel layoutPanel1 = new LayoutPanel()
						{
							Orientation = Orientation.Vertical
						};
						layoutPanel1.Children.Add(floatingWindow.RootPanel);
						layoutPanel1.Children.Add(this._manager.Layout.RootPanel);
						this._manager.Layout.RootPanel = layoutPanel1;
						break;
					}
					else
					{
						LayoutAnchorablePaneGroup layoutAnchorablePaneGroup = floatingWindow.RootPanel;
						if (layoutAnchorablePaneGroup == null || layoutAnchorablePaneGroup.Orientation != Orientation.Vertical)
						{
							this._manager.Layout.RootPanel.Children.Insert(0, floatingWindow.RootPanel);
							break;
						}
						else
						{
							ILayoutAnchorablePane[] layoutAnchorablePaneArray = layoutAnchorablePaneGroup.Children.ToArray<ILayoutAnchorablePane>();
							for (int j = 0; j < (int)layoutAnchorablePaneArray.Length; j++)
							{
								this._manager.Layout.RootPanel.Children.Insert(j, layoutAnchorablePaneArray[j]);
							}
							break;
						}
					}
				}
				case DropTargetType.DockingManagerDockRight:
				{
					if (this._manager.Layout.RootPanel.Orientation != Orientation.Horizontal && this._manager.Layout.RootPanel.Children.Count == 1)
					{
						this._manager.Layout.RootPanel.Orientation = Orientation.Horizontal;
					}
					if (this._manager.Layout.RootPanel.Orientation != Orientation.Horizontal)
					{
						LayoutPanel layoutPanel2 = new LayoutPanel()
						{
							Orientation = Orientation.Horizontal
						};
						layoutPanel2.Children.Add(this._manager.Layout.RootPanel);
						layoutPanel2.Children.Add(floatingWindow.RootPanel);
						this._manager.Layout.RootPanel = layoutPanel2;
						break;
					}
					else
					{
						LayoutAnchorablePaneGroup rootPanel1 = floatingWindow.RootPanel;
						if (rootPanel1 == null || rootPanel1.Orientation != Orientation.Horizontal)
						{
							this._manager.Layout.RootPanel.Children.Add(floatingWindow.RootPanel);
							break;
						}
						else
						{
							ILayoutAnchorablePane[] array1 = rootPanel1.Children.ToArray<ILayoutAnchorablePane>();
							for (int k = 0; k < (int)array1.Length; k++)
							{
								this._manager.Layout.RootPanel.Children.Add(array1[k]);
							}
							break;
						}
					}
				}
				case DropTargetType.DockingManagerDockBottom:
				{
					if (this._manager.Layout.RootPanel.Orientation != Orientation.Vertical && this._manager.Layout.RootPanel.Children.Count == 1)
					{
						this._manager.Layout.RootPanel.Orientation = Orientation.Vertical;
					}
					if (this._manager.Layout.RootPanel.Orientation != Orientation.Vertical)
					{
						LayoutPanel layoutPanel3 = new LayoutPanel()
						{
							Orientation = Orientation.Vertical
						};
						layoutPanel3.Children.Add(this._manager.Layout.RootPanel);
						layoutPanel3.Children.Add(floatingWindow.RootPanel);
						this._manager.Layout.RootPanel = layoutPanel3;
						break;
					}
					else
					{
						LayoutAnchorablePaneGroup layoutAnchorablePaneGroup1 = floatingWindow.RootPanel;
						if (layoutAnchorablePaneGroup1 == null || layoutAnchorablePaneGroup1.Orientation != Orientation.Vertical)
						{
							this._manager.Layout.RootPanel.Children.Add(floatingWindow.RootPanel);
							break;
						}
						else
						{
							ILayoutAnchorablePane[] layoutAnchorablePaneArray1 = layoutAnchorablePaneGroup1.Children.ToArray<ILayoutAnchorablePane>();
							for (int l = 0; l < (int)layoutAnchorablePaneArray1.Length; l++)
							{
								this._manager.Layout.RootPanel.Children.Add(layoutAnchorablePaneArray1[l]);
							}
							break;
						}
					}
				}
			}
			base.Drop(floatingWindow);
		}

		public override Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindowModel)
		{
			LayoutAnchorableFloatingWindow layoutAnchorableFloatingWindow = floatingWindowModel as LayoutAnchorableFloatingWindow;
			ILayoutPositionableElement rootPanel = layoutAnchorableFloatingWindow.RootPanel;
			ILayoutPositionableElementWithActualSize layoutPositionableElementWithActualSize = layoutAnchorableFloatingWindow.RootPanel;
			Rect screenArea = base.TargetElement.GetScreenArea();
			switch (base.Type)
			{
				case DropTargetType.DockingManagerDockLeft:
				{
					double num = (rootPanel.DockWidth.IsAbsolute ? rootPanel.DockWidth.Value : layoutPositionableElementWithActualSize.ActualWidth);
					return new RectangleGeometry(new Rect(screenArea.Left - overlayWindow.Left, screenArea.Top - overlayWindow.Top, Math.Min(num, screenArea.Width / 2), screenArea.Height));
				}
				case DropTargetType.DockingManagerDockTop:
				{
					double num1 = (rootPanel.DockHeight.IsAbsolute ? rootPanel.DockHeight.Value : layoutPositionableElementWithActualSize.ActualHeight);
					return new RectangleGeometry(new Rect(screenArea.Left - overlayWindow.Left, screenArea.Top - overlayWindow.Top, screenArea.Width, Math.Min(num1, screenArea.Height / 2)));
				}
				case DropTargetType.DockingManagerDockRight:
				{
					double num2 = (rootPanel.DockWidth.IsAbsolute ? rootPanel.DockWidth.Value : layoutPositionableElementWithActualSize.ActualWidth);
					return new RectangleGeometry(new Rect(screenArea.Right - overlayWindow.Left - Math.Min(num2, screenArea.Width / 2), screenArea.Top - overlayWindow.Top, Math.Min(num2, screenArea.Width / 2), screenArea.Height));
				}
				case DropTargetType.DockingManagerDockBottom:
				{
					double num3 = (rootPanel.DockHeight.IsAbsolute ? rootPanel.DockHeight.Value : layoutPositionableElementWithActualSize.ActualHeight);
					return new RectangleGeometry(new Rect(screenArea.Left - overlayWindow.Left, screenArea.Bottom - overlayWindow.Top - Math.Min(num3, screenArea.Height / 2), screenArea.Width, Math.Min(num3, screenArea.Height / 2)));
				}
			}
			throw new InvalidOperationException();
		}
	}
}