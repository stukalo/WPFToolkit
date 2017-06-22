using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Layout.Serialization
{
	public class LayoutSerializationCallbackEventArgs : CancelEventArgs
	{
		public object Content
		{
			get;
			set;
		}

		public LayoutContent Model
		{
			get;
			private set;
		}

		public LayoutSerializationCallbackEventArgs(LayoutContent model, object previousContent)
		{
			base.Cancel = false;
			this.Model = model;
			this.Content = previousContent;
		}
	}
}