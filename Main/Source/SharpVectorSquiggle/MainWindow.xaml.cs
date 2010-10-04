using System;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;

using Microsoft.Win32;

using SharpVectors.Runtime;
using SharpVectors.Converters;

namespace SharpVectors.Squiggle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private delegate void FileChangedToUIThread(FileSystemEventArgs e);

        private string _titleBase;

        //private string _svgFilePath;
        //private string _xamlFilePath;

        private FileSystemWatcher _fileWatcher;

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

        /// <summary>
        /// The point that was clicked relative to the ZoomAndPanControl.
        /// </summary>
        private Point origZoomAndPanControlMouseDownPoint;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point origContentMouseDownPoint;

        /// <summary>
        /// Records which mouse button clicked during mouse dragging.
        /// </summary>
        private MouseButton mouseButtonDown;

        private FileSvgReader _fileReader;

        #endregion

        #region Constructors and Destructor

        public MainWindow()
        {
            InitializeComponent();

            _titleBase = this.Title;

            this.Loaded  += new RoutedEventHandler(OnWindowLoaded);
            //this.Unloaded += new RoutedEventHandler(OnWindowUnloaded);
            this.Closing += new CancelEventHandler(OnWindowClosing);  

            _fileReader = new FileSvgReader();
            _fileReader.SaveXaml = false;
            _fileReader.SaveZaml = false;
        }

        #endregion

        #region Protected Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            double width  = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;

            this.Width  = Math.Min(1600, width) * 0.85;
            this.Height = height * 0.85;

            this.Left = (width  - this.Width) / 2.0;
            this.Top  = (height - this.Height) / 2.0;

            this.WindowStartupLocation = WindowStartupLocation.Manual;
        }

        #endregion

        #region Private Event Handlers

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
        }

        #region Drag/Drop Methods

        private void OnDragEnter(object sender, DragEventArgs de)
        {
            if (de.Data.GetDataPresent(DataFormats.Text) ||
               de.Data.GetDataPresent(DataFormats.FileDrop))
            {
                de.Effects = DragDropEffects.Copy;
            }
            else
            {
                de.Effects = DragDropEffects.None;
            }
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {

        }

        private void OnDragDrop(object sender, DragEventArgs de)
        {
            string fileName = "";
            if (de.Data.GetDataPresent(DataFormats.Text))
            {
                fileName = (string)de.Data.GetData(DataFormats.Text);
            }
            else if (de.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileNames;
                fileNames = (string[])de.Data.GetData(DataFormats.FileDrop);
                fileName = fileNames[0];
            }

            if (!String.IsNullOrEmpty(fileName))
            {
            }
            if (String.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return;
            }

            this.CloseFile();

            try
            {
                this.Cursor      = Cursors.Wait;
                this.ForceCursor = true;

                this.LoadFile(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), _titleBase,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor      = Cursors.Arrow;
                this.ForceCursor = false;
            }
        }

        #endregion

        #region FileSystemWatcher Event Handlers

        private void OnFileChangedToUIThread(FileSystemEventArgs e)
        {
            // Stop watching.
            _fileWatcher.EnableRaisingEvents = false;

            try
            {
                this.Cursor      = Cursors.Wait;
                this.ForceCursor = true;

                this.LoadFile(e.FullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), _titleBase,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                this.ForceCursor = false;
            }
        }

        // Define the event handlers.
        private void OnFileChanged(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                this.Dispatcher.BeginInvoke(new FileChangedToUIThread(OnFileChangedToUIThread),
                    System.Windows.Threading.DispatcherPriority.Normal, e);
            }
        }

        private void OnFileRenamed(object source, RenamedEventArgs e)
        {
        }

        #endregion

        #region Private Zoom Panel Handlers

        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        private void OnZoomPanMouseDown(object sender, MouseButtonEventArgs e)
        {
            svgViewer.Focus();
            Keyboard.Focus(svgViewer);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomPanControl);
            origContentMouseDownPoint = e.GetPosition(svgViewer);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
                (e.ChangedButton == MouseButton.Left ||
                 e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                mouseHandlingMode = MouseHandlingMode.Zooming;
            }
            else if (mouseButtonDown == MouseButton.Left)
            {
                // Just a plain old left-down initiates panning mode.
                mouseHandlingMode = MouseHandlingMode.Panning;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                zoomPanControl.CaptureMouse();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomAndPanControl.
        /// </summary>
        private void OnZoomPanMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                if (mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the content.
                        ZoomIn();
                    }
                    else if (mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the content.
                        ZoomOut();
                    }
                }

                zoomPanControl.ReleaseMouseCapture();
                mouseHandlingMode = MouseHandlingMode.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        private void OnZoomPanMouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                Point curContentMousePoint = e.GetPosition(svgViewer);
                Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

                zoomPanControl.ContentOffsetX -= dragOffset.X;
                zoomPanControl.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised by rotating the mouse wheel
        /// </summary>
        private void OnZoomPanMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                ZoomIn();
            }
            else if (e.Delta < 0)
            {
                ZoomOut();
            }
        }

        /// <summary>
        /// The 'ZoomIn' command (bound to the plus key) was executed.
        /// </summary>
        private void OnZoomIn(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomIn();
        }

        /// <summary>
        /// The 'ZoomOut' command (bound to the minus key) was executed.
        /// </summary>
        private void OnZoomOut(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomOut();
        }

        /// <summary>
        /// Zoom the viewport out by a small increment.
        /// </summary>
        private void ZoomOut()
        {
            zoomPanControl.ContentScale -= 0.1;
        }

        /// <summary>
        /// Zoom the viewport in by a small increment.
        /// </summary>
        private void ZoomIn()
        {
            zoomPanControl.ContentScale += 0.1;
        }

        #endregion

        #endregion

        #region Private Methods

        private void LoadFile(string fileName)
        {
            string fileExt = Path.GetExtension(fileName);
            if (String.IsNullOrEmpty(fileExt))
            {
                return;
            }

            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
            }

            if (String.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    this.CloseFile();

                    DrawingGroup drawing = _fileReader.Read(fileName);

                    svgViewer.UnloadDiagrams();

                    if (drawing != null)
                    {
                        svgViewer.RenderDiagrams(drawing);
                    }
                    else
                    {
                        return;
                    }
                    zoomPanControl.InvalidateMeasure();

                    Rect bounds = drawing.Bounds;

                    //zoomPanControl.AnimatedScaleToFit();
                    //Rect rect = new Rect(0, 0,
                    //    mainFrame.RenderSize.Width, mainFrame.RenderSize.Height);
                    //Rect rect = new Rect(0, 0,
                    //    bounds.Width, bounds.Height);
                    if (bounds.IsEmpty)
                    {
                        bounds = new Rect(0, 0,
                            mainFrame.ActualWidth, mainFrame.ActualHeight);
                    }
                    zoomPanControl.AnimatedZoomTo(bounds);

                    if (_fileWatcher == null)
                    {
                        // Create a new FileSystemWatcher and set its properties.
                        _fileWatcher = new FileSystemWatcher();
                        // Watch for changes in LastAccess and LastWrite times
                        _fileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;

                        _fileWatcher.IncludeSubdirectories = false;

                        // Add event handlers.
                        _fileWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
                        _fileWatcher.Created += new FileSystemEventHandler(OnFileChanged);
                        _fileWatcher.Deleted += new FileSystemEventHandler(OnFileChanged);
                        _fileWatcher.Renamed += new RenamedEventHandler(OnFileRenamed);
                    }

                    _fileWatcher.Path = Path.GetDirectoryName(fileName);
                    // Only watch current file
                    _fileWatcher.Filter = Path.GetFileName(fileName);
                    // Begin watching.
                    _fileWatcher.EnableRaisingEvents = true;
                }
                catch
                {
                    //svgViewer.Source = null;
                    svgViewer.UnloadDiagrams();
                }
            }
        }

        private void CloseFile()
        {
            try
            {
                if (svgViewer != null)
                {
                    //svgViewer.Source = null;
                    svgViewer.UnloadDiagrams();
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}
