using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[ContentProperty("Children")]
	[Serializable]
	public class LayoutAnchorGroup : LayoutGroup<LayoutAnchorable>, ILayoutPreviousContainer, ILayoutPaneSerializable
	{
		[NonSerialized]
		private ILayoutContainer _previousContainer;

		private string _id;

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

		string Xceed.Wpf.AvalonDock.Layout.ILayoutPreviousContainer.PreviousContainerId
		{
			get;
			set;
		}

		public LayoutAnchorGroup()
		{
		}

		protected override bool GetVisibility()
		{
			return base.Children.Count > 0;
		}

		public override void ReadXml(XmlReader reader)
		{
			if (reader.MoveToAttribute("Id"))
			{
				this._id = reader.Value;
			}
			if (reader.MoveToAttribute("PreviousContainerId"))
			{
				((ILayoutPreviousContainer)this).PreviousContainerId = reader.Value;
			}
			base.ReadXml(reader);
		}

		public override void WriteXml(XmlWriter writer)
		{
			if (this._id != null)
			{
				writer.WriteAttributeString("Id", this._id);
			}
			if (this._previousContainer != null)
			{
				ILayoutPaneSerializable layoutPaneSerializable = this._previousContainer as ILayoutPaneSerializable;
				if (layoutPaneSerializable != null)
				{
					writer.WriteAttributeString("PreviousContainerId", layoutPaneSerializable.Id);
				}
			}
			base.WriteXml(writer);
		}
	}
}