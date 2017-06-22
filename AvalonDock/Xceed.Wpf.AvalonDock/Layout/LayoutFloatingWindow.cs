using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[Serializable]
	[XmlInclude(typeof(LayoutAnchorableFloatingWindow))]
	[XmlInclude(typeof(LayoutDocumentFloatingWindow))]
	public abstract class LayoutFloatingWindow : LayoutElement, ILayoutContainer, ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging
	{
		public abstract IEnumerable<ILayoutElement> Children
		{
			get;
		}

		public abstract int ChildrenCount
		{
			get;
		}

		public abstract bool IsValid
		{
			get;
		}

		public LayoutFloatingWindow()
		{
		}

		public abstract void RemoveChild(ILayoutElement element);

		public abstract void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement);
	}
}