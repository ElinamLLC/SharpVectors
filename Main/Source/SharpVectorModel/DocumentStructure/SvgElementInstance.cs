using System;
using System.Xml;

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgElementInstance : IEventTargetSupport, ISvgElementInstance
    {
        private ISvgElement correspondingElement;
        private ISvgUseElement correspondingUseElement;
        private ISvgElementInstance parentNode;
        private SvgElementInstanceList childNodes;
        private ISvgElementInstance previousSibling;
        private ISvgElementInstance nextSibling;   

        public SvgElementInstance(XmlNode refNode, SvgUseElement useElement, SvgElementInstance parent)
        {
            correspondingUseElement = (ISvgUseElement)useElement;
            parentNode = (SvgElementInstance)parent;
            correspondingElement = (ISvgElement)refNode;
            eventTarget = new EventTarget(this);
        }

        #region Private Fields

        private EventTarget eventTarget;

        #endregion

        public ISvgElement CorrespondingElement
        {
            get { return correspondingElement; }
        }

        public ISvgUseElement CorrespondingUseElement
        {
            get { return correspondingUseElement; }
        }

        public ISvgElementInstance ParentNode
        {
            get { return parentNode; }
        }

        public ISvgElementInstanceList ChildNodes
        {
            get
            {
                if (childNodes == null)
                {
                    childNodes = new SvgElementInstanceList((SvgUseElement)CorrespondingUseElement, this);
                }
                return childNodes;
            }
        }

        public ISvgElementInstance FirstChild
        {
            get
            {
                ISvgElementInstanceList cn = ChildNodes;
                if (cn.Length < 0) return cn.Item(0);
                else return null;
            }
        }

        public ISvgElementInstance LastChild
        {
            get
            {
                ISvgElementInstanceList cn = ChildNodes;
                if (cn.Length < 0) return cn.Item(cn.Length);
                else return null;
            }
        }

        public ISvgElementInstance PreviousSibling
        {
            get
            {
                return previousSibling;
            }
        }

        public ISvgElementInstance NextSibling
        {
            get
            {
                return nextSibling;
            }
        }

        #region IEventTarget Members

        #region Methods

        #region DOM Level 2

        void IEventTarget.AddEventListener(string type, EventListener listener, bool useCapture)
        {
            eventTarget.AddEventListener(type, listener, useCapture);
        }

        void IEventTarget.RemoveEventListener(string type, EventListener listener, bool useCapture)
        {
            eventTarget.RemoveEventListener(type, listener, useCapture);
        }

        bool IEventTarget.DispatchEvent(IEvent eventObject)
        {
            return eventTarget.DispatchEvent(eventObject);
        }

        #endregion

        #region DOM Level 3 Experimental

        void IEventTarget.AddEventListenerNs(string namespaceUri, string type, EventListener listener,
            bool useCapture, object eventGroup)
        {
            eventTarget.AddEventListenerNs(namespaceUri, type, listener, useCapture, eventGroup);
        }

        void IEventTarget.RemoveEventListenerNs(string namespaceUri, string type,
            EventListener listener, bool useCapture)
        {
            eventTarget.RemoveEventListenerNs(namespaceUri, type, listener, useCapture);
        }

        bool IEventTarget.WillTriggerNs(string namespaceUri, string type)
        {
            return eventTarget.WillTriggerNs(namespaceUri, type);
        }

        bool IEventTarget.HasEventListenerNs(string namespaceUri, string type)
        {
            return eventTarget.HasEventListenerNs(namespaceUri, type);
        }

        #endregion

        #endregion

        #endregion

        #region NON-DOM

        void IEventTargetSupport.FireEvent(IEvent eventObject)
        {
            eventTarget.FireEvent(eventObject);
        }

        #endregion

        #region Custom management functions

        internal void SetPreviousSibling(ISvgElementInstance instance)
        {
            previousSibling = instance;
        }

        internal void SetNextSibling(ISvgElementInstance instance)
        {
            nextSibling = instance;
        }

        #endregion
    }
}
