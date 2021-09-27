using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace SharpVectors.Woffs
{
    public sealed class WoffTableHmtx : WoffTable
    {
        /// <summary>
        /// Offsets to specific elements in the underlying data, relative to the start of the table. 
        /// </summary>
        public enum FieldOffsets
        {
        }

        public WoffTableHmtx(WoffFont woffFont, WoffTableDirectory woffDir)
            : base(woffFont, woffDir)
        {
        }

        protected override bool ReconstructTable()
        {
            // While at it, transform the other tables too...
            if (_hheaIndex != -1 && _hmtxIndex != -1 && _woffDir.IsTransformed)
            {
                return TransformHmtx();
            }

            return true;
        }

        private bool TransformHmtx()
        {
            var glyfTable = _woffFont.Tables[_glyfIndex] as WoffTableGlyf;
            Debug.Assert(glyfTable != null);
            if (glyfTable == null)
            {
                return false;
            }

            if (_hheaIndex == -1 || _hmtxIndex== -1)
            {
                return false;
            }

            var glyphs = glyfTable.Glyphs;
            var glyphAdvances = glyfTable.GlyphsAdvances;

            var woffDirs = _woffFont.Directories;
            var hheaDir = woffDirs[_hheaIndex];
            // Get numberOfHMetrics, https://www.microsoft.com/typography/otspec/hhea.htm
            var hheaReader = new WoffReader(hheaDir.OrigTable);
            // Skip 34 to reach 'hhea' numberOfHMetrics
            if (hheaReader.Skip(34) == false)
            {
                return false;
            }
            ushort numOfHMetrics = hheaReader.ReadUInt16();

            var reader = new WoffReader(_woffDir.CompTable);
            var flags = reader.ReadByte();
            // By the specs: When hmtx transform is indicated by the table directory, 
            // the Flags (bits 0 or 1 or both) must be set.
            if ((flags & 0x01) != 1 && (flags & 0x02) != 1)
            {
                Trace.TraceError("Hmtx Compression Error: When hmtx transform is indicated by the table directory, "
                    +"the Flags (bits 0 or 1 or both) must be set.");
                return false;
            }
            // By the specs: Bits 2-7 are reserved and must be zero.
            if ((flags & 0xFC) != 0)
            {
                Trace.TraceError("Hmtx Compression Error: Bits 2-7 are reserved and must be zero.");
            }

            int numOfGlyphs = glyphs.Length;

            Debug.Assert(glyphAdvances.Length == numOfGlyphs);

            // The number of glyphs numGlyphs can be 0, if there is no 'glyf' but cannot then xform 'hmtx'.
            Debug.Assert(numOfHMetrics <= numOfGlyphs);
            if (numOfHMetrics > numOfGlyphs)
            {
                return false;
            }

            // https://www.microsoft.com/typography/otspec/hmtx.htm
            // "...only one entry need be in the array, but that entry is required."
            if (numOfHMetrics < 1)
            {
                return false;
            }

            bool hasProportionalLsbs = (flags & 0x01) == 0;
            bool hasMonospaceLsbs    = (flags & 0x02) == 0;

            IList<ushort> advanceWidths = new List<ushort>();
            IList<short> lsbs = new List<short>();
            for (ushort glyphIndex = 0; glyphIndex < numOfHMetrics; glyphIndex++)
            {
                advanceWidths.Add(reader.ReadUInt16());
            }

            for (ushort glyphIndex = 0; glyphIndex < numOfHMetrics; glyphIndex++)
            {
                if (hasProportionalLsbs)
                {
                    lsbs.Add(reader.ReadInt16());
                }
                else
                {
                    lsbs.Add(glyphAdvances[glyphIndex]);
                }
            }

            for (ushort glyphIndex = numOfHMetrics; glyphIndex < numOfGlyphs; glyphIndex++)
            {
                if (hasMonospaceLsbs)
                {
                    lsbs.Add(reader.ReadInt16());
                }
                else
                {
                    lsbs.Add(glyphAdvances[glyphIndex]);
                }
            }

            return SerializeHmtx(advanceWidths, lsbs, numOfHMetrics, numOfGlyphs);
        }

        private bool SerializeHmtx(IList<ushort> advanceWidths, IList<short> lsbs, int numOfHMetrics, int numOfGlyphs)
        {
            var hmtxBufferSize = 2 * numOfGlyphs + 2 * numOfHMetrics;

            var writer = new WoffWriter(hmtxBufferSize);
            for (int i = 0; i < numOfGlyphs; i++)
            {
                if (i < numOfHMetrics)
                {
                    writer.WriteUInt16(advanceWidths[i]);
                }
                writer.WriteInt16(lsbs[i]);
            }

            var tableBuffer = writer.GetBuffer();
            var hmtxBuffer = new WoffBuffer(tableBuffer);

            _woffDir.OrigTable  = tableBuffer;
            _woffDir.OrigLength = (uint)tableBuffer.Length;

            return true;
        }

    }
}
