using System;
using System.IO;
using System.ComponentModel;

using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using SharpVectors.Dom.Utils;

namespace SharpVectors.Converters
{
    /// <summary>
    /// This implements a markup extension that enables the creation
    /// of <see cref="DrawingImage"/> from SVG sources.
    /// </summary>
    /// <remarks>
    /// The SVG source file can be:
    /// <list type="bullet">
    /// <item>
    /// <description>From the web</description>
    /// </item>
    /// <item>
    /// <description>From the local computer (relative or absolute paths)</description>
    /// </item>
    /// <item>
    /// <description>From the resources.</description>
    /// </item>
    /// </list>
    /// <para>
    /// The rendering settings are provided as properties for customizations.
    /// </para>
    /// </remarks>
    [MarkupExtensionReturnType(typeof(DrawingImage))]
    public sealed class SvgImageExtension : SvgImageBase
    {
        #region Private Fields

        private string _svgPath;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="SvgImageExtension"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="SvgImageExtension"/> 
        /// class with the default parameters.
        /// </summary>
        public SvgImageExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgImageExtension"/> 
        /// class with the specified SVG file path.
        /// </summary>
        /// <param name="svgPath"></param>
        public SvgImageExtension(string svgPath)
            : this()
        {
            _svgPath = svgPath;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the SVG source file.
        /// </summary>
        /// <value>
        /// A string specifying the path of the SVG source file.
        /// The default is <see langword="null"/>.
        /// </value>
        public string Source
        {
            get {
                return _svgPath;
            }
            set {
                _svgPath = value;

                if (string.IsNullOrWhiteSpace(_appName))
                {
                    if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) ||
                        LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    {
                        this.GetAppName();
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs the conversion of a valid SVG source file to the 
        /// <see cref="DrawingImage"/> that is set as the value of the target 
        /// property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">
        /// Object that can provide services for the markup extension.
        /// </param>
        /// <returns>
        /// This returns <see cref="DrawingImage"/> if successful; otherwise, it
        /// returns <see langword="null"/>.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_appName))
                {
                    if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) ||
                        LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    {
                        this.GetAppName();
                    }
                }

                Uri svgSource = this.ResolveUri(serviceProvider);

                if (svgSource == null)
                {
                    return null;
                }
                return this.GetImage(svgSource);
            }
            catch
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) ||
                    LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    return null;
                }

                //throw; #82
            }

            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Converts the SVG source file to <see cref="Uri"/>
        /// </summary>
        /// <param name="serviceProvider">
        /// Object that can provide services for the markup extension.
        /// </param>
        /// <returns>
        /// Returns the valid <see cref="Uri"/> of the SVG source path if
        /// successful; otherwise, it returns <see langword="null"/>.
        /// </returns>
        private Uri ResolveUri(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrWhiteSpace(_svgPath))
            {
                return null;
            }

            Uri svgSource;
            if (Uri.TryCreate(_svgPath, UriKind.RelativeOrAbsolute, out svgSource))
            {
                if (svgSource.IsAbsoluteUri)
                {
                    return svgSource;
                }
                // Try getting a local file in the same directory....
                string svgPath = _svgPath;
                if (_svgPath[0] == '\\' || _svgPath[0] == '/')
                {
                    svgPath = _svgPath.Substring(1);
                }
                svgPath = svgPath.Replace('/', '\\');

                var assembly = this.GetExecutingAssembly();
                if (assembly != null)
                {
                    string localFile = Path.Combine(LocationUtils.GetAssemblyDirectory(assembly), svgPath);

                    if (File.Exists(localFile))
                    {
                        return new Uri(localFile);
                    }
                }

                // Try getting it as resource file...
                IUriContext uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;
                if (uriContext != null && uriContext.BaseUri != null)
                {
                    return new Uri(uriContext.BaseUri, svgSource);
                }
                string asmName = _appName;
                // It should not be the SharpVectors.Converters.Wpf.dll, which contains this extension...
                if (assembly != null && !string.Equals("SharpVectors.Converters.Wpf",
                    assembly.GetName().Name, StringComparison.OrdinalIgnoreCase))
                {
                    asmName = assembly.GetName().Name;
                }

                svgPath = _svgPath;
                if (_svgPath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    svgPath = svgPath.TrimStart('/');
                }

                // A little hack to display preview in design mode
                bool designTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());
                if (designTime && !string.IsNullOrWhiteSpace(_appName))
                {
                    string uriDesign = string.Format("/{0};component/{1}", _appName, svgPath);

                    return new Uri(uriDesign, UriKind.Relative);
                }

                string uriString = string.Format("pack://application:,,,/{0};component/{1}",
                    asmName, svgPath);

                return new Uri(uriString);
            }

            return null;
        }

        #endregion
    }
}
