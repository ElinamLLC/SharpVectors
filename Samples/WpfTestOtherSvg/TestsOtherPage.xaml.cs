using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.IO;
using System.IO.Pipes;
using System.IO.Compression;

using System.Windows;
using System.Windows.Controls;

using SharpVectors.Converters;
using WpfTestOtherSvg.Handlers;

namespace WpfTestOtherSvg
{
    public class TestImage
    {
        private Uri _uri;
        private string _title;

        public TestImage()
        {
        }

        public TestImage(string toolName, string imagePath)
        {
            _title = toolName;
            _uri   = new Uri(imagePath);
        }

        public string ImageTitle
        {
            get { return _title; }
            set { _title = value; }
        }

        public Uri ImageUri
        {
            get { return _uri; }
            set { _uri = value; }
        }
    }

    /// <summary>
    /// Interaction logic for TestsOtherPage.xaml
    /// </summary>
    public partial class TestsOtherPage : Page
    {
        #region Public Fields

        public const int ImageWidth  = 350;
        public const int ImageHeight = 350;

        private const int WaitTime   = 1000 * 60 * 3;

        // Microsoft's "stack overflow/stack exhaustion" error code 0xC00000FD (-1073741571).
        private const int ExitSuccess       = 0;
        private const int ExitStackOverflow = -1073741571;

        #endregion

        private bool _isLoadingImages;
        private string _svgFilePath;

        private OptionSettings _optionSettings;

        private ObservableCollection<TestImage> _testImages;

        public TestsOtherPage()
        {
            InitializeComponent();

            _testImages = new ObservableCollection<TestImage>();
            this.DataContext = this;
        }

        public ObservableCollection<TestImage> TestImages
        {
            get {
                return _testImages;
            }
        }

        public void PageSelected(bool isSelected)
        {
        }

        public Task<bool> LoadDocumentAsync(OptionSettings optionSettings, string svgFilePath)
        {
            if (_isLoadingImages || string.IsNullOrWhiteSpace(svgFilePath) || !File.Exists(svgFilePath))
            {
#if DOTNET40
                return TaskEx.FromResult<bool>(false);
#else
                return Task.FromResult<bool>(false);
#endif
            }

            string fileExt = Path.GetExtension(svgFilePath);

            if (!(string.Equals(fileExt, SvgConverter.SvgExt, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fileExt, SvgConverter.CompressedSvgExt, StringComparison.OrdinalIgnoreCase)))
            {
#if DOTNET40
                return TaskEx.FromResult<bool>(false);
#else
                return Task.FromResult<bool>(false);
#endif
            }

            _optionSettings  = optionSettings;
            _isLoadingImages = true;
            _svgFilePath     = svgFilePath;

            _testImages.Clear();

            List<TestImage> testImages = new List<TestImage>();

            // Get the UI thread's context
            var context = TaskScheduler.FromCurrentSynchronizationContext();

            return Task<bool>.Factory.StartNew((Func<bool>)(() =>
            {
                string svgFileName = Path.GetFileName(svgFilePath);

                string imageFile;
                imageFile = ProcessSvgnet(svgFileName);
                if (!string.IsNullOrWhiteSpace(imageFile) && File.Exists(imageFile))
                {
                    testImages.Add((TestImage)new TestImage("SVG-NET", GetPng(imageFile)));
                }
                imageFile = ProcessBatik(svgFileName);
                if (!string.IsNullOrWhiteSpace(imageFile) && File.Exists(imageFile))
                {
                    testImages.Add((TestImage)new TestImage("Apache Batik", GetPng(imageFile)));
                }
                imageFile = ProcessResvg(svgFileName);
                if (!string.IsNullOrWhiteSpace(imageFile) && File.Exists(imageFile))
                {
                    testImages.Add((TestImage)new TestImage("Resvg", GetPng(imageFile)));
                }
                imageFile = ProcessFirefox(svgFileName);
                if (!string.IsNullOrWhiteSpace(imageFile) && File.Exists(imageFile))
                {
                    testImages.Add((TestImage)new TestImage("Firefox", GetPng(imageFile)));
                }
                imageFile = ProcessInkscape(svgFileName);
                if (!string.IsNullOrWhiteSpace(imageFile) && File.Exists(imageFile))
                {
                    testImages.Add((TestImage)new TestImage("Inkscape", GetPng(imageFile)));
                }
                imageFile = ProcessMagick(svgFileName);
                if (!string.IsNullOrWhiteSpace(imageFile) && File.Exists(imageFile))
                {
                    testImages.Add((TestImage)new TestImage("Image Magick", GetPng(imageFile)));
                } 
                else
                {
                    imageFile = ProcessRsvg(svgFileName);
                    if (!string.IsNullOrWhiteSpace(imageFile) && File.Exists(imageFile))
                    {
                        testImages.Add((TestImage)new TestImage("Librsvg", GetPng(imageFile)));
                    }
                }

                return testImages.Count != 0;
            })).ContinueWith((t) => {
                try
                {
                    if (!t.Result)
                    {
                        _isLoadingImages = false;
                        _svgFilePath = null;
                        return false;
                    }

                    foreach (var testImage in testImages)
                    {
                        _testImages.Add(testImage);
                    }

                    _isLoadingImages = false;

                    return true;
                }
                catch
                {
                    _isLoadingImages = false;
                    throw;
                }
            }, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <remarks>
        /// <para>A PNG file starts with an 8-byte signature[10] (refer to hex editor image on the right):</para>
        /// <para>
        /// <see href="https://en.wikipedia.org/wiki/Portable_Network_Graphics#File_header">Portable Network Graphics</see>
        /// </para>
        /// </remarks>
        private string GetPng(string file)
        {
            var isPng = false;
            using (var stream = File.OpenRead(file))
            {
                isPng = stream.ReadByte() == 137; // or 89 (HEX)
            }
            if (isPng)
            {
                return file;
            }
            return _optionSettings.EmptyImageFile;
        }

        private string ProcessSvgnet(string svgFileName)
        {
            var svgInputDir = _optionSettings.SvgDirectory;
            var workingDir  = _optionSettings.CacheDirectory;
            var toolsDir    = _optionSettings.ToolsDirectory;

            var appDir      = Path.Combine(toolsDir, "Svgnet");

            var testHandler = new SvgnetTestHandler(svgInputDir, workingDir, appDir);

            testHandler.Initialize(svgFileName);
            if (!testHandler.IsInitialized)
            {
                return null;
            }
            if (File.Exists(testHandler.OutputFile))
            {
                return testHandler.OutputFile;
            }

            var outputBuilder  = new StringBuilder();
            Process pipeClient = new Process();

            pipeClient.StartInfo.FileName               = _optionSettings.TestRunnerFile;
            pipeClient.StartInfo.RedirectStandardError  = true;
            pipeClient.StartInfo.RedirectStandardOutput = true;
            pipeClient.StartInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            pipeClient.StartInfo.CreateNoWindow         = true;
            pipeClient.StartInfo.UseShellExecute        = false;
            pipeClient.EnableRaisingEvents              = false;

            pipeClient.OutputDataReceived += (sender, eventArgs) => 
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data); 
                }
            };
            pipeClient.ErrorDataReceived += (sender, eventArgs) =>
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data);
                }
            };

            using (var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            {
                // Pass the client process a handle to the server.
                pipeClient.StartInfo.Arguments = pipeServer.GetClientHandleAsString();
                pipeClient.Start();
                pipeClient.BeginOutputReadLine();
                pipeClient.BeginErrorReadLine();

                pipeServer.DisposeLocalCopyOfClientHandle();

                try
                {
                    // Read user input and send that to the client process.
                    using (StreamWriter writer = new StreamWriter(pipeServer))
                    {
                        writer.AutoFlush = true;
                        // Send a 'sync message' and wait for client to receive it.
                        writer.WriteLine(SvgTestHandler.KeySync);
                        pipeServer.WaitForPipeDrain();
                        // Send the console input to the client process.
                        testHandler.Marshal(writer);
                    }
                }                
                catch (IOException ex) 
                {
                    // Catch the IOException that is raised if the pipe is broken or disconnected.
                    Trace.TraceError("[Server] Error: {0}", ex.Message);
                }
            }

            var processExited = pipeClient.WaitForExit(WaitTime);
            if (processExited == false) // process is timed out...
            {
                pipeClient.Kill();
                pipeClient.Close();
                return _optionSettings.CrashImageFile;
            }
            var exitCode = pipeClient.ExitCode;
            //Trace.WriteLine("ExitCode: " + pipeClient.ExitCode);

            pipeClient.Close();
            //Trace.WriteLine("[Server] Client quit. Server terminating.");

            var outputText = outputBuilder.ToString().Trim();
            if (outputText.Length != 0)
            {
                Trace.WriteLine(string.Empty);
                Trace.WriteLine("###***** Svg-Net *****");
                Trace.WriteLine(outputText);
            }

            if (exitCode == ExitSuccess && File.Exists(testHandler.OutputFile))
            {
                return testHandler.OutputFile;
            }
            if (exitCode == ExitStackOverflow)
            {
                return _optionSettings.CrashImageFile;
            }
            return _optionSettings.EmptyImageFile;
        }

        private string ProcessBatik(string svgFileName)
        {
            var svgInputDir = _optionSettings.SvgDirectory;
            var workingDir  = _optionSettings.CacheDirectory;
            var toolsDir    = _optionSettings.ToolsDirectory;

            var appDir      = Path.Combine(toolsDir, "batik-1.14");

            var testHandler = new BatikTestHandler(svgInputDir, workingDir, appDir);

            testHandler.Initialize(svgFileName);
            if (!testHandler.IsInitialized)
            {
                return null;
            }
            if (File.Exists(testHandler.OutputFile))
            {
                return testHandler.OutputFile;
            }

            var outputBuilder  = new StringBuilder();
            Process pipeClient = new Process();

            pipeClient.StartInfo.FileName               = _optionSettings.TestRunnerFile;
            pipeClient.StartInfo.RedirectStandardError  = true;
            pipeClient.StartInfo.RedirectStandardOutput = true;
            pipeClient.StartInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            pipeClient.StartInfo.CreateNoWindow         = true;
            pipeClient.StartInfo.UseShellExecute        = false;
            pipeClient.EnableRaisingEvents              = false;

            pipeClient.OutputDataReceived += (sender, eventArgs) => 
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data); 
                }
            };
            pipeClient.ErrorDataReceived += (sender, eventArgs) =>
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data);
                }
            };

            using (var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            {
                // Pass the client process a handle to the server.
                pipeClient.StartInfo.Arguments = pipeServer.GetClientHandleAsString();
                pipeClient.Start();
                pipeClient.BeginOutputReadLine();
                pipeClient.BeginErrorReadLine();

                pipeServer.DisposeLocalCopyOfClientHandle();

                try
                {
                    // Read user input and send that to the client process.
                    using (StreamWriter writer = new StreamWriter(pipeServer))
                    {
                        writer.AutoFlush = true;
                        // Send a 'sync message' and wait for client to receive it.
                        writer.WriteLine(SvgTestHandler.KeySync);
                        pipeServer.WaitForPipeDrain();
                        // Send the console input to the client process.
                        testHandler.Marshal(writer);
                    }
                }
                catch (IOException ex)
                {
                    // Catch the IOException that is raised if the pipe is broken or disconnected.
                    Trace.TraceError("[Server] Error: {0}", ex.Message);
                }
            }

            var processExited = pipeClient.WaitForExit(WaitTime);
            if (processExited == false) // process is timed out...
            {
                pipeClient.Kill();
                pipeClient.Close();
                return _optionSettings.CrashImageFile;
            }
            var exitCode = pipeClient.ExitCode;
            //Trace.WriteLine("ExitCode: " + pipeClient.ExitCode);

            pipeClient.Close();
            //Trace.WriteLine("[Server] Client quit. Server terminating.");

            var outputText = outputBuilder.ToString().Trim();
            if (outputText.Length != 0)
            {
                Trace.WriteLine(string.Empty);
                Trace.WriteLine("###***** Apache Batik *****");
                Trace.WriteLine(outputText);
            }

            if (exitCode == ExitSuccess && File.Exists(testHandler.OutputFile))
            {
                return testHandler.OutputFile;
            }
            if (exitCode == ExitStackOverflow)
            {
                return _optionSettings.CrashImageFile;
            }
            return _optionSettings.EmptyImageFile;
        }

        private string ProcessResvg(string svgFileName)
        {
            var svgInputDir = _optionSettings.SvgDirectory;
            var workingDir  = _optionSettings.CacheDirectory;
            var toolsDir    = _optionSettings.ToolsDirectory;

            var appDir      = Path.Combine(toolsDir, "viewsvg-win");

            var testHandler = new ResvgTestHandler(svgInputDir, workingDir, appDir);

            testHandler.Initialize(svgFileName);
            if (!testHandler.IsInitialized)
            {
                return null;
            }
            if (File.Exists(testHandler.OutputFile))
            {
                return testHandler.OutputFile;
            }

            var outputBuilder  = new StringBuilder();
            Process pipeClient = new Process();

            pipeClient.StartInfo.FileName               = _optionSettings.TestRunnerFile;
            pipeClient.StartInfo.RedirectStandardError  = true;
            pipeClient.StartInfo.RedirectStandardOutput = true;
            pipeClient.StartInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            pipeClient.StartInfo.CreateNoWindow         = true;
            pipeClient.StartInfo.UseShellExecute        = false;
            pipeClient.EnableRaisingEvents              = false;

            pipeClient.OutputDataReceived += (sender, eventArgs) => 
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data); 
                }
            };
            pipeClient.ErrorDataReceived += (sender, eventArgs) =>
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data);
                }
            };

            using (var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            {
                // Pass the client process a handle to the server.
                pipeClient.StartInfo.Arguments = pipeServer.GetClientHandleAsString();
                pipeClient.Start();
                pipeClient.BeginOutputReadLine();
                pipeClient.BeginErrorReadLine();

                pipeServer.DisposeLocalCopyOfClientHandle();

                try
                {
                    // Read user input and send that to the client process.
                    using (StreamWriter writer = new StreamWriter(pipeServer))
                    {
                        writer.AutoFlush = true;
                        // Send a 'sync message' and wait for client to receive it.
                        writer.WriteLine(SvgTestHandler.KeySync);
                        pipeServer.WaitForPipeDrain();
                        // Send the console input to the client process.
                        testHandler.Marshal(writer);
                    }
                }
                catch (IOException ex)
                {
                    // Catch the IOException that is raised if the pipe is broken or disconnected.
                    Trace.TraceError("[Server] Error: {0}", ex.Message);
                }
            }

            var processExited = pipeClient.WaitForExit(WaitTime);
            if (processExited == false) // process is timed out...
            {
                pipeClient.Kill();
                pipeClient.Close();
                return _optionSettings.CrashImageFile;
            }
            var exitCode = pipeClient.ExitCode;
            //Trace.WriteLine("ExitCode: " + pipeClient.ExitCode);

            pipeClient.Close();
            //Trace.WriteLine("[Server] Client quit. Server terminating.");

            var outputText = outputBuilder.ToString().Trim();
            if (outputText.Length != 0)
            {
                Trace.WriteLine(string.Empty);
                Trace.WriteLine("###***** Resvg *****");
                Trace.WriteLine(outputText);
            }

            if (exitCode == ExitSuccess && File.Exists(testHandler.OutputFile))
            {
                return testHandler.OutputFile;
            }
            if (exitCode == ExitStackOverflow)
            {
                return _optionSettings.CrashImageFile;
            }
            return _optionSettings.EmptyImageFile;
        }

        private string ProcessFirefox(string svgFileName)
        {
            var svgInputDir = _optionSettings.SvgDirectory;
            var workingDir  = _optionSettings.CacheDirectory;

            var testHandler = new FirefoxTestHandler(svgInputDir, workingDir);

            testHandler.Initialize(svgFileName);
            if (!testHandler.IsInitialized)
            {
                return null;
            }
            if (File.Exists(testHandler.OutputFile))
            {
                return testHandler.OutputFile;
            }

            var outputBuilder  = new StringBuilder();
            Process pipeClient = new Process();

            pipeClient.StartInfo.FileName               = _optionSettings.TestRunnerFile;
            pipeClient.StartInfo.RedirectStandardError  = true;
            pipeClient.StartInfo.RedirectStandardOutput = true;
            pipeClient.StartInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            pipeClient.StartInfo.CreateNoWindow         = true;
            pipeClient.StartInfo.UseShellExecute        = false;
            pipeClient.EnableRaisingEvents              = false;

            pipeClient.OutputDataReceived += (sender, eventArgs) => 
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data); 
                }
            };
            pipeClient.ErrorDataReceived += (sender, eventArgs) =>
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data);
                }
            };

            using (var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            {
                // Pass the client process a handle to the server.
                pipeClient.StartInfo.Arguments = pipeServer.GetClientHandleAsString();
                pipeClient.Start();
                pipeClient.BeginOutputReadLine();
                pipeClient.BeginErrorReadLine();

                pipeServer.DisposeLocalCopyOfClientHandle();

                try
                {
                    // Read user input and send that to the client process.
                    using (StreamWriter writer = new StreamWriter(pipeServer))
                    {
                        writer.AutoFlush = true;
                        // Send a 'sync message' and wait for client to receive it.
                        writer.WriteLine(SvgTestHandler.KeySync);
                        pipeServer.WaitForPipeDrain();
                        // Send the console input to the client process.
                        testHandler.Marshal(writer);
                    }
                }
                catch (IOException ex)
                {
                    // Catch the IOException that is raised if the pipe is broken or disconnected.
                    Trace.TraceError("[Server] Error: {0}", ex.Message);
                }
            }

            var processExited = pipeClient.WaitForExit(WaitTime);
            if (processExited == false) // process is timed out...
            {
                pipeClient.Kill();
                pipeClient.Close();
                return _optionSettings.CrashImageFile;
            }
            var exitCode = pipeClient.ExitCode;
            //Trace.WriteLine("ExitCode: " + pipeClient.ExitCode);

            pipeClient.Close();
            //Trace.WriteLine("[Server] Client quit. Server terminating.");

            var outputText = outputBuilder.ToString().Trim();
            if (outputText.Length != 0)
            {
                Trace.WriteLine(string.Empty);
                Trace.WriteLine("###***** Firefox *****");
                Trace.WriteLine(outputText);
            }

            if (exitCode == ExitSuccess)
            {
                if (File.Exists(testHandler.OutputFile))
                {
                    return testHandler.OutputFile;
                }
                while (!File.Exists(testHandler.OutputFile))
                {
                    System.Threading.Thread.Sleep(100);
                }
                return testHandler.OutputFile;
            }
            if (exitCode == ExitStackOverflow)
            {
                return _optionSettings.CrashImageFile;
            }
            return _optionSettings.EmptyImageFile;
        }

        private string ProcessInkscape(string svgFileName)
        {
            var svgInputDir = _optionSettings.SvgDirectory;
            var workingDir  = _optionSettings.CacheDirectory;

            var testHandler = new InkscapeTestHandler(svgInputDir, workingDir);

            testHandler.Initialize(svgFileName);
            if (!testHandler.IsInitialized)
            {
                return null;
            }
            if (File.Exists(testHandler.OutputFile))
            {
                return testHandler.OutputFile;
            }

            var outputBuilder  = new StringBuilder();
            Process pipeClient = new Process();

            pipeClient.StartInfo.FileName               = _optionSettings.TestRunnerFile;
            pipeClient.StartInfo.RedirectStandardError  = true;
            pipeClient.StartInfo.RedirectStandardOutput = true;
            pipeClient.StartInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            pipeClient.StartInfo.CreateNoWindow         = true;
            pipeClient.StartInfo.UseShellExecute        = false;
            pipeClient.EnableRaisingEvents              = false;

            pipeClient.OutputDataReceived += (sender, eventArgs) => 
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data); 
                }
            };
            pipeClient.ErrorDataReceived += (sender, eventArgs) =>
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data);
                }
            };

            using (var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            {
                // Pass the client process a handle to the server.
                pipeClient.StartInfo.Arguments = pipeServer.GetClientHandleAsString();
                pipeClient.Start();
                pipeClient.BeginOutputReadLine();
                pipeClient.BeginErrorReadLine();

                pipeServer.DisposeLocalCopyOfClientHandle();

                try
                {
                    // Read user input and send that to the client process.
                    using (StreamWriter writer = new StreamWriter(pipeServer))
                    {
                        writer.AutoFlush = true;
                        // Send a 'sync message' and wait for client to receive it.
                        writer.WriteLine(SvgTestHandler.KeySync);
                        pipeServer.WaitForPipeDrain();
                        // Send the console input to the client process.
                        testHandler.Marshal(writer);
                    }
                }
                catch (IOException ex)
                {
                    // Catch the IOException that is raised if the pipe is broken or disconnected.
                    Trace.TraceError("[Server] Error: {0}", ex.Message);
                }
            }

            var processExited = pipeClient.WaitForExit(WaitTime);
            if (processExited == false) // process is timed out...
            {
                pipeClient.Kill();
                pipeClient.Close();
                return _optionSettings.CrashImageFile;
            }
            var exitCode = pipeClient.ExitCode;
            //Trace.WriteLine("ExitCode: " + pipeClient.ExitCode);

            pipeClient.Close();
            //Trace.WriteLine("[Server] Client quit. Server terminating.");

            var outputText = outputBuilder.ToString().Trim();
            if (outputText.Length != 0)
            {
                Trace.WriteLine(string.Empty);
                Trace.WriteLine("###***** Inkscape *****");
                Trace.WriteLine(outputText);
            }

            if (exitCode == ExitSuccess && File.Exists(testHandler.OutputFile))
            {
                return testHandler.OutputFile;
            }
            if (exitCode == ExitStackOverflow)
            {
                return _optionSettings.CrashImageFile;
            }
            return _optionSettings.EmptyImageFile;
        }

        private string ProcessMagick(string svgFileName)
        {
            var svgInputDir = _optionSettings.SvgDirectory;
            var workingDir  = _optionSettings.CacheDirectory;
            var toolsDir    = _optionSettings.ToolsDirectory;

            var appDir      = _optionSettings.MagickDirectory;

            var testHandler = new MagickTestHandler(svgInputDir, workingDir, appDir);

            testHandler.Initialize(svgFileName);
            if (!testHandler.IsInitialized)
            {
                return null;
            }
            if (File.Exists(testHandler.OutputFile))
            {
                return testHandler.OutputFile;
            }

            var outputBuilder  = new StringBuilder();
            Process pipeClient = new Process();

            pipeClient.StartInfo.FileName               = _optionSettings.TestRunnerFile;
            pipeClient.StartInfo.RedirectStandardError  = true;
            pipeClient.StartInfo.RedirectStandardOutput = true;
            pipeClient.StartInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            pipeClient.StartInfo.CreateNoWindow         = true;
            pipeClient.StartInfo.UseShellExecute        = false;
            pipeClient.EnableRaisingEvents              = false;

            pipeClient.OutputDataReceived += (sender, eventArgs) => 
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data); 
                }
            };
            pipeClient.ErrorDataReceived += (sender, eventArgs) =>
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data);
                }
            };

            using (var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            {
                // Pass the client process a handle to the server.
                pipeClient.StartInfo.Arguments = pipeServer.GetClientHandleAsString();
                pipeClient.Start();
                pipeClient.BeginOutputReadLine();
                pipeClient.BeginErrorReadLine();

                pipeServer.DisposeLocalCopyOfClientHandle();

                try
                {
                    // Read user input and send that to the client process.
                    using (StreamWriter writer = new StreamWriter(pipeServer))
                    {
                        writer.AutoFlush = true;
                        // Send a 'sync message' and wait for client to receive it.
                        writer.WriteLine(SvgTestHandler.KeySync);
                        pipeServer.WaitForPipeDrain();
                        // Send the console input to the client process.
                        testHandler.Marshal(writer);
                    }
                }
                catch (IOException ex)
                {
                    // Catch the IOException that is raised if the pipe is broken or disconnected.
                    Trace.TraceError("[Server] Error: {0}", ex.Message);
                }
            }

            var processExited = pipeClient.WaitForExit(WaitTime);
            if (processExited == false) // process is timed out...
            {
                pipeClient.Kill();
                pipeClient.Close();
                return _optionSettings.CrashImageFile;
            }
            var exitCode = pipeClient.ExitCode;
            //Trace.WriteLine("ExitCode: " + pipeClient.ExitCode);

            pipeClient.Close();
            //Trace.WriteLine("[Server] Client quit. Server terminating.");

            var outputText = outputBuilder.ToString().Trim();
            if (outputText.Length != 0)
            {
                Trace.WriteLine(string.Empty);
                Trace.WriteLine("###***** Image Magick *****");
                Trace.WriteLine(outputText);
            }

            if (exitCode == ExitSuccess && File.Exists(testHandler.OutputFile))
            {
                return testHandler.OutputFile;
            }
            if (exitCode == ExitStackOverflow)
            {
                return _optionSettings.CrashImageFile;
            }
            return _optionSettings.EmptyImageFile;
        }

        private string ProcessRsvg(string svgFileName)
        {
            var appDir      = _optionSettings.RsvgDirectory;
            if (string.IsNullOrWhiteSpace(appDir) || Directory.Exists(appDir))
            {
                return null;
            }
            var svgInputDir = _optionSettings.SvgDirectory;
            var workingDir  = _optionSettings.CacheDirectory;
            var toolsDir    = _optionSettings.ToolsDirectory;

            var testHandler = new RsvgTestHandler(svgInputDir, workingDir, appDir);

            testHandler.Initialize(svgFileName);
            if (!testHandler.IsInitialized)
            {
                return null;
            }
            if (File.Exists(testHandler.OutputFile))
            {
                return testHandler.OutputFile;
            }

            var outputBuilder  = new StringBuilder();
            Process pipeClient = new Process();

            pipeClient.StartInfo.FileName               = _optionSettings.TestRunnerFile;
            pipeClient.StartInfo.RedirectStandardError  = true;
            pipeClient.StartInfo.RedirectStandardOutput = true;
            pipeClient.StartInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            pipeClient.StartInfo.CreateNoWindow         = true;
            pipeClient.StartInfo.UseShellExecute        = false;
            pipeClient.EnableRaisingEvents              = false;

            pipeClient.OutputDataReceived += (sender, eventArgs) => 
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data); 
                }
            };
            pipeClient.ErrorDataReceived += (sender, eventArgs) =>
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    outputBuilder.AppendLine(eventArgs.Data);
                }
            };

            using (var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            {
                // Pass the client process a handle to the server.
                pipeClient.StartInfo.Arguments = pipeServer.GetClientHandleAsString();
                pipeClient.Start();
                pipeClient.BeginOutputReadLine();
                pipeClient.BeginErrorReadLine();

                pipeServer.DisposeLocalCopyOfClientHandle();

                try
                {
                    // Read user input and send that to the client process.
                    using (StreamWriter writer = new StreamWriter(pipeServer))
                    {
                        writer.AutoFlush = true;
                        // Send a 'sync message' and wait for client to receive it.
                        writer.WriteLine(SvgTestHandler.KeySync);
                        pipeServer.WaitForPipeDrain();
                        // Send the console input to the client process.
                        testHandler.Marshal(writer);
                    }
                }
                catch (IOException ex)
                {
                    // Catch the IOException that is raised if the pipe is broken or disconnected.
                    Trace.TraceError("[Server] Error: {0}", ex.Message);
                }
            }

            var processExited = pipeClient.WaitForExit(WaitTime);
            if (processExited == false) // process is timed out...
            {
                pipeClient.Kill();
                pipeClient.Close();
                return _optionSettings.CrashImageFile;
            }
            var exitCode = pipeClient.ExitCode;
            //Trace.WriteLine("ExitCode: " + pipeClient.ExitCode);

            pipeClient.Close();
            //Trace.WriteLine("[Server] Client quit. Server terminating.");

            var outputText = outputBuilder.ToString().Trim();
            if (outputText.Length != 0)
            {
                Trace.WriteLine(string.Empty);
                Trace.WriteLine("###***** LibRsvg *****");
                Trace.WriteLine(outputText);
            }

            if (exitCode == ExitSuccess && File.Exists(testHandler.OutputFile))
            {
                return testHandler.OutputFile;
            }
            if (exitCode == ExitStackOverflow)
            {
                return _optionSettings.CrashImageFile;
            }
            return _optionSettings.EmptyImageFile;
        }
    }
}
