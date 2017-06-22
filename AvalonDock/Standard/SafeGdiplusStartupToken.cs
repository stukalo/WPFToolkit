using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Standard
{
	internal sealed class SafeGdiplusStartupToken : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeGdiplusStartupToken() : base(true)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected override bool ReleaseHandle()
		{
			return Standard.NativeMethods.GdiplusShutdown(this.handle) == Status.Ok;
		}

		public static SafeGdiplusStartupToken Startup()
		{
			IntPtr intPtr;
			StartupOutput startupOutput;
			SafeGdiplusStartupToken safeGdiplusStartupToken = new SafeGdiplusStartupToken();
			if (Standard.NativeMethods.GdiplusStartup(out intPtr, new StartupInput(), out startupOutput) != Status.Ok)
			{
				safeGdiplusStartupToken.Dispose();
				throw new Exception("Unable to initialize GDI+");
			}
			safeGdiplusStartupToken.handle = intPtr;
			return safeGdiplusStartupToken;
		}
	}
}