using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Win32;

namespace WpfTestOtherSvg.Handlers
{
    public sealed class MagickTestHandler : SvgTestHandler
    {
        public const string FileName = "magick.exe";

        private MagickTestHandler() : base(FileName, TagMagick, null, null)
        {
        }

        public MagickTestHandler(string inputDir, string workingDir, string appDir)
            : base(FileName, TagMagick, inputDir, workingDir)
        {
            if (!string.IsNullOrWhiteSpace(appDir) && Directory.Exists(appDir))
            { 
                if (File.Exists(Path.Combine(appDir, _appName)))
                {
                    _appDir = appDir;
                }
            }
            if (string.IsNullOrWhiteSpace(_appDir))
            {   
                appDir = GetInstalledMagickPath();
                if (!string.IsNullOrWhiteSpace(appDir) && Directory.Exists(appDir))
                {
                    _appDir = appDir;
                }
            }
        }

        public static string GetInstalledDir()
        {
            MagickTestHandler handler = new MagickTestHandler();
            return handler.GetInstalledMagickPath();
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
                writer.WriteLine(string.Format("{0} {1}", KeyArgs, string.Format("convert -size {0}x{1} -density {2} \"{3}\" png32:{4}",
                    _imageWidth, _imageHeight, _imageDpi, this.InputFile, _outputName)));
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

        private string GetInstalledMagickPath()
        {
            string[] regPaths = {
                @"SOFTWARE\ImageMagick\Current",
            };
            DirectoryInfo browserPath = null;
            foreach (string regPath in regPaths)
            {
                using (RegistryKey pathKey = Registry.LocalMachine.OpenSubKey(regPath))
                {
                    if (pathKey != null)
                    {
                        try
                        {
                            // Trim quotes from parameters
                            string filePath = pathKey.GetValue("BinPath").ToString();
                            if (!string.IsNullOrWhiteSpace(filePath) && Directory.Exists(filePath))
                            {
                                if (File.Exists(Path.Combine(filePath, _appName)))
                                {
                                    browserPath = new DirectoryInfo(filePath);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                if (browserPath != null && browserPath.Exists)
                {
                    break;
                }
            }
            if (browserPath != null && browserPath.Exists)
            {
                return browserPath.FullName;
            }
            return null;
        }
    }
}
