using System;
using System.Xml;
using System.Diagnostics;
using System.Globalization;
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

        private bool _isVertical;
        private bool _isSingleText;
        private bool _isSingleLine;
        private bool _isTextPath;

        private Point _positioningStart;
        private Point _positioningEnd;
        private SvgTextContentElement _positioningElement;

        private SvgTextBaseElement _textElement;
        private WpfTextRendering _textRendering;

        private CultureInfo _culture;

        private IDictionary<double, double> _sizeMap;

        #endregion

        #region Constructors and Destructor

        public WpfTextContext(SvgTextBaseElement textElement, WpfTextRendering textRendering)
        {
            if (textRendering == null)
            {
                throw new ArgumentNullException(nameof(textRendering),
                    "The text rendering object is required, and cannot be null (or Nothing).");
            }
            _textRendering = textRendering;

            this.SetElement(textElement);
        }

        #endregion

        #region Public Properties

        public SvgTextBaseElement TextElement
        {
            get {
                return _textElement;
            }
        }

        public bool IsVertical
        {
            get {
                return _isVertical;
            }
        }

        public bool IsSingleText
        {
            get {
                return _isSingleText;
            }
        }

        public bool IsTextPath
        {
            get {
                return _isTextPath;
            }
        }

        public bool IsSingleLine
        {
            get {
                return _isSingleLine;
            }
            set {
                _isSingleLine = value;
            }
        }

        public Point PositioningStart
        {
            get {
                return _positioningStart;
            }
            set {
                _positioningStart = value;
            }
        }

        public Point PositioningEnd
        {
            get {
                return _positioningEnd;
            }
            set {
                _positioningEnd = value;
            }
        }

        public SvgTextContentElement PositioningElement
        {
            get {
                return _positioningElement;
            }
            set {
                _positioningElement = value;
            }
        }

        public CultureInfo Culture
        {
            get {
                return _culture;
            }
        }

        #endregion

        #region Public Methods

        public void SetElement(SvgTextBaseElement textElement)
        {
            if (textElement == null)
            {
                throw new ArgumentNullException(nameof(textElement),
                    "The SVG text element is required, and cannot be null (or Nothing).");
            }

            _culture          = null;

            _isVertical       = false;
            _isSingleText     = false;
            _isSingleLine     = false;
            _isTextPath       = false;
            _positioningElement = null;

            _positioningStart = new Point(0, 0);
            _positioningEnd   = new Point(0, 0);

            _textElement = textElement;

            this.Initialize();
        }

        public bool IsPositionChanged(SvgTextContentElement element)
        {
            if (_positioningElement != null && _positioningElement == element)
            {
                return (_positioningStart.Equals(_positioningEnd) == false);
            }
            return false;
        }

        public void BeginMeasure(int count)
        {
            _sizeMap = new Dictionary<double, double>(count, new DoubleEquality());

            _positioningStart   = new Point(0, 0);
            _positioningEnd     = new Point(0, 0);
            _positioningElement = null;
        }

        public void AddTextSize(Point point, double size)
        {
            if (_sizeMap == null)
            {
                _sizeMap = new Dictionary<double, double>(new DoubleEquality());
            }

            var value = Math.Round(_isVertical ? point.X : point.Y, 4);
            if (_sizeMap.ContainsKey(value))
            {
                _sizeMap[value] = _sizeMap[value] + size;
            }
            else
            {
                _sizeMap.Add(value, size);
            }
        }

        public void EndMeasure()
        {
            if (_sizeMap == null || _sizeMap.Count == 0)
            {
                return;
            }
            var textSizes = _sizeMap.Values;

            double maxTextSize = 0;

            foreach (var textSize in textSizes)
            {
                maxTextSize = Math.Max(maxTextSize, textSize);
            }
            _textRendering.SetTextWidth(maxTextSize);

            _sizeMap = null;
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            var comparer = StringComparison.OrdinalIgnoreCase;

            _isVertical = false;
            string writingMode = _textElement.GetPropertyValue("writing-mode");
            if (!string.IsNullOrWhiteSpace(writingMode) && string.Equals(writingMode, "tb", comparer))
            {
                _isVertical = true;
            }
            _isSingleText = _textElement.ChildNodes.Count == 1;

            if (_isSingleText == false)
            {
                int textNode = 0;
                int spanNode = 0;
                int pathNode = 0;

                XmlNodeList nodeList = _textElement.ChildNodes;
                int nodeCount = nodeList.Count;
                for (int i = 0; i < nodeCount; i++)
                {
                    XmlNode childNode = nodeList[i];
                    XmlNodeType nodeType = childNode.NodeType;
                    if (nodeType == XmlNodeType.Text)
                    {
                        textNode++;
                    }
                    else if (nodeType == XmlNodeType.Whitespace)
                    {
                        if (i != 0 && i != (nodeCount - 1))
                        {
                            textNode++;
                        }
                    }
                    else if (nodeType == XmlNodeType.Element)
                    {
                        string nodeName = childNode.Name;
                        if (string.Equals(nodeName, "tref", comparer))
                        {
                            //var trefNode = (SvgTRefElement)child;
                            spanNode++;
                        }
                        else if (string.Equals(nodeName, "tspan", comparer))
                        {
                            //var tspanNode = (SvgTSpanElement)child;
                            spanNode++;
                        }
                        else if (string.Equals(nodeName, "textPath", comparer))
                        {
                            //var textPathNode = (SvgTextPathElement)child;
                            pathNode++;
                        }
                    }
                }

                if (textNode == 0 && spanNode == 0)
                {
                    _isTextPath = (pathNode != 0);
                }
            }
            else
            {
                var childNode = _textElement.ChildNodes[0];
                if (string.Equals(childNode.Name, "textPath", comparer))
                {
                    _isTextPath = true;
                }
            }

            try
            {
                string xmlLang = _textElement.XmlLang;
                if (!string.IsNullOrWhiteSpace(xmlLang))
                {
                    _culture = CultureInfo.GetCultureInfo(xmlLang);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

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

        #region Private Inner Classes

        private sealed class DoubleEquality : IEqualityComparer<double>
        {
            public bool Equals(double x, double y)
            {
                var value1 = Math.Round(x, 4);
                var value2 = Math.Round(y, 4);

                return Math.Abs(value1 - value2) < 0.0001;
            }

            public int GetHashCode(double obj)
            {
                var value = Math.Round(obj, 4);
                return value.GetHashCode();
            }
        }

        #endregion
    }
}
