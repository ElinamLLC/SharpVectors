using System;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    public class GdiRendering : GdiRenderingBase
    {
        #region Private Fields

        private Matrix _transformMatrix;
        internal Color _uniqueColor;
        internal GdiGraphicsContainer _graphicsContainer;

        #endregion

        #region Constructors and Destructor

        public GdiRendering(SvgElement element)
            : base(element)
        {
            _uniqueColor = Color.Empty;
        }

        #endregion

        #region Public Properties

        public Color UniqueColor
        {
            get
            {
                return _uniqueColor;
            }
        }

        public Matrix TransformMatrix
        {
            get
            {
                return _transformMatrix;
            }
            set
            {
                _transformMatrix = value;
            }
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(GdiGraphicsRenderer renderer)
        {
            if (renderer == null)
            {
                return;
            }

            if (_uniqueColor.IsEmpty)
                _uniqueColor = renderer.GetNextColor(element);

            GdiGraphicsWrapper graphics = renderer.GraphicsWrapper;
            if (graphics == null)
            {
                return;
            }

            _graphicsContainer = graphics.BeginContainer();
            SetQuality(graphics);
            Transform(graphics);
            Clip(graphics);
        }

        public override void AfterRender(GdiGraphicsRenderer renderer)
        {
            if (renderer == null)
            {
                return;
            }

            GdiGraphicsWrapper graphics = renderer.GraphicsWrapper;
            if (graphics == null)
            {
                return;
            }

            graphics.EndContainer(_graphicsContainer);
        }

        #endregion

        #region Public Static Methods

        public static GdiRendering Create(ISvgElement element)
        {
            if (element == null)
            {
                return null;
            }

            SvgRenderingHint hint = element.RenderingHint;
            // For the shapes and text contents...
            if (hint == SvgRenderingHint.Shape)
            {
                return new GdiPathRendering((SvgElement)element);
            }
            if (hint == SvgRenderingHint.Text)
            {
                return new GdiTextRendering((SvgElement)element);
            }

            string localName = element.LocalName;
            if (String.IsNullOrEmpty(localName))
            {
                return new GdiRendering((SvgElement)element);
            }

            switch (localName)
            {
                case "svg":
                    return new GdiRootRendering((SvgElement)element);
                case "image":
                    return new GdiImageRendering((SvgElement)element);
                case "marker":
                    return new GdiMarkerRendering((SvgElement)element);
            }

            return new GdiRendering((SvgElement)element);
        }

        /// <summary>
        /// Generates a new <see cref="RenderingNode">RenderingNode</see> that
        /// corresponds to the given Uri.
        /// </summary>
        /// <param name="baseUri">
        /// The base Uri.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The generated <see cref="RenderingNode">RenderingNode</see> that
        /// corresponds to the given Uri.
        /// </returns>
        public static GdiRendering CreateByUri(SvgDocument document, string baseUri, string url)
        {
            if (url.StartsWith("#"))
            {
                // do nothing
            }
            else if (baseUri != "")
            {
                Uri absoluteUri = new Uri(new Uri(baseUri), url);
                url = absoluteUri.AbsoluteUri;
            }
            else
            {
                // TODO: Handle xml:base here?        
                // Right now just skip this... it can't be resolved, must assume it is absolute
            }
            ISvgElement elm = document.GetNodeByUri(url) as ISvgElement;

            if (elm != null)
            {
                return GdiRendering.Create(elm);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Protected Methods

        protected void Clip(GdiGraphicsWrapper gr)
        {
            if (element == null)
            {
                return;
            }

            SvgRenderingHint hint = element.RenderingHint;

            // todo: should we correct the clipping to adjust to the off-one-pixel drawing?
            gr.TranslateClip(1, 1);

            #region Clip with clip
            // see http://www.w3.org/TR/SVG/masking.html#OverflowAndClipProperties 
            if (element is ISvgSvgElement    || element is ISvgMarkerElement ||
                element is ISvgSymbolElement || element is ISvgPatternElement)
            {
                // check overflow property
                CssValue overflow = ((SvgElement)element).GetComputedCssValue("overflow", String.Empty) as CssValue;
                // TODO: clip can have "rect(10 10 auto 10)"
                CssPrimitiveValue clip = ((SvgElement)element).GetComputedCssValue("clip", String.Empty) as CssPrimitiveValue;

                string sOverflow = null;

                if (overflow != null || overflow.CssText == "")
                {
                    sOverflow = overflow.CssText;
                }
                else
                {
                    if (this is ISvgSvgElement)
                        sOverflow = "hidden";
                }

                if (sOverflow != null)
                {
                    // "If the 'overflow' property has a value other than hidden or scroll, the property has no effect (i.e., a clipping rectangle is not created)."
                    if (sOverflow == "hidden" || sOverflow == "scroll")
                    {
                        RectangleF clipRect = RectangleF.Empty;
                        if (clip != null && clip.PrimitiveType == CssPrimitiveType.Rect)
                        {
                            if (element is ISvgSvgElement)
                            {
                                ISvgSvgElement svgElement = (ISvgSvgElement)element;
                                SvgRect viewPort = svgElement.Viewport as SvgRect;
                                clipRect = GdiConverter.ToRectangle(viewPort);
                                ICssRect clipShape = (CssRect)clip.GetRectValue();
                                if (clipShape.Top.PrimitiveType != CssPrimitiveType.Ident)
                                    clipRect.Y += (float)clipShape.Top.GetFloatValue(CssPrimitiveType.Number);
                                if (clipShape.Left.PrimitiveType != CssPrimitiveType.Ident)
                                    clipRect.X += (float)clipShape.Left.GetFloatValue(CssPrimitiveType.Number);
                                if (clipShape.Right.PrimitiveType != CssPrimitiveType.Ident)
                                    clipRect.Width = (clipRect.Right - clipRect.X) - (float)clipShape.Right.GetFloatValue(CssPrimitiveType.Number);
                                if (clipShape.Bottom.PrimitiveType != CssPrimitiveType.Ident)
                                    clipRect.Height = (clipRect.Bottom - clipRect.Y) - (float)clipShape.Bottom.GetFloatValue(CssPrimitiveType.Number);
                            }
                        }
                        else if (clip == null || (clip.PrimitiveType == CssPrimitiveType.Ident && clip.GetStringValue() == "auto"))
                        {
                            if (element is ISvgSvgElement)
                            {
                                ISvgSvgElement svgElement = (ISvgSvgElement)element;
                                SvgRect viewPort = svgElement.Viewport as SvgRect;
                                clipRect = GdiConverter.ToRectangle(viewPort);
                            }
                            else if (element is ISvgMarkerElement ||
                              element is ISvgSymbolElement ||
                              element is ISvgPatternElement)
                            {
                                // TODO: what to do here?
                            }
                        }
                        if (clipRect != RectangleF.Empty)
                        {
                            gr.SetClip(clipRect);
                        }
                    }
                }
            }
            #endregion

            #region Clip with clip-path

            // see: http://www.w3.org/TR/SVG/masking.html#EstablishingANewClippingPath

            if (hint == SvgRenderingHint.Shape || hint == SvgRenderingHint.Text || 
                hint == SvgRenderingHint.Clipping || hint == SvgRenderingHint.Masking ||
                hint == SvgRenderingHint.Containment)
            {
                CssPrimitiveValue clipPath = ((SvgElement)element).GetComputedCssValue("clip-path", String.Empty) as CssPrimitiveValue;

                if (clipPath != null && clipPath.PrimitiveType == CssPrimitiveType.Uri)
                {
                    string absoluteUri = ((SvgElement)element).ResolveUri(clipPath.GetStringValue());

                    SvgClipPathElement eClipPath = element.OwnerDocument.GetNodeByUri(absoluteUri) as SvgClipPathElement;

                    if (eClipPath != null)
                    {
                        GraphicsPath gpClip = CreateClippingRegion(eClipPath);

                        RectangleF clipBounds = gpClip != null ? gpClip.GetBounds() : RectangleF.Empty;

                        if (clipBounds.Width == 0 || clipBounds.Height == 0)
                        {
                            return;
                        }

                        SvgUnitType pathUnits = (SvgUnitType)eClipPath.ClipPathUnits.AnimVal;

                        if (pathUnits == SvgUnitType.ObjectBoundingBox)
                        {
                            SvgTransformableElement transElement = element as SvgTransformableElement;

                            if (transElement != null)
                            {
                                ISvgRect bbox = transElement.GetBBox();

                                // scale clipping path
                                Matrix matrix = new Matrix();
                                matrix.Scale((float)bbox.Width, (float)bbox.Height);
                                gpClip.Transform(matrix);
                                gr.SetClip(gpClip);

                                // offset clip
                                gr.TranslateClip((float)bbox.X, (float)bbox.Y);
                            }
                            else
                            {
                                throw new NotImplementedException("clip-path with SvgUnitType.ObjectBoundingBox "
                                  + "not supported for this type of element: " + element.GetType());
                            }
                        }
                        else
                        {
                            gr.SetClip(gpClip);
                        }

                        gpClip.Dispose();
                        gpClip = null;
                    }
                }
            }
            #endregion
        }

        protected void SetQuality(GdiGraphicsWrapper gr)
        {
            Graphics graphics = gr.Graphics;

            string colorRendering = ((SvgElement)element).GetComputedStringValue("color-rendering", String.Empty);
            switch (colorRendering)
            {
                case "optimizeSpeed":
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                    break;
                case "optimizeQuality":
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    break;
                default:
                    // "auto"
                    // todo: could use AssumeLinear for slightly better
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
                    break;
            }

            if (element is SvgTextContentElement)
            {
                // Unfortunately the text rendering hints are not applied because the
                // text path is recorded and painted to the Graphics object as a path
                // not as text.
                string textRendering = ((SvgElement)element).GetComputedStringValue("text-rendering", String.Empty);
                switch (textRendering)
                {
                    case "optimizeSpeed":
                        graphics.SmoothingMode = SmoothingMode.HighSpeed;
                        //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                        break;
                    case "optimizeLegibility":
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        break;
                    case "geometricPrecision":
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        break;
                    default:
                        // "auto"
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                        break;
                }
            }
            else
            {
                string shapeRendering = ((SvgElement)element).GetComputedStringValue("shape-rendering", String.Empty);
                switch (shapeRendering)
                {
                    case "optimizeSpeed":
                        graphics.SmoothingMode = SmoothingMode.HighSpeed;
                        break;
                    case "crispEdges":
                        graphics.SmoothingMode = SmoothingMode.None;
                        break;
                    case "geometricPrecision":
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        break;
                    default:
                        // "auto"
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        break;
                }
            }
        }

        protected void Transform(GdiGraphicsWrapper gr)
        {
            if (element is ISvgTransformable)
            {
                if (_transformMatrix == null)
                {
                    ISvgTransformable transElm = (ISvgTransformable)element;
                    SvgTransformList svgTList  = (SvgTransformList)transElm.Transform.AnimVal;
                    SvgMatrix svgMatrix = ((SvgTransformList)transElm.Transform.AnimVal).TotalMatrix;

                    _transformMatrix = new Matrix((float)svgMatrix.A, (float)svgMatrix.B, (float)svgMatrix.C,
                      (float)svgMatrix.D, (float)svgMatrix.E, (float)svgMatrix.F);
                }
                gr.Transform = _transformMatrix;
            }
        }

        protected void FitToViewbox(GdiGraphicsWrapper graphics, RectangleF elmRect)
        {
            if (element is ISvgFitToViewBox)
            {
                ISvgFitToViewBox fitToVBElm = (ISvgFitToViewBox)element;
                SvgPreserveAspectRatio spar = (SvgPreserveAspectRatio)fitToVBElm.PreserveAspectRatio.AnimVal;

                double[] translateAndScale = spar.FitToViewBox(
                  (SvgRect)fitToVBElm.ViewBox.AnimVal,
                  new SvgRect(elmRect.X, elmRect.Y, elmRect.Width, elmRect.Height));
                graphics.TranslateTransform((float)translateAndScale[0], (float)translateAndScale[1]);
                graphics.ScaleTransform((float)translateAndScale[2], (float)translateAndScale[3]);
            }
        }

        #endregion

        #region Private Methods

        private GraphicsPath CreateClippingRegion(SvgClipPathElement clipPath)
        {
            GraphicsPath path = new GraphicsPath();

            foreach (XmlNode node in clipPath.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                // Handle a case where the clip element has "use" element as a child...
                if (String.Equals(node.LocalName, "use"))
                {
                    SvgUseElement useElement = (SvgUseElement)node;

                    XmlElement refEl = useElement.ReferencedElement;
                    if (refEl != null)
                    {
                        XmlElement refElParent = (XmlElement)refEl.ParentNode;
                        useElement.OwnerDocument.Static = true;
                        useElement.CopyToReferencedElement(refEl);
                        refElParent.RemoveChild(refEl);
                        useElement.AppendChild(refEl);

                        foreach (XmlNode useChild in useElement.ChildNodes)
                        {
                            if (useChild.NodeType != XmlNodeType.Element)
                            {
                                continue;
                            }

                            SvgStyleableElement element = useChild as SvgStyleableElement;
                            if (element != null && element.RenderingHint == SvgRenderingHint.Shape)
                            {
                                GraphicsPath childPath = CreatePath(element);

                                if (childPath != null)
                                {
                                    string clipRule = element.GetPropertyValue("clip-rule");
                                    path.FillMode = (clipRule == "evenodd") ? FillMode.Alternate : FillMode.Winding;

                                    path.AddPath(childPath, true);
                                }
                            }
                        }

                        useElement.RemoveChild(refEl);
                        useElement.RestoreReferencedElement(refEl);
                        refElParent.AppendChild(refEl);
                        useElement.OwnerDocument.Static = false;
                    }
                }
                else
                {
                    SvgStyleableElement element = node as SvgStyleableElement;
                    if (element != null && element.RenderingHint == SvgRenderingHint.Shape)
                    {
                        GraphicsPath childPath = CreatePath(element);

                        if (childPath != null)
                        {
                            string clipRule = element.GetPropertyValue("clip-rule");
                            path.FillMode = (clipRule == "evenodd") ? FillMode.Alternate : FillMode.Winding;

                            path.AddPath(childPath, true);
                        }
                    }
                }
            }

            return path;
        }

        #endregion

        #region GraphicsPath Methods

        public static GraphicsPath CreatePath(ISvgElement element)
        {
            if (element == null || element.RenderingHint != SvgRenderingHint.Shape)
            {
                return null;
            }

            string localName = element.LocalName;
            switch (localName)
            {
                case "ellipse":
                    return CreatePath((SvgEllipseElement)element);
                case "rect":
                    return CreatePath((SvgRectElement)element);
                case "line":
                    return CreatePath((SvgLineElement)element);
                case "path":
                    return CreatePath((SvgPathElement)element);
                case "circle":
                    return CreatePath((SvgCircleElement)element);
                case "polyline":
                    return CreatePath((SvgPolylineElement)element);
                case "polygon":
                    return CreatePath((SvgPolygonElement)element);
            }

            return null;
        }

        #region SvgEllipseElement Path

        public static GraphicsPath CreatePath(SvgEllipseElement element)
        {
            GraphicsPath gp = new GraphicsPath();
            float _cx = (float)element.Cx.AnimVal.Value;
            float _cy = (float)element.Cy.AnimVal.Value;
            float _rx = (float)element.Rx.AnimVal.Value;
            float _ry = (float)element.Ry.AnimVal.Value;

            /*if (_cx <= 1 && _cy <= 1 && _rx <= 1 && _ry <= 1)
            {
                gp.AddEllipse(_cx-_rx, _cy-_ry, _rx*2, _ry*2);
            }
            else
            {
                gp.AddEllipse(_cx-_rx, _cy-_ry, _rx*2 - 1, _ry*2 - 1);
            }*/
            gp.AddEllipse(_cx - _rx, _cy - _ry, _rx * 2, _ry * 2);

            return gp;
        }

        #endregion

        #region SvgRectElement Path

        public static GraphicsPath CreatePath(SvgRectElement element)
        {
            GraphicsPath gp = new GraphicsPath();

            RectangleF rect = new RectangleF((float)element.X.AnimVal.Value,
                (float)element.Y.AnimVal.Value, (float)element.Width.AnimVal.Value,
                (float)element.Height.AnimVal.Value);

            float rx = (float)element.Rx.AnimVal.Value;
            float ry = (float)element.Ry.AnimVal.Value;

            if (rx == 0F && ry == 0F)
            {
                gp.AddRectangle(rect);
            }
            else
            {
                AddRoundedRect(gp, rect, rx, ry);
            }

            return gp;
        }

        public static void AddRoundedRect(GraphicsPath path, RectangleF rect, float rx, float ry)
        {
            if (rx == 0F) rx = ry;
            else if (ry == 0F) ry = rx;

            rx = Math.Min(rect.Width / 2, rx);
            ry = Math.Min(rect.Height / 2, ry);

            float a = rect.X + rect.Width - rx;
            path.AddLine(rect.X + rx, rect.Y, a, rect.Y);
            path.AddArc(a - rx, rect.Y, rx * 2, ry * 2, 270, 90);

            float right = rect.X + rect.Width;	// rightmost X
            float b = rect.Y + rect.Height - ry;

            path.AddLine(right, rect.Y + ry, right, b);
            path.AddArc(right - rx * 2, b - ry, rx * 2, ry * 2, 0, 90);

            path.AddLine(right - rx, rect.Y + rect.Height, rect.X + rx, rect.Y + rect.Height);
            path.AddArc(rect.X, b - ry, rx * 2, ry * 2, 90, 90);

            path.AddLine(rect.X, b, rect.X, rect.Y + ry);
            path.AddArc(rect.X, rect.Y, rx * 2, ry * 2, 180, 90);
        }

        #endregion

        #region SvgLineElement Path

        public static GraphicsPath CreatePath(SvgLineElement element)
        {
            GraphicsPath gp = new GraphicsPath();

            gp.AddLine((float)element.X1.AnimVal.Value, (float)element.Y1.AnimVal.Value,
                (float)element.X2.AnimVal.Value, (float)element.Y2.AnimVal.Value);

            return gp;
        }

        #endregion

        #region SvgPathElement Path

        public static GraphicsPath CreatePath(SvgPathElement element)
        {
            GraphicsPath gp = new GraphicsPath();

            SvgPointF initPoint = new SvgPointF(0, 0);
            SvgPointF lastPoint = new SvgPointF(0, 0);

            ISvgPathSeg segment = null;
            SvgPathSegMoveto pathMoveTo = null;
            SvgPathSegLineto pathLineTo = null;
            SvgPathSegCurveto pathCurveTo = null;
            SvgPathSegArc pathArc = null;

            ISvgPathSegList segments = element.PathSegList;
            int nElems = segments.NumberOfItems;

            for (int i = 0; i < nElems; i++)
            {
                segment = segments.GetItem(i);

                if (DynamicCast.Cast(segment, out pathMoveTo))
                {
                    //SvgPathSegMoveto seg = (SvgPathSegMoveto)segment;
                    gp.StartFigure();
                    lastPoint = initPoint = pathMoveTo.AbsXY;
                }
                else if (DynamicCast.Cast(segment, out pathLineTo))
                {
                    //SvgPathSegLineto seg = (SvgPathSegLineto)segment;
                    SvgPointF p = pathLineTo.AbsXY;
                    gp.AddLine(lastPoint.X, lastPoint.Y, p.X, p.Y);

                    lastPoint = p;
                }
                else if (DynamicCast.Cast(segment, out pathCurveTo))
                {
                    // SvgPathSegCurveto seg = (SvgPathSegCurveto)segment;
                    SvgPointF xy = pathCurveTo.AbsXY;
                    SvgPointF x1y1 = pathCurveTo.CubicX1Y1;
                    SvgPointF x2y2 = pathCurveTo.CubicX2Y2;
                    gp.AddBezier(lastPoint.X, lastPoint.Y, x1y1.X, x1y1.Y, x2y2.X, x2y2.Y, xy.X, xy.Y);

                    lastPoint = xy;
                }
                else if (DynamicCast.Cast(segment, out pathArc))
                {
                    //SvgPathSegArc seg = (SvgPathSegArc)segment;
                    SvgPointF p = pathArc.AbsXY;
                    if (lastPoint.Equals(p))
                    {
                        // If the endpoints (x, y) and (x0, y0) are identical, then this
                        // is equivalent to omitting the elliptical arc segment entirely.
                    }
                    else if (pathArc.R1 == 0 || pathArc.R2 == 0)
                    {
                        // Ensure radii are valid
                        gp.AddLine(lastPoint.X, lastPoint.Y, p.X, p.Y);
                    }
                    else
                    {
                        CalculatedArcValues calcValues = pathArc.GetCalculatedArcValues();

                        GraphicsPath gp2 = new GraphicsPath();
                        gp2.StartFigure();
                        gp2.AddArc((float)(calcValues.Cx - calcValues.CorrRx),
                            (float)(calcValues.Cy - calcValues.CorrRy),
                            (float)calcValues.CorrRx * 2, (float)calcValues.CorrRy * 2,
                            (float)calcValues.AngleStart, (float)calcValues.AngleExtent);

                        Matrix matrix = new Matrix();
                        matrix.Translate(-(float)calcValues.Cx, -(float)calcValues.Cy);
                        gp2.Transform(matrix);

                        matrix = new Matrix();
                        matrix.Rotate((float)pathArc.Angle);
                        gp2.Transform(matrix);

                        matrix = new Matrix();
                        matrix.Translate((float)calcValues.Cx, (float)calcValues.Cy);
                        gp2.Transform(matrix);

                        gp.AddPath(gp2, true);
                    }

                    lastPoint = p;
                }
                else if (segment is SvgPathSegClosePath)
                {
                    gp.CloseFigure();
                    lastPoint = initPoint;
                }
            }

            string fillRule = element.GetPropertyValue("fill-rule");
            if (fillRule == "evenodd")
                gp.FillMode = FillMode.Alternate;
            else
                gp.FillMode = FillMode.Winding;

            return gp;
        }

        #endregion

        #region SvgCircleElement Path

        public static GraphicsPath CreatePath(SvgCircleElement element)
        {
            GraphicsPath gp = new GraphicsPath();

            float _cx = (float)element.Cx.AnimVal.Value;
            float _cy = (float)element.Cy.AnimVal.Value;
            float _r = (float)element.R.AnimVal.Value;

            gp.AddEllipse(_cx - _r, _cy - _r, _r * 2, _r * 2);

            return gp;
        }

        #endregion

        #region SvgPolylineElement Path

        public static GraphicsPath CreatePath(SvgPolylineElement element)
        {
            GraphicsPath gp = new GraphicsPath();

            ISvgPointList list = element.AnimatedPoints;
            ulong nElems = list.NumberOfItems;

            PointF[] points = new PointF[nElems];

            for (uint i = 0; i < nElems; i++)
            {
                points[i] = new PointF((float)list.GetItem(i).X, (float)list.GetItem(i).Y);
            }

            gp.AddLines(points);

            string fillRule = element.GetPropertyValue("fill-rule");
            if (fillRule == "evenodd")
                gp.FillMode = FillMode.Alternate;
            else
                gp.FillMode = FillMode.Winding;

            return gp;
        }

        #endregion

        #region SvgPolygonElement Path

        public static GraphicsPath CreatePath(SvgPolygonElement element)
        {
            GraphicsPath gp = new GraphicsPath();

            ISvgPointList list = element.AnimatedPoints;
            ulong nElems = list.NumberOfItems;

            PointF[] points = new PointF[nElems];

            for (uint i = 0; i < nElems; i++)
            {
                points[i] = new PointF((float)list.GetItem(i).X, (float)list.GetItem(i).Y);
            }

            gp.AddPolygon(points);

            string fillRule = element.GetPropertyValue("fill-rule");
            if (fillRule == "evenodd")
                gp.FillMode = FillMode.Alternate;
            else
                gp.FillMode = FillMode.Winding;

            return gp;
        }

        #endregion

        #endregion

        #region Marker Methods

        protected static string ExtractMarkerUrl(string propValue)
        {
            Regex reUrl = new Regex(@"^url\((?<uri>.+)\)$");
            Match match = reUrl.Match(propValue);
            if (match.Success)
            {
                return match.Groups["uri"].Value;
            }
            else
            {
                return String.Empty;
            }
        }

        protected static void PaintMarkers(GdiGraphicsRenderer renderer,
            SvgStyleableElement styleElm, GdiGraphicsWrapper gr)
        {
            // OPTIMIZE

            if (styleElm is ISharpMarkerHost)
            {
                string markerStartUrl = ExtractMarkerUrl(styleElm.GetPropertyValue("marker-start", "marker"));
                string markerMiddleUrl = ExtractMarkerUrl(styleElm.GetPropertyValue("marker-mid", "marker"));
                string markerEndUrl = ExtractMarkerUrl(styleElm.GetPropertyValue("marker-end", "marker"));

                if (markerStartUrl.Length > 0)
                {
                    GdiMarkerRendering grNode = CreateByUri(styleElm.OwnerDocument,
                        styleElm.BaseURI, markerStartUrl) as GdiMarkerRendering;
                    if (grNode != null)
                    {
                        grNode.PaintMarker(renderer, gr, SvgMarkerPosition.Start, styleElm);
                    }
                }

                if (markerMiddleUrl.Length > 0)
                {
                    // TODO markerMiddleUrl != markerStartUrl
                    GdiMarkerRendering grNode = CreateByUri(styleElm.OwnerDocument,
                        styleElm.BaseURI, markerMiddleUrl) as GdiMarkerRendering;
                    if (grNode != null)
                    {
                        grNode.PaintMarker(renderer, gr, SvgMarkerPosition.Mid, styleElm);
                    }
                }

                if (markerEndUrl.Length > 0)
                {
                    // TODO: markerEndUrl != markerMiddleUrl
                    GdiMarkerRendering grNode = CreateByUri(styleElm.OwnerDocument,
                        styleElm.BaseURI, markerEndUrl) as GdiMarkerRendering;
                    if (grNode != null)
                    {
                        grNode.PaintMarker(renderer, gr, SvgMarkerPosition.End, styleElm);
                    }
                }
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_transformMatrix != null)
                {
                    _transformMatrix.Dispose();
                    _transformMatrix = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
