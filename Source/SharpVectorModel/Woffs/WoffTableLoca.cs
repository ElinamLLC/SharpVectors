using System;
using System.Diagnostics;

namespace SharpVectors.Woffs
{
    public sealed class WoffTableLoca : WoffTable
    {
        /// <summary>
        /// Offsets to specific elements in the underlying data, relative to the start of the table. 
        /// </summary>
        public enum FieldOffsets
        {
        }

        public WoffTableLoca(WoffFont woffFont, WoffTableDirectory woffDir)
            : base(woffFont, woffDir)
        {
        }

        protected override bool ReconstructTable()
        {
            if (_glyfIndex != -1 && _locaIndex != -1 && _woffDir.IsTransformed)
            {
                return SerializeLoca();
            }

            return true;
        }

        private bool SerializeLoca()
        {
            var glyfTable = _woffFont.Tables[_glyfIndex] as WoffTableGlyf;
            Debug.Assert(glyfTable != null);
            if (glyfTable == null)
            {
                return false;
            }

            // indexToLocFormat	0 for short offsets (Offset16), 1 for long (Offset32).
            ushort indexFormat = glyfTable.IndexFormat;
            if (indexFormat != 0 && indexFormat != 1)
            {
                return false;
            }

            var glyphs = glyfTable.Glyphs;
            var glyphsLocations = glyfTable.GlyphsLocations;

            var locaWriter  = new WoffWriter();
            uint locaOffset = 0;

            int glyphCount = glyphs.Length;
            for (ushort i = 0; i < glyphCount; i++)
            {
                locaOffset = glyphsLocations[i];

                // indexToLocFormat	0 for short offsets (Offset16), 1 for long (Offset32).
                if (indexFormat == 1)
                {
                    locaWriter.WriteUInt32(locaOffset);
                }
                else
                {
                    var shortOffset = (ushort)(locaOffset >> 1);
                    locaWriter.WriteUInt16(shortOffset);
                }
            }

            //if (locaOffset != glyfDir.OrigLength) //TODO: Needed?
            locaOffset = glyfTable.Length;
            if (indexFormat == 1)
            {
                locaWriter.WriteUInt32(locaOffset);
            }
            else
            {
                var shortOffset = (ushort)(locaOffset >> 1);
                locaWriter.WriteUInt16(shortOffset);
            }

            var locaBuffer = locaWriter.GetBuffer();

            _woffDir.OrigTable  = locaBuffer;
            _woffDir.OrigLength = (uint)locaBuffer.Length;

            return true;
        }
    }
}
