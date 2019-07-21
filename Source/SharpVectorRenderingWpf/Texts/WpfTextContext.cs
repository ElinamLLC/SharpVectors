using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Renderers.Texts
{
    public sealed class WpfTextContext
    {
        #region Private Fields

        private SvgTextElement _textElement;
        private WpfTextRendering _textRendering;

        #endregion

        #region Constructors and Destructor

        public WpfTextContext(SvgTextElement textElement, WpfTextRendering textRendering)
        {
            if (textElement == null)
            {
                throw new ArgumentNullException(nameof(textElement),
                    "The SVG text element is required, and cannot be null (or Nothing).");
            }
            if (textRendering == null)
            {
                throw new ArgumentNullException(nameof(textRendering),
                    "The text rendering object is required, and cannot be null (or Nothing).");
            }

            _textElement = textElement;
            _textRendering = textRendering;
        }

        #endregion

        #region Public Properties

        public SvgTextElement TextElement
        {
            get {
                return _textElement;
            }
        }

        #endregion

        #region Public Methods

        public static TextDecoration Squiggly(Color color, TextDecorationLocation location = TextDecorationLocation.Underline)
        {
            var penVisual = new Path
            {
                Stroke             = new SolidColorBrush(color),
                StrokeThickness    = 0.2,
                StrokeEndLineCap   = PenLineCap.Square,
                StrokeStartLineCap = PenLineCap.Square,
                Data               = new PathGeometry(new[] 
                {
                    new PathFigure(new Point(0, 1), new[] 
                    {
                        new BezierSegment(new Point(1, 0), new Point(2, 2), new Point(3, 1), true)
                    }, false)
                })
            };

            var penBrush = new VisualBrush
            {
                Viewbox       = new Rect(0, 0, 3, 2),
                ViewboxUnits  = BrushMappingMode.Absolute,
                Viewport      = new Rect(0, 0.8, 6, 3),
                ViewportUnits = BrushMappingMode.Absolute,
                TileMode      = TileMode.Tile,
                Visual        = penVisual
            };

            var pen = new Pen
            {
                Brush = penBrush,
                Thickness = 6
            };

            return new TextDecoration(location, pen, 0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended);
        }
        #endregion
    }
}
