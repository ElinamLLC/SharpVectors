using System;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;

using FolderBrowserDialog = ShellFileDialogs.FolderBrowserDialog;

namespace WpfTestOtherSvg
{
    /// <summary>
    /// Interaction logic for PromptDialog.xaml
    /// </summary>
    public partial class PromptDialog : Window
    {
        private int _downloadedCount;
        private bool _cancelledOrError;
        private OptionSettings _optionSettings;

        public PromptDialog()
        {
            InitializeComponent();

            this.Loaded  += OnWindowLoaded;
            this.Closing += OnWindowClosing;
        }

        public OptionSettings OptionSettings
        {
            get {
                return _optionSettings;
            }
            set {
                if (value != null)
                {
                    _optionSettings = value;
                }
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (_optionSettings != null)
            {
                btnOK.IsEnabled       = OptionSettings.IsTestAvailable();
                btnDownload.IsEnabled = NetworkInterface.GetIsNetworkAvailable();
            }

            txtSvgSuitePath.Text    = _optionSettings.TestsDirectory;
            txtSvgSuitePathWeb.Text = "https://github.com/RazrFalcon/resvg-test-suite/archive/refs/heads/master.zip";

            UpdateStates();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
        }

        private void OnBrowseForSvgSuitePath(object sender, RoutedEventArgs e)
        {
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;
            string selectedDirectory = FolderBrowserDialog.ShowDialog(windowHandle,
                "Select the location of the Resvg SVG Test Suite", null);
            if (!string.IsNullOrWhiteSpace(selectedDirectory))
            {
                txtSvgSuitePath.Text = selectedDirectory;

                UpdateStates();
            }
        }

        private void OnOpenSvgSuitePath(object sender, RoutedEventArgs e)
        {
            var filePath = txtSvgSuitePath.Text;
            if (string.IsNullOrWhiteSpace(filePath) || Directory.Exists(filePath) == false)
            {
                return;
            }

            OptionSettings.OpenFolderAndSelectItem(filePath, null);
        }

        private void ProcessStyles()
        {
            try
            {
                var testCss      = Path.Combine(_optionSettings.TestsDirectory, "test.css");
                string sourceDir = Path.Combine(_optionSettings.TestsDirectory, "svg0");
                string destDir   = Path.Combine(_optionSettings.TestsDirectory, "svg");
                if (!File.Exists(testCss))
                {
                    Trace.TraceError("Source CSS file does not exist. " + testCss);
                    return;
                }
                if (!Directory.Exists(sourceDir))
                {
                    Trace.TraceError("SVG source directory does not exist. " + sourceDir);
                    return;
                }
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                var cssLines = File.ReadAllLines(testCss);
                if (cssLines == null || cssLines.Length == 0)
                {
                    Trace.TraceError("Source CSS file has no contents. " + testCss);
                    return;
                }

                const string KeyStart = "<title>";
                const string KeyEnd   = "</title>";
                //const string KeyOther = "</svg>";

                var sourceFiles = Directory.EnumerateFiles(sourceDir, "*.svg", SearchOption.TopDirectoryOnly);
                foreach (var sourceFile in sourceFiles)
                {
                    var isCssApplied = false;
                    var fileName     = Path.GetFileName(sourceFile);
                    string destFile  = Path.Combine(destDir, fileName);

                    using (var writer = File.CreateText(destFile))
                    {
                        using (var reader = File.OpenText(sourceFile))
                        {
                            string textLine;
                            while (!reader.EndOfStream)
                            {
                                textLine = reader.ReadLine();
                                if (textLine == null)
                                {
                                    break;
                                }
                                writer.WriteLine(textLine);
                                if (!isCssApplied)
                                {
                                    if (textLine.IndexOf(KeyStart) != -1 && textLine.IndexOf(KeyEnd) != 0)
                                    {
                                        writer.WriteLine();
                                        foreach (var cssLine in cssLines)
                                        {
                                            writer.WriteLine(cssLine);
                                        }

                                        isCssApplied = true;
                                    }
                                }
                            }
                        }
                    }
                }

                using (var writer = File.CreateText(Path.Combine(_optionSettings.TestsDirectory, "test.txt")))
                {
                    writer.WriteLine(DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                _cancelledOrError = true;
                Trace.TraceError(ex.ToString());
            } 
        }

        private delegate void ReportDownloadedDelegate(string targetName);

        private void ReportDownloaded(string targetName)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ReportDownloadedDelegate(ReportDownloaded), targetName);
                return;
            }

            _downloadedCount--;

            if (_downloadedCount <= 0)
            {
                if (!_cancelledOrError)
                {
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    LoadingAdorner.IsAdornerVisible = false;
                }
            }
        }
        private async void OnDownloadClicked(object sender, RoutedEventArgs e)
        {
            _downloadedCount  = 3;
            _cancelledOrError = false;

            string workingDir = _optionSettings.CacheDirectory;
            string appDir     = Path.GetDirectoryName(workingDir);

            string urlTests          = "https://github.com/RazrFalcon/resvg-test-suite/archive/refs/heads/master.zip";
            string downloadedTests   = Path.Combine(workingDir, "master.zip");
            string outputDirTests    = _optionSettings.TestsDirectory;
            string extractedDirTests = Path.Combine(workingDir, "resvg-test-suite-master");

            string urlBatik          = "https://www.apache.org/dyn/closer.cgi?filename=/xmlgraphics/batik/binaries/batik-bin-1.14.zip&action=download";
            string downloadedBatik   = Path.Combine(workingDir, "batik-bin-1.14.zip");
            string outputDirBatik    = _optionSettings.ToolsDirectory;
            string extractedDirBatik = Path.Combine(outputDirBatik, "batik-1.14");

            string urlResvg          = "https://github.com/RazrFalcon/resvg/releases/download/v0.15.0/viewsvg-win.zip";
            string downloadedResvg   = Path.Combine(workingDir, "viewsvg-win.zip");
            string outputDirResvg    = _optionSettings.ToolsDirectory;
            string extractedDirResvg = Path.Combine(outputDirResvg, "viewsvg-win");

            var isDownloadTests = !_optionSettings.IsTestAvailable();
            var isDownloadBatik = !Directory.Exists(extractedDirBatik) || !File.Exists(Path.Combine(extractedDirBatik, "batik-rasterizer-1.14.jar"));
            var isDownloadResvg = !Directory.Exists(extractedDirResvg) || !File.Exists(Path.Combine(extractedDirResvg, "resvg.exe"));

            if (!isDownloadTests)
            {
                _downloadedCount--;
            }
            if (!isDownloadBatik)
            {
                _downloadedCount--;
            }
            if (!isDownloadResvg)
            {
                _downloadedCount--;
            }

            if (_downloadedCount == 0)
            {
                return;
            }

            LoadingAdorner.IsAdornerVisible = true;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            if (isDownloadTests)
            {
                var cacheDir = _optionSettings.CacheDirectory;
                if (Directory.Exists(cacheDir))
                {
                    var dirs = Directory.GetDirectories(cacheDir);
                    if (dirs != null && dirs.Length != 0)
                    {
                        foreach (var dir in dirs)
                        {
                            try
                            {
                                Directory.Delete(dir, true);
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceError(ex.ToString());
                            }
                        }
                    }
                }

                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(urlTests, HttpCompletionOption.ResponseHeadersRead))
                    {
                        using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        {
                            using (Stream streamToWriteTo = File.Open(downloadedTests, FileMode.Create))
                            {
                                await streamToReadFrom.CopyToAsync(streamToWriteTo);
                            }
                        }

                        await Task.Run(() =>
                        {
                            ZipFile.ExtractToDirectory(downloadedTests, workingDir);
                            if (!Directory.Exists(outputDirTests))
                            {
                                Directory.CreateDirectory(outputDirTests);
                            }
                            else
                            {
                                var tempDir = Path.Combine(outputDirTests, "fonts");
                                if (Directory.Exists(tempDir))
                                {
                                    Directory.Delete(tempDir, true);
                                }
                                tempDir = Path.Combine(outputDirTests, "images");
                                if (Directory.Exists(tempDir))
                                {
                                    Directory.Delete(tempDir, true);
                                }
                                tempDir = Path.Combine(outputDirTests, "png");
                                if (Directory.Exists(tempDir))
                                {
                                    Directory.Delete(tempDir, true);
                                }
                                tempDir = Path.Combine(outputDirTests, "svg0");
                                if (Directory.Exists(tempDir))
                                {
                                    Directory.Delete(tempDir, true);
                                }
                                tempDir = Path.Combine(outputDirTests, "svg");
                                if (Directory.Exists(tempDir))
                                {
                                    Directory.Delete(tempDir, true);
                                }
                            }

                            if (!Directory.Exists(extractedDirTests))
                            {
                                Directory.Delete(extractedDirTests, true);
                            }

                            Directory.Move(Path.Combine(extractedDirTests, "fonts"), Path.Combine(outputDirTests, "fonts"));
                            Directory.Move(Path.Combine(extractedDirTests, "images"), Path.Combine(outputDirTests, "images"));
                            Directory.Move(Path.Combine(extractedDirTests, "png"), Path.Combine(outputDirTests, "png"));
                            Directory.Move(Path.Combine(extractedDirTests, "svg"), Path.Combine(outputDirTests, "svg0"));

                            Directory.Delete(extractedDirTests, true);
                        });

                        await Task.Run(() =>
                        {
                            if (File.Exists(downloadedTests))
                            {
                                File.Delete(downloadedTests);
                            }

                            ProcessStyles();

                            ReportDownloaded("resvg-test-suite");
                        });
                    }
                }
            }

            if (isDownloadBatik)
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(urlBatik, HttpCompletionOption.ResponseHeadersRead))
                    {
                        using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        {
                            using (Stream streamToWriteTo = File.Open(downloadedBatik, FileMode.Create))
                            {
                                await streamToReadFrom.CopyToAsync(streamToWriteTo);
                            }
                        }

                        await Task.Run(() =>
                        {
                            if (!Directory.Exists(outputDirBatik))
                            {
                                Directory.CreateDirectory(outputDirBatik);
                            }
                            if (Directory.Exists(extractedDirBatik))
                            {
                                Directory.Delete(extractedDirBatik, true);
                            }

                            ZipFile.ExtractToDirectory(downloadedBatik, outputDirBatik);
                        });

                        await Task.Run(() =>
                        {
                            if (File.Exists(downloadedBatik))
                            {
                                File.Delete(downloadedBatik);
                            }

                            ReportDownloaded("batik-bin-1.14");
                        });
                    }
                }
            }

            if (isDownloadResvg)
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(urlResvg, HttpCompletionOption.ResponseHeadersRead))
                    {
                        using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        {
                            using (Stream streamToWriteTo = File.Open(downloadedResvg, FileMode.Create))
                            {
                                await streamToReadFrom.CopyToAsync(streamToWriteTo);
                            }
                        }

                        await Task.Run(() =>
                        {
                            if (!Directory.Exists(outputDirResvg))
                            {
                                Directory.CreateDirectory(outputDirResvg);
                            }

                            if (Directory.Exists(extractedDirResvg))
                            {
                                Directory.Delete(extractedDirResvg, true);
                            }

                            ZipFile.ExtractToDirectory(downloadedResvg, extractedDirResvg);
                        });

                        await Task.Run(() =>
                        {
                            if (File.Exists(downloadedResvg))
                            {
                                File.Delete(downloadedResvg);
                            }
                            string vcRedist = Path.Combine(extractedDirResvg, "vc_redist.x64.exe");
                            if (File.Exists(vcRedist))
                            {
                                File.Delete(vcRedist);
                            }

                            ReportDownloaded("viewsvg-win");
                        });
                    }
                }
            }
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OnOKClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void OnSvgSuitePathTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateStates();
        }

        private void OnSvgSuitePathFocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateStates();
        }

        private void OnHyperlinkRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var startInfo = new ProcessStartInfo(e.Uri.AbsoluteUri);
#if NETCORE
            // The default is true on .NET Framework apps and false on .NET Core apps.
            startInfo.UseShellExecute = true;
#endif
            Process.Start(startInfo);
            e.Handled = true;
        }

        private void UpdateStates()
        {
            if (!this.IsLoaded)
            {
                return;
            }

            string selectePath = txtSvgSuitePath.Text;
            if (selectePath != null)
            {
                selectePath = selectePath.Trim();
            }
            if (string.IsNullOrWhiteSpace(selectePath) || !Directory.Exists(selectePath))
            {
                btnOK.IsEnabled         = false;
                btnDownload.IsEnabled   = false;
                btnPathLocate.IsEnabled = false;

                return;
            }

            btnPathLocate.IsEnabled = true;
            btnDownload.IsEnabled   = NetworkInterface.GetIsNetworkAvailable();

            btnOK.IsEnabled         = OptionSettings.IsTestAvailable();
        }
    }
}
