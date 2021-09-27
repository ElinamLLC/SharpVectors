using System;

namespace SharpVectors.Woffs
{
    public sealed class WoffTableHead : WoffTable
    {
        public const uint SizeOfTable = 54;

        /// <summary>
        /// Offsets to specific elements in the underlying data, relative to the start of the table. 
        /// </summary>
        public enum FieldOffsets
        {
            TableVersionNumber = 0,
            FontRevision       = 4,
            CheckSumAdjustment = 8,
            MagicNumber        = 12,
            Flags              = 16,
            UnitsPerEm         = 18,
            Created            = 20,
            Modified           = 28,
            XMin               = 36,
            YMin               = 38,
            XMax               = 40,
            YMax               = 42,
            MacStyle           = 44,
            LowestRecPPEM      = 46,
            FontDirectionHint  = 48,
            IndexToLocFormat   = 50,
            GlyphDataFormat    = 52
        }

        public WoffTableHead(WoffFont woffFont, WoffTableDirectory woffDir)
            : base(woffFont, woffDir)
        {
            var buffer = woffDir.OrigTable;
            if (buffer != null && buffer.Length >= SizeOfTable)
            {
                var length = buffer.Length;

                _tableBuffer = new WoffBuffer((uint)length);
                _tableBuffer.Copy(buffer);
            }
        }

        public uint TableVersionNumber
        {
            get { return _tableBuffer.GetUInt((uint)FieldOffsets.TableVersionNumber); }
        }

        public uint FontRevision
        {
            get { return _tableBuffer.GetUInt((uint)FieldOffsets.FontRevision); }
        }

        public uint CheckSumAdjustment
        {
            get {
                return _tableBuffer.GetUInt((uint)FieldOffsets.CheckSumAdjustment);
            }
            set {
                _tableBuffer.SetUInt(value, (uint)FieldOffsets.CheckSumAdjustment);
            }
        }

        public uint MagicNumber
        {
            get { return _tableBuffer.GetUInt((uint)FieldOffsets.MagicNumber); }
        }

        public ushort Flags
        {
            get { return _tableBuffer.GetUShort((uint)FieldOffsets.Flags); }
        }

        public ushort UnitsPerEm
        {
            get { return _tableBuffer.GetUShort((uint)FieldOffsets.UnitsPerEm); }
        }

        public long Created
        {
            get { return _tableBuffer.GetLong((uint)FieldOffsets.Created); }
        }

        public long Modified
        {
            get { return _tableBuffer.GetLong((uint)FieldOffsets.Modified); }
        }

        public short XMin
        {
            get { return _tableBuffer.GetShort((uint)FieldOffsets.XMin); }
        }

        public short YMin
        {
            get { return _tableBuffer.GetShort((uint)FieldOffsets.YMin); }
        }

        public short XMax
        {
            get { return _tableBuffer.GetShort((uint)FieldOffsets.XMax); }
        }

        public short YMax
        {
            get { return _tableBuffer.GetShort((uint)FieldOffsets.YMax); }
        }

        public ushort MacStyle
        {
            get { return _tableBuffer.GetUShort((uint)FieldOffsets.MacStyle); }
        }

        public ushort LowestRecPPEM
        {
            get { return _tableBuffer.GetUShort((uint)FieldOffsets.LowestRecPPEM); }
        }

        public short FontDirectionHint
        {
            get { return _tableBuffer.GetShort((uint)FieldOffsets.FontDirectionHint); }
        }

        public short IndexToLocFormat
        {
            get { return _tableBuffer.GetShort((uint)FieldOffsets.IndexToLocFormat); }
        }

        public short GlyphDataFormat
        {
            get { return _tableBuffer.GetShort((uint)FieldOffsets.GlyphDataFormat); }
        }

        // head table checksum requires leaving out the checkSumAdjustment field
        public override uint CalcChecksum()
        {
            return _tableBuffer.CalcChecksum() - this.CheckSumAdjustment;
        }

        public DateTime GetCreatedDateTime()
        {
            DateTime epoch = new DateTime(1904, 1, 1);
            return epoch.AddSeconds(this.Created);
        }

        public DateTime GetModifiedDateTime()
        {
            DateTime epoch = new DateTime(1904, 1, 1);
            return epoch.AddSeconds(this.Modified);
        }

        public byte[] Rebuild(bool checksumAdjustment)
        {
            // create a Motorola Byte Order buffer for the new table
            WoffBuffer headBuffer = new WoffBuffer(SizeOfTable);

            // populate the buffer
            headBuffer.SetFixed(this.TableVersionNumber, (uint)FieldOffsets.TableVersionNumber);
            headBuffer.SetFixed(this.FontRevision,       (uint)FieldOffsets.FontRevision);
            headBuffer.SetUInt(this.CheckSumAdjustment,  (uint)FieldOffsets.CheckSumAdjustment);
            headBuffer.SetUInt(this.MagicNumber,         (uint)FieldOffsets.MagicNumber);
            headBuffer.SetUShort(this.Flags,             (uint)FieldOffsets.Flags);
            headBuffer.SetUShort(this.UnitsPerEm,        (uint)FieldOffsets.UnitsPerEm);
            headBuffer.SetLong(this.Created,             (uint)FieldOffsets.Created);
            headBuffer.SetLong(this.Modified,            (uint)FieldOffsets.Modified);
            headBuffer.SetShort(this.XMin,               (uint)FieldOffsets.XMin);
            headBuffer.SetShort(this.YMin,               (uint)FieldOffsets.YMin);
            headBuffer.SetShort(this.XMax,               (uint)FieldOffsets.XMax);
            headBuffer.SetShort(this.YMax,               (uint)FieldOffsets.YMax);
            headBuffer.SetUShort(this.MacStyle,          (uint)FieldOffsets.MacStyle);
            headBuffer.SetUShort(this.LowestRecPPEM,     (uint)FieldOffsets.LowestRecPPEM);
            headBuffer.SetShort(this.FontDirectionHint,  (uint)FieldOffsets.FontDirectionHint);
            headBuffer.SetShort(this.IndexToLocFormat,   (uint)FieldOffsets.IndexToLocFormat);
            headBuffer.SetShort(this.GlyphDataFormat,    (uint)FieldOffsets.GlyphDataFormat);

            if (checksumAdjustment)
            {
                // For checksum adjustment, we set this value to 0
                headBuffer.SetUInt(0, (uint)FieldOffsets.CheckSumAdjustment);
            }

            return headBuffer.GetBuffer();
        }

        protected override bool ReconstructTable()
        {
            var headBuffer = _woffDir.OrigTable;
            if (headBuffer == null || headBuffer.Length < SizeOfTable)
            {
                return false;
            }
            var length = headBuffer.Length;

            _tableBuffer = new WoffBuffer((uint)length);
            _tableBuffer.Copy(headBuffer);
            // For checksum adjustment, we set this value to 0
            _tableBuffer.SetUInt(0, (uint)FieldOffsets.CheckSumAdjustment);

            _woffDir.OrigTable  = headBuffer;
            _woffDir.OrigLength = (uint)headBuffer.Length;

            return true;
        }
    }
}
