using System;
using System.Collections.Generic;

namespace SharpVectors.Dom.Css
{
    internal sealed class CssProperty
    {
        internal bool     IsInherited;
        internal string   InitialValue;
        internal CssValue InitialCssValue;

        internal CssProperty(bool isInherited, string initialValue)
        {
            this.IsInherited     = isInherited;
            this.InitialValue    = initialValue;
            this.InitialCssValue = null;
        }
    }

    public sealed class CssPropertyProfile
    {
        #region Private Fields

        private static CssPropertyProfile _svgProfile;

        private Dictionary<string, CssProperty> _properties;

        #endregion

        #region Constructors and Destructor

        public CssPropertyProfile()
        {
            _properties = new Dictionary<string, CssProperty>();
        }

        #endregion

        #region Public Properties

        public int Length
        {
            get
            {
                return _properties.Count;
            }
        }

        public static CssPropertyProfile SvgProfile
        {
            get
            {
                if (_svgProfile == null)
                {
                    _svgProfile = new CssPropertyProfile();

                    _svgProfile.InitializeDefaults();
                }

                return _svgProfile;
            }
        }

        #endregion

        #region Public Methods

        public ICollection<string> GetAllPropertyNames()
        {
            return _properties.Keys;
        }

        public string GetInitialValue(string propertyName)
        {
            if (_properties.ContainsKey(propertyName))
            {
                return _properties[propertyName].InitialValue;
            }
            else
            {
                return null;
            }
        }

        public CssValue GetInitialCssValue(string propertyName)
        {
            if (_properties.ContainsKey(propertyName))
            {
                CssProperty cssProp = _properties[propertyName];
                if (cssProp.InitialCssValue == null)
                {
                    cssProp.InitialCssValue = CssValue.GetCssValue(cssProp.InitialValue, false);
                }
                return cssProp.InitialCssValue;
            }
            else
            {
                return null;
            }
        }

        public bool IsInheritable(string propertyName)
        {
            if (_properties.ContainsKey(propertyName))
            {
                return _properties[propertyName].IsInherited;
            }
            else
            {
                return true;
            }
        }

        public void Add(string propertyName, bool isInheritable, string initialValue)
        {
            _properties.Add(propertyName, new CssProperty(isInheritable, initialValue));
        }

        #endregion

        #region Private Methods

        private void InitializeDefaults()
        {
            Add("alignment-baseline", false, String.Empty);
            Add("baseline-shift", false, "baseline");
            Add("clip", false, "auto");
            Add("clip-path", false, "none");
            Add("clip-rule", true, "nonzero");
            Add("color", true, "black");
            Add("color-interpolation", true, String.Empty);
            Add("color-interpolation-filters", true, "linearRGB");
            Add("color-profile", true, "auto");
            Add("color-rendering", true, "auto");
            Add("cursor", true, "auto");
            Add("direction", true, "ltr");
            Add("display", false, "inline");
            Add("dominant-baseline", false, "auto");
            Add("enable-background", false, "accumulate");
            Add("fill", true, "black");
            Add("fill-opacity", true, "1");
            Add("fill-rule", true, "nonzero");
            Add("filter", false, "none");
            Add("flood-color", false, "black");
            Add("flood-opacity", false, "1");
            Add("font", true, String.Empty);
            Add("font-family", true, "Arial");
            Add("font-size", true, "medium");
            Add("font-size-adjust", true, "none");
            Add("font-stretch", true, "normal");
            Add("font-style", true, "normal");
            Add("font-variant", true, "normal");
            Add("font-weight", true, "normal");
            Add("glyph-orientation-horizontal", true, "0deg");
            Add("glyph-orientation-vertical", true, "auto");
            Add("image-rendering", true, "auto");
            Add("kerning", true, "auto");
            Add("letter-spacing", true, "normal");
            Add("lighting-color", false, "white");
            Add("marker", true, "none");
            Add("marker-end", true, "none");
            Add("marker-mid", true, "none");
            Add("marker-start", true, "none");
            Add("mask", false, "none");
            Add("opacity", false, "1");
            Add("overflow", false, "visible");
            Add("pointer-events", true, "visiblePainted");
            Add("shape-rendering", true, "auto");
            Add("stop-color", true, "black");
            Add("stop-opacity", true, "1");
            Add("stroke", true, "none");
            Add("stroke-dasharray", true, "none");
            Add("stroke-dashoffset", true, "0");
            Add("stroke-linecap", true, "butt");
            Add("stroke-linejoin", true, "miter");
            Add("stroke-miterlimit", true, "4");
            Add("stroke-opacity", true, "1");
            Add("stroke-width", true, "1");
            Add("text-anchor", true, "start");
            Add("text-decoration", false, "none");
            Add("text-rendering", true, "auto");
            Add("unicode-bidi", false, "normal");
            Add("visibility", true, "visible");
            Add("word-spacing", true, "normal");
            Add("writing-mode", true, "lr-tb");
        }

        #endregion
    }
}
