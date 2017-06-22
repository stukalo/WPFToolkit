using Standard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Windows.Shell
{
	public class WindowChrome : Freezable
	{
		public readonly static DependencyProperty WindowChromeProperty;

		public readonly static DependencyProperty IsHitTestVisibleInChromeProperty;

		public readonly static DependencyProperty CaptionHeightProperty;

		public readonly static DependencyProperty ResizeBorderThicknessProperty;

		public readonly static DependencyProperty GlassFrameThicknessProperty;

		public readonly static DependencyProperty CornerRadiusProperty;

		private readonly static List<WindowChrome._SystemParameterBoundProperty> _BoundProperties;

		public double CaptionHeight
		{
			get
			{
				return (double)base.GetValue(WindowChrome.CaptionHeightProperty);
			}
			set
			{
				base.SetValue(WindowChrome.CaptionHeightProperty, value);
			}
		}

		public System.Windows.CornerRadius CornerRadius
		{
			get
			{
				return (System.Windows.CornerRadius)base.GetValue(WindowChrome.CornerRadiusProperty);
			}
			set
			{
				base.SetValue(WindowChrome.CornerRadiusProperty, value);
			}
		}

		public static Thickness GlassFrameCompleteThickness
		{
			get
			{
				return new Thickness(-1);
			}
		}

		public Thickness GlassFrameThickness
		{
			get
			{
				return (Thickness)base.GetValue(WindowChrome.GlassFrameThicknessProperty);
			}
			set
			{
				base.SetValue(WindowChrome.GlassFrameThicknessProperty, value);
			}
		}

		public Thickness ResizeBorderThickness
		{
			get
			{
				return (Thickness)base.GetValue(WindowChrome.ResizeBorderThicknessProperty);
			}
			set
			{
				base.SetValue(WindowChrome.ResizeBorderThicknessProperty, value);
			}
		}

		public bool ShowSystemMenu
		{
			get;
			set;
		}

		static WindowChrome()
		{
			WindowChrome.WindowChromeProperty = DependencyProperty.RegisterAttached("WindowChrome", typeof(WindowChrome), typeof(WindowChrome), new PropertyMetadata(null, new PropertyChangedCallback(WindowChrome._OnChromeChanged)));
			WindowChrome.IsHitTestVisibleInChromeProperty = DependencyProperty.RegisterAttached("IsHitTestVisibleInChrome", typeof(bool), typeof(WindowChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
			WindowChrome.CaptionHeightProperty = DependencyProperty.Register("CaptionHeight", typeof(double), typeof(WindowChrome), new PropertyMetadata((object)0, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((WindowChrome)d)._OnPropertyChangedThatRequiresRepaint()), (object value) => (double)value >= 0);
			Type type = typeof(Thickness);
			Type type1 = typeof(WindowChrome);
			Thickness thickness = new Thickness();
			WindowChrome.ResizeBorderThicknessProperty = DependencyProperty.Register("ResizeBorderThickness", type, type1, new PropertyMetadata((object)thickness), (object value) => Utility.IsThicknessNonNegative((Thickness)value));
			Type type2 = typeof(Thickness);
			Type type3 = typeof(WindowChrome);
			thickness = new Thickness();
			WindowChrome.GlassFrameThicknessProperty = DependencyProperty.Register("GlassFrameThickness", type2, type3, new PropertyMetadata((object)thickness, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((WindowChrome)d)._OnPropertyChangedThatRequiresRepaint(), (DependencyObject d, object o) => WindowChrome._CoerceGlassFrameThickness((Thickness)o)));
			Type type4 = typeof(System.Windows.CornerRadius);
			Type type5 = typeof(WindowChrome);
			System.Windows.CornerRadius cornerRadiu = new System.Windows.CornerRadius();
			WindowChrome.CornerRadiusProperty = DependencyProperty.Register("CornerRadius", type4, type5, new PropertyMetadata((object)cornerRadiu, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((WindowChrome)d)._OnPropertyChangedThatRequiresRepaint()), (object value) => Utility.IsCornerRadiusValid((System.Windows.CornerRadius)value));
			List<WindowChrome._SystemParameterBoundProperty> _SystemParameterBoundProperties = new List<WindowChrome._SystemParameterBoundProperty>();
			WindowChrome._SystemParameterBoundProperty __SystemParameterBoundProperty = new WindowChrome._SystemParameterBoundProperty()
			{
				DependencyProperty = WindowChrome.CornerRadiusProperty,
				SystemParameterPropertyName = "WindowCornerRadius"
			};
			_SystemParameterBoundProperties.Add(__SystemParameterBoundProperty);
			__SystemParameterBoundProperty = new WindowChrome._SystemParameterBoundProperty()
			{
				DependencyProperty = WindowChrome.CaptionHeightProperty,
				SystemParameterPropertyName = "WindowCaptionHeight"
			};
			_SystemParameterBoundProperties.Add(__SystemParameterBoundProperty);
			__SystemParameterBoundProperty = new WindowChrome._SystemParameterBoundProperty()
			{
				DependencyProperty = WindowChrome.ResizeBorderThicknessProperty,
				SystemParameterPropertyName = "WindowResizeBorderThickness"
			};
			_SystemParameterBoundProperties.Add(__SystemParameterBoundProperty);
			__SystemParameterBoundProperty = new WindowChrome._SystemParameterBoundProperty()
			{
				DependencyProperty = WindowChrome.GlassFrameThicknessProperty,
				SystemParameterPropertyName = "WindowNonClientFrameThickness"
			};
			_SystemParameterBoundProperties.Add(__SystemParameterBoundProperty);
			WindowChrome._BoundProperties = _SystemParameterBoundProperties;
		}

		public WindowChrome()
		{
			foreach (WindowChrome._SystemParameterBoundProperty _BoundProperty in WindowChrome._BoundProperties)
			{
				DependencyProperty dependencyProperty = _BoundProperty.DependencyProperty;
				Binding binding = new Binding()
				{
					Source = SystemParameters2.Current,
					Path = new PropertyPath(_BoundProperty.SystemParameterPropertyName, new object[0]),
					Mode = BindingMode.OneWay,
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				BindingOperations.SetBinding(this, dependencyProperty, binding);
			}
		}

		private static object _CoerceGlassFrameThickness(Thickness thickness)
		{
			if (!Utility.IsThicknessNonNegative(thickness))
			{
				return WindowChrome.GlassFrameCompleteThickness;
			}
			return thickness;
		}

		private static void _OnChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(d))
			{
				return;
			}
			Window window = (Window)d;
			WindowChrome newValue = (WindowChrome)e.NewValue;
			WindowChromeWorker windowChromeWorker = WindowChromeWorker.GetWindowChromeWorker(window);
			if (windowChromeWorker == null)
			{
				windowChromeWorker = new WindowChromeWorker();
				WindowChromeWorker.SetWindowChromeWorker(window, windowChromeWorker);
			}
			windowChromeWorker.SetWindowChrome(newValue);
		}

		private void _OnPropertyChangedThatRequiresRepaint()
		{
			EventHandler eventHandler = this.PropertyChangedThatRequiresRepaint;
			if (eventHandler != null)
			{
				eventHandler(this, EventArgs.Empty);
			}
		}

		protected override Freezable CreateInstanceCore()
		{
			return new WindowChrome();
		}

		public static bool GetIsHitTestVisibleInChrome(IInputElement inputElement)
		{
			Verify.IsNotNull<IInputElement>(inputElement, "inputElement");
			DependencyObject dependencyObject = inputElement as DependencyObject;
			if (dependencyObject == null)
			{
				throw new ArgumentException("The element must be a DependencyObject", "inputElement");
			}
			return (bool)dependencyObject.GetValue(WindowChrome.IsHitTestVisibleInChromeProperty);
		}

		public static WindowChrome GetWindowChrome(Window window)
		{
			Verify.IsNotNull<Window>(window, "window");
			return (WindowChrome)window.GetValue(WindowChrome.WindowChromeProperty);
		}

		public static void SetIsHitTestVisibleInChrome(IInputElement inputElement, bool hitTestVisible)
		{
			Verify.IsNotNull<IInputElement>(inputElement, "inputElement");
			DependencyObject dependencyObject = inputElement as DependencyObject;
			if (dependencyObject == null)
			{
				throw new ArgumentException("The element must be a DependencyObject", "inputElement");
			}
			dependencyObject.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, hitTestVisible);
		}

		public static void SetWindowChrome(Window window, WindowChrome chrome)
		{
			Verify.IsNotNull<Window>(window, "window");
			window.SetValue(WindowChrome.WindowChromeProperty, chrome);
		}

		internal event EventHandler PropertyChangedThatRequiresRepaint;

		private struct _SystemParameterBoundProperty
		{
			public DependencyProperty DependencyProperty
			{
				get;
				set;
			}

			public string SystemParameterPropertyName
			{
				get;
				set;
			}
		}
	}
}