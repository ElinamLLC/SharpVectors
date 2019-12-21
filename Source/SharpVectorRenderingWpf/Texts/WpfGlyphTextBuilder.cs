//------------------------------------------------------------------------
// Portions based on Glyphs: Copyright (C) Microsoft Corporation 
//------------------------------------------------------------------------

using System;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Renderers.Texts
{
    public sealed class WpfGlyphTextBuilder : WpfTextBuilder
    {
        #region Private Fields

        private const double EmMultiplier = 100.0;

        private ushort[] _glyphIndices;
        private double[] _advanceWidths;
        private Point[] _glyphOffsets;
        private ushort[] _clusterMap;
        private bool _isSideways;
        private int _bidiLevel;
        private GlyphTypeface _glyphTypeface;
        private string _unicodeString;
        private string _deviceFontName;

        private double _textWidth;

        private GlyphRun _glyphRun;

        private Point _glyphRunOrigin = new Point();

        private Typeface _typeface;

        #endregion Private Fields

        #region Constructors and Destructor

        public WpfGlyphTextBuilder(CultureInfo culture, double fontSize)
            : this(WpfDrawingSettings.DefaultFontFamily, culture, fontSize)
        {
        }

        public WpfGlyphTextBuilder(WpfFontFamilyInfo familyInfo, CultureInfo culture, double fontSize)
            : base(culture, familyInfo.Name, fontSize, null)
        {
            _typeface = new Typeface(familyInfo.Family, familyInfo.Style, familyInfo.Weight, familyInfo.Stretch);

            if (!_typeface.TryGetGlyphTypeface(out _glyphTypeface))
            {
                throw new ArgumentException();
            }
        }

        public WpfGlyphTextBuilder(FontFamily fontFamily, CultureInfo culture, double fontSize)
            : this(fontFamily, FontStyles.Normal, FontWeights.Normal, culture, fontSize)
        {
        }

        public WpfGlyphTextBuilder(FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, 
            CultureInfo culture, double fontSize)
            : base(culture, fontFamily.Source, fontSize, null)
        {
            if (fontFamily == null)
            {
                fontFamily = WpfDrawingSettings.DefaultFontFamily;
            }

            _typeface = new Typeface(fontFamily, fontStyle, fontWeight, FontStretches.Normal);

            if (!_typeface.TryGetGlyphTypeface(out _glyphTypeface))
            {
                throw new ArgumentException();
            }
        }

        public WpfGlyphTextBuilder(CultureInfo culture, string fontName,
            double fontSize, Uri fontUri = null) : base(culture, fontName, fontSize, fontUri)
        {
            _typeface = new Typeface(fontName);

            if (!_typeface.TryGetGlyphTypeface(out _glyphTypeface))
            {
                throw new ArgumentException();
            }
        }

        #endregion

        #region Public Properties

        public override WpfFontFamilyType FontFamilyType
        {
            get {
                return WpfFontFamilyType.OpenType;
            }
        }

        /// <summary>
        /// Gets a value that determines whether to simulate a bold weight for the glyphs represented by the typeface.
        /// </summary>
        /// <value>true if bold simulation is used for glyphs; otherwise, false.</value>
        public override bool IsBoldSimulated
        {
            get {
                if (_typeface != null)
                {
                    return _typeface.IsBoldSimulated;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets a value that determines whether to simulate an italic style for the glyphs represented by the typeface.
        /// </summary>
        /// <value>true if italic simulation is used for glyphs; otherwise, false.</value>
        public override bool IsObliqueSimulated
        {
            get {
                if (_typeface != null)
                {
                    return _typeface.IsObliqueSimulated;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets a value that indicates the distance from the baseline to the strikethrough for the typeface.
        /// </summary>
        /// <value>A <see cref="double"/> that indicates the strikethrough position, measured from the baseline and expressed 
        /// as a fraction of the font em size.</value>
        public override double StrikethroughPosition
        {
            get {
                if (_typeface != null)
                {
                    return _typeface.StrikethroughPosition;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets a value that indicates the thickness of the strikethrough relative to the font em size.
        /// </summary>
        /// <value>A <see cref="double"/> that indicates the strikethrough thickness, expressed as a fraction 
        /// of the font em size.</value>
        public override double StrikethroughThickness
        {
            get {
                if (_typeface != null)
                {
                    return _typeface.StrikethroughThickness;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets a value that indicates the distance of the underline from the baseline for the typeface.
        /// </summary>
        /// <value>A <see cref="double"/> that indicates the underline position, measured from the baseline 
        /// and expressed as a fraction of the font em size.</value>
        public override double UnderlinePosition
        {
            get {
                if (_typeface != null)
                {
                    return _typeface.UnderlinePosition;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets a value that indicates the thickness of the underline relative to the font em size for the typeface.
        /// </summary>
        /// <value>A <see cref="double"/> that indicates the underline thickness, expressed as a fraction 
        /// of the font em size.</value>
        public override double UnderlineThickness
        {
            get {
                if (_typeface != null)
                {
                    return _typeface.UnderlineThickness;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets a value that indicates the distance of the overline from the baseline for the typeface.
        /// </summary>
        /// <value>A <see cref="double"/> that indicates the overline position, measured from the baseline 
        /// and expressed as a fraction of the font em size.</value>
        public override double OverlinePosition
        {
            get {
                if (_typeface != null)
                {
                    //return _typeface.OverlinePosition;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets a value that indicates the thickness of the overline relative to the font em size for the typeface.
        /// </summary>
        /// <value>A <see cref="double"/> that indicates the overline thickness, expressed as a fraction 
        /// of the font em size.</value>
        public override double OverlineThickness
        {
            get {
                if (_typeface != null)
                {
                    return _typeface.UnderlineThickness; // Overline and underline: should be the same!
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets the distance from the baseline to the top of an English lowercase letter for a typeface. 
        /// The distance excludes ascenders.
        /// </summary>
        /// <value>
        /// A <see cref="double"/> that indicates the distance from the baseline to the top of an
        /// English lowercase letter (excluding ascenders), expressed as a fraction of the font em size.
        /// </value>
        public override double XHeight
        {
            get {
                if (_typeface != null)
                {
                    return _typeface.XHeight;
                }
                return 0;
            }
        }

        public override double Alphabetic
        {
            get {
                return 0;
            }
        }

        public override double Ascent
        {
            get {
                return _glyphTypeface.Baseline;
            }
        }

        public override double Baseline
        {
            get {
                return _glyphTypeface.Baseline * this.FontSize;
            }
        }

        public bool IsSideways
        {
            get {
                return _isSideways;
            }
            set {
                _isSideways = value;
            }
        }

        public override double Width
        {
            get {
                return _textWidth;
            }
        }

        /// <summary>
        /// Gets or sets the bidirectional nesting level of Glyphs.
        /// </summary>
        /// <value>An <see cref="int"/> value that represents the bidirectional nesting level.</value>
        public int BidiLevel
        {
            get {
                return _bidiLevel;
            }
            set {
                _bidiLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the specific device font for which the Glyphs object has been optimized.
        /// </summary>
        /// <value>A <see cref="string"/> value that represents the name of the device font.</value>
        public string DeviceFontName
        {
            get {
                return _deviceFontName;
            }
            set {
                _deviceFontName = value;
            }
        }

        #endregion

        #region Public Methods

        public override IList<Rect> MeasureChars(SvgTextContentElement element, string text, bool canBeWhitespace = true)
        {
            //TODO: This is not an efficient implementation
            if (canBeWhitespace && string.IsNullOrEmpty(text))
            {
                return new List<Rect>();
            }
            if (!canBeWhitespace && string.IsNullOrWhiteSpace(text))
            {
                return new List<Rect>();
            }

            var textBounds = new List<Rect>();
            //var path = this.Build(element, text, textBounds, false);
            return textBounds;
        }

        public override Size MeasureText(SvgTextContentElement element, string text, bool canBeWhitespace = true)
        {
            ComputeMeasurement(text, 0, 0);

            if (_glyphRun == null)
            {
                return new Size(0, 0);
            }

            Rect designRect = _glyphRun.ComputeAlignmentBox();

            //designRect.Offset(_glyphRunOrigin.X, _glyphRunOrigin.Y);

            _textWidth = Math.Max(0, designRect.Right);

            return new Size(Math.Max(0, designRect.Right), Math.Max(0, designRect.Bottom));
        }

        public override Geometry Build(SvgTextContentElement element, string text, double x, double y)
        {
//            bool isRightToLeft = false;
            var xmlLang = element.XmlLang;
            if (!string.IsNullOrWhiteSpace(xmlLang))
            {
                if (string.Equals(xmlLang, "ar", StringComparison.OrdinalIgnoreCase)      // Arabic language
                    || string.Equals(xmlLang, "he", StringComparison.OrdinalIgnoreCase))  // Hebrew language
                {
//                    isRightToLeft = true;

                    //                    this.BidiLevel = 1;

                    if (text.Length > 0)
                    {
                        char[] charArray = text.ToCharArray();
                        Array.Reverse(charArray);
                        text = new string(charArray);
                    }
                }
            }

            var alignment = this.TextAlignment;
            if (alignment != TextAlignment.Left)
            {
                var textSize = this.MeasureText(element, text, true);
                var textWidth = Math.Max(textSize.Width, this.Width);
                if (alignment == TextAlignment.Center)
                {
                    x -= textWidth / 2;
                }
                else
                {
                    x -= textWidth;
                }
            }
            //else if (isRightToLeft)
            //{
            //    var textSize  = this.MeasureText(element, text, true);
            //    var textWidth = Math.Max(textSize.Width, this.Width);

            //    x -= textWidth;
            //}

            ComputeMeasurement(text, x, y + this.Baseline);

            _textWidth = 0;

            if (_glyphRun == null)
            {
                return new GeometryGroup();
            }

            // Approximate the width of the text...
            Rect designRect = _glyphRun.ComputeAlignmentBox();
            //designRect.Offset(_glyphRunOrigin.X, _glyphRunOrigin.Y);

            _textWidth = Math.Max(0, designRect.Right);

            var geometry = _glyphRun.BuildGeometry();
            if (geometry == null)
            {
                return geometry;
            }

            if (_textDecorations == null || _textDecorations.Count == 0)
            {
                if (_buildPathGeometry)
                {
                    PathGeometry pathGeometry = geometry as PathGeometry;
                    if (pathGeometry == null)
                    {
                        pathGeometry = new PathGeometry();
                        pathGeometry.AddGeometry(geometry);
                    }
                    return pathGeometry;
                }
                return geometry;
            }

            var baseline = y + this.Baseline;

            GeometryGroup geomGroup = geometry as GeometryGroup;
            if (geomGroup == null)
            {
                geomGroup = new GeometryGroup();
                geomGroup.Children.Add(geometry);
            }

            foreach (var textDeDecoration in _textDecorations)
            {
                double decorationPos = 0;
                double decorationThickness = 0;

                if (textDeDecoration.Location == TextDecorationLocation.Strikethrough)
                {
                    decorationPos = baseline - (this.StrikethroughPosition * _fontSize);
                    decorationThickness = this.StrikethroughThickness * _fontSize;
                }
                else if (textDeDecoration.Location == TextDecorationLocation.Underline)
                {
                    decorationPos = baseline - (this.UnderlinePosition * _fontSize);
                    decorationThickness = this.UnderlineThickness * _fontSize;
                }
                else if (textDeDecoration.Location == TextDecorationLocation.OverLine)
                {
                    decorationPos = baseline - _fontSize;
                    decorationThickness = this.OverlineThickness * _fontSize;
                }
                Rect bounds = new Rect(geomGroup.Bounds.Left, decorationPos, geomGroup.Bounds.Width, decorationThickness + 0.5);

                var rectGeom = new RectangleGeometry(bounds);
                if (_buildPathGeometry)
                {
                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.AddGeometry(rectGeom);
                    geomGroup.Children.Add(pathGeometry);
                }
                else
                {
                    geomGroup.Children.Add(rectGeom);
                }
            }

            if (_buildPathGeometry)
            {
                PathGeometry pathGeometry = new PathGeometry();
                pathGeometry.AddGeometry(geomGroup);

                return pathGeometry;
            }
            return geomGroup;
        }

        #endregion

        #region Private Methods

        private XmlLanguage Language
        {
            get {
                if (_culture != null)
                {
                    return XmlLanguage.GetLanguage(_culture.TwoLetterISOLanguageName);
                }
                return XmlLanguage.Empty;
            }
        }

        private sealed class ParsedGlyphData
        {
            public ushort glyphIndex;
            public double advanceWidth;
            public double offsetX;
            public double offsetY;
        };

        private GlyphRun CreateGlyphRun(Point origin, XmlLanguage language)
        {
            return new GlyphRun(
                _glyphTypeface,               // GlyphTypeface
                _bidiLevel,                   // Bidi level
                _isSideways,                  // sideways flag
                _fontSize,                    // rendering em size in MIL units
                _glyphIndices,                // glyph indices
                origin,                       // origin of glyph-drawing space
                _advanceWidths,               // glyph advances
                _glyphOffsets,                // glyph offsets
                _unicodeString.ToCharArray(), // unicode characters
                _deviceFontName,              // device font
                _clusterMap,                  // cluster map
                null,                         // caret stops
                language                      // language
            );
        }

        private void ComputeMeasurement(string text, double OriginX, double OriginY)
        {
            _unicodeString = text;

            _glyphRun = null;
            ParseGlyphRunProperties();
            if (_glyphRun != null)
            {
                return;
            }

            bool leftToRight = ((BidiLevel & 1) == 0);

            bool haveOriginX = !double.IsNaN(OriginX);
            bool haveOriginY = !double.IsNaN(OriginY);

            bool measurementGlyphRunOriginValid = false;

            Rect alignmentRect = new Rect();
            if (haveOriginX && haveOriginY && leftToRight)
            {
                _glyphRun = CreateGlyphRun(new Point(OriginX, OriginY), this.Language);
                measurementGlyphRunOriginValid = true;
            }
            else
            {
                _glyphRun = CreateGlyphRun(new Point(), this.Language);
                // compute alignment box for origins
                alignmentRect = _glyphRun.ComputeAlignmentBox();
            }

            if (haveOriginX)
                _glyphRunOrigin.X = OriginX;
            else
                _glyphRunOrigin.X = leftToRight ? 0 : alignmentRect.Width;

            if (haveOriginY)
                _glyphRunOrigin.Y = OriginY;
            else
                _glyphRunOrigin.Y = -alignmentRect.Y;

            if (!measurementGlyphRunOriginValid)
                _glyphRun = CreateGlyphRun(_glyphRunOrigin, this.Language);
        }

        ///<SecurityNote>
        /// Critical as it accesses the base Uri
        /// TreatAsSafe as it only uses this to load glyphtypefaces, and this information is not disclosed.
        ///</SecurityNote>
        private void ParseGlyphRunProperties()
        {
            if (string.IsNullOrEmpty(_unicodeString))
                throw new ArgumentException();

            // parse the Indices property
            List<ParsedGlyphData> parsedGlyphs;
            int glyphCount = ParseGlyphsProperty(_glyphTypeface, _unicodeString,
                _isSideways, out parsedGlyphs, out _clusterMap);

            Debug.Assert(parsedGlyphs.Count == glyphCount);

            _glyphIndices  = new ushort[glyphCount];
            _advanceWidths = new double[glyphCount];

            // Delay creating glyphOffsets array because in many common cases it will contain only zeroed entries.
            _glyphOffsets = null;

            int i = 0;

            double fromEmToMil = _fontSize / EmMultiplier;

            foreach (ParsedGlyphData parsedGlyphData in parsedGlyphs)
            {
                _glyphIndices[i] = parsedGlyphData.glyphIndex;

                // convert advances and offsets from integers in em space to doubles coordinates in MIL space
                _advanceWidths[i] = parsedGlyphData.advanceWidth * fromEmToMil;

                if (!parsedGlyphData.offsetX.Equals(0) || !parsedGlyphData.offsetY.Equals(0))
                {
                    // Lazily create glyph offset array. Previous entries will be correctly set to zero
                    // by the default Point ctor.
                    if (_glyphOffsets == null)
                        _glyphOffsets = new Point[glyphCount];

                    _glyphOffsets[i].X = parsedGlyphData.offsetX * fromEmToMil;
                    _glyphOffsets[i].Y = parsedGlyphData.offsetY * fromEmToMil;
                }
                ++i;
            }
        }

        private static bool IsEmpty(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsWhiteSpace(c))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Read GlyphIndex specification - glyph index value with an optional glyph cluster prefix.
        /// </summary>
        /// <param name="valueSpec"></param>
        /// <param name="inCluster"></param>
        /// <param name="glyphClusterSize"></param>
        /// <param name="characterClusterSize"></param>
        /// <param name="glyphIndex"></param>
        /// <returns>true if glyph index is present, false if glyph index is not present.</returns>
        private bool ReadGlyphIndex(string valueSpec, ref bool inCluster, ref int glyphClusterSize,
            ref int characterClusterSize, ref ushort glyphIndex)
        {
            // the format is ... [(CharacterClusterSize[:GlyphClusterSize])] GlyphIndex ...
            string glyphIndexString = valueSpec;

            int firstBracket = valueSpec.IndexOf('(');
            if (firstBracket != -1)
            {
                // Only spaces are allowed before the bracket
                for (int i = 0; i < firstBracket; i++)
                {
                    if (!char.IsWhiteSpace(valueSpec[i]))
                        throw new ArgumentException();
                }

                if (inCluster)
                    throw new ArgumentException();

                int secondBracket = valueSpec.IndexOf(')');
                if (secondBracket == -1 || secondBracket <= firstBracket + 1)
                    throw new ArgumentException();

                // look for colon separator
                int colon = valueSpec.IndexOf(':');
                if (colon == -1)
                {
                    // parse glyph cluster size
                    string characterClusterSpec = valueSpec.Substring(firstBracket + 1, secondBracket - (firstBracket + 1));
                    characterClusterSize        = int.Parse(characterClusterSpec, CultureInfo.InvariantCulture);
                    glyphClusterSize            = 1;
                }
                else
                {
                    if (colon <= firstBracket + 1 || colon >= secondBracket - 1)
                        throw new ArgumentException();
                    string characterClusterSpec = valueSpec.Substring(firstBracket + 1, colon - (firstBracket + 1));
                    characterClusterSize        = int.Parse(characterClusterSpec, CultureInfo.InvariantCulture);
                    string glyphClusterSpec     = valueSpec.Substring(colon + 1, secondBracket - (colon + 1));
                    glyphClusterSize            = int.Parse(glyphClusterSpec, CultureInfo.InvariantCulture);
                }
                inCluster = true;
                glyphIndexString = valueSpec.Substring(secondBracket + 1);
            }
            if (IsEmpty(glyphIndexString))
                return false;

            glyphIndex = ushort.Parse(glyphIndexString, CultureInfo.InvariantCulture);
            return true;
        }

        private static double GetAdvanceWidth(GlyphTypeface glyphTypeface, ushort glyphIndex, bool sideways)
        {
            double advance = sideways ? glyphTypeface.AdvanceHeights[glyphIndex] : glyphTypeface.AdvanceWidths[glyphIndex];
            return advance * EmMultiplier;
        }

        private ushort GetGlyphFromCharacter(GlyphTypeface glyphTypeface, char character)
        {
            ushort glyphIndex;
            // TryGetValue will return zero glyph index for missing code points,
            // which is the right thing to display per http://www.microsoft.com/typography/otspec/cmap.htm
            glyphTypeface.CharacterToGlyphMap.TryGetValue(character, out glyphIndex);
            return glyphIndex;
        }

        /// <summary>
        /// Performs validation against cluster map size and throws a well defined exception.
        /// </summary>
        private static void SetClusterMapEntry(ushort[] clusterMap, int index, ushort value)
        {
            if (index < 0 || index >= clusterMap.Length)
                throw new ArgumentException();
            clusterMap[index] = value;
        }

        // ------------------------------------------------------------------------------------------------
        // Parses a semicolon-delimited list of glyph specifiers, each of which consists
        // of up to 4 comma-delimited values:
        //   - glyph index (ushort)
        //   - glyph advance (double)
        //   - glyph offset X (double)
        //   - glyph offset Y (double)
        // A glyph entry can be have a cluster size prefix (int or pair of ints separated by a colon)
        // Whitespace adjacent to a delimiter (comma or semicolon) is ignored.
        // Returns the number of glyph specs parsed (number of semicolons plus 1).
        // ------------------------------------------------------------------------------------------------
        private int ParseGlyphsProperty(GlyphTypeface fontFace, string unicodeString, bool sideways,
            out List<ParsedGlyphData> parsedGlyphs, out ushort[] clusterMap)
        {
            string glyphsProp = string.Empty;

            // init for the whole parse, including the result arrays
            int parsedGlyphCount     = 0;
            int parsedCharacterCount = 0;

            int characterClusterSize = 1;
            int glyphClusterSize     = 1;

            bool inCluster = false;

            // make reasonable capacity guess on how many glyphs we can expect
            int estimatedNumberOfGlyphs;

            if (!string.IsNullOrEmpty(unicodeString))
            {
                clusterMap              = new ushort[unicodeString.Length];
                estimatedNumberOfGlyphs = unicodeString.Length;
            }
            else
            {
                clusterMap              = null;
                estimatedNumberOfGlyphs = 8;
            }

            if (!string.IsNullOrEmpty(glyphsProp))
                estimatedNumberOfGlyphs = Math.Max(estimatedNumberOfGlyphs, glyphsProp.Length / 5);

            parsedGlyphs = new List<ParsedGlyphData>(estimatedNumberOfGlyphs);

            ParsedGlyphData parsedGlyphData = new ParsedGlyphData();

            if (!string.IsNullOrEmpty(glyphsProp))
            {
                // init per-glyph values for the first glyph/position
                int valueWithinGlyph = 0; // which value we're on (how many commas have we seen in this glyph)?
                int valueStartIndex  = 0; // where (what index of Glyphs prop string) did this value start?

                // iterate and parse the characters of the Indices property
                for (int i = 0; i <= glyphsProp.Length; i++)
                {
                    // get next char or pseudo-terminator
                    char c = i < glyphsProp.Length ? glyphsProp[i] : '\0';

                    // finished scanning the current per-glyph value?
                    if ((c == ',') || (c == ';') || (i == glyphsProp.Length))
                    {
                        int len = i - valueStartIndex;

                        string valueSpec = glyphsProp.Substring(valueStartIndex, len);

                        switch (valueWithinGlyph)
                        {
                            case 0:
                                bool wasInCluster = inCluster;
                                // interpret cluster size and glyph index spec
                                if (!ReadGlyphIndex(valueSpec, ref inCluster, ref glyphClusterSize,
                                    ref characterClusterSize, ref parsedGlyphData.glyphIndex))
                                {
                                    if (string.IsNullOrEmpty(unicodeString))
                                        throw new ArgumentException();

                                    if (unicodeString.Length <= parsedCharacterCount)
                                        throw new ArgumentException();

                                    parsedGlyphData.glyphIndex = GetGlyphFromCharacter(fontFace, unicodeString[parsedCharacterCount]);
                                }

                                if (!wasInCluster && clusterMap != null)
                                {
                                    // fill out cluster map at the start of each cluster
                                    if (inCluster)
                                    {
                                        for (int ch = parsedCharacterCount; ch < parsedCharacterCount + characterClusterSize; ++ch)
                                        {
                                            SetClusterMapEntry(clusterMap, ch, (ushort)parsedGlyphCount);
                                        }
                                    }
                                    else
                                    {
                                        SetClusterMapEntry(clusterMap, parsedCharacterCount, (ushort)parsedGlyphCount);
                                    }
                                }
                                parsedGlyphData.advanceWidth = GetAdvanceWidth(fontFace, parsedGlyphData.glyphIndex, sideways);
                                break;

                            case 1:
                                // interpret glyph advance spec
                                if (!IsEmpty(valueSpec))
                                {
                                    parsedGlyphData.advanceWidth = double.Parse(valueSpec, CultureInfo.InvariantCulture);
                                    if (parsedGlyphData.advanceWidth < 0)
                                        throw new ArgumentException();
                                }
                                break;

                            case 2:
                                // interpret glyph offset X
                                if (!IsEmpty(valueSpec))
                                    parsedGlyphData.offsetX = double.Parse(valueSpec, CultureInfo.InvariantCulture);
                                break;

                            case 3:
                                // interpret glyph offset Y
                                if (!IsEmpty(valueSpec))
                                    parsedGlyphData.offsetY = double.Parse(valueSpec, CultureInfo.InvariantCulture);
                                break;

                            default:
                                // too many commas; can't interpret
                                throw new ArgumentException();
                        }

                        // prepare to scan next value (if any)
                        valueWithinGlyph++;
                        valueStartIndex = i + 1;
                    }

                    // finished processing the current glyph?
                    if ((c == ';') || (i == glyphsProp.Length))
                    {
                        parsedGlyphs.Add(parsedGlyphData);
                        parsedGlyphData = new ParsedGlyphData();

                        if (inCluster)
                        {
                            --glyphClusterSize;
                            // when we reach the end of a glyph cluster, increment character index
                            if (glyphClusterSize == 0)
                            {
                                parsedCharacterCount += characterClusterSize;
                                inCluster = false;
                            }
                        }
                        else
                        {
                            ++parsedCharacterCount;
                        }
                        parsedGlyphCount++;

                        // initalize new per-glyph values
                        valueWithinGlyph = 0; // which value we're on (how many commas have we seen in this glyph)?
                        valueStartIndex = i + 1; // where (what index of Glyphs prop string) did this value start?
                    }
                }
            }

            // fill the remaining glyphs with defaults, assuming 1:1 mapping
            if (unicodeString != null)
            {
                while (parsedCharacterCount < unicodeString.Length)
                {
                    if (inCluster)
                        throw new ArgumentException();

                    if (unicodeString.Length <= parsedCharacterCount)
                        throw new ArgumentException();

                    parsedGlyphData.glyphIndex = GetGlyphFromCharacter(fontFace, unicodeString[parsedCharacterCount]);
                    parsedGlyphData.advanceWidth = GetAdvanceWidth(fontFace, parsedGlyphData.glyphIndex, sideways);
                    parsedGlyphs.Add(parsedGlyphData);
                    parsedGlyphData = new ParsedGlyphData();
                    SetClusterMapEntry(clusterMap, parsedCharacterCount, (ushort)parsedGlyphCount);
                    ++parsedCharacterCount;
                    ++parsedGlyphCount;
                }
            }

            // return number of glyphs actually specified
            return parsedGlyphCount;
        }

        #endregion
    }
}
