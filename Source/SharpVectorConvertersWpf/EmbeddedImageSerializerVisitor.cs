using System;
using System.IO;
using System.IO.Compression;
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
    public sealed class EmbeddedImageSerializerArgs : EventArgs
    {
        public EmbeddedImageSerializerArgs(string imagePath, BitmapImage image)
        {
            this.ImagePath = imagePath;
            this.Image     = image;
        }

        public string ImagePath { get; private set; }
        public BitmapImage Image { get; private set; }
    }

    public sealed class EmbeddedImageSerializerVisitor : WpfEmbeddedImageVisitor
    {
        #region Public Private Fields

        public const string ImageExt = ".png";

        private int _imageCount;

        private bool _saveImages;
        private bool _converterFallback;
        private string _namePrefix;
        private string _saveDirectory;

        private IDictionary<string, ImageSource> _imageCache;

        private event EventHandler<EmbeddedImageSerializerArgs> _imageCreated;

        #endregion

        #region Constructors and Destructor

        public EmbeddedImageSerializerVisitor(bool converterFallback)
            : this(false, null)
        {
            _converterFallback = converterFallback;
        }

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

            _imageCount = 1;
            _imageCache = new Dictionary<string, ImageSource>(StringComparer.Ordinal);
        }

        #endregion

        #region Public Events

        public event EventHandler<EmbeddedImageSerializerArgs> ImageCreated
        {
            add { _imageCreated += value; }
            remove { _imageCreated -= value; }
        }

        #endregion

        #region Public Properties

        public bool SaveImages
        {
            get {
                return _saveImages;
            }
            set {
                _saveImages = value;
            }
        }

        public bool ConverterFallback
        {
            get {
                return _converterFallback;
            }
            set {
                _converterFallback = value;
            }
        }


        public string SaveDirectory
        {
            get {
                return _saveDirectory;
            }
            set {
                _saveDirectory = value;
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
                    var cachedSource = _imageCache[imageId];
                    if (cachedSource != null)
                    {
                        var cachedImage = cachedSource as BitmapImage;
                        if (cachedImage != null)
                        {
                            var imageUri = cachedImage.UriSource;
                            if (imageUri != null)
                            {
                                if (imageUri.IsFile && File.Exists(imageUri.LocalPath))
                                {
                                    return cachedImage;
                                }
                                _imageCache.Remove(imageId);
                            }
                            else if (cachedImage.StreamSource != null)
                            {
                                return cachedImage;
                            }
                        }
                        else
                        {
                            return cachedSource;
                        }
                    }
                    else
                    {
                        _imageCache.Remove(imageId);
                    }
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
            bool isGZiped = sContent.StartsWith(SvgObject.GZipSignature, StringComparison.Ordinal);

            if (string.Equals(sMimeType, "image/svg+xml", comparer))
            {
                if (isGZiped)
                {
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            using (var reader = new FileSvgReader(context.Settings, true))
                            {
                                return new DrawingImage(reader.Read(zipStream));
                            }
                        }
                    }
                }
                else
                {
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        using (var reader = new FileSvgReader(context.Settings, true))
                        {
                            return new DrawingImage(reader.Read(stream));
                        }
                    }
                }
            }

            var memStream = new MemoryStream(imageBytes);

            BitmapImage imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource  = memStream;
            imageSource.CacheOption   = BitmapCacheOption.OnLoad;
            imageSource.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            imageSource.EndInit();

            string imagePath = null;

            if (isSavingImages && !string.IsNullOrWhiteSpace(imagesDir) && Directory.Exists(imagesDir))
            {
                imagePath = this.GetImagePath(imagesDir);

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageSource));

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                imageSource.CreateOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreImageCache;
                imageSource.UriSource     = new Uri(imagePath);

                //imageSource.StreamSource.Dispose();
                //imageSource = null;

                //BitmapImage savedSource = new BitmapImage();

                //savedSource.BeginInit();
                //savedSource.CacheOption   = BitmapCacheOption.None;
                //savedSource.CreateOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreImageCache;
                //savedSource.UriSource = new Uri(imagePath);
                //savedSource.EndInit();

                //savedSource.Freeze();

                //if (_imageCreated != null)
                //{
                //    var eventArgs = new EmbeddedImageSerializerArgs(imagePath, savedSource);
                //    _imageCreated.Invoke(this, eventArgs);
                //}

                //return savedSource;
            }
            else if (_converterFallback)
            {
                //if (_imageCreated != null)
                //{
                //    var eventArgs = new EmbeddedImageSerializerArgs(imagePath, imageSource);
                //    _imageCreated.Invoke(this, eventArgs);
                //}
                return new EmbeddedBitmapSource(memStream, imageSource);
            }
            if (_imageCreated != null)
            {
                var eventArgs = new EmbeddedImageSerializerArgs(imagePath, imageSource);
                _imageCreated.Invoke(this, eventArgs);
            }

            imageSource.Freeze();

            return imageSource;
        }

        private string GetImagePath(string savedDir)
        {
            if (string.IsNullOrWhiteSpace(savedDir))
            {
                savedDir = _saveDirectory;
            }

            int imageCount = _imageCount;
            var nextPath   = Path.Combine(savedDir, string.Format("{0}{1}{2}",
                _namePrefix, imageCount, ImageExt));

            while (File.Exists(nextPath))
            {
                imageCount++;
                nextPath = Path.Combine(_saveDirectory, string.Format("{0}{1}{2}",
                    _namePrefix, imageCount, ImageExt));
            }

            _imageCount = imageCount + 1;

            return nextPath;
        }

        #endregion
    }
}
