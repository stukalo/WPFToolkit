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
	public class LayoutAnchorGroupControl : Control, ILayoutControl
	{
		private ObservableCollection<LayoutAnchorControl> _childViews = new ObservableCollection<LayoutAnchorControl>();

		private LayoutAnchorGroup _model;

		public ObservableCollection<LayoutAnchorControl> Children
		{
			get
			{
				return this._childViews;
			}
		}

		public ILayoutElement Model
		{
			get
			{
				return this._model;
			}
		}

		static LayoutAnchorGroupControl()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorGroupControl), new FrameworkPropertyMetadata(typeof(LayoutAnchorGroupControl)));
		}

		internal LayoutAnchorGroupControl(LayoutAnchorGroup model)
		{
			this._model = model;
			this.CreateChildrenViews();
			this._model.Children.CollectionChanged += new NotifyCollectionChangedEventHandler((object s, NotifyCollectionChangedEventArgs e) => this.OnModelChildrenCollectionChanged(e));
		}

		private void CreateChildrenViews()
		{
			DockingManager manager = this._model.Root.Manager;
			foreach (LayoutAnchorable child in this._model.Children)
			{
				this._childViews.Add(new LayoutAnchorControl(child)
				{
					Template = manager.AnchorTemplate
				});
			}
		}

		private void OnModelChildrenCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if ((e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace) && e.OldItems != null)
			{
				foreach (object oldItem in e.OldItems)
				{
					this._childViews.Remove(this._childViews.First<LayoutAnchorControl>((LayoutAnchorControl cv) => cv.Model == oldItem));
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				this._childViews.Clear();
			}
			if ((e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace) && e.NewItems != null)
			{
				DockingManager manager = this._model.Root.Manager;
				int newStartingIndex = e.NewStartingIndex;
				foreach (LayoutAnchorable newItem in e.NewItems)
				{
					ObservableCollection<LayoutAnchorControl> observableCollection = this._childViews;
					int num = newStartingIndex;
					newStartingIndex = num + 1;
					observableCollection.Insert(num, new LayoutAnchorControl(newItem)
					{
						Template = manager.AnchorTemplate
					});
				}
			}
		}
	}
}