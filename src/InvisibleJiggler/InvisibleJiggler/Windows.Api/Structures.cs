using System;
using System.Runtime.InteropServices;

namespace InvisibleJiggler.Windows.Api
{
	[StructLayout(LayoutKind.Sequential)]
	public struct INPUT
	{
		public uint type;
		public MOUSEINPUT mi;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOUSEINPUT
	{
		public int dx;
		public int dy;
		public uint mouseData;
		public uint dwFlags;
		public uint time;
		public IntPtr dwExtraInfo;
	}
}
