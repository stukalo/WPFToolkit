using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[ContentProperty("RootPanel")]
	[Serializable]
	public class LayoutAnchorableFloatingWindow : LayoutFloatingWindow, ILayoutElementWithVisibility
	{
		private LayoutAnchorablePaneGroup _rootPanel;

		[NonSerialized]
		private bool _isVisible = true;

		public override IEnumerable<ILayoutElement> Children
		{
			get
			{
				if (this.ChildrenCount == 1)
				{
					yield return this.RootPanel;
				}
			}
		}

		public override int ChildrenCount
		{
			get
			{
				if (this.RootPanel == null)
				{
					return 0;
				}
				return 1;
			}
		}

		public bool IsSinglePane
		{
			get
			{
				if (this.RootPanel == null)
				{
					return false;
				}
				return (
					from p in this.RootPanel.Descendents().OfType<ILayoutAnchorablePane>()
					where p.IsVisible
					select p).Count<ILayoutAnchorablePane>() == 1;
			}
		}

		public override bool IsValid
		{
			get
			{
				return this.RootPanel != null;
			}
		}

		[XmlIgnore]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			private set
			{
				if (this._isVisible != value)
				{
					this.RaisePropertyChanging("IsVisible");
					this._isVisible = value;
					this.RaisePropertyChanged("IsVisible");
					if (this.IsVisibleChanged != null)
					{
						this.IsVisibleChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		public LayoutAnchorablePaneGroup RootPanel
		{
			get
			{
				return this._rootPanel;
			}
			set
			{
				if (this._rootPanel != value)
				{
					this.RaisePropertyChanging("RootPanel");
					if (this._rootPanel != null)
					{
						this._rootPanel.ChildrenTreeChanged -= new EventHandler<ChildrenTreeChangedEventArgs>(this._rootPanel_ChildrenTreeChanged);
					}
					this._rootPanel = value;
					if (this._rootPanel != null)
					{
						this._rootPanel.Parent = this;
					}
					if (this._rootPanel != null)
					{
						this._rootPanel.ChildrenTreeChanged += new EventHandler<ChildrenTreeChangedEventArgs>(this._rootPanel_ChildrenTreeChanged);
					}
					this.RaisePropertyChanged("RootPanel");
					this.RaisePropertyChanged("IsSinglePane");
					this.RaisePropertyChanged("SinglePane");
					this.RaisePropertyChanged("Children");
					this.RaisePropertyChanged("ChildrenCount");
					((ILayoutElementWithVisibility)this).ComputeVisibility();
				}
			}
		}

		public ILayoutAnchorablePane SinglePane
		{
			get
			{
				if (!this.IsSinglePane)
				{
					return null;
				}
				LayoutAnchorablePane layoutAnchorablePane = this.RootPanel.Descendents().OfType<LayoutAnchorablePane>().Single<LayoutAnchorablePane>((LayoutAnchorablePane p) => p.IsVisible);
				layoutAnchorablePane.UpdateIsDirectlyHostedInFloatingWindow();
				return layoutAnchorablePane;
			}
		}

		public LayoutAnchorableFloatingWindow()
		{
		}

		private void _rootPanel_ChildrenTreeChanged(object sender, ChildrenTreeChangedEventArgs e)
		{
			this.RaisePropertyChanged("IsSinglePane");
			this.RaisePropertyChanged("SinglePane");
		}

		public override void ConsoleDump(int tab)
		{
			Trace.Write(new string(' ', tab * 4));
			Trace.WriteLine("FloatingAnchorableWindow()");
			this.RootPanel.ConsoleDump(tab + 1);
		}

		public override void RemoveChild(ILayoutElement element)
		{
			this.RootPanel = null;
		}

		public override void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
		{
			this.RootPanel = newElement as LayoutAnchorablePaneGroup;
		}

		void Xceed.Wpf.AvalonDock.Layout.ILayoutElementWithVisibility.ComputeVisibility()
		{
			if (this.RootPanel == null)
			{
				this.IsVisible = false;
				return;
			}
			this.IsVisible = this.RootPanel.IsVisible;
		}

		public event EventHandler IsVisibleChanged;
	}
}