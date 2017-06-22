using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class OverlayWindow : Window, IOverlayWindow
	{
		private ResourceDictionary currentThemeResourceDictionary;

		private Canvas _mainCanvasPanel;

		private Grid _gridDockingManagerDropTargets;

		private Grid _gridAnchorablePaneDropTargets;

		private Grid _gridDocumentPaneDropTargets;

		private Grid _gridDocumentPaneFullDropTargets;

		private FrameworkElement _dockingManagerDropTargetBottom;

		private FrameworkElement _dockingManagerDropTargetTop;

		private FrameworkElement _dockingManagerDropTargetLeft;

		private FrameworkElement _dockingManagerDropTargetRight;

		private FrameworkElement _anchorablePaneDropTargetBottom;

		private FrameworkElement _anchorablePaneDropTargetTop;

		private FrameworkElement _anchorablePaneDropTargetLeft;

		private FrameworkElement _anchorablePaneDropTargetRight;

		private FrameworkElement _anchorablePaneDropTargetInto;

		private FrameworkElement _documentPaneDropTargetBottom;

		private FrameworkElement _documentPaneDropTargetTop;

		private FrameworkElement _documentPaneDropTargetLeft;

		private FrameworkElement _documentPaneDropTargetRight;

		private FrameworkElement _documentPaneDropTargetInto;

		private FrameworkElement _documentPaneDropTargetBottomAsAnchorablePane;

		private FrameworkElement _documentPaneDropTargetTopAsAnchorablePane;

		private FrameworkElement _documentPaneDropTargetLeftAsAnchorablePane;

		private FrameworkElement _documentPaneDropTargetRightAsAnchorablePane;

		private FrameworkElement _documentPaneFullDropTargetBottom;

		private FrameworkElement _documentPaneFullDropTargetTop;

		private FrameworkElement _documentPaneFullDropTargetLeft;

		private FrameworkElement _documentPaneFullDropTargetRight;

		private FrameworkElement _documentPaneFullDropTargetInto;

		private Path _previewBox;

		private IOverlayWindowHost _host;

		private LayoutFloatingWindowControl _floatingWindow;

		private List<IDropArea> _visibleAreas = new List<IDropArea>();

		static OverlayWindow()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(typeof(OverlayWindow)));
			Window.AllowsTransparencyProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(true));
			Window.WindowStyleProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata((object)System.Windows.WindowStyle.None));
			Window.ShowInTaskbarProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(false));
			Window.ShowActivatedProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(false));
			UIElement.VisibilityProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata((object)System.Windows.Visibility.Hidden));
		}

		internal OverlayWindow(IOverlayWindowHost host)
		{
			this._host = host;
			this.UpdateThemeResources(null);
		}

		internal void EnableDropTargets()
		{
			if (this._mainCanvasPanel != null)
			{
				this._mainCanvasPanel.Visibility = System.Windows.Visibility.Visible;
			}
		}

		internal void HideDropTargets()
		{
			if (this._mainCanvasPanel != null)
			{
				this._mainCanvasPanel.Visibility = System.Windows.Visibility.Hidden;
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this._mainCanvasPanel = base.GetTemplateChild("PART_DropTargetsContainer") as Canvas;
			this._gridDockingManagerDropTargets = base.GetTemplateChild("PART_DockingManagerDropTargets") as Grid;
			this._gridAnchorablePaneDropTargets = base.GetTemplateChild("PART_AnchorablePaneDropTargets") as Grid;
			this._gridDocumentPaneDropTargets = base.GetTemplateChild("PART_DocumentPaneDropTargets") as Grid;
			this._gridDocumentPaneFullDropTargets = base.GetTemplateChild("PART_DocumentPaneFullDropTargets") as Grid;
			this._gridDockingManagerDropTargets.Visibility = System.Windows.Visibility.Hidden;
			this._gridAnchorablePaneDropTargets.Visibility = System.Windows.Visibility.Hidden;
			this._gridDocumentPaneDropTargets.Visibility = System.Windows.Visibility.Hidden;
			if (this._gridDocumentPaneFullDropTargets != null)
			{
				this._gridDocumentPaneFullDropTargets.Visibility = System.Windows.Visibility.Hidden;
			}
			this._dockingManagerDropTargetBottom = base.GetTemplateChild("PART_DockingManagerDropTargetBottom") as FrameworkElement;
			this._dockingManagerDropTargetTop = base.GetTemplateChild("PART_DockingManagerDropTargetTop") as FrameworkElement;
			this._dockingManagerDropTargetLeft = base.GetTemplateChild("PART_DockingManagerDropTargetLeft") as FrameworkElement;
			this._dockingManagerDropTargetRight = base.GetTemplateChild("PART_DockingManagerDropTargetRight") as FrameworkElement;
			this._anchorablePaneDropTargetBottom = base.GetTemplateChild("PART_AnchorablePaneDropTargetBottom") as FrameworkElement;
			this._anchorablePaneDropTargetTop = base.GetTemplateChild("PART_AnchorablePaneDropTargetTop") as FrameworkElement;
			this._anchorablePaneDropTargetLeft = base.GetTemplateChild("PART_AnchorablePaneDropTargetLeft") as FrameworkElement;
			this._anchorablePaneDropTargetRight = base.GetTemplateChild("PART_AnchorablePaneDropTargetRight") as FrameworkElement;
			this._anchorablePaneDropTargetInto = base.GetTemplateChild("PART_AnchorablePaneDropTargetInto") as FrameworkElement;
			this._documentPaneDropTargetBottom = base.GetTemplateChild("PART_DocumentPaneDropTargetBottom") as FrameworkElement;
			this._documentPaneDropTargetTop = base.GetTemplateChild("PART_DocumentPaneDropTargetTop") as FrameworkElement;
			this._documentPaneDropTargetLeft = base.GetTemplateChild("PART_DocumentPaneDropTargetLeft") as FrameworkElement;
			this._documentPaneDropTargetRight = base.GetTemplateChild("PART_DocumentPaneDropTargetRight") as FrameworkElement;
			this._documentPaneDropTargetInto = base.GetTemplateChild("PART_DocumentPaneDropTargetInto") as FrameworkElement;
			this._documentPaneDropTargetBottomAsAnchorablePane = base.GetTemplateChild("PART_DocumentPaneDropTargetBottomAsAnchorablePane") as FrameworkElement;
			this._documentPaneDropTargetTopAsAnchorablePane = base.GetTemplateChild("PART_DocumentPaneDropTargetTopAsAnchorablePane") as FrameworkElement;
			this._documentPaneDropTargetLeftAsAnchorablePane = base.GetTemplateChild("PART_DocumentPaneDropTargetLeftAsAnchorablePane") as FrameworkElement;
			this._documentPaneDropTargetRightAsAnchorablePane = base.GetTemplateChild("PART_DocumentPaneDropTargetRightAsAnchorablePane") as FrameworkElement;
			this._documentPaneFullDropTargetBottom = base.GetTemplateChild("PART_DocumentPaneFullDropTargetBottom") as FrameworkElement;
			this._documentPaneFullDropTargetTop = base.GetTemplateChild("PART_DocumentPaneFullDropTargetTop") as FrameworkElement;
			this._documentPaneFullDropTargetLeft = base.GetTemplateChild("PART_DocumentPaneFullDropTargetLeft") as FrameworkElement;
			this._documentPaneFullDropTargetRight = base.GetTemplateChild("PART_DocumentPaneFullDropTargetRight") as FrameworkElement;
			this._documentPaneFullDropTargetInto = base.GetTemplateChild("PART_DocumentPaneFullDropTargetInto") as FrameworkElement;
			this._previewBox = base.GetTemplateChild("PART_PreviewBox") as Path;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
		}

		internal void UpdateThemeResources(Theme oldTheme = null)
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
			if (this._host.Manager.Theme != null)
			{
				if (this._host.Manager.Theme is DictionaryTheme)
				{
					this.currentThemeResourceDictionary = ((DictionaryTheme)this._host.Manager.Theme).ThemeResourceDictionary;
					base.Resources.MergedDictionaries.Add(this.currentThemeResourceDictionary);
					return;
				}
				base.Resources.MergedDictionaries.Add(new ResourceDictionary()
				{
					Source = this._host.Manager.Theme.GetResourceUri()
				});
			}
		}

		void Xceed.Wpf.AvalonDock.Controls.IOverlayWindow.DragDrop(IDropTarget target)
		{
			target.Drop(this._floatingWindow.Model as LayoutFloatingWindow);
		}

		void Xceed.Wpf.AvalonDock.Controls.IOverlayWindow.DragEnter(LayoutFloatingWindowControl floatingWindow)
		{
			this._floatingWindow = floatingWindow;
			this.EnableDropTargets();
		}

		void Xceed.Wpf.AvalonDock.Controls.IOverlayWindow.DragEnter(IDropArea area)
		{
			FrameworkElement width;
			System.Windows.Visibility visibility;
			System.Windows.Visibility visibility1;
			System.Windows.Visibility visibility2;
			System.Windows.Visibility visibility3;
			this._visibleAreas.Add(area);
			switch (area.Type)
			{
				case DropAreaType.DockingManager:
				{
					width = this._gridDockingManagerDropTargets;
					break;
				}
				case DropAreaType.DocumentPane:
				{
					if (!(this._floatingWindow.Model is LayoutAnchorableFloatingWindow) || this._gridDocumentPaneFullDropTargets == null)
					{
						width = this._gridDocumentPaneDropTargets;
						LayoutDocumentPane model = (area as DropArea<LayoutDocumentPaneControl>).AreaElement.Model as LayoutDocumentPane;
						LayoutDocumentPaneGroup parent = model.Parent as LayoutDocumentPaneGroup;
						if (parent != null)
						{
							if ((
								from c in parent.Children
								where c.IsVisible
								select c).Count<ILayoutDocumentPane>() > 1)
							{
								if (parent.Root.Manager.AllowMixedOrientation)
								{
									this._documentPaneDropTargetLeft.Visibility = System.Windows.Visibility.Visible;
									this._documentPaneDropTargetRight.Visibility = System.Windows.Visibility.Visible;
									this._documentPaneDropTargetTop.Visibility = System.Windows.Visibility.Visible;
									this._documentPaneDropTargetBottom.Visibility = System.Windows.Visibility.Visible;
									break;
								}
								else
								{
									this._documentPaneDropTargetLeft.Visibility = (parent.Orientation == Orientation.Horizontal ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
									this._documentPaneDropTargetRight.Visibility = (parent.Orientation == Orientation.Horizontal ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
									this._documentPaneDropTargetTop.Visibility = (parent.Orientation == Orientation.Vertical ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
									this._documentPaneDropTargetBottom.Visibility = (parent.Orientation == Orientation.Vertical ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
									break;
								}
							}
						}
						if (parent != null || model == null || model.ChildrenCount != 0)
						{
							this._documentPaneDropTargetLeft.Visibility = System.Windows.Visibility.Visible;
							this._documentPaneDropTargetRight.Visibility = System.Windows.Visibility.Visible;
							this._documentPaneDropTargetTop.Visibility = System.Windows.Visibility.Visible;
							this._documentPaneDropTargetBottom.Visibility = System.Windows.Visibility.Visible;
							break;
						}
						else
						{
							this._documentPaneDropTargetLeft.Visibility = System.Windows.Visibility.Hidden;
							this._documentPaneDropTargetRight.Visibility = System.Windows.Visibility.Hidden;
							this._documentPaneDropTargetTop.Visibility = System.Windows.Visibility.Hidden;
							this._documentPaneDropTargetBottom.Visibility = System.Windows.Visibility.Hidden;
							break;
						}
					}
					else
					{
						width = this._gridDocumentPaneFullDropTargets;
						LayoutDocumentPane layoutDocumentPane = (area as DropArea<LayoutDocumentPaneControl>).AreaElement.Model as LayoutDocumentPane;
						LayoutDocumentPaneGroup layoutDocumentPaneGroup = layoutDocumentPane.Parent as LayoutDocumentPaneGroup;
						if (layoutDocumentPaneGroup != null)
						{
							if ((
								from c in layoutDocumentPaneGroup.Children
								where c.IsVisible
								select c).Count<ILayoutDocumentPane>() <= 1)
							{
								goto Label1;
							}
							if (layoutDocumentPaneGroup.Root.Manager.AllowMixedOrientation)
							{
								this._documentPaneFullDropTargetLeft.Visibility = System.Windows.Visibility.Visible;
								this._documentPaneFullDropTargetRight.Visibility = System.Windows.Visibility.Visible;
								this._documentPaneFullDropTargetTop.Visibility = System.Windows.Visibility.Visible;
								this._documentPaneFullDropTargetBottom.Visibility = System.Windows.Visibility.Visible;
								goto Label0;
							}
							else
							{
								this._documentPaneFullDropTargetLeft.Visibility = (layoutDocumentPaneGroup.Orientation == Orientation.Horizontal ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
								this._documentPaneFullDropTargetRight.Visibility = (layoutDocumentPaneGroup.Orientation == Orientation.Horizontal ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
								this._documentPaneFullDropTargetTop.Visibility = (layoutDocumentPaneGroup.Orientation == Orientation.Vertical ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
								this._documentPaneFullDropTargetBottom.Visibility = (layoutDocumentPaneGroup.Orientation == Orientation.Vertical ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
								goto Label0;
							}
						}
					Label1:
						if (layoutDocumentPaneGroup != null || layoutDocumentPane == null || layoutDocumentPane.ChildrenCount != 0)
						{
							this._documentPaneFullDropTargetLeft.Visibility = System.Windows.Visibility.Visible;
							this._documentPaneFullDropTargetRight.Visibility = System.Windows.Visibility.Visible;
							this._documentPaneFullDropTargetTop.Visibility = System.Windows.Visibility.Visible;
							this._documentPaneFullDropTargetBottom.Visibility = System.Windows.Visibility.Visible;
						}
						else
						{
							this._documentPaneFullDropTargetLeft.Visibility = System.Windows.Visibility.Hidden;
							this._documentPaneFullDropTargetRight.Visibility = System.Windows.Visibility.Hidden;
							this._documentPaneFullDropTargetTop.Visibility = System.Windows.Visibility.Hidden;
							this._documentPaneFullDropTargetBottom.Visibility = System.Windows.Visibility.Hidden;
						}
					Label0:
						if (layoutDocumentPaneGroup != null)
						{
							if ((
								from c in layoutDocumentPaneGroup.Children
								where c.IsVisible
								select c).Count<ILayoutDocumentPane>() > 1)
							{
								int num = (
									from ch in layoutDocumentPaneGroup.Children
									where ch.IsVisible
									select ch).ToList<ILayoutDocumentPane>().IndexOf(layoutDocumentPane);
								bool flag = num == 0;
								bool childrenCount = num == layoutDocumentPaneGroup.ChildrenCount - 1;
								if (layoutDocumentPaneGroup.Root.Manager.AllowMixedOrientation)
								{
									this._documentPaneDropTargetBottomAsAnchorablePane.Visibility = System.Windows.Visibility.Visible;
									this._documentPaneDropTargetLeftAsAnchorablePane.Visibility = System.Windows.Visibility.Visible;
									this._documentPaneDropTargetRightAsAnchorablePane.Visibility = System.Windows.Visibility.Visible;
									this._documentPaneDropTargetTopAsAnchorablePane.Visibility = System.Windows.Visibility.Visible;
									break;
								}
								else
								{
									FrameworkElement frameworkElement = this._documentPaneDropTargetBottomAsAnchorablePane;
									if (layoutDocumentPaneGroup.Orientation == Orientation.Vertical)
									{
										visibility = (childrenCount ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
									}
									else
									{
										visibility = System.Windows.Visibility.Hidden;
									}
									frameworkElement.Visibility = visibility;
									FrameworkElement frameworkElement1 = this._documentPaneDropTargetTopAsAnchorablePane;
									if (layoutDocumentPaneGroup.Orientation == Orientation.Vertical)
									{
										visibility1 = (flag ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
									}
									else
									{
										visibility1 = System.Windows.Visibility.Hidden;
									}
									frameworkElement1.Visibility = visibility1;
									FrameworkElement frameworkElement2 = this._documentPaneDropTargetLeftAsAnchorablePane;
									if (layoutDocumentPaneGroup.Orientation == Orientation.Horizontal)
									{
										visibility2 = (flag ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
									}
									else
									{
										visibility2 = System.Windows.Visibility.Hidden;
									}
									frameworkElement2.Visibility = visibility2;
									FrameworkElement frameworkElement3 = this._documentPaneDropTargetRightAsAnchorablePane;
									if (layoutDocumentPaneGroup.Orientation == Orientation.Horizontal)
									{
										visibility3 = (childrenCount ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
									}
									else
									{
										visibility3 = System.Windows.Visibility.Hidden;
									}
									frameworkElement3.Visibility = visibility3;
									break;
								}
							}
						}
						this._documentPaneDropTargetBottomAsAnchorablePane.Visibility = System.Windows.Visibility.Visible;
						this._documentPaneDropTargetLeftAsAnchorablePane.Visibility = System.Windows.Visibility.Visible;
						this._documentPaneDropTargetRightAsAnchorablePane.Visibility = System.Windows.Visibility.Visible;
						this._documentPaneDropTargetTopAsAnchorablePane.Visibility = System.Windows.Visibility.Visible;
						break;
					}
				}
				case DropAreaType.DocumentPaneGroup:
				{
					width = this._gridDocumentPaneDropTargets;
					ILayoutContainer layoutContainer = (((area as DropArea<LayoutDocumentPaneGroupControl>).AreaElement.Model as LayoutDocumentPaneGroup).Children.First<ILayoutDocumentPane>() as LayoutDocumentPane).Parent;
					this._documentPaneDropTargetLeft.Visibility = System.Windows.Visibility.Hidden;
					this._documentPaneDropTargetRight.Visibility = System.Windows.Visibility.Hidden;
					this._documentPaneDropTargetTop.Visibility = System.Windows.Visibility.Hidden;
					this._documentPaneDropTargetBottom.Visibility = System.Windows.Visibility.Hidden;
					break;
				}
				case DropAreaType.AnchorablePane:
				{
					width = this._gridAnchorablePaneDropTargets;
					break;
				}
				default:
				{
					goto case DropAreaType.DocumentPane;
				}
			}
			Rect detectionRect = area.DetectionRect;
			Canvas.SetLeft(width, detectionRect.Left - base.Left);
			detectionRect = area.DetectionRect;
			Canvas.SetTop(width, detectionRect.Top - base.Top);
			width.Width = area.DetectionRect.Width;
			width.Height = area.DetectionRect.Height;
			width.Visibility = System.Windows.Visibility.Visible;
		}

		void Xceed.Wpf.AvalonDock.Controls.IOverlayWindow.DragEnter(IDropTarget target)
		{
			Geometry previewPath = target.GetPreviewPath(this, this._floatingWindow.Model as LayoutFloatingWindow);
			if (previewPath != null)
			{
				this._previewBox.Data = previewPath;
				this._previewBox.Visibility = System.Windows.Visibility.Visible;
			}
		}

		void Xceed.Wpf.AvalonDock.Controls.IOverlayWindow.DragLeave(LayoutFloatingWindowControl floatingWindow)
		{
			base.Visibility = System.Windows.Visibility.Hidden;
			this._floatingWindow = null;
		}

		void Xceed.Wpf.AvalonDock.Controls.IOverlayWindow.DragLeave(IDropArea area)
		{
			FrameworkElement frameworkElement;
			this._visibleAreas.Remove(area);
			switch (area.Type)
			{
				case DropAreaType.DockingManager:
				{
					frameworkElement = this._gridDockingManagerDropTargets;
					break;
				}
				case DropAreaType.DocumentPane:
				{
					if (!(this._floatingWindow.Model is LayoutAnchorableFloatingWindow) || this._gridDocumentPaneFullDropTargets == null)
					{
						frameworkElement = this._gridDocumentPaneDropTargets;
						break;
					}
					else
					{
						frameworkElement = this._gridDocumentPaneFullDropTargets;
						break;
					}
				}
				case DropAreaType.DocumentPaneGroup:
				{
					frameworkElement = this._gridDocumentPaneDropTargets;
					break;
				}
				case DropAreaType.AnchorablePane:
				{
					frameworkElement = this._gridAnchorablePaneDropTargets;
					break;
				}
				default:
				{
					goto case DropAreaType.DocumentPane;
				}
			}
			frameworkElement.Visibility = System.Windows.Visibility.Hidden;
		}

		void Xceed.Wpf.AvalonDock.Controls.IOverlayWindow.DragLeave(IDropTarget target)
		{
			this._previewBox.Visibility = System.Windows.Visibility.Hidden;
		}

		IEnumerable<IDropTarget> Xceed.Wpf.AvalonDock.Controls.IOverlayWindow.GetTargets()
		{
			LayoutDocumentTabItem layoutDocumentTabItem;
			LayoutDocumentTabItem layoutDocumentTabItem1;
			LayoutAnchorableTabItem layoutAnchorableTabItem;
			foreach (IDropArea _visibleArea in this._visibleAreas)
			{
				switch (_visibleArea.Type)
				{
					case DropAreaType.DockingManager:
					{
						DropArea<DockingManager> dropArea = _visibleArea as DropArea<DockingManager>;
						yield return new DockingManagerDropTarget(dropArea.AreaElement, this._dockingManagerDropTargetLeft.GetScreenArea(), DropTargetType.DockingManagerDockLeft);
						yield return new DockingManagerDropTarget(dropArea.AreaElement, this._dockingManagerDropTargetTop.GetScreenArea(), DropTargetType.DockingManagerDockTop);
						yield return new DockingManagerDropTarget(dropArea.AreaElement, this._dockingManagerDropTargetBottom.GetScreenArea(), DropTargetType.DockingManagerDockBottom);
						yield return new DockingManagerDropTarget(dropArea.AreaElement, this._dockingManagerDropTargetRight.GetScreenArea(), DropTargetType.DockingManagerDockRight);
						dropArea = null;
						break;
					}
					case DropAreaType.DocumentPane:
					{
						if (!(this._floatingWindow.Model is LayoutAnchorableFloatingWindow) || this._gridDocumentPaneFullDropTargets == null)
						{
							DropArea<LayoutDocumentPaneControl> dropArea1 = _visibleArea as DropArea<LayoutDocumentPaneControl>;
							if (this._documentPaneDropTargetLeft.IsVisible)
							{
								yield return new DocumentPaneDropTarget(dropArea1.AreaElement, this._documentPaneDropTargetLeft.GetScreenArea(), DropTargetType.DocumentPaneDockLeft);
							}
							if (this._documentPaneDropTargetTop.IsVisible)
							{
								yield return new DocumentPaneDropTarget(dropArea1.AreaElement, this._documentPaneDropTargetTop.GetScreenArea(), DropTargetType.DocumentPaneDockTop);
							}
							if (this._documentPaneDropTargetRight.IsVisible)
							{
								yield return new DocumentPaneDropTarget(dropArea1.AreaElement, this._documentPaneDropTargetRight.GetScreenArea(), DropTargetType.DocumentPaneDockRight);
							}
							if (this._documentPaneDropTargetBottom.IsVisible)
							{
								yield return new DocumentPaneDropTarget(dropArea1.AreaElement, this._documentPaneDropTargetBottom.GetScreenArea(), DropTargetType.DocumentPaneDockBottom);
							}
							if (this._documentPaneDropTargetInto.IsVisible)
							{
								yield return new DocumentPaneDropTarget(dropArea1.AreaElement, this._documentPaneDropTargetInto.GetScreenArea(), DropTargetType.DocumentPaneDockInside);
							}
							LayoutDocumentPane model = dropArea1.AreaElement.Model as LayoutDocumentPane;
							LayoutDocumentTabItem layoutDocumentTabItem2 = null;
							foreach (LayoutDocumentTabItem layoutDocumentTabItem3 in dropArea1.AreaElement.FindVisualChildren<LayoutDocumentTabItem>())
							{
								LayoutContent layoutContent = layoutDocumentTabItem3.Model;
								layoutDocumentTabItem = (layoutDocumentTabItem2 == null || layoutDocumentTabItem2.GetScreenArea().Right < layoutDocumentTabItem3.GetScreenArea().Right ? layoutDocumentTabItem3 : layoutDocumentTabItem2);
								layoutDocumentTabItem2 = layoutDocumentTabItem;
								int num = model.Children.IndexOf(layoutContent);
								yield return new DocumentPaneDropTarget(dropArea1.AreaElement, layoutDocumentTabItem3.GetScreenArea(), DropTargetType.DocumentPaneDockInside, num);
							}
							if (layoutDocumentTabItem2 != null)
							{
								Rect screenArea = layoutDocumentTabItem2.GetScreenArea();
								Rect rect = new Rect(screenArea.TopRight, new Point(screenArea.Right + screenArea.Width, screenArea.Bottom));
								if (rect.Right < dropArea1.AreaElement.GetScreenArea().Right)
								{
									yield return new DocumentPaneDropTarget(dropArea1.AreaElement, rect, DropTargetType.DocumentPaneDockInside, model.Children.Count);
								}
							}
							dropArea1 = null;
							model = null;
							layoutDocumentTabItem2 = null;
							break;
						}
						else
						{
							DropArea<LayoutDocumentPaneControl> dropArea2 = _visibleArea as DropArea<LayoutDocumentPaneControl>;
							if (this._documentPaneFullDropTargetLeft.IsVisible)
							{
								yield return new DocumentPaneDropTarget(dropArea2.AreaElement, this._documentPaneFullDropTargetLeft.GetScreenArea(), DropTargetType.DocumentPaneDockLeft);
							}
							if (this._documentPaneFullDropTargetTop.IsVisible)
							{
								yield return new DocumentPaneDropTarget(dropArea2.AreaElement, this._documentPaneFullDropTargetTop.GetScreenArea(), DropTargetType.DocumentPaneDockTop);
							}
							if (this._documentPaneFullDropTargetRight.IsVisible)
							{
								yield return new DocumentPaneDropTarget(dropArea2.AreaElement, this._documentPaneFullDropTargetRight.GetScreenArea(), DropTargetType.DocumentPaneDockRight);
							}
							if (this._documentPaneFullDropTargetBottom.IsVisible)
							{
								yield return new DocumentPaneDropTarget(dropArea2.AreaElement, this._documentPaneFullDropTargetBottom.GetScreenArea(), DropTargetType.DocumentPaneDockBottom);
							}
							if (this._documentPaneFullDropTargetInto.IsVisible)
							{
								yield return new DocumentPaneDropTarget(dropArea2.AreaElement, this._documentPaneFullDropTargetInto.GetScreenArea(), DropTargetType.DocumentPaneDockInside);
							}
							LayoutDocumentPane layoutDocumentPane = dropArea2.AreaElement.Model as LayoutDocumentPane;
							LayoutDocumentTabItem layoutDocumentTabItem4 = null;
							foreach (LayoutDocumentTabItem layoutDocumentTabItem5 in dropArea2.AreaElement.FindVisualChildren<LayoutDocumentTabItem>())
							{
								LayoutContent model1 = layoutDocumentTabItem5.Model;
								layoutDocumentTabItem1 = (layoutDocumentTabItem4 == null || layoutDocumentTabItem4.GetScreenArea().Right < layoutDocumentTabItem5.GetScreenArea().Right ? layoutDocumentTabItem5 : layoutDocumentTabItem4);
								layoutDocumentTabItem4 = layoutDocumentTabItem1;
								int num1 = layoutDocumentPane.Children.IndexOf(model1);
								yield return new DocumentPaneDropTarget(dropArea2.AreaElement, layoutDocumentTabItem5.GetScreenArea(), DropTargetType.DocumentPaneDockInside, num1);
							}
							if (layoutDocumentTabItem4 != null)
							{
								Rect screenArea1 = layoutDocumentTabItem4.GetScreenArea();
								Rect rect1 = new Rect(screenArea1.TopRight, new Point(screenArea1.Right + screenArea1.Width, screenArea1.Bottom));
								if (rect1.Right < dropArea2.AreaElement.GetScreenArea().Right)
								{
									yield return new DocumentPaneDropTarget(dropArea2.AreaElement, rect1, DropTargetType.DocumentPaneDockInside, layoutDocumentPane.Children.Count);
								}
							}
							if (this._documentPaneDropTargetLeftAsAnchorablePane.IsVisible)
							{
								yield return new DocumentPaneDropAsAnchorableTarget(dropArea2.AreaElement, this._documentPaneDropTargetLeftAsAnchorablePane.GetScreenArea(), DropTargetType.DocumentPaneDockAsAnchorableLeft);
							}
							if (this._documentPaneDropTargetTopAsAnchorablePane.IsVisible)
							{
								yield return new DocumentPaneDropAsAnchorableTarget(dropArea2.AreaElement, this._documentPaneDropTargetTopAsAnchorablePane.GetScreenArea(), DropTargetType.DocumentPaneDockAsAnchorableTop);
							}
							if (this._documentPaneDropTargetRightAsAnchorablePane.IsVisible)
							{
								yield return new DocumentPaneDropAsAnchorableTarget(dropArea2.AreaElement, this._documentPaneDropTargetRightAsAnchorablePane.GetScreenArea(), DropTargetType.DocumentPaneDockAsAnchorableRight);
							}
							if (this._documentPaneDropTargetBottomAsAnchorablePane.IsVisible)
							{
								yield return new DocumentPaneDropAsAnchorableTarget(dropArea2.AreaElement, this._documentPaneDropTargetBottomAsAnchorablePane.GetScreenArea(), DropTargetType.DocumentPaneDockAsAnchorableBottom);
							}
							dropArea2 = null;
							layoutDocumentPane = null;
							layoutDocumentTabItem4 = null;
							break;
						}
					}
					case DropAreaType.DocumentPaneGroup:
					{
						DropArea<LayoutDocumentPaneGroupControl> dropArea3 = _visibleArea as DropArea<LayoutDocumentPaneGroupControl>;
						if (!this._documentPaneDropTargetInto.IsVisible)
						{
							break;
						}
						yield return new DocumentPaneGroupDropTarget(dropArea3.AreaElement, this._documentPaneDropTargetInto.GetScreenArea(), DropTargetType.DocumentPaneGroupDockInside);
						break;
					}
					case DropAreaType.AnchorablePane:
					{
						DropArea<LayoutAnchorablePaneControl> dropArea4 = _visibleArea as DropArea<LayoutAnchorablePaneControl>;
						yield return new AnchorablePaneDropTarget(dropArea4.AreaElement, this._anchorablePaneDropTargetLeft.GetScreenArea(), DropTargetType.AnchorablePaneDockLeft);
						yield return new AnchorablePaneDropTarget(dropArea4.AreaElement, this._anchorablePaneDropTargetTop.GetScreenArea(), DropTargetType.AnchorablePaneDockTop);
						yield return new AnchorablePaneDropTarget(dropArea4.AreaElement, this._anchorablePaneDropTargetRight.GetScreenArea(), DropTargetType.AnchorablePaneDockRight);
						yield return new AnchorablePaneDropTarget(dropArea4.AreaElement, this._anchorablePaneDropTargetBottom.GetScreenArea(), DropTargetType.AnchorablePaneDockBottom);
						yield return new AnchorablePaneDropTarget(dropArea4.AreaElement, this._anchorablePaneDropTargetInto.GetScreenArea(), DropTargetType.AnchorablePaneDockInside);
						LayoutAnchorablePane layoutAnchorablePane = dropArea4.AreaElement.Model as LayoutAnchorablePane;
						LayoutAnchorableTabItem layoutAnchorableTabItem1 = null;
						foreach (LayoutAnchorableTabItem layoutAnchorableTabItem2 in dropArea4.AreaElement.FindVisualChildren<LayoutAnchorableTabItem>())
						{
							LayoutAnchorable layoutAnchorable = layoutAnchorableTabItem2.Model as LayoutAnchorable;
							layoutAnchorableTabItem = (layoutAnchorableTabItem1 == null || layoutAnchorableTabItem1.GetScreenArea().Right < layoutAnchorableTabItem2.GetScreenArea().Right ? layoutAnchorableTabItem2 : layoutAnchorableTabItem1);
							layoutAnchorableTabItem1 = layoutAnchorableTabItem;
							int num2 = layoutAnchorablePane.Children.IndexOf(layoutAnchorable);
							yield return new AnchorablePaneDropTarget(dropArea4.AreaElement, layoutAnchorableTabItem2.GetScreenArea(), DropTargetType.AnchorablePaneDockInside, num2);
						}
						if (layoutAnchorableTabItem1 != null)
						{
							Rect screenArea2 = layoutAnchorableTabItem1.GetScreenArea();
							Rect rect2 = new Rect(screenArea2.TopRight, new Point(screenArea2.Right + screenArea2.Width, screenArea2.Bottom));
							if (rect2.Right < dropArea4.AreaElement.GetScreenArea().Right)
							{
								yield return new AnchorablePaneDropTarget(dropArea4.AreaElement, rect2, DropTargetType.AnchorablePaneDockInside, layoutAnchorablePane.Children.Count);
							}
						}
						AnchorablePaneTitle anchorablePaneTitle = dropArea4.AreaElement.FindVisualChildren<AnchorablePaneTitle>().FirstOrDefault<AnchorablePaneTitle>();
						if (anchorablePaneTitle != null)
						{
							yield return new AnchorablePaneDropTarget(dropArea4.AreaElement, anchorablePaneTitle.GetScreenArea(), DropTargetType.AnchorablePaneDockInside);
						}
						dropArea4 = null;
						layoutAnchorablePane = null;
						layoutAnchorableTabItem1 = null;
						break;
					}
				}
			}
		}
	}
}