using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class AnchorablePaneTabPanel : Panel
	{
		public AnchorablePaneTabPanel()
		{
			base.FlowDirection = System.Windows.FlowDirection.LeftToRight;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			IEnumerable<UIElement> children = 
				from UIElement ch in base.Children
				where ch.Visibility != System.Windows.Visibility.Collapsed
				select ch;
			double width = finalSize.Width;
			double num = children.Sum<UIElement>((UIElement ch) => ch.DesiredSize.Width);
			double num1 = 0;
			if (width <= num)
			{
				double num2 = width / (double)children.Count<UIElement>();
				foreach (UIElement child in children)
				{
					((FrameworkElement)child).Arrange(new Rect(num1, 0, num2, finalSize.Height));
					num1 = num1 + num2;
				}
			}
			else
			{
				foreach (FrameworkElement frameworkElement in children)
				{
					double width1 = frameworkElement.DesiredSize.Width;
					frameworkElement.Arrange(new Rect(num1, 0, width1, finalSize.Height));
					num1 = num1 + width1;
				}
			}
			return finalSize;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			double width = 0;
			double num = 0;
			IEnumerable<UIElement> children = 
				from UIElement ch in base.Children
				where ch.Visibility != System.Windows.Visibility.Collapsed
				select ch;
			foreach (FrameworkElement child in children)
			{
				child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
				Size desiredSize = child.DesiredSize;
				width = width + desiredSize.Width;
				desiredSize = child.DesiredSize;
				num = Math.Max(num, desiredSize.Height);
			}
			if (width > availableSize.Width)
			{
				double width1 = availableSize.Width / (double)children.Count<UIElement>();
				foreach (UIElement uIElement in children)
				{
					((FrameworkElement)uIElement).Measure(new Size(width1, availableSize.Height));
				}
			}
			return new Size(Math.Min(availableSize.Width, width), num);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && LayoutAnchorableTabItem.IsDraggingItem())
			{
				LayoutAnchorable model = LayoutAnchorableTabItem.GetDraggingItem().Model as LayoutAnchorable;
				DockingManager manager = model.Root.Manager;
				LayoutAnchorableTabItem.ResetDraggingItem();
				manager.StartDraggingFloatingWindowForContent(model, true);
			}
			base.OnMouseLeave(e);
		}
	}
}