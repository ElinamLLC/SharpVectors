using System;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;

using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Markup;

namespace SharpVectors.Converters
{
    /// <summary>
    /// This implements a markup extension that enables the creation
    /// of <see cref="DrawingImage"/> from SVG files.
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
        private UriTypeConverter _uriConverter;

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
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) ||
                    LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    return null;
                }

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

                Assembly assembly = Assembly.GetExecutingAssembly();
                string localFile = Path.Combine(Path.GetDirectoryName(
                    assembly.Location), svgPath);

                if (File.Exists(localFile))
                {
                    return new Uri(localFile);
                }

                // Try getting it as resource file...
                var inputUri = _uriConverter.ConvertFrom(inputParameter) as Uri;
                if (inputUri != null)
                {
                    return inputUri.IsAbsoluteUri ? inputUri : new Uri(_baseUri, inputUri);
                }
                string asmName = assembly.GetName().Name;
                string uriString = string.Format("pack://application:,,,/{0};component/{1}",
                    asmName, inputParameter);

                return new Uri(uriString);
            }

            return null;
        }

        #endregion
    }
}
