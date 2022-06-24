using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

namespace SharpVectors.Renderers.Wpf
{
    public enum WpfResourceMode
    {
        None,
        Drawing,
        Image
    }

    public enum WpfResourceAccess
    {
        None,
        Dynamic,
        Static
    }

    public sealed class WpfDrawingResources
    {
        public static readonly string TagName   = "${name}";
        public static readonly string TagNumber = "${number}";

        public static readonly string DefaultPenNameFormat      = "Pen{0}";
        public static readonly string DefaultColorNameFormat    = "Color{0}";
        public static readonly string DefaultBrushNameFormat    = "Brush{0}";
        public static readonly string DefaultResourceNameFormat = string.Empty;

        public static readonly DependencyProperty SharedProperty =
            DependencyProperty.RegisterAttached("Shared", typeof(bool), typeof(FrameworkElement),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None));

        private bool _bindToColors;
        private bool _bindToResources;
        private bool _bindPenToBrushes;

        private string _penNameFormat;
        private string _colorNameFormat;
        private string _brushNameFormat;
        private string _resourceNameFormat;

        private bool _resourceFreeze;
        private bool _useResourceIndex;
        private WpfResourceMode _resourceMode;
        private WpfResourceAccess _resourceAccess;

        private IDictionary<Pen, int> _pens;
        private IDictionary<Color, int> _colors;
        private IDictionary<SolidColorBrush, int> _brushes;

        private IList<string> _resourceKeys;
        private IDictionary<Pen, string> _penKeys;
        private IDictionary<Color, string> _colorKeys;
        private IDictionary<SolidColorBrush, string> _brushKeys;

        private IDictionary<Color, string> _colorPalette;

        private ResourceDictionary _resourceDictionary;

        public WpfDrawingResources()
        {
            _bindToResources    = true;
            _bindToColors       = true;
            _bindPenToBrushes   = true;

            _resourceFreeze     = true;
            _useResourceIndex   = false;
            _resourceMode       = WpfResourceMode.Drawing;
            _resourceAccess     = WpfResourceAccess.Dynamic;

            _penNameFormat      = DefaultPenNameFormat;
            _colorNameFormat    = DefaultColorNameFormat;
            _brushNameFormat    = DefaultBrushNameFormat;
            _resourceNameFormat = DefaultResourceNameFormat;

            _pens               = new Dictionary<Pen, int>(new PenEqualityComparer());
            _colors             = new Dictionary<Color, int>(new ColorEqualityComparer());
            _brushes            = new Dictionary<SolidColorBrush, int>(new BrushEqualityComparer());
        }

        public bool BindToColors
        {
            get {
                return _bindToColors;
            }
            set {
                _bindToColors = value;
            }
        }

        public bool BindToResources
        {
            get {
                return _bindToResources;
            }
            set {
                _bindToResources = value;
            }
        }

        public bool BindPenToBrushes
        {
            get {
                return _bindPenToBrushes;
            }
            set {
                _bindPenToBrushes = value;
            }
        }

        public string PenNameFormat
        {
            get {
                return _penNameFormat;
            }
            set {
                if (!string.IsNullOrWhiteSpace(value) && value.Length > 4)
                {
                    _penNameFormat = value;
                }
            }
        }

        public string ColorNameFormat
        {
            get {
                return _colorNameFormat;
            }
            set {
                if (!string.IsNullOrWhiteSpace(value) && value.Length > 4)
                {
                    _colorNameFormat = value;
                }
            }
        }

        public string BrushNameFormat
        {
            get {
                return _brushNameFormat;
            }
            set {
                if (!string.IsNullOrWhiteSpace(value) && value.Length > 4)
                {
                    _brushNameFormat = value;
                }
            }
        }

        public string ResourceNameFormat
        {
            get {
                return _resourceNameFormat;
            }
            set {
                _resourceNameFormat = value;
            }
        }

        public bool ResourceFreeze
        {
            get {
                return _resourceFreeze;
            }
            set {
                _resourceFreeze = value;
            }
        }

        public bool UseResourceIndex
        {
            get {
                return _useResourceIndex;
            }
            set {
                _useResourceIndex = value;
            }
        }

        public WpfResourceMode ResourceMode
        {
            get {
                return _resourceMode;
            }
            set {
                _resourceMode = value;
            }
        }

        public WpfResourceAccess ResourceAccess
        {
            get {
                return _resourceAccess;
            }
            set {
                _resourceAccess = value;
            }
        }

        public bool IsInitialized
        {
            get {
                return (_resourceKeys != null && _resourceDictionary != null);
            }
        }

        public bool IsReady
        {
            get {
                if (this.IsInitialized == false)
                {
                    return false;
                }

                return (_resourceKeys.Count != 0 && _resourceKeys.Count == _resourceDictionary.Count);
            }
        }

        public object this[string key]
        {
            get {
                if (this.IsReady == false)
                {
                    return null;
                }
                if (_resourceDictionary.Contains(key))
                {
                    return _resourceDictionary[key];
                }
                return null;
            }
        }

        public IList<string> Keys
        {
            get {
                return _resourceKeys;
            }
        }

        public ICollection<string> ColorKeys
        {
            get {
                if (_colorKeys != null)
                {
                    return _colorKeys.Values;
                }
                return null;
            }
        }

        public ICollection<string> PenKeys
        {
            get {
                if (_penKeys != null)
                {
                    return _penKeys.Values;
                }
                return null;
            }
        }

        public IDictionary<Color, string> ColorPalette
        {
            get {
                return _colorPalette;
            }
            set {
                _colorPalette = value;
            }
        }

        public bool Contains(string resourceKey)
        {
            if (string.IsNullOrWhiteSpace(resourceKey) || _resourceDictionary == null || _resourceDictionary.Count == 0)
            {
                return false;
            }

            return _resourceDictionary.Contains(resourceKey);
        }

        public ICollection<string> BrushKeys
        {
            get {
                if (_brushKeys != null)
                {
                    return _brushKeys.Values;
                }
                return null;
            }
        }

        public static IEqualityComparer<Color> ColorComparer
        {
            get {
                return new ColorEqualityComparer();
            }
        }

        public static IEqualityComparer<SolidColorBrush> BrushComparer
        {
            get {
                return new BrushEqualityComparer();
            }
        }

        public static IEqualityComparer<Pen> PenComparer
        {
            get {
                return new PenEqualityComparer();
            }
        }

        public void AddResource(Brush brush)
        {
            if (_bindToResources == false || IsValid(brush) == false)
            {
                return;
            }
            this.AddResource((SolidColorBrush)brush);
        }

        public void AddResource(SolidColorBrush brush)
        {
            if (_bindToResources == false || brush == null)
            {
                return;
            }
            if (_brushes.ContainsKey(brush))
            {
                _brushes[brush] = _brushes[brush] + 1;
            }
            else
            {
                _brushes[brush] = 0;
            }
        }

        public void AddResource(Color color)
        {
            if (_bindToResources == false)
            {
                return;
            }
            if (_colors.ContainsKey(color))
            {
                _colors[color] = _colors[color] + 1;
            }
            else
            {
                _colors[color] = 0;
            }
        }

        public void AddResource(Pen pen)
        {
            if (_bindToResources == false || IsValid(pen) == false)
            {
                return;
            }
            if (_pens.ContainsKey(pen))
            {
                _pens[pen] = _pens[pen] + 1;
            }
            else
            {
                _pens[pen] = 0;
            }
        }

        public bool HasResource(Pen pen)
        {
            if (pen == null || _pens == null || _pens.Count == 0)
            {
                return false;
            }
            return _pens.ContainsKey(pen);
        }

        public bool HasResource(Color color)
        {
            if (_colorPalette != null && _colorPalette.Count != 0)
            {
                if (_colorPalette.ContainsKey(color))
                {
                    return true;
                }
            }
            if (_colors == null || _colors.Count == 0)
            {
                return false;
            }
            return _colors.ContainsKey(color);
        }

        public static bool TryParseColor(string colorText, out Color color)
        {
            color = Colors.Transparent;
            try
            {
                var colorVal = ColorConverter.ConvertFromString(colorText);
                if (colorVal != null && colorVal is Color)
                {
                    color = (Color)colorVal;
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool HasResource(string colorText, out Color color)
        {
            if (TryParseColor(colorText, out color) == false)
            {
                return false;
            }
            if (_colorPalette != null && _colorPalette.Count != 0)
            {
                if (_colorPalette.ContainsKey(color))
                {
                    return true;
                }
            }
            
            if (_colors == null || _colors.Count == 0)
            {
                return false;
            }
            return _colors.ContainsKey(color);
        }

        public bool HasResource(Brush brush)
        {
            if (brush == null || _brushes == null || _brushes.Count == 0)
            {
                return false;
            }
            if (IsValid(brush) == false)
            {
                return false;
            }
            return _brushes.ContainsKey((SolidColorBrush)brush);
        }

        public bool HasResource(SolidColorBrush brush)
        {
            if (brush == null || _brushes == null || _brushes.Count == 0)
            {
                return false;
            }
            return _brushes.ContainsKey(brush);
        }

        public string GetResourceKey(Pen pen)
        {
            if (this.HasResource(pen) == false || this.IsReady == false)
            {
                return null;
            }
            return _penKeys[pen];
        }

        public string GetResourceKey(Color color)
        {
            if (_colorPalette != null && _colorPalette.Count != 0)
            {
                if (_colorPalette.ContainsKey(color))
                {
                    return _colorPalette[color];
                }
            }

            if (this.HasResource(color) == false || this.IsReady == false)
            {
                return null;
            }
            return _colorKeys[color];
        }

        public string GetResourceKey(Brush brush)
        {
            if (IsValid(brush) == false)
            {
                return null;
            }
            var solidBrush = (SolidColorBrush)brush;

            if (this.HasResource(solidBrush) == false || this.IsReady == false)
            {
                return null;
            }
            return _brushKeys[solidBrush];
        }

        public string GetResourceKey(SolidColorBrush brush)
        {
            if (brush == null)
            {
                return null;
            }
            if (this.HasResource(brush) == false || this.IsReady == false)
            {
                return null;
            }
            return _brushKeys[brush];
        }

        public void InitialiseKeys()
        {
            if (this.IsReady)
            {
                return;
            }

            _penKeys   = new Dictionary<Pen, string>(new PenEqualityComparer());
            _colorKeys = new Dictionary<Color, string>(new ColorEqualityComparer());
            _brushKeys = new Dictionary<SolidColorBrush, string>(new BrushEqualityComparer());

            _resourceKeys = new List<string>();
            _resourceDictionary = new ResourceDictionary();

            if (_bindToResources == false)
            {
                _bindToColors = false;
                _bindPenToBrushes = false;
                return;
            }

            if (_bindPenToBrushes && (_pens != null && _pens.Count != 0))
            {
                foreach (var pen in _pens.Keys)
                {
                    this.AddResource((SolidColorBrush)pen.Brush);
                }
            }
            if (_bindToColors && (_brushes != null && _brushes.Count != 0))
            {
                foreach (var brush in _brushes.Keys)
                {
                    this.AddResource(brush.Color);
                }
            }

            if (_colors != null && _colors.Count != 0)
            {
                int itemCount = _useResourceIndex ? 0 : 1;
                foreach (var color in _colors.Keys)
                {
                    string colorKey = string.Format(_colorNameFormat, itemCount);
                    _colorKeys.Add(color, colorKey);
                    _resourceDictionary[colorKey] = color;
                    _resourceKeys.Add(colorKey);
                    itemCount++;
                }
            }

            if (_colorPalette != null && _colorPalette.Count != 0)
            {
                foreach (var color in _colorPalette.Keys)
                {
                    string colorKey = _colorPalette[color];
                    if (_colorKeys.ContainsKey(color))
                    {
                        var curColorKey = _colorKeys[color];
                        _resourceKeys.Remove(curColorKey);
                        _resourceDictionary.Remove(curColorKey);
                    }
                    _colorKeys[color] = colorKey;
                    _resourceDictionary[colorKey] = color;
                    _resourceKeys.Add(colorKey);
                }
            }

            if (_brushes != null && _brushes.Count != 0)
            {
                int itemCount = _useResourceIndex ? 0 : 1;
                foreach (var brush in _brushes.Keys)
                {
                    string brushKey = string.Format(_brushNameFormat, itemCount);
                    _brushKeys.Add(brush, brushKey);
                    _resourceDictionary[brushKey] = brush;
                    _resourceKeys.Add(brushKey);
                    itemCount++;
                }
            }
            if (_pens != null && _pens.Count != 0)
            {
                int itemCount = _useResourceIndex ? 0 : 1;
                foreach (var pen in _pens.Keys)
                {
                    string penKey = string.Format(_penNameFormat, itemCount);
                    _penKeys.Add(pen, penKey);
                    _resourceDictionary[penKey] = pen;
                    _resourceKeys.Add(penKey);
                    itemCount++;
                }
            }
        }

        internal static bool IsValid(Pen pen)
        {
            return (pen != null && pen.Brush is SolidColorBrush); 
        }

        internal static bool IsValid(Brush brush)
        {
            return (brush != null && brush is SolidColorBrush); 
        }

        internal static bool Equals(SolidColorBrush x, SolidColorBrush y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return Color.Equals(x.Color, y.Color)
                && WpfSvgPaint.Equals(x.Opacity, y.Opacity)
                && WpfSvgPaint.Equals(x.Transform, y.Transform);
        }

        internal static int GetHashCode(SolidColorBrush obj)
        {
            if (obj is null)
            {
                return 13;
            }
            if (obj.Transform != null)
            {
                return 13 ^ obj.Color.GetHashCode() ^ obj.Opacity.GetHashCode() ^ obj.Transform.GetHashCode();
            }

            return 13 ^ obj.Color.GetHashCode() ^ obj.Opacity.GetHashCode();
        }

        private sealed class BrushEqualityComparer : IEqualityComparer<SolidColorBrush>
        {
            public bool Equals(SolidColorBrush x, SolidColorBrush y)
            {
                return WpfDrawingResources.Equals(x, y);
            }

            public int GetHashCode(SolidColorBrush obj)
            {
                return WpfDrawingResources.GetHashCode(obj);
            }
        }

        private sealed class ColorEqualityComparer : IEqualityComparer<Color>
        {
            public bool Equals(Color x, Color y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(Color obj)
            {
                return obj.GetHashCode();
            }
        }

        private sealed class PenEqualityComparer : IEqualityComparer<Pen>
        {
            public bool Equals(Pen x, Pen y)
            {
                if (WpfDrawingResources.Equals((SolidColorBrush)x.Brush, (SolidColorBrush)y.Brush) == false)
                {
                    return false;
                }
                if (WpfSvgPaint.Equals(x.Thickness, y.Thickness) == false ||
                    WpfSvgPaint.Equals(x.MiterLimit, y.MiterLimit) == false)
                {
                    return false;
                }
                if (x.StartLineCap != y.StartLineCap || x.EndLineCap != y.EndLineCap
                    || x.DashCap != y.DashCap || x.LineJoin != y.LineJoin || x.DashStyle != y.DashStyle)
                {
                    return false;
                }
                return true;
            }

            public int GetHashCode(Pen obj)
            {
                if (obj is null)
                {
                    return 17;
                }

                int hashCode = WpfDrawingResources.GetHashCode((SolidColorBrush)obj.Brush);

                return 17 ^ hashCode 
                    ^ obj.Thickness.GetHashCode() 
                    ^ obj.MiterLimit.GetHashCode()
                    ^ obj.StartLineCap.GetHashCode()
                    ^ obj.EndLineCap.GetHashCode()
                    ^ obj.DashCap.GetHashCode()
                    ^ obj.LineJoin.GetHashCode()
                    ^ obj.DashStyle.GetHashCode();
            }
        }
    }
}
