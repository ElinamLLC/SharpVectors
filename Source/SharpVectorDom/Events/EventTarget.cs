using System;
using System.Collections;
using System.Xml;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Dom.Events
{
    /// <summary>
    /// Summary description for EventTarget.
    /// </summary>
    public class EventTarget : IEventTargetSupport
    {
        #region Private Fields

        private IEventTargetSupport _eventTarget;

        private EventListenerMap _captureMap;
        private EventListenerMap _bubbleMap;
        private ArrayList _ancestors;

        #endregion

        #region Constructors

        public EventTarget(IEventTargetSupport eventTarget)
        {
            _eventTarget = eventTarget;
            _captureMap  = new EventListenerMap();
            _bubbleMap   = new EventListenerMap();
            _ancestors   = new ArrayList();
        }

        #endregion

        #region IEventTarget interface

        #region Methods

        #region DOM Level 2

        public void AddEventListener(string type, EventListener listener,
            bool useCapture)
        {
            if (useCapture)
            {
                _captureMap.AddEventListener(null, type, null, listener);
            }
            else
            {
                _bubbleMap.AddEventListener(null, type, null, listener);
            }
        }

        public void RemoveEventListener(string type, EventListener listener,
            bool useCapture)
        {
            if (useCapture)
            {
                _captureMap.RemoveEventListener(null, type, listener);
            }
            else
            {
                _bubbleMap.RemoveEventListener(null, type, listener);
            }
        }

        public bool DispatchEvent(IEvent eventInfo)
        {
            if (eventInfo == null || string.IsNullOrWhiteSpace(eventInfo.Type))
            {
                throw new EventException(EventExceptionCode.UnspecifiedEventTypeErr);
            }
            try
            {
                // The actual target may be an SvgElement or an SvgElementInstance from 
                // a conceptual tree <see href="http://www.w3.org/TR/SVG/struct.html#UseElement"/>
                XmlNode currNode = null;
                ISvgElementInstance currInstance = null;

                if (_eventTarget is ISvgElementInstance)
                    currInstance = (ISvgElementInstance)_eventTarget;
                else
                    currNode = (XmlNode)_eventTarget;

                // We can't use an XPath ancestor axe because we must account for 
                // conceptual nodes
                _ancestors.Clear();

                // Build the ancestors from the conceptual tree
                if (currInstance != null)
                {
                    while (currInstance.ParentNode != null)
                    {
                        currInstance = currInstance.ParentNode;
                        _ancestors.Add(currInstance);
                    }
                    currNode = (XmlNode)currInstance.CorrespondingUseElement;
                    _ancestors.Add(currNode);
                }

                // Build actual ancestors
                while (currNode != null && currNode.ParentNode != null)
                {
                    currNode = currNode.ParentNode;
                    _ancestors.Add(currNode);
                }

                Event realEvent = (Event)eventInfo;
                realEvent.EventTarget = _eventTarget;

                if (!realEvent.Stopped)
                {
                    realEvent.EventPhase = EventPhase.CapturingPhase;

                    for (int i = _ancestors.Count - 1; i >= 0; i--)
                    {
                        if (realEvent.Stopped)
                        {
                            break;
                        }

                        var ancestor = _ancestors[i] as IEventTarget;

                        if (ancestor != null)
                        {
                            realEvent.CurrentTarget = ancestor;

                            if (ancestor is IEventTargetSupport)
                            {
                                ((IEventTargetSupport)ancestor).FireEvent(realEvent);
                            }
                        }
                    }
                }

                if (!realEvent.Stopped)
                {
                    realEvent.EventPhase = EventPhase.AtTarget;
                    realEvent.CurrentTarget = _eventTarget;
                    _eventTarget.FireEvent(realEvent);
                }

                if (!realEvent.Stopped)
                {
                    realEvent.EventPhase = EventPhase.BubblingPhase;

                    for (int i = 0; i < _ancestors.Count; i++)
                    {
                        if (realEvent.Stopped)
                        {
                            break;
                        }

                        var ancestor = _ancestors[i] as IEventTarget;

                        if (ancestor != null)
                        {
                            realEvent.CurrentTarget = ancestor;
                            ((IEventTargetSupport)ancestor).FireEvent(realEvent);
                        }
                    }
                }

                return realEvent.Stopped;
            }
            catch (InvalidCastException)
            {
                throw new DomException(DomExceptionType.WrongDocumentErr);
            }
        }

        #endregion

        #region DOM Level 3 Experimental

        public void AddEventListenerNs(string namespaceUri, string type,
            EventListener listener, bool useCapture, object evtGroup)
        {
            if (useCapture)
            {
                _captureMap.AddEventListener(namespaceUri, type, evtGroup, listener);
            }
            else
            {
                _bubbleMap.AddEventListener(namespaceUri, type, evtGroup, listener);
            }
        }

        public void RemoveEventListenerNs(string namespaceUri, string type,
            EventListener listener, bool useCapture)
        {
            if (useCapture)
            {
                _captureMap.RemoveEventListener(namespaceUri, type, listener);
            }
            else
            {
                _bubbleMap.RemoveEventListener(namespaceUri, type, listener);
            }
        }

        public bool WillTriggerNs(string namespaceUri, string type)
        {
            XmlNode node = (XmlNode)this._eventTarget;
            XmlNodeList ancestors = node.SelectNodes("ancestor::node()");

            for (int i = 0; i < ancestors.Count; i++)
            {
                IEventTarget ancestor = ancestors[i] as IEventTarget;

                if (ancestor.HasEventListenerNs(namespaceUri, type))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasEventListenerNs(string namespaceUri, string eventType)
        {
            return _captureMap.HasEventListenerNs(namespaceUri, eventType) ||
                _bubbleMap.HasEventListenerNs(namespaceUri, eventType);
        }

        #endregion

        #endregion

        #region NON-DOM

        public void FireEvent(IEvent eventInfo)
        {
            switch (eventInfo.EventPhase)
            {
                case EventPhase.AtTarget:
                case EventPhase.BubblingPhase:
                    _bubbleMap.Lock();
                    _bubbleMap.FireEvent(eventInfo);
                    _bubbleMap.Unlock();
                    break;
                case EventPhase.CapturingPhase:
                    _captureMap.Lock();
                    _captureMap.FireEvent(eventInfo);
                    _captureMap.Unlock();
                    break;
            }
        }

        public event EventListener OnMouseMove
        {
            add
            {
                throw new NotImplementedException();
            }
            remove
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #endregion
    }
}
