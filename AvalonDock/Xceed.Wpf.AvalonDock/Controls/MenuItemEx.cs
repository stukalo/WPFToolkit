using System;
using System.Windows;
using System.Windows.Controls;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class MenuItemEx : MenuItem
	{
		public readonly static DependencyProperty IconTemplateProperty;

		public readonly static DependencyProperty IconTemplateSelectorProperty;

		private bool _reentrantFlag;

		public DataTemplate IconTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(MenuItemEx.IconTemplateProperty);
			}
			set
			{
				base.SetValue(MenuItemEx.IconTemplateProperty, value);
			}
		}

		public DataTemplateSelector IconTemplateSelector
		{
			get
			{
				return (DataTemplateSelector)base.GetValue(MenuItemEx.IconTemplateSelectorProperty);
			}
			set
			{
				base.SetValue(MenuItemEx.IconTemplateSelectorProperty, value);
			}
		}

		static MenuItemEx()
		{
			MenuItemEx.IconTemplateProperty = DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(MenuItemEx), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(MenuItemEx.OnIconTemplateChanged)));
			MenuItemEx.IconTemplateSelectorProperty = DependencyProperty.Register("IconTemplateSelector", typeof(DataTemplateSelector), typeof(MenuItemEx), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(MenuItemEx.OnIconTemplateSelectorChanged)));
			MenuItem.IconProperty.OverrideMetadata(typeof(MenuItemEx), new FrameworkPropertyMetadata(new PropertyChangedCallback(MenuItemEx.OnIconPropertyChanged)));
		}

		public MenuItemEx()
		{
		}

		private static void OnIconPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null)
			{
				((MenuItemEx)sender).UpdateIcon();
			}
		}

		private static void OnIconTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((MenuItemEx)d).OnIconTemplateChanged(e);
		}

		protected virtual void OnIconTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
			this.UpdateIcon();
		}

		private static void OnIconTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((MenuItemEx)d).OnIconTemplateSelectorChanged(e);
		}

		protected virtual void OnIconTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
			this.UpdateIcon();
		}

		private void UpdateIcon()
		{
			if (this._reentrantFlag)
			{
				return;
			}
			this._reentrantFlag = true;
			if (this.IconTemplateSelector != null)
			{
				DataTemplate dataTemplate = this.IconTemplateSelector.SelectTemplate(base.Icon, this);
				if (dataTemplate != null)
				{
					base.Icon = dataTemplate.LoadContent();
				}
			}
			else if (this.IconTemplate != null)
			{
				base.Icon = this.IconTemplate.LoadContent();
			}
			this._reentrantFlag = false;
		}
	}
}