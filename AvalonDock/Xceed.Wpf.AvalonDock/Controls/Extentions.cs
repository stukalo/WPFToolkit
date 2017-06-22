using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public static class Extentions
	{
		public static T FindLogicalAncestor<T>(this DependencyObject dependencyObject)
		where T : class
		{
			DependencyObject parent = dependencyObject;
			do
			{
				DependencyObject dependencyObject1 = parent;
				parent = LogicalTreeHelper.GetParent(parent);
				if (parent != null)
				{
					continue;
				}
				parent = VisualTreeHelper.GetParent(dependencyObject1);
			}
			while (parent != null && !(parent is T));
			return (T)(parent as T);
		}

		public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject depObj)
		where T : DependencyObject
		{
			if (depObj != null)
			{
				foreach (DependencyObject dependencyObject in LogicalTreeHelper.GetChildren(depObj).OfType<DependencyObject>())
				{
					if (dependencyObject != null && dependencyObject is T)
					{
						yield return (T)dependencyObject;
					}
					foreach (T t in dependencyObject.FindLogicalChildren<T>())
					{
						yield return t;
					}
				}
			}
		}

		public static T FindVisualAncestor<T>(this DependencyObject dependencyObject)
		where T : class
		{
			DependencyObject parent = dependencyObject;
			do
			{
				parent = VisualTreeHelper.GetParent(parent);
			}
			while (parent != null && !(parent is T));
			return (T)(parent as T);
		}

		public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj)
		where T : DependencyObject
		{
			if (depObj != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
				{
					DependencyObject dependencyObject = VisualTreeHelper.GetChild(depObj, i);
					if (dependencyObject != null && dependencyObject is T)
					{
						yield return (T)dependencyObject;
					}
					foreach (T t in dependencyObject.FindVisualChildren<T>())
					{
						yield return t;
					}
					dependencyObject = null;
				}
			}
		}

		public static DependencyObject FindVisualTreeRoot(this DependencyObject initial)
		{
			DependencyObject dependencyObject = initial;
			DependencyObject dependencyObject1 = initial;
			while (dependencyObject != null)
			{
				dependencyObject1 = dependencyObject;
				dependencyObject = (dependencyObject is Visual || dependencyObject is Visual3D ? VisualTreeHelper.GetParent(dependencyObject) : LogicalTreeHelper.GetParent(dependencyObject));
			}
			return dependencyObject1;
		}
	}
}