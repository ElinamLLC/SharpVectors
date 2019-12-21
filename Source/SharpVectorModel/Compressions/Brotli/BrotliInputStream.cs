//------------------------------------------------------------------------------------
// Copyright 2015 Google Inc. All Rights Reserved. 
//
// Distributed under MIT license.
// See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
//------------------------------------------------------------------------------------

using System;
using System.IO;

namespace SharpVectors.Compressions.Brotli
{
    /// <summary>
    /// A <see cref="Stream"/> decorator that decompresses brotli data.
    /// <para> Not thread-safe.</para>
    /// </summary>
    public class BrotliInputStream : Stream
    {
        public const int DefaultInternalBufferSize = 16384;

        /// <summary>Internal buffer used for efficient byte-by-byte reading.</summary>
        private byte[] _buffer;

        /// <summary>Number of decoded but still unused bytes in internal buffer.</summary>
        private int _remainingBufferBytes;

        /// <summary>Next unused byte offset.</summary>
        private int _bufferOffset;

        /// <summary>Decoder state.</summary>
        private readonly BrotliState _state = new BrotliState();

        /// <summary>
        /// Creates a <see cref="Stream"/> wrapper that decompresses brotli data.
        /// <para> For byte-by-byte reading (<see cref="ReadByte()"/>) internal buffer with
        /// <see cref="DefaultInternalBufferSize"/> size is allocated and used.</para>
        /// <para> Will block the thread until first kilobyte of data of source is available.</para>
        /// </summary>
        /// <param name="source">underlying data source</param>
        /// <exception cref="IOException">in case of corrupted data or source stream problems</exception>
        public BrotliInputStream(Stream source)
            : this(source, DefaultInternalBufferSize, null)
        {
        }

        /// <summary>
        /// Creates a <see cref="Stream"/> wrapper that decompresses brotli data.
        /// <para> For byte-by-byte reading (<see cref="ReadByte()"/>) internal buffer of specified size is
        /// allocated and used.</para>
        /// <para> Will block the thread until first kilobyte of data of source is available.</para>
        /// </summary>
        /// <param name="source">compressed data source</param>
        /// <param name="byteReadBufferSize">size of internal buffer used in case of byte-by-byte reading</param>
        /// <exception cref="IOException">in case of corrupted data or source stream problems</exception>
        public BrotliInputStream(Stream source, int byteReadBufferSize)
            : this(source, byteReadBufferSize, null)
        {
        }

        /// <summary>
        /// Creates a <see cref="Stream"/> wrapper that decompresses brotli data.
        /// <para> For byte-by-byte reading (<see cref="ReadByte()"/>) internal buffer of specified size is
        /// allocated and used.</para>
        /// <para> Will block the thread until first kilobyte of data of source is available.</para>
        /// </summary>
        /// <param name="source">compressed data source</param>
        /// <param name="byteReadBufferSize">size of internal buffer used in case of byte-by-byte reading</param>
        /// <param name="customDictionary">custom dictionary data; <see langword="null"/> if not used</param>
        /// <exception cref="IOException">in case of corrupted data or source stream problems</exception>
        public BrotliInputStream(Stream source, int byteReadBufferSize, byte[] customDictionary)
        {
            if (byteReadBufferSize <= 0)
            {
                throw new ArgumentException("Bad buffer size:" + byteReadBufferSize);
            }
            else if (source == null)
            {
                throw new ArgumentException("source is null");
            }
            _buffer = new byte[byteReadBufferSize];
            _remainingBufferBytes = 0;
            _bufferOffset = 0;
            try
            {
                BrotliState.SetInput(_state, source);
            }
            catch (BrotliRuntimeException ex)
            {
                throw new IOException("Brotli decoder initialization failed", ex);
            }
            if (customDictionary != null && customDictionary.Length != 0)
            {
                BrotliDecode.SetCustomDictionary(_state, customDictionary);
            }
        }

        /// <summary><inheritDoc/></summary>
        /// <exception cref="IOException"/>
        public override void Close()
        {
            BrotliState.Close(_state);
        }

        /// <summary><inheritDoc/></summary>
        /// <exception cref="IOException"/>
        public override int ReadByte()
        {
            if (_bufferOffset >= _remainingBufferBytes)
            {
                _remainingBufferBytes = Read(_buffer, 0, _buffer.Length);
                _bufferOffset = 0;
                if (_remainingBufferBytes == -1)
                {
                    return -1;
                }
            }
            return _buffer[_bufferOffset++] & unchecked((int)(0xFF));
        }

        /// <summary><inheritDoc/></summary>
        /// <exception cref="IOException"/>
        public override int Read(byte[] destBuffer, int destOffset, int destLen)
        {
            if (destOffset < 0)
            {
                throw new ArgumentException("Bad offset: " + destOffset);
            }
            else if (destLen < 0)
            {
                throw new ArgumentException("Bad length: " + destLen);
            }
            else if (destOffset + destLen > destBuffer.Length)
            {
                throw new ArgumentException("Buffer overflow: " + (destOffset + destLen) + " > " + destBuffer.Length);
            }
            else if (destLen == 0)
            {
                return 0;
            }
            int copyLen = Math.Max(_remainingBufferBytes - _bufferOffset, 0);
            if (copyLen != 0)
            {
                copyLen = Math.Min(copyLen, destLen);
                Array.Copy(_buffer, _bufferOffset, destBuffer, destOffset, copyLen);
                _bufferOffset += copyLen;
                destOffset += copyLen;
                destLen -= copyLen;
                if (destLen == 0)
                {
                    return copyLen;
                }
            }
            try
            {
                _state.output       = destBuffer;
                _state.outputOffset = destOffset;
                _state.outputLength = destLen;
                _state.outputUsed   = 0;
                BrotliDecode.Decompress(_state);
                if (_state.outputUsed == 0)
                {
                    return -1;
                }
                return _state.outputUsed + copyLen;
            }
            catch (BrotliRuntimeException ex)
            {
                throw new IOException("Brotli stream decoding failed", ex);
            }
        }

        // <{[INJECTED CODE]}>
        /// <summary><inheritDoc/></summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary><inheritDoc/></summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary><inheritDoc/></summary>
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary><inheritDoc/></summary>
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary><inheritDoc/></summary>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary><inheritDoc/></summary>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary><inheritDoc/></summary>
        public override bool CanWrite { get { return false; } }

        /// <summary><inheritDoc/></summary>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset,
                int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        /// <summary><inheritDoc/></summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary><inheritDoc/></summary>
        public override void Flush() { }
    }
}
