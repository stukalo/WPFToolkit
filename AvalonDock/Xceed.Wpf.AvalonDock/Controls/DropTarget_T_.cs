using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal abstract class DropTarget<T> : DropTargetBase, IDropTarget
	where T : FrameworkElement
	{
		private Rect[] _detectionRect;

		private T _targetElement;

		private DropTargetType _type;

		public Rect[] DetectionRects
		{
			get
			{
				return this._detectionRect;
			}
		}

		public T TargetElement
		{
			get
			{
				return this._targetElement;
			}
		}

		public DropTargetType Type
		{
			get
			{
				return this._type;
			}
		}

		protected DropTarget(T targetElement, Rect detectionRect, DropTargetType type)
		{
			this._targetElement = targetElement;
			this._detectionRect = new Rect[] { detectionRect };
			this._type = type;
		}

		protected DropTarget(T targetElement, IEnumerable<Rect> detectionRects, DropTargetType type)
		{
			this._targetElement = targetElement;
			this._detectionRect = detectionRects.ToArray<Rect>();
			this._type = type;
		}

		public void DragEnter()
		{
			DropTargetBase.SetIsDraggingOver(this.TargetElement, true);
		}

		public void DragLeave()
		{
			DropTargetBase.SetIsDraggingOver(this.TargetElement, false);
		}

		protected virtual void Drop(LayoutAnchorableFloatingWindow floatingWindow)
		{
		}

		protected virtual void Drop(LayoutDocumentFloatingWindow floatingWindow)
		{
		}

		public void Drop(LayoutFloatingWindow floatingWindow)
		{
			ILayoutRoot root = floatingWindow.Root;
			LayoutContent activeContent = floatingWindow.Root.ActiveContent;
			LayoutAnchorableFloatingWindow layoutAnchorableFloatingWindow = floatingWindow as LayoutAnchorableFloatingWindow;
			if (layoutAnchorableFloatingWindow == null)
			{
				this.Drop(floatingWindow as LayoutDocumentFloatingWindow);
			}
			else
			{
				this.Drop(layoutAnchorableFloatingWindow);
			}
			base.Dispatcher.BeginInvoke(new Action(() => {
				activeContent.IsSelected = false;
				activeContent.IsActive = false;
				activeContent.IsActive = true;
			}), DispatcherPriority.Background, new object[0]);
		}

		public abstract Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindow);

		public virtual bool HitTest(Point dragPoint)
		{
			return this._detectionRect.Any<Rect>((Rect dr) => dr.Contains(dragPoint));
		}
	}
}