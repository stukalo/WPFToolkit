using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal class DragService
	{
		private DockingManager _manager;

		private LayoutFloatingWindowControl _floatingWindow;

		private List<IOverlayWindowHost> _overlayWindowHosts = new List<IOverlayWindowHost>();

		private IOverlayWindowHost _currentHost;

		private IOverlayWindow _currentWindow;

		private List<IDropArea> _currentWindowAreas = new List<IDropArea>();

		private IDropTarget _currentDropTarget;

		public DragService(LayoutFloatingWindowControl floatingWindow)
		{
			this._floatingWindow = floatingWindow;
			this._manager = floatingWindow.Model.Root.Manager;
			this.GetOverlayWindowHosts();
		}

		internal void Abort()
		{
			ILayoutElement model = this._floatingWindow.Model;
			this._currentWindowAreas.ForEach((IDropArea a) => this._currentWindow.DragLeave(a));
			if (this._currentDropTarget != null)
			{
				this._currentWindow.DragLeave(this._currentDropTarget);
			}
			if (this._currentWindow != null)
			{
				this._currentWindow.DragLeave(this._floatingWindow);
			}
			this._currentWindow = null;
			if (this._currentHost != null)
			{
				this._currentHost.HideOverlayWindow();
			}
			this._currentHost = null;
		}

		public void Drop(Point dropLocation, out bool dropHandled)
		{
			dropHandled = false;
			this.UpdateMouseLocation(dropLocation);
			ILayoutRoot root = (this._floatingWindow.Model as LayoutFloatingWindow).Root;
			if (this._currentHost != null)
			{
				this._currentHost.HideOverlayWindow();
			}
			if (this._currentDropTarget != null)
			{
				this._currentWindow.DragDrop(this._currentDropTarget);
				root.CollectGarbage();
				dropHandled = true;
			}
			this._currentWindowAreas.ForEach((IDropArea a) => this._currentWindow.DragLeave(a));
			if (this._currentDropTarget != null)
			{
				this._currentWindow.DragLeave(this._currentDropTarget);
			}
			if (this._currentWindow != null)
			{
				this._currentWindow.DragLeave(this._floatingWindow);
			}
			this._currentWindow = null;
			this._currentHost = null;
		}

		private void GetOverlayWindowHosts()
		{
			this._overlayWindowHosts.AddRange(this._manager.GetFloatingWindowsByZOrder().OfType<LayoutAnchorableFloatingWindowControl>().Where<LayoutAnchorableFloatingWindowControl>((LayoutAnchorableFloatingWindowControl fw) => {
				if (fw == this._floatingWindow)
				{
					return false;
				}
				return fw.IsVisible;
			}));
			this._overlayWindowHosts.Add(this._manager);
		}

		public void UpdateMouseLocation(Point dragPosition)
		{
			Func<IDropTarget, bool> func2 = null;
			ILayoutElement model = this._floatingWindow.Model;
			IOverlayWindowHost overlayWindowHost = this._overlayWindowHosts.FirstOrDefault<IOverlayWindowHost>((IOverlayWindowHost oh) => oh.HitTest(dragPosition));
			if (this._currentHost != null || this._currentHost != overlayWindowHost)
			{
				if (this._currentHost != null && !this._currentHost.HitTest(dragPosition) || this._currentHost != overlayWindowHost)
				{
					if (this._currentDropTarget != null)
					{
						this._currentWindow.DragLeave(this._currentDropTarget);
					}
					this._currentDropTarget = null;
					this._currentWindowAreas.ForEach((IDropArea a) => this._currentWindow.DragLeave(a));
					this._currentWindowAreas.Clear();
					if (this._currentWindow != null)
					{
						this._currentWindow.DragLeave(this._floatingWindow);
					}
					if (this._currentHost != null)
					{
						this._currentHost.HideOverlayWindow();
					}
					this._currentHost = null;
				}
				if (this._currentHost != overlayWindowHost)
				{
					this._currentHost = overlayWindowHost;
					this._currentWindow = this._currentHost.ShowOverlayWindow(this._floatingWindow);
					this._currentWindow.DragEnter(this._floatingWindow);
				}
			}
			if (this._currentHost == null)
			{
				return;
			}
			if (this._currentDropTarget != null && !this._currentDropTarget.HitTest(dragPosition))
			{
				this._currentWindow.DragLeave(this._currentDropTarget);
				this._currentDropTarget = null;
			}
			List<IDropArea> dropAreas = new List<IDropArea>();
			this._currentWindowAreas.ForEach((IDropArea a) => {
				if (!a.DetectionRect.Contains(dragPosition))
				{
					this._currentWindow.DragLeave(a);
					dropAreas.Add(a);
				}
			});
			dropAreas.ForEach((IDropArea a) => this._currentWindowAreas.Remove(a));
			List<IDropArea> list = this._currentHost.GetDropAreas(this._floatingWindow).Where<IDropArea>((IDropArea cw) => {
				if (this._currentWindowAreas.Contains(cw))
				{
					return false;
				}
				return cw.DetectionRect.Contains(dragPosition);
			}).ToList<IDropArea>();
			this._currentWindowAreas.AddRange(list);
			list.ForEach((IDropArea a) => this._currentWindow.DragEnter(a));
			if (this._currentDropTarget == null)
			{
				this._currentWindowAreas.ForEach((IDropArea wa) => {
					if (this._currentDropTarget != null)
					{
						return;
					}
					DragService u003cu003e4_this = this;
					IEnumerable<IDropTarget> targets = this._currentWindow.GetTargets();
					Func<IDropTarget, bool> u003cu003e9_7 = func2;
					if (u003cu003e9_7 == null)
					{
						Func<IDropTarget, bool> func = (IDropTarget dt) => dt.HitTest(dragPosition);
						Func<IDropTarget, bool> func1 = func;
						func2 = func;
						u003cu003e9_7 = func1;
					}
					u003cu003e4_this._currentDropTarget = targets.FirstOrDefault<IDropTarget>(u003cu003e9_7);
					if (this._currentDropTarget == null)
					{
						return;
					}
					this._currentWindow.DragEnter(this._currentDropTarget);
				});
			}
		}
	}
}