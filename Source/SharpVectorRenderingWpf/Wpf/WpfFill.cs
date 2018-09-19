using System.Xml;
using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public enum WpfFillType
    {
        None     = 0,
        Solid    = 1,
        Gradient = 2,
        Pattern  = 3
    }

    public abstract class WpfFill : DependencyObject
    {
        #region Constructors and Destructor

        protected WpfFill()
        {
        }

        #endregion

        #region Public Properties

        public abstract bool IsUserSpace
        {
            get;
        }

        public abstract WpfFillType FillType
        {
            get;
        }

        #endregion

        #region Public Methods

        public abstract Brush GetBrush(Rect elementBounds, WpfDrawingContext context, Transform viewTransform);

        public static WpfFill CreateFill(SvgDocument document, string absoluteUri)
        {
            XmlNode node = document.GetNodeByUri(absoluteUri);

            SvgGradientElement gradientNode = node as SvgGradientElement;
            if (gradientNode != null)
            {
                return new WpfGradientFill(gradientNode);
            }

            SvgPatternElement patternNode = node as SvgPatternElement;
            if (patternNode != null)
            {
                return new WpfPatternFill(patternNode);
            }

            return null;
        }

        #endregion
    }
}
