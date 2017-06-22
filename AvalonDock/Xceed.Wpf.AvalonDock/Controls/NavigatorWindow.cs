using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class NavigatorWindow : Window
	{
		private ResourceDictionary currentThemeResourceDictionary;

		private DockingManager _manager;

		private readonly static DependencyPropertyKey DocumentsPropertyKey;

		public readonly static DependencyProperty DocumentsProperty;

		private readonly static DependencyPropertyKey AnchorablesPropertyKey;

		public readonly static DependencyProperty AnchorablesProperty;

		public readonly static DependencyProperty SelectedDocumentProperty;

		private bool _internalSetSelectedDocument;

		public readonly static DependencyProperty SelectedAnchorableProperty;

		public IEnumerable<LayoutAnchorableItem> Anchorables
		{
			get
			{
				return (IEnumerable<LayoutAnchorableItem>)base.GetValue(NavigatorWindow.AnchorablesProperty);
			}
		}

		public LayoutDocumentItem[] Documents
		{
			get
			{
				return (LayoutDocumentItem[])base.GetValue(NavigatorWindow.DocumentsProperty);
			}
		}

		public LayoutAnchorableItem SelectedAnchorable
		{
			get
			{
				return (LayoutAnchorableItem)base.GetValue(NavigatorWindow.SelectedAnchorableProperty);
			}
			set
			{
				base.SetValue(NavigatorWindow.SelectedAnchorableProperty, value);
			}
		}

		public LayoutDocumentItem SelectedDocument
		{
			get
			{
				return (LayoutDocumentItem)base.GetValue(NavigatorWindow.SelectedDocumentProperty);
			}
			set
			{
				base.SetValue(NavigatorWindow.SelectedDocumentProperty, value);
			}
		}

		static NavigatorWindow()
		{
			NavigatorWindow.DocumentsPropertyKey = DependencyProperty.RegisterReadOnly("Documents", typeof(IEnumerable<LayoutDocumentItem>), typeof(NavigatorWindow), new FrameworkPropertyMetadata(null));
			NavigatorWindow.DocumentsProperty = NavigatorWindow.DocumentsPropertyKey.DependencyProperty;
			NavigatorWindow.AnchorablesPropertyKey = DependencyProperty.RegisterReadOnly("Anchorables", typeof(IEnumerable<LayoutAnchorableItem>), typeof(NavigatorWindow), new FrameworkPropertyMetadata(null));
			NavigatorWindow.AnchorablesProperty = NavigatorWindow.AnchorablesPropertyKey.DependencyProperty;
			NavigatorWindow.SelectedDocumentProperty = DependencyProperty.Register("SelectedDocument", typeof(LayoutDocumentItem), typeof(NavigatorWindow), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(NavigatorWindow.OnSelectedDocumentChanged)));
			NavigatorWindow.SelectedAnchorableProperty = DependencyProperty.Register("SelectedAnchorable", typeof(LayoutAnchorableItem), typeof(NavigatorWindow), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(NavigatorWindow.OnSelectedAnchorableChanged)));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigatorWindow), new FrameworkPropertyMetadata(typeof(NavigatorWindow)));
			Window.ShowActivatedProperty.OverrideMetadata(typeof(NavigatorWindow), new FrameworkPropertyMetadata(false));
			Window.ShowInTaskbarProperty.OverrideMetadata(typeof(NavigatorWindow), new FrameworkPropertyMetadata(false));
		}

		internal NavigatorWindow(DockingManager manager)
		{
			this._manager = manager;
			this._internalSetSelectedDocument = true;
			this.SetAnchorables((
				from a in this._manager.Layout.Descendents().OfType<LayoutAnchorable>()
				where a.IsVisible
				select a into d
				select (LayoutAnchorableItem)this._manager.GetLayoutItemFromModel(d)).ToArray<LayoutAnchorableItem>());
			this.SetDocuments((
				from d in this._manager.Layout.Descendents().OfType<LayoutDocument>()
				orderby d.LastActivationTimeStamp.GetValueOrDefault() descending
				select (LayoutDocumentItem)this._manager.GetLayoutItemFromModel(d)).ToArray<LayoutDocumentItem>());
			this._internalSetSelectedDocument = false;
			if ((int)this.Documents.Length > 1)
			{
				this.InternalSetSelectedDocument(this.Documents[1]);
			}
			base.DataContext = this;
			base.Loaded += new RoutedEventHandler(this.OnLoaded);
			base.Unloaded += new RoutedEventHandler(this.OnUnloaded);
			this.UpdateThemeResources(null);
		}

		private void InternalSetSelectedDocument(LayoutDocumentItem documentToSelect)
		{
			this._internalSetSelectedDocument = true;
			this.SelectedDocument = documentToSelect;
			this._internalSetSelectedDocument = false;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			base.Loaded -= new RoutedEventHandler(this.OnLoaded);
			base.Focus();
			base.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Tab)
			{
				this.SelectNextDocument();
				e.Handled = true;
			}
			base.OnPreviewKeyDown(e);
		}

		protected override void OnPreviewKeyUp(KeyEventArgs e)
		{
			if (e.Key != Key.Tab)
			{
				if (this.SelectedAnchorable != null && this.SelectedAnchorable.ActivateCommand.CanExecute(null))
				{
					this.SelectedAnchorable.ActivateCommand.Execute(null);
				}
				if (this.SelectedAnchorable == null && this.SelectedDocument != null && this.SelectedDocument.ActivateCommand.CanExecute(null))
				{
					this.SelectedDocument.ActivateCommand.Execute(null);
				}
				base.Close();
				e.Handled = true;
			}
			base.OnPreviewKeyUp(e);
		}

		private static void OnSelectedAnchorableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((NavigatorWindow)d).OnSelectedAnchorableChanged(e);
		}

		protected virtual void OnSelectedAnchorableChanged(DependencyPropertyChangedEventArgs e)
		{
			object newValue = e.NewValue;
			if (this.SelectedAnchorable != null && this.SelectedAnchorable.ActivateCommand.CanExecute(null))
			{
				this.SelectedAnchorable.ActivateCommand.Execute(null);
				base.Close();
			}
		}

		private static void OnSelectedDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((NavigatorWindow)d).OnSelectedDocumentChanged(e);
		}

		protected virtual void OnSelectedDocumentChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this._internalSetSelectedDocument)
			{
				return;
			}
			if (this.SelectedDocument != null && this.SelectedDocument.ActivateCommand.CanExecute(null))
			{
				this.SelectedDocument.ActivateCommand.Execute(null);
				base.Hide();
			}
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			base.Unloaded -= new RoutedEventHandler(this.OnUnloaded);
		}

		internal void SelectNextDocument()
		{
			if (this.SelectedDocument != null)
			{
				int num = this.Documents.IndexOf<LayoutDocumentItem>(this.SelectedDocument);
				num++;
				if (num == (int)this.Documents.Length)
				{
					num = 0;
				}
				this.InternalSetSelectedDocument(this.Documents[num]);
			}
		}

		protected void SetAnchorables(IEnumerable<LayoutAnchorableItem> value)
		{
			base.SetValue(NavigatorWindow.AnchorablesPropertyKey, value);
		}

		protected void SetDocuments(LayoutDocumentItem[] value)
		{
			base.SetValue(NavigatorWindow.DocumentsPropertyKey, value);
		}

		internal void UpdateThemeResources(Theme oldTheme = null)
		{
			if (oldTheme != null)
			{
				if (!(oldTheme is DictionaryTheme))
				{
					ResourceDictionary resourceDictionaries = base.Resources.MergedDictionaries.FirstOrDefault<ResourceDictionary>((ResourceDictionary r) => r.Source == oldTheme.GetResourceUri());
					if (resourceDictionaries != null)
					{
						base.Resources.MergedDictionaries.Remove(resourceDictionaries);
					}
				}
				else if (this.currentThemeResourceDictionary != null)
				{
					base.Resources.MergedDictionaries.Remove(this.currentThemeResourceDictionary);
					this.currentThemeResourceDictionary = null;
				}
			}
			if (this._manager.Theme != null)
			{
				if (this._manager.Theme is DictionaryTheme)
				{
					this.currentThemeResourceDictionary = ((DictionaryTheme)this._manager.Theme).ThemeResourceDictionary;
					base.Resources.MergedDictionaries.Add(this.currentThemeResourceDictionary);
					return;
				}
				base.Resources.MergedDictionaries.Add(new ResourceDictionary()
				{
					Source = this._manager.Theme.GetResourceUri()
				});
			}
		}
	}
}