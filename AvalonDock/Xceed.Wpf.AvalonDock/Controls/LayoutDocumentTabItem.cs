using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutDocumentTabItem : Control
	{
		public readonly static DependencyProperty ModelProperty;

		private readonly static DependencyPropertyKey LayoutItemPropertyKey;

		public readonly static DependencyProperty LayoutItemProperty;

		private List<Rect> _otherTabsScreenArea;

		private List<TabItem> _otherTabs;

		private Rect _parentDocumentTabPanelScreenArea;

		private DocumentPaneTabPanel _parentDocumentTabPanel;

		private bool _isMouseDown;

		private Point _mouseDownPoint;

		public Xceed.Wpf.AvalonDock.Controls.LayoutItem LayoutItem
		{
			get
			{
				return (Xceed.Wpf.AvalonDock.Controls.LayoutItem)base.GetValue(LayoutDocumentTabItem.LayoutItemProperty);
			}
		}

		public LayoutContent Model
		{
			get
			{
				return (LayoutContent)base.GetValue(LayoutDocumentTabItem.ModelProperty);
			}
			set
			{
				base.SetValue(LayoutDocumentTabItem.ModelProperty, value);
			}
		}

		static LayoutDocumentTabItem()
		{
			LayoutDocumentTabItem.ModelProperty = DependencyProperty.Register("Model", typeof(LayoutContent), typeof(LayoutDocumentTabItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(LayoutDocumentTabItem.OnModelChanged)));
			LayoutDocumentTabItem.LayoutItemPropertyKey = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(Xceed.Wpf.AvalonDock.Controls.LayoutItem), typeof(LayoutDocumentTabItem), new FrameworkPropertyMetadata(null));
			LayoutDocumentTabItem.LayoutItemProperty = LayoutDocumentTabItem.LayoutItemPropertyKey.DependencyProperty;
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDocumentTabItem), new FrameworkPropertyMetadata(typeof(LayoutDocumentTabItem)));
		}

		public LayoutDocumentTabItem()
		{
		}

		private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutDocumentTabItem)d).OnModelChanged(e);
		}

		protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.Model == null)
			{
				this.SetLayoutItem(null);
				return;
			}
			this.SetLayoutItem(this.Model.Root.Manager.GetLayoutItemFromModel(this.Model));
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Middle && this.LayoutItem.CloseCommand.CanExecute(null))
			{
				this.LayoutItem.CloseCommand.Execute(null);
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			this._isMouseDown = false;
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			this._isMouseDown = false;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			this.Model.IsActive = true;
			if (e.ClickCount == 1)
			{
				this._mouseDownPoint = e.GetPosition(this);
				this._isMouseDown = true;
			}
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (base.IsMouseCaptured)
			{
				base.ReleaseMouseCapture();
			}
			this._isMouseDown = false;
			base.OnMouseLeftButtonUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (this._isMouseDown)
			{
				Point position = e.GetPosition(this);
				if (Math.Abs(position.X - this._mouseDownPoint.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - this._mouseDownPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					this.UpdateDragDetails();
					base.CaptureMouse();
					this._isMouseDown = false;
				}
			}
			if (base.IsMouseCaptured)
			{
				Point screenDPI = this.PointToScreenDPI(e.GetPosition(this));
				if (!this._parentDocumentTabPanelScreenArea.Contains(screenDPI))
				{
					base.ReleaseMouseCapture();
					this.Model.Root.Manager.StartDraggingFloatingWindowForContent(this.Model, true);
					return;
				}
				int num = this._otherTabsScreenArea.FindIndex((Rect r) => r.Contains(screenDPI));
				if (num >= 0)
				{
					LayoutContent content = this._otherTabs[num].Content as LayoutContent;
					ILayoutContainer parent = this.Model.Parent;
					ILayoutPane layoutPane = this.Model.Parent as ILayoutPane;
					if (layoutPane is LayoutDocumentPane && !((LayoutDocumentPane)layoutPane).CanRepositionItems)
					{
						return;
					}
					if (layoutPane.Parent != null && layoutPane.Parent is LayoutDocumentPaneGroup && !((LayoutDocumentPaneGroup)layoutPane.Parent).CanRepositionItems)
					{
						return;
					}
					List<ILayoutElement> list = parent.Children.ToList<ILayoutElement>();
					layoutPane.MoveChild(list.IndexOf(this.Model), list.IndexOf(content));
					this.Model.IsActive = true;
					this._parentDocumentTabPanel.UpdateLayout();
					this.UpdateDragDetails();
				}
			}
		}

		protected void SetLayoutItem(Xceed.Wpf.AvalonDock.Controls.LayoutItem value)
		{
			base.SetValue(LayoutDocumentTabItem.LayoutItemPropertyKey, value);
		}

		private void UpdateDragDetails()
		{
			this._parentDocumentTabPanel = this.FindLogicalAncestor<DocumentPaneTabPanel>();
			this._parentDocumentTabPanelScreenArea = this._parentDocumentTabPanel.GetScreenArea();
			this._otherTabs = (
				from TabItem ch in this._parentDocumentTabPanel.Children
				where ch.Visibility != System.Windows.Visibility.Collapsed
				select ch).ToList<TabItem>();
			Rect rect = this.FindLogicalAncestor<TabItem>().GetScreenArea();
			this._otherTabsScreenArea = this._otherTabs.Select<TabItem, Rect>((TabItem ti) => {
				Rect screenArea = ti.GetScreenArea();
				return new Rect(screenArea.Left, screenArea.Top, rect.Width, screenArea.Height);
			}).ToList<Rect>();
		}
	}
}