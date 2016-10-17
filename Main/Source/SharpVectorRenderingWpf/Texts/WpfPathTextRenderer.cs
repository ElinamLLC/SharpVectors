using System;
using System.Xml;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Renderers.Texts
{
    public sealed class WpfPathTextRenderer : WpfTextRenderer
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public WpfPathTextRenderer(SvgTextElement textElement, WpfTextRendering textRendering)
            : base(textElement, textRendering)
        {
        }

        #endregion

        #region Public Methods

        public override void RenderSingleLineText(SvgTextContentElement element,
            ref Point ctp, string text, double rotate, WpfTextPlacement placement)
        {
            this.RenderTextPath((SvgTextPathElement)element, ref ctp, rotate, placement);
        }

        public override void RenderTextRun(SvgTextContentElement element,
            ref Point ctp, string text, double rotate, WpfTextPlacement placement)
        {
        }

        #endregion

        #region Private Methods

        private void RenderTextPath(SvgTextPathElement textPath, ref Point ctp,
            double rotate, WpfTextPlacement placement)
        {
            if (textPath.ChildNodes == null || textPath.ChildNodes.Count == 0)
            {
                return;
            }

            SvgElement targetPath = textPath.ReferencedElement as SvgElement;
            if (targetPath == null)
            {
                return;
            }

            PathGeometry pathGeometry = WpfRendering.CreateGeometry(targetPath, true) as PathGeometry;
            if (pathGeometry == null)
            {
                return;
            }

            this.IsTextPath = true;

            WpfTextOnPathDrawing pathDrawing = new WpfTextOnPathDrawing();

            pathDrawing.BeginTextPath();

            XmlNodeType nodeType = XmlNodeType.None;

            foreach (XmlNode child in textPath.ChildNodes)
            {
                nodeType = child.NodeType;
                if (nodeType == XmlNodeType.Text)
                {
                    RenderTextPath(textPath, pathDrawing, GetText(textPath, child),
                        new Point(ctp.X, ctp.Y), rotate, placement);
                }
                else if (nodeType == XmlNodeType.Element)
                {
                    string nodeName = child.Name;
                    if (String.Equals(nodeName, "tref"))
                    {
                        RenderTRefPath((SvgTRefElement)child, pathDrawing, ref ctp);
                    }
                    else if (String.Equals(nodeName, "tspan"))
                    {
                        RenderTSpanPath((SvgTSpanElement)child, pathDrawing, ref ctp);
                    }
                }
            }

            WpfTextStringFormat stringFormat = GetTextStringFormat(_textElement);

            ISvgAnimatedLength pathOffset  = textPath.StartOffset;
            SvgTextPathMethod pathMethod   = (SvgTextPathMethod)textPath.Method.BaseVal;
            SvgTextPathSpacing pathSpacing = (SvgTextPathSpacing)textPath.Spacing.BaseVal;
            pathDrawing.DrawTextPath(_textContext, pathGeometry, pathOffset,
                stringFormat.Alignment, pathMethod, pathSpacing);

            pathDrawing.EndTextPath();
        }

        private void RenderTRefPath(SvgTRefElement element, WpfTextOnPathDrawing pathDrawing,
            ref Point ctp)
        {
            WpfTextPlacement placement = GetCurrentTextPosition(element, ctp);
            ctp           = placement.Location;
            double rotate = placement.Rotation;
            if (!placement.HasPositions)
            {
                placement = null; // Render it useless.
            }

            this.RenderTextPath(element, pathDrawing, GetTRefText(element),
                new Point(ctp.X, ctp.Y), rotate, placement);
        }

        private void RenderTSpanPath(SvgTSpanElement element, WpfTextOnPathDrawing pathDrawing,
            ref Point ctp)
        {
            WpfTextPlacement placement = GetCurrentTextPosition(element, ctp);
            ctp           = placement.Location;
            double rotate = placement.Rotation;
            if (!placement.HasPositions)
            {
                placement = null; // Render it useless.
            }

            string sBaselineShift = element.GetPropertyValue("baseline-shift").Trim();
            double shiftBy = 0;

            if (sBaselineShift.Length > 0)
            {
                SvgTextElement textElement = (SvgTextElement)element.SelectSingleNode("ancestor::svg:text",
                    element.OwnerDocument.NamespaceManager);

                double textFontSize = GetComputedFontSize(textElement);
                if (sBaselineShift.EndsWith("%"))
                {
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift.Substring(0,
                        sBaselineShift.Length - 1)) / 100f * textFontSize;
                }
                else if (sBaselineShift == "sub")
                {
                    shiftBy = -0.6F * textFontSize;
                }
                else if (sBaselineShift == "super")
                {
                    shiftBy = 0.6F * textFontSize;
                }
                else if (sBaselineShift == "baseline")
                {
                    shiftBy = 0;
                }
                else
                {
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift);
                }
            }

            foreach (XmlNode child in element.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Text)
                {
                    ctp.Y -= shiftBy;
                    RenderTextPath(element, pathDrawing, GetText(element, child),
                        new Point(ctp.X, ctp.Y), rotate, placement);
                    ctp.Y += shiftBy;
                }
            }
        }

        private void RenderTextPath(SvgTextContentElement element, WpfTextOnPathDrawing pathDrawing,
            string text, Point origin, double rotate, WpfTextPlacement placement)
        {
            if (String.IsNullOrEmpty(text))
            {
                return;
            }

            double emSize         = GetComputedFontSize(element);
            FontFamily fontFamily = GetTextFontFamily(element, emSize);

            FontStyle fontStyle = GetTextFontStyle(element);
            FontWeight fontWeight = GetTextFontWeight(element);

            FontStretch fontStretch = GetTextFontStretch(element);

            WpfTextStringFormat stringFormat = GetTextStringFormat(element);

            // Fix the use of Postscript fonts...
            WpfFontFamilyVisitor fontFamilyVisitor = _drawContext.FontFamilyVisitor;
            if (!String.IsNullOrEmpty(_actualFontName) && fontFamilyVisitor != null)
            {
                WpfFontFamilyInfo currentFamily = new WpfFontFamilyInfo(fontFamily, fontWeight,
                    fontStyle, fontStretch);
                WpfFontFamilyInfo familyInfo = fontFamilyVisitor.Visit(_actualFontName,
                    currentFamily, _drawContext);
                if (familyInfo != null && !familyInfo.IsEmpty)
                {
                    fontFamily  = familyInfo.Family;
                    fontWeight  = familyInfo.Weight;
                    fontStyle   = familyInfo.Style;
                    fontStretch = familyInfo.Stretch;
                }
            }

            WpfSvgPaint fillPaint = new WpfSvgPaint(_drawContext, element, "fill");
            Brush textBrush = fillPaint.GetBrush();

            WpfSvgPaint strokePaint = new WpfSvgPaint(_drawContext, element, "stroke");
            Pen pen = strokePaint.GetPen();

            TextDecorationCollection textDecors = GetTextDecoration(element);
            TextAlignment alignment = stringFormat.Alignment;

            pathDrawing.FontSize = emSize;

            pathDrawing.FontFamily  = fontFamily;
            pathDrawing.FontWeight  = fontWeight;
            pathDrawing.FontStretch = fontStretch;

            pathDrawing.Foreground = textBrush;

            pathDrawing.AddTextPath(text, origin);
        }

        #endregion
    }
}
