using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Css;

namespace SharpVectors.Renderers.Wpf
{
    public abstract class WpfFill : DependencyObject
    {
        #region Constructors and Destructor

        protected WpfFill()
        {
        }

        #endregion

        #region Public Methods

        public abstract Brush GetBrush(WpfDrawingContext context);

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
