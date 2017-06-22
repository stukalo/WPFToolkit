using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Standard
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct HRESULT
	{
		[FieldOffset(0)]
		private readonly uint _value;

		[FieldOffset(-1)]
		public readonly static HRESULT S_OK;

		[FieldOffset(-1)]
		public readonly static HRESULT S_FALSE;

		[FieldOffset(-1)]
		public readonly static HRESULT E_PENDING;

		[FieldOffset(-1)]
		public readonly static HRESULT E_NOTIMPL;

		[FieldOffset(-1)]
		public readonly static HRESULT E_NOINTERFACE;

		[FieldOffset(-1)]
		public readonly static HRESULT E_POINTER;

		[FieldOffset(-1)]
		public readonly static HRESULT E_ABORT;

		[FieldOffset(-1)]
		public readonly static HRESULT E_FAIL;

		[FieldOffset(-1)]
		public readonly static HRESULT E_UNEXPECTED;

		[FieldOffset(-1)]
		public readonly static HRESULT STG_E_INVALIDFUNCTION;

		[FieldOffset(-1)]
		public readonly static HRESULT REGDB_E_CLASSNOTREG;

		[FieldOffset(-1)]
		public readonly static HRESULT DESTS_E_NO_MATCHING_ASSOC_HANDLER;

		[FieldOffset(-1)]
		public readonly static HRESULT DESTS_E_NORECDOCS;

		[FieldOffset(-1)]
		public readonly static HRESULT DESTS_E_NOTALLCLEARED;

		[FieldOffset(-1)]
		public readonly static HRESULT E_ACCESSDENIED;

		[FieldOffset(-1)]
		public readonly static HRESULT E_OUTOFMEMORY;

		[FieldOffset(-1)]
		public readonly static HRESULT E_INVALIDARG;

		[FieldOffset(-1)]
		public readonly static HRESULT INTSAFE_E_ARITHMETIC_OVERFLOW;

		[FieldOffset(-1)]
		public readonly static HRESULT COR_E_OBJECTDISPOSED;

		[FieldOffset(-1)]
		public readonly static HRESULT WC_E_GREATERTHAN;

		[FieldOffset(-1)]
		public readonly static HRESULT WC_E_SYNTAX;

		public int Code
		{
			get
			{
				return HRESULT.GetCode((int)this._value);
			}
		}

		public Standard.Facility Facility
		{
			get
			{
				return HRESULT.GetFacility((int)this._value);
			}
		}

		public bool Failed
		{
			get
			{
				return this._value < 0;
			}
		}

		public bool Succeeded
		{
			get
			{
				return this._value >= 0;
			}
		}

		static HRESULT()
		{
			HRESULT.S_OK = new HRESULT(0);
			HRESULT.S_FALSE = new HRESULT(1);
			HRESULT.E_PENDING = new HRESULT(-2147483638);
			HRESULT.E_NOTIMPL = new HRESULT(-2147467263);
			HRESULT.E_NOINTERFACE = new HRESULT(-2147467262);
			HRESULT.E_POINTER = new HRESULT(-2147467261);
			HRESULT.E_ABORT = new HRESULT(-2147467260);
			HRESULT.E_FAIL = new HRESULT(-2147467259);
			HRESULT.E_UNEXPECTED = new HRESULT(-2147418113);
			HRESULT.STG_E_INVALIDFUNCTION = new HRESULT(-2147287039);
			HRESULT.REGDB_E_CLASSNOTREG = new HRESULT(-2147221164);
			HRESULT.DESTS_E_NO_MATCHING_ASSOC_HANDLER = new HRESULT(-2147217661);
			HRESULT.DESTS_E_NORECDOCS = new HRESULT(-2147217660);
			HRESULT.DESTS_E_NOTALLCLEARED = new HRESULT(-2147217659);
			HRESULT.E_ACCESSDENIED = new HRESULT(-2147024891);
			HRESULT.E_OUTOFMEMORY = new HRESULT(-2147024882);
			HRESULT.E_INVALIDARG = new HRESULT(-2147024809);
			HRESULT.INTSAFE_E_ARITHMETIC_OVERFLOW = new HRESULT(-2147024362);
			HRESULT.COR_E_OBJECTDISPOSED = new HRESULT(-2146232798);
			HRESULT.WC_E_GREATERTHAN = new HRESULT(-1072894429);
			HRESULT.WC_E_SYNTAX = new HRESULT(-1072894419);
		}

		public HRESULT(uint i)
		{
			this._value = i;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			try
			{
				flag = ((HRESULT)obj)._value == this._value;
			}
			catch (InvalidCastException invalidCastException)
			{
				flag = false;
			}
			return flag;
		}

		public static int GetCode(int error)
		{
			return error & 65535;
		}

		public static Standard.Facility GetFacility(int errorCode)
		{
			return (Standard.Facility)(errorCode >> 16 & 8191);
		}

		public override int GetHashCode()
		{
			return this._value.GetHashCode();
		}

		public static HRESULT Make(bool severe, Standard.Facility facility, int code)
		{
			Standard.Facility facility1;
			if (severe)
			{
				facility1 = (Standard.Facility)-2147483648;
			}
			else
			{
				facility1 = Standard.Facility.Null;
			}
			return new HRESULT((uint)((int)facility1 | (int)facility << 16 | code));
		}

		public static bool operator ==(HRESULT hrLeft, HRESULT hrRight)
		{
			return hrLeft._value == hrRight._value;
		}

		public static bool operator !=(HRESULT hrLeft, HRESULT hrRight)
		{
			return !(hrLeft == hrRight);
		}

		public void ThrowIfFailed()
		{
			this.ThrowIfFailed(null);
		}

		public void ThrowIfFailed(string message)
		{
			if (this.Failed)
			{
				if (string.IsNullOrEmpty(message))
				{
					message = this.ToString();
				}
				Exception exceptionForHR = Marshal.GetExceptionForHR((int)this._value, new IntPtr(-1));
				if (exceptionForHR.GetType() != typeof(COMException))
				{
					ConstructorInfo constructor = exceptionForHR.GetType().GetConstructor(new Type[] { typeof(string) });
					if (null != constructor)
					{
						exceptionForHR = constructor.Invoke(new object[] { message }) as Exception;
					}
				}
				else if (this.Facility != Standard.Facility.Win32)
				{
					exceptionForHR = new COMException(message, (int)this._value);
				}
				else
				{
					exceptionForHR = new Win32Exception(this.Code, message);
				}
				throw exceptionForHR;
			}
		}

		public static void ThrowLastError()
		{
			((HRESULT)Win32Error.GetLastError()).ThrowIfFailed();
		}

		public override string ToString()
		{
			int i;
			FieldInfo[] fields = typeof(HRESULT).GetFields(BindingFlags.Static | BindingFlags.Public);
			for (i = 0; i < (int)fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				if (fieldInfo.FieldType == typeof(HRESULT) && (HRESULT)fieldInfo.GetValue(null) == this)
				{
					return fieldInfo.Name;
				}
			}
			if (this.Facility == Standard.Facility.Win32)
			{
				fields = typeof(Win32Error).GetFields(BindingFlags.Static | BindingFlags.Public);
				for (i = 0; i < (int)fields.Length; i++)
				{
					FieldInfo fieldInfo1 = fields[i];
					if (fieldInfo1.FieldType == typeof(Win32Error) && (HRESULT)((Win32Error)fieldInfo1.GetValue(null)) == this)
					{
						return string.Concat("HRESULT_FROM_WIN32(", fieldInfo1.Name, ")");
					}
				}
			}
			return string.Format(CultureInfo.InvariantCulture, "0x{0:X8}", new object[] { this._value });
		}
	}
}