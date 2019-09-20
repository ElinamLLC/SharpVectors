using System;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfSvgPaintContext
    {
        private string _id;
        private string _targetId;

        private WpfSvgPaint _fill;
        private WpfSvgPaint _stroke;

        private object _tag;

        public WpfSvgPaintContext(string id)
        {
            _id       = id;
            _targetId = string.Empty;
        }

        public string Id
        {
            get {
                return _id;
            }
        }

        public string TargetId
        {
            get {
                return _targetId;
            }
            set {
                _targetId = value;
            }
        }

        public bool HasTarget
        {
            get {
                return (string.IsNullOrWhiteSpace(_targetId) == false);
            }
        }

        public WpfSvgPaint Fill
        {
            get {
                return _fill;
            }
            set {
                _fill = value;
            }
        }

        public WpfSvgPaint Stroke
        {
            get {
                return _stroke;
            }
            set {
                _stroke = value;
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
    }
}
