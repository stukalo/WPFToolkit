using System;

namespace Standard
{
	internal class NOTIFYICONDATA
	{
		public int cbSize;

		public IntPtr hWnd;

		public int uID;

		public NIF uFlags;

		public int uCallbackMessage;

		public IntPtr hIcon;

		public char[] szTip = new char[128];

		public uint dwState;

		public uint dwStateMask;

		public char[] szInfo = new char[256];

		public uint uVersion;

		public char[] szInfoTitle = new char[64];

		public uint dwInfoFlags;

		public Guid guidItem;

		private IntPtr hBalloonIcon;

		public NOTIFYICONDATA()
		{
		}
	}
}