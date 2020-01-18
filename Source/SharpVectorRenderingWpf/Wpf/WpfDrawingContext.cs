using System;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Runtime;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfDrawingContext : DependencyObject, IEnumerable<DrawingGroup>
    {
        #region Private Fields

        private const double DefaultDpi = 96.0d;

        private const string RegisteredIdKey = "_registeredIds";

        private string _name;

        protected readonly double _dpiX;
        protected readonly double _dpiY;

        private bool _renderingClip;
        private bool _isFragment;

        private Rect _quickBounds;
        private object _tag;
        private DrawingGroup _rootDrawing;
        private DrawingGroup _linkDrawing;

        private WpfDrawingSettings _settings;

        private WpfLinkVisitor _linkVisitor;
        private WpfFontFamilyVisitor _fontFamilyVisitor;
        private WpfEmbeddedImageVisitor _imageVisitor;
        private WpfIDVisitor _idVisitor;
        private WpfClassVisitor _classVisitor;

        private Stack<DrawingGroup> _drawStack;

        private HashSet<string> _registeredIds;

        private WpfDrawingDocument _drawingDocument;
        private Dictionary<string, WpfSvgPaintContext> _paintContexts;

        #endregion

        #region Constructors and Destructor

        public WpfDrawingContext(bool isFragment)
            : this(isFragment, new WpfDrawingSettings())
        {
        }

        public WpfDrawingContext(bool isFragment, WpfDrawingSettings settings)
        {
            var sysParam = typeof(SystemParameters);

            var dpiXProperty = sysParam.GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = sysParam.GetProperty("Dpi",  BindingFlags.NonPublic | BindingFlags.Static);

            _dpiX = (int)dpiXProperty.GetValue(null, null);
            _dpiY = (int)dpiYProperty.GetValue(null, null);

            if (settings == null)
            {
                settings = new WpfDrawingSettings();                
            }
            _quickBounds   = Rect.Empty;
            _isFragment    = isFragment;
            _settings      = settings;
            _drawStack     = new Stack<DrawingGroup>();
            _paintContexts = new Dictionary<string, WpfSvgPaintContext>(StringComparer.Ordinal);

            _registeredIds = settings[RegisteredIdKey] as HashSet<string>;
            if (_registeredIds == null)
            {
                _registeredIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                settings[RegisteredIdKey] = _registeredIds;
            }

            var visitors = settings.Visitors;
            if (visitors != null)
            {
                WpfLinkVisitor linkVisitor = visitors.LinkVisitor;
                if (linkVisitor != null)
                {
                    _linkVisitor = linkVisitor;
                }
                WpfFontFamilyVisitor fontFamilyVisitor = visitors.FontFamilyVisitor;
                if (fontFamilyVisitor != null)
                {
                    _fontFamilyVisitor = fontFamilyVisitor;
                }
                WpfEmbeddedImageVisitor imageVisitor = visitors.ImageVisitor;
                if (imageVisitor != null)
                {
                    _imageVisitor = imageVisitor;
                }
                WpfIDVisitor idVisitor = visitors.IDVisitor;
                if (idVisitor != null)
                {
                    _idVisitor = idVisitor;
                }
                WpfClassVisitor classVisitor = visitors.ClassVisitor;
                if (classVisitor != null)
                {
                    _classVisitor = classVisitor;
                }
            }
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the drawing stack.
        /// </summary>
        /// <value>
        /// The number of elements contained in the drawing stack.
        /// </value>
        public int Count
        {
            get {
                if (_drawStack != null)
                {
                    return _drawStack.Count;
                }

                return 0;
            }
        }

        public bool RenderingClipRegion
        {
            get {
                return _renderingClip;
            }
            set {
                _renderingClip = value;
            }
        }

        public DrawingGroup Root
        {
            get {
                return _rootDrawing;
            }
            internal set {
                if (value != null)
                {
                    _rootDrawing = value;

                    _quickBounds.Union(_rootDrawing.Bounds);
                }
            }
        }

        public DrawingGroup Links
        {
            get {
                return _linkDrawing;
            }
        }

        public bool IsFragment
        {
            get {
                return _isFragment;
            }
        }

        public object Tag
        {
            get {
                return _tag;
            }
            set {
                _tag = value;
            }
        }

        public WpfDrawingSettings Settings
        {
            get {
                return _settings;
            }
            set {
                if (value != null)
                {
                    _settings = value;
                }
            }
        }

        public WpfLinkVisitor LinkVisitor
        {
            get {
                return _linkVisitor;
            }
            set {
                _linkVisitor = value;
            }
        }

        public WpfEmbeddedImageVisitor ImageVisitor
        {
            get {
                return _imageVisitor;
            }
            set {
                _imageVisitor = value;
            }
        }

        public WpfFontFamilyVisitor FontFamilyVisitor
        {
            get {
                return _fontFamilyVisitor;
            }
            set {
                _fontFamilyVisitor = value;
            }
        }

        public WpfIDVisitor IDVisitor
        {
            get {
                return _idVisitor;
            }
            set {
                _idVisitor = value;
            }
        }

        public WpfClassVisitor ClassVisitor
        {
            get {
                return _classVisitor;
            }
            set {
                _classVisitor = value;
            }
        }

        public Rect Bounds
        {
            get {
                return _quickBounds;
            }
        }

        /// <summary>Gets the DPI along X axis.</summary>
        /// <value>The DPI along the X axis.</value>
        public double PixelsPerInchX
        {
            get {
                return _dpiX;
            }
        }

        /// <summary>Gets the DPI along Y axis.</summary>
        /// <value>The DPI along the Y axis.</value>
        public double PixelsPerInchY
        {
            get {
                return _dpiY;
            }
        }

        /// <summary>Gets the DPI scale on the X axis.</summary>
        /// <value>The DPI scale for the X axis.</value>
        public double DpiScaleX
        {
            get {
                return _dpiX / DefaultDpi;
            }
        }

        /// <summary>Gets the DPI scale on the Yaxis.</summary>
        /// <value>The DPI scale for the Y axis.</value>
        public double DpiScaleY
        {
            get {
                return _dpiY / DefaultDpi;
            }
        }

        /// <summary>Get or sets the PixelsPerDip at which the text should be rendered.</summary>
        /// <value>The current PixelsPerDip value.</value>
        public double PixelsPerDip
        {
            get {
                return _dpiY / DefaultDpi;
            }
        }

        #endregion

        #region Internal Properties

        internal bool OptimizePath
        {
            get {
                return _settings.OptimizePath;
            }
        }

        internal bool TextAsGeometry
        {
            get {
                return _settings.TextAsGeometry;
            }
            set {
                _settings.TextAsGeometry = value;
            }
        }

        internal bool IncludeRuntime
        {
            get {
                return _settings.IncludeRuntime;
            }
        }

        internal CultureInfo CultureInfo
        {
            get {
                return _settings.CultureInfo;
            }
        }

        internal CultureInfo EnglishCultureInfo
        {
            get {
                return _settings.NeutralCultureInfo;
            }
        }

        internal string DefaultFontName
        {
            get {
                return _settings.DefaultFontName;
            }
        }

        #endregion

        #region Public Methods

        public void UpdateBounds(Rect bounds)
        {
            _quickBounds.Union(bounds);
        }

        /// <summary>
        /// Removes all objects from the drawing stack.
        /// </summary>
        public void Clear()
        {
            if (_drawStack != null)
            {
                _drawStack.Clear();
            }
        }

        /// <summary>
        /// Determines whether an element is in the drawing stack.
        /// </summary>
        /// <param name="item">The object to locate in the drawing stack. The value can be null for reference types.</param>
        /// <returns>true if item is found in the drawing stack; otherwise, false.</returns>
        public bool Contains(DrawingGroup item)
        {
            if (_drawStack != null)
            {
                return _drawStack.Contains(item);
            }

            return false;
        }

        /// <summary>
        /// Copies the drawing stack to an existing one-dimensional array, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional System.Array that is the destination of the elements
        /// copied from drawing stack. The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of array.
        /// <para>-or-</para>
        /// The number of elements in the source drawing stack is greater than the available 
        /// space from <paramref name="arrayIndex"/> to the end of the destination array.
        /// </exception>
        public void CopyTo(DrawingGroup[] array, int arrayIndex)
        {
            if (_drawStack != null)
            {
                _drawStack.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// Returns the object at the top of the drawing stack without removing it.
        /// </summary>
        /// <returns>The object at the top of the drawing stack.</returns>
        /// <exception cref="System.InvalidOperationException">The drawing stack is empty.</exception>
        public DrawingGroup Peek()
        {
            if (_drawStack != null && _drawStack.Count != 0)
            {
                return _drawStack.Peek();
            }
            return null;
        }

        /// <summary>
        /// Removes and returns the object at the top of the drawing stack.
        /// </summary>
        /// <returns>The object removed from the top of the drawing stack.</returns>
        /// <exception cref="System.InvalidOperationException">The drawing stack is empty.</exception>
        public DrawingGroup Pop()
        {
            if (_drawStack != null && _drawStack.Count != 0)
            {
                return _drawStack.Pop();
            }

            return null;
        }

        /// <summary>
        /// Inserts an object at the top of the drawing stack.
        /// </summary>
        /// <param name="item">The object to push onto the drawing stack. The value can be null for reference types.</param>
        public void Push(DrawingGroup item)
        {
            if (_drawStack != null && item != null)
            {
                _drawStack.Push(item);
            }
        }

        /// <summary>
        /// Copies the drawing stack to a new array.
        /// </summary>
        /// <returns>A new array containing copies of the elements of the drawing stack.</returns>
        public DrawingGroup[] ToArray()
        {
            if (_drawStack != null)
            {
                return _drawStack.ToArray();
            }
            return new DrawingGroup[0];
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the drawing stack,
        /// if that number is less than 90 percent of current capacity.
        /// </summary>
        public void TrimExcess()
        {
            if (_drawStack != null)
            {
                _drawStack.TrimExcess();
            }
        }

        public void Initialize(WpfLinkVisitor linkVisitor, WpfFontFamilyVisitor fontFamilyVisitor,
            WpfEmbeddedImageVisitor imageVisitor)
        {
            //TODO: Depreciate this operation as we have more visitors...
            if (linkVisitor != null)
            {
                _linkVisitor = linkVisitor;
            }
            if (fontFamilyVisitor != null)
            {
                _fontFamilyVisitor = fontFamilyVisitor;
            }
            if (imageVisitor != null)
            {
                _imageVisitor = imageVisitor;
            }

            _rootDrawing = new DrawingGroup();

            this.Push(_rootDrawing);

            if (_idVisitor != null && !_idVisitor.IsInitialized)
            {
                _idVisitor.Initialize(this);
            }
            if (_linkVisitor != null && !_linkVisitor.IsInitialized)
            {
                _linkVisitor.Initialize(this);
            }
            if (_classVisitor != null && !_classVisitor.IsInitialized)
            {
                _classVisitor.Initialize(this);
            }
            if (_fontFamilyVisitor != null && !_fontFamilyVisitor.IsInitialized)
            {
                _fontFamilyVisitor.Initialize(this);
            }
            if (_imageVisitor != null && !_imageVisitor.IsInitialized)
            {
                _imageVisitor.Initialize(this);
            }
            if (_linkVisitor != null && _linkVisitor.Aggregates)
            {
                _linkDrawing = new DrawingGroup();

                string groupId = _linkVisitor.AggregatedLayerName;
                if (!string.IsNullOrWhiteSpace(groupId))
                {
                    SvgObject.SetName(_linkDrawing, groupId);
                }

                _linkVisitor.Initialize(_linkDrawing, this);
            }
        }

        public void Uninitialize()
        {
            if (_idVisitor != null)
            {
                _idVisitor.Uninitialize();
            }
            if (_linkVisitor != null)
            {
                _linkVisitor.Uninitialize();
            }
            if (_classVisitor != null)
            {
                _classVisitor.Uninitialize();
            }
            if (_fontFamilyVisitor != null)
            {
                _fontFamilyVisitor.Uninitialize();
            }
            if (_imageVisitor != null)
            {
                _imageVisitor.Uninitialize();
            }
        }

        public void BeginDrawing(WpfDrawingDocument drawingDocument)
        {
            _drawingDocument = drawingDocument;
        }

        public void EndDrawing()
        {
            if (_rootDrawing != null && _linkDrawing != null)
            {
                if (_linkDrawing.Children.Count != 0)
                {
                    _rootDrawing.Children.Add(_linkDrawing);
                }
            }
        }

        public bool IsRegisteredId(string elementId)
        {
            if (string.IsNullOrWhiteSpace(elementId))
            {
                return false;
            }

            if (_registeredIds != null && _registeredIds.Count != 0)
            {
                return _registeredIds.Contains(elementId);
            }

            return false;
        }

        public void RegisterId(string elementId)
        {
            if (string.IsNullOrWhiteSpace(elementId))
            {
                return;
            }

            if (_registeredIds != null)
            {
                _registeredIds.Add(elementId);
            }
        }

        public void UnRegisterId(string elementId)
        {
            if (string.IsNullOrWhiteSpace(elementId))
            {
                return;
            }

            if (_registeredIds != null)
            {
                _registeredIds.Remove(elementId);
            }
        }

        public bool IsPaintContext(string guidId)
        {
            if (_paintContexts != null && _paintContexts.Count != 0)
            {
                return _paintContexts.ContainsKey(guidId);
            }

            return false;
        }

        public WpfSvgPaintContext GetPaintContext(string guidId)
        {
            if (_paintContexts != null && _paintContexts.Count != 0)
            {
                if (_paintContexts.ContainsKey(guidId))
                {
                    return _paintContexts[guidId];
                }
            }
            return null;
        }

        public void RegisterPaintContext(WpfSvgPaintContext paintContext)
        {
            if (_paintContexts != null)
            {
                _paintContexts[paintContext.Id] = paintContext;
            }
        }

        public void UnRegisterPaintContext(string guidId)
        {
            if (_paintContexts != null)
            {
                _paintContexts.Remove(guidId);
            }
        }

        public void RegisterDrawing(string elementId, string uniqueId, Drawing drawing)
        {
            if (_drawingDocument != null)
            {
                if (_settings != null && _settings.IncludeRuntime)
                {
                    if (!string.IsNullOrWhiteSpace(elementId))
                    {
                        SvgObject.SetId(drawing, elementId);
                    }
                    if (!string.IsNullOrWhiteSpace(uniqueId))
                    {
                        SvgObject.SetUniqueId(drawing, uniqueId);
                    }
                }
                _drawingDocument.Add(elementId, uniqueId, drawing);
            }
        }

        #endregion

        #region IEnumerable Members

        //
        // Summary:
        //     Returns an enumerator for the drawing stack.
        //
        // Returns:
        //     An drawing stack.Enumerator for the drawing stack.
        public IEnumerator<DrawingGroup> GetEnumerator()
        {
            if (_drawStack != null)
            {
                return _drawStack.GetEnumerator();
            }

            return null;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (_drawStack != null)
            {
                return _drawStack.GetEnumerator();
            }

            return null;
        }

        #endregion
    }
}
