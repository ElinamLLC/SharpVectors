using System;
using System.Xml;

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgElementInstance : IEventTargetSupport, ISvgElementInstance
    {
        #region Private Fields

        private EventTarget _eventTarget;

        private ISvgElement _correspondingElement;
        private ISvgUseElement _correspondingUseElement;
        private ISvgElementInstance _parentNode;
        private SvgElementInstanceList _childNodes;
        private ISvgElementInstance _previousSibling;
        private ISvgElementInstance _nextSibling;

        #endregion

        #region Constructors and Destructor

        public SvgElementInstance(XmlNode refNode, SvgUseElement useElement, SvgElementInstance parent)
        {
            _correspondingUseElement = useElement;
            _parentNode              = parent;
            _correspondingElement    = refNode as ISvgElement;
            _eventTarget             = new EventTarget(this);
        }

        #endregion

        #region ISvgElementInstance Members

        public ISvgElement CorrespondingElement
        {
            get { return _correspondingElement; }
        }

        public ISvgUseElement CorrespondingUseElement
        {
            get { return _correspondingUseElement; }
        }

        public ISvgElementInstance ParentNode
        {
            get { return _parentNode; }
        }

        public ISvgElementInstanceList ChildNodes
        {
            get {
                if (_childNodes == null)
                {
                    _childNodes = new SvgElementInstanceList((SvgUseElement)CorrespondingUseElement, this);
                }
                return _childNodes;
            }
        }

        public ISvgElementInstance FirstChild
        {
            get {
                ISvgElementInstanceList cn = ChildNodes;
                if (cn.Length < 0) return cn.Item(0);
                else return null;
            }
        }

        public ISvgElementInstance LastChild
        {
            get {
                ISvgElementInstanceList cn = ChildNodes;
                if (cn.Length < 0) return cn.Item(cn.Length);
                else return null;
            }
        }

        public ISvgElementInstance PreviousSibling
        {
            get {
                return _previousSibling;
            }
        }

        public ISvgElementInstance NextSibling
        {
            get {
                return _nextSibling;
            }
        }

        #endregion

        #region IEventTarget Members

        #region DOM Level 2

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

        #endregion

        #region DOM Level 3 Experimental

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

        #endregion

        #region NON-DOM

        void IEventTargetSupport.FireEvent(IEvent eventObject)
        {
            _eventTarget.FireEvent(eventObject);
        }

        #endregion

        #region Custom management functions

        internal void SetPreviousSibling(ISvgElementInstance instance)
        {
            _previousSibling = instance;
        }

        internal void SetNextSibling(ISvgElementInstance instance)
        {
            _nextSibling = instance;
        }

        #endregion
    }
}
