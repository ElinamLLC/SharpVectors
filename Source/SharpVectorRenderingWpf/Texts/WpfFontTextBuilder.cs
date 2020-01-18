using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Renderers.Texts
{
    public sealed class WpfFontTextBuilder : WpfTextBuilder
    {
        #region Private Fields

        private double _textWidth;
        private Typeface _typeface;

        #endregion Private Fields

        #region Constructors and Destructor

        public WpfFontTextBuilder(CultureInfo culture, double fontSize)
            : this(WpfDrawingSettings.DefaultFontFamily, culture, fontSize)
        {
            _textWidth = 0;
        }

        public WpfFontTextBuilder(WpfFontFamilyInfo familyInfo, CultureInfo culture, double fontSize)
            : base(culture, familyInfo.Name, fontSize, null)
        {
            _typeface = new Typeface(familyInfo.Family, familyInfo.Style, familyInfo.Weight, familyInfo.Stretch);
        }

        public WpfFontTextBuilder(FontFamily fontFamily, CultureInfo culture, double fontSize)
            : this(fontFamily, FontStyles.Normal, FontWeights.Normal, culture, fontSize)
        {
        }

        public WpfFontTextBuilder(FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight,
            CultureInfo culture, double fontSize)
            : base(culture, fontFamily.Source, fontSize, null)
        {
            if (fontFamily == null)
            {
                fontFamily = WpfDrawingSettings.DefaultFontFamily;
            }

            _typeface = new Typeface(fontFamily, fontStyle, fontWeight, FontStretches.Normal);
        }

        public WpfFontTextBuilder(CultureInfo culture, string fontName,
            double fontSize, Uri fontUri = null) : base(culture, fontName, fontSize, fontUri)
        {
            _typeface = new Typeface(fontName);
        }

        #endregion

        #region Public Properties

        public override WpfFontFamilyType FontFamilyType { get => throw new NotImplementedException(); }
        public override double Ascent { get => throw new NotImplementedException(); }
        public override bool IsBoldSimulated { get => throw new NotImplementedException(); }
        public override bool IsObliqueSimulated { get => throw new NotImplementedException(); }
        public override double StrikethroughPosition { get => throw new NotImplementedException(); }
        public override double StrikethroughThickness { get => throw new NotImplementedException(); }
        public override double UnderlinePosition { get => throw new NotImplementedException(); }
        public override double UnderlineThickness { get => throw new NotImplementedException(); }
        public override double OverlinePosition { get => throw new NotImplementedException(); }
        public override double OverlineThickness { get => throw new NotImplementedException(); }
        public override double XHeight { get => throw new NotImplementedException(); }
        public override double Alphabetic { get => throw new NotImplementedException(); }

        public override double Width
        {
            get {
                return _textWidth;
            }
        }

        #endregion

        #region Public Methods

        public override Geometry Build(SvgTextContentElement element, string text, double x, double y)
        {
            throw new NotImplementedException();
        }

        public override IList<Rect> MeasureChars(SvgTextContentElement element, string text, bool canBeWhitespace = true)
        {
            throw new NotImplementedException();
        }

        public override Size MeasureText(SvgTextContentElement element, string text, bool canBeWhitespace = true)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
