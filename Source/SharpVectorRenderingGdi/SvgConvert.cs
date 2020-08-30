using System;
using System.Drawing;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers
{
    public static class SvgConvert
    {
        public static SvgPointF ToPoint(Point pt)
        {
            return new SvgPointF(pt.X, pt.Y);
        }

        public static SvgPointF ToPoint(PointF pt)
        {
            return new SvgPointF(pt.X, pt.Y);
        }

        public static SvgRectF ToRect(Rectangle rect)
        {
            return new SvgRectF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static SvgRectF ToRect(RectangleF rect)
        {
            return new SvgRectF(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
