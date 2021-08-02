using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfTestOtherSvg.Handlers
{
    public sealed class SvgnetTestHandler : SvgTestHandler
    {
        public const string FileName = "Svgnet.exe";

        public SvgnetTestHandler(string inputDir, string workingDir, string appDir)
            : base(FileName, TagSvgNet, inputDir, workingDir)
        {
            _appDir = appDir;
        }

        public override bool Marshal(TextWriter writer, bool singleFile = true)
        {
            if (!this.IsInitialized)
            {
                return false;
            }

            writer.WriteLine(string.Format("{0} {1}", KeyDir, this.WorkingDir));
            writer.WriteLine(string.Format("{0} {1}", KeyOut, this.OutputDir));
            if (singleFile)
            {
                writer.WriteLine(string.Format("{0} {1}", KeyArgs, string.Format("-w={0} -h={1} -d={2} -o={3} \"{4}\"",
                    _imageWidth, _imageHeight, _imageDpi, _appTag, this.InputFile)));
            }
            else
            {
                writer.WriteLine(string.Format("{0} {1}", KeyArgs, string.Format("-w={0} -h={1} -d={2} -o={3} \"{4}\"",
                    _imageWidth, _imageHeight, _imageDpi, _appTag, this.InputDir)));
            }
            writer.WriteLine(string.Format("{0} {1}", KeyApps, this.AppFile));

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
