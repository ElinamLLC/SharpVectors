using System;
using System.Xml;
using System.Drawing;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    public enum GdiFillType
    {
        None           = 0,
        Solid          = 1,
        LinearGradient = 2,
        RadialGradient = 3,
        Pattern        = 4
    }

	public abstract class GdiFill
    {
        #region Constructors and Destructor

        protected GdiFill() 
        {
        }

        #endregion

        #region Public Properties

        public abstract bool IsUserSpace
        {
            get;
        }

        public abstract GdiFillType FillType
        {
            get;
        }

        #endregion

        #region Public Methods

        public abstract Brush GetBrush(RectangleF bounds, float opacity = 1);

		public static GdiFill CreateFill(SvgDocument document, string absoluteUri)
		{
			XmlNode node = document.GetNodeByUri(absoluteUri);

            var linearGradient = node as SvgLinearGradientElement;
            if (linearGradient != null)
			{
                return new GdiLinearGradientFill(linearGradient);
			}
            var radialGradient = node as SvgRadialGradientElement;
            if (radialGradient != null)
			{
                return new GdiRadialGradientFill(radialGradient);
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
