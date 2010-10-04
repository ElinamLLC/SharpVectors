using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace SharpVectors.Renderers.Texts
{
    public sealed class WpfTextRun
    {
        #region Private Fields

        private int    _vertOrientation;
        private int    _horzOrientation;
        private bool   _isLatinGlyph;
        private string _text;

        #endregion

        #region Constructors and Destructor

        public WpfTextRun()
        {
            _vertOrientation = -1;
            _horzOrientation = -1;
            _text            = String.Empty;
            _isLatinGlyph    = true;
        }

        public WpfTextRun(string text, bool isLatin, int vertOrientation,
            int horzOrientation)
        {
            _text            = text;
            _isLatinGlyph    = isLatin;
            _vertOrientation = vertOrientation;
            _horzOrientation = horzOrientation;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_text);
            }
        }

        public bool IsLatin
        {
            get
            {
                return _isLatinGlyph;
            }
            set
            {
                _isLatinGlyph = value;
            }                         
        }

        public int VerticalOrientation
        {
            get
            {
                return _vertOrientation;
            }
            set
            {
                _vertOrientation = value;
            }
        }

        public int HorizontalOrientation
        {
            get
            {
                return _horzOrientation;
            }
            set
            {
                _horzOrientation = value;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        #endregion

        #region Public Methods

        public static bool IsLatinGlyph(char ch)
        {
            if ((int)ch < 256)
            {
                return true;
            }

            return false;
        }

        public static IList<WpfTextRun> BreakWords(string text)
        {
            return BreakWords(text, -1, -1);
        }

        public static IList<WpfTextRun> BreakWords(string text, int vertOrientation,
            int horzOrientation)
        {
            if (String.IsNullOrEmpty(text))
            {
                return null;
            }

            List<WpfTextRun> textRunList = new List<WpfTextRun>();

            StringBuilder builder = new StringBuilder();

            int textLength    = text.Length;
            bool isLatinStart = IsLatinGlyph(text[0]);
            for (int i = 0; i < textLength; i++)
            {
                char nextChar = text[i];
                if (IsLatinGlyph(nextChar) == isLatinStart)
                {
                    builder.Append(nextChar);
                }
                else
                {
                    textRunList.Add(new WpfTextRun(builder.ToString(), isLatinStart, 
                        vertOrientation, horzOrientation));

                    builder.Length = 0;
                    isLatinStart   = IsLatinGlyph(nextChar);

                    builder.Append(nextChar);
                }
            }

            if (builder.Length != 0)
            {
                textRunList.Add(new WpfTextRun(builder.ToString(), isLatinStart,
                    vertOrientation, horzOrientation));
            }

            return textRunList;
        }

        #endregion
    }
}
