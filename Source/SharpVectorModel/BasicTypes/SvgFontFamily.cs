using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This provides information on the OpenType and Web fonts to the <see cref="SvgDocument"/>.
    /// </summary>
    public sealed class SvgFontFamily
    {
        #region Private Fields

        private bool _isDisposable;

        private bool _isLoaded;

        private object _tag;
        private string _name;
        private string _sourceUri;
        private string _fontUri;

        #endregion

        #region Constructors and Destructor

        public SvgFontFamily(bool isDisposable)
            : this(isDisposable, null, null)
        {
        }

        public SvgFontFamily(bool isDisposable, string fontUri)
            : this(isDisposable, fontUri, null)
        {
        }

        public SvgFontFamily(bool isDisposable, string fontUri, string sourceUri)
        {
            _fontUri     = fontUri;
            _sourceUri   = sourceUri;
            _isDisposable = isDisposable;
        }

        #endregion

        #region Public Properties

        public object Tag
        {
            get {
                return _tag;
            }
            set {
                _tag = value;
            }
        }

        public string Name
        {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }

        public bool IsLoaded
        {
            get {
                return _isLoaded;
            }
            set {
                _isLoaded = value;
            }
        }

        public string SourceUri
        {
            get {
                return _sourceUri;
            }
        }

        public string FontUri
        {
            get {
                return _fontUri;
            }
        }

        #endregion

        #region IDisposable Members

        public bool IsDisposable
        {
            get {
                return _isDisposable;
            }
            set {
                _isDisposable = value;
            }
        }

        #endregion
    }
}
