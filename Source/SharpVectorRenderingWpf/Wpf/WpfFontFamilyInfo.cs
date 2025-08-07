using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public class WpfFontFamilyInfo
    {
        public readonly static WpfFontFamilyInfo Empty = new WpfFontFamilyInfo(null,
            FontWeights.Normal, FontStyles.Normal, FontStretches.Normal);

        private string _fontName;
        private string _fontVariant;
        private SvgFontElement _fontElement;

        private FontStyle _style;
        private FontFamily _family;
        private FontWeight _weight;
        private FontStretch _stretch;

        private WpfFontFamilyType _familyType;

        public WpfFontFamilyInfo(FontFamily family, FontWeight weight,
            FontStyle style, FontStretch stretch)
        {
            _familyType = WpfFontFamilyType.System;
            _family     = family;
            _weight     = weight;
            _style      = style;
            _stretch    = stretch;

            _stretch = DecomposeFontName();
        }

        public WpfFontFamilyInfo(WpfFontFamilyType familyType, FontFamily family, FontWeight weight,
            FontStyle style, FontStretch stretch)
        {
            _familyType = familyType;
            _family     = family;
            _weight     = weight;
            _style      = style;
            _stretch    = stretch;

            _stretch = DecomposeFontName();
        }

        public WpfFontFamilyInfo(WpfFontFamilyType familyType, string fontName, 
            FontFamily family, FontWeight weight, FontStyle style, FontStretch stretch)
        {
            _fontName   = fontName;
            _familyType = familyType;
            _family     = family;
            _weight     = weight;
            _style      = style;
            _stretch    = stretch;

            _stretch = DecomposeFontName();
        }

        public WpfFontFamilyInfo(string fontName, SvgFontElement fontElement, 
            FontWeight weight, FontStyle style, FontStretch stretch)
        {
            _fontName    = fontName;
            _fontElement = fontElement;
            _familyType  = WpfFontFamilyType.Svg;
            _family      = null;
            _weight      = weight;
            _style       = style;
            _stretch     = stretch;

            _stretch = DecomposeFontName();
        }

        public bool IsEmpty
        {
            get { return (_family == null); }
        }

        public WpfFontFamilyType FontFamilyType
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
                    return _family.Source;
                }
                return string.Empty;
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
            get {
                return _family;
            }
            internal set {
                _family = value;
            }
        }

        public FontWeight Weight
        {
            get { return _weight; }
        }

        public FontStyle Style
        {
            get { return _style; }
        }

        public FontStretch Stretch
        {
            get { return _stretch; }
        }

        private FontStretch DecomposeFontName()
        {
            FontStretch stretch = _stretch;
            if (stretch != FontStretches.Normal || string.IsNullOrWhiteSpace(_fontName))
            {
                return stretch; //TODO
            }
            string[] parts = _fontName.Split(' ');
            if (parts.Length < 2)
            {
                return stretch;
            }

            string familyPart = parts[0];
            string stylePart = string.Join(" ", parts.Skip(1));

            // Now try to map the stylePart to FontStretch, FontWeight, and FontStyle enums.
            if (stylePart.IndexOf("Narrow", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                stretch = FontStretches.Condensed;
            }
            else if (stylePart.IndexOf("SemiCondensed", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                stretch = FontStretches.SemiCondensed;
            }
            else if (stylePart.IndexOf("ExtraCondensed", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                stretch = FontStretches.ExtraCondensed;
            }
            else if (stylePart.IndexOf("Expanded", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                stretch = FontStretches.Expanded;
            }
            // ... add more mappings for other stretch values as needed.

            // Note: The `stylePart` can also contain weight and style.
            // A more advanced parser would check for "Bold", "Italic", etc., and override the passed-in style/weight.

            return stretch;
        }
    }
}
