using System;
using System.Collections.Generic;
using System.Reflection;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Renderers.Texts
{
    public abstract class WpfTextBuilder
    {
        private const double DefaultDpi = 96.0d;

        protected readonly string _fontName;
        protected readonly double _fontSize;
        protected readonly Uri _fontUri;

        protected readonly double _dpiX;
        protected readonly double _dpiY;

        protected WpfTextBuilder(double fontSize)
        {
            var sysParam = typeof(SystemParameters);

            var dpiXProperty = sysParam.GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = sysParam.GetProperty("Dpi",  BindingFlags.NonPublic | BindingFlags.Static);

            _dpiX     = (int)dpiXProperty.GetValue(null, null);
            _dpiY     = (int)dpiYProperty.GetValue(null, null);
            _fontSize = fontSize;
        }

        protected WpfTextBuilder(string fontName, double fontSize, Uri fontUri = null)
            : this(fontSize)
        {
            _fontName = fontName;
            _fontUri = fontUri;
        }

        public abstract WpfFontFamilyType FontFamilyType
        {
            get;
        }
         

        public string FontName
        {
            get {
                return _fontName;
            }
        }

        public double FontSize
        {
            get {
                return _fontSize;
            }
        }

        public double FontSizeInPoints
        {
            get {
                return _fontSize * 72.0d / _dpiY;
            }
        }

        public Uri FontUri
        {
            get {
                return _fontUri;
            }
        }

        /// <summary>Gets the DPI along X axis.</summary>
        /// <value>The DPI along the X axis.</value>
        public double PixelsPerInchX
        {
            get {
                return _dpiX;
            }
        }

        /// <summary>Gets the DPI along Y axis.</summary>
        /// <value>The DPI along the Y axis.</value>
        public double PixelsPerInchY
        {
            get {
                return _dpiY;
            }
        }

        /// <summary>Gets the DPI scale on the X axis.</summary>
        /// <value>The DPI scale for the X axis.</value>
        public double DpiScaleX
        {
            get {
                return _dpiX / DefaultDpi;
            }
        }

        /// <summary>Gets the DPI scale on the Yaxis.</summary>
        /// <value>The DPI scale for the Y axis.</value>
        public double DpiScaleY
        {
            get {
                return _dpiY / DefaultDpi;
            }
        }

        /// <summary>Get or sets the PixelsPerDip at which the text should be rendered.</summary>
        /// <value>The current PixelsPerDip value.</value>
        public double PixelsPerDip
        {
            get {
                return _dpiY / DefaultDpi;
            }
        }

        public abstract double Ascent
        {
            get;
        }

        public abstract IList<Rect> MeasureChars(string text, bool canBeWhitespace = true);

        public abstract Size MeasureText(string text, bool canBeWhitespace = true);

        public abstract PathGeometry Build(string text, double x, double y);
    }
}
