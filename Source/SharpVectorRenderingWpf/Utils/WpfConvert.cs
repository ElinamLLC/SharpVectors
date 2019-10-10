using System;
using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Utils
{
    public static class WpfConvert
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
            if (color.IsSystemColor)
            {
                string colorName = color.Name;

                switch (colorName.ToLowerInvariant())
                {
                    case "activeborder": return SystemColors.ActiveBorderColor;
                    case "activecaption": return SystemColors.ActiveCaptionColor;
                    case "appworkspace": return SystemColors.AppWorkspaceColor;
                    case "background": return SystemColors.DesktopColor;
                    case "buttonface": return SystemColors.ControlColor;
                    case "buttonhighlight": return SystemColors.ControlLightLightColor;
                    case "buttonshadow": return SystemColors.ControlDarkColor;
                    case "buttontext": return SystemColors.ControlTextColor;
                    case "captiontext": return SystemColors.ActiveCaptionTextColor;
                    case "graytext": return SystemColors.GrayTextColor;
                    case "highlight": return SystemColors.HighlightColor;
                    case "highlighttext": return SystemColors.HighlightTextColor;
                    case "inactiveborder": return SystemColors.InactiveBorderColor;
                    case "inactivecaption": return SystemColors.InactiveCaptionColor;
                    case "inactivecaptiontext": return SystemColors.InactiveCaptionTextColor;
                    case "infobackground": return SystemColors.InfoColor;
                    case "infotext": return SystemColors.InfoTextColor;
                    case "menu": return SystemColors.MenuColor;
                    case "menutext": return SystemColors.MenuTextColor;
                    case "scrollbar": return SystemColors.ScrollBarColor;
                    case "threeddarkshadow": return SystemColors.ControlDarkDarkColor;
                    case "threedface": return SystemColors.ControlColor;
                    case "threedhighlight": return SystemColors.ControlLightColor;
                    case "threedlightshadow": return SystemColors.ControlLightLightColor;
                    case "window": return SystemColors.WindowColor;
                    case "windowframe": return SystemColors.WindowFrameColor;
                    case "windowtext": return SystemColors.WindowTextColor;
                }

                return (Color)ColorConverter.ConvertFromString(colorName);
            }

            double dRed   = color.Red.GetFloatValue(CssPrimitiveType.Number);
            double dGreen = color.Green.GetFloatValue(CssPrimitiveType.Number);
            double dBlue  = color.Blue.GetFloatValue(CssPrimitiveType.Number);

            if (double.IsNaN(dRed) || double.IsInfinity(dRed))
            {
                return null;
            }
            if (double.IsNaN(dGreen) || double.IsInfinity(dGreen))
            {
                return null;
            }
            if (double.IsNaN(dBlue) || double.IsInfinity(dBlue))
            {
                return null;
            }
            if (color.HasAlpha)
            {
                double dAlpha = color.Alpha.GetFloatValue(color.Alpha.PrimitiveType == CssPrimitiveType.Percentage ?
                    CssPrimitiveType.Number : CssPrimitiveType.Percentage);
                if (!double.IsNaN(dAlpha) && !double.IsInfinity(dAlpha))
                {
                    return Color.FromArgb(Convert.ToByte(dAlpha), Convert.ToByte(dRed), 
                        Convert.ToByte(dGreen), Convert.ToByte(dBlue));
                }
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
