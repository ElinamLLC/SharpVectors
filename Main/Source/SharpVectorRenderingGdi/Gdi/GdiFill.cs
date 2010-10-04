using System;
using System.Xml;
using System.Drawing;

using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Css;

namespace SharpVectors.Renderers.Gdi
{
	public abstract class GdiFill
    {
        #region Constructors and Destructor

        protected GdiFill() 
        {
        }

        #endregion

        #region Public Methods

        public abstract Brush GetBrush(RectangleF bounds);

		public static GdiFill CreateFill(SvgDocument document, string absoluteUri)
		{
			XmlNode node = document.GetNodeByUri(absoluteUri);

            SvgGradientElement gradientNode = node as SvgGradientElement;
            if (gradientNode != null)
			{
                return new GdiGradientFill(gradientNode);
			}

            SvgPatternElement patternNode = node as SvgPatternElement;
            if (patternNode != null)
			{
                return new GdiPatternFill(patternNode);
			}

            return null;
        }

        #endregion
    }
}
