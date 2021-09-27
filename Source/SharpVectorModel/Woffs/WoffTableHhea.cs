using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpVectors.Woffs
{
    /// <summary>
    /// hhea — Horizontal Header Table.
    /// </summary>
    /// <remarks>
    /// <para>The following parameters maybe affected due updates in <see cref="WoffTableHmtx"/> table.</para>
    /// <list type="bullet">
    /// <item>
    /// <term>advanceWidthMax - UFWORD</term>
    /// <description>Maximum advance width value in 'hmtx' table.</description>
    /// </item>
    /// <item>
    /// <term>minLeftSideBearing - FWORD</term>
    /// <description>Minimum left sidebearing value in 'hmtx' table for glyphs with contours (empty glyphs should be ignored).</description>
    /// </item>
    /// <item>
    /// <term>minRightSideBearing - FWORD</term>
    /// <description>Minimum right sidebearing value; calculated as <c>Min(aw - (lsb + xMax - xMin))</c> 
    /// for glyphs with contours (empty glyphs should be ignored).</description>
    /// </item>
    /// <item>
    /// <term>xMaxExtent - FWORD</term>
    /// <description>Max(lsb + (xMax - xMin)).</description>
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class WoffTableHhea : WoffTable
    {
        /// <summary>
        /// Offsets to specific elements in the underlying data, relative to the start of the table. 
        /// </summary>
        public enum FieldOffsets
        {
        }

        public WoffTableHhea(WoffFont woffFont, WoffTableDirectory woffDir)
            : base(woffFont, woffDir)
        {
        }

        protected override bool ReconstructTable()
        {
            return true;
        }

    }
}
