using System;

namespace Standard
{
	internal class StartupInput
	{
		public int GdiplusVersion = 1;

		public IntPtr DebugEventCallback;

		public bool SuppressBackgroundThread;

		public bool SuppressExternalCodecs;

		public StartupInput()
		{
		}
	}
}