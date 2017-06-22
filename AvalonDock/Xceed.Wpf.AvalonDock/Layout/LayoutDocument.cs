using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Xceed.Wpf.AvalonDock;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[Serializable]
	public class LayoutDocument : LayoutContent
	{
		private string _description;

		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (this._description != value)
				{
					this._description = value;
					this.RaisePropertyChanged("Description");
				}
			}
		}

		public bool IsVisible
		{
			get
			{
				return true;
			}
		}

		public LayoutDocument()
		{
		}

		public override void Close()
		{
			if (base.Root == null || base.Root.Manager == null)
			{
				this.CloseDocument();
				return;
			}
			base.Root.Manager._ExecuteCloseCommand(this);
		}

		internal bool CloseDocument()
		{
			if (!base.TestCanClose())
			{
				return false;
			}
			base.CloseInternal();
			return true;
		}

		public override void ConsoleDump(int tab)
		{
			Trace.Write(new string(' ', tab * 4));
			Trace.WriteLine("Document()");
		}

		protected override void InternalDock()
		{
			LayoutRoot root = base.Root as LayoutRoot;
			LayoutDocumentPane parent = null;
			if (root.LastFocusedDocument != null && root.LastFocusedDocument != this)
			{
				parent = root.LastFocusedDocument.Parent as LayoutDocumentPane;
			}
			if (parent == null)
			{
				parent = root.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>();
			}
			bool flag = false;
			if (root.Manager.LayoutUpdateStrategy != null)
			{
				flag = root.Manager.LayoutUpdateStrategy.BeforeInsertDocument(root, this, parent);
			}
			if (!flag)
			{
				if (parent == null)
				{
					throw new InvalidOperationException("Layout must contains at least one LayoutDocumentPane in order to host documents");
				}
				parent.Children.Add(this);
				flag = true;
			}
			if (root.Manager.LayoutUpdateStrategy != null)
			{
				root.Manager.LayoutUpdateStrategy.AfterInsertDocument(root, this);
			}
			base.InternalDock();
		}

		public override void ReadXml(XmlReader reader)
		{
			if (reader.MoveToAttribute("Description"))
			{
				this.Description = reader.Value;
			}
			base.ReadXml(reader);
		}

		public override void WriteXml(XmlWriter writer)
		{
			base.WriteXml(writer);
			if (!string.IsNullOrWhiteSpace(this.Description))
			{
				writer.WriteAttributeString("Description", this.Description);
			}
		}
	}
}