using System.Windows;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Utils
{
    public static class SvgConvert
    {
        public static SvgPointF ToPoint(Point pt)
        {
            return new SvgPointF((float)pt.X, (float)pt.Y);
        }

        public static SvgRectF ToRect(Rect rect)
        {
            return new SvgRectF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
        }
    }
}
