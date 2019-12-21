using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace SharpVectors.Woffs
{
    public sealed class SvgWoffMetadata
    {
        /// <summary>
        /// Offset to metadata block, from beginning of WOFF file.
        /// </summary>
        private uint _offset;

        /// <summary>
        /// Length of compressed metadata block.
        /// </summary>
        private uint _length;

        /// <summary>
        /// Uncompressed size of metadata block.
        /// </summary>
        private uint _origLength;

        private byte[] _data;

        private byte[] _origData;

        private XDocument _document;

        public SvgWoffMetadata()
        {
        }

        public SvgWoffMetadata(uint offset, uint length, uint origLength)
        {
            _offset     = offset;
            _length     = length;
            _origLength = origLength;
        }

        public uint Offset
        {
            get {
                return _offset;
            }
        }

        public uint Length
        {
            get {
                return _length;
            }
        }

        public uint OrigLength
        {
            get {
                return _origLength;
            }
        }

        public byte[] Data
        {
            get {
                return _data;
            }
            set {
                _data = value;
                if (value != null && _origData == null)
                {
                    _origData = value;
                    _document = null;
                }
            }
        }

        public byte[] OrigData
        {
            get {
                return _origData;
            }
            set {
                _origData = value;
                if (value != null && _data == null)
                {
                    _data = value;
                }
                if (value != null)
                {
                    _document = null;
                }
            }
        }

        public XDocument Document
        {
            get {
                if (_document == null)
                {
                    if (_origData != null && _origData.Length != 0)
                    {
                        _document = XDocument.Load(new MemoryStream(_origData));
                    }
                }
                return _document;
            }
            set {
                _document = value;
            }
        }
    }
}
