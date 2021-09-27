using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace SharpVectors.Woffs
{
    /// <summary>
    /// A memory buffer with support for Big-Endian based data types conversions (bytes to/from integers). 
    /// </summary>
    public sealed class WoffBuffer
    {
        #region Public Constants

        public const byte SizeOfByte         = 1;
        public const byte SizeOfChar         = 1;
        public const byte SizeOfUShort       = 2;
        public const byte SizeOfShort        = 2;
        public const byte SizeOfUInt24       = 3;
        public const byte SizeOfUInt         = 4;
        public const byte SizeOfInt          = 4;
        public const byte SizeOfULong        = 8;
        public const byte SizeOfLong         = 8;
        public const byte SizeOfFixed        = 4;
        public const byte SizeOfFUnit        = 4;
        public const byte SizeOfFWord        = 2;
        public const byte SizeOfUFWord       = 2;
        public const byte SizeOfF2Dot14      = 2;
        public const byte SizeOfLongDatetime = 8;
        public const byte SizeOfTag          = 4;
        public const byte SizeOfGlyphID      = 2;
        public const byte SizeOfOffset       = 2;

        // 255UInt16 data type parameters
        public const byte OneMoreByteCode1 = 255;
        public const byte OneMoreByteCode2 = 254;
        public const byte WordCode         = 253;
        public const byte LowestUCode      = 253;

        #endregion

        #region Private Fields

        private long _filePos; // file position from which this buffer was read, -1 if not from file
        private uint _length; // number of data bytes
        private uint _padBytesLength; // number of padding bytes on the end
        private byte[] _buffer;
        private uint _cachedChecksum;
        private bool _isValidChecksumAvailable;

        #endregion

        #region Constructors and Destructor

        public WoffBuffer()
        {
            _filePos        = -1; // -1 means not read from a file
            _length         = 0;
            _padBytesLength = 0;
            _buffer         = null;

            _cachedChecksum = 0;
        }  

        public WoffBuffer(uint length)
        {
            _filePos        = -1; // -1 means not read from a file
            _length         = length;
            _padBytesLength = (uint)CalcPadBytes((int)length, 4);
            _buffer         = new byte[_length + _padBytesLength];

            _cachedChecksum = 0;
        } 

        public WoffBuffer(byte[] buffer, bool padBytes = false)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "The buffer parameter is required and cannot be null.");
            }
            var length      = buffer.Length;
            _filePos        = -1; // -1 means not read from a file
            _length         = (uint)length;

            if (padBytes)
            {
                _padBytesLength = (uint)CalcPadBytes((int)length, 4);
                if (_padBytesLength > 0)
                {
                    _buffer = new byte[_length + _padBytesLength];
                    this.Copy(buffer);
                }
                else
                {
                    _buffer = buffer;
                }
            }
            else
            {
                _padBytesLength = 0;
                _buffer = buffer;
            }

            _cachedChecksum = 0;
        } 

        public WoffBuffer(uint filepos, uint length)
        {
            _filePos        = filepos;
            _length         = length;
            _padBytesLength = (uint)CalcPadBytes((int)length, 4);
            _buffer         = new byte[_length + _padBytesLength];

            _cachedChecksum = 0;
        }

        #endregion

        #region Public Properties

        public uint PadBytesLength
        {
            get {
                return _padBytesLength;
            }
        }

        public byte[] Buffer
        {
            get {
                return _buffer;
            }
        }

        public uint Length
        {
            get {
                return _length;
            }
        }

        public byte this[short index]
        {
            get {
                return _buffer[index];
            }
            set {
                _buffer[index] = value;
            }
        }

        public byte this[ushort index]
        {
            get {
                return _buffer[index];
            }
            set {
                _buffer[index] = value;
            }
        }

        public byte this[int index]
        {
            get {
                return _buffer[index];
            }
            set {
                _buffer[index] = value;
            }
        }

        public byte this[uint index]
        {
            get {
                return _buffer[index];
            }
            set {
                _buffer[index] = value;
            }
        }

        #endregion

        #region Public Methods

        public byte[] GetBuffer()
        {
            return _buffer;
        }

        public uint GetLength()
        {
            return _length;
        }

        public uint GetPaddedLength()
        {
            return _length + _padBytesLength;
        }

        public void Copy(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return;
            }
            if (_buffer == null || _buffer.Length < buffer.Length)
            {
                _length = (uint)Math.Max(_length, buffer.Length);
                _buffer = new byte[_length + _padBytesLength];
            }
            System.Buffer.BlockCopy(buffer, 0, _buffer, 0, buffer.Length);
        }

        public sbyte GetSbyte(uint offset)
        {
            return (sbyte)_buffer[offset];
        }

        public void SetSbyte(sbyte value, uint offset)
        {
            _buffer[offset] = (byte)value;

            _isValidChecksumAvailable = false;
        }  

        public byte GetByte(uint offset)
        {
            return _buffer[offset];
        }

        public void SetByte(byte value, uint offset)
        {
            _buffer[offset] = value;

            _isValidChecksumAvailable = false;
        }     

        public byte[] GetBytes(uint offset, int size)
        {
            byte[] buf = new byte[size];
            System.Buffer.BlockCopy(_buffer, (int)offset, buf, 0, size);
            return buf;
        }

        public byte[] GetBytes(uint offset, uint size)
        {
            byte[] buf = new byte[size];
            System.Buffer.BlockCopy(_buffer, (int)offset, buf, 0, (int)size);
            return buf;
        }

        public short GetShort(uint offset)
        {
            return (short)(_buffer[offset] << 8 | _buffer[offset + 1]);
        }

        public void SetShort(short value, uint offset)
        {
            _buffer[offset] = (byte)(value >> 8);
            _buffer[offset + 1] = (byte)value;

            _isValidChecksumAvailable = false;
        }  

        public void SetShort(int value, uint offset)
        {
            _buffer[offset] = (byte)(value >> 8);
            _buffer[offset + 1] = (byte)value;

            _isValidChecksumAvailable = false;
        }  

        public ushort GetUShort(uint offset)
        {
            return (ushort)(_buffer[offset] << 8 | _buffer[offset + 1]);
        }

        public void SetUShort(ushort value, uint offset)
        {
            _buffer[offset] = (byte)(value >> 8);
            _buffer[offset + 1] = (byte)value;

            _isValidChecksumAvailable = false;
        }  

        public int GetInt(uint offset)
        {
            return _buffer[offset] << 24 | _buffer[offset + 1] << 16 | _buffer[offset + 2] << 8 | _buffer[offset + 3];
        }

        public void SetInt(int value, uint offset)
        {
            _buffer[offset] = (byte)(value >> 24);
            _buffer[offset + 1] = (byte)(value >> 16);
            _buffer[offset + 2] = (byte)(value >> 8);
            _buffer[offset + 3] = (byte)value;

            _isValidChecksumAvailable = false;
        }  

        public uint GetUInt24(uint offset)
        {
            return (uint)
                (_buffer[offset] << 16 | _buffer[offset + 1] << 8 | _buffer[offset + 2]);
        }

        public uint GetUInt(uint offset)
        {
            return (uint)(
                _buffer[offset] << 24 | 
                _buffer[offset + 1] << 16 | 
                _buffer[offset + 2] << 8 | 
                _buffer[offset + 3]);
        }

        public void SetUInt(uint value, uint offset)
        {
            _buffer[offset] = (byte)(value >> 24);
            _buffer[offset + 1] = (byte)(value >> 16);
            _buffer[offset + 2] = (byte)(value >> 8);
            _buffer[offset + 3] = (byte)value;

            _isValidChecksumAvailable = false;
        }

        public long GetLong(uint offset)
        {
            return (long)(
                (ulong)_buffer[offset] << 56 |
                (ulong)_buffer[offset + 1] << 48 |
                (ulong)_buffer[offset + 2] << 40 |
                (ulong)_buffer[offset + 3] << 32 |
                (ulong)_buffer[offset + 4] << 24 |
                (ulong)_buffer[offset + 5] << 16 |
                (ulong)_buffer[offset + 6] << 8 |
                (ulong)_buffer[offset + 7]);

        }

        public void SetLong(long value, uint offset)
        {
            _buffer[offset] = (byte)(value >> 56);
            _buffer[offset + 1] = (byte)(value >> 48);
            _buffer[offset + 2] = (byte)(value >> 40);
            _buffer[offset + 3] = (byte)(value >> 32);
            _buffer[offset + 4] = (byte)(value >> 24);
            _buffer[offset + 5] = (byte)(value >> 16);
            _buffer[offset + 6] = (byte)(value >> 8);
            _buffer[offset + 7] = (byte)value;

            _isValidChecksumAvailable = false;
        }     

        public ulong GetULong(uint offset)
        {
            return
                (ulong)_buffer[offset] << 56 |
                (ulong)_buffer[offset + 1] << 48 |
                (ulong)_buffer[offset + 2] << 40 |
                (ulong)_buffer[offset + 3] << 32 |
                (ulong)_buffer[offset + 4] << 24 |
                (ulong)_buffer[offset + 5] << 16 |
                (ulong)_buffer[offset + 6] << 8 |
                (ulong)_buffer[offset + 7];
        }

        public void SetULong(ulong value, uint offset)
        {
            _buffer[offset] = (byte)(value >> 56);
            _buffer[offset + 1] = (byte)(value >> 48);
            _buffer[offset + 2] = (byte)(value >> 40);
            _buffer[offset + 3] = (byte)(value >> 32);
            _buffer[offset + 4] = (byte)(value >> 24);
            _buffer[offset + 5] = (byte)(value >> 16);
            _buffer[offset + 6] = (byte)(value >> 8);
            _buffer[offset + 7] = (byte)value;

            _isValidChecksumAvailable = false;
        }  

        public float GetFixed(uint offset)
        {
            return GetInt(offset) / 65536F;
        }

        public void SetFixed(float value, uint offset)
        {
        }

        public float GetF2Dot14(uint offset)
        {
            return (float)GetShort(offset) / 16384;
        }

        public string GetTag(uint offset)
        {
            var tag = GetUInt(offset);
            return TagString(tag);
        }

        public void SetTag(string tag, uint offset)
        {
            byte[] buf = TagBytes(TagInt(tag));
            for (int i = 0; i < 4; i++) 
                _buffer[offset + i] = buf[i];
            
            _isValidChecksumAvailable = false;
        }  

        public long GetFilePos()
        {
            return _filePos;
        }

        public uint CalcChecksum()
        {
            if (!_isValidChecksumAvailable)
            {
                _cachedChecksum = CalculateChecksum();
                _isValidChecksumAvailable = true;
            }
            return _cachedChecksum;
        }

        #endregion

        #region Public Static Methods

        public static int CalcPadBytes(int nLength, int nByteAlignment)
        {
            int nPadBytes = 0;
            int nRemainderBytes = nLength % nByteAlignment;

            if (nRemainderBytes != 0)
            {
                nPadBytes = nByteAlignment - nRemainderBytes;
            }

            return nPadBytes;
        }     

        // get a short from a buffer that is storing data in MBO
        public static short GetShortBE(byte[] buf, uint offset)
        {
            return (short)(buf[offset] << 8 | buf[offset + 1]);
        }   

        public static ushort GetUShortBE(byte[] buf, uint offset)
        {
            return (ushort)(buf[offset] << 8 | buf[offset + 1]);
        }    

        public static int GetIntBE(byte[] buf, uint offset)
        {
            return buf[offset] << 24 | buf[offset + 1] << 16 | buf[offset + 2] << 8 | buf[offset + 3];
        }   

        public static uint GetUIntBE(byte[] buf, uint offset)
        {
            return (uint)(buf[offset] << 24 | buf[offset + 1] << 16 | buf[offset + 2] << 8 | buf[offset + 3]);
        }

        public static bool BinaryEqual(WoffBuffer buf1, WoffBuffer buf2)
        {
            bool bEqual = true;

            if (buf1.GetLength() != buf2.GetLength())
            {
                bEqual = false;
            }
            else
            {
                byte[] b1 = buf1.GetBuffer();
                byte[] b2 = buf2.GetBuffer();
                for (int i = 0; i < b1.Length; i++)
                {
                    if (b1[i] != b2[i])
                    {
                        bEqual = false;
                        break;
                    }
                }
            }

            return bEqual;
        }     

        public static uint TagInt(byte[] tag)
        {
            if (tag == null || tag.Length < 3)
            {
                throw new InvalidOperationException(nameof(tag));
            }
            if (tag.Length == 3)
            {
                var temp = new byte[4] { tag[0], tag[1], tag[2], 0 };
                tag = temp;
            }
            return (uint)(tag[0] << 24 | tag[1] << 16 | tag[2] << 8 | tag[3]);
        }

        public static uint TagInt(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new InvalidOperationException(nameof(tag));
            }

            string sub = tag.PadRight(4).Substring(0, 4);
            byte[] b = Encoding.ASCII.GetBytes(sub);
            return TagInt(b);
        }

        public static byte[] TagBytes(uint tag)
        {
            byte[] b = new byte[4];
            b[0] = (byte)(0xff & (tag >> 24));
            b[1] = (byte)(0xff & (tag >> 16));
            b[2] = (byte)(0xff & (tag >> 8));
            b[3] = (byte)(0xff & tag);
            return b;
        }

        public static string TagString(uint tag)
        {
            return Encoding.ASCII.GetString(TagBytes(tag));
        }

        #endregion

        #region Private Methods

        private uint CalculateChecksum()
        {
            Debug.Assert(_length != 0);

            uint sum = 0;

            uint nLongs = (_length + 3) / 4;

            for (uint i = 0; i < nLongs; i++)
            {
                sum += GetUInt(i * 4);
            }  
            return sum;
        }

        #endregion

        #region Public Stream-based Methods

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
                System.Buffer.BlockCopy(buffer, 0, trimmedBuffer, 0, bytesRead);
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
        /// Read a <c>255UInt16</c>, which is a variable-length encoding of an unsigned integer in the range 
        /// <c>0</c> to <c>65535</c> inclusive.
        /// </summary>
        /// <returns>A variable-length encoding of an unsigned short.</returns>
        /// <remarks>
        /// This data type is intended to be used as intermediate representation of various font values, 
        /// which are typically expressed as UInt16 but represent relatively small values.
        /// </remarks>
        public static ushort Read255UInt16(Stream stream)
        {
            ushort value;

            byte code = (byte)stream.ReadByte();
            if (code == WordCode)
            {
                // Read two more bytes and concatenate them to form UInt16 value
                value = (byte)stream.ReadByte();
                value <<= 8;
                value &= 0xff00;
                ushort value2 = (byte)stream.ReadByte();
                value |= (ushort)(value2 & 0x00ff);
            }
            else if (code == OneMoreByteCode1)
            {
                value = (byte)stream.ReadByte();
                value += LowestUCode;
            }
            else if (code == OneMoreByteCode2)
            {
                value = (byte)stream.ReadByte();
                value += LowestUCode * 2;
            }
            else
            {
                value = code;
            }
            return value;
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
        public static void WriteBytes(string data, Stream fileStream)
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
        public static void WriteChars(string data, Stream fileStream)
        {
            WriteBytes(data, fileStream);
        }

        #endregion

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
                   ((buffer[offset + 1] & 0xff) << 8) |
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
                   ((buffer[offset + 2] & 0xff) << 8) |
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
                   (((uint)buffer[offset + 1] & 0xff) << 8) |
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
                   (((uint)buffer[offset + 2] & 0xff) << 8) |
                   (((uint)buffer[offset + 3] & 0xff));
        }

        /// <summary>Reads a little endian 8 byte integer.</summary>
        /// <param name="buffer">The raw data buffer.</param>
        /// <param name="offset">The offset into the buffer where the long resides.</param>
        /// <returns>The long read from the buffer at the offset location.</returns>
        public static long ReadInt64LE(byte[] buffer, int offset)
        {
            return ((buffer[offset + 0] & 0xffL)) |
                   ((buffer[offset + 1] & 0xffL) << 8) |
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
            return ((buffer[offset + 7] & 0xffL)) |
                   ((buffer[offset + 6] & 0xffL) << 8) |
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
            return (((ulong)buffer[offset + 0] & 0xffL)) |
                   (((ulong)buffer[offset + 1] & 0xffL) << 8) |
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
            return (((ulong)buffer[offset + 7] & 0xffL)) |
                   (((ulong)buffer[offset + 6] & 0xffL) << 8) |
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
