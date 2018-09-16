namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgFontFaceElement interface corresponds to the 'font-face' element. 
    /// </summary>
    public sealed class SvgFontFaceElement : SvgElement, ISvgFontFaceElement
    {
        #region Constructors and Destructor

        public SvgFontFaceElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFontFaceElement Properties

        // attribute name = "alphabetic" <number>
        public float Alphabetic
        {
            get { return this.GetAttribute("alphabetic", 0.0f); }
            set { this.SetAttribute("alphabetic", value); }
        }

        // attribute name = "ascent" <number>
        public float Ascent
        {
            get {
                if (!this.HasAttribute("ascent"))
                {
                    var fontElement = this.ParentNode as SvgFontElement;
                    return (fontElement == null ? 0 : (this.UnitsPerEm - fontElement.VertOriginY));
                }
                return this.GetAttribute("ascent", 0.0f);
            }
            set { this.SetAttribute("ascent", value); }
        }

        // attribute name = "ascent-height" <number>
        public float AscentHeight
        {
            get {
                if (this.HasAttribute("ascent-height"))
                {
                    return this.GetAttribute("ascent-height", 0.0f);
                }
                return this.Ascent;
            }
            set { this.SetAttribute("ascent-height", value); }
        }

        // attribute name = "descent" <number>
        public float Descent
        {
            get {
                if (!this.HasAttribute("descent"))
                {
                    var fontElement = this.ParentNode as SvgFontElement;
                    return (fontElement == null ? 0.0f : fontElement.VertOriginY);
                }
                return this.GetAttribute("descent", 0.0f);
            }
            set { this.SetAttribute("descent", value); }
        }

        /// <summary>
        /// Indicates which font family is to be used to render the text.
        /// </summary>
        // attribute name = "font-family" <string>
        public string FontFamily
        {
            get {
                return this.GetAttribute("font-family");
            }
            set { this.SetAttribute("font-family", value); }
        }

        /// <summary>
        /// Refers to the size of the font from baseline to baseline when multiple 
        /// lines of text are set solid in a multiline layout environment.
        /// </summary>
        // attribute name = "font-size" <string>
        public string FontSize
        {
            get {
                if (!this.HasAttribute("font-size"))
                {
                    return "all";
                }
                return this.GetAttribute("font-size");
            }
            set { this.SetAttribute("font-size", value); }
        }

        /// <summary>
        /// Refers to the style of the font.
        /// </summary>
        // attribute name = "font-style" <string>
        public string FontStyle
        {
            get {
                if (!this.HasAttribute("font-style"))
                {
                    return "all";
                }
                return this.GetAttribute("font-style");
            }
            set { this.SetAttribute("font-style", value); }
        }

        /// <summary>
        /// Refers to the varient of the font.
        /// </summary>
        // attribute name = "font-variant" <string>
        public string FontVariant
        {
            get {
                if (!this.HasAttribute("font-variant"))
                {
                    return "normal";
                }
                return this.GetAttribute("font-variant");
            }
            set { this.SetAttribute("font-variant", value); }
        }

        /// <summary>
        /// Refers to the boldness of the font.
        /// </summary>
        // attribute name = "font-weight" <string>
        public string FontWeight
        {
            get {
                if (!this.HasAttribute("font-weight"))
                {
                    return "all";
                }
                return this.GetAttribute("font-weight");
            }
            set { this.SetAttribute("font-weight", value); }
        }

        // attribute name = "font-stretch" <string>
        public string FontStretch
        {
            get {
                if (!this.HasAttribute("font-stretch"))
                {
                    return "normal";
                }
                return this.GetAttribute("font-stretch");
            }
            set { this.SetAttribute("font-stretch", value); }
        }

        // attribute name = "panose-1" <string>
        public string Panose1
        {
            get { return this.GetAttribute("panose-1"); }
            set { this.SetAttribute("panose-1", value); }
        }

        // attribute name = "unicode-range" <<urange> [, <urange>]*>
        public string UnicodeRange
        {
            get { return this.GetAttribute("unicode-range"); }
            set { this.SetAttribute("unicode-range", value); }
        }

        // attribute name = "units-per-em" <number>
        public float UnitsPerEm
        {
            get {
                return this.GetAttribute("units-per-em", 1000.0f);
            }
            set { this.SetAttribute("units-per-em", value); }
        }

        // attribute name = "x-height" <number>
        public float XHeight
        {
            get { return this.GetAttribute("x-height", float.NaN); }
            set { this.SetAttribute("x-height", value); }
        }

        #endregion
    }
}

