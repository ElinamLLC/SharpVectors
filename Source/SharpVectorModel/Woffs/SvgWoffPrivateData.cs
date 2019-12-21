using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpVectors.Woffs
{
    public sealed class SvgWoffPrivateData
    {
        /// <summary>
        /// Offset to private data block, from beginning of WOFF file.
        /// </summary>
        private uint _offset;

        /// <summary>
        /// Length of private data block.
        /// </summary>
        private uint _length;

        private byte[] _data;

        public SvgWoffPrivateData()
        {
        }

        public SvgWoffPrivateData(uint offset, uint length)
        {
            _offset = offset;
            _length = length;
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

        public byte[] Data
        {
            get {
                return _data;
            }
            set {
                _data = value;
            }
        }
    }
}
