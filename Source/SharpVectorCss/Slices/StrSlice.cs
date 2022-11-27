namespace SharpVectors.Dom.Css
{
    struct StrSlice
    {
        public string Text { get; }
        public int Pos { get; }
        public int Length { get; }

        public bool IsEmpty
        {
            get { return Length == 0; }
        }

        public static StrSlice Empty = new StrSlice("");

        public override string ToString()
        {
            return Text.Substring(Pos, Length);
        }

        public StrSlice(string text)
        {
            Text = text;
            Pos = 0;
            Length = text.Length;
        }

        private StrSlice(string text, int pos, int length)
        {
            Text = text;
            Pos = pos;
            Length = length;
        }

        public char this[int i]
        {
            get { return Text[i+Pos]; }
        }

        public StrSlice Substring(int pos, int len)
        {
            return new StrSlice(Text, Pos+pos, len);
        }

        public StrSlice Substring(int pos)
        {
            return new StrSlice(Text, Pos+pos, Length-pos);
        }
        public static implicit operator string(StrSlice strSlice) => strSlice.ToString();

        public StrSlice Trim()
        {
            if (IsEmpty)
            {
                return this;
            }
            int startNoSpaces = -1;
            for (var i = 0; i < Length; i++)
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
            var indexDesc = Length-1;
            while (char.IsWhiteSpace(this[indexDesc]))
            {
                indexDesc--;
            }
            return this.Substring(startNoSpaces, indexDesc-startNoSpaces+1);
        }
    }
}