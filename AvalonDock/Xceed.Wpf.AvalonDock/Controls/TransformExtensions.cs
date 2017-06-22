using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal static class TransformExtensions
	{
		public static bool CanTransform(this Visual visual)
		{
			return PresentationSource.FromVisual(visual) != null;
		}

		public static Rect GetScreenArea(this FrameworkElement element)
		{
			Point screenDPI = element.PointToScreenDPI(new Point());
			if (FrameworkElement.GetFlowDirection(element) != FlowDirection.RightToLeft)
			{
				return new Rect(screenDPI, element.TransformActualSizeToAncestor());
			}
			Size ancestor = element.TransformActualSizeToAncestor();
			return new Rect(new Point(ancestor.Width - screenDPI.X, screenDPI.Y), ancestor);
		}

		public static Point PointToScreenDPI(this Visual visual, Point pt)
		{
			return visual.TransformToDeviceDPI(visual.PointToScreen(pt));
		}

		public static Point PointToScreenDPIWithoutFlowDirection(this FrameworkElement element, Point point)
		{
			if (FrameworkElement.GetFlowDirection(element) != FlowDirection.RightToLeft)
			{
				return element.PointToScreenDPI(point);
			}
			Size ancestor = element.TransformActualSizeToAncestor();
			Point point1 = new Point(ancestor.Width - point.X, point.Y);
			return element.PointToScreenDPI(point1);
		}

		public static GeneralTransform TansformToAncestor(this FrameworkElement element)
		{
			if (PresentationSource.FromVisual(element) == null)
			{
				return new MatrixTransform(Matrix.Identity);
			}
			return element.TransformToAncestor(PresentationSource.FromVisual(element).RootVisual);
		}

		public static Size TransformActualSizeToAncestor(this FrameworkElement element)
		{
			if (PresentationSource.FromVisual(element) == null)
			{
				return new Size(element.ActualWidth, element.ActualHeight);
			}
			Visual rootVisual = PresentationSource.FromVisual(element).RootVisual;
			Rect rect = element.TransformToAncestor(rootVisual).TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
			return rect.Size;
		}

		public static Size TransformFromDeviceDPI(this Visual visual, Size size)
		{
			Matrix transformToDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return new Size(size.Width * transformToDevice.M11, size.Height * transformToDevice.M22);
		}

		public static Point TransformFromDeviceDPI(this Visual visual, Point pt)
		{
			Matrix transformToDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return new Point(pt.X * transformToDevice.M11, pt.Y * transformToDevice.M22);
		}

		public static Size TransformSizeToAncestor(this FrameworkElement element, Size sizeToTransform)
		{
			if (PresentationSource.FromVisual(element) == null)
			{
				return sizeToTransform;
			}
			Visual rootVisual = PresentationSource.FromVisual(element).RootVisual;
			Rect rect = element.TransformToAncestor(rootVisual).TransformBounds(new Rect(0, 0, sizeToTransform.Width, sizeToTransform.Height));
			return rect.Size;
		}

		public static Point TransformToDeviceDPI(this Visual visual, Point pt)
		{
			Matrix transformToDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return new Point(pt.X / transformToDevice.M11, pt.Y / transformToDevice.M22);
		}
	}
}