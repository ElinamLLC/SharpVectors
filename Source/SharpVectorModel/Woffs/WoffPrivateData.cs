using System;

namespace SharpVectors.Woffs
{
    public sealed class WoffPrivateData
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

        public WoffPrivateData()
        {
        }

        public WoffPrivateData(uint offset, uint length)
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
