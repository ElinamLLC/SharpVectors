using System;

using System.Windows.Media;

namespace SharpVectors.Renderers.Wpf
{
    using SharpVectors.Runtime;

    public sealed class WpfHitPath : IEquatable<WpfHitPath>
    {
        public const string RootValue = "(Root)";
        public const string Separator = "/";
        public static readonly char[] Splitter = Separator.ToCharArray();

        private string _value;
        private WpfHitPath _parent;
        private WpfHitPath[] _children;

        public WpfHitPath()
            : this(null, null)
        {
        }

        public WpfHitPath(string value)
            : this(null, value)
        {
        }

        public WpfHitPath(WpfHitPath parent, string value)
        {
            _parent = parent;
            _value  = value ?? string.Empty;
        }

        public bool IsRoot
        {
            get {
                return (_parent == null);
            }
        }

        public WpfHitPath Root
        {
            get {
                var path = this;

                while (path._parent != null)
                {
                    path = path.Parent;
                }
                return path;
            }
        }

        public bool HasChild
        {
            get {
                return (_children != null && _children.Length != 0);
            }
        }

        public WpfHitPath FirstChild
        {
            get {
                if (_children != null && _children.Length != 0)
                {
                    return _children[0];
                }
                return null;
            }
        }

        public WpfHitPath LastChild
        {
            get {
                if (_children != null && _children.Length != 0)
                {
                    return _children[_children.Length - 1];
                }
                return null;
            }
        }

        public string Path
        {
            get {
                if (_parent == null)
                {
                    return string.Empty;
                }
                return string.Format($"{_parent.Path}{Separator}{_value}");
            }
        }

        public string[] SplitPath
        {
            get {
                var path = this.Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    return new string[0];
                }
                var paths = path.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);
                return paths;
            }
        }

        public string Value
        {
            get {
                return _value;
            }
            set {
                if (value ==  null)
                {
                    _value = string.Empty;
                }
                else
                {
                    _value = value;
                }
            }
        }

        public WpfHitPath Parent
        {
            get {
                return _parent;
            }
            set {
                _parent = value;
            }
        }

        public WpfHitPath[] Children
        {
            get {
                return _children;
            }
        }

        public WpfHitPath AddChild(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (_parent == null)
                {
                    return this;
                }
                return _parent;
            }
            return this.AddChild(new WpfHitPath(value));
        }

        public WpfHitPath AddChild(WpfHitPath child)
        {
            if (child == null)
            {
                if (_parent == null)
                {
                    return this;
                }
                return _parent;
            }
            child._parent = this;

            if (_children == null)
            {
                _children = new WpfHitPath[] { child };
            }
            else
            {
                Array.Resize(ref _children, _children.Length + 1);
                _children[_children.Length - 1] = child;
            }

            return child;
        }

        public override string ToString()
        {
            if (_parent == null)
            {
                return RootValue;
            }
            return string.Format($"Value={_value}, Parent={_parent.Value}, Path={this.Path}");
        }

        public override int GetHashCode()
        {
            var value = this.Value ?? string.Empty;
            return value.GetHashCode();
        }

        public static bool operator ==(WpfHitPath path1, WpfHitPath path2)
        {
            return Equals(path1, path2);
        }

        public static bool operator !=(WpfHitPath path1, WpfHitPath path2)
        {
            return !Equals(path1, path2);
        }

        public override bool Equals(object obj)
        {
            if (obj is WpfHitPath)
            {
                return this.Equals((WpfHitPath)obj);
            }
            return false;
        }

        public bool Equals(WpfHitPath other)
        {
            if (other is null)
            {
                return false;
            }

            if (!object.ReferenceEquals(_parent, other._parent))
            {
                return false;
            }
            if (!string.Equals(_value, other._value))
            {
                return false;
            }

            return EqualArrays(_children, other._children);
        }

        public static bool Equals(WpfHitPath path1, WpfHitPath path2)
        {
            if (object.ReferenceEquals(path1, path2))
                return true;
            if (path1 is null || path2 is null)
                return false;

            return path1.Equals(path2);
        }

        public Transform GetTransform(WpfDrawingDocument document, Drawing hitDrawing)
        {
            var displayTransform = (Transform)document.DisplayTransform.Inverse;

            if (_parent == null)
            {
                return displayTransform;
            }
            var splitPaths = this.SplitPath;
            if (splitPaths == null || splitPaths.Length == 0)
            {
                return displayTransform;
            }
            foreach (var splitPath in splitPaths)
            {
                System.Diagnostics.Trace.WriteLine(splitPath);
            }

            var uniqueId = SvgObject.GetUniqueId(hitDrawing);

            TransformGroup transforms = new TransformGroup();
            transforms.Children.Add(displayTransform);

            var drawingLayer = document.GetDrawingLayer();
            if (drawingLayer != null && drawingLayer.Transform != null)
            {
                var transform = drawingLayer.Transform;
                if (!transform.Value.IsIdentity)
                {
                    transforms.Children.Add(transform);
                }
            }

            for (int i = 0; i < splitPaths.Length; i++)
            {
                var splitPath = splitPaths[i];
                if (!string.IsNullOrWhiteSpace(uniqueId) && 
                    string.Equals(splitPath, uniqueId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                var drawing = document.GetByUniqueId(splitPath) as DrawingGroup;
                if (drawing != null && drawing.Transform != null)
                {
                    if (drawing.Transform != drawingLayer.Transform)
                    {
                        transforms.Children.Add(drawing.Transform);
                    }
                }
            }

            return transforms;
        }

        private static bool EqualArrays(WpfHitPath[] paths1, WpfHitPath[] paths2)
        {
            if (object.ReferenceEquals(paths1, paths2))
                return true;
            if (paths1 == null || paths2 == null)
                return false;
            if (paths1.Length != paths2.Length)
                return false;
            for (var i = 0; i < paths1.Length; i++)
            {
                if (!paths1[i].Equals(paths2[i]))
                    return false;
            }
            return true;
        }
    }
}
