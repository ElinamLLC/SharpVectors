using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfTestOtherSvg.Handlers
{
    public sealed class BatikTestHandler : SvgTestHandler
    {
        public const string FileName = "batik-rasterizer-1.14.jar";

        public BatikTestHandler(string inputDir, string workingDir, string appDir)
            : base(FileName, TagBatik, inputDir, workingDir)
        {
            _appDir = appDir;
        }

        public override bool Marshal(TextWriter writer, bool singleFile = true)
        {
            if (!this.IsInitialized)
            {
                return false;
            }

//            writer.WriteLine(string.Format("{0} {1}", KeyDir, this.WorkingDir));
            writer.WriteLine(string.Format("{0} {1}", KeyDir, this.OutputDir));
            writer.WriteLine(string.Format("{0} {1}", KeyOut, this.OutputDir));
            if (singleFile)
            {
                writer.WriteLine(string.Format("{0} {1}", KeyArgs, string.Format(
                    "-jar \"{0}\" -scriptSecurityOff -w {1} -h {2} -dpi {3} -m {4} -d {5} \"{6}\"", this.AppFile,
                    _imageWidth, _imageHeight, _imageDpi, this.ImageMimeType, _outputName, this.InputFile)));
            }
            else
            {
                writer.WriteLine(string.Format("{0} {1}", KeyArgs, string.Format(
                    "-jar \"{0}\" -scriptSecurityOff -w {1} -h {2} -dpi {3} -m {4} -d {5} \"{6}\"", this.AppFile,
                    _imageWidth, _imageHeight, _imageDpi, this.ImageMimeType, _outputName, this.InputDir)));
            }
            writer.WriteLine(string.Format("{0} {1}", KeyApps, "java.exe"));

            return true;
        }

        protected override void OnInitialized()
        {
            if (!this.Validate())
            {
                return;
            }
        }
    }
}
