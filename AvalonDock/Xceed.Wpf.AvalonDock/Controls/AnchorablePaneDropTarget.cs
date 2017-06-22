using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal class AnchorablePaneDropTarget : DropTarget<LayoutAnchorablePaneControl>
	{
		private LayoutAnchorablePaneControl _targetPane;

		private int _tabIndex = -1;

		internal AnchorablePaneDropTarget(LayoutAnchorablePaneControl paneControl, Rect detectionRect, DropTargetType type) : base(paneControl, detectionRect, type)
		{
			this._targetPane = paneControl;
		}

		internal AnchorablePaneDropTarget(LayoutAnchorablePaneControl paneControl, Rect detectionRect, DropTargetType type, int tabIndex) : base(paneControl, detectionRect, type)
		{
			this._targetPane = paneControl;
			this._tabIndex = tabIndex;
		}

		protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
		{
			ILayoutAnchorablePane model = this._targetPane.Model as ILayoutAnchorablePane;
			LayoutAnchorable layoutAnchorable = floatingWindow.Descendents().OfType<LayoutAnchorable>().FirstOrDefault<LayoutAnchorable>();
			switch (base.Type)
			{
				case DropTargetType.AnchorablePaneDockLeft:
				{
					ILayoutGroup parent = model.Parent as ILayoutGroup;
					ILayoutOrientableGroup layoutOrientableGroup = model.Parent as ILayoutOrientableGroup;
					int num = parent.IndexOfChild(model);
					if (layoutOrientableGroup.Orientation != Orientation.Horizontal && parent.ChildrenCount == 1)
					{
						layoutOrientableGroup.Orientation = Orientation.Horizontal;
					}
					if (layoutOrientableGroup.Orientation != Orientation.Horizontal)
					{
						ILayoutPositionableElement layoutPositionableElement = model as ILayoutPositionableElement;
						LayoutAnchorablePaneGroup layoutAnchorablePaneGroup = new LayoutAnchorablePaneGroup()
						{
							Orientation = Orientation.Horizontal,
							DockWidth = layoutPositionableElement.DockWidth,
							DockHeight = layoutPositionableElement.DockHeight
						};
						parent.InsertChildAt(num, layoutAnchorablePaneGroup);
						layoutAnchorablePaneGroup.Children.Add(model);
						layoutAnchorablePaneGroup.Children.Insert(0, floatingWindow.RootPanel);
						break;
					}
					else
					{
						LayoutAnchorablePaneGroup rootPanel = floatingWindow.RootPanel;
						if (rootPanel == null || rootPanel.Children.Count != 1 && rootPanel.Orientation != Orientation.Horizontal)
						{
							parent.InsertChildAt(num, floatingWindow.RootPanel);
							break;
						}
						else
						{
							ILayoutAnchorablePane[] array = rootPanel.Children.ToArray<ILayoutAnchorablePane>();
							for (int i = 0; i < (int)array.Length; i++)
							{
								parent.InsertChildAt(num + i, array[i]);
							}
							break;
						}
					}
				}
				case DropTargetType.AnchorablePaneDockTop:
				{
					ILayoutGroup layoutGroup = model.Parent as ILayoutGroup;
					ILayoutOrientableGroup parent1 = model.Parent as ILayoutOrientableGroup;
					int num1 = layoutGroup.IndexOfChild(model);
					if (parent1.Orientation != Orientation.Vertical && layoutGroup.ChildrenCount == 1)
					{
						parent1.Orientation = Orientation.Vertical;
					}
					if (parent1.Orientation != Orientation.Vertical)
					{
						ILayoutPositionableElement layoutPositionableElement1 = model as ILayoutPositionableElement;
						LayoutAnchorablePaneGroup layoutAnchorablePaneGroup1 = new LayoutAnchorablePaneGroup()
						{
							Orientation = Orientation.Vertical,
							DockWidth = layoutPositionableElement1.DockWidth,
							DockHeight = layoutPositionableElement1.DockHeight
						};
						layoutGroup.InsertChildAt(num1, layoutAnchorablePaneGroup1);
						layoutAnchorablePaneGroup1.Children.Add(model);
						layoutAnchorablePaneGroup1.Children.Insert(0, floatingWindow.RootPanel);
						break;
					}
					else
					{
						LayoutAnchorablePaneGroup rootPanel1 = floatingWindow.RootPanel;
						if (rootPanel1 == null || rootPanel1.Children.Count != 1 && rootPanel1.Orientation != Orientation.Vertical)
						{
							layoutGroup.InsertChildAt(num1, floatingWindow.RootPanel);
							break;
						}
						else
						{
							ILayoutAnchorablePane[] layoutAnchorablePaneArray = rootPanel1.Children.ToArray<ILayoutAnchorablePane>();
							for (int j = 0; j < (int)layoutAnchorablePaneArray.Length; j++)
							{
								layoutGroup.InsertChildAt(num1 + j, layoutAnchorablePaneArray[j]);
							}
							break;
						}
					}
				}
				case DropTargetType.AnchorablePaneDockRight:
				{
					ILayoutGroup layoutGroup1 = model.Parent as ILayoutGroup;
					ILayoutOrientableGroup layoutOrientableGroup1 = model.Parent as ILayoutOrientableGroup;
					int num2 = layoutGroup1.IndexOfChild(model);
					if (layoutOrientableGroup1.Orientation != Orientation.Horizontal && layoutGroup1.ChildrenCount == 1)
					{
						layoutOrientableGroup1.Orientation = Orientation.Horizontal;
					}
					if (layoutOrientableGroup1.Orientation != Orientation.Horizontal)
					{
						ILayoutPositionableElement layoutPositionableElement2 = model as ILayoutPositionableElement;
						LayoutAnchorablePaneGroup layoutAnchorablePaneGroup2 = new LayoutAnchorablePaneGroup()
						{
							Orientation = Orientation.Horizontal,
							DockWidth = layoutPositionableElement2.DockWidth,
							DockHeight = layoutPositionableElement2.DockHeight
						};
						layoutGroup1.InsertChildAt(num2, layoutAnchorablePaneGroup2);
						layoutAnchorablePaneGroup2.Children.Add(model);
						layoutAnchorablePaneGroup2.Children.Add(floatingWindow.RootPanel);
						break;
					}
					else
					{
						LayoutAnchorablePaneGroup rootPanel2 = floatingWindow.RootPanel;
						if (rootPanel2 == null || rootPanel2.Children.Count != 1 && rootPanel2.Orientation != Orientation.Horizontal)
						{
							layoutGroup1.InsertChildAt(num2 + 1, floatingWindow.RootPanel);
							break;
						}
						else
						{
							ILayoutAnchorablePane[] array1 = rootPanel2.Children.ToArray<ILayoutAnchorablePane>();
							for (int k = 0; k < (int)array1.Length; k++)
							{
								layoutGroup1.InsertChildAt(num2 + 1 + k, array1[k]);
							}
							break;
						}
					}
				}
				case DropTargetType.AnchorablePaneDockBottom:
				{
					ILayoutGroup parent2 = model.Parent as ILayoutGroup;
					ILayoutOrientableGroup layoutOrientableGroup2 = model.Parent as ILayoutOrientableGroup;
					int num3 = parent2.IndexOfChild(model);
					if (layoutOrientableGroup2.Orientation != Orientation.Vertical && parent2.ChildrenCount == 1)
					{
						layoutOrientableGroup2.Orientation = Orientation.Vertical;
					}
					if (layoutOrientableGroup2.Orientation != Orientation.Vertical)
					{
						ILayoutPositionableElement layoutPositionableElement3 = model as ILayoutPositionableElement;
						LayoutAnchorablePaneGroup layoutAnchorablePaneGroup3 = new LayoutAnchorablePaneGroup()
						{
							Orientation = Orientation.Vertical,
							DockWidth = layoutPositionableElement3.DockWidth,
							DockHeight = layoutPositionableElement3.DockHeight
						};
						parent2.InsertChildAt(num3, layoutAnchorablePaneGroup3);
						layoutAnchorablePaneGroup3.Children.Add(model);
						layoutAnchorablePaneGroup3.Children.Add(floatingWindow.RootPanel);
						break;
					}
					else
					{
						LayoutAnchorablePaneGroup rootPanel3 = floatingWindow.RootPanel;
						if (rootPanel3 == null || rootPanel3.Children.Count != 1 && rootPanel3.Orientation != Orientation.Vertical)
						{
							parent2.InsertChildAt(num3 + 1, floatingWindow.RootPanel);
							break;
						}
						else
						{
							ILayoutAnchorablePane[] layoutAnchorablePaneArray1 = rootPanel3.Children.ToArray<ILayoutAnchorablePane>();
							for (int l = 0; l < (int)layoutAnchorablePaneArray1.Length; l++)
							{
								parent2.InsertChildAt(num3 + 1 + l, layoutAnchorablePaneArray1[l]);
							}
							break;
						}
					}
				}
				case DropTargetType.AnchorablePaneDockInside:
				{
					LayoutAnchorablePane layoutAnchorablePane = model as LayoutAnchorablePane;
					LayoutAnchorablePaneGroup rootPanel4 = floatingWindow.RootPanel;
					int num4 = (this._tabIndex == -1 ? 0 : this._tabIndex);
					LayoutAnchorable[] layoutAnchorableArray = rootPanel4.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>();
					for (int m = 0; m < (int)layoutAnchorableArray.Length; m++)
					{
						LayoutAnchorable layoutAnchorable1 = layoutAnchorableArray[m];
						layoutAnchorablePane.Children.Insert(num4, layoutAnchorable1);
						num4++;
					}
					break;
				}
			}
			layoutAnchorable.IsActive = true;
			base.Drop(floatingWindow);
		}

		public override Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindowModel)
		{
			LayoutAnchorableFloatingWindow layoutAnchorableFloatingWindow = floatingWindowModel as LayoutAnchorableFloatingWindow;
			LayoutAnchorablePaneGroup rootPanel = layoutAnchorableFloatingWindow.RootPanel;
			LayoutAnchorablePaneGroup layoutAnchorablePaneGroup = layoutAnchorableFloatingWindow.RootPanel;
			switch (base.Type)
			{
				case DropTargetType.AnchorablePaneDockLeft:
				{
					Rect screenArea = base.TargetElement.GetScreenArea();
					screenArea.Offset(-overlayWindow.Left, -overlayWindow.Top);
					screenArea.Width = screenArea.Width / 2;
					return new RectangleGeometry(screenArea);
				}
				case DropTargetType.AnchorablePaneDockTop:
				{
					Rect height = base.TargetElement.GetScreenArea();
					height.Offset(-overlayWindow.Left, -overlayWindow.Top);
					height.Height = height.Height / 2;
					return new RectangleGeometry(height);
				}
				case DropTargetType.AnchorablePaneDockRight:
				{
					Rect width = base.TargetElement.GetScreenArea();
					width.Offset(-overlayWindow.Left, -overlayWindow.Top);
					width.Offset(width.Width / 2, 0);
					width.Width = width.Width / 2;
					return new RectangleGeometry(width);
				}
				case DropTargetType.AnchorablePaneDockBottom:
				{
					Rect rect = base.TargetElement.GetScreenArea();
					rect.Offset(-overlayWindow.Left, -overlayWindow.Top);
					rect.Offset(0, rect.Height / 2);
					rect.Height = rect.Height / 2;
					return new RectangleGeometry(rect);
				}
				case DropTargetType.AnchorablePaneDockInside:
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
						StartPoint = screenArea1.TopLeft
					};
					pathFigure.Segments.Add(new LineSegment()
					{
						Point = new Point(screenArea1.Left, rect1.Top)
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
						Point = rect1.BottomRight
					});
					pathFigure.Segments.Add(new LineSegment()
					{
						Point = rect1.TopRight
					});
					pathFigure.Segments.Add(new LineSegment()
					{
						Point = new Point(screenArea1.Right, rect1.Top)
					});
					pathFigure.Segments.Add(new LineSegment()
					{
						Point = screenArea1.TopRight
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