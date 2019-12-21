//------------------------------------------------------------------------------------
// Copyright 2015 Google Inc. All Rights Reserved. 
//
// Distributed under MIT license.
// See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
//------------------------------------------------------------------------------------

namespace SharpVectors.Compressions.Brotli
{
    /// <summary>Transformations on dictionary words.</summary>
    internal sealed class BrotliTransform
    {
        private readonly byte[] _prefix;

        private readonly int _type;

        private readonly byte[] _suffix;

        internal BrotliTransform(string prefix, int type, string suffix)
        {
            _prefix = ReadUniBytes(prefix);
            _type   = type;
            _suffix = ReadUniBytes(suffix);
        }

        internal static byte[] ReadUniBytes(string uniBytes)
        {
            byte[] result = new byte[uniBytes.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = unchecked((byte)uniBytes[i]);
            }
            return result;
        }

        internal static readonly BrotliTransform[] Transforms = new BrotliTransform[]
        {
            new BrotliTransform(string.Empty, WordTransformType.Identity, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " "),
            new BrotliTransform(" ", WordTransformType.Identity, " "),
            new BrotliTransform(string.Empty, WordTransformType.OmitFirst1, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseFirst, " "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " the "),
            new BrotliTransform(" ", WordTransformType.Identity, string.Empty),
            new BrotliTransform("s ", WordTransformType.Identity, " "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " of "),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseFirst, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " and "),
            new BrotliTransform(string.Empty, WordTransformType.OmitFirst2, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.OmitLast1, string.Empty),
            new BrotliTransform(", ", WordTransformType.Identity, " "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, ", "),
            new BrotliTransform(" ", WordTransformType.UppercaseFirst, " "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " in "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " to "),
            new BrotliTransform("e ", WordTransformType.Identity, " "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "\""),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "."),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "\">"),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "\n"),
            new BrotliTransform(string.Empty, WordTransformType.OmitLast3, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "]"),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " for "),
            new BrotliTransform(string.Empty, WordTransformType.OmitFirst3, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.OmitLast2, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " a "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " that "),
            new BrotliTransform(" ", WordTransformType.UppercaseFirst, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.Identity, ". "),
            new BrotliTransform(".", WordTransformType.Identity, string.Empty),
            new BrotliTransform(" ", WordTransformType.Identity, ", "),
            new BrotliTransform(string.Empty, WordTransformType.OmitFirst4, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " with "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "'"),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " from "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " by "),
            new BrotliTransform(string.Empty, WordTransformType.OmitFirst5, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.OmitFirst6, string.Empty),
            new BrotliTransform(" the ", WordTransformType.Identity, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.OmitLast4, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.Identity, ". The "),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseAll, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " on "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " as "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " is "),
            new BrotliTransform(string.Empty, WordTransformType.OmitLast7, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.OmitLast1, "ing "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "\n\t"),
            new BrotliTransform(string.Empty, WordTransformType.Identity, ":"),
            new BrotliTransform(" ", WordTransformType.Identity, ". "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "ed "),
            new BrotliTransform(string.Empty, WordTransformType.OmitFirst9, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.OmitFirst7, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.OmitLast6, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "("),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseFirst, ", "),
            new BrotliTransform(string.Empty, WordTransformType.OmitLast8, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " at "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "ly "),
            new BrotliTransform(" the ", WordTransformType.Identity, " of "),
            new BrotliTransform(string.Empty, WordTransformType.OmitLast5, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.OmitLast9, string.Empty),
            new BrotliTransform(" ", WordTransformType.UppercaseFirst, ", "),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseFirst, "\""),
            new BrotliTransform(".", WordTransformType.Identity, "("),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseAll, " "),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseFirst, "\">"),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "=\""),
            new BrotliTransform(" ", WordTransformType.Identity, "."),
            new BrotliTransform(".com/", WordTransformType.Identity, string.Empty),
            new BrotliTransform(" the ", WordTransformType.Identity, " of the "),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseFirst, "'"),
            new BrotliTransform(string.Empty, WordTransformType.Identity, ". This "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, ","),
            new BrotliTransform(".", WordTransformType.Identity, " "),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseFirst, "("),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseFirst, "."),
            new BrotliTransform(string.Empty, WordTransformType.Identity, " not "),
            new BrotliTransform(" ", WordTransformType.Identity, "=\""),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "er "),
            new BrotliTransform(" ", WordTransformType.UppercaseAll, " "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "al "),
            new BrotliTransform(" ", WordTransformType.UppercaseAll, string.Empty),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "='"),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseAll, "\""),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseFirst, ". "),
            new BrotliTransform(" ", WordTransformType.Identity, "("),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "ful "),
            new BrotliTransform(" ", WordTransformType.UppercaseFirst, ". "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "ive "),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "less "),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseAll, "'"),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "est "),
            new BrotliTransform(" ", WordTransformType.UppercaseFirst, "."),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseAll, "\">"),
            new BrotliTransform(" ", WordTransformType.Identity, "='"),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseFirst, ","),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "ize "),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseAll, "."),
            new BrotliTransform("\u00c2\u00a0", WordTransformType.Identity, string.Empty),
            new BrotliTransform(" ", WordTransformType.Identity, ","),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseFirst, "=\""),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseAll, "=\""),
            new BrotliTransform(string.Empty, WordTransformType.Identity, "ous "),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseAll, ", "),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseFirst, "='"),
            new BrotliTransform(" ", WordTransformType.UppercaseFirst, ","),
            new BrotliTransform(" ", WordTransformType.UppercaseAll, "=\""),
            new BrotliTransform(" ", WordTransformType.UppercaseAll, ", "),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseAll, ","),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseAll, "("),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseAll, ". "),
            new BrotliTransform(" ", WordTransformType.UppercaseAll, "."),
            new BrotliTransform(string.Empty, WordTransformType.UppercaseAll, "='"),
            new BrotliTransform(" ", WordTransformType.UppercaseAll, ". "),
            new BrotliTransform(" ", WordTransformType.UppercaseFirst, "=\""),
            new BrotliTransform(" ", WordTransformType.UppercaseAll, "='"),
            new BrotliTransform(" ", WordTransformType.UppercaseFirst, "='")
        };

        internal static int TransformDictionaryWord(byte[] dst, int dstOffset, byte[] word,
            int wordOffset, int len, BrotliTransform transform)
        {
            int offset = dstOffset;
            // Copy prefix.
            byte[] prefixBuffer = transform._prefix;
            int tmp = prefixBuffer.Length;
            int i = 0;
            // In most cases tmp < 10 -> no benefits from System.arrayCopy
            while (i < tmp)
            {
                dst[offset++] = prefixBuffer[i++];
            }
            // Copy trimmed word.
            int op = transform._type;
            tmp = WordTransformType.GetOmitFirst(op);
            if (tmp > len)
            {
                tmp = len;
            }
            wordOffset += tmp;
            len -= tmp;
            len -= WordTransformType.GetOmitLast(op);
            i = len;
            while (i > 0)
            {
                dst[offset++] = word[wordOffset++];
                i--;
            }
            if (op == WordTransformType.UppercaseAll || op == WordTransformType.UppercaseFirst)
            {
                int uppercaseOffset = offset - len;
                if (op == WordTransformType.UppercaseFirst)
                {
                    len = 1;
                }
                while (len > 0)
                {
                    tmp = dst[uppercaseOffset] & unchecked((int)(0xFF));
                    if (tmp < unchecked((int)(0xc0)))
                    {
                        if (tmp >= 'a' && tmp <= 'z')
                        {
                            dst[uppercaseOffset] ^= unchecked((byte)32);
                        }
                        uppercaseOffset += 1;
                        len -= 1;
                    }
                    else if (tmp < unchecked((int)(0xe0)))
                    {
                        dst[uppercaseOffset + 1] ^= unchecked((byte)32);
                        uppercaseOffset += 2;
                        len -= 2;
                    }
                    else
                    {
                        dst[uppercaseOffset + 2] ^= unchecked((byte)5);
                        uppercaseOffset += 3;
                        len -= 3;
                    }
                }
            }

            // Copy suffix.
            prefixBuffer = transform._suffix;
            tmp = prefixBuffer.Length;
            i = 0;
            while (i < tmp)
            {
                dst[offset++] = prefixBuffer[i++];
            }
            return offset - dstOffset;
        }
    }
}
