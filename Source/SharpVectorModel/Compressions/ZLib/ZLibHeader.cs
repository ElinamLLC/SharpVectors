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
    public enum ZLibCompressionLevel
    {
        Faster  = 0,
        Fast    = 1,
        Default = 2,
        Optimal = 3,
    }

    public sealed class ZLibHeader
    {
        #region Private Fields

        private bool _isZLibSupported;
        private byte _compressionMethod;                //CMF 0-3
        private byte _compressionInfo;                  //CMF 4-7
        private byte _checkFlag;                        //Flag 0-4 (Check bits for CMF and FLG)
        private bool _dictFlag;                         //Flag 5 (Preset dictionary)
        private ZLibCompressionLevel _compressionLevel; //Flag 6-7 (Compression level)
        
        #endregion

        #region Constructors and Destructor

        public ZLibHeader()
        {

        }

        #endregion

        #region Public Properties

        public bool IsZLibSupported
        {
            get {
                return _isZLibSupported;
            }
            set {
                _isZLibSupported = value;
            }
        }

        public byte CompressionMethod
        {
            get {
                return _compressionMethod;
            }
            set {
                if (value > 15)
                {
                    throw new ArgumentOutOfRangeException("Argument cannot be greater than 15");
                }
                _compressionMethod = value;
            }
        }

        public byte CompressionInfo
        {
            get {
                return _compressionInfo;
            }
            set {
                if (value > 15)
                {
                    throw new ArgumentOutOfRangeException("Argument cannot be greater than 15");
                }
                _compressionInfo = value;
            }
        }

        public byte CheckFlag
        {
            get {
                return _checkFlag;
            }
            set {
                if (value > 31)
                {
                    throw new ArgumentOutOfRangeException("Argument cannot be greater than 31");
                }
                _checkFlag = value;
            }
        }

        public bool DictFlag
        {
            get {
                return _dictFlag;
            }
            set {
                _dictFlag = value;
            }
        }

        public ZLibCompressionLevel CompressionLevel
        {
            get {
                return _compressionLevel;
            }
            set {
                _compressionLevel = value;
            }
        }

        #endregion

        #region Public Methods

        public byte[] EncodeZlibHeader()
        {
            byte[] result = new byte[2];

            this.RefreshCheckFlag();

            result[0] = this.GetCMF();
            result[1] = this.GetFLG();

            return result;
        }

        public static ZLibHeader DecodeHeader(int pCMF, int pFlag)
        {
            ZLibHeader result = new ZLibHeader();

            //Ensure that parameters are bytes
            pCMF  = pCMF & 0x0FF;
            pFlag = pFlag & 0x0FF;

            //Decode bytes
            result.CompressionInfo   = Convert.ToByte((pCMF & 0xF0) >> 4);
            result.CompressionMethod = Convert.ToByte(pCMF & 0x0F);

            result.CheckFlag         = Convert.ToByte(pFlag & 0x1F);
            result.DictFlag          = Convert.ToBoolean(Convert.ToByte((pFlag & 0x20) >> 5));
            result.CompressionLevel  = (ZLibCompressionLevel)Convert.ToByte((pFlag & 0xC0) >> 6);

            result.IsZLibSupported   = (result.CompressionMethod == 8) 
                && (result.CompressionInfo == 7) && (((pCMF * 256 + pFlag) % 31 == 0)) && (result.DictFlag == false);

            return result;
        }

        #endregion

        #region Private Methods

        private void RefreshCheckFlag()
        {
            byte byteFLG = 0x00;

            byteFLG  = (byte)(Convert.ToByte(this.CompressionLevel) << 1);
            byteFLG |= Convert.ToByte(this.DictFlag);

            this.CheckFlag = Convert.ToByte(31 - Convert.ToByte((this.GetCMF() * 256 + byteFLG) % 31));
        }

        private byte GetCMF()
        {
            byte byteCMF = 0x00;

            byteCMF  = (byte)(this.CompressionInfo << 4);
            byteCMF |= (byte)(this.CompressionMethod);

            return byteCMF;
        }

        private byte GetFLG()
        {
            byte byteFLG = 0x00;

            byteFLG  = (byte)(Convert.ToByte(this.CompressionLevel) << 6);
            byteFLG |= (byte)(Convert.ToByte(this.DictFlag) << 5);
            byteFLG |= this.CheckFlag;

            return byteFLG;
        }

        #endregion
    }
}
