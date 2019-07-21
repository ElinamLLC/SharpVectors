using System;

namespace SharpVectors.Renderers.Wpf
{
    public abstract class WpfVisitor : WpfRendererObject
    {
        protected bool _isInitialized;

        protected WpfVisitor()
        {
        }

        protected WpfVisitor(WpfDrawingContext context)
            : base(context)
        {
        }

        public override bool IsInitialized
        {
            get {
                return (_isInitialized && base.IsInitialized);
            }
        }

        public virtual void Initialize(WpfDrawingContext context)
        {
            if (context != null)
            {
                _context = context;
            }
            _isInitialized = (context != null);
        }

        public virtual void Uninitialize()
        {
            _isInitialized = false;
        }
    }

    public sealed class WpfVisitors
    {
        private WpfIDVisitor _idVisitor;
        private WpfLinkVisitor _linkVisitor;
        private WpfClassVisitor _classVisitor;
        private WpfFontFamilyVisitor _fontFamilyVisitor;
        private WpfEmbeddedImageVisitor _imageVisitor;

        public WpfVisitors()
        {
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

        public void Initialize(WpfDrawingContext context)
        {
            if (_idVisitor != null)
            {
                _idVisitor.Initialize(context);
            }
            if (_linkVisitor != null)
            {
                _linkVisitor.Initialize(context);
            }
            if (_classVisitor != null)
            {
                _classVisitor.Initialize(context);
            }
            if (_fontFamilyVisitor != null)
            {
                _fontFamilyVisitor.Initialize(context);
            }
            if (_imageVisitor != null)
            {
                _imageVisitor.Initialize(context);
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
    }

}
