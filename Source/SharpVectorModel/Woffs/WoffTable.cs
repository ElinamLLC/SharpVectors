using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace SharpVectors.Woffs
{
    using WoffTableState = WoffTableDirectory.WoffTableState;

    public class WoffTable
    {
        private readonly static IList<string> _tableFlags;

        protected uint _length;
        /// <summary>
        /// Checksum of the uncompressed table.
        /// </summary>
        protected uint _checksum;

        protected ushort _index;
        protected WoffFont _woffFont;
        protected WoffTableDirectory _woffDir;

        protected WoffBuffer _tableBuffer;

        // WOFF2 only: Target for transformation
        protected int _headIndex;  // Index =  1
        protected int _hheaIndex;  // Index =  2
        protected int _hmtxIndex;  // Index =  3
        protected int _maxpIndex;  // Index =  4
        protected int _nameIndex;  // Index =  5
        protected int _glyfIndex;  // Index = 10
        protected int _locaIndex;  // Index = 11

        #region Constructors and Destructor

        public WoffTable(WoffFont woffFont, WoffTableDirectory woffDir)
        {
            if (woffFont == null)
            {
                throw new ArgumentNullException(nameof(woffFont), "The font object is required by the font table object.");
            }
            if (woffDir == null)
            {
                throw new ArgumentNullException(nameof(woffDir), "The font directory object is required by the font table object.");
            }
            _headIndex = -1;
            _hheaIndex = -1;
            _hmtxIndex = -1;
            _maxpIndex = -1;
            _nameIndex = -1;
            _glyfIndex = -1;
            _locaIndex = -1;

            _index = ushort.MaxValue;
            _length     = woffDir.OrigLength;
            _checksum   = woffDir.OrigChecksum;
            _woffDir    = woffDir;
            _woffFont   = woffFont;

            if (!woffFont.IsCollection)
            {
                _index = woffDir.WoffIndex;
            }
        }

        static WoffTable()
        {
            _tableFlags = new List<string>(64);
            _tableFlags.Add("cmap"); //  0	
            _tableFlags.Add("head"); //  1	
            _tableFlags.Add("hhea"); //  2	
            _tableFlags.Add("hmtx"); //  3	
            _tableFlags.Add("maxp"); //  4	
            _tableFlags.Add("name"); //  5	
            _tableFlags.Add("OS/2"); //  6	
            _tableFlags.Add("post"); //  7	
            _tableFlags.Add("cvt "); //  8	 
            _tableFlags.Add("fpgm"); //  9	
            _tableFlags.Add("glyf"); // 10	
            _tableFlags.Add("loca"); // 11	
            _tableFlags.Add("prep"); // 12	
            _tableFlags.Add("CFF "); // 13	 
            _tableFlags.Add("VORG"); // 14	
            _tableFlags.Add("EBDT"); // 15
            _tableFlags.Add("EBLC"); // 16	
            _tableFlags.Add("gasp"); // 17	
            _tableFlags.Add("hdmx"); // 18	
            _tableFlags.Add("kern"); // 19	
            _tableFlags.Add("LTSH"); // 20	
            _tableFlags.Add("PCLT"); // 21	
            _tableFlags.Add("VDMX"); // 22	
            _tableFlags.Add("vhea"); // 23	
            _tableFlags.Add("vmtx"); // 24	
            _tableFlags.Add("BASE"); // 25	
            _tableFlags.Add("GDEF"); // 26	
            _tableFlags.Add("GPOS"); // 27	
            _tableFlags.Add("GSUB"); // 28	
            _tableFlags.Add("EBSC"); // 29	
            _tableFlags.Add("JSTF"); // 30	
            _tableFlags.Add("MATH"); // 31
            _tableFlags.Add("CBDT"); // 32		
            _tableFlags.Add("CBLC"); // 33		
            _tableFlags.Add("COLR"); // 34		
            _tableFlags.Add("CPAL"); // 35		
            _tableFlags.Add("SVG "); // 36		    
            _tableFlags.Add("sbix"); // 37		
            _tableFlags.Add("acnt"); // 38		
            _tableFlags.Add("avar"); // 39		
            _tableFlags.Add("bdat"); // 40		
            _tableFlags.Add("bloc"); // 41		
            _tableFlags.Add("bsln"); // 42		
            _tableFlags.Add("cvar"); // 43		
            _tableFlags.Add("fdsc"); // 44		
            _tableFlags.Add("feat"); // 45		
            _tableFlags.Add("fmtx"); // 46		
            _tableFlags.Add("fvar"); // 47
            _tableFlags.Add("gvar"); // 48		
            _tableFlags.Add("hsty"); // 49	
            _tableFlags.Add("just"); // 50	
            _tableFlags.Add("lcar"); // 51	
            _tableFlags.Add("mort"); // 52	
            _tableFlags.Add("morx"); // 53	
            _tableFlags.Add("opbd"); // 54	
            _tableFlags.Add("prop"); // 55	
            _tableFlags.Add("trak"); // 56	
            _tableFlags.Add("Zapf"); // 57	
            _tableFlags.Add("Silf"); // 58	
            _tableFlags.Add("Glat"); // 59	
            _tableFlags.Add("Gloc"); // 60	
            _tableFlags.Add("Feat"); // 61	
            _tableFlags.Add("Sill"); // 62	
            _tableFlags.Add("****"); // 63 : arbitrary tag follows	
        }

        #endregion

        public static IList<string> TableFlags
        {
            get {
                return _tableFlags;
            }
        }

        public uint Length
        {
            get {
                return _length;
            }
            private set {
                _length = value;
            }
        }

        public uint Checksum
        {
            get {
                return _checksum;
            }
            private set {
                _checksum = value;
            }
        }

        public ushort Index
        {
            get {
                return _index;
            }
            private set {
                _index = value;
            }
        }

        public WoffFont Font
        {
            get {
                return _woffFont;
            }
            private set {
                _woffFont = value;
            }
        }

        public WoffTableDirectory Directory
        {
            get {
                return _woffDir;
            }
            private set {
                _woffDir = value;
            }
        }

        /// <summary>Calculate checksum for all except for 'head'</summary>
        public virtual uint CalcChecksum()
        {
            // NOTE: this method gets overridden by the head table class
            if (_tableBuffer != null)
            {
                return _tableBuffer.CalcChecksum();
            }

            return _woffDir.CalculateChecksum();
        }

        public bool Reconstruct()
        {
            Debug.Assert(_woffDir != null);
            if (_woffDir.WoffState == WoffTableState.Recontructed)
            {
                return true;
            }

            _headIndex = _woffFont.HeadTableIndex;
            _hheaIndex = _woffFont.HheaTableIndex;
            _hmtxIndex = _woffFont.HmtxTableIndex;
            _maxpIndex = _woffFont.MaxpTableIndex;
            _nameIndex = _woffFont.NameTableIndex;
            _glyfIndex = _woffFont.GlyfTableIndex;
            _locaIndex = _woffFont.LocaTableIndex;

            var isRecontructed = this.ReconstructTable();
            if (isRecontructed)
            {
                _woffDir.WoffState = WoffTableState.Recontructed;
            }
            return isRecontructed;
        }

        protected virtual bool ReconstructTable()
        {
            return true;
        }
    }
}
