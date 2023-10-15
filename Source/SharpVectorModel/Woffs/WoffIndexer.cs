using System;

namespace SharpVectors.Woffs
{
    /// <summary>
    /// This is a writer wrapper, providing index access to the buffer (usually a fixed buffer).
    /// </summary>
    public sealed class WoffIndexer : IDisposable
    {
        private WoffWriter _writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WoffIndexer"/> class with an expandable
        /// capacity initialized to zero.
        /// </summary>
        public WoffIndexer()
        {
            _writer = new WoffWriter();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WoffIndexer"/> class with a non-resizable
        /// capacity initialized as specified.
        /// </summary>
        /// <param name="capacity">The initial size of the internal array in bytes.</param>
        public WoffIndexer(int capacity)
        {
            _writer = new WoffWriter(capacity);
        }

        /// <summary>
        ///  Initializes a new non-resizable instance of the <see cref="WoffIndexer"/> class
        ///  based on the specified byte array.
        /// </summary>
        /// <param name="buffer">The array of unsigned bytes from which to create the current stream.</param>
        public WoffIndexer(byte[] buffer)
        {
            _writer = new WoffWriter(buffer);
        }

        /// <summary>
        /// Initializes a new non-resizable instance of the <see cref="WoffIndexer"/> class
        /// based on the specified region (index) of a byte array.
        /// </summary>
        /// <param name="buffer">The array of unsigned bytes from which to create this stream.</param>
        /// <param name="index">The index into buffer at which the stream begins.</param>
        /// <param name="count">The length of the stream in bytes.</param>
        public WoffIndexer(byte[] buffer, int index, int count)
        {
            _writer = new WoffWriter(buffer, index, count);
        }

        /// <summary>
        /// 
        /// </summary>
        ~WoffIndexer()
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
                if (_writer != null)
                {
                    return _writer.Offset;
                }
                return 0;
            }
            set {
                if (_writer != null && value != uint.MaxValue)
                {
                    _writer.Offset = value;
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
                if (_writer != null)
                {
                    return _writer.Length;
                }
                return 0;
            }
        }

        public byte this[ushort index]
        {
            get {
                if (_writer != null)
                {
                    var buffer = _writer.GetBuffer();
                    return buffer[index];
                }
                throw new InvalidOperationException();
            }
            set {
                if (_writer != null)
                {
                    _writer.Offset = index;
                    _writer.Write(value);
                }
            }
        }

        public byte this[int index]
        {
            get {
                if (_writer != null)
                {
                    var buffer = _writer.GetBuffer();
                    return buffer[index];
                }
                throw new InvalidOperationException();
            }
            set {
                if (_writer != null)
                {
                    _writer.Offset = (uint)index;
                    _writer.Write(value);
                }
            }
        }

        public byte this[uint index]
        {
            get {
                if (_writer != null)
                {
                    var buffer = _writer.GetBuffer();
                    return buffer[index];
                }
                throw new InvalidOperationException();
            }
            set {
                if (_writer != null)
                {
                    _writer.Offset = index;
                    _writer.Write(value);
                }
            }
        }

        public byte this[short index]
        {
            get {
                if (_writer != null)
                {
                    var buffer = _writer.GetBuffer();
                    return buffer[index];
                }
                throw new InvalidOperationException();
            }
            set {
                if (_writer != null)
                {
                    _writer.Offset = (uint)index;
                    _writer.Write(value);
                }
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
            if (_writer != null)
            {
                return _writer.GetBuffer();
            }
            return null;
        }

        /// <summary>
        /// Writes an eight-byte signed integer to the current stream and advances the stream
        /// position by eight bytes.
        /// </summary>
        /// <param name="value">The eight-byte signed integer to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool WriteInt64(long value, ref uint offset)
        {
            this.Offset = offset;

            if (_writer != null && _writer.WriteInt64(value))
            {
                offset = _writer.Offset;
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
        public bool WriteUInt64(ulong value, ref uint offset)
        {
            this.Offset = offset;

            if (_writer != null && _writer.WriteUInt64(value))
            {
                offset = _writer.Offset;
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
        public bool WriteInt32(int value, ref uint offset)
        {
            this.Offset = offset;

            if (_writer != null && _writer.WriteInt32(value))
            {
                offset = _writer.Offset;
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
        public bool WriteUInt32(uint value, ref uint offset)
        {
            this.Offset = offset;

            if (_writer != null && _writer.WriteUInt32(value))
            {
                offset = _writer.Offset;
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
        public bool WriteInt16(short value, ref uint offset)
        {
            this.Offset = offset;

            if (_writer != null && _writer.WriteInt16(value))
            {
                offset = _writer.Offset;
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
        public bool WriteInt16(int value, ref uint offset)
        {
            this.Offset = offset;

            if (_writer != null && _writer.WriteInt16(value))
            {
                offset = _writer.Offset;
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
        public bool WriteUInt16(ushort value, ref uint offset)
        {
            this.Offset = offset;

            if (_writer != null && _writer.WriteUInt16(value))
            {
                offset = _writer.Offset;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes a signed byte to the current stream and advances the stream position by one byte.
        /// </summary>
        /// <param name="value">The signed byte to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool Write(sbyte value, ref uint offset)
        {
            this.Offset = offset;

            if (_writer != null && _writer.Write(value))
            {
                offset = _writer.Offset;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes a signed byte array to the underlying stream.
        /// </summary>
        /// <param name="buffer">A signed byte array containing the data to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool Write(sbyte[] buffer, ref uint offset)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return false;
            }

            this.Offset = offset;

            if (_writer != null && _writer.Write(buffer))
            {
                offset = _writer.Offset;
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
        public bool Write(sbyte[] buffer, int index, int count, ref uint offset)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return false;
            }

            this.Offset = offset;

            if (_writer != null && _writer.Write(buffer, index, count))
            {
                offset = _writer.Offset;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes an unsigned byte to the current stream and advances the stream position by one byte.
        /// </summary>
        /// <param name="value">The unsigned byte to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool Write(byte value, ref uint offset)
        {
            this.Offset = offset;

            if (_writer != null && _writer.Write(value))
            {
                offset = _writer.Offset;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes a byte array to the underlying stream.
        /// </summary>
        /// <param name="buffer">A byte array containing the data to write.</param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise, returns <see langword="false"/>.</returns>
        public bool Write(byte[] buffer, ref uint offset)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return false;
            }

            this.Offset = offset;

            if (_writer != null && _writer.Write(buffer))
            {
                offset = _writer.Offset;
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
        public bool Write(byte[] buffer, int index, int count, ref uint offset)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return false;
            }

            this.Offset = offset;

            if (_writer != null && _writer.Write(buffer, index, count))
            {
                offset = _writer.Offset;
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
                    if (_writer != null)
                    {
                        _writer.Dispose();
                        _writer = null;
                    }
                }
                _isDisposed = true;
            }
        }

        #endregion
    }
}
