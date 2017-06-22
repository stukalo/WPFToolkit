using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock;

namespace Xceed.Wpf.AvalonDock.Layout
{
	public static class Extensions
	{
		public static bool ContainsChildOfType<T>(this ILayoutContainer element)
		{
			bool flag;
			using (IEnumerator<ILayoutElement> enumerator = element.Descendents().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!(enumerator.Current is T))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		public static bool ContainsChildOfType<T, S>(this ILayoutContainer container)
		{
			bool flag;
			using (IEnumerator<ILayoutElement> enumerator = container.Descendents().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ILayoutElement current = enumerator.Current;
					if (!(current is T) && !(current is S))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		public static IEnumerable<ILayoutElement> Descendents(this ILayoutElement element)
		{
			ILayoutContainer layoutContainer = element as ILayoutContainer;
			if (layoutContainer != null)
			{
				foreach (ILayoutElement child in layoutContainer.Children)
				{
					yield return child;
					foreach (ILayoutElement layoutElement in child.Descendents())
					{
						yield return layoutElement;
					}
				}
			}
		}

		public static T FindParent<T>(this ILayoutElement element)
		{
			ILayoutContainer parent = element.Parent;
			while (parent != null && !(parent is T))
			{
				parent = parent.Parent;
			}
			return (T)parent;
		}

		public static ILayoutRoot GetRoot(this ILayoutElement element)
		{
			if (element is ILayoutRoot)
			{
				return element as ILayoutRoot;
			}
			ILayoutContainer parent = element.Parent;
			while (parent != null && !(parent is ILayoutRoot))
			{
				parent = parent.Parent;
			}
			return (ILayoutRoot)parent;
		}

		public static AnchorSide GetSide(this ILayoutElement element)
		{
			AnchorSide anchorSide;
			ILayoutOrientableGroup parent = element.Parent as ILayoutOrientableGroup;
			if (parent != null)
			{
				if (!parent.ContainsChildOfType<LayoutDocumentPaneGroup, LayoutDocumentPane>())
				{
					return parent.GetSide();
				}
				using (IEnumerator<ILayoutElement> enumerator = parent.Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ILayoutElement current = enumerator.Current;
						if (current == element || current.Descendents().Contains(element))
						{
							anchorSide = (parent.Orientation == Orientation.Horizontal ? AnchorSide.Left : AnchorSide.Top);
							return anchorSide;
						}
						else
						{
							ILayoutContainer layoutContainer = current as ILayoutContainer;
							if (layoutContainer == null || !layoutContainer.IsOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>() && !layoutContainer.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>())
							{
								continue;
							}
							anchorSide = (parent.Orientation == Orientation.Horizontal ? AnchorSide.Right : AnchorSide.Bottom);
							return anchorSide;
						}
					}
					return AnchorSide.Right;
				}
				return anchorSide;
			}
			return AnchorSide.Right;
		}

		public static bool IsOfType<T, S>(this ILayoutContainer container)
		{
			if (container is T)
			{
				return true;
			}
			return container is S;
		}

		internal static void KeepInsideNearestMonitor(this ILayoutElementForFloatingWindow paneInsideFloatingWindow)
		{
			Win32Helper.RECT rECT = new Win32Helper.RECT()
			{
				Left = (int)paneInsideFloatingWindow.FloatingLeft,
				Top = (int)paneInsideFloatingWindow.FloatingTop,
				Bottom = rECT.Top + (int)paneInsideFloatingWindow.FloatingHeight,
				Right = rECT.Left + (int)paneInsideFloatingWindow.FloatingWidth
			};
			uint num = 2;
			if (Win32Helper.MonitorFromRect(ref rECT, 0) == IntPtr.Zero)
			{
				IntPtr intPtr = Win32Helper.MonitorFromRect(ref rECT, num);
				if (intPtr != IntPtr.Zero)
				{
					Win32Helper.MonitorInfo monitorInfo = new Win32Helper.MonitorInfo()
					{
						Size = Marshal.SizeOf(monitorInfo)
					};
					Win32Helper.GetMonitorInfo(intPtr, monitorInfo);
					if (paneInsideFloatingWindow.FloatingLeft < (double)monitorInfo.Work.Left)
					{
						paneInsideFloatingWindow.FloatingLeft = (double)(monitorInfo.Work.Left + 10);
					}
					if (paneInsideFloatingWindow.FloatingLeft + paneInsideFloatingWindow.FloatingWidth > (double)monitorInfo.Work.Right)
					{
						paneInsideFloatingWindow.FloatingLeft = (double)monitorInfo.Work.Right - (paneInsideFloatingWindow.FloatingWidth + 10);
					}
					if (paneInsideFloatingWindow.FloatingTop < (double)monitorInfo.Work.Top)
					{
						paneInsideFloatingWindow.FloatingTop = (double)(monitorInfo.Work.Top + 10);
					}
					if (paneInsideFloatingWindow.FloatingTop + paneInsideFloatingWindow.FloatingHeight > (double)monitorInfo.Work.Bottom)
					{
						paneInsideFloatingWindow.FloatingTop = (double)monitorInfo.Work.Bottom - (paneInsideFloatingWindow.FloatingHeight + 10);
					}
				}
			}
		}
	}
}