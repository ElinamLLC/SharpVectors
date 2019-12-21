//------------------------------------------------------------------------------------
// .NET ZLib Implementation by Alberto M. 
// https://www.codeproject.com/Tips/830793/NET-ZLib-Implementation
//
// Distributed under The Code Project Open License (CPOL) license.
// See file LICENSE for detail or copy at https://www.codeproject.com/info/cpol10.aspx
//------------------------------------------------------------------------------------

using System;
using System.IO;
using System.IO.Compression;

namespace SharpVectors.Compressions.ZLib
{
    public sealed class ZLibStream : Stream
    {
        #region Private Fields

        private byte[] _crc;
        private bool _isLeaveOpen;
        private bool _isClosed;

        private Adler32 _adler32;

        private Stream _rawStream;
        private DeflateStream _deflateStream;

        private CompressionMode _compressionMode;
        private ZLibCompressionLevel _compressionLevel;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Inicializa una nueva instancia de la clase ZLibStream usando la secuencia y nivel de compresión especificados.
        /// </summary>
        /// <param name="stream">Secuencia que se va a comprimir</param>
        /// <param name="compressionLevel">Nivel de compresión</param>
        public ZLibStream(Stream stream, ZLibCompressionLevel compressionLevel)
            : this(stream, compressionLevel, false)
        {
        }
        /// <summary>
        /// Inicializa una nueva instancia de la clase ZLibStream usando la secuencia y modo de compresión especificados.
        /// </summary>
        /// <param name="stream">Secuencia que se va a comprimir o descomprimir</param>
        /// <param name="compressionMode">Modo de compresión</param>
        public ZLibStream(Stream stream, CompressionMode compressionMode)
            : this(stream, compressionMode, false)
        {
        }
        /// <summary>
        /// Inicializa una nueva instancia de la clase ZLibStream usando la secuencia y nivel de compresión especificados y, opcionalmente, deja la secuencia abierta.
        /// </summary>
        /// <param name="stream">Secuencia que se va a comprimir</param>
        /// <param name="compressionLevel">Nivel de compresión</param>
        /// <param name="leaveOpen">Indica si se debe de dejar la secuencia abierta después de comprimir la secuencia</param>
        public ZLibStream(Stream stream, ZLibCompressionLevel compressionLevel, bool leaveOpen)
            : this()
        {
            _compressionMode  = CompressionMode.Compress;
            _compressionLevel = compressionLevel;
            _isLeaveOpen      = leaveOpen;
            _rawStream        = stream;

            this.InitializeStream();
        }
        /// <summary>
        /// Inicializa una nueva instancia de la clase ZLibStream usando la secuencia y modo de compresión especificados y, opcionalmente, deja la secuencia abierta.
        /// </summary>
        /// <param name="stream">Secuencia que se va a comprimir o descomprimir</param>
        /// <param name="compressionMode">Modo de compresión</param>
        /// <param name="leaveOpen">Indica si se debe de dejar la secuencia abierta después de comprimir o descomprimir la secuencia</param>
        public ZLibStream(Stream stream, CompressionMode compressionMode, bool leaveOpen)
            : this()
        {
            _compressionMode  = compressionMode;
            _compressionLevel = ZLibCompressionLevel.Default;
            _isLeaveOpen      = leaveOpen;
            _rawStream        = stream;

            this.InitializeStream();
        }

        private ZLibStream()
        {
            _compressionMode  = CompressionMode.Compress;
            _compressionLevel = ZLibCompressionLevel.Faster;
            _isLeaveOpen      = false;
            _adler32          = new Adler32();

            _isClosed         = false;
            _crc              = null;
        }

        #endregion

        #region Public Overwritten Properties

        public override bool CanRead
        {
            get {
                return ((_compressionMode == CompressionMode.Decompress) && (_isClosed != true));
            }
        }

        public override bool CanWrite
        {
            get {
                return ((_compressionMode == CompressionMode.Compress) && (_isClosed != true));
            }
        }

        public override bool CanSeek
        {
            get {
                return false;
            }
        }

        public override long Length
        {
            get {
                throw new NotImplementedException();
            }
        }

        public override long Position
        {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Public Overwritten Methods

        public override int ReadByte()
        {
            int result = 0;

            if (this.CanRead == true)
            {
                result = _deflateStream.ReadByte();

                // Check if the end of the stream has been reached
                if (result == -1)
                {
                    this.ReadCRC();
                }
                else
                {
                    _adler32.Update(Convert.ToByte(result));
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = 0;

            if (this.CanRead == true)
            {
                result = _deflateStream.Read(buffer, offset, count);

                // We check if we have reached the end of the stream
                if ((result < 1) && (count > 0))
                {
                    this.ReadCRC();
                }
                else
                {
                    _adler32.Update(buffer, offset, result);
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        public override void WriteByte(byte value)
        {
            if (this.CanWrite == true)
            {
                _deflateStream.WriteByte(value);
                _adler32.Update(value);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.CanWrite == true)
            {
                _deflateStream.Write(buffer, offset, count);
                _adler32.Update(buffer, offset, count);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override void Close()
        {
            if (_isClosed == false)
            {
                _isClosed = true;

                if (_compressionMode == CompressionMode.Compress)
                {
                    this.Flush();
                    _deflateStream.Close();

                    _crc = BitConverter.GetBytes(_adler32.GetValue());

                    if (BitConverter.IsLittleEndian == true)
                    {
                        Array.Reverse(_crc);
                    }

                    _rawStream.Write(_crc, 0, _crc.Length);
                }
                else
                {
                    _deflateStream.Close();
                    if (_crc == null)
                    {
                        this.ReadCRC();
                    }
                }

                if (_isLeaveOpen == false)
                {
                    _rawStream.Close();
                }
            }
            else
            {
                if (_isLeaveOpen == false && _rawStream != null)
                {
                    _rawStream.Close();
                }
            }
            //else
            //{
            //    throw new InvalidOperationException("Stream already closed");
            //}
        }

        public override void Flush()
        {
            if (_deflateStream != null)
            {
                _deflateStream.Flush();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if the stream is in ZLib format
        /// </summary>
        /// <param name="stream">Stream to check</param>
        /// <returns>Returns <see langword="true"/> in case the stream is in ZLib format, 
        /// otherwise returns <see langword="true"/> or error.</returns>
        public static bool IsZLibStream(Stream stream)
        {
            bool bResult = false;
            int CMF = 0;
            int Flag = 0;
            ZLibHeader header;

            // We check if the sequence is in position 0, if not, we throw an exception
            if (stream.Position != 0)
            {
                throw new ArgumentOutOfRangeException("Sequence must be at position 0");
            }

            // We check if we can read the two bytes that make up the header
            if (stream.CanRead == true)
            {
                CMF = stream.ReadByte();
                Flag = stream.ReadByte();
                try
                {
                    header = ZLibHeader.DecodeHeader(CMF, Flag);
                    bResult = header.IsZLibSupported;
                }
                catch
                {
                    //Nada
                }
            }

            return bResult;
        }

        /// <summary>
        /// Read the last 4 bytes of the stream as it is where is the CRC
        /// </summary>
        private void ReadCRC()
        {
            _crc = new byte[4];
            _rawStream.Seek(-4, SeekOrigin.End);
            if (_rawStream.Read(_crc, 0, 4) < 4)
            {
                throw new EndOfStreamException();
            }

            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(_crc);
            }

            uint crcAdler = _adler32.GetValue();
            uint crcStream = BitConverter.ToUInt32(_crc, 0);

            if (crcStream != crcAdler)
            {
                throw new Exception("CRC mismatch");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize the stream
        /// </summary>
        private void InitializeStream()
        {
            switch (_compressionMode)
            {
                case CompressionMode.Compress:
                    this.InitializeZLibHeader();

                    _deflateStream = new DeflateStream(_rawStream, CompressionMode.Compress, true);
                    break;
                case CompressionMode.Decompress:
                    if (IsZLibStream(_rawStream) == false)
                    {
                        throw new InvalidDataException();
                    }

                    _deflateStream = new DeflateStream(_rawStream, CompressionMode.Decompress, true);
                    break;
            }
        }

        /// <summary>
        /// Initialize the stream header in ZLib format
        /// </summary>
        private void InitializeZLibHeader()
        {
            byte[] bytesHeader;

            // We set the header configuration
            ZLibHeader header = new ZLibHeader();

            header.CompressionMethod = 8; //Deflate
            header.CompressionInfo = 7;

            header.DictFlag = false; // No dictionary
            header.CompressionLevel = _compressionLevel;
            //switch (_compressionLevel)
            //{
            //    case CompressionLevel.NoCompression:
            //        {
            //            header.FLevel = ZLibCompressionLevel.Faster;
            //            break;
            //        }
            //    case CompressionLevel.Fastest:
            //        {
            //            header.FLevel = ZLibCompressionLevel.Default;
            //            break;
            //        }
            //    case CompressionLevel.Optimal:
            //        {
            //            header.FLevel = ZLibCompressionLevel.Optimal;
            //            break;
            //        }
            //}

            bytesHeader = header.EncodeZlibHeader();

            _rawStream.WriteByte(bytesHeader[0]);
            _rawStream.WriteByte(bytesHeader[1]);
        }

        #endregion
    }
}
