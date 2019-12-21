//------------------------------------------------------------------------------------
// .NET ZLib Implementation by Alberto M. 
// https://www.codeproject.com/Tips/830793/NET-ZLib-Implementation
//
// Distributed under The Code Project Open License (CPOL) license.
// See file LICENSE for detail or copy at https://www.codeproject.com/info/cpol10.aspx
//------------------------------------------------------------------------------------

using System;

namespace SharpVectors.Compressions.ZLib
{
    public sealed class Adler32
    {
        #region Private Fields

        private const int Base = 65521;
        private const int NMax = 5550;

        private uint _a;
        private uint _b;
        private int _pend;

        #endregion

        #region Constructors and Destructor

        public Adler32()
        {
            _a    = 1;
            _b    = 0;
            _pend = 0;
        }

        #endregion

        #region Public Methods

        public void Update(byte data)
        {
            if (_pend >= NMax) UpdateModulus();

            _a += data;
            _b += _a;
            _pend++;
        }

        public void Update(byte[] data)
        {
            Update(data, 0, data.Length);
        }

        public void Update(byte[] data, int offset, int length)
        {
            int nextJToComputeModulus = NMax - _pend;
            for (int j = 0; j < length; j++)
            {
                if (j == nextJToComputeModulus)
                {
                    UpdateModulus();
                    nextJToComputeModulus = j + NMax;
                }
                unchecked
                {
                    _a += data[j + offset];
                }
                _b += _a;
                _pend++;
            }
        }

        public void Reset()
        {
            _a = 1;
            _b = 0;
            _pend = 0;
        }

        public uint GetValue()
        {
            if (_pend > 0) UpdateModulus();

            return (_b << 16) | _a;
        }

        #endregion

        #region Private Methods

        private void UpdateModulus()
        {
            _a %= Base;
            _b %= Base;
            _pend = 0;
        }

        #endregion
    }
}
