using System;
using System.Windows;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutDocumentItem : LayoutItem
	{
		private LayoutDocument _document;

		public readonly static DependencyProperty DescriptionProperty;

		public string Description
		{
			get
			{
				return (string)base.GetValue(LayoutDocumentItem.DescriptionProperty);
			}
			set
			{
				base.SetValue(LayoutDocumentItem.DescriptionProperty, value);
			}
		}

		static LayoutDocumentItem()
		{
			LayoutDocumentItem.DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(LayoutDocumentItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutDocumentItem.OnDescriptionChanged)));
		}

		internal LayoutDocumentItem()
		{
		}

		internal override void Attach(LayoutContent model)
		{
			this._document = model as LayoutDocument;
			base.Attach(model);
		}

		protected override void Close()
		{
			if (this._document.Root != null && this._document.Root.Manager != null)
			{
				this._document.Root.Manager._ExecuteCloseCommand(this._document);
			}
		}

		internal override void Detach()
		{
			this._document = null;
			base.Detach();
		}

		private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutDocumentItem)d).OnDescriptionChanged(e);
		}

		protected virtual void OnDescriptionChanged(DependencyPropertyChangedEventArgs e)
		{
			this._document.Description = (string)e.NewValue;
		}
	}
}