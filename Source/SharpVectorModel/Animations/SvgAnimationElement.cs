using System;
using System.Xml;

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
    public abstract class SvgAnimationElement : SvgElement, ISvgAnimationElement
    {
        #region Protected Fields

        protected SvgTests _svgTests;
        protected EventTarget _eventTarget;
        protected SvgExternalResourcesRequired _externalResourcesRequired;

        private SvgUriReference _uriReference;

        #endregion

        #region Constructors and Destructors

        protected SvgAnimationElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _svgTests                  = new SvgTests(this);
            _eventTarget               = new EventTarget(this);
            _uriReference              = new SvgUriReference(this);
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
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
        /// This will always return <see cref="SvgRenderingHint.Animation"/>
        /// </value>
        public override SvgRenderingHint RenderingHint
        {
            get {
                return SvgRenderingHint.Animation;
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

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get {
                return _externalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion

        #region IEventTarget Members

        void IEventTarget.AddEventListener(string type, EventListener listener, bool useCapture)
        {
            _eventTarget.AddEventListener(type, listener, useCapture);
        }

        void IEventTarget.RemoveEventListener(string type, EventListener listener, bool useCapture)
        {
            _eventTarget.RemoveEventListener(type, listener, useCapture);
        }

        bool IEventTarget.DispatchEvent(IEvent eventObject)
        {
            return _eventTarget.DispatchEvent(eventObject);
        }

        void IEventTarget.AddEventListenerNs(string namespaceUri, string type, EventListener listener,
            bool useCapture, object eventGroup)
        {
            _eventTarget.AddEventListenerNs(namespaceUri, type, listener, useCapture, eventGroup);
        }

        void IEventTarget.RemoveEventListenerNs(string namespaceUri, string type,
            EventListener listener, bool useCapture)
        {
            _eventTarget.RemoveEventListenerNs(namespaceUri, type, listener, useCapture);
        }

        bool IEventTarget.WillTriggerNs(string namespaceUri, string type)
        {
            return _eventTarget.WillTriggerNs(namespaceUri, type);
        }

        bool IEventTarget.HasEventListenerNs(string namespaceUri, string type)
        {
            return _eventTarget.HasEventListenerNs(namespaceUri, type);
        }

        #endregion

        #region ISvgAnimationElement Members

        public ISvgElement TargetElement
        {
            get {
                throw new NotImplementedException();
            }
        }

        public float CurrentTime
        {
            get {
                throw new NotImplementedException();
            }
        }

        public float SimpleDuration
        {
            get {
                throw new NotImplementedException();
            }
        }

        public float StartTime
        {
            get {
                throw new NotImplementedException();
            }
        }

        public string Begin
        {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string Duration
        {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string End
        {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string Maximum
        {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string Minimum
        {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string RepeatCount
        {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string RepeatDuration
        {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string Restart
        {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string Fill
        {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ISvgUriReference Members

        public ISvgAnimatedString Href
        {
            get {
                return _uriReference.Href;
            }
        }

        public XmlElement ReferencedElement
        {
            get {
                return _uriReference.ReferencedNode as XmlElement;
            }
        }

        #endregion

        #region IElementTimeControl Members

        public void BeginElement()
        {
            throw new NotImplementedException();
        }

        public void BeginElementAt(float offset)
        {
            throw new NotImplementedException();
        }

        public void EndElement()
        {
            throw new NotImplementedException();
        }

        public void EndElementAt(float offset)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
