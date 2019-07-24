using System;
using System.Drawing;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    public class GdiHitTestResult
    {
        public static readonly GdiHitTestResult Empty = new GdiHitTestResult();

        private Point _point;
        private SvgElement _element;

        private GdiHitTestResult()
            : this(Point.Empty, null)
        {
        }

        public GdiHitTestResult(int pointX, int pointY, SvgElement element)
            : this(new Point(pointX, pointY), element)
        {
        }

        public GdiHitTestResult(Point point, SvgElement element)
        {
            _point   = point;
            _element = element;
        }

        public bool IsHit
        {
            get {
                return (_point != Point.Empty && _element != null);
            }
        }

        /// <summary>
        /// The point value to hit test against.
        /// </summary>
        public Point Point
        {
            get {
                return _point;
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
    }
}
