using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace WpfTestThreadSafety
{
    public sealed class ImageData
    {
        private int _index;
        private string _fileName;
        private Drawing _drawing;

        public ImageData()
        {
        }

        public ImageData(int index, Drawing drawing, string fileName)
        {
            _index    = index;
            _drawing  = drawing;
            _fileName = fileName;
        }

        public int Index
        {
            get {
                return _index;
            }
        }

        public string Name
        {
            get {
                return string.Format("{0:D3}: {1}" , _index + 1, _fileName);
            }
        }

        public ImageSource Image
        {
            get {
                return new DrawingImage(_drawing);
            }
        }       
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void AppendImageDelegate(Drawing drawing, string fileName);
        private delegate void AppendTextDelegate(string msg, string style);

        private const string W3CDirPrefix = "Svg";
        private const string LocalDirBase = @"..\..\..\W3cSvgTestSuites\";

        [DllImport("Shlwapi.dll", EntryPoint = "PathIsDirectoryEmpty")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsDirectoryEmpty([MarshalAs(UnmanagedType.LPStr)]string directory);

        private const int NumberOfImages    = 300;
        private const int NumberOfColumns   = 100;
        private const int NumberOfConsumers = 8;

        private bool _isVerbose;
        private int _columnCount;
        private int _imageCount;

        private ObservableCollection<ImageData> _imageList;

        public MainWindow()
        {
            InitializeComponent();

            _imageList = new ObservableCollection<ImageData>();
            this.DataContext = this;
        }

        public ObservableCollection<ImageData> ImageList
        {
            get {
                return _imageList;
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.SetValidSvgDir();

        }

        private void OnWindowClosed(object sender, EventArgs e)
        {

        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void SetValidSvgDir()
        {
            string svgDir;
            if (GetValidSvgTestsDir(out svgDir, 1, 1))
            {
                txtSvgSource.Text = svgDir;
                return;
            }
            if (GetValidSvgTestsDir(out svgDir, 1, 2))
            {
                txtSvgSource.Text = svgDir;
                return;
            }
            if (GetValidSvgTestsDir(out svgDir, 1, 0)) // this has fewer that 300 images
            {
                txtSvgSource.Text = svgDir;
                return;
            }
        }

        private void UpdateDrawing(string svgFilePath)
        {
            var wpfSettings = new WpfDrawingSettings();
            wpfSettings.CultureInfo = wpfSettings.NeutralCultureInfo;
            using (var textReader = new StreamReader(svgFilePath))
            {
                using (var fileReader = new FileSvgReader(wpfSettings))
                {
                    _imageCount++;
                    try
                    {
                        if (_isVerbose)
                        {
                            this.AppendLine("Start Converting: " + svgFilePath);
                        }
                        else
                        {
                            _columnCount++;
                            AppendText("*");
                            if (_columnCount >= NumberOfColumns)
                            {
                                _columnCount = 0;
                                if (_imageCount < NumberOfImages)
                                {
                                    AppendLine("");
                                }
                            }
                        }

                        fileReader.SaveXaml = false;
                        fileReader.SaveZaml = false;
                        var drawing = fileReader.Read(textReader);
                        drawing.Freeze();

                        AppendImage(drawing, Path.GetFileNameWithoutExtension(svgFilePath));

                        if (_isVerbose)
                        {
                            this.AppendLine("Completed Converting: " + svgFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (_isVerbose)
                        {
                            AppendClear();
                        }
                        this.AppendLine("File: " + svgFilePath);
                        AppendError(ex.ToString());
                    }
                }
            }
        }

        public void AppendError(string msg)
        {
            AppendLine(msg, "Error");
        }

        public void AppendClear()
        {
            AppendLine(string.Empty, "Clear");
        }

        private void AppendImage(Drawing drawing, string fileName)
        {
            if (Dispatcher.CheckAccess())
            {
                _imageList.Add(new ImageData(_imageList.Count, drawing, fileName));
            }
            else
            {
                Dispatcher.Invoke(new AppendImageDelegate(AppendImage), drawing, fileName);
            }
        }

        private void AppendText(string msg, string style = "")
        {
            if (Dispatcher.CheckAccess())
            {
                txtDebug.AppendText(msg);
            }
            else
            {
                Dispatcher.Invoke(new AppendTextDelegate(AppendText), msg, string.Empty);
            }
        }

        private void AppendLine(string msg, string style = null)
        {
            if (Dispatcher.CheckAccess())
            {
                if (string.IsNullOrWhiteSpace(style))
                {
                    txtDebug.AppendText(msg + Environment.NewLine);
                }
                else
                {
                    if (style.Equals("Clear", StringComparison.OrdinalIgnoreCase))
                    {
                        txtDebug.Clear();
                        return;
                    }
                    else
                    {
                        txtDebug.AppendText(style + ": " + msg + Environment.NewLine);
                    }
                }

                txtDebug.CaretIndex = txtDebug.Text.Length;
                txtDebug.ScrollToEnd();
            }
            else
            {
                Dispatcher.Invoke(new AppendTextDelegate(AppendLine), msg, style);
            }
        }

        private static bool GetValidSvgTestsDir(out string svgDir, int majorVersion, int minorVersion)
        {
            svgDir = "";

            string versionSuffix = string.Format("{0}{1}", majorVersion, minorVersion);
            var suiteDirName = W3CDirPrefix + versionSuffix;
            var localSuitePath = Path.GetFullPath(Path.Combine(LocalDirBase, suiteDirName));
            if (Directory.Exists(localSuitePath) == false)
            {
                return false;
            }
            if (IsTestSuiteAvailable(localSuitePath))
            {
                svgDir = Path.Combine(localSuitePath, "svg");
                return true;
            }
            return false;
        }

        private static bool IsTestSuiteAvailable(string testPath)
        {
            if (string.IsNullOrWhiteSpace(testPath) || Directory.Exists(testPath) == false)
            {
                return false;
            }
            string svgDir = Path.Combine(testPath, "svg");
            if (!Directory.Exists(svgDir) || IsDirectoryEmpty(svgDir) == true)
            {
                return false;
            }
            string pngDir = Path.Combine(testPath, "png");
            if (!Directory.Exists(pngDir) || IsDirectoryEmpty(pngDir) == true)
            {
                return false;
            }

            return true;
        }

        private async void OnStartClick(object sender, RoutedEventArgs e)
        {
            string svgPath = txtSvgSource.Text;
            if (string.IsNullOrWhiteSpace(svgPath) || Directory.Exists(svgPath) == false)
            {
                return;
            }

            txtDebug.Clear();
            _imageList.Clear();

            _columnCount = 0;
            _imageCount  = 0;
            _isVerbose   = (chkVerbose.IsChecked != null && chkVerbose.IsChecked.Value);

            btnStart.IsEnabled = false;
            chkVerbose.IsEnabled = false;

            var queue = new ConcurrentQueue<string>();

            int imageCount = 0;
            foreach (var svgFilePath in Directory.EnumerateFiles(svgPath, "*.svg"))
            {
                // Eliminate any compressed file, not supported in this test...
                if (svgFilePath.EndsWith(".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                queue.Enqueue(svgFilePath);

                imageCount++;
                if (imageCount >= NumberOfImages)
                {
                    break;
                }
            }

            AppendLine("Starting Tests ****");
            if (_isVerbose)
            {
                AppendLine("");
            }

            var allTasks = new List<Task>();

            for (int i = 0; i < NumberOfConsumers; ++i)
            {
                allTasks.Add(Task.Run(() =>
                {
                    while (queue.TryDequeue(out string imageData))
                    {
                        UpdateDrawing(imageData);
                    }
                }));
            }

            await Task.WhenAll(allTasks);

            AppendLine("");
            AppendLine("**** Completed Tests");

            btnStart.IsEnabled = true;
            chkVerbose.IsEnabled = true;
        }
    }
}
