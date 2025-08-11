using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfFontFamilyInfo
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

            string[] parts = this.Name.Split(' ');
            if (parts.Length >= 2)
            {
                string stylePart = string.Join(" ", parts.Skip(1));
                _style = UpdateFontStyle(stylePart);
                _weight = UpdateFontWeight(stylePart);
                _stretch = UpdateFontStretch(stylePart);
            }
        }

        public WpfFontFamilyInfo(WpfFontFamilyType familyType, FontFamily family, FontWeight weight,
            FontStyle style, FontStretch stretch)
        {
            _familyType = familyType;
            _family     = family;
            _weight     = weight;
            _style      = style;
            _stretch    = stretch;

            string[] parts = this.Name.Split(' ');
            if (parts.Length >= 2)
            {
                string stylePart = string.Join(" ", parts.Skip(1));
                _style = UpdateFontStyle(stylePart);
                _weight = UpdateFontWeight(stylePart);
                _stretch = UpdateFontStretch(stylePart);
            }
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

            string[] parts = this.Name.Split(' ');
            if (parts.Length >= 2)
            {
                string stylePart = string.Join(" ", parts.Skip(1));
                _style = UpdateFontStyle(stylePart);
                _weight = UpdateFontWeight(stylePart);
                _stretch = UpdateFontStretch(stylePart);
            }
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

            string[] parts = this.Name.Split(' ');
            if (parts.Length >= 2)
            {
                string stylePart = string.Join(" ", parts.Skip(1));
                _style = UpdateFontStyle(stylePart);
                _weight = UpdateFontWeight(stylePart);
                _stretch = UpdateFontStretch(stylePart);
            }
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

        private FontStretch UpdateFontStretch(string stylePart)
        {
            FontStretch stretch = _stretch;
            if (stretch != FontStretches.Normal || string.IsNullOrWhiteSpace(_fontName))
            {
                return stretch;
            }

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

            return stretch;
        }

        private FontWeight UpdateFontWeight(string stylePart)
        {
            FontWeight weight = _weight;
            if (weight != FontWeights.Normal || string.IsNullOrWhiteSpace(_fontName))
            {
                return weight;
            }

            // Now try to map the stylePart to FontStretch, FontWeight, and FontStyle enums.
            if (stylePart.IndexOf("Thin", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                weight = FontWeights.Thin;
            }
            else if (stylePart.IndexOf("ExtraLight", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                weight = FontWeights.ExtraLight;
            }
            else if (stylePart.IndexOf("Light", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                weight = FontWeights.Light;
            }
            else if (stylePart.IndexOf("Medium", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                weight = FontWeights.Medium;
            }
            else if (stylePart.IndexOf("Demi-bold", StringComparison.OrdinalIgnoreCase) >= 0
                || stylePart.IndexOf("DemiBold", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                weight = FontWeights.DemiBold;
            }
            else if (stylePart.IndexOf("Semi-bold", StringComparison.OrdinalIgnoreCase) >= 0
                || stylePart.IndexOf("SemiBold", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                weight = FontWeights.SemiBold;
            }
            else if (stylePart.IndexOf("Bold", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                weight = FontWeights.Bold;
            }
            else if (stylePart.IndexOf("Black", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                weight = FontWeights.Black;
            }
            else if (stylePart.IndexOf("Heavy", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                weight = FontWeights.Heavy;
            }
            else if (stylePart.IndexOf("Extra-bold", StringComparison.OrdinalIgnoreCase) >= 0
                || stylePart.IndexOf("ExtraBold", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                weight = FontWeights.ExtraBold;
            }
            else if (stylePart.IndexOf("Ultra-bold", StringComparison.OrdinalIgnoreCase) >= 0
                || stylePart.IndexOf("UltraBold", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                weight = FontWeights.UltraBold;
            }
            else if (stylePart.IndexOf("Extra-black", StringComparison.OrdinalIgnoreCase) >= 0
                || stylePart.IndexOf("ExtraBlack", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                weight = FontWeights.ExtraBlack;
            }
            else if (stylePart.IndexOf("Ultra-black", StringComparison.OrdinalIgnoreCase) >= 0
                || stylePart.IndexOf("UltraBlack", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                weight = FontWeights.UltraBlack;
            }
            // ... add more mappings for other weight values as needed.

            return weight;
        }

        private FontStyle UpdateFontStyle(string stylePart)
        {
            FontStyle style = _style;
            if (style != FontStyles.Normal || string.IsNullOrWhiteSpace(_fontName))
            {
                return style;
            }

            // Now try to map the stylePart to FontStretch, FontWeight, and FontStyle enums.
            if (stylePart.IndexOf("Italic", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                style = FontStyles.Italic;
            }
            else if (stylePart.IndexOf("Oblique", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                style = FontStyles.Oblique;
            }
            // ... add more mappings for other style values as needed.

            return style;
        }
    }
}
