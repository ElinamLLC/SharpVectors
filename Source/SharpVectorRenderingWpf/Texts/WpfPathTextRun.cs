using System;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom;
using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Renderers.Texts
{
    /// <summary>
    /// This defines the information required to render a text-run on a path.
    /// </summary>
    public sealed class WpfPathTextRun
    {
        #region Private Fields

        private Pen _pen;
        private Brush _brush;

        private Point _contentPos;
        private string _contentText;

        private GeometryGroup _contentGeometry;

        private WpfTextBuilder _contentBuilder;
        private SvgTextContentElement _contentElement;

        private ISvgAnimatedLength _startOffset;

        #endregion

        #region Constructors and Destructor

        public WpfPathTextRun(SvgTextContentElement textElement, WpfTextBuilder textBuilder)
        {
            _contentPos     = new Point(0, 0);
            _contentElement = textElement;
            _contentBuilder = textBuilder;

            if (textBuilder != null)
            {
                textBuilder.BuildPathGeometry = true;
            }
        }

        #endregion

        #region Public Properties

        public Pen Pen
        {
            get {
                return _pen;
            }
        }
        public Brush Brush
        {
            get {
                return _brush;
            }
        }

        public Point Position
        {
            get {
                return _contentPos;
            }
        }

        public string Text
        {
            get {
                return _contentText;
            }
        }

        public WpfTextBuilder Builder
        {
            get {
                return _contentBuilder;
            }
        }

        public SvgTextContentElement Element
        {
            get {
                return _contentElement;
            }
        }

        public ISvgAnimatedLength StartOffset
        {
            get {
                return _startOffset;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(string text, Brush brush, Pen pen)
        {
            _pen         = pen;
            _brush       = brush;
            _contentText = text;
            if (text != null && (brush != null || pen != null))
            {
                _contentGeometry = new GeometryGroup();
            }
        }

        public void SetPosition(Point pos, SvgTextPathElement pathElement, SvgTextBaseElement textElement)
        {
            _contentPos  = pos;
            _startOffset = this.GetStartOffset(pathElement, textElement);
        }

        public void AddRun(Geometry textPath)
        {
            if (textPath == null || _contentGeometry == null)
            {
                return;
            }
            _contentGeometry.Children.Add(textPath);
        }

        public void RenderRun(DrawingContext context)
        {
            if (_contentGeometry == null || _contentGeometry.Children.Count == 0)
            {
                return;
            }

            // Check for paint-order to support stroke-first rendering
            WpfPaintOrder paintOrder = GetPaintOrder();

            // If stroke-first and both brush and pen exist, render stroke first then fill
            if (paintOrder == WpfPaintOrder.Stroke && _pen != null && _brush != null)
            {
                // Render stroke first (will be underneath)
                if (_contentGeometry.Children.Count == 1)
                {
                    context.DrawDrawing(new GeometryDrawing(null, _pen, _contentGeometry.Children[0]));
                }
                else
                {
                    context.DrawDrawing(new GeometryDrawing(null, _pen, _contentGeometry));
                }

                // Render fill second (will be on top)
                if (_contentGeometry.Children.Count == 1)
                {
                    context.DrawDrawing(new GeometryDrawing(_brush, null, _contentGeometry.Children[0]));
                }
                else
                {
                    context.DrawDrawing(new GeometryDrawing(_brush, null, _contentGeometry));
                }
            }
            else
            {
                // Default: draw with combined brush and pen (fill then stroke)
                if (_contentGeometry.Children.Count == 1)
                {
                    context.DrawDrawing(new GeometryDrawing(_brush, _pen, _contentGeometry.Children[0]));
                }
                else
                {
                    context.DrawDrawing(new GeometryDrawing(_brush, _pen, _contentGeometry));
                }
            }
        }

        public void UnInitialize()
        {
            _pen             = null;
            _brush           = null;
            _contentText     = null;
            _contentGeometry = null;
        }

        #endregion

        #region Private Methods

        private WpfPaintOrder GetPaintOrder()
        {
            if (_contentElement == null)
            {
                return WpfPaintOrder.Normal;
            }

            // Parse paint-order CSS property
            string paintOrderValue = _contentElement.GetAttribute(CssConstants.PropPaintOrder);
            if (string.IsNullOrWhiteSpace(paintOrderValue))
            {
                paintOrderValue = _contentElement.GetPropertyValue(CssConstants.PropPaintOrder);
            }

            return WpfPaintOrderHelper.Parse(paintOrderValue);
        }

        private ISvgAnimatedLength GetStartOffset(SvgTextPathElement pathElement, SvgTextBaseElement textElement)
        {
            ISvgAnimatedLength pathOffset = pathElement.StartOffset;
            if (pathOffset != null && pathOffset.AnimVal != null)
            {
                if (pathOffset.AnimVal.Value.Equals(0))
                {
                    var curOffset = this.GetStartOffset(textElement);
                    if (curOffset != null && curOffset.AnimVal != null)
                    {
                        if (!curOffset.AnimVal.Value.Equals(0))
                        {
                            return curOffset;
                        }
                    }
                }

                return pathOffset;
            }
            return this.GetStartOffset(textElement);
        }

        private ISvgAnimatedLength GetStartOffset(SvgTextBaseElement textElement)
        {
            ISvgAnimatedLengthList pathOffsets   = null;
            SvgTextPositioningElement posElement = _contentElement as SvgTextPositioningElement;
            if (posElement == null)
            {
                pathOffsets = textElement.Dx;
                if (pathOffsets != null && pathOffsets.Count != 0)
                {
                    return pathOffsets[0];
                }
                pathOffsets = textElement.X;
                if (pathOffsets != null && pathOffsets.Count != 0)
                {
                    return pathOffsets[0];
                }
                return null;
            }
            pathOffsets = posElement.Dx;
            if (pathOffsets != null && pathOffsets.Count != 0)
            {
                return pathOffsets[0];
            }
            pathOffsets = posElement.X;
            if (pathOffsets != null && pathOffsets.Count != 0)
            {
                return pathOffsets[0];
            }

            pathOffsets = textElement.Dx;
            if (pathOffsets != null && pathOffsets.Count != 0)
            {
                return pathOffsets[0];
            }
            pathOffsets = textElement.X;
            if (pathOffsets != null && pathOffsets.Count != 0)
            {
                return pathOffsets[0];
            }
            return null;
        }

        #endregion
    }
}
