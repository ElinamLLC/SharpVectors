using System;
using System.Xml;
using System.Text.RegularExpressions;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Renderers.Wpf
{
    public class WpfRendering : WpfRenderingBase
    {
        #region Private Fields

        private static Regex _reUrl = new Regex(@"^url\((?<uri>.+)\)$", RegexOptions.Compiled);

        private Geometry     _clipGeometry;
        private Transform    _transformMatrix;
        private Brush        _maskBrush;

        private SvgUnitType  _clipPathUnits;

        private SvgUnitType _maskUnits;

        private SvgUnitType _maskContentUnits;

        #endregion

        #region Constructor and Destructor

        public WpfRendering(SvgElement element)
            : base(element)
        {
            _maskUnits        = SvgUnitType.UserSpaceOnUse;
            _clipPathUnits    = SvgUnitType.UserSpaceOnUse;
            _maskContentUnits = SvgUnitType.UserSpaceOnUse;
        }

        #endregion

        #region Public Properties

        public Transform Transform
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

        public Geometry ClipGeometry
        {
            get
            {
                return _clipGeometry;
            }
            set
            {
                _clipGeometry = value;
            }
        }

        public SvgUnitType ClipUnits
        {
            get
            {
                return _clipPathUnits;
            }
        }

        public Brush Masking
        {
            get
            {
                return _maskBrush;
            }
            set
            {
                _maskBrush = value;
            }
        }

        public SvgUnitType MaskUnits
        {
            get
            {
                return _maskUnits;
            }
        }

        public SvgUnitType MaskContentUnits
        {
            get
            {
                return _maskContentUnits;
            }
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            if (renderer == null)
            {
                return;
            }

            _maskUnits        = SvgUnitType.UserSpaceOnUse;
            _clipPathUnits    = SvgUnitType.UserSpaceOnUse;
            _maskContentUnits = SvgUnitType.UserSpaceOnUse;

            WpfDrawingContext context = renderer.Context;

            SetQuality(context);
            SetTransform(context);
            SetClip(context);
            SetMask(context);
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            _clipGeometry    = null;
            _transformMatrix = null;
            _maskBrush       = null;

            _maskUnits        = SvgUnitType.UserSpaceOnUse;
            _clipPathUnits    = SvgUnitType.UserSpaceOnUse;
            _maskContentUnits = SvgUnitType.UserSpaceOnUse;
        }

        #endregion

        #region Public Static Methods

        public static WpfRendering Create(ISvgElement element)
        {
            if (element == null)
            {
                return null;
            }

            SvgRenderingHint hint = element.RenderingHint;
            // For the shapes and text contents...
            if (hint == SvgRenderingHint.Shape)
            {
                return new WpfPathRendering((SvgElement)element);
            }
            if (hint == SvgRenderingHint.Text)
            {
                return new WpfTextRendering((SvgElement)element);
            }

            string localName = element.LocalName;
            if (String.IsNullOrEmpty(localName))
            {
                return new WpfRendering((SvgElement)element);
            }

            switch (localName)
            {
                case "svg":
                    return new WpfSvgRendering((SvgElement)element);
                case "g":
                    return new WpfGroupRendering((SvgElement)element);
                case "a":
                    return new WpfARendering((SvgElement)element);
                case "use":
                    return new WpfUseRendering((SvgElement)element);
                case "switch":
                    return new WpfSwitchRendering((SvgElement)element);
                case "image":
                    return new WpfImageRendering((SvgElement)element);
                case "marker":
                    return new WpfMarkerRendering((SvgElement)element);
            }

            return new WpfRendering((SvgElement)element);
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
        public static WpfRendering CreateByUri(SvgDocument document, string baseUri, string url)
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
                return WpfRendering.Create(elm);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Protected Methods

        protected SvgTitleElement GetTitleElement()
        {
            if (_svgElement == null)
            {
                return null;
            }

            SvgTitleElement titleElement = null;
            foreach (XmlNode node in _svgElement.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element &&
                    String.Equals(node.LocalName, "title", StringComparison.OrdinalIgnoreCase))
                {
                    titleElement = node as SvgTitleElement;
                    break;
                }
            }

            return titleElement;
        }

        protected void SetClip(WpfDrawingContext context)
        {
            _clipPathUnits = SvgUnitType.UserSpaceOnUse;

            if (_svgElement == null)
            {
                return;
            }

            #region Clip with clip

            // see http://www.w3.org/TR/SVG/masking.html#OverflowAndClipProperties 
            if (_svgElement is ISvgSvgElement || _svgElement is ISvgMarkerElement ||
                _svgElement is ISvgSymbolElement || _svgElement is ISvgPatternElement)
            {
                // check overflow property
                CssValue overflow = _svgElement.GetComputedCssValue("overflow", String.Empty) as CssValue;
                // TODO: clip can have "rect(10 10 auto 10)"
                CssPrimitiveValue clip = _svgElement.GetComputedCssValue("clip", String.Empty) as CssPrimitiveValue;

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
                        Rect clipRect = Rect.Empty;
                        if (clip != null && clip.PrimitiveType == CssPrimitiveType.Rect)
                        {
                            if (_svgElement is ISvgSvgElement)
                            {
                                ISvgSvgElement svgElement = (ISvgSvgElement)_svgElement;
                                SvgRect viewPort = svgElement.Viewport as SvgRect;
                                clipRect = WpfConvert.ToRect(viewPort);
                                ICssRect clipShape = (CssRect)clip.GetRectValue();
                                if (clipShape.Top.PrimitiveType != CssPrimitiveType.Ident)
                                    clipRect.Y += clipShape.Top.GetFloatValue(CssPrimitiveType.Number);
                                if (clipShape.Left.PrimitiveType != CssPrimitiveType.Ident)
                                    clipRect.X += clipShape.Left.GetFloatValue(CssPrimitiveType.Number);
                                if (clipShape.Right.PrimitiveType != CssPrimitiveType.Ident)
                                    clipRect.Width = (clipRect.Right - clipRect.X) - clipShape.Right.GetFloatValue(CssPrimitiveType.Number);
                                if (clipShape.Bottom.PrimitiveType != CssPrimitiveType.Ident)
                                    clipRect.Height = (clipRect.Bottom - clipRect.Y) - clipShape.Bottom.GetFloatValue(CssPrimitiveType.Number);
                            }
                        }
                        else if (clip == null || (clip.PrimitiveType == CssPrimitiveType.Ident && clip.GetStringValue() == "auto"))
                        {
                            if (_svgElement is ISvgSvgElement)
                            {
                                ISvgSvgElement svgElement = (ISvgSvgElement)_svgElement;
                                SvgRect viewPort = svgElement.Viewport as SvgRect;
                                clipRect = WpfConvert.ToRect(viewPort);
                            }
                            else if (_svgElement is ISvgMarkerElement ||
                              _svgElement is ISvgSymbolElement ||
                              _svgElement is ISvgPatternElement)
                            {
                                // TODO: what to do here?
                            }
                        }
                        if (clipRect != Rect.Empty)
                        {
                            _clipGeometry = new RectangleGeometry(clipRect);
                            //gr.SetClip(clipRect);
                        }
                    }
                }
            }
            #endregion

            #region Clip with clip-path

            SvgRenderingHint hint = _svgElement.RenderingHint;

            if (hint == SvgRenderingHint.Image)
            {
            }

            // see: http://www.w3.org/TR/SVG/masking.html#EstablishingANewClippingPath

            if (hint == SvgRenderingHint.Shape || hint == SvgRenderingHint.Text ||
                hint == SvgRenderingHint.Clipping || hint == SvgRenderingHint.Masking ||
                hint == SvgRenderingHint.Containment || hint == SvgRenderingHint.Image)
            {
                CssPrimitiveValue clipPath = _svgElement.GetComputedCssValue("clip-path", String.Empty) as CssPrimitiveValue;

                if (clipPath != null && clipPath.PrimitiveType == CssPrimitiveType.Uri)
                {
                    string absoluteUri = _svgElement.ResolveUri(clipPath.GetStringValue());

                    SvgClipPathElement eClipPath = _svgElement.OwnerDocument.GetNodeByUri(absoluteUri) as SvgClipPathElement;

                    if (eClipPath != null)
                    {
                        GeometryCollection geomColl = CreateClippingRegion(eClipPath, context);
                        if (geomColl == null || geomColl.Count == 0)
                        {
                            return;
                        }
                        Geometry gpClip = geomColl[0];
                        int geomCount = geomColl.Count;
                        if (geomCount > 1)
                        {  
                            //GeometryGroup clipGroup = new GeometryGroup();
                            //clipGroup.Children.Add(gpClip);
                            for (int k = 1; k < geomCount; k++)
                            {
                                gpClip = CombinedGeometry.Combine(gpClip, geomColl[k],
                                    GeometryCombineMode.Union, null);
                                //clipGroup.Children.Add(geomColl[k]);
                            }

                            //clipGroup.Children.Reverse();

                            //gpClip = clipGroup;
                        }

                        if (gpClip == null || gpClip.IsEmpty())
                        {
                            return;
                        }

                        _clipPathUnits = (SvgUnitType)eClipPath.ClipPathUnits.AnimVal;

                        //if (_clipPathUnits == SvgUnitType.ObjectBoundingBox)
                        //{
                        //    SvgTransformableElement transElement = _svgElement as SvgTransformableElement;

                        //    if (transElement != null)
                        //    {
                        //        ISvgRect bbox = transElement.GetBBox();

                        //        // scale clipping path
                        //        gpClip.Transform = new ScaleTransform(bbox.Width, bbox.Height);
                        //        //gr.SetClip(gpClip);

                        //        // offset clip
                        //        //TODO--PAUL gr.TranslateClip((float)bbox.X, (float)bbox.Y);

                        //        _clipGeometry = gpClip;
                        //    }
                        //    else
                        //    {
                        //        throw new NotImplementedException("clip-path with SvgUnitType.ObjectBoundingBox "
                        //          + "not supported for this type of element: " + _svgElement.GetType());
                        //    }
                        //}
                        //else
                        {
                            //gr.SetClip(gpClip);

                            _clipGeometry = gpClip;
                        }
                    }
                }
            }

            #endregion
        }

        protected void SetMask(WpfDrawingContext context)
        {
            _maskUnits        = SvgUnitType.UserSpaceOnUse;  
            _maskContentUnits = SvgUnitType.UserSpaceOnUse;

            CssPrimitiveValue maskPath = _svgElement.GetComputedCssValue(
                "mask", String.Empty) as CssPrimitiveValue;

            if (maskPath != null && maskPath.PrimitiveType == CssPrimitiveType.Uri)
            {
                string absoluteUri = _svgElement.ResolveUri(maskPath.GetStringValue());

                SvgMaskElement maskElement = 
                    _svgElement.OwnerDocument.GetNodeByUri(absoluteUri) as SvgMaskElement;

                if (maskElement != null)
                {
                    WpfDrawingRenderer renderer = new WpfDrawingRenderer();
                    renderer.Window = _svgElement.OwnerDocument.Window as SvgWindow;

                    WpfDrawingSettings settings = context.Settings.Clone();
                    settings.TextAsGeometry = true;
                    WpfDrawingContext maskContext = new WpfDrawingContext(true,
                        settings);

                    //maskContext.Initialize(null, context.FontFamilyVisitor, null);
                    maskContext.Initialize(context.LinkVisitor,
                        context.FontFamilyVisitor, context.ImageVisitor);

                    renderer.RenderMask(maskElement, maskContext);
                    Drawing image = renderer.Drawing;

                    Rect bounds   = new Rect(0, 0, 1, 1);
                    //Rect destRect = GetMaskDestRect(maskElement, bounds);

                    //destRect = bounds;

                    //DrawingImage drawImage = new DrawingImage(image);

                    //DrawingVisual drawingVisual = new DrawingVisual();
                    //DrawingContext drawingContext = drawingVisual.RenderOpen();
                    //drawingContext.DrawDrawing(image);
                    //drawingContext.Close();

                    //RenderTargetBitmap drawImage = new RenderTargetBitmap((int)200,
                    //    (int)200, 96, 96, PixelFormats.Pbgra32);
                    //drawImage.Render(drawingVisual);

                    //ImageBrush imageBrush = new ImageBrush(drawImage);
                    //imageBrush.Viewbox = image.Bounds;
                    //imageBrush.Viewport = image.Bounds;
                    //imageBrush.ViewboxUnits = BrushMappingMode.Absolute;
                    //imageBrush.ViewportUnits = BrushMappingMode.Absolute;
                    //imageBrush.TileMode = TileMode.None;
                    //imageBrush.Stretch = Stretch.None;

                    //this.Masking = imageBrush;

                    DrawingBrush maskBrush = new DrawingBrush(image);
                    //tb.Viewbox = new Rect(0, 0, destRect.Width, destRect.Height);
                    //tb.Viewport = new Rect(0, 0, destRect.Width, destRect.Height);
                    maskBrush.Viewbox = image.Bounds;
                    maskBrush.Viewport = image.Bounds;
                    maskBrush.ViewboxUnits = BrushMappingMode.Absolute;
                    maskBrush.ViewportUnits = BrushMappingMode.Absolute;
                    maskBrush.TileMode = TileMode.None;
                    maskBrush.Stretch = Stretch.Uniform;

                    ////maskBrush.AlignmentX = AlignmentX.Center;
                    ////maskBrush.AlignmentY = AlignmentY.Center;

                    this.Masking      = maskBrush;

                    _maskUnits        = (SvgUnitType)maskElement.MaskUnits.AnimVal;
                    _maskContentUnits = (SvgUnitType)maskElement.MaskContentUnits.AnimVal;
                }
            }
        }

        private static double CalcPatternUnit(SvgMaskElement maskElement, SvgLength length, 
            SvgLengthDirection dir, Rect bounds)
        {
            if (maskElement.MaskUnits.AnimVal.Equals(SvgUnitType.UserSpaceOnUse))
            {
                return length.Value;
            }
            else
            {
                double calcValue = length.ValueInSpecifiedUnits;
                if (dir == SvgLengthDirection.Horizontal)
                {
                    calcValue *= bounds.Width;
                }
                else
                {
                    calcValue *= bounds.Height;
                }
                if (length.UnitType == SvgLengthType.Percentage)
                {
                    calcValue /= 100F;
                }

                return calcValue;
            }
        }

        private static Rect GetMaskDestRect(SvgMaskElement maskElement, Rect bounds)
        {
            Rect result = new Rect(0, 0, 0, 0);

            result.X = CalcPatternUnit(maskElement, maskElement.X.AnimVal as SvgLength,
                SvgLengthDirection.Horizontal, bounds);
            result.Y = CalcPatternUnit(maskElement, maskElement.Y.AnimVal as SvgLength,
                SvgLengthDirection.Vertical, bounds);

            result.Width = CalcPatternUnit(maskElement, maskElement.Width.AnimVal as SvgLength,
                SvgLengthDirection.Horizontal, bounds);
            result.Height = CalcPatternUnit(maskElement, maskElement.Height.AnimVal as SvgLength,
                SvgLengthDirection.Vertical, bounds);

            return result;
        }

        protected void SetQuality(WpfDrawingContext context)
        {
            //Graphics graphics = gr.Graphics;

            //string colorRendering = _svgElement.GetComputedStringValue("color-rendering", String.Empty);
            //switch (colorRendering)
            //{
            //    case "optimizeSpeed":
            //        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            //        break;
            //    case "optimizeQuality":
            //        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //        break;
            //    default:
            //        // "auto"
            //        // todo: could use AssumeLinear for slightly better
            //        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
            //        break;
            //}

            //if (element is SvgTextContentElement)
            //{
            //    // Unfortunately the text rendering hints are not applied because the
            //    // text path is recorded and painted to the Graphics object as a path
            //    // not as text.
            //    string textRendering = _svgElement.GetComputedStringValue("text-rendering", String.Empty);
            //    switch (textRendering)
            //    {
            //        case "optimizeSpeed":
            //            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            //            //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
            //            break;
            //        case "optimizeLegibility":
            //            graphics.SmoothingMode = SmoothingMode.HighQuality;
            //            //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            //            break;
            //        case "geometricPrecision":
            //            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //            //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            //            break;
            //        default:
            //            // "auto"
            //            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //            //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            //            break;
            //    }
            //}
            //else
            //{
            //    string shapeRendering = _svgElement.GetComputedStringValue("shape-rendering", String.Empty);
            //    switch (shapeRendering)
            //    {
            //        case "optimizeSpeed":
            //            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            //            break;
            //        case "crispEdges":
            //            graphics.SmoothingMode = SmoothingMode.None;
            //            break;
            //        case "geometricPrecision":
            //            graphics.SmoothingMode = SmoothingMode.HighQuality;
            //            break;
            //        default:
            //            // "auto"
            //            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //            break;
            //    }
            //}
        }

        protected void SetTransform(WpfDrawingContext context)
        {
            _transformMatrix = null;

            ISvgTransformable transElm = _svgElement as ISvgTransformable;
            if (transElm != null)
            {
                SvgTransformList svgTList = (SvgTransformList)transElm.Transform.AnimVal;
                SvgMatrix svgMatrix = ((SvgTransformList)transElm.Transform.AnimVal).TotalMatrix;

                if (svgMatrix.IsIdentity)
                {
                    return;
                }

                _transformMatrix = new MatrixTransform(svgMatrix.A, svgMatrix.B, svgMatrix.C,
                  svgMatrix.D, svgMatrix.E, svgMatrix.F);
            }
        }

        protected void FitToViewbox(WpfDrawingContext context, Rect elementBounds)
        {
            ISvgFitToViewBox fitToView = _svgElement as ISvgFitToViewBox;
            if (fitToView == null)
            {
                return;
            }

            SvgPreserveAspectRatio spar = (SvgPreserveAspectRatio)fitToView.PreserveAspectRatio.AnimVal;

            double[] transformArray = spar.FitToViewBox((SvgRect)fitToView.ViewBox.AnimVal,
              new SvgRect(elementBounds.X, elementBounds.Y, elementBounds.Width, elementBounds.Height));

            double translateX = transformArray[0];
            double translateY = transformArray[1];
            double scaleX     = transformArray[2];
            double scaleY     = transformArray[3];

            Transform translateMatrix = null;
            Transform scaleMatrix     = null;
            //if (translateX >= 0 && translateY >= 0)
            {
                translateMatrix = new TranslateTransform(translateX, translateY);
            }
            if ((float)scaleX != 1.0f && (float)scaleY != 1.0f)
            {
                scaleMatrix = new ScaleTransform(scaleX, scaleY);
            }

            if (translateMatrix != null && scaleMatrix != null)
            {
                // Create a TransformGroup to contain the transforms
                // and add the transforms to it.
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaleMatrix);
                transformGroup.Children.Add(translateMatrix);

                this.Transform = transformGroup;
            }
            else if (translateMatrix != null)
            {
                this.Transform = translateMatrix;
            }
            else if (scaleMatrix != null)
            {
                this.Transform = scaleMatrix;
            }
        }

        protected void FitToViewbox(WpfDrawingContext context, SvgElement svgElement, Rect elementBounds)
        {
            if (svgElement == null)
            {
                return;
            }
            ISvgFitToViewBox fitToView = svgElement as ISvgFitToViewBox;
            if (fitToView == null)
            {
                return;
            }

            SvgPreserveAspectRatio spar = (SvgPreserveAspectRatio)fitToView.PreserveAspectRatio.AnimVal;

            double[] transformArray = spar.FitToViewBox((SvgRect)fitToView.ViewBox.AnimVal,
              new SvgRect(elementBounds.X, elementBounds.Y, elementBounds.Width, elementBounds.Height));

            double translateX = transformArray[0];
            double translateY = transformArray[1];
            double scaleX     = transformArray[2];
            double scaleY     = transformArray[3];

            Transform translateMatrix = null;
            Transform scaleMatrix     = null;
            //if (translateX != 0 || translateY != 0)
            {
                translateMatrix = new TranslateTransform(translateX, translateY);
            }
            if ((float)scaleX != 1.0f && (float)scaleY != 1.0f)
            {
                scaleMatrix = new ScaleTransform(scaleX, scaleY);
            }

            if (translateMatrix != null && scaleMatrix != null)
            {
                // Create a TransformGroup to contain the transforms
                // and add the transforms to it.
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaleMatrix);
                transformGroup.Children.Add(translateMatrix);

                this.Transform = transformGroup;
            }
            else if (translateMatrix != null)
            {
                this.Transform = translateMatrix;
            }
            else if (scaleMatrix != null)
            {
                this.Transform = scaleMatrix;
            }
        }

        #endregion

        #region Private Methods

        private GeometryCollection CreateClippingRegion(SvgClipPathElement clipPath,
            WpfDrawingContext context)
        {
            GeometryCollection geomColl = new GeometryCollection();

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
                                Geometry childPath = CreateGeometry(element, context.OptimizePath);

                                if (childPath != null)
                                {
                                    geomColl.Add(childPath);
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
                    if (element != null)
                    {
                        if (element.RenderingHint == SvgRenderingHint.Shape)
                        {
                            Geometry childPath = CreateGeometry(element, context.OptimizePath);

                            if (childPath != null)
                            {
                                geomColl.Add(childPath);
                            }
                        }
                        else if (element.RenderingHint == SvgRenderingHint.Text)
                        {
                            GeometryCollection textGeomColl = GetTextClippingRegion(element, context);
                            if (textGeomColl != null)
                            {
                                for (int i = 0; i < textGeomColl.Count; i++)
                                {
                                    geomColl.Add(textGeomColl[i]);
                                }
                            }
                        }
                    }
                }
            }

            return geomColl;
        }

        private GeometryCollection GetTextClippingRegion(SvgStyleableElement element,
            WpfDrawingContext context)
        {
            GeometryCollection geomColl = new GeometryCollection();

            WpfDrawingRenderer renderer = new WpfDrawingRenderer();
            renderer.Window = _svgElement.OwnerDocument.Window as SvgWindow;

            WpfDrawingSettings settings = context.Settings.Clone();
            settings.TextAsGeometry = true;
            WpfDrawingContext clipContext = new WpfDrawingContext(true,
                settings);
            clipContext.RenderingClipRegion = true;

            clipContext.Initialize(null, context.FontFamilyVisitor, null);

            renderer.Render(element, clipContext);

            DrawingGroup rootGroup = renderer.Drawing as DrawingGroup;
            if (rootGroup != null && rootGroup.Children.Count == 1)
            {
                DrawingGroup textGroup = rootGroup.Children[0] as DrawingGroup;
                if (textGroup != null)
                {
                    ExtractGeometry(textGroup, geomColl);
                }
            }

            return geomColl;
        }

        private static void ExtractGeometry(DrawingGroup group, GeometryCollection geomColl)
        {
            if (geomColl == null)
            {
                return;
            }

            DrawingCollection drawings = group.Children;
            int textItem = drawings.Count;
            for (int i = 0; i < textItem; i++)
            {
                Drawing drawing = drawings[i];
                GeometryDrawing aDrawing = drawing as GeometryDrawing;
                if (aDrawing != null)
                {
                    Geometry aGeometry = aDrawing.Geometry;
                    if (aGeometry != null)
                    {
                        GeometryGroup geomGroup = aGeometry as GeometryGroup;
                        if (geomGroup != null)
                        {
                            GeometryCollection children = geomGroup.Children;
                            for (int j = 0; j < children.Count; j++)
                            {
                                geomColl.Add(children[j]);
                            }
                        }
                        else
                        {
                            geomColl.Add(aGeometry);
                        }
                    }
                }
                else
                {
                    DrawingGroup innerGroup = drawing as DrawingGroup;
                    if (innerGroup != null)
                    {
                        ExtractGeometry(innerGroup, geomColl);
                    }
                }
            }
        }


        #endregion

        #region Geometry Methods

        public static Geometry CreateGeometry(ISvgElement element, bool optimizePath)
        {
            if (element == null || element.RenderingHint != SvgRenderingHint.Shape)
            {
                return null;
            }

            string localName = element.LocalName;
            switch (localName)
            {
                case "ellipse":
                    return CreateGeometry((SvgEllipseElement)element);
                case "rect":
                    return CreateGeometry((SvgRectElement)element);
                case "line":
                    return CreateGeometry((SvgLineElement)element);
                case "path":
                    if (optimizePath)
                    {
                        return CreateGeometryEx((SvgPathElement)element);
                    }
                    else
                    {
                        return CreateGeometry((SvgPathElement)element);
                    }
                case "circle":
                    return CreateGeometry((SvgCircleElement)element);
                case "polyline":
                    return CreateGeometry((SvgPolylineElement)element);
                case "polygon":
                    return CreateGeometry((SvgPolygonElement)element);
            }

            return null;
        }

        #region SvgEllipseElement Geometry

        public static Geometry CreateGeometry(SvgEllipseElement element)
        {
            double _cx = Math.Round(element.Cx.AnimVal.Value, 4);
            double _cy = Math.Round(element.Cy.AnimVal.Value, 4);
            double _rx = Math.Round(element.Rx.AnimVal.Value, 4);
            double _ry = Math.Round(element.Ry.AnimVal.Value, 4);

            if (_rx <= 0 || _ry <= 0)
            {
                return null;
            }

            /*if (_cx <= 1 && _cy <= 1 && _rx <= 1 && _ry <= 1)
            {
                gp.AddEllipse(_cx-_rx, _cy-_ry, _rx*2, _ry*2);
            }
            else
            {
                gp.AddEllipse(_cx-_rx, _cy-_ry, _rx*2 - 1, _ry*2 - 1);
            }*/
            //gp.AddEllipse(_cx - _rx, _cy - _ry, _rx * 2, _ry * 2);

            EllipseGeometry geometry = new EllipseGeometry(new Point(_cx, _cy),
                _rx, _ry);

            return geometry;
        }

        #endregion

        #region SvgRectElement Geometry

        public static Geometry CreateGeometry(SvgRectElement element)
        {
            double dx     = Math.Round(element.X.AnimVal.Value, 4);
            double dy     = Math.Round(element.Y.AnimVal.Value, 4);
            double width  = Math.Round(element.Width.AnimVal.Value, 4);
            double height = Math.Round(element.Height.AnimVal.Value, 4);
            double rx = Math.Round(element.Rx.AnimVal.Value, 4);
            double ry = Math.Round(element.Ry.AnimVal.Value, 4);

            if (width <= 0 || height <= 0)
            {
                return null;
            }
            if (rx <= 0 && ry > 0)
            {
                rx = ry;
            }
            else if (rx > 0 && ry <= 0)
            {
                ry = rx;
            }

            return new RectangleGeometry(new Rect(dx, dy, width, height), rx, ry);
        }

        #endregion

        #region SvgLineElement Geometry

        public static Geometry CreateGeometry(SvgLineElement element)
        {
            return new LineGeometry(new Point(Math.Round(element.X1.AnimVal.Value, 4), 
                Math.Round(element.Y1.AnimVal.Value, 4)),
                new Point(Math.Round(element.X2.AnimVal.Value, 4), Math.Round(element.Y2.AnimVal.Value, 4)));
        }

        #endregion

        #region SvgPathElement Geometry
        
        public static Geometry CreateGeometryEx(SvgPathElement element)
        {
            PathGeometry geometry = new PathGeometry();

            string pathScript = element.PathScript;
            if (String.IsNullOrEmpty(pathScript))
            {
                return geometry;
            }

            string fillRule = element.GetPropertyValue("fill-rule");
            string clipRule = element.GetAttribute("clip-rule");
            if (!String.IsNullOrEmpty(clipRule) &&
                String.Equals(clipRule, "evenodd") || String.Equals(clipRule, "nonzero"))
            {
                fillRule = clipRule;
            }
            if (fillRule == "evenodd")
                geometry.FillRule = FillRule.EvenOdd;
            else if (fillRule == "nonzero")
                geometry.FillRule = FillRule.Nonzero;

            try
            {
                geometry.Figures = PathFigureCollection.Parse(pathScript);
            }
            catch
            {
            }

            return geometry;
        }

        public static Geometry CreateGeometry(SvgPathElement element)
        {
            PathGeometry geometry = new PathGeometry();

            string fillRule = element.GetPropertyValue("fill-rule");
            string clipRule = element.GetAttribute("clip-rule");
            if (!String.IsNullOrEmpty(clipRule) &&
                String.Equals(clipRule, "evenodd") || String.Equals(clipRule, "nonzero"))
            {
                fillRule = clipRule;
            }
            if (fillRule == "evenodd")
                geometry.FillRule = FillRule.EvenOdd;
            else if (fillRule == "nonzero")
                geometry.FillRule = FillRule.Nonzero;

            SvgPointF initPoint = new SvgPointF(0, 0);
            SvgPointF lastPoint = new SvgPointF(0, 0);

            ISvgPathSeg segment           = null;
            SvgPathSegMoveto pathMoveTo   = null;
            SvgPathSegLineto pathLineTo   = null;
            SvgPathSegCurveto pathCurveTo = null;
            SvgPathSegArc pathArc         = null;

            ISvgPathSegList segments = element.PathSegList;
            int nElems = segments.NumberOfItems;

            PathFigure pathFigure = null;

            for (int i = 0; i < nElems; i++)
            {
                segment = segments.GetItem(i);

                if (DynamicCast.Cast(segment, out pathMoveTo))
                {
                    if (pathFigure != null)
                    {
                        pathFigure.IsClosed = false;
                        pathFigure.IsFilled = true;
                        geometry.Figures.Add(pathFigure);
                        pathFigure = null;
                    }

                    lastPoint = initPoint = pathMoveTo.AbsXY;

                    pathFigure = new PathFigure();
                    pathFigure.StartPoint = new Point(initPoint.ValueX, initPoint.ValueY);
                }
                else if (DynamicCast.Cast(segment, out pathLineTo))
                {
                    SvgPointF p = pathLineTo.AbsXY;
                    pathFigure.Segments.Add(new LineSegment(new Point(p.ValueX, p.ValueY), true));

                    lastPoint = p;
                }
                else if (DynamicCast.Cast(segment, out pathCurveTo))
                {
                    SvgPointF xy   = pathCurveTo.AbsXY;
                    SvgPointF x1y1 = pathCurveTo.CubicX1Y1;
                    SvgPointF x2y2 = pathCurveTo.CubicX2Y2;
                    pathFigure.Segments.Add(new BezierSegment(new Point(x1y1.ValueX, x1y1.ValueY),
                        new Point(x2y2.ValueX, x2y2.ValueY), new Point(xy.ValueX, xy.ValueY), true));

                    lastPoint = xy;
                }
                else if (DynamicCast.Cast(segment, out pathArc))
                {
                    SvgPointF p = pathArc.AbsXY;
                    if (lastPoint.Equals(p))
                    {
                        // If the endpoints (x, y) and (x0, y0) are identical, then this
                        // is equivalent to omitting the elliptical arc segment entirely.
                    }
                    else if (pathArc.R1 == 0 || pathArc.R2 == 0)
                    {
                        // Ensure radii are valid
                        pathFigure.Segments.Add(new LineSegment(new Point(p.ValueX, p.ValueY), true));
                    }
                    else
                    {                       
                        CalculatedArcValues calcValues = pathArc.GetCalculatedArcValues();

                        pathFigure.Segments.Add(new ArcSegment(new Point(p.ValueX, p.ValueY),
                            new Size(pathArc.R1, pathArc.R2), pathArc.Angle, pathArc.LargeArcFlag,
                            pathArc.SweepFlag ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                            true));
                    }

                    lastPoint = p;
                }
                else if (segment is SvgPathSegClosePath)
                {
                    if (pathFigure != null)
                    {
                        pathFigure.IsClosed = true;
                        pathFigure.IsFilled = true;
                        geometry.Figures.Add(pathFigure);
                        pathFigure = null;
                    }

                    lastPoint = initPoint;
                }
            }

            if (pathFigure != null)
            {
                pathFigure.IsClosed = false;
                pathFigure.IsFilled = true;
                geometry.Figures.Add(pathFigure);
            }

            return geometry;
        }

        #endregion

        #region SvgCircleElement Geometry

        public static Geometry CreateGeometry(SvgCircleElement element)
        {
            double _cx = Math.Round(element.Cx.AnimVal.Value, 4);
            double _cy = Math.Round(element.Cy.AnimVal.Value, 4);
            double _r  = Math.Round(element.R.AnimVal.Value, 4);

            if (_r <= 0)
            {
                return null;
            }

            EllipseGeometry geometry = new EllipseGeometry(new Point(_cx, _cy), _r, _r);

            return geometry;
        }

        #endregion

        #region SvgPolylineElement Geometry

        public static Geometry CreateGeometry(SvgPolylineElement element)
        {
            ISvgPointList list = element.AnimatedPoints;
            ulong nElems = list.NumberOfItems;
            if (nElems == 0)
            {
                return null;
            }

            PointCollection points = new PointCollection((int)nElems);

            for (uint i = 0; i < nElems; i++)
            {
                ISvgPoint point = list.GetItem(i);
                points.Add(new Point(Math.Round(point.X, 4), Math.Round(point.Y, 4)));
            }
            PolyLineSegment polyline = new PolyLineSegment();
            polyline.Points = points;

            PathFigure polylineFigure = new PathFigure();
            polylineFigure.StartPoint = points[0];
            polylineFigure.IsClosed = false;
            polylineFigure.IsFilled = true;

            polylineFigure.Segments.Add(polyline);

            PathGeometry geometry = new PathGeometry();

            string fillRule = element.GetPropertyValue("fill-rule");
            string clipRule = element.GetAttribute("clip-rule");
            if (!String.IsNullOrEmpty(clipRule) &&
                String.Equals(clipRule, "evenodd") || String.Equals(clipRule, "nonzero"))
            {
                fillRule = clipRule;
            }
            if (fillRule == "evenodd")
                geometry.FillRule = FillRule.EvenOdd;
            else if (fillRule == "nonzero")
                geometry.FillRule = FillRule.Nonzero;

            geometry.Figures.Add(polylineFigure);

            return geometry;
        }

        #endregion

        #region SvgPolygonElement Geometry

        public static Geometry CreateGeometry(SvgPolygonElement element)
        {
            ISvgPointList list = element.AnimatedPoints;
            ulong nElems = list.NumberOfItems;
            if (nElems == 0)
            {
                return null;
            }

            PointCollection points = new PointCollection((int)nElems);

            for (uint i = 0; i < nElems; i++)
            {
                ISvgPoint point = list.GetItem(i);
                points.Add(new Point(Math.Round(point.X, 4), Math.Round(point.Y, 4)));
            }

            PolyLineSegment polyline = new PolyLineSegment();
            polyline.Points = points;

            PathFigure polylineFigure = new PathFigure();
            polylineFigure.StartPoint = points[0];
            polylineFigure.IsClosed   = true;
            polylineFigure.IsFilled   = true;

            polylineFigure.Segments.Add(polyline);

            PathGeometry geometry = new PathGeometry();

            string fillRule = element.GetPropertyValue("fill-rule");
            string clipRule = element.GetAttribute("clip-rule");
            if (!String.IsNullOrEmpty(clipRule) &&
                String.Equals(clipRule, "evenodd") || String.Equals(clipRule, "nonzero"))
            {
                fillRule = clipRule;
            }
            if (fillRule == "evenodd")
                geometry.FillRule = FillRule.EvenOdd;
            else if (fillRule == "nonzero")
                geometry.FillRule = FillRule.Nonzero;

            geometry.Figures.Add(polylineFigure);

            return geometry;
        }

        #endregion

        #endregion

        #region Marker Methods

        protected static string ExtractMarkerUrl(string propValue)
        {
            Match match = _reUrl.Match(propValue);
            if (match.Success)
            {
                return match.Groups["uri"].Value;
            }
            else
            {
                return String.Empty;
            }
        }

        protected static void RenderMarkers(WpfDrawingRenderer renderer,
            SvgStyleableElement styleElm, WpfDrawingContext gr)
        {
            // OPTIMIZE

            if (styleElm is ISharpMarkerHost)
            {
                string markerStartUrl  = ExtractMarkerUrl(styleElm.GetPropertyValue("marker-start", "marker"));
                string markerMiddleUrl = ExtractMarkerUrl(styleElm.GetPropertyValue("marker-mid", "marker"));
                string markerEndUrl    = ExtractMarkerUrl(styleElm.GetPropertyValue("marker-end", "marker"));

                if (markerStartUrl.Length > 0)
                {
                    WpfMarkerRendering markerRenderer = CreateByUri(styleElm.OwnerDocument,
                        styleElm.BaseURI, markerStartUrl) as WpfMarkerRendering;
                    if (markerRenderer != null)
                    {
                        markerRenderer.RenderMarker(renderer, gr, SvgMarkerPosition.Start, styleElm);
                    }
                }

                if (markerMiddleUrl.Length > 0)
                {
                    // TODO markerMiddleUrl != markerStartUrl
                    WpfMarkerRendering markerRenderer = CreateByUri(styleElm.OwnerDocument,
                        styleElm.BaseURI, markerMiddleUrl) as WpfMarkerRendering;
                    if (markerRenderer != null)
                    {
                        markerRenderer.RenderMarker(renderer, gr, SvgMarkerPosition.Mid, styleElm);
                    }
                }

                if (markerEndUrl.Length > 0)
                {
                    // TODO: markerEndUrl != markerMiddleUrl
                    WpfMarkerRendering markerRenderer = CreateByUri(styleElm.OwnerDocument,
                        styleElm.BaseURI, markerEndUrl) as WpfMarkerRendering;
                    if (markerRenderer != null)
                    {
                        markerRenderer.RenderMarker(renderer, gr, SvgMarkerPosition.End, styleElm);
                    }
                }
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
