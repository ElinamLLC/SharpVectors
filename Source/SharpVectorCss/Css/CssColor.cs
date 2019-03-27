using System;
using System.Drawing;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// The RGBColor interface is used to represent any RGB color value. 
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

        #endregion

        #region Public Properties

        public string CssText
        {
            get {
                return "rgb(" + ((CssPrimitiveValue)Red).CssText + ","
                    + ((CssPrimitiveValue)Green).CssText + "," + ((CssPrimitiveValue)Blue).CssText + ")";
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

        #endregion

        #region Private Methods

        private void ParseColor(string str)
        {
            str = str.Trim();
            if (str.StartsWith("rgb(", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Substring(4, str.Length - 5);
                string[] parts = str.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

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
                string[] parts = str.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

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
            else
            {
                str = str.ToLower();
                // fix a difference in the GDI+ color table
                if (str.Equals("darkseagreen", StringComparison.OrdinalIgnoreCase)) // for "#8FBC8F"
                {
                    SetPrimitiveValues(143, 188, 143);
                }
                else if (str.Equals("rebeccapurple", StringComparison.OrdinalIgnoreCase)) // for "#663399"
                {
                    SetPrimitiveValues(143, 188, 143); // CSS Color Module Level 4
                }
                else if (str.Equals("transparent", StringComparison.OrdinalIgnoreCase)) // for "transparent"
                {
                    SetPrimitiveValues(0, 0, 0, 0);
                }
                else
                {
                    str = str.Replace("grey", "gray");

                    try
                    {
                        SetPrimitiveValues(ColorTranslator.FromHtml(str));
                    }
                    catch
                    {
                        SetPrimitiveValues(0, 0, 0);
                    }
                }
            }
        }

        private void SetPrimitiveValues(Color color)
        {
            _red   = new CssPrimitiveColorValue(color.R, false);
            _green = new CssPrimitiveColorValue(color.G, false);
            _blue  = new CssPrimitiveColorValue(color.B, false);
            _alpha = new CssPrimitiveColorValue(color.A, false);
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

        #endregion
    }
}
