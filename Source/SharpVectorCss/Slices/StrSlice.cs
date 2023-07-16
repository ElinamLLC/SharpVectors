namespace SharpVectors.Dom.Css
{
    struct StrSlice
    {
        private int _pos;
        private int _length;
        private string _text;

        public static readonly StrSlice Empty = new StrSlice(string.Empty);

        public StrSlice(string text)
        {
            _text = text;
            _pos = 0;
            _length = text.Length;
        }

        private StrSlice(string text, int pos, int length)
        {
            _text = text;
            _pos = pos;
            _length = length;
        }

        public string Text
        {
            get {
                return _text;
            }
        }

        public int Pos
        {
            get {
                return _pos;
            }
        }

        public int Length
        {
            get {
                return _length;
            }
        }

        public bool IsEmpty
        {
            get { return _length == 0; }
        }

        public char this[int i]
        {
            get { return _text[i + _pos]; }
        }

        public StrSlice Substring(int pos, int len)
        {
            return new StrSlice(_text, _pos + pos, len);
        }

        public StrSlice Substring(int pos)
        {
            return new StrSlice(_text, _pos + pos, _length - pos);
        }

        public override string ToString()
        {
            return _text.Substring(_pos, _length);
        }

        public static implicit operator string(StrSlice strSlice) => strSlice.ToString();

        public StrSlice Trim()
        {
            if (_length == 0)
            {
                return this;
            }
            int startNoSpaces = -1;
            for (var i = 0; i < _length; i++)
            {
                if (!char.IsWhiteSpace(this[i]))
                {
                    startNoSpaces = i;
                    break;
                }
            }
            if (startNoSpaces == -1)
            {
                return Empty;
            }
            var indexDesc = _length - 1;
            while (char.IsWhiteSpace(this[indexDesc]))
            {
                indexDesc--;
            }
            return this.Substring(startNoSpaces, indexDesc - startNoSpaces + 1);
        }
    }
}