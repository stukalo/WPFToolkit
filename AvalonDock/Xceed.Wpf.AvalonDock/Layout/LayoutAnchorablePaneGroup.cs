using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[ContentProperty("Children")]
	[Serializable]
	public class LayoutAnchorablePaneGroup : LayoutPositionableGroup<ILayoutAnchorablePane>, ILayoutAnchorablePane, ILayoutPanelElement, ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging, ILayoutPane, ILayoutContainer, ILayoutElementWithVisibility, ILayoutOrientableGroup, ILayoutGroup
	{
		private System.Windows.Controls.Orientation _orientation;

		public System.Windows.Controls.Orientation Orientation
		{
			get
			{
				return this._orientation;
			}
			set
			{
				if (this._orientation != value)
				{
					this.RaisePropertyChanging("Orientation");
					this._orientation = value;
					this.RaisePropertyChanged("Orientation");
				}
			}
		}

		public LayoutAnchorablePaneGroup()
		{
		}

		public LayoutAnchorablePaneGroup(LayoutAnchorablePane firstChild)
		{
			base.Children.Add(firstChild);
		}

		public override void ConsoleDump(int tab)
		{
			Trace.Write(new string(' ', tab * 4));
			Trace.WriteLine(string.Format("AnchorablePaneGroup({0})", this.Orientation));
			foreach (ILayoutAnchorablePane child in base.Children)
			{
				((LayoutElement)child).ConsoleDump(tab + 1);
			}
		}

		protected override bool GetVisibility()
		{
			if (base.Children.Count <= 0)
			{
				return false;
			}
			return base.Children.Any<ILayoutAnchorablePane>((ILayoutAnchorablePane c) => c.IsVisible);
		}

		protected override void OnChildrenCollectionChanged()
		{
			if (base.DockWidth.IsAbsolute && base.ChildrenCount == 1)
			{
				((ILayoutPositionableElement)base.Children[0]).DockWidth = base.DockWidth;
			}
			if (base.DockHeight.IsAbsolute && base.ChildrenCount == 1)
			{
				((ILayoutPositionableElement)base.Children[0]).DockHeight = base.DockHeight;
			}
			base.OnChildrenCollectionChanged();
		}

		protected override void OnDockHeightChanged()
		{
			if (base.DockHeight.IsAbsolute && base.ChildrenCount == 1)
			{
				((ILayoutPositionableElement)base.Children[0]).DockHeight = base.DockHeight;
			}
			base.OnDockHeightChanged();
		}

		protected override void OnDockWidthChanged()
		{
			if (base.DockWidth.IsAbsolute && base.ChildrenCount == 1)
			{
				((ILayoutPositionableElement)base.Children[0]).DockWidth = base.DockWidth;
			}
			base.OnDockWidthChanged();
		}

		protected override void OnIsVisibleChanged()
		{
			this.UpdateParentVisibility();
			base.OnIsVisibleChanged();
		}

		public override void ReadXml(XmlReader reader)
		{
			if (reader.MoveToAttribute("Orientation"))
			{
				this.Orientation = (System.Windows.Controls.Orientation)Enum.Parse(typeof(System.Windows.Controls.Orientation), reader.Value, true);
			}
			base.ReadXml(reader);
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
			writer.WriteAttributeString("Orientation", this.Orientation.ToString());
			base.WriteXml(writer);
		}
	}
}