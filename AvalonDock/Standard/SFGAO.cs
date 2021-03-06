using System;

namespace Standard
{
	[Flags]
	internal enum SFGAO : uint
	{
		CANCOPY = 1,
		CANMOVE = 2,
		CANLINK = 4,
		STORAGE = 8,
		CANRENAME = 16,
		CANDELETE = 32,
		HASPROPSHEET = 64,
		DROPTARGET = 256,
		CAPABILITYMASK = 375,
		ENCRYPTED = 8192,
		ISSLOW = 16384,
		GHOSTED = 32768,
		LINK = 65536,
		SHARE = 131072,
		READONLY = 262144,
		HIDDEN = 524288,
		DISPLAYATTRMASK = 1032192,
		NONENUMERATED = 1048576,
		NEWCONTENT = 2097152,
		CANMONIKER = 4194304,
		HASSTORAGE = 4194304,
		STREAM = 4194304,
		STORAGEANCESTOR = 8388608,
		VALIDATE = 16777216,
		REMOVABLE = 33554432,
		COMPRESSED = 67108864,
		BROWSABLE = 134217728,
		FILESYSANCESTOR = 268435456,
		FOLDER = 536870912,
		FILESYSTEM = 1073741824,
		STORAGECAPMASK = 1891958792,
		CONTENTSMASK = 2147483648,
		HASSUBFOLDER = 2147483648,
		PKEYSFGAOMASK = 2164539392
	}
}