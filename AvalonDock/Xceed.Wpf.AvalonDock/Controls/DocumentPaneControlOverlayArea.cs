using System;
using System.Windows;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class DocumentPaneControlOverlayArea : OverlayArea
	{
		private LayoutDocumentPaneControl _documentPaneControl;

		internal DocumentPaneControlOverlayArea(IOverlayWindow overlayWindow, LayoutDocumentPaneControl documentPaneControl) : base(overlayWindow)
		{
			this._documentPaneControl = documentPaneControl;
			LayoutDocumentPaneControl layoutDocumentPaneControl = this._documentPaneControl;
			Point point = new Point();
			base.SetScreenDetectionArea(new Rect(layoutDocumentPaneControl.PointToScreenDPI(point), this._documentPaneControl.TransformActualSizeToAncestor()));
		}
	}
}