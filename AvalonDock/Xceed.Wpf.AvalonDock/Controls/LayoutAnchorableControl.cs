using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutAnchorableControl : Control
	{
		public readonly static DependencyProperty ModelProperty;

		private readonly static DependencyPropertyKey LayoutItemPropertyKey;

		public readonly static DependencyProperty LayoutItemProperty;

		public Xceed.Wpf.AvalonDock.Controls.LayoutItem LayoutItem
		{
			get
			{
				return (Xceed.Wpf.AvalonDock.Controls.LayoutItem)base.GetValue(LayoutAnchorableControl.LayoutItemProperty);
			}
		}

		public LayoutAnchorable Model
		{
			get
			{
				return (LayoutAnchorable)base.GetValue(LayoutAnchorableControl.ModelProperty);
			}
			set
			{
				base.SetValue(LayoutAnchorableControl.ModelProperty, value);
			}
		}

		static LayoutAnchorableControl()
		{
			LayoutAnchorableControl.ModelProperty = DependencyProperty.Register("Model", typeof(LayoutAnchorable), typeof(LayoutAnchorableControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutAnchorableControl.OnModelChanged)));
			LayoutAnchorableControl.LayoutItemPropertyKey = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(Xceed.Wpf.AvalonDock.Controls.LayoutItem), typeof(LayoutAnchorableControl), new FrameworkPropertyMetadata(null));
			LayoutAnchorableControl.LayoutItemProperty = LayoutAnchorableControl.LayoutItemPropertyKey.DependencyProperty;
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorableControl), new FrameworkPropertyMetadata(typeof(LayoutAnchorableControl)));
			UIElement.FocusableProperty.OverrideMetadata(typeof(LayoutAnchorableControl), new FrameworkPropertyMetadata(false));
		}

		public LayoutAnchorableControl()
		{
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsEnabled" && this.Model != null)
			{
				base.IsEnabled = this.Model.IsEnabled;
				if (!base.IsEnabled && this.Model.IsActive && this.Model.Parent != null && this.Model.Parent is LayoutAnchorablePane)
				{
					((LayoutAnchorablePane)this.Model.Parent).SetNextSelectedIndex();
				}
			}
		}

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			if (this.Model != null)
			{
				this.Model.IsActive = true;
			}
			base.OnGotKeyboardFocus(e);
		}

		private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutAnchorableControl)d).OnModelChanged(e);
		}

		protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
			{
				((LayoutContent)e.OldValue).PropertyChanged -= new PropertyChangedEventHandler(this.Model_PropertyChanged);
			}
			if (this.Model == null)
			{
				this.SetLayoutItem(null);
				return;
			}
			this.Model.PropertyChanged += new PropertyChangedEventHandler(this.Model_PropertyChanged);
			this.SetLayoutItem(this.Model.Root.Manager.GetLayoutItemFromModel(this.Model));
		}

		protected void SetLayoutItem(Xceed.Wpf.AvalonDock.Controls.LayoutItem value)
		{
			base.SetValue(LayoutAnchorableControl.LayoutItemPropertyKey, value);
		}
	}
}