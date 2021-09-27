using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace SharpVectors.Woffs
{
    public sealed class WoffFont
    {
        private bool _isTransformed;
        private byte _woffVersion;

        private WoffHeader _woffHeader;
        private IList<WoffTable> _woffTables;
        private IList<WoffTableDirectory> _woffDirs;

        // Font Collection Only...
        private CollectionHeader _collectionHeader;
        private CollectionFontEntry _collectionEntry;

        // WOFF2 only: Target for transformation
        private int _headIndex;  // Index =  1
        private int _hheaIndex;  // Index =  2
        private int _hmtxIndex;  // Index =  3
        private int _maxpIndex;  // Index =  4
        private int _nameIndex;  // Index =  5
        private int _glyfIndex;  // Index = 10
        private int _locaIndex;  // Index = 11

        public WoffFont(WoffHeader woffHeader)
        {
            _headIndex     = -1;
            _hheaIndex     = -1;
            _hmtxIndex     = -1;
            _maxpIndex     = -1;
            _nameIndex     = -1;
            _glyfIndex     = -1;
            _locaIndex     = -1;

            _woffVersion   = byte.MaxValue;
            _isTransformed = false;

            _woffHeader    = woffHeader;
        }

        public WoffFont(WoffHeader woffHeader, CollectionHeader collHeader, CollectionFontEntry collEntry)
            : this(woffHeader)
        {
            _collectionHeader = collHeader;
            _collectionEntry  = collEntry;
        }

        public bool IsCollection
        {
            get {
                if (_woffHeader != null)
                {
                    return _woffHeader.IsCollection;
                }
                return false;
            }
        }

        public WoffHeader Header
        {
            get {
                return _woffHeader;
            }
            private set {
                _woffHeader = value;
            }
        }

        public IList<WoffTableDirectory> Directories
        {
            get {
                return _woffDirs;
            }
            private set {
                _woffDirs = value;
            }
        }

        public IList<WoffTable> Tables
        {
            get {
                return _woffTables;
            }
            private set {
                _woffTables = value;
            }
        }

        public bool IsTransformed
        {
            get {
                return _isTransformed;
            }
            private set {
                _isTransformed = value;
            }
        }

        public int HeadTableIndex
        {
            get {
                return _headIndex;
            }
        }

        public int HheaTableIndex
        {
            get {
                return _hheaIndex;
            }
        }

        public int HmtxTableIndex
        {
            get {
                return _hmtxIndex;
            }
        }

        public int MaxpTableIndex
        {
            get {
                return _maxpIndex;
            }
        }

        public int NameTableIndex
        {
            get {
                return _nameIndex;
            }
        }

        public int GlyfTableIndex
        {
            get {
                return _glyfIndex;
            }
        }

        public int LocaTableIndex
        {
            get {
                return _locaIndex;
            }
        }

        public uint ComputeTotalSfntSize()
        {
            if (_woffDirs == null || _woffDirs.Count == 0)
            {
                return 0;
            }

            var totalSfntSize = GetOrigTablesLength(_woffDirs) + GetOrigHeaderLength(_woffDirs);

            return totalSfntSize;
        }

        public void ChecksumAdjustment(byte[] headersBuffer, uint headerOffset, uint headerLength, uint tablesChecksum = 0)
        {
            Debug.Assert(_headIndex != -1);
            if (_headIndex == -1)
            {
                return;
            }
            var headTable = _woffTables[_headIndex] as WoffTableHead;
            Debug.Assert(headTable != null);
            if (headTable == null)
            {
                return;
            }

            // Compute the checksum of the font's header
            Debug.Assert(headerLength != 0);
            Debug.Assert(headersBuffer != null && headersBuffer.Length != 0);

            var headerPadding = (uint)WoffBuffer.CalcPadBytes((int)headerLength, 4);

            var buffer = headersBuffer;
            if (headerPadding > 0)
            {
                var paddedLength = headerLength + headerPadding;
                buffer = new byte[paddedLength];
                Buffer.BlockCopy(headersBuffer, 0, buffer, 0, headersBuffer.Length);
            }

            uint nLongs = (headerLength + 3) / 4;
            uint headerChecksum = 0;
            for (uint i = 0; i < nLongs; i += 4)
            {
                headerChecksum += WoffBuffer.GetUIntBE(buffer, i);
            }

            // Compute the checksum of the font by summing the checksum of the header and tables
            var fontChecksum = headerChecksum;
            if (tablesChecksum == 0)
            {
                tablesChecksum = headerChecksum;
                for (ushort i = 0; i < _woffDirs.Count; i++)
                {
                    tablesChecksum += _woffDirs[i].OrigChecksum;
                }
            }
            fontChecksum += headerChecksum;

            // Finally, update the 'head' table's checkSumAdjustment.
            headTable.CheckSumAdjustment = 0xB1B0AFBA - fontChecksum;
        }

        public bool BeginDirectory(byte woffVersion)
        {
            _headIndex = -1;
            _hheaIndex = -1;
            _hmtxIndex = -1;
            _maxpIndex = -1;
            _nameIndex = -1;
            _glyfIndex = -1;
            _locaIndex = -1;

            _woffVersion = woffVersion;
            if (_woffDirs == null)
            {
                _woffDirs = new List<WoffTableDirectory>();
            }
            if (_woffTables == null)
            {
                _woffTables = new List<WoffTable>();
            }

            return true;
        }

        public void AddDirectory(WoffTableDirectory woffDir)
        {
            if (woffDir == null)
            {
                throw new ArgumentNullException(nameof(woffDir), 
                    "The font directory object is required by the font table object.");
            }

            if (_woffVersion == WoffUtils.Woff1Version)
            {
                _woffDirs.Add(woffDir);

                if (string.Equals(woffDir.Name, "name", StringComparison.OrdinalIgnoreCase))
                {
                    _nameIndex = woffDir.WoffIndex;
                    _woffTables.Add(new WoffTableName(this, woffDir));
                }
                else
                {
                    _woffTables.Add(new WoffTable(this, woffDir));
                }

                // For WOFF1, not further processing or transformation is required.
                return;
            }

            if (woffDir.IsTransformed)
            {
                _isTransformed = true;
            }
            var tableIndex = _woffDirs.Count;
            _woffDirs.Add(woffDir);

            if (!this.IsCollection)
            {
                tableIndex = woffDir.WoffIndex;
            }

            var tagIndex = woffDir.Flags & 0x3F;
            if (_headIndex == -1 || _hheaIndex == -1)
            {
                if (_headIndex == -1 && tagIndex == WoffUtils.HeadIndex) // 1 for head
                {
                    _headIndex = tableIndex;
                }
                if (_hheaIndex == -1 && tagIndex == WoffUtils.HheaIndex) // 2 for hhea	
                {
                    _hheaIndex = tableIndex;
                }
            }
            if (_hmtxIndex == -1 || _maxpIndex == -1 || _nameIndex == -1)
            {
                if (_hmtxIndex == -1 && tagIndex == WoffUtils.HmtxIndex) // 3 for hmtx	
                {
                    _hmtxIndex = tableIndex;
                }
                if (_maxpIndex == -1 && tagIndex == WoffUtils.MaxpIndex) // 4 for hmtx	
                {
                    _maxpIndex = tableIndex;
                }
                if (_nameIndex == -1 && tagIndex == WoffUtils.NameIndex) // 5 for name
                {
                    _nameIndex = tableIndex;
                }
            }
            if (_glyfIndex == -1 || _locaIndex == -1)
            {
                if (_glyfIndex == -1 && tagIndex == WoffUtils.GlyfIndex) // 10 for glyf	
                {
                    _glyfIndex = tableIndex;
                }
                if (_locaIndex == -1 && tagIndex == WoffUtils.LocaIndex) // 11 for loca	
                {
                    _locaIndex = tableIndex;
                }
            }

            switch (tagIndex)
            {
                case WoffUtils.HeadIndex: // head = 1 
                    _woffTables.Add(new WoffTableHead(this, woffDir));
                    break;
                case WoffUtils.HheaIndex: // hhea = 2 
                    _woffTables.Add(new WoffTableHhea(this, woffDir));
                    break;
                case WoffUtils.HmtxIndex: // hmtx = 3 
                    _woffTables.Add(new WoffTableHmtx(this, woffDir));
                    break;
                case WoffUtils.MaxpIndex: // maxp = 4 
                    _woffTables.Add(new WoffTableMaxp(this, woffDir));
                    break;
                case WoffUtils.NameIndex: // name = 5
                    _woffTables.Add(new WoffTableName(this, woffDir));
                    break;
                case WoffUtils.GlyfIndex: // glyf = 10
                    _woffTables.Add(new WoffTableGlyf(this, woffDir));
                    break;
                case WoffUtils.LocaIndex: // loca = 11
                    _woffTables.Add(new WoffTableLoca(this, woffDir));
                    break;
                default:                  // All others
                    _woffTables.Add(new WoffTable(this, woffDir));
                    break;
            }
        }

        public bool EndDirectory()
        {
            // 'name' is not transformed, but may be altered by some tools to reduce size
            if (_nameIndex != -1)
            {
                _woffTables[_nameIndex].Reconstruct();
            }

            if (_woffVersion == WoffUtils.Woff1Version)
            {
                return true;
            }

            Debug.Assert(_woffTables != null && _woffTables.Count != 0);

            // If there is a transformed data, decode it.
            if (_isTransformed)
            {
                // WOFF2 expects the transformable tables to be in specified order, 
                // so sequential decoding should work...
                var tableCount = _woffTables.Count;
                for (var i = 0; i < tableCount; i++)
                {
                    if (_woffTables[i].Reconstruct() == false)
                    {
                        return false;
                    }
                }
            }

            // For collection, create a sorted indices
            if (_collectionHeader != null && _collectionEntry != null)
            {
                _collectionEntry.Initialize(_woffDirs);
            }

            return true;
        }

        private static uint Align4(uint value)
        {
            return (uint)((value + 3) & -4);
        }

        private uint GetOrigHeaderLength(IList<WoffTableDirectory> tableDirectories)
        {
            var totalLength = WoffBuffer.SizeOfUInt + (4 * WoffBuffer.SizeOfUShort)
                + ((4 * WoffBuffer.SizeOfUInt) * tableDirectories.Count);

            return (uint)totalLength;
        }

        private uint GetOrigTablesLength(IList<WoffTableDirectory> tableDirectories)
        {
            uint length = 0;
            foreach (var entry in tableDirectories)
            {
                length += entry.OrigLength;
                length = Align4(length);

            }
            return length;
        }

        public sealed class CollectionHeader
        {
            /// <summary>
            /// Font Collection ID string: 'ttcf'
            /// </summary>
            public uint TtcTag;

            /// <summary>
            /// The Version of the TTC Header in the original font.
            /// </summary>
            public uint Version; // UInt32

            /// <summary>
            /// Major version of the TTC Header.
            /// </summary>
            public ushort MajorVersion;

            /// <summary>
            /// Minor version of the TTC Header.
            /// </summary>
            public ushort MinorVersion;

            /// <summary>
            /// The number of fonts in the collection.
            /// </summary>
            public ushort NumFonts; // 255UInt16

            /// <summary>
            /// Tag indicating that a DSIG table exists, 0x44534947 ('DSIG') (null if no signature)
            /// </summary>
            public uint DsigTag;

            /// <summary>
            /// The length (in bytes) of the DSIG table (null if no signature)
            /// </summary>
            public uint DsigLength;

            /// <summary>
            /// The offset (in bytes) of the DSIG table from the beginning of the TTC file (null if no signature)
            /// </summary>
            public uint DsigOffset;

            public CollectionHeader()
            {
            }

            public bool Read(Stream stream)
            {
                int bytesRead = 0;
                this.Version = WoffBuffer.ReadUInt32BE(WoffBuffer.ReadBytes(stream,
                    WoffBuffer.SizeOfUInt, out bytesRead), 0);
                this.NumFonts = WoffBuffer.Read255UInt16(stream);

                if (this.Version == 0x00010000 || this.Version == 0x00020000)
                {
                    TtcTag            = WoffUtils.TtcSignature;
                    this.MajorVersion = (ushort)(this.Version == 0x00010000 ? 1 : 2);
                    return true;
                }

                return false;
            }

            public bool IsVersion(ushort version)
            {
                if (version == 1)
                {
                    return this.Version == 0x00010000;
                }
                if (version == 2)
                {
                    return this.Version == 0x00020000;
                }

                return false;
            }
        }

        public sealed class CollectionFontEntry
        {
            public ushort Index;

            /// <summary>
            /// The number of tables in this font
            /// </summary>
            public ushort NumTables; // 255UInt16

            /// <summary>
            /// The "sfnt version" of the font
            /// </summary>
            public uint Flavor; // UInt32

            /// <summary>
            /// The index identifying an entry in the Table Directory for each table in this font 
            /// (where the index of the first Table Directory entry is zero.)
            /// </summary>
            public ushort[] TableIndices; // 255UInt16	index[numTables]

            public ushort[] SortedIndices; // 255UInt16	index[numTables]

            public CollectionFontEntry(ushort index)
            {
                this.Index = index;
            }

            public bool Read(Stream stream, int directoryCount)
            {
                int bytesRead = 0;
                this.NumTables = WoffBuffer.Read255UInt16(stream);
                if (this.NumTables == 0)
                {
                    return false;
                }

                this.Flavor = WoffBuffer.ReadUInt32BE(WoffBuffer.ReadBytes(stream, WoffBuffer.SizeOfUInt, out bytesRead), 0);

                this.TableIndices = new ushort[this.NumTables];

                for (ushort i = 0; i < this.NumTables; i++)
                {
                    this.TableIndices[i] = WoffBuffer.Read255UInt16(stream);
                    if (this.TableIndices[i] >= directoryCount)
                    {
                        Trace.TraceError("Invalid collection font entry: " + this.TableIndices[i]);
                        return false;
                    }
                }
                return true;
            }

            public void Initialize(IList<WoffTableDirectory> woffDirs)
            {
                Debug.Assert(woffDirs != null);
                Debug.Assert(woffDirs.Count == TableIndices.Length);

                this.SortedIndices = TableIndices.Zip(woffDirs, (ind, dir)
                    => new { tag = dir.Tag, index = ind }).OrderBy(x => x.tag).Select(x => x.index).ToArray();

                //this.SortedIndices = TableIndices.Zip(woffDirs, (index, dir)
                //    => new TableIndex(dir.Tag, index)).OrderBy(x => x.Tag).Select(x => x.Index).ToArray();
            }

            //private struct TableIndex
            //{
            //    public uint Tag;
            //    public ushort Index;

            //    public TableIndex(uint tag, ushort index)
            //    {
            //        this.Tag = tag;
            //        this.Index = index;
            //    }
            //}
        }
    }
}
