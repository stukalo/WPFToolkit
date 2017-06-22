using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Markup;
using System.Xml.Serialization;
using Xceed.Wpf.AvalonDock;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[ContentProperty("RootPanel")]
	[Serializable]
	public class LayoutRoot : LayoutElement, ILayoutContainer, ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging, ILayoutRoot
	{
		private LayoutPanel _rootPanel;

		private LayoutAnchorSide _topSide;

		private LayoutAnchorSide _rightSide;

		private LayoutAnchorSide _leftSide;

		private LayoutAnchorSide _bottomSide;

		private ObservableCollection<LayoutFloatingWindow> _floatingWindows;

		private ObservableCollection<LayoutAnchorable> _hiddenAnchorables;

		[NonSerialized]
		private WeakReference _activeContent;

		private bool _activeContentSet;

		[NonSerialized]
		private WeakReference _lastFocusedDocument;

		[NonSerialized]
		private bool _lastFocusedDocumentSet;

		[NonSerialized]
		private DockingManager _manager;

		[XmlIgnore]
		public LayoutContent ActiveContent
		{
			get
			{
				return this._activeContent.GetValueOrDefault<LayoutContent>();
			}
			set
			{
				LayoutContent activeContent = this.ActiveContent;
				if (activeContent != value)
				{
					this.InternalSetActiveContent(activeContent, value);
				}
			}
		}

		public LayoutAnchorSide BottomSide
		{
			get
			{
				return JustDecompileGenerated_get_BottomSide();
			}
			set
			{
				JustDecompileGenerated_set_BottomSide(value);
			}
		}

		public LayoutAnchorSide JustDecompileGenerated_get_BottomSide()
		{
			return this._bottomSide;
		}

		public void JustDecompileGenerated_set_BottomSide(LayoutAnchorSide value)
		{
			if (this._bottomSide != value)
			{
				this.RaisePropertyChanging("BottomSide");
				this._bottomSide = value;
				if (this._bottomSide != null)
				{
					this._bottomSide.Parent = this;
				}
				this.RaisePropertyChanged("BottomSide");
			}
		}

		public IEnumerable<ILayoutElement> Children
		{
			get
			{
				if (this.RootPanel != null)
				{
					yield return this.RootPanel;
				}
				if (this._floatingWindows != null)
				{
					foreach (LayoutFloatingWindow _floatingWindow in this._floatingWindows)
					{
						yield return _floatingWindow;
					}
				}
				if (this.TopSide != null)
				{
					yield return this.TopSide;
				}
				if (this.RightSide != null)
				{
					yield return this.RightSide;
				}
				if (this.BottomSide != null)
				{
					yield return this.BottomSide;
				}
				if (this.LeftSide != null)
				{
					yield return this.LeftSide;
				}
				if (this._hiddenAnchorables != null)
				{
					foreach (LayoutAnchorable _hiddenAnchorable in this._hiddenAnchorables)
					{
						yield return _hiddenAnchorable;
					}
				}
			}
		}

		public int ChildrenCount
		{
			get
			{
				return 5 + (this._floatingWindows != null ? this._floatingWindows.Count : 0) + (this._hiddenAnchorables != null ? this._hiddenAnchorables.Count : 0);
			}
		}

		public ObservableCollection<LayoutFloatingWindow> FloatingWindows
		{
			get
			{
				if (this._floatingWindows == null)
				{
					this._floatingWindows = new ObservableCollection<LayoutFloatingWindow>();
					this._floatingWindows.CollectionChanged += new NotifyCollectionChangedEventHandler(this._floatingWindows_CollectionChanged);
				}
				return this._floatingWindows;
			}
		}

		public ObservableCollection<LayoutAnchorable> Hidden
		{
			get
			{
				if (this._hiddenAnchorables == null)
				{
					this._hiddenAnchorables = new ObservableCollection<LayoutAnchorable>();
					this._hiddenAnchorables.CollectionChanged += new NotifyCollectionChangedEventHandler(this._hiddenAnchorables_CollectionChanged);
				}
				return this._hiddenAnchorables;
			}
		}

		[XmlIgnore]
		public LayoutContent LastFocusedDocument
		{
			get
			{
				return this._lastFocusedDocument.GetValueOrDefault<LayoutContent>();
			}
			private set
			{
				LayoutContent lastFocusedDocument = this.LastFocusedDocument;
				if (lastFocusedDocument != value)
				{
					this.RaisePropertyChanging("LastFocusedDocument");
					if (lastFocusedDocument != null)
					{
						lastFocusedDocument.IsLastFocusedDocument = false;
					}
					this._lastFocusedDocument = new WeakReference(value);
					lastFocusedDocument = this.LastFocusedDocument;
					if (lastFocusedDocument != null)
					{
						lastFocusedDocument.IsLastFocusedDocument = true;
					}
					this._lastFocusedDocumentSet = lastFocusedDocument != null;
					this.RaisePropertyChanged("LastFocusedDocument");
				}
			}
		}

		public LayoutAnchorSide LeftSide
		{
			get
			{
				return JustDecompileGenerated_get_LeftSide();
			}
			set
			{
				JustDecompileGenerated_set_LeftSide(value);
			}
		}

		public LayoutAnchorSide JustDecompileGenerated_get_LeftSide()
		{
			return this._leftSide;
		}

		public void JustDecompileGenerated_set_LeftSide(LayoutAnchorSide value)
		{
			if (this._leftSide != value)
			{
				this.RaisePropertyChanging("LeftSide");
				this._leftSide = value;
				if (this._leftSide != null)
				{
					this._leftSide.Parent = this;
				}
				this.RaisePropertyChanged("LeftSide");
			}
		}

		[XmlIgnore]
		public DockingManager Manager
		{
			get
			{
				return JustDecompileGenerated_get_Manager();
			}
			set
			{
				JustDecompileGenerated_set_Manager(value);
			}
		}

		public DockingManager JustDecompileGenerated_get_Manager()
		{
			return this._manager;
		}

		internal void JustDecompileGenerated_set_Manager(DockingManager value)
		{
			if (this._manager != value)
			{
				this.RaisePropertyChanging("Manager");
				this._manager = value;
				this.RaisePropertyChanged("Manager");
			}
		}

		public LayoutAnchorSide RightSide
		{
			get
			{
				return JustDecompileGenerated_get_RightSide();
			}
			set
			{
				JustDecompileGenerated_set_RightSide(value);
			}
		}

		public LayoutAnchorSide JustDecompileGenerated_get_RightSide()
		{
			return this._rightSide;
		}

		public void JustDecompileGenerated_set_RightSide(LayoutAnchorSide value)
		{
			if (this._rightSide != value)
			{
				this.RaisePropertyChanging("RightSide");
				this._rightSide = value;
				if (this._rightSide != null)
				{
					this._rightSide.Parent = this;
				}
				this.RaisePropertyChanged("RightSide");
			}
		}

		public LayoutPanel RootPanel
		{
			get
			{
				return JustDecompileGenerated_get_RootPanel();
			}
			set
			{
				JustDecompileGenerated_set_RootPanel(value);
			}
		}

		public LayoutPanel JustDecompileGenerated_get_RootPanel()
		{
			return this._rootPanel;
		}

		public void JustDecompileGenerated_set_RootPanel(LayoutPanel value)
		{
			if (this._rootPanel != value)
			{
				this.RaisePropertyChanging("RootPanel");
				if (this._rootPanel != null && this._rootPanel.Parent == this)
				{
					this._rootPanel.Parent = null;
				}
				this._rootPanel = value;
				if (this._rootPanel == null)
				{
					this._rootPanel = new LayoutPanel(new LayoutDocumentPane());
				}
				if (this._rootPanel != null)
				{
					this._rootPanel.Parent = this;
				}
				this.RaisePropertyChanged("RootPanel");
			}
		}

		public LayoutAnchorSide TopSide
		{
			get
			{
				return JustDecompileGenerated_get_TopSide();
			}
			set
			{
				JustDecompileGenerated_set_TopSide(value);
			}
		}

		public LayoutAnchorSide JustDecompileGenerated_get_TopSide()
		{
			return this._topSide;
		}

		public void JustDecompileGenerated_set_TopSide(LayoutAnchorSide value)
		{
			if (this._topSide != value)
			{
				this.RaisePropertyChanging("TopSide");
				this._topSide = value;
				if (this._topSide != null)
				{
					this._topSide.Parent = this;
				}
				this.RaisePropertyChanged("TopSide");
			}
		}

		public LayoutRoot()
		{
			this.RightSide = new LayoutAnchorSide();
			this.LeftSide = new LayoutAnchorSide();
			this.TopSide = new LayoutAnchorSide();
			this.BottomSide = new LayoutAnchorSide();
			this.RootPanel = new LayoutPanel(new LayoutDocumentPane());
		}

		private void _floatingWindows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null && (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace))
			{
				foreach (LayoutFloatingWindow oldItem in e.OldItems)
				{
					if (oldItem.Parent != this)
					{
						continue;
					}
					oldItem.Parent = null;
				}
			}
			if (e.NewItems != null && (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace))
			{
				foreach (object newItem in e.NewItems)
				{
					((LayoutFloatingWindow)newItem).Parent = this;
				}
			}
		}

		private void _hiddenAnchorables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if ((e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace) && e.OldItems != null)
			{
				foreach (LayoutAnchorable oldItem in e.OldItems)
				{
					if (oldItem.Parent != this)
					{
						continue;
					}
					oldItem.Parent = null;
				}
			}
			if ((e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace) && e.NewItems != null)
			{
				foreach (LayoutAnchorable newItem in e.NewItems)
				{
					if (newItem.Parent == this)
					{
						continue;
					}
					if (newItem.Parent != null)
					{
						newItem.Parent.RemoveChild(newItem);
					}
					newItem.Parent = this;
				}
			}
		}

		public void CollectGarbage()
		{
			int num;
			Func<LayoutContent, bool> func = null;
			bool flag = true;
			do
			{
				flag = true;
				foreach (ILayoutPreviousContainer layoutPreviousContainer in this.Descendents().OfType<ILayoutPreviousContainer>().Where<ILayoutPreviousContainer>((ILayoutPreviousContainer c) => {
					if (c.PreviousContainer == null)
					{
						return false;
					}
					if (c.PreviousContainer.Parent == null)
					{
						return true;
					}
					return c.PreviousContainer.Parent.Root != this;
				}))
				{
					layoutPreviousContainer.PreviousContainer = null;
				}
				foreach (ILayoutPane layoutPane in 
					from p in this.Descendents().OfType<ILayoutPane>()
					where p.ChildrenCount == 0
					select p)
				{
					IEnumerable<LayoutContent> layoutContents = this.Descendents().OfType<LayoutContent>();
					Func<LayoutContent, bool> func1 = func;
					if (func1 == null)
					{
						Func<LayoutContent, bool> previousContainer = (LayoutContent c) => {
							if (((ILayoutPreviousContainer)c).PreviousContainer != layoutPane)
							{
								return false;
							}
							return !c.IsFloating;
						};
						Func<LayoutContent, bool> func2 = previousContainer;
						func = previousContainer;
						func1 = func2;
					}
					foreach (LayoutContent layoutContent in layoutContents.Where<LayoutContent>(func1))
					{
						if (layoutContent is LayoutAnchorable && !((LayoutAnchorable)layoutContent).IsVisible)
						{
							continue;
						}
						((ILayoutPreviousContainer)layoutContent).PreviousContainer = null;
						layoutContent.PreviousContainerIndex = -1;
					}
					if (layoutPane is LayoutDocumentPane && this.Descendents().OfType<LayoutDocumentPane>().Count<LayoutDocumentPane>((LayoutDocumentPane c) => c != layoutPane) == 0 || this.Descendents().OfType<ILayoutPreviousContainer>().Any<ILayoutPreviousContainer>((ILayoutPreviousContainer c) => c.PreviousContainer == layoutPane))
					{
						continue;
					}
					layoutPane.Parent.RemoveChild(layoutPane);
					flag = false;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
					using (IEnumerator<LayoutAnchorablePaneGroup> enumerator = (
						from p in this.Descendents().OfType<LayoutAnchorablePaneGroup>()
						where p.ChildrenCount == 0
						select p).GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							LayoutAnchorablePaneGroup current = enumerator.Current;
							current.Parent.RemoveChild(current);
							flag = false;
						}
					}
				}
				if (!flag)
				{
					using (IEnumerator<LayoutPanel> enumerator1 = (
						from p in this.Descendents().OfType<LayoutPanel>()
						where p.ChildrenCount == 0
						select p).GetEnumerator())
					{
						if (enumerator1.MoveNext())
						{
							LayoutPanel layoutPanel = enumerator1.Current;
							layoutPanel.Parent.RemoveChild(layoutPanel);
							flag = false;
						}
					}
				}
				if (!flag)
				{
					using (IEnumerator<LayoutFloatingWindow> enumerator2 = (
						from p in this.Descendents().OfType<LayoutFloatingWindow>()
						where p.ChildrenCount == 0
						select p).GetEnumerator())
					{
						if (enumerator2.MoveNext())
						{
							LayoutFloatingWindow layoutFloatingWindow = enumerator2.Current;
							layoutFloatingWindow.Parent.RemoveChild(layoutFloatingWindow);
							flag = false;
						}
					}
				}
				if (flag)
				{
					continue;
				}
				using (IEnumerator<LayoutAnchorGroup> enumerator3 = (
					from p in this.Descendents().OfType<LayoutAnchorGroup>()
					where p.ChildrenCount == 0
					select p).GetEnumerator())
				{
					if (enumerator3.MoveNext())
					{
						LayoutAnchorGroup layoutAnchorGroup = enumerator3.Current;
						layoutAnchorGroup.Parent.RemoveChild(layoutAnchorGroup);
						flag = false;
					}
				}
			}
			while (!flag);
			do
			{
				flag = true;
				LayoutAnchorablePaneGroup[] array = this.Descendents().OfType<LayoutAnchorablePaneGroup>().Where<LayoutAnchorablePaneGroup>((LayoutAnchorablePaneGroup p) => {
					if (p.ChildrenCount != 1)
					{
						return false;
					}
					return p.Children[0] is LayoutAnchorablePaneGroup;
				}).ToArray<LayoutAnchorablePaneGroup>();
				num = 0;
				if (num >= (int)array.Length)
				{
					continue;
				}
				LayoutAnchorablePaneGroup orientation = array[num];
				LayoutAnchorablePaneGroup item = orientation.Children[0] as LayoutAnchorablePaneGroup;
				orientation.Orientation = item.Orientation;
				orientation.RemoveChild(item);
				while (item.ChildrenCount > 0)
				{
					orientation.InsertChildAt(orientation.ChildrenCount, item.Children[0]);
				}
				flag = false;
			}
			while (!flag);
			do
			{
				flag = true;
				LayoutDocumentPaneGroup[] layoutDocumentPaneGroupArray = this.Descendents().OfType<LayoutDocumentPaneGroup>().Where<LayoutDocumentPaneGroup>((LayoutDocumentPaneGroup p) => {
					if (p.ChildrenCount != 1)
					{
						return false;
					}
					return p.Children[0] is LayoutDocumentPaneGroup;
				}).ToArray<LayoutDocumentPaneGroup>();
				num = 0;
				if (num >= (int)layoutDocumentPaneGroupArray.Length)
				{
					continue;
				}
				LayoutDocumentPaneGroup layoutDocumentPaneGroup = layoutDocumentPaneGroupArray[num];
				LayoutDocumentPaneGroup item1 = layoutDocumentPaneGroup.Children[0] as LayoutDocumentPaneGroup;
				layoutDocumentPaneGroup.Orientation = item1.Orientation;
				layoutDocumentPaneGroup.RemoveChild(item1);
				while (item1.ChildrenCount > 0)
				{
					layoutDocumentPaneGroup.InsertChildAt(layoutDocumentPaneGroup.ChildrenCount, item1.Children[0]);
				}
				flag = false;
			}
			while (!flag);
			this.UpdateActiveContentProperty();
		}

		public override void ConsoleDump(int tab)
		{
			Trace.Write(new string(' ', tab * 4));
			Trace.WriteLine("RootPanel()");
			this.RootPanel.ConsoleDump(tab + 1);
			Trace.Write(new string(' ', tab * 4));
			Trace.WriteLine("FloatingWindows()");
			foreach (LayoutFloatingWindow floatingWindow in this.FloatingWindows)
			{
				floatingWindow.ConsoleDump(tab + 1);
			}
			Trace.Write(new string(' ', tab * 4));
			Trace.WriteLine("Hidden()");
			foreach (LayoutAnchorable hidden in this.Hidden)
			{
				hidden.ConsoleDump(tab + 1);
			}
		}

		internal void FireLayoutUpdated()
		{
			if (this.Updated != null)
			{
				this.Updated(this, EventArgs.Empty);
			}
		}

		private void InternalSetActiveContent(LayoutContent currentValue, LayoutContent newActiveContent)
		{
			this.RaisePropertyChanging("ActiveContent");
			if (currentValue != null)
			{
				currentValue.IsActive = false;
			}
			this._activeContent = new WeakReference(newActiveContent);
			currentValue = this.ActiveContent;
			if (currentValue != null)
			{
				currentValue.IsActive = true;
			}
			this.RaisePropertyChanged("ActiveContent");
			this._activeContentSet = currentValue != null;
			if (currentValue == null)
			{
				this.LastFocusedDocument = null;
			}
			else if (currentValue.Parent is LayoutDocumentPane || currentValue is LayoutDocument)
			{
				this.LastFocusedDocument = currentValue;
				return;
			}
		}

		internal void OnLayoutElementAdded(LayoutElement element)
		{
			if (this.ElementAdded != null)
			{
				this.ElementAdded(this, new LayoutElementEventArgs(element));
			}
		}

		internal void OnLayoutElementRemoved(LayoutElement element)
		{
			if (element.Descendents().OfType<LayoutContent>().Any<LayoutContent>((LayoutContent c) => c == this.LastFocusedDocument))
			{
				this.LastFocusedDocument = null;
			}
			if (element.Descendents().OfType<LayoutContent>().Any<LayoutContent>((LayoutContent c) => c == this.ActiveContent))
			{
				this.ActiveContent = null;
			}
			if (this.ElementRemoved != null)
			{
				this.ElementRemoved(this, new LayoutElementEventArgs(element));
			}
		}

		public void RemoveChild(ILayoutElement element)
		{
			if (element == this.RootPanel)
			{
				this.RootPanel = null;
				return;
			}
			if (this._floatingWindows != null && this._floatingWindows.Contains(element))
			{
				this._floatingWindows.Remove(element as LayoutFloatingWindow);
				return;
			}
			if (this._hiddenAnchorables != null && this._hiddenAnchorables.Contains(element))
			{
				this._hiddenAnchorables.Remove(element as LayoutAnchorable);
				return;
			}
			if (element == this.TopSide)
			{
				this.TopSide = null;
				return;
			}
			if (element == this.RightSide)
			{
				this.RightSide = null;
				return;
			}
			if (element == this.BottomSide)
			{
				this.BottomSide = null;
				return;
			}
			if (element == this.LeftSide)
			{
				this.LeftSide = null;
			}
		}

		public void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
		{
			if (oldElement == this.RootPanel)
			{
				this.RootPanel = (LayoutPanel)newElement;
				return;
			}
			if (this._floatingWindows != null && this._floatingWindows.Contains(oldElement))
			{
				int num = this._floatingWindows.IndexOf(oldElement as LayoutFloatingWindow);
				this._floatingWindows.Remove(oldElement as LayoutFloatingWindow);
				this._floatingWindows.Insert(num, newElement as LayoutFloatingWindow);
				return;
			}
			if (this._hiddenAnchorables != null && this._hiddenAnchorables.Contains(oldElement))
			{
				int num1 = this._hiddenAnchorables.IndexOf(oldElement as LayoutAnchorable);
				this._hiddenAnchorables.Remove(oldElement as LayoutAnchorable);
				this._hiddenAnchorables.Insert(num1, newElement as LayoutAnchorable);
				return;
			}
			if (oldElement == this.TopSide)
			{
				this.TopSide = (LayoutAnchorSide)newElement;
				return;
			}
			if (oldElement == this.RightSide)
			{
				this.RightSide = (LayoutAnchorSide)newElement;
				return;
			}
			if (oldElement == this.BottomSide)
			{
				this.BottomSide = (LayoutAnchorSide)newElement;
				return;
			}
			if (oldElement == this.LeftSide)
			{
				this.LeftSide = (LayoutAnchorSide)newElement;
			}
		}

		private void UpdateActiveContentProperty()
		{
			LayoutContent activeContent = this.ActiveContent;
			if (this._activeContentSet && (activeContent == null || activeContent.Root != this))
			{
				this._activeContentSet = false;
				this.InternalSetActiveContent(activeContent, null);
			}
		}

		public event EventHandler<LayoutElementEventArgs> ElementAdded;

		public event EventHandler<LayoutElementEventArgs> ElementRemoved;

		public event EventHandler Updated;
	}
}