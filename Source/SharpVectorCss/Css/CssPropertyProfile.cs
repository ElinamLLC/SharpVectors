using System;
using System.Collections.Generic;

namespace SharpVectors.Dom.Css
{
    internal sealed class CssProperty
    {
        public bool     IsInherited;
        public string   InitialValue;
        public CssValue InitialCssValue;

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

        private IDictionary<string, CssProperty> _properties;

        #endregion

        #region Constructors and Destructor

        public CssPropertyProfile()
        {
            _properties = new Dictionary<string, CssProperty>(StringComparer.OrdinalIgnoreCase);
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
            return null;
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
            return null;
        }

        public bool IsInheritable(string propertyName)
        {
            if (_properties.ContainsKey(propertyName))
            {
                return _properties[propertyName].IsInherited;
            }
            return true;
        }

        public void Add(string propertyName, bool isInheritable, string initialValue)
        {
            _properties.Add(propertyName, new CssProperty(isInheritable, initialValue));
        }

        #endregion

        #region Private Methods

        private void InitializeDefaults()
        {
            Add("alignment-baseline", false, string.Empty);
            Add("baseline-shift", false, CssConstants.ValBaseline);
            Add("clip", false, CssConstants.ValAuto);
            Add("clip-path", false, CssConstants.ValNone);
            Add("clip-rule", true, CssConstants.ValNonzero);
            Add(CssConstants.PropColor, true, CssConstants.ValBlack);
            Add("color-interpolation", true, string.Empty);
            Add("color-interpolation-filters", true, CssConstants.ValLinearRgb);
            Add("color-profile", true, CssConstants.ValAuto);
            Add("color-rendering", true, CssConstants.ValAuto);
            Add("cursor", true, CssConstants.ValAuto);
            Add("direction", true, "ltr");
            Add(CssConstants.PropDisplay, false, CssConstants.ValInline);
            Add("dominant-baseline", true, CssConstants.ValAuto);
            Add("enable-background", false, CssConstants.ValAccumulate);
            Add("fill", true, CssConstants.ValBlack);
            Add("fill-opacity", true, SvgConstants.ValOne);
            Add("fill-rule", true, CssConstants.ValNonzero);
            Add("filter", false, CssConstants.ValNone);
            Add("flood-color", false, CssConstants.ValBlack);
            Add("flood-opacity", false, SvgConstants.ValOne);
            Add("font", true, string.Empty);
            Add("font-family", true, "Arial");
            Add("font-size", true, CssConstants.ValMedium);
            Add("font-size-adjust", true, CssConstants.ValNone);
            Add("font-stretch", true, CssConstants.ValNormal);
            Add("font-style", true, CssConstants.ValNormal);
            Add("font-variant", true, CssConstants.ValNormal);
            Add("font-weight", true, CssConstants.ValNormal);
            Add("glyph-orientation-horizontal", true, "0deg");
            Add("glyph-orientation-vertical", true, CssConstants.ValAuto);
            Add("image-rendering", true, CssConstants.ValAuto);
            Add("kerning", true, CssConstants.ValAuto);
            Add("letter-spacing", true, CssConstants.ValNormal);
            Add("lighting-color", false, CssConstants.ValWhite);
            Add("marker", true, CssConstants.ValNone);
            Add("marker-end", true, CssConstants.ValNone);
            Add("marker-mid", true, CssConstants.ValNone);
            Add("marker-start", true, CssConstants.ValNone);
            Add("mask", false, CssConstants.ValNone);
            Add("opacity", false, SvgConstants.ValOne);
            Add("overflow", false, CssConstants.ValVisible);
            Add("pointer-events", true, "visiblePainted");
            Add("shape-rendering", true, CssConstants.ValAuto);
            Add("solid-color", true, CssConstants.ValBlack);
            Add("solid-opacity", true, SvgConstants.ValOne);
            Add("stop-color", false, CssConstants.ValBlack);
            Add("stop-opacity", false, SvgConstants.ValOne);
            Add("stroke", true, CssConstants.ValNone);
            Add("stroke-dasharray", true, CssConstants.ValNone);
            Add("stroke-dashoffset", true, SvgConstants.ValZero);
            Add("stroke-linecap", true, "butt");
            Add("stroke-linejoin", true, "miter");
            Add("stroke-miterlimit", true, "4");
            Add("stroke-opacity", true, SvgConstants.ValOne);
            Add("stroke-width", true, SvgConstants.ValOne);
            Add("text-anchor", true, "start");
            Add("text-decoration", false, CssConstants.ValNone);
            Add("text-rendering", true, CssConstants.ValAuto);
            Add("unicode-bidi", false, CssConstants.ValNormal);
            Add(CssConstants.PropVisibility, true, CssConstants.ValVisible);
            Add("word-spacing", true, CssConstants.ValNormal);
            Add("writing-mode", true, "lr-tb");
            Add("viewport-fill", false, string.Empty);
            Add("viewport-fill-opacity", false, SvgConstants.ValOne);
        }

        #endregion
    }
}
