using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;

using System.Windows;
using System.Windows.Data;
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
    public sealed class SvgImageConverterExtension : SvgImageBase, IValueConverter
    {
        #region Private Fields

        private Uri _baseUri;
        private readonly UriTypeConverter _uriConverter;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="SvgImageConverterExtension"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="SvgImageConverterExtension"/> 
        /// class with the default parameters.
        /// </summary>
        public SvgImageConverterExtension()
        {
            _uriConverter = new UriTypeConverter();
        }

        public SvgImageConverterExtension(Uri baseUri)
            : this()
        {
            _baseUri = baseUri;
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
            var uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;
            if (uriContext != null)
            {
                _baseUri = uriContext.BaseUri;
            }

            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_appName))
                {
                    if (this.IsDesignMode() || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    {
                        this.GetAppName();
                    }
                }

                Uri inputUri = null;
                if (parameter != null)
                {
                    inputUri = this.ResolveUri(parameter.ToString());
                }
                else if (value != null)
                {
                    inputUri = _uriConverter.ConvertFrom(value) as Uri;
                    if (inputUri == null)
                    {
                        inputUri = this.ResolveUri(value.ToString());
                    }
                    else if (!inputUri.IsAbsoluteUri)
                    {
                        inputUri = this.ResolveUri(value.ToString());
                    }
                }

                if (inputUri == null)
                {
                    return null;
                }
                var svgSource = inputUri.IsAbsoluteUri ? inputUri : new Uri(_baseUri, inputUri);
                return this.GetImage(svgSource);
            }
            catch
            {
                //throw; #82
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Converts the SVG source file to <see cref="Uri"/>
        /// </summary>
        /// <param name="inputParameter">
        /// Object that can provide services for the markup extension.
        /// </param>
        /// <returns>
        /// Returns the valid <see cref="Uri"/> of the SVG source path if
        /// successful; otherwise, it returns <see langword="null"/>.
        /// </returns>
        private Uri ResolveUri(string inputParameter)
        {
            if (string.IsNullOrWhiteSpace(inputParameter))
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(_appName))
            {
                this.GetAppName();
            }
            var comparer = StringComparison.OrdinalIgnoreCase;

            Uri svgSource;
            if (Uri.TryCreate(inputParameter, UriKind.RelativeOrAbsolute, out svgSource))
            {
                if (svgSource.IsAbsoluteUri)
                {
                    return svgSource;
                }
                // Try getting a local file in the same directory....
                string svgPath = inputParameter;
                if (inputParameter[0] == '\\' || inputParameter[0] == '/')
                {
                    svgPath = inputParameter.Substring(1);
                }
                svgPath = svgPath.Replace('/', '\\');

                var assembly = this.GetExecutingAssembly();
                if (assembly != null)
                {
                    string localFile = PathUtils.Combine(assembly, svgPath);

                    if (File.Exists(localFile))
                    {
                        return new Uri(localFile);
                    }
                }

                // Try getting it as resource file...
                var inputUri = _uriConverter.ConvertFrom(inputParameter) as Uri;
                if (inputUri != null)
                {
                    if (inputUri.IsAbsoluteUri)
                    {
                        return inputUri;
                    }
                    if (_baseUri != null)
                    {
                        var validUri = new Uri(_baseUri, inputUri);
                        if (validUri.IsAbsoluteUri)
                        {
                            if (validUri.IsFile && File.Exists(validUri.LocalPath))
                            {
                                return validUri;
                            }
                        }
                    }
                }

                string asmName = _appName;
                if (string.IsNullOrWhiteSpace(asmName) && assembly != null)
                {
                    // It should not be the SharpVectors.Converters.Wpf.dll, which contains this extension...
                    string tempName = assembly.GetName().Name;
                    if (!string.Equals("SharpVectors.Converters.Wpf", tempName, comparer)
                        && !string.Equals("WpfSurface", tempName, comparer))
                    {
                        asmName = tempName;
                    }
                }

                svgPath = inputParameter;
                if (inputParameter.StartsWith("/", comparer))
                {
                    svgPath = svgPath.TrimStart('/');
                }

                // A little hack to display preview in design mode
                if (this.IsDesignMode() && !string.IsNullOrWhiteSpace(_appName))
                {
                    //string uriDesign = string.Format("/{0};component/{1}", _appName, svgPath);
                    //return new Uri(uriDesign, UriKind.Relative);

                    // The relative path is not working with the Converter...
                    string uriDesign = string.Format("pack://application:,,,/{0};component/{1}",
                        _appName, svgPath);

                    return new Uri(uriDesign);
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
