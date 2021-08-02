using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Win32;

namespace WpfTestOtherSvg.Handlers
{
    public sealed class InkscapeTestHandler : SvgTestHandler
    {
        public const string FileName = "inkscape.exe";

        public InkscapeTestHandler(string inputDir, string workingDir)
            : base(FileName, TagInkscape, inputDir, workingDir)
        {
            string appDir = GetInstalledInkscapePath();
            if (!string.IsNullOrWhiteSpace(appDir) && File.Exists(appDir))
            {   
                _appDir = Path.GetDirectoryName(appDir);
            }
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
                writer.WriteLine(string.Format("{0} {1}", KeyArgs, string.Format("-w {0} -h {1} -d {2} -o {3} \"{4}\"",
                    _imageWidth, _imageHeight, _imageDpi, _outputName, this.InputFile)));
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

        private string GetInstalledInkscapePath()
        {
            const string exeSuffix = ".exe";

            string[] regPaths = {
                @"SOFTWARE\Classes\Inkscape.SVG\shell\open\command",
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\inkscape.exe",
                @"SOFTWARE\Classes\svgfile\shell\edit\command",
                @"SOFTWARE\Classes\svgfile\shell\Inkscape\command",
                @"SOFTWARE\Classes\svgfile\shell\open\command",
            };
            FileInfo browserPath = null;
            foreach (string regPath in regPaths)
            {
                using (RegistryKey pathKey = Registry.LocalMachine.OpenSubKey(regPath))
                {
                    if (pathKey != null)
                    {
                        try
                        {
                            // Trim quotes from parameters
                            string filePath = pathKey.GetValue(null).ToString().Replace("\"", "");
                            if (!filePath.EndsWith(exeSuffix, StringComparison.OrdinalIgnoreCase))
                            {
                                filePath = filePath.Substring(0, filePath.LastIndexOf(exeSuffix, StringComparison.OrdinalIgnoreCase) + exeSuffix.Length);
                                if (filePath.EndsWith(_appName, StringComparison.OrdinalIgnoreCase))
                                {
                                    browserPath = new FileInfo(filePath);
                                }
                            }
                            else if (!string.IsNullOrWhiteSpace(filePath))
                            {
                                if (filePath.EndsWith(_appName, StringComparison.OrdinalIgnoreCase))
                                {
                                    browserPath = new FileInfo(filePath);
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
                else if (browserPath != null)
                {
                    var filePath = Path.Combine(browserPath.DirectoryName, "bin", browserPath.Name);
                    if (File.Exists(filePath))
                    {
                        browserPath = new FileInfo(filePath);
                        break;
                    }
                }
            }
            if (browserPath != null && browserPath.Exists)
            {
                return browserPath.FullName;
            }

            regPaths = new string[] {
                @"Inkscape.SVG\shell\open\command",
                @"svgfile\shell\open\command",
                @"svgfile\shell\edit\command",
                @"svgfile\shell\Inkscape\command"
            };
            browserPath = null;
            foreach (string regPath in regPaths)
            {
                using (RegistryKey pathKey = Registry.ClassesRoot.OpenSubKey(regPath))
                {
                    if (pathKey != null)
                    {
                        try
                        {
                            // Trim quotes from parameters
                            string filePath = pathKey.GetValue(null).ToString().Replace("\"", "");
                            if (!filePath.EndsWith(exeSuffix, StringComparison.OrdinalIgnoreCase))
                            {
                                filePath = filePath.Substring(0, filePath.LastIndexOf(exeSuffix, StringComparison.OrdinalIgnoreCase) + exeSuffix.Length);
                                if (filePath.EndsWith(_appName, StringComparison.OrdinalIgnoreCase))
                                {
                                    browserPath = new FileInfo(filePath);
                                }
                            }
                            else if (!string.IsNullOrWhiteSpace(filePath))
                            {
                                if (filePath.EndsWith(_appName, StringComparison.OrdinalIgnoreCase))
                                {
                                    browserPath = new FileInfo(filePath);
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
                else if (browserPath != null)
                {
                    var filePath = Path.Combine(browserPath.DirectoryName, "bin", browserPath.Name);
                    if (File.Exists(filePath))
                    {
                        browserPath = new FileInfo(filePath);
                        break;
                    }
                }
            }
            if (browserPath != null && browserPath.Exists)
            {
                return browserPath.FullName;
            }

            regPaths = new string[] {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\inkscape.exe"
            };
            browserPath = null;
            foreach (string regPath in regPaths)
            {
                using (RegistryKey pathKey = Registry.CurrentUser.OpenSubKey(regPath))
                {
                    if (pathKey != null)
                    {
                        try
                        {
                            // Trim quotes from parameters
                            string filePath = pathKey.GetValue(null).ToString().Replace("\"", "");
                            if (!filePath.EndsWith(exeSuffix, StringComparison.OrdinalIgnoreCase))
                            {
                                filePath = filePath.Substring(0, filePath.LastIndexOf(exeSuffix, StringComparison.OrdinalIgnoreCase) + exeSuffix.Length);
                                if (filePath.EndsWith(_appName, StringComparison.OrdinalIgnoreCase))
                                {
                                    browserPath = new FileInfo(filePath);
                                }
                            }
                            else if (!string.IsNullOrWhiteSpace(filePath))
                            {
                                if (filePath.EndsWith(_appName, StringComparison.OrdinalIgnoreCase))
                                {
                                    browserPath = new FileInfo(filePath);
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
                else if (browserPath != null)
                {
                    var filePath = Path.Combine(browserPath.DirectoryName, "bin", browserPath.Name);
                    if (File.Exists(filePath))
                    {
                        browserPath = new FileInfo(filePath);
                        break;
                    }
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
