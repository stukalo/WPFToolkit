using System;
using System.Runtime.InteropServices;

namespace Standard
{
	internal class MONITORINFO
	{
		public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

		public RECT rcMonitor;

		public RECT rcWork;

		public int dwFlags;

		public MONITORINFO()
		{
		}
	}
}