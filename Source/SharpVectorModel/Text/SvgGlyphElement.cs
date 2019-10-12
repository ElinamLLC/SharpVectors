using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgGlyphElement interface corresponds to the 'glyph' element. 
    /// </summary>
    public class SvgGlyphElement : SvgStyleableElement, ISvgGlyphElement
    {
        #region Private Fields

        private object _tag;
        private SvgPathSegList _pathSegList;

        #endregion

        #region Constructors and Destructor

        public SvgGlyphElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region Public Properties

        public object Tag
        {
            get {
                return _tag;
            }
            set {
                _tag = value;
            }
        }

        #endregion

        #region ISvgGlyphElement Properties

        /// <summary>
        /// Gets a <see cref="ISvgPathSegList"/> of path data.
        /// </summary>
        public SvgPathSegList PathData
        {
            get {
                if (_pathSegList == null)
                {
                    string path = null;
                    if (!this.HasAttribute("d"))
                    {
                        path = this.GetAttribute("d");
                    }
                    if (this.HasChildNodes)
                    {
                        foreach (XmlNode child in this.ChildNodes)
                        {
                            if (child.NodeType == XmlNodeType.Element &&
                                string.Equals(child.LocalName, "path", StringComparison.OrdinalIgnoreCase))
                            {
                                var pathElement = (XmlElement)child;
                                path = pathElement.GetAttribute("d");
                                break;
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        return null;
                    }
                    _pathSegList = new SvgPathSegList(path, true);
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
            get {
                if (this.HasAttribute("d"))
                {
                    return this.GetAttribute("d");
                }
                if (this.HasChildNodes)
                {
                    foreach (XmlNode child in this.ChildNodes)
                    {
                        if (child.NodeType == XmlNodeType.Element &&
                            string.Equals(child.LocalName, "path", StringComparison.OrdinalIgnoreCase))
                        {
                            var pathElement = (XmlElement)child;
                            return pathElement.GetAttribute("d");
                        }
                    }
                }

                return string.Empty;
            }
            set { this.SetAttribute("d", value); }
        }

        // attribute name = "glyph-name" <string>
        public string GlyphName
        {
            get { return this.GetAttribute("glyph-name"); }
            set { this.SetAttribute("glyph-name", value); }
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

        // attribute name = "arabic-form" <string>: "initial | medial | terminal | isolated"
        public string ArabicForm
        {
            get { return this.GetAttribute("arabic-form"); }
            set { this.SetAttribute("arabic-form", value); }
        }

        // attribute name = "lang" <string>: "%LanguageCodes;"
        public string Lang
        {
            get { return this.GetAttribute("lang"); }
            set { this.SetAttribute("lang", value); }
        }

        // attribute name = "orientation" <string>: "h | v"
        public string Orientation
        {
            get { return this.GetAttribute("orientation"); }
            set { this.SetAttribute("orientation", value); }
        }

        #endregion
    }
}

