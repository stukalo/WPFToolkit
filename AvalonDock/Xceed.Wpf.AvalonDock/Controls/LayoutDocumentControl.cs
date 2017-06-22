using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutDocumentControl : Control
	{
		public readonly static DependencyProperty ModelProperty;

		private readonly static DependencyPropertyKey LayoutItemPropertyKey;

		public readonly static DependencyProperty LayoutItemProperty;

		public Xceed.Wpf.AvalonDock.Controls.LayoutItem LayoutItem
		{
			get
			{
				return (Xceed.Wpf.AvalonDock.Controls.LayoutItem)base.GetValue(LayoutDocumentControl.LayoutItemProperty);
			}
		}

		public LayoutContent Model
		{
			get
			{
				return (LayoutContent)base.GetValue(LayoutDocumentControl.ModelProperty);
			}
			set
			{
				base.SetValue(LayoutDocumentControl.ModelProperty, value);
			}
		}

		static LayoutDocumentControl()
		{
			LayoutDocumentControl.ModelProperty = DependencyProperty.Register("Model", typeof(LayoutContent), typeof(LayoutDocumentControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutDocumentControl.OnModelChanged)));
			LayoutDocumentControl.LayoutItemPropertyKey = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(Xceed.Wpf.AvalonDock.Controls.LayoutItem), typeof(LayoutDocumentControl), new FrameworkPropertyMetadata(null));
			LayoutDocumentControl.LayoutItemProperty = LayoutDocumentControl.LayoutItemPropertyKey.DependencyProperty;
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDocumentControl), new FrameworkPropertyMetadata(typeof(LayoutDocumentControl)));
			UIElement.FocusableProperty.OverrideMetadata(typeof(LayoutDocumentControl), new FrameworkPropertyMetadata(false));
		}

		public LayoutDocumentControl()
		{
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsEnabled" && this.Model != null)
			{
				base.IsEnabled = this.Model.IsEnabled;
				if (!base.IsEnabled && this.Model.IsActive && this.Model.Parent != null && this.Model.Parent is LayoutDocumentPane)
				{
					((LayoutDocumentPane)this.Model.Parent).SetNextSelectedIndex();
				}
			}
		}

		private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutDocumentControl)d).OnModelChanged(e);
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

		protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			if (this.Model != null)
			{
				this.Model.IsActive = true;
			}
			base.OnPreviewGotKeyboardFocus(e);
		}

		protected void SetLayoutItem(Xceed.Wpf.AvalonDock.Controls.LayoutItem value)
		{
			base.SetValue(LayoutDocumentControl.LayoutItemPropertyKey, value);
		}
	}
}