using System;
using System.Runtime.CompilerServices;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock
{
	public class DocumentClosedEventArgs : EventArgs
	{
		public LayoutDocument Document
		{
			get;
			private set;
		}

		public DocumentClosedEventArgs(LayoutDocument document)
		{
			this.Document = document;
		}
	}
}