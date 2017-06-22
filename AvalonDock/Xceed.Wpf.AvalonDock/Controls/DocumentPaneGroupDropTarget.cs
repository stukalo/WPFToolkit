using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal class DocumentPaneGroupDropTarget : DropTarget<LayoutDocumentPaneGroupControl>
	{
		private LayoutDocumentPaneGroupControl _targetPane;

		internal DocumentPaneGroupDropTarget(LayoutDocumentPaneGroupControl paneControl, Rect detectionRect, DropTargetType type) : base(paneControl, detectionRect, type)
		{
			this._targetPane = paneControl;
		}

		protected override void Drop(LayoutDocumentFloatingWindow floatingWindow)
		{
			ILayoutPane model = this._targetPane.Model as ILayoutPane;
			if (base.Type == DropTargetType.DocumentPaneGroupDockInside)
			{
				LayoutDocumentPane item = (model as LayoutDocumentPaneGroup).Children[0] as LayoutDocumentPane;
				LayoutDocument rootDocument = floatingWindow.RootDocument;
				item.Children.Insert(0, rootDocument);
			}
			base.Drop(floatingWindow);
		}

		protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
		{
			ILayoutPane model = this._targetPane.Model as ILayoutPane;
			if (base.Type == DropTargetType.DocumentPaneGroupDockInside)
			{
				LayoutDocumentPane item = (model as LayoutDocumentPaneGroup).Children[0] as LayoutDocumentPane;
				int num = 0;
				LayoutAnchorable[] array = floatingWindow.RootPanel.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>();
				for (int i = 0; i < (int)array.Length; i++)
				{
					LayoutAnchorable layoutAnchorable = array[i];
					item.Children.Insert(num, layoutAnchorable);
					num++;
				}
			}
			base.Drop(floatingWindow);
		}

		public override Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindowModel)
		{
			if (base.Type != DropTargetType.DocumentPaneGroupDockInside)
			{
				return null;
			}
			Rect screenArea = base.TargetElement.GetScreenArea();
			screenArea.Offset(-overlayWindow.Left, -overlayWindow.Top);
			return new RectangleGeometry(screenArea);
		}
	}
}