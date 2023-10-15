using System;
using System.IO;

namespace SharpVectors.Woffs
{
    public sealed class WoffWriter : IDisposable
    {
        private bool _isExpandable;
        private MemoryStream _buffer;
        private WoffBuffer _converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="WoffWriter"/> class with an expandable
        /// capacity initialized to zero.
        /// </summary>
        public WoffWriter()
        {
            _buffer       = new MemoryStream();
            _converter    = new WoffBuffer(WoffBuffer.SizeOfLong * 2); // 16-bytes
            _isExpandable = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WoffWriter"/> class with a non-resizable
        /// capacity initialized as specified.
        /// </summary>
        /// <param name="capacity">The initial size of the internal array in bytes.</param>
        public WoffWriter(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
            _converter = new WoffBuffer(WoffBuffer.SizeOfLong * 2); // 16-bytes

            if (capacity == 0)
            {
                _buffer       = new MemoryStream();
                _isExpandable = true;
            }
            else
            {
                _buffer = new MemoryStream(new byte[capacity], 0, capacity, true, true);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WoffWriter"/> class with a non-resizable
        /// capacity initialized as specified.
        /// </summary>
        /// <param name="capacity">The initial size of the internal array in bytes.</param>
        public WoffWriter(uint capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
            _converter = new WoffBuffer(WoffBuffer.SizeOfLong * 2); // 16-bytes

            if (capacity == 0)
            {
                _buffer       = new MemoryStream();
                _isExpandable = true;
            }
            else
            {
                _buffer = new MemoryStream(new byte[capacity], 0, (int)capacity, true, true);
            }
        }

        /// <summary>
        ///  Initializes a new non-resizable instance of the <see cref="WoffWriter"/> class
        ///  based on the specified byte array.
        /// </summary>
        /// <param name="buffer">The array of unsigned bytes from which to create the current stream.</param>
        public WoffWriter(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "ArgumentNull_Buffer");
            }
            _converter = new WoffBuffer(WoffBuffer.SizeOfLong * 2); // 16-bytes

            if (buffer.Length == 0)
            {
                _buffer       = new MemoryStream();
                _isExpandable = true;
            }
            else
            {
                _buffer = new MemoryStream(buffer, 0, buffer.Length, true, true);
            }
        }

        /// <summary>
        /// Initializes a new non-resizable instance of the <see cref="WoffWriter"/> class
        /// based on the specified region (index) of a byte array.
        /// </summary>
        /// <param name="buffer">The array of unsigned bytes from which to create this stream.</param>
        /// <param name="index">The index into buffer at which the stream begins.</param>
        /// <param name="count">The length of the stream in bytes.</param>
        public WoffWriter(byte[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "ArgumentNull_Buffer");
            }
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "ArgumentOutOfRange_NeedNonNegNum");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "ArgumentOutOfRange_NeedNonNegNum");
            if (buffer.Length - index < count)
                throw new ArgumentException("Argument_InvalidOffLen");

            _converter = new WoffBuffer(WoffBuffer.SizeOfLong * 2); // 16-bytes

            if (buffer.Length == 0)
            {
                _buffer       = new MemoryStream();
                _isExpandable = true;
            }
            else
            {
                _buffer = new MemoryStream(buffer, index, count, true, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ~WoffWriter()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets or sets the current position within the stream.
        /// </summary>
        /// <value>The current position within the stream.</value>
        public uint Offset
        {
            get {
                if (_buffer != null && _buffer.CanWrite)
                {
                    return (uint)_buffer.Position;
                }
                return 0;
            }
            set {
                if (_buffer != null && _buffer.CanWrite)
                {
                    if (value != _buffer.Position)
                    {
                        _buffer.Position = value; 
                    }
                }
            }
        }

        /// <summary>
        ///  Gets the length of the stream in bytes.
        /// </summary>
        /// <value>The length of the stream in bytes.</value>
        public uint Length
        {
            get {
                if (_buffer != null && _buffer.CanWrite)
                {
                    return (uint)_buffer.Length;
                }
                return 0;
            }
        }

        /// <summary>
        /// Returns the array of unsigned bytes from which this stream was created.
        /// </summary>
        /// <returns>
        /// The byte array from which this stream was created, or the underlying array if
        /// a byte array was not provided to the System.IO.MemoryStream constructor during
        /// construction of the current instance.
        /// </returns>
        public byte[] GetBuffer()
        {
            if (_buffer != null && _buffer.CanWrite)
            {
                if (_isExpandable)
                {
                    return _buffer.ToArray();
                }
                return _buffer.GetBuffer();
            }
            return null;
        }

        /// <summary>
        /// Writes a two-byte signed integer to the current stream and advances the stream
        /// position by two bytes.
        /// </summary>
        /// <param name="value">The two-byte signed integer to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool WriteInt16(short value)
        {
            if (_buffer != null && _buffer.CanWrite)
            {
                _converter.SetShort(value, 0);
                _buffer.Write(_converter.Buffer, 0, WoffBuffer.SizeOfShort);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes a two-byte unsigned integer to the current stream and advances the stream
        /// position by two bytes.
        /// </summary>
        /// <param name="value">The two-byte unsigned integer to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool WriteUInt16(ushort value)
        {
            if (_buffer != null && _buffer.CanWrite)
            {
                _converter.SetUShort(value, 0);
                _buffer.Write(_converter.Buffer, 0, WoffBuffer.SizeOfUShort);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes a two-byte signed integer to the current stream and advances the stream
        /// position by two bytes.
        /// </summary>
        /// <param name="value">The two-byte signed integer to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool WriteInt16(int value)
        {
            if (_buffer != null && _buffer.CanWrite)
            {
                _converter.SetShort(value, 0);
                _buffer.Write(_converter.Buffer, 0, WoffBuffer.SizeOfShort);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes a four-byte signed integer to the current stream and advances the stream
        /// position by four bytes.
        /// </summary>
        /// <param name="value">The four-byte signed integer to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool WriteInt32(int value)
        {
            if (_buffer != null && _buffer.CanWrite)
            {
                _converter.SetInt(value, 0);
                _buffer.Write(_converter.Buffer, 0, WoffBuffer.SizeOfInt);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes a four-byte unsigned integer to the current stream and advances the stream
        /// position by four bytes.
        /// </summary>
        /// <param name="value">The four-byte unsigned integer to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool WriteUInt32(uint value)
        {
            if (_buffer != null && _buffer.CanWrite)
            {
                _converter.SetUInt(value, 0);
                _buffer.Write(_converter.Buffer, 0, WoffBuffer.SizeOfUInt);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes an eight-byte signed integer to the current stream and advances the stream
        /// position by eight bytes.
        /// </summary>
        /// <param name="value">The eight-byte signed integer to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool WriteInt64(long value)
        {
            if (_buffer != null && _buffer.CanWrite)
            {
                _converter.SetLong(value, 0);
                _buffer.Write(_converter.Buffer, 0, WoffBuffer.SizeOfLong);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes an eight-byte unsigned integer to the current stream and advances the
        /// stream position by eight bytes.
        /// </summary>
        /// <param name="value">The eight-byte unsigned integer to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool WriteUInt64(ulong value)
        {
            if (_buffer != null && _buffer.CanWrite)
            {
                _converter.SetULong(value, 0);
                _buffer.Write(_converter.Buffer, 0, WoffBuffer.SizeOfULong);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes a signed byte to the current stream and advances the stream position by one byte.
        /// </summary>
        /// <param name="value">The signed byte to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool Write(sbyte value)
        {
            if (_buffer != null && _buffer.CanWrite)
            {
                _converter.SetSbyte(value, 0);
                _buffer.Write(_converter.Buffer, 0, WoffBuffer.SizeOfByte);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes a signed byte array to the underlying stream.
        /// </summary>
        /// <param name="buffer">A signed byte array containing the data to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool Write(sbyte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return false;
            }

            if (_buffer != null && _buffer.CanWrite)
            {
                var bufferSize = buffer.Length;
                var updatedbuffer = new byte[bufferSize];
                for (int i = 0; i < bufferSize; i++)
                {
                    updatedbuffer[i] = unchecked((byte)buffer[i]);
                }
                _buffer.Write(updatedbuffer, 0, bufferSize);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes a region of a signed byte array to the current stream.
        /// </summary>
        /// <param name="buffer">A signed byte array containing the data to write.</param>
        /// <param name="index">The starting point in buffer at which to begin writing.</param>
        /// <param name="count">The number of signed bytes to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool Write(sbyte[] buffer, int index, int count)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return false;
            }

            if (_buffer != null && _buffer.CanWrite)
            {
                var updatedbuffer = new byte[count];
                for (int i = 0; i < count; i++)
                {
                    updatedbuffer[i] = unchecked((byte)buffer[index + i]);
                }
                _buffer.Write(updatedbuffer, 0, count);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes an unsigned byte to the current stream and advances the stream position by one byte.
        /// </summary>
        /// <param name="value">The unsigned byte to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool Write(byte value)
        {
            if (_buffer != null && _buffer.CanWrite)
            {
                _converter.SetByte(value, 0);
                _buffer.Write(_converter.Buffer, 0, WoffBuffer.SizeOfByte);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes a byte array to the underlying stream.
        /// </summary>
        /// <param name="buffer">A byte array containing the data to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool Write(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return false;
            }

            if (_buffer != null && _buffer.CanWrite)
            {
                _buffer.Write(buffer, 0, buffer.Length);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes a region of a byte array to the current stream.
        /// </summary>
        /// <param name="buffer">A byte array containing the data to write.</param>
        /// <param name="index">The starting point in buffer at which to begin writing.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool Write(byte[] buffer, int index, int count)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return false;
            }

            if (_buffer != null && _buffer.CanWrite)
            {
                _buffer.Write(buffer, index, count);
                return true;
            }
            return false;
        }

        #region IDisposable Members

        private bool _isDisposed; // To detect redundant calls

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (_buffer != null)
                    {
                        _buffer.Dispose();
                        _buffer = null;
                    }
                }
                _isDisposed = true;
            }
        }

        #endregion
    }
}
