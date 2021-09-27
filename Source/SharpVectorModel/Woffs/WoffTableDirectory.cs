using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace SharpVectors.Woffs
{
    public sealed class WoffTableDirectory
    {
        /// <summary>
        /// Index of the font directory in the WOFF store order.
        /// </summary>
        private ushort _woffIndex;

        /// <summary>
        /// The format version of the WOFF file. Possible values are 1 or 2.
        /// </summary>
        private byte _woffVersion;

        /// <summary>
        /// WOFF2: The table type and flags
        /// </summary>
        private byte _flags;

        /// <summary>
        /// 4-byte sfnt table identifier.
        /// </summary>
        private uint _tag;

        /// <summary>
        /// Offset to the data, from beginning of WOFF file.
        /// </summary>
        private uint _offset;
        /// <summary>
        /// Length of the compressed data, excluding padding.
        /// </summary>
        private uint _compLength;
        /// <summary>
        /// Length of the uncompressed table, excluding padding.
        /// </summary>
        private uint _origLength;

        /// <summary>
        /// WOFF2: The transformed length (if applicable)
        /// </summary>
        private uint _transformLength;

        /// <summary>
        /// Checksum of the uncompressed table.
        /// </summary>
        private uint _origChecksum;

        /// <summary>
        /// The padding to the next block.
        /// </summary>
        private uint _padding;

        private byte[] _compTable;
        private byte[] _origTable;

        public enum WoffTableState
        {
            None,
            Constructed,
            Recontructed
        }

        public enum WoffTableMode
        {
            None,
            Decoding,
            Encoding
        }

        private WoffTableMode _tableMode;
        private WoffTableState _tableState;

        public WoffTableDirectory(ushort woffIndex, byte woffVersion)
        {
            if (woffVersion != WoffUtils.Woff1Version && woffVersion != WoffUtils.Woff2Version)
            {
                throw new ArgumentException(nameof(woffVersion), "Possible values for woffVersion are 1 or 2.");
            }
            _tableMode   = WoffTableMode.Decoding;
            _tableState  = WoffTableState.None;

            _woffIndex   = woffIndex;
            _woffVersion = woffVersion;
            _flags       = byte.MaxValue;
        }

        public WoffTableDirectory(ushort woffIndex, byte woffVersion, uint offset)
            : this(woffIndex, woffVersion)
        {
            _offset = offset;
        }

        public ushort WoffIndex
        {
            get {
                return _woffIndex;
            }
        }

        public WoffTableMode WoffMode
        {
            get {
                return _tableMode;
            }
            set {
                _tableMode = value;
            }
        }

        public WoffTableState WoffState
        {
            get {
                return _tableState;
            }
            set {
                _tableState = value;
            }
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
                return WoffBuffer.TagString(_tag);
            }
        }

        public bool IsTransformed
        {
            get {
                if (_flags != byte.MaxValue)
                {
                    return (_origLength != _compLength);
//                    return ((_flags >> 5) & 0x3) == 1;
                }
                return false;
            }
        }

        /// <summary>
        /// 4-byte sfnt table identifier.
        /// </summary>
        public uint Tag
        {
            get {
                return _tag;
            }
            set {
                _tag = value;
            }
        }

        /// <summary>
        /// Offset to the data, from beginning of WOFF file.
        /// </summary>
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
        /// Length of the compressed data, excluding padding.
        /// </summary>
        public uint CompLength
        {
            get {
                return _compLength;
            }
            set {
                _compLength = value;
            }
        }

        /// <summary>
        /// Length of the uncompressed table, excluding padding.
        /// </summary>
        public uint OrigLength
        {
            get {
                return _origLength;
            }
            set {
                _origLength = value;
            }
        }

        /// <summary>
        /// Checksum of the uncompressed table.
        /// </summary>
        public uint OrigChecksum
        {
            get {
                return _origChecksum;
            }
            set {
                _origChecksum = value;
            }
        }

        public uint Padding
        {
            get {
                return _padding;
            }
            private set {
                _padding = value;
            }
        }

        public byte[] CompTable
        {
            get {
                return _compTable;
            }
            set {
                _compTable = value;
                if (value != null && _origTable == null)
                {
                    _origTable = value;
                }
            }
        }

        public byte[] OrigTable
        {
            get {
                return _origTable;
            }
            set {
                _origTable = value;
                if (value != null && _compTable == null)
                {
                    _compTable = value;
                }
            }
        }

        public byte Flags
        {
            get {
                return _flags;
            }
            set {
                _flags = value;
            }
        }

        public uint TransformLength
        {
            get {
                return _transformLength;
            }
            set {
                _transformLength = value;
            }
        }

        public uint Length
        {
            get {
                return _origLength + _padding;
            }
        }

        public bool Read(Stream stream, bool isTrueType = true)
        {
            if (stream == null)
            {
                return false;
            }
            Debug.Assert(_woffVersion == WoffUtils.Woff1Version || _woffVersion == WoffUtils.Woff2Version);

            if (_woffVersion == WoffUtils.Woff1Version)
            {
                return this.ReadData(stream);
            }

            return this.ReadData(stream, isTrueType);
        }

        private bool ReadData(Stream stream)
        {
            if (stream == null)
            {
                return false;
            }

            int bytesRead = 0;
            byte[] header = WoffBuffer.ReadBytes(stream, WoffUtils.Woff1DirSize, out bytesRead);

            _tag          = WoffBuffer.ReadUInt32BE(header, 0);
            _offset       = WoffBuffer.ReadUInt32BE(header, 4);
            _compLength   = WoffBuffer.ReadUInt32BE(header, 8);
            _origLength   = WoffBuffer.ReadUInt32BE(header, 12);
            _origChecksum = WoffBuffer.ReadUInt32BE(header, 16);

            if (_origLength > 0)
            {
                _padding = WoffUtils.CalculatePadding(_origLength);
            }

            return true;
        }

        private bool ReadData(Stream stream, bool isTrueType)
        {
            if (stream == null)
            {
                return false;
            }
            var tableFlags = WoffTable.TableFlags;

            int bytesRead = 0;
            // UInt8: flags	table type and flags
            _flags = (byte)stream.ReadByte();
            // The interpretation of the flags field is as follows:
            // 1. Bits [0..5] contain an index to the "known tag" table, which represents tags likely to appear in fonts
            int tagIndex = _flags & 0x3F;
            // If the tag is not present in this table, then the value of this bit field is 63.
            if (tagIndex >= 63)
            {
                // UInt32: tag	4-byte tag (optional)
                _tag = WoffBuffer.ReadUInt32BE(WoffBuffer.ReadBytes(stream, 4, out bytesRead), 0);
                var name = WoffBuffer.TagString(_tag);

                for (int i  = 0; i < tableFlags.Count; i++)
                {
                    if (string.Equals(name, tableFlags[i], StringComparison.Ordinal))
                    {
                        tagIndex = i;
                        break;
                    }
                }
            }
            else
            {
                _tag = WoffBuffer.TagInt(tableFlags[tagIndex]);
            }

            // 2. Bits 6 and 7 indicate the preprocessing transformation version number (0-3) that was applied to each table.
            int transformVersion = (_flags >> 5) & 0x3;

            // UIntBase128:	origLength	length of original table
            if (!WoffBuffer.ReadUIntBase128(stream, out _origLength))
            {
                return false;
            }

            // UIntBase128	transformLength	transformed length (if applicable)
            //            _transformLength = _origLength;
            _compLength = _origLength;
            _transformLength = 0;

            // For all tables in a font, except for 'glyf' and 'loca' tables, transformation version 0 
            // indicates the null transform where the original table data is passed directly to the 
            // Brotli compressor for inclusion in the compressed data stream.  
            if (transformVersion == 0)
            {
                // "glyf" : 10,  "loca" : 11
                if (tagIndex == 10 || tagIndex == 11)
                {
                    if (!WoffBuffer.ReadUIntBase128(stream, out _transformLength))
                    {
                        return false;
                    }
                    _compLength = _transformLength;
                }
            }
            // The transformation version "1", specified below, is optional and can be applied to 
            // eliminate certain redundancies in the hmtx table data.      
            if (transformVersion == 1)
            {
                // The transformation version 1 described in this subclause is optional and can only
                // be used when an input font is TrueType-flavored (i.e. has a glyf table)
                // "hmtx" : 3
                if (tagIndex == 3 && isTrueType)
                {
                    if (!WoffBuffer.ReadUIntBase128(stream, out _transformLength))
                    {
                        return false;
                    }
                    _compLength = _transformLength;
                }
            }

            if (_origLength > 0)
            {
                _padding = WoffUtils.CalculatePadding(_origLength);
            }

            return true;
        }

        public uint RecalculateChecksum()
        {
            _origChecksum = this.CalculateChecksum();

            return _origChecksum;
        }

        public uint CalculateChecksum()
        {
            Debug.Assert(_origLength != 0);
            Debug.Assert(_origTable != null && _origTable.Length != 0);

            var padBytesLength = (uint)WoffBuffer.CalcPadBytes((int)_origLength, 4);
            if (padBytesLength != 0)
            {
                _padding = padBytesLength;
            }

            var origLength = _origLength;
            var buffer = _origTable;
            if (_padding > 0)
            {
                origLength = _origLength + _padding;
                buffer = new byte[origLength];
                Buffer.BlockCopy(_origTable, 0, buffer, 0, _origTable.Length);
            }

            uint nLongs = (_origLength + 3) / 4;
            uint sum = 0;
            //for (uint i = 0; i < nLongs; i++)
            //{
            //    sum += WoffBuffer.GetUIntBE(buffer, i * 4);
            //}
            for (uint i = 0; i < nLongs; i += 4)
            {
                sum += WoffBuffer.GetUIntBE(buffer, i);
            }

            return sum;
        }

        public static IComparer<WoffTableDirectory> GetComparer()
        {
            return new WoffTableComparer();
        }

        private sealed class WoffTableComparer : IComparer<WoffTableDirectory>
        {
            public WoffTableComparer()
            {
            }

            public int Compare(WoffTableDirectory x, WoffTableDirectory y)
            {
                return x.Tag.CompareTo(y.Tag);
            }
        }
    }
}
