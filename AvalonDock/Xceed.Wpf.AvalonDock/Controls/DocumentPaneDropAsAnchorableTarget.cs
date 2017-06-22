using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal class DocumentPaneDropAsAnchorableTarget : DropTarget<LayoutDocumentPaneControl>
	{
		private LayoutDocumentPaneControl _targetPane;

		private int _tabIndex = -1;

		internal DocumentPaneDropAsAnchorableTarget(LayoutDocumentPaneControl paneControl, Rect detectionRect, DropTargetType type) : base(paneControl, detectionRect, type)
		{
			this._targetPane = paneControl;
		}

		internal DocumentPaneDropAsAnchorableTarget(LayoutDocumentPaneControl paneControl, Rect detectionRect, DropTargetType type, int tabIndex) : base(paneControl, detectionRect, type)
		{
			this._targetPane = paneControl;
			this._tabIndex = tabIndex;
		}

		protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
		{
			LayoutDocumentPaneGroup layoutDocumentPaneGroup;
			LayoutPanel layoutPanel;
			ILayoutElement layoutElement;
			ILayoutPanelElement layoutPanelElement;
			ILayoutElement layoutElement1;
			ILayoutElement layoutElement2;
			ILayoutPanelElement layoutPanelElement1;
			ILayoutElement layoutElement3;
			ILayoutElement layoutElement4;
			ILayoutPanelElement layoutPanelElement2;
			ILayoutElement layoutElement5;
			ILayoutElement layoutElement6;
			ILayoutPanelElement layoutPanelElement3;
			ILayoutElement layoutElement7;
			ILayoutDocumentPane model = this._targetPane.Model as ILayoutDocumentPane;
			this.FindParentLayoutDocumentPane(model, out layoutDocumentPaneGroup, out layoutPanel);
			switch (base.Type)
			{
				case DropTargetType.DocumentPaneDockAsAnchorableLeft:
				{
					if (layoutPanel != null && layoutPanel.ChildrenCount == 1)
					{
						layoutPanel.Orientation = Orientation.Horizontal;
					}
					if (layoutPanel == null || layoutPanel.Orientation != Orientation.Horizontal)
					{
						if (layoutPanel == null)
						{
							throw new NotImplementedException();
						}
						LayoutPanel layoutPanel1 = new LayoutPanel()
						{
							Orientation = Orientation.Horizontal
						};
						LayoutPanel layoutPanel2 = layoutPanel;
						if (layoutDocumentPaneGroup != null)
						{
							layoutElement = layoutDocumentPaneGroup;
						}
						else
						{
							layoutElement = model;
						}
						layoutPanel2.ReplaceChild(layoutElement, layoutPanel1);
						ObservableCollection<ILayoutPanelElement> children = layoutPanel1.Children;
						if (layoutDocumentPaneGroup != null)
						{
							layoutPanelElement = layoutDocumentPaneGroup;
						}
						else
						{
							layoutPanelElement = model;
						}
						children.Add(layoutPanelElement);
						layoutPanel1.Children.Insert(0, floatingWindow.RootPanel);
						break;
					}
					else
					{
						ObservableCollection<ILayoutPanelElement> observableCollection = layoutPanel.Children;
						LayoutPanel layoutPanel3 = layoutPanel;
						if (layoutDocumentPaneGroup != null)
						{
							layoutElement1 = layoutDocumentPaneGroup;
						}
						else
						{
							layoutElement1 = model;
						}
						observableCollection.Insert(layoutPanel3.IndexOfChild(layoutElement1), floatingWindow.RootPanel);
						break;
					}
				}
				case DropTargetType.DocumentPaneDockAsAnchorableTop:
				{
					if (layoutPanel != null && layoutPanel.ChildrenCount == 1)
					{
						layoutPanel.Orientation = Orientation.Vertical;
					}
					if (layoutPanel == null || layoutPanel.Orientation != Orientation.Vertical)
					{
						if (layoutPanel == null)
						{
							throw new NotImplementedException();
						}
						LayoutPanel layoutPanel4 = new LayoutPanel()
						{
							Orientation = Orientation.Vertical
						};
						LayoutPanel layoutPanel5 = layoutPanel;
						if (layoutDocumentPaneGroup != null)
						{
							layoutElement2 = layoutDocumentPaneGroup;
						}
						else
						{
							layoutElement2 = model;
						}
						layoutPanel5.ReplaceChild(layoutElement2, layoutPanel4);
						ObservableCollection<ILayoutPanelElement> children1 = layoutPanel4.Children;
						if (layoutDocumentPaneGroup != null)
						{
							layoutPanelElement1 = layoutDocumentPaneGroup;
						}
						else
						{
							layoutPanelElement1 = model;
						}
						children1.Add(layoutPanelElement1);
						layoutPanel4.Children.Insert(0, floatingWindow.RootPanel);
						break;
					}
					else
					{
						ObservableCollection<ILayoutPanelElement> observableCollection1 = layoutPanel.Children;
						LayoutPanel layoutPanel6 = layoutPanel;
						if (layoutDocumentPaneGroup != null)
						{
							layoutElement3 = layoutDocumentPaneGroup;
						}
						else
						{
							layoutElement3 = model;
						}
						observableCollection1.Insert(layoutPanel6.IndexOfChild(layoutElement3), floatingWindow.RootPanel);
						break;
					}
				}
				case DropTargetType.DocumentPaneDockAsAnchorableRight:
				{
					if (layoutPanel != null && layoutPanel.ChildrenCount == 1)
					{
						layoutPanel.Orientation = Orientation.Horizontal;
					}
					if (layoutPanel == null || layoutPanel.Orientation != Orientation.Horizontal)
					{
						if (layoutPanel == null)
						{
							throw new NotImplementedException();
						}
						LayoutPanel layoutPanel7 = new LayoutPanel()
						{
							Orientation = Orientation.Horizontal
						};
						LayoutPanel layoutPanel8 = layoutPanel;
						if (layoutDocumentPaneGroup != null)
						{
							layoutElement4 = layoutDocumentPaneGroup;
						}
						else
						{
							layoutElement4 = model;
						}
						layoutPanel8.ReplaceChild(layoutElement4, layoutPanel7);
						ObservableCollection<ILayoutPanelElement> children2 = layoutPanel7.Children;
						if (layoutDocumentPaneGroup != null)
						{
							layoutPanelElement2 = layoutDocumentPaneGroup;
						}
						else
						{
							layoutPanelElement2 = model;
						}
						children2.Add(layoutPanelElement2);
						layoutPanel7.Children.Add(floatingWindow.RootPanel);
						break;
					}
					else
					{
						ObservableCollection<ILayoutPanelElement> observableCollection2 = layoutPanel.Children;
						LayoutPanel layoutPanel9 = layoutPanel;
						if (layoutDocumentPaneGroup != null)
						{
							layoutElement5 = layoutDocumentPaneGroup;
						}
						else
						{
							layoutElement5 = model;
						}
						observableCollection2.Insert(layoutPanel9.IndexOfChild(layoutElement5) + 1, floatingWindow.RootPanel);
						break;
					}
				}
				case DropTargetType.DocumentPaneDockAsAnchorableBottom:
				{
					if (layoutPanel != null && layoutPanel.ChildrenCount == 1)
					{
						layoutPanel.Orientation = Orientation.Vertical;
					}
					if (layoutPanel == null || layoutPanel.Orientation != Orientation.Vertical)
					{
						if (layoutPanel == null)
						{
							throw new NotImplementedException();
						}
						LayoutPanel layoutPanel10 = new LayoutPanel()
						{
							Orientation = Orientation.Vertical
						};
						LayoutPanel layoutPanel11 = layoutPanel;
						if (layoutDocumentPaneGroup != null)
						{
							layoutElement6 = layoutDocumentPaneGroup;
						}
						else
						{
							layoutElement6 = model;
						}
						layoutPanel11.ReplaceChild(layoutElement6, layoutPanel10);
						ObservableCollection<ILayoutPanelElement> children3 = layoutPanel10.Children;
						if (layoutDocumentPaneGroup != null)
						{
							layoutPanelElement3 = layoutDocumentPaneGroup;
						}
						else
						{
							layoutPanelElement3 = model;
						}
						children3.Add(layoutPanelElement3);
						layoutPanel10.Children.Add(floatingWindow.RootPanel);
						break;
					}
					else
					{
						ObservableCollection<ILayoutPanelElement> observableCollection3 = layoutPanel.Children;
						LayoutPanel layoutPanel12 = layoutPanel;
						if (layoutDocumentPaneGroup != null)
						{
							layoutElement7 = layoutDocumentPaneGroup;
						}
						else
						{
							layoutElement7 = model;
						}
						observableCollection3.Insert(layoutPanel12.IndexOfChild(layoutElement7) + 1, floatingWindow.RootPanel);
						break;
					}
				}
			}
			base.Drop(floatingWindow);
		}

		private bool FindParentLayoutDocumentPane(ILayoutDocumentPane documentPane, out LayoutDocumentPaneGroup containerPaneGroup, out LayoutPanel containerPanel)
		{
			containerPaneGroup = null;
			containerPanel = null;
			if (documentPane.Parent is LayoutPanel)
			{
				containerPaneGroup = null;
				containerPanel = documentPane.Parent as LayoutPanel;
				return true;
			}
			if (!(documentPane.Parent is LayoutDocumentPaneGroup))
			{
				return false;
			}
			LayoutDocumentPaneGroup parent = documentPane.Parent as LayoutDocumentPaneGroup;
			do
			{
				if (parent.Parent is LayoutPanel)
				{
					break;
				}
				parent = parent.Parent as LayoutDocumentPaneGroup;
			}
			while (parent != null);
			if (parent == null)
			{
				return false;
			}
			containerPaneGroup = parent;
			containerPanel = parent.Parent as LayoutPanel;
			return true;
		}

		public override Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindowModel)
		{
			LayoutDocumentPaneGroup layoutDocumentPaneGroup;
			LayoutPanel layoutPanel;
			ILayoutDocumentPane model = this._targetPane.Model as ILayoutDocumentPane;
			DockingManager manager = model.Root.Manager;
			if (!this.FindParentLayoutDocumentPane(model, out layoutDocumentPaneGroup, out layoutPanel))
			{
				return null;
			}
			Rect screenArea = (manager.FindLogicalChildren<FrameworkElement>().OfType<ILayoutControl>().First<ILayoutControl>((ILayoutControl d) => {
				if (layoutDocumentPaneGroup == null)
				{
					return d.Model == layoutPanel;
				}
				return d.Model == layoutDocumentPaneGroup;
			}) as FrameworkElement).GetScreenArea();
			switch (base.Type)
			{
				case DropTargetType.DocumentPaneDockAsAnchorableLeft:
				{
					screenArea.Offset(-overlayWindow.Left, -overlayWindow.Top);
					screenArea.Width = screenArea.Width / 3;
					return new RectangleGeometry(screenArea);
				}
				case DropTargetType.DocumentPaneDockAsAnchorableTop:
				{
					screenArea.Offset(-overlayWindow.Left, -overlayWindow.Top);
					screenArea.Height = screenArea.Height / 3;
					return new RectangleGeometry(screenArea);
				}
				case DropTargetType.DocumentPaneDockAsAnchorableRight:
				{
					screenArea.Offset(-overlayWindow.Left, -overlayWindow.Top);
					screenArea.Offset(screenArea.Width - screenArea.Width / 3, 0);
					screenArea.Width = screenArea.Width / 3;
					return new RectangleGeometry(screenArea);
				}
				case DropTargetType.DocumentPaneDockAsAnchorableBottom:
				{
					screenArea.Offset(-overlayWindow.Left, -overlayWindow.Top);
					screenArea.Offset(0, screenArea.Height - screenArea.Height / 3);
					screenArea.Height = screenArea.Height / 3;
					return new RectangleGeometry(screenArea);
				}
			}
			return null;
		}
	}
}