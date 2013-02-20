using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FreeCapture
{
    class CaptureShape
    {
        List<Point> _points;

        public System.Collections.ObjectModel.ReadOnlyCollection<Point> Points
        {
            get { return _points.AsReadOnly(); }
        }

        public CaptureShape()
        {
            _points = new List<Point>();
        }

        public CaptureShape(IEnumerable<Point> points)
            : this()
        {
            _points.AddRange(points);
        }

        public void AddPoint(int x, int y)
        {
            AddPoint(new Point(x, y));
        }

        public void AddPoint(Point point)
        {
            _points.Add(point);
        }

        public void Clear()
        {
            _points.Clear();
        }
    }
}
