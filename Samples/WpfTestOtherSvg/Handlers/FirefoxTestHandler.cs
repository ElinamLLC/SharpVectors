using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Win32;

namespace WpfTestOtherSvg.Handlers
{
    public sealed class FirefoxTestHandler : SvgTestHandler
    {
        public const string FileName = "firefox.exe";

        public FirefoxTestHandler(string inputDir, string workingDir)
            : base(FileName, TagFirefox, inputDir, workingDir)
        {
            var browsers = BrowserInfo.GetBrowsers("firefox");
            if (browsers.Count == 1)
            {
                string appDir = browsers[0].Path;
                _appDir = Path.GetDirectoryName(appDir);
            }
            else
            {
                var currentBrowser = browsers[0];
                for (int i = 1; i < browsers.Count; i++)
                {
                    var browser = browsers[i];
                    if (browser.Version > currentBrowser.Version)
                    {
                        currentBrowser = browser;
                    }
                }
                string appDir = currentBrowser.Path;
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
                writer.WriteLine(string.Format("{0} {1}", KeyArgs, string.Format("--screenshot={0} --window-size={1},{2} \"{3}\"", 
                    _outputName, _imageWidth, _imageHeight, this.InputFile)));
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

        private sealed class BrowserInfo
        {
            public BrowserInfo()
            {
            }

            public static IList<BrowserInfo> GetBrowsers(string nameFilter = null)
            {
                // For the 64-bits browsers...
                var browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");
                if (browserKeys == null)
                {
                    // Try the the 32-bits browsers
                    browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
                }
                var browserNames = browserKeys.GetSubKeyNames();
                var browsers = new List<BrowserInfo>();
                for (int i = 0; i < browserNames.Length; i++)
                {
                    var browserKey = browserKeys.OpenSubKey(browserNames[i]);
                    var browserName = (string)browserKey.GetValue(null);
                    if (!string.IsNullOrWhiteSpace(nameFilter) && browserName.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        continue;
                    }
                    var browserKeyPath = browserKey.OpenSubKey(@"shell\open\command");
                    var installedPath = BrowserInfo.StripQuotes(browserKeyPath.GetValue(null).ToString());
                    if (!string.IsNullOrWhiteSpace(installedPath) && File.Exists(installedPath))
                    {
                        var browserIconPath = browserKey.OpenSubKey(@"DefaultIcon");

                        BrowserInfo browser = new BrowserInfo();
                        browser.Name = browserName;
                        browser.Path = BrowserInfo.StripQuotes(browserKeyPath.GetValue(null).ToString());
                        browser.IconPath = BrowserInfo.StripQuotes(browserIconPath.GetValue(null).ToString());

                        // Split for stuff like: 11.00.19041.1100 (WinBuild.160101.0800)
                        var versionParts = FileVersionInfo.GetVersionInfo(installedPath).FileVersion.Split();
                        browser.Version = Version.Parse(versionParts[0]);

                        browsers.Add(browser);
                    }
                }
                return browsers;
            }

            public string Name { get; set; }
            public string Path { get; set; }
            public string IconPath { get; set; }
            public Version Version { get; set; }

            static string StripQuotes(string info)
            {
                if (info.EndsWith("\"") && info.StartsWith("\""))
                {
                    return info.Substring(1, info.Length - 2);
                }
                return info;
            }
        }
    }
}
