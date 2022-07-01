using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Threading;

using SharpVectors.Dom;
using SharpVectors.Runtime;
using SharpVectors.Converters;
using SharpVectors.Renderers;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ResourceSvgConverter : SvgConverter
    {
        #region Private Fields

        private FileSvgReader _fileReader;

        private ResourceDictionary _resourceDictionary;
        private IList<ResourceItem> _resourceItems;

        private WpfResourceSettings _resourceSettings;
        private WpfDrawingResources _drawingResources;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSvgConverter"/> 
        /// class with the specified drawing or rendering settings.
        /// </summary>
        /// <param name="drawingSettings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        public ResourceSvgConverter(WpfDrawingSettings drawingSettings)
            : this(drawingSettings, new WpfResourceSettings())
        {
        }

        public ResourceSvgConverter(WpfDrawingSettings drawingSettings, WpfResourceSettings resourceSettings)
            : base(drawingSettings)
        {
            _resourceSettings = resourceSettings;
            if (_resourceSettings == null)
            {
                _resourceSettings = new WpfResourceSettings();
            }
        }

        #endregion

        #region Public Properties

        public bool IsResolverRegistered
        {
            get {
                if (_resourceSettings == null)
                {
                    return false;
                }
                return _resourceSettings.IsResolverRegistered();
            }
        }

        public int SourceCount
        {
            get {
                if (_resourceSettings == null)
                {
                    return 0;
                }
                return _resourceSettings.SourceCount;
           }
        }

        public WpfResourceSettings ResourceSettings
        {
            get {
                return _resourceSettings;
            }
            set {
                if (value != null)
                {
                    _resourceSettings = value;
                }
            }
        }

        #endregion

        #region Public Methods

        public void RegisterResolver(IResourceKeyResolver keyResolver)
        {
            if (_resourceSettings == null || keyResolver == null)
            {
                return;
            }

            _resourceSettings.RegisterResolver(keyResolver);
        }

        public void RegisterResolver(Func<IResourceKeyResolver> keyResolver)
        {
            if (_resourceSettings == null || keyResolver == null)
            {
                return;
            }

            _resourceSettings.RegisterResolver(keyResolver);
        }

        public IResourceKeyResolver RetrieveResolver()
        {
            if (_resourceSettings == null)
            {
                return null;
            }
            return _resourceSettings.RetrieveResolver();
        }

        public bool AddSource(string sourcePath)
        {
            if (_resourceSettings == null || string.IsNullOrWhiteSpace(sourcePath))
            {
                return false;
            }

            return _resourceSettings.AddSource(sourcePath);
        }

        public bool RemoveSource(string sourcePath)
        {
            if (_resourceSettings == null || string.IsNullOrWhiteSpace(sourcePath))
            {
                return false;
            }

            return _resourceSettings.RemoveSource(sourcePath);
        }

        public void ClearSources()
        {
            if (_resourceSettings == null)
            {
                return;
            }

            _resourceSettings.ClearSources();
        }

        public string Convert()
        {
            if (this.CreateResourceItems() == false)
            {
                return string.Empty;
            }
            if (this.CreateResourceDictionary() == false)
            {
                return string.Empty;
            }

            XmlXamlWriter writer = new XmlXamlWriter(_wpfSettings);
            writer.NumberDecimalDigits = _resourceSettings.NumericPrecision;
            writer.IndentSpaces = _resourceSettings.IndentSpaces;

            return writer.Save(_resourceDictionary);
        }

        public void Convert(string outputPath)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException("The resource directionary output path is required.", nameof(outputPath));
            }

            if (this.CreateResourceItems() == false)
            {
                return;
            }
            if (this.CreateResourceDictionary() == false)
            {
                return;
            }

            XmlXamlWriter writer = new XmlXamlWriter(_wpfSettings);
            writer.NumberDecimalDigits = _resourceSettings.NumericPrecision;
            writer.IndentSpaces = _resourceSettings.IndentSpaces;

            using (var streamWriter = File.CreateText(outputPath))
            {
                writer.Save(_resourceDictionary, streamWriter);
            }
        }

        public void Convert(Stream outputStream)
        {
            if (outputStream == null)
            {
                throw new ArgumentException("The resource directionary output path is required.", nameof(outputStream));
            }

            if (this.CreateResourceItems() == false)
            {
                return;
            }
            if (this.CreateResourceDictionary() == false)
            {
                return;
            }

            XmlXamlWriter writer = new XmlXamlWriter(_wpfSettings);
            writer.NumberDecimalDigits = _resourceSettings.NumericPrecision;
            writer.IndentSpaces = _resourceSettings.IndentSpaces;

            writer.Save(_resourceDictionary, outputStream);
        }

        public void Convert(TextWriter outputWriter)
        {
            if (outputWriter == null)
            {
                throw new ArgumentException("The resource directionary output path is required.", nameof(outputWriter));
            }

            if (this.CreateResourceItems() == false)
            {
                return;
            }
            if (this.CreateResourceDictionary() == false)
            {
                return;
            }

            XmlXamlWriter writer = new XmlXamlWriter(_wpfSettings);
            writer.NumberDecimalDigits = _resourceSettings.NumericPrecision;
            writer.IndentSpaces = _resourceSettings.IndentSpaces;

            writer.Save(_resourceDictionary, outputWriter);
        }

        #endregion

        #region Private Methods

        private bool CreateResourceItems()
        {
            if (_resourceSettings == null || _resourceSettings.SourceCount == 0)
            {
                return false;
            }
            if (_resourceItems == null || _resourceItems.Count != 0)
            {
                _resourceItems = new List<ResourceItem>();
            }

            if (_wpfSettings == null)
            {
                _wpfSettings = new WpfDrawingSettings();
            }

            _wpfSettings.CultureInfo = _wpfSettings.NeutralCultureInfo;
            _wpfSettings.IncludeRuntime = false;
            _wpfSettings.TextAsGeometry = true;
            _wpfSettings.InteractiveMode = SvgInteractiveModes.None;

            _wpfSettings[WpfDrawingSettings.PropertyIsResources] = true;

            _fileReader = new FileSvgReader(_wpfSettings);
            _fileReader.SaveXaml = false;
            _fileReader.SaveZaml = false;

            int resourceIndex = 0;
            foreach (var svgSource in _resourceSettings.Sources)
            {
                if (string.IsNullOrWhiteSpace(svgSource))
                {
                    continue;
                }

                // A file or directory
                if (svgSource.EndsWith(SvgConstants.FileExt, StringComparison.OrdinalIgnoreCase)
                    || svgSource.EndsWith(SvgConstants.FileExtZ, StringComparison.OrdinalIgnoreCase))
                {
                    AddResource(svgSource, resourceIndex);
                    resourceIndex++;
                }
                else if (Directory.Exists(svgSource))
                {
                    foreach (var svgFilePath in Directory.EnumerateFiles(svgSource, "*.svg"))
                    {
                        AddResource(svgFilePath, resourceIndex);
                        resourceIndex++;
                    }
                }
            }

            return _resourceItems.Count != 0;
        }

        private void AddResource(string svgFilePath, int resourceIndex)
        {
            var drawing = _fileReader.Read(svgFilePath);

            string fileName = Path.GetFileNameWithoutExtension(svgFilePath);
            _resourceItems.Add(new ResourceItem(resourceIndex, drawing, fileName));
        }

        private bool CreateResourceDictionary()
        {
            if (_resourceItems == null || _resourceItems.Count == 0)
            {
                return false;
            }
            _resourceDictionary = new ResourceDictionary();

            if (_drawingResources == null)
            {
                _drawingResources = _wpfSettings.DrawingResources;
            }

            _resourceSettings.CopyTo(_drawingResources);
            _drawingResources.InitialiseKeys();
            var resourceFreeze = _resourceSettings.ResourceFreeze;

            var keyResolver = _resourceSettings.RetrieveResolver();
            if (keyResolver == null || keyResolver.IsValid == false)
            {
                keyResolver = new ResourceKeyResolver();
            }

            keyResolver.BeginResolve();

            int itemCount = _resourceSettings.UseResourceIndex ? 0 : 1;
            foreach (var resourceItem in _resourceItems)
            {
                var drawGroup = resourceItem.Drawing;
                if (drawGroup.Children.Count == 1)
                {
                    drawGroup = (DrawingGroup)drawGroup.Children[0];
                    string drawingName = drawGroup.GetValue(FrameworkElement.NameProperty) as string;
                    if (!string.IsNullOrWhiteSpace(drawingName) && string.Equals(drawingName, SvgObject.DrawLayer))
                    {
                        drawGroup.SetValue(FrameworkElement.NameProperty, null);
                    }
                }

                string resourceName = string.Empty;
                if (_drawingResources.ResourceMode == ResourceModeType.Image)
                {
                    var drawImage = new DrawingImage(drawGroup);
                    resourceName = keyResolver.Resolve(drawImage, itemCount, resourceItem.FileName, resourceItem.FileName);
                    if (resourceFreeze)
                    {
                        drawImage.Freeze();
                    }
                    _resourceDictionary.Add(resourceName, drawImage);
                }
                else
                {
                    resourceName = keyResolver.Resolve(drawGroup, itemCount, resourceItem.FileName, resourceItem.FileName);
                    if (resourceFreeze)
                    {
                        drawGroup.Freeze();
                    }
                    _resourceDictionary.Add(resourceName, drawGroup);
                }
                resourceItem.ResourceName = resourceName;

                itemCount++;
            }

            keyResolver.EndResolve();

            return true;
        }

        #endregion

        #region Private Classes

        public sealed class ResourceItem
        {
            private int _index;
            private string _fileName;
            private DrawingGroup _drawing;

            private string _resourceName;

            public ResourceItem()
            {
            }

            public ResourceItem(int index, DrawingGroup drawing, string fileName)
            {
                _index = index;
                _drawing = drawing;
                _fileName = fileName;
            }

            public int Index
            {
                get {
                    return _index;
                }
            }

            public string Name
            {
                get {
                    return string.Format("{0:D3}: {1}", _index + 1, _fileName);
                }
            }

            public string FileName
            {
                get {
                    return _fileName;
                }
            }

            public DrawingGroup Drawing
            {
                get {
                    return _drawing;
                }
            }

            public DrawingImage Image
            {
                get {
                    return new DrawingImage(_drawing);
                }
            }

            public string ResourceName
            {
                get {
                    if (string.IsNullOrWhiteSpace(_resourceName))
                    {
                        return _fileName;
                    }
                    return _resourceName;
                }
                set {
                    _resourceName = value;
                }
            }
        }

        #endregion
    }
}
