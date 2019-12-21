using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace SharpVectors.Woffs
{
    public sealed class SvgWoffTableDirectory : SvgWoffObject
    {
        public const uint Woff1Size = 5 * SizeULong;

        private readonly static IList<string> _tableFlags;

        /// <summary>
        /// The byte boundary.
        /// </summary>
        private const uint PaddingSize = 4;

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

        static SvgWoffTableDirectory()
        {
            _tableFlags = new List<string>(64);
            _tableFlags.Add("cmap"); //0	
            _tableFlags.Add("head"); //1	
            _tableFlags.Add("hhea"); //2	
            _tableFlags.Add("hmtx"); //3	
            _tableFlags.Add("maxp"); //4	
            _tableFlags.Add("name"); //5	
            _tableFlags.Add("OS/2"); //6	
            _tableFlags.Add("post"); //7	
            _tableFlags.Add("cvt "); //8	 
            _tableFlags.Add("fpgm"); //9	
            _tableFlags.Add("glyf"); //10	
            _tableFlags.Add("loca"); //11	
            _tableFlags.Add("prep"); //12	
            _tableFlags.Add("CFF "); //13	 
            _tableFlags.Add("VORG"); //14	
            _tableFlags.Add("EBDT"); //15
            _tableFlags.Add("EBLC"); //16	
            _tableFlags.Add("gasp"); //17	
            _tableFlags.Add("hdmx"); //18	
            _tableFlags.Add("kern"); //19	
            _tableFlags.Add("LTSH"); //20	
            _tableFlags.Add("PCLT"); //21	
            _tableFlags.Add("VDMX"); //22	
            _tableFlags.Add("vhea"); //23	
            _tableFlags.Add("vmtx"); //24	
            _tableFlags.Add("BASE"); //25	
            _tableFlags.Add("GDEF"); //26	
            _tableFlags.Add("GPOS"); //27	
            _tableFlags.Add("GSUB"); //28	
            _tableFlags.Add("EBSC"); //29	
            _tableFlags.Add("JSTF"); //30	
            _tableFlags.Add("MATH"); //31
            _tableFlags.Add("CBDT"); //32		
            _tableFlags.Add("CBLC"); //33		
            _tableFlags.Add("COLR"); //34		
            _tableFlags.Add("CPAL"); //35		
            _tableFlags.Add("SVG "); //36		    
            _tableFlags.Add("sbix"); //37		
            _tableFlags.Add("acnt"); //38		
            _tableFlags.Add("avar"); //39		
            _tableFlags.Add("bdat"); //40		
            _tableFlags.Add("bloc"); //41		
            _tableFlags.Add("bsln"); //42		
            _tableFlags.Add("cvar"); //43		
            _tableFlags.Add("fdsc"); //44		
            _tableFlags.Add("feat"); //45		
            _tableFlags.Add("fmtx"); //46		
            _tableFlags.Add("fvar"); //47
            _tableFlags.Add("gvar"); //48		
            _tableFlags.Add("hsty"); //49	
            _tableFlags.Add("just"); //50	
            _tableFlags.Add("lcar"); //51	
            _tableFlags.Add("mort"); //52	
            _tableFlags.Add("morx"); //53	
            _tableFlags.Add("opbd"); //54	
            _tableFlags.Add("prop"); //55	
            _tableFlags.Add("trak"); //56	
            _tableFlags.Add("Zapf"); //57	
            _tableFlags.Add("Silf"); //58	
            _tableFlags.Add("Glat"); //59	
            _tableFlags.Add("Gloc"); //60	
            _tableFlags.Add("Feat"); //61	
            _tableFlags.Add("Sill"); //62	
            _tableFlags.Add("    "); //63 : arbitrary tag follows	
        }

        public SvgWoffTableDirectory()
        {
            _flags = byte.MaxValue;
        }

        public string Name
        {
            get {
                return StringValue(_tag);
            }
        }

        public bool IsTransformed
        {
            get {
                if (_flags != byte.MaxValue)
                {
                    return (_origLength != _transformLength);
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

        public override uint HeaderSize
        {
            get {
                return Woff1Size;
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

        public bool SetHeader(Stream stream, bool isTrueType)
        {
            if (stream == null)
            {
                return false;
            }

            int bytesRead = 0;
            // UInt8: flags	table type and flags
            _flags = (byte)stream.ReadByte();
            // The interpretation of the flags field is as follows:
            // 1. Bits [0..5] contain an index to the "known tag" table, which represents tags likely to appear in fonts
            int tagIndex = _flags & 0x1F;
            // If the tag is not present in this table, then the value of this bit field is 63.
            if (tagIndex >= 63)
            {
                // UInt32: tag	4-byte tag (optional)
                _tag = ReadUInt32BE(ReadBytes(stream, 4, out bytesRead), 0);
                var name = StringValue(_tag);

                for (int i  = 0; i < _tableFlags.Count; i++)
                {
                    if (string.Equals(name, _tableFlags[i], StringComparison.Ordinal))
                    {
                        tagIndex = i;
                        break;
                    }
                }
            }
            else
            {
                _tag = IntValue(_tableFlags[tagIndex]);
            }

            // 2. Bits 6 and 7 indicate the preprocessing transformation version number (0-3) that was applied to each table.
            int transformVersion = (_flags >> 5) & 0x3;

            // UIntBase128:	origLength	length of original table
            if (!ReadUIntBase128(stream, out _origLength))
            {
                return false;
            }

            // UIntBase128	transformLength	transformed length (if applicable)
            _transformLength = _origLength;

            // For all tables in a font, except for 'glyf' and 'loca' tables, transformation version 0 
            // indicates the null transform where the original table data is passed directly to the 
            // Brotli compressor for inclusion in the compressed data stream.  
            if (transformVersion == 0)
            {
                // "glyf" : 10,  "loca" : 11
                if (tagIndex == 10 || tagIndex == 11)
                {
                    if (!ReadUIntBase128(stream, out _transformLength))
                    {
                        return false;
                    }
                }
            }
            // The transformation version "1", specified below, is optional and can be applied to 
            // eliminate certain redundancies in the hmtx table data.      
            if (transformVersion == 1)
            {
                // The transformation version 1 described in this subclause is optional and can only
                // be used when an input font is TrueType-flavoured (i.e. has a glyf table)
                // "hmtx" : 3
                if (tagIndex == 3 && isTrueType)
                {
                    if (!ReadUIntBase128(stream, out _transformLength))
                    {
                        return false;
                    }
                }
            }

            if (_origLength > 0)
            {
                _padding = CalculatePadding(_origLength);
            }

            return true;
        }

        public override bool SetHeader(byte[] header)
        {
            if (header == null || header.Length != Woff1Size)
            {
                return false;
            }

            base.SetHeader(header);

            _tag          = ReadUInt32BE(header, 0);
            _offset       = ReadUInt32BE(header, 4);
            _compLength   = ReadUInt32BE(header, 8);
            _origLength   = ReadUInt32BE(header, 12);
            _origChecksum = ReadUInt32BE(header, 16);

            if (_origLength > 0)
            {
                _padding = CalculatePadding(_origLength);
            }

            return true;
        }

        public uint CalculateChecksum()
        {
            Debug.Assert(_origLength != 0);
            Debug.Assert(_origTable != null && _origTable.Length != 0);

            var buffer = new byte[_origLength + _padding];
            Buffer.BlockCopy(_origTable, 0, buffer, 0, _origTable.Length);

            uint sum = 0;

            uint nLongs = (_origLength + 3) / 4;

            for (uint i = 0; i < nLongs; i++)
            {
                sum += GetUInt(buffer, i * 4);
            }
            return sum;
        }

        private static uint CalculatePadding(uint length)
        {
            uint mod = length % PaddingSize;
            return (mod == 0) ? 0 : PaddingSize - mod;
        }

        private static uint GetUInt(byte[] buffer, uint offset)
        {
            return (uint)(
                buffer[offset + 0] << 24 |
                buffer[offset + 1] << 16 |
                buffer[offset + 2] << 8  |
                buffer[offset + 3]);
        }

    }
}
