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
	internal class DocumentPaneDropTarget : DropTarget<LayoutDocumentPaneControl>
	{
		private LayoutDocumentPaneControl _targetPane;

		private int _tabIndex = -1;

		internal DocumentPaneDropTarget(LayoutDocumentPaneControl paneControl, Rect detectionRect, DropTargetType type) : base(paneControl, detectionRect, type)
		{
			this._targetPane = paneControl;
		}

		internal DocumentPaneDropTarget(LayoutDocumentPaneControl paneControl, Rect detectionRect, DropTargetType type, int tabIndex) : base(paneControl, detectionRect, type)
		{
			this._targetPane = paneControl;
			this._tabIndex = tabIndex;
		}

		protected override void Drop(LayoutDocumentFloatingWindow floatingWindow)
		{
			ILayoutDocumentPane model = this._targetPane.Model as ILayoutDocumentPane;
			switch (base.Type)
			{
				case DropTargetType.DocumentPaneDockLeft:
				{
					LayoutDocumentPane layoutDocumentPane = new LayoutDocumentPane(floatingWindow.RootDocument);
					LayoutDocumentPaneGroup parent = model.Parent as LayoutDocumentPaneGroup;
					if (parent == null)
					{
						ILayoutContainer layoutContainer = model.Parent;
						LayoutDocumentPaneGroup layoutDocumentPaneGroup = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Horizontal
						};
						layoutContainer.ReplaceChild(model, layoutDocumentPaneGroup);
						layoutDocumentPaneGroup.Children.Add(model);
						layoutDocumentPaneGroup.Children.Insert(0, layoutDocumentPane);
						break;
					}
					else if (!parent.Root.Manager.AllowMixedOrientation || parent.Orientation == Orientation.Horizontal)
					{
						parent.Orientation = Orientation.Horizontal;
						int num = parent.IndexOfChild(model);
						parent.Children.Insert(num, layoutDocumentPane);
						break;
					}
					else
					{
						LayoutDocumentPaneGroup layoutDocumentPaneGroup1 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Horizontal
						};
						parent.ReplaceChild(model, layoutDocumentPaneGroup1);
						layoutDocumentPaneGroup1.Children.Add(layoutDocumentPane);
						layoutDocumentPaneGroup1.Children.Add(model);
						break;
					}
				}
				case DropTargetType.DocumentPaneDockTop:
				{
					LayoutDocumentPane layoutDocumentPane1 = new LayoutDocumentPane(floatingWindow.RootDocument);
					LayoutDocumentPaneGroup parent1 = model.Parent as LayoutDocumentPaneGroup;
					if (parent1 == null)
					{
						ILayoutContainer layoutContainer1 = model.Parent;
						LayoutDocumentPaneGroup layoutDocumentPaneGroup2 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Vertical
						};
						layoutContainer1.ReplaceChild(model, layoutDocumentPaneGroup2);
						layoutDocumentPaneGroup2.Children.Add(model as LayoutDocumentPane);
						layoutDocumentPaneGroup2.Children.Insert(0, layoutDocumentPane1);
						break;
					}
					else if (!parent1.Root.Manager.AllowMixedOrientation || parent1.Orientation == Orientation.Vertical)
					{
						parent1.Orientation = Orientation.Vertical;
						int num1 = parent1.IndexOfChild(model);
						parent1.Children.Insert(num1, layoutDocumentPane1);
						break;
					}
					else
					{
						LayoutDocumentPaneGroup layoutDocumentPaneGroup3 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Vertical
						};
						parent1.ReplaceChild(model, layoutDocumentPaneGroup3);
						layoutDocumentPaneGroup3.Children.Add(layoutDocumentPane1);
						layoutDocumentPaneGroup3.Children.Add(model);
						break;
					}
				}
				case DropTargetType.DocumentPaneDockRight:
				{
					LayoutDocumentPane layoutDocumentPane2 = new LayoutDocumentPane(floatingWindow.RootDocument);
					LayoutDocumentPaneGroup parent2 = model.Parent as LayoutDocumentPaneGroup;
					if (parent2 == null)
					{
						ILayoutContainer layoutContainer2 = model.Parent;
						LayoutDocumentPaneGroup layoutDocumentPaneGroup4 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Horizontal
						};
						layoutContainer2.ReplaceChild(model, layoutDocumentPaneGroup4);
						layoutDocumentPaneGroup4.Children.Add(model as LayoutDocumentPane);
						layoutDocumentPaneGroup4.Children.Add(layoutDocumentPane2);
						break;
					}
					else if (!parent2.Root.Manager.AllowMixedOrientation || parent2.Orientation == Orientation.Horizontal)
					{
						parent2.Orientation = Orientation.Horizontal;
						int num2 = parent2.IndexOfChild(model);
						parent2.Children.Insert(num2 + 1, layoutDocumentPane2);
						break;
					}
					else
					{
						LayoutDocumentPaneGroup layoutDocumentPaneGroup5 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Horizontal
						};
						parent2.ReplaceChild(model, layoutDocumentPaneGroup5);
						layoutDocumentPaneGroup5.Children.Add(model);
						layoutDocumentPaneGroup5.Children.Add(layoutDocumentPane2);
						break;
					}
				}
				case DropTargetType.DocumentPaneDockBottom:
				{
					LayoutDocumentPane layoutDocumentPane3 = new LayoutDocumentPane(floatingWindow.RootDocument);
					LayoutDocumentPaneGroup parent3 = model.Parent as LayoutDocumentPaneGroup;
					if (parent3 == null)
					{
						ILayoutContainer layoutContainer3 = model.Parent;
						LayoutDocumentPaneGroup layoutDocumentPaneGroup6 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Vertical
						};
						layoutContainer3.ReplaceChild(model, layoutDocumentPaneGroup6);
						layoutDocumentPaneGroup6.Children.Add(model as LayoutDocumentPane);
						layoutDocumentPaneGroup6.Children.Add(layoutDocumentPane3);
						break;
					}
					else if (!parent3.Root.Manager.AllowMixedOrientation || parent3.Orientation == Orientation.Vertical)
					{
						parent3.Orientation = Orientation.Vertical;
						int num3 = parent3.IndexOfChild(model);
						parent3.Children.Insert(num3 + 1, layoutDocumentPane3);
						break;
					}
					else
					{
						LayoutDocumentPaneGroup layoutDocumentPaneGroup7 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Vertical
						};
						parent3.ReplaceChild(model, layoutDocumentPaneGroup7);
						layoutDocumentPaneGroup7.Children.Add(model);
						layoutDocumentPaneGroup7.Children.Add(layoutDocumentPane3);
						break;
					}
				}
				case DropTargetType.DocumentPaneDockInside:
				{
					LayoutDocumentPane layoutDocumentPane4 = model as LayoutDocumentPane;
					LayoutDocument rootDocument = floatingWindow.RootDocument;
					int previousContainerIndex = 0;
					if (this._tabIndex != -1)
					{
						previousContainerIndex = this._tabIndex;
					}
					else if (((ILayoutPreviousContainer)rootDocument).PreviousContainer == model && rootDocument.PreviousContainerIndex != -1)
					{
						previousContainerIndex = rootDocument.PreviousContainerIndex;
					}
					rootDocument.IsActive = false;
					layoutDocumentPane4.Children.Insert(previousContainerIndex, rootDocument);
					rootDocument.IsActive = true;
					break;
				}
			}
			base.Drop(floatingWindow);
		}

		protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
		{
			LayoutAnchorable[] array;
			int i;
			ILayoutDocumentPane model = this._targetPane.Model as ILayoutDocumentPane;
			switch (base.Type)
			{
				case DropTargetType.DocumentPaneDockLeft:
				{
					LayoutDocumentPaneGroup parent = model.Parent as LayoutDocumentPaneGroup;
					LayoutDocumentPane layoutDocumentPane = new LayoutDocumentPane();
					if (parent == null)
					{
						ILayoutContainer layoutContainer = model.Parent;
						LayoutDocumentPaneGroup layoutDocumentPaneGroup = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Horizontal
						};
						layoutContainer.ReplaceChild(model, layoutDocumentPaneGroup);
						layoutDocumentPaneGroup.Children.Add(layoutDocumentPane);
						layoutDocumentPaneGroup.Children.Add(model as LayoutDocumentPane);
					}
					else if (!parent.Root.Manager.AllowMixedOrientation || parent.Orientation == Orientation.Horizontal)
					{
						parent.Orientation = Orientation.Horizontal;
						int num = parent.IndexOfChild(model);
						parent.Children.Insert(num, layoutDocumentPane);
					}
					else
					{
						LayoutDocumentPaneGroup layoutDocumentPaneGroup1 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Horizontal
						};
						parent.ReplaceChild(model, layoutDocumentPaneGroup1);
						layoutDocumentPaneGroup1.Children.Add(layoutDocumentPane);
						layoutDocumentPaneGroup1.Children.Add(model);
					}
					array = floatingWindow.RootPanel.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>();
					for (i = 0; i < (int)array.Length; i++)
					{
						LayoutAnchorable layoutAnchorable = array[i];
						layoutDocumentPane.Children.Add(layoutAnchorable);
					}
					break;
				}
				case DropTargetType.DocumentPaneDockTop:
				{
					LayoutDocumentPaneGroup parent1 = model.Parent as LayoutDocumentPaneGroup;
					LayoutDocumentPane layoutDocumentPane1 = new LayoutDocumentPane();
					if (parent1 == null)
					{
						ILayoutContainer layoutContainer1 = model.Parent;
						LayoutDocumentPaneGroup layoutDocumentPaneGroup2 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Vertical
						};
						layoutContainer1.ReplaceChild(model, layoutDocumentPaneGroup2);
						layoutDocumentPaneGroup2.Children.Add(layoutDocumentPane1);
						layoutDocumentPaneGroup2.Children.Add(model as LayoutDocumentPane);
					}
					else if (!parent1.Root.Manager.AllowMixedOrientation || parent1.Orientation == Orientation.Vertical)
					{
						parent1.Orientation = Orientation.Vertical;
						int num1 = parent1.IndexOfChild(model);
						parent1.Children.Insert(num1, layoutDocumentPane1);
					}
					else
					{
						LayoutDocumentPaneGroup layoutDocumentPaneGroup3 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Vertical
						};
						parent1.ReplaceChild(model, layoutDocumentPaneGroup3);
						layoutDocumentPaneGroup3.Children.Add(layoutDocumentPane1);
						layoutDocumentPaneGroup3.Children.Add(model);
					}
					array = floatingWindow.RootPanel.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>();
					for (i = 0; i < (int)array.Length; i++)
					{
						LayoutAnchorable layoutAnchorable1 = array[i];
						layoutDocumentPane1.Children.Add(layoutAnchorable1);
					}
					break;
				}
				case DropTargetType.DocumentPaneDockRight:
				{
					LayoutDocumentPaneGroup parent2 = model.Parent as LayoutDocumentPaneGroup;
					LayoutDocumentPane layoutDocumentPane2 = new LayoutDocumentPane();
					if (parent2 == null)
					{
						ILayoutContainer layoutContainer2 = model.Parent;
						LayoutDocumentPaneGroup layoutDocumentPaneGroup4 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Horizontal
						};
						layoutContainer2.ReplaceChild(model, layoutDocumentPaneGroup4);
						layoutDocumentPaneGroup4.Children.Add(model as LayoutDocumentPane);
						layoutDocumentPaneGroup4.Children.Add(layoutDocumentPane2);
					}
					else if (!parent2.Root.Manager.AllowMixedOrientation || parent2.Orientation == Orientation.Horizontal)
					{
						parent2.Orientation = Orientation.Horizontal;
						int num2 = parent2.IndexOfChild(model);
						parent2.Children.Insert(num2 + 1, layoutDocumentPane2);
					}
					else
					{
						LayoutDocumentPaneGroup layoutDocumentPaneGroup5 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Horizontal
						};
						parent2.ReplaceChild(model, layoutDocumentPaneGroup5);
						layoutDocumentPaneGroup5.Children.Add(model);
						layoutDocumentPaneGroup5.Children.Add(layoutDocumentPane2);
					}
					array = floatingWindow.RootPanel.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>();
					for (i = 0; i < (int)array.Length; i++)
					{
						LayoutAnchorable layoutAnchorable2 = array[i];
						layoutDocumentPane2.Children.Add(layoutAnchorable2);
					}
					break;
				}
				case DropTargetType.DocumentPaneDockBottom:
				{
					LayoutDocumentPaneGroup parent3 = model.Parent as LayoutDocumentPaneGroup;
					LayoutDocumentPane layoutDocumentPane3 = new LayoutDocumentPane();
					if (parent3 == null)
					{
						ILayoutContainer layoutContainer3 = model.Parent;
						LayoutDocumentPaneGroup layoutDocumentPaneGroup6 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Vertical
						};
						layoutContainer3.ReplaceChild(model, layoutDocumentPaneGroup6);
						layoutDocumentPaneGroup6.Children.Add(model as LayoutDocumentPane);
						layoutDocumentPaneGroup6.Children.Add(layoutDocumentPane3);
					}
					else if (!parent3.Root.Manager.AllowMixedOrientation || parent3.Orientation == Orientation.Vertical)
					{
						parent3.Orientation = Orientation.Vertical;
						int num3 = parent3.IndexOfChild(model);
						parent3.Children.Insert(num3 + 1, layoutDocumentPane3);
					}
					else
					{
						LayoutDocumentPaneGroup layoutDocumentPaneGroup7 = new LayoutDocumentPaneGroup()
						{
							Orientation = Orientation.Vertical
						};
						parent3.ReplaceChild(model, layoutDocumentPaneGroup7);
						layoutDocumentPaneGroup7.Children.Add(model);
						layoutDocumentPaneGroup7.Children.Add(layoutDocumentPane3);
					}
					array = floatingWindow.RootPanel.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>();
					for (i = 0; i < (int)array.Length; i++)
					{
						LayoutAnchorable layoutAnchorable3 = array[i];
						layoutDocumentPane3.Children.Add(layoutAnchorable3);
					}
					break;
				}
				case DropTargetType.DocumentPaneDockInside:
				{
					LayoutDocumentPane layoutDocumentPane4 = model as LayoutDocumentPane;
					LayoutAnchorablePaneGroup rootPanel = floatingWindow.RootPanel;
					bool flag = true;
					int previousContainerIndex = 0;
					if (this._tabIndex != -1)
					{
						previousContainerIndex = this._tabIndex;
						flag = false;
					}
					LayoutAnchorable layoutAnchorable4 = null;
					array = rootPanel.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>();
					for (i = 0; i < (int)array.Length; i++)
					{
						LayoutAnchorable layoutAnchorable5 = array[i];
						if (flag)
						{
							if (((ILayoutPreviousContainer)layoutAnchorable5).PreviousContainer == model && layoutAnchorable5.PreviousContainerIndex != -1)
							{
								previousContainerIndex = layoutAnchorable5.PreviousContainerIndex;
							}
							flag = false;
						}
						layoutDocumentPane4.Children.Insert(previousContainerIndex, layoutAnchorable5);
						previousContainerIndex++;
						layoutAnchorable4 = layoutAnchorable5;
					}
					layoutAnchorable4.IsActive = true;
					break;
				}
			}
			base.Drop(floatingWindow);
		}

		public override Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindowModel)
		{
			switch (base.Type)
			{
				case DropTargetType.DocumentPaneDockLeft:
				{
					Rect screenArea = base.TargetElement.GetScreenArea();
					screenArea.Offset(-overlayWindow.Left, -overlayWindow.Top);
					screenArea.Width = screenArea.Width / 2;
					return new RectangleGeometry(screenArea);
				}
				case DropTargetType.DocumentPaneDockTop:
				{
					Rect height = base.TargetElement.GetScreenArea();
					height.Offset(-overlayWindow.Left, -overlayWindow.Top);
					height.Height = height.Height / 2;
					return new RectangleGeometry(height);
				}
				case DropTargetType.DocumentPaneDockRight:
				{
					Rect width = base.TargetElement.GetScreenArea();
					width.Offset(-overlayWindow.Left, -overlayWindow.Top);
					width.Offset(width.Width / 2, 0);
					width.Width = width.Width / 2;
					return new RectangleGeometry(width);
				}
				case DropTargetType.DocumentPaneDockBottom:
				{
					Rect rect = base.TargetElement.GetScreenArea();
					rect.Offset(-overlayWindow.Left, -overlayWindow.Top);
					rect.Offset(0, rect.Height / 2);
					rect.Height = rect.Height / 2;
					return new RectangleGeometry(rect);
				}
				case DropTargetType.DocumentPaneDockInside:
				{
					Rect screenArea1 = base.TargetElement.GetScreenArea();
					screenArea1.Offset(-overlayWindow.Left, -overlayWindow.Top);
					if (this._tabIndex == -1)
					{
						return new RectangleGeometry(screenArea1);
					}
					Rect rect1 = new Rect(base.DetectionRects[0].TopLeft, base.DetectionRects[0].BottomRight);
					rect1.Offset(-overlayWindow.Left, -overlayWindow.Top);
					PathFigure pathFigure = new PathFigure()
					{
						StartPoint = screenArea1.BottomRight
					};
					pathFigure.Segments.Add(new LineSegment()
					{
						Point = new Point(screenArea1.Right, rect1.Bottom)
					});
					pathFigure.Segments.Add(new LineSegment()
					{
						Point = rect1.BottomRight
					});
					pathFigure.Segments.Add(new LineSegment()
					{
						Point = rect1.TopRight
					});
					pathFigure.Segments.Add(new LineSegment()
					{
						Point = rect1.TopLeft
					});
					pathFigure.Segments.Add(new LineSegment()
					{
						Point = rect1.BottomLeft
					});
					pathFigure.Segments.Add(new LineSegment()
					{
						Point = new Point(screenArea1.Left, rect1.Bottom)
					});
					pathFigure.Segments.Add(new LineSegment()
					{
						Point = screenArea1.BottomLeft
					});
					pathFigure.IsClosed = true;
					pathFigure.IsFilled = true;
					pathFigure.Freeze();
					return new PathGeometry(new PathFigure[] { pathFigure });
				}
			}
			return null;
		}
	}
}