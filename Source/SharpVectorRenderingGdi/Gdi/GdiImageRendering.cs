using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    /// <summary>
    /// Summary description for GdiImageRendering.
    /// </summary>
    public sealed class GdiImageRendering : GdiRendering
    {
        #region Private Fields

        private GdiGraphicsRenderer _embeddedRenderer;

        #endregion

        #region Constructors and Destructor

        public GdiImageRendering(SvgElement element) 
            : base(element)
		{
		}

        #endregion

        #region Public Methods

        public override void BeforeRender(GdiGraphicsRenderer renderer)
        {
            base.BeforeRender(renderer);
        }

        public override void Render(GdiGraphicsRenderer renderer)
		{
            base.Render(renderer);

            var graphics = renderer.GdiGraphics;
			SvgImageElement iElement = (SvgImageElement)_svgElement;

			ImageAttributes imageAttributes = new ImageAttributes();

            float opacityValue = -1;

            string opacity = iElement.GetAttribute("opacity");

            if (string.IsNullOrWhiteSpace(opacity))
            {
                opacity = iElement.GetPropertyValue("opacity");
            }
            if (!string.IsNullOrWhiteSpace(opacity))
            {
                opacityValue = (float)SvgNumber.ParseNumber(opacity);
                opacityValue = Math.Min(opacityValue, 1);
                opacityValue = Math.Max(opacityValue, 0);
            }

            if (opacityValue >= 0 && opacityValue < 1)
            {
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix00 = 1.00f; // Red
				colorMatrix.Matrix11 = 1.00f; // Green
				colorMatrix.Matrix22 = 1.00f; // Blue
				colorMatrix.Matrix33 = opacityValue; // alpha
				colorMatrix.Matrix44 = 1.00f; // w

				imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            }

            float width  = (float)iElement.Width.AnimVal.Value;
			float height = (float)iElement.Height.AnimVal.Value;

            RectangleF destRect = new RectangleF((float)iElement.X.AnimVal.Value,
                (float)iElement.Y.AnimVal.Value, (float)iElement.Width.AnimVal.Value,
                (float)iElement.Height.AnimVal.Value);

            RectangleF srcRect;
            RectangleF clipRect = destRect;

//            var container = graphics.BeginContainer();
            graphics.SetClip(new Region(clipRect), CombineMode.Intersect);

            Image image = null;
            SvgWindow svgWnd = null;

            if (iElement.IsSvgImage)
			{
				svgWnd = GetSvgWindow(graphics);
                if (width > 0 && height > 0)
                {
                    srcRect = new RectangleF(0, 0, width, height);
                }
                else
                {
                    SvgSvgElement svgEl = (SvgSvgElement)svgWnd.Document.RootElement;

                    SvgSizeF size = svgEl.GetSize();

                    srcRect = new RectangleF(new PointF(0, 0), new SizeF(size.Width, size.Height));
                }
            }
            else
			{
				image = GetBitmap(iElement);
                if (image == null)
                {
                    return;
                }
                srcRect = new RectangleF(0, 0, image.Width, image.Height);
            }

            ISvgAnimatedPreserveAspectRatio animatedAspectRatio = iElement.PreserveAspectRatio;
            if (animatedAspectRatio != null && animatedAspectRatio.AnimVal != null)
            {
                SvgPreserveAspectRatio aspectRatio = animatedAspectRatio.AnimVal as SvgPreserveAspectRatio;
                SvgPreserveAspectRatioType aspectRatioType =
                    (aspectRatio != null) ? aspectRatio.Align : SvgPreserveAspectRatioType.Unknown;

                if (aspectRatio != null && aspectRatioType != SvgPreserveAspectRatioType.None &&
                    aspectRatioType != SvgPreserveAspectRatioType.Unknown)
                {
                    var fScaleX = destRect.Width / srcRect.Width;
                    var fScaleY = destRect.Height / srcRect.Height;
                    var xOffset = 0.0f;
                    var yOffset = 0.0f;

                    SvgMeetOrSlice meetOrSlice = aspectRatio.MeetOrSlice;
                    if (meetOrSlice == SvgMeetOrSlice.Slice)
                    {
                        fScaleX = Math.Max(fScaleX, fScaleY);
                        fScaleY = Math.Max(fScaleX, fScaleY);
                    }
                    else
                    {
                        fScaleX = Math.Min(fScaleX, fScaleY);
                        fScaleY = Math.Min(fScaleX, fScaleY);
                    }

                    switch (aspectRatioType)
                    {
                        case SvgPreserveAspectRatioType.XMinYMin:
                            break;
                        case SvgPreserveAspectRatioType.XMidYMin:
                            xOffset = (destRect.Width - srcRect.Width * fScaleX) / 2;
                            break;
                        case SvgPreserveAspectRatioType.XMaxYMin:
                            xOffset = (destRect.Width - srcRect.Width * fScaleX);
                            break;
                        case SvgPreserveAspectRatioType.XMinYMid:
                            yOffset = (destRect.Height - srcRect.Height * fScaleY) / 2;
                            break;
                        case SvgPreserveAspectRatioType.XMidYMid:
                            xOffset = (destRect.Width - srcRect.Width * fScaleX) / 2;
                            yOffset = (destRect.Height - srcRect.Height * fScaleY) / 2;
                            break;
                        case SvgPreserveAspectRatioType.XMaxYMid:
                            xOffset = (destRect.Width - srcRect.Width * fScaleX);
                            yOffset = (destRect.Height - srcRect.Height * fScaleY) / 2;
                            break;
                        case SvgPreserveAspectRatioType.XMinYMax:
                            yOffset = (destRect.Height - srcRect.Height * fScaleY);
                            break;
                        case SvgPreserveAspectRatioType.XMidYMax:
                            xOffset = (destRect.Width - srcRect.Width * fScaleX) / 2;
                            yOffset = (destRect.Height - srcRect.Height * fScaleY);
                            break;
                        case SvgPreserveAspectRatioType.XMaxYMax:
                            xOffset = (destRect.Width - srcRect.Width * fScaleX);
                            yOffset = (destRect.Height - srcRect.Height * fScaleY);
                            break;
                    }

                    destRect = new RectangleF(destRect.X + xOffset, destRect.Y + yOffset,
                                                srcRect.Width * fScaleX, srcRect.Height * fScaleY);
                }
                if (image != null)
			    {
                    SvgColorProfileElement colorProfile = (SvgColorProfileElement)iElement.ColorProfile;
                    if (colorProfile != null)
                    {
                        SvgUriReference svgUri = colorProfile.UriReference;
                        Uri profileUri = new Uri(svgUri.AbsoluteUri);

                        imageAttributes.SetOutputChannelColorProfile(profileUri.LocalPath, ColorAdjustType.Default);
                    }

                    graphics.DrawImage(this, image, destRect, srcRect, GraphicsUnit.Pixel, imageAttributes);

                    image.Dispose();
                    image = null;
                }
                else if (iElement.IsSvgImage && svgWnd != null)
                {
                    svgWnd.Resize((int)srcRect.Width, (int)srcRect.Height);

                    var currOffset = new PointF(graphics.Transform.OffsetX, graphics.Transform.OffsetY);
                    if (!currOffset.IsEmpty)
                    {
                        graphics.TranslateTransform(-currOffset.X, -currOffset.Y);
                    }
                    graphics.ScaleTransform(destRect.Width / srcRect.Width, destRect.Height / srcRect.Height);
                    if (!currOffset.IsEmpty)
                    {
                        graphics.TranslateTransform(currOffset.X + destRect.X, currOffset.Y + destRect.Y);
                    }

                    _embeddedRenderer.Render(svgWnd.Document);
                }

                graphics.ResetClip();
//                graphics.EndContainer(container);
			}

            if (_embeddedRenderer != null)
            {
                _embeddedRenderer.GdiGraphics = null;
                _embeddedRenderer.Dispose();
                _embeddedRenderer = null;
            }

            if (imageAttributes != null)
            {
                imageAttributes.Dispose();
                imageAttributes = null;
            }
		}

        #endregion

        #region Private Methods
 
	    private static Rectangle ToRectangle(RectangleF rect)
	    {
	        return new Rectangle((int)Math.Round(rect.Left), (int)Math.Round(rect.Top), 
                (int)Math.Round(rect.Width), (int)Math.Round(rect.Height));
	    }

        private SvgWindow GetSvgWindow(GdiGraphics graphics)
		{
			SvgImageElement iElm = this.Element as SvgImageElement;
			SvgWindow wnd = iElm.SvgWindow;

            if (_embeddedRenderer == null)
            {
                _embeddedRenderer = new GdiGraphicsRenderer(graphics, wnd);
            }
            else
            {
                wnd.Renderer = _embeddedRenderer;
                _embeddedRenderer.Window = wnd;
            }

            return wnd;
		}

        public static Image GetBitmap(SvgImageElement element)
        {
            var comparer = StringComparison.OrdinalIgnoreCase;
            if (!element.IsSvgImage)
            {
                if (!element.Href.AnimVal.StartsWith("data:", comparer))
                {
                    SvgUriReference svgUri = element.UriReference;
                    string absoluteUri = svgUri.AbsoluteUri;
                    if (string.IsNullOrWhiteSpace(absoluteUri))
                    {
                        return null; // most likely, the image does not exist...
                    }
                    if (absoluteUri.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.WriteLine("Uri: " + absoluteUri); // image elements can't reference elements in an svg file
                        return null;
                    }

                    Uri imageUri = new Uri(svgUri.AbsoluteUri);
                    if (imageUri.IsFile && File.Exists(imageUri.LocalPath))
                    {
                        return Image.FromFile(imageUri.LocalPath, element.ColorProfile != null);
                    }

                    WebResponse resource = svgUri.ReferencedResource;
                    if (resource == null)
                    {
                        return null;
                    }

                    Stream stream = resource.GetResponseStream();
                    if (stream == null)
                    {
                        return null;
                    }
 
                    return Image.FromStream(stream, element.ColorProfile != null);
                }

                string sURI    = element.Href.AnimVal.Replace(" ", "").Trim();
                sURI = sURI.Replace(@"\n", "");
                int nColon     = sURI.IndexOf(":", comparer);
                int nSemiColon = sURI.IndexOf(";", comparer);
                int nComma     = sURI.IndexOf(",", comparer);

                string sMimeType = sURI.Substring(nColon + 1, nSemiColon - nColon - 1);
                string sContent  = sURI.Substring(nComma + 1);
                
                sContent = sContent.Replace('-', '+').Replace('_', '/');
                sContent = sContent.PadRight(4 * ((sContent.Length + 3) / 4), '=');
                byte[] bResult   = Convert.FromBase64CharArray(sContent.ToCharArray(), 
                    0, sContent.Length);

                if (sMimeType.Equals("image/svg+xml", StringComparison.OrdinalIgnoreCase))
                {
                    GdiGraphicsRenderer renderer = new GdiGraphicsRenderer(true, false);

                    var currentWindow = element.OwnerDocument.Window as SvgWindow;
                    var svgWindow = currentWindow.CreateOwnedWindow();
                    renderer.Window = svgWindow;

                    SvgDocument doc = svgWindow.CreateEmptySvgDocument();
                    bool isGZiped = sContent.StartsWith(GdiObject.GZipSignature, StringComparison.Ordinal);
                    if (isGZiped)
                    {
                        byte[] imageBytes = Convert.FromBase64CharArray(sContent.ToCharArray(),
                            0, sContent.Length);
                        using (var stream = new MemoryStream(imageBytes))
                        {
                            using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress))
                            {
                                doc.Load(zipStream);
                            }
                        }
                    }
                    else
                    {
                        var svgData     = Convert.FromBase64String(Convert.ToBase64String(bResult));
                        var svgFragment = Encoding.ASCII.GetString(svgData);

                        doc.LoadXml(svgFragment);
                    }
                    svgWindow.Document = doc;

                    SvgSvgElement elm = (SvgSvgElement)doc.RootElement;

                    int winWidth = (int)elm.Width.BaseVal.Value;
                    int winHeight = (int)elm.Height.BaseVal.Value;
                    if (winWidth == 0 || winHeight == 0)
                    {
                        var size = elm.GetSize();
                        winWidth = (int)size.Width;
                        winHeight = (int)size.Height;
                    }

                    svgWindow.Resize(winWidth, winHeight);

                    renderer.Render(elm as SvgElement);
                    Image img = renderer.RasterImage;

                    return img;
                }

                MemoryStream ms = new MemoryStream(bResult);
                return Image.FromStream(ms, element.ColorProfile != null);
            }
            return null;
        }

        #endregion
	}
}
