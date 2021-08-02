using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace WpfTestOtherSvg.Handlers
{
    public abstract class SvgTestHandler
    {
        #region Public Constants

        public const int OutputWidth    = 500;
        public const int OutputHeight   = 500;

        public const string KeyDir      = "appdir:";
        public const string KeyArgs     = "appargs:";
        public const string KeyApps     = "appname:";
        public const string KeyOut      = "appout";
        public const string KeySync     = "appsync";

        public const string InputExt1   = ".svg";
        public const string InputExt2   = ".svgz";
        public const string OutputExt   = ".png";

        public const string TagBatik    = "batik";
        public const string TagResvg    = "resvg";
        public const string TagSvgNet   = "svgnet";
        public const string TagFirefox  = "firefox";
        public const string TagInkscape = "inkscape";
        public const string TagMagick   = "magick";
        public const string TagRsvg     = "rsvg";

        #endregion

        #region Protected Fields

        protected string _appName;
        protected string _appTag;
        protected string _appDir;
        protected string _inputName;
        protected string _inputDir;
        protected string _fontDir;
        protected string _outputName;
        protected string _outputDir;
        protected string _workingDir;

        protected int _imageWidth;
        protected int _imageHeight;
        protected int _imageDpi;
        protected string _imageExt;

        #endregion

        #region Constructors and Destructor

        protected SvgTestHandler(string appName, string appTag, string inputDir, string workingDir)
        {
            _appName    = appName;
            _appTag     = appTag;
            _inputDir   = inputDir;
            _outputDir  = string.Empty;
            _workingDir = workingDir;
            _imageExt   = OutputExt;
            _imageDpi   = 96;

            if (!string.IsNullOrWhiteSpace(workingDir) && !string.IsNullOrWhiteSpace(appTag))
            {
                _outputDir  = Path.Combine(workingDir, appTag);
            }
            if (!string.IsNullOrWhiteSpace(inputDir) && Directory.Exists(inputDir))
            {
                _fontDir = Path.Combine(Path.GetDirectoryName(inputDir), "fonts");
            }
        }  

        #endregion

        #region Public Properties

        public string AppName
        {
            get { return _appName; }
            protected set { _appName = value; }
        }

        public string AppTag
        {
            get { return _appTag; }
            protected set { _appTag = value; }
        }

        public string AppDir
        {
            get { return _appDir; }
            set { _appDir = value; }
        }

        public string AppFile
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_appName) || string.IsNullOrWhiteSpace(_appDir))
                {
                    return null;
                }
                return Path.Combine(_appDir, _appName);
            }
        }

        public string InputName
        {
            get { return _inputName; }
        }

        public string InputDir
        {
            get { return _inputDir; }
            protected set { _inputDir = value; }
        }

        public string InputFile
        {
            get {
                if (string.IsNullOrWhiteSpace(_inputName) || string.IsNullOrWhiteSpace(_inputDir))
                {
                    return null;
                }
                return Path.Combine(_inputDir, _inputName); 
            }
        }

        public string FontDir
        {
            get { return _fontDir; }
            protected set { _fontDir = value; }
        }

        public string OutputName
        {
            get { return _outputName; }
            set { _outputName = value; }
        }

        public string OutputDir
        {
            get { return _outputDir; }
            protected set { _outputDir = value; }
        }

        public string OutputFile
        {
            get {
                if (string.IsNullOrWhiteSpace(_outputName) || string.IsNullOrWhiteSpace(_outputDir))
                {
                    return null;
                }
                return Path.Combine(_outputDir, _outputName); 
            }
        }

        public string WorkingDir
        {
            get { return _workingDir; }
            protected set { _workingDir = value; }
        }   

        public int ImageWidth
        {
            get
            {
                return _imageWidth;
            }
            set
            {
                if (value < 0)
                    value = 0;
                _imageWidth = value;
            }
        }

        public int ImageHeight
        {
            get
            {
                return _imageHeight;
            }
            set
            {
                if (value < 0)
                    value = 0;
                _imageHeight = value;
            }
        }

        public int ImageDpi
        {
            get
            {
                return _imageDpi;
            }
            set
            {
                if (value < 0)
                    value = 0;
                _imageDpi = value;
            }
        }

        public string ImageExtension
        {
            get
            {
                return _imageExt;
            }
            set
            {
                if (IsValidImage(value))
                {
                    _imageExt = value.ToLower();
                }
                else
                {
                    _imageExt = OutputExt;
                }
            }
        }

        public string ImageMimeType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_imageExt))
                {
                    return "image/png";
                }
                string image = _imageExt.TrimStart('.');
                if (image.Equals("png", StringComparison.OrdinalIgnoreCase))
                {
                    return "image/png";
                }
                if (image.Equals("jpeg", StringComparison.OrdinalIgnoreCase) ||
                    image.Equals("jpg", StringComparison.OrdinalIgnoreCase))
                {
                    return "image/jpeg";
                }
                if (image.Equals("tiff", StringComparison.OrdinalIgnoreCase) ||
                    image.Equals("tif", StringComparison.OrdinalIgnoreCase))
                {
                    return "image/tiff";
                }
                return "image/png";
            }
        }

        public virtual bool IsInitialized
        {
            get
            {
                if (!this.Validate())
                {
                    return false;
                }
                return true;
            }
        }

        #endregion

        #region Public Methods

        public abstract bool Marshal(TextWriter writer, bool singleFile = true);

        public void Initialize(string svgFileName, int width = OutputWidth, int height = OutputHeight)
        {
            if (!string.IsNullOrWhiteSpace(svgFileName) &&
                (svgFileName.EndsWith(InputExt1, StringComparison.OrdinalIgnoreCase)
                || svgFileName.EndsWith(InputExt2, StringComparison.OrdinalIgnoreCase)))
            {
                var inputDir = Path.GetDirectoryName(svgFileName);
                if (!string.IsNullOrWhiteSpace(inputDir) && Directory.Exists(inputDir))
                {
                    _inputDir = inputDir;
                }

                _inputName  = Path.GetFileName(svgFileName);
                _outputName = Path.ChangeExtension(_inputName, OutputExt);
            }
            else
            {
                _inputName  = svgFileName;
                _outputName = svgFileName;
            }

            _imageWidth  = width;
            _imageHeight = height;

            this.OnInitialized();
        }

        #endregion

        #region Protected Methods

        protected virtual bool Validate()
        {
            if (string.IsNullOrWhiteSpace(_appName) || string.IsNullOrWhiteSpace(_appDir))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(_inputName) || string.IsNullOrWhiteSpace(_inputDir))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(_outputName) || string.IsNullOrWhiteSpace(_outputDir))
            {
                return false;
            }
            if (!File.Exists(Path.Combine(_appDir, _appName)) || !File.Exists(Path.Combine(_inputDir, _inputName)))
            {
                return false;
            }

            return true;
        }

        protected abstract void OnInitialized();

        #endregion

        #region Private Methods

        private static bool IsValidImage(string imageExt)
        {
            string image = imageExt.TrimStart('.');
            if (string.IsNullOrWhiteSpace(image))
            {
                return false;
            }
            if (image.Equals("png", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (image.Equals("jpeg", StringComparison.OrdinalIgnoreCase) ||
                image.Equals("jpg", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (image.Equals("tiff", StringComparison.OrdinalIgnoreCase) ||
                image.Equals("tif", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (image.Equals("bmp", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (image.Equals("wdp", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
