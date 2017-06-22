using Microsoft.Windows.Shell;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutDocumentFloatingWindowControl : LayoutFloatingWindowControl
	{
		private LayoutDocumentFloatingWindow _model;

		public override ILayoutElement Model
		{
			get
			{
				return this._model;
			}
		}

		public LayoutItem RootDocumentLayoutItem
		{
			get
			{
				return this._model.Root.Manager.GetLayoutItemFromModel(this._model.RootDocument);
			}
		}

		static LayoutDocumentFloatingWindowControl()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDocumentFloatingWindowControl), new FrameworkPropertyMetadata(typeof(LayoutDocumentFloatingWindowControl)));
		}

		internal LayoutDocumentFloatingWindowControl(LayoutDocumentFloatingWindow model) : base(model)
		{
			this._model = model;
		}

		private void _model_RootDocumentChanged(object sender, EventArgs e)
		{
			if (this._model.RootDocument == null)
			{
				base.InternalClose();
			}
		}

		protected override IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg != 161)
			{
				if (msg == 165)
				{
					if (wParam.ToInt32() == 2)
					{
						if (this.OpenContextMenu())
						{
							handled = true;
						}
						if (!this._model.Root.Manager.ShowSystemMenu)
						{
							WindowChrome.GetWindowChrome(this).ShowSystemMenu = false;
						}
						else
						{
							WindowChrome.GetWindowChrome(this).ShowSystemMenu = !handled;
						}
					}
				}
			}
			else if (wParam.ToInt32() == 2 && this._model.RootDocument != null)
			{
				this._model.RootDocument.IsActive = true;
			}
			return base.FilterMessage(hwnd, msg, wParam, lParam, ref handled);
		}

		protected override void OnClosed(EventArgs e)
		{
			ILayoutRoot root = this.Model.Root;
			root.Manager.RemoveFloatingWindow(this);
			root.CollectGarbage();
			base.OnClosed(e);
			if (!base.CloseInitiatedByUser)
			{
				root.FloatingWindows.Remove(this._model);
			}
			this._model.RootDocumentChanged -= new EventHandler(this._model_RootDocumentChanged);
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			if (this._model.RootDocument == null)
			{
				base.InternalClose();
				return;
			}
			DockingManager manager = this._model.Root.Manager;
			base.Content = manager.CreateUIElementForModel(this._model.RootDocument);
			this._model.RootDocumentChanged += new EventHandler(this._model_RootDocumentChanged);
		}

		private bool OpenContextMenu()
		{
			System.Windows.Controls.ContextMenu documentContextMenu = this._model.Root.Manager.DocumentContextMenu;
			if (documentContextMenu == null || this.RootDocumentLayoutItem == null)
			{
				return false;
			}
			documentContextMenu.PlacementTarget = null;
			documentContextMenu.Placement = PlacementMode.MousePoint;
			documentContextMenu.DataContext = this.RootDocumentLayoutItem;
			documentContextMenu.IsOpen = true;
			return true;
		}
	}
}