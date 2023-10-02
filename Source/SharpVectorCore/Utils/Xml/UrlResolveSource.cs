using System.Xml;

namespace SharpVectors.Xml
{
    public enum UrlResolveSource
    {
        None = 0,
        Entity = 1,
        Element = 2,
        Document = 3,
        Font = 4,
        Image = 5,
        Style = 6,
        Script = 7
    }

    public struct UrlResolveArgs
    {
        private DtdProcessing _processing;
        private UrlResolveTypes _entity;
        private UrlResolveTypes _element;
        private UrlResolveTypes _document;
        private UrlResolveTypes _font;
        private UrlResolveTypes _image;
        private UrlResolveTypes _style;
        private UrlResolveTypes _script;

        public UrlResolveArgs(DtdProcessing processing)
        {
            _processing = processing;
            _entity = UrlResolveTypes.Resource;
            _element = UrlResolveTypes.Local | UrlResolveTypes.Remote;
            _document = UrlResolveTypes.Local | UrlResolveTypes.Remote;
            _font = UrlResolveTypes.Local | UrlResolveTypes.Remote;
            _image = UrlResolveTypes.Local | UrlResolveTypes.Remote;
            _style = UrlResolveTypes.Local | UrlResolveTypes.Remote;
            _script = UrlResolveTypes.Local | UrlResolveTypes.Remote;
        }

        public UrlResolveArgs(DtdProcessing processing, UrlResolveTypes entity,
            UrlResolveTypes element, UrlResolveTypes document, UrlResolveTypes font,
            UrlResolveTypes image, UrlResolveTypes style, UrlResolveTypes script)
        {
            _processing = processing;
            _entity = entity;
            _element = element;
            _document = document;
            _font = font;
            _image = image;
            _style = style;
            _script = script;
        }

        public DtdProcessing Processing { get => _processing; set => _processing = value; }
        public UrlResolveTypes Entity { get => _entity; set => _entity = value; }
        public UrlResolveTypes Element { get => _element; set => _element = value; }
        public UrlResolveTypes Document { get => _document; set => _document = value; }
        public UrlResolveTypes Font { get => _font; set => _font = value; }
        public UrlResolveTypes Image { get => _image; set => _image = value; }
        public UrlResolveTypes Style { get => _style; set => _style = value; }
        public UrlResolveTypes Script { get => _script; set => _script = value; }
    }
}
