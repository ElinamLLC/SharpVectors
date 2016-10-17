using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfImageRendering : WpfRendering
    {
        #region Private Fields

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
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            WpfDrawingContext context    = renderer.Context;
            SvgImageElement imageElement = (SvgImageElement)_svgElement;

            double width  = imageElement.Width.AnimVal.Value;
            double height = imageElement.Height.AnimVal.Value;

            Rect destRect = new Rect(imageElement.X.AnimVal.Value, imageElement.Y.AnimVal.Value,
                width, height);

            ImageSource imageSource = null;
            if (imageElement.IsSvgImage)
            {
                SvgWindow wnd = GetSvgWindow();
                //_embeddedRenderer.BackColor = Color.Empty;  
                _embeddedRenderer.Render(wnd.Document);

                DrawingGroup imageGroup = _embeddedRenderer.Drawing as DrawingGroup;
                if (imageGroup != null &&
                    (imageGroup.Children != null && imageGroup.Children.Count == 1))
                {
                    DrawingGroup drawImage = imageGroup.Children[0] as DrawingGroup;
                    if (drawImage != null)
                    {
                        if (drawImage.ClipGeometry != null)
                        {
                            drawImage.ClipGeometry = null;
                        }

                        imageSource = new DrawingImage(drawImage);
                    }
                    else
                    {
                        if (imageGroup.ClipGeometry != null)
                        {
                            imageGroup.ClipGeometry = null;
                        }

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
            imageSource.Freeze();

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
                                destRect = this.GetBounds(destRect,
                                    new Size(imageWidth, imageHeight), aspectRatioType);
                            }
                            else
                            {
                                destRect = new Rect(0, 0, viewWidth, viewHeight);
                            }
                        }
                        else
                        {
                            Transform viewTransform = this.GetAspectRatioTransform(aspectRatio,
                              new SvgRect(0, 0, imageWidth, imageHeight),
                              new SvgRect(destRect.X, destRect.Y, destRect.Width, destRect.Height));
                            //Transform scaleTransform = this.FitToViewbox(aspectRatio,
                            //  new SvgRect(destRect.X, destRect.Y, imageWidth, imageHeight),
                            //  new SvgRect(destRect.X, destRect.Y, destRect.Width, destRect.Height));

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
                    else if (meetOrSlice == SvgMeetOrSlice.Slice)
                    {   
                    }
                }
            }

            ImageDrawing drawing = new ImageDrawing(imageSource, destRect);

            float opacityValue = -1;

            string opacity = imageElement.GetAttribute("opacity");
            if (opacity != null && opacity.Length > 0)
            {
                opacityValue = (float)SvgNumber.ParseNumber(opacity);
                opacityValue = Math.Min(opacityValue, 1);
                opacityValue = Math.Max(opacityValue, 0);
            }

            Geometry clipGeom   = this.ClipGeometry;
            Transform transform = this.Transform;

            if (drawGroup == null)
            {
                drawGroup = context.Peek();
            }
            Debug.Assert(drawGroup != null);
            if (drawGroup != null)
            {
                if (opacityValue >= 0 || (clipGeom != null && !clipGeom.IsEmpty()) ||
                    (transform != null && !transform.Value.IsIdentity))
                {
                    DrawingGroup clipGroup = new DrawingGroup();
                    if (opacityValue >= 0)
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
                        clipGroup.Transform = transform;
                    }

                    clipGroup.Children.Add(drawing);
                    drawGroup.Children.Add(clipGroup);
                }
                else
                {
                    drawGroup.Children.Add(drawing);
                }
            }
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            base.AfterRender(renderer);
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
            wnd.Renderer  = _embeddedRenderer;

            _embeddedRenderer.Window = wnd;

            return wnd;
        }

        private ImageSource GetBitmapSource(SvgImageElement element, WpfDrawingContext context)
        {
            BitmapSource bitmapSource = this.GetBitmap(element, context);
            if (bitmapSource == null)
            {
                return bitmapSource;
            }

            SvgColorProfileElement colorProfile = (SvgColorProfileElement)element.ColorProfile;
            if (colorProfile == null)
            {
                return bitmapSource;
            }
            else
            {
                BitmapFrame bitmapSourceFrame   = BitmapFrame.Create(bitmapSource);
                ColorContext sourceColorContext = null;
                IList<ColorContext> colorContexts = bitmapSourceFrame.ColorContexts;
                if (colorContexts != null && colorContexts.Count != 0)
                {
                    sourceColorContext = colorContexts[0];
                }
                else
                {
                    sourceColorContext = new ColorContext(bitmapSource.Format);
                    //sourceColorContext = new ColorContext(PixelFormats.Default);
                }

                SvgUriReference svgUri = colorProfile.UriReference;
                Uri profileUri = new Uri(svgUri.AbsoluteUri);

                ColorContext destColorContext = new ColorContext(profileUri);
                ColorConvertedBitmap convertedBitmap = new ColorConvertedBitmap(bitmapSource,
                    sourceColorContext, destColorContext, bitmapSource.Format);

                return convertedBitmap;
            }
        }

        private BitmapSource GetBitmap(SvgImageElement element, WpfDrawingContext context)
        {
            if (element.IsSvgImage)
            {
                return null;
            }

            if (!element.Href.AnimVal.StartsWith("data:"))
            {
                SvgUriReference svgUri = element.UriReference;
                string absoluteUri = svgUri.AbsoluteUri;
                if (String.IsNullOrEmpty(absoluteUri))
                {
                    return null; // most likely, the image does not exist...
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
                        imageSource.UriSource     = imageUri;
                        imageSource.EndInit();

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

                    return imageSource;
                }
            }
            else
            {
                WpfEmbeddedImageVisitor imageVisitor = context.ImageVisitor;
                if (imageVisitor != null)
                {
                    BitmapSource visitorSource = imageVisitor.Visit(element, context);
                    if (visitorSource != null)
                    {
                        return visitorSource;
                    }
                }

                string sURI    = element.Href.AnimVal;
                int nColon     = sURI.IndexOf(":");
                int nSemiColon = sURI.IndexOf(";");
                int nComma     = sURI.IndexOf(",");

                string sMimeType = sURI.Substring(nColon + 1, nSemiColon - nColon - 1);

                string sContent  = sURI.Substring(nComma + 1);
                byte[] bResult   = Convert.FromBase64CharArray(sContent.ToCharArray(),
                    0, sContent.Length);

                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                imageSource.StreamSource  = new MemoryStream(bResult);
                imageSource.EndInit();

                return imageSource;
            }
        }

        #region GetBounds Method

        private Rect GetBounds(Rect bounds, Size textSize, SvgPreserveAspectRatioType alignment)
        {
            switch (alignment)
            {
                case SvgPreserveAspectRatioType.XMinYMin:  //Top-Left
                    {
                        return new Rect(bounds.X, bounds.Y,
                            textSize.Width, textSize.Height);
                    }
                case SvgPreserveAspectRatioType.XMidYMin: //Top-Center
                    {
                        return new Rect(bounds.X +
                            (bounds.Width - textSize.Width) / 2f,
                            bounds.Y, textSize.Width, textSize.Height);
                    }
                case SvgPreserveAspectRatioType.XMaxYMin: //Top-Right
                    {
                        return new Rect(bounds.Right - textSize.Width,
                            bounds.Y, textSize.Width, textSize.Height);
                    }
                case SvgPreserveAspectRatioType.XMinYMid: //Middle-Left
                    {
                        return new Rect(bounds.X, bounds.Y +
                            (bounds.Height - textSize.Height) / 2f, textSize.Width,
                            textSize.Height);
                    }
                case SvgPreserveAspectRatioType.XMidYMid:  //Middle-Center
                    {
                        return new Rect(bounds.X + bounds.Width / 2f - textSize.Width / 2f,
                            bounds.Y + bounds.Height / 2f - textSize.Height / 2f,
                            textSize.Width, textSize.Height);
                    }
                case SvgPreserveAspectRatioType.XMaxYMid: //Middle-Right
                    {
                        return new Rect(bounds.Right - textSize.Width,
                            bounds.Y + bounds.Height / 2f - textSize.Height / 2f,
                            textSize.Width, textSize.Height);
                    }
                case SvgPreserveAspectRatioType.XMinYMax: //Bottom-Left
                    {
                        return new Rect(bounds.X,
                            bounds.Bottom - textSize.Height,
                            textSize.Width, textSize.Height);
                    }
                case SvgPreserveAspectRatioType.XMidYMax:  //Bottom-Center
                    {
                        return new Rect(bounds.X +
                            (bounds.Width - textSize.Width) / 2f,
                            bounds.Bottom - textSize.Height,
                            textSize.Width, textSize.Height);
                    }
                case SvgPreserveAspectRatioType.XMaxYMax:  // Bottom-Right
                    {
                        return new Rect(bounds.Right - textSize.Width,
                            bounds.Bottom - textSize.Height,
                            textSize.Width, textSize.Height);
                    }
            }

            return bounds;
        }

        private Transform GetAspectRatioTransform(SvgPreserveAspectRatio spar,
            SvgRect sourceBounds, SvgRect elementBounds)
        {
            double[] transformArray = spar.FitToViewBox(sourceBounds, elementBounds);

            double translateX = transformArray[0];
            double translateY = transformArray[1];
            double scaleX = transformArray[2];
            double scaleY = transformArray[3];

            Transform translateMatrix = null;
            Transform scaleMatrix = null;
            if ((float)translateX >= 0 && (float)translateY >= 0)
            {
                translateMatrix = new TranslateTransform(translateX, translateY);
            }
            if ((float)scaleX != 1.0f && (float)scaleY != 1.0)
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

                return transformGroup;
            }
            else if (translateMatrix != null)
            {
                return translateMatrix;
            }
            else if (scaleMatrix != null)
            {
                return scaleMatrix;
            }

            return null;
        }

        #endregion

        #endregion
    }
}
