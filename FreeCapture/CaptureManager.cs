using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FreeCapture
{
    public class CaptureManager
    {
        CaptureShape _shape;
        CaptureSurface _surface;

        public CaptureManager(CaptureSurface surface)
        {
            _shape = new CaptureShape();
            _surface = surface;
            _surface.SetManager(this);
        }

        public void BeginCapture()
        {
            _shape.Clear();
        }

        public void AddCapturePoint(int x, int y)
        {
            AddCapturePoint(new Point(x, y));
        }

        public void AddCapturePoint(Point point)
        {
            _shape.AddPoint(point);
        }

        public System.Windows.Media.Geometry GetCaptureShape()
        {
            System.Windows.Media.StreamGeometry geo = new System.Windows.Media.StreamGeometry();
            System.Windows.Media.StreamGeometryContext ctx = geo.Open();
            System.Collections.ObjectModel.ReadOnlyCollection<System.Drawing.Point> points = _shape.Points;

            List<System.Windows.Point> mPoints = new List<System.Windows.Point>();
            foreach (Point p in points)
                mPoints.Add(new System.Windows.Point(p.X, p.Y));

            if (mPoints.Count > 0)
            {
                //mPoints.Add(mPoints[0]);
                ctx.BeginFigure(mPoints[0], false, false);
                ctx.PolyLineTo(mPoints, true, false);
            }
            ctx.Close();
            return geo;
        }

        public Image CreateImage()
        {
            using (Bitmap desktop = CaptureScreen.CaptureScreen.GetDesktopImage())
            {
                System.Windows.Media.Geometry shapeGeometry = GetCaptureShape();
                System.Windows.Rect bounds =
                    shapeGeometry.GetRenderBounds(new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 1));

                System.Windows.Point screenTopLeft = _surface.PointToScreen(bounds.TopLeft),
                                     screenBotRight = _surface.PointToScreen(bounds.BottomRight);


                RectangleF screenRect =
                           new RectangleF((float)screenTopLeft.X, (float)screenTopLeft.Y,
                                                 (float)(screenBotRight.X - screenTopLeft.X),
                                                 (float)(screenBotRight.Y - screenTopLeft.Y)),

                           dstRect = new RectangleF(0, 0, screenRect.Width, screenRect.Height);

                System.Drawing.Bitmap img = new Bitmap((int)dstRect.Width, (int)dstRect.Height);
                img.SetResolution(desktop.HorizontalResolution, desktop.VerticalResolution);
                using (Graphics g = Graphics.FromImage(img))
                {
                    g.Clear(Color.Transparent);
                    g.DrawImage(desktop, dstRect, screenRect, GraphicsUnit.Pixel);
                }

                System.Drawing.Imaging.BitmapData data = null;
                try
                {
                    data = img.LockBits(
                     new Rectangle(0, 0, (int)dstRect.Width, (int)dstRect.Height),
                     System.Drawing.Imaging.ImageLockMode.ReadWrite,
                     img.PixelFormat);

                    // Get the address of the first line.
                    IntPtr ptr = data.Scan0;
                    // Assume 4 channels - blue, green, red, alpha
                    int bytesPerPixel = 4;
                    // Declare an array to hold the bytes of the bitmap.
                    // This code is specific to a bitmap with 32 bits per pixels 
                    // (32 bits = 4 bytes, 3 for RGB and 1 byte for alpha).
                    int numBytes = img.Width * img.Height * bytesPerPixel;
                    byte[] argbValues = new byte[numBytes];

                    // Copy the ARGB values into the array.
                    System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, numBytes);

                    int stride = data.Stride,
                        bytes = data.Stride * img.Height;

                    // Split the points into x and y vertices
                    System.Collections.ObjectModel.ReadOnlyCollection<System.Drawing.Point> points = _shape.Points;
                    List<float> vx = new List<float>(), vy = new List<float>();
                    foreach (System.Drawing.Point p in points)
                    {
                        vx.Add(p.X);
                        vy.Add(p.Y);
                    }

                    // Set alpha channel to 0 for all pixels not within the selected shape.
                    // This is the slowest part of the capture.
                    ScreenInfo si = ScreenInfo.AllScreenInfo;
                    for (int y = 0; y < data.Height; y++)
                    {
                        for (int x = 0; x < data.Width; x++)
                        {
                            // Offset by the lowest screen points
                            System.Windows.Point scr = new System.Windows.Point((float)(x + screenTopLeft.X - si.MinX),
                                                                                (float)(y + screenTopLeft.Y - si.MinY));
                            bool poly = pnpoly(points.Count, vx, vy, (float)scr.X, (float)scr.Y);
                            if (!poly)
                            {
                                argbValues[(y * stride) + (x * bytesPerPixel) + 3] = 0;
                            }
                        }
                    }

                    // Copy the ARGB values back to the bitmap
                    System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, numBytes);
                }
                finally
                {
                    if (data != null)
                        img.UnlockBits(data);
                }
                                
                return img;
                //System.Windows.Media.Imaging.PngBitmapEncoder encoder =
                //    new System.Windows.Media.Imaging.PngBitmapEncoder();       
            }
        }

        /// <summary>
        /// Hit test the lists of vertices.
        /// </summary>
        /// <param name="nvert"></param>
        /// <param name="vertx"></param>
        /// <param name="verty"></param>
        /// <param name="testx"></param>
        /// <param name="testy"></param>
        /// <returns></returns>
        bool pnpoly(int nvert, IList<float> vertx, IList<float> verty, float testx, float testy)
        {
            bool c = false;
            int i, j;
            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((verty[i] > testy) != (verty[j] > testy)) &&
                 (testx < (vertx[j] - vertx[i]) * (testy - verty[i]) / (verty[j] - verty[i]) + vertx[i]))
                    c = !c;
            }
            return c;
        }
    }
}
