using System;
using System.Runtime.InteropServices;
namespace CaptureScreen
{
	/// <summary>
	/// This class shall keep the User32 APIs being used in 
	/// our program.
	/// </summary>
	public class PlatformInvokeUSER32
	{

		#region Class Variables
		public  const int SM_CXSCREEN=0;
		public  const int SM_CYSCREEN=1;
        public const int SM_CXVIRTUALSCREEN = 78;
        public const int SM_CYVIRTUALSCREEN = 79;
		#endregion		
		
		#region Class Functions
		[DllImport("user32.dll", EntryPoint="GetDesktopWindow")]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll",EntryPoint="GetDC")]
		public static extern IntPtr GetDC(IntPtr ptr);

		[DllImport("user32.dll",EntryPoint="GetSystemMetrics")]
		public static extern int GetSystemMetrics(int abc);

		[DllImport("user32.dll",EntryPoint="GetWindowDC")]
		public static extern IntPtr GetWindowDC(Int32 ptr);

		[DllImport("user32.dll",EntryPoint="ReleaseDC")]
		public static extern IntPtr ReleaseDC(IntPtr hWnd,IntPtr hDc);
		
		#endregion

		#region Public Constructor
		public PlatformInvokeUSER32()
		{
			// 
			// TODO: Add constructor logic here
			//
		}
		#endregion
	}

	//This structure shall be used to keep the size of the screen.
	public struct SIZE
	{
		public int cx;
		public int cy;
	}
}
