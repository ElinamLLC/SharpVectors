using System;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Utils
{
    public static class WpfConverter
    {
        /// <summary>
        /// A GDI Color representation of the RgbColor
        /// </summary>
        public static Color? ToColor(ICssColor color)
        {
            if (color == null)
            {
                return null;
            }

            double dRed   = color.Red.GetFloatValue(CssPrimitiveType.Number);
            double dGreen = color.Green.GetFloatValue(CssPrimitiveType.Number);
            double dBlue  = color.Blue.GetFloatValue(CssPrimitiveType.Number);

            if (Double.IsNaN(dRed) || Double.IsInfinity(dRed))
            {
                return null;
            }
            if (Double.IsNaN(dGreen) || Double.IsInfinity(dGreen))
            {
                return null;
            }
            if (Double.IsNaN(dBlue) || Double.IsInfinity(dBlue))
            {
                return null;
            }

            return Color.FromRgb(Convert.ToByte(dRed), Convert.ToByte(dGreen), Convert.ToByte(dBlue));
        }

        public static Rect ToRect(ICssRect rect)
        {
            if (rect == null)
            {
                return Rect.Empty;
            }

            double x      = rect.Left.GetFloatValue(CssPrimitiveType.Px);
            double y      = rect.Top.GetFloatValue(CssPrimitiveType.Px);
            double width  = rect.Right.GetFloatValue(CssPrimitiveType.Px) - x;
            double height = rect.Bottom.GetFloatValue(CssPrimitiveType.Px) - y;

            return new Rect(x, y, width, height);
        }


        /// <summary>
        /// This converts the specified <see cref="Rect"/> structure to a 
        /// <see cref="SvgRectF"/> structure.
        /// </summary>
        /// <param name="rect">The <see cref="Rect"/> structure to convert.</param>
        /// <returns>
        /// The <see cref="SvgRectF"/> structure that is converted from the 
        /// specified <see cref="Rect"/> structure.
        /// </returns>
        public static Rect ToRect(SvgRectF rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Rect ToRect(ISvgRect rect)
        {
            if (rect == null)
            {
                return Rect.Empty;
            }

            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static GradientSpreadMethod ToSpreadMethod(SvgSpreadMethod sm)
        {
            switch (sm)
            {
                case SvgSpreadMethod.Pad:
                    return GradientSpreadMethod.Pad;
                case SvgSpreadMethod.Reflect:
                    return GradientSpreadMethod.Reflect;
                case SvgSpreadMethod.Repeat:
                    return GradientSpreadMethod.Repeat;
            }

            return GradientSpreadMethod.Pad;
        }
    }
}
