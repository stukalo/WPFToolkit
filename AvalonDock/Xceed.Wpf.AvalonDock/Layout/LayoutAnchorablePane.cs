using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[ContentProperty("Children")]
	[Serializable]
	public class LayoutAnchorablePane : LayoutPositionableGroup<LayoutAnchorable>, ILayoutAnchorablePane, ILayoutPanelElement, ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging, ILayoutPane, ILayoutContainer, ILayoutElementWithVisibility, ILayoutPositionableElement, ILayoutElementForFloatingWindow, ILayoutContentSelector, ILayoutPaneSerializable
	{
		private int _selectedIndex = -1;

		[XmlIgnore]
		private bool _autoFixSelectedContent = true;

		private string _id;

		private string _name;

		public bool CanClose
		{
			get
			{
				return base.Children.All<LayoutAnchorable>((LayoutAnchorable a) => a.CanClose);
			}
		}

		public bool CanHide
		{
			get
			{
				return base.Children.All<LayoutAnchorable>((LayoutAnchorable a) => a.CanHide);
			}
		}

		public bool IsDirectlyHostedInFloatingWindow
		{
			get
			{
				LayoutAnchorableFloatingWindow layoutAnchorableFloatingWindow = this.FindParent<LayoutAnchorableFloatingWindow>();
				if (layoutAnchorableFloatingWindow == null)
				{
					return false;
				}
				return layoutAnchorableFloatingWindow.IsSinglePane;
			}
		}

		public bool IsHostedInFloatingWindow
		{
			get
			{
				return this.FindParent<LayoutFloatingWindow>() != null;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (this._name != value)
				{
					this._name = value;
					this.RaisePropertyChanged("Name");
				}
			}
		}

		public LayoutContent SelectedContent
		{
			get
			{
				if (this._selectedIndex == -1)
				{
					return null;
				}
				return base.Children[this._selectedIndex];
			}
		}

		public int SelectedContentIndex
		{
			get
			{
				return this._selectedIndex;
			}
			set
			{
				if (value < 0 || value >= base.Children.Count)
				{
					value = -1;
				}
				if (this._selectedIndex != value)
				{
					this.RaisePropertyChanging("SelectedContentIndex");
					this.RaisePropertyChanging("SelectedContent");
					if (this._selectedIndex >= 0 && this._selectedIndex < base.Children.Count)
					{
						base.Children[this._selectedIndex].IsSelected = false;
					}
					this._selectedIndex = value;
					if (this._selectedIndex >= 0 && this._selectedIndex < base.Children.Count)
					{
						base.Children[this._selectedIndex].IsSelected = true;
					}
					this.RaisePropertyChanged("SelectedContentIndex");
					this.RaisePropertyChanged("SelectedContent");
				}
			}
		}

		string Xceed.Wpf.AvalonDock.Layout.ILayoutPaneSerializable.Id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		public LayoutAnchorablePane()
		{
		}

		public LayoutAnchorablePane(LayoutAnchorable anchorable)
		{
			base.Children.Add(anchorable);
		}

		private void AutoFixSelectedContent()
		{
			if (this._autoFixSelectedContent)
			{
				if (this.SelectedContentIndex >= base.ChildrenCount)
				{
					this.SelectedContentIndex = base.Children.Count - 1;
				}
				if (this.SelectedContentIndex == -1 && base.ChildrenCount > 0)
				{
					this.SetNextSelectedIndex();
				}
			}
		}

		protected override void ChildMoved(int oldIndex, int newIndex)
		{
			if (this._selectedIndex == oldIndex)
			{
				this.RaisePropertyChanging("SelectedContentIndex");
				this._selectedIndex = newIndex;
				this.RaisePropertyChanged("SelectedContentIndex");
			}
			base.ChildMoved(oldIndex, newIndex);
		}

		public override void ConsoleDump(int tab)
		{
			Trace.Write(new string(' ', tab * 4));
			Trace.WriteLine("AnchorablePane()");
			foreach (LayoutAnchorable child in base.Children)
			{
				child.ConsoleDump(tab + 1);
			}
		}

		protected override bool GetVisibility()
		{
			if (base.Children.Count <= 0)
			{
				return false;
			}
			return base.Children.Any<LayoutAnchorable>((LayoutAnchorable c) => c.IsVisible);
		}

		public int IndexOf(LayoutContent content)
		{
			LayoutAnchorable layoutAnchorable = content as LayoutAnchorable;
			if (layoutAnchorable == null)
			{
				return -1;
			}
			return base.Children.IndexOf(layoutAnchorable);
		}

		protected override void OnChildrenCollectionChanged()
		{
			this.AutoFixSelectedContent();
			int num = 0;
			while (num < base.Children.Count)
			{
				if (!base.Children[num].IsSelected)
				{
					num++;
				}
				else
				{
					this.SelectedContentIndex = num;
					break;
				}
			}
			this.RaisePropertyChanged("CanClose");
			this.RaisePropertyChanged("CanHide");
			this.RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
			base.OnChildrenCollectionChanged();
		}

		protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
		{
			ILayoutGroup layoutGroup = oldValue as ILayoutGroup;
			if (layoutGroup != null)
			{
				layoutGroup.ChildrenCollectionChanged -= new EventHandler(this.OnParentChildrenCollectionChanged);
			}
			this.RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
			ILayoutGroup layoutGroup1 = newValue as ILayoutGroup;
			if (layoutGroup1 != null)
			{
				layoutGroup1.ChildrenCollectionChanged += new EventHandler(this.OnParentChildrenCollectionChanged);
			}
			base.OnParentChanged(oldValue, newValue);
		}

		private void OnParentChildrenCollectionChanged(object sender, EventArgs e)
		{
			this.RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
		}

		public override void ReadXml(XmlReader reader)
		{
			if (reader.MoveToAttribute("Id"))
			{
				this._id = reader.Value;
			}
			if (reader.MoveToAttribute("Name"))
			{
				this._name = reader.Value;
			}
			this._autoFixSelectedContent = false;
			base.ReadXml(reader);
			this._autoFixSelectedContent = true;
			this.AutoFixSelectedContent();
		}

		internal void SetNextSelectedIndex()
		{
			this.SelectedContentIndex = -1;
			for (int i = 0; i < base.Children.Count; i++)
			{
				if (base.Children[i].IsEnabled)
				{
					this.SelectedContentIndex = i;
					return;
				}
			}
		}

		internal void UpdateIsDirectlyHostedInFloatingWindow()
		{
			this.RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
		}

		public override void WriteXml(XmlWriter writer)
		{
			if (this._id != null)
			{
				writer.WriteAttributeString("Id", this._id);
			}
			if (this._name != null)
			{
				writer.WriteAttributeString("Name", this._name);
			}
			base.WriteXml(writer);
		}
	}
}