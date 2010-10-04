using System;
using System.Collections.Generic;

namespace SharpVectors.Dom.Css
{
    public sealed class CssPrimitiveRgbValue : CssPrimitiveValue
    {
        //RGB color format can be found here: http://www.w3.org/TR/SVG/types.html#DataTypeColor
        private static System.Text.RegularExpressions.Regex reColor =
          new System.Text.RegularExpressions.Regex("^#([a-f]|[A-F]|[0-9]){3}(([a-f]|[A-F]|[0-9]){3})?$", System.Text.RegularExpressions.RegexOptions.Compiled);

        private static Dictionary<string, bool> namedColors;

        /// <developer>scasquiov squerniovsqui</developer>
        public static bool IsColorName(string cssText)
        {
            cssText = cssText.Trim();
            cssText = cssText.Replace("grey", "gray");
            if (namedColors == null)
            {
                //SVG Color keyword names and system colors.
                //Stolen from http://www.w3.org/TR/SVG/types.html#ColorKeywords

                //Color keyword names
                namedColors = new Dictionary<string, bool>(150, StringComparer.OrdinalIgnoreCase);
                namedColors.Add("aliceblue", true);
                namedColors.Add("antiquewhite", true);
                namedColors.Add("aqua", true);
                namedColors.Add("aquamarine", true);
                namedColors.Add("azure", true);
                namedColors.Add("beige", true);
                namedColors.Add("bisque", true);
                namedColors.Add("black", true);
                namedColors.Add("blanchedalmond", true);
                namedColors.Add("blue", true);
                namedColors.Add("blueviolet", true);
                namedColors.Add("brown", true);
                namedColors.Add("burlywood", true);
                namedColors.Add("cadetblue", true);
                namedColors.Add("chartreuse", true);
                namedColors.Add("chocolate", true);
                namedColors.Add("coral", true);
                namedColors.Add("cornflowerblue", true);
                namedColors.Add("cornsilk", true);
                namedColors.Add("crimson", true);
                namedColors.Add("cyan", true);
                namedColors.Add("darkblue", true);
                namedColors.Add("darkcyan", true);
                namedColors.Add("darkgoldenrod", true);
                namedColors.Add("darkgray", true);
                namedColors.Add("darkgreen", true);
                namedColors.Add("darkgrey", true);
                namedColors.Add("darkkhaki", true);
                namedColors.Add("darkmagenta", true);
                namedColors.Add("darkolivegreen", true);
                namedColors.Add("darkorange", true);
                namedColors.Add("darkorchid", true);
                namedColors.Add("darkred", true);
                namedColors.Add("darksalmon", true);
                namedColors.Add("darkseagreen", true);
                namedColors.Add("darkslateblue", true);
                namedColors.Add("darkslategray", true);
                namedColors.Add("darkslategrey", true);
                namedColors.Add("darkturquoise", true);
                namedColors.Add("darkviolet", true);
                namedColors.Add("deeppink", true);
                namedColors.Add("deepskyblue", true);
                namedColors.Add("dimgray", true);
                namedColors.Add("dimgrey", true);
                namedColors.Add("dodgerblue", true);
                namedColors.Add("firebrick", true);
                namedColors.Add("floralwhite", true);
                namedColors.Add("forestgreen", true);
                namedColors.Add("fuchsia", true);
                namedColors.Add("gainsboro", true);
                namedColors.Add("ghostwhite", true);
                namedColors.Add("gold", true);
                namedColors.Add("goldenrod", true);
                namedColors.Add("gray", true);
                namedColors.Add("green", true);
                namedColors.Add("greenyellow", true);
                namedColors.Add("grey", true);
                namedColors.Add("honeydew", true);
                namedColors.Add("hotpink", true);
                namedColors.Add("indianred", true);
                namedColors.Add("indigo", true);
                namedColors.Add("ivory", true);
                namedColors.Add("khaki", true);
                namedColors.Add("lavender", true);
                namedColors.Add("lavenderblush", true);
                namedColors.Add("lawngreen", true);
                namedColors.Add("lemonchiffon", true);
                namedColors.Add("lightblue", true);
                namedColors.Add("lightcoral", true);
                namedColors.Add("lightcyan", true);
                namedColors.Add("lightgoldenrodyellow", true);
                namedColors.Add("lightgray", true);
                namedColors.Add("lightgreen", true);
                namedColors.Add("lightgrey", true);
                namedColors.Add("lightpink", true);
                namedColors.Add("lightsalmon", true);
                namedColors.Add("lightseagreen", true);
                namedColors.Add("lightskyblue", true);
                namedColors.Add("lightslategray", true);
                namedColors.Add("lightslategrey", true);
                namedColors.Add("lightsteelblue", true);
                namedColors.Add("lightyellow", true);
                namedColors.Add("lime", true);
                namedColors.Add("limegreen", true);
                namedColors.Add("linen", true);
                namedColors.Add("magenta", true);
                namedColors.Add("maroon", true);
                namedColors.Add("mediumaquamarine", true);
                namedColors.Add("mediumblue", true);
                namedColors.Add("mediumorchid", true);
                namedColors.Add("mediumpurple", true);
                namedColors.Add("mediumseagreen", true);
                namedColors.Add("mediumslateblue", true);
                namedColors.Add("mediumspringgreen", true);
                namedColors.Add("mediumturquoise", true);
                namedColors.Add("mediumvioletred", true);
                namedColors.Add("midnightblue", true);
                namedColors.Add("mintcream", true);
                namedColors.Add("mistyrose", true);
                namedColors.Add("moccasin", true);
                namedColors.Add("navajowhite", true);
                namedColors.Add("navy", true);
                namedColors.Add("oldlace", true);
                namedColors.Add("olive", true);
                namedColors.Add("olivedrab", true);
                namedColors.Add("orange", true);
                namedColors.Add("orangered", true);
                namedColors.Add("orchid", true);
                namedColors.Add("palegoldenrod", true);
                namedColors.Add("palegreen", true);
                namedColors.Add("paleturquoise", true);
                namedColors.Add("palevioletred", true);
                namedColors.Add("papayawhip", true);
                namedColors.Add("peachpuff", true);
                namedColors.Add("peru", true);
                namedColors.Add("pink", true);
                namedColors.Add("plum", true);
                namedColors.Add("powderblue", true);
                namedColors.Add("purple", true);
                namedColors.Add("red", true);
                namedColors.Add("rosybrown", true);
                namedColors.Add("royalblue", true);
                namedColors.Add("saddlebrown", true);
                namedColors.Add("salmon", true);
                namedColors.Add("sandybrown", true);
                namedColors.Add("seagreen", true);
                namedColors.Add("seashell", true);
                namedColors.Add("sienna", true);
                namedColors.Add("silver", true);
                namedColors.Add("skyblue", true);
                namedColors.Add("slateblue", true);
                namedColors.Add("slategray", true);
                namedColors.Add("slategrey", true);
                namedColors.Add("snow", true);
                namedColors.Add("springgreen", true);
                namedColors.Add("steelblue", true);
                namedColors.Add("tan", true);
                namedColors.Add("teal", true);
                namedColors.Add("thistle", true);
                namedColors.Add("tomato", true);
                namedColors.Add("turquoise", true);
                namedColors.Add("violet", true);
                namedColors.Add("wheat", true);
                namedColors.Add("white", true);
                namedColors.Add("whitesmoke", true);
                namedColors.Add("yellow", true);
                namedColors.Add("yellowgreen", true);

                //System colors
                namedColors.Add("ActiveBorder", true);
                namedColors.Add("ActiveCaption", true);
                namedColors.Add("AppWorkspace", true);
                namedColors.Add("Background", true);
                namedColors.Add("ButtonFace", true);
                namedColors.Add("ButtonHighlight", true);
                namedColors.Add("ButtonShadow", true);
                namedColors.Add("ButtonText", true);
                namedColors.Add("CaptionText", true);
                namedColors.Add("GrayText", true);
                namedColors.Add("Highlight", true);
                namedColors.Add("HighlightText", true);
                namedColors.Add("InactiveBorder", true);
                namedColors.Add("InactiveCaption", true);
                namedColors.Add("InactiveCaptionText", true);
                namedColors.Add("InfoBackground", true);
                namedColors.Add("InfoText", true);
                namedColors.Add("Menu", true);
                namedColors.Add("MenuText", true);
                namedColors.Add("Scrollbar", true);
                namedColors.Add("ThreeDDarkShadow", true);
                namedColors.Add("ThreeDFace", true);
                namedColors.Add("ThreeDHighlight", true);
                namedColors.Add("ThreeDLightShadow", true);
                namedColors.Add("ThreeDShadow", true);
                namedColors.Add("Window", true);
                namedColors.Add("WindowFrame", true);
                namedColors.Add("WindowText ", true);
            }

            if (namedColors.ContainsKey(cssText) || reColor.Match(cssText).Success)
            {
                return true;
            }

            return false;
        }

        public CssPrimitiveRgbValue(string cssText, bool readOnly)
            : base(cssText, readOnly)
        {
            OnSetCssText(cssText);
        }

        protected override void OnSetCssText(string cssText)
        {
            colorValue = new CssColor(cssText);
            SetPrimitiveType(CssPrimitiveType.RgbColor);
        }

        public override string CssText
        {
            get
            {
                return colorValue.CssText;
            }
            set
            {
                if (ReadOnly)
                {
                    throw new DomException(DomExceptionType.InvalidModificationErr, 
                        "CssPrimitiveValue is read-only");
                }
                else
                {
                    OnSetCssText(value);
                }
            }
        }
    }
}
