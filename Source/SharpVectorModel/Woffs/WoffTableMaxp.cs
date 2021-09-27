using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpVectors.Woffs
{
    public sealed class WoffTableMaxp : WoffTable
    {
        /// <summary>
        /// Offsets to specific elements in the underlying data, relative to the start of the table. 
        /// </summary>
        public enum FieldOffsets
        {
        }

        public WoffTableMaxp(WoffFont woffFont, WoffTableDirectory woffDir)
            : base(woffFont, woffDir)
        {
        }

        protected override bool ReconstructTable()
        {
            return true;
        }

    }
}
