using System;
using System.Windows;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class DropArea<T> : IDropArea
	where T : FrameworkElement
	{
		private Rect _detectionRect;

		private DropAreaType _type;

		private T _element;

		public T AreaElement
		{
			get
			{
				return this._element;
			}
		}

		public Rect DetectionRect
		{
			get
			{
				return this._detectionRect;
			}
		}

		public DropAreaType Type
		{
			get
			{
				return this._type;
			}
		}

		internal DropArea(T areaElement, DropAreaType type)
		{
			this._element = areaElement;
			this._detectionRect = areaElement.GetScreenArea();
			this._type = type;
		}
	}
}