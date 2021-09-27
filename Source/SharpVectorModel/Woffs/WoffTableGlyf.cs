using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace SharpVectors.Woffs
{
    public sealed class WoffTableGlyf : WoffTable
    {
        private uint _maxNumPoints;
        private ushort _indexFormat;
        private short[] _glyphAdvances;
        private WoffGlyph[] _glyphs;
        private IList<uint> _glyphsLocations;

        /// <summary>
        /// Offsets to specific elements in the underlying data, relative to the start of the table. 
        /// </summary>
        public enum FieldOffsets
        {
        }

        public WoffTableGlyf(WoffFont woffFont, WoffTableDirectory woffDir)
            : base(woffFont, woffDir)
        {
            _maxNumPoints = 0;
            _indexFormat  = ushort.MaxValue;
        }

        public ushort IndexFormat
        {
            get {
                return _indexFormat;
            }
            private set {
                _indexFormat = value;
            }
        }

        public IList<uint> GlyphsLocations
        {
            get {
                return _glyphsLocations;
            }
            private set {
                _glyphsLocations = value;
            }
        }

        public short[] GlyphsAdvances
        {
            get {
                return _glyphAdvances;
            }
            private set {
                _glyphAdvances = value;
            }
        }

        public WoffGlyph[] Glyphs
        {
            get {
                return _glyphs;
            }
            private set {
                _glyphs = value;
            }
        }

        protected override bool ReconstructTable()
        {
            if (_glyfIndex != -1 && _locaIndex != -1 && _woffDir.IsTransformed)
            {
                _glyphsLocations = new List<uint>();

                return TransformGlyf();
            }

            return true;
        }

        private bool TransformGlyf()
        {
            var woffDirs = _woffFont.Directories;

            // By the specs: both glyf and loca tables must either be present in their transformed format
            //               or with null transform applied to both tables.
            if (_glyfIndex == -1 || _locaIndex == -1) // Already verified!
            {
                Trace.TraceError("Glyph Compression Error: The glyf and loca tables must "
                    +"either be present in their transformed format or with null transform applied to both tables.");
                return false;
            }
            var locaDir = woffDirs[_locaIndex];

            // By the specs: The transformLength of the transformed loca table must always be zero.
            Debug.Assert(locaDir.TransformLength == 0);
            if (locaDir.TransformLength != 0)
            {
                Trace.TraceError("Glyph Compression Error: "
                    +"The transformLength of the transformed loca table must always be zero.");
                return false;
            }

            var reader = new WoffReader(_woffDir.CompTable);

            var version               = reader.ReadUInt32(); // Fixed version = 0x00000000 
            var numGlyphs             = reader.ReadUInt16(); // Number of glyphs 
            var indexFormat           = reader.ReadUInt16(); // Offset format for loca table, should be consistent with indexToLocFormat of the original head table (see [OFF] specification) 

            var nContourStreamSize    = reader.ReadUInt32(); // Size of nContour stream in bytes 
            var nPointsStreamSize     = reader.ReadUInt32(); // Size of nPoints stream in bytes 
            var flagStreamSize        = reader.ReadUInt32(); // Size of flag stream in bytes 
            var glyphStreamSize       = reader.ReadUInt32(); // Size of glyph stream in bytes (a stream of variable-length encoded values, see description below) 
            var compositeStreamSize   = reader.ReadUInt32(); // Size of composite stream in bytes (a stream of variable-length encoded values, see description below) 
            var bboxStreamSize        = reader.ReadUInt32(); // Size of bbox data in bytes representing combined length of bboxBitmap (a packed bit array) and bboxStream (a stream of Int16 values) 
            var instructionStreamSize = reader.ReadUInt32(); // Size of instruction stream (a stream of UInt8 values) 

            Debug.Assert(version == 0x00000000);
            
            // https://dev.w3.org/webfonts/WOFF2/spec/#conform-mustRejectLoca
            // dst_length here is origLength in the spec
            uint expectedLocaDstLength = (uint)((indexFormat != 0 ? 4 : 2) * (numGlyphs + 1));
            Debug.Assert(locaDir.OrigLength == expectedLocaDstLength);

            uint nContourStreamOffset   = reader.Offset;
            var nPointsStreamOffset     = nContourStreamOffset  + nContourStreamSize;
            var flagStreamOffset        = nPointsStreamOffset   + nPointsStreamSize;
            var glyphStreamOffset       = flagStreamOffset      + flagStreamSize;
            var compositeStreamOffset   = glyphStreamOffset     + glyphStreamSize;
            var bboxStreamOffset        = compositeStreamOffset + compositeStreamSize;
            var instructionStreamOffset = bboxStreamOffset      + bboxStreamSize;

            var bboxBitmapOffset = bboxStreamOffset;
            // Safe because numGlyphs is bounded
            uint bboxBitmapSize = (uint)(((numGlyphs + 31) >> 5) << 2);

            // Store the glyph data type format
            _indexFormat = indexFormat;

            // Create the glyph array to store the transformed glyphs
            _glyphs = new WoffGlyph[numGlyphs];            
            _maxNumPoints = 0;

            // 	Stream of Int16 values representing number of contours for each glyph record
            // byte[] nContourStream   = reader.ReadByte(nContourStreamSize, nContourStreamOffset); 
            var nContourStream  = new List<short>();
            var compositeGlyphs = new List<ushort>(); // Store the composite glyph positions
            var nContoursCount  = 0;                  // The combined number of contours
            for (ushort glyphIndex = 0; glyphIndex < numGlyphs; glyphIndex++)
            {
                var nContours = reader.ReadInt16();
                nContourStream.Add(nContours);

                if (nContours == 0)
                {
                    // Empty glyph
                    _glyphs[glyphIndex] = new WoffGlyph(glyphIndex, 0, WoffGlyphType.None);
                }
                else if (nContours > 0) // -1 for signed, 0xffff for unsigned
                {
                    // Simple glyph
                    var glyph = new WoffGlyph(glyphIndex, nContours, WoffGlyphType.Simple);
                    _glyphs[glyphIndex] = glyph;

                    nContoursCount += nContours;
                }
                else if (nContours < 0)
                {
                    // Composite glyph
                    var glyph = new WoffGlyph(glyphIndex, nContours, WoffGlyphType.Composite);
                    _glyphs[glyphIndex] = glyph;

                    compositeGlyphs.Add(glyphIndex);
                }
            }

            Debug.Assert(nPointsStreamSize == nContoursCount);
            Debug.Assert(reader.Offset == nPointsStreamOffset);
            if (reader.Offset != nPointsStreamOffset)
            {
                Trace.TraceError("Glyph Compression Error: Invalid parameter {0}.", nContourStreamSize);
                return false;
            }

            // 	Stream of values representing number of outline points for each contour in glyph records
            //byte[] nPointsStream  = reader.ReadByte(nPointsStreamSize, nPointsStreamOffset);
            var nPointsStream = new List<ushort>(nContoursCount);
            for (int i = 0; i < nContoursCount; i++)
            {
                nPointsStream.Add(reader.Read255UInt16());
            }

            Debug.Assert(reader.Offset == flagStreamOffset);
            if (reader.Offset != flagStreamOffset)
            {
                Trace.TraceError("Glyph Compression Error: Invalid parameter {0}.", nPointsStreamSize);
                return false;
            }

            // 	Stream of UInt8 values representing flag values for each outline point.
            //byte[] flagStream = reader.ReadByte(flagStreamSize, flagStreamOffset);

            var flagReader = reader.GetReader(flagStreamOffset);
            //Debug.Assert(reader.Offset == glyphStreamOffset + flagStreamSize);
            //if (reader.Offset != glyphStreamOffset)
            //{
            //    Trace.TraceError("Glyph Compression Error: Invalid parameter {0}.", flagStreamSize);
            //    return false;
            //}

            // 	Stream of bytes representing component flag values and associated composite glyph data
            if (compositeGlyphs.Count != 0) // If we have composite glyphs, CFF sources do not contain composite glyph
            {
                byte[] compositeStream = reader.ReadByte(compositeStreamSize, compositeStreamOffset);
                var compositeReader = new WoffReader(compositeStream);
                for (ushort i = 0; i < compositeGlyphs.Count; i++)
                {
                    var glyphIndex = compositeGlyphs[i];
                    var glyph = _glyphs[glyphIndex];
                    Debug.Assert(glyph != null && glyph.GlyphType == WoffGlyphType.Composite);

                    if (glyph.SetComponents(compositeReader) == false)
                    {
                        Trace.TraceError("Glyph Compression Error: Invalid composite stream. GlyphIndex = {0}.", glyphIndex);
                        return false;
                    }
                }
            }

            // 	Stream of bytes representing point coordinate values using variable length encoding format (defined in subclause 5.2)
            //byte[] glyphStream = reader.ReadByte(glyphStreamSize, glyphStreamOffset); 
            reader.Offset    = glyphStreamOffset;
            int nPointsIndex = 0;
            //int nFlagIndex   = 0;
            for (ushort glyphIndex = 0; glyphIndex < numGlyphs; glyphIndex++)
            {
                var glyph = _glyphs[glyphIndex];
                var nContours = nContourStream[glyphIndex];
                if (nContours == 0)
                {
                    // Empty glyph - do nothing
                    Debug.Assert(glyph != null && glyph.GlyphType == WoffGlyphType.None);
                }
                else if (nContours > 0)
                {
                    // Simple glyph
                    Debug.Assert(glyph != null && glyph.GlyphType == WoffGlyphType.Simple);

                    ushort nPoints = 0;

                    var endPtsOfContours = new ushort[nContours];
                    for (ushort i = 0; i < nContours; i++)
                    {
                        nPoints += nPointsStream[nPointsIndex + i];
                        endPtsOfContours[i] = (ushort)(nPoints - 1);
                    }
                    nPointsIndex += nContours;

                    var glyphPoints = glyph.Contours;
                    Debug.Assert(glyphPoints != null);
                    if (WoffTriplet.Decode(flagReader, reader, nPoints, glyphPoints) == false)
                    {
                        Trace.TraceError("Glyph Compression Error: Triplet decoding failed. GlyphIndex = " + glyphIndex);
                        return false;
                    }

                    glyph.InstructionLength = reader.Read255UInt16();
                    glyph.NumberOfPoints    = nPoints;
                    glyph.EndPtsOfContours  = endPtsOfContours;

                    _maxNumPoints = Math.Max(_maxNumPoints, nPoints);
                }
                else
                {
                    // Composite glyph
                    Debug.Assert(glyph != null && glyph.GlyphType == WoffGlyphType.Composite);

                    if (glyph.HasInstructions)
                    {
                        glyph.InstructionLength = reader.Read255UInt16();
                    }
                }
            }

            // Store xMin of the bounding box to reconstruct 'hmtx'
            _glyphAdvances = new short[numGlyphs];

            // 	Bitmap (a numGlyphs-long bit array) indicating explicit bounding boxes
            byte[] bboxBitmap  = reader.ReadByte(bboxBitmapSize, bboxBitmapOffset);
            const int bitIndex = 7;
            for (ushort glyphIndex = 0; glyphIndex < numGlyphs; glyphIndex++)
            {
                var glyph     = _glyphs[glyphIndex];
                var glyphType = glyph.GlyphType;
                bool hasBbox  = false;
                if ((bboxBitmap[glyphIndex >> 3] & (0x80 >> (glyphIndex & bitIndex))) > 0)
                {
                    hasBbox = true;
                }

                if (glyphType == WoffGlyphType.None)
                {
                    if (hasBbox)
                    {
                        // An empty glyph must not have a bbox.
                        Trace.TraceError("Glyph Compression Error: An empty glyph must not have a bbox. GlyphIndex = {0}.", glyphIndex);
                        return false;
                    }
                }
                else if (glyphType == WoffGlyphType.Composite)
                {
                    if (!hasBbox)
                    {
                        // A composite glyphs must have an explicit bbox.
                        Trace.TraceError("Glyph Compression Error: A composite glyph must have an explicit bbox. GlyphIndex = {0}.", glyphIndex);
                        return false;
                    }
                }

                if (hasBbox)
                {
                    glyph.XMin = reader.ReadInt16();
                    glyph.YMin = reader.ReadInt16();
                    glyph.XMax = reader.ReadInt16();
                    glyph.YMax = reader.ReadInt16();
                }
                else
                {
                    glyph.RecalcBounds();
                }

                _glyphAdvances[glyphIndex] = glyph.XMin;
            }

            // 	Stream of Int16 values representing glyph bounding box data
            //byte[] bboxStream = reader.ReadByte(bboxStreamSize, bboxStreamOffset); 

            Debug.Assert(reader.Offset == instructionStreamOffset);
            if (reader.Offset != instructionStreamOffset)
            {
                Trace.TraceError("Glyph Compression Error: Invalid parameter {0}.", bboxStreamSize);
                return false;
            }

            // 	Stream of UInt8 values representing a set of instructions for each corresponding glyph
            //byte[] instructionStream = reader.ReadByte(instructionStreamSize, instructionStreamOffset); 
            if (instructionStreamSize != 0) // Check if we have instructions, CFF sources do not have instructions.
            {
                for (ushort glyphIndex = 0; glyphIndex < numGlyphs; glyphIndex++)
                {
                    var glyph = _glyphs[glyphIndex];
                    if (glyph.InstructionLength > 0)
                    {
                        glyph.Instructions = reader.ReadByte(glyph.InstructionLength);
                    }
                }
            }

            Debug.Assert(reader.Offset == instructionStreamOffset + instructionStreamSize);
            if (reader.Offset != instructionStreamOffset + instructionStreamSize)
            {
                Trace.TraceError("Glyph Compression Error: Invalid parameter {0}.", instructionStreamSize);
                return false;
            }

            // Serialize the transformed glyphs...
            if (SerializeGlyf(indexFormat) == false)
            {
                return false;
            }

            return true;
        }

        private bool SerializeGlyf(ushort indexFormat)
        {
            Debug.Assert(_glyphs != null && _glyphs.Length != 0);
            Debug.Assert(indexFormat == 0 || indexFormat == 1);

            uint locaOffset = 0;
            var glyphWriter = new WoffWriter();

            uint maxCapacity = 5 * _maxNumPoints; // Size without the application of packing.
            var pointsWriter = new WoffIndexer((int)maxCapacity);  // Created here for reuse

            int glyphCount = _glyphs.Length;
            for (ushort i = 0; i < glyphCount; i++)
            {
                var glyph = _glyphs[i];
                uint glyphSize = 0;
                pointsWriter.Offset = 0; // Reset the offset, it is non-resizable buffer (size is fixed).
                if (glyph.Serialize(glyphWriter, pointsWriter, out glyphSize) == false)
                {
                    Trace.TraceError("Glyph Serialization Error: Glyph Index = {0}.", glyph.GlyphIndex);
                    return false;
                }
                _glyphsLocations.Add(locaOffset);
                locaOffset += glyphSize;
            }

            var glyphBuffer = glyphWriter.GetBuffer();

            _woffDir.OrigTable  = glyphBuffer;
            _woffDir.OrigLength = (uint)glyphBuffer.Length;

            return true;
        }

    }
}
