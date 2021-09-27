using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Collections.Generic;

using SharpVectors.Compressions.ZLib;
using SharpVectors.Compressions.Brotli;

namespace SharpVectors.Woffs
{
    internal sealed class SvgWoffParser
    {
        public const string DirectoryName = "SharpVectors";

        private string _fontPath;
        private WoffDecoder _woffDecoder;

        public SvgWoffParser()
        {
            _woffDecoder = new WoffDecoder();
        }

        public bool Import(string fontPath)
        {
            if (_woffDecoder.ReadFont(fontPath))
            {
                _fontPath = fontPath;
                return true;
            }
            return false;
        }

        public bool Export(string fontPath)
        {
            if (_woffDecoder.WriteFont(fontPath))
            {
                return true;
            }
            return false;
        }

        public bool Export(Stream stream)
        {
            if (_woffDecoder.WriteFont(stream))
            {
                return true;
            }
            return false;
        }

        public string DefaultExportPath
        {
            get {
                string fontFileDir = Path.GetTempPath();
                if (!Directory.Exists(fontFileDir))
                {
                    fontFileDir  = Path.GetDirectoryName(_fontPath);
                }
                else
                {
                    fontFileDir = Path.Combine(fontFileDir, DirectoryName);
                    if (!Directory.Exists(fontFileDir))
                    {
                        Directory.CreateDirectory(fontFileDir);
                    }
                }
                var fontFileBase = Path.GetFileNameWithoutExtension(_fontPath);

                string fontExt = string.Empty;
                if (_woffDecoder.Header.IsCollection)
                {
                    fontExt = _woffDecoder.Header.IsTrueType ? WoffUtils.TtcFileExt : WoffUtils.OtcFileExt;
                }
                else
                {
                    fontExt = _woffDecoder.Header.IsTrueType ? WoffUtils.TtfFileExt : WoffUtils.OtfFileExt;
                }

                string fontFileName = fontFileBase;
                string fontFilePath = Path.Combine(fontFileDir, fontFileName + fontExt);

                return fontFilePath;
            }
        }

        public string GetExportPath()
        {
            string fontFileDir = Path.GetTempPath();
            if (!Directory.Exists(fontFileDir))
            {
                fontFileDir  = Path.GetDirectoryName(_fontPath);
            }
            else
            {
                fontFileDir = Path.Combine(fontFileDir, DirectoryName);
                if (!Directory.Exists(fontFileDir))
                {
                    Directory.CreateDirectory(fontFileDir);
                }
            }
            var fontFileBase = Path.GetFileNameWithoutExtension(_fontPath);

            string fontExt = string.Empty;
            if (_woffDecoder.Header.IsCollection)
            {
                fontExt = _woffDecoder.Header.IsTrueType ? WoffUtils.TtcFileExt : WoffUtils.OtcFileExt;
            }
            else
            {
                fontExt = _woffDecoder.Header.IsTrueType ? WoffUtils.TtfFileExt : WoffUtils.OtfFileExt;
            }

            string fontFileName = fontFileBase;
            string fontFilePath = Path.Combine(fontFileDir, fontFileName + fontExt);
            if (File.Exists(fontFilePath) == false)
            {
                return fontFilePath;
            }

            int fileCount = 1;
            fontFileName = fontFileBase + fileCount;
            fontFilePath = Path.Combine(fontFileDir, fontFileName + fontExt);
            while (File.Exists(fontFilePath))
            {
                fileCount++;
                fontFileName = fontFileBase + fileCount;
                fontFilePath = Path.Combine(fontFileDir, fontFileName + fontExt);
            }

            return fontFilePath;
        }
    }
}
