using System.Collections.Generic;

namespace SharpVectors.Dom.Css
{
    static class StrSliceExtensions
    {
        public static StrSlice ToSlice(this string text)
        {
            return new StrSlice(text);
        }

        public static StrSlice[] Split(this StrSlice slice, char[] separators, bool removeEmptyEntries = true)
        {
            var len = slice.Length;
            var pos = 0;
            var items = new List<StrSlice>();
            for (var i = 0; i < len; i++)
            {
                if (IsInArrayOfChar(separators, slice[i]))
                {
                    var subSlice = slice.Substring(pos, i - pos);
                    if (!removeEmptyEntries || !subSlice.IsEmpty)
                    {
                        items.Add(subSlice);
                    }
                    pos = i + 1;
                }
            }
            var subSliceFinal = slice.Substring(pos, len - pos);
            if (!removeEmptyEntries || !subSliceFinal.IsEmpty)
            {
                items.Add(subSliceFinal);
            }
            return items.ToArray();
        }

        static bool IsInArrayOfChar(char[] separators, char charValue)
        {
            foreach (var c in separators)
            {
                if (c == charValue)
                {
                    return true;
                }
            }
            return false;
        }

        private static int DigitBase10Value(char ch)
        {
            return ch - '0';
        }

        private static int DigitBase16Value(char ch)
        {
            if (ch >= 0 && ch <= '9')
            {
                return ch - '0';
            }
            if (ch >= 'a' && ch <= 'f')
            {
                return ch - 'a' + 10;
            }
            if (ch >= 'A' && ch <= 'F')
            {
                return ch - 'A' + 10;
            }
            return 0;
        }

        internal static int ParseIntBase10(this StrSlice strSlice)
        {
            bool isMinus = strSlice[0] == '-';
            if (isMinus)
            {
                strSlice = strSlice.Substring(1);
            }
            int result = 0;
            for (var index = 0; index < strSlice.Length; index++)
            {
                result = result * 10 + DigitBase10Value(strSlice[index]);
            }
            if (isMinus)
            {
                return -result;
            }
            return result;
        }

        public static int ParseIntBase16(this StrSlice strSlice)
        {
            bool isMinus = strSlice[0] == '-';
            if (isMinus)
            {
                strSlice = strSlice.Substring(1);
            }
            int result = 0;
            for (var index = 0; index < strSlice.Length; index++)
            {
                result = result * 16 + DigitBase16Value(strSlice[index]);
            }
            if (isMinus)
            {
                return -result;
            }
            return result;
        }
    }
}