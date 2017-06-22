using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Layout.Serialization
{
	public class XmlLayoutSerializer : LayoutSerializer
	{
		public XmlLayoutSerializer(DockingManager manager) : base(manager)
		{
		}

		public void Deserialize(Stream stream)
		{
			try
			{
				base.StartDeserialization();
				LayoutRoot layoutRoot = (new XmlSerializer(typeof(LayoutRoot))).Deserialize(stream) as LayoutRoot;
				this.FixupLayout(layoutRoot);
				base.Manager.Layout = layoutRoot;
			}
			finally
			{
				base.EndDeserialization();
			}
		}

		public void Deserialize(TextReader reader)
		{
			try
			{
				base.StartDeserialization();
				LayoutRoot layoutRoot = (new XmlSerializer(typeof(LayoutRoot))).Deserialize(reader) as LayoutRoot;
				this.FixupLayout(layoutRoot);
				base.Manager.Layout = layoutRoot;
			}
			finally
			{
				base.EndDeserialization();
			}
		}

		public void Deserialize(XmlReader reader)
		{
			try
			{
				base.StartDeserialization();
				LayoutRoot layoutRoot = (new XmlSerializer(typeof(LayoutRoot))).Deserialize(reader) as LayoutRoot;
				this.FixupLayout(layoutRoot);
				base.Manager.Layout = layoutRoot;
			}
			finally
			{
				base.EndDeserialization();
			}
		}

		public void Deserialize(string filepath)
		{
			using (StreamReader streamReader = new StreamReader(filepath))
			{
				this.Deserialize(streamReader);
			}
		}

		public void Serialize(XmlWriter writer)
		{
			(new XmlSerializer(typeof(LayoutRoot))).Serialize(writer, base.Manager.Layout);
		}

		public void Serialize(TextWriter writer)
		{
			(new XmlSerializer(typeof(LayoutRoot))).Serialize(writer, base.Manager.Layout);
		}

		public void Serialize(Stream stream)
		{
			(new XmlSerializer(typeof(LayoutRoot))).Serialize(stream, base.Manager.Layout);
		}

		public void Serialize(string filepath)
		{
			using (StreamWriter streamWriter = new StreamWriter(filepath))
			{
				this.Serialize(streamWriter);
			}
		}
	}
}