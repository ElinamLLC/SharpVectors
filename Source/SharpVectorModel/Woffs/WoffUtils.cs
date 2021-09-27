using System;

namespace SharpVectors.Woffs
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>Data types</para>
    /// <para>UInt32 32-bit (4-byte) unsigned integer in big-endian format</para>
    /// <para>UInt16 16-bit (2-byte) unsigned integer in big-endian format</para>
    /// </remarks>
    public static class WoffUtils
    {
        public const byte Woff1Version      = 1;  //  Web Open Font Format (WOFF), File Format 1.0
        public const byte Woff2Version      = 2;  //  Web Open Font Format (WOFF), File Format 2.0

        public const string Woff1FileExt    = ".woff";
        public const string Woff2FileExt    = ".woff2";

        public const string TtfFileExt      = ".ttf";
        public const string OtfFileExt      = ".otf";

        public const string TtcFileExt      = ".ttc";
        public const string OtcFileExt      = ".otc";

        public const ushort Woff1HeaderSize = (9 * WoffBuffer.SizeOfUInt)  + (4 * WoffBuffer.SizeOfUShort);  //  a 44-byte header
        public const ushort Woff2HeaderSize = (10 * WoffBuffer.SizeOfUInt) + (4 * WoffBuffer.SizeOfUShort);  //  a 48-byte header

        public const ushort Woff1DirSize    = 5 * WoffBuffer.SizeOfUInt;

        public const uint TtfSignature      = 0x00010000;
        public const uint TtcSignature      = 0x74746366; // 'ttcf'
        public const uint OtfSignature      = 0x4F54544F; // 'OTTO'

        public const uint Woff1Signature    = 0x774F4646; // 'wOFF'
        public const uint Woff2Signature    = 0x774F4632; // 'wOF2'

        /// <summary>
        /// The byte boundary.
        /// </summary>
        public const uint PaddingSize       = 4;

        // WOFF2 only: Target for transformation
        public const ushort HeadIndex       = 1;   // head
        public const ushort HheaIndex       = 2;   // hhea
        public const ushort HmtxIndex       = 3;   // hmtx
        public const ushort MaxpIndex       = 4;   // maxp
        public const ushort NameIndex       = 5;   // name
        public const ushort GlyfIndex       = 10;  // glyf
        public const ushort LocaIndex       = 11;  // loca

        public static uint CalculatePadding(uint length)
        {
            uint mod = length % PaddingSize;
            return (mod == 0) ? 0 : PaddingSize - mod;
        }

        public static ushort MaxPower2LE(ushort n)
        {
            // returns max power of 2 <= n
            if (n == 0)
            {
                throw new ArithmeticException();
            }

            ushort pow2 = 1;
            n >>= 1;
            while (n != 0)
            {
                n >>= 1;
                pow2 <<= 1;
            }
            return pow2;
        }

        public static ushort Log2(ushort n)
        {
            // returns the integer component of log2 of n
            // fractional component is lost, but not needed by font spec
            if (n == 0)
            {
                throw new ArithmeticException();
            }

            ushort log2 = 0;
            n >>= 1;

            while (n != 0)
            {
                n >>= 1;
                log2++;
            }
            return log2;
        }
    }
}
