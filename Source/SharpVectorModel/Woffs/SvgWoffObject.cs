using System;
using System.IO;
using System.Text;

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
    public abstract class SvgWoffObject
    {
        public const byte SizeByte         = 1;
        public const byte SizeChar         = 1;
        public const byte SizeUShort       = 2;
        public const byte SizeShort        = 2;
        public const byte SizeUInt24       = 3;
        public const byte SizeULong        = 4;
        public const byte SizeLong         = 4;
        public const byte SizeFixed        = 4;
        public const byte SizeFUnit        = 4;
        public const byte SizeFWord        = 2;
        public const byte SizeUFWord       = 2;
        public const byte SizeF2Dot14      = 2;
        public const byte SizeLongDatetime = 8;
        public const byte SizeTag          = 4;
        public const byte SizeGlyphID      = 2;
        public const byte SizeOffset       = 2;

        protected byte[] _header;

        protected SvgWoffObject()
        {
        }

        public byte[] Header
        {
            get {
                return _header;
            }
            protected set {
                _header = value;
            }
        }

        public abstract uint HeaderSize { get; }

        public virtual bool SetHeader(byte[] header)
        {
            _header = header;
            return (_header != null && _header.Length == this.HeaderSize);
        }

        public static uint IntValue(byte[] tag)
        {
            return (uint)(tag[0] << 24 | tag[1] << 16 | tag[2] << 8 | tag[3]);
        }

        public static uint IntValue(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new InvalidOperationException(nameof(s));
            }

            try
            {
                string sub = s.PadRight(4).Substring(0, 4);
                byte[] b = Encoding.ASCII.GetBytes(sub);
                return IntValue(b);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("SvgWoffObject.IntValue", e);
            }
        }

        public static byte[] ByteValue(uint tag)
        {
            byte[] b = new byte[4];
            b[0] = (byte)(0xff & (tag >> 24));
            b[1] = (byte)(0xff & (tag >> 16));
            b[2] = (byte)(0xff & (tag >> 8));
            b[3] = (byte)(0xff & tag);
            return b;
        }

        public static string StringValue(uint tag)
        {
            try
            {
                return Encoding.ASCII.GetString(ByteValue(tag));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("SvgWoffObject.StringValue", e);
            }
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

        /// <summary>
        /// Reads a number of characters from the current source Stream and 
        /// writes the data to the target array at the specified index.
        /// </summary>
        /// <param name="sourceStream">
        /// The source Stream to read from.
        /// </param>
        /// <param name="target">
        /// Contains the array of characteres read from the source Stream.
        /// </param>
        /// <param name="start">
        /// The starting index of the target array.
        /// </param>
        /// <param name="count">
        /// The maximum number of characters to read from the source Stream.
        /// </param>
        /// <returns>
        /// The number of characters read. The number will be less 
        /// than or equal to count depending on the data available 
        /// in the source Stream. Returns -1 if the end of the stream 
        /// is reached.
        /// </returns>
        public static int Read(Stream sourceStream, ref byte[] target, int start, int count)
        {
            // Returns 0 bytes if not enough space in target
            if (target.Length == 0)
                return 0;

            int bytesRead = sourceStream.Read(target, start, count);

            // Returns -1 if EOF
            if (bytesRead == 0)
                return -1;

            return bytesRead;
        }

        public static byte[] ReadBytes(Stream stream, int count, out int bytesRead)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            bytesRead = 0;

            if (count == 0)
            {
                return new byte[0];
            }

            byte[] buffer = new byte[count];

            do
            {
                int n = stream.Read(buffer, bytesRead, count);
                if (n == 0)
                    break;
                bytesRead += n;
                count -= n;
            } while (count > 0);

            if (bytesRead != buffer.Length)
            {
                // Trim buffer in this case.
                byte[] trimmedBuffer = new byte[bytesRead];
                Buffer.BlockCopy(buffer, 0, trimmedBuffer, 0, bytesRead);
                buffer = trimmedBuffer;
            }

            return buffer;
        }

        /// <summary>
        /// Reads a number of characters from the current source TextReader 
        /// and writes the data to the target array at the specified index.
        /// </summary>
        /// <param name="sourceTextReader">
        /// The source TextReader to read from
        /// </param>
        /// <param name="target">
        /// Contains the array of characteres read from the source TextReader.
        /// </param>
        /// <param name="start">
        /// The starting index of the target array.
        /// </param>
        /// <param name="count">
        /// The maximum number of characters to read from the 
        /// source TextReader.
        /// </param>
        /// <returns>
        /// The number of characters read. The number will be less than or 
        /// equal to count depending on the data available in the source 
        /// TextReader. Returns -1 if the end of the stream is reached.
        /// </returns>
        public static int Read(TextReader sourceTextReader, ref byte[] target, int start, int count)
        {
            // Returns 0 bytes if not enough space in target
            if (target.Length == 0) return 0;

            char[] charArray = new char[target.Length];
            int bytesRead = sourceTextReader.Read(charArray, start, count);

            // Returns -1 if EOF
            if (bytesRead == 0)
                return -1;

            for (int i = start; i < start + bytesRead; i++)
            {
                target[i] = (byte)charArray[i];
            }

            return bytesRead;
        }

        /// <summary>
        /// <c>UIntBase128</c> is a different variable length encoding of unsigned integers, 
        /// suitable for values up to <c>2^32 - 1</c>.
        /// </summary>
        /// <param name="stream">The font stream.</param>
        /// <param name="result">The <c>UIntBase128</c> encoded number</param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// A <c>UIntBase128</c>  encoded number is a sequence of bytes for which the most significant bit is set 
        /// for all but the last byte, and clear for the last byte. The number itself is base <c>128</c> 
        /// encoded in the lower 7 bits of each byte. 
        /// </para>
        /// <para>
        /// Thus, a decoding procedure for a <c>UIntBase128</c> is: 
        /// </para>
        /// <para>
        /// start with value = 0. Consume a byte, setting value = old value times 128 + (byte bitwise-and 127). 
        /// Repeat last step until the most significant bit of byte is false.
        /// </para>
        /// </remarks>
        public static bool ReadUIntBase128(Stream stream, out uint result)
        {
            // The "C-like" pseudo-code describing how to read the UIntBase128 format is presented below:
            result = 0;
            uint accum = 0;

            for (int i = 0; i < 5; i++)
            {
                int dataByte = stream.ReadByte();

                // No leading 0's
                if (i == 0 && dataByte == 0x80)
                    return false;

                // If any of top 7 bits are set then << 7 would overflow
                if ((accum & 0xFE000000) != 0)
                    return false;

                accum = (accum << 7) | (uint)(dataByte & 0x7F);

                // Spin until most significant bit of data byte is false
                if ((dataByte & 0x80) == 0)
                {
                    result = accum;
                    return true;
                }
            }
            // UIntBase128 sequence exceeds 5 bytes
            return false;
        }

        /// <summary>
        /// Try to skip bytes in the input stream and return the actual 
        /// number of bytes skipped.
        /// </summary>
        /// <param name="stream">
        /// Input stream that will be used to skip the bytes.
        /// </param>
        /// <param name="skipBytes">
        /// Number of bytes to be skipped.
        /// </param>
        /// <returns>
        /// Actual number of bytes skipped.
        /// </returns>
        public static int Skip(Stream stream, int skipBytes)
        {
            long oldPosition = stream.Position;
            long result = stream.Seek(skipBytes, SeekOrigin.Current) - oldPosition;
            return (int)result;
        }

        /// <summary>Skips a given number of characters into a given Stream.</summary>
        /// <param name="stream">The number of caracters to skip.</param>
        /// <param name="number">The stream in which the skips are done.</param>
        /// <returns>The number of characters skipped.</returns>
        public static long Skip(StreamReader stream, long number)
        {
            long skippedBytes = 0;
            for (long index = 0; index < number; index++)
            {
                stream.Read();
                skippedBytes++;
            }
            return skippedBytes;
        }

        /// <summary>
        /// Writes the data to the specified file stream
        /// </summary>
        /// <param name="data">Data to write</param>
        /// <param name="fileStream">File to write to</param>
        public static void WriteBytes(string data, FileStream fileStream)
        {
            int index = 0;
            int length = data.Length;

            while (index < length)
            {
                fileStream.WriteByte((byte)data[index++]);
            }
        }

        /// <summary>
        /// Writes the received string to the file stream
        /// </summary>
        /// <param name="data">String of information to write</param>
        /// <param name="fileStream">File to write to</param>
        public static void WriteChars(string data, FileStream fileStream)
        {
            WriteBytes(data, fileStream);
        }

        #region Public BytesReader Methods

        /// <summary>Reads a little endian small integer.</summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the int resides.</param>
        /// <returns>The int read from the buffer at the offset location.</returns>
        public static short ReadInt16LE(byte[] buffer, int offset)
        {
            return (short)(((buffer[offset + 1] & 0xff) << 8) |
                           ((buffer[offset + 0] & 0xff)));
        }

        /// <summary>Reads a big endian small integer.</summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the int resides.</param>
        /// <returns>The int read from the buffer at the offset location.</returns>
        public static short ReadInt16BE(byte[] buffer, int offset)
        {
            return (short)(((buffer[offset + 0] & 0xff) << 8) |
                           ((buffer[offset + 1] & 0xff)));
        }

        /// <summary>Reads a little endian small integer.</summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the int resides.</param>
        /// <returns>The int read from the buffer at the offset location.</returns>
        public static ushort ReadUInt16LE(byte[] buffer, int offset)
        {
            return (ushort)(((buffer[offset + 1] & 0xff) << 8) |
                           ((buffer[offset + 0] & 0xff)));
        }

        /// <summary>Reads a big endian small integer.</summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the int resides.</param>
        /// <returns>The int read from the buffer at the offset location.</returns>
        public static ushort ReadUInt16BE(byte[] buffer, int offset)
        {
            return (ushort)(((buffer[offset + 0] & 0xff) << 8) |
                            ((buffer[offset + 1] & 0xff)));
        }

        /// <summary>Reads a little endian integer.</summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the int resides.</param>
        /// <returns>The int read from the buffer at the offset location.</returns>
        public static int ReadInt32LE(byte[] buffer, int offset)
        {
            return ((buffer[offset + 3] & 0xff) << 24) |
                   ((buffer[offset + 2] & 0xff) << 16) |
                   ((buffer[offset + 1] & 0xff) << 8)  |
                   ((buffer[offset + 0] & 0xff));
        }

        /// <summary>Reads a big endian integer.</summary>
        /// <param name="b">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the int resides.</param>
        /// <returns>The int read from the buffer at the offset location.</returns>
        public static int ReadInt32BE(byte[] buffer, int offset)
        {
            return ((buffer[offset + 0] & 0xff) << 24) |
                   ((buffer[offset + 1] & 0xff) << 16) |
                   ((buffer[offset + 2] & 0xff) << 8)  |
                   ((buffer[offset + 3] & 0xff));
        }

        /// <summary>Reads a little endian integer.</summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the int resides.</param>
        /// <returns>The int read from the buffer at the offset location.</returns>
        public static uint ReadUInt32LE(byte[] buffer, int offset)
        {
            return (((uint)buffer[offset + 3] & 0xff) << 24) |
                   (((uint)buffer[offset + 2] & 0xff) << 16) |
                   (((uint)buffer[offset + 1] & 0xff) << 8)  |
                   (((uint)buffer[offset + 0] & 0xff));
        }

        /// <summary>Reads a big endian integer.</summary>
        /// <param name="b">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the int resides.</param>
        /// <returns>The int read from the buffer at the offset location.</returns>
        public static uint ReadUInt32BE(byte[] buffer, int offset)
        {
            return (((uint)buffer[offset + 0] & 0xff) << 24) |
                   (((uint)buffer[offset + 1] & 0xff) << 16) |
                   (((uint)buffer[offset + 2] & 0xff) << 8)  |
                   (((uint)buffer[offset + 3] & 0xff));
        }

        /// <summary>Reads a little endian 8 byte integer.</summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the long resides.</param>
        /// <returns>The long read from the buffer at the offset location.</returns>
        public static long ReadInt64LE(byte[] buffer, int offset)
        {
            return ((buffer[offset + 0] & 0xffL))       |
                   ((buffer[offset + 1] & 0xffL) << 8)  |
                   ((buffer[offset + 2] & 0xffL) << 16) |
                   ((buffer[offset + 3] & 0xffL) << 24) |
                   ((buffer[offset + 4] & 0xffL) << 32) |
                   ((buffer[offset + 5] & 0xffL) << 40) |
                   ((buffer[offset + 6] & 0xffL) << 48) |
                   ((buffer[offset + 7] & 0xffL) << 56);
        }

        /// <summary>Reads a little endian 8 byte integer.</summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the long resides.</param>
        /// <returns>The long read from the buffer at the offset location.</returns>
        public static long ReadInt64BE(byte[] buffer, int offset)
        {
            return ((buffer[offset + 7] & 0xffL))       |
                   ((buffer[offset + 6] & 0xffL) << 8)  |
                   ((buffer[offset + 5] & 0xffL) << 16) |
                   ((buffer[offset + 4] & 0xffL) << 24) |
                   ((buffer[offset + 3] & 0xffL) << 32) |
                   ((buffer[offset + 2] & 0xffL) << 40) |
                   ((buffer[offset + 1] & 0xffL) << 48) |
                   ((buffer[offset + 0] & 0xffL) << 56);
        }

        /// <summary>Reads a little endian 8 byte integer.</summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the long resides.</param>
        /// <returns>The long read from the buffer at the offset location.</returns>
        public static ulong ReadUInt64LE(byte[] buffer, int offset)
        {
            return (((ulong)buffer[offset + 0] & 0xffL))       |
                   (((ulong)buffer[offset + 1] & 0xffL) << 8)  |
                   (((ulong)buffer[offset + 2] & 0xffL) << 16) |
                   (((ulong)buffer[offset + 3] & 0xffL) << 24) |
                   (((ulong)buffer[offset + 4] & 0xffL) << 32) |
                   (((ulong)buffer[offset + 5] & 0xffL) << 40) |
                   (((ulong)buffer[offset + 6] & 0xffL) << 48) |
                   (((ulong)buffer[offset + 7] & 0xffL) << 56);
        }

        /// <summary>Reads a little endian 8 byte integer.</summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the long resides.</param>
        /// <returns>The long read from the buffer at the offset location.</returns>
        public static ulong ReadUInt64BE(byte[] buffer, int offset)
        {
            return (((ulong)buffer[offset + 7] & 0xffL))       |
                   (((ulong)buffer[offset + 6] & 0xffL) << 8)  |
                   (((ulong)buffer[offset + 5] & 0xffL) << 16) |
                   (((ulong)buffer[offset + 4] & 0xffL) << 24) |
                   (((ulong)buffer[offset + 3] & 0xffL) << 32) |
                   (((ulong)buffer[offset + 2] & 0xffL) << 40) |
                   (((ulong)buffer[offset + 1] & 0xffL) << 48) |
                   (((ulong)buffer[offset + 0] & 0xffL) << 56);
        }

        /// <summary>Reads a little endian float.</summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the float resides.</param>
        /// <returns>The float read from the buffer at the offset location.</returns>
        public static float ReadSingleLE(byte[] buffer, int offset)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32LE(buffer, offset)), 0);
        }

        /// <summary>Reads a big endian float.</summary>
        /// <param name="buffer">The raw data buffer</param>
        /// <param name="offset">The offset into the buffer where the float resides</param>
        /// <returns>The float read from the buffer at the offset location.</returns>
        public static float ReadSingleBE(byte[] buffer, int offset)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32BE(buffer, offset)), 0);
        }

        /// <summary>Reads a little endian double.</summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the double resides.</param>
        /// <returns>The double read from the buffer at the offset location.</returns>
        public static double ReadDoubleLE(byte[] buffer, int offset)
        {
            return BitConverter.Int64BitsToDouble(ReadInt64LE(buffer, offset));
        }

        /// <summary>
        /// Reads a big endian double.
        /// </summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the double resides.</param>
        /// <returns>The double read from the buffer at the offset location.</returns>
        public static double ReadDoubleBE(byte[] buffer, int offset)
        {
            return BitConverter.Int64BitsToDouble(ReadInt64BE(buffer, offset));
        }

        #endregion

        #region Public BytesWriter Methods

        /// <summary>
        /// Writes the given short to the given buffer at the given location
        /// in big endian format.
        /// </summary>
        /// <param name="buffer">
        /// the data buffer.
        /// </param>
        /// <param name="offset">
        /// the offset into the buffer where writing should occur.
        /// </param>
        /// <param name="value">
        /// the short to write.
        /// </param>
        /// <returns> 
        /// the number of bytes written.
        /// </returns>
        public static int WriteInt16LE(byte[] buffer, int offset, int value)
        {
            buffer[offset + 0] = (byte)((value) & 0xff);
            buffer[offset + 1] = (byte)((value >> 8) & 0xff);

            return 2;
        }

        /// <summary>
        /// Writes the given short to the given buffer at the given location
        /// in big endian format.
        /// </summary>
        /// <param name="buffer">
        /// the data buffer.
        /// </param>
        /// <param name="offset">
        /// the offset into the buffer where writing should occur.
        /// </param>
        /// <param name="value">
        /// the short to write.
        /// </param>
        /// <returns> 
        /// the number of bytes written.
        /// </returns>
        public static int WriteInt16BE(byte[] buffer, int offset, int value)
        {
            buffer[offset + 0] = (byte)((value >> 8) & 0xff);
            buffer[offset + 1] = (byte)((value) & 0xff);

            return 2;
        }

        /// <summary>
        /// Writes the given integer to the given buffer at the given location
        /// in little endian format.
        /// </summary>
        /// <param name="buffer">
        /// the data buffer
        /// </param>
        /// <param name="offset">
        /// the offset into the buffer where writing should occur
        /// </param>
        /// <param name="value">
        /// the integer to write
        /// </param>
        /// <returns> 
        /// the number of bytes written.
        /// </returns>
        public static int WriteInt32LE(byte[] buffer, int offset, int value)
        {
            buffer[offset + 0] = (byte)((value) & 0xff);
            buffer[offset + 1] = (byte)((value >> 8) & 0xff);
            buffer[offset + 2] = (byte)((value >> 16) & 0xff);
            buffer[offset + 3] = (byte)((value >> 24) & 0xff);

            return 4;
        }

        /// <summary>
        /// Writes the given integer to the given buffer at the given location
        /// in big endian format.
        /// </summary>
        /// <param name="buffer">
        /// the data buffer.
        /// </param>
        /// <param name="offset">
        /// the offset into the buffer where writing should occur.
        /// </param>
        /// <param name="value">
        /// the integer to write.
        /// </param>
        /// <returns> 
        /// the number of bytes written.
        /// </returns>
        public static int WriteInt32BE(byte[] buffer, int offset, int value)
        {
            buffer[offset + 0] = (byte)((value >> 24) & 0xff);
            buffer[offset + 1] = (byte)((value >> 16) & 0xff);
            buffer[offset + 2] = (byte)((value >> 8) & 0xff);
            buffer[offset + 3] = (byte)((value) & 0xff);

            return 4;
        }

        /// <summary>
        /// Writes the given long to the given buffer at the given location
        /// in little endian format.
        /// </summary>
        /// <param name="buffer">
        /// the data buffer
        /// </param>
        /// <param name="offset">
        /// the offset into the buffer where writing should occur
        /// </param>
        /// <param name="value">
        /// the long to write
        /// </param>
        /// <returns> 
        /// the number of bytes written
        /// </returns>
        public static int WriteInt64LE(byte[] buffer, int offset, long value)
        {
            buffer[offset + 0] = (byte)((value) & 0xff);
            buffer[offset + 1] = (byte)((value >> 8) & 0xff);
            buffer[offset + 2] = (byte)((value >> 16) & 0xff);
            buffer[offset + 3] = (byte)((value >> 24) & 0xff);
            buffer[offset + 4] = (byte)((value >> 32) & 0xff);
            buffer[offset + 5] = (byte)((value >> 40) & 0xff);
            buffer[offset + 6] = (byte)((value >> 48) & 0xff);
            buffer[offset + 7] = (byte)((value >> 56) & 0xff);

            return 8;
        }

        /// <summary>
        /// Writes the given long to the given buffer at the given location
        /// in big endian format.
        /// </summary>
        /// <param name="buffer">
        /// the data buffer
        /// </param>
        /// <param name="offset">
        /// the offset into the buffer where writing should occur.
        /// </param>
        /// <param name="value">
        /// the long to write.
        /// </param>
        /// <returns> 
        /// the number of bytes written.
        /// </returns>
        public static int WriteInt64BE(byte[] buffer, int offset, long value)
        {
            buffer[offset + 0] = (byte)((value >> 56) & 0xff);
            buffer[offset + 1] = (byte)((value >> 48) & 0xff);
            buffer[offset + 2] = (byte)((value >> 40) & 0xff);
            buffer[offset + 3] = (byte)((value >> 32) & 0xff);
            buffer[offset + 4] = (byte)((value >> 24) & 0xff);
            buffer[offset + 5] = (byte)((value >> 16) & 0xff);
            buffer[offset + 6] = (byte)((value >> 8) & 0xff);
            buffer[offset + 7] = (byte)((value) & 0xff);

            return 8;
        }

        /// <summary>
        /// Writes the given double to the given buffer at the given location
        /// in little endian format.
        /// </summary>
        /// <param name="buffer">
        /// the data buffer
        /// </param>
        /// <param name="offset">
        /// the offset into the buffer where writing should occur
        /// </param>
        /// <param name="value">
        /// the double to write
        /// </param>
        /// <returns> 
        /// the number of bytes written
        /// </returns>
        public static int WriteDoubleLE(byte[] buffer, int offset, double value)
        {
            return WriteInt64LE(buffer, offset, BitConverter.DoubleToInt64Bits(value));
        }

        /// <summary>
        /// Writes the given double to the given buffer at the given location
        /// in big endian format.
        /// </summary>
        /// <param name="buffer">
        /// the data buffer
        /// </param>
        /// <param name="offset">
        /// the offset into the buffer where writing should occur
        /// </param>
        /// <param name="value">
        /// the double to write
        /// </param>
        /// <returns> 
        /// the number of bytes written
        /// </returns>
        public static int WriteDoubleBE(byte[] buffer, int offset, double value)
        {
            return WriteInt64BE(buffer, offset, BitConverter.DoubleToInt64Bits(value));
        }

        public static int WriteUInt16BE(byte[] buffer, int offset, ushort value)
        {
            buffer[offset + 0] = (byte)(value >> 8);
            buffer[offset + 1] = (byte)value;

            return 2;
        }

        public static int WriteUInt32BE(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)value;

            return 4;
        }

        public static int WriteUInt64BE(byte[] buffer, int offset, ulong value)
        {
            buffer[offset + 0] = (byte)(value >> 56);
            buffer[offset + 1] = (byte)(value >> 48);
            buffer[offset + 2] = (byte)(value >> 40);
            buffer[offset + 3] = (byte)(value >> 32);
            buffer[offset + 4] = (byte)(value >> 24);
            buffer[offset + 5] = (byte)(value >> 16);
            buffer[offset + 6] = (byte)(value >> 8);
            buffer[offset + 7] = (byte)value;

            return 8;
        }

        #endregion

    }
}
