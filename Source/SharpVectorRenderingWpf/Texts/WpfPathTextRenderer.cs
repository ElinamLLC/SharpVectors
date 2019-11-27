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

        public WpfPathTextRenderer(SvgTextBaseElement textElement, WpfTextRendering textRendering)
            : base(textElement, textRendering)
        {
        }

        #endregion

        #region Public Methods

        public override void RenderText(SvgTextContentElement element,
            ref Point ctp, string text, double rotate, WpfTextPlacement placement)
        {
            this.IsTextPath = true;

            this.RenderTextPath((SvgTextPathElement)element, ref ctp, rotate, placement);
        }

        public override void RenderTextRun(SvgTextContentElement element,
            ref Point ctp, string text, double rotate, WpfTextPlacement placement)
        {
            throw new NotImplementedException();
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

            Geometry textGeometry = CreateGeometry(targetPath, true);
            if (textGeometry == null)
            {
                return;
            }
            PathGeometry pathGeometry = textGeometry as PathGeometry;
            if (pathGeometry == null)
            {
                pathGeometry = textGeometry.GetFlattenedPathGeometry();
            }

            // If the path has a transform, apply it to get a transformed path...
            if (targetPath.HasAttribute("transform"))
            {
                var svgTransform = new SvgTransform(targetPath.GetAttribute("transform"));
                var svgMatrix = svgTransform.Matrix;
                if (!svgMatrix.IsIdentity)
                {
                    pathGeometry.Transform = new MatrixTransform(svgMatrix.A, svgMatrix.B, svgMatrix.C,
                        svgMatrix.D, svgMatrix.E, svgMatrix.F);

                    var transformPath = new PathGeometry();
                    transformPath.AddGeometry(pathGeometry);

                    pathGeometry = transformPath;
                }
            }

            WpfPathTextBuilder pathBuilder = new WpfPathTextBuilder(_textElement);

            pathBuilder.BeginTextPath(textPath);

            var comparer = StringComparison.OrdinalIgnoreCase;

            XmlNodeType nodeType = XmlNodeType.None;

            int nodeCount = textPath.ChildNodes.Count;
            for (int i = 0; i < nodeCount; i++)
            {
                XmlNode child = textPath.ChildNodes[i];
                nodeType = child.NodeType;
                if (nodeType == XmlNodeType.Text)
                {
                    var nodeText = GetText(textPath, child);
                    if (i == (nodeCount - 1))
                    {
                        // No need to render the last white space...
                        if (nodeCount == 1)
                        {
                            if (nodeText.StartsWith(NonBreaking, StringComparison.OrdinalIgnoreCase))
                            {
                                if (!nodeText.EndsWith(NonBreaking, StringComparison.OrdinalIgnoreCase))
                                {
                                    nodeText = nodeText.TrimEnd();
                                }
                            }
                            else if (nodeText.EndsWith(NonBreaking, StringComparison.OrdinalIgnoreCase))
                            {
                                nodeText = nodeText.TrimStart();
                            }
                            else
                            {
                                nodeText = nodeText.Trim();
                            }
                        }
                        else
                        {
                            if (!nodeText.EndsWith(NonBreaking, StringComparison.OrdinalIgnoreCase))
                            {
                                nodeText = nodeText.TrimEnd();
                            }
                        }
                    }
                    else if (i == 0)
                    {
                        nodeText = nodeText.Trim();
                    }

                    RenderTextPath(textPath, pathBuilder, nodeText, new Point(ctp.X, ctp.Y), rotate, placement);
                }
                else if (nodeType == XmlNodeType.Element)
                {
                    string nodeName = child.Name;
                    if (string.Equals(nodeName, "tref", comparer))
                    {
                        RenderTRefPath((SvgTRefElement)child, pathBuilder, ref ctp);
                    }
                    else if (string.Equals(nodeName, "tspan", comparer))
                    {
                        RenderTSpanPath((SvgTSpanElement)child, pathBuilder, ref ctp);
                    }
                }
            }

            WpfTextStringFormat stringFormat = GetTextStringFormat(textPath);

            //ISvgAnimatedLength pathOffset  = textPath.StartOffset;
            //SvgTextPathMethod pathMethod   = (SvgTextPathMethod)textPath.Method.BaseVal;
            //SvgTextPathSpacing pathSpacing = (SvgTextPathSpacing)textPath.Spacing.BaseVal;

            pathBuilder.RenderTextPath(_drawContext, pathGeometry, stringFormat.Alignment);

            pathBuilder.EndTextPath();
        }

        private void RenderTRefPath(SvgTRefElement element, WpfPathTextBuilder pathBuilder, ref Point ctp)
        {
            WpfTextPlacement placement = WpfTextPlacement.Create(element, ctp, true);
            ctp           = placement.Location;
            double rotate = placement.Rotation;
            if (!placement.HasPositions)
            {
                placement = null; // Render it useless.
            }

            this.RenderTextPath(element, pathBuilder, GetText(element),
                new Point(ctp.X, ctp.Y), rotate, placement);
        }

        private void RenderTSpanPath(SvgTSpanElement element, WpfPathTextBuilder pathBuilder, ref Point ctp)
        {
            WpfTextPlacement placement = WpfTextPlacement.Create(element, ctp, true);
            ctp           = placement.Location;
            double rotate = placement.Rotation;
            if (!placement.HasPositions)
            {
                placement = null; // Render it useless.
            }

            string sBaselineShift = element.GetPropertyValue("baseline-shift").Trim();
            double shiftBy = 0;

            var comparer = StringComparison.OrdinalIgnoreCase;

            if (sBaselineShift.Length > 0)
            {
                SvgTextBaseElement textElement = (SvgTextBaseElement)element.SelectSingleNode("ancestor::svg:text",
                    element.OwnerDocument.NamespaceManager);

                double textFontSize = GetComputedFontSize(textElement);
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

            foreach (XmlNode child in element.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Text)
                {
                    ctp.Y -= shiftBy;
                    RenderTextPath(element, pathBuilder, GetText(element, child),
                        new Point(ctp.X, ctp.Y), rotate, placement);
                    ctp.Y += shiftBy;
                }
            }
        }

        private void RenderTextPath(SvgTextContentElement element, WpfPathTextBuilder pathBuilder,
            string text, Point origin, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            WpfSvgPaint fillPaint   = new WpfSvgPaint(_context, element, "fill");
            WpfSvgPaint strokePaint = new WpfSvgPaint(_context, element, "stroke");
            Brush textBrush = fillPaint.GetBrush();
            Pen textPen     = strokePaint.GetPen();
            if (textBrush == null && textPen == null)
            {
                return;
            }

            double emSize      = GetComputedFontSize(element);
            var fontFamilyInfo = GetTextFontFamilyInfo(element);

            WpfTextStringFormat stringFormat = GetTextStringFormat(element);

            // Fix the use of Postscript fonts...
            WpfFontFamilyVisitor fontFamilyVisitor = _context.FontFamilyVisitor;
            if (!string.IsNullOrWhiteSpace(_actualFontName) && fontFamilyVisitor != null)
            {
                WpfFontFamilyInfo familyInfo = fontFamilyVisitor.Visit(_actualFontName,
                    fontFamilyInfo, _context);
                if (familyInfo != null && !familyInfo.IsEmpty)
                {
                    fontFamilyInfo = familyInfo;
                }
            }

            // Create the text builder object defined by the font family information
            WpfTextBuilder textBuilder = WpfTextBuilder.Create(fontFamilyInfo, this.TextCulture, emSize);

            textBuilder.TextDecorations = GetTextDecoration(element);
            textBuilder.TextAlignment   = stringFormat.Alignment;
            // This is character-by-character placement, text-alignment will distort the view...
            textBuilder.TextAlignment   = TextAlignment.Left;  

            // Create the path-run information holder for this render request...
            WpfPathTextRun textPathRun = new WpfPathTextRun(element, textBuilder);

            // Finally, register the path-run with path builder...
            pathBuilder.AddTextRun(textPathRun, text, origin, textBrush, textPen);
        }

        #endregion
    }
}
