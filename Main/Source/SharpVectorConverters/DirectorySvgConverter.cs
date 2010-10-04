using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.AccessControl;

using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;

namespace SharpVectors.Converters
{
    public sealed class DirectorySvgConverter
    {
        #region Private Fields

        private int  _convertedCount;
        private bool _isOverwrite;
        private bool _isRecursive;
        private bool _includeHidden;
        private bool _includeSecurity;

        private bool _saveXaml;
        private bool _saveZaml;

        private string _errorFile;

        private DirectoryInfo _sourceDir;
        private DirectoryInfo _destinationDir;

        #endregion

        #region Constructors and Destructor

        public DirectorySvgConverter()
        {
            _saveXaml    = true;
            _saveZaml    = true;
            _isOverwrite = true;
            _isRecursive = true;
        }

        #endregion

        #region Public Properties

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

        public bool SaveXaml
        {
            get
            {
                return _saveXaml;
            }
            set
            {
                _saveXaml = value;
            }
        }

        public bool SaveZaml
        {
            get
            {
                return _saveZaml;
            }
            set
            {
                _saveZaml = value;
            }
        }

        public string ErrorFile
        {
            get { return _errorFile; }
            set { _errorFile = value; }
        }

        public DirectoryInfo SourceDir
        {
            get 
            { 
                return _sourceDir; 
            }
            set 
            {
                _sourceDir = value; 
            }
        }

        public DirectoryInfo DestinationDir
        {
            get 
            { 
                return _destinationDir; 
            }
            set 
            {
                _destinationDir = value; 
            }
        }

        #endregion

        #region Public Methods

        public void Convert(DirectoryInfo sourceInfo, DirectoryInfo destInfo)
        {
            _convertedCount    = 0;
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

            FileSvgConverter fileReader = new FileSvgConverter(_saveXaml, _saveZaml);

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

                        fileReader.Convert(svgFileName, xamlFilePath);

                        File.SetAttributes(xamlFilePath, fileAttr);
                        // if required to set the security or access control
                        if (_includeSecurity)
                        {
                            File.SetAccessControl(xamlFilePath, File.GetAccessControl(svgFileName));
                        }

                        _convertedCount++;
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
