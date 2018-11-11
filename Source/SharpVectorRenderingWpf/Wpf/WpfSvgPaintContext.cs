using System;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfSvgPaintContext
    {
        private Guid _id;
        private Guid _targetId;

        private WpfSvgPaint _fill;
        private WpfSvgPaint _stroke;

        private object _tag;

        public WpfSvgPaintContext(Guid id)
        {
            _id = id;
            _targetId = Guid.Empty;
        }

        public Guid Id
        {
            get {
                return _id;
            }
        }

        public Guid TargetId
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
                return (_targetId.Equals(Guid.Empty) == false);
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
