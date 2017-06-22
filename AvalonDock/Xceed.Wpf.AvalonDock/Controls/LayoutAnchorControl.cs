using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutAnchorControl : Control, ILayoutControl
	{
		private LayoutAnchorable _model;

		private DispatcherTimer _openUpTimer;

		private readonly static DependencyPropertyKey SidePropertyKey;

		public readonly static DependencyProperty SideProperty;

		public ILayoutElement Model
		{
			get
			{
				return this._model;
			}
		}

		public AnchorSide Side
		{
			get
			{
				return (AnchorSide)base.GetValue(LayoutAnchorControl.SideProperty);
			}
		}

		static LayoutAnchorControl()
		{
			LayoutAnchorControl.SidePropertyKey = DependencyProperty.RegisterReadOnly("Side", typeof(AnchorSide), typeof(LayoutAnchorControl), new FrameworkPropertyMetadata((object)AnchorSide.Left));
			LayoutAnchorControl.SideProperty = LayoutAnchorControl.SidePropertyKey.DependencyProperty;
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorControl), new FrameworkPropertyMetadata(typeof(LayoutAnchorControl)));
			UIElement.IsHitTestVisibleProperty.AddOwner(typeof(LayoutAnchorControl), new FrameworkPropertyMetadata(true));
		}

		internal LayoutAnchorControl(LayoutAnchorable model)
		{
			this._model = model;
			this._model.IsActiveChanged += new EventHandler(this._model_IsActiveChanged);
			this._model.IsSelectedChanged += new EventHandler(this._model_IsSelectedChanged);
			this.SetSide(this._model.FindParent<LayoutAnchorSide>().Side);
		}

		private void _model_IsActiveChanged(object sender, EventArgs e)
		{
			if (!this._model.IsAutoHidden)
			{
				this._model.IsActiveChanged -= new EventHandler(this._model_IsActiveChanged);
				return;
			}
			if (this._model.IsActive)
			{
				this._model.Root.Manager.ShowAutoHideWindow(this);
			}
		}

		private void _model_IsSelectedChanged(object sender, EventArgs e)
		{
			if (!this._model.IsAutoHidden)
			{
				this._model.IsSelectedChanged -= new EventHandler(this._model_IsSelectedChanged);
				return;
			}
			if (this._model.IsSelected)
			{
				this._model.Root.Manager.ShowAutoHideWindow(this);
				this._model.IsSelected = false;
			}
		}

		private void _openUpTimer_Tick(object sender, EventArgs e)
		{
			this._openUpTimer.Tick -= new EventHandler(this._openUpTimer_Tick);
			this._openUpTimer.Stop();
			this._openUpTimer = null;
			this._model.Root.Manager.ShowAutoHideWindow(this);
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (!e.Handled)
			{
				this._model.Root.Manager.ShowAutoHideWindow(this);
				this._model.IsActive = true;
			}
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			if (!e.Handled)
			{
				this._openUpTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
				{
					Interval = TimeSpan.FromMilliseconds(400)
				};
				this._openUpTimer.Tick += new EventHandler(this._openUpTimer_Tick);
				this._openUpTimer.Start();
			}
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			if (this._openUpTimer != null)
			{
				this._openUpTimer.Tick -= new EventHandler(this._openUpTimer_Tick);
				this._openUpTimer.Stop();
				this._openUpTimer = null;
			}
			base.OnMouseLeave(e);
		}

		protected void SetSide(AnchorSide value)
		{
			base.SetValue(LayoutAnchorControl.SidePropertyKey, value);
		}
	}
}