using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal class AutoHideWindowManager
	{
		private DockingManager _manager;

		private WeakReference _currentAutohiddenAnchor;

		private DispatcherTimer _closeTimer;

		internal AutoHideWindowManager(DockingManager manager)
		{
			this._manager = manager;
			this.SetupCloseTimer();
		}

		public void HideAutoWindow(LayoutAnchorControl anchor = null)
		{
			if (anchor == null || anchor == this._currentAutohiddenAnchor.GetValueOrDefault<LayoutAnchorControl>())
			{
				this.StopCloseTimer();
			}
		}

		private void SetupCloseTimer()
		{
			this._closeTimer = new DispatcherTimer(DispatcherPriority.Background)
			{
				Interval = TimeSpan.FromMilliseconds(1500)
			};
			this._closeTimer.Tick += new EventHandler((object s, EventArgs e) => {
				if (this._manager.AutoHideWindow.IsWin32MouseOver || ((LayoutAnchorable)this._manager.AutoHideWindow.Model).IsActive || this._manager.AutoHideWindow.IsResizing)
				{
					return;
				}
				this.StopCloseTimer();
			});
		}

		public void ShowAutoHideWindow(LayoutAnchorControl anchor)
		{
			if (this._currentAutohiddenAnchor.GetValueOrDefault<LayoutAnchorControl>() != anchor)
			{
				this.StopCloseTimer();
				this._currentAutohiddenAnchor = new WeakReference(anchor);
				this._manager.AutoHideWindow.Show(anchor);
				this.StartCloseTimer();
			}
		}

		private void StartCloseTimer()
		{
			this._closeTimer.Start();
		}

		private void StopCloseTimer()
		{
			this._closeTimer.Stop();
			this._manager.AutoHideWindow.Hide();
			this._currentAutohiddenAnchor = null;
		}
	}
}