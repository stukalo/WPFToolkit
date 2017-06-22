using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Xml;
using Xceed.Wpf.AvalonDock;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[Serializable]
	public abstract class LayoutPositionableGroup<T> : LayoutGroup<T>, ILayoutPositionableElement, ILayoutElement, INotifyPropertyChanged, INotifyPropertyChanging, ILayoutElementForFloatingWindow, ILayoutPositionableElementWithActualSize
	where T : class, ILayoutElement
	{
		private GridLength _dockWidth;

		private GridLength _dockHeight;

		private bool _canRepositionItems;

		private double _dockMinWidth;

		private double _dockMinHeight;

		private double _floatingWidth;

		private double _floatingHeight;

		private double _floatingLeft;

		private double _floatingTop;

		private bool _isMaximized;

		[NonSerialized]
		private double _actualWidth;

		[NonSerialized]
		private double _actualHeight;

		private static GridLengthConverter _gridLengthConverter;

		public bool CanRepositionItems
		{
			get
			{
				return this._canRepositionItems;
			}
			set
			{
				if (this._canRepositionItems != value)
				{
					this.RaisePropertyChanging("CanRepositionItems");
					this._canRepositionItems = value;
					this.RaisePropertyChanged("CanRepositionItems");
				}
			}
		}

		public GridLength DockHeight
		{
			get
			{
				return this._dockHeight;
			}
			set
			{
				if (this.DockHeight != value)
				{
					this.RaisePropertyChanging("DockHeight");
					this._dockHeight = value;
					this.RaisePropertyChanged("DockHeight");
					this.OnDockHeightChanged();
				}
			}
		}

		public double DockMinHeight
		{
			get
			{
				return this._dockMinHeight;
			}
			set
			{
				if (this._dockMinHeight != value)
				{
					MathHelper.AssertIsPositiveOrZero(value);
					this.RaisePropertyChanging("DockMinHeight");
					this._dockMinHeight = value;
					this.RaisePropertyChanged("DockMinHeight");
				}
			}
		}

		public double DockMinWidth
		{
			get
			{
				return this._dockMinWidth;
			}
			set
			{
				if (this._dockMinWidth != value)
				{
					MathHelper.AssertIsPositiveOrZero(value);
					this.RaisePropertyChanging("DockMinWidth");
					this._dockMinWidth = value;
					this.RaisePropertyChanged("DockMinWidth");
				}
			}
		}

		public GridLength DockWidth
		{
			get
			{
				return this._dockWidth;
			}
			set
			{
				if (this.DockWidth != value)
				{
					this.RaisePropertyChanging("DockWidth");
					this._dockWidth = value;
					this.RaisePropertyChanged("DockWidth");
					this.OnDockWidthChanged();
				}
			}
		}

		public double FloatingHeight
		{
			get
			{
				return this._floatingHeight;
			}
			set
			{
				if (this._floatingHeight != value)
				{
					this.RaisePropertyChanging("FloatingHeight");
					this._floatingHeight = value;
					this.RaisePropertyChanged("FloatingHeight");
				}
			}
		}

		public double FloatingLeft
		{
			get
			{
				return this._floatingLeft;
			}
			set
			{
				if (this._floatingLeft != value)
				{
					this.RaisePropertyChanging("FloatingLeft");
					this._floatingLeft = value;
					this.RaisePropertyChanged("FloatingLeft");
				}
			}
		}

		public double FloatingTop
		{
			get
			{
				return this._floatingTop;
			}
			set
			{
				if (this._floatingTop != value)
				{
					this.RaisePropertyChanging("FloatingTop");
					this._floatingTop = value;
					this.RaisePropertyChanged("FloatingTop");
				}
			}
		}

		public double FloatingWidth
		{
			get
			{
				return this._floatingWidth;
			}
			set
			{
				if (this._floatingWidth != value)
				{
					this.RaisePropertyChanging("FloatingWidth");
					this._floatingWidth = value;
					this.RaisePropertyChanged("FloatingWidth");
				}
			}
		}

		public bool IsMaximized
		{
			get
			{
				return this._isMaximized;
			}
			set
			{
				if (this._isMaximized != value)
				{
					this._isMaximized = value;
					this.RaisePropertyChanged("IsMaximized");
				}
			}
		}

		double Xceed.Wpf.AvalonDock.Layout.ILayoutPositionableElementWithActualSize.ActualHeight
		{
			get
			{
				return this._actualHeight;
			}
			set
			{
				this._actualHeight = value;
			}
		}

		double Xceed.Wpf.AvalonDock.Layout.ILayoutPositionableElementWithActualSize.ActualWidth
		{
			get
			{
				return this._actualWidth;
			}
			set
			{
				this._actualWidth = value;
			}
		}

		static LayoutPositionableGroup()
		{
			LayoutPositionableGroup<T>._gridLengthConverter = new GridLengthConverter();
		}

		public LayoutPositionableGroup()
		{
		}

		protected virtual void OnDockHeightChanged()
		{
		}

		protected virtual void OnDockWidthChanged()
		{
		}

		public override void ReadXml(XmlReader reader)
		{
			if (reader.MoveToAttribute("DockWidth"))
			{
				this._dockWidth = (GridLength)LayoutPositionableGroup<T>._gridLengthConverter.ConvertFromInvariantString(reader.Value);
			}
			if (reader.MoveToAttribute("DockHeight"))
			{
				this._dockHeight = (GridLength)LayoutPositionableGroup<T>._gridLengthConverter.ConvertFromInvariantString(reader.Value);
			}
			if (reader.MoveToAttribute("DocMinWidth"))
			{
				this._dockMinWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			if (reader.MoveToAttribute("DocMinHeight"))
			{
				this._dockMinHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			if (reader.MoveToAttribute("FloatingWidth"))
			{
				this._floatingWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			if (reader.MoveToAttribute("FloatingHeight"))
			{
				this._floatingHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			if (reader.MoveToAttribute("FloatingLeft"))
			{
				this._floatingLeft = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			if (reader.MoveToAttribute("FloatingTop"))
			{
				this._floatingTop = double.Parse(reader.Value, CultureInfo.InvariantCulture);
			}
			if (reader.MoveToAttribute("IsMaximized"))
			{
				this._isMaximized = bool.Parse(reader.Value);
			}
			base.ReadXml(reader);
		}

		public override void WriteXml(XmlWriter writer)
		{
			double dockMinWidth;
			if (this.DockWidth.Value != 1 || !this.DockWidth.IsStar)
			{
				writer.WriteAttributeString("DockWidth", LayoutPositionableGroup<T>._gridLengthConverter.ConvertToInvariantString(this.DockWidth));
			}
			if (this.DockHeight.Value != 1 || !this.DockHeight.IsStar)
			{
				writer.WriteAttributeString("DockHeight", LayoutPositionableGroup<T>._gridLengthConverter.ConvertToInvariantString(this.DockHeight));
			}
			if (this.DockMinWidth != 25)
			{
				dockMinWidth = this.DockMinWidth;
				writer.WriteAttributeString("DocMinWidth", dockMinWidth.ToString(CultureInfo.InvariantCulture));
			}
			if (this.DockMinHeight != 25)
			{
				dockMinWidth = this.DockMinHeight;
				writer.WriteAttributeString("DockMinHeight", dockMinWidth.ToString(CultureInfo.InvariantCulture));
			}
			if (this.FloatingWidth != 0)
			{
				dockMinWidth = this.FloatingWidth;
				writer.WriteAttributeString("FloatingWidth", dockMinWidth.ToString(CultureInfo.InvariantCulture));
			}
			if (this.FloatingHeight != 0)
			{
				dockMinWidth = this.FloatingHeight;
				writer.WriteAttributeString("FloatingHeight", dockMinWidth.ToString(CultureInfo.InvariantCulture));
			}
			if (this.FloatingLeft != 0)
			{
				dockMinWidth = this.FloatingLeft;
				writer.WriteAttributeString("FloatingLeft", dockMinWidth.ToString(CultureInfo.InvariantCulture));
			}
			if (this.FloatingTop != 0)
			{
				dockMinWidth = this.FloatingTop;
				writer.WriteAttributeString("FloatingTop", dockMinWidth.ToString(CultureInfo.InvariantCulture));
			}
			if (this.IsMaximized)
			{
				writer.WriteAttributeString("IsMaximized", this.IsMaximized.ToString());
			}
			base.WriteXml(writer);
		}
	}
}