using System;
using System.Runtime.InteropServices;

namespace Viewer
{
	/// <summary>
	/// Summary description for Win32.
	/// </summary>
	public class Win32
	{
		/// <summary>
		/// API Open clipboard
		/// </summary>
		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern bool OpenClipboard(IntPtr h);
		
		/// <summary>
		/// Empty clipboard
		/// </summary>
		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern bool EmptyClipboard();
		
		/// <summary>
		/// Set clipboard.
		/// </summary>
		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern bool SetClipboardData(int type,IntPtr h);
		
		/// <summary>
		/// Close clipboard.
		/// </summary>
		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern bool CloseClipboard();
	}
}
