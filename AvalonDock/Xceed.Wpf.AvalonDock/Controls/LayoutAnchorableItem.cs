using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Commands;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutAnchorableItem : LayoutItem
	{
		private LayoutAnchorable _anchorable;

		private ICommand _defaultHideCommand;

		private ICommand _defaultAutoHideCommand;

		private ICommand _defaultDockCommand;

		public readonly static DependencyProperty HideCommandProperty;

		public readonly static DependencyProperty AutoHideCommandProperty;

		public readonly static DependencyProperty DockCommandProperty;

		private ReentrantFlag _visibilityReentrantFlag = new ReentrantFlag();

		public readonly static DependencyProperty CanHideProperty;

		public ICommand AutoHideCommand
		{
			get
			{
				return (ICommand)base.GetValue(LayoutAnchorableItem.AutoHideCommandProperty);
			}
			set
			{
				base.SetValue(LayoutAnchorableItem.AutoHideCommandProperty, value);
			}
		}

		public bool CanHide
		{
			get
			{
				return (bool)base.GetValue(LayoutAnchorableItem.CanHideProperty);
			}
			set
			{
				base.SetValue(LayoutAnchorableItem.CanHideProperty, value);
			}
		}

		public ICommand DockCommand
		{
			get
			{
				return (ICommand)base.GetValue(LayoutAnchorableItem.DockCommandProperty);
			}
			set
			{
				base.SetValue(LayoutAnchorableItem.DockCommandProperty, value);
			}
		}

		public ICommand HideCommand
		{
			get
			{
				return (ICommand)base.GetValue(LayoutAnchorableItem.HideCommandProperty);
			}
			set
			{
				base.SetValue(LayoutAnchorableItem.HideCommandProperty, value);
			}
		}

		static LayoutAnchorableItem()
		{
			LayoutAnchorableItem.HideCommandProperty = DependencyProperty.Register("HideCommand", typeof(ICommand), typeof(LayoutAnchorableItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutAnchorableItem.OnHideCommandChanged), new CoerceValueCallback(LayoutAnchorableItem.CoerceHideCommandValue)));
			LayoutAnchorableItem.AutoHideCommandProperty = DependencyProperty.Register("AutoHideCommand", typeof(ICommand), typeof(LayoutAnchorableItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutAnchorableItem.OnAutoHideCommandChanged), new CoerceValueCallback(LayoutAnchorableItem.CoerceAutoHideCommandValue)));
			LayoutAnchorableItem.DockCommandProperty = DependencyProperty.Register("DockCommand", typeof(ICommand), typeof(LayoutAnchorableItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutAnchorableItem.OnDockCommandChanged), new CoerceValueCallback(LayoutAnchorableItem.CoerceDockCommandValue)));
			LayoutAnchorableItem.CanHideProperty = DependencyProperty.Register("CanHide", typeof(bool), typeof(LayoutAnchorableItem), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(LayoutAnchorableItem.OnCanHideChanged)));
		}

		internal LayoutAnchorableItem()
		{
		}

		private void _anchorable_IsVisibleChanged(object sender, EventArgs e)
		{
			if (this._anchorable != null && this._anchorable.Root != null && this._visibilityReentrantFlag.CanEnter)
			{
				using (ReentrantFlag._ReentrantFlagHandler __ReentrantFlagHandler = this._visibilityReentrantFlag.Enter())
				{
					if (!this._anchorable.IsVisible)
					{
						base.Visibility = System.Windows.Visibility.Hidden;
					}
					else
					{
						base.Visibility = System.Windows.Visibility.Visible;
					}
				}
			}
		}

		internal override void Attach(LayoutContent model)
		{
			this._anchorable = model as LayoutAnchorable;
			this._anchorable.IsVisibleChanged += new EventHandler(this._anchorable_IsVisibleChanged);
			base.Attach(model);
		}

		private bool CanExecuteAutoHideCommand(object parameter)
		{
			if (base.LayoutElement == null)
			{
				return false;
			}
			if (base.LayoutElement.FindParent<LayoutAnchorableFloatingWindow>() != null)
			{
				return false;
			}
			return this._anchorable.CanAutoHide;
		}

		private bool CanExecuteDockCommand(object parameter)
		{
			if (base.LayoutElement == null)
			{
				return false;
			}
			return base.LayoutElement.FindParent<LayoutAnchorableFloatingWindow>() != null;
		}

		private bool CanExecuteHideCommand(object parameter)
		{
			if (base.LayoutElement == null)
			{
				return false;
			}
			return this._anchorable.CanHide;
		}

		protected override void ClearDefaultBindings()
		{
			if (this.HideCommand == this._defaultHideCommand)
			{
				BindingOperations.ClearBinding(this, LayoutAnchorableItem.HideCommandProperty);
			}
			if (this.AutoHideCommand == this._defaultAutoHideCommand)
			{
				BindingOperations.ClearBinding(this, LayoutAnchorableItem.AutoHideCommandProperty);
			}
			if (this.DockCommand == this._defaultDockCommand)
			{
				BindingOperations.ClearBinding(this, LayoutAnchorableItem.DockCommandProperty);
			}
			base.ClearDefaultBindings();
		}

		protected override void Close()
		{
			if (this._anchorable.Root != null && this._anchorable.Root.Manager != null)
			{
				this._anchorable.Root.Manager._ExecuteCloseCommand(this._anchorable);
			}
		}

		private static object CoerceAutoHideCommandValue(DependencyObject d, object value)
		{
			return value;
		}

		private static object CoerceDockCommandValue(DependencyObject d, object value)
		{
			return value;
		}

		private static object CoerceHideCommandValue(DependencyObject d, object value)
		{
			return value;
		}

		internal override void Detach()
		{
			this._anchorable.IsVisibleChanged -= new EventHandler(this._anchorable_IsVisibleChanged);
			this._anchorable = null;
			base.Detach();
		}

		private void ExecuteAutoHideCommand(object parameter)
		{
			if (this._anchorable != null && this._anchorable.Root != null && this._anchorable.Root.Manager != null)
			{
				this._anchorable.Root.Manager._ExecuteAutoHideCommand(this._anchorable);
			}
		}

		private void ExecuteDockCommand(object parameter)
		{
			base.LayoutElement.Root.Manager._ExecuteDockCommand(this._anchorable);
		}

		private void ExecuteHideCommand(object parameter)
		{
			if (this._anchorable != null && this._anchorable.Root != null && this._anchorable.Root.Manager != null)
			{
				this._anchorable.Root.Manager._ExecuteHideCommand(this._anchorable);
			}
		}

		protected override void InitDefaultCommands()
		{
			this._defaultHideCommand = new RelayCommand((object p) => this.ExecuteHideCommand(p), (object p) => this.CanExecuteHideCommand(p));
			this._defaultAutoHideCommand = new RelayCommand((object p) => this.ExecuteAutoHideCommand(p), (object p) => this.CanExecuteAutoHideCommand(p));
			this._defaultDockCommand = new RelayCommand((object p) => this.ExecuteDockCommand(p), (object p) => this.CanExecuteDockCommand(p));
			base.InitDefaultCommands();
		}

		private static void OnAutoHideCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutAnchorableItem)d).OnAutoHideCommandChanged(e);
		}

		protected virtual void OnAutoHideCommandChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnCanHideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutAnchorableItem)d).OnCanHideChanged(e);
		}

		protected virtual void OnCanHideChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this._anchorable != null)
			{
				this._anchorable.CanHide = (bool)e.NewValue;
			}
		}

		private static void OnDockCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutAnchorableItem)d).OnDockCommandChanged(e);
		}

		protected virtual void OnDockCommandChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnHideCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutAnchorableItem)d).OnHideCommandChanged(e);
		}

		protected virtual void OnHideCommandChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		protected override void OnVisibilityChanged()
		{
			if (this._anchorable != null && this._anchorable.Root != null && this._visibilityReentrantFlag.CanEnter)
			{
				using (ReentrantFlag._ReentrantFlagHandler __ReentrantFlagHandler = this._visibilityReentrantFlag.Enter())
				{
					if (base.Visibility == System.Windows.Visibility.Hidden)
					{
						this._anchorable.Hide(false);
					}
					else if (base.Visibility == System.Windows.Visibility.Visible)
					{
						this._anchorable.Show();
					}
				}
			}
			base.OnVisibilityChanged();
		}

		protected override void SetDefaultBindings()
		{
			if (this.HideCommand == null)
			{
				this.HideCommand = this._defaultHideCommand;
			}
			if (this.AutoHideCommand == null)
			{
				this.AutoHideCommand = this._defaultAutoHideCommand;
			}
			if (this.DockCommand == null)
			{
				this.DockCommand = this._defaultDockCommand;
			}
			base.Visibility = (this._anchorable.IsVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
			base.SetDefaultBindings();
		}
	}
}