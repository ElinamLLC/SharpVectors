using System;
using System.Diagnostics;

namespace SharpVectors.Woffs
{
    public sealed class WoffReader
    {
        private uint _offset;
        private WoffBuffer _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WoffReader"/> class with the specified bytes stream.
        /// </summary>
        /// <param name="buffer">A object containing the bytes stream.</param>
        public WoffReader(WoffBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "The buffer parameter is required and cannot be null.");
            }
            _buffer = buffer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WoffReader"/> class with the specified bytes stream.
        /// </summary>
        /// <param name="buffer">A object containing the bytes stream.</param>
        public WoffReader(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "The buffer parameter is required and cannot be null.");
            }
            _buffer = new WoffBuffer(buffer);
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <value>The current position within the stream.</value>
        public uint Offset
        {
            get {
                return _offset;
            }
            set {
                _offset = value;
            }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <value>A signed integer value representing the length of the stream in bytes.</value>
        public uint Length
        {
            get {
                if (_buffer != null)
                {
                    return _buffer.GetLength();
                }
                return 0;
            }
        }

        public WoffBuffer Buffer
        {
            get {
                return _buffer;
            }
        }

        public WoffReader GetReader(uint offset)
        {
            var reader = new WoffReader(_buffer);
            reader.Offset = offset;

            return reader;
        }

        /// <summary>
        ///  Reads a unsigned byte from this stream and advances the current position of the
        ///  stream by one byte.
        /// </summary>
        /// <returns>A unsigned byte read from the current stream.</returns>
        public byte ReadByte()
        {
            Debug.Assert(_buffer != null);
            var valRead = _buffer.GetByte(_offset);
            _offset += WoffBuffer.SizeOfByte;

            return valRead;
        }

        /// <summary>
        ///  Reads a signed byte from this stream and advances the current position of the
        ///  stream by one byte.
        /// </summary>
        /// <returns>A signed byte read from the current stream.</returns>
        public sbyte ReadSByte()
        {
            Debug.Assert(_buffer != null);
            var valRead = _buffer.GetSbyte(_offset);
            _offset += WoffBuffer.SizeOfByte;

            return valRead;
        }

        /// <summary>
        ///  Reads a 2-byte signed integer from the current stream and advances the current
        ///  position of the stream by two bytes.
        /// </summary>
        /// <returns>A 2-byte signed integer read from the current stream.</returns>
        public short ReadInt16()
        {
            Debug.Assert(_buffer != null);
            var valRead = _buffer.GetShort(_offset);
            _offset += WoffBuffer.SizeOfShort;

            return valRead;
        }

        /// <summary>
        ///  Reads a 2-byte unsigned integer from the current stream using little-endian encoding
        ///  and advances the position of the stream by two bytes.
        /// </summary>
        /// <returns>A 2-byte unsigned integer read from this stream.</returns>
        public ushort ReadUInt16()
        {
            Debug.Assert(_buffer != null);
            var valRead = _buffer.GetUShort(_offset);
            _offset += WoffBuffer.SizeOfUShort;

            return valRead;
        }

        /// <summary>
        ///  Reads a 4-byte signed integer from the current stream and advances the current
        ///  position of the stream by four bytes.
        /// </summary>
        /// <returns>A 4-byte signed integer read from the current stream.</returns>
        public int ReadInt32()
        {
            Debug.Assert(_buffer != null);
            var valRead = _buffer.GetInt(_offset);
            _offset += WoffBuffer.SizeOfInt;

            return valRead;
        }

        /// <summary>
        ///  Reads a 4-byte unsigned integer from the current stream and advances the position
        ///  of the stream by four bytes.
        /// </summary>
        /// <returns>A 4-byte unsigned integer read from this stream.</returns>
        public uint ReadUInt32()
        {
            Debug.Assert(_buffer != null);
            var valRead = _buffer.GetUInt(_offset);
            _offset += WoffBuffer.SizeOfUInt;

            return valRead;
        }

        /// <summary>
        ///  Reads an 8-byte signed integer from the current stream and advances the current
        ///  position of the stream by eight bytes.
        /// </summary>
        /// <returns>An 8-byte signed integer read from the current stream.</returns>
        public long ReadInt64()
        {
            Debug.Assert(_buffer != null);
            var valRead = _buffer.GetLong(_offset);
            _offset += WoffBuffer.SizeOfLong;

            return valRead;
        }

        /// <summary>
        ///  Reads an 8-byte unsigned integer from the current stream and advances the position
        ///  of the stream by eight bytes.
        /// </summary>
        /// <returns> An 8-byte unsigned integer read from this stream.</returns>
        public ulong ReadUInt64()
        {
            Debug.Assert(_buffer != null);
            var valRead = _buffer.GetULong(_offset);
            _offset += WoffBuffer.SizeOfULong;

            return valRead;
        }

        public byte[] ReadByte(uint arraySize)
        {
            var buffer = _buffer.GetBytes(_offset, arraySize);
            _offset += WoffBuffer.SizeOfByte * arraySize;

            return buffer;
        }

        public sbyte[] ReadSByte(uint arraySize)
        {
            sbyte[] buffer = new sbyte[arraySize];
            for (int i = 0; i < arraySize; ++i)
            {
                buffer[i] = this.ReadSByte();
            }
            return buffer;
        }

        public short[] ReadInt16(uint arraySize)
        {
            short[] arr = new short[arraySize];
            for (int i = 0; i < arraySize; ++i)
            {
                arr[i] = this.ReadInt16();
            }
            return arr;
        }

        public ushort[] ReadUInt16(uint arraySize)
        {
            ushort[] arr = new ushort[arraySize];
            for (int i = 0; i < arraySize; ++i)
            {
                arr[i] = this.ReadUInt16();
            }
            return arr;
        }

        public int[] ReadInt32(uint arraySize)
        {
            int[] arr = new int[arraySize];
            for (int i = 0; i < arraySize; ++i)
            {
                arr[i] = this.ReadInt32();
            }
            return arr;
        }

        public uint[] ReadUInt32(uint arraySize)
        {
            uint[] arr = new uint[arraySize];
            for (int i = 0; i < arraySize; ++i)
            {
                arr[i] = this.ReadUInt32();
            }
            return arr;
        }

        public ushort[] Read255UInt16(uint arraySize)
        {
            ushort[] arr = new ushort[arraySize];
            for (int i = 0; i < arraySize; ++i)
            {
                arr[i] = this.Read255UInt16();
            }
            return arr;
        }

        public float ReadFixed()
        {
            Debug.Assert(_buffer != null);
            var valRead = _buffer.GetInt(_offset);
            _offset += WoffBuffer.SizeOfInt;

            return valRead / 65536F;
        }

        public short ReadFword()
        {
            Debug.Assert(_buffer != null);
            var valRead = _buffer.GetShort(_offset);
            _offset += WoffBuffer.SizeOfShort;

            return valRead;
        }

        public float ReadF2Dot14()
        {
            Debug.Assert(_buffer != null);
            var valRead = _buffer.GetShort(_offset);
            _offset += WoffBuffer.SizeOfShort;

            return (float)valRead / 16384;
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
        public bool ReadUIntBase128(out uint result)
        {
            // The "C-like" pseudo-code describing how to read the UIntBase128 format is presented below:
            result = 0;
            uint accum = 0;

            for (ushort i = 0; i < 5; i++)
            {
                int dataByte = this.ReadByte();

                // Leading zeros are invalid.
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
        public ushort Read255UInt16()
        {
            ushort value;

            byte code = this.ReadByte();
            if (code == WoffBuffer.WordCode)
            {
                // Read two more bytes and concatenate them to form UInt16 value
                value = this.ReadByte();
                value <<= 8;
                value &= 0xff00;
                ushort value2 = this.ReadByte();
                value |= (ushort)(value2 & 0x00ff);
            }
            else if (code == WoffBuffer.OneMoreByteCode1)
            {
                value = this.ReadByte();
                value += WoffBuffer.LowestUCode;
            }
            else if (code == WoffBuffer.OneMoreByteCode2)
            {
                value = this.ReadByte();
                value += WoffBuffer.LowestUCode * 2;
            }
            else
            {
                value = code;
            }
            return value;
        }

        public byte[] ReadByte(uint arraySize, uint offset)
        {
            _offset = offset;
            return ReadByte(arraySize);
        }

        public sbyte[] ReadSByte(uint arraySize, uint offset)
        {
            _offset = offset;
            return ReadSByte(arraySize);
        }

        public short[] ReadInt16(uint arraySize, uint offset)
        {
            _offset = offset;
            return ReadInt16(arraySize);
        }

        public ushort[] ReadUInt16(uint arraySize, uint offset)
        {
            _offset = offset;
            return ReadUInt16(arraySize);
        }

        public int[] ReadInt32(uint arraySize, uint offset)
        {
            _offset = offset;
            return ReadInt32(arraySize);
        }

        public uint[] ReadUInt32(uint arraySize, uint offset)
        {
            _offset = offset;
            return ReadUInt32(arraySize);
        }

        public ushort[] Read255UInt16(uint arraySize, uint offset)
        {
            _offset = offset;
            return Read255UInt16(arraySize);
        }

        public bool SkipInt16()
        {
            _offset += WoffBuffer.SizeOfShort;
            return (_offset <= _buffer.Length);
        }

        public bool SkipUInt16()
        {
            _offset += WoffBuffer.SizeOfUShort;
            return (_offset <= _buffer.Length);
        }

        public bool SkipInt32()
        {
            _offset += WoffBuffer.SizeOfInt;
            return (_offset <= _buffer.Length);
        }

        public bool SkipUInt32()
        {
            _offset += WoffBuffer.SizeOfUInt;
            return (_offset <= _buffer.Length);
        }

        public bool Skip()
        {
            return Skip(1);
        }

        public bool Skip(ushort numBytes)
        {
            _offset += numBytes;
            return (_offset <= _buffer.Length);
        }
    }
}
