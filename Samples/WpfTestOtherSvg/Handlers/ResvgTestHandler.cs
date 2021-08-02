using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfTestOtherSvg.Handlers
{
    public sealed class ResvgTestHandler : SvgTestHandler
    {
        public const string FileName = "resvg.exe";

        public ResvgTestHandler(string inputDir, string workingDir, string appDir)
            : base(FileName, TagResvg, inputDir, workingDir)
        {
            _appDir = appDir;
        }

        public override bool Marshal(TextWriter writer, bool singleFile = true)
        {
            if (!this.IsInitialized)
            {
                return false;
            }

            var fontItems = new Dictionary<string, string>(6)
            {
                { "--font-family",       "Noto Sans" },
                { "--serif-family",      "Noto Serif" },
                { "--sans-serif-family", "Noto Sans" },
                { "--cursive-family",    "Yellowtail" },
                { "--fantasy-family",    "Sedgwick Ave Display" },
                { "--monospace-family",  "Noto Mono" }
            };

            var builder = new StringBuilder();
            foreach (KeyValuePair<string, string> fontItem in fontItems)
            {
                builder.AppendFormat("{0} \"{1}\" ", fontItem.Key, fontItem.Value);
            }

//            writer.WriteLine(string.Format("{0} {1}", KeyDir, this.WorkingDir));
            writer.WriteLine(string.Format("{0} {1}", KeyDir, this.OutputDir));
            writer.WriteLine(string.Format("{0} {1}", KeyOut, this.OutputDir));
            if (singleFile)
            {
                writer.WriteLine(string.Format("{0} {1}", KeyArgs, string.Format(
                    "\"{0}\" {1} -w {2} -h {3} -dpi {4} --skip-system-fonts --use-fonts-dir \"{5}\" " + builder.ToString(), 
                    this.InputFile, _outputName, _imageWidth, _imageHeight, _imageDpi, this.FontDir)));
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
