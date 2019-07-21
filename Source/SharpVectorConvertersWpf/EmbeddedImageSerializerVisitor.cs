using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Dom.Svg;
using SharpVectors.Runtime;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Converters
{
    public sealed class EmbeddedImageSerializerVisitor : WpfEmbeddedImageVisitor
    {
        #region Public Private Fields

        public const string ImageExt = ".png";

        private bool _saveImages;
        private string _namePrefix;
        private string _saveDirectory;

        private IDictionary<string, ImageSource> _imageCache;

        #endregion

        #region Constructors and Destructor

        public EmbeddedImageSerializerVisitor(string saveDirectory)
            : this(true, saveDirectory)
        {
        }

        public EmbeddedImageSerializerVisitor(bool saveImages, string saveDirectory)
        {
            _saveImages    = saveImages;
            _saveDirectory = saveDirectory;
            _namePrefix    = "_embeddedimage";
            try
            {
                if (!string.IsNullOrWhiteSpace(_saveDirectory))
                {
                    if (!Directory.Exists(_saveDirectory))
                    {
                        Directory.CreateDirectory(_saveDirectory);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            _imageCache = new Dictionary<string, ImageSource>(StringComparer.Ordinal);
        }

        #endregion

        #region Public Properties

        public bool SaveImages
        {
            get {
                return _saveImages;
            }
        }

        public string SaveDirectory
        {
            get {
                return _saveDirectory;
            }
        }

        public string ImageNamePrefix
        {
            get {
                return _namePrefix;
            }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _namePrefix = value;
                }
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(WpfDrawingContext context)
        {
            if (this.IsInitialized)
            {
                return;
            }
            base.Initialize(context);

            _imageCache = new Dictionary<string, ImageSource>(StringComparer.Ordinal);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();

            _imageCache = null;
        }

        public override ImageSource Visit(SvgImageElement element, WpfDrawingContext context)
        {
            if (_imageCache == null)
            {
                _imageCache = new Dictionary<string, ImageSource>(StringComparer.Ordinal);
            }

            var imageId = element.Id;
            if (_imageCache.Count != 0)
            {
                if (!string.IsNullOrWhiteSpace(imageId) && _imageCache.ContainsKey(imageId))
                {
                    return _imageCache[imageId];
                }
            }

            var imageSource = this.GetImage(element, context);

            if (!string.IsNullOrWhiteSpace(imageId))
            {
                _imageCache[imageId] = imageSource;
            }

            return imageSource;
        }

        #endregion

        #region Private Methods

        private ImageSource GetImage(SvgImageElement element, WpfDrawingContext context)
        {
            bool isSavingImages = _saveImages;
            string imagesDir    = _saveDirectory;

            if (context != null && context.Settings.IncludeRuntime == false)
            {
                isSavingImages = true;
                if (string.IsNullOrWhiteSpace(_saveDirectory) ||
                    Directory.Exists(_saveDirectory) == false)
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    if (assembly != null)
                    {
                        imagesDir = Path.GetDirectoryName(assembly.Location);
                    }
                }
            }

            var comparer = StringComparison.OrdinalIgnoreCase;

            string sURI    = element.Href.AnimVal.Replace(" ", "");
            int nColon     = sURI.IndexOf(":", comparer);
            int nSemiColon = sURI.IndexOf(";", comparer);
            int nComma     = sURI.IndexOf(",", comparer);

            string sMimeType = sURI.Substring(nColon + 1, nSemiColon - nColon - 1);

            string sContent = SvgObject.RemoveWhitespace(sURI.Substring(nComma + 1));
            byte[] imageBytes = Convert.FromBase64CharArray(sContent.ToCharArray(),
                0, sContent.Length);

            if (string.Equals(sMimeType, "image/svg+xml", comparer))
            {
                using (var stream = new MemoryStream(imageBytes))
                using (var reader = new FileSvgReader(context.Settings))
                    return new DrawingImage(reader.Read(stream));
            }

            BitmapImage imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            imageSource.StreamSource  = new MemoryStream(imageBytes);
            imageSource.EndInit();

            imageSource.Freeze();

            if (isSavingImages && !string.IsNullOrWhiteSpace(imagesDir) 
                && Directory.Exists(imagesDir))
            {
                var imagePath = this.GetImagePath(imagesDir);

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageSource));

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                BitmapImage savedSource = new BitmapImage();

                savedSource.BeginInit();
                savedSource.CacheOption   = BitmapCacheOption.OnLoad;
                savedSource.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                savedSource.UriSource = new Uri(imagePath);
                savedSource.EndInit();

                savedSource.Freeze();

                return savedSource;
            }

            return imageSource;
        }

        private string GetImagePath(string savedDir)
        {
            if (string.IsNullOrWhiteSpace(savedDir))
            {
                savedDir = _saveDirectory;
            }

            int imageCount = 1;
            var nextPath   = Path.Combine(savedDir, string.Format("{0}{1}{2}",
                _namePrefix, imageCount, ImageExt));

            while (File.Exists(nextPath))
            {
                imageCount++;
                nextPath = Path.Combine(_saveDirectory, string.Format("{0}{1}{2}",
                    _namePrefix, imageCount, ImageExt));
            }

            return nextPath;
        }

        #endregion
    }
}
