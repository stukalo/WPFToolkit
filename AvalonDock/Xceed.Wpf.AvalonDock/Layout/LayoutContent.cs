using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Xceed.Wpf.AvalonDock;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[ContentProperty("Content")]
	[Serializable]
	public abstract class LayoutContent : LayoutElement, IXmlSerializable, ILayoutElementForFloatingWindow, IComparable<LayoutContent>, ILayoutPreviousContainer
	{
		public readonly static DependencyProperty TitleProperty;

		[NonSerialized]
		private object _content;

		private string _contentId;

		private bool _isSelected;

		[NonSerialized]
		private bool _isActive;

		private bool _isLastFocusedDocument;

		[NonSerialized]
		private ILayoutContainer _previousContainer;

		[NonSerialized]
		private int _previousContainerIndex = -1;

		private DateTime? _lastActivationTimeStamp;

		private double _floatingWidth;

		private double _floatingHeight;

		private double _floatingLeft;

		private double _floatingTop;

		private bool _isMaximized;

		private object _toolTip;

		private ImageSource _iconSource;

		private bool _canClose = true;

		private bool _canFloat = true;

		private bool _isEnabled = true;

		public bool CanClose
		{
			get
			{
				return this._canClose;
			}
			set
			{
				if (this._canClose != value)
				{
					this._canClose = value;
					this.RaisePropertyChanged("CanClose");
				}
			}
		}

		public bool CanFloat
		{
			get
			{
				return this._canFloat;
			}
			set
			{
				if (this._canFloat != value)
				{
					this._canFloat = value;
					this.RaisePropertyChanged("CanFloat");
				}
			}
		}

		[XmlIgnore]
		public object Content
		{
			get
			{
				return this._content;
			}
			set
			{
				if (this._content != value)
				{
					this.RaisePropertyChanging("Content");
					this._content = value;
					this.RaisePropertyChanged("Content");
				}
			}
		}

		public string ContentId
		{
			get
			{
				if (this._contentId == null)
				{
					FrameworkElement frameworkElement = this._content as FrameworkElement;
					if (frameworkElement != null && !string.IsNullOrWhiteSpace(frameworkElement.Name))
					{
						return frameworkElement.Name;
					}
				}
				return this._contentId;
			}
			set
			{
				if (this._contentId != value)
				{
					this._contentId = value;
					this.RaisePropertyChanged("ContentId");
				}
			}
		}

		public double FloatingHeight
		{
			get
			{
				return this._floatingHeight;
			}
			set
			{
				if (this._floatingHeight != value)
				{
					this.RaisePropertyChanging("FloatingHeight");
					this._floatingHeight = value;
					this.RaisePropertyChanged("FloatingHeight");
				}
			}
		}

		public double FloatingLeft
		{
			get
			{
				return this._floatingLeft;
			}
			set
			{
				if (this._floatingLeft != value)
				{
					this.RaisePropertyChanging("FloatingLeft");
					this._floatingLeft = value;
					this.RaisePropertyChanged("FloatingLeft");
				}
			}
		}

		public double FloatingTop
		{
			get
			{
				return this._floatingTop;
			}
			set
			{
				if (this._floatingTop != value)
				{
					this.RaisePropertyChanging("FloatingTop");
					this._floatingTop = value;
					this.RaisePropertyChanged("FloatingTop");
				}
			}
		}

		public double FloatingWidth
		{
			get
			{
				return this._floatingWidth;
			}
			set
			{
				if (this._floatingWidth != value)
				{
					this.RaisePropertyChanging("FloatingWidth");
					this._floatingWidth = value;
					this.RaisePropertyChanged("FloatingWidth");
				}
			}
		}

		public ImageSource IconSource
		{
			get
			{
				return this._iconSource;
			}
			set
			{
				if (this._iconSource != value)
				{
					this._iconSource = value;
					this.RaisePropertyChanged("IconSource");
				}
			}
		}

		[XmlIgnore]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (this._isActive != value)
				{
					this.RaisePropertyChanging("IsActive");
					bool flag = this._isActive;
					this._isActive = value;
					ILayoutRoot root = base.Root;
					if (root != null && this._isActive)
					{
						root.ActiveContent = this;
					}
					if (this._isActive)
					{
						this.IsSelected = true;
					}
					this.OnIsActiveChanged(flag, value);
					this.RaisePropertyChanged("IsActive");
				}
			}
		}

		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (this._isEnabled != value)
				{
					this._isEnabled = value;
					this.RaisePropertyChanged("IsEnabled");
				}
			}
		}

		public bool IsFloating
		{
			get
			{
				return this.FindParent<LayoutFloatingWindow>() != null;
			}
		}

		public bool IsLastFocusedDocument
		{
			get
			{
				return this._isLastFocusedDocument;
			}
			internal set
			{
				if (this._isLastFocusedDocument != value)
				{
					this.RaisePropertyChanging("IsLastFocusedDocument");
					this._isLastFocusedDocument = value;
					this.RaisePropertyChanged("IsLastFocusedDocument");
				}
			}
		}

		public bool IsMaximized
		{
			get
			{
				return this._isMaximized;
			}
			set
			{
				if (this._isMaximized != value)
				{
					this.RaisePropertyChanging("IsMaximized");
					this._isMaximized = value;
					this.RaisePropertyChanged("IsMaximized");
				}
			}
		}

		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (this._isSelected != value)
				{
					bool flag = this._isSelected;
					this.RaisePropertyChanging("IsSelected");
					this._isSelected = value;
					ILayoutContentSelector parent = base.Parent as ILayoutContentSelector;
					if (parent != null)
					{
						parent.SelectedContentIndex = (this._isSelected ? parent.IndexOf(this) : -1);
					}
					this.OnIsSelectedChanged(flag, value);
					this.RaisePropertyChanged("IsSelected");
				}
			}
		}

		public DateTime? LastActivationTimeStamp
		{
			get
			{
				return this._lastActivationTimeStamp;
			}
			set
			{
				bool flag;
				DateTime? nullable = this._lastActivationTimeStamp;
				DateTime? nullable1 = value;
				if (nullable.HasValue == nullable1.HasValue)
				{
					flag = (nullable.HasValue ? nullable.GetValueOrDefault() != nullable1.GetValueOrDefault() : false);
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					this._lastActivationTimeStamp = value;
					this.RaisePropertyChanged("LastActivationTimeStamp");
				}
			}
		}

		protected ILayoutContainer PreviousContainer
		{
			get
			{
				return ((ILayoutPreviousContainer)this).PreviousContainer;
			}
			set
			{
				((ILayoutPreviousContainer)this).PreviousContainer = value;
			}
		}

		protected string PreviousContainerId
		{
			get
			{
				return ((ILayoutPreviousContainer)this).PreviousContainerId;
			}
			set
			{
				((ILayoutPreviousContainer)this).PreviousContainerId = value;
			}
		}

		[XmlIgnore]
		public int PreviousContainerIndex
		{
			get
			{
				return this._previousContainerIndex;
			}
			set
			{
				if (this._previousContainerIndex != value)
				{
					this._previousContainerIndex = value;
					this.RaisePropertyChanged("PreviousContainerIndex");
				}
			}
		}

		public string Title
		{
			get
			{
				return (string)base.GetValue(LayoutContent.TitleProperty);
			}
			set
			{
				base.SetValue(LayoutContent.TitleProperty, value);
			}
		}

		public object ToolTip
		{
			get
			{
				return this._toolTip;
			}
			set
			{
				if (this._toolTip != value)
				{
					this._toolTip = value;
					this.RaisePropertyChanged("ToolTip");
				}
			}
		}

		[XmlIgnore]
		ILayoutContainer Xceed.Wpf.AvalonDock.Layout.ILayoutPreviousContainer.PreviousContainer
		{
			get
			{
				return this._previousContainer;
			}
			set
			{
				if (this._previousContainer != value)
				{
					this._previousContainer = value;
					this.RaisePropertyChanged("PreviousContainer");
					ILayoutPaneSerializable str = this._previousContainer as ILayoutPaneSerializable;
					if (str != null && str.Id == null)
					{
						str.Id = Guid.NewGuid().ToString();
					}
				}
			}
		}

		[XmlIgnore]
		string Xceed.Wpf.AvalonDock.Layout.ILayoutPreviousContainer.PreviousContainerId
		{
			get;
			set;
		}

		static LayoutContent()
		{
			LayoutContent.TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(LayoutContent), new UIPropertyMetadata(null, new PropertyChangedCallback(LayoutContent.OnTitlePropertyChanged), new CoerceValueCallback(LayoutContent.CoerceTitleValue)));
		}

		internal LayoutContent()
		{
		}

		public abstract void Close();

		internal void CloseInternal()
		{
			ILayoutRoot root = base.Root;
			base.Parent.RemoveChild(this);
			if (root != null)
			{
				root.CollectGarbage();
			}
			this.OnClosed();
		}

		private static object CoerceTitleValue(DependencyObject obj, object value)
		{
			LayoutContent layoutContent = (LayoutContent)obj;
			if ((string)value != layoutContent.Title)
			{
				layoutContent.RaisePropertyChanging(LayoutContent.TitleProperty.Name);
			}
			return value;
		}

		public int CompareTo(LayoutContent other)
		{
			IComparable content = this.Content as IComparable;
			if (content != null)
			{
				return content.CompareTo(other.Content);
			}
			return string.Compare(this.Title, other.Title);
		}

		public void Dock()
		{
			if (this.PreviousContainer == null)
			{
				this.InternalDock();
			}
			else
			{
				ILayoutContainer parent = base.Parent;
				int num = (parent is ILayoutGroup ? (parent as ILayoutGroup).IndexOfChild(this) : -1);
				ILayoutGroup previousContainer = this.PreviousContainer as ILayoutGroup;
				if (this.PreviousContainerIndex >= previousContainer.ChildrenCount)
				{
					previousContainer.InsertChildAt(previousContainer.ChildrenCount, this);
				}
				else
				{
					previousContainer.InsertChildAt(this.PreviousContainerIndex, this);
				}
				if (num <= -1)
				{
					this.PreviousContainer = null;
					this.PreviousContainerIndex = 0;
				}
				else
				{
					this.PreviousContainer = parent;
					this.PreviousContainerIndex = num;
				}
				this.IsSelected = true;
				this.IsActive = true;
			}
			base.Root.CollectGarbage();
		}

		public void DockAsDocument()
		{
			LayoutDocumentPane layoutDocumentPane;
			LayoutRoot root = base.Root as LayoutRoot;
			if (root == null)
			{
				throw new InvalidOperationException();
			}
			if (base.Parent is LayoutDocumentPane)
			{
				return;
			}
			if (this.PreviousContainer is LayoutDocumentPane)
			{
				this.Dock();
				return;
			}
			layoutDocumentPane = (root.LastFocusedDocument == null ? root.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>() : root.LastFocusedDocument.Parent as LayoutDocumentPane);
			if (layoutDocumentPane != null)
			{
				layoutDocumentPane.Children.Add(this);
				root.CollectGarbage();
			}
			this.IsSelected = true;
			this.IsActive = true;
		}

		public void Float()
		{
			if (this.PreviousContainer == null || this.PreviousContainer.FindParent<LayoutFloatingWindow>() == null)
			{
				base.Root.Manager.StartDraggingFloatingWindowForContent(this, false);
				this.IsSelected = true;
				this.IsActive = true;
				return;
			}
			ILayoutPane parent = base.Parent as ILayoutPane;
			int num = (parent as ILayoutGroup).IndexOfChild(this);
			ILayoutGroup previousContainer = this.PreviousContainer as ILayoutGroup;
			if (this.PreviousContainerIndex >= previousContainer.ChildrenCount)
			{
				previousContainer.InsertChildAt(previousContainer.ChildrenCount, this);
			}
			else
			{
				previousContainer.InsertChildAt(this.PreviousContainerIndex, this);
			}
			this.PreviousContainer = parent;
			this.PreviousContainerIndex = num;
			this.IsSelected = true;
			this.IsActive = true;
			base.Root.CollectGarbage();
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		protected virtual void InternalDock()
		{
		}

		protected virtual void OnClosed()
		{
			if (this.Closed != null)
			{
				this.Closed(this, EventArgs.Empty);
			}
		}

		protected virtual void OnClosing(CancelEventArgs args)
		{
			if (this.Closing != null)
			{
				this.Closing(this, args);
			}
		}

		protected virtual void OnIsActiveChanged(bool oldValue, bool newValue)
		{
			if (newValue)
			{
				this.LastActivationTimeStamp = new DateTime?(DateTime.Now);
			}
			if (this.IsActiveChanged != null)
			{
				this.IsActiveChanged(this, EventArgs.Empty);
			}
		}

		protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
		{
			if (this.IsSelectedChanged != null)
			{
				this.IsSelectedChanged(this, EventArgs.Empty);
			}
		}

		protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
		{
			if (this.IsSelected && base.Parent != null && base.Parent is ILayoutContentSelector)
			{
				ILayoutContentSelector parent = base.Parent as ILayoutContentSelector;
				parent.SelectedContentIndex = parent.IndexOf(this);
			}
			base.OnParentChanged(oldValue, newValue);
		}

		protected override void OnParentChanging(ILayoutContainer oldValue, ILayoutContainer newValue)
		{
			ILayoutRoot root = base.Root;
			if (oldValue != null)
			{
				this.IsSelected = false;
			}
			base.OnParentChanging(oldValue, newValue);
		}

		private static void OnTitlePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((LayoutContent)obj).RaisePropertyChanged(LayoutContent.TitleProperty.Name);
		}

		public virtual void ReadXml(XmlReader reader)
		{
			if (reader.MoveToAttribute("Title"))
			{
				this.Title = reader.Value;
			}
			if (reader.MoveToAttribute("IsSelected"))
			{
				this.IsSelected = bool.Parse(reader.Value);
			}
			if (reader.MoveToAttribute("ContentId"))
			{
				this.ContentId = reader.Value;
			}
			if (reader.MoveToAttribute("IsLastFocusedDocument"))
			{
				this.IsLastFocusedDocument = bool.Parse(reader.Value);
			}
			if (reader.MoveToAttribute("PreviousContainerId"))
			{
				this.PreviousContainerId = reader.Value;
			}
			if (reader.MoveToAttribute("PreviousContainerIndex"))
			{
				this.PreviousContainerIndex = int.Parse(reader.Value);
			}
			if (reader.MoveToAttribute("FloatingLeft"))
			{
				this.FloatingLeft = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			if (reader.MoveToAttribute("FloatingTop"))
			{
				this.FloatingTop = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			if (reader.MoveToAttribute("FloatingWidth"))
			{
				this.FloatingWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			if (reader.MoveToAttribute("FloatingHeight"))
			{
				this.FloatingHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			if (reader.MoveToAttribute("IsMaximized"))
			{
				this.IsMaximized = bool.Parse(reader.Value);
			}
			if (reader.MoveToAttribute("CanClose"))
			{
				this.CanClose = bool.Parse(reader.Value);
			}
			if (reader.MoveToAttribute("CanFloat"))
			{
				this.CanFloat = bool.Parse(reader.Value);
			}
			if (reader.MoveToAttribute("LastActivationTimeStamp"))
			{
				this.LastActivationTimeStamp = new DateTime?(DateTime.Parse(reader.Value, CultureInfo.InvariantCulture));
			}
			reader.Read();
		}

		internal bool TestCanClose()
		{
			CancelEventArgs cancelEventArg = new CancelEventArgs();
			this.OnClosing(cancelEventArg);
			if (cancelEventArg.Cancel)
			{
				return false;
			}
			return true;
		}

		public virtual void WriteXml(XmlWriter writer)
		{
			bool isSelected;
			double floatingLeft;
			if (!string.IsNullOrWhiteSpace(this.Title))
			{
				writer.WriteAttributeString("Title", this.Title);
			}
			if (this.IsSelected)
			{
				isSelected = this.IsSelected;
				writer.WriteAttributeString("IsSelected", isSelected.ToString());
			}
			if (this.IsLastFocusedDocument)
			{
				isSelected = this.IsLastFocusedDocument;
				writer.WriteAttributeString("IsLastFocusedDocument", isSelected.ToString());
			}
			if (!string.IsNullOrWhiteSpace(this.ContentId))
			{
				writer.WriteAttributeString("ContentId", this.ContentId);
			}
			if (this.ToolTip != null && this.ToolTip is string && !string.IsNullOrWhiteSpace((string)this.ToolTip))
			{
				writer.WriteAttributeString("ToolTip", (string)this.ToolTip);
			}
			if (this.FloatingLeft != 0)
			{
				floatingLeft = this.FloatingLeft;
				writer.WriteAttributeString("FloatingLeft", floatingLeft.ToString(CultureInfo.InvariantCulture));
			}
			if (this.FloatingTop != 0)
			{
				floatingLeft = this.FloatingTop;
				writer.WriteAttributeString("FloatingTop", floatingLeft.ToString(CultureInfo.InvariantCulture));
			}
			if (this.FloatingWidth != 0)
			{
				floatingLeft = this.FloatingWidth;
				writer.WriteAttributeString("FloatingWidth", floatingLeft.ToString(CultureInfo.InvariantCulture));
			}
			if (this.FloatingHeight != 0)
			{
				floatingLeft = this.FloatingHeight;
				writer.WriteAttributeString("FloatingHeight", floatingLeft.ToString(CultureInfo.InvariantCulture));
			}
			if (this.IsMaximized)
			{
				isSelected = this.IsMaximized;
				writer.WriteAttributeString("IsMaximized", isSelected.ToString());
			}
			if (!this.CanClose)
			{
				isSelected = this.CanClose;
				writer.WriteAttributeString("CanClose", isSelected.ToString());
			}
			if (!this.CanFloat)
			{
				isSelected = this.CanFloat;
				writer.WriteAttributeString("CanFloat", isSelected.ToString());
			}
			if (this.LastActivationTimeStamp.HasValue)
			{
				DateTime value = this.LastActivationTimeStamp.Value;
				writer.WriteAttributeString("LastActivationTimeStamp", value.ToString(CultureInfo.InvariantCulture));
			}
			if (this._previousContainer != null)
			{
				ILayoutPaneSerializable layoutPaneSerializable = this._previousContainer as ILayoutPaneSerializable;
				if (layoutPaneSerializable != null)
				{
					writer.WriteAttributeString("PreviousContainerId", layoutPaneSerializable.Id);
					writer.WriteAttributeString("PreviousContainerIndex", this._previousContainerIndex.ToString());
				}
			}
		}

		public event EventHandler Closed;

		public event EventHandler<CancelEventArgs> Closing;

		public event EventHandler IsActiveChanged;

		public event EventHandler IsSelectedChanged;
	}
}