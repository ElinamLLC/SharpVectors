using System;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Css;

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

        #region Public Methods

        public override Brush GetBrush(RectangleF bounds)
        {
            Image image = GetImage(bounds);
            RectangleF destRect = GetDestRect(bounds);

            TextureBrush tb = new TextureBrush(image, destRect);
            tb.Transform = GetTransformMatrix(bounds);
            return tb;
        }

        #endregion

		#region Private Methods

		private SvgSvgElement MoveIntoSvgElement()
		{
            SvgDocument doc = _patternElement.OwnerDocument;
			SvgSvgElement svgElm = doc.CreateElement("", "svg", SvgDocument.SvgNamespace) as SvgSvgElement;

			XmlNodeList children = _patternElement.Children;
			if (children.Count > 0)
			{
				oldParent = children[0].ParentNode as XmlElement;
			}

			for (int i = 0; i<children.Count; i++)
			{
				svgElm.AppendChild(children[i]);
			}

			if (_patternElement.HasAttribute("viewBox"))
			{
				svgElm.SetAttribute("viewBox", _patternElement.GetAttribute("viewBox"));
			}
			svgElm.SetAttribute("x", "0");
			svgElm.SetAttribute("y", "0");
			svgElm.SetAttribute("width", _patternElement.GetAttribute("width"));
			svgElm.SetAttribute("height", _patternElement.GetAttribute("height"));

			if (_patternElement.PatternContentUnits.AnimVal.Equals(SvgUnitType.ObjectBoundingBox))
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
            GdiGraphicsRenderer renderer = new GdiGraphicsRenderer();
			renderer.Window = _patternElement.OwnerDocument.Window as SvgWindow;

			SvgSvgElement elm = MoveIntoSvgElement();

			renderer.Render(elm as SvgElement);
            Image img = renderer.RasterImage;

			MoveOutOfSvgElement(elm);

			return img;
		}


		private float CalcPatternUnit(SvgLength length, SvgLengthDirection dir, RectangleF bounds)
		{
			if (_patternElement.PatternUnits.AnimVal.Equals(SvgUnitType.UserSpaceOnUse))
			{
                return (float)length.Value;
			}
			else
			{
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
