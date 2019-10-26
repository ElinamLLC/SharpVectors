using System;
using System.Xml;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Texts;
using SharpVectors.Runtime;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfTextRendering : WpfRendering
    {
        #region Private Fields

        private const string Whitespace = " ";

        private readonly static Regex _tabNewline    = new Regex(@"[\n\f\t]");
        private readonly static Regex _decimalNumber = new Regex(@"^\d");

        private bool _isMeasuring;

        private bool _isGroupAdded;

        private bool _isTextPath;

        private double _textWidth;

        private SvgTextBaseElement _textElement;

        private DrawingGroup _drawGroup;
        private DrawingContext _drawContext;

        private WpfTextContext _textContext;

        private WpfHorzTextRenderer _horzRenderer;
        private WpfVertTextRenderer _vertRenderer;
        private WpfPathTextRenderer _pathRenderer;

        #endregion

        #region Constructors and Destructor

        public WpfTextRendering(SvgElement element)
            : base(element)
        {
            _textElement = element as SvgTextBaseElement;
            if (_textElement == null)
            {
                throw new InvalidOperationException();
            }

            _textContext = new WpfTextContext(_textElement, this);

            _horzRenderer = new WpfHorzTextRenderer(_textElement, this);
            _vertRenderer = new WpfVertTextRenderer(_textElement, this);
            _pathRenderer = new WpfPathTextRenderer(_textElement, this);
        }

        #endregion

        #region Public Properties

        public override bool IsRecursive
        {
            get {
                return true;
            }
        }

        public bool IsMeasuring
        {
            get {
                return _isMeasuring;
            }
        }

        public bool IsTextPath
        {
            get {
                return _isTextPath;
            }
            set {
                _isTextPath = value;
            }
        }

        public double TextWidth
        {
            get {
                return _textWidth;
            }
        }

        public WpfTextContext TextContext
        {
            get {
                return _textContext;
            }
            private set {
                _textContext = value;
            }
        }

        #endregion

        #region Public Methods

        #region BeforeRender Method

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);

            _isTextPath   = false;
            _isGroupAdded = false;
            _textWidth    = 0;
            _isMeasuring  = false;

            WpfDrawingContext context = renderer.Context;

            SvgRenderingHint hint = _svgElement.RenderingHint;
            if (hint == SvgRenderingHint.Clipping)
            {
                return;
            }

            var comparer = StringComparison.OrdinalIgnoreCase;

            // We do not directly render the contents of the clip-path, unless specifically requested...
            if (string.Equals(_svgElement.ParentNode.LocalName, "clipPath", comparer) &&
                !context.RenderingClipRegion)
            {
                return;
            }

            _context = renderer.Context;

            SetQuality(context);
            SetTransform(context);

            SetClip(_context);

            SetMask(_context);

            _drawGroup = new DrawingGroup();

            string sVisibility = _textElement.GetPropertyValue("visibility");
            string sDisplay = _textElement.GetPropertyValue("display");
            if (string.Equals(sVisibility, "hidden", comparer) || string.Equals(sDisplay, "none", comparer))
            {
                _drawGroup.Opacity = 0;
            }

            string elementId = this.GetElementName();
            if (!string.IsNullOrWhiteSpace(elementId) && !context.IsRegisteredId(elementId))
            {
                SvgObject.SetName(_drawGroup, elementId);

                context.RegisterId(elementId);

                if (context.IncludeRuntime)
                {
                    SvgObject.SetId(_drawGroup, elementId);
                }
            }

            string elementClass = this.GetElementClass();
            if (!string.IsNullOrWhiteSpace(elementClass) && context.IncludeRuntime)
            {
                SvgObject.SetClass(_drawGroup, elementClass);
            }

            Transform textTransform = this.Transform;
            if (textTransform != null && !textTransform.Value.IsIdentity)
            {
                _drawGroup.Transform = textTransform;
            }
            else
            {
                textTransform = null; // render any identity transform useless...
            }
            Geometry textClip = this.ClipGeometry;
            if (textClip != null && !textClip.IsEmpty())
            {
                _drawGroup.ClipGeometry = textClip;
            }
            else
            {
                textClip = null; // render any empty geometry useless...
            }
            Brush textMask = this.Masking;
            if (textMask != null)
            {
                _drawGroup.OpacityMask = textMask;
            }

            if (textTransform != null || textClip != null || textMask != null)
            {
                DrawingGroup curGroup = _context.Peek();
                Debug.Assert(curGroup != null);
                if (curGroup != null)
                {
                    curGroup.Children.Add(_drawGroup);
                    context.Push(_drawGroup);

                    _isGroupAdded = true;
                }
            }

            _drawContext = _drawGroup.Open();

            _horzRenderer.Initialize(_drawContext, _context);
            _vertRenderer.Initialize(_drawContext, _context);
            _pathRenderer.Initialize(_drawContext, _context);
        }

        #endregion

        #region Render Method

        public override void Render(WpfDrawingRenderer renderer)
        {
            if (_drawGroup == null || _drawContext == null)
            {
                return;
            }

            var comparer = StringComparison.OrdinalIgnoreCase;

            Point ctp = new Point(0, 0); // current text position

            WpfTextPlacement placement = WpfTextPlacement.Create(_textElement, ctp, _textContext.IsTextPath);
            ctp = placement.Location;
            double rotate = placement.Rotation;
            if (!placement.HasPositions)
            {
                placement = null; // render it useless
            }
            string sBaselineShift = _textElement.GetPropertyValue("baseline-shift").Trim();
            double shiftBy = 0;

            if (sBaselineShift.Length > 0)
            {
                double textFontSize = WpfTextRenderer.GetComputedFontSize(_textElement);
                if (sBaselineShift.EndsWith("%", comparer))
                {
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift.Substring(0,
                        sBaselineShift.Length - 1)) / 100 * textFontSize;
                }
                else if (string.Equals(sBaselineShift, "sub", comparer))
                {
                    shiftBy = -0.6F * textFontSize;
                }
                else if (string.Equals(sBaselineShift, "super", comparer))
                {
                    shiftBy = 0.6F * textFontSize;
                }
                else if (string.Equals(sBaselineShift, "baseline", comparer))
                {
                    shiftBy = 0;
                }
                else
                {
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift);
                }
            }

            // For for fonts loading in the background...
            var svgDoc = _svgElement.OwnerDocument;
            if (svgDoc.IsFontsLoaded == false)
            {
                //TODO: Use of SpinUntil is known to CPU heavy, but will work for now...
                //SpinWait.SpinUntil(() => svgDoc.IsFontsLoaded == true);

                var svgWnd = svgDoc.Window as SvgWindow;
                if (svgWnd != null)
                {
                    svgWnd.AwaitTasks("SvgDocument");
                }
            }

            XmlNodeType nodeType = XmlNodeType.None;

            bool isVertical = false;
            string writingMode = _textElement.GetPropertyValue("writing-mode");
            if (!string.IsNullOrWhiteSpace(writingMode) && string.Equals(writingMode, "tb", comparer))
            {
                isVertical = true;
            }

            if (_svgElement.ChildNodes.Count == 1)
            {
                XmlNode child = _svgElement.ChildNodes[0];
                nodeType = child.NodeType;
                if (nodeType == XmlNodeType.Text || nodeType == XmlNodeType.CDATA)
                {
                    if (isVertical)
                    {
                        ctp.X -= shiftBy;
                        RenderVertText(_textElement, ref ctp,
                            WpfTextRenderer.GetText(_textElement, child), rotate, placement);
                        ctp.X += shiftBy;
                    }
                    else
                    {
                        ctp.Y -= shiftBy;
                        RenderHorzText(_textElement, ref ctp,
                            WpfTextRenderer.GetText(_textElement, child), rotate, placement);
                        ctp.Y += shiftBy;
                    }
                }
                else if (nodeType == XmlNodeType.Element)
                {
                    string nodeName = child.Name;
                    if (string.Equals(nodeName, "tref", comparer))
                    {
                        AddTRefElementRun((SvgTRefElement)child, ref ctp, isVertical, true);
                    }
                    else if (string.Equals(nodeName, "tspan", comparer))
                    {
                        AddTSpanElementRun((SvgTSpanElement)child, ref ctp, isVertical, true);
                    }
                    else if (string.Equals(nodeName, "textPath", comparer))
                    {
                        RenderTextPath((SvgTextPathElement)child, ref ctp, rotate, placement);
                    }
                    else if (string.Equals(nodeName, "altGlyph", comparer))
                    {
                        AddAltGlyphElementRun((SvgAltGlyphElement)child, ref ctp, isVertical, true);
                    }
                }
                else if (nodeType == XmlNodeType.Whitespace)
                {
                    if (isVertical)
                    {
                        ctp.X -= shiftBy;
                        RenderVertText(_textElement, ref ctp,
                            WpfTextRenderer.GetText(_textElement, child), rotate, placement);
                        ctp.X += shiftBy;
                    }
                    else
                    {
                        ctp.Y -= shiftBy;
                        RenderHorzText(_textElement, ref ctp,
                            WpfTextRenderer.GetText(_textElement, child), rotate, placement);
                        ctp.Y += shiftBy;
                    }
                }
            }
            else
            {
                string textAnchor = _textElement.GetPropertyValue("text-anchor");

                WpfTextAnchor anchor = WpfTextAnchor.None;

                if (string.Equals(textAnchor, "middle", comparer))
                    anchor = WpfTextAnchor.Middle;
                else if (string.Equals(textAnchor, "end", comparer))
                    anchor = WpfTextAnchor.End;

                XmlNodeList nodeList = _svgElement.ChildNodes;
                int nodeCount = nodeList.Count;
                // This is a very simply hack to change centered text to left align, since for
                // text containing spans, different font weights may be applied to the spans...
                if (anchor == WpfTextAnchor.Middle)
                {
                    // Suspend the rendering...
                    _isMeasuring = true;
                    Point savedPt = new Point(ctp.X, ctp.Y);

                    _textContext.BeginMeasure(nodeCount);

                    for (int i = 0; i < nodeCount; i++)
                    {
                        XmlNode child = nodeList[i];
                        nodeType = child.NodeType;
                        //if (i == 0 && nodeType == XmlNodeType.Whitespace)
                        //{
                        //    continue;
                        //}
                        if (nodeType == XmlNodeType.Text)
                        {
                            var nodeText = WpfTextRenderer.GetText(_textElement, child);
                            if (i == (nodeCount - 1))
                            {
                                // No need to render the last white space...
                                nodeText = nodeText.TrimEnd();
                            }
                            else if (i == 0)
                            {
                                nodeText = nodeText.Trim();
                            }

                            if (isVertical)
                            {
                                ctp.X -= shiftBy;
                                RenderVertTextRun(_textElement, ref ctp, nodeText, rotate, placement);
                                ctp.X += shiftBy;
                            }
                            else
                            {
                                ctp.Y -= shiftBy;
                                RenderHorzTextRun(_textElement, ref ctp, nodeText, rotate, placement);
                                ctp.Y += shiftBy;
                            }
                        }
                        else if (nodeType == XmlNodeType.Element)
                        {
                            string nodeName = child.Name;
                            if (string.Equals(nodeName, "tref", comparer))
                            {
                                AddTRefElementRun((SvgTRefElement)child, ref ctp, isVertical, false);
                            }
                            else if (string.Equals(nodeName, "tspan", comparer))
                            {
                                bool isAdded = false;
                                if ((i + 1) < nodeCount - 1)
                                {
                                    XmlNode nextChild = nodeList[i + 1];
                                    if (nextChild.NodeType == XmlNodeType.Whitespace)
                                    {
                                        isAdded = true;
                                        AddTSpanElementRun((SvgTSpanElement)child, ref ctp, isVertical, false, nextChild);
                                        i++;
                                    }
                                }
                                if (!isAdded)
                                {
                                    AddTSpanElementRun((SvgTSpanElement)child, ref ctp, isVertical, false);
                                }
                            }
                            else if (string.Equals(nodeName, "textPath", comparer))
                            {
                                RenderTextPath((SvgTextPathElement)child, ref ctp, rotate, placement);
                            }
                            else if (string.Equals(nodeName, "altGlyph", comparer))
                            {
                                AddAltGlyphElementRun((SvgAltGlyphElement)child, ref ctp, isVertical, false);
                            }
                        }
                        else if (nodeType == XmlNodeType.Whitespace)
                        {
                            if (isVertical)
                            {
                                ctp.X -= shiftBy;
                                RenderVertTextRun(_textElement, ref ctp, Whitespace, rotate, placement, true);
                                ctp.X += shiftBy;
                            }
                            else
                            {
                                ctp.Y -= shiftBy;
                                RenderHorzTextRun(_textElement, ref ctp, Whitespace, rotate, placement, true);
                                ctp.Y += shiftBy;
                            }
                        }
                    }

                    _textContext.EndMeasure();

                    ctp = savedPt;

                    ctp.X -= (_textWidth / 2d);

                    // Resume the rendering...
                    _isMeasuring = false;
                }

                bool textRendered = false;

                for (int i = 0; i < nodeCount; i++)
                {
                    XmlNode child = nodeList[i];
                    nodeType = child.NodeType;
                    //if (i == 0 && nodeType == XmlNodeType.Whitespace)
                    //{
                    //    continue;
                    //}

                    if (nodeType == XmlNodeType.Text)
                    {
                        var nodeText = WpfTextRenderer.GetText(_textElement, child);
                        if (i == (nodeCount - 1)) 
                        {
                            // No need to render the last white space...
                            nodeText = nodeText.TrimEnd();
                        }
                        else if (i == 0)
                        {
                            nodeText = nodeText.Trim();
                        }

                        if (isVertical)
                        {
                            ctp.X -= shiftBy;
                            RenderVertTextRun(_textElement, ref ctp, nodeText, rotate, placement);
                            ctp.X += shiftBy;
                        }
                        else
                        {
                            ctp.Y -= shiftBy;
                            RenderHorzTextRun(_textElement, ref ctp, nodeText, rotate, placement);
                            ctp.Y += shiftBy;
                        }

                        textRendered = true;
                    }
                    else if (nodeType == XmlNodeType.Element)
                    {
                        string nodeName = child.Name;
                        if (string.Equals(nodeName, "tref", comparer))
                        {
                            AddTRefElementRun((SvgTRefElement)child, ref ctp, isVertical, false);

                            textRendered = true;
                        }
                        else if (string.Equals(nodeName, "tspan", comparer))
                        {
                            bool isAdded = false;
                            if ((i + 1) < nodeCount - 1)
                            {
                                XmlNode nextChild = nodeList[i + 1];
                                if (nextChild.NodeType == XmlNodeType.Whitespace)
                                {
                                    isAdded = true;
                                    AddTSpanElementRun((SvgTSpanElement)child, ref ctp, isVertical, false, nextChild);
                                    i++;
                                }
                            }
                            if (!isAdded)
                            {
                                AddTSpanElementRun((SvgTSpanElement)child, ref ctp, isVertical, false);
                            }

                            textRendered = true;
                        }
                        else if (string.Equals(nodeName, "textPath", comparer))
                        {
                            RenderTextPath((SvgTextPathElement)child, ref ctp, rotate, placement);

                            textRendered = false;
                        }
                        else if (string.Equals(nodeName, "altGlyph", comparer))
                        {
                            AddAltGlyphElementRun((SvgAltGlyphElement)child, ref ctp, isVertical, false);
                        }
                    }
                    else if (nodeType == XmlNodeType.Whitespace)
                    {
                        if (textRendered)
                        {
                            if (isVertical)
                            {
                                ctp.X -= shiftBy;
                                RenderVertTextRun(_textElement, ref ctp, Whitespace, rotate, placement, true);
                                ctp.X += shiftBy;
                            }
                            else
                            {
                                ctp.Y -= shiftBy;
                                RenderHorzTextRun(_textElement, ref ctp, Whitespace, rotate, placement, true);
                                ctp.Y += shiftBy;
                            }

                            textRendered = false;
                        }
                    }
                }
            }

            // Register this drawing with the Drawing-Document...
            if (_drawGroup != null)
            {
                this.Rendered(_drawGroup);
            }
        }

        public void SetTextWidth(double textWidth)
        {
            _textWidth = textWidth;
        }

        public void AddTextWidth(double textWidth)
        {
            _textWidth += textWidth;
        }

        #endregion

        #region AfterRender Method

        private void ResetGuidelineSet(DrawingGroup group)
        {
            if (_isTextPath)
            {
                return;
            }

            DrawingCollection drawings = group.Children;
            int itemCount = drawings.Count;
            for (int i = 0; i < itemCount; i++)
            {
                DrawingGroup childGroup = drawings[i] as DrawingGroup;
                if (childGroup != null)
                {
                    childGroup.GuidelineSet = null;

                    ResetGuidelineSet(childGroup);
                }
            }
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            if (_horzRenderer != null)
            {
                _horzRenderer.Uninitialize();
            }
            if (_vertRenderer != null)
            {
                _vertRenderer.Uninitialize();
            }
            if (_pathRenderer != null)
            {
                _pathRenderer.Uninitialize();
            }

            if (_drawContext != null)
            {
                _drawContext.Close();
                _drawContext = null;
            }

            WpfDrawingContext context = renderer.Context;

            // TODO-PAUL: Testing this for validity...
            // Remove the GuidelineSet from the groups added by the FormattedText to reduced the 
            // size of output XAML...
            if (_drawGroup != null)
            {
                ResetGuidelineSet(_drawGroup);

                if (_drawGroup.Children.Count == 1 &&
                    (_drawGroup.Transform == null && _drawGroup.ClipGeometry == null))
                {
                    DrawingGroup firstGroup = _drawGroup.Children[0] as DrawingGroup;
                    if (firstGroup != null && firstGroup.Transform == null && firstGroup.ClipGeometry == null)
                    {
                        _drawGroup.Children.RemoveAt(0);
                        foreach (var drawing in firstGroup.Children)
                        {
                            _drawGroup.Children.Add(drawing);
                        }

                        firstGroup = null;
                    }
                }
            }

            if (context.IncludeRuntime)
            {
                if (_drawGroup != null)
                {
                    // Add the element/object type...
                    SvgObject.SetType(_drawGroup, SvgObjectType.Text);

                    // Add title for tooltips, if any...
                    SvgTitleElement titleElement = _svgElement.SelectSingleNode("title") as SvgTitleElement;
                    if (titleElement != null)
                    {
                        string titleValue = titleElement.InnerText;
                        if (!string.IsNullOrWhiteSpace(titleValue))
                        {
                            SvgObject.SetTitle(_drawGroup, titleValue);
                        }
                    }
                }
            }

            if (!_isGroupAdded)
            {
                if (_drawGroup != null)
                {
                    DrawingGroup currentGroup = _context.Peek();
                    Debug.Assert(currentGroup != null);
                    if (_isTextPath || _drawGroup.Transform != null || _drawGroup.ClipGeometry != null)
                    {
                        if (currentGroup != null)
                        {
                            currentGroup.Children.Add(_drawGroup);
                        }
                    }
                    else if (_drawGroup.Children.Count != 0)
                    {
                        if (currentGroup != null)
                        {
                            currentGroup.Children.Add(_drawGroup);
                        }
                        //DrawingGroup firstGroup = _drawGroup.Children[0] as DrawingGroup;
                        //if (firstGroup != null && firstGroup.Children.Count != 0)
                        //{
                        //    if (currentGroup != null)
                        //    {
                        //        currentGroup.Children.Add(_drawGroup);
                        //    }
                        //}
                    }
                }
            }
            else
            {
                if (_drawGroup != null)
                {
                    DrawingGroup currentGroup = context.Peek();

                    if (currentGroup == null || currentGroup != _drawGroup)
                    {
                        throw new InvalidOperationException("An existing group is expected.");
                    }

                    context.Pop();
                }
            }

            _context   = null;
            _drawGroup = null;

            base.AfterRender(renderer);
        }

        #endregion

        #endregion

        #region Protected Methods

        protected override void Initialize(SvgElement element)
        {
            base.Initialize(element);

            _isMeasuring  = false;
            _isGroupAdded = false;
            _isTextPath   = false;
            _textWidth    = 0;
            _drawGroup    = null;

            _textElement = element as SvgTextBaseElement;
            if (_textElement == null)
            {
                throw new InvalidOperationException();
            }

            if (_textContext == null)
            {
                _textContext  = new WpfTextContext(_textElement, this);
            }
            else
            {
                _textContext.SetElement(_textElement);
            }

            if (_horzRenderer == null)
            {
                _horzRenderer = new WpfHorzTextRenderer(_textElement, this);
            }
            else
            {
                _horzRenderer.Uninitialize();
                _horzRenderer.SetElement(_textElement);
            }
            if (_vertRenderer == null)
            {
                _vertRenderer = new WpfVertTextRenderer(_textElement, this);
            }
            else
            {
                _vertRenderer.Uninitialize();
                _vertRenderer.SetElement(_textElement);
            }
            if (_pathRenderer == null)
            {
                _pathRenderer = new WpfPathTextRenderer(_textElement, this);
            }
            else
            {
                _pathRenderer.Uninitialize();
                _pathRenderer.SetElement(_textElement);
            }
        }

        #endregion

        #region Private Methods

        #region Horizontal Render Methods

        private void RenderHorzText(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement, bool isWhitespace = false)
        {
            if (_horzRenderer == null)
                return;
            if (string.IsNullOrWhiteSpace(text) && !isWhitespace)
                return;

            string targetText = text.Trim();
            if (placement != null)
            {
                placement.UpdatePositions(targetText);
            }

            // Force conversion to path geometry for text with surrogate pair, XmlXamlWriter cannot handle the output
            bool isGeometryMode = _context.TextAsGeometry;
            for (int i = 0; i < targetText.Length - 1; i++)
            {
                if (char.IsSurrogatePair(targetText[i], targetText[i + 1]))
                {
                    _context.TextAsGeometry = true;
                    break;
                }
            }
            _horzRenderer.RenderText(element, ref ctp, targetText, rotate, placement);

            _context.TextAsGeometry = isGeometryMode;
        }

        private void RenderHorzTextRun(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement, bool isWhitespace = false)
        {
            if (_horzRenderer == null)
                return;
            if (string.IsNullOrWhiteSpace(text) && !isWhitespace)
                return;

            if (placement != null)
            {
                placement.UpdatePositions(text);
            }

            // Force conversion to path geometry for text with surrogate pair, XmlXamlWriter cannot handle the output
            bool isGeometryMode = _context.TextAsGeometry;
            for (int i = 0; i < text.Length - 1; i++)
            {
                if (char.IsSurrogatePair(text[i], text[i + 1]))
                {
                    _context.TextAsGeometry = true;
                    break;
                }
            }
            _horzRenderer.RenderTextRun(element, ref ctp, text, rotate, placement);

            _context.TextAsGeometry = isGeometryMode;
        }

        #endregion

        #region Vertical Render Methods

        private void RenderVertText(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement, bool isWhitespace = false)
        {
            if (_vertRenderer == null)
                return;
            if (string.IsNullOrWhiteSpace(text) && !isWhitespace)
                return;

            string targetText = text.Trim();
            if (placement != null)
            {
                placement.UpdatePositions(targetText);
            }
            _vertRenderer.RenderText(element, ref ctp, targetText, rotate, placement);
        }

        private void RenderVertTextRun(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement, bool isWhitespace = false)
        {
            if (_vertRenderer == null)
                return;
            if (string.IsNullOrWhiteSpace(text) && !isWhitespace)
                return;

            if (placement != null)
            {
                placement.UpdatePositions(text);
            }
            _vertRenderer.RenderTextRun(element, ref ctp, text, rotate, placement);
        }

        #endregion

        #region Text Path Methods

        private void RenderTextPath(SvgTextPathElement textPath, ref Point ctp,
            double rotate, WpfTextPlacement placement)
        {
            if (_pathRenderer == null)
            {
                return;
            }

            _pathRenderer.RenderText(textPath, ref ctp, string.Empty, rotate, placement);
        }

        #endregion

        #region TRef/TSpan Methods

        private void AddAltGlyphElementRun(SvgAltGlyphElement element, ref Point ctp,
            bool isVertical, bool isSingleLine)
        {
            _textContext.PositioningElement = element;
            _textContext.PositioningStart = new Point(ctp.X, ctp.Y);

            WpfTextPlacement placement = WpfTextPlacement.Create(element, ctp);
            ctp = placement.Location;
            double rotate = placement.Rotation;
            if (!placement.HasPositions)
            {
                placement = null; // render it useless
            }

            _textContext.PositioningEnd = new Point(ctp.X, ctp.Y);

            if (isVertical)
            {
                if (isSingleLine)
                {
                    this.RenderVertText(element, ref ctp, WpfTextRenderer.GetText(element), rotate, placement);
                }
                else
                {
                    this.RenderVertTextRun(element, ref ctp, WpfTextRenderer.GetText(element), rotate, placement);
                }
            }
            else
            {
                if (isSingleLine)
                {
                    this.RenderHorzText(element, ref ctp, WpfTextRenderer.GetText(element), rotate, placement);
                }
                else
                {
                    this.RenderHorzTextRun(element, ref ctp, WpfTextRenderer.GetText(element), rotate, placement);
                }
            }
        }

        private void AddTRefElementRun(SvgTRefElement element, ref Point ctp,
            bool isVertical, bool isSingleLine)
        {
            _textContext.PositioningElement = element;
            _textContext.PositioningStart = new Point(ctp.X, ctp.Y);

            WpfTextPlacement placement = WpfTextPlacement.Create(element, ctp);
            ctp = placement.Location;
            double rotate = placement.Rotation;
            if (!placement.HasPositions)
            {
                placement = null; // render it useless
            }

            _textContext.PositioningEnd = new Point(ctp.X, ctp.Y);

            if (isVertical)
            {
                if (isSingleLine)
                {
                    this.RenderVertText(element, ref ctp, WpfTextRenderer.GetText(element), rotate, placement);
                }
                else
                {
                    this.RenderVertTextRun(element, ref ctp, WpfTextRenderer.GetText(element), rotate, placement);
                }
            }
            else
            {
                if (isSingleLine)
                {
                    this.RenderHorzText(element, ref ctp, WpfTextRenderer.GetText(element), rotate, placement);
                }
                else
                {
                    this.RenderHorzTextRun(element, ref ctp, WpfTextRenderer.GetText(element), rotate, placement);
                }
            }
        }

        private void AddTSpanElementRun(SvgTSpanElement element, ref Point ctp,
            bool isVertical, bool isSingleLine, XmlNode spaceNode = null)
        {
            var nodeList = element.ChildNodes;
            int nodeCount = nodeList.Count;
            if (nodeCount == 0)
            {
                return;
            }

            _textContext.PositioningElement = element;
            _textContext.PositioningStart   = new Point(ctp.X, ctp.Y);

            WpfTextPlacement placement = WpfTextPlacement.Create(element, ctp);
            ctp = placement.Location;
            double rotate = placement.Rotation;
            if (!placement.HasPositions)
            {
                placement = null; // render it useless
            }

            _textContext.PositioningEnd = new Point(ctp.X, ctp.Y);

            var comparer = StringComparison.OrdinalIgnoreCase;

            string sBaselineShift = element.GetPropertyValue("baseline-shift").Trim();
            double shiftBy = 0;

            if (sBaselineShift.Length > 0)
            {
                double textFontSize = WpfTextRenderer.GetComputedFontSize(_textElement);
                if (sBaselineShift.EndsWith("%", comparer))
                {
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift.Substring(0,
                        sBaselineShift.Length - 1)) / 100f * textFontSize;
                }
                else if (string.Equals(sBaselineShift, "sub", comparer))
                {
                    shiftBy = -0.6F * textFontSize;
                }
                else if (string.Equals(sBaselineShift, "super", comparer))
                {
                    shiftBy = 0.6F * textFontSize;
                }
                else if (string.Equals(sBaselineShift, "baseline", comparer))
                {
                    shiftBy = 0;
                }
                else
                {
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift);
                }
            }

            for (int i = 0; i < nodeCount; i++)
            {
                XmlNode child = nodeList[i];
                if (child.NodeType == XmlNodeType.Text)
                {
                    if (isVertical)
                    {
                        ctp.X += shiftBy;
                        if (isSingleLine)
                        {
                            if (i == (nodeCount - 1) && spaceNode != null)
                            {
                                RenderVertText(element, ref ctp,
                                    WpfTextRenderer.GetText(element, child, spaceNode), rotate, placement);
                            }
                            else
                            {
                                RenderVertText(element, ref ctp,
                                    WpfTextRenderer.GetText(element, child), rotate, placement);
                            }
                        }
                        else
                        {
                            if (i == (nodeCount - 1) && spaceNode != null)
                            {
                                RenderVertTextRun(element, ref ctp,
                                    WpfTextRenderer.GetText(element, child, spaceNode), rotate, placement);
                            }
                            else
                            {
                                RenderVertTextRun(element, ref ctp,
                                    WpfTextRenderer.GetText(element, child), rotate, placement);
                            }
                        }
                        ctp.X -= shiftBy;
                    }
                    else
                    {
                        ctp.Y -= shiftBy;
                        if (isSingleLine)
                        {
                            if (i == (nodeCount - 1) && spaceNode != null)
                            {
                                RenderHorzText(element, ref ctp,
                                    WpfTextRenderer.GetText(element, child, spaceNode), rotate, placement);
                            }
                            else
                            {
                                RenderHorzText(element, ref ctp,
                                    WpfTextRenderer.GetText(element, child), rotate, placement);
                            }
                        }
                        else
                        {
                            if (i == (nodeCount - 1) && spaceNode != null)
                            {
                                RenderHorzTextRun(element, ref ctp,
                                    WpfTextRenderer.GetText(element, child, spaceNode), rotate, placement);
                            }
                            else
                            {
                                RenderHorzTextRun(element, ref ctp,
                                    WpfTextRenderer.GetText(element, child), rotate, placement);
                            }
                        }
                        ctp.Y += shiftBy;
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
