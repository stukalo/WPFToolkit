using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Xml;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[ContentProperty("Children")]
	[Serializable]
	public class LayoutDocumentPane : LayoutPositionableGroup<LayoutContent>, ILayoutDocumentPane, ILayoutPanelElement, ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging, ILayoutPane, ILayoutContainer, ILayoutElementWithVisibility, ILayoutPositionableElement, ILayoutElementForFloatingWindow, ILayoutContentSelector, ILayoutPaneSerializable
	{
		private bool _showHeader = true;

		private int _selectedIndex = -1;

		private string _id;

		public IEnumerable<LayoutContent> ChildrenSorted
		{
			get
			{
				List<LayoutContent> list = base.Children.ToList<LayoutContent>();
				list.Sort();
				return list;
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

		public bool ShowHeader
		{
			get
			{
				return this._showHeader;
			}
			set
			{
				if (value != this._showHeader)
				{
					this._showHeader = value;
					this.RaisePropertyChanged("ShowHeader");
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

		public LayoutDocumentPane()
		{
		}

		public LayoutDocumentPane(LayoutContent firstChild)
		{
			base.Children.Add(firstChild);
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
			Trace.WriteLine("DocumentPane()");
			foreach (LayoutContent child in base.Children)
			{
				child.ConsoleDump(tab + 1);
			}
		}

		protected override bool GetVisibility()
		{
			if (!(base.Parent is LayoutDocumentPaneGroup))
			{
				return true;
			}
			return base.ChildrenCount > 0;
		}

		public int IndexOf(LayoutContent content)
		{
			return base.Children.IndexOf(content);
		}

		protected override void OnChildrenCollectionChanged()
		{
			if (this.SelectedContentIndex >= base.ChildrenCount)
			{
				this.SelectedContentIndex = base.Children.Count - 1;
			}
			if (this.SelectedContentIndex == -1 && base.ChildrenCount > 0)
			{
				if (base.Root != null)
				{
					LayoutContent layoutContent = (
						from c in base.Children
						orderby c.LastActivationTimeStamp.GetValueOrDefault() descending
						select c).First<LayoutContent>();
					this.SelectedContentIndex = base.Children.IndexOf(layoutContent);
					layoutContent.IsActive = true;
				}
				else
				{
					this.SetNextSelectedIndex();
				}
			}
			base.OnChildrenCollectionChanged();
			this.RaisePropertyChanged("ChildrenSorted");
		}

		protected override void OnIsVisibleChanged()
		{
			this.UpdateParentVisibility();
			base.OnIsVisibleChanged();
		}

		public override void ReadXml(XmlReader reader)
		{
			if (reader.MoveToAttribute("Id"))
			{
				this._id = reader.Value;
			}
			if (reader.MoveToAttribute("ShowHeader"))
			{
				this._showHeader = bool.Parse(reader.Value);
			}
			base.ReadXml(reader);
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

		private void UpdateParentVisibility()
		{
			ILayoutElementWithVisibility parent = base.Parent as ILayoutElementWithVisibility;
			if (parent != null)
			{
				parent.ComputeVisibility();
			}
		}

		public override void WriteXml(XmlWriter writer)
		{
			if (this._id != null)
			{
				writer.WriteAttributeString("Id", this._id);
			}
			if (!this._showHeader)
			{
				writer.WriteAttributeString("ShowHeader", this._showHeader.ToString());
			}
			base.WriteXml(writer);
		}
	}
}