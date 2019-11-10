using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// An SVG object represents an 'solidcolor' element in the DOM.
    /// </summary>
    public sealed class SvgSolidColorElement : SvgStyleableElement, ISvgSolidColorElement
    {
        #region Private Fields

        private SvgUriReference _uriReference;
        private SvgExternalResourcesRequired _externalResourcesRequired;
        private SvgTests _svgTests;

        #endregion

        #region Constructors and Destructor

        public SvgSolidColorElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uriReference              = new SvgUriReference(this);
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
            _svgTests                  = new SvgTests(this);
        }

        #endregion

        #region Public Properties

        public XmlNodeList Children
        {
            get {
                XmlNodeList children = SelectNodes("svg:*", OwnerDocument.NamespaceManager);
                if (children.Count > 0)
                {
                    return children;
                }
                // check any eventually referenced gradient
                if (ReferencedElement == null)
                {
                    // return an empty list
                    return children;
                }
                return ReferencedElement.Children;
            }
        }

        #endregion

        #region ISvgElement Members

        /// <summary>
        /// Gets a value indicating whether this SVG element is renderable.
        /// </summary>
        /// <value>
        /// This is <see langword="'true"/> if the element is renderable; otherwise,
        /// it is <see langword="false"/>.
        /// </value>
        public override bool IsRenderable
        {
            get {
                return false;
            }
        }

        /// <summary>
        /// Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        /// An enumeration of the <see cref="SvgRenderingHint"/> specifying the rendering hint.
        /// This will always return <see cref="SvgRenderingHint.Containment"/>
        /// </value>
        public override SvgRenderingHint RenderingHint
        {
            get {
                return SvgRenderingHint.Containment;
            }
        }

        #endregion

        #region ISvgSolidColorElement Members


        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href
        {
            get {
                return _uriReference.Href;
            }
        }

        public SvgSolidColorElement ReferencedElement
        {
            get {
                return _uriReference.ReferencedNode as SvgSolidColorElement;
            }
        }

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get {
                return _externalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion

        #region ISvgTests Members

        public ISvgStringList RequiredFeatures
        {
            get { return _svgTests.RequiredFeatures; }
        }

        public ISvgStringList RequiredExtensions
        {
            get { return _svgTests.RequiredExtensions; }
        }

        public ISvgStringList SystemLanguage
        {
            get { return _svgTests.SystemLanguage; }
        }

        public bool HasExtension(string extension)
        {
            return _svgTests.HasExtension(extension);
        }

        #endregion
    }
}
