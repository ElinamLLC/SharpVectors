using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using SharpVectors.Net;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
	/// <summary>
	/// Summary description for SvgImageGraphicsNode.
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

        public override void Render(GdiGraphicsRenderer renderer)
		{
            GdiGraphicsWrapper graphics = renderer.GraphicsWrapper;
			SvgImageElement iElement = (SvgImageElement)element;

			ImageAttributes imageAttributes = new ImageAttributes();

			string sOpacity = iElement.GetPropertyValue("opacity");
			if (sOpacity != null && sOpacity.Length > 0)
			{
				double opacity = SvgNumber.ParseNumber(sOpacity);
				ColorMatrix myColorMatrix = new ColorMatrix();
				myColorMatrix.Matrix00 = 1.00f; // Red
				myColorMatrix.Matrix11 = 1.00f; // Green
				myColorMatrix.Matrix22 = 1.00f; // Blue
				myColorMatrix.Matrix33 = (float)opacity; // alpha
				myColorMatrix.Matrix44 = 1.00f; // w

				imageAttributes.SetColorMatrix(myColorMatrix,
                    ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			}

            float width  = (float)iElement.Width.AnimVal.Value;
			float height = (float)iElement.Height.AnimVal.Value;

            //Rectangle destRect = new Rectangle(Convert.ToInt32(iElement.X.AnimVal.Value),
            //    Convert.ToInt32(iElement.Y.AnimVal.Value), 
            //    Convert.ToInt32(width), Convert.ToInt32(height));
            RectangleF destRect = new RectangleF((float)iElement.X.AnimVal.Value,
                (float)iElement.Y.AnimVal.Value, (float)iElement.Width.AnimVal.Value,
                (float)iElement.Height.AnimVal.Value);

			Image image = null;
			if (iElement.IsSvgImage)
			{
				SvgWindow wnd = GetSvgWindow();
                _embeddedRenderer.BackColor = Color.Empty;
                _embeddedRenderer.Render(wnd.Document);

                image = _embeddedRenderer.RasterImage;
			}
			else
			{
				image = GetBitmap(iElement);
			}

			if (image != null)
			{
                //graphics.DrawImage(this, image, destRect, 0f, 0f,
                //    image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);

                // code extracted from FitToViewbox
                SvgPreserveAspectRatio spar = (SvgPreserveAspectRatio)iElement.PreserveAspectRatio.AnimVal ?? new SvgPreserveAspectRatio("none", iElement);

                double[] translateAndScale =
                    spar.FitToViewBox(new SvgRect(0, 0, image.Width, image.Height),
                                      new SvgRect(destRect.X, destRect.Y, destRect.Width, destRect.Height));
                graphics.TranslateTransform((float)translateAndScale[0], (float)translateAndScale[1]);
                graphics.ScaleTransform((float)translateAndScale[2], (float)translateAndScale[3]);
                graphics.DrawImage(this, image, new Rectangle(0, 0, image.Width, image.Height), 0f, 0f,
                     image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);

                image.Dispose();
                image = null;
			}

            if (_embeddedRenderer != null)
            {
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
	        return new Rectangle((int) Math.Round(rect.Left), 
                (int) Math.Round(rect.Top), (int) Math.Round(rect.Width), 
                (int) Math.Round(rect.Height));
	    }

        private SvgWindow GetSvgWindow()
		{
            if (_embeddedRenderer == null)
            {
                _embeddedRenderer = new GdiGraphicsRenderer();
            }

			SvgImageElement iElm = this.Element as SvgImageElement;
			SvgWindow wnd = iElm.SvgWindow;
            wnd.Renderer  = _embeddedRenderer;

            _embeddedRenderer.Window = wnd;

			return wnd;
		}

        private Image GetBitmap(SvgImageElement element)
        {
            if (!element.IsSvgImage)
            {
                if (!element.Href.AnimVal.StartsWith("data:"))
                {
                    SvgUriReference svgUri = element.UriReference;
                    Uri imageUri = new Uri(svgUri.AbsoluteUri);
                    if (imageUri.IsFile && File.Exists(imageUri.LocalPath))
                    {
                        return Bitmap.FromFile(imageUri.LocalPath);
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
 
                    return Bitmap.FromStream(stream);
                }
                else
                {
                    string sURI    = element.Href.AnimVal;
                    int nColon     = sURI.IndexOf(":");
                    int nSemiColon = sURI.IndexOf(";");
                    int nComma     = sURI.IndexOf(",");

                    string sMimeType = sURI.Substring(nColon + 1, nSemiColon - nColon - 1);

                    string sContent  = sURI.Substring(nComma + 1);
                    byte[] bResult   = Convert.FromBase64CharArray(sContent.ToCharArray(), 
                        0, sContent.Length);

                    MemoryStream ms = new MemoryStream(bResult);

                    return Bitmap.FromStream(ms);
                }
            }
            else
            {
                return null;
            }
        }

        #endregion
	}
}
