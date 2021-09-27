using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

using SharpVectors.Compressions.ZLib;
using SharpVectors.Compressions.Brotli;

namespace SharpVectors.Woffs
{
    using CollectionHeader    = WoffFont.CollectionHeader;
    using CollectionFontEntry = WoffFont.CollectionFontEntry;

    public sealed class WoffDecoder
    {
        #region Private Fields

        // Offsets within the main directory
        private const uint HeaderSize = 12;

        // Offsets within a specific table record
        private const uint TableSize = 16;

        private bool _isTransformed;
        private byte _woffVersion;

        private WoffMetadata _metadata;
        private WoffPrivateData _privateData;

        private WoffHeader _woffHeader;
        private IList<WoffTableDirectory> _woffDirs;

        private IList<WoffFont> _woffFonts;

        // Collection Font Header
        private CollectionHeader _collectionHeader;

        // Collection Font Entry
        private IList<CollectionFontEntry> _collectionEntries;

        #endregion

        #region Constructors and Destructor

        public WoffDecoder()
        {
            _woffFonts = new List<WoffFont>();
        }

        #endregion

        #region Public Properties

        public WoffMetadata Metadata
        {
            get {
                return _metadata;
            }
            private set {
                _metadata = value;
            }
        }

        public WoffPrivateData PrivateData
        {
            get {
                return _privateData;
            }
            private set {
                _privateData = value;
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

        public bool IsTransformed
        {
            get {
                return _isTransformed;
            }
            private set {
                _isTransformed = value;
            }
        }

        public bool IsCollection
        {
            get {
                if (_collectionHeader == null || _collectionEntries == null || _collectionEntries.Count == 0)
                {
                    return false;
                }
                return true; // Not checking number of fonts, single font collection is possible!
            }
        }

        #endregion

        #region Public Methods

        public bool ReadFont(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }
            var fileExt = Path.GetExtension(filePath);
            if (string.IsNullOrWhiteSpace(fileExt))
            {
                return false;
            }

            if (!string.Equals(fileExt, WoffUtils.Woff1FileExt, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(fileExt, WoffUtils.Woff2FileExt, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            using (var stream = File.OpenRead(filePath))
            {
                if (string.Equals(fileExt, WoffUtils.Woff1FileExt, StringComparison.OrdinalIgnoreCase))
                {
                    return ReadWoff1(stream);
                }
                if (string.Equals(fileExt, WoffUtils.Woff2FileExt, StringComparison.OrdinalIgnoreCase))
                {
                    return ReadWoff2(stream);
                }
            }

            return false;
        }

        public bool WriteFont(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            using (var stream = File.OpenWrite(filePath))
            {
                return this.WriteFont(stream);
            }
        }

        public bool WriteFont(Stream stream)
        {
            if (_woffVersion == 1)
            {
                return WriteWoff1(stream);
            }
            if (_woffVersion == 2)
            {
                return WriteWoff2(stream);
            }

            return false;
        }

        #endregion

        #region Private WOFF1 Methods

        private bool ReadWoff1(Stream stream)
        {
            _woffVersion = WoffUtils.Woff1Version;

            var woffFont = new WoffFont(_woffHeader);
            _woffFonts.Add(woffFont);
            woffFont.BeginDirectory(_woffVersion);

            _woffHeader = new WoffHeader(WoffUtils.Woff1Version);
            _woffDirs = null;

            int sizeRead = 0;
            sizeRead = _woffHeader.HeaderSize;

            Debug.Assert(sizeRead == WoffUtils.Woff1HeaderSize);

            if (!_woffHeader.Read(stream))
            {
                return false;
            }

            _woffDirs = new List<WoffTableDirectory>(_woffHeader.NumTables);

            // 2. TableDirectory: Directory of font tables, containing size and other info.  
            for (ushort i = 0; i < _woffHeader.NumTables; i++)
            {
                var woffDir = new WoffTableDirectory(i, _woffVersion);
                if (woffDir.Read(stream))
                {
                    _woffDirs.Add(woffDir);

                    woffFont.AddDirectory(woffDir);
                }
            }

            for (int i = 0; i < _woffHeader.NumTables; i++)
            {
                var woffDir = _woffDirs[i];

                stream.Seek(woffDir.Offset, SeekOrigin.Begin);

                int bytesRead = 0;
                int bytesCount = (int)woffDir.CompLength;
                if (bytesCount == 0)
                {
                    continue;
                }
                var tableBuffer = WoffBuffer.ReadBytes(stream, bytesCount, out bytesRead);

                Debug.Assert(bytesRead == bytesCount);

                if (bytesRead != bytesCount)
                {
                    return false;
                }

                woffDir.CompTable = tableBuffer;
                if (woffDir.CompLength == woffDir.OrigLength)
                {
                    // table data is not compressed
                    woffDir.OrigTable = tableBuffer;
                }
                else
                {
                    bytesCount = (int)woffDir.OrigLength;
                    var origBuffer = new byte[bytesCount];

                    using (var zlibStream = new ZLibStream(new MemoryStream(tableBuffer),
                        CompressionMode.Decompress, false))
                    {
                        int bytesStart = 0;

                        do
                        {
                            bytesRead = zlibStream.Read(origBuffer, bytesStart, bytesCount);
                            if (bytesRead == 0)
                                break;
                            bytesStart += bytesRead;
                            bytesCount -= bytesRead;
                        } while (bytesCount > 0);
                    }

                    woffDir.OrigTable = origBuffer;
                }
            }

            _metadata = new WoffMetadata(_woffHeader.MetaOffset, 
                _woffHeader.MetaLength, _woffHeader.MetaOrigLength);
            if (_woffHeader.HasMetadata)
            {
                stream.Seek(_woffHeader.MetaOffset, SeekOrigin.Begin);

                int bytesRead = 0;
                int bytesCount = (int)_woffHeader.MetaLength;

                var metaBuffer = WoffBuffer.ReadBytes(stream, bytesCount, out bytesRead);

                Debug.Assert(bytesRead == bytesCount);

                if (bytesRead != bytesCount)
                {
                    return false;
                }

                _metadata.Data     = metaBuffer;
                _metadata.OrigData = metaBuffer;

                if (_woffHeader.MetaLength != _woffHeader.MetaOrigLength)
                {
                    bytesCount = (int)_woffHeader.MetaOrigLength;
                    var origBuffer = new byte[bytesCount];

                    using (var zlibStream = new ZLibStream(new MemoryStream(metaBuffer),
                        CompressionMode.Decompress, false))
                    {
                        int bytesStart = 0;

                        do
                        {
                            bytesRead = zlibStream.Read(origBuffer, bytesStart, bytesCount);
                            if (bytesRead == 0)
                                break;
                            bytesStart += bytesRead;
                            bytesCount -= bytesRead;
                        } while (bytesCount > 0);
                    }

                    _metadata.OrigData = origBuffer;
                }
            }

            _privateData = new WoffPrivateData(_woffHeader.PrivateOffset, _woffHeader.PrivateLength);
            if (_woffHeader.HasPrivateData)
            {
                stream.Seek(_woffHeader.PrivateOffset, SeekOrigin.Begin);

                int bytesRead = 0;
                int bytesCount = (int)_woffHeader.PrivateLength;

                var privateBuffer = WoffBuffer.ReadBytes(stream, bytesCount, out bytesRead);

                Debug.Assert(bytesRead == bytesCount);

                if (bytesRead != bytesCount)
                {
                    return false;
                }

                _privateData.Data = privateBuffer;
            }

            woffFont.EndDirectory();

            return true;
        }

        private bool WriteWoff1(Stream stream)
        {
            if (stream == null || _woffHeader == null || _woffDirs == null || _woffDirs.Count == 0)
            {
                return false;
            }

            var numTables = _woffHeader.NumTables;
            var searchRange   = (ushort)(WoffUtils.MaxPower2LE(numTables) * TableSize);
            var entrySelector = WoffUtils.Log2(WoffUtils.MaxPower2LE(numTables));
            var rangeShift    = (ushort)(numTables * TableSize - searchRange);

            int offset = 0;
            // Write the font offset table...
            var headerBuffer = new byte[HeaderSize];
            // uint32:	sfntVersion	0x00010000 or 0x4F54544F ('OTTO')
            offset += WoffBuffer.WriteUInt32BE(headerBuffer, offset, _woffHeader.Flavor);
            // uint16:	numTables	Number of tables.
            offset += WoffBuffer.WriteUInt16BE(headerBuffer, offset, _woffHeader.NumTables);
            // uint16	searchRange	(Maximum power of 2 <= numTables) x 16.
            offset += WoffBuffer.WriteUInt16BE(headerBuffer, offset, searchRange);
            // uint16:	entrySelector	Log2(maximum power of 2 <= numTables).
            offset += WoffBuffer.WriteUInt16BE(headerBuffer, offset, entrySelector);
            // uint16:	rangeShift	NumTables x 16-searchRange.
            offset += WoffBuffer.WriteUInt16BE(headerBuffer, offset, rangeShift);

            stream.Write(headerBuffer, 0, (int)HeaderSize);

            uint tablesOffset = HeaderSize + numTables * TableSize;

            var tableBuffer = new byte[TableSize];
            for (int i = 0; i < numTables; i++)
            {
                offset = 0;
                var woffDir = _woffDirs[i];
                // Tag:	tableTag	Table identifier.
                offset += WoffBuffer.WriteUInt32BE(tableBuffer, offset, woffDir.Tag);
                // uint32:	checkSum	CheckSum for this table.
                offset += WoffBuffer.WriteUInt32BE(tableBuffer, offset, woffDir.OrigChecksum);
                // Offset32	offset	Offset from beginning of TrueType font file.
                offset += WoffBuffer.WriteUInt32BE(tableBuffer, offset, tablesOffset);
                // uint32	length	Length of this table.
                offset += WoffBuffer.WriteUInt32BE(tableBuffer, offset, woffDir.OrigLength);

                stream.Write(tableBuffer, 0, (int)TableSize);

                tablesOffset += woffDir.OrigLength;
                if (tablesOffset % 4 != 0)
                {
                    tablesOffset += 4 - (tablesOffset % 4);
                }
            }

            var paddingBuffer = new byte[4];

            for (int i = 0; i < numTables; i++)
            {
                var woffDir = _woffDirs[i];

                stream.Write(woffDir.OrigTable, 0, (int)woffDir.OrigLength);
                stream.Write(paddingBuffer, 0, (int)woffDir.Padding);
            }

            return true;
        }

        #endregion

        #region Private WOFF2 Methods

        private bool ReadWoff2(Stream stream)
        {
            _woffVersion = WoffUtils.Woff2Version;

            uint sizeRead = 0;

            // 1. WOFF2Header: File header with basic font type and version, along with offsets 
            //    to metadata and private data blocks.
            _woffHeader = new WoffHeader(WoffUtils.Woff2Version);
            _woffDirs = null;

            sizeRead = _woffHeader.HeaderSize;

            Debug.Assert(sizeRead == WoffUtils.Woff2HeaderSize);

            if (!_woffHeader.Read(stream))
            {
                return false;
            }

            // 2. TableDirectory: Directory of font tables, containing size and other info.  
            uint offset = 0;
            _woffDirs = new List<WoffTableDirectory>(_woffHeader.NumTables);
            for (ushort i = 0; i < _woffHeader.NumTables; i++)
            {
                var woffDir = new WoffTableDirectory(i, _woffVersion, offset);

                if (woffDir.Read(stream, _woffHeader.IsTrueType))
                {
                    if (woffDir.IsTransformed)
                    {
                        _isTransformed = true;
                    }
                    _woffDirs.Add(woffDir);

                    offset += woffDir.CompLength;
                }
            }

            // 3. CollectionDirectory: An optional table containing the font fragment descriptions 
            //    of font collection entries.
            if (_woffHeader.IsCollection)
            {
                var isLoaded = this.ReadWoff2Fonts(stream);
            }
            else
            {
                var isLoaded = this.ReadWoff2Font(stream);
            }

            // 5. ExtendedMetadata: An optional block of extended metadata, represented in XML format 
            //    and compressed for storage in the WOFF2 file.
            _metadata = new WoffMetadata(_woffHeader.MetaOffset,
                _woffHeader.MetaLength, _woffHeader.MetaOrigLength);
            if (_woffHeader.HasMetadata)
            {
                stream.Seek(_woffHeader.MetaOffset, SeekOrigin.Begin);

                int bytesRead = 0;
                int bytesCount = (int)_woffHeader.MetaLength;

                var metaBuffer = WoffBuffer.ReadBytes(stream, bytesCount, out bytesRead);

                Debug.Assert(bytesRead == bytesCount);

                if (bytesRead != bytesCount)
                {
                    return false;
                }

                _metadata.Data     = metaBuffer;
                _metadata.OrigData = metaBuffer;

                if (_woffHeader.MetaLength != _woffHeader.MetaOrigLength)
                {
                    bytesCount = (int)_woffHeader.MetaOrigLength;

                    using (var brotliStream = new BrotliInputStream(new MemoryStream(metaBuffer)))
                    {
                        var origBuffer = WoffBuffer.ReadBytes(brotliStream, bytesCount, out bytesRead);

                        Debug.Assert(bytesRead == bytesCount);

                        if (bytesRead != bytesCount)
                        {
                            return false;
                        }

                        _metadata.OrigData = origBuffer;
                    }
                }
            }

            // 6. PrivateData: An optional block of private data for the font designer, foundry, or vendor to use.  
            _privateData = new WoffPrivateData(_woffHeader.PrivateOffset, _woffHeader.PrivateLength);
            if (_woffHeader.HasPrivateData)
            {
                stream.Seek(_woffHeader.PrivateOffset, SeekOrigin.Begin);

                int bytesRead = 0;
                int bytesCount = (int)_woffHeader.PrivateLength;

                var privateBuffer = WoffBuffer.ReadBytes(stream, bytesCount, out bytesRead);

                Debug.Assert(bytesRead == bytesCount);

                if (bytesRead != bytesCount)
                {
                    return false;
                }

                _privateData.Data = privateBuffer;
            }

            return true;
        }

        private bool ReadWoff2Font(Stream stream)
        {
            var woffFont = new WoffFont(_woffHeader);
            _woffFonts.Add(woffFont);
            woffFont.BeginDirectory(_woffVersion);

            // 2. TableDirectory: Directory of font tables, containing size and other info.  
            for (ushort i = 0; i < _woffHeader.NumTables; i++)
            {
                var woffDir = _woffDirs[i];

                woffFont.AddDirectory(woffDir);
            }

            // 4. CompressedFontData: Contents of font tables, compressed for storage in the WOFF2 file. 
            int bytesRead = 0;
            int bytesCount = (int)_woffHeader.TotalCompressedSize;
            if (bytesCount != 0)
            {
                byte[] compressedBuffer = WoffBuffer.ReadBytes(stream, bytesCount, out bytesRead);
                Debug.Assert(bytesRead == bytesCount);
                if (bytesRead != bytesCount)
                {
                    return false;
                }

                bool errorOccurred = false;

                var memoryStream = new MemoryStream(compressedBuffer);

                using (var brotliStream = new BrotliInputStream(memoryStream))
                {
                    var decompressedStream = new MemoryStream();
                    brotliStream.CopyTo(decompressedStream);
                    decompressedStream.Seek(0, SeekOrigin.Begin);

                    for (int i = 0; i < _woffHeader.NumTables; i++)
                    {
                        var woffDir = _woffDirs[i];

                        try
                        {
                            // bytesCount = (int)woffDir.TransformLength;
                            bytesCount = (int)woffDir.CompLength;

                            decompressedStream.Seek(woffDir.Offset, SeekOrigin.Begin);

                            var tableBuffer = WoffBuffer.ReadBytes(decompressedStream, bytesCount, out bytesRead);

                            Debug.Assert(bytesRead == bytesCount);

                            if (bytesRead != bytesCount)
                            {
                                return false;
                            }

                            woffDir.CompTable = tableBuffer;
                            woffDir.OrigTable = tableBuffer;
                        }
                        catch (Exception ex)
                        {
                            errorOccurred = true;
                            Trace.TraceError(ex.Message);
                        }
                    }
                }

                if (errorOccurred)
                {
                    return false;
                }
            }

            woffFont.EndDirectory();
            return true;
        }

        private bool ReadWoff2Fonts(Stream stream)
        {
            // 1. Read the collection font header
            _collectionHeader = new CollectionHeader();
            if (_collectionHeader.Read(stream) == false)
            {
                Trace.TraceError("Collection Font Error: Reading of collection header failed.");
                return false;
            }

            // 2. Read this collection font entries
            var numFonts = _collectionHeader.NumFonts;
            _collectionEntries = new List<CollectionFontEntry>(numFonts);
            for (ushort i = 0; i < numFonts; i++)
            {
                var collectionEntry = new CollectionFontEntry(i);
                if (collectionEntry.Read(stream, _woffDirs.Count) == false)
                {
                    return false;
                }
                _collectionEntries.Add(collectionEntry);
            }

            // 3. TableDirectory: Directory of font tables, containing size and other info.  
            for (ushort i = 0; i < numFonts; i++)
            {
                var collectionEntry = _collectionEntries[i];

                var woffFont = new WoffFont(_woffHeader, _collectionHeader, collectionEntry);
                _woffFonts.Add(woffFont);
                woffFont.BeginDirectory(_woffVersion);

                for (ushort j = 0; j < collectionEntry.NumTables; j++)
                {
                    var index = collectionEntry.TableIndices[j];

                    var woffDir = _woffDirs[index];

                    woffFont.AddDirectory(woffDir);
                }
            }

            // 4. CompressedFontData: Contents of font tables, compressed for storage in the WOFF2 file. 
            int bytesRead = 0;
            int bytesCount = (int)_woffHeader.TotalCompressedSize;
            if (bytesCount != 0)
            {
                byte[] compressedBuffer = WoffBuffer.ReadBytes(stream, bytesCount, out bytesRead);
                Debug.Assert(bytesRead == bytesCount);
                if (bytesRead != bytesCount)
                {
                    return false;
                }

                bool errorOccurred = false;

                var memoryStream = new MemoryStream(compressedBuffer);

                using (var brotliStream = new BrotliInputStream(memoryStream))
                {
                    var decompressedStream = new MemoryStream();
                    brotliStream.CopyTo(decompressedStream);
                    decompressedStream.Seek(0, SeekOrigin.Begin);

                    for (int i = 0; i < _woffHeader.NumTables; i++)
                    {
                        var woffDir = _woffDirs[i];

                        try
                        {
                            // bytesCount = (int)woffDir.TransformLength;
                            bytesCount = (int)woffDir.CompLength;

                            decompressedStream.Seek(woffDir.Offset, SeekOrigin.Begin);

                            var tableBuffer = WoffBuffer.ReadBytes(decompressedStream, bytesCount, out bytesRead);

                            Debug.Assert(bytesRead == bytesCount);

                            if (bytesRead != bytesCount)
                            {
                                return false;
                            }

                            woffDir.CompTable = tableBuffer;
                            woffDir.OrigTable = tableBuffer;
                        }
                        catch (Exception ex)
                        {
                            errorOccurred = true;
                            Trace.TraceError(ex.Message);
                        }
                    }
                }

                if (errorOccurred)
                {
                    return false;
                }
            }

            for (ushort i = 0; i < numFonts; i++)
            {
                var collectionEntry = _collectionEntries[i];

                var woffFont = _woffFonts[i];
                _woffFonts.Add(woffFont);
                woffFont.EndDirectory();
            }

            return true;
        }

        private bool WriteWoff2(Stream stream)
        {
            if (_woffHeader.IsCollection)
            {
                return WriteWoff2Collection(stream);
            }

            if (stream == null || _woffHeader == null || _woffDirs == null || _woffDirs.Count == 0)
            {
                return false;
            }

            var numTables = _woffHeader.NumTables;

            // Re-order tables in output order (by the OpenType Specifications)
            var sortedDirs = _woffDirs.ToArray();
            Array.Sort(sortedDirs, WoffTableDirectory.GetComparer());

            //-------------------------------------------------------------------------------------------------
            // 1. Compute the checksums, the paddings of the tables will be recalculated after the transformations.
            uint tablesChecksum = 0; // We have only one font here...
            for (int i = 0; i < numTables; i++)
            {
                tablesChecksum += sortedDirs[i].RecalculateChecksum();
            }

            //-------------------------------------------------------------------------------------------------
            // Build the smaller headers + directories data in memory...
            var writer = new WoffWriter();

            var searchRange   = (ushort)(WoffUtils.MaxPower2LE(numTables) * TableSize);
            var entrySelector = WoffUtils.Log2(WoffUtils.MaxPower2LE(numTables));
            var rangeShift    = (ushort)(numTables * TableSize - searchRange);

            // Write the font offset table...
            // uint32:	sfntVersion	0x00010000 or 0x4F54544F ('OTTO')
            writer.WriteUInt32(_woffHeader.Flavor);
            // uint16:	numTables	Number of tables.
            writer.WriteUInt16(_woffHeader.NumTables);
            // uint16	searchRange	(Maximum power of 2 <= numTables) x 16.
            writer.WriteUInt16(searchRange);
            // uint16:	entrySelector	Log2(maximum power of 2 <= numTables).
            writer.WriteUInt16(entrySelector);
            // uint16:	rangeShift	NumTables x 16-searchRange.
            writer.WriteUInt16(rangeShift);

            uint tablesOffset = HeaderSize + numTables * TableSize;

            for (int i = 0; i < numTables; i++)
            {
                var woffDir = sortedDirs[i];
                // Tag:	tableTag	Table identifier.
                writer.WriteUInt32(woffDir.Tag);
                // uint32:	checkSum	CheckSum for this table.
                writer.WriteUInt32(woffDir.OrigChecksum);
                // Offset32	offset	Offset from beginning of TrueType font file.
                writer.WriteUInt32(tablesOffset);
                // uint32	length	Length of this table.
                writer.WriteUInt32(woffDir.OrigLength);

                tablesOffset += woffDir.OrigLength;
                if (tablesOffset % 4 != 0)
                {
                    tablesOffset += 4 - (tablesOffset % 4);
                }
            }

            // Get the buffer of the collection header and font headers...
            var fontBuffer = writer.GetBuffer();

            // For the font headers, calculate the checksum to create the font checksum
            _woffFonts[0].ChecksumAdjustment(fontBuffer, 0, (uint)fontBuffer.Length, tablesChecksum);

            //-------------------------------------------------------------------------------------------------
            // Write the actual tables data to the stream, with the required offsets...
            stream.Write(fontBuffer, 0, fontBuffer.Length);

            var paddingBuffer = new byte[4];

            for (int i = 0; i < numTables; i++)
            {
                var woffDir = sortedDirs[i];

                stream.Write(woffDir.OrigTable, 0, (int)woffDir.OrigLength);
                stream.Write(paddingBuffer, 0, (int)woffDir.Padding);
            }

            return true;
        }

        /// <summary>
        /// This writes the transformed WOFF2 font collection data to the TTC OpenType.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <remarks>
        /// The strategy is to produce a layout similar to the simple layout used in the WOFF2 data structure and
        /// still confirm to the OpenType format.
        /// <code>
        /// <![CDATA[
        ///  +---------------------+
        ///  |      TTC Header     |
        ///  +---------------------+
        ///  | All TTC Directories |
        ///  +---------------------+
        ///  |  All the Table Data |
        ///  +---------------------+
        /// ]]>
        /// </code>
        /// </remarks>
        private bool WriteWoff2Collection(Stream stream)
        {
            if (stream == null || _woffHeader == null || _woffDirs == null || _woffDirs.Count == 0)
            {
                return false;
            }

            var numFonts = _collectionHeader.NumFonts;

            //-------------------------------------------------------------------------------------------------
            // 1. Compute the checksums, the paddings of the tables will be recalculated after the transformations.
            for (int i = 0; i < _woffDirs.Count; i++)
            {
                _woffDirs[i].RecalculateChecksum();
            }

            // Compute the sizes of the data structures to determine the offsets...
            // 2. Compute the font collection header size
            int collectionOffset = 0;            
            collectionOffset += WoffBuffer.SizeOfUInt;            // TAG	  ttcTag	                        
            collectionOffset += WoffBuffer.SizeOfUShort;          // uint16	  majorVersion	                
            collectionOffset += WoffBuffer.SizeOfUShort;          // uint16	  minorVersion	                
            collectionOffset += WoffBuffer.SizeOfUInt;            // uint32	  numFonts	                    
            collectionOffset += WoffBuffer.SizeOfUInt * numFonts; // Offset32 tableDirectoryOffsets[numFonts]	
            if (_collectionHeader.IsVersion(2))
            {
                collectionOffset += WoffBuffer.SizeOfUInt;        // uint32	dsigTag	                        
                collectionOffset += WoffBuffer.SizeOfUInt;        // uint32	dsigLength	                    
                collectionOffset += WoffBuffer.SizeOfUInt;        // uint32	dsigOffset	                    
            }

            var headerOffset = (uint)collectionOffset;

            // 3. Compute for each for in the collection the (Header + Directories) size 
            // and sum all to get the size/offset of the fonts
            var directoryOffsets = new uint[numFonts];
            uint directoryOffset = headerOffset;
            for (ushort i = 0; i < numFonts; i++)
            {
                directoryOffsets[i] = directoryOffset;
                // TableDirectory = HeaderSize + Total Table Records
                var HeaderDirSize = HeaderSize + _collectionEntries[i].NumTables * TableSize;
                directoryOffset += HeaderDirSize;
            }

            // 4. Compute for each directory in the collection the offsets
            uint[] tablesOffsets = new uint[_woffDirs.Count];
            uint tablesOffset = directoryOffset;
            for (ushort i = 0; i < _woffDirs.Count; i++)
            {
                var woffDir = _woffDirs[i];
                tablesOffsets[i] = tablesOffset;

                tablesOffset += woffDir.OrigLength;
                if (tablesOffset % 4 != 0)
                {
                    tablesOffset += 4 - (tablesOffset % 4);
                }
            }

            //-------------------------------------------------------------------------------------------------
            // Build the smaller headers + directories data in memory...
            var writer = new WoffWriter();

            // 1. The Font Collection File Structure
            // A font collection file consists of a single TTC Header table, one or more Table Directories 
            // (each corresponding to a different font resource), and a number of OpenType tables. 
            // The TTC Header must be located at the beginning of the TTC file.
            // TAG	     ttcTag	- Font Collection ID string: 'ttcf'
            writer.WriteUInt32(_collectionHeader.TtcTag);
            // uint16	majorVersion - Major version of the TTC Header.
            writer.WriteUInt16(_collectionHeader.MajorVersion);
            // uint16	minorVersion - Minor version of the TTC Header.
            writer.WriteUInt16(_collectionHeader.MinorVersion);
            // uint32	numFonts - Number of fonts in TTC
            writer.WriteUInt32(_collectionHeader.NumFonts);

            // Offset32	tableDirectoryOffsets[numFonts]	Array of offsets to the TableDirectory for each font from the beginning of the file
            for (ushort i = 0; i < numFonts; i++)
            {
                writer.WriteUInt32(directoryOffsets[i]);
            }

            // For the version 2.0 only add the empty DSIG information.
            if (_collectionHeader.IsVersion(2))
            {
                // uint32	dsigTag - Tag indicating that a DSIG table exists, 0x44534947 ('DSIG') (null if no signature)
                writer.WriteUInt32(_collectionHeader.DsigTag);
                // uint32	dsigLength - The length (in bytes) of the DSIG table (null if no signature)
                writer.WriteUInt32(_collectionHeader.DsigLength);
                // uint32	dsigOffset - The offset (in bytes) of the DSIG table from the beginning of the TTC file (null if no signature)
                writer.WriteUInt32(_collectionHeader.DsigOffset);
            }

            // Write out the header for each font
            uint fontHeaderOffset = writer.Offset;
            var fontHeaderOffsets = new List<uint>(numFonts + 1);
            for (ushort j = 0; j < numFonts; j++)
            {
                fontHeaderOffsets.Add(fontHeaderOffset);
                var collectionEntry = _collectionEntries[j];
                var numTables = collectionEntry.NumTables;

                var searchRange   = (ushort)(WoffUtils.MaxPower2LE(numTables) * TableSize);
                var entrySelector = WoffUtils.Log2(WoffUtils.MaxPower2LE(numTables));
                var rangeShift    = (ushort)(numTables * TableSize - searchRange);

                // Write the font offset table...
                // uint32:	sfntVersion	0x00010000 or 0x4F54544F ('OTTO')
                writer.WriteUInt32(collectionEntry.Flavor);
                // uint16:	numTables	Number of tables.
                writer.WriteUInt16(numTables);
                // uint16	searchRange	(Maximum power of 2 <= numTables) x 16.
                writer.WriteUInt16(searchRange);
                // uint16:	entrySelector	Log2(maximum power of 2 <= numTables).
                writer.WriteUInt16(entrySelector);
                // uint16:	rangeShift	NumTables x 16-searchRange.
                writer.WriteUInt16(rangeShift);

                var sortedIndices = collectionEntry.SortedIndices;
                for (ushort i = 0; i < numTables; i++)
                {
                    var dirIndex = sortedIndices[i];
                    var woffDir = _woffDirs[dirIndex];
                    // Tag:	tableTag	Table identifier.
                    writer.WriteUInt32(woffDir.Tag);
                    // uint32:	checkSum	CheckSum for this table.
                    writer.WriteUInt32(woffDir.OrigChecksum);
                    // Offset32	offset	Offset from beginning of TrueType font file.
                    writer.WriteUInt32(tablesOffsets[dirIndex]);
                    // uint32	length	Length of this table.
                    writer.WriteUInt32(woffDir.OrigLength);
                }

                fontHeaderOffset += writer.Offset;
            }
            fontHeaderOffsets.Add(fontHeaderOffset);

            // Get the buffer of the collection header and font headers...
            var fontBuffer = writer.GetBuffer();

            // For the font headers, calculate the checksum to create the font checksum
            for (ushort i = 0; i < numFonts; i += 2)
            {
                uint fontOffset = fontHeaderOffsets[i];
                uint fontLength = fontHeaderOffsets[i + 1] - fontOffset;

                _woffFonts[i].ChecksumAdjustment(fontBuffer, fontOffset, fontLength);
            }

            //-------------------------------------------------------------------------------------------------
            // Write the actual tables data to the stream, with the required offsets...
            stream.Write(fontBuffer, 0, fontBuffer.Length);

            var paddingBuffer = new byte[4];

            for (int i = 0; i < _woffDirs.Count; i++)
            {
                var woffDir = _woffDirs[i];

                stream.Write(woffDir.OrigTable, 0, (int)woffDir.OrigLength);
                stream.Write(paddingBuffer, 0, (int)woffDir.Padding);
            }

            return true;
        }

        #endregion
    }
}
