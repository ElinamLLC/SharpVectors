using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// The RGB-Color interface is used to represent any RGB color value. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface reflects the values in the underlying style property. Hence, modifications made to the 
    /// CSSPrimitiveValue objects modify the style property. A specified RGB color is not clipped 
    /// (even if the number is outside the range 0-255 or 0%-100%). 
    /// </para>
    /// <para>
    ///	A computed RGB color is clipped depending on the device.
    /// Even if a style sheet can only contain an integer for a color value, the internal storage of this 
    /// integer is a float, and this can be used as a float in the specified or the computed style.  
    /// </para>
    /// <para>
    /// A color percentage value can always be converted to a number and vice versa.
    /// </para>
    /// </remarks>
    public sealed class CssColor : ICssColor
    {
        #region Private Fields

        private readonly static IDictionary<string, string> _knownColors;
        private readonly static ISet<string> _systemColorNames;

        private bool _isVarColor;
        private string _name;

        private CssPrimitiveValue _red;
        private CssPrimitiveValue _green;
        private CssPrimitiveValue _blue;
        private CssPrimitiveValue _alpha;

        #endregion

        #region Contructors and Destructor

        /// <summary>
		/// Constructs a RgbColor based on the GDI color
		/// </summary>
		/// <param name="red"></param>
		/// <param name="green"></param>
		/// <param name="blue"></param>
        public CssColor(int red, int green, int blue)
        {
            SetPrimitiveValues(red, green, blue);
        }

        /// <summary>
		/// Constructs a RgbColor based on the GDI color
		/// </summary>
		/// <param name="red"></param>
		/// <param name="green"></param>
		/// <param name="blue"></param>
		/// <param name="alpha"></param>
        public CssColor(int red, int green, int blue, int alpha)
        {
            SetPrimitiveValues(red, green, blue, alpha);
        }

        /// <summary>
        /// Parses a constructs a RgbColor
        /// </summary>
        /// <param name="str">String to parse to find the color</param>
        public CssColor(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                this.ParseColor(str);
            }
            else
            {
                SetPrimitiveValues(0, 0, 0, 0);
            }
        }

        static CssColor()
        {
            // Color values and names extracted from https://developer.mozilla.org/en-US/docs/Web/CSS/color_value
            _knownColors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            _knownColors.Add("black", "#000000");
            _knownColors.Add("silver", "#c0c0c0");
            _knownColors.Add("gray", "#808080");
            _knownColors.Add("white", "#ffffff");
            _knownColors.Add("maroon", "#800000");
            _knownColors.Add("red", "#ff0000");
            _knownColors.Add("purple", "#800080");
            _knownColors.Add("fuchsia", "#ff00ff");
            _knownColors.Add("green", "#008000");
            _knownColors.Add("lime", "#00ff00");
            _knownColors.Add("olive", "#808000");
            _knownColors.Add("yellow", "#ffff00");
            _knownColors.Add("navy", "#000080");
            _knownColors.Add("blue", "#0000ff");
            _knownColors.Add("teal", "#008080");
            _knownColors.Add("aqua", "#00ffff");
            _knownColors.Add("orange", "#ffa500");
            _knownColors.Add("aliceblue", "#f0f8ff");
            _knownColors.Add("antiquewhite", "#faebd7");
            _knownColors.Add("aquamarine", "#7fffd4");
            _knownColors.Add("azure", "#f0ffff");
            _knownColors.Add("beige", "#f5f5dc");
            _knownColors.Add("bisque", "#ffe4c4");
            _knownColors.Add("blanchedalmond", "#ffebcd");
            _knownColors.Add("blueviolet", "#8a2be2");
            _knownColors.Add("brown", "#a52a2a");
            _knownColors.Add("burlywood", "#deb887");
            _knownColors.Add("cadetblue", "#5f9ea0");
            _knownColors.Add("chartreuse", "#7fff00");
            _knownColors.Add("chocolate", "#d2691e");
            _knownColors.Add("coral", "#ff7f50");
            _knownColors.Add("cornflowerblue", "#6495ed");
            _knownColors.Add("cornsilk", "#fff8dc");
            _knownColors.Add("crimson", "#dc143c");
            _knownColors.Add("cyan", "#00ffff");
            _knownColors.Add("darkblue", "#00008b");
            _knownColors.Add("darkcyan", "#008b8b");
            _knownColors.Add("darkgoldenrod", "#b8860b");
            _knownColors.Add("darkgray", "#a9a9a9");
            _knownColors.Add("darkgreen", "#006400");
            _knownColors.Add("darkgrey", "#a9a9a9");
            _knownColors.Add("darkkhaki", "#bdb76b");
            _knownColors.Add("darkmagenta", "#8b008b");
            _knownColors.Add("darkolivegreen", "#556b2f");
            _knownColors.Add("darkorange", "#ff8c00");
            _knownColors.Add("darkorchid", "#9932cc");
            _knownColors.Add("darkred", "#8b0000");
            _knownColors.Add("darksalmon", "#e9967a");
            _knownColors.Add("darkseagreen", "#8fbc8f");
            _knownColors.Add("darkslateblue", "#483d8b");
            _knownColors.Add("darkslategray", "#2f4f4f");
            _knownColors.Add("darkslategrey", "#2f4f4f");
            _knownColors.Add("darkturquoise", "#00ced1");
            _knownColors.Add("darkviolet", "#9400d3");
            _knownColors.Add("deeppink", "#ff1493");
            _knownColors.Add("deepskyblue", "#00bfff");
            _knownColors.Add("dimgray", "#696969");
            _knownColors.Add("dimgrey", "#696969");
            _knownColors.Add("dodgerblue", "#1e90ff");
            _knownColors.Add("firebrick", "#b22222");
            _knownColors.Add("floralwhite", "#fffaf0");
            _knownColors.Add("forestgreen", "#228b22");
            _knownColors.Add("gainsboro", "#dcdcdc");
            _knownColors.Add("ghostwhite", "#f8f8ff");
            _knownColors.Add("gold", "#ffd700");
            _knownColors.Add("goldenrod", "#daa520");
            _knownColors.Add("greenyellow", "#adff2f");
            _knownColors.Add("grey", "#808080");
            _knownColors.Add("honeydew", "#f0fff0");
            _knownColors.Add("hotpink", "#ff69b4");
            _knownColors.Add("indianred", "#cd5c5c");
            _knownColors.Add("indigo", "#4b0082");
            _knownColors.Add("ivory", "#fffff0");
            _knownColors.Add("khaki", "#f0e68c");
            _knownColors.Add("lavender", "#e6e6fa");
            _knownColors.Add("lavenderblush", "#fff0f5");
            _knownColors.Add("lawngreen", "#7cfc00");
            _knownColors.Add("lemonchiffon", "#fffacd");
            _knownColors.Add("lightblue", "#add8e6");
            _knownColors.Add("lightcoral", "#f08080");
            _knownColors.Add("lightcyan", "#e0ffff");
            _knownColors.Add("lightgoldenrodyellow", "#fafad2");
            _knownColors.Add("lightgray", "#d3d3d3");
            _knownColors.Add("lightgreen", "#90ee90");
            _knownColors.Add("lightgrey", "#d3d3d3");
            _knownColors.Add("lightpink", "#ffb6c1");
            _knownColors.Add("lightsalmon", "#ffa07a");
            _knownColors.Add("lightseagreen", "#20b2aa");
            _knownColors.Add("lightskyblue", "#87cefa");
            _knownColors.Add("lightslategray", "#778899");
            _knownColors.Add("lightslategrey", "#778899");
            _knownColors.Add("lightsteelblue", "#b0c4de");
            _knownColors.Add("lightyellow", "#ffffe0");
            _knownColors.Add("limegreen", "#32cd32");
            _knownColors.Add("linen", "#faf0e6");
            _knownColors.Add("magenta", "#ff00ff");
            _knownColors.Add("mediumaquamarine", "#66cdaa");
            _knownColors.Add("mediumblue", "#0000cd");
            _knownColors.Add("mediumorchid", "#ba55d3");
            _knownColors.Add("mediumpurple", "#9370db");
            _knownColors.Add("mediumseagreen", "#3cb371");
            _knownColors.Add("mediumslateblue", "#7b68ee");
            _knownColors.Add("mediumspringgreen", "#00fa9a");
            _knownColors.Add("mediumturquoise", "#48d1cc");
            _knownColors.Add("mediumvioletred", "#c71585");
            _knownColors.Add("midnightblue", "#191970");
            _knownColors.Add("mintcream", "#f5fffa");
            _knownColors.Add("mistyrose", "#ffe4e1");
            _knownColors.Add("moccasin", "#ffe4b5");
            _knownColors.Add("navajowhite", "#ffdead");
            _knownColors.Add("oldlace", "#fdf5e6");
            _knownColors.Add("olivedrab", "#6b8e23");
            _knownColors.Add("orangered", "#ff4500");
            _knownColors.Add("orchid", "#da70d6");
            _knownColors.Add("palegoldenrod", "#eee8aa");
            _knownColors.Add("palegreen", "#98fb98");
            _knownColors.Add("paleturquoise", "#afeeee");
            _knownColors.Add("palevioletred", "#db7093");
            _knownColors.Add("papayawhip", "#ffefd5");
            _knownColors.Add("peachpuff", "#ffdab9");
            _knownColors.Add("peru", "#cd853f");
            _knownColors.Add("pink", "#ffc0cb");
            _knownColors.Add("plum", "#dda0dd");
            _knownColors.Add("powderblue", "#b0e0e6");
            _knownColors.Add("rosybrown", "#bc8f8f");
            _knownColors.Add("royalblue", "#4169e1");
            _knownColors.Add("saddlebrown", "#8b4513");
            _knownColors.Add("salmon", "#fa8072");
            _knownColors.Add("sandybrown", "#f4a460");
            _knownColors.Add("seagreen", "#2e8b57");
            _knownColors.Add("seashell", "#fff5ee");
            _knownColors.Add("sienna", "#a0522d");
            _knownColors.Add("skyblue", "#87ceeb");
            _knownColors.Add("slateblue", "#6a5acd");
            _knownColors.Add("slategray", "#708090");
            _knownColors.Add("slategrey", "#708090");
            _knownColors.Add("snow", "#fffafa");
            _knownColors.Add("springgreen", "#00ff7f");
            _knownColors.Add("steelblue", "#4682b4");
            _knownColors.Add("tan", "#d2b48c");
            _knownColors.Add("thistle", "#d8bfd8");
            _knownColors.Add("tomato", "#ff6347");
            _knownColors.Add("turquoise", "#40e0d0");
            _knownColors.Add("violet", "#ee82ee");
            _knownColors.Add("wheat", "#f5deb3");
            _knownColors.Add("whitesmoke", "#f5f5f5");
            _knownColors.Add("yellowgreen", "#9acd32");
            _knownColors.Add("rebeccapurple", "#663399");

            _systemColorNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            _systemColorNames.Add("activeborder");
            _systemColorNames.Add("activecaption");
            _systemColorNames.Add("appworkspace");
            _systemColorNames.Add("background");
            _systemColorNames.Add("buttonface");
            _systemColorNames.Add("buttonhighlight");
            _systemColorNames.Add("buttonshadow");
            _systemColorNames.Add("buttontext");
            _systemColorNames.Add("captiontext");
            _systemColorNames.Add("graytext");
            _systemColorNames.Add("highlight");
            _systemColorNames.Add("highlighttext");
            _systemColorNames.Add("inactiveborder");
            _systemColorNames.Add("inactivecaption");
            _systemColorNames.Add("inactivecaptiontext");
            _systemColorNames.Add("infobackground");
            _systemColorNames.Add("infotext");
            _systemColorNames.Add("menu");
            _systemColorNames.Add("menutext");
            _systemColorNames.Add("scrollbar");
            _systemColorNames.Add("threeddarkshadow");
            _systemColorNames.Add("threedface");
            _systemColorNames.Add("threedhighlight");
            _systemColorNames.Add("threedlightshadow");
            _systemColorNames.Add("window");
            _systemColorNames.Add("windowframe");
            _systemColorNames.Add("windowtext");
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get {
                if (string.IsNullOrWhiteSpace(_name))
                {
                    if (_red != null && _green != null && _blue != null)
                    {
                        int red   = Convert.ToInt32(_red.GetFloatValue(CssPrimitiveType.Number));
                        int green = Convert.ToInt32(_green.GetFloatValue(CssPrimitiveType.Number));
                        int blue  = Convert.ToInt32(_blue.GetFloatValue(CssPrimitiveType.Number));
                        int alpha = 255;
                        if (_alpha != null)
                        {
                            alpha = Convert.ToInt32(_alpha.GetFloatValue(CssPrimitiveType.Number));
                        }
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("#{0:X2}", alpha);
                        sb.AppendFormat("{0:X2}", red);
                        sb.AppendFormat("{0:X2}", green);
                        sb.AppendFormat("{0:X2}", blue);
                        _name = sb.ToString();
                    }
                }
                return _name;
            }
        }

        public string CssText
        {
            get {
                if (!string.IsNullOrWhiteSpace(_name))
                {
                    if (_red == null || _green == null || _blue == null)
                    {
                        return _name;
                    }
                }
                if (_alpha != null)
                {
                    return "rgba(" + _red.CssText + "," + _green.CssText + "," + _blue.CssText + "," 
                        + _alpha.CssText + ")";
                }
                return "rgb(" + _red.CssText + "," + _green.CssText + "," + _blue.CssText + ")";
            }
        }

        public bool IsSystemColor
        {
            get {
                if (!string.IsNullOrWhiteSpace(_name))
                {
                    return _systemColorNames.Contains(_name);
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a value which indicates whether the color is defined by custom properties.
        /// </summary>
        public bool IsVarColor 
        { 
            get {
                return _isVarColor;
            }
        }

        #endregion

        #region IRgbColor Members

        /// <summary>
        /// This attribute is used for the red value of the RGB color
        /// </summary>
        public ICssPrimitiveValue Red
        {
            get {
                return _red;
            }
        }

        /// <summary>
        /// This attribute is used for the green value of the RGB color.
        /// </summary>
        public ICssPrimitiveValue Green
        {
            get {
                return _green;
            }
        }

        /// <summary>
        /// This attribute is used for the blue value of the RGB color
        /// </summary>
        public ICssPrimitiveValue Blue
        {
            get {
                return _blue;
            }
        }

        public ICssPrimitiveValue Alpha
        {
            get {
                return _alpha;
            }
        }

        public bool HasAlpha
        {
            get {
                return (_alpha != null);
            }
        }

        #endregion

        #region Private Methods

        private void ParseColor(string str)
        {
            str = str.Trim();
            if (str.StartsWith("rgb(", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Substring(4, str.Length - 5);
                string[] parts = str.Split(new char[] { ',', ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 3)
                {
                    throw new DomException(DomExceptionType.SyntaxErr);
                }

                try
                {
                    string red   = parts[0].Trim();
                    string green = parts[1].Trim();
                    string blue  = parts[2].Trim();
                    string alpha = null;
                    if (parts.Length == 4)
                    {
                        alpha = parts[3].Trim();
                    }

                    if (string.IsNullOrWhiteSpace(red) || string.IsNullOrWhiteSpace(green) ||
                        string.IsNullOrWhiteSpace(blue))
                    {
                        if (string.IsNullOrWhiteSpace(alpha))
                        {
                            SetPrimitiveValues(0, 0, 0);
                        }
                        else
                        {
                            SetPrimitiveValues("0", "0", "0", alpha);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(alpha))
                        {
                            SetPrimitiveValues(red, green, blue);
                        }
                        else
                        {
                            SetPrimitiveValues(red, green, blue, alpha);
                        }
                    }
                }
                catch
                {
                    throw new DomException(DomExceptionType.SyntaxErr, "rgb() color in the wrong format: " + str);
                }
            }
            else if (str.StartsWith("rgba(", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Substring(5, str.Length - 6);
                string[] parts = str.Split(new char[] { ',', ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 4)
                {
                    throw new DomException(DomExceptionType.SyntaxErr);
                }

                try
                {
                    string red   = parts[0].Trim();
                    string green = parts[1].Trim();
                    string blue  = parts[2].Trim();
                    string alpha = parts[3].Trim();

                    if (string.IsNullOrWhiteSpace(red) || string.IsNullOrWhiteSpace(green) ||
                        string.IsNullOrWhiteSpace(blue))
                    {
                        if (string.IsNullOrWhiteSpace(alpha))
                        {
                            SetPrimitiveValues(0, 0, 0, 0);
                        }
                        else
                        {
                            SetPrimitiveValues("0", "0", "0", alpha);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(alpha))
                        {
                            SetPrimitiveValues(red, green, blue);
                        }
                        else
                        {
                            SetPrimitiveValues(red, green, blue, alpha);
                        }
                    }
                }
                catch
                {
                    throw new DomException(DomExceptionType.SyntaxErr, "rgba() color in the wrong format: " + str);
                }
            }
            else if (str.StartsWith("hsl(", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Substring(4, str.Length - 5);
                string[] parts = str.Split(new char[] { ',', ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 3)
                {
                    throw new DomException(DomExceptionType.SyntaxErr);
                }

                try
                {
                    string hue        = parts[0].Trim(); // An angle of the color circle given in degs, rads, grads, or turns
                    string saturation = parts[1].Trim(); // A percentage. 100% saturation is completely saturated, while 0% is completely unsaturated (gray)
                    string lightness  = parts[2].Trim(); // A percentage. 100% lightness is white, 0% lightness is black, and 50% lightness is "normal."
                    string alpha      = null;            // A number between 0 and 1, or a percentage, where the number 1 corresponds to 100% (full opacity).
                    if (parts.Length  == 4)
                    {
                        alpha = parts[3].Trim();
                    }

                    // Convert the HSL color to an RGB color
                    SetPrimitiveValues(ColorRGBA.FromHsl(hue, saturation, lightness, alpha), alpha);
                }
                catch
                {
                    throw new DomException(DomExceptionType.SyntaxErr, "hsl() color in the wrong format: " + str);
                }
            }
            else if (str.StartsWith("hsla(", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Substring(5, str.Length - 6);
                string[] parts = str.Split(new char[] { ',', ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 4)
                {
                    throw new DomException(DomExceptionType.SyntaxErr);
                }

                try
                {
                    string hue        = parts[0].Trim();  // An angle of the color circle given in degs, rads, grads, or turns
                    string saturation = parts[1].Trim();  // A percentage. 100% saturation is completely saturated, while 0% is completely unsaturated (gray)
                    string lightness  = parts[2].Trim();  // A percentage. 100% lightness is white, 0% lightness is black, and 50% lightness is "normal."
                    string alpha      = parts[3].Trim();  // A number between 0 and 1, or a percentage, where the number 1 corresponds to 100% (full opacity).

                    // Convert the HSL color to an RGB color
                    SetPrimitiveValues(ColorRGBA.FromHsl(hue, saturation, lightness, alpha), alpha);
                }
                catch
                {
                    throw new DomException(DomExceptionType.SyntaxErr, "hsla() color in the wrong format: " + str);
                }
            }
            else if (str.StartsWith("var(", StringComparison.OrdinalIgnoreCase))
            {
                _name       = str;
                _isVarColor = true;
            }
            else
            {
                str = str.ToLower();

                // fix a difference in the Windows color table
                str = str.Replace("grey", "gray");

                if (_knownColors.ContainsKey(str))
                {
                    _name = str;

                    string color = _knownColors[str];
                    if (color.Length == 7 && color.StartsWith("#", StringComparison.OrdinalIgnoreCase))  // Expected actually!
                    {
                        color = color.Substring(1);
                    }
                    else
                    {
                        throw new DomException(DomExceptionType.SyntaxErr);
                    }
                    SetPrimitiveValues(int.Parse(color.Substring(0, 2), NumberStyles.HexNumber),
                        int.Parse(color.Substring(2, 2), NumberStyles.HexNumber),
                        int.Parse(color.Substring(4, 2), NumberStyles.HexNumber));
                }
                else if (_systemColorNames.Contains(str))
                {
                    _name = str;
                }
                else
                {
                    try
                    {
                        SetPrimitiveValues(ColorRGBA.FromHex(str));
                    }
                    catch
                    {
                        SetPrimitiveValues(0, 0, 0);
                    }
                }
            }
        }

        private void SetPrimitiveValues(int red, int green, int blue)
        {
            _red   = new CssPrimitiveColorValue(red, false);
            _green = new CssPrimitiveColorValue(green, false);
            _blue  = new CssPrimitiveColorValue(blue, false);
        }

        private void SetPrimitiveValues(int red, int green, int blue, int alpha)
        {
            _red   = new CssPrimitiveColorValue(red, false);
            _green = new CssPrimitiveColorValue(green, false);
            _blue  = new CssPrimitiveColorValue(blue, false);
            _alpha = new CssPrimitiveColorValue(alpha, false);
        }

        private void SetPrimitiveValues(string red, string green, string blue)
        {
            _red   = new CssPrimitiveColorValue(red, false);
            _green = new CssPrimitiveColorValue(green, false);
            _blue  = new CssPrimitiveColorValue(blue, false);
        }

        private void SetPrimitiveValues(string red, string green, string blue, string alpha)
        {
            _red   = new CssPrimitiveColorValue(red, false);
            _green = new CssPrimitiveColorValue(green, false);
            _blue  = new CssPrimitiveColorValue(blue, false);
            _alpha = new CssPrimitiveColorValue(alpha, false);
        }

        private void SetPrimitiveValues(ColorRGBA color)
        {
            _red   = new CssPrimitiveColorValue(color.R, false);
            _green = new CssPrimitiveColorValue(color.G, false);
            _blue  = new CssPrimitiveColorValue(color.B, false);

            if (color.A >= 0)
            {
                _alpha = new CssPrimitiveColorValue(color.A, false);
            }
        }

        private void SetPrimitiveValues(ColorRGBA color, string alpha)
        {
            SetPrimitiveValues(color);

            if (!string.IsNullOrWhiteSpace(alpha))
            {
                _alpha = new CssPrimitiveColorValue(alpha, false);
            }
        }

        #endregion

        #region ColorRGBA Structure

        private struct ColorRGBA
        {
            public int R;
            public int G;
            public int B;
            public int A;

            public ColorRGBA(int red, int green, int blue)
            {
                this.R = red;
                this.G = green;
                this.B = blue;
                this.A = -1;
            }

            public ColorRGBA(int red, int green, int blue, int alpha)
            {
                this.R = red;
                this.G = green;
                this.B = blue;
                this.A = alpha;
            }

            public bool HasAlpha
            {
                get {
                    return (this.A >= 0);
                }
            }

            public override string ToString()
            {
                // using the Windows format
                if (this.A < 0)
                {
                    return string.Format("Color [A=255, R={0}, G={1}, B={2}]", this.R, this.G, this.B);
                }
                return string.Format("Color [A={0}, R={1}, G={2}, B={3}]", this.A, this.R, this.G, this.B);
            }

            public static ColorRGBA FromHex(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return new ColorRGBA();
                }

                string color = value.Trim();
                if (value.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                {
                    color = value.Substring(1);
                }
                else if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    color = value.Substring(2);
                }

                int a = -1;
                int r = 0;
                int g = 0;
                int b = 0;

                switch (color.Length)
                {
                    case 3:
                        var rgbColor = string.Format("{0}{0}{1}{1}{2}{2}", color[0], color[1], color[2]);
                        r = int.Parse(rgbColor.Substring(0, 2), NumberStyles.HexNumber);
                        g = int.Parse(rgbColor.Substring(2, 2), NumberStyles.HexNumber);
                        b = int.Parse(rgbColor.Substring(4, 2), NumberStyles.HexNumber);
                        break;
                    case 4:
                        var argbColor = string.Format("{0}{0}{1}{1}{2}{2}{3}{3}", color[0], color[1], color[2], color[3]);
                        r = int.Parse(argbColor.Substring(0, 2), NumberStyles.HexNumber);
                        g = int.Parse(argbColor.Substring(2, 2), NumberStyles.HexNumber);
                        b = int.Parse(argbColor.Substring(4, 2), NumberStyles.HexNumber);
                        a = int.Parse(argbColor.Substring(6, 2), NumberStyles.HexNumber);
                        break;
                    case 6:
                        r = int.Parse(color.Substring(0, 2), NumberStyles.HexNumber);
                        g = int.Parse(color.Substring(2, 2), NumberStyles.HexNumber);
                        b = int.Parse(color.Substring(4, 2), NumberStyles.HexNumber);
                        break;
                    case 8:
                        r = int.Parse(color.Substring(0, 2), NumberStyles.HexNumber);
                        g = int.Parse(color.Substring(2, 2), NumberStyles.HexNumber);
                        b = int.Parse(color.Substring(4, 2), NumberStyles.HexNumber);
                        a = int.Parse(color.Substring(6, 2), NumberStyles.HexNumber);
                        break;
                }

                return new ColorRGBA(r, g, b, a);
            }

            public static ColorRGBA FromHsl(string hue, string saturation, string lightness, string alpha = null)
            {
                hue        = hue.ToLower();
                saturation = saturation.TrimEnd('%');
                lightness  = lightness.TrimEnd('%');

                string hueUnit = "";
                if (hue.EndsWith("deg", StringComparison.OrdinalIgnoreCase))
                {
                    hueUnit = "deg";
                    hue = hue.Replace("deg", "");
                }
                else if (hue.EndsWith("rad", StringComparison.OrdinalIgnoreCase))
                {
                    hueUnit = "rad";
                    hue = hue.Replace("rad", "");
                }
                else if (hue.EndsWith("turn", StringComparison.OrdinalIgnoreCase))
                {
                    hueUnit = "turn";
                    hue = hue.Replace("turn", "");
                }
                else if (hue.EndsWith("grad", StringComparison.OrdinalIgnoreCase))
                {
                    hueUnit = "grad";
                    hue = hue.Replace("grad", "");
                }
                else if (hue.EndsWith("gon", StringComparison.OrdinalIgnoreCase))
                {
                    hueUnit = "grad";
                    hue = hue.Replace("gon", "");
                }
                else if (hue.EndsWith("g", StringComparison.OrdinalIgnoreCase))
                {
                    hueUnit = "grad";
                    hue = hue.Replace("g", "");
                }

                // Get the HSL values in a range from 0 to 1.
                double h = 0;
                if (string.IsNullOrWhiteSpace(hueUnit) || string.Equals(hueUnit, "deg", StringComparison.Ordinal))  // Degrees
                {
                    h = double.Parse(hue) / 360.0;
                }
                else if (string.Equals(hueUnit, "rad", StringComparison.Ordinal))  // Radian
                {
                    h = Math.Round(double.Parse(hue) * 180 / Math.PI, 2) / 360.0;
                }
                else if (string.Equals(hueUnit, "grad", StringComparison.Ordinal) ||
                    string.Equals(hueUnit, "gon", StringComparison.Ordinal) ||
                    string.Equals(hueUnit, "g", StringComparison.Ordinal))  // Gradian
                {
                    h = Math.Round(double.Parse(hue) * 180 / 200, 2) / 360.0;
                }
                else if (string.Equals(hueUnit, "turn", StringComparison.Ordinal))  // Turns
                {
                    h = double.Parse(hue);
                }

                double s = double.Parse(saturation) / 100.0;
                double l = double.Parse(lightness) / 100.0;

                int a = -1;
                if (!string.IsNullOrWhiteSpace(alpha))
                {
                    double alphaValue = -1;
                    if (alpha.EndsWith("%", StringComparison.OrdinalIgnoreCase))
                    {
                        alphaValue = double.Parse(alpha.TrimEnd('%')) / 100.0;
                    }
                    else
                    {
                        alphaValue = double.Parse(alpha);
                    }
                    if (alphaValue >= 0 && alphaValue <= 1)
                    {
                        a = Convert.ToInt16(alphaValue * 255);
                    }
                }

                // Convert the HSL color to an RGB color
                return ColorRGBA.FromHsl(h, s, l, a);
            }

            /// <summary>
            /// Converts HSL color (values from 0 to 1) to RGB color.
            /// Based on codes from: https://geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
            /// </summary>
            /// <param name="h">hue</param>
            /// <param name="s">saturation</param>
            /// <param name="l">lightness</param>
            /// <param name="a">alpha</param>
            /// <returns></returns>
            public static ColorRGBA FromHsl(double h, double s, double l, int a = -1)
            {
                double r = l;   // default to gray
                double g = l;
                double b = l;
                double v = (l <= 0.5) ? (l * (1.0 + s)) : (l + s - l * s);
                if (v > 0)
                {
                    double m;
                    double sv;
                    int sextant;
                    double fract, vsf, mid1, mid2;

                    m = l + l - v;
                    sv = (v - m) / v;
                    h *= 6.0;
                    sextant = (int)h;
                    fract = h - sextant;
                    vsf = v * sv * fract;
                    mid1 = m + vsf;
                    mid2 = v - vsf;
                    switch (sextant)
                    {
                        case 0:
                            r = v;
                            g = mid1;
                            b = m;
                            break;
                        case 1:
                            r = mid2;
                            g = v;
                            b = m;
                            break;
                        case 2:
                            r = m;
                            g = v;
                            b = mid1;
                            break;
                        case 3:
                            r = m;
                            g = mid2;
                            b = v;
                            break;
                        case 4:
                            r = mid1;
                            g = m;
                            b = v;
                            break;
                        case 5:
                            r = v;
                            g = m;
                            b = mid2;
                            break;
                    }
                }

                return new ColorRGBA(Convert.ToInt16(r * 255.0), Convert.ToInt16(g * 255.0), Convert.ToInt16(b * 255.0), a);
            }
        }

        #endregion
    }
}
