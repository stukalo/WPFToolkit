using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Standard
{
	internal sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
		private SafeFindHandle() : base(true)
		{
		}

		protected override bool ReleaseHandle()
		{
			return Standard.NativeMethods.FindClose(this.handle);
		}
	}
}