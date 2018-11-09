using System;
using System.Globalization;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfDrawingContext : DependencyObject, IEnumerable<DrawingGroup>
    {
        #region Private Fields

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

        private Dictionary<Guid, WpfSvgPaintContext> _paintContexts;

        #endregion

        #region Constructors and Destructor

        public WpfDrawingContext(bool isFragment)
        {
            _quickBounds   = Rect.Empty;
            _isFragment    = isFragment;
            _settings      = new WpfDrawingSettings();
            _drawStack     = new Stack<DrawingGroup>();
            _registeredIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _paintContexts = new Dictionary<Guid, WpfSvgPaintContext>();
        }

        public WpfDrawingContext(bool isFragment, WpfDrawingSettings settings)
            : this(isFragment)
        {
            if (settings != null)
            {
                _settings = settings;
            }
        }

        #endregion

        #region Public Properties

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
            get
            {
                return _classVisitor;
            }
            set
            {
                _classVisitor = value;
            }
        }

        public Rect Bounds
        {
            get {
                return _quickBounds;
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

        //
        // Summary:
        //     Copies the drawing stack to an existing one-dimensional
        //     System.Array, starting at the specified array index.
        //
        // Parameters:
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements
        //     copied from drawing stack. The System.Array must have
        //     zero-based indexing.
        //
        //   arrayIndex:
        //     The zero-based index in array at which copying begins.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     array is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     arrayIndex is less than zero.
        //
        //   System.ArgumentException:
        //     arrayIndex is equal to or greater than the length of array.  -or- The number
        //     of elements in the source drawing stack is greater
        //     than the available space from arrayIndex to the end of the destination array.
        public void CopyTo(DrawingGroup[] array, int arrayIndex)
        {
            if (_drawStack != null)
            {
                _drawStack.CopyTo(array, arrayIndex);
            }
        }

        //
        // Summary:
        //     Returns the object at the top of the drawing stack
        //     without removing it.
        //
        // Returns:
        //     The object at the top of the drawing stack.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The drawing stack is empty.
        public DrawingGroup Peek()
        {
            if (_drawStack != null)
            {
                return _drawStack.Peek();
            }

            return null;
        }

        //
        // Summary:
        //     Removes and returns the object at the top of the drawing stack.
        //
        // Returns:
        //     The object removed from the top of the drawing stack.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The drawing stack is empty.
        public DrawingGroup Pop()
        {
            if (_drawStack != null)
            {
                return _drawStack.Pop();
            }

            return null;
        }

        //
        // Summary:
        //     Inserts an object at the top of the drawing stack.
        //
        // Parameters:
        //   item:
        //     The object to push onto the drawing stack. The value
        //     can be null for reference types.
        public void Push(DrawingGroup item)
        {
            if (_drawStack != null && item != null)
            {
                _drawStack.Push(item);
            }
        }

        //
        // Summary:
        //     Copies the drawing stack to a new array.
        //
        // Returns:
        //     A new array containing copies of the elements of the drawing stack.
        public DrawingGroup[] ToArray()
        {
            if (_drawStack != null)
            {
                return _drawStack.ToArray();
            }

            return new DrawingGroup[0];
        }

        //
        // Summary:
        //     Sets the capacity to the actual number of elements in the drawing stack,
        //     if that number is less than 90 percent of current capacity.
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
            _linkVisitor = linkVisitor;
            _fontFamilyVisitor = fontFamilyVisitor;
            _imageVisitor = imageVisitor;

            _rootDrawing = new DrawingGroup();

            this.Push(_rootDrawing);

            if (_linkVisitor != null && _linkVisitor.Aggregates)
            {
                _linkDrawing = new DrawingGroup();

                string groupId = _linkVisitor.AggregatedLayerName;
                if (!string.IsNullOrWhiteSpace(groupId))
                {
                    Runtime.SvgObject.SetName(_linkDrawing, groupId);
                }

                linkVisitor.Initialize(_linkDrawing, this);
            }
        }

        public void Uninitialize()
        {
        }

        public void BeginDrawing()
        {
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

        public bool IsPaintContext(Guid guidId)
        {
            if (_paintContexts != null && _paintContexts.Count != 0)
            {
                return _paintContexts.ContainsKey(guidId);
            }

            return false;
        }

        public WpfSvgPaintContext GetPaintContext(Guid guidId)
        {
            if (_paintContexts != null && _paintContexts.Count != 0)
            {
                if(_paintContexts.ContainsKey(guidId))
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

        public void UnRegisterPaintContext(Guid guidId)
        {
            if (_paintContexts != null)
            {
                _paintContexts.Remove(guidId);
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
