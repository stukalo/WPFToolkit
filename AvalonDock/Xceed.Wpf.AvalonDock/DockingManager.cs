using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;

namespace Xceed.Wpf.AvalonDock
{
	[ContentProperty("Layout")]
	[TemplatePart(Name="PART_AutoHideArea")]
	public class DockingManager : Control, IOverlayWindowHost
	{
		private ResourceDictionary currentThemeResourceDictionary;

		public readonly static DependencyProperty LayoutProperty;

		public readonly static DependencyProperty LayoutUpdateStrategyProperty;

		public readonly static DependencyProperty DocumentPaneTemplateProperty;

		public readonly static DependencyProperty AnchorablePaneTemplateProperty;

		public readonly static DependencyProperty AnchorSideTemplateProperty;

		public readonly static DependencyProperty AnchorGroupTemplateProperty;

		public readonly static DependencyProperty AnchorTemplateProperty;

		public readonly static DependencyProperty DocumentPaneControlStyleProperty;

		public readonly static DependencyProperty AnchorablePaneControlStyleProperty;

		public readonly static DependencyProperty DocumentHeaderTemplateProperty;

		public readonly static DependencyProperty DocumentHeaderTemplateSelectorProperty;

		public readonly static DependencyProperty DocumentTitleTemplateProperty;

		public readonly static DependencyProperty DocumentTitleTemplateSelectorProperty;

		public readonly static DependencyProperty AnchorableTitleTemplateProperty;

		public readonly static DependencyProperty AnchorableTitleTemplateSelectorProperty;

		public readonly static DependencyProperty AnchorableHeaderTemplateProperty;

		public readonly static DependencyProperty AnchorableHeaderTemplateSelectorProperty;

		public readonly static DependencyProperty LayoutRootPanelProperty;

		public readonly static DependencyProperty RightSidePanelProperty;

		public readonly static DependencyProperty LeftSidePanelProperty;

		public readonly static DependencyProperty TopSidePanelProperty;

		public readonly static DependencyProperty BottomSidePanelProperty;

		private List<WeakReference> _logicalChildren = new List<WeakReference>();

		private AutoHideWindowManager _autoHideWindowManager;

		private FrameworkElement _autohideArea;

		private readonly static DependencyPropertyKey AutoHideWindowPropertyKey;

		public readonly static DependencyProperty AutoHideWindowProperty;

		private List<LayoutFloatingWindowControl> _fwList = new List<LayoutFloatingWindowControl>();

		private OverlayWindow _overlayWindow;

		private List<IDropArea> _areas;

		public readonly static DependencyProperty LayoutItemTemplateProperty;

		public readonly static DependencyProperty LayoutItemTemplateSelectorProperty;

		public readonly static DependencyProperty DocumentsSourceProperty;

		internal bool SuspendDocumentsSourceBinding;

		public readonly static DependencyProperty DocumentContextMenuProperty;

		public readonly static DependencyProperty AnchorablesSourceProperty;

		internal bool SuspendAnchorablesSourceBinding;

		public readonly static DependencyProperty ActiveContentProperty;

		private bool _insideInternalSetActiveContent;

		public readonly static DependencyProperty AnchorableContextMenuProperty;

		public readonly static DependencyProperty ThemeProperty;

		public readonly static DependencyProperty GridSplitterWidthProperty;

		public readonly static DependencyProperty GridSplitterHeightProperty;

		public readonly static DependencyProperty DocumentPaneMenuItemHeaderTemplateProperty;

		public readonly static DependencyProperty DocumentPaneMenuItemHeaderTemplateSelectorProperty;

		public readonly static DependencyProperty IconContentTemplateProperty;

		public readonly static DependencyProperty IconContentTemplateSelectorProperty;

		private List<LayoutItem> _layoutItems = new List<LayoutItem>();

		private bool _suspendLayoutItemCreation;

		private DispatcherOperation _collectLayoutItemsOperations;

		public readonly static DependencyProperty LayoutItemContainerStyleProperty;

		public readonly static DependencyProperty LayoutItemContainerStyleSelectorProperty;

		private NavigatorWindow _navigatorWindow;

		public readonly static DependencyProperty ShowSystemMenuProperty;

		public readonly static DependencyProperty AllowMixedOrientationProperty;

		public object ActiveContent
		{
			get
			{
				return base.GetValue(DockingManager.ActiveContentProperty);
			}
			set
			{
				base.SetValue(DockingManager.ActiveContentProperty, value);
			}
		}

		public bool AllowMixedOrientation
		{
			get
			{
				return (bool)base.GetValue(DockingManager.AllowMixedOrientationProperty);
			}
			set
			{
				base.SetValue(DockingManager.AllowMixedOrientationProperty, value);
			}
		}

		public System.Windows.Controls.ContextMenu AnchorableContextMenu
		{
			get
			{
				return (System.Windows.Controls.ContextMenu)base.GetValue(DockingManager.AnchorableContextMenuProperty);
			}
			set
			{
				base.SetValue(DockingManager.AnchorableContextMenuProperty, value);
			}
		}

		public DataTemplate AnchorableHeaderTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(DockingManager.AnchorableHeaderTemplateProperty);
			}
			set
			{
				base.SetValue(DockingManager.AnchorableHeaderTemplateProperty, value);
			}
		}

		public DataTemplateSelector AnchorableHeaderTemplateSelector
		{
			get
			{
				return (DataTemplateSelector)base.GetValue(DockingManager.AnchorableHeaderTemplateSelectorProperty);
			}
			set
			{
				base.SetValue(DockingManager.AnchorableHeaderTemplateSelectorProperty, value);
			}
		}

		public System.Windows.Style AnchorablePaneControlStyle
		{
			get
			{
				return (System.Windows.Style)base.GetValue(DockingManager.AnchorablePaneControlStyleProperty);
			}
			set
			{
				base.SetValue(DockingManager.AnchorablePaneControlStyleProperty, value);
			}
		}

		public ControlTemplate AnchorablePaneTemplate
		{
			get
			{
				return (ControlTemplate)base.GetValue(DockingManager.AnchorablePaneTemplateProperty);
			}
			set
			{
				base.SetValue(DockingManager.AnchorablePaneTemplateProperty, value);
			}
		}

		public IEnumerable AnchorablesSource
		{
			get
			{
				return (IEnumerable)base.GetValue(DockingManager.AnchorablesSourceProperty);
			}
			set
			{
				base.SetValue(DockingManager.AnchorablesSourceProperty, value);
			}
		}

		public DataTemplate AnchorableTitleTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(DockingManager.AnchorableTitleTemplateProperty);
			}
			set
			{
				base.SetValue(DockingManager.AnchorableTitleTemplateProperty, value);
			}
		}

		public DataTemplateSelector AnchorableTitleTemplateSelector
		{
			get
			{
				return (DataTemplateSelector)base.GetValue(DockingManager.AnchorableTitleTemplateSelectorProperty);
			}
			set
			{
				base.SetValue(DockingManager.AnchorableTitleTemplateSelectorProperty, value);
			}
		}

		public ControlTemplate AnchorGroupTemplate
		{
			get
			{
				return (ControlTemplate)base.GetValue(DockingManager.AnchorGroupTemplateProperty);
			}
			set
			{
				base.SetValue(DockingManager.AnchorGroupTemplateProperty, value);
			}
		}

		public ControlTemplate AnchorSideTemplate
		{
			get
			{
				return (ControlTemplate)base.GetValue(DockingManager.AnchorSideTemplateProperty);
			}
			set
			{
				base.SetValue(DockingManager.AnchorSideTemplateProperty, value);
			}
		}

		public ControlTemplate AnchorTemplate
		{
			get
			{
				return (ControlTemplate)base.GetValue(DockingManager.AnchorTemplateProperty);
			}
			set
			{
				base.SetValue(DockingManager.AnchorTemplateProperty, value);
			}
		}

		public LayoutAutoHideWindowControl AutoHideWindow
		{
			get
			{
				return (LayoutAutoHideWindowControl)base.GetValue(DockingManager.AutoHideWindowProperty);
			}
		}

		public LayoutAnchorSideControl BottomSidePanel
		{
			get
			{
				return (LayoutAnchorSideControl)base.GetValue(DockingManager.BottomSidePanelProperty);
			}
			set
			{
				base.SetValue(DockingManager.BottomSidePanelProperty, value);
			}
		}

		public System.Windows.Controls.ContextMenu DocumentContextMenu
		{
			get
			{
				return (System.Windows.Controls.ContextMenu)base.GetValue(DockingManager.DocumentContextMenuProperty);
			}
			set
			{
				base.SetValue(DockingManager.DocumentContextMenuProperty, value);
			}
		}

		public DataTemplate DocumentHeaderTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(DockingManager.DocumentHeaderTemplateProperty);
			}
			set
			{
				base.SetValue(DockingManager.DocumentHeaderTemplateProperty, value);
			}
		}

		public DataTemplateSelector DocumentHeaderTemplateSelector
		{
			get
			{
				return (DataTemplateSelector)base.GetValue(DockingManager.DocumentHeaderTemplateSelectorProperty);
			}
			set
			{
				base.SetValue(DockingManager.DocumentHeaderTemplateSelectorProperty, value);
			}
		}

		public System.Windows.Style DocumentPaneControlStyle
		{
			get
			{
				return (System.Windows.Style)base.GetValue(DockingManager.DocumentPaneControlStyleProperty);
			}
			set
			{
				base.SetValue(DockingManager.DocumentPaneControlStyleProperty, value);
			}
		}

		public DataTemplate DocumentPaneMenuItemHeaderTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(DockingManager.DocumentPaneMenuItemHeaderTemplateProperty);
			}
			set
			{
				base.SetValue(DockingManager.DocumentPaneMenuItemHeaderTemplateProperty, value);
			}
		}

		public DataTemplateSelector DocumentPaneMenuItemHeaderTemplateSelector
		{
			get
			{
				return (DataTemplateSelector)base.GetValue(DockingManager.DocumentPaneMenuItemHeaderTemplateSelectorProperty);
			}
			set
			{
				base.SetValue(DockingManager.DocumentPaneMenuItemHeaderTemplateSelectorProperty, value);
			}
		}

		public ControlTemplate DocumentPaneTemplate
		{
			get
			{
				return (ControlTemplate)base.GetValue(DockingManager.DocumentPaneTemplateProperty);
			}
			set
			{
				base.SetValue(DockingManager.DocumentPaneTemplateProperty, value);
			}
		}

		public IEnumerable DocumentsSource
		{
			get
			{
				return (IEnumerable)base.GetValue(DockingManager.DocumentsSourceProperty);
			}
			set
			{
				base.SetValue(DockingManager.DocumentsSourceProperty, value);
			}
		}

		public DataTemplate DocumentTitleTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(DockingManager.DocumentTitleTemplateProperty);
			}
			set
			{
				base.SetValue(DockingManager.DocumentTitleTemplateProperty, value);
			}
		}

		public DataTemplateSelector DocumentTitleTemplateSelector
		{
			get
			{
				return (DataTemplateSelector)base.GetValue(DockingManager.DocumentTitleTemplateSelectorProperty);
			}
			set
			{
				base.SetValue(DockingManager.DocumentTitleTemplateSelectorProperty, value);
			}
		}

		public IEnumerable<LayoutFloatingWindowControl> FloatingWindows
		{
			get
			{
				return this._fwList;
			}
		}

		public double GridSplitterHeight
		{
			get
			{
				return (double)base.GetValue(DockingManager.GridSplitterHeightProperty);
			}
			set
			{
				base.SetValue(DockingManager.GridSplitterHeightProperty, value);
			}
		}

		public double GridSplitterWidth
		{
			get
			{
				return (double)base.GetValue(DockingManager.GridSplitterWidthProperty);
			}
			set
			{
				base.SetValue(DockingManager.GridSplitterWidthProperty, value);
			}
		}

		public DataTemplate IconContentTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(DockingManager.IconContentTemplateProperty);
			}
			set
			{
				base.SetValue(DockingManager.IconContentTemplateProperty, value);
			}
		}

		public DataTemplateSelector IconContentTemplateSelector
		{
			get
			{
				return (DataTemplateSelector)base.GetValue(DockingManager.IconContentTemplateSelectorProperty);
			}
			set
			{
				base.SetValue(DockingManager.IconContentTemplateSelectorProperty, value);
			}
		}

		private bool IsNavigatorWindowActive
		{
			get
			{
				return this._navigatorWindow != null;
			}
		}

		public LayoutRoot Layout
		{
			get
			{
				return (LayoutRoot)base.GetValue(DockingManager.LayoutProperty);
			}
			set
			{
				base.SetValue(DockingManager.LayoutProperty, value);
			}
		}

		public System.Windows.Style LayoutItemContainerStyle
		{
			get
			{
				return (System.Windows.Style)base.GetValue(DockingManager.LayoutItemContainerStyleProperty);
			}
			set
			{
				base.SetValue(DockingManager.LayoutItemContainerStyleProperty, value);
			}
		}

		public StyleSelector LayoutItemContainerStyleSelector
		{
			get
			{
				return (StyleSelector)base.GetValue(DockingManager.LayoutItemContainerStyleSelectorProperty);
			}
			set
			{
				base.SetValue(DockingManager.LayoutItemContainerStyleSelectorProperty, value);
			}
		}

		public DataTemplate LayoutItemTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(DockingManager.LayoutItemTemplateProperty);
			}
			set
			{
				base.SetValue(DockingManager.LayoutItemTemplateProperty, value);
			}
		}

		public DataTemplateSelector LayoutItemTemplateSelector
		{
			get
			{
				return (DataTemplateSelector)base.GetValue(DockingManager.LayoutItemTemplateSelectorProperty);
			}
			set
			{
				base.SetValue(DockingManager.LayoutItemTemplateSelectorProperty, value);
			}
		}

		public LayoutPanelControl LayoutRootPanel
		{
			get
			{
				return (LayoutPanelControl)base.GetValue(DockingManager.LayoutRootPanelProperty);
			}
			set
			{
				base.SetValue(DockingManager.LayoutRootPanelProperty, value);
			}
		}

		public ILayoutUpdateStrategy LayoutUpdateStrategy
		{
			get
			{
				return (ILayoutUpdateStrategy)base.GetValue(DockingManager.LayoutUpdateStrategyProperty);
			}
			set
			{
				base.SetValue(DockingManager.LayoutUpdateStrategyProperty, value);
			}
		}

		public LayoutAnchorSideControl LeftSidePanel
		{
			get
			{
				return (LayoutAnchorSideControl)base.GetValue(DockingManager.LeftSidePanelProperty);
			}
			set
			{
				base.SetValue(DockingManager.LeftSidePanelProperty, value);
			}
		}

		protected override IEnumerator LogicalChildren
		{
			get
			{
				return (
					from ch in this._logicalChildren
					select ch.GetValueOrDefault<object>()).GetEnumerator();
			}
		}

		public IEnumerator LogicalChildrenPublic
		{
			get
			{
				return this.LogicalChildren;
			}
		}

		public LayoutAnchorSideControl RightSidePanel
		{
			get
			{
				return (LayoutAnchorSideControl)base.GetValue(DockingManager.RightSidePanelProperty);
			}
			set
			{
				base.SetValue(DockingManager.RightSidePanelProperty, value);
			}
		}

		public bool ShowSystemMenu
		{
			get
			{
				return (bool)base.GetValue(DockingManager.ShowSystemMenuProperty);
			}
			set
			{
				base.SetValue(DockingManager.ShowSystemMenuProperty, value);
			}
		}

		public Xceed.Wpf.AvalonDock.Themes.Theme Theme
		{
			get
			{
				return (Xceed.Wpf.AvalonDock.Themes.Theme)base.GetValue(DockingManager.ThemeProperty);
			}
			set
			{
				base.SetValue(DockingManager.ThemeProperty, value);
			}
		}

		public LayoutAnchorSideControl TopSidePanel
		{
			get
			{
				return (LayoutAnchorSideControl)base.GetValue(DockingManager.TopSidePanelProperty);
			}
			set
			{
				base.SetValue(DockingManager.TopSidePanelProperty, value);
			}
		}

		DockingManager Xceed.Wpf.AvalonDock.Controls.IOverlayWindowHost.Manager
		{
			get
			{
				return this;
			}
		}

		static DockingManager()
		{
			DockingManager.LayoutProperty = DependencyProperty.Register("Layout", typeof(LayoutRoot), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnLayoutChanged), new CoerceValueCallback(DockingManager.CoerceLayoutValue)));
			DockingManager.LayoutUpdateStrategyProperty = DependencyProperty.Register("LayoutUpdateStrategy", typeof(ILayoutUpdateStrategy), typeof(DockingManager), new FrameworkPropertyMetadata(null));
			DockingManager.DocumentPaneTemplateProperty = DependencyProperty.Register("DocumentPaneTemplate", typeof(ControlTemplate), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnDocumentPaneTemplateChanged)));
			DockingManager.AnchorablePaneTemplateProperty = DependencyProperty.Register("AnchorablePaneTemplate", typeof(ControlTemplate), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnAnchorablePaneTemplateChanged)));
			DockingManager.AnchorSideTemplateProperty = DependencyProperty.Register("AnchorSideTemplate", typeof(ControlTemplate), typeof(DockingManager), new FrameworkPropertyMetadata(null));
			DockingManager.AnchorGroupTemplateProperty = DependencyProperty.Register("AnchorGroupTemplate", typeof(ControlTemplate), typeof(DockingManager), new FrameworkPropertyMetadata(null));
			DockingManager.AnchorTemplateProperty = DependencyProperty.Register("AnchorTemplate", typeof(ControlTemplate), typeof(DockingManager), new FrameworkPropertyMetadata(null));
			DockingManager.DocumentPaneControlStyleProperty = DependencyProperty.Register("DocumentPaneControlStyle", typeof(System.Windows.Style), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnDocumentPaneControlStyleChanged)));
			DockingManager.AnchorablePaneControlStyleProperty = DependencyProperty.Register("AnchorablePaneControlStyle", typeof(System.Windows.Style), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnAnchorablePaneControlStyleChanged)));
			DockingManager.DocumentHeaderTemplateProperty = DependencyProperty.Register("DocumentHeaderTemplate", typeof(DataTemplate), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnDocumentHeaderTemplateChanged), new CoerceValueCallback(DockingManager.CoerceDocumentHeaderTemplateValue)));
			DockingManager.DocumentHeaderTemplateSelectorProperty = DependencyProperty.Register("DocumentHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnDocumentHeaderTemplateSelectorChanged), new CoerceValueCallback(DockingManager.CoerceDocumentHeaderTemplateSelectorValue)));
			DockingManager.DocumentTitleTemplateProperty = DependencyProperty.Register("DocumentTitleTemplate", typeof(DataTemplate), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnDocumentTitleTemplateChanged), new CoerceValueCallback(DockingManager.CoerceDocumentTitleTemplateValue)));
			DockingManager.DocumentTitleTemplateSelectorProperty = DependencyProperty.Register("DocumentTitleTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnDocumentTitleTemplateSelectorChanged), new CoerceValueCallback(DockingManager.CoerceDocumentTitleTemplateSelectorValue)));
			DockingManager.AnchorableTitleTemplateProperty = DependencyProperty.Register("AnchorableTitleTemplate", typeof(DataTemplate), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnAnchorableTitleTemplateChanged), new CoerceValueCallback(DockingManager.CoerceAnchorableTitleTemplateValue)));
			DockingManager.AnchorableTitleTemplateSelectorProperty = DependencyProperty.Register("AnchorableTitleTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnAnchorableTitleTemplateSelectorChanged)));
			DockingManager.AnchorableHeaderTemplateProperty = DependencyProperty.Register("AnchorableHeaderTemplate", typeof(DataTemplate), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnAnchorableHeaderTemplateChanged), new CoerceValueCallback(DockingManager.CoerceAnchorableHeaderTemplateValue)));
			DockingManager.AnchorableHeaderTemplateSelectorProperty = DependencyProperty.Register("AnchorableHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnAnchorableHeaderTemplateSelectorChanged)));
			DockingManager.LayoutRootPanelProperty = DependencyProperty.Register("LayoutRootPanel", typeof(LayoutPanelControl), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnLayoutRootPanelChanged)));
			DockingManager.RightSidePanelProperty = DependencyProperty.Register("RightSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnRightSidePanelChanged)));
			DockingManager.LeftSidePanelProperty = DependencyProperty.Register("LeftSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnLeftSidePanelChanged)));
			DockingManager.TopSidePanelProperty = DependencyProperty.Register("TopSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnTopSidePanelChanged)));
			DockingManager.BottomSidePanelProperty = DependencyProperty.Register("BottomSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnBottomSidePanelChanged)));
			DockingManager.AutoHideWindowPropertyKey = DependencyProperty.RegisterReadOnly("AutoHideWindow", typeof(LayoutAutoHideWindowControl), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnAutoHideWindowChanged)));
			DockingManager.AutoHideWindowProperty = DockingManager.AutoHideWindowPropertyKey.DependencyProperty;
			DockingManager.LayoutItemTemplateProperty = DependencyProperty.Register("LayoutItemTemplate", typeof(DataTemplate), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnLayoutItemTemplateChanged)));
			DockingManager.LayoutItemTemplateSelectorProperty = DependencyProperty.Register("LayoutItemTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnLayoutItemTemplateSelectorChanged)));
			DockingManager.DocumentsSourceProperty = DependencyProperty.Register("DocumentsSource", typeof(IEnumerable), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnDocumentsSourceChanged)));
			DockingManager.DocumentContextMenuProperty = DependencyProperty.Register("DocumentContextMenu", typeof(System.Windows.Controls.ContextMenu), typeof(DockingManager), new FrameworkPropertyMetadata(null));
			DockingManager.AnchorablesSourceProperty = DependencyProperty.Register("AnchorablesSource", typeof(IEnumerable), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnAnchorablesSourceChanged)));
			DockingManager.ActiveContentProperty = DependencyProperty.Register("ActiveContent", typeof(object), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnActiveContentChanged)));
			DockingManager.AnchorableContextMenuProperty = DependencyProperty.Register("AnchorableContextMenu", typeof(System.Windows.Controls.ContextMenu), typeof(DockingManager), new FrameworkPropertyMetadata(null));
			DockingManager.ThemeProperty = DependencyProperty.Register("Theme", typeof(Xceed.Wpf.AvalonDock.Themes.Theme), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnThemeChanged)));
			DockingManager.GridSplitterWidthProperty = DependencyProperty.Register("GridSplitterWidth", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata((object)6));
			DockingManager.GridSplitterHeightProperty = DependencyProperty.Register("GridSplitterHeight", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata((object)6));
			DockingManager.DocumentPaneMenuItemHeaderTemplateProperty = DependencyProperty.Register("DocumentPaneMenuItemHeaderTemplate", typeof(DataTemplate), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnDocumentPaneMenuItemHeaderTemplateChanged), new CoerceValueCallback(DockingManager.CoerceDocumentPaneMenuItemHeaderTemplateValue)));
			DockingManager.DocumentPaneMenuItemHeaderTemplateSelectorProperty = DependencyProperty.Register("DocumentPaneMenuItemHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnDocumentPaneMenuItemHeaderTemplateSelectorChanged), new CoerceValueCallback(DockingManager.CoerceDocumentPaneMenuItemHeaderTemplateSelectorValue)));
			DockingManager.IconContentTemplateProperty = DependencyProperty.Register("IconContentTemplate", typeof(DataTemplate), typeof(DockingManager), new FrameworkPropertyMetadata(null));
			DockingManager.IconContentTemplateSelectorProperty = DependencyProperty.Register("IconContentTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager), new FrameworkPropertyMetadata(null));
			DockingManager.LayoutItemContainerStyleProperty = DependencyProperty.Register("LayoutItemContainerStyle", typeof(System.Windows.Style), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnLayoutItemContainerStyleChanged)));
			DockingManager.LayoutItemContainerStyleSelectorProperty = DependencyProperty.Register("LayoutItemContainerStyleSelector", typeof(StyleSelector), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DockingManager.OnLayoutItemContainerStyleSelectorChanged)));
			DockingManager.ShowSystemMenuProperty = DependencyProperty.Register("ShowSystemMenu", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));
			DockingManager.AllowMixedOrientationProperty = DependencyProperty.Register("AllowMixedOrientation", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DockingManager), new FrameworkPropertyMetadata(typeof(DockingManager)));
			UIElement.FocusableProperty.OverrideMetadata(typeof(DockingManager), new FrameworkPropertyMetadata(false));
			HwndSource.DefaultAcquireHwndFocusInMenuMode = false;
		}

		public DockingManager()
		{
			this.Layout = new LayoutRoot()
			{
				RootPanel = new LayoutPanel(new LayoutDocumentPaneGroup(new LayoutDocumentPane()))
			};
			base.Loaded += new RoutedEventHandler(this.DockingManager_Loaded);
			base.Unloaded += new RoutedEventHandler(this.DockingManager_Unloaded);
		}

		internal void _ExecuteAutoHideCommand(LayoutAnchorable _anchorable)
		{
			_anchorable.ToggleAutoHide();
		}

		internal void _ExecuteCloseAllButThisCommand(LayoutContent contentSelected)
		{
			Func<LayoutContent, bool> func = null;
			IEnumerable<LayoutContent> layoutContents = this.Layout.Descendents().OfType<LayoutContent>();
			Func<LayoutContent, bool> func1 = func;
			if (func1 == null)
			{
				Func<LayoutContent, bool> func2 = (LayoutContent d) => {
					if (d == contentSelected)
					{
						return false;
					}
					if (d.Parent is LayoutDocumentPane)
					{
						return true;
					}
					return d.Parent is LayoutDocumentFloatingWindow;
				};
				Func<LayoutContent, bool> func3 = func2;
				func = func2;
				func1 = func3;
			}
			LayoutContent[] array = layoutContents.Where<LayoutContent>(func1).ToArray<LayoutContent>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				this.Close(array[i]);
			}
		}

		internal void _ExecuteCloseAllCommand(LayoutContent contentSelected)
		{
			LayoutContent[] array = this.Layout.Descendents().OfType<LayoutContent>().Where<LayoutContent>((LayoutContent d) => {
				if (d.Parent is LayoutDocumentPane)
				{
					return true;
				}
				return d.Parent is LayoutDocumentFloatingWindow;
			}).ToArray<LayoutContent>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				this.Close(array[i]);
			}
		}

		internal void _ExecuteCloseCommand(LayoutDocument document)
		{
			if (this.DocumentClosing != null)
			{
				DocumentClosingEventArgs documentClosingEventArg = new DocumentClosingEventArgs(document);
				this.DocumentClosing(this, documentClosingEventArg);
				if (documentClosingEventArg.Cancel)
				{
					return;
				}
			}
			if (document.CloseDocument())
			{
				this.RemoveViewFromLogicalChild(document);
				if (this.DocumentClosed != null)
				{
					DocumentClosedEventArgs documentClosedEventArg = new DocumentClosedEventArgs(document);
					this.DocumentClosed(this, documentClosedEventArg);
				}
			}
		}

		internal void _ExecuteCloseCommand(LayoutAnchorable anchorable)
		{
			LayoutAnchorable layoutAnchorable = anchorable;
			if (layoutAnchorable != null)
			{
				layoutAnchorable.CloseAnchorable();
				this.RemoveViewFromLogicalChild(anchorable);
			}
		}

		internal void _ExecuteContentActivateCommand(LayoutContent content)
		{
			content.IsActive = true;
		}

		internal void _ExecuteDockAsDocumentCommand(LayoutContent content)
		{
			content.DockAsDocument();
		}

		internal void _ExecuteDockCommand(LayoutAnchorable anchorable)
		{
			anchorable.Dock();
		}

		internal void _ExecuteFloatCommand(LayoutContent contentToFloat)
		{
			contentToFloat.Float();
		}

		internal void _ExecuteHideCommand(LayoutAnchorable anchorable)
		{
			LayoutAnchorable layoutAnchorable = anchorable;
			if (layoutAnchorable != null)
			{
				layoutAnchorable.Hide(true);
			}
		}

		private void anchorablesSourceElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			LayoutAnchorable[] array;
			int i;
			if (this.Layout == null)
			{
				return;
			}
			if (this.SuspendAnchorablesSourceBinding)
			{
				return;
			}
			if ((e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace) && e.OldItems != null)
			{
				array = (
					from d in this.Layout.Descendents().OfType<LayoutAnchorable>()
					where e.OldItems.Contains(d.Content)
					select d).ToArray<LayoutAnchorable>();
				for (i = 0; i < (int)array.Length; i++)
				{
					LayoutAnchorable layoutAnchorable = array[i];
					layoutAnchorable.Content = null;
					layoutAnchorable.Parent.RemoveChild(layoutAnchorable);
					this.RemoveViewFromLogicalChild(layoutAnchorable);
				}
			}
			if (e.NewItems != null && (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace) && e.NewItems != null)
			{
				LayoutAnchorablePane parent = null;
				if (this.Layout.ActiveContent != null)
				{
					parent = this.Layout.ActiveContent.Parent as LayoutAnchorablePane;
				}
				if (parent == null)
				{
					parent = this.Layout.Descendents().OfType<LayoutAnchorablePane>().Where<LayoutAnchorablePane>((LayoutAnchorablePane pane) => {
						if (pane.IsHostedInFloatingWindow)
						{
							return false;
						}
						return pane.GetSide() == AnchorSide.Right;
					}).FirstOrDefault<LayoutAnchorablePane>();
				}
				if (parent == null)
				{
					parent = this.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault<LayoutAnchorablePane>();
				}
				this._suspendLayoutItemCreation = true;
				foreach (object newItem in e.NewItems)
				{
					LayoutAnchorable layoutAnchorable1 = new LayoutAnchorable()
					{
						Content = newItem
					};
					bool flag = false;
					if (this.LayoutUpdateStrategy != null)
					{
						flag = this.LayoutUpdateStrategy.BeforeInsertAnchorable(this.Layout, layoutAnchorable1, parent);
					}
					if (!flag)
					{
						if (parent == null)
						{
							LayoutPanel layoutPanel = new LayoutPanel()
							{
								Orientation = Orientation.Horizontal
							};
							if (this.Layout.RootPanel != null)
							{
								layoutPanel.Children.Add(this.Layout.RootPanel);
							}
							this.Layout.RootPanel = layoutPanel;
							parent = new LayoutAnchorablePane()
							{
								DockWidth = new GridLength(200, GridUnitType.Pixel)
							};
							layoutPanel.Children.Add(parent);
						}
						parent.Children.Add(layoutAnchorable1);
						flag = true;
					}
					if (this.LayoutUpdateStrategy != null)
					{
						this.LayoutUpdateStrategy.AfterInsertAnchorable(this.Layout, layoutAnchorable1);
					}
					ILayoutRoot root = layoutAnchorable1.Root;
					if (root == null || root.Manager != this)
					{
						continue;
					}
					this.CreateAnchorableLayoutItem(layoutAnchorable1);
				}
				this._suspendLayoutItemCreation = false;
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				array = this.Layout.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>();
				for (i = 0; i < (int)array.Length; i++)
				{
					LayoutAnchorable layoutAnchorable2 = array[i];
					layoutAnchorable2.Parent.RemoveChild(layoutAnchorable2);
					this.RemoveViewFromLogicalChild(layoutAnchorable2);
				}
			}
			if (this.Layout != null)
			{
				this.Layout.CollectGarbage();
			}
		}

		private void ApplyStyleToLayoutItem(LayoutItem layoutItem)
		{
			layoutItem._ClearDefaultBindings();
			if (this.LayoutItemContainerStyle != null)
			{
				layoutItem.Style = this.LayoutItemContainerStyle;
			}
			else if (this.LayoutItemContainerStyleSelector != null)
			{
				layoutItem.Style = this.LayoutItemContainerStyleSelector.SelectStyle(layoutItem.Model, layoutItem);
			}
			layoutItem._SetDefaultBindings();
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			this._areas = null;
			return base.ArrangeOverride(arrangeBounds);
		}

		private void AttachAnchorablesSource(LayoutRoot layout, IEnumerable anchorablesSource)
		{
			if (anchorablesSource == null)
			{
				return;
			}
			if (layout == null)
			{
				return;
			}
			object[] array = (
				from d in layout.Descendents().OfType<LayoutAnchorable>()
				select d.Content).ToArray<object>();
			List<object> objs = new List<object>(anchorablesSource.OfType<object>());
			object[] objArray = objs.ToArray();
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				object obj = objArray[i];
				if (array.Contains(obj))
				{
					objs.Remove(obj);
				}
			}
			LayoutAnchorablePane parent = null;
			if (layout.ActiveContent != null)
			{
				parent = layout.ActiveContent.Parent as LayoutAnchorablePane;
			}
			if (parent == null)
			{
				parent = layout.Descendents().OfType<LayoutAnchorablePane>().Where<LayoutAnchorablePane>((LayoutAnchorablePane pane) => {
					if (pane.IsHostedInFloatingWindow)
					{
						return false;
					}
					return pane.GetSide() == AnchorSide.Right;
				}).FirstOrDefault<LayoutAnchorablePane>();
			}
			if (parent == null)
			{
				parent = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault<LayoutAnchorablePane>();
			}
			this._suspendLayoutItemCreation = true;
			foreach (object obj1 in objs)
			{
				LayoutAnchorable layoutAnchorable = new LayoutAnchorable()
				{
					Content = obj1
				};
				bool flag = false;
				if (this.LayoutUpdateStrategy != null)
				{
					flag = this.LayoutUpdateStrategy.BeforeInsertAnchorable(layout, layoutAnchorable, parent);
				}
				if (!flag)
				{
					if (parent == null)
					{
						LayoutPanel layoutPanel = new LayoutPanel()
						{
							Orientation = Orientation.Horizontal
						};
						if (layout.RootPanel != null)
						{
							layoutPanel.Children.Add(layout.RootPanel);
						}
						layout.RootPanel = layoutPanel;
						parent = new LayoutAnchorablePane()
						{
							DockWidth = new GridLength(200, GridUnitType.Pixel)
						};
						layoutPanel.Children.Add(parent);
					}
					parent.Children.Add(layoutAnchorable);
					flag = true;
				}
				if (this.LayoutUpdateStrategy != null)
				{
					this.LayoutUpdateStrategy.AfterInsertAnchorable(layout, layoutAnchorable);
				}
				this.CreateAnchorableLayoutItem(layoutAnchorable);
			}
			this._suspendLayoutItemCreation = false;
			INotifyCollectionChanged notifyCollectionChanged = anchorablesSource as INotifyCollectionChanged;
			if (notifyCollectionChanged != null)
			{
				notifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.anchorablesSourceElementsChanged);
			}
		}

		private void AttachDocumentsSource(LayoutRoot layout, IEnumerable documentsSource)
		{
			if (documentsSource == null)
			{
				return;
			}
			if (layout == null)
			{
				return;
			}
			object[] array = (
				from d in layout.Descendents().OfType<LayoutDocument>()
				select d.Content).ToArray<object>();
			List<object> objs = new List<object>(documentsSource.OfType<object>());
			object[] objArray = objs.ToArray();
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				object obj = objArray[i];
				if (array.Contains(obj))
				{
					objs.Remove(obj);
				}
			}
			LayoutDocumentPane parent = null;
			if (layout.LastFocusedDocument != null)
			{
				parent = layout.LastFocusedDocument.Parent as LayoutDocumentPane;
			}
			if (parent == null)
			{
				parent = layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>();
			}
			this._suspendLayoutItemCreation = true;
			foreach (object obj1 in objs)
			{
				LayoutDocument layoutDocument = new LayoutDocument()
				{
					Content = obj1
				};
				bool flag = false;
				if (this.LayoutUpdateStrategy != null)
				{
					flag = this.LayoutUpdateStrategy.BeforeInsertDocument(layout, layoutDocument, parent);
				}
				if (!flag)
				{
					if (parent == null)
					{
						throw new InvalidOperationException("Layout must contains at least one LayoutDocumentPane in order to host documents");
					}
					parent.Children.Add(layoutDocument);
					flag = true;
				}
				if (this.LayoutUpdateStrategy != null)
				{
					this.LayoutUpdateStrategy.AfterInsertDocument(layout, layoutDocument);
				}
				this.CreateDocumentLayoutItem(layoutDocument);
			}
			this._suspendLayoutItemCreation = false;
			INotifyCollectionChanged notifyCollectionChanged = documentsSource as INotifyCollectionChanged;
			if (notifyCollectionChanged != null)
			{
				notifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.documentsSourceElementsChanged);
			}
		}

		private void AttachLayoutItems()
		{
			int i;
			if (this.Layout != null)
			{
				LayoutDocument[] array = this.Layout.Descendents().OfType<LayoutDocument>().ToArray<LayoutDocument>();
				for (i = 0; i < (int)array.Length; i++)
				{
					this.CreateDocumentLayoutItem(array[i]);
				}
				LayoutAnchorable[] layoutAnchorableArray = this.Layout.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>();
				for (i = 0; i < (int)layoutAnchorableArray.Length; i++)
				{
					this.CreateAnchorableLayoutItem(layoutAnchorableArray[i]);
				}
				this.Layout.ElementAdded += new EventHandler<LayoutElementEventArgs>(this.Layout_ElementAdded);
				this.Layout.ElementRemoved += new EventHandler<LayoutElementEventArgs>(this.Layout_ElementRemoved);
			}
		}

		private void ClearLogicalChildrenList()
		{
			object[] array = (
				from ch in this._logicalChildren
				select ch.GetValueOrDefault<object>()).ToArray<object>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				base.RemoveLogicalChild(array[i]);
			}
			this._logicalChildren.Clear();
		}

		private void Close(LayoutContent contentToClose)
		{
			if (!contentToClose.CanClose)
			{
				return;
			}
			LayoutItem layoutItemFromModel = this.GetLayoutItemFromModel(contentToClose);
			if (layoutItemFromModel.CloseCommand == null)
			{
				if (contentToClose is LayoutDocument)
				{
					this._ExecuteCloseCommand(contentToClose as LayoutDocument);
					return;
				}
				if (contentToClose is LayoutAnchorable)
				{
					this._ExecuteCloseCommand(contentToClose as LayoutAnchorable);
				}
			}
			else if (layoutItemFromModel.CloseCommand.CanExecute(null))
			{
				layoutItemFromModel.CloseCommand.Execute(null);
				return;
			}
		}

		private static object CoerceAnchorableHeaderTemplateValue(DependencyObject d, object value)
		{
			if (value != null && d.GetValue(DockingManager.AnchorableHeaderTemplateSelectorProperty) != null)
			{
				return null;
			}
			return value;
		}

		private static object CoerceAnchorableTitleTemplateValue(DependencyObject d, object value)
		{
			if (value != null && d.GetValue(DockingManager.AnchorableTitleTemplateSelectorProperty) != null)
			{
				return null;
			}
			return value;
		}

		private static object CoerceDocumentHeaderTemplateSelectorValue(DependencyObject d, object value)
		{
			return value;
		}

		private static object CoerceDocumentHeaderTemplateValue(DependencyObject d, object value)
		{
			if (value != null && d.GetValue(DockingManager.DocumentHeaderTemplateSelectorProperty) != null)
			{
				return null;
			}
			return value;
		}

		private static object CoerceDocumentPaneMenuItemHeaderTemplateSelectorValue(DependencyObject d, object value)
		{
			return value;
		}

		private static object CoerceDocumentPaneMenuItemHeaderTemplateValue(DependencyObject d, object value)
		{
			if (value != null && d.GetValue(DockingManager.DocumentPaneMenuItemHeaderTemplateSelectorProperty) != null)
			{
				return null;
			}
			if (value != null)
			{
				return value;
			}
			return d.GetValue(DockingManager.DocumentHeaderTemplateProperty);
		}

		private static object CoerceDocumentTitleTemplateSelectorValue(DependencyObject d, object value)
		{
			return value;
		}

		private static object CoerceDocumentTitleTemplateValue(DependencyObject d, object value)
		{
			if (value != null && d.GetValue(DockingManager.DocumentTitleTemplateSelectorProperty) != null)
			{
				return null;
			}
			return value;
		}

		private static object CoerceLayoutValue(DependencyObject d, object value)
		{
			if (value == null)
			{
				return new LayoutRoot()
				{
					RootPanel = new LayoutPanel(new LayoutDocumentPaneGroup(new LayoutDocumentPane()))
				};
			}
			((DockingManager)d).OnLayoutChanging(value as LayoutRoot);
			return value;
		}

		private void CollectLayoutItemsDeleted()
		{
			if (this._collectLayoutItemsOperations != null)
			{
				return;
			}
			this._collectLayoutItemsOperations = base.Dispatcher.BeginInvoke(new Action(() => {
				this._collectLayoutItemsOperations = null;
				LayoutItem[] array = (
					from item in this._layoutItems
					where item.LayoutElement.Root != this.Layout
					select item).ToArray<LayoutItem>();
				for (int i = 0; i < (int)array.Length; i++)
				{
					LayoutItem layoutItem = array[i];
					if (layoutItem != null && layoutItem.Model != null)
					{
						UIElement model = layoutItem.Model as UIElement;
					}
					layoutItem.Detach();
					this._layoutItems.Remove(layoutItem);
				}
			}), new object[0]);
		}

		private void CreateAnchorableLayoutItem(LayoutAnchorable contentToAttach)
		{
			if (this._layoutItems.Any<LayoutItem>((LayoutItem item) => item.LayoutElement == contentToAttach))
			{
				return;
			}
			LayoutAnchorableItem layoutAnchorableItem = new LayoutAnchorableItem();
			layoutAnchorableItem.Attach(contentToAttach);
			this.ApplyStyleToLayoutItem(layoutAnchorableItem);
			this._layoutItems.Add(layoutAnchorableItem);
			if (contentToAttach != null && contentToAttach.Content != null && contentToAttach.Content is UIElement)
			{
				this.InternalAddLogicalChild(contentToAttach.Content);
			}
		}

		private void CreateDocumentLayoutItem(LayoutDocument contentToAttach)
		{
			if (this._layoutItems.Any<LayoutItem>((LayoutItem item) => item.LayoutElement == contentToAttach))
			{
				return;
			}
			LayoutDocumentItem layoutDocumentItem = new LayoutDocumentItem();
			layoutDocumentItem.Attach(contentToAttach);
			this.ApplyStyleToLayoutItem(layoutDocumentItem);
			this._layoutItems.Add(layoutDocumentItem);
			if (contentToAttach != null && contentToAttach.Content != null && contentToAttach.Content is UIElement)
			{
				this.InternalAddLogicalChild(contentToAttach.Content);
			}
		}

		private void CreateOverlayWindow()
		{
			if (this._overlayWindow == null)
			{
				this._overlayWindow = new OverlayWindow(this);
			}
			Point point = new Point();
			Rect rect = new Rect(this.PointToScreenDPIWithoutFlowDirection(point), this.TransformActualSizeToAncestor());
			this._overlayWindow.Left = rect.Left;
			this._overlayWindow.Top = rect.Top;
			this._overlayWindow.Width = rect.Width;
			this._overlayWindow.Height = rect.Height;
		}

		internal UIElement CreateUIElementForModel(ILayoutElement model)
		{
			if (model is LayoutPanel)
			{
				return new LayoutPanelControl(model as LayoutPanel);
			}
			if (model is LayoutAnchorablePaneGroup)
			{
				return new LayoutAnchorablePaneGroupControl(model as LayoutAnchorablePaneGroup);
			}
			if (model is LayoutDocumentPaneGroup)
			{
				return new LayoutDocumentPaneGroupControl(model as LayoutDocumentPaneGroup);
			}
			if (model is LayoutAnchorSide)
			{
				LayoutAnchorSideControl layoutAnchorSideControl = new LayoutAnchorSideControl(model as LayoutAnchorSide);
				layoutAnchorSideControl.SetBinding(Control.TemplateProperty, new Binding("AnchorSideTemplate")
				{
					Source = this
				});
				return layoutAnchorSideControl;
			}
			if (model is LayoutAnchorGroup)
			{
				LayoutAnchorGroupControl layoutAnchorGroupControl = new LayoutAnchorGroupControl(model as LayoutAnchorGroup);
				layoutAnchorGroupControl.SetBinding(Control.TemplateProperty, new Binding("AnchorGroupTemplate")
				{
					Source = this
				});
				return layoutAnchorGroupControl;
			}
			if (model is LayoutDocumentPane)
			{
				LayoutDocumentPaneControl layoutDocumentPaneControl = new LayoutDocumentPaneControl(model as LayoutDocumentPane);
				layoutDocumentPaneControl.SetBinding(FrameworkElement.StyleProperty, new Binding("DocumentPaneControlStyle")
				{
					Source = this
				});
				return layoutDocumentPaneControl;
			}
			if (model is LayoutAnchorablePane)
			{
				LayoutAnchorablePaneControl layoutAnchorablePaneControl = new LayoutAnchorablePaneControl(model as LayoutAnchorablePane);
				layoutAnchorablePaneControl.SetBinding(FrameworkElement.StyleProperty, new Binding("AnchorablePaneControlStyle")
				{
					Source = this
				});
				return layoutAnchorablePaneControl;
			}
			if (model is LayoutAnchorableFloatingWindow)
			{
				if (DesignerProperties.GetIsInDesignMode(this))
				{
					return null;
				}
				LayoutAnchorableFloatingWindow layoutAnchorableFloatingWindow = model as LayoutAnchorableFloatingWindow;
				LayoutAnchorableFloatingWindowControl layoutAnchorableFloatingWindowControl = new LayoutAnchorableFloatingWindowControl(layoutAnchorableFloatingWindow);
				layoutAnchorableFloatingWindowControl.SetParentToMainWindowOf(this);
				LayoutAnchorablePane layoutAnchorablePane = layoutAnchorableFloatingWindow.RootPanel.Children.OfType<LayoutAnchorablePane>().FirstOrDefault<LayoutAnchorablePane>();
				if (layoutAnchorablePane != null)
				{
					layoutAnchorablePane.KeepInsideNearestMonitor();
					layoutAnchorableFloatingWindowControl.Left = layoutAnchorablePane.FloatingLeft;
					layoutAnchorableFloatingWindowControl.Top = layoutAnchorablePane.FloatingTop;
					layoutAnchorableFloatingWindowControl.Width = layoutAnchorablePane.FloatingWidth;
					layoutAnchorableFloatingWindowControl.Height = layoutAnchorablePane.FloatingHeight;
				}
				layoutAnchorableFloatingWindowControl.ShowInTaskbar = false;
				layoutAnchorableFloatingWindowControl.Show();
				if (layoutAnchorablePane != null && layoutAnchorablePane.IsMaximized)
				{
					layoutAnchorableFloatingWindowControl.WindowState = WindowState.Maximized;
				}
				return layoutAnchorableFloatingWindowControl;
			}
			if (!(model is LayoutDocumentFloatingWindow))
			{
				if (!(model is LayoutDocument))
				{
					return null;
				}
				return new LayoutDocumentControl()
				{
					Model = model as LayoutDocument
				};
			}
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				return null;
			}
			LayoutDocumentFloatingWindow layoutDocumentFloatingWindow = model as LayoutDocumentFloatingWindow;
			LayoutDocumentFloatingWindowControl layoutDocumentFloatingWindowControl = new LayoutDocumentFloatingWindowControl(layoutDocumentFloatingWindow);
			layoutDocumentFloatingWindowControl.SetParentToMainWindowOf(this);
			LayoutDocument rootDocument = layoutDocumentFloatingWindow.RootDocument;
			if (rootDocument != null)
			{
				rootDocument.KeepInsideNearestMonitor();
				layoutDocumentFloatingWindowControl.Left = rootDocument.FloatingLeft;
				layoutDocumentFloatingWindowControl.Top = rootDocument.FloatingTop;
				layoutDocumentFloatingWindowControl.Width = rootDocument.FloatingWidth;
				layoutDocumentFloatingWindowControl.Height = rootDocument.FloatingHeight;
			}
			layoutDocumentFloatingWindowControl.ShowInTaskbar = false;
			layoutDocumentFloatingWindowControl.Show();
			if (rootDocument != null && rootDocument.IsMaximized)
			{
				layoutDocumentFloatingWindowControl.WindowState = WindowState.Maximized;
			}
			return layoutDocumentFloatingWindowControl;
		}

		private void DestroyOverlayWindow()
		{
			if (this._overlayWindow != null)
			{
				this._overlayWindow.Close();
				this._overlayWindow = null;
			}
		}

		private void DetachAnchorablesSource(LayoutRoot layout, IEnumerable anchorablesSource)
		{
			if (anchorablesSource == null)
			{
				return;
			}
			if (layout == null)
			{
				return;
			}
			LayoutAnchorable[] array = (
				from d in layout.Descendents().OfType<LayoutAnchorable>()
				where anchorablesSource.Contains(d.Content)
				select d).ToArray<LayoutAnchorable>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				LayoutAnchorable layoutAnchorable = array[i];
				layoutAnchorable.Parent.RemoveChild(layoutAnchorable);
				this.RemoveViewFromLogicalChild(layoutAnchorable);
			}
			INotifyCollectionChanged notifyCollectionChanged = anchorablesSource as INotifyCollectionChanged;
			if (notifyCollectionChanged != null)
			{
				notifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.anchorablesSourceElementsChanged);
			}
		}

		private void DetachDocumentsSource(LayoutRoot layout, IEnumerable documentsSource)
		{
			if (documentsSource == null)
			{
				return;
			}
			if (layout == null)
			{
				return;
			}
			LayoutDocument[] array = (
				from d in layout.Descendents().OfType<LayoutDocument>()
				where documentsSource.Contains(d.Content)
				select d).ToArray<LayoutDocument>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				LayoutDocument layoutDocument = array[i];
				layoutDocument.Parent.RemoveChild(layoutDocument);
				this.RemoveViewFromLogicalChild(layoutDocument);
			}
			INotifyCollectionChanged notifyCollectionChanged = documentsSource as INotifyCollectionChanged;
			if (notifyCollectionChanged != null)
			{
				notifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.documentsSourceElementsChanged);
			}
		}

		private void DetachLayoutItems()
		{
			if (this.Layout != null)
			{
				this._layoutItems.ForEach<LayoutItem>((LayoutItem i) => i.Detach());
				this._layoutItems.Clear();
				this.Layout.ElementAdded -= new EventHandler<LayoutElementEventArgs>(this.Layout_ElementAdded);
				this.Layout.ElementRemoved -= new EventHandler<LayoutElementEventArgs>(this.Layout_ElementRemoved);
			}
		}

		private void DockingManager_Loaded(object sender, RoutedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				if (this.Layout.Manager == this)
				{
					this.LayoutRootPanel = this.CreateUIElementForModel(this.Layout.RootPanel) as LayoutPanelControl;
					this.LeftSidePanel = this.CreateUIElementForModel(this.Layout.LeftSide) as LayoutAnchorSideControl;
					this.TopSidePanel = this.CreateUIElementForModel(this.Layout.TopSide) as LayoutAnchorSideControl;
					this.RightSidePanel = this.CreateUIElementForModel(this.Layout.RightSide) as LayoutAnchorSideControl;
					this.BottomSidePanel = this.CreateUIElementForModel(this.Layout.BottomSide) as LayoutAnchorSideControl;
				}
				this.SetupAutoHideWindow();
				foreach (LayoutFloatingWindow layoutFloatingWindow in 
					from fw in this.Layout.FloatingWindows
					where !this._fwList.Any<LayoutFloatingWindowControl>((LayoutFloatingWindowControl fwc) => fwc.Model == fw)
					select fw)
				{
					this._fwList.Add(this.CreateUIElementForModel(layoutFloatingWindow) as LayoutFloatingWindowControl);
				}
				if (base.IsVisible)
				{
					this.CreateOverlayWindow();
				}
				FocusElementManager.SetupFocusManagement(this);
			}
		}

		private void DockingManager_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				if (this._autoHideWindowManager != null)
				{
					this._autoHideWindowManager.HideAutoWindow(null);
				}
				if (this.AutoHideWindow != null)
				{
					this.AutoHideWindow.Dispose();
				}
				LayoutFloatingWindowControl[] array = this._fwList.ToArray();
				for (int i = 0; i < (int)array.Length; i++)
				{
					LayoutFloatingWindowControl layoutFloatingWindowControl = array[i];
					layoutFloatingWindowControl.SetParentWindowToNull();
					layoutFloatingWindowControl.KeepContentVisibleOnClose = true;
					layoutFloatingWindowControl.Close();
				}
				this.DestroyOverlayWindow();
				FocusElementManager.FinalizeFocusManagement(this);
			}
		}

		private void documentsSourceElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			LayoutDocument[] array;
			int i;
			if (this.Layout == null)
			{
				return;
			}
			if (this.SuspendDocumentsSourceBinding)
			{
				return;
			}
			if ((e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace) && e.OldItems != null)
			{
				array = (
					from d in this.Layout.Descendents().OfType<LayoutDocument>()
					where e.OldItems.Contains(d.Content)
					select d).ToArray<LayoutDocument>();
				for (i = 0; i < (int)array.Length; i++)
				{
					LayoutDocument layoutDocument = array[i];
					layoutDocument.Parent.RemoveChild(layoutDocument);
					this.RemoveViewFromLogicalChild(layoutDocument);
				}
			}
			if (e.NewItems != null && (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace) && e.NewItems != null)
			{
				LayoutDocumentPane parent = null;
				if (this.Layout.LastFocusedDocument != null)
				{
					parent = this.Layout.LastFocusedDocument.Parent as LayoutDocumentPane;
				}
				if (parent == null)
				{
					parent = this.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>();
				}
				this._suspendLayoutItemCreation = true;
				foreach (object newItem in e.NewItems)
				{
					LayoutDocument layoutDocument1 = new LayoutDocument()
					{
						Content = newItem
					};
					bool flag = false;
					if (this.LayoutUpdateStrategy != null)
					{
						flag = this.LayoutUpdateStrategy.BeforeInsertDocument(this.Layout, layoutDocument1, parent);
					}
					if (!flag)
					{
						if (parent == null)
						{
							throw new InvalidOperationException("Layout must contains at least one LayoutDocumentPane in order to host documents");
						}
						parent.Children.Add(layoutDocument1);
						flag = true;
					}
					if (this.LayoutUpdateStrategy != null)
					{
						this.LayoutUpdateStrategy.AfterInsertDocument(this.Layout, layoutDocument1);
					}
					ILayoutRoot root = layoutDocument1.Root;
					if (root == null || root.Manager != this)
					{
						continue;
					}
					this.CreateDocumentLayoutItem(layoutDocument1);
				}
				this._suspendLayoutItemCreation = false;
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				array = this.Layout.Descendents().OfType<LayoutDocument>().ToArray<LayoutDocument>();
				for (i = 0; i < (int)array.Length; i++)
				{
					LayoutDocument layoutDocument2 = array[i];
					layoutDocument2.Parent.RemoveChild(layoutDocument2);
					this.RemoveViewFromLogicalChild(layoutDocument2);
				}
			}
			if (this.Layout != null)
			{
				this.Layout.CollectGarbage();
			}
		}

		internal FrameworkElement GetAutoHideAreaElement()
		{
			return this._autohideArea;
		}

		internal IEnumerable<LayoutFloatingWindowControl> GetFloatingWindowsByZOrder()
		{
			Func<LayoutFloatingWindowControl, bool> func = null;
			Window window = Window.GetWindow(this);
			if (window == null)
			{
				yield break;
			}
			IntPtr handle = (new WindowInteropHelper(window)).Handle;
			for (IntPtr i = Win32Helper.GetWindow(handle, 0); i != IntPtr.Zero; i = Win32Helper.GetWindow(i, 2))
			{
				List<LayoutFloatingWindowControl> layoutFloatingWindowControls = this._fwList;
				Func<LayoutFloatingWindowControl, bool> func1 = func;
				if (func1 == null)
				{
					Func<LayoutFloatingWindowControl, bool> handle1 = (LayoutFloatingWindowControl fw) => (new WindowInteropHelper(fw)).Handle == i;
					Func<LayoutFloatingWindowControl, bool> func2 = handle1;
					func = handle1;
					func1 = func2;
				}
				LayoutFloatingWindowControl layoutFloatingWindowControl = layoutFloatingWindowControls.FirstOrDefault<LayoutFloatingWindowControl>(func1);
				if (layoutFloatingWindowControl != null && layoutFloatingWindowControl.Model.Root.Manager == this)
				{
					yield return layoutFloatingWindowControl;
				}
			}
		}

		public LayoutItem GetLayoutItemFromModel(LayoutContent content)
		{
			return this._layoutItems.FirstOrDefault<LayoutItem>((LayoutItem item) => item.LayoutElement == content);
		}

		internal void HideAutoHideWindow(LayoutAnchorControl anchor)
		{
			this._autoHideWindowManager.HideAutoWindow(anchor);
		}

		internal void InternalAddLogicalChild(object element)
		{
			if ((
				from ch in this._logicalChildren
				select ch.GetValueOrDefault<object>()).Contains(element))
			{
				return;
			}
			this._logicalChildren.Add(new WeakReference(element));
			base.AddLogicalChild(element);
		}

		internal void InternalRemoveLogicalChild(object element)
		{
			WeakReference weakReference = this._logicalChildren.FirstOrDefault<WeakReference>((WeakReference ch) => ch.GetValueOrDefault<object>() == element);
			if (weakReference != null)
			{
				this._logicalChildren.Remove(weakReference);
			}
			base.RemoveLogicalChild(element);
		}

		private void InternalSetActiveContent(object contentObject)
		{
			LayoutContent layoutContent = this.Layout.Descendents().OfType<LayoutContent>().FirstOrDefault<LayoutContent>((LayoutContent lc) => {
				if (lc == contentObject)
				{
					return true;
				}
				return lc.Content == contentObject;
			});
			this._insideInternalSetActiveContent = true;
			this.Layout.ActiveContent = layoutContent;
			this._insideInternalSetActiveContent = false;
		}

		private void Layout_ElementAdded(object sender, LayoutElementEventArgs e)
		{
			if (this._suspendLayoutItemCreation)
			{
				return;
			}
			foreach (LayoutContent layoutContent in this.Layout.Descendents().OfType<LayoutContent>())
			{
				if (!(layoutContent is LayoutDocument))
				{
					this.CreateAnchorableLayoutItem(layoutContent as LayoutAnchorable);
				}
				else
				{
					this.CreateDocumentLayoutItem(layoutContent as LayoutDocument);
				}
			}
			this.CollectLayoutItemsDeleted();
		}

		private void Layout_ElementRemoved(object sender, LayoutElementEventArgs e)
		{
			if (this._suspendLayoutItemCreation)
			{
				return;
			}
			this.CollectLayoutItemsDeleted();
		}

		private static void OnActiveContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).InternalSetActiveContent(e.NewValue);
			((DockingManager)d).OnActiveContentChanged(e);
		}

		protected virtual void OnActiveContentChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.ActiveContentChanged != null)
			{
				this.ActiveContentChanged(this, EventArgs.Empty);
			}
		}

		private static void OnAnchorableHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnAnchorableHeaderTemplateChanged(e);
		}

		protected virtual void OnAnchorableHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnAnchorableHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnAnchorableHeaderTemplateSelectorChanged(e);
		}

		protected virtual void OnAnchorableHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null)
			{
				this.AnchorableHeaderTemplate = null;
			}
		}

		private static void OnAnchorablePaneControlStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnAnchorablePaneControlStyleChanged(e);
		}

		protected virtual void OnAnchorablePaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnAnchorablePaneTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnAnchorablePaneTemplateChanged(e);
		}

		protected virtual void OnAnchorablePaneTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnAnchorablesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnAnchorablesSourceChanged(e);
		}

		protected virtual void OnAnchorablesSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			this.DetachAnchorablesSource(this.Layout, e.OldValue as IEnumerable);
			this.AttachAnchorablesSource(this.Layout, e.NewValue as IEnumerable);
		}

		private static void OnAnchorableTitleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnAnchorableTitleTemplateChanged(e);
		}

		protected virtual void OnAnchorableTitleTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnAnchorableTitleTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnAnchorableTitleTemplateSelectorChanged(e);
		}

		protected virtual void OnAnchorableTitleTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null && this.AnchorableTitleTemplate != null)
			{
				this.AnchorableTitleTemplate = null;
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
		}

		private static void OnAutoHideWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnAutoHideWindowChanged(e);
		}

		protected virtual void OnAutoHideWindowChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
			{
				this.InternalRemoveLogicalChild(e.OldValue);
			}
			if (e.NewValue != null)
			{
				this.InternalAddLogicalChild(e.NewValue);
			}
		}

		private static void OnBottomSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnBottomSidePanelChanged(e);
		}

		protected virtual void OnBottomSidePanelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
			{
				this.InternalRemoveLogicalChild(e.OldValue);
			}
			if (e.NewValue != null)
			{
				this.InternalAddLogicalChild(e.NewValue);
			}
		}

		private static void OnDocumentHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnDocumentHeaderTemplateChanged(e);
		}

		protected virtual void OnDocumentHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnDocumentHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnDocumentHeaderTemplateSelectorChanged(e);
		}

		protected virtual void OnDocumentHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null && this.DocumentHeaderTemplate != null)
			{
				this.DocumentHeaderTemplate = null;
			}
			if (this.DocumentPaneMenuItemHeaderTemplateSelector == null)
			{
				this.DocumentPaneMenuItemHeaderTemplateSelector = this.DocumentHeaderTemplateSelector;
			}
		}

		private static void OnDocumentPaneControlStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnDocumentPaneControlStyleChanged(e);
		}

		protected virtual void OnDocumentPaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnDocumentPaneMenuItemHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnDocumentPaneMenuItemHeaderTemplateChanged(e);
		}

		protected virtual void OnDocumentPaneMenuItemHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(e);
		}

		protected virtual void OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null && this.DocumentPaneMenuItemHeaderTemplate != null)
			{
				this.DocumentPaneMenuItemHeaderTemplate = null;
			}
		}

		private static void OnDocumentPaneTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnDocumentPaneTemplateChanged(e);
		}

		protected virtual void OnDocumentPaneTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnDocumentsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnDocumentsSourceChanged(e);
		}

		protected virtual void OnDocumentsSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			this.DetachDocumentsSource(this.Layout, e.OldValue as IEnumerable);
			this.AttachDocumentsSource(this.Layout, e.NewValue as IEnumerable);
		}

		private static void OnDocumentTitleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnDocumentTitleTemplateChanged(e);
		}

		protected virtual void OnDocumentTitleTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnDocumentTitleTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnDocumentTitleTemplateSelectorChanged(e);
		}

		protected virtual void OnDocumentTitleTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null)
			{
				this.DocumentTitleTemplate = null;
			}
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
		}

		private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnLayoutChanged(e.OldValue as LayoutRoot, e.NewValue as LayoutRoot);
		}

		protected virtual void OnLayoutChanged(LayoutRoot oldLayout, LayoutRoot newLayout)
		{
			int i;
			if (oldLayout != null)
			{
				oldLayout.PropertyChanged -= new PropertyChangedEventHandler(this.OnLayoutRootPropertyChanged);
				oldLayout.Updated -= new EventHandler(this.OnLayoutRootUpdated);
			}
			LayoutFloatingWindowControl[] array = this._fwList.ToArray();
			for (i = 0; i < (int)array.Length; i++)
			{
				LayoutFloatingWindowControl layoutFloatingWindowControl = array[i];
				layoutFloatingWindowControl.KeepContentVisibleOnClose = true;
				layoutFloatingWindowControl.InternalClose();
			}
			this._fwList.Clear();
			this.DetachDocumentsSource(oldLayout, this.DocumentsSource);
			this.DetachAnchorablesSource(oldLayout, this.AnchorablesSource);
			if (oldLayout != null && oldLayout.Manager == this)
			{
				oldLayout.Manager = null;
			}
			this.ClearLogicalChildrenList();
			this.DetachLayoutItems();
			this.Layout.Manager = this;
			this.AttachLayoutItems();
			this.AttachDocumentsSource(newLayout, this.DocumentsSource);
			this.AttachAnchorablesSource(newLayout, this.AnchorablesSource);
			if (base.IsLoaded)
			{
				this.LayoutRootPanel = this.CreateUIElementForModel(this.Layout.RootPanel) as LayoutPanelControl;
				this.LeftSidePanel = this.CreateUIElementForModel(this.Layout.LeftSide) as LayoutAnchorSideControl;
				this.TopSidePanel = this.CreateUIElementForModel(this.Layout.TopSide) as LayoutAnchorSideControl;
				this.RightSidePanel = this.CreateUIElementForModel(this.Layout.RightSide) as LayoutAnchorSideControl;
				this.BottomSidePanel = this.CreateUIElementForModel(this.Layout.BottomSide) as LayoutAnchorSideControl;
				LayoutFloatingWindow[] layoutFloatingWindowArray = this.Layout.FloatingWindows.ToArray<LayoutFloatingWindow>();
				for (i = 0; i < (int)layoutFloatingWindowArray.Length; i++)
				{
					LayoutFloatingWindow layoutFloatingWindow = layoutFloatingWindowArray[i];
					if (layoutFloatingWindow.IsValid)
					{
						this._fwList.Add(this.CreateUIElementForModel(layoutFloatingWindow) as LayoutFloatingWindowControl);
					}
				}
				foreach (LayoutFloatingWindowControl layoutFloatingWindowControl1 in this._fwList)
				{
				}
			}
			if (newLayout != null)
			{
				newLayout.PropertyChanged += new PropertyChangedEventHandler(this.OnLayoutRootPropertyChanged);
				newLayout.Updated += new EventHandler(this.OnLayoutRootUpdated);
			}
			if (this.LayoutChanged != null)
			{
				this.LayoutChanged(this, EventArgs.Empty);
			}
			CommandManager.InvalidateRequerySuggested();
		}

		private void OnLayoutChanging(LayoutRoot newLayout)
		{
			if (this.LayoutChanging != null)
			{
				this.LayoutChanging(this, EventArgs.Empty);
			}
		}

		private static void OnLayoutItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnLayoutItemContainerStyleChanged(e);
		}

		protected virtual void OnLayoutItemContainerStyleChanged(DependencyPropertyChangedEventArgs e)
		{
			this.AttachLayoutItems();
		}

		private static void OnLayoutItemContainerStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnLayoutItemContainerStyleSelectorChanged(e);
		}

		protected virtual void OnLayoutItemContainerStyleSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
			this.AttachLayoutItems();
		}

		private static void OnLayoutItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnLayoutItemTemplateChanged(e);
		}

		protected virtual void OnLayoutItemTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnLayoutItemTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnLayoutItemTemplateSelectorChanged(e);
		}

		protected virtual void OnLayoutItemTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnLayoutRootPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnLayoutRootPanelChanged(e);
		}

		protected virtual void OnLayoutRootPanelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
			{
				this.InternalRemoveLogicalChild(e.OldValue);
			}
			if (e.NewValue != null)
			{
				this.InternalAddLogicalChild(e.NewValue);
			}
		}

		private void OnLayoutRootPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "RootPanel")
			{
				if (base.IsInitialized)
				{
					LayoutPanelControl layoutPanelControl = this.CreateUIElementForModel(this.Layout.RootPanel) as LayoutPanelControl;
					this.LayoutRootPanel = layoutPanelControl;
					return;
				}
			}
			else if (e.PropertyName == "ActiveContent")
			{
				if (this.Layout.ActiveContent != null && this.Layout.ActiveContent != null)
				{
					FocusElementManager.SetFocusOnLastElement(this.Layout.ActiveContent);
				}
				if (!this._insideInternalSetActiveContent && this.Layout.ActiveContent != null)
				{
					this.ActiveContent = this.Layout.ActiveContent.Content;
				}
			}
		}

		private void OnLayoutRootUpdated(object sender, EventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
		}

		private static void OnLeftSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnLeftSidePanelChanged(e);
		}

		protected virtual void OnLeftSidePanelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
			{
				this.InternalRemoveLogicalChild(e.OldValue);
			}
			if (e.NewValue != null)
			{
				this.InternalAddLogicalChild(e.NewValue);
			}
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && e.IsDown && e.Key == Key.Tab && !this.IsNavigatorWindowActive)
			{
				this.ShowNavigatorWindow();
				e.Handled = true;
			}
			base.OnPreviewKeyDown(e);
		}

		private static void OnRightSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnRightSidePanelChanged(e);
		}

		protected virtual void OnRightSidePanelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
			{
				this.InternalRemoveLogicalChild(e.OldValue);
			}
			if (e.NewValue != null)
			{
				this.InternalAddLogicalChild(e.NewValue);
			}
		}

		private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnThemeChanged(e);
		}

		protected virtual void OnThemeChanged(DependencyPropertyChangedEventArgs e)
		{
			Xceed.Wpf.AvalonDock.Themes.Theme oldValue = e.OldValue as Xceed.Wpf.AvalonDock.Themes.Theme;
			Xceed.Wpf.AvalonDock.Themes.Theme newValue = e.NewValue as Xceed.Wpf.AvalonDock.Themes.Theme;
			ResourceDictionary resources = base.Resources;
			if (oldValue != null)
			{
				if (!(oldValue is DictionaryTheme))
				{
					ResourceDictionary resourceDictionaries = resources.MergedDictionaries.FirstOrDefault<ResourceDictionary>((ResourceDictionary r) => r.Source == oldValue.GetResourceUri());
					if (resourceDictionaries != null)
					{
						resources.MergedDictionaries.Remove(resourceDictionaries);
					}
				}
				else if (this.currentThemeResourceDictionary != null)
				{
					resources.MergedDictionaries.Remove(this.currentThemeResourceDictionary);
					this.currentThemeResourceDictionary = null;
				}
			}
			if (newValue != null)
			{
				if (!(newValue is DictionaryTheme))
				{
					resources.MergedDictionaries.Add(new ResourceDictionary()
					{
						Source = newValue.GetResourceUri()
					});
				}
				else
				{
					this.currentThemeResourceDictionary = ((DictionaryTheme)newValue).ThemeResourceDictionary;
					resources.MergedDictionaries.Add(this.currentThemeResourceDictionary);
				}
			}
			foreach (LayoutFloatingWindowControl layoutFloatingWindowControl in this._fwList)
			{
				layoutFloatingWindowControl.UpdateThemeResources(oldValue);
			}
			if (this._navigatorWindow != null)
			{
				this._navigatorWindow.UpdateThemeResources(null);
			}
			if (this._overlayWindow != null)
			{
				this._overlayWindow.UpdateThemeResources(null);
			}
		}

		private static void OnTopSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager)d).OnTopSidePanelChanged(e);
		}

		protected virtual void OnTopSidePanelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
			{
				this.InternalRemoveLogicalChild(e.OldValue);
			}
			if (e.NewValue != null)
			{
				this.InternalAddLogicalChild(e.NewValue);
			}
		}

		internal void RemoveFloatingWindow(LayoutFloatingWindowControl floatingWindow)
		{
			this._fwList.Remove(floatingWindow);
		}

		private void RemoveViewFromLogicalChild(LayoutContent layoutContent)
		{
			if (layoutContent == null)
			{
				return;
			}
			LayoutItem layoutItemFromModel = this.GetLayoutItemFromModel(layoutContent);
			if (layoutItemFromModel != null)
			{
				this.InternalRemoveLogicalChild(layoutItemFromModel.View);
			}
		}

		protected void SetAutoHideWindow(LayoutAutoHideWindowControl value)
		{
			base.SetValue(DockingManager.AutoHideWindowPropertyKey, value);
		}

		private void SetupAutoHideWindow()
		{
			this._autohideArea = base.GetTemplateChild("PART_AutoHideArea") as FrameworkElement;
			if (this._autoHideWindowManager == null)
			{
				this._autoHideWindowManager = new AutoHideWindowManager(this);
			}
			else
			{
				this._autoHideWindowManager.HideAutoWindow(null);
			}
			if (this.AutoHideWindow != null)
			{
				this.AutoHideWindow.Dispose();
			}
			this.SetAutoHideWindow(new LayoutAutoHideWindowControl());
		}

		internal void ShowAutoHideWindow(LayoutAnchorControl anchor)
		{
			this._autoHideWindowManager.ShowAutoHideWindow(anchor);
		}

		private void ShowNavigatorWindow()
		{
			if (this._navigatorWindow == null)
			{
				this._navigatorWindow = new NavigatorWindow(this)
				{
					Owner = Window.GetWindow(this),
					WindowStartupLocation = WindowStartupLocation.CenterOwner
				};
			}
			this._navigatorWindow.ShowDialog();
			this._navigatorWindow = null;
		}

		internal void StartDraggingFloatingWindowForContent(LayoutContent contentModel, bool startDrag = true)
		{
			LayoutFloatingWindow layoutDocumentFloatingWindow;
			LayoutFloatingWindowControl layoutDocumentFloatingWindowControl;
			if (!contentModel.CanFloat)
			{
				return;
			}
			LayoutAnchorable layoutAnchorable = contentModel as LayoutAnchorable;
			if (layoutAnchorable != null && layoutAnchorable.IsAutoHidden)
			{
				layoutAnchorable.ToggleAutoHide();
			}
			ILayoutPane parent = contentModel.Parent as ILayoutPane;
			ILayoutPositionableElement layoutPositionableElement = contentModel.Parent as ILayoutPositionableElement;
			ILayoutPositionableElementWithActualSize layoutPositionableElementWithActualSize = contentModel.Parent as ILayoutPositionableElementWithActualSize;
			int num = parent.Children.ToList<ILayoutElement>().IndexOf(contentModel);
			if (contentModel.FindParent<LayoutFloatingWindow>() == null)
			{
				((ILayoutPreviousContainer)contentModel).PreviousContainer = parent;
				contentModel.PreviousContainerIndex = num;
			}
			parent.RemoveChildAt(num);
			double floatingWidth = contentModel.FloatingWidth;
			double floatingHeight = contentModel.FloatingHeight;
			if (floatingWidth == 0)
			{
				floatingWidth = layoutPositionableElement.FloatingWidth;
			}
			if (floatingHeight == 0)
			{
				floatingHeight = layoutPositionableElement.FloatingHeight;
			}
			if (floatingWidth == 0)
			{
				floatingWidth = layoutPositionableElementWithActualSize.ActualWidth;
			}
			if (floatingHeight == 0)
			{
				floatingHeight = layoutPositionableElementWithActualSize.ActualHeight;
			}
			if (!(contentModel is LayoutAnchorable))
			{
				layoutDocumentFloatingWindow = new LayoutDocumentFloatingWindow()
				{
					RootDocument = contentModel as LayoutDocument
				};
				this.Layout.FloatingWindows.Add(layoutDocumentFloatingWindow);
				layoutDocumentFloatingWindowControl = new LayoutDocumentFloatingWindowControl(layoutDocumentFloatingWindow as LayoutDocumentFloatingWindow)
				{
					Width = floatingWidth,
					Height = floatingHeight,
					Left = contentModel.FloatingLeft,
					Top = contentModel.FloatingTop
				};
			}
			else
			{
				LayoutAnchorable layoutAnchorable1 = contentModel as LayoutAnchorable;
				layoutDocumentFloatingWindow = new LayoutAnchorableFloatingWindow()
				{
					RootPanel = new LayoutAnchorablePaneGroup(new LayoutAnchorablePane(layoutAnchorable1)
					{
						DockWidth = layoutPositionableElement.DockWidth,
						DockHeight = layoutPositionableElement.DockHeight,
						DockMinHeight = layoutPositionableElement.DockMinHeight,
						DockMinWidth = layoutPositionableElement.DockMinWidth,
						FloatingLeft = layoutPositionableElement.FloatingLeft,
						FloatingTop = layoutPositionableElement.FloatingTop,
						FloatingWidth = layoutPositionableElement.FloatingWidth,
						FloatingHeight = layoutPositionableElement.FloatingHeight
					})
				};
				this.Layout.FloatingWindows.Add(layoutDocumentFloatingWindow);
				layoutDocumentFloatingWindowControl = new LayoutAnchorableFloatingWindowControl(layoutDocumentFloatingWindow as LayoutAnchorableFloatingWindow)
				{
					Width = floatingWidth,
					Height = floatingHeight,
					Left = contentModel.FloatingLeft,
					Top = contentModel.FloatingTop
				};
			}
			this._fwList.Add(layoutDocumentFloatingWindowControl);
			this.Layout.CollectGarbage();
			base.UpdateLayout();
			base.Dispatcher.BeginInvoke(new Action(() => {
				if (startDrag)
				{
					layoutDocumentFloatingWindowControl.AttachDrag(true);
				}
				layoutDocumentFloatingWindowControl.Show();
			}), DispatcherPriority.Send, new object[0]);
		}

		internal void StartDraggingFloatingWindowForPane(LayoutAnchorablePane paneModel)
		{
			if (paneModel.Children.Any<LayoutAnchorable>((LayoutAnchorable c) => !c.CanFloat))
			{
				return;
			}
			ILayoutPositionableElement layoutPositionableElement = paneModel;
			ILayoutPositionableElementWithActualSize layoutPositionableElementWithActualSize = paneModel;
			double floatingWidth = layoutPositionableElement.FloatingWidth;
			double floatingHeight = layoutPositionableElement.FloatingHeight;
			double floatingLeft = layoutPositionableElement.FloatingLeft;
			double floatingTop = layoutPositionableElement.FloatingTop;
			if (floatingWidth == 0)
			{
				floatingWidth = layoutPositionableElementWithActualSize.ActualWidth;
			}
			if (floatingHeight == 0)
			{
				floatingHeight = layoutPositionableElementWithActualSize.ActualHeight;
			}
			LayoutAnchorablePane layoutAnchorablePane = new LayoutAnchorablePane()
			{
				DockWidth = layoutPositionableElement.DockWidth,
				DockHeight = layoutPositionableElement.DockHeight,
				DockMinHeight = layoutPositionableElement.DockMinHeight,
				DockMinWidth = layoutPositionableElement.DockMinWidth,
				FloatingLeft = layoutPositionableElement.FloatingLeft,
				FloatingTop = layoutPositionableElement.FloatingTop,
				FloatingWidth = layoutPositionableElement.FloatingWidth,
				FloatingHeight = layoutPositionableElement.FloatingHeight
			};
			bool flag = paneModel.FindParent<LayoutFloatingWindow>() == null;
			int selectedContentIndex = paneModel.SelectedContentIndex;
			while (paneModel.Children.Count > 0)
			{
				LayoutAnchorable item = paneModel.Children[paneModel.Children.Count - 1];
				if (flag)
				{
					((ILayoutPreviousContainer)item).PreviousContainer = paneModel;
					item.PreviousContainerIndex = paneModel.Children.Count - 1;
				}
				paneModel.RemoveChildAt(paneModel.Children.Count - 1);
				layoutAnchorablePane.Children.Insert(0, item);
			}
			if (layoutAnchorablePane.Children.Count > 0)
			{
				layoutAnchorablePane.SelectedContentIndex = selectedContentIndex;
			}
			LayoutFloatingWindow layoutAnchorableFloatingWindow = new LayoutAnchorableFloatingWindow()
			{
				RootPanel = new LayoutAnchorablePaneGroup(layoutAnchorablePane)
				{
					DockHeight = layoutAnchorablePane.DockHeight,
					DockWidth = layoutAnchorablePane.DockWidth,
					DockMinHeight = layoutAnchorablePane.DockMinHeight,
					DockMinWidth = layoutAnchorablePane.DockMinWidth
				}
			};
			this.Layout.FloatingWindows.Add(layoutAnchorableFloatingWindow);
			LayoutFloatingWindowControl layoutAnchorableFloatingWindowControl = new LayoutAnchorableFloatingWindowControl(layoutAnchorableFloatingWindow as LayoutAnchorableFloatingWindow)
			{
				Width = floatingWidth,
				Height = floatingHeight
			};
			this._fwList.Add(layoutAnchorableFloatingWindowControl);
			this.Layout.CollectGarbage();
			base.InvalidateArrange();
			layoutAnchorableFloatingWindowControl.AttachDrag(true);
			layoutAnchorableFloatingWindowControl.Show();
		}

		IEnumerable<IDropArea> Xceed.Wpf.AvalonDock.Controls.IOverlayWindowHost.GetDropAreas(LayoutFloatingWindowControl draggingWindow)
		{
			if (this._areas != null)
			{
				return this._areas;
			}
			this._areas = new List<IDropArea>();
			if (!(draggingWindow.Model is LayoutDocumentFloatingWindow))
			{
				this._areas.Add(new DropArea<DockingManager>(this, DropAreaType.DockingManager));
				foreach (LayoutAnchorablePaneControl layoutAnchorablePaneControl in this.FindVisualChildren<LayoutAnchorablePaneControl>())
				{
					if (!layoutAnchorablePaneControl.Model.Descendents().Any<ILayoutElement>())
					{
						continue;
					}
					this._areas.Add(new DropArea<LayoutAnchorablePaneControl>(layoutAnchorablePaneControl, DropAreaType.AnchorablePane));
				}
			}
			foreach (LayoutDocumentPaneControl layoutDocumentPaneControl in this.FindVisualChildren<LayoutDocumentPaneControl>())
			{
				this._areas.Add(new DropArea<LayoutDocumentPaneControl>(layoutDocumentPaneControl, DropAreaType.DocumentPane));
			}
			foreach (LayoutDocumentPaneGroupControl layoutDocumentPaneGroupControl in this.FindVisualChildren<LayoutDocumentPaneGroupControl>())
			{
				if ((
					from c in (layoutDocumentPaneGroupControl.Model as LayoutDocumentPaneGroup).Children
					where c.IsVisible
					select c).Count<ILayoutDocumentPane>() != 0)
				{
					continue;
				}
				this._areas.Add(new DropArea<LayoutDocumentPaneGroupControl>(layoutDocumentPaneGroupControl, DropAreaType.DocumentPaneGroup));
			}
			return this._areas;
		}

		void Xceed.Wpf.AvalonDock.Controls.IOverlayWindowHost.HideOverlayWindow()
		{
			this._areas = null;
			this._overlayWindow.Owner = null;
			this._overlayWindow.HideDropTargets();
		}

		bool Xceed.Wpf.AvalonDock.Controls.IOverlayWindowHost.HitTest(Point dragPoint)
		{
			Point point = new Point();
			Rect rect = new Rect(this.PointToScreenDPIWithoutFlowDirection(point), this.TransformActualSizeToAncestor());
			return rect.Contains(dragPoint);
		}

		IOverlayWindow Xceed.Wpf.AvalonDock.Controls.IOverlayWindowHost.ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow)
		{
			this.CreateOverlayWindow();
			this._overlayWindow.Owner = draggingWindow;
			this._overlayWindow.EnableDropTargets();
			this._overlayWindow.Show();
			return this._overlayWindow;
		}

		public event EventHandler ActiveContentChanged;

		public event EventHandler<DocumentClosedEventArgs> DocumentClosed;

		public event EventHandler<DocumentClosingEventArgs> DocumentClosing;

		public event EventHandler LayoutChanged;

		public event EventHandler LayoutChanging;
	}
}