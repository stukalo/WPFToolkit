using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutAnchorSideControl : Control, ILayoutControl
	{
		private LayoutAnchorSide _model;

		private ObservableCollection<LayoutAnchorGroupControl> _childViews = new ObservableCollection<LayoutAnchorGroupControl>();

		private readonly static DependencyPropertyKey IsLeftSidePropertyKey;

		public readonly static DependencyProperty IsLeftSideProperty;

		private readonly static DependencyPropertyKey IsTopSidePropertyKey;

		public readonly static DependencyProperty IsTopSideProperty;

		private readonly static DependencyPropertyKey IsRightSidePropertyKey;

		public readonly static DependencyProperty IsRightSideProperty;

		private readonly static DependencyPropertyKey IsBottomSidePropertyKey;

		public readonly static DependencyProperty IsBottomSideProperty;

		public ObservableCollection<LayoutAnchorGroupControl> Children
		{
			get
			{
				return this._childViews;
			}
		}

		public bool IsBottomSide
		{
			get
			{
				return (bool)base.GetValue(LayoutAnchorSideControl.IsBottomSideProperty);
			}
		}

		public bool IsLeftSide
		{
			get
			{
				return (bool)base.GetValue(LayoutAnchorSideControl.IsLeftSideProperty);
			}
		}

		public bool IsRightSide
		{
			get
			{
				return (bool)base.GetValue(LayoutAnchorSideControl.IsRightSideProperty);
			}
		}

		public bool IsTopSide
		{
			get
			{
				return (bool)base.GetValue(LayoutAnchorSideControl.IsTopSideProperty);
			}
		}

		public ILayoutElement Model
		{
			get
			{
				return this._model;
			}
		}

		static LayoutAnchorSideControl()
		{
			LayoutAnchorSideControl.IsLeftSidePropertyKey = DependencyProperty.RegisterReadOnly("IsLeftSide", typeof(bool), typeof(LayoutAnchorSideControl), new FrameworkPropertyMetadata(false));
			LayoutAnchorSideControl.IsLeftSideProperty = LayoutAnchorSideControl.IsLeftSidePropertyKey.DependencyProperty;
			LayoutAnchorSideControl.IsTopSidePropertyKey = DependencyProperty.RegisterReadOnly("IsTopSide", typeof(bool), typeof(LayoutAnchorSideControl), new FrameworkPropertyMetadata(false));
			LayoutAnchorSideControl.IsTopSideProperty = LayoutAnchorSideControl.IsTopSidePropertyKey.DependencyProperty;
			LayoutAnchorSideControl.IsRightSidePropertyKey = DependencyProperty.RegisterReadOnly("IsRightSide", typeof(bool), typeof(LayoutAnchorSideControl), new FrameworkPropertyMetadata(false));
			LayoutAnchorSideControl.IsRightSideProperty = LayoutAnchorSideControl.IsRightSidePropertyKey.DependencyProperty;
			LayoutAnchorSideControl.IsBottomSidePropertyKey = DependencyProperty.RegisterReadOnly("IsBottomSide", typeof(bool), typeof(LayoutAnchorSideControl), new FrameworkPropertyMetadata(false));
			LayoutAnchorSideControl.IsBottomSideProperty = LayoutAnchorSideControl.IsBottomSidePropertyKey.DependencyProperty;
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorSideControl), new FrameworkPropertyMetadata(typeof(LayoutAnchorSideControl)));
		}

		internal LayoutAnchorSideControl(LayoutAnchorSide model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			this._model = model;
			this.CreateChildrenViews();
			this._model.Children.CollectionChanged += new NotifyCollectionChangedEventHandler((object s, NotifyCollectionChangedEventArgs e) => this.OnModelChildrenCollectionChanged(e));
			this.UpdateSide();
		}

		private void CreateChildrenViews()
		{
			DockingManager manager = this._model.Root.Manager;
			foreach (LayoutAnchorGroup child in this._model.Children)
			{
				this._childViews.Add(manager.CreateUIElementForModel(child) as LayoutAnchorGroupControl);
			}
		}

		private void OnModelChildrenCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null && (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace))
			{
				foreach (object oldItem in e.OldItems)
				{
					this._childViews.Remove(this._childViews.First<LayoutAnchorGroupControl>((LayoutAnchorGroupControl cv) => cv.Model == oldItem));
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				this._childViews.Clear();
			}
			if (e.NewItems != null && (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace))
			{
				DockingManager manager = this._model.Root.Manager;
				int newStartingIndex = e.NewStartingIndex;
				foreach (LayoutAnchorGroup newItem in e.NewItems)
				{
					int num = newStartingIndex;
					newStartingIndex = num + 1;
					this._childViews.Insert(num, manager.CreateUIElementForModel(newItem) as LayoutAnchorGroupControl);
				}
			}
		}

		protected void SetIsBottomSide(bool value)
		{
			base.SetValue(LayoutAnchorSideControl.IsBottomSidePropertyKey, value);
		}

		protected void SetIsLeftSide(bool value)
		{
			base.SetValue(LayoutAnchorSideControl.IsLeftSidePropertyKey, value);
		}

		protected void SetIsRightSide(bool value)
		{
			base.SetValue(LayoutAnchorSideControl.IsRightSidePropertyKey, value);
		}

		protected void SetIsTopSide(bool value)
		{
			base.SetValue(LayoutAnchorSideControl.IsTopSidePropertyKey, value);
		}

		private void UpdateSide()
		{
			switch (this._model.Side)
			{
				case AnchorSide.Left:
				{
					this.SetIsLeftSide(true);
					return;
				}
				case AnchorSide.Top:
				{
					this.SetIsTopSide(true);
					return;
				}
				case AnchorSide.Right:
				{
					this.SetIsRightSide(true);
					return;
				}
				case AnchorSide.Bottom:
				{
					this.SetIsBottomSide(true);
					return;
				}
				default:
				{
					return;
				}
			}
		}
	}
}