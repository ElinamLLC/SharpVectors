using System;
using System.Drawing;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    public class GdiFontFamilyInfo
    {
        public readonly static GdiFontFamilyInfo Empty = new GdiFontFamilyInfo(null,
            GdiFontWeights.Normal, GdiFontStyles.Normal, GdiFontStretches.Normal);

        private string _fontName;
        private string _fontVariant;
        private SvgFontElement _fontElement;

        private FontFamily _family;
        private GdiFontStyles _style;
        private GdiFontWeights _weight;
        private GdiFontStretches _stretch;

        private GdiFontFamilyType _familyType;

        public GdiFontFamilyInfo(FontFamily family, GdiFontWeights weight,
            GdiFontStyles style, GdiFontStretches stretch)
        {
            _familyType = GdiFontFamilyType.System;
            _family     = family;
            _weight     = weight;
            _style      = style;
            _stretch    = stretch;
        }

        public GdiFontFamilyInfo(GdiFontFamilyType familyType, FontFamily family, GdiFontWeights weight,
            GdiFontStyles style, GdiFontStretches stretch)
        {
            _familyType = familyType;
            _family     = family;
            _weight     = weight;
            _style      = style;
            _stretch    = stretch;
        }

        public GdiFontFamilyInfo(GdiFontFamilyType familyType, string fontName, 
            FontFamily family, GdiFontWeights weight, GdiFontStyles style, GdiFontStretches stretch)
        {
            _fontName   = fontName;
            _familyType = familyType;
            _family     = family;
            _weight     = weight;
            _style      = style;
            _stretch    = stretch;
        }

        public GdiFontFamilyInfo(string fontName, SvgFontElement fontElement,
            GdiFontWeights weight, GdiFontStyles style, GdiFontStretches stretch)
        {
            _fontName    = fontName;
            _fontElement = fontElement;
            _familyType  = GdiFontFamilyType.Svg;
            _family      = null;
            _weight      = weight;
            _style       = style;
            _stretch     = stretch;
        }

        public bool IsEmpty
        {
            get { return (_family == null); }
        }

        public GdiFontFamilyType FontFamilyType
        {
            get {
                return _familyType;
            }
        }

        public string Name
        {
            get {
                if (!string.IsNullOrWhiteSpace(_fontName))
                {
                    return _fontName;
                }
                if (_family != null)
                {
                    return _family.Name;
                }
                return "";
            }
        }

        public string Variant
        {
            get {
                return _fontVariant;
            }
            set {
                _fontVariant = value;
            }
        }

        public SvgFontElement FontElement
        {
            get {
                return _fontElement;
            }
        }

        public FontFamily Family
        {
            get { return _family; }
        }

        public GdiFontWeights Weight
        {
            get { return _weight; }
        }

        public GdiFontStyles Style
        {
            get { return _style; }
        }

        public GdiFontStretches Stretch
        {
            get { return _stretch; }
        }
    }

    /// <summary>
    /// Provides a set of static predefined GdiFontStyles values.
    /// </summary>
    public enum GdiFontStyles
    {
        /// <summary>
        /// Specifies a normal GdiFontStyles.
        /// </summary>
        Normal,
        /// <summary>
        /// Specifies an oblique GdiFontStyles.
        /// </summary>
        Oblique,
        /// <summary>
        /// Specifies an italic GdiFontStyles.
        /// </summary>
        Italic
    }

    /// <summary>
    /// Provides a set of static predefined FontWeight values.
    /// </summary>
    public enum GdiFontWeights
    {
        /// <summary>
        /// Specifies a "Thin" font weight.
        /// </summary>
        Thin,
        /// <summary>
        /// Specifies an "Extra-light" font weight.
        /// </summary>
        ExtraLight,
        /// <summary>
        /// Specifies an "Ultra-light" font weight.
        /// </summary>
        UltraLight,
        /// <summary>
        /// Specifies a "Light" font weight.
        /// </summary>
        Light,
        /// <summary>
        /// Specifies a "Normal" font weight.
        /// </summary>
        Normal,
        /// <summary>
        /// Specifies a "Regular" font weight.
        /// </summary>
        Regular,
        /// <summary>
        /// Specifies a "Medium" font weight.
        /// </summary>
        Medium,
        /// <summary>
        /// Specifies a "Demi-bold" font weight.
        /// </summary>
        DemiBold,
        /// <summary>
        /// Specifies a "Semi-bold" font weight.
        /// </summary>
        SemiBold,
        /// <summary>
        /// Specifies a "Bold" font weight.
        /// </summary>
        Bold,
        /// <summary>
        /// Specifies an "Extra-bold" font weight.
        /// </summary>
        ExtraBold,
        /// <summary>
        /// Specifies an "Ultra-bold" font weight.
        /// </summary>
        UltraBold,
        /// <summary>
        /// Specifies a "Black" font weight.
        /// </summary>
        Black,
        /// <summary>
        /// Specifies a "Heavy" font weight.
        /// </summary>
        Heavy,
        /// <summary>
        /// Specifies an "Extra-black" font weight.
        /// </summary>
        ExtraBlack,
        /// <summary>
        /// Specifies an "Ultra-black" font weight.
        /// </summary>
        UltraBlack
    }

    /// <summary>
    /// Provides a set of static predefined GdiFontStretches values.
    /// </summary>
    public enum GdiFontStretches
    {
        /// <summary>
        /// Specifies an ultra-condensed GdiFontStretches.
        /// </summary>
        UltraCondensed,
        /// <summary>
        /// Specifies an extra-condensed GdiFontStretches.
        /// </summary>
        ExtraCondensed,
        /// <summary>
        /// Specifies a condensed GdiFontStretches.
        /// </summary>
        Condensed,
        /// <summary>
        /// Specifies a semi-condensed GdiFontStretches.
        /// </summary>
        SemiCondensed,
        /// <summary>
        /// Specifies a normal GdiFontStretches.
        /// </summary>
        Normal,
        /// <summary>
        /// Specifies a medium GdiFontStretches.
        /// </summary>
        Medium,
        /// <summary>
        /// Specifies a semi-expanded GdiFontStretches.
        /// </summary>
        SemiExpanded,
        /// <summary>
        /// Specifies an expanded GdiFontStretches.
        /// </summary>
        Expanded,
        /// <summary>
        /// Specifies an extra-expanded GdiFontStretches.
        /// </summary>
        ExtraExpanded,
        /// <summary>
        /// Specifies an ultra-expanded GdiFontStretches.
        /// </summary>
        UltraExpanded,        
        /// <summary>
        /// 
        /// </summary>
        Custom
    }
}
