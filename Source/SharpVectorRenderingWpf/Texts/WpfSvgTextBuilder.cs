using System;
using System.Linq;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Renderers.Texts
{
    public sealed class WpfSvgTextBuilder : WpfTextBuilder
    {
        private readonly SvgFontElement _font;

        private double _emScale;
        private SvgFontFaceElement _fontFaceElement;
        private SvgMissingGlyphElement _missingGlyph;
        private IDictionary<string, SvgGlyphElement> _glyphs;
        private IDictionary<string, SvgKernElement> _kerning;

        public WpfSvgTextBuilder(SvgFontElement font, double fontSize)
            : base(fontSize)
        {
            _font = font;

            this.Initialize();
        }

        public WpfSvgTextBuilder(SvgFontElement font, string fontName, double fontSize, Uri fontUri = null)
            : base(fontName, fontSize, fontUri)
        {
            _font = font;

            this.Initialize();
        }

        public override WpfFontFamilyType FontFamilyType
        {
            get {
                return WpfFontFamilyType.Svg;
            }
        }

        public override double Ascent
        {
            get {
                if (_fontFaceElement != null)
                {
                    double ascent = _fontFaceElement.Ascent;
                    double baselineOffset = this.FontSizeInPoints * (_emScale / _fontSize) * ascent;
                    return _dpiY / 72f * baselineOffset;
                }
                return 0;
            }
        }

        public override IList<Rect> MeasureChars(string text, bool canBeWhitespace = true)
        {
            if (canBeWhitespace && string.IsNullOrEmpty(text))
            {
                return new List<Rect>();
            }
            if (!canBeWhitespace && string.IsNullOrWhiteSpace(text))
            {
                return new List<Rect>();
            }

            var textBounds = new List<Rect>();
            var path = this.Build(text, textBounds, false);
            return textBounds;
        }

        public override Size MeasureText(string text, bool canBeWhitespace = true)
        {
            if (canBeWhitespace && string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }
            else if (string.IsNullOrWhiteSpace(text))
            {
                return Size.Empty;
            }

            var result = new List<Rect>();
            var path = this.Build(text, result, true);
            var nonEmpty = result.Where(r => r != Rect.Empty);
            if (!nonEmpty.Any())
            {
                return Size.Empty;
            }
            return new Size(nonEmpty.Last().Right - nonEmpty.First().Left, this.Ascent);
        }

        public override PathGeometry Build(string text, double x, double y)
        {
            var textPath = this.Build(text, null, false);
            if (textPath.Figures != null && textPath.Figures.Count > 0)
            {
                textPath.Transform = new TranslateTransform(x, y);
            }

            return textPath;
        }

        private bool Initialize()
        {
            if (_font == null)
            {
                return false;
            }

            if (_fontFaceElement == null)
            {
                _fontFaceElement = _font.FontFace;
            }

            if (_missingGlyph == null)
            {
                _missingGlyph = _font.MissingGlyph;
            }

            if (_fontFaceElement != null)
            {
                if (_emScale <= 0.0d)
                {
                    _emScale = _fontSize / _fontFaceElement.UnitsPerEm;
                }
            }
            else
            {
                return false;
            }

            if (_glyphs == null)
            {
                _glyphs       = new Dictionary<string, SvgGlyphElement>(StringComparer.Ordinal);
                var glyphList = _font.Glyphs;

                if (glyphList != null && glyphList.Count != 0)
                {
                    foreach(var glyph in glyphList)
                    {
                        _glyphs.Add(glyph.Unicode ?? glyph.GlyphName ?? glyph.Id, glyph);
                    }
                }
            }

            if (_kerning == null)
            {
                _kerning     = new Dictionary<string, SvgKernElement>(StringComparer.Ordinal);
                var kernList = _font.Kerning;
                if (kernList != null && kernList.Count != 0)
                {
                    foreach(var kern in kernList)
                    {
                        _kerning.Add(kern.Glyph1 + "|" + kern.Glyph2, kern);
                    }
                }
            }

            return (_fontFaceElement != null && _missingGlyph != null && _glyphs != null && _kerning != null);
        }

        private PathGeometry Build(string text, IList<Rect> textBounds, bool measureSpaces)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new PathGeometry();
            }

            var textPath = new PathGeometry();

            SvgGlyphElement prevGlyph = null;
            double xPos = 0;

            var ascent = this.Ascent;

            for (int i = 0; i < text.Length; i++)
            {
                SvgGlyphElement glyph;
                if (!_glyphs.TryGetValue(text.Substring(i, 1), out glyph))
                {
                    glyph = _missingGlyph;
                }
                SvgKernElement kern;
                if (prevGlyph != null && _kerning.TryGetValue(prevGlyph.GlyphName + "|" + glyph.GlyphName, 
                    out kern))
                {
                    xPos -= kern.Kerning * _emScale;
                }
                PathGeometry glyphPath = new PathGeometry();
                glyphPath.Figures = PathFigureCollection.Parse(glyph.D);

                var groupTransform = new TransformGroup();
                groupTransform.Children.Add(new ScaleTransform(_emScale, -1 * _emScale));
                groupTransform.Children.Add(new TranslateTransform(xPos, ascent));
                glyphPath.Transform = groupTransform;

                if (textBounds != null)
                {
                    Rect bounds = glyphPath.Bounds;
                    if (measureSpaces && bounds == Rect.Empty)
                    {
                        textBounds.Add(new Rect(xPos, 0, glyph.HorizAdvX * _emScale, ascent));
                    }
                    else
                    {
                        textBounds.Add(bounds);
                    }
                }

                if (glyphPath.Figures != null && glyphPath.Figures.Count > 0)
                {
                    textPath.AddGeometry(glyphPath);
                }

                xPos += glyph.HorizAdvX * _emScale;
                prevGlyph = glyph;
            }

            return textPath;
        }
    }
}
