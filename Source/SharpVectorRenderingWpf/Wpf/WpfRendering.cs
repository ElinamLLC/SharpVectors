using System;
using System.Xml;
using System.Collections.Generic;
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

        private static Regex _reUrl  = new Regex(@"^url\((?<uri>.+)\)$", RegexOptions.Compiled);
        private static object _synch = new object();
        private static IDictionary<string, WpfRendering> _cacheRendering;

        private Geometry _clipGeometry;
        private Transform _transformMatrix;
        private Brush _maskBrush;

        private SvgUnitType _clipPathUnits;
        private SvgUnitType _maskUnits;
        private SvgUnitType _maskContentUnits;

        private bool _combineTransforms;

        #endregion

        #region Constructor and Destructor

        public WpfRendering(SvgElement element)
            : base(element)
        {
            _maskUnits         = SvgUnitType.UserSpaceOnUse;
            _clipPathUnits     = SvgUnitType.UserSpaceOnUse;
            _maskContentUnits  = SvgUnitType.UserSpaceOnUse;
            _combineTransforms = true;
        }

        #endregion

        #region Public Properties

        public Transform Transform
        {
            get {
                return _transformMatrix;
            }
            set {
                _transformMatrix = value;
            }
        }

        public Geometry ClipGeometry
        {
            get {
                return _clipGeometry;
            }
            set {
                _clipGeometry = value;
            }
        }

        public SvgUnitType ClipUnits
        {
            get {
                return _clipPathUnits;
            }
        }

        public Brush Masking
        {
            get {
                return _maskBrush;
            }
            set {
                _maskBrush = value;
            }
        }

        public SvgUnitType MaskUnits
        {
            get {
                return _maskUnits;
            }
        }

        public SvgUnitType MaskContentUnits
        {
            get {
                return _maskContentUnits;
            }
        }

        public bool CombineTransforms
        {
            get {
                return _combineTransforms;
            }
            set {
                _combineTransforms = value;
            }
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);

            _maskUnits        = SvgUnitType.UserSpaceOnUse;
            _clipPathUnits    = SvgUnitType.UserSpaceOnUse;
            _maskContentUnits = SvgUnitType.UserSpaceOnUse;

            if (renderer == null)
            {
                return;
            }

            WpfDrawingContext context = renderer.Context;

            SetQuality(context);
            SetTransform(context);
            SetClip(context);
            SetMask(context);
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            base.AfterRender(renderer);

            _clipGeometry     = null;
            _transformMatrix  = null;
            _maskBrush        = null;

            _maskUnits        = SvgUnitType.UserSpaceOnUse;
            _clipPathUnits    = SvgUnitType.UserSpaceOnUse;
            _maskContentUnits = SvgUnitType.UserSpaceOnUse;
        }

        #endregion

        #region Public Static Methods

        public static WpfRendering Create(ISvgElement element)
        {
            lock(_synch)
            {
                if (element == null)
                {
                    return null;
                }
                if (_cacheRendering == null)
                {
                    _cacheRendering = new Dictionary<string, WpfRendering>(StringComparer.Ordinal);
                }
                SvgElement svgElement = (SvgElement)element;
                string localName = svgElement.LocalName;
                if (!string.IsNullOrWhiteSpace(localName))
                {
                    if (_cacheRendering.ContainsKey(localName))
                    {
                        var wpfRendering = _cacheRendering[localName];
                        if (wpfRendering.IsReady)
                        {
                            wpfRendering.Initialize(svgElement);

                            wpfRendering.IsReady = false;
                            return wpfRendering;
                        }
                    }
                    else
                    {
                        var wpfRendering = CreateRendering(svgElement);
                        _cacheRendering.Add(localName, wpfRendering);

                        wpfRendering.IsReady = false;
                        return wpfRendering;
                    }
                }
                return CreateRendering(svgElement);
            }
        }

        private static WpfRendering CreateRendering(SvgElement svgElement)
        {
            SvgRenderingHint hint = svgElement.RenderingHint;
            // For the shapes and text contents...
            if (hint == SvgRenderingHint.Shape)
            {
                return new WpfPathRendering(svgElement);
            }
            if (hint == SvgRenderingHint.Text)
            {
                return new WpfTextRendering(svgElement);
            }

            string localName = svgElement.LocalName;
            if (string.IsNullOrWhiteSpace(localName))
            {
                return new WpfRendering(svgElement);
            }

            switch (localName)
            {
                case "svg":
                    return new WpfSvgRendering(svgElement);
                case "g":
                    return new WpfGroupRendering(svgElement);
                case "a":
                    return new WpfARendering(svgElement);
                case "use":
                    return new WpfUseRendering(svgElement);
                case "symbol":
                    return new WpfSymbolRendering(svgElement);
                case "switch":
                    return new WpfSwitchRendering(svgElement);
                case "image":
                    return new WpfImageRendering(svgElement);
                case "marker":
                    return new WpfMarkerRendering(svgElement);
                case "pattern":
                    return new WpfPatternRendering(svgElement);
            }

            return new WpfRendering(svgElement);
        }

        /// <summary>
        /// Generates a new <see cref="WpfRendering">RenderingNode</see> that
        /// corresponds to the given Uri.
        /// </summary>
        /// <param name="baseUri">
        /// The base Uri.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The generated <see cref="WpfRendering">RenderingNode</see> that
        /// corresponds to the given Uri.
        /// </returns>
        public static WpfRendering CreateByUri(SvgDocument document, string baseUri, string url)
        {
            url = url.Trim().Trim(new char[] { '\"', '\'' });
            if (url.StartsWith("#", StringComparison.OrdinalIgnoreCase))
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
            return null;
        }

        #endregion

        #region Protected Methods

        protected override void Initialize(SvgElement element)
        {
            base.Initialize(element);

            _clipGeometry     = null;
            _transformMatrix  = null;
            _maskBrush        = null;

            _maskUnits        = SvgUnitType.UserSpaceOnUse;
            _clipPathUnits    = SvgUnitType.UserSpaceOnUse;
            _maskContentUnits = SvgUnitType.UserSpaceOnUse;
        }

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
                    string.Equals(node.LocalName, "title", StringComparison.OrdinalIgnoreCase))
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
                CssValue overflow = _svgElement.GetComputedCssValue("overflow", string.Empty) as CssValue;
                // TODO: clip can have "rect(10 10 auto 10)"
                CssPrimitiveValue clip = _svgElement.GetComputedCssValue("clip", string.Empty) as CssPrimitiveValue;

                string sOverflow = null;

                if (overflow != null && !string.IsNullOrWhiteSpace(overflow.CssText))
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
                    // "If the 'overflow' property has a value other than hidden or scroll, 
                    // the property has no effect (i.e., a clipping rectangle is not created)."
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
                            else if (_svgElement is ISvgMarkerElement || _svgElement is ISvgSymbolElement ||
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
                CssPrimitiveValue clipPath = _svgElement.GetComputedCssValue("clip-path", string.Empty) as CssPrimitiveValue;

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
                                gpClip = Geometry.Combine(gpClip, geomColl[k],
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
            _maskUnits = SvgUnitType.UserSpaceOnUse;
            _maskContentUnits = SvgUnitType.UserSpaceOnUse;

            CssPrimitiveValue maskPath = _svgElement.GetComputedCssValue("mask", string.Empty) as CssPrimitiveValue;

            SvgMaskElement maskElement = null;

            if (maskPath != null && maskPath.PrimitiveType == CssPrimitiveType.Uri)
            {
                string absoluteUri = _svgElement.ResolveUri(maskPath.GetStringValue());

                maskElement = _svgElement.OwnerDocument.GetNodeByUri(absoluteUri) as SvgMaskElement;
            }
            else if (string.Equals(_svgElement.ParentNode.LocalName, "use"))
            {
                var parentElement = _svgElement.ParentNode as SvgElement;

                maskPath = parentElement.GetComputedCssValue("mask", string.Empty) as CssPrimitiveValue;

                if (maskPath != null && maskPath.PrimitiveType == CssPrimitiveType.Uri)
                {
                    string absoluteUri = _svgElement.ResolveUri(maskPath.GetStringValue());

                    maskElement = _svgElement.OwnerDocument.GetNodeByUri(absoluteUri) as SvgMaskElement;
                }
            }

            if (maskElement != null)
            {
                WpfDrawingRenderer renderer = new WpfDrawingRenderer();
                renderer.Window = _svgElement.OwnerDocument.Window as SvgWindow;

                WpfDrawingSettings settings = context.Settings.Clone();
                settings.TextAsGeometry = true;
                WpfDrawingContext maskContext = new WpfDrawingContext(true, settings);

                //maskContext.Initialize(null, context.FontFamilyVisitor, null);
                maskContext.Initialize(context.LinkVisitor, context.FontFamilyVisitor, context.ImageVisitor);

                renderer.RenderMask(maskElement, maskContext);
                DrawingGroup maskDrawing = renderer.Drawing;

                Rect bounds = new Rect(0, 0, 1, 1);
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

                DrawingBrush maskBrush = new DrawingBrush(maskDrawing);
                //tb.Viewbox = new Rect(0, 0, destRect.Width, destRect.Height);
                //tb.Viewport = new Rect(0, 0, destRect.Width, destRect.Height);
                maskBrush.Viewbox = maskDrawing.Bounds;
                maskBrush.Viewport = maskDrawing.Bounds;
                maskBrush.ViewboxUnits = BrushMappingMode.Absolute;
                maskBrush.ViewportUnits = BrushMappingMode.Absolute;
                maskBrush.TileMode = TileMode.None;
                maskBrush.Stretch = Stretch.Uniform;

                ////maskBrush.AlignmentX = AlignmentX.Center;
                ////maskBrush.AlignmentY = AlignmentY.Center;

                this.Masking = maskBrush;

                _maskUnits        = (SvgUnitType)maskElement.MaskUnits.AnimVal;
                _maskContentUnits = (SvgUnitType)maskElement.MaskContentUnits.AnimVal;
            }
        }

        private static double CalcPatternUnit(SvgMaskElement maskElement, SvgLength length,
            SvgLengthDirection dir, Rect bounds)
        {
            if (maskElement.MaskUnits.AnimVal.Equals((ushort)SvgUnitType.UserSpaceOnUse))
            {
                return length.Value;
            }
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

            //string colorRendering = _svgElement.GetComputedStringValue("color-rendering", string.Empty);
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
            //    string textRendering = _svgElement.GetComputedStringValue("text-rendering", string.Empty);
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
            //    string shapeRendering = _svgElement.GetComputedStringValue("shape-rendering", string.Empty);
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
                SvgTransformList transformList = (SvgTransformList)transElm.Transform.AnimVal;
                if (transformList.NumberOfItems != 0 && _combineTransforms == false)
                {
                    List<Transform> transforms = new List<Transform>();

                    for (uint i = 0; i < transformList.NumberOfItems; i++)
                    {
                        ISvgTransform transform = transformList.GetItem(i);
                        double[] values = transform.InputValues;
                        switch (transform.TransformType)
                        {
                            case SvgTransformType.Translate:
                                if (values.Length == 1)
                                {
                                    transforms.Add(new TranslateTransform(values[0], 0));
                                }
                                else if (values.Length == 2)
                                {
                                    transforms.Add(new TranslateTransform(values[0], values[1]));
                                }
                                break;
                            case SvgTransformType.Rotate:
                                if (values.Length == 1)
                                {
                                    transforms.Add(new RotateTransform(values[0]));
                                }
                                else if (values.Length == 3)
                                {
                                    transforms.Add(new RotateTransform(values[0], values[1], values[2]));
                                }
                                break;
                            case SvgTransformType.Scale:
                                if (values.Length == 1)
                                {
                                    transforms.Add(new ScaleTransform(values[0], values[0]));
                                }
                                else if (values.Length == 2)
                                {
                                    transforms.Add(new ScaleTransform(values[0], values[1]));
                                }
                                break;
                            case SvgTransformType.SkewX:
                                if (values.Length == 1)
                                {
                                    transforms.Add(new SkewTransform(values[0], 0));
                                }
                                break;
                            case SvgTransformType.SkewY:
                                if (values.Length == 1)
                                {
                                    transforms.Add(new SkewTransform(0, values[0]));
                                }
                                break;
                            case SvgTransformType.Matrix:
                                if (values.Length == 6)
                                {
                                    transforms.Add(new MatrixTransform(values[0], values[1], values[2], values[3], values[4], values[5]));
                                }
                                break;
                        }
                    }

                    if (transforms.Count == 1)
                    {
                        _transformMatrix = transforms[0];

                    }
                    else if(transforms.Count > 1)
                    {
                        transforms.Reverse();

                        TransformGroup transformGroup = new TransformGroup();
                        transformGroup.Children = new TransformCollection(transforms);
                        _transformMatrix = transformGroup;
                    }

                    return;
                }
                SvgMatrix svgMatrix = transformList.TotalMatrix;

                if (svgMatrix.IsIdentity)
                {
                    return;
                }

                _transformMatrix = new MatrixTransform(Math.Round(svgMatrix.A, 6), Math.Round(svgMatrix.B, 6),
                    Math.Round(svgMatrix.C, 6), Math.Round(svgMatrix.D, 6), 
                    Math.Round(svgMatrix.E, 6), Math.Round(svgMatrix.F, 6));
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

            double translateX = Math.Round(transformArray[0], 6);
            double translateY = Math.Round(transformArray[1], 6);
            double scaleX     = Math.Round(transformArray[2], 6);
            double scaleY     = Math.Round(transformArray[3], 6);

            Transform translateMatrix = null;
            Transform scaleMatrix = null;
            if (!translateX.Equals(0.0) || !translateY.Equals(0.0))
            {
                translateMatrix = new TranslateTransform(translateX, translateY);
            }
            if (!scaleX.Equals(1.0) || !scaleY.Equals(1.0))
            {
                scaleMatrix = new ScaleTransform(scaleX, scaleY);
            }

            if (translateMatrix != null && scaleMatrix != null)
            {
                // Create a TransformGroup to contain the transforms
                // and add the transforms to it.
                if (translateMatrix.Value.IsIdentity && scaleMatrix.Value.IsIdentity)
                {
                    return;
                }
                if (translateMatrix.Value.IsIdentity)
                {
                    this.Transform = scaleMatrix;
                    return;
                }
                if (scaleMatrix.Value.IsIdentity)
                {
                    this.Transform = translateMatrix;
                    return;
                }
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

            SvgRect viewBox   = (SvgRect)fitToView.ViewBox.AnimVal;
            SvgRect rectToFit = new SvgRect(elementBounds.X, elementBounds.Y, elementBounds.Width, elementBounds.Height);

            double[] transformArray = spar.FitToViewBox(viewBox, rectToFit);

            double translateX = Math.Round(transformArray[0], 6);
            double translateY = Math.Round(transformArray[1], 6);
            double scaleX     = Math.Round(transformArray[2], 6);
            double scaleY     = Math.Round(transformArray[3], 6);

            Transform translateMatrix = null;
            Transform scaleMatrix = null;
            if (!translateX.Equals(0.0) || !translateY.Equals(0.0))
            {
                translateMatrix = new TranslateTransform(translateX, translateY);
            }
            if (!scaleX.Equals(1.0) || !scaleY.Equals(1.0))
            {
                scaleMatrix = new ScaleTransform(scaleX, scaleY);
            }

            if (translateMatrix != null && scaleMatrix != null)
            {
                // Create a TransformGroup to contain the transforms
                // and add the transforms to it.
                if (translateMatrix.Value.IsIdentity && scaleMatrix.Value.IsIdentity)
                {
                    return;
                }
                if (translateMatrix.Value.IsIdentity)
                {
                    this.Transform = scaleMatrix;
                    return;
                }
                if (scaleMatrix.Value.IsIdentity)
                {
                    this.Transform = translateMatrix;
                    return;
                }
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

        protected double CalcLengthUnit(SvgLength length, SvgLengthDirection dir, Rect bounds)
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
                if (string.Equals(node.LocalName, "use"))
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

        #region Marker Methods

        protected static string ExtractMarkerUrl(string propValue)
        {
            Match match = _reUrl.Match(propValue);
            if (match.Success)
            {
                return match.Groups["uri"].Value;
            }
            return string.Empty;
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
                string markerAll       = ExtractMarkerUrl(styleElm.GetPropertyValue("marker", "marker"));

                //  The SVG specification defines three properties to reference markers: marker-start, 
                // marker -mid, marker-end. It also provides a shorthand property, marker. Using the marker 
                // property from a style sheet is equivalent to using all three (start, mid, end). 
                // However, shorthand properties cannot be used as presentation attributes.
                if (!string.IsNullOrWhiteSpace(markerAll) && !IsPresentationMarker(styleElm))
                {
                    if (string.IsNullOrWhiteSpace(markerStartUrl))
                    {
                        markerStartUrl = markerAll;
                    }
                    if (string.IsNullOrWhiteSpace(markerMiddleUrl))
                    {
                        markerMiddleUrl = markerAll;
                    }
                    if (string.IsNullOrWhiteSpace(markerEndUrl))
                    {
                        markerEndUrl = markerAll;
                    }
                }

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

        protected static bool IsPresentationMarker(SvgStyleableElement styleElm)
        {
            if (!string.IsNullOrWhiteSpace(styleElm.GetAttribute("marker")))
            {
                return true;
            }
            SvgElement parentElm = styleElm.ParentNode as SvgElement;
            if (parentElm == null)
            {
                return false;
            }
            switch (parentElm.LocalName)
            {
                case "g":
                    if (!string.IsNullOrWhiteSpace(parentElm.GetAttribute("marker")))
                    {
                        return true;
                    }
                    break;
                case "use":
                    if (!string.IsNullOrWhiteSpace(parentElm.GetAttribute("marker")))
                    {
                        return true;
                    }
                    //TODO--PAUL
                    break;
            }
            return false;
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
