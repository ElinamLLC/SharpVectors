using System;
using System.IO;
using System.Collections.Generic;
using System.Security.AccessControl;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Renderers.Wpf;
using SharpVectors.Converters.Utils;

namespace SharpVectors.Converters
{
    /// <summary>
    /// This converts a directory (and optionally the sub-directories) of SVG 
    /// files to XAML files in a specified directory, maintaining the original 
    /// directory structure.
    /// </summary>
    public sealed class DirectorySvgConverter : SvgConverter
    {
        #region Private Fields

        private int  _convertedCount;
        private bool _isOverwrite;
        private bool _isRecursive;
        private bool _includeHidden;
        private bool _includeSecurity;

        private bool _writerErrorOccurred;
        private bool _fallbackOnWriterError;

        private string _errorFile;

        private DirectoryInfo _sourceDir;
        private DirectoryInfo _destinationDir;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfDrawingSettings"/> 
        /// class with the specified drawing or rendering settings.
        /// </summary>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        public DirectorySvgConverter(WpfDrawingSettings settings)
            : base(settings)
        {
            _isOverwrite = true;
            _isRecursive = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the directory copying is
        /// recursive, that is includes the sub-directories.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the sub-directories are
        /// included in the directory copy; otherwise, it is <see langword="false"/>.
        /// The default is <see langword="true"/>.
        /// </value>
        public bool Recursive
        {
            get
            {
                return _isRecursive;
            }

            set
            {
                _isRecursive = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether an existing file is overwritten.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if existing file is overwritten;
        /// otherwise, it is <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        public bool Overwrite
        {
            get
            {
                return _isOverwrite;
            }

            set
            {
                _isOverwrite = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the security settings of the
        /// copied file is retained.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the security settings of the
        /// file is also copied; otherwise, it is <see langword="false"/>. The
        /// default is <see langword="false"/>.
        /// </value>
        public bool IncludeSecurity
        {
            get
            {
                return _includeSecurity;
            }

            set
            {
                _includeSecurity = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the copy operation includes
        /// hidden directories and files.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if hidden directories and files
        /// are included in the copy operation; otherwise, it is 
        /// <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        public bool IncludeHidden
        {
            get
            {
                return _includeHidden;
            }

            set
            {
                _includeHidden = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a writer error occurred when
        /// using the custom XAML writer.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if an error occurred when using
        /// the custom XAML writer; otherwise, it is <see langword="false"/>.
        /// </value>
        public bool WriterErrorOccurred
        {
            get
            {
                return _writerErrorOccurred;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to fall back and use
        /// the .NET Framework XAML writer when an error occurred in using the
        /// custom writer.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the converter falls back to using
        /// the system XAML writer when an error occurred in using the custom
        /// writer; otherwise, it is <see langword="false"/>. If <see langword="false"/>,
        /// an exception, which occurred in using the custom writer will be
        /// thrown. The default is <see langword="false"/>. 
        /// </value>
        public bool FallbackOnWriterError
        {
            get
            {
                return _fallbackOnWriterError;
            }
            set
            {
                _fallbackOnWriterError = value;
            }
        }

        /// <summary>
        /// Gets the source directory of the SVG files to be converted.
        /// </summary>
        /// <value>
        /// A <see cref="DirectoryInfo"/> specifying the source directory of
        /// the SVG files.
        /// </value>
        public DirectoryInfo SourceDir
        {
            get 
            { 
                return _sourceDir; 
            }
        }

        /// <summary>
        /// Gets the destination directory of the converted XAML files.
        /// </summary>
        /// <value>
        /// A <see cref="DirectoryInfo"/> specifying the destination directory of
        /// the converted XAML files.
        /// </value>
        public DirectoryInfo DestinationDir
        {
            get 
            { 
                return _destinationDir; 
            }
        }

        /// <summary>
        /// Gets the full path of the last SVG file not successfully converted.
        /// </summary>
        /// <value>
        /// A string containing the full path of the last SVG file not 
        /// successfully converted to the XAML
        /// </value>
        /// <remarks>
        /// Whenever an error occurred in the conversion of a file, the 
        /// conversion process will stop. Use this property to retrieve the full
        /// path of the SVG file causing the error.
        /// </remarks>
        public string ErrorFile
        {
            get 
            { 
                return _errorFile; 
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Convert the SVG files in the specified source directory, saving the
        /// results in the specified destination directory.
        /// </summary>
        /// <param name="sourceInfo">
        /// A <see cref="DirectoryInfo"/> specifying the source directory of
        /// the SVG files.
        /// </param>
        /// <param name="destInfo">
        /// A <see cref="DirectoryInfo"/> specifying the source directory of
        /// the SVG files.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>
        /// If the <paramref name="sourceInfo"/> is <see langword="null"/>.
        /// </para>
        /// <para>
        /// -or-
        /// </para>
        /// <para>
        /// If the <paramref name="destInfo"/> is <see langword="null"/>.
        /// </para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the directory specified by <paramref name="sourceInfo"/> does not
        /// exists.
        /// </exception>
        public void Convert(DirectoryInfo sourceInfo, DirectoryInfo destInfo)
        {
            if (sourceInfo == null)
            {
                throw new ArgumentNullException("sourceInfo", 
                    "The source directory cannot be null (or Nothing).");
            }
            if (destInfo == null)
            {
                throw new ArgumentNullException("destInfo",
                    "The destination directory cannot be null (or Nothing).");
            }
            if (!sourceInfo.Exists)
            {
                throw new ArgumentException(
                    "The source directory must exists.", "sourceInfo");
            }

            _convertedCount = 0;
            _sourceDir      = sourceInfo;
            _destinationDir = destInfo;
            DirectorySecurity dirSecurity = null;
            if (_includeSecurity)
            {
                dirSecurity = sourceInfo.GetAccessControl();
            }
            if (!destInfo.Exists)
            {
                if (dirSecurity != null)
                {
                    destInfo.Create(dirSecurity);
                }
                else
                {
                    destInfo.Create();
                }
                destInfo.Attributes = sourceInfo.Attributes;
            }
            else
            {
                if (dirSecurity != null)
                {
                    destInfo.SetAccessControl(dirSecurity);
                }
            }

            this.ProcessConversion(_sourceDir, _destinationDir);
        }

        #endregion

        #region Private Methods

        private void ProcessConversion(DirectoryInfo source, DirectoryInfo target)
        {
            // Convert the files in the specified directory...
            this.ConvertFiles(source, target);

            if (!_isRecursive)
            {
                return;
            }

            // If recursive, process any sub-directory...
            DirectoryInfo[] arrSourceInfo = source.GetDirectories();

            int dirCount = (arrSourceInfo == null) ? 0 : arrSourceInfo.Length;

            for (int i = 0; i < dirCount; i++)
            {
                DirectoryInfo sourceInfo = arrSourceInfo[i];
                FileAttributes fileAttr  = sourceInfo.Attributes;
                if (!_includeHidden)
                {
                    if ((fileAttr & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        continue;
                    }
                }

                DirectoryInfo targetInfo = null;
                if (_includeSecurity)
                {
                    targetInfo = target.CreateSubdirectory(sourceInfo.Name,
                        sourceInfo.GetAccessControl());
                }
                else
                {
                    targetInfo = target.CreateSubdirectory(sourceInfo.Name);
                }
                targetInfo.Attributes = fileAttr;

                this.ProcessConversion(sourceInfo, targetInfo);
            }
        }

        private void ConvertFiles(DirectoryInfo source, DirectoryInfo target)
        {
            _errorFile = null;

            FileSvgConverter fileConverter = new FileSvgConverter(this.SaveXaml,
                this.SaveZaml, this.DrawingSettings);
            fileConverter.FallbackOnWriterError = _fallbackOnWriterError;

            string targetDirName = target.ToString();
            string xamlFilePath;

            IEnumerable<string> fileIterator = DirectoryUtils.FindFiles(
              source, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string svgFileName in fileIterator)
            {
                string fileExt = Path.GetExtension(svgFileName);
                if (String.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase) || 
                    String.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        FileAttributes fileAttr = File.GetAttributes(svgFileName);
                        if (!_includeHidden)
                        {
                            if ((fileAttr & FileAttributes.Hidden) == FileAttributes.Hidden)
                            {
                                continue;
                            }
                        }

                        xamlFilePath = Path.Combine(targetDirName, 
                            Path.GetFileNameWithoutExtension(svgFileName) + ".xaml");

                        fileConverter.Convert(svgFileName, xamlFilePath);

                        File.SetAttributes(xamlFilePath, fileAttr);
                        // if required to set the security or access control
                        if (_includeSecurity)
                        {
                            File.SetAccessControl(xamlFilePath, File.GetAccessControl(svgFileName));
                        }

                        _convertedCount++;

                        if (fileConverter.WriterErrorOccurred)
                        {
                            _writerErrorOccurred = true;
                        }
                    }
                    catch
                    {
                        _errorFile = svgFileName;

                        throw;
                    }
                }
            }
        }

        #endregion
    }
}
