using System;
using System.IO;
using System.ComponentModel;

namespace SharpVectors.Runtime
{
    [Serializable]
    [TypeConverter(typeof(EmbeddedBitmapDataConverter))]
    public struct EmbeddedBitmapData
    {
        #region Private Fields

        private MemoryStream _stream;

        #endregion

        #region Constructors

        public EmbeddedBitmapData(MemoryStream stream)
        {
            _stream = stream;
        }

        #endregion

        #region Public Properties

        public MemoryStream Stream
        {
            get { return _stream; }
            set { _stream = value; }
        }

        #endregion
    }
}