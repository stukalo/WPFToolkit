using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[Serializable]
	public abstract class LayoutGroup<T> : LayoutGroupBase, ILayoutContainer, ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging, ILayoutGroup, IXmlSerializable
	where T : class, ILayoutElement
	{
		private ObservableCollection<T> _children;

		private bool _isVisible;

		public ObservableCollection<T> Children
		{
			get
			{
				return this._children;
			}
		}

		public int ChildrenCount
		{
			get
			{
				return this._children.Count;
			}
		}

		public bool IsVisible
		{
			get
			{
				return JustDecompileGenerated_get_IsVisible();
			}
			set
			{
				JustDecompileGenerated_set_IsVisible(value);
			}
		}

		public bool JustDecompileGenerated_get_IsVisible()
		{
			return this._isVisible;
		}

		protected void JustDecompileGenerated_set_IsVisible(bool value)
		{
			if (this._isVisible != value)
			{
				this.RaisePropertyChanging("IsVisible");
				this._isVisible = value;
				this.OnIsVisibleChanged();
				this.RaisePropertyChanged("IsVisible");
			}
		}

		IEnumerable<ILayoutElement> Xceed.Wpf.AvalonDock.Layout.ILayoutContainer.Children
		{
			get
			{
				return this._children.Cast<ILayoutElement>();
			}
		}

		internal LayoutGroup()
		{
			this._children.CollectionChanged += new NotifyCollectionChangedEventHandler(this._children_CollectionChanged);
		}

		private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if ((e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace) && e.OldItems != null)
			{
				foreach (LayoutElement oldItem in e.OldItems)
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
				foreach (LayoutElement newItem in e.NewItems)
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
			this.ComputeVisibility();
			this.OnChildrenCollectionChanged();
			base.NotifyChildrenTreeChanged(ChildrenTreeChange.DirectChildrenChanged);
			this.RaisePropertyChanged("ChildrenCount");
		}

		protected virtual void ChildMoved(int oldIndex, int newIndex)
		{
		}

		public void ComputeVisibility()
		{
			this.IsVisible = this.GetVisibility();
		}

		private Type FindType(string name)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < (int)assemblies.Length; i++)
			{
				Type[] types = assemblies[i].GetTypes();
				for (int j = 0; j < (int)types.Length; j++)
				{
					Type type = types[j];
					if (type.Name.Equals(name))
					{
						return type;
					}
				}
			}
			return null;
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		protected abstract bool GetVisibility();

		public int IndexOfChild(ILayoutElement element)
		{
			return this._children.Cast<ILayoutElement>().ToList<ILayoutElement>().IndexOf(element);
		}

		public void InsertChildAt(int index, ILayoutElement element)
		{
			this._children.Insert(index, (T)element);
		}

		public void MoveChild(int oldIndex, int newIndex)
		{
			if (oldIndex == newIndex)
			{
				return;
			}
			this._children.Move(oldIndex, newIndex);
			this.ChildMoved(oldIndex, newIndex);
		}

		protected virtual void OnIsVisibleChanged()
		{
			this.UpdateParentVisibility();
		}

		protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
		{
			base.OnParentChanged(oldValue, newValue);
			this.ComputeVisibility();
		}

		public virtual void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			if (reader.IsEmptyElement)
			{
				reader.Read();
				this.ComputeVisibility();
				return;
			}
			string localName = reader.LocalName;
			reader.Read();
			while (!(reader.LocalName == localName) || reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType != XmlNodeType.Whitespace)
				{
					XmlSerializer xmlSerializer = null;
					if (reader.LocalName == "LayoutAnchorablePaneGroup")
					{
						xmlSerializer = new XmlSerializer(typeof(LayoutAnchorablePaneGroup));
					}
					else if (reader.LocalName == "LayoutAnchorablePane")
					{
						xmlSerializer = new XmlSerializer(typeof(LayoutAnchorablePane));
					}
					else if (reader.LocalName == "LayoutAnchorable")
					{
						xmlSerializer = new XmlSerializer(typeof(LayoutAnchorable));
					}
					else if (reader.LocalName == "LayoutDocumentPaneGroup")
					{
						xmlSerializer = new XmlSerializer(typeof(LayoutDocumentPaneGroup));
					}
					else if (reader.LocalName == "LayoutDocumentPane")
					{
						xmlSerializer = new XmlSerializer(typeof(LayoutDocumentPane));
					}
					else if (reader.LocalName == "LayoutDocument")
					{
						xmlSerializer = new XmlSerializer(typeof(LayoutDocument));
					}
					else if (reader.LocalName == "LayoutAnchorGroup")
					{
						xmlSerializer = new XmlSerializer(typeof(LayoutAnchorGroup));
					}
					else if (reader.LocalName != "LayoutPanel")
					{
						Type type = this.FindType(reader.LocalName);
						if (type == null)
						{
							throw new ArgumentException(string.Concat("AvalonDock.LayoutGroup doesn't know how to deserialize ", reader.LocalName));
						}
						xmlSerializer = new XmlSerializer(type);
					}
					else
					{
						xmlSerializer = new XmlSerializer(typeof(LayoutPanel));
					}
					this.Children.Add((T)xmlSerializer.Deserialize(reader));
				}
				else
				{
					reader.Read();
				}
			}
			reader.ReadEndElement();
		}

		public void RemoveChild(ILayoutElement element)
		{
			this._children.Remove((T)element);
		}

		public void RemoveChildAt(int childIndex)
		{
			this._children.RemoveAt(childIndex);
		}

		public void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
		{
			int num = this._children.IndexOf((T)oldElement);
			this._children.Insert(num, (T)newElement);
			this._children.RemoveAt(num + 1);
		}

		public void ReplaceChildAt(int index, ILayoutElement element)
		{
			this._children[index] = (T)element;
		}

		private void UpdateParentVisibility()
		{
			ILayoutElementWithVisibility parent = base.Parent as ILayoutElementWithVisibility;
			if (parent != null)
			{
				parent.ComputeVisibility();
			}
		}

		public virtual void WriteXml(XmlWriter writer)
		{
			foreach (T child in this.Children)
			{
				(new XmlSerializer(child.GetType())).Serialize(writer, child);
			}
		}
	}
}