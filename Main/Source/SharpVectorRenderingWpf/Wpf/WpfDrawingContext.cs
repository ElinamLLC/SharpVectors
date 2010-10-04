using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfDrawingContext : DependencyObject, IEnumerable<DrawingGroup>
    {
        #region Private Fields

        private bool              _renderingClip;
        private bool              _textAsGeometry;
        private bool              _includeRuntime;
        private bool              _optimizePath;
        private bool              _isFragment;

        private object            _tag;
        private string            _defaultFontName;
        private CultureInfo       _culture;
        private CultureInfo       _englishCulture;
        private DrawingGroup      _rootDrawing;
        private DrawingGroup      _linkDrawing;

        private WpfLinkVisitor          _linkVisitor;
        private WpfFontFamilyVisitor    _fontFamilyVisitor;
        private WpfEmbeddedImageVisitor _imageVisitor;

        private static FontFamily _defaultFontFamily;
        private static FontFamily _genericSerif;
        private static FontFamily _genericSansSerif;
        private static FontFamily _genericMonospace;

        private Stack<DrawingGroup> _drawStack;

        private HashSet<string> _registeredIds;

        #endregion

        #region Constructors and Destructor

        public WpfDrawingContext(bool isFragment)
        {
            _textAsGeometry  = false;
            _optimizePath    = true;
            _includeRuntime  = true;
            _isFragment      = isFragment;

            _defaultFontName = "Arial";
            _englishCulture  = CultureInfo.GetCultureInfo("en-us");
            _culture         = CultureInfo.GetCultureInfo("ja-JP");
            _drawStack       = new Stack<DrawingGroup>();

            _registeredIds   = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region Public Properties

        // Summary:
        //     Gets the number of elements contained in the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Returns:
        //     The number of elements contained in the System.Collections.Generic.Stack<DrawingGroup>.
        public int Count 
        {
            get
            {
                if (_drawStack != null)
                {
                    return _drawStack.Count;
                }

                return 0;
            }
        }

        public bool OptimizePath
        {
            get 
            { 
                return _optimizePath; 
            }
            set 
            { 
                _optimizePath = value; 
            }
        }

        public bool RenderingClipRegion
        {
            get
            {
                return _renderingClip;
            }
            set
            {
                _renderingClip = value;
            }
        }

        public bool TextAsGeometry
        {
            get
            {
                return _textAsGeometry;
            }
            set
            {
                _textAsGeometry = value;
            }
        }

        public bool IncludeRuntime
        {
            get
            {
                return _includeRuntime;
            }
            set
            {
                _includeRuntime = value;
            }
        }

        public string DefaultFontName
        {
            get
            {
                return _defaultFontName;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }

                if (!String.IsNullOrEmpty(value))
                {
                    _defaultFontName   = value;
                    _defaultFontFamily = new FontFamily(value);
                }
            }
        }

        public CultureInfo CultureInfo
        {
            get
            {
                return _culture;
            }
            set
            {
                if (value != null)
                {
                    _culture = value;
                }
            }
        }

        public CultureInfo EnglishCultureInfo
        {
            get
            {
                return _englishCulture;
            }
        }

        public DrawingGroup Root
        {
            get
            {
                return _rootDrawing;
            }
            internal set
            {
                if (value != null)
                {
                    _rootDrawing = value;
                }
            }
        }

        public DrawingGroup Links
        {
            get
            {
                return _linkDrawing;
            }
        }

        public static FontFamily DefaultFontFamily
        {
            get
            {
                if (_defaultFontFamily == null)
                {
                    _defaultFontFamily = new FontFamily("Arial");
                }

                return _defaultFontFamily;
            }
        }

        public static FontFamily GenericSerif
        {
            get
            {
                if (_genericSerif == null)
                {
                    _genericSerif = new FontFamily("Times New Roman");
                }

                return _genericSerif;
            }
            set
            {
                if (value != null)
                {
                    _genericSerif = value;
                }
            }
        }

        public static FontFamily GenericSansSerif
        {
            get
            {
                if (_genericSansSerif == null)
                {
                    // Possibilities: Tahoma, Arial, Verdana, Trebuchet, MS Sans Serif, Helvetica
                    _genericSansSerif = new FontFamily("Tahoma");
                }

                return _genericSansSerif;
            }
            set
            {
                if (value != null)
                {
                    _genericSansSerif = value;
                }
            }
        }

        public static FontFamily GenericMonospace
        {
            get
            {
                if (_genericMonospace == null)
                {
                    // Possibilities: Courier New, MS Gothic
                    _genericMonospace = new FontFamily("MS Gothic");
                }

                return _genericMonospace;
            }
            set
            {
                if (value != null)
                {
                    _genericMonospace = value;
                }
            }
        }

        public bool IsFragment
        {
            get
            {
                return _isFragment;
            }
        }

        public object Tag
        {
            get 
            { 
                return _tag; 
            }
            set 
            { 
                _tag = value; 
            }
        }

        public WpfLinkVisitor LinkVisitor
        {
            get 
            { 
                return _linkVisitor; 
            }
            set 
            { 
                _linkVisitor = value; 
            }
        }

        public WpfEmbeddedImageVisitor ImageVisitor
        {
            get
            {
                return _imageVisitor;
            }
            set
            {
                _imageVisitor = value;
            }
        }

        public WpfFontFamilyVisitor FontFamilyVisitor
        {
            get
            {
                return _fontFamilyVisitor;
            }
            set
            {
                _fontFamilyVisitor = value;
            }
        }

        #endregion

        #region Public Methods

        // Summary:
        //     Removes all objects from the System.Collections.Generic.Stack<DrawingGroup>.
        public void Clear()
        {
            if (_drawStack != null)
            {
                _drawStack.Clear();
            }
        }

        //
        // Summary:
        //     Determines whether an element is in the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Parameters:
        //   item:
        //     The object to locate in the System.Collections.Generic.Stack<DrawingGroup>. The value
        //     can be null for reference types.
        //
        // Returns:
        //     true if item is found in the System.Collections.Generic.Stack<DrawingGroup>; otherwise,
        //     false.
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
        //     Copies the System.Collections.Generic.Stack<DrawingGroup> to an existing one-dimensional
        //     System.Array, starting at the specified array index.
        //
        // Parameters:
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements
        //     copied from System.Collections.Generic.Stack<DrawingGroup>. The System.Array must have
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
        //     of elements in the source System.Collections.Generic.Stack<DrawingGroup> is greater
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
        //     Returns the object at the top of the System.Collections.Generic.Stack<DrawingGroup>
        //     without removing it.
        //
        // Returns:
        //     The object at the top of the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.Collections.Generic.Stack<DrawingGroup> is empty.
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
        //     Removes and returns the object at the top of the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Returns:
        //     The object removed from the top of the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.Collections.Generic.Stack<DrawingGroup> is empty.
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
        //     Inserts an object at the top of the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Parameters:
        //   item:
        //     The object to push onto the System.Collections.Generic.Stack<DrawingGroup>. The value
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
        //     Copies the System.Collections.Generic.Stack<DrawingGroup> to a new array.
        //
        // Returns:
        //     A new array containing copies of the elements of the System.Collections.Generic.Stack<DrawingGroup>.
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
        //     Sets the capacity to the actual number of elements in the System.Collections.Generic.Stack<DrawingGroup>,
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
            _linkVisitor       = linkVisitor;
            _fontFamilyVisitor = fontFamilyVisitor;
            _imageVisitor      = imageVisitor;

            _rootDrawing = new DrawingGroup();

            this.Push(_rootDrawing);

            if (_linkVisitor != null && _linkVisitor.Aggregates)
            {
                _linkDrawing = new DrawingGroup();

                string groupId = _linkVisitor.AggregatedLayerName;
                if (!String.IsNullOrEmpty(groupId))
                {
                    _linkDrawing.SetValue(FrameworkElement.NameProperty, groupId);
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
            if (String.IsNullOrEmpty(elementId))
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
            if (String.IsNullOrEmpty(elementId))
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
            if (String.IsNullOrEmpty(elementId))
            {
                return;
            }

            if (_registeredIds != null)
            {
                _registeredIds.Remove(elementId);
            }
        }

        #endregion

        #region IEnumerable Members

        //
        // Summary:
        //     Returns an enumerator for the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Returns:
        //     An System.Collections.Generic.Stack<DrawingGroup>.Enumerator for the System.Collections.Generic.Stack<DrawingGroup>.
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
