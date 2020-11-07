using System;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using SharpVectors.Dom;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
	public sealed class GdiPatternFill : GdiFill
    {
        #region Private Fields

        private XmlElement oldParent;
        private SvgPatternElement _patternElement;

        #endregion

        #region Constructors and Destructor

        public GdiPatternFill(SvgPatternElement patternElement)
		{
			_patternElement = patternElement;
        }

        #endregion

        #region Public Properties

        public override bool IsUserSpace
        {
            get {
                return false;
            }
        }

        public override GdiFillType FillType
        {
            get {
                return GdiFillType.Pattern;
            }
        }

        #endregion

        #region Public Methods

        public override Brush GetBrush(RectangleF bounds, float opacity = 1)
        {
            Image image = GetImage(bounds);
            if (image == null)
            {
                return new SolidBrush(Color.Black);
            }

            RectangleF destRect = GetDestRect(bounds);

            ImageAttributes imageAttr = null;
            if (opacity >= 0 && opacity < 1)
            {
                imageAttr = new ImageAttributes();
                ColorMatrix colorMatrix = new ColorMatrix();
                colorMatrix.Matrix00 = 1.00f;   // Red
                colorMatrix.Matrix11 = 1.00f;   // Green
                colorMatrix.Matrix22 = 1.00f;   // Blue
                colorMatrix.Matrix33 = opacity; // alpha
                colorMatrix.Matrix44 = 1.00f;   // w

                imageAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                TextureBrush brush = new TextureBrush(image, destRect, imageAttr);
                brush.Transform = GetTransformMatrix(bounds);
                return brush;
            }

            TextureBrush textureBrush = null;
            if (destRect.IsEmpty)
            {
                textureBrush = new TextureBrush(image);
            }
            else
            {
                textureBrush = new TextureBrush(image, destRect);
            }
            textureBrush.Transform = GetTransformMatrix(bounds);
            return textureBrush;
        }

        #endregion

		#region Private Methods

		private SvgSvgElement MoveIntoSvgElement()
		{
            SvgDocument doc = _patternElement.OwnerDocument;
			SvgSvgElement svgElm = doc.CreateElement(string.Empty, "svg", SvgDocument.SvgNamespace) as SvgSvgElement;

			XmlNodeList children = _patternElement.Children;
			if (children.Count > 0)
			{
				oldParent = children[0].ParentNode as XmlElement;
			}

			for (int i = 0; i < children.Count; i++)
			{
				svgElm.AppendChild(children[i]);
			}

			if (_patternElement.HasAttribute("viewBox"))
			{
				svgElm.SetAttribute("viewBox", _patternElement.GetAttribute("viewBox"));
			}
			svgElm.SetAttribute("x", SvgConstants.ValZero);
			svgElm.SetAttribute("y", SvgConstants.ValZero);
			svgElm.SetAttribute("width", _patternElement.GetAttribute("width"));
			svgElm.SetAttribute("height", _patternElement.GetAttribute("height"));

			if (_patternElement.PatternContentUnits.AnimVal.Equals((ushort)SvgUnitType.ObjectBoundingBox))
			{
				svgElm.SetAttribute("viewBox", "0 0 1 1");
			}

			_patternElement.AppendChild(svgElm);

            return svgElm;
		}

		private void MoveOutOfSvgElement(SvgSvgElement svgElm)
		{
			while (svgElm.ChildNodes.Count > 0)
			{
				oldParent.AppendChild(svgElm.ChildNodes[0]);
			}

			_patternElement.RemoveChild(svgElm);
		}

		private Image GetImage(RectangleF bounds)
		{
            _patternElement.PatternBounds = new SvgRect(bounds.X, bounds.Y, bounds.Width, bounds.Height);

            // For single image pattern...
            if (_patternElement.ChildNodes.Count == 1)
            {
                var imageElement = _patternElement.ChildNodes[0] as SvgImageElement;
                if (imageElement != null)
                {
                    var image = GdiImageRendering.GetBitmap(imageElement);
                    if (image != null)
                    {
                        return image;
                    }
                }
            }

            GdiGraphicsRenderer renderer = new GdiGraphicsRenderer(true, false);
            var svgWindow = _patternElement.OwnerDocument.Window as SvgWindow;
            var ownedWindow = svgWindow.CreateOwnedWindow();
			renderer.Window = ownedWindow;

			SvgSvgElement elm = MoveIntoSvgElement();

            int winWidth  = (int)elm.Width.BaseVal.Value;
            int winHeight = (int)elm.Height.BaseVal.Value;
            if (winWidth == 0 || winHeight == 0)
            {
                var size = elm.GetSize();
                if (size.Width.Equals(0) || size.Height.Equals(0))
                {
                    winWidth  = (int)bounds.Width;
                    winHeight = (int)bounds.Height;
                }
                else
                {
                    winWidth  = (int)size.Width;
                    winHeight = (int)size.Height;
                }
            }

            ownedWindow.Resize(winWidth, winHeight);

            renderer.Render(elm as SvgElement);
            Image img = renderer.RasterImage;

			MoveOutOfSvgElement(elm);

			return img;
		}

		private float CalcPatternUnit(SvgLength length, SvgLengthDirection dir, RectangleF bounds)
		{
			if (_patternElement.PatternUnits.AnimVal.Equals((ushort)SvgUnitType.UserSpaceOnUse))
			{
                return (float)length.Value;
			}
            float calcValue = (float)length.ValueInSpecifiedUnits;
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

		private RectangleF GetDestRect(RectangleF bounds)
		{
			RectangleF result = new RectangleF(0, 0, 0, 0);
			result.Width  = CalcPatternUnit(_patternElement.Width.AnimVal as SvgLength, 
                SvgLengthDirection.Horizontal, bounds);
			result.Height = CalcPatternUnit(_patternElement.Height.AnimVal as SvgLength, 
                SvgLengthDirection.Vertical, bounds);
			
			return result;
		}

		private Matrix GetTransformMatrix(RectangleF bounds)
		{
			SvgMatrix svgMatrix = ((SvgTransformList)_patternElement.PatternTransform.AnimVal).TotalMatrix;

            Matrix transformMatrix = new Matrix((float)svgMatrix.A, (float)svgMatrix.B, (float)svgMatrix.C,
                (float)svgMatrix.D, (float)svgMatrix.E, (float)svgMatrix.F);

			float translateX = CalcPatternUnit(_patternElement.X.AnimVal as SvgLength, 
                SvgLengthDirection.Horizontal, bounds);
			float translateY = CalcPatternUnit(_patternElement.Y.AnimVal as SvgLength, 
                SvgLengthDirection.Vertical, bounds);

			transformMatrix.Translate(translateX, translateY, MatrixOrder.Prepend);

			return transformMatrix;
		}

		#endregion
	}
}
