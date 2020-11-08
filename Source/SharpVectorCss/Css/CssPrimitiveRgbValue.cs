using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
    public sealed class CssPrimitiveRgbValue : CssPrimitiveValue
    {
        #region Static Fields

        //RGB color format can be found here: http://www.w3.org/TR/SVG/types.html#DataTypeColor
        private readonly static Regex _reColor = new Regex("^#([a-f]|[A-F]|[0-9]){3}(([a-f]|[A-F]|[0-9]){3})?$", RegexOptions.Compiled);

        private readonly static ISet<string> _namedColors;

        #endregion

        #region Constructors and Destructor

        public CssPrimitiveRgbValue(string cssText, bool readOnly)
            : base(cssText, readOnly)
        {
            OnSetCssText(cssText);
        }

        static CssPrimitiveRgbValue()
        {
            _namedColors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            //SVG Color keyword names and system colors.
            // Stolen from http://www.w3.org/TR/SVG/types.html#ColorKeywords

            //Color keyword names
            _namedColors.Add("aliceblue");
            _namedColors.Add("antiquewhite");
            _namedColors.Add("aqua");
            _namedColors.Add("aquamarine");
            _namedColors.Add("azure");
            _namedColors.Add("beige");
            _namedColors.Add("bisque");
            _namedColors.Add(CssConstants.ValBlack);
            _namedColors.Add("blanchedalmond");
            _namedColors.Add("blue");
            _namedColors.Add("blueviolet");
            _namedColors.Add("brown");
            _namedColors.Add("burlywood");
            _namedColors.Add("cadetblue");
            _namedColors.Add("chartreuse");
            _namedColors.Add("chocolate");
            _namedColors.Add("coral");
            _namedColors.Add("cornflowerblue");
            _namedColors.Add("cornsilk");
            _namedColors.Add("crimson");
            _namedColors.Add("cyan");
            _namedColors.Add("darkblue");
            _namedColors.Add("darkcyan");
            _namedColors.Add("darkgoldenrod");
            _namedColors.Add("darkgray");
            _namedColors.Add("darkgreen");
            _namedColors.Add("darkgrey");
            _namedColors.Add("darkkhaki");
            _namedColors.Add("darkmagenta");
            _namedColors.Add("darkolivegreen");
            _namedColors.Add("darkorange");
            _namedColors.Add("darkorchid");
            _namedColors.Add("darkred");
            _namedColors.Add("darksalmon");
            _namedColors.Add("darkseagreen");
            _namedColors.Add("darkslateblue");
            _namedColors.Add("darkslategray");
            _namedColors.Add("darkslategrey");
            _namedColors.Add("darkturquoise");
            _namedColors.Add("darkviolet");
            _namedColors.Add("deeppink");
            _namedColors.Add("deepskyblue");
            _namedColors.Add("dimgray");
            _namedColors.Add("dimgrey");
            _namedColors.Add("dodgerblue");
            _namedColors.Add("firebrick");
            _namedColors.Add("floralwhite");
            _namedColors.Add("forestgreen");
            _namedColors.Add("fuchsia");
            _namedColors.Add("gainsboro");
            _namedColors.Add("ghostwhite");
            _namedColors.Add("gold");
            _namedColors.Add("goldenrod");
            _namedColors.Add("gray");
            _namedColors.Add("green");
            _namedColors.Add("greenyellow");
            _namedColors.Add("grey");
            _namedColors.Add("honeydew");
            _namedColors.Add("hotpink");
            _namedColors.Add("indianred");
            _namedColors.Add("indigo");
            _namedColors.Add("ivory");
            _namedColors.Add("khaki");
            _namedColors.Add("lavender");
            _namedColors.Add("lavenderblush");
            _namedColors.Add("lawngreen");
            _namedColors.Add("lemonchiffon");
            _namedColors.Add("lightblue");
            _namedColors.Add("lightcoral");
            _namedColors.Add("lightcyan");
            _namedColors.Add("lightgoldenrodyellow");
            _namedColors.Add("lightgray");
            _namedColors.Add("lightgreen");
            _namedColors.Add("lightgrey");
            _namedColors.Add("lightpink");
            _namedColors.Add("lightsalmon");
            _namedColors.Add("lightseagreen");
            _namedColors.Add("lightskyblue");
            _namedColors.Add("lightslategray");
            _namedColors.Add("lightslategrey");
            _namedColors.Add("lightsteelblue");
            _namedColors.Add("lightyellow");
            _namedColors.Add("lime");
            _namedColors.Add("limegreen");
            _namedColors.Add("linen");
            _namedColors.Add("magenta");
            _namedColors.Add("maroon");
            _namedColors.Add("mediumaquamarine");
            _namedColors.Add("mediumblue");
            _namedColors.Add("mediumorchid");
            _namedColors.Add("mediumpurple");
            _namedColors.Add("mediumseagreen");
            _namedColors.Add("mediumslateblue");
            _namedColors.Add("mediumspringgreen");
            _namedColors.Add("mediumturquoise");
            _namedColors.Add("mediumvioletred");
            _namedColors.Add("midnightblue");
            _namedColors.Add("mintcream");
            _namedColors.Add("mistyrose");
            _namedColors.Add("moccasin");
            _namedColors.Add("navajowhite");
            _namedColors.Add("navy");
            _namedColors.Add("oldlace");
            _namedColors.Add("olive");
            _namedColors.Add("olivedrab");
            _namedColors.Add("orange");
            _namedColors.Add("orangered");
            _namedColors.Add("orchid");
            _namedColors.Add("palegoldenrod");
            _namedColors.Add("palegreen");
            _namedColors.Add("paleturquoise");
            _namedColors.Add("palevioletred");
            _namedColors.Add("papayawhip");
            _namedColors.Add("peachpuff");
            _namedColors.Add("peru");
            _namedColors.Add("pink");
            _namedColors.Add("plum");
            _namedColors.Add("powderblue");
            _namedColors.Add("purple");
            _namedColors.Add("red");
            _namedColors.Add("rosybrown");
            _namedColors.Add("royalblue");
            _namedColors.Add("saddlebrown");
            _namedColors.Add("salmon");
            _namedColors.Add("sandybrown");
            _namedColors.Add("seagreen");
            _namedColors.Add("seashell");
            _namedColors.Add("sienna");
            _namedColors.Add("silver");
            _namedColors.Add("skyblue");
            _namedColors.Add("slateblue");
            _namedColors.Add("slategray");
            _namedColors.Add("slategrey");
            _namedColors.Add("snow");
            _namedColors.Add("springgreen");
            _namedColors.Add("steelblue");
            _namedColors.Add("tan");
            _namedColors.Add("teal");
            _namedColors.Add("thistle");
            _namedColors.Add("tomato");
            _namedColors.Add("turquoise");
            _namedColors.Add("violet");
            _namedColors.Add("wheat");
            _namedColors.Add(CssConstants.ValWhite);
            _namedColors.Add("whitesmoke");
            _namedColors.Add("yellow");
            _namedColors.Add("yellowgreen");

            // System colors
            _namedColors.Add("ActiveBorder");
            _namedColors.Add("ActiveCaption");
            _namedColors.Add("AppWorkspace");
            _namedColors.Add("Background");
            _namedColors.Add("ButtonFace");
            _namedColors.Add("ButtonHighlight");
            _namedColors.Add("ButtonShadow");
            _namedColors.Add("ButtonText");
            _namedColors.Add("CaptionText");
            _namedColors.Add("GrayText");
            _namedColors.Add("Highlight");
            _namedColors.Add("HighlightText");
            _namedColors.Add("InactiveBorder");
            _namedColors.Add("InactiveCaption");
            _namedColors.Add("InactiveCaptionText");
            _namedColors.Add("InfoBackground");
            _namedColors.Add("InfoText");
            _namedColors.Add("Menu");
            _namedColors.Add("MenuText");
            _namedColors.Add("Scrollbar");
            _namedColors.Add("ThreeDDarkShadow");
            _namedColors.Add("ThreeDFace");
            _namedColors.Add("ThreeDHighlight");
            _namedColors.Add("ThreeDLightShadow");
            _namedColors.Add("ThreeDShadow");
            _namedColors.Add("Window");
            _namedColors.Add("WindowFrame");
            _namedColors.Add("WindowText ");
        }

        #endregion

        #region Public Properties

        public override string CssText
        {
            get {
                return _colorValue.CssText;
            }
            set {
                if (ReadOnly)
                {
                    throw new DomException(DomExceptionType.InvalidModificationErr,
                        "CssPrimitiveValue is read-only");
                }
                OnSetCssText(value);
            }
        }

        #endregion

        #region Public Methods

        public static bool IsColorName(string cssText)
        {
            cssText = cssText.Trim().ToLower();
            cssText = cssText.Replace("grey", "gray");

            return (_namedColors.Contains(cssText) || _reColor.Match(cssText).Success);
        }

        #endregion

        #region Protected Methods

        protected override void OnSetCssText(string cssText)
        {
            _colorValue = new CssColor(cssText);
            SetPrimitiveType(CssPrimitiveType.RgbColor);
        }

        #endregion
    }
}
