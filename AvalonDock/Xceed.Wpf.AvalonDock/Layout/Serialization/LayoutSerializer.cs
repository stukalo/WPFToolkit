using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Layout.Serialization
{
	public abstract class LayoutSerializer
	{
		private DockingManager _manager;

		private LayoutAnchorable[] _previousAnchorables;

		private LayoutDocument[] _previousDocuments;

		public DockingManager Manager
		{
			get
			{
				return this._manager;
			}
		}

		public LayoutSerializer(DockingManager manager)
		{
			if (manager == null)
			{
				throw new ArgumentNullException("manager");
			}
			this._manager = manager;
			this._previousAnchorables = this._manager.Layout.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>();
			this._previousDocuments = this._manager.Layout.Descendents().OfType<LayoutDocument>().ToArray<LayoutDocument>();
		}

		protected void EndDeserialization()
		{
			this.Manager.SuspendDocumentsSourceBinding = false;
			this.Manager.SuspendAnchorablesSourceBinding = false;
		}

		protected virtual void FixupLayout(LayoutRoot layout)
		{
			int i;
			foreach (ILayoutPreviousContainer layoutPreviousContainer in 
				from lc in layout.Descendents().OfType<ILayoutPreviousContainer>()
				where lc.PreviousContainerId != null
				select lc)
			{
				ILayoutPaneSerializable layoutPaneSerializable = layout.Descendents().OfType<ILayoutPaneSerializable>().FirstOrDefault<ILayoutPaneSerializable>((ILayoutPaneSerializable lps) => lps.Id == layoutPreviousContainer.PreviousContainerId);
				if (layoutPaneSerializable == null)
				{
					throw new ArgumentException(string.Format("Unable to find a pane with id ='{0}'", layoutPreviousContainer.PreviousContainerId));
				}
				layoutPreviousContainer.PreviousContainer = layoutPaneSerializable as ILayoutContainer;
			}
			LayoutAnchorable[] array = (
				from lc in layout.Descendents().OfType<LayoutAnchorable>()
				where lc.Content == null
				select lc).ToArray<LayoutAnchorable>();
			for (i = 0; i < (int)array.Length; i++)
			{
				LayoutAnchorable content = array[i];
				LayoutAnchorable layoutAnchorable = null;
				if (content.ContentId != null)
				{
					layoutAnchorable = this._previousAnchorables.FirstOrDefault<LayoutAnchorable>((LayoutAnchorable a) => a.ContentId == content.ContentId);
				}
				if (this.LayoutSerializationCallback != null)
				{
					LayoutSerializationCallbackEventArgs layoutSerializationCallbackEventArg = new LayoutSerializationCallbackEventArgs(content, (layoutAnchorable != null ? layoutAnchorable.Content : null));
					this.LayoutSerializationCallback(this, layoutSerializationCallbackEventArg);
					if (layoutSerializationCallbackEventArg.Cancel)
					{
						content.Close();
					}
					else if (layoutSerializationCallbackEventArg.Content != null)
					{
						content.Content = layoutSerializationCallbackEventArg.Content;
					}
					else if (layoutSerializationCallbackEventArg.Model.Content != null)
					{
						content.Hide(false);
					}
				}
				else if (layoutAnchorable != null)
				{
					content.Content = layoutAnchorable.Content;
					content.IconSource = layoutAnchorable.IconSource;
				}
				else
				{
					content.Hide(false);
				}
			}
			LayoutDocument[] layoutDocumentArray = (
				from lc in layout.Descendents().OfType<LayoutDocument>()
				where lc.Content == null
				select lc).ToArray<LayoutDocument>();
			for (i = 0; i < (int)layoutDocumentArray.Length; i++)
			{
				LayoutDocument layoutDocument = layoutDocumentArray[i];
				LayoutDocument layoutDocument1 = null;
				if (layoutDocument.ContentId != null)
				{
					layoutDocument1 = this._previousDocuments.FirstOrDefault<LayoutDocument>((LayoutDocument a) => a.ContentId == layoutDocument.ContentId);
				}
				if (this.LayoutSerializationCallback != null)
				{
					LayoutSerializationCallbackEventArgs layoutSerializationCallbackEventArg1 = new LayoutSerializationCallbackEventArgs(layoutDocument, (layoutDocument1 != null ? layoutDocument1.Content : null));
					this.LayoutSerializationCallback(this, layoutSerializationCallbackEventArg1);
					if (layoutSerializationCallbackEventArg1.Cancel)
					{
						layoutDocument.Close();
					}
					else if (layoutSerializationCallbackEventArg1.Content != null)
					{
						layoutDocument.Content = layoutSerializationCallbackEventArg1.Content;
					}
					else if (layoutSerializationCallbackEventArg1.Model.Content != null)
					{
						layoutDocument.Close();
					}
				}
				else if (layoutDocument1 != null)
				{
					layoutDocument.Content = layoutDocument1.Content;
				}
				else
				{
					layoutDocument.Close();
				}
			}
			layout.CollectGarbage();
		}

		protected void StartDeserialization()
		{
			this.Manager.SuspendDocumentsSourceBinding = true;
			this.Manager.SuspendAnchorablesSourceBinding = true;
		}

		public event EventHandler<LayoutSerializationCallbackEventArgs> LayoutSerializationCallback;
	}
}