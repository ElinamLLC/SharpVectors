using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace SharpVectors.Woffs
{
    public sealed class WoffTableName : WoffTable
    {
        private UnicodeEncoding _unicodeNoBOM;

        public WoffTableName(WoffFont woffFont, WoffTableDirectory woffDir)
            : base(woffFont, woffDir)
        {
            var buffer = woffDir.OrigTable;
            if (buffer != null && buffer.Length >= 6)
            {
                var length = buffer.Length;

                _tableBuffer = new WoffBuffer((uint)length);
                _tableBuffer.Copy(buffer);
            }

            _unicodeNoBOM = new UnicodeEncoding(true, false);
        }

        /// <summary>
        /// Offsets to specific elements in the underlying data, relative to the start of the table. 
        /// </summary>
        public enum FieldOffsets : ushort
        {
            FormatSelector    = 0,
            NumberNameRecords = 2,
            OffsetToStrings   = 4,
            NameRecords       = 6
        }

        public enum PlatformIdentifiers : ushort
        {
            Unicode   = 0,
            Macintosh = 1,
            Iso       = 2,
            Windows   = 3,
            Custom    = 4
        }

        public enum NameIdentifiers : ushort
        {
            CopyrightNotice               = 0,
            FontFamilyName                = 1,
            FontSubfamilyName             = 2,
            UniqueFontIdentifier          = 3,
            FullFontName                  = 4,
            VersionString                 = 5,
            PostScriptName                = 6,
            Trademark                     = 7,
            ManufacturerName              = 8,
            Designer                      = 9,
            Description                   = 10,
            VendorUrl                     = 11,
            DesignerUrl                   = 12,
            LicenseDescription            = 13,
            LicenseInfoUrl                = 14,
            Reserved                      = 15,
            PreferredFamily               = 16,
            PreferredSubfamily            = 17,
            CompatibleFullName            = 18,
            SampleText                    = 19,
            PostScriptCid                 = 20,
            WWSFamilyName                 = 21,
            WWSSubfamilyName              = 22,
            LightBackgroundPalette        = 23,
            DarkBackgroundPalette         = 24,
            VariationsPostScripNamePrefix = 25
        }

        public ushort FormatSelector
        {
            get { return _tableBuffer.GetUShort((uint)FieldOffsets.FormatSelector); }
        }

        public ushort NumberNameRecords
        {
            get { return _tableBuffer.GetUShort((uint)FieldOffsets.NumberNameRecords); }
        }

        public ushort OffsetToStrings
        {
            get { return _tableBuffer.GetUShort((uint)FieldOffsets.OffsetToStrings); }
        }

        public string GetNameString()
        {
            string sName = null;
            try
            {
                sName = GetString(3, 0xffff, 0x0409, (ushort)NameIdentifiers.FullFontName);  // MS, any encoding, English, name
                if (sName == null)
                {
                    sName = GetString(3, 0xffff, 0xffff, (ushort)NameIdentifiers.FullFontName); // MS, any encoding, any language, name
                }
                if (sName == null)
                {
                    sName = GetString(1, 0, 0, (ushort)NameIdentifiers.FullFontName); // mac, roman, English, name
                }
                if (string.IsNullOrWhiteSpace(sName))
                {
                    return sName;
                }

                // validate surrogate content
                for (int i = 0; i < sName.Length - 1; i++)
                {
                    if (((sName[i] >= 0xd800 && sName[i] <= 0xdbff) && !(sName[i + 1] >= 0xdc00 && sName[i + 1] <= 0xdfff))
                        || (!(sName[i] >= 0xd800 && sName[i] <= 0xdbff) && (sName[i + 1] >= 0xdc00 && sName[i + 1] <= 0xdfff)))
                    {
                        sName = null;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            return sName;
        }

        public string GetFamilyString()
        {
            string sName = null;
            try
            {
                // MS, any encoding, English, name
                sName = GetString(3, 0xffff, 0x0409, (ushort)NameIdentifiers.FontFamilyName);  
                if (sName == null)
                {
                    // MS, any encoding, any language, name
                    sName = GetString(3, 0xffff, 0xffff, (ushort)NameIdentifiers.FontFamilyName); 
                }
                if (sName == null)
                {
                    // mac, roman, English, name
                    sName = GetString(1, 0, 0, (ushort)NameIdentifiers.FontFamilyName); 
                }
                if (string.IsNullOrWhiteSpace(sName))
                {
                    return sName;
                }

                // validate surrogate content
                for (int i = 0; i < sName.Length - 1; i++)
                {
                    if (((sName[i] >= 0xd800 && sName[i] <= 0xdbff) && !(sName[i + 1] >= 0xdc00 && sName[i + 1] <= 0xdfff))
                        || (!(sName[i] >= 0xd800 && sName[i] <= 0xdbff) && (sName[i + 1] >= 0xdc00 && sName[i + 1] <= 0xdfff)))
                    {
                        sName = null;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            return sName;
        }

        public string GetPostScriptString()
        {
            string sName = null;
            try
            {
                // MS, any encoding, English, name
                sName = GetString(3, 0xffff, 0x0409, (ushort)NameIdentifiers.PostScriptName);  
                if (sName == null)
                {
                    // MS, any encoding, any language, name
                    sName = GetString(3, 0xffff, 0xffff, (ushort)NameIdentifiers.PostScriptName); 
                }
                if (sName == null)
                {
                    // mac, roman, English, name
                    sName = GetString(1, 0, 0, (ushort)NameIdentifiers.PostScriptName); 
                }
                if (string.IsNullOrWhiteSpace(sName))
                {
                    return sName;
                }

                // validate surrogate content
                for (int i = 0; i < sName.Length - 1; i++)
                {
                    if (((sName[i] >= 0xd800 && sName[i] <= 0xdbff) && !(sName[i + 1] >= 0xdc00 && sName[i + 1] <= 0xdfff))
                        || (!(sName[i] >= 0xd800 && sName[i] <= 0xdbff) && (sName[i + 1] >= 0xdc00 && sName[i + 1] <= 0xdfff)))
                    {
                        sName = null;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            return sName;
        }

        public string GetVersionString()
        {
            string sVersion = null;

            try
            {
                // MS, any encoding, English, version
                sVersion = GetString(3, 0xffff, 0x0409, (ushort)NameIdentifiers.VersionString);  
                if (sVersion == null)
                {
                    // MS, any encoding, any language, version
                    sVersion = GetString(3, 0xffff, 0xffff, (ushort)NameIdentifiers.VersionString); 
                }
                if (sVersion == null)
                {
                    // mac, roman, English, version
                    sVersion = GetString(1, 0, 0, (ushort)NameIdentifiers.VersionString); 
                }
                if (string.IsNullOrWhiteSpace(sVersion))
                {
                    return sVersion;
                }

                // validate surrogate content
                for (int i = 0; i < sVersion.Length - 1; i++)
                {
                    if (((sVersion[i] >= 0xd800 && sVersion[i] <= 0xdbff)
                        && !(sVersion[i + 1] >= 0xdc00 && sVersion[i + 1] <= 0xdfff))
                        || (!(sVersion[i] >= 0xd800 && sVersion[i] <= 0xdbff)
                        && (sVersion[i + 1] >= 0xdc00 && sVersion[i + 1] <= 0xdfff)))
                    {
                        sVersion = null;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            return sVersion;
        }

        public string GetStyleString()
        {
            string sStyle = null;

            try
            {
                // MS, any encoding, English, subfamily (style)
                sStyle = GetString(3, 0xffff, 0x0409, (ushort)NameIdentifiers.FontSubfamilyName);  
                if (sStyle == null)
                {
                    // MS, any encoding, any language, subfamily (style)
                    sStyle = GetString(3, 0xffff, 0xffff, (ushort)NameIdentifiers.FontSubfamilyName); 
                }
                if (sStyle == null)
                {
                    // mac, roman, English, subfamily (style)
                    sStyle = GetString(1, 0, 0, (ushort)NameIdentifiers.FontSubfamilyName); 
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            return sStyle;
        }

        public NameRecord GetNameRecord(uint i)
        {
            NameRecord nr = null;

            if (i < this.NumberNameRecords)
            {
                uint offset = (uint)FieldOffsets.NameRecords + i * 12;
                if (offset + 12 < _tableBuffer.GetLength())
                {
                    nr = new NameRecord((ushort)offset, _tableBuffer);
                }
            }

            return nr;
        }

        public string GetString(ushort PlatID, ushort EncID, ushort LangID, ushort NameID)
        {
            // !!! NOTE: a value of 0xffff for PlatID, EncID, or LangID is used !!!
            // !!! as a wildcard and will match any value found in the table    !!!

            string s = null;

            var numNameRecords = this.NumberNameRecords;
            for (uint i = 0; i < numNameRecords; i++)
            {
                NameRecord nr = GetNameRecord(i);
                if (nr != null)
                {
                    if ((PlatID == 0xffff || nr.PlatformID == PlatID) &&
                        (EncID == 0xffff || nr.EncodingID == EncID) &&
                        (LangID == 0xffff || nr.LanguageID == LangID) &&
                        nr.NameID == NameID)
                    {
                        byte[] buf = GetEncodedString(nr);
                        if (buf != null)
                        {
                            s = DecodeString(nr.PlatformID, nr.EncodingID, buf);
                        }

                        break;
                    }
                }
            }

            return s;
        }

        public byte[] GetEncodedString(NameRecord nr)
        {
            byte[] buf = null;
            int offset = OffsetToStrings + nr.StringOffset;
            if (offset + nr.StringLength - 1 <= _tableBuffer.GetLength())
            {
                buf = new byte[nr.StringLength];
                Buffer.BlockCopy(_tableBuffer.GetBuffer(), offset, buf, 0, nr.StringLength);
            }
            return buf;
        }

        protected override bool ReconstructTable()
        {
            var headBuffer = _woffDir.OrigTable;
            if (headBuffer == null || headBuffer.Length < 6)
            {
                return false;
            }
            var length = headBuffer.Length;

            _tableBuffer = new WoffBuffer((uint)length);
            _tableBuffer.Copy(headBuffer);

            var fullFontName = this.GetNameString();
            var fontFamilyName = this.GetFamilyString();
            var postScriptName = this.GetPostScriptString();

            // Check if the FullFontName and FontFamilyName are stripped from the compressed table data
            // Some tools (such as the dvisvgm – A fast DVI to SVG converter) strip both leaving only the PostScriptName
            if (!string.IsNullOrWhiteSpace(fullFontName) && !string.IsNullOrWhiteSpace(fontFamilyName))
            {
                return true;
            }

            var fixFullFontName   = string.IsNullOrWhiteSpace(fullFontName);
            var fixFontFamilyName = string.IsNullOrWhiteSpace(fontFamilyName);

            // For the fix, we will just duplicate an existing name record, find out which one.
            ushort searchRecord = 0;
            if (!fixFullFontName)
            {
                searchRecord = (ushort)NameIdentifiers.FullFontName;
            }
            else if (!fixFontFamilyName)
            {
                searchRecord = (ushort)NameIdentifiers.FontFamilyName;
            }
            else if (!string.IsNullOrWhiteSpace(postScriptName))
            {
                searchRecord = (ushort)NameIdentifiers.PostScriptName;
            }
            else
            {
                // We cannot fix it...
                return true;
            }

            int numRecords = this.NumberNameRecords;
            List<NameRecord> nameRecords = new List<NameRecord>(numRecords);

            for (uint i = 0; i < numRecords; i++)
            {
                NameRecord nr = GetNameRecord(i);
                if (nr != null)
                {
                    byte[] nameBytes = GetEncodedString(nr);
                    if (nameBytes != null && nameBytes.Length != 0)
                    {
                        nr.NameBytes  = nameBytes;
                        nr.NameString = DecodeString(nr.PlatformID, nr.EncodingID, nameBytes);

                        nameRecords.Add(nr);

                        if (searchRecord == nr.NameID)
                        {
                            if (fixFontFamilyName)
                            {
                                var nrFontFamily = nr.Clone();
                                nrFontFamily.NameID = (ushort)NameIdentifiers.FontFamilyName;

                                nameRecords.Add(nrFontFamily);
                            }
                            if (fixFullFontName)
                            {
                                var nrFullFont = nr.Clone();
                                nrFullFont.NameID = (ushort)NameIdentifiers.FullFontName;

                                nameRecords.Add(nrFullFont);
                            }
                        }
                    }
                }
            }

            numRecords = nameRecords.Count;

            List<byte[]> bytesNameString = new List<byte[]>();
            uint lengthOfStrings = 0;
            ushort offsetToStrings = (ushort)(6 + (numRecords * 12));

            for (int i = 0; i < numRecords; i++)
            {
                var nrc = nameRecords[i];
                byte[] byteString = nrc.NameBytes;
                bytesNameString.Add(byteString);
                lengthOfStrings += (ushort)byteString.Length;
            }

            // create a Motorola Byte Order buffer for the new table
            var tableBuffer = new WoffBuffer((uint)((ushort)FieldOffsets.NameRecords + numRecords * 12 + lengthOfStrings));

            // populate the buffer                
            tableBuffer.SetUShort(this.FormatSelector, (uint)FieldOffsets.FormatSelector);
            tableBuffer.SetUShort((ushort)numRecords, (uint)FieldOffsets.NumberNameRecords);
            tableBuffer.SetUShort(offsetToStrings, (uint)FieldOffsets.OffsetToStrings);

            ushort nOffset = 0;
            // Write the NameRecords and Strings
            for (int i = 0; i < numRecords; i++)
            {
                byte[] namBytes = bytesNameString[i];

                uint startOffset = (uint)((ushort)(FieldOffsets.NameRecords) + i * NameRecord.SizeOf);

                tableBuffer.SetUShort((nameRecords[i]).PlatformID, startOffset + (ushort)NameRecord.FieldOffsets.PlatformID);
                tableBuffer.SetUShort((nameRecords[i]).EncodingID, startOffset + (ushort)NameRecord.FieldOffsets.EncodingID);
                tableBuffer.SetUShort((nameRecords[i]).LanguageID, startOffset + (ushort)NameRecord.FieldOffsets.LanguageID);
                tableBuffer.SetUShort((nameRecords[i]).NameID,     startOffset + (ushort)NameRecord.FieldOffsets.NameID);
                tableBuffer.SetUShort((ushort)namBytes.Length,     startOffset + (ushort)NameRecord.FieldOffsets.StringLength);
                tableBuffer.SetUShort(nOffset,                     startOffset + (ushort)NameRecord.FieldOffsets.StringOffset);

                //Write the string to the buffer
                for (int j = 0; j < namBytes.Length; j++)
                {
                    tableBuffer.SetByte(namBytes[j], (uint)(offsetToStrings + nOffset + j));
                }

                nOffset += (ushort)namBytes.Length;
            }


            _tableBuffer = tableBuffer;

            var nameBuffer = _tableBuffer.GetBuffer();

            _woffDir.OrigTable = nameBuffer;
            _woffDir.OrigLength = (uint)nameBuffer.Length;
            _woffDir.RecalculateChecksum();

            return true;
        }

        private static int MacEncIdToCodePage(ushort macEncodingID)
        {
            /*
                Q187858 INFO: Macintosh Code Pages Supported Under Windows NT
                
                10000 (MAC - Roman)
                10001 (MAC - Japanese)
                10002 (MAC - Traditional Chinese Big5)
                10003 (MAC - Korean)
                10004 (MAC - Arabic)
                10005 (MAC - Hebrew)
                10006 (MAC - Greek I)
                10007 (MAC - Cyrillic)
                10008 (MAC - Simplified Chinese GB 2312)
                10010 (MAC - Romania)
                10017 (MAC - Ukraine)
                10029 (MAC - Latin II)
                10079 (MAC - Icelandic)
                10081 (MAC - Turkish)
                10082 (MAC - Croatia) 
            */
            // NOTE: code pages 10010 through 10082
            // don't seem to map to Encoding IDs in the OT spec

            int macCodePage = -1;

            switch (macEncodingID)
            {
                case 0: // Roman
                    macCodePage = 10000;
                    break;
                case 1: // Japanese
                    macCodePage = 10001;
                    break;
                case 2: // Chinese (Traditional)
                    macCodePage = 10002;
                    break;
                case 3: // Korean
                    macCodePage = 10003;
                    break;
                case 4: // Arabic
                    macCodePage = 10004;
                    break;
                case 5: // Hebrew
                    macCodePage = 10005;
                    break;
                case 6: // Greek
                    macCodePage = 10006;
                    break;
                case 7: // Russian
                    macCodePage = 10007;
                    break;

                case 25: // Chinese (Simplified)
                    macCodePage = 10008;
                    break;

                default:
                    Debug.Assert(false, "unsupported text encoding");
                    break;
            }

            return macCodePage;
        }

        private static int MSEncIdToCodePage(ushort msEncID)
        {
            int nCodePage = -1;

            switch (msEncID)
            {
                case 2: // ShiftJIS
                    nCodePage = 932;
                    break;
                case 3: // PRC
                    nCodePage = 936;
                    break;
                case 4: // Big5
                    nCodePage = 950;
                    break;
                case 5: // Wansung
                    nCodePage = 949;
                    break;
                case 6: // Johab
                    nCodePage = 1361;
                    break;
            }

            return nCodePage;
        }

        private static string GetUnicodeStrFromCodePageBuf(byte[] buf, int codepage)
        {
            Encoding enc = Encoding.GetEncoding(codepage);
            Decoder dec = enc.GetDecoder();
            int nChars = dec.GetCharCount(buf, 0, buf.Length);
            char[] destbuf = new char[nChars];
            dec.GetChars(buf, 0, buf.Length, destbuf, 0);

            return new string(destbuf);
        }

        private static byte[] GetCodePageBufFromUnicodeStr(string sNameString, int nCodepage)
        {
            byte[] bString;

            Encoding enc = Encoding.GetEncoding(nCodepage);
            bString = enc.GetBytes(sNameString);

            return bString;
        }

        private string DecodeString(ushort PlatID, ushort EncID, byte[] EncodedStringBuf)
        {
            string s = null;

            if (PlatID == 0) // unicode
            {
                s = _unicodeNoBOM.GetString(EncodedStringBuf);
            }
            else if (PlatID == 1) // Mac
            {
                int nMacCodePage = MacEncIdToCodePage(EncID);
                if (nMacCodePage != -1)
                {
                    if (Type.GetType("Mono.Runtime") != null)
                    {
                        // Mono.Runtime don't currently support
                        // 10001 to 10008.
                        switch (nMacCodePage)
                        {
                            // Close-enough substitutes for names:
                            case 10001:             // Japanese
                                nMacCodePage = 932; // ShiftJIS
                                break;

                            case 10002:             // Chinese (Traditional)
                                nMacCodePage = 950; // Big5
                                break;

                            case 10003:             // Korean
                                nMacCodePage = 949; //
                                break;

                            case 10004:             // mac-arabic
                                nMacCodePage = 1256;
                                break;

                            case 10005:             // mac-hebrew
                                nMacCodePage = 1255;
                                break;

                            case 10006:             // mac-greek
                                nMacCodePage = 1253;
                                break;

                            case 10007:             // mac-cyrillic
                                nMacCodePage = 1251;
                                break;

                            case 10008:             // Chinese (Simplified)
                                nMacCodePage = 936; // PRC
                                break;

                            default:
                                break;
                        }
                    }
                    s = GetUnicodeStrFromCodePageBuf(EncodedStringBuf, nMacCodePage);
                }
            }
            else if (PlatID == 3) // MS
            {
                if (EncID == 0 || // symbol - strings identified as symbol encoded strings 
                                  // aren't symbol encoded, they're unicode encoded!!!
                    EncID == 1 || // unicode
                    EncID == 10) // unicode with surrogate support for UCS-4
                {
                    s = _unicodeNoBOM.GetString(EncodedStringBuf);
                }
                else if (EncID >= 2 && EncID <= 6)
                {
                    int nCodePage = MSEncIdToCodePage(EncID);
                    s = GetUnicodeStrFromCodePageBuf(EncodedStringBuf, nCodePage);
                }
                else
                {
                    //Debug.Assert(false, "unsupported text encoding");
                }
            }

            return s;
        }

        private byte[] EncodeString(string s, ushort PlatID, ushort EncID)
        {
            byte[] buf = null;

            if (PlatID == 0) // unicode
            {
                buf = _unicodeNoBOM.GetBytes(s);
            }
            else if (PlatID == 1) // Mac
            {
                int nCodePage = MacEncIdToCodePage(EncID);
                if (nCodePage != -1)
                {
                    buf = GetCodePageBufFromUnicodeStr(s, nCodePage);
                }
            }
            else if (PlatID == 3) // MS
            {
                if (EncID == 0 || // symbol - strings identified as symbol encoded strings 
                                  // aren't symbol encoded, they're unicode encoded!!!
                    EncID == 1 || // unicode
                    EncID == 10) // unicode with surrogate support for UCS-4
                {
                    buf = _unicodeNoBOM.GetBytes(s);
                }
                else if (EncID >= 2 || EncID <= 6)
                {
                    int nCodePage = MSEncIdToCodePage(EncID);
                    if (nCodePage != -1)
                    {
                        buf = GetCodePageBufFromUnicodeStr(s, nCodePage);
                    }
                }
                else
                {
                    //Debug.Assert(false, "unsupported text encoding");
                }
            }

            return buf;
        }

        public sealed class NameRecord : ICloneable
        {
            public const int SizeOf = 12;

            private ushort _offsetNameRecord;
            private WoffBuffer _tableBuffer;

            private byte[] _nameBytes;
            private string _nameString;

            public NameRecord(ushort offset, WoffBuffer bufTable)
            {
                _offsetNameRecord = offset;
                _tableBuffer      = bufTable;
            }

            public enum FieldOffsets
            {
                PlatformID   = 0,
                EncodingID   = 2,
                LanguageID   = 4,
                NameID       = 6,
                StringLength = 8,
                StringOffset = 10
            }

            public ushort PlatformID
            {
                get {
                    return _tableBuffer.GetUShort(_offsetNameRecord + (uint)FieldOffsets.PlatformID);
                }
                set {
                    _tableBuffer.SetUShort(value, _offsetNameRecord + (uint)FieldOffsets.PlatformID);
                }
            }

            public ushort EncodingID
            {
                get {
                    return _tableBuffer.GetUShort(_offsetNameRecord + (uint)FieldOffsets.EncodingID);
                }
                set {
                    _tableBuffer.SetUShort(value, _offsetNameRecord + (uint)FieldOffsets.EncodingID);
                }
            }

            public ushort LanguageID
            {
                get {
                    return _tableBuffer.GetUShort(_offsetNameRecord + (uint)FieldOffsets.LanguageID);
                }
                set {
                    _tableBuffer.SetUShort(value, _offsetNameRecord + (uint)FieldOffsets.LanguageID);
                }
            }

            public ushort NameID
            {
                get {
                    return _tableBuffer.GetUShort(_offsetNameRecord + (uint)FieldOffsets.NameID);
                }
                set {
                    _tableBuffer.SetUShort(value, _offsetNameRecord + (uint)FieldOffsets.NameID);
                }
            }

            public ushort StringLength
            {
                get {
                    return _tableBuffer.GetUShort(_offsetNameRecord + (uint)FieldOffsets.StringLength);
                }
                set {
                    _tableBuffer.SetUShort(value, _offsetNameRecord + (uint)FieldOffsets.StringLength);
                }
            }

            public ushort StringOffset
            {
                get {
                    return _tableBuffer.GetUShort(_offsetNameRecord + (uint)FieldOffsets.StringOffset);
                }
                set {
                    _tableBuffer.SetUShort(value, _offsetNameRecord + (uint)FieldOffsets.StringOffset);
                }
            }

            public byte[] NameBytes
            {
                get {
                    return _nameBytes;
                }
                set {
                    _nameBytes = value;
                }
            }

            public string NameString
            {
                get {
                    return _nameString;
                }
                set {
                    _nameString = value;
                }
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            public NameRecord Clone()
            {
                ushort offset = 0;
                WoffBuffer bufTable = new WoffBuffer(SizeOf);

                NameRecord cloned   = new NameRecord(offset, bufTable);
                cloned.PlatformID   = this.PlatformID;
                cloned.EncodingID   = this.EncodingID;
                cloned.LanguageID   = this.LanguageID;
                cloned.NameID       = this.NameID;
                cloned.StringLength = this.StringLength;
                cloned.StringOffset = this.StringOffset;

                if (_nameBytes != null)
                {
                    byte[] nameBytes = new byte[_nameBytes.Length];
                    Buffer.BlockCopy(_nameBytes, 0, nameBytes, 0, _nameBytes.Length);

                    cloned.NameBytes = nameBytes;
                }
                if (_nameString != null)
                {
                    cloned.NameString = new string(_nameString.ToCharArray());
                }
                return cloned;
            }
        }
    }
}
