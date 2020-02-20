using System;
using System.Diagnostics;

namespace SharpVectors.Woffs
{
    /// <summary>
    /// File header with basic font type and version, along with offsets to metadata and private data blocks.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/WOFF/">WOFF File Format 1.0</seealso>
    /// <seealso href="https://www.w3.org/TR/WOFF2/">WOFF File Format 2.0</seealso>
    public sealed class SvgWoffHeader : SvgWoffObject
    {
        public const uint Woff1Size     = (9 * SizeULong) + (4 * SizeUShort);  //  a 44-byte header
        public const uint Woff2Size     = (10 * SizeULong) + (4 * SizeUShort);  //  a 48-byte header

        public const uint TtfSignature  = 0x00010000;
        public const uint TtcSignature  = 0x74746366; // 'ttcf'
        public const uint OtfSignature  = 0x4F54544F; // 'OTTO'

        public const uint Woff1Signature = 0x774F4646; // 'wOFF'
        public const uint Woff2Signature = 0x774F4632; // 'wOF2'

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

        public SvgWoffHeader()
        {
            _signature = Woff1Signature;
        }

        public string Name
        {
            get {
                return StringValue(_signature);
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
                return (_flavor == TtfSignature);
            }
        }

        public bool IsCollection
        {
            get {
                return (_flavor == TtcSignature);
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
                _totalCompressedSize = value;
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

        public override uint HeaderSize
        {
            get {
                return Woff1Size;
            }
        }

        public override bool SetHeader(byte[] header)
        {
            if (header == null || (header.Length != Woff1Size && header.Length != Woff2Size))
            {
                return false;
            }

            var bufferSize = header.Length;

            base.SetHeader(header);

            _signature      = ReadUInt32BE(header, 0);
            _flavor         = ReadUInt32BE(header, 4);
            _length         = ReadUInt32BE(header, 8);

            _numTables      = ReadUInt16BE(header, 12);
            _reserved       = ReadUInt16BE(header, 14);
            if (_reserved != 0)
            {
                return false;
            }

            _totalSfntSize = ReadUInt32BE(header, 16);
            if (bufferSize == Woff1Size)
            {
                _majorVersion   = ReadUInt16BE(header, 20);
                _minorVersion   = ReadUInt16BE(header, 22);

                _metaOffset     = ReadUInt32BE(header, 24);
                _metaLength     = ReadUInt32BE(header, 28);
                _metaOrigLength = ReadUInt32BE(header, 32);
                _privateOffset  = ReadUInt32BE(header, 36);
                _privateLength  = ReadUInt32BE(header, 40);
            }
            else
            {
                _totalCompressedSize = ReadUInt32BE(header, 20);

                _majorVersion   = ReadUInt16BE(header, 24);
                _minorVersion   = ReadUInt16BE(header, 26);

                _metaOffset     = ReadUInt32BE(header, 28);
                _metaLength     = ReadUInt32BE(header, 32);
                _metaOrigLength = ReadUInt32BE(header, 36);
                _privateOffset  = ReadUInt32BE(header, 40);
                _privateLength  = ReadUInt32BE(header, 44);
            }

            // The signature field in the WOFF header MUST contain the "magic number" 0x774F4646. 
            // If the field does not contain this value, user agents MUST reject the file as invalid.
            Debug.Assert((bufferSize == Woff1Size) ? _signature == Woff1Signature : _signature == Woff2Signature);
            if (bufferSize == Woff1Size)
            {
                if (_signature != Woff1Signature)
                {
                    return false;
                }
            }
            else
            {
                if (_signature != Woff2Signature)
                {
                    return false;
                }
            }
            // The header includes a reserved field; this MUST be set to zero. 
            // If this field is non-zero, a conforming user agent MUST reject the file as invalid.
            Debug.Assert(_reserved == 0);
            if (_reserved != 0)
            {
                return false;
            }

            return true;
        }
    }
}
