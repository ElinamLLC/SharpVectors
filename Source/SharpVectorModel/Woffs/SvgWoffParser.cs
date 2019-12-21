using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Collections.Generic;

using SharpVectors.Compressions.ZLib;
using SharpVectors.Compressions.Brotli;

namespace SharpVectors.Woffs
{
    public class SvgWoffParser
    {
        public const string Woff1FileExt = ".woff";
        public const string Woff2FileExt = ".woff2";

        public const string TtfFileExt   = ".ttf";
        public const string OtfFileExt   = ".otf";

        public const string TtcFileExt   = ".ttc";
        public const string OtcFileExt   = ".otc";

        public const string DirectoryName = "SharpVectors";

        // Offsets within the main directory
        private const uint HeaderSize    = 12;

        // Offsets within a specific table record
        private const uint TableSize     = 16;

        private bool _isTransformed;
        private short _woffVersion;

        private string _fontPath;

        private SvgWoffMetadata _metadata;
        private SvgWoffPrivateData _privateData;

        private SvgWoffHeader _woffHeader;
        private IList<SvgWoffTableDirectory> _woffTables;

        public SvgWoffParser()
        {
            _woffVersion   = 0;
            _isTransformed = false;
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

        public SvgWoffHeader Header
        {
            get {
                return _woffHeader;
            }
            private set {
                _woffHeader = value;
            }
        }

        public IList<SvgWoffTableDirectory> Tables
        {
            get {
                return _woffTables;
            }
            private set {
                _woffTables = value;
            }
        }

        public SvgWoffMetadata Metadata
        {
            get {
                return _metadata;
            }
            private set {
                _metadata = value;
            }
        }

        public SvgWoffPrivateData PrivateData
        {
            get {
                return _privateData;
            }
            private set {
                _privateData = value;
            }
        }

        public bool Import(string fontPath)
        {
            if (string.IsNullOrWhiteSpace(fontPath))
            {
                return false;
            }
            var fileExt = Path.GetExtension(fontPath);
            if (string.IsNullOrWhiteSpace(fileExt))
            {
                return false;
            }

            if (!string.Equals(fileExt, Woff1FileExt, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(fileExt, Woff2FileExt, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            _fontPath = fontPath;

            using (var stream = File.OpenRead(fontPath))
            {
                if (string.Equals(fileExt, Woff1FileExt, StringComparison.OrdinalIgnoreCase))
                {
                    return ImportWoff1(stream);
                }
                if (string.Equals(fileExt, Woff2FileExt, StringComparison.OrdinalIgnoreCase))
                {
                    return ImportWoff2(stream);
                }
            }

            return false;
        }

        public bool Export(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            using (var stream = File.OpenWrite(filePath))
            {
                return this.Export(stream);
            }
        }

        public bool Export(Stream stream)
        {
            if (_woffVersion == 1)
            {
                return ExportSingle(stream);
            }
            if (_woffVersion == 2)
            {
                return ExportWoff2(stream);
            }

            return false;
        }

        public string DefaultExportPath
        {
            get {
                string fontFileDir = Path.GetTempPath();
                if (!Directory.Exists(fontFileDir))
                {
                    fontFileDir  = Path.GetDirectoryName(_fontPath);
                }
                else
                {
                    fontFileDir = Path.Combine(fontFileDir, DirectoryName);
                    if (!Directory.Exists(fontFileDir))
                    {
                        Directory.CreateDirectory(fontFileDir);
                    }
                }
                var fontFileBase = Path.GetFileNameWithoutExtension(_fontPath);

                string fontExt = "";
                if (_woffHeader.IsCollection)
                {
                    fontExt = _woffHeader.IsTrueType ? TtcFileExt : OtcFileExt;
                }
                else
                {
                    fontExt = _woffHeader.IsTrueType ? TtfFileExt : OtfFileExt;
                }

                string fontFileName = fontFileBase;
                string fontFilePath = Path.Combine(fontFileDir, fontFileName + fontExt);

                return fontFilePath;
            }
        }

        public string GetExportPath()
        {
            string fontFileDir = Path.GetTempPath();
            if (!Directory.Exists(fontFileDir))
            {
                fontFileDir  = Path.GetDirectoryName(_fontPath);
            }
            else
            {
                fontFileDir = Path.Combine(fontFileDir, DirectoryName);
                if (!Directory.Exists(fontFileDir))
                {
                    Directory.CreateDirectory(fontFileDir);
                }
            }
            var fontFileBase = Path.GetFileNameWithoutExtension(_fontPath);

            string fontExt = "";
            if (_woffHeader.IsCollection)
            {
                fontExt = _woffHeader.IsTrueType ? TtcFileExt : OtcFileExt;
            }
            else
            {
                fontExt = _woffHeader.IsTrueType ? TtfFileExt : OtfFileExt;
            }

            string fontFileName = fontFileBase;
            string fontFilePath = Path.Combine(fontFileDir, fontFileName + fontExt);
            if (File.Exists(fontFilePath) == false)
            {
                return fontFilePath;
            }

            int fileCount = 1;
            fontFileName = fontFileBase + fileCount;
            fontFilePath = Path.Combine(fontFileDir, fontFileName + fontExt);
            while (File.Exists(fontFilePath))
            {
                fileCount++;
                fontFileName = fontFileBase + fileCount;
                fontFilePath = Path.Combine(fontFileDir, fontFileName + fontExt);
            }

            return fontFilePath;
        }

        private bool ImportWoff1(Stream stream)
        {
            int sizeRead = 0;

            _woffHeader = new SvgWoffHeader();
            _woffTables = null;

            var buffer = new byte[SvgWoffHeader.Woff1Size];

            sizeRead = stream.Read(buffer, 0, (int)SvgWoffHeader.Woff1Size);

            Debug.Assert(sizeRead == SvgWoffHeader.Woff1Size);

            if (!_woffHeader.SetHeader(buffer))
            {
                return false;
            }

            _woffTables = new List<SvgWoffTableDirectory>(_woffHeader.NumTables);

            for (int i = 0; i < _woffHeader.NumTables; i++)
            {
                buffer = new byte[SvgWoffTableDirectory.Woff1Size];

                sizeRead = stream.Read(buffer, 0, (int)SvgWoffTableDirectory.Woff1Size);

                Debug.Assert(sizeRead == SvgWoffTableDirectory.Woff1Size);

                var woffTable = new SvgWoffTableDirectory();
                if (woffTable.SetHeader(buffer))
                {
                    _woffTables.Add(woffTable);
                }
            }

            for (int i = 0; i < _woffHeader.NumTables; i++)
            {
                var woffTable = _woffTables[i];

                stream.Seek(woffTable.Offset, SeekOrigin.Begin);

                int bytesRead = 0;
                int bytesCount = (int)woffTable.CompLength;
                if (bytesCount == 0)
                {
                    continue;
                }
                var tableBuffer = SvgWoffObject.ReadBytes(stream, bytesCount, out bytesRead);

                Debug.Assert(bytesRead == bytesCount);

                if (bytesRead != bytesCount)
                {
                    return false;
                }

                woffTable.CompTable = tableBuffer;
                if (woffTable.CompLength == woffTable.OrigLength)
                {
                    // table data is not compressed
                    woffTable.OrigTable = tableBuffer;
                }
                else
                {
                    bytesCount = (int)woffTable.OrigLength;
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

                    woffTable.OrigTable = origBuffer;
                }
            }

            _metadata = new SvgWoffMetadata(_woffHeader.MetaOffset, _woffHeader.MetaLength, _woffHeader.MetaOrigLength);
            if (_woffHeader.HasMetadata)
            {
                stream.Seek(_woffHeader.MetaOffset, SeekOrigin.Begin);

                int bytesRead = 0;
                int bytesCount = (int)_woffHeader.MetaLength;

                var metaBuffer = SvgWoffObject.ReadBytes(stream, bytesCount, out bytesRead);

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

            _privateData = new SvgWoffPrivateData(_woffHeader.PrivateOffset, _woffHeader.PrivateLength);
            if (_woffHeader.HasPrivateData)
            {
                stream.Seek(_woffHeader.PrivateOffset, SeekOrigin.Begin);

                int bytesRead = 0;
                int bytesCount = (int)_woffHeader.PrivateLength;

                var privateBuffer = SvgWoffObject.ReadBytes(stream, bytesCount, out bytesRead);

                Debug.Assert(bytesRead == bytesCount);

                if (bytesRead != bytesCount)
                {
                    return false;
                }

                _privateData.Data = privateBuffer;
            }
            _woffVersion = 1;

            return true;
        }

        private bool ImportWoff2(Stream stream)
        {
            int sizeRead = 0;

            // 1. WOFF2Header: File header with basic font type and version, along with offsets 
            //    to metadata and private data blocks.
            _woffHeader = new SvgWoffHeader();
            _woffTables = null;

            var buffer = new byte[SvgWoffHeader.Woff2Size];

            sizeRead = stream.Read(buffer, 0, (int)SvgWoffHeader.Woff2Size);

            Debug.Assert(sizeRead == SvgWoffHeader.Woff2Size);

            if (!_woffHeader.SetHeader(buffer))
            {
                return false;
            }

            // 2. TableDirectory: Directory of font tables, containing size and other info.  
            _woffTables = new List<SvgWoffTableDirectory>(_woffHeader.NumTables);
            for (int i = 0; i < _woffHeader.NumTables; i++)
            {
                var woffTable = new SvgWoffTableDirectory();
                if (woffTable.SetHeader(stream, _woffHeader.IsTrueType))
                {
                    if (woffTable.IsTransformed)
                    {
                        _isTransformed = true;
                    }
                    _woffTables.Add(woffTable);
                }
            }

            // 3. CollectionDirectory: An optional table containing the font fragment descriptions 
            //    of font collection entries.
            if (_woffHeader.IsCollection)
            {
                //TODO: WOFF2 - Font collection not yet supported
                return false;
            }

            // 4. CompressedFontData: Contents of font tables, compressed for storage in the WOFF2 file. 
            int bytesRead = 0;
            int bytesCount = (int)_woffHeader.TotalCompressedSize;
            if (bytesCount != 0)
            {
                byte[] compressedBuffer = SvgWoffObject.ReadBytes(stream, bytesCount, out bytesRead);
                Debug.Assert(bytesRead == bytesCount);
                if (bytesRead != bytesCount)
                {
                    return false;
                }

                using (var brotliStream = new BrotliInputStream(new MemoryStream(compressedBuffer)))
                {
                    for (int i = 0; i < _woffHeader.NumTables; i++)
                    {
                        var woffTable = _woffTables[i];

                        bytesCount = (int)woffTable.TransformLength;

                        var tableBuffer = SvgWoffObject.ReadBytes(brotliStream, bytesCount, out bytesRead);

                        Debug.Assert(bytesRead == bytesCount);

                        if (bytesRead != bytesCount)
                        {
                            return false;
                        }

                        woffTable.CompTable = tableBuffer;
                        woffTable.OrigTable = tableBuffer;

                        if (!_isTransformed)
                        {
                            woffTable.OrigChecksum = woffTable.CalculateChecksum();
                        }
                    }
                }
            }

            // 5. ExtendedMetadata: An optional block of extended metadata, represented in XML format 
            //    and compressed for storage in the WOFF2 file.
            _metadata = new SvgWoffMetadata(_woffHeader.MetaOffset,
                _woffHeader.MetaLength, _woffHeader.MetaOrigLength);
            if (_woffHeader.HasMetadata)
            {
                stream.Seek(_woffHeader.MetaOffset, SeekOrigin.Begin);

                bytesRead = 0;
                bytesCount = (int)_woffHeader.MetaLength;

                var metaBuffer = SvgWoffObject.ReadBytes(stream, bytesCount, out bytesRead);

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
                        var origBuffer = SvgWoffObject.ReadBytes(brotliStream, bytesCount, out bytesRead);

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
            _privateData = new SvgWoffPrivateData(_woffHeader.PrivateOffset, _woffHeader.PrivateLength);
            if (_woffHeader.HasPrivateData)
            {
                stream.Seek(_woffHeader.PrivateOffset, SeekOrigin.Begin);

                bytesRead = 0;
                bytesCount = (int)_woffHeader.PrivateLength;

                var privateBuffer = SvgWoffObject.ReadBytes(stream, bytesCount, out bytesRead);

                Debug.Assert(bytesRead == bytesCount);

                if (bytesRead != bytesCount)
                {
                    return false;
                }

                _privateData.Data = privateBuffer;
            }
            _woffVersion = 2;

//            ((List<SvgWoffTableDirectory>)(_woffTables)).Sort(new WoffTableComparer());

            return true;
        }

        private bool ExportSingle(Stream stream)
        {
            if (stream == null || _woffHeader == null || _woffTables == null || _woffTables.Count == 0)
            {
                return false;
            }

            var numTables = _woffHeader.NumTables;
            var searchRange   = (ushort)(SvgWoffObject.MaxPower2LE(numTables) * TableSize);
            var entrySelector = SvgWoffObject.Log2(SvgWoffObject.MaxPower2LE(numTables));
            var rangeShift    = (ushort)(numTables * TableSize - searchRange);

            int offset = 0;
            // Write the font offset table...
            var headerBuffer = new byte[HeaderSize];
            // uint32:	sfntVersion	0x00010000 or 0x4F54544F ('OTTO')
            offset += SvgWoffObject.WriteUInt32BE(headerBuffer, offset, _woffHeader.Flavor);
            // uint16:	numTables	Number of tables.
            offset += SvgWoffObject.WriteUInt16BE(headerBuffer, offset, _woffHeader.NumTables);
            // uint16	searchRange	(Maximum power of 2 <= numTables) x 16.
            offset += SvgWoffObject.WriteUInt16BE(headerBuffer, offset, searchRange);
            // uint16:	entrySelector	Log2(maximum power of 2 <= numTables).
            offset += SvgWoffObject.WriteUInt16BE(headerBuffer, offset, entrySelector);
            // uint16:	rangeShift	NumTables x 16-searchRange.
            offset += SvgWoffObject.WriteUInt16BE(headerBuffer, offset, rangeShift);

            stream.Write(headerBuffer, 0, (int)HeaderSize);

            uint tablesOffset = HeaderSize + numTables * TableSize;

            var tableBuffer = new byte[TableSize];
            for (int i = 0; i < numTables; i++)
            {
                offset = 0;
                var woffTable = _woffTables[i];
                // Tag:	tableTag	Table identifier.
                offset += SvgWoffObject.WriteUInt32BE(tableBuffer, offset, woffTable.Tag);
                // uint32:	checkSum	CheckSum for this table.
                offset += SvgWoffObject.WriteUInt32BE(tableBuffer, offset, woffTable.OrigChecksum);
                // Offset32	offset	Offset from beginning of TrueType font file.
                offset += SvgWoffObject.WriteUInt32BE(tableBuffer, offset, tablesOffset);
                // uint32	length	Length of this table.
                offset += SvgWoffObject.WriteUInt32BE(tableBuffer, offset, woffTable.OrigLength);

                stream.Write(tableBuffer, 0, (int)TableSize);

                tablesOffset += woffTable.OrigLength;
                if (tablesOffset % 4 != 0)
                {
                    tablesOffset += 4 - (tablesOffset % 4);
                }
            }

            var paddingBuffer = new byte[4];

            for (int i = 0; i < numTables; i++)
            {
                var woffTable = _woffTables[i];

                stream.Write(woffTable.OrigTable, 0, (int)woffTable.OrigLength);
                stream.Write(paddingBuffer, 0, (int)woffTable.Padding);
            }

            return true;
        }

        private bool ExportWoff2(Stream stream)
        {
            if (_woffHeader.IsCollection)
            {
                return false;
            }
            if (_isTransformed)
            {
                //TODO: WOFF2: Transformed font tables are not supported in export yet
                return false;
            }
            return ExportSingle(stream);
        }

        public uint ComputeTotalSfntSize()
        {
            if (_woffTables == null || _woffTables.Count == 0)
            {
                return 0;
            }

            var totalSfntSize = GetOrigTablesLength(_woffTables) + GetOrigHeaderLength(_woffTables);

            return totalSfntSize;
        }

        private static uint Align4(uint value)
        {
            return (uint)((value + 3) & -4);
        }

        private uint GetOrigHeaderLength(IList<SvgWoffTableDirectory> tableDirectories)
        {
            var totalLength = SvgWoffObject.SizeULong + (4 * SvgWoffObject.SizeUShort)
                + ((4 * SvgWoffObject.SizeULong) * tableDirectories.Count);


            return (uint)totalLength;
        }

        private uint GetOrigTablesLength(IList<SvgWoffTableDirectory> tableDirectories)
        {
            uint length = 0;
            foreach (var entry in tableDirectories)
            {
                length += entry.OrigLength;
                length = Align4(length);

            }
            return length;
        }

        private sealed class WoffTableComparer : IComparer<SvgWoffTableDirectory>
        {
            public WoffTableComparer()
            {
            }

            public int Compare(SvgWoffTableDirectory x, SvgWoffTableDirectory y)
            {
                return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            }
        }
    }
}
