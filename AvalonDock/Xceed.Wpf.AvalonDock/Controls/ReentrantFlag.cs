using System;

namespace Xceed.Wpf.AvalonDock.Controls
{
	internal class ReentrantFlag
	{
		private bool _flag;

		public bool CanEnter
		{
			get
			{
				return !this._flag;
			}
		}

		public ReentrantFlag()
		{
		}

		public ReentrantFlag._ReentrantFlagHandler Enter()
		{
			if (this._flag)
			{
				throw new InvalidOperationException();
			}
			return new ReentrantFlag._ReentrantFlagHandler(this);
		}

		public class _ReentrantFlagHandler : IDisposable
		{
			private ReentrantFlag _owner;

			public _ReentrantFlagHandler(ReentrantFlag owner)
			{
				this._owner = owner;
				this._owner._flag = true;
			}

			public void Dispose()
			{
				this._owner._flag = false;
			}
		}
	}
}