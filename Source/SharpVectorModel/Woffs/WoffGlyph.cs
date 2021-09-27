using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace SharpVectors.Woffs
{
    public enum WoffGlyphType
    {
        None,
        Simple,
        Composite
    }

    public struct WoffPoint : IEquatable<WoffPoint>
    {
        public short X;
        public short Y;
        public bool IsOnCurve;

        public WoffPoint(short x, short y, bool onCurve)
        {
            this.X = x;
            this.Y = y;
            this.IsOnCurve = onCurve;
        }

        public WoffPoint(int x, int y, bool onCurve)
        {
            this.X = (short)x;
            this.Y = (short)y;
            this.IsOnCurve = onCurve;
        }

        public override int GetHashCode()
        {
            var hashCode = (this.IsOnCurve ? 1 : 0).GetHashCode();
            hashCode = (hashCode * 13) ^ this.X.GetHashCode();
            hashCode = (hashCode * 13) ^ this.Y.GetHashCode();

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is WoffPoint)
            {
                return base.Equals((WoffPoint)obj);
            }
            return false;
        }

        public bool Equals(WoffPoint obj)
        {
            return (this.X == obj.X && this.Y == obj.Y && this.IsOnCurve == obj.IsOnCurve);
        }
    }

    public sealed class WoffGlyph
    {
        // simple glyph flags
        private const byte ON_CURVE_POINT             = 0x01;
        private const byte X_SHORT_VECTOR             = 0x02;
        private const byte Y_SHORT_VECTOR             = 0x04;
        private const byte REPEAT_FLAG                = 0x08;
        private const byte X_IS_SAME_OR_POSITIVE      = 0x10;
        private const byte Y_IS_SAME_OR_POSITIVE      = 0x20;
        private const byte OVERLAP_SIMPLE             = 0x40;

        // composite glyph flags
        private const ushort MORE_COMPONENTS          = 0x0020;

        private const ushort ARG_1_AND_2_ARE_WORDS    = 0x0001;
        private const ushort ARGS_ARE_XY_VALUES       = 0x0002;

        private const ushort WE_HAVE_INSTRUCTIONS     = 0x0100;
        private const ushort WE_HAVE_A_SCALE          = 0x0008;
        private const ushort WE_HAVE_AN_X_AND_Y_SCALE = 0x0040;
        private const ushort WE_HAVE_A_TWO_BY_TWO     = 0x0080;
        private const ushort OVERLAP_COMPOUND         = 0x0400;

        private static readonly byte[] DataEmpty = new byte[0];
        private static readonly IList<WoffPoint> ContoursEmpty = new List<WoffPoint>(0);

        private short _numContours;

        // Glyph bounding box.
        private short _xMin;
        private short _xMax;
        private short _yMin;
        private short _yMax;

        private uint _glyphIndex;
        private WoffGlyphType _glyphType;

        private ushort[] _endPtsOfContours;
        private IList<WoffPoint> _contours;

        private ushort _numPoints;

        // Glyph instructions.
        private ushort _instructionLength;
        private byte[] _instructions;

        // Data for composite glyphs.
        private bool _hasInstructions;
        private ushort _componentLength;
        private byte[] _components;

        public WoffGlyph(uint glyphIndex, WoffGlyphType glyphType)
        {
            _glyphIndex   = glyphIndex;
            _glyphType    = glyphType;
            _instructions = DataEmpty;
            _components   = DataEmpty;

            switch (glyphType)
            {
                case WoffGlyphType.None:
                    _contours = ContoursEmpty;
                    break;
                case WoffGlyphType.Simple:
                    _contours = new List<WoffPoint>();
                    break;
                case WoffGlyphType.Composite:
                    _contours = ContoursEmpty;
                    break;
            }
        }

        public WoffGlyph(uint glyphIndex, short numContours, WoffGlyphType glyphType)
            : this(glyphIndex, glyphType)
        {
            // for the composite glyph — the value -1 should be used for composite glyphs.
            if (numContours < 0)
            {
                numContours = -1;
            }
            _numContours = numContours;
        }

        public bool IsComposite
        {
            get {
                return _glyphType == WoffGlyphType.Composite;
            }
        }

        public uint GlyphIndex
        {
            get {
                return _glyphIndex;
            }
            private set {
                _glyphIndex = value;
            }
        }

        public WoffGlyphType GlyphType
        {
            get {
                return _glyphType;
            }
            private set {
                _glyphType = value;
            }
        }

        public short NumberOfContours
        {
            get {
                return _numContours;
            }
            set {
                if (value < 0)
                {
                    value = -1; // for composite glyph — the value -1 should be used for composite glyphs.
                }
                _numContours = value;
            }
        }

        public ushort NumberOfPoints
        {
            get {
                return _numPoints;
            }
            set {
                _numPoints = value;
            }
        }

        public short XMin
        {
            get {
                return _xMin;
            }
            set {
                _xMin = value;
            }
        }

        public short XMax
        {
            get {
                return _xMax;
            }
            set {
                _xMax = value;
            }
        }

        public short YMin
        {
            get {
                return _yMin;
            }
            set {
                _yMin = value;
            }
        }

        public short YMax
        {
            get {
                return _yMax;
            }
            set {
                _yMax = value;
            }
        }

        public ushort[] EndPtsOfContours
        {
            get {
                return _endPtsOfContours;
            }
            set {
                _endPtsOfContours = value;
            }
        }

        public bool HasInstructions
        {
            get {
                return _hasInstructions;
            }
            set {
                _hasInstructions = value;
            }
        }

        public ushort InstructionLength
        {
            get {
                if (_instructionLength != 0)
                {
                    return _instructionLength;
                }
                return _instructionLength;
            }
            set {
                _instructionLength = value;
            }
        }

        public byte[] Instructions
        {
            get {
                return _instructions;
            }
            set {
                _instructions = value;
            }
        }

        public IList<WoffPoint> Contours
        {
            get {
                return _contours;
            }
            private set {
                _contours = value;
            }
        }

        public ushort ComponentLength
        {
            get {
                return _componentLength;
            }
            private set {
                _componentLength = value;
            }
        }

        public byte[] Components
        {
            get {
                return _components;
            }
            private set {
                _components = value;
            }
        }

        public bool SetComponents(WoffReader reader)
        {
            uint startOffset = reader.Offset;
            bool weHaveInstructions = false;

            ushort flags = MORE_COMPONENTS;
            while ((flags & MORE_COMPONENTS) > 0)
            {
                flags = reader.ReadUInt16();
                weHaveInstructions |= (flags & WE_HAVE_INSTRUCTIONS) != 0;
                ushort argSize = WoffBuffer.SizeOfUShort;   // 2-bytes glyph index
                if ((flags & ARG_1_AND_2_ARE_WORDS) > 0)
                {
                    argSize += WoffBuffer.SizeOfUShort * 2; // 4-bytes
                }
                else
                {
                    argSize += WoffBuffer.SizeOfUShort;     // 2-bytes
                }
                if ((flags & WE_HAVE_A_SCALE) > 0)
                {
                    argSize += WoffBuffer.SizeOfUShort;     // 2-bytes
                }
                else if ((flags & WE_HAVE_AN_X_AND_Y_SCALE) > 0)
                {
                    argSize += WoffBuffer.SizeOfUShort * 2; // 4-bytes
                }
                else if ((flags & WE_HAVE_A_TWO_BY_TWO) > 0)
                {
                    argSize += WoffBuffer.SizeOfUShort * 4; // 8-bytes
                }
                if (!reader.Skip(argSize))
                {
                    return false;
                }
            }
            uint endOffset = reader.Offset;

            _hasInstructions = weHaveInstructions;

            _componentLength = (ushort)(endOffset - startOffset);
            _components    = reader.ReadByte(_componentLength, startOffset);

            Debug.Assert(reader.Offset == endOffset);

            return reader.Offset == endOffset;
        }

        public void RecalcBounds()
        {
            if (_contours == null || _contours.Count == 0)
            {
                return;
            }
            var nPoints = _contours.Count;

            int xMin = _contours[0].X;
            int xMax = _contours[0].X;
            int yMin = _contours[0].Y;
            int yMax = _contours[0].Y;
            for (int i = 1; i < nPoints; ++i)
            {
                int x = _contours[i].X;
                int y = _contours[i].Y;
                xMin = Math.Min(x, xMin);
                xMax = Math.Max(x, xMax);
                yMin = Math.Min(y, yMin);
                yMax = Math.Max(y, yMax);
            }

            _xMin = (short)xMin;
            _xMax = (short)xMax;
            _yMin = (short)yMin;
            _yMax = (short)yMax;
        }

        // simple glyph flags
        private const int kGlyfOnCurve     = 1 << 0;
        private const int kGlyfXShort      = 1 << 1;
        private const int kGlyfYShort      = 1 << 2;
        private const int kGlyfRepeat      = 1 << 3;
        private const int kGlyfThisXIsSame = 1 << 4;
        private const int kGlyfThisYIsSame = 1 << 5;

        const ushort kCheckSumAdjustmentOffset = 8;

        const ushort kEndPtsOfContoursOffset = 10;
        const ushort kCompositeGlyphBegin = 10;

        public bool Serialize(WoffWriter writer, WoffIndexer pointsWriter, out uint glyphSize)
        {
            glyphSize = 0;
            if (writer == null)
            {
                return false;
            }

            uint startOffset = writer.Offset;

            // 1. Write the Glyph Header
            writer.WriteInt16(_numContours); // int16 numberOfContours for number of contours:  >= 0 is simple glyph, < 0 composite glyph.
            writer.WriteInt16(_xMin); // int16 xMin Minimum x for coordinate data.
            writer.WriteInt16(_yMin); // int16 yMin Minimum y for coordinate data.
            writer.WriteInt16(_xMax); // int16 xMax Maximum x for coordinate data.
            writer.WriteInt16(_yMax); // int16 yMax Maximum y for coordinate data.

            // 2. Write the Simple Glyph Description, if applicable
            if (_glyphType == WoffGlyphType.Simple)
            {
                // uint16	endPtsOfContours[numberOfContours]	
                // Array of point indices for the last point of each contour, in increasing numeric order.
                Debug.Assert(_endPtsOfContours != null && _endPtsOfContours.Length == _numContours);
                if (_endPtsOfContours == null || _endPtsOfContours.Length != _numContours)
                {
                    Debug.Assert(false, "Invalid condition.");
                    return false;
                }
                for (short i = 0; i < _numContours; i++)
                {
                    writer.WriteUInt16(_endPtsOfContours[i]);
                }

                // uint16	instructionLength	Total number of bytes for instructions. 
                // If instructionLength is zero, no instructions are present for this glyph, 
                // and this field is followed directly by the flags field.instructionLength
                writer.WriteUInt16(_instructionLength);

                // uint8	instructions[instructionLength]	Array of instruction byte code for the glyph.
                if (_instructionLength != 0)
                {
                    Debug.Assert(_instructions != null && _instructions.Length == _instructionLength);
                    if (_instructions == null || _instructions.Length != _instructionLength)
                    {
                        Debug.Assert(false, "Invalid condition.");
                        return false;
                    }

                    writer.Write(_instructions);
                }

                // Pack the points
                Debug.Assert(_numPoints == _contours.Count);
                int pointsCapacity = 5 * _numPoints; // Size without the application of packing.
                if (pointsWriter == null)
                {
                    pointsWriter = new WoffIndexer(pointsCapacity);
                }
                else
                {
                    pointsWriter.Offset = 0;
                }
                uint pointsLength = 0;

                if (PackPoints(pointsWriter, (uint)pointsCapacity, ref pointsLength) == false)
                {
                    Debug.Assert(false, "Invalid condition.");
                    return false;
                }

                // Serialize the points...
                writer.Write(pointsWriter.GetBuffer(), 0, (int)pointsLength);
            }
            // Write the Simple Glyph Description, if applicable
            else if (_glyphType == WoffGlyphType.Composite)
            {
                Debug.Assert(_componentLength != 0);
                Debug.Assert(_components != null && _componentLength == _components.Length);
                if (_componentLength == 0 || _components == null || _componentLength != _components.Length)
                {
                    Debug.Assert(false, "Invalid condition.");
                    return false;
                }

                // Serialize the Composite Glyph data...
                writer.Write(_components, 0, _componentLength);
            }

            // NOTE: Without the padding the serialization of the glyph fails!
            var padBytesLength = WoffBuffer.CalcPadBytes((int)writer.Offset, 4);
            if (padBytesLength > 0)
            {
                var paddingBuffer = new byte[4];
                writer.Write(paddingBuffer, 0, padBytesLength);
            }

            uint endOffset = writer.Offset;
            if (endOffset <= startOffset)
            {
                Debug.Assert(false, "Invalid condition.");
                return false;
            }
            glyphSize = endOffset - startOffset;

            return true;
        }

        /// <summary>
        /// This function stores just the point data. Coordinates packing algorithm based on the Google/woff2 sources.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="pointsCapacity"></param>
        /// <param name="pointsLength"></param>
        /// <returns>Returns true on success.</returns>
        private bool PackPoints(WoffIndexer writer, uint pointsCapacity, ref uint pointsLength)
        {
            uint flagOffset = 0;
            int lastFlag    = -1;
            int repeatCount = 0;
            int lastX       = 0;
            int lastY       = 0;
            uint xBytes     = 0;
            uint yBytes     = 0;

            int nPoints     = _contours.Count;

            for (int i = 0; i < nPoints; ++i)
            {
                var point = _contours[i];
                int flag  = point.IsOnCurve ? kGlyfOnCurve : 0;
                int dx    = point.X - lastX;
                int dy    = point.Y - lastY;
                if (dx == 0)
                {
                    flag |= kGlyfThisXIsSame;
                }
                else if (dx > -256 && dx < 256)
                {
                    flag |= kGlyfXShort | (dx > 0 ? kGlyfThisXIsSame : 0);
                    xBytes += 1;
                }
                else
                {
                    xBytes += 2;
                }
                if (dy == 0)
                {
                    flag |= kGlyfThisYIsSame;
                }
                else if (dy > -256 && dy < 256)
                {
                    flag   |= kGlyfYShort | (dy > 0 ? kGlyfThisYIsSame : 0);
                    yBytes += 1;
                }
                else
                {
                    yBytes += 2;
                }

                if (flag == lastFlag && repeatCount != 255)
                {
                    writer[flagOffset - 1] |= kGlyfRepeat;
                    repeatCount++;
                }
                else
                {
                    if (repeatCount != 0)
                    {
                        if ((flagOffset >= pointsCapacity))
                        {
                            return false;
                        }
                        writer[flagOffset++] = (byte)repeatCount;
                    }
                    if ((flagOffset >= pointsCapacity))
                    {
                        return false;
                    }
                    writer[flagOffset++] = (byte)flag;
                    repeatCount = 0;
                }
                lastX    = point.X;
                lastY    = point.Y;
                lastFlag = flag;
            }

            if (repeatCount != 0)
            {
                if ((flagOffset >= pointsCapacity))
                {
                    return false;
                }
                writer[flagOffset++] = (byte)repeatCount;
            }
            uint xyBytes = xBytes + yBytes;
            if ((xyBytes < xBytes || flagOffset + xyBytes < flagOffset || flagOffset + xyBytes > pointsCapacity))
            {
                return false;
            }

            uint xOffset = flagOffset;
            uint yOffset = flagOffset + xBytes;

            lastX = 0;
            lastY = 0;
            for (int i = 0; i < nPoints; ++i)
            {
                int dx = _contours[i].X - lastX;
                if (dx == 0)
                {
                    // pass
                }
                else if (dx > -256 && dx < 256)
                {
                    writer[xOffset++] = (byte)Math.Abs(dx);
                }
                else
                {
                    // will always fit for valid input, but overflow is harmless
                    writer.WriteInt16(dx, ref xOffset); 
                }
                lastX += dx;
                int dy = _contours[i].Y - lastY;
                if (dy == 0)
                {
                    // pass
                }
                else if (dy > -256 && dy < 256)
                {
                    writer[yOffset++] = (byte)Math.Abs(dy);
                }
                else
                {
                    writer.WriteInt16(dy, ref yOffset);
                }
                lastY += dy;
            }
            pointsLength = yOffset;

            return true;
        }
    }
}
