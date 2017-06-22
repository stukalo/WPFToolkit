using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class DocumentPaneTabPanel : Panel
	{
		public DocumentPaneTabPanel()
		{
			base.FlowDirection = System.Windows.FlowDirection.LeftToRight;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			Size desiredSize;
			IEnumerable<UIElement> uIElements = base.Children.Cast<UIElement>();
			double right = 0;
			bool flag = false;
			using (IEnumerator<UIElement> enumerator = (
				from ch in uIElements
				where ch.Visibility != System.Windows.Visibility.Collapsed
				select ch).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TabItem current = (TabItem)enumerator.Current;
					LayoutContent content = current.Content as LayoutContent;
					if (!flag)
					{
						desiredSize = current.DesiredSize;
						if (right + desiredSize.Width <= finalSize.Width)
						{
							current.Visibility = System.Windows.Visibility.Visible;
							Size size = current.DesiredSize;
							current.Arrange(new Rect(right, 0, size.Width, finalSize.Height));
							double actualWidth = current.ActualWidth;
							Thickness margin = current.Margin;
							double left = actualWidth + margin.Left;
							margin = current.Margin;
							right = right + (left + margin.Right);
							continue;
						}
					}
					if (content.IsSelected && !current.IsVisible)
					{
						ILayoutContainer parent = content.Parent;
						ILayoutContentSelector layoutContentSelector = content.Parent as ILayoutContentSelector;
						ILayoutPane layoutPane = content.Parent as ILayoutPane;
						int num = layoutContentSelector.IndexOf(content);
						if (num > 0 && parent.ChildrenCount > 1)
						{
							layoutPane.MoveChild(num, 0);
							layoutContentSelector.SelectedContentIndex = 0;
							desiredSize = this.ArrangeOverride(finalSize);
							return desiredSize;
						}
					}
					current.Visibility = System.Windows.Visibility.Hidden;
					flag = true;
				}
				return finalSize;
			}
			return desiredSize;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			Size width = new Size();
			foreach (FrameworkElement child in base.Children)
			{
				child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				double num = width.Width;
				Size desiredSize = child.DesiredSize;
				width.Width = num + desiredSize.Width;
				double height = width.Height;
				desiredSize = child.DesiredSize;
				width.Height = Math.Max(height, desiredSize.Height);
			}
			return new Size(Math.Min(width.Width, availableSize.Width), width.Height);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
		}
	}
}