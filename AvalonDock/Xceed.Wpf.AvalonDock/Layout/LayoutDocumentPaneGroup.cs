using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[ContentProperty("Children")]
	[Serializable]
	public class LayoutDocumentPaneGroup : LayoutPositionableGroup<ILayoutDocumentPane>, ILayoutDocumentPane, ILayoutPanelElement, ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging, ILayoutPane, ILayoutContainer, ILayoutElementWithVisibility, ILayoutOrientableGroup, ILayoutGroup
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

		public LayoutDocumentPaneGroup()
		{
		}

		public LayoutDocumentPaneGroup(LayoutDocumentPane documentPane)
		{
			base.Children.Add(documentPane);
		}

		public override void ConsoleDump(int tab)
		{
			Trace.Write(new string(' ', tab * 4));
			Trace.WriteLine(string.Format("DocumentPaneGroup({0})", this.Orientation));
			foreach (ILayoutDocumentPane child in base.Children)
			{
				((LayoutElement)child).ConsoleDump(tab + 1);
			}
		}

		protected override bool GetVisibility()
		{
			return true;
		}

		public override void ReadXml(XmlReader reader)
		{
			if (reader.MoveToAttribute("Orientation"))
			{
				this.Orientation = (System.Windows.Controls.Orientation)Enum.Parse(typeof(System.Windows.Controls.Orientation), reader.Value, true);
			}
			base.ReadXml(reader);
		}

		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("Orientation", this.Orientation.ToString());
			base.WriteXml(writer);
		}
	}
}