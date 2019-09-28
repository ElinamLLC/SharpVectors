using System;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public class WpfHitTestResult
    {
        public static readonly WpfHitTestResult Empty = new WpfHitTestResult();

        private Point? _point;
        private Rect? _rect;
        private Drawing _drawing;
        private SvgElement _element;

        private WpfHitTestResult()
        {
        }

        public WpfHitTestResult(int pointX, int pointY, SvgElement element, Drawing drawing)
            : this(new Point(pointX, pointY), element, drawing)
        {
        }

        public WpfHitTestResult(Point point, SvgElement element, Drawing drawing)
        {
            _point   = point;
            _element = element;
            _drawing = drawing;
        }

        public WpfHitTestResult(Rect rect, SvgElement element, Drawing drawing)
        {
            _rect    = rect;
            _element = element;
            _drawing = drawing;
        }

        public bool IsHit
        {
            get {
                return (_element != null && _drawing != null);
            }
        }

        /// <summary>
        /// The point value to hit test against.
        /// </summary>
        public Point? Point
        {
            get {
                return _point;
            }
        }

        public Rect? Rect
        {
            get {
                return _rect;
            }
        }

        /// <summary>
        /// Gets the SVG object that was hit.
        /// </summary>
        public SvgElement Element
        {
            get {
                return _element;
            }
        }

        public Drawing Drawing
        {
            get {
                return _drawing;
            }
        }
    }
}
