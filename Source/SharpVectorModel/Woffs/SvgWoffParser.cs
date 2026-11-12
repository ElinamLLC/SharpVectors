using System.IO;

namespace SharpVectors.Woffs
{
    internal sealed class SvgWoffParser
    {
        public const string DirectoryName = "SharpVectors";

        private WoffDecoder _woffDecoder;

        public SvgWoffParser()
        {
            _woffDecoder = new WoffDecoder();
        }

        public bool Import(string fontPath)
        {
            return _woffDecoder.ReadFont(fontPath);
        }

        public bool Export(Stream stream)
        {
            if (_woffDecoder.WriteFont(stream))
            {
                return true;
            }
            return false;
        }

        public string FontExtension
        {
            get {
                if (_woffDecoder.Header.IsCollection)
                {
                    return _woffDecoder.Header.IsTrueType ? WoffUtils.TtcFileExt : WoffUtils.OtcFileExt;
                }
                return _woffDecoder.Header.IsTrueType ? WoffUtils.TtfFileExt : WoffUtils.OtfFileExt;
            }
        }
    }
}
