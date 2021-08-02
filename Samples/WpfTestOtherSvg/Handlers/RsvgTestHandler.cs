using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace WpfTestOtherSvg.Handlers
{
    public sealed class RsvgTestHandler : SvgTestHandler
    {
        public const string FileName = "rsvg-convert.exe";

        public RsvgTestHandler(string inputDir, string workingDir, string appDir)
            : base(FileName, TagRsvg, inputDir, workingDir)
        {
            _appDir = appDir;
        }

        public override bool Marshal(TextWriter writer, bool singleFile = true)
        {
            if (!this.IsInitialized)
            {
                return false;
            }

            //writer.WriteLine(string.Format("{0} {1}", KeyDir, this.WorkingDir));
            writer.WriteLine(string.Format("{0} {1}", KeyDir, this.OutputDir));
            writer.WriteLine(string.Format("{0} {1}", KeyOut, this.OutputDir));
            if (singleFile)
            {
                writer.WriteLine(string.Format("{0} {1}", KeyArgs, string.Format("-w {0} -h {1} -d {2} -p {3} -f {4} -o {5} \"{6}\"",
                    _imageWidth, _imageHeight, _imageDpi, _imageDpi, _imageExt.TrimStart('.'), _outputName, this.InputFile)));
            }
            else
            {
                throw new NotImplementedException();
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
