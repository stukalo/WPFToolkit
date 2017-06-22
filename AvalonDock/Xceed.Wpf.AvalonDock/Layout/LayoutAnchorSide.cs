using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[ContentProperty("Children")]
	[Serializable]
	public class LayoutAnchorSide : LayoutGroup<LayoutAnchorGroup>
	{
		private AnchorSide _side;

		public AnchorSide Side
		{
			get
			{
				return this._side;
			}
			private set
			{
				if (this._side != value)
				{
					this.RaisePropertyChanging("Side");
					this._side = value;
					this.RaisePropertyChanged("Side");
				}
			}
		}

		public LayoutAnchorSide()
		{
		}

		protected override bool GetVisibility()
		{
			return base.Children.Count > 0;
		}

		protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
		{
			base.OnParentChanged(oldValue, newValue);
			this.UpdateSide();
		}

		private void UpdateSide()
		{
			if (base.Root.LeftSide == this)
			{
				this.Side = AnchorSide.Left;
				return;
			}
			if (base.Root.TopSide == this)
			{
				this.Side = AnchorSide.Top;
				return;
			}
			if (base.Root.RightSide == this)
			{
				this.Side = AnchorSide.Right;
				return;
			}
			if (base.Root.BottomSide == this)
			{
				this.Side = AnchorSide.Bottom;
			}
		}
	}
}