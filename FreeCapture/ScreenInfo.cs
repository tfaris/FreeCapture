using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeCapture
{
    struct ScreenInfo
    {
        int _totalWidth, _totalHeight, _minX, _minY;

        /// <summary>
        /// Get the minimum screen boundary Y coordinate of all screens.
        /// </summary>
        public int MinY
        {
            get { return _minY; }
        }

        /// <summary>
        /// Get the minimum screen boundary X coordinate of all screens.
        /// </summary>
        public int MinX
        {
            get { return _minX; }
        }
        
        /// <summary>
        /// Get the total bounding height of all screens.
        /// </summary>
        public int TotalHeight
        {
            get { return _totalHeight; }
        }

        /// <summary>
        /// Get the total bounding width of all screens.
        /// </summary>
        public int TotalWidth
        {
            get { return _totalWidth; }
        }

        /// <summary>
        /// Get information about all screens.
        /// </summary>
        public static ScreenInfo AllScreenInfo
        {
            get
            {
                ScreenInfo si = new ScreenInfo();
                System.Windows.Forms.Screen.AllScreens.ToList().ForEach(s =>
                {
                    si._totalWidth += s.Bounds.Width;
                    si._totalHeight += s.Bounds.Height;
                    si._minX = Math.Min(si._minX, s.Bounds.X);
                    si._minY = Math.Min(si._minY, s.Bounds.Y);
                });
                return si;
            }
        }
    }
}
