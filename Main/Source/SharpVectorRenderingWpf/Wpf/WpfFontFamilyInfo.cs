using System.Windows;
using System.Windows.Media;

namespace SharpVectors.Renderers.Wpf
{
    public class WpfFontFamilyInfo
    {
        public readonly static WpfFontFamilyInfo Empty = new WpfFontFamilyInfo(null,
            FontWeights.Normal, FontStyles.Normal, FontStretches.Normal);

        private FontFamily  _family;
        private FontWeight  _weight;
        private FontStyle   _style;
        private FontStretch _stretch;

        public WpfFontFamilyInfo(FontFamily family, FontWeight weight, 
            FontStyle style, FontStretch stretch)
        {
            _family  = family;
            _weight  = weight;
            _style   = style;
            _stretch = stretch;
        }

        public bool IsEmpty
        {
            get
            {
                return (_family == null);
            }
        }

        public FontFamily Family
        {
            get { return _family; }
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
    }
}
