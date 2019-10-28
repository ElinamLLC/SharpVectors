using System;

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

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <remarks>attribute name = "alphabetic" {number}</remarks>
        public float Alphabetic
        {
            get { return this.GetAttribute("alphabetic", 0.0f); }
            set { this.SetAttribute("alphabetic", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <remarks>attribute name = "ascent" {number}</remarks>
        public float Ascent
        {
            get {
                if (!this.HasAttribute("ascent"))
                {
                    var fontElement = this.ParentNode as SvgFontElement;
                    return (fontElement == null ? 0 : (this.UnitsPerEm - fontElement.VertOriginY));
                }
                return this.GetAttribute("ascent", this.UnitsPerEm * 0.8f);
            }
            set { this.SetAttribute("ascent", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <remarks>attribute name = "ascent-height" {number}</remarks>
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

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <remarks>attribute name = "descent" {number}</remarks>
        public float Descent
        {
            get {
                if (!this.HasAttribute("descent"))
                {
                    var fontElement = this.ParentNode as SvgFontElement;
                    return (fontElement == null ? 0.0f : fontElement.VertOriginY);
                }
                return this.GetAttribute("descent", this.UnitsPerEm * 0.2f);
            }
            set { this.SetAttribute("descent", value); }
        }

        /// <summary>
        /// Indicates which font family is to be used to render the text.
        /// </summary>
        /// <value></value>
        /// <remarks>attribute name = "font-family" {string}</remarks>
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
        /// <value></value>
        /// <remarks>attribute name = "font-size" {string}</remarks>
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
        /// <value></value>
        /// <remarks>attribute name = "font-style" {string}</remarks>
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
        /// <value></value>
        /// <remarks>attribute name = "font-variant" {string}</remarks>
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
        /// <value></value>
        /// <remarks>attribute name = "font-weight" {string}</remarks>
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

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <remarks>attribute name = "font-stretch" {string}</remarks>
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

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <remarks>attribute name = "panose-1" {string}</remarks>
        public string Panose1
        {
            get { return this.GetAttribute("panose-1", SvgConstants.DefFontFacePanose_1); }
            set { this.SetAttribute("panose-1", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <remarks>attribute name = "unicode-range" {{urange} [, {urange}]*></remarks>
        public string UnicodeRange
        {
            get { return this.GetAttribute("unicode-range"); }
            set { this.SetAttribute("unicode-range", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <remarks>attribute name = "units-per-em" {number}</remarks>
        public float UnitsPerEm
        {
            get {
                return this.GetAttribute("units-per-em", Convert.ToSingle(SvgConstants.DefFontFaceUnitsPerEm));
            }
            set { this.SetAttribute("units-per-em", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <remarks>attribute name = "x-height" {number}</remarks>
        public float XHeight
        {
            get { return this.GetAttribute("x-height", float.NaN); }
            set { this.SetAttribute("x-height", value); }
        }

        /// <summary>
        /// Gets the distance from the baseline to the top of an English lowercase letter for a typeface. 
        /// The distance excludes ascenders.
        /// </summary>
        /// <value>
        /// A <see cref="float"/> that indicates the distance from the baseline to the top of an
        /// English lowercase letter (excluding ascenders), expressed as a fraction of the font em size.
        /// </value>
        /// <remarks>attribute name = "cap-height" {number}</remarks>
        public float CapHeight
        {
            get { return this.GetAttribute("cap-height", float.NaN); }
            set { this.SetAttribute("cap-height", value); }
        }

        /// <summary>
        /// Gets a value that indicates the distance from the baseline to the strikethrough for the typeface.
        /// </summary>
        /// <value>A <see cref="float"/> that indicates the strikethrough position, measured from the baseline and expressed 
        /// as a fraction of the font em size.</value>
        /// <remarks>attribute name = "strikethrough-position" {number}</remarks>
        public float StrikethroughPosition
        {
            get { return this.GetAttribute("strikethrough-position", 3 * this.Ascent / 8f); }
            set { this.SetAttribute("strikethrough-position", value); }
        }

        /// <summary>
        /// Gets a value that indicates the thickness of the strikethrough relative to the font em size.
        /// </summary>
        /// <value>A <see cref="float"/> that indicates the strikethrough thickness, expressed as a fraction 
        /// of the font em size.</value>
        /// <remarks>attribute name = "strikethrough-thickness" {number}</remarks>
        public float StrikethroughThickness
        {
            get { return this.GetAttribute("strikethrough-thickness", this.UnitsPerEm / 20f); }
            set { this.SetAttribute("strikethrough-thickness", value); }
        }

        /// <summary>
        /// Gets a value that indicates the distance of the underline from the baseline for the typeface.
        /// </summary>
        /// <value>A <see cref="float"/> that indicates the underline position, measured from the baseline 
        /// and expressed as a fraction of the font em size.</value>
        /// <remarks>attribute name = "underline-position" {number}</remarks>
        public float UnderlinePosition
        {
            get { return this.GetAttribute("underline-position", -3 * this.UnitsPerEm / 40f); }
            set { this.SetAttribute("underline-position", value); }
        }

        /// <summary>
        /// Gets a value that indicates the thickness of the underline relative to the font em size for the typeface.
        /// </summary>
        /// <value>A <see cref="float"/> that indicates the underline thickness, expressed as a fraction 
        /// of the font em size.</value>
        /// <remarks>attribute name = "underline-thickness" {number}</remarks>
        public float UnderlineThickness
        {
            get { return this.GetAttribute("underline-thickness", this.UnitsPerEm / 20f); }
            set { this.SetAttribute("underline-thickness", value); }
        }

        /// <summary>
        /// Gets a value that indicates the distance of the overline from the baseline for the typeface.
        /// </summary>
        /// <value>A <see cref="float"/> that indicates the overline position, measured from the baseline 
        /// and expressed as a fraction of the font em size.</value>
        /// <remarks>attribute name = "overline-position" {number}</remarks>
        public float OverlinePosition
        {
            get { return this.GetAttribute("overline-position", this.Ascent); }
            set { this.SetAttribute("overline-position", value); }
        }

        /// <summary>
        /// Gets a value that indicates the thickness of the overline relative to the font em size for the typeface.
        /// </summary>
        /// <value>A <see cref="float"/> that indicates the overline thickness, expressed as a fraction 
        /// of the font em size.</value>
        /// <remarks>attribute name = "overline-thickness" {number}</remarks>
        public float OverlineThickness
        {
            get { return this.GetAttribute("overline-thickness", this.UnitsPerEm / 20f); }
            set { this.SetAttribute("overline-thickness", value); }
        }

        #endregion
    }
}

