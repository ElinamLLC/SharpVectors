using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;

using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

using SharpVectors.Dom;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Utils;
using SharpVectors.Runtime;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Converters.Utils;

namespace SharpVectors.Converters
{
    using DpiScale = SharpVectors.Runtime.DpiScale;
    using DpiUtilities = SharpVectors.Runtime.DpiUtilities;

    /// <summary>
    /// This is an extension of the WPF <see cref="Image"/> control to provide SVG-based image sources.
    /// </summary>
    /// <seealso cref="SvgIcon"/>
    public class SvgBitmap : Image
    {
        #region Public Fields

        /// <summary>
        /// Identifies the <see cref="UriSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(Uri), typeof(SvgBitmap),
                new FrameworkPropertyMetadata(null, OnUriSourceChanged));

        /// <summary>
        /// Identifies the <see cref="SvgSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SvgSourceProperty =
            DependencyProperty.Register("SvgSource", typeof(string), typeof(SvgBitmap),
                new FrameworkPropertyMetadata(null, OnSvgSourceChanged));

        /// <summary>
        /// The <see cref="DependencyProperty"/> for the <c>AppName</c> property.
        /// </summary>
        public static readonly DependencyProperty AppNameProperty = DependencyProperty.Register(
            "AppName", typeof(string), typeof(SvgBitmap), new FrameworkPropertyMetadata((string)null));

        #endregion

        #region Private Fields

        private Uri _uriSource;
        private string _svgSource;

        private DpiScale _dpiScale;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgBitmap"/> class.
        /// </summary>
        public SvgBitmap()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the <c>name</c> of the parent assembly for this element.
        /// </summary>
        /// <value>
        /// A string containing the name of the parent assembly or the name of the assembly containing <c>SVG</c> file 
        /// referenced on this control in XAML, if the source type is <see cref="Uri"/>.
        /// </value>
        [Bindable(true), Category("Appearance")]
        public string AppName
        {
            get {
                return (string)GetValue(AppNameProperty);
            }
            set {
                this.SetValue(AppNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the path to the SVG file to load into this <see cref="SvgBitmap"/>.
        /// </summary>
        /// <value>
        /// A <see cref="System.Uri"/> specifying the path to the SVG source file.
        /// The file can be located on a computer, network or assembly resources.
        /// Settings this to <see langword="null"/> will close any opened diagram.
        /// </value>
        /// <seealso cref="SvgSource"/>
        public Uri UriSource
        {
            get {
                return (Uri)GetValue(UriSourceProperty);
            }
            set {
                if (value != null)
                {
                    _svgSource = null;
                }
                _uriSource = value;
                this.SetValue(UriSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the SVG contents to load into this <see cref="SvgBitmap"/>.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> specifying the embedded SVG contents.
        /// Settings this to <see langword="null"/> will close any opened diagram.
        /// </value>
        /// <seealso cref="UriSource"/>
        public string SvgSource
        {
            get {
                return (string)GetValue(SvgSourceProperty);
            }
            set {
                if (value != null)
                {
                    _uriSource = null;
                }
                _svgSource = value;
                this.SetValue(SvgSourceProperty, value);
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void OnLoadDrawing(DrawingGroup drawing)
        {
            try
            {
                if (drawing == null)
                {
                    this.Source = null;
                    return;
                }

                this.Source = new DrawingImage(drawing);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        protected virtual void OnUnloadDiagram()
        {
            try
            {
                this.Source = null;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        protected virtual WpfDrawingSettings GetDrawingSettings()
        {
            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = false;
            settings.TextAsGeometry = true;
            settings.OptimizePath = true;

            //settings.IgnoreRootViewbox = _ignoreRootViewbox;
            //settings.EnsureViewboxSize = _ensureViewboxSize;
            //settings.EnsureViewboxPosition = _ensureViewboxPosition;

            settings.InteractiveMode = SvgInteractiveModes.None;

            return settings;
        }

        /// <summary>
        /// Raises the Initialized event. This method is invoked whenever IsInitialized is set to true.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (_uriSource != null || !string.IsNullOrWhiteSpace(_svgSource))
            {
                if (this.Source == null)
                {
                    DrawingGroup drawing = this.CreateDrawing();
                    if (drawing != null)
                    {
                        this.OnLoadDrawing(drawing);
                    }
                }
            }

            //HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            //source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...

            return IntPtr.Zero;
        }

        /// <summary>
        /// This handles changes in the rendering settings of this control.
        /// </summary>
        protected virtual void OnSettingsChanged()
        {
            if (!this.IsInitialized || (_uriSource == null && string.IsNullOrWhiteSpace(_svgSource)))
            {
                return;
            }

            DrawingGroup drawing = this.CreateDrawing();
            if (drawing != null)
            {
                this.OnLoadDrawing(drawing);
            }
        }

        /// <summary>
        /// Performs the conversion of a valid SVG source to the <see cref="DrawingGroup"/>.
        /// </summary>
        /// <returns>
        /// This returns <see cref="DrawingGroup"/> if successful; otherwise, it
        /// returns <see langword="null"/>.
        /// </returns>
        protected virtual DrawingGroup CreateDrawing()
        {
            WpfDrawingSettings settings = this.GetDrawingSettings();

            try
            {
                // 2. Load from the Uri, if available
                Uri svgSource = this.GetAbsoluteUri();
                if (svgSource != null)
                {
                    return this.CreateDrawing(svgSource, settings);
                }

                // 3. Load embedded SVG contents...
                if (!string.IsNullOrWhiteSpace(_svgSource))
                {
                    return this.CreateDrawing(_svgSource, settings);
                }

                return null;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Performs the conversion of a valid SVG source file to the <see cref="DrawingGroup"/>.
        /// </summary>
        /// <param name="svgSource">A <see cref="Uri"/> defining the path to the SVG source.</param>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        /// <returns>
        /// This returns <see cref="DrawingGroup"/> if successful; otherwise, it
        /// returns <see langword="null"/>.
        /// </returns>
        protected virtual DrawingGroup CreateDrawing(Uri svgSource, WpfDrawingSettings settings)
        {
            if (svgSource == null)
            {
                return null;
            }
            if (settings != null)
            {
                if (_dpiScale == null)
                {
                    _dpiScale = DpiUtilities.GetWindowScale(this);
                }
                settings.DpiScale = _dpiScale;
            }

            string scheme = null;
            if (DesignerProperties.GetIsInDesignMode(this) && svgSource.IsAbsoluteUri == false)
            {
                scheme = "pack";
            }
            else if (svgSource.IsAbsoluteUri == false)
            {
                scheme = "pack"; //TODO
            }
            else
            {
                scheme = svgSource.Scheme;
            }
            if (string.IsNullOrWhiteSpace(scheme))
            {
                return null;
            }

            try
            {
                var comparer = StringComparison.OrdinalIgnoreCase;

                DrawingGroup drawing = null;

                switch (scheme)
                {
                    case "file":
                    //case "ftp":
                    case "https":
                    case "http":
                        using (FileSvgReader reader = new FileSvgReader(settings))
                        {
                            drawing = reader.Read(svgSource);
                        }
                        break;
                    case "pack":
                        StreamResourceInfo svgStreamInfo = null;
                        if (svgSource.ToString().IndexOf("siteoforigin", comparer) >= 0)
                        {
                            svgStreamInfo = Application.GetRemoteStream(svgSource);
                        }
                        else
                        {
                            svgStreamInfo = Application.GetResourceStream(svgSource);
                        }

                        Stream svgStream = (svgStreamInfo != null) ? svgStreamInfo.Stream : null;

                        if (svgStream != null)
                        {
                            string fileExt = Path.GetExtension(svgSource.ToString());
                            bool isCompressed = !string.IsNullOrWhiteSpace(fileExt) &&
                                string.Equals(fileExt, ".svgz", comparer);

                            if (isCompressed)
                            {
                                using (svgStream)
                                {
                                    using (GZipStream zipStream = new GZipStream(svgStream, CompressionMode.Decompress))
                                    {
                                        using (FileSvgReader reader = new FileSvgReader(settings))
                                        {
                                            drawing = reader.Read(zipStream);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (svgStream)
                                {
                                    using (FileSvgReader reader = new FileSvgReader(settings))
                                    {
                                        drawing = reader.Read(svgStream);
                                    }
                                }
                            }
                        }
                        break;
                    case "data":
                        var sourceData = svgSource.OriginalString.Replace(" ", string.Empty);

                        int nColon = sourceData.IndexOf(":", comparer);
                        int nSemiColon = sourceData.IndexOf(";", comparer);
                        int nComma = sourceData.IndexOf(",", comparer);

                        string sMimeType = sourceData.Substring(nColon + 1, nSemiColon - nColon - 1);
                        string sEncoding = sourceData.Substring(nSemiColon + 1, nComma - nSemiColon - 1);

                        if (string.Equals(sMimeType.Trim(), "image/svg+xml", comparer)
                            && string.Equals(sEncoding.Trim(), "base64", comparer))
                        {
                            string sContent = SvgObject.RemoveWhitespace(sourceData.Substring(nComma + 1));
                            byte[] imageBytes = Convert.FromBase64CharArray(sContent.ToCharArray(),
                                0, sContent.Length);
                            bool isGZiped = sContent.StartsWith(SvgConstants.GZipSignature, StringComparison.Ordinal);
                            if (isGZiped)
                            {
                                using (var stream = new MemoryStream(imageBytes))
                                {
                                    using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress))
                                    {
                                        using (var reader = new FileSvgReader(settings))
                                        {
                                            drawing = reader.Read(zipStream);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (var stream = new MemoryStream(imageBytes))
                                {
                                    using (var reader = new FileSvgReader(settings))
                                    {
                                        drawing = reader.Read(stream);
                                    }
                                }
                            }
                        }
                        break;
                }

                return drawing;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Performs the conversion of a valid SVG source stream to the <see cref="DrawingGroup"/>.
        /// </summary>
        /// <param name="svgSource">A stream providing access to the SVG source data.</param>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        /// <returns>
        /// This returns <see cref="DrawingGroup"/> if successful; otherwise, it
        /// returns <see langword="null"/>.
        /// </returns>
        protected virtual DrawingGroup CreateDrawing(string svgSource, WpfDrawingSettings settings)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(svgSource))
                {
                    return null;
                }

                DrawingGroup drawing = null;

                var svgContent = svgSource.Trim();

                var cdataStart = "<![CDATA[";
                var cdataEnd = "]]>";

                if (svgContent.StartsWith(cdataStart, StringComparison.OrdinalIgnoreCase) ||
                    svgContent.EndsWith(cdataEnd, StringComparison.OrdinalIgnoreCase))
                {
                    var xmlDoc = XDocument.Parse(svgSource);
                    var cdataElement = xmlDoc.DescendantNodes().OfType<XCData>().FirstOrDefault();
                    if (cdataElement != null)
                    {
                        svgContent = cdataElement.Value;
                    }
                }

                using (FileSvgReader reader = new FileSvgReader(settings))
                {
                    var textReader = new StringReader(svgContent);
                    drawing = reader.Read(textReader);

                    textReader.Dispose();
                }

                return drawing;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return null;
            }
        }

        #endregion

        #region Private Methods

        private Uri GetAbsoluteUri()
        {
            if (_uriSource == null)
            {
                return null;
            }

            return this.ResolveUri(_uriSource);
        }

        private Uri ResolveUri(Uri svgSource)
        {
            if (svgSource == null)
            {
                return null;
            }

            if (svgSource.IsAbsoluteUri)
            {
                return svgSource;
            }

            // Try getting a local file in the same directory....
            string svgPath = svgSource.ToString();
            if (svgPath[0] == '\\' || svgPath[0] == '/')
            {
                svgPath = svgPath.Substring(1);
            }
            svgPath = svgPath.Replace('/', '\\');

            Assembly assembly = this.GetExecutingAssembly(); // Assembly.GetExecutingAssembly();
            string localFile = PathUtils.Combine(assembly, svgPath);

            if (File.Exists(localFile))
            {
                return new Uri(localFile);
            }

            // Try getting it as resource file...
            var baseUri = this.BaseUri;
            if (baseUri != null)
            {
                var validUri = new Uri(baseUri, svgSource);
                if (validUri.IsAbsoluteUri)
                {
                    if (validUri.IsFile && File.Exists(validUri.LocalPath))
                    {
                        return validUri;
                    }
                }
            }
            baseUri = System.Windows.Navigation.BaseUriHelper.GetBaseUri(this);
            if (baseUri != null)
            {
                var contextUri = new Uri(baseUri, svgSource);
                if (contextUri.IsFile && File.Exists(contextUri.LocalPath))
                {
                    return contextUri;
                }
            }

            string asmName = this.AppName;
            if (string.IsNullOrWhiteSpace(asmName))
            {
                if (assembly != null)
                {
                    asmName = assembly.GetName().Name;
                }
                else
                {
                    asmName = this.GetAppName();
                }
            }
            else
            {
                string uriDesign = string.Format("/{0};component/{1}", asmName, svgPath);

                return new Uri(uriDesign, UriKind.Relative);
            }

            if (DesignerProperties.GetIsInDesignMode(this) || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                string uriDesign = string.Format("/{0};component/{1}", asmName, svgPath);

                return new Uri(uriDesign, UriKind.Relative);
            }

            string uriString = string.Format("pack://application:,,,/{0};component/{1}",
                asmName, svgPath);
            if (!WpfResources.ResourceExists(assembly, svgPath))
            {
                return null;
            }
            return new Uri(uriString);
        }

        private static void OnUriSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SvgBitmap viewbox = obj as SvgBitmap;
            if (viewbox == null)
            {
                return;
            }

            viewbox._uriSource = (Uri)args.NewValue;
            if (viewbox._uriSource == null)
            {
                viewbox.OnUnloadDiagram();
            }
            else
            {
                viewbox.OnSettingsChanged();
            }
        }

        private static void OnSvgSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SvgBitmap viewbox = obj as SvgBitmap;
            if (viewbox == null)
            {
                return;
            }

            viewbox._svgSource = (string)args.NewValue;
            if (string.IsNullOrWhiteSpace(viewbox._svgSource))
            {
                viewbox.OnUnloadDiagram();
            }
            else
            {
                viewbox.OnSettingsChanged();
            }
        }

        private string GetAppName()
        {
            try
            {
                Assembly asm = this.GetEntryAssembly();

                if (asm != null)
                {
                    return asm.GetName().Name;
                }
            }
            catch (Exception ex)
            {
                // Issue #125
                Trace.TraceError(ex.Message);
            }
            return null;
        }

        private Assembly GetEntryAssembly()
        {
            string XDesProc = "XDesProc";   // WPF designer process - Designer Isolation
            string DevEnv = "DevEnv";     // WPF designer process - Surface Isolation
            string WpfSurface = "WpfSurface"; // WPF designer process - New .NETCore
            var comparer = StringComparison.OrdinalIgnoreCase;

            Assembly asm = null;
            try
            {
                // Should work at runtime...
                asm = Assembly.GetEntryAssembly(); //...but mostly loading the design-time process: XDesProc.exe
                if (asm != null)
                {
                    var appName = asm.GetName().Name;
                    if (appName.StartsWith(XDesProc, comparer)
                        || appName.StartsWith(DevEnv, comparer)
                        || appName.StartsWith(WpfSurface, comparer))
                    {
                        asm = null;
                    }
                }
                // Design time
                if (asm == null)
                {
#if NETCORE
                    asm = (
                          from assembly in AppDomain.CurrentDomain.GetAssemblies()
                          where !assembly.IsDynamic
                          let assmName = SharpVectors.Dom.Utils.PathUtils.GetAssemblyFileName(assembly).Trim()
                          where assmName.EndsWith(".exe", comparer)
                              && !assmName.StartsWith(XDesProc, comparer) // should not be XDesProc.exe
                              && !assmName.StartsWith(DevEnv, comparer)   // should not be DevEnv.exe
                              && !assmName.StartsWith(WpfSurface, comparer)   // should not be WpfSurface.exe
                          select assembly
                          ).FirstOrDefault();
#else
                    asm = (
                          from assembly in AppDomain.CurrentDomain.GetAssemblies()
                          where !assembly.IsDynamic
                          let assmName = Path.GetFileName(assembly.CodeBase).Trim()
                          where assmName.EndsWith(".exe", comparer)
                              && !assmName.StartsWith(XDesProc, comparer) // should not be XDesProc.exe
                              && !assmName.StartsWith(DevEnv, comparer)   // should not be DevEnv.exe
                              && !assmName.StartsWith(WpfSurface, comparer)   // should not be WpfSurface.exe
                          select assembly
                          ).FirstOrDefault();
#endif

                    if (asm == null)
                    {
                        asm = Application.ResourceAssembly;
                        if (asm != null)
                        {
                            var appName = asm.GetName().Name;
                            if (appName.StartsWith(XDesProc, comparer) || appName.StartsWith(DevEnv, comparer))
                            {
                                asm = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (asm == null)
                {
#if NETCORE
                    asm = (
                          from assembly in AppDomain.CurrentDomain.GetAssemblies()
                          where !assembly.IsDynamic
                          let assmName = SharpVectors.Dom.Utils.PathUtils.GetAssemblyFileName(assembly).Trim()
                          where assmName.EndsWith(".exe", comparer)
                              && !assmName.StartsWith(XDesProc, comparer) // should not be XDesProc.exe
                              && !assmName.StartsWith(DevEnv, comparer)   // should not be DevEnv.exe
                              && !assmName.StartsWith(WpfSurface, comparer)   // should not be WpfSurface.exe
                          select assembly
                          ).FirstOrDefault();
#else
                    asm = (
                          from assembly in AppDomain.CurrentDomain.GetAssemblies()
                          where !assembly.IsDynamic
                          let assmName = Path.GetFileName(assembly.CodeBase).Trim()
                          where assmName.EndsWith(".exe", comparer)
                              && !assmName.StartsWith(XDesProc, comparer) // should not be XDesProc.exe
                              && !assmName.StartsWith(DevEnv, comparer)   // should not be DevEnv.exe
                              && !assmName.StartsWith(WpfSurface, comparer)   // should not be WpfSurface.exe
                          select assembly
                          ).FirstOrDefault();
#endif
                }

                Trace.TraceError(ex.Message);
            }
            return asm;
        }

        private Assembly GetExecutingAssembly()
        {
            Assembly asm = null;
            try
            {
                var invalidAssemblies = new string[] { "SharpVectors.Converters.Wpf", "WpfSurface", "XDesProc", "DevEnv" };

                asm = Assembly.GetExecutingAssembly();
                string asmName = asm == null ? null : Path.GetFileName(asm.GetName().Name);
                if (asmName != null && !invalidAssemblies.Contains(asmName, StringComparer.OrdinalIgnoreCase))
                {
                    return asm;
                }
                else
                {
                    asm = Assembly.GetEntryAssembly();
                    asmName = asm == null ? null : Path.GetFileName(asm.GetName().Name);
                    if (asmName != null && !invalidAssemblies.Contains(asmName, StringComparer.OrdinalIgnoreCase))
                    {
                        return asm;
                    }
                }

                return this.GetEntryAssembly();
            }
            catch (Exception ex)
            {
                asm = Assembly.GetEntryAssembly();
                Trace.TraceError(ex.Message);
            }
            return asm;
        }

        #endregion
    }
}
