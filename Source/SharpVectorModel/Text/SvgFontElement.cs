using System;
using System.Xml;
using System.Collections.Generic;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgFontElement interface corresponds to the 'font' element. 
    /// </summary>
    public sealed class SvgFontElement : SvgStyleableElement, ISvgFontElement
    {
        #region Private Fields

        private string _fontFamily;
        private SvgTests _svgTests;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgFontElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _svgTests                  = new SvgTests(this);
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);

            doc.RegisterFont(this);
        }

        #endregion

        #region Public Properties

        public string FontFamily
        {
            get {
                if (string.IsNullOrWhiteSpace(_fontFamily))
                {
                    var fontFace = this.FontFace;
                    if (fontFace != null)
                    {
                        _fontFamily = fontFace.FontFamily;

                        // Support cases where font IDs are used instead of font family name...
                        if (string.IsNullOrWhiteSpace(_fontFamily))
                        {
                            _fontFamily = this.Id;
                        }
                    }
                }
                return _fontFamily;
            }
        }

        public SvgFontFaceElement FontFace
        {
            get {
                SvgFontFaceElement fontFace = null;
                if (this.HasChildNodes)
                {
                    foreach (XmlNode child in this.ChildNodes)
                    {
                        if (child.NodeType == XmlNodeType.Element && 
                            string.Equals(child.LocalName, "font-face", StringComparison.OrdinalIgnoreCase))
                        {
                            fontFace = (SvgFontFaceElement)child;
                            break;
                        }
                    }
                }
                if (fontFace != null && string.IsNullOrWhiteSpace(_fontFamily))
                {
                    _fontFamily = fontFace.FontFamily;
                }
                return fontFace;
            }
        } 

        public SvgMissingGlyphElement MissingGlyph
        {
            get {
                if (this.HasChildNodes)
                {
                    foreach (XmlNode child in this.ChildNodes)
                    {
                        if (child.NodeType == XmlNodeType.Element && 
                            string.Equals(child.LocalName, "missing-glyph", StringComparison.OrdinalIgnoreCase))
                        {
                            return (SvgMissingGlyphElement)child;
                        }
                    }
                }
                return null;
            }
        } 

        public IList<SvgGlyphElement> Glyphs
        {
            get {

                List<SvgGlyphElement> glyphList = new List<SvgGlyphElement>();

                if (this.HasChildNodes)
                {
                    foreach (XmlNode child in this.ChildNodes)
                    {
                        if (child.NodeType == XmlNodeType.Element && 
                            string.Equals(child.LocalName, "glyph", StringComparison.OrdinalIgnoreCase))
                        {
                            glyphList.Add((SvgGlyphElement)child);
                        }
                    }
                }
                return glyphList;
            }
        } 

        public IList<SvgKernElement> Kerning
        {
            get {

                List<SvgKernElement> kernList = new List<SvgKernElement>();

                if (this.HasChildNodes)
                {
                    foreach (XmlNode child in this.ChildNodes)
                    {
                        if (child.NodeType == XmlNodeType.Element)
                        {
                            //TODO: Look into the possibility of having both kerning on the same font
                            if (string.Equals(child.LocalName, "hkern", StringComparison.OrdinalIgnoreCase)
                                || string.Equals(child.LocalName, "vkern", StringComparison.OrdinalIgnoreCase))
                            {
                                kernList.Add((SvgKernElement)child);
                            }
                        }
                    }
                }
                return kernList;
            }
        } 

        #endregion

        #region ISvgFontElement Properties

        // attribute name = "horiz-adv-x" <number>
        /// <summary>
        /// Gets or sets the default horizontal advance after rendering a glyph in horizontal orientation.  
        /// </summary>
        /// <remarks>
        /// Glyph widths are required to be non-negative, even if the glyph is typically rendered 
        /// right-to-left, as in Hebrew and Arabic scripts.
        /// </remarks>
        public float HorizAdvX
        {
            get { return this.GetAttribute("horiz-adv-x", 0.0f); }
            set { this.SetAttribute("horiz-adv-x", value); }
        }

        // attribute name = "horiz-origin-x" <number>
        /// <summary>
        /// Gets or sets X-coordinate in the font coordinate system of the origin of a glyph 
        /// to be used when drawing horizontally oriented text
        /// </summary>
        /// <remarks>
        /// <para>Note that the origin applies to all glyphs in the font.</para>
        /// <para>If the attribute is not specified, the effect is as if a value of '0' were specified.</para>
        /// </remarks>
        public float HorizOriginX
        {
            get { return this.GetAttribute("horiz-origin-x", 0.0f); }
            set { this.SetAttribute("horiz-origin-x", value); }
        }

        // attribute name = "horiz-origin-y" <number>
        /// <summary>
        /// Gets or sets the Y-coordinate in the font coordinate system of the origin of a glyph to be 
        /// used when drawing horizontally oriented text. 
        /// </summary>
        /// <remarks>
        /// <para>Note that the origin applies to all glyphs in the font.</para>
        /// <para>If the attribute is not specified, the effect is as if a value of '0' were specified.</para>
        /// </remarks>
        public float HorizOriginY
        {
            get { return this.GetAttribute("horiz-origin-y", 0.0f); }
            set { this.SetAttribute("horiz-origin-y", value); }
        }

        // attribute name = "vert-adv-y" <number>
        /// <summary>
        /// Gets or sets the 
        /// </summary>
        /// <remarks>
        /// <para>
        /// </para>
        /// <para>
        /// </para>
        /// </remarks>
        public float VertAdvY
        {
            get {
                if (this.HasAttribute("vert-adv-y"))
                {
                    return this.GetAttribute("vert-adv-y", 0.0f);
                }
                var fontFace = this["font-face"] as SvgFontFaceElement;
                if (fontFace != null)
                {
                    return fontFace.UnitsPerEm;
                }
                return 0;
            }
            set { this.SetAttribute("vert-adv-y", value); }
        }

        // attribute name = "vert-origin-x" <number>
        /// <summary>
        /// Gets or sets the default X-coordinate in the font coordinate system of the origin of a glyph 
        /// to be used when drawing vertically oriented text. 
        /// </summary>
        /// <remarks>
        /// If the attribute is not specified, the effect is as if the attribute were set to half of the 
        /// effective value of attribute ‘horiz-adv-x’.
        /// </remarks>
        public float VertOriginX
        {
            get {
                if (this.HasAttribute("vert-origin-x"))
                {
                    return this.GetAttribute("vert-origin-x", 0.0f);
                }
                return this.GetAttribute("horiz-adv-x", 0) / 2;
            }
            set { this.SetAttribute("vert-origin-x", value); }
        }

        // attribute name = "vert-origin-y" <number>
        /// <summary>
        /// Gets or sets the default Y-coordinate in the font coordinate system of the origin of a 
        /// glyph to be used when drawing vertically oriented text. 
        /// </summary>
        /// <remarks>
        /// If the attribute is not specified, the effect is as if the attribute were set to the 
        /// position specified by the font's ‘ascent’ attribute.
        /// </remarks>
        public float VertOriginY
        {
            get {
                if (this.HasAttribute("vert-origin-y"))
                {
                    return this.GetAttribute("vert-origin-y", 0.0f);
                }

                var fontFace = this["font-face"] as SvgFontFaceElement;
                if (fontFace != null)
                {
                    if (!fontFace.HasAttribute("ascent"))
                    {
                        return 0;
                    }

                    return fontFace.Ascent;
                }
                return 0;
            }
            set { this.SetAttribute("vert-origin-y", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// This is an extension attribute for externally defined fonts with its unicode-range in 'defs' tag.
        /// The unicode-range value is lost in the current processing method.
        /// </remarks>
        public string UnicodeRange
        {
            get { return this.GetAttribute("unicode-range"); }
            set { this.SetAttribute("unicode-range", value); }
        }

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get {
                return _externalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion

        #region ISvgTests Members

        public ISvgStringList RequiredFeatures
        {
            get { return _svgTests.RequiredFeatures; }
        }

        public ISvgStringList RequiredExtensions
        {
            get { return _svgTests.RequiredExtensions; }
        }

        public ISvgStringList SystemLanguage
        {
            get { return _svgTests.SystemLanguage; }
        }

        public bool HasExtension(string extension)
        {
            return _svgTests.HasExtension(extension);
        }

        #endregion
    }
}

