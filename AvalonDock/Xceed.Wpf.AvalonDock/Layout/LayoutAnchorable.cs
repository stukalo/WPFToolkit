using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;
using Xceed.Wpf.AvalonDock;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[Serializable]
	public class LayoutAnchorable : LayoutContent
	{
		private double _autohideWidth;

		private double _autohideMinWidth = 100;

		private double _autohideHeight;

		private double _autohideMinHeight = 100;

		private bool _canHide = true;

		private bool _canAutoHide = true;

		public double AutoHideHeight
		{
			get
			{
				return this._autohideHeight;
			}
			set
			{
				if (this._autohideHeight != value)
				{
					this.RaisePropertyChanging("AutoHideHeight");
					value = Math.Max(value, this._autohideMinHeight);
					this._autohideHeight = value;
					this.RaisePropertyChanged("AutoHideHeight");
				}
			}
		}

		public double AutoHideMinHeight
		{
			get
			{
				return this._autohideMinHeight;
			}
			set
			{
				if (this._autohideMinHeight != value)
				{
					this.RaisePropertyChanging("AutoHideMinHeight");
					if (value < 0)
					{
						throw new ArgumentException("value");
					}
					this._autohideMinHeight = value;
					this.RaisePropertyChanged("AutoHideMinHeight");
				}
			}
		}

		public double AutoHideMinWidth
		{
			get
			{
				return this._autohideMinWidth;
			}
			set
			{
				if (this._autohideMinWidth != value)
				{
					this.RaisePropertyChanging("AutoHideMinWidth");
					if (value < 0)
					{
						throw new ArgumentException("value");
					}
					this._autohideMinWidth = value;
					this.RaisePropertyChanged("AutoHideMinWidth");
				}
			}
		}

		public double AutoHideWidth
		{
			get
			{
				return this._autohideWidth;
			}
			set
			{
				if (this._autohideWidth != value)
				{
					this.RaisePropertyChanging("AutoHideWidth");
					value = Math.Max(value, this._autohideMinWidth);
					this._autohideWidth = value;
					this.RaisePropertyChanged("AutoHideWidth");
				}
			}
		}

		public bool CanAutoHide
		{
			get
			{
				return this._canAutoHide;
			}
			set
			{
				if (this._canAutoHide != value)
				{
					this._canAutoHide = value;
					this.RaisePropertyChanged("CanAutoHide");
				}
			}
		}

		public bool CanHide
		{
			get
			{
				return this._canHide;
			}
			set
			{
				if (this._canHide != value)
				{
					this._canHide = value;
					this.RaisePropertyChanged("CanHide");
				}
			}
		}

		public bool IsAutoHidden
		{
			get
			{
				if (base.Parent == null)
				{
					return false;
				}
				return base.Parent is LayoutAnchorGroup;
			}
		}

		[XmlIgnore]
		public bool IsHidden
		{
			get
			{
				return base.Parent is LayoutRoot;
			}
		}

		[XmlIgnore]
		public bool IsVisible
		{
			get
			{
				if (base.Parent == null)
				{
					return false;
				}
				return !(base.Parent is LayoutRoot);
			}
			set
			{
				if (value)
				{
					this.Show();
					return;
				}
				this.Hide(true);
			}
		}

		public LayoutAnchorable()
		{
		}

		public void AddToLayout(DockingManager manager, AnchorableShowStrategy strategy)
		{
			if (this.IsVisible || this.IsHidden)
			{
				throw new InvalidOperationException();
			}
			bool flag = (strategy & AnchorableShowStrategy.Most) == AnchorableShowStrategy.Most;
			bool flag1 = (strategy & AnchorableShowStrategy.Left) == AnchorableShowStrategy.Left;
			bool flag2 = (strategy & AnchorableShowStrategy.Right) == AnchorableShowStrategy.Right;
			bool flag3 = (strategy & AnchorableShowStrategy.Top) == AnchorableShowStrategy.Top;
			bool flag4 = (strategy & AnchorableShowStrategy.Bottom) == AnchorableShowStrategy.Bottom;
			if (!flag)
			{
				AnchorSide anchorSide = AnchorSide.Left;
				if (flag1)
				{
					anchorSide = AnchorSide.Left;
				}
				if (flag2)
				{
					anchorSide = AnchorSide.Right;
				}
				if (flag3)
				{
					anchorSide = AnchorSide.Top;
				}
				if (flag4)
				{
					anchorSide = AnchorSide.Bottom;
				}
				LayoutAnchorablePane layoutAnchorablePane = manager.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault<LayoutAnchorablePane>((LayoutAnchorablePane p) => p.GetSide() == anchorSide);
				if (layoutAnchorablePane == null)
				{
					flag = true;
				}
				else
				{
					layoutAnchorablePane.Children.Add(this);
				}
			}
			if (flag)
			{
				if (manager.Layout.RootPanel == null)
				{
					manager.Layout.RootPanel = new LayoutPanel()
					{
						Orientation = (flag1 | flag2 ? Orientation.Horizontal : Orientation.Vertical)
					};
				}
				if (flag1 | flag2)
				{
					if (manager.Layout.RootPanel.Orientation == Orientation.Vertical && manager.Layout.RootPanel.ChildrenCount > 1)
					{
						manager.Layout.RootPanel = new LayoutPanel(manager.Layout.RootPanel);
					}
					manager.Layout.RootPanel.Orientation = Orientation.Horizontal;
					if (!flag1)
					{
						manager.Layout.RootPanel.Children.Add(new LayoutAnchorablePane(this));
						return;
					}
					manager.Layout.RootPanel.Children.Insert(0, new LayoutAnchorablePane(this));
					return;
				}
				if (manager.Layout.RootPanel.Orientation == Orientation.Horizontal && manager.Layout.RootPanel.ChildrenCount > 1)
				{
					manager.Layout.RootPanel = new LayoutPanel(manager.Layout.RootPanel);
				}
				manager.Layout.RootPanel.Orientation = Orientation.Vertical;
				if (flag3)
				{
					manager.Layout.RootPanel.Children.Insert(0, new LayoutAnchorablePane(this));
					return;
				}
				manager.Layout.RootPanel.Children.Add(new LayoutAnchorablePane(this));
			}
		}

		public override void Close()
		{
			this.CloseAnchorable();
		}

		internal void CloseAnchorable()
		{
			if (base.TestCanClose())
			{
				if (this.IsAutoHidden)
				{
					this.ToggleAutoHide();
				}
				base.CloseInternal();
			}
		}

		public override void ConsoleDump(int tab)
		{
			Trace.Write(new string(' ', tab * 4));
			Trace.WriteLine("Anchorable()");
		}

		public void Hide(bool cancelable = true)
		{
			if (!this.IsVisible)
			{
				base.IsSelected = true;
				base.IsActive = true;
				return;
			}
			if (cancelable)
			{
				CancelEventArgs cancelEventArg = new CancelEventArgs();
				this.OnHiding(cancelEventArg);
				if (cancelEventArg.Cancel)
				{
					return;
				}
			}
			this.RaisePropertyChanging("IsHidden");
			this.RaisePropertyChanging("IsVisible");
			ILayoutGroup parent = base.Parent as ILayoutGroup;
			base.PreviousContainer = parent;
			base.PreviousContainerIndex = parent.IndexOfChild(this);
			base.Root.Hidden.Add(this);
			this.RaisePropertyChanged("IsVisible");
			this.RaisePropertyChanged("IsHidden");
			this.NotifyIsVisibleChanged();
		}

		protected override void InternalDock()
		{
			LayoutRoot root = base.Root as LayoutRoot;
			LayoutAnchorablePane parent = null;
			if (root.ActiveContent != null && root.ActiveContent != this)
			{
				parent = root.ActiveContent.Parent as LayoutAnchorablePane;
			}
			if (parent == null)
			{
				parent = root.Descendents().OfType<LayoutAnchorablePane>().Where<LayoutAnchorablePane>((LayoutAnchorablePane pane) => {
					if (pane.IsHostedInFloatingWindow)
					{
						return false;
					}
					return pane.GetSide() == AnchorSide.Right;
				}).FirstOrDefault<LayoutAnchorablePane>();
			}
			if (parent == null)
			{
				parent = root.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault<LayoutAnchorablePane>();
			}
			bool flag = false;
			if (root.Manager.LayoutUpdateStrategy != null)
			{
				flag = root.Manager.LayoutUpdateStrategy.BeforeInsertAnchorable(root, this, parent);
			}
			if (!flag)
			{
				if (parent == null)
				{
					LayoutPanel layoutPanel = new LayoutPanel()
					{
						Orientation = Orientation.Horizontal
					};
					if (root.RootPanel != null)
					{
						layoutPanel.Children.Add(root.RootPanel);
					}
					root.RootPanel = layoutPanel;
					parent = new LayoutAnchorablePane()
					{
						DockWidth = new GridLength(200, GridUnitType.Pixel)
					};
					layoutPanel.Children.Add(parent);
				}
				parent.Children.Add(this);
				flag = true;
			}
			if (root.Manager.LayoutUpdateStrategy != null)
			{
				root.Manager.LayoutUpdateStrategy.AfterInsertAnchorable(root, this);
			}
			base.InternalDock();
		}

		private void NotifyIsVisibleChanged()
		{
			if (this.IsVisibleChanged != null)
			{
				this.IsVisibleChanged(this, EventArgs.Empty);
			}
		}

		protected virtual void OnHiding(CancelEventArgs args)
		{
			if (this.Hiding != null)
			{
				this.Hiding(this, args);
			}
		}

		protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
		{
			this.UpdateParentVisibility();
			this.RaisePropertyChanged("IsVisible");
			this.NotifyIsVisibleChanged();
			this.RaisePropertyChanged("IsHidden");
			this.RaisePropertyChanged("IsAutoHidden");
			base.OnParentChanged(oldValue, newValue);
		}

		public override void ReadXml(XmlReader reader)
		{
			if (reader.MoveToAttribute("CanHide"))
			{
				this.CanHide = bool.Parse(reader.Value);
			}
			if (reader.MoveToAttribute("CanAutoHide"))
			{
				this.CanAutoHide = bool.Parse(reader.Value);
			}
			if (reader.MoveToAttribute("AutoHideWidth"))
			{
				this.AutoHideWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			if (reader.MoveToAttribute("AutoHideHeight"))
			{
				this.AutoHideHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			if (reader.MoveToAttribute("AutoHideMinWidth"))
			{
				this.AutoHideMinWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			if (reader.MoveToAttribute("AutoHideMinHeight"))
			{
				this.AutoHideMinHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			base.ReadXml(reader);
		}

		public void Show()
		{
			if (this.IsVisible)
			{
				return;
			}
			if (!this.IsHidden)
			{
				throw new InvalidOperationException();
			}
			this.RaisePropertyChanging("IsHidden");
			this.RaisePropertyChanging("IsVisible");
			bool flag = false;
			ILayoutRoot root = base.Root;
			if (root != null && root.Manager != null && root.Manager.LayoutUpdateStrategy != null)
			{
				flag = root.Manager.LayoutUpdateStrategy.BeforeInsertAnchorable(root as LayoutRoot, this, base.PreviousContainer);
			}
			if (!flag && base.PreviousContainer != null)
			{
				ILayoutGroup previousContainer = base.PreviousContainer as ILayoutGroup;
				if (base.PreviousContainerIndex >= previousContainer.ChildrenCount)
				{
					previousContainer.InsertChildAt(previousContainer.ChildrenCount, this);
				}
				else
				{
					previousContainer.InsertChildAt(base.PreviousContainerIndex, this);
				}
				base.IsSelected = true;
				base.IsActive = true;
			}
			if (root != null && root.Manager != null && root.Manager.LayoutUpdateStrategy != null)
			{
				root.Manager.LayoutUpdateStrategy.AfterInsertAnchorable(root as LayoutRoot, this);
			}
			base.PreviousContainer = null;
			base.PreviousContainerIndex = -1;
			this.RaisePropertyChanged("IsVisible");
			this.RaisePropertyChanged("IsHidden");
			this.NotifyIsVisibleChanged();
		}

		public void ToggleAutoHide()
		{
			LayoutAnchorable[] array;
			int i;
			Func<ILayoutPreviousContainer, bool> func = null;
			if (!this.IsAutoHidden)
			{
				if (base.Parent is LayoutAnchorablePane)
				{
					ILayoutRoot root = base.Root;
					LayoutAnchorablePane parent = base.Parent as LayoutAnchorablePane;
					LayoutAnchorGroup layoutAnchorGroup = new LayoutAnchorGroup();
					((ILayoutPreviousContainer)layoutAnchorGroup).PreviousContainer = parent;
					array = parent.Children.ToArray<LayoutAnchorable>();
					for (i = 0; i < (int)array.Length; i++)
					{
						LayoutAnchorable layoutAnchorable = array[i];
						layoutAnchorGroup.Children.Add(layoutAnchorable);
					}
					switch (parent.GetSide())
					{
						case AnchorSide.Left:
						{
							root.LeftSide.Children.Add(layoutAnchorGroup);
							return;
						}
						case AnchorSide.Top:
						{
							root.TopSide.Children.Add(layoutAnchorGroup);
							return;
						}
						case AnchorSide.Right:
						{
							root.RightSide.Children.Add(layoutAnchorGroup);
							return;
						}
						case AnchorSide.Bottom:
						{
							root.BottomSide.Children.Add(layoutAnchorGroup);
							break;
						}
						default:
						{
							return;
						}
					}
				}
				return;
			}
			LayoutAnchorGroup parent1 = base.Parent as LayoutAnchorGroup;
			LayoutAnchorSide layoutAnchorSide = parent1.Parent as LayoutAnchorSide;
			LayoutAnchorablePane previousContainer = ((ILayoutPreviousContainer)parent1).PreviousContainer as LayoutAnchorablePane;
			if (previousContainer != null)
			{
				IEnumerable<ILayoutPreviousContainer> layoutPreviousContainers = (parent1.Root as LayoutRoot).Descendents().OfType<ILayoutPreviousContainer>();
				Func<ILayoutPreviousContainer, bool> func1 = func;
				if (func1 == null)
				{
					Func<ILayoutPreviousContainer, bool> previousContainer1 = (ILayoutPreviousContainer c) => c.PreviousContainer == parent1;
					Func<ILayoutPreviousContainer, bool> func2 = previousContainer1;
					func = previousContainer1;
					func1 = func2;
				}
				foreach (ILayoutPreviousContainer layoutPreviousContainer in layoutPreviousContainers.Where<ILayoutPreviousContainer>(func1))
				{
					layoutPreviousContainer.PreviousContainer = previousContainer;
				}
			}
			else
			{
				switch ((parent1.Parent as LayoutAnchorSide).Side)
				{
					case AnchorSide.Left:
					{
						if (parent1.Root.RootPanel.Orientation != Orientation.Horizontal)
						{
							previousContainer = new LayoutAnchorablePane();
							LayoutPanel layoutPanel = new LayoutPanel()
							{
								Orientation = Orientation.Horizontal
							};
							LayoutRoot layoutRoot = parent1.Root as LayoutRoot;
							LayoutPanel rootPanel = parent1.Root.RootPanel;
							layoutRoot.RootPanel = layoutPanel;
							layoutPanel.Children.Add(previousContainer);
							layoutPanel.Children.Add(rootPanel);
							break;
						}
						else
						{
							previousContainer = new LayoutAnchorablePane()
							{
								DockMinWidth = this.AutoHideMinWidth
							};
							parent1.Root.RootPanel.Children.Insert(0, previousContainer);
							break;
						}
					}
					case AnchorSide.Top:
					{
						if (parent1.Root.RootPanel.Orientation != Orientation.Vertical)
						{
							previousContainer = new LayoutAnchorablePane();
							LayoutPanel layoutPanel1 = new LayoutPanel()
							{
								Orientation = Orientation.Vertical
							};
							LayoutRoot root1 = parent1.Root as LayoutRoot;
							LayoutPanel rootPanel1 = parent1.Root.RootPanel;
							root1.RootPanel = layoutPanel1;
							layoutPanel1.Children.Add(previousContainer);
							layoutPanel1.Children.Add(rootPanel1);
							break;
						}
						else
						{
							previousContainer = new LayoutAnchorablePane()
							{
								DockMinHeight = this.AutoHideMinHeight
							};
							parent1.Root.RootPanel.Children.Insert(0, previousContainer);
							break;
						}
					}
					case AnchorSide.Right:
					{
						if (parent1.Root.RootPanel.Orientation != Orientation.Horizontal)
						{
							previousContainer = new LayoutAnchorablePane();
							LayoutPanel layoutPanel2 = new LayoutPanel()
							{
								Orientation = Orientation.Horizontal
							};
							LayoutRoot layoutRoot1 = parent1.Root as LayoutRoot;
							LayoutPanel rootPanel2 = parent1.Root.RootPanel;
							layoutRoot1.RootPanel = layoutPanel2;
							layoutPanel2.Children.Add(rootPanel2);
							layoutPanel2.Children.Add(previousContainer);
							break;
						}
						else
						{
							previousContainer = new LayoutAnchorablePane()
							{
								DockMinWidth = this.AutoHideMinWidth
							};
							parent1.Root.RootPanel.Children.Add(previousContainer);
							break;
						}
					}
					case AnchorSide.Bottom:
					{
						if (parent1.Root.RootPanel.Orientation != Orientation.Vertical)
						{
							previousContainer = new LayoutAnchorablePane();
							LayoutPanel layoutPanel3 = new LayoutPanel()
							{
								Orientation = Orientation.Vertical
							};
							LayoutRoot root2 = parent1.Root as LayoutRoot;
							LayoutPanel rootPanel3 = parent1.Root.RootPanel;
							root2.RootPanel = layoutPanel3;
							layoutPanel3.Children.Add(rootPanel3);
							layoutPanel3.Children.Add(previousContainer);
							break;
						}
						else
						{
							previousContainer = new LayoutAnchorablePane()
							{
								DockMinHeight = this.AutoHideMinHeight
							};
							parent1.Root.RootPanel.Children.Add(previousContainer);
							break;
						}
					}
				}
			}
			array = parent1.Children.ToArray<LayoutAnchorable>();
			for (i = 0; i < (int)array.Length; i++)
			{
				LayoutAnchorable layoutAnchorable1 = array[i];
				previousContainer.Children.Add(layoutAnchorable1);
			}
			layoutAnchorSide.Children.Remove(parent1);
		}

		private void UpdateParentVisibility()
		{
			ILayoutElementWithVisibility parent = base.Parent as ILayoutElementWithVisibility;
			if (parent != null)
			{
				parent.ComputeVisibility();
			}
		}

		public override void WriteXml(XmlWriter writer)
		{
			bool canHide;
			double autoHideWidth;
			if (!this.CanHide)
			{
				canHide = this.CanHide;
				writer.WriteAttributeString("CanHide", canHide.ToString());
			}
			if (!this.CanAutoHide)
			{
				canHide = this.CanAutoHide;
				writer.WriteAttributeString("CanAutoHide", canHide.ToString(CultureInfo.InvariantCulture));
			}
			if (this.AutoHideWidth > 0)
			{
				autoHideWidth = this.AutoHideWidth;
				writer.WriteAttributeString("AutoHideWidth", autoHideWidth.ToString(CultureInfo.InvariantCulture));
			}
			if (this.AutoHideHeight > 0)
			{
				autoHideWidth = this.AutoHideHeight;
				writer.WriteAttributeString("AutoHideHeight", autoHideWidth.ToString(CultureInfo.InvariantCulture));
			}
			if (this.AutoHideMinWidth != 25)
			{
				autoHideWidth = this.AutoHideMinWidth;
				writer.WriteAttributeString("AutoHideMinWidth", autoHideWidth.ToString(CultureInfo.InvariantCulture));
			}
			if (this.AutoHideMinHeight != 25)
			{
				autoHideWidth = this.AutoHideMinHeight;
				writer.WriteAttributeString("AutoHideMinHeight", autoHideWidth.ToString(CultureInfo.InvariantCulture));
			}
			base.WriteXml(writer);
		}

		public event EventHandler<CancelEventArgs> Hiding;

		public event EventHandler IsVisibleChanged;
	}
}