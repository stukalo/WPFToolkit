using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Markup;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[ContentProperty("RootDocument")]
	[Serializable]
	public class LayoutDocumentFloatingWindow : LayoutFloatingWindow
	{
		private LayoutDocument _rootDocument;

		public override IEnumerable<ILayoutElement> Children
		{
			get
			{
				if (this.RootDocument == null)
				{
					yield break;
				}
				yield return this.RootDocument;
			}
		}

		public override int ChildrenCount
		{
			get
			{
				if (this.RootDocument == null)
				{
					return 0;
				}
				return 1;
			}
		}

		public override bool IsValid
		{
			get
			{
				return this.RootDocument != null;
			}
		}

		public LayoutDocument RootDocument
		{
			get
			{
				return this._rootDocument;
			}
			set
			{
				if (this._rootDocument != value)
				{
					this.RaisePropertyChanging("RootDocument");
					this._rootDocument = value;
					if (this._rootDocument != null)
					{
						this._rootDocument.Parent = this;
					}
					this.RaisePropertyChanged("RootDocument");
					if (this.RootDocumentChanged != null)
					{
						this.RootDocumentChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		public LayoutDocumentFloatingWindow()
		{
		}

		public override void ConsoleDump(int tab)
		{
			Trace.Write(new string(' ', tab * 4));
			Trace.WriteLine("FloatingDocumentWindow()");
			this.RootDocument.ConsoleDump(tab + 1);
		}

		public override void RemoveChild(ILayoutElement element)
		{
			this.RootDocument = null;
		}

		public override void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
		{
			this.RootDocument = newElement as LayoutDocument;
		}

		public event EventHandler RootDocumentChanged;
	}
}