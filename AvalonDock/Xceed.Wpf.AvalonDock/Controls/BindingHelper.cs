using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal class BindingHelper
	{
		public BindingHelper()
		{
		}

		public static void RebindInactiveBindings(DependencyObject dependencyObject)
		{
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(dependencyObject.GetType()))
			{
				DependencyPropertyDescriptor dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(property);
				if (dependencyPropertyDescriptor == null)
				{
					continue;
				}
				BindingExpressionBase bindingExpressionBase = BindingOperations.GetBindingExpressionBase(dependencyObject, dependencyPropertyDescriptor.DependencyProperty);
				if (bindingExpressionBase == null)
				{
					continue;
				}
				Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new Action(() => {
					dependencyObject.ClearValue(dependencyPropertyDescriptor.DependencyProperty);
					BindingOperations.SetBinding(dependencyObject, dependencyPropertyDescriptor.DependencyProperty, bindingExpressionBase.ParentBindingBase);
				}));
			}
		}
	}
}