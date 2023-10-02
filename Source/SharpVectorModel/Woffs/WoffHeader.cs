using System;
using System.IO;
using System.Diagnostics;

namespace SharpVectors.Woffs
{
    /// <summary>
    /// File header with basic font type and version, along with offsets to metadata and private data blocks.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/WOFF/">WOFF File Format 1.0</seealso>
    /// <seealso href="https://www.w3.org/TR/WOFF2/">WOFF File Format 2.0</seealso>
    public sealed class WoffHeader
    {
        /// <summary>
        /// The Woff 1 or 2 signature
        /// </summary>
        private uint _signature;

        /// <summary>
        /// The "sfnt version" of the input font.
        /// </summary>
        private uint _flavor;

        /// <summary>
        /// Total size of the WOFF file.
        /// </summary>
        private uint _length;

        /// <summary>
        /// Number of entries in directory of font tables.
        /// </summary>
        private ushort _numTables;

        /// <summary>
        /// Reserved; set to zero.
        /// </summary>
        private ushort _reserved;

        /// <summary>
        /// Total size needed for the uncompressed font data, including the sfnt header, 
        /// directory, and font tables (including padding).
        /// </summary>
        private uint _totalSfntSize;

        /// <summary>
        /// WOFF2 only: Total length of the compressed data block.
        /// </summary>
        private uint _totalCompressedSize;

        /// <summary>
        /// Major version of the WOFF file.
        /// </summary>
        private ushort _majorVersion;

        /// <summary>
        /// Minor version of the WOFF file.
        /// </summary>
        private ushort _minorVersion;

        /// <summary>
        /// Offset to metadata block, from beginning of WOFF file.
        /// </summary>
        private uint _metaOffset;

        /// <summary>
        /// Length of compressed metadata block.
        /// </summary>
        private uint _metaLength;

        /// <summary>
        /// Uncompressed size of metadata block.
        /// </summary>
        private uint _metaOrigLength;

        /// <summary>
        /// Offset to private data block, from beginning of WOFF file.
        /// </summary>
        private uint _privateOffset;

        /// <summary>
        /// Length of private data block.
        /// </summary>
        private uint _privateLength;

        // Extra information

        /// <summary>
        /// The format version of the WOFF file. Possible values are 1 or 2.
        /// </summary>
        private byte _woffVersion;

        public WoffHeader(byte woffVersion)
        {
            if (woffVersion != WoffUtils.Woff1Version && woffVersion != WoffUtils.Woff2Version)
            {
                throw new ArgumentException(nameof(woffVersion), "Possible values for woffVersion are 1 or 2.");
            }

            _woffVersion = woffVersion;
            _signature   = _woffVersion == WoffUtils.Woff1Version 
                ? WoffUtils.Woff1Signature : WoffUtils.Woff2Signature;
        }

        /// <summary>
        /// Gets the <c>W3C</c> Specifications Format Version of the WOFF file.
        /// </summary>
        /// <value>Possible values are <c>1</c> or <c>2</c>, for the Web Open Font Format 1 (WOFF1) and 2 (WOFF2).</value>
        public byte WoffVersion
        {
            get {
                return _woffVersion;
            }
        }

        public string Name
        {
            get {
                return WoffBuffer.TagString(_signature);
            }
        }

        public bool HasMetadata
        {
            get {
                return _metaOffset > 0 && _metaLength != 0;
            }
        }

        public bool HasPrivateData
        {
            get {
                return _privateOffset > 0 && _privateLength != 0;
            }
        }

        public bool IsTrueType
        {
            get {
                return (_flavor == WoffUtils.TtfSignature);
            }
        }

        public bool IsCollection
        {
            get {
                return (_flavor == WoffUtils.TtcSignature);
            }
        }

        /// <summary>
        /// Gets or set the signature field of the WOFF format header. The value is <c>0x774F4646 or 'wOFF'</c>.
        /// </summary>
        /// <value>
        /// The signature field in the WOFF header must contain the "magic number" <c>0x774F4646</c>. 
        /// If the field does not contain this value, user agents MUST reject the file as invalid.
        /// </value>
        public uint Signature
        {
            get {
                return _signature;
            }
            set {
                _signature = value;
            }
        }

        /// <summary>
        /// The "sfnt version" of the input font.
        /// </summary>
        public uint Flavor
        {
            get {
                return _flavor;
            }
            set {
                _flavor = value;
            }
        }

        /// <summary>
        /// Total size of the WOFF file.
        /// </summary>
        public uint Length
        {
            get {
                return _length;
            }
            set {
                _length = value;
            }
        }

        /// <summary>
        /// Number of entries in directory of font tables.
        /// </summary>
        public ushort NumTables
        {
            get {
                return _numTables;
            }
            set {
                _numTables = value;
            }
        }

        /// <summary>
        /// Reserved; set to zero.
        /// </summary>
        public ushort Reserved
        {
            get {
                return _reserved;
            }
            set {
                _reserved = value;
            }
        }

        /// <summary>
        /// Total size needed for the uncompressed font data, including the sfnt header, 
        /// directory, and font tables (including padding).
        /// </summary>
        /// <remarks>
        /// <para>WOFF 2</para>
        /// <para>
        /// The <see cref="TotalSfntSize"/> value in the WOFF2 Header is intended to be used for 
        /// reference purposes only. It may represent the size of the uncompressed input font file, 
        /// but if the transformed 'glyf' and 'loca' tables are present, the uncompressed size of 
        /// the reconstructed tables and the total decompressed font size may differ substantially 
        /// from the original total size specified in the WOFF2 Header.
        /// </para>
        /// </remarks>
        public uint TotalSfntSize
        {
            get {
                return _totalSfntSize;
            }
            set {
                _totalSfntSize = value;
            }
        }

        public uint TotalCompressedSize
        {
            get {
                return _totalCompressedSize;
            }
            set {
                this._totalCompressedSize = value;
            }
        }

        /// <summary>
        /// Major version of the WOFF file.
        /// </summary>
        public ushort MajorVersion
        {
            get {
                return _majorVersion;
            }
            set {
                _majorVersion = value;
            }
        }

        /// <summary>
        /// Minor version of the WOFF file.
        /// </summary>
        public ushort MinorVersion
        {
            get {
                return _minorVersion;
            }
            set {
                _minorVersion = value;
            }
        }

        /// <summary>
        /// Offset to metadata block, from beginning of WOFF file.
        /// </summary>
        public uint MetaOffset
        {
            get {
                return _metaOffset;
            }
            set {
                _metaOffset = value;
            }
        }

        /// <summary>
        /// Length of compressed metadata block.
        /// </summary>
        public uint MetaLength
        {
            get {
                return _metaLength;
            }
            set {
                _metaLength = value;
            }
        }

        /// <summary>
        /// Uncompressed size of metadata block.
        /// </summary>
        public uint MetaOrigLength
        {
            get {
                return _metaOrigLength;
            }
            set {
                _metaOrigLength = value;
            }
        }

        /// <summary>
        /// Offset to private data block, from beginning of WOFF file.
        /// </summary>
        public uint PrivateOffset
        {
            get {
                return _privateOffset;
            }
            set {
                _privateOffset = value;
            }
        }

        /// <summary>
        /// Length of private data block.
        /// </summary>
        public uint PrivateLength
        {
            get {
                return _privateLength;
            }
            set {
                _privateLength = value;
            }
        }

        public ushort HeaderSize
        {
            get {
                return _woffVersion == WoffUtils.Woff1Version 
                    ? WoffUtils.Woff1HeaderSize : WoffUtils.Woff2HeaderSize;
            }
        }

        public bool Read(Stream stream)
        {
            if (stream == null)
            {
                return false;
            }
            Debug.Assert(_woffVersion == WoffUtils.Woff1Version || _woffVersion == WoffUtils.Woff2Version);

            var headerSize = this.HeaderSize;

            var header = new byte[headerSize];

            var sizeRead = stream.Read(header, 0, headerSize);
            Debug.Assert(sizeRead == headerSize);

            var bufferSize = header.Length;

            _signature      = WoffBuffer.ReadUInt32BE(header, 0);
            _flavor         = WoffBuffer.ReadUInt32BE(header, 4);
            _length         = WoffBuffer.ReadUInt32BE(header, 8);

            _numTables      = WoffBuffer.ReadUInt16BE(header, 12);
            _reserved       = WoffBuffer.ReadUInt16BE(header, 14);

            _totalSfntSize = WoffBuffer.ReadUInt32BE(header, 16);
            if (_woffVersion == WoffUtils.Woff1Version)
            {
                _majorVersion   = WoffBuffer.ReadUInt16BE(header, 20);
                _minorVersion   = WoffBuffer.ReadUInt16BE(header, 22);

                _metaOffset     = WoffBuffer.ReadUInt32BE(header, 24);
                _metaLength     = WoffBuffer.ReadUInt32BE(header, 28);
                _metaOrigLength = WoffBuffer.ReadUInt32BE(header, 32);
                _privateOffset  = WoffBuffer.ReadUInt32BE(header, 36);
                _privateLength  = WoffBuffer.ReadUInt32BE(header, 40);
            }
            else
            {
                _totalCompressedSize = WoffBuffer.ReadUInt32BE(header, 20);

                _majorVersion   = WoffBuffer.ReadUInt16BE(header, 24);
                _minorVersion   = WoffBuffer.ReadUInt16BE(header, 26);

                _metaOffset     = WoffBuffer.ReadUInt32BE(header, 28);
                _metaLength     = WoffBuffer.ReadUInt32BE(header, 32);
                _metaOrigLength = WoffBuffer.ReadUInt32BE(header, 36);
                _privateOffset  = WoffBuffer.ReadUInt32BE(header, 40);
                _privateLength  = WoffBuffer.ReadUInt32BE(header, 44);
            }

            // The signature field in the WOFF-1 header must contain the "magic number" 0x774F4646. 
            // If the field does not contain this value, user agents must reject the file as invalid.
            //Debug.Assert((bufferSize == WoffUtils.Woff1HeaderSize)
            //    ? _signature == WoffUtils.Woff1Signature : _signature == WoffUtils.Woff2Signature);
            if (bufferSize == WoffUtils.Woff1HeaderSize)
            {
                if (_signature != WoffUtils.Woff1Signature)
                {
                    return false;
                }
            }
            else if (bufferSize == WoffUtils.Woff2HeaderSize)
            {
                if (_signature != WoffUtils.Woff2Signature)
                {
                    return false;
                }
            }

            // The header includes a reserved field; this must be set to zero. 
            // If this field is non-zero, a conforming user agent must reject the file as invalid.
            Debug.Assert(_reserved == 0);

            return true;
        }
    }
}
