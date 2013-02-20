using System;
using System.Drawing;


namespace CaptureScreen
{
	/// <summary>
	/// This class shall keep all the functionality for capturing 
	/// the desktop.
	/// </summary>
	public class CaptureScreen
	{
	
		#region Constructor
		public CaptureScreen()
		{
			// 
			// TODO: Add constructor logic here
			//
		}

		#endregion

		#region Class Variable Declaration
			protected static IntPtr m_HBitmap;
		#endregion

		#region Public Class Functions
		public static Bitmap GetDesktopImage()
		{
			//In size variable we shall keep the size of the screen.
			SIZE size;
			
			//Here we get the handle to the desktop device context.
			IntPtr 	hDC = PlatformInvokeUSER32.GetDC(PlatformInvokeUSER32.GetDesktopWindow()); 

			//Here we make a compatible device context in memory for screen device context.
			IntPtr hMemDC = PlatformInvokeGDI32.CreateCompatibleDC(hDC);
			
			//We pass SM_CXSCREEN constant to GetSystemMetrics to get the X coordinates of screen.
			size.cx = PlatformInvokeUSER32.GetSystemMetrics(PlatformInvokeUSER32.SM_CXVIRTUALSCREEN);

			//We pass SM_CYSCREEN constant to GetSystemMetrics to get the Y coordinates of screen.
			size.cy = PlatformInvokeUSER32.GetSystemMetrics(PlatformInvokeUSER32.SM_CYVIRTUALSCREEN);
			
			//We create a compatible bitmap of screen size and using screen device context.
			m_HBitmap = PlatformInvokeGDI32.CreateCompatibleBitmap(hDC, size.cx, size.cy);

			//As m_HBitmap is IntPtr we can not check it against null. For this purspose IntPtr.Zero is used.
			if (m_HBitmap!=IntPtr.Zero)
			{
				//Here we select the compatible bitmap in memeory device context and keeps the refrence to Old bitmap.
				IntPtr hOld = (IntPtr) PlatformInvokeGDI32.SelectObject(hMemDC, m_HBitmap);
				//We copy the Bitmap to the memory device context.
				PlatformInvokeGDI32.BitBlt(hMemDC, 0, 0,size.cx,size.cy, hDC, 0, 0, PlatformInvokeGDI32.SRCCOPY);
				//We select the old bitmap back to the memory device context.
				PlatformInvokeGDI32.SelectObject(hMemDC, hOld);
				//We delete the memory device context.
				PlatformInvokeGDI32.DeleteDC(hMemDC);
				//We release the screen device context.
				PlatformInvokeUSER32.ReleaseDC(PlatformInvokeUSER32.GetDesktopWindow(), hDC);
				//Image is created by Image bitmap handle and returned.
				return System.Drawing.Image.FromHbitmap(m_HBitmap); 
			}
			//If m_HBitmap is null retunrn null.
			return null;
		}
		#endregion
	}
}
