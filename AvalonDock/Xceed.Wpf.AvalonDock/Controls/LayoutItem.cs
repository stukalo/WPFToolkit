using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Commands;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public abstract class LayoutItem : FrameworkElement
	{
		private ICommand _defaultCloseCommand;

		private ICommand _defaultFloatCommand;

		private ICommand _defaultDockAsDocumentCommand;

		private ICommand _defaultCloseAllButThisCommand;

		private ICommand _defaultCloseAllCommand;

		private ICommand _defaultActivateCommand;

		private ICommand _defaultNewVerticalTabGroupCommand;

		private ICommand _defaultNewHorizontalTabGroupCommand;

		private ICommand _defaultMoveToNextTabGroupCommand;

		private ICommand _defaultMoveToPreviousTabGroupCommand;

		private ContentPresenter _view;

		public readonly static DependencyProperty TitleProperty;

		public readonly static DependencyProperty IconSourceProperty;

		public readonly static DependencyProperty ContentIdProperty;

		private ReentrantFlag _isSelectedReentrantFlag = new ReentrantFlag();

		public readonly static DependencyProperty IsSelectedProperty;

		private ReentrantFlag _isActiveReentrantFlag = new ReentrantFlag();

		public readonly static DependencyProperty IsActiveProperty;

		public readonly static DependencyProperty CanCloseProperty;

		public readonly static DependencyProperty CanFloatProperty;

		public readonly static DependencyProperty CloseCommandProperty;

		public readonly static DependencyProperty FloatCommandProperty;

		public readonly static DependencyProperty DockAsDocumentCommandProperty;

		public readonly static DependencyProperty CloseAllButThisCommandProperty;

		public readonly static DependencyProperty CloseAllCommandProperty;

		public readonly static DependencyProperty ActivateCommandProperty;

		public readonly static DependencyProperty NewVerticalTabGroupCommandProperty;

		public readonly static DependencyProperty NewHorizontalTabGroupCommandProperty;

		public readonly static DependencyProperty MoveToNextTabGroupCommandProperty;

		public readonly static DependencyProperty MoveToPreviousTabGroupCommandProperty;

		public ICommand ActivateCommand
		{
			get
			{
				return (ICommand)base.GetValue(LayoutItem.ActivateCommandProperty);
			}
			set
			{
				base.SetValue(LayoutItem.ActivateCommandProperty, value);
			}
		}

		public bool CanClose
		{
			get
			{
				return (bool)base.GetValue(LayoutItem.CanCloseProperty);
			}
			set
			{
				base.SetValue(LayoutItem.CanCloseProperty, value);
			}
		}

		public bool CanFloat
		{
			get
			{
				return (bool)base.GetValue(LayoutItem.CanFloatProperty);
			}
			set
			{
				base.SetValue(LayoutItem.CanFloatProperty, value);
			}
		}

		public ICommand CloseAllButThisCommand
		{
			get
			{
				return (ICommand)base.GetValue(LayoutItem.CloseAllButThisCommandProperty);
			}
			set
			{
				base.SetValue(LayoutItem.CloseAllButThisCommandProperty, value);
			}
		}

		public ICommand CloseAllCommand
		{
			get
			{
				return (ICommand)base.GetValue(LayoutItem.CloseAllCommandProperty);
			}
			set
			{
				base.SetValue(LayoutItem.CloseAllCommandProperty, value);
			}
		}

		public ICommand CloseCommand
		{
			get
			{
				return (ICommand)base.GetValue(LayoutItem.CloseCommandProperty);
			}
			set
			{
				base.SetValue(LayoutItem.CloseCommandProperty, value);
			}
		}

		public string ContentId
		{
			get
			{
				return (string)base.GetValue(LayoutItem.ContentIdProperty);
			}
			set
			{
				base.SetValue(LayoutItem.ContentIdProperty, value);
			}
		}

		public ICommand DockAsDocumentCommand
		{
			get
			{
				return (ICommand)base.GetValue(LayoutItem.DockAsDocumentCommandProperty);
			}
			set
			{
				base.SetValue(LayoutItem.DockAsDocumentCommandProperty, value);
			}
		}

		public ICommand FloatCommand
		{
			get
			{
				return (ICommand)base.GetValue(LayoutItem.FloatCommandProperty);
			}
			set
			{
				base.SetValue(LayoutItem.FloatCommandProperty, value);
			}
		}

		public ImageSource IconSource
		{
			get
			{
				return (ImageSource)base.GetValue(LayoutItem.IconSourceProperty);
			}
			set
			{
				base.SetValue(LayoutItem.IconSourceProperty, value);
			}
		}

		public bool IsActive
		{
			get
			{
				return (bool)base.GetValue(LayoutItem.IsActiveProperty);
			}
			set
			{
				base.SetValue(LayoutItem.IsActiveProperty, value);
			}
		}

		public bool IsSelected
		{
			get
			{
				return (bool)base.GetValue(LayoutItem.IsSelectedProperty);
			}
			set
			{
				base.SetValue(LayoutItem.IsSelectedProperty, value);
			}
		}

		public LayoutContent LayoutElement
		{
			get;
			private set;
		}

		public object Model
		{
			get;
			private set;
		}

		public ICommand MoveToNextTabGroupCommand
		{
			get
			{
				return (ICommand)base.GetValue(LayoutItem.MoveToNextTabGroupCommandProperty);
			}
			set
			{
				base.SetValue(LayoutItem.MoveToNextTabGroupCommandProperty, value);
			}
		}

		public ICommand MoveToPreviousTabGroupCommand
		{
			get
			{
				return (ICommand)base.GetValue(LayoutItem.MoveToPreviousTabGroupCommandProperty);
			}
			set
			{
				base.SetValue(LayoutItem.MoveToPreviousTabGroupCommandProperty, value);
			}
		}

		public ICommand NewHorizontalTabGroupCommand
		{
			get
			{
				return (ICommand)base.GetValue(LayoutItem.NewHorizontalTabGroupCommandProperty);
			}
			set
			{
				base.SetValue(LayoutItem.NewHorizontalTabGroupCommandProperty, value);
			}
		}

		public ICommand NewVerticalTabGroupCommand
		{
			get
			{
				return (ICommand)base.GetValue(LayoutItem.NewVerticalTabGroupCommandProperty);
			}
			set
			{
				base.SetValue(LayoutItem.NewVerticalTabGroupCommandProperty, value);
			}
		}

		public string Title
		{
			get
			{
				return (string)base.GetValue(LayoutItem.TitleProperty);
			}
			set
			{
				base.SetValue(LayoutItem.TitleProperty, value);
			}
		}

		public ContentPresenter View
		{
			get
			{
				if (this._view == null)
				{
					this._view = new ContentPresenter();
					this._view.SetBinding(ContentPresenter.ContentProperty, new Binding("Content")
					{
						Source = this.LayoutElement
					});
					this._view.SetBinding(ContentPresenter.ContentTemplateProperty, new Binding("LayoutItemTemplate")
					{
						Source = this.LayoutElement.Root.Manager
					});
					this._view.SetBinding(ContentPresenter.ContentTemplateSelectorProperty, new Binding("LayoutItemTemplateSelector")
					{
						Source = this.LayoutElement.Root.Manager
					});
					this.LayoutElement.Root.Manager.InternalAddLogicalChild(this._view);
				}
				return this._view;
			}
		}

		static LayoutItem()
		{
			LayoutItem.TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(LayoutItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutItem.OnTitleChanged)));
			LayoutItem.IconSourceProperty = DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(LayoutItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutItem.OnIconSourceChanged)));
			LayoutItem.ContentIdProperty = DependencyProperty.Register("ContentId", typeof(string), typeof(LayoutItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutItem.OnContentIdChanged)));
			LayoutItem.IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(LayoutItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(LayoutItem.OnIsSelectedChanged)));
			LayoutItem.IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(LayoutItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(LayoutItem.OnIsActiveChanged)));
			LayoutItem.CanCloseProperty = DependencyProperty.Register("CanClose", typeof(bool), typeof(LayoutItem), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(LayoutItem.OnCanCloseChanged)));
			LayoutItem.CanFloatProperty = DependencyProperty.Register("CanFloat", typeof(bool), typeof(LayoutItem), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(LayoutItem.OnCanFloatChanged)));
			LayoutItem.CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(LayoutItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutItem.OnCloseCommandChanged), new CoerceValueCallback(LayoutItem.CoerceCloseCommandValue)));
			LayoutItem.FloatCommandProperty = DependencyProperty.Register("FloatCommand", typeof(ICommand), typeof(LayoutItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutItem.OnFloatCommandChanged), new CoerceValueCallback(LayoutItem.CoerceFloatCommandValue)));
			LayoutItem.DockAsDocumentCommandProperty = DependencyProperty.Register("DockAsDocumentCommand", typeof(ICommand), typeof(LayoutItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutItem.OnDockAsDocumentCommandChanged), new CoerceValueCallback(LayoutItem.CoerceDockAsDocumentCommandValue)));
			LayoutItem.CloseAllButThisCommandProperty = DependencyProperty.Register("CloseAllButThisCommand", typeof(ICommand), typeof(LayoutItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutItem.OnCloseAllButThisCommandChanged), new CoerceValueCallback(LayoutItem.CoerceCloseAllButThisCommandValue)));
			LayoutItem.CloseAllCommandProperty = DependencyProperty.Register("CloseAllCommand", typeof(ICommand), typeof(LayoutItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutItem.OnCloseAllCommandChanged), new CoerceValueCallback(LayoutItem.CoerceCloseAllCommandValue)));
			LayoutItem.ActivateCommandProperty = DependencyProperty.Register("ActivateCommand", typeof(ICommand), typeof(LayoutItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutItem.OnActivateCommandChanged), new CoerceValueCallback(LayoutItem.CoerceActivateCommandValue)));
			LayoutItem.NewVerticalTabGroupCommandProperty = DependencyProperty.Register("NewVerticalTabGroupCommand", typeof(ICommand), typeof(LayoutItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutItem.OnNewVerticalTabGroupCommandChanged)));
			LayoutItem.NewHorizontalTabGroupCommandProperty = DependencyProperty.Register("NewHorizontalTabGroupCommand", typeof(ICommand), typeof(LayoutItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutItem.OnNewHorizontalTabGroupCommandChanged)));
			LayoutItem.MoveToNextTabGroupCommandProperty = DependencyProperty.Register("MoveToNextTabGroupCommand", typeof(ICommand), typeof(LayoutItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutItem.OnMoveToNextTabGroupCommandChanged)));
			LayoutItem.MoveToPreviousTabGroupCommandProperty = DependencyProperty.Register("MoveToPreviousTabGroupCommand", typeof(ICommand), typeof(LayoutItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutItem.OnMoveToPreviousTabGroupCommandChanged)));
			FrameworkElement.ToolTipProperty.OverrideMetadata(typeof(LayoutItem), new FrameworkPropertyMetadata(null, (DependencyObject s, DependencyPropertyChangedEventArgs e) => LayoutItem.OnToolTipChanged(s, e)));
			UIElement.VisibilityProperty.OverrideMetadata(typeof(LayoutItem), new FrameworkPropertyMetadata((object)System.Windows.Visibility.Visible, (DependencyObject s, DependencyPropertyChangedEventArgs e) => LayoutItem.OnVisibilityChanged(s, e)));
		}

		internal LayoutItem()
		{
		}

		internal void _ClearDefaultBindings()
		{
			this.ClearDefaultBindings();
		}

		internal void _SetDefaultBindings()
		{
			this.SetDefaultBindings();
		}

		internal virtual void Attach(LayoutContent model)
		{
			this.LayoutElement = model;
			this.Model = model.Content;
			this.InitDefaultCommands();
			this.LayoutElement.IsSelectedChanged += new EventHandler(this.LayoutElement_IsSelectedChanged);
			this.LayoutElement.IsActiveChanged += new EventHandler(this.LayoutElement_IsActiveChanged);
			base.DataContext = this;
		}

		private bool CanExecuteActivateCommand(object parameter)
		{
			return this.LayoutElement != null;
		}

		private bool CanExecuteCloseAllButThisCommand(object parameter)
		{
			if (this.LayoutElement == null)
			{
				return false;
			}
			if (this.LayoutElement.Root == null)
			{
				return false;
			}
			return this.LayoutElement.Root.Manager.Layout.Descendents().OfType<LayoutContent>().Where<LayoutContent>((LayoutContent d) => {
				if (d == this.LayoutElement)
				{
					return false;
				}
				if (d.Parent is LayoutDocumentPane)
				{
					return true;
				}
				return d.Parent is LayoutDocumentFloatingWindow;
			}).Any<LayoutContent>();
		}

		private bool CanExecuteCloseAllCommand(object parameter)
		{
			if (this.LayoutElement == null)
			{
				return false;
			}
			if (this.LayoutElement.Root == null)
			{
				return false;
			}
			return this.LayoutElement.Root.Manager.Layout.Descendents().OfType<LayoutContent>().Where<LayoutContent>((LayoutContent d) => {
				if (d.Parent is LayoutDocumentPane)
				{
					return true;
				}
				return d.Parent is LayoutDocumentFloatingWindow;
			}).Any<LayoutContent>();
		}

		private bool CanExecuteCloseCommand(object parameter)
		{
			if (this.LayoutElement == null)
			{
				return false;
			}
			return this.LayoutElement.CanClose;
		}

		private bool CanExecuteDockAsDocumentCommand(object parameter)
		{
			if (this.LayoutElement == null)
			{
				return false;
			}
			return this.LayoutElement.FindParent<LayoutDocumentPane>() == null;
		}

		private bool CanExecuteFloatCommand(object anchorable)
		{
			if (this.LayoutElement == null || !this.LayoutElement.CanFloat)
			{
				return false;
			}
			return this.LayoutElement.FindParent<LayoutFloatingWindow>() == null;
		}

		private bool CanExecuteMoveToNextTabGroupCommand(object parameter)
		{
			if (this.LayoutElement == null)
			{
				return false;
			}
			LayoutDocumentPaneGroup layoutDocumentPaneGroup = this.LayoutElement.FindParent<LayoutDocumentPaneGroup>();
			LayoutDocumentPane parent = this.LayoutElement.Parent as LayoutDocumentPane;
			if (layoutDocumentPaneGroup == null || parent == null || layoutDocumentPaneGroup.ChildrenCount <= 1 || layoutDocumentPaneGroup.IndexOfChild(parent) >= layoutDocumentPaneGroup.ChildrenCount - 1)
			{
				return false;
			}
			return layoutDocumentPaneGroup.Children[layoutDocumentPaneGroup.IndexOfChild(parent) + 1] is LayoutDocumentPane;
		}

		private bool CanExecuteMoveToPreviousTabGroupCommand(object parameter)
		{
			if (this.LayoutElement == null)
			{
				return false;
			}
			LayoutDocumentPaneGroup layoutDocumentPaneGroup = this.LayoutElement.FindParent<LayoutDocumentPaneGroup>();
			LayoutDocumentPane parent = this.LayoutElement.Parent as LayoutDocumentPane;
			if (layoutDocumentPaneGroup == null || parent == null || layoutDocumentPaneGroup.ChildrenCount <= 1 || layoutDocumentPaneGroup.IndexOfChild(parent) <= 0)
			{
				return false;
			}
			return layoutDocumentPaneGroup.Children[layoutDocumentPaneGroup.IndexOfChild(parent) - 1] is LayoutDocumentPane;
		}

		private bool CanExecuteNewHorizontalTabGroupCommand(object parameter)
		{
			if (this.LayoutElement == null)
			{
				return false;
			}
			LayoutDocumentPaneGroup layoutDocumentPaneGroup = this.LayoutElement.FindParent<LayoutDocumentPaneGroup>();
			LayoutDocumentPane parent = this.LayoutElement.Parent as LayoutDocumentPane;
			if (layoutDocumentPaneGroup != null && layoutDocumentPaneGroup.ChildrenCount != 1 && !layoutDocumentPaneGroup.Root.Manager.AllowMixedOrientation && layoutDocumentPaneGroup.Orientation != Orientation.Vertical || parent == null)
			{
				return false;
			}
			return parent.ChildrenCount > 1;
		}

		private bool CanExecuteNewVerticalTabGroupCommand(object parameter)
		{
			if (this.LayoutElement == null)
			{
				return false;
			}
			LayoutDocumentPaneGroup layoutDocumentPaneGroup = this.LayoutElement.FindParent<LayoutDocumentPaneGroup>();
			LayoutDocumentPane parent = this.LayoutElement.Parent as LayoutDocumentPane;
			if (layoutDocumentPaneGroup != null && layoutDocumentPaneGroup.ChildrenCount != 1 && !layoutDocumentPaneGroup.Root.Manager.AllowMixedOrientation && layoutDocumentPaneGroup.Orientation != Orientation.Horizontal || parent == null)
			{
				return false;
			}
			return parent.ChildrenCount > 1;
		}

		protected virtual void ClearDefaultBindings()
		{
			if (this.CloseCommand == this._defaultCloseCommand)
			{
				BindingOperations.ClearBinding(this, LayoutItem.CloseCommandProperty);
			}
			if (this.FloatCommand == this._defaultFloatCommand)
			{
				BindingOperations.ClearBinding(this, LayoutItem.FloatCommandProperty);
			}
			if (this.DockAsDocumentCommand == this._defaultDockAsDocumentCommand)
			{
				BindingOperations.ClearBinding(this, LayoutItem.DockAsDocumentCommandProperty);
			}
			if (this.CloseAllButThisCommand == this._defaultCloseAllButThisCommand)
			{
				BindingOperations.ClearBinding(this, LayoutItem.CloseAllButThisCommandProperty);
			}
			if (this.CloseAllCommand == this._defaultCloseAllCommand)
			{
				BindingOperations.ClearBinding(this, LayoutItem.CloseAllCommandProperty);
			}
			if (this.ActivateCommand == this._defaultActivateCommand)
			{
				BindingOperations.ClearBinding(this, LayoutItem.ActivateCommandProperty);
			}
			if (this.NewVerticalTabGroupCommand == this._defaultNewVerticalTabGroupCommand)
			{
				BindingOperations.ClearBinding(this, LayoutItem.NewVerticalTabGroupCommandProperty);
			}
			if (this.NewHorizontalTabGroupCommand == this._defaultNewHorizontalTabGroupCommand)
			{
				BindingOperations.ClearBinding(this, LayoutItem.NewHorizontalTabGroupCommandProperty);
			}
			if (this.MoveToNextTabGroupCommand == this._defaultMoveToNextTabGroupCommand)
			{
				BindingOperations.ClearBinding(this, LayoutItem.MoveToNextTabGroupCommandProperty);
			}
			if (this.MoveToPreviousTabGroupCommand == this._defaultMoveToPreviousTabGroupCommand)
			{
				BindingOperations.ClearBinding(this, LayoutItem.MoveToPreviousTabGroupCommandProperty);
			}
		}

		protected abstract void Close();

		private static object CoerceActivateCommandValue(DependencyObject d, object value)
		{
			return value;
		}

		private static object CoerceCloseAllButThisCommandValue(DependencyObject d, object value)
		{
			return value;
		}

		private static object CoerceCloseAllCommandValue(DependencyObject d, object value)
		{
			return value;
		}

		private static object CoerceCloseCommandValue(DependencyObject d, object value)
		{
			return value;
		}

		private static object CoerceDockAsDocumentCommandValue(DependencyObject d, object value)
		{
			return value;
		}

		private static object CoerceFloatCommandValue(DependencyObject d, object value)
		{
			return value;
		}

		internal virtual void Detach()
		{
			this.LayoutElement.IsSelectedChanged -= new EventHandler(this.LayoutElement_IsSelectedChanged);
			this.LayoutElement.IsActiveChanged -= new EventHandler(this.LayoutElement_IsActiveChanged);
			this.LayoutElement = null;
			this.Model = null;
		}

		private void ExecuteActivateCommand(object parameter)
		{
			this.LayoutElement.Root.Manager._ExecuteContentActivateCommand(this.LayoutElement);
		}

		private void ExecuteCloseAllButThisCommand(object parameter)
		{
			this.LayoutElement.Root.Manager._ExecuteCloseAllButThisCommand(this.LayoutElement);
		}

		private void ExecuteCloseAllCommand(object parameter)
		{
			this.LayoutElement.Root.Manager._ExecuteCloseAllCommand(this.LayoutElement);
		}

		private void ExecuteCloseCommand(object parameter)
		{
			this.Close();
		}

		private void ExecuteDockAsDocumentCommand(object parameter)
		{
			this.LayoutElement.Root.Manager._ExecuteDockAsDocumentCommand(this.LayoutElement);
		}

		private void ExecuteFloatCommand(object parameter)
		{
			this.LayoutElement.Root.Manager._ExecuteFloatCommand(this.LayoutElement);
		}

		private void ExecuteMoveToNextTabGroupCommand(object parameter)
		{
			LayoutContent layoutElement = this.LayoutElement;
			LayoutDocumentPaneGroup layoutDocumentPaneGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
			int num = layoutDocumentPaneGroup.IndexOfChild(layoutElement.Parent as LayoutDocumentPane);
			(layoutDocumentPaneGroup.Children[num + 1] as LayoutDocumentPane).InsertChildAt(0, layoutElement);
			layoutElement.IsActive = true;
			layoutElement.Root.CollectGarbage();
		}

		private void ExecuteMoveToPreviousTabGroupCommand(object parameter)
		{
			LayoutContent layoutElement = this.LayoutElement;
			LayoutDocumentPaneGroup layoutDocumentPaneGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
			int num = layoutDocumentPaneGroup.IndexOfChild(layoutElement.Parent as LayoutDocumentPane);
			(layoutDocumentPaneGroup.Children[num - 1] as LayoutDocumentPane).InsertChildAt(0, layoutElement);
			layoutElement.IsActive = true;
			layoutElement.Root.CollectGarbage();
		}

		private void ExecuteNewHorizontalTabGroupCommand(object parameter)
		{
			LayoutContent layoutElement = this.LayoutElement;
			LayoutDocumentPaneGroup layoutDocumentPaneGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
			LayoutDocumentPane parent = layoutElement.Parent as LayoutDocumentPane;
			if (layoutDocumentPaneGroup == null)
			{
				ILayoutContainer layoutContainer = parent.Parent;
				layoutDocumentPaneGroup = new LayoutDocumentPaneGroup()
				{
					Orientation = Orientation.Vertical
				};
				layoutContainer.ReplaceChild(parent, layoutDocumentPaneGroup);
				layoutDocumentPaneGroup.Children.Add(parent);
			}
			layoutDocumentPaneGroup.Orientation = Orientation.Vertical;
			int num = layoutDocumentPaneGroup.IndexOfChild(parent);
			layoutDocumentPaneGroup.InsertChildAt(num + 1, new LayoutDocumentPane(layoutElement));
			layoutElement.IsActive = true;
			layoutElement.Root.CollectGarbage();
		}

		private void ExecuteNewVerticalTabGroupCommand(object parameter)
		{
			LayoutContent layoutElement = this.LayoutElement;
			LayoutDocumentPaneGroup layoutDocumentPaneGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
			LayoutDocumentPane parent = layoutElement.Parent as LayoutDocumentPane;
			if (layoutDocumentPaneGroup == null)
			{
				ILayoutContainer layoutContainer = parent.Parent;
				layoutDocumentPaneGroup = new LayoutDocumentPaneGroup()
				{
					Orientation = Orientation.Horizontal
				};
				layoutContainer.ReplaceChild(parent, layoutDocumentPaneGroup);
				layoutDocumentPaneGroup.Children.Add(parent);
			}
			layoutDocumentPaneGroup.Orientation = Orientation.Horizontal;
			int num = layoutDocumentPaneGroup.IndexOfChild(parent);
			layoutDocumentPaneGroup.InsertChildAt(num + 1, new LayoutDocumentPane(layoutElement));
			layoutElement.IsActive = true;
			layoutElement.Root.CollectGarbage();
		}

		protected virtual void Float()
		{
		}

		protected virtual void InitDefaultCommands()
		{
			this._defaultCloseCommand = new RelayCommand((object p) => this.ExecuteCloseCommand(p), (object p) => this.CanExecuteCloseCommand(p));
			this._defaultFloatCommand = new RelayCommand((object p) => this.ExecuteFloatCommand(p), (object p) => this.CanExecuteFloatCommand(p));
			this._defaultDockAsDocumentCommand = new RelayCommand((object p) => this.ExecuteDockAsDocumentCommand(p), (object p) => this.CanExecuteDockAsDocumentCommand(p));
			this._defaultCloseAllButThisCommand = new RelayCommand((object p) => this.ExecuteCloseAllButThisCommand(p), (object p) => this.CanExecuteCloseAllButThisCommand(p));
			this._defaultCloseAllCommand = new RelayCommand((object p) => this.ExecuteCloseAllCommand(p), (object p) => this.CanExecuteCloseAllCommand(p));
			this._defaultActivateCommand = new RelayCommand((object p) => this.ExecuteActivateCommand(p), (object p) => this.CanExecuteActivateCommand(p));
			this._defaultNewVerticalTabGroupCommand = new RelayCommand((object p) => this.ExecuteNewVerticalTabGroupCommand(p), (object p) => this.CanExecuteNewVerticalTabGroupCommand(p));
			this._defaultNewHorizontalTabGroupCommand = new RelayCommand((object p) => this.ExecuteNewHorizontalTabGroupCommand(p), (object p) => this.CanExecuteNewHorizontalTabGroupCommand(p));
			this._defaultMoveToNextTabGroupCommand = new RelayCommand((object p) => this.ExecuteMoveToNextTabGroupCommand(p), (object p) => this.CanExecuteMoveToNextTabGroupCommand(p));
			this._defaultMoveToPreviousTabGroupCommand = new RelayCommand((object p) => this.ExecuteMoveToPreviousTabGroupCommand(p), (object p) => this.CanExecuteMoveToPreviousTabGroupCommand(p));
		}

		private void LayoutElement_IsActiveChanged(object sender, EventArgs e)
		{
			if (this._isActiveReentrantFlag.CanEnter)
			{
				using (ReentrantFlag._ReentrantFlagHandler __ReentrantFlagHandler = this._isActiveReentrantFlag.Enter())
				{
					BindingOperations.GetBinding(this, LayoutItem.IsActiveProperty);
					this.IsActive = this.LayoutElement.IsActive;
					BindingOperations.GetBinding(this, LayoutItem.IsActiveProperty);
				}
			}
		}

		private void LayoutElement_IsSelectedChanged(object sender, EventArgs e)
		{
			if (this._isSelectedReentrantFlag.CanEnter)
			{
				using (ReentrantFlag._ReentrantFlagHandler __ReentrantFlagHandler = this._isSelectedReentrantFlag.Enter())
				{
					this.IsSelected = this.LayoutElement.IsSelected;
				}
			}
		}

		private static void OnActivateCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnActivateCommandChanged(e);
		}

		protected virtual void OnActivateCommandChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnCanCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnCanCloseChanged(e);
		}

		protected virtual void OnCanCloseChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.LayoutElement != null)
			{
				this.LayoutElement.CanClose = (bool)e.NewValue;
			}
		}

		private static void OnCanFloatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnCanFloatChanged(e);
		}

		protected virtual void OnCanFloatChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.LayoutElement != null)
			{
				this.LayoutElement.CanFloat = (bool)e.NewValue;
			}
		}

		private static void OnCloseAllButThisCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnCloseAllButThisCommandChanged(e);
		}

		protected virtual void OnCloseAllButThisCommandChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnCloseAllCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnCloseAllCommandChanged(e);
		}

		protected virtual void OnCloseAllCommandChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnCloseCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnCloseCommandChanged(e);
		}

		protected virtual void OnCloseCommandChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnContentIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnContentIdChanged(e);
		}

		protected virtual void OnContentIdChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.LayoutElement != null)
			{
				this.LayoutElement.ContentId = (string)e.NewValue;
			}
		}

		private static void OnDockAsDocumentCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnDockAsDocumentCommandChanged(e);
		}

		protected virtual void OnDockAsDocumentCommandChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnFloatCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnFloatCommandChanged(e);
		}

		protected virtual void OnFloatCommandChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnIconSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnIconSourceChanged(e);
		}

		protected virtual void OnIconSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.LayoutElement != null)
			{
				this.LayoutElement.IconSource = this.IconSource;
			}
		}

		private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnIsActiveChanged(e);
		}

		protected virtual void OnIsActiveChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this._isActiveReentrantFlag.CanEnter)
			{
				using (ReentrantFlag._ReentrantFlagHandler __ReentrantFlagHandler = this._isActiveReentrantFlag.Enter())
				{
					if (this.LayoutElement != null)
					{
						this.LayoutElement.IsActive = (bool)e.NewValue;
					}
				}
			}
		}

		private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnIsSelectedChanged(e);
		}

		protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this._isSelectedReentrantFlag.CanEnter)
			{
				using (ReentrantFlag._ReentrantFlagHandler __ReentrantFlagHandler = this._isSelectedReentrantFlag.Enter())
				{
					if (this.LayoutElement != null)
					{
						this.LayoutElement.IsSelected = (bool)e.NewValue;
					}
				}
			}
		}

		private static void OnMoveToNextTabGroupCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnMoveToNextTabGroupCommandChanged(e);
		}

		protected virtual void OnMoveToNextTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnMoveToPreviousTabGroupCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnMoveToPreviousTabGroupCommandChanged(e);
		}

		protected virtual void OnMoveToPreviousTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnNewHorizontalTabGroupCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnNewHorizontalTabGroupCommandChanged(e);
		}

		protected virtual void OnNewHorizontalTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnNewVerticalTabGroupCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnNewVerticalTabGroupCommandChanged(e);
		}

		protected virtual void OnNewVerticalTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)d).OnTitleChanged(e);
		}

		protected virtual void OnTitleChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.LayoutElement != null)
			{
				this.LayoutElement.Title = (string)e.NewValue;
			}
		}

		private static void OnToolTipChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)s).OnToolTipChanged();
		}

		private void OnToolTipChanged()
		{
			if (this.LayoutElement != null)
			{
				this.LayoutElement.ToolTip = base.ToolTip;
			}
		}

		private static void OnVisibilityChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
		{
			((LayoutItem)s).OnVisibilityChanged();
		}

		protected virtual void OnVisibilityChanged()
		{
			if (this.LayoutElement != null && base.Visibility == System.Windows.Visibility.Collapsed)
			{
				this.LayoutElement.Close();
			}
		}

		protected virtual void SetDefaultBindings()
		{
			if (this.CloseCommand == null)
			{
				this.CloseCommand = this._defaultCloseCommand;
			}
			if (this.FloatCommand == null)
			{
				this.FloatCommand = this._defaultFloatCommand;
			}
			if (this.DockAsDocumentCommand == null)
			{
				this.DockAsDocumentCommand = this._defaultDockAsDocumentCommand;
			}
			if (this.CloseAllButThisCommand == null)
			{
				this.CloseAllButThisCommand = this._defaultCloseAllButThisCommand;
			}
			if (this.CloseAllCommand == null)
			{
				this.CloseAllCommand = this._defaultCloseAllCommand;
			}
			if (this.ActivateCommand == null)
			{
				this.ActivateCommand = this._defaultActivateCommand;
			}
			if (this.NewVerticalTabGroupCommand == null)
			{
				this.NewVerticalTabGroupCommand = this._defaultNewVerticalTabGroupCommand;
			}
			if (this.NewHorizontalTabGroupCommand == null)
			{
				this.NewHorizontalTabGroupCommand = this._defaultNewHorizontalTabGroupCommand;
			}
			if (this.MoveToNextTabGroupCommand == null)
			{
				this.MoveToNextTabGroupCommand = this._defaultMoveToNextTabGroupCommand;
			}
			if (this.MoveToPreviousTabGroupCommand == null)
			{
				this.MoveToPreviousTabGroupCommand = this._defaultMoveToPreviousTabGroupCommand;
			}
			this.IsSelected = this.LayoutElement.IsSelected;
			this.IsActive = this.LayoutElement.IsActive;
		}
	}
}