using System;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Dom.Svg;
using SharpVectors.Runtime;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfImageRendering : WpfRendering
    {
        #region Private Fields

        private bool _idAssigned;
        private WpfDrawingRenderer _embeddedRenderer;

        #endregion

        #region Constructors and Destructor

        public WpfImageRendering(SvgElement element) 
            : base(element)
        {
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);

            string imageId = _svgElement.Id;
            if (string.IsNullOrWhiteSpace(imageId))
            {
                _svgElement.Id = "img" + Guid.NewGuid().ToString("N");
                _idAssigned = true;
            }
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            WpfDrawingContext context    = renderer.Context;
            SvgImageElement imageElement = (SvgImageElement)_svgElement;

            double x      = imageElement.X.AnimVal.Value;
            double y      = imageElement.Y.AnimVal.Value;
            double width  = imageElement.Width.AnimVal.Value;
            double height = imageElement.Height.AnimVal.Value;

            Rect destRect = new Rect(x, y, width, height);
            Rect clipRect = new Rect(x, y, width, height);

            ImageSource imageSource = null;
            if (imageElement.IsSvgImage)
            {
                if (imageElement.IsRootReferenced(imageElement.OwnerDocument.BaseURI))
                {
                    return;
                }

                SvgWindow wnd = GetSvgWindow();
                if (wnd == null)
                {
                    return;
                }
                //_embeddedRenderer.BackColor = Color.Empty;  
                _embeddedRenderer.Render(wnd.Document);

                DrawingGroup imageGroup = _embeddedRenderer.Drawing as DrawingGroup;
                if (imageGroup != null &&
                    (imageGroup.Children != null && imageGroup.Children.Count == 1))
                {
                    DrawingGroup imageDrawing = imageGroup.Children[0] as DrawingGroup;
                    if (imageDrawing != null)
                    {
                        imageDrawing.ClipGeometry = null;

                        imageSource = new DrawingImage(imageDrawing);
                    }
                    else
                    {
                        imageGroup.ClipGeometry = null;

                        imageSource = new DrawingImage(imageGroup);
                    }
                }
                else
                {
                    imageSource = new DrawingImage(_embeddedRenderer.Drawing);
                }

                if (_embeddedRenderer != null)
                {
                    _embeddedRenderer.Dispose();
                    _embeddedRenderer = null;
                }
            }
            else
            {
                imageSource = GetBitmapSource(imageElement, context);
            }
            
            if (imageSource == null)
            {
                return;
            }

            //TODO--PAUL: Set the DecodePixelWidth/DecodePixelHeight?

            // Freeze the DrawingImage for performance benefits. 
            //imageSource.Freeze();

            DrawingGroup drawGroup = null;

            ISvgAnimatedPreserveAspectRatio animatedAspectRatio = imageElement.PreserveAspectRatio;
            if (animatedAspectRatio != null && animatedAspectRatio.AnimVal != null)
            {
                SvgPreserveAspectRatio aspectRatio = animatedAspectRatio.AnimVal as SvgPreserveAspectRatio;
                SvgPreserveAspectRatioType aspectRatioType = 
                    (aspectRatio != null) ? aspectRatio.Align : SvgPreserveAspectRatioType.Unknown;
                if (aspectRatio != null && aspectRatioType != SvgPreserveAspectRatioType.None &&
                    aspectRatioType != SvgPreserveAspectRatioType.Unknown)
                {                      
                    double imageWidth  = imageSource.Width;
                    double imageHeight = imageSource.Height;

                    double viewWidth  = destRect.Width;
                    double viewHeight = destRect.Height;

                    SvgMeetOrSlice meetOrSlice = aspectRatio.MeetOrSlice;
                    if (meetOrSlice == SvgMeetOrSlice.Meet)
                    {
                        if (imageWidth <= viewWidth && imageHeight <= viewHeight)
                        {
                            if (this.Transform == null)
                            {
                                if (!aspectRatio.IsDefaultAlign) // Cacxa
                                {
                                    destRect = this.GetBounds(destRect, new Size(imageWidth, imageHeight), aspectRatioType);
                                }
                                else
                                {
                                    Transform viewTransform = this.GetAspectRatioTransform(aspectRatio,
                                      new SvgRect(0, 0, imageWidth, imageHeight),
                                      new SvgRect(destRect.X, destRect.Y, destRect.Width, destRect.Height));

                                    if (viewTransform != null)
                                    {
                                        drawGroup = new DrawingGroup();
                                        drawGroup.Transform = viewTransform;

                                        DrawingGroup lastGroup = context.Peek();
                                        Debug.Assert(lastGroup != null);

                                        if (lastGroup != null)
                                        {
                                            lastGroup.Children.Add(drawGroup);
                                        }

                                        destRect = this.GetBounds(destRect,
                                            new Size(imageWidth, imageHeight), aspectRatioType);

                                        // The origin is already handled by the view transform...
                                        destRect.X = 0;
                                        destRect.Y = 0;
                                    }
                                }
                            }
                            else
                            {
                                destRect = new Rect(0, 0, viewWidth, viewHeight);
                            }
                        }
                        else
                        {
                            if (this.Transform == null)
                            {
                                Transform viewTransform = this.GetAspectRatioTransform(aspectRatio,
                                  new SvgRect(0, 0, imageWidth, imageHeight),
                                  new SvgRect(destRect.X, destRect.Y, destRect.Width, destRect.Height));

                                if (viewTransform != null)
                                {
                                    drawGroup = new DrawingGroup();
                                    drawGroup.Transform = viewTransform;

                                    DrawingGroup lastGroup = context.Peek();
                                    Debug.Assert(lastGroup != null);

                                    if (lastGroup != null)
                                    {
                                        lastGroup.Children.Add(drawGroup);
                                    }

                                    destRect = this.GetBounds(destRect,
                                        new Size(imageWidth, imageHeight), aspectRatioType);

                                    // The origin is already handled by the view transform...
                                    destRect.X = 0;
                                    destRect.Y = 0;
                                }
                            }
                        }
                    }
                    else if (meetOrSlice == SvgMeetOrSlice.Slice)
                    {
                        var fScaleX = viewWidth / imageWidth;
                        var fScaleY = viewHeight / imageHeight;
                        Transform viewTransform = this.GetAspectRatioTransform(aspectRatio,
                          new SvgRect(0, 0, imageWidth, imageHeight),
                          new SvgRect(destRect.X, destRect.Y, destRect.Width, destRect.Height));

                        DrawingGroup sliceGroup = new DrawingGroup();
                        sliceGroup.ClipGeometry = new RectangleGeometry(clipRect);

                        DrawingGroup lastGroup = context.Peek();
                        Debug.Assert(lastGroup != null);

                        if (lastGroup != null)
                        {
                            lastGroup.Children.Add(sliceGroup);
                        }

                        if (viewTransform != null)
                        {
                            drawGroup = new DrawingGroup();
                            drawGroup.Transform = viewTransform;

                            sliceGroup.Children.Add(drawGroup);

                            destRect = this.GetBounds(destRect,
                                new Size(imageWidth, imageHeight), aspectRatioType);

                            // The origin is already handled by the view transform...
                            destRect.X = 0;
                            destRect.Y = 0;
                        }
                        else
                        {
                            drawGroup = sliceGroup;
                        }
                    }
                }
            }

            ImageDrawing drawing = new ImageDrawing(imageSource, destRect);

            float opacityValue = -1;

            string opacity = imageElement.GetAttribute("opacity");
            if (string.IsNullOrWhiteSpace(opacity))
            {
                opacity = imageElement.GetPropertyValue("opacity");
            }
            if (!string.IsNullOrWhiteSpace(opacity))
            {
                opacityValue = (float)SvgNumber.ParseNumber(opacity);
                opacityValue = Math.Min(opacityValue, 1);
                opacityValue = Math.Max(opacityValue, 0);
            }

            Geometry clipGeom   = this.ClipGeometry;
            Transform transform = this.Transform;

            GeometryDrawing viewportDrawing = null;

            // Support for Tiny 1.2 viewport-fill property...
            if (_svgElement.HasAttribute("viewport-fill"))
            {
                var viewportFill = _svgElement.GetAttribute("viewport-fill");
                if (!string.IsNullOrWhiteSpace(viewportFill))
                {
                    var brush = WpfFill.CreateViewportBrush(imageElement);
                    if (brush != null)
                    {
                        var viewportBounds = new RectangleGeometry(destRect);
                        viewportDrawing = new GeometryDrawing(brush, null, viewportBounds);
                    }
                }
            }

            bool ownedGroup = true;
            if (drawGroup == null)
            {
                drawGroup  = context.Peek();
                ownedGroup = false;
            }
            else
            {
                if (viewportDrawing != null)
                {
                    drawGroup.Children.Insert(0, viewportDrawing);
                }
            }

            Debug.Assert(drawGroup != null);
            if (drawGroup != null)
            {
                if ((opacityValue >= 0 && opacityValue < 1) || (clipGeom != null && !clipGeom.IsEmpty()) ||
                    (transform != null && !transform.Value.IsIdentity))
                {
                    DrawingGroup clipGroup = ownedGroup ? drawGroup : new DrawingGroup();
                    if (opacityValue >= 0 && opacityValue < 1)
                    {
                        clipGroup.Opacity = opacityValue;
                    }
                    if (clipGeom != null)
                    {
                        SvgUnitType clipUnits = this.ClipUnits;
                        if (clipUnits == SvgUnitType.ObjectBoundingBox)
                        {
                            Rect drawingBounds = drawing.Bounds;

                            TransformGroup transformGroup = new TransformGroup();

                            // Scale the clip region (at (0, 0)) and translate to the top-left corner of the target.
                            transformGroup.Children.Add(
                                new ScaleTransform(drawingBounds.Width, drawingBounds.Height));
                            transformGroup.Children.Add(
                                new TranslateTransform(drawingBounds.X, drawingBounds.Y));

                            clipGeom.Transform = transformGroup;
                        }

                        clipGroup.ClipGeometry = clipGeom;
                    }
                    if (transform != null)
                    {
                        Transform curTransform = clipGroup.Transform;
                        if (curTransform != null && curTransform.Value.IsIdentity == false)
                        {
                            TransformGroup transformGroup = new TransformGroup();
                            transformGroup.Children.Add(curTransform);
                            transformGroup.Children.Add(transform);
                            clipGroup.Transform = transformGroup;
                        }
                        else
                        {
                            clipGroup.Transform = transform;
                        }
                    }

                    clipGroup.Children.Add(drawing);
                    if (!ownedGroup)
                    {
                        if (viewportDrawing != null)
                        {
                            clipGroup.Children.Insert(0, viewportDrawing);
                        }
                        drawGroup.Children.Add(clipGroup);
                    }
                }
                else
                {
                    drawGroup.Children.Add(drawing);
                }

                string elementId = this.GetElementName();
                if (ownedGroup)
                {
                    string sVisibility = imageElement.GetPropertyValue("visibility");
                    string sDisplay = imageElement.GetPropertyValue("display");
                    if (string.Equals(sVisibility, "hidden") || string.Equals(sDisplay, "none"))
                    {
                        drawGroup.Opacity = 0;
                    }

                    if (!_idAssigned && !string.IsNullOrWhiteSpace(elementId) && !context.IsRegisteredId(elementId))
                    {
                        context.RegisterId(elementId);

                        if (context.IncludeRuntime)
                        {
                            SvgObject.SetName(drawGroup, elementId);

                            SvgObject.SetId(drawGroup, elementId);
                        }
                    }

                    // Register this drawing with the Drawing-Document...
                    this.Rendered(drawGroup);
                }
                else if (!_idAssigned)
                {
                    if (!_idAssigned && !string.IsNullOrWhiteSpace(elementId) && !context.IsRegisteredId(elementId))
                    {
                        context.RegisterId(elementId);

                        if (context.IncludeRuntime)
                        {
                            SvgObject.SetName(imageSource, elementId);

                            SvgObject.SetId(imageSource, elementId);
                        }
                    }

                    // Register this drawing with the Drawing-Document...
                    this.Rendered(drawing);
                }
            }
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            base.AfterRender(renderer);
        }

        #endregion

        #region Protected Methods

        protected override void Initialize(SvgElement element)
        {
            base.Initialize(element);

            _idAssigned       = false;
            _embeddedRenderer = null;
        }

        #endregion

        #region Private Methods

        private SvgWindow GetSvgWindow()
        {
            if (_embeddedRenderer == null)
            {
                _embeddedRenderer = new WpfDrawingRenderer();
            }

            SvgImageElement iElm = (SvgImageElement)this.Element;
            SvgWindow wnd = iElm.SvgWindow;
            if (wnd == null)
            {
                return null;
            }
            wnd.Renderer  = _embeddedRenderer;

            _embeddedRenderer.Window = wnd;

            return wnd;
        }

        private ImageSource GetBitmapSource(SvgImageElement element, WpfDrawingContext context)
        {
            ImageSource imageSource = this.GetBitmap(element, context);
            if (imageSource == null)
            {
                return imageSource;
            }

            SvgColorProfileElement colorProfile = (SvgColorProfileElement)element.ColorProfile;
            if (colorProfile == null || !(imageSource is BitmapSource))
            {
                return imageSource;
            }
            BitmapSource bitmapSource = (BitmapSource)imageSource;
            BitmapFrame inputFrame    = BitmapFrame.Create(bitmapSource);

            SvgUriReference svgUri = colorProfile.UriReference;
            Uri profileUri = new Uri(svgUri.AbsoluteUri);

            ColorContext colorContext = new ColorContext(new Uri(svgUri.AbsoluteUri));

            var colorContexts = new ReadOnlyCollection<ColorContext>(new ColorContext[] { colorContext });

            BitmapFrame outputFrame = BitmapFrame.Create(inputFrame, null, null, colorContexts);
            var bitmapImage   = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(outputFrame);

            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

//            bitmapImage.Freeze();

            return bitmapImage;
        }

        private ImageSource GetBitmap(SvgImageElement element, WpfDrawingContext context)
        {
            if (element.IsSvgImage)
            {
                return null;
            }

            if (element.Href == null)
            {
                return null;
            }

            if (!element.Href.AnimVal.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                SvgUriReference svgUri = element.UriReference;
                string absoluteUri = svgUri.AbsoluteUri;
                if (string.IsNullOrWhiteSpace(absoluteUri))
                {
                    return null; // most likely, the image does not exist...
                }
                if (absoluteUri.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                {                    
                    Trace.WriteLine("Uri: " + absoluteUri); // image elements can't reference elements in an svg file
                    return null;
                }

                Uri imageUri = new Uri(svgUri.AbsoluteUri);
                if (imageUri.IsFile)
                {
                    if (File.Exists(imageUri.LocalPath))
                    {
                        BitmapImage imageSource = new BitmapImage();

                        imageSource.BeginInit();
                        imageSource.CacheOption = BitmapCacheOption.OnLoad;
                        imageSource.CreateOptions = BitmapCreateOptions.IgnoreImageCache 
                            | BitmapCreateOptions.PreservePixelFormat;
                        imageSource.UriSource    = imageUri;
                        imageSource.EndInit();

//                        imageSource.Freeze();

                        return imageSource;
                    }

                    return null;
                }
                else
                {
                    Stream stream = svgUri.ReferencedResource.GetResponseStream();

                    BitmapImage imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.CacheOption   = BitmapCacheOption.OnLoad;
                    imageSource.CreateOptions = BitmapCreateOptions.IgnoreImageCache
                        | BitmapCreateOptions.PreservePixelFormat;
                    imageSource.StreamSource = stream;
                    imageSource.EndInit();

//                    imageSource.Freeze();

                    return imageSource;
                }
            }
            else
            {
                WpfEmbeddedImageVisitor imageVisitor = context.ImageVisitor;
                if (imageVisitor != null)
                {
                    ImageSource visitorSource = imageVisitor.Visit(element, context);
                    if (visitorSource != null)
                    {
                        return visitorSource;
                    }
                }

                string sURI    = element.Href.AnimVal.Replace(" ", "");
                int nColon     = sURI.IndexOf(":", StringComparison.OrdinalIgnoreCase);
                int nSemiColon = sURI.IndexOf(";", StringComparison.OrdinalIgnoreCase);
                int nComma     = sURI.IndexOf(",", StringComparison.OrdinalIgnoreCase);

                string sMimeType = sURI.Substring(nColon + 1, nSemiColon - nColon - 1);

                string sContent  = sURI.Substring(nComma + 1);
                byte[] bResult   = Convert.FromBase64CharArray(sContent.ToCharArray(),
                    0, sContent.Length);

                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                imageSource.StreamSource  = new MemoryStream(bResult);
                imageSource.EndInit();

//                imageSource.Freeze();

                return imageSource;
            }
        }

        #region GetBounds Method

        private Rect GetBounds(Rect bounds, Size textSize, SvgPreserveAspectRatioType alignment)
        {
            switch (alignment)
            {
                case SvgPreserveAspectRatioType.XMinYMin:  //Top-Left
                    return new Rect(bounds.X, bounds.Y, textSize.Width, textSize.Height);
                case SvgPreserveAspectRatioType.XMidYMin: //Top-Center
                    return new Rect(bounds.X + (bounds.Width - textSize.Width) / 2f,
                        bounds.Y, textSize.Width, textSize.Height);
                case SvgPreserveAspectRatioType.XMaxYMin: //Top-Right
                    return new Rect(bounds.Right - textSize.Width, bounds.Y, textSize.Width, textSize.Height);
                case SvgPreserveAspectRatioType.XMinYMid: //Middle-Left
                    return new Rect(bounds.X, bounds.Y +
                        (bounds.Height - textSize.Height) / 2f, textSize.Width, textSize.Height);
                case SvgPreserveAspectRatioType.XMidYMid:  //Middle-Center
                    return new Rect(bounds.X + bounds.Width / 2f - textSize.Width / 2f,
                        bounds.Y + bounds.Height / 2f - textSize.Height / 2f, textSize.Width, textSize.Height);
                case SvgPreserveAspectRatioType.XMaxYMid: //Middle-Right
                    return new Rect(bounds.Right - textSize.Width,
                        bounds.Y + bounds.Height / 2f - textSize.Height / 2f,
                        textSize.Width, textSize.Height);
                case SvgPreserveAspectRatioType.XMinYMax: //Bottom-Left
                    return new Rect(bounds.X, bounds.Bottom - textSize.Height,
                        textSize.Width, textSize.Height);
                case SvgPreserveAspectRatioType.XMidYMax:  //Bottom-Center
                    return new Rect(bounds.X + (bounds.Width - textSize.Width) / 2f,
                        bounds.Bottom - textSize.Height, textSize.Width, textSize.Height);
                case SvgPreserveAspectRatioType.XMaxYMax:  // Bottom-Right
                    return new Rect(bounds.Right - textSize.Width,
                        bounds.Bottom - textSize.Height, textSize.Width, textSize.Height);
            }

            return bounds;
        }

        private Transform GetAspectRatioTransform(SvgPreserveAspectRatio spar,
            SvgRect sourceBounds, SvgRect elementBounds)
        {
            double[] transformArray = spar.FitToViewBox(sourceBounds, elementBounds);

            double translateX = Math.Round(transformArray[0], 4);
            double translateY = Math.Round(transformArray[1], 4);
            double scaleX     = Math.Round(transformArray[2], 4);
            double scaleY     = Math.Round(transformArray[3], 4);

            // Cacxa
            if (this.Transform != null)
            {
                if (!scaleX.Equals(1.0) && !this.Transform.Value.OffsetX.Equals(0.0))
                    translateX = translateX + this.Transform.Value.OffsetX * (1 - scaleX);
                if (!scaleY.Equals(1.0) && !this.Transform.Value.OffsetY.Equals(0.0))
                    translateY = translateY + this.Transform.Value.OffsetY * (1 - scaleY);
            }

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
                    return null;
                }
                if (translateMatrix.Value.IsIdentity)
                {
                    return scaleMatrix;
                }
                if (scaleMatrix.Value.IsIdentity)
                {
                    return translateMatrix;
                }

                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaleMatrix);
                transformGroup.Children.Add(translateMatrix);

                return transformGroup;
            }

            if (translateMatrix != null)
            {
                return translateMatrix.Value.IsIdentity ? null : translateMatrix;
            }

            if (scaleMatrix != null)
            {
                return scaleMatrix.Value.IsIdentity ? null : scaleMatrix;
            }

            return null;
        }

        #endregion

        #endregion
    }
}
