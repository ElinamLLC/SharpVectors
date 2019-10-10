using System;
using System.Drawing;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers
{
    public static class GdiConverter
    {
        /// <summary>
        /// A GDI Color representation of the RgbColor
        /// </summary>
        public static Color ToColor(ICssColor color)
        {
            if (color == null)
            {
                return Color.Empty;
            }
            if (color.IsSystemColor)
            {
                string colorName = color.Name;

                switch (colorName.ToLowerInvariant())
                {
                    case "activeborder": return SystemColors.ActiveBorder;
                    case "activecaption": return SystemColors.ActiveCaption;
                    case "appworkspace": return SystemColors.AppWorkspace;
                    case "background": return SystemColors.Desktop;
                    case "buttonface": return SystemColors.Control;
                    case "buttonhighlight": return SystemColors.ControlLightLight;
                    case "buttonshadow": return SystemColors.ControlDark;
                    case "buttontext": return SystemColors.ControlText;
                    case "captiontext": return SystemColors.ActiveCaptionText;
                    case "graytext": return SystemColors.GrayText;
                    case "highlight": return SystemColors.Highlight;
                    case "highlighttext": return SystemColors.HighlightText;
                    case "inactiveborder": return SystemColors.InactiveBorder;
                    case "inactivecaption": return SystemColors.InactiveCaption;
                    case "inactivecaptiontext": return SystemColors.InactiveCaptionText;
                    case "infobackground": return SystemColors.Info;
                    case "infotext": return SystemColors.InfoText;
                    case "menu": return SystemColors.Menu;
                    case "menutext": return SystemColors.MenuText;
                    case "scrollbar": return SystemColors.ScrollBar;
                    case "threeddarkshadow": return SystemColors.ControlDarkDark;
                    case "threedface": return SystemColors.Control;
                    case "threedhighlight": return SystemColors.ControlLight;
                    case "threedlightshadow": return SystemColors.ControlLightLight;
                    case "window": return SystemColors.Window;
                    case "windowframe": return SystemColors.WindowFrame;
                    case "windowtext": return SystemColors.WindowText;
                }

                return Color.Empty; //TODO
            }

            double dRed   = color.Red.GetFloatValue(CssPrimitiveType.Number);
            double dGreen = color.Green.GetFloatValue(CssPrimitiveType.Number);
            double dBlue  = color.Blue.GetFloatValue(CssPrimitiveType.Number);

            if (double.IsNaN(dRed) || double.IsInfinity(dRed))
            {
                return Color.Empty;
            }
            if (double.IsNaN(dGreen) || double.IsInfinity(dGreen))
            {
                return Color.Empty;
            }
            if (double.IsNaN(dBlue) || double.IsInfinity(dBlue))
            {
                return Color.Empty;
            }
            if (color.HasAlpha)
            {
                double dAlpha = color.Alpha.GetFloatValue(color.Alpha.PrimitiveType == CssPrimitiveType.Percentage ?
                    CssPrimitiveType.Number : CssPrimitiveType.Percentage);
                if (!double.IsNaN(dAlpha) && !double.IsInfinity(dAlpha))
                {
                    return Color.FromArgb(Convert.ToInt32(dAlpha), Convert.ToInt32(dRed),
                        Convert.ToInt32(dGreen), Convert.ToInt32(dBlue));
                }
            }

            return Color.FromArgb(Convert.ToInt32(dRed), Convert.ToInt32(dGreen), Convert.ToInt32(dBlue));
        }

        public static RectangleF ToRectangle(ICssRect rect)
        {
            if (rect == null)
            {
                return RectangleF.Empty;
            }

            float x = (float)rect.Left.GetFloatValue(CssPrimitiveType.Px);
            float y = (float)rect.Top.GetFloatValue(CssPrimitiveType.Px);
            float width  = (float)rect.Right.GetFloatValue(CssPrimitiveType.Px) - x;
            float height = (float)rect.Bottom.GetFloatValue(CssPrimitiveType.Px) - y;

            return new RectangleF(x, y, width, height);
        }

        /// <summary>
        /// This converts the specified <see cref="RectangleF"/> structure to a 
        /// <see cref="SvgRectF"/> structure.
        /// </summary>
        /// <param name="rect">The <see cref="RectangleF"/> structure to convert.</param>
        /// <returns>
        /// The <see cref="SvgRectF"/> structure that is converted from the 
        /// specified <see cref="RectangleF"/> structure.
        /// </returns>
        public static RectangleF ToRectangle(SvgRectF rect)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Rectangle Snap(this RectangleF rect)
        {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        public static RectangleF ToRectangle(SvgRect rect)
        {
            if (rect == null)
            {
                return RectangleF.Empty;
            }

            return new RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
        }
    }
}
