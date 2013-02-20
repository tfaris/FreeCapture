using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace FreeCapture
{
    public class CaptureSurface : UIElement
    {
        CaptureManager _mgr;
        Point _mouseDownPoint;
        bool _mouseDown;

        public CaptureSurface()
        {
        }

        public CaptureSurface(CaptureManager manager)
            : this()
        {
            _mgr = manager;
        }

        /// <summary>
        /// Set the capture manager.
        /// </summary>
        /// <param name="manager"></param>
        public void SetManager(CaptureManager manager)
        {
            _mgr = manager;
        }
        
        /// <summary>
        /// Draw the freeform selection shape.
        /// </summary>
        /// <param name="dc"></param>
        protected virtual void DrawSelectionShape(DrawingContext dc)
        {
            if (_mgr != null)
            {
                // Draw the freeform selection
                System.Windows.Media.Geometry shapeGeo = _mgr.GetCaptureShape();
                System.Windows.Media.Brush brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Yellow);
                System.Windows.Media.Pen pen = new System.Windows.Media.Pen(brush, 5.0);
                pen.DashStyle = System.Windows.Media.DashStyles.Dash;
                dc.DrawGeometry(brush, pen, shapeGeo);
            }          
        }

        /// <summary>
        /// Draw some helpful text.
        /// </summary>
        /// <param name="dc"></param>
        protected virtual void DrawHelpText(DrawingContext dc)
        {
            System.Windows.Media.Typeface backType =
                new System.Windows.Media.Typeface(new System.Windows.Media.FontFamily("sans courier"),
                                                  FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            System.Windows.Media.FormattedText formatted = new System.Windows.Media.FormattedText(
                                                            "Click & move the mouse to select a capture area.\nENTER/F10: Capture\nBACKSPACE/DEL: Start over\nESC: Exit",
                                                            System.Globalization.CultureInfo.CurrentCulture,
                                                            FlowDirection.LeftToRight,
                                                            backType,
                                                            32.0f,
                                                            new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White));
            // Make sure the text shows at 0,0 on the primary screen
            System.Drawing.Point primScreen = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Location;
            Point clientBase = PointFromScreen(new Point(primScreen.X + 5, primScreen.Y + 5));
            Geometry textGeo = formatted.BuildGeometry(clientBase);
            dc.DrawGeometry(
                System.Windows.Media.Brushes.White,
                null,
                textGeo);

            dc.DrawGeometry(
                null,
                new System.Windows.Media.Pen(System.Windows.Media.Brushes.White, 1),
                textGeo);
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
            // Drawing a transparent rect the size of our render area keeps the control visible and
            // "interactable". Is there another way to do this?
            dc.DrawRectangle(
                new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black),
                null,
                new Rect(RenderSize));

            DrawSelectionShape(dc);
            DrawHelpText(dc);
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            _mouseDown = true;
            _mouseDownPoint = e.MouseDevice.GetPosition(this);
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            _mouseDown = false;
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_mouseDown)
            {
                // Trace mouse selection
                Point mousePoint = e.GetPosition(this);
                _mgr.AddCapturePoint((int)mousePoint.X, (int)mousePoint.Y);
                InvalidateVisual();
            }
        }
    }
}
