namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgMissingGlyphElement interface corresponds to the 'missing-glyph' element. 
    /// </summary>
    public sealed class SvgMissingGlyphElement : SvgStyleableElement, ISvgMissingGlyphElement
    {
        #region Private Fields

        private SvgPathSegList _pathSegList;

        #endregion

        #region Constructors and Destructor

        public SvgMissingGlyphElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgGlyphElement Properties

        /// <summary>
        /// Gets a <see cref="ISvgPathSegList"/> of path data.
        /// </summary>
        public SvgPathSegList PathData
        {
            get {
                if (!this.HasAttribute("d"))
                {
                    return null;
                }
                if (_pathSegList == null)
                {
                    _pathSegList = new SvgPathSegList(this.GetAttribute("d"), true);
                }
                return _pathSegList;
            }
        }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        // attribute name = "d" <string>
        public string D
        {
            get { return this.GetAttribute("d"); }
            set { this.SetAttribute("d", value); }
        }

        // attribute name = "horiz-adv-x" <number>
        public float HorizAdvX
        {
            get {
                if (this.HasAttribute("horiz-adv-x"))
                {
                    return this.GetAttribute("horiz-adv-x", 0.0f);
                }
                var fontElement = this.ParentNode as SvgFontElement;
                return (fontElement == null ? 0.0f : fontElement.HorizAdvX);
            }
            set { this.SetAttribute("horiz-adv-x", value); }
        }

        // attribute name = "unicode" <string>
        public string Unicode
        {
            get { return this.GetAttribute("unicode"); }
            set { this.SetAttribute("unicode", value); }
        }

        // attribute name = "vert-adv-y" <number>
        public float VertAdvY
        {
            get {
                if (this.HasAttribute("vert-adv-y"))
                {
                    return this.GetAttribute("vert-adv-y", 0.0f);
                }
                var fontElement = this.ParentNode as SvgFontElement;
                return (fontElement == null ? 0.0f : fontElement.VertAdvY);
            }
            set { this.SetAttribute("vert-adv-y", value); }
        }

        // attribute name = "vert-origin-x" <number>
        public float VertOriginX
        {
            get {
                if (this.HasAttribute("vert-origin-x"))
                {
                    return this.GetAttribute("vert-origin-x", 0.0f);
                }
                var fontElement = this.ParentNode as SvgFontElement;
                return (fontElement == null ? 0.0f : fontElement.VertOriginX);
            }
            set { this.SetAttribute("vert-origin-x", value); }
        }

        // attribute name = "vert-origin-y" <number>
        public float VertOriginY
        {
            get {
                if (this.HasAttribute("vert-origin-y"))
                {
                    return this.GetAttribute("vert-origin-y", 0.0f);
                }
                var fontElement = this.ParentNode as SvgFontElement;
                return (fontElement == null ? 0.0f : fontElement.VertOriginY);
            }
            set { this.SetAttribute("vert-origin-y", value); }
        }

        #endregion
    }
}

