using System;

namespace SharpVectors.Dom.Events
{
    /// <summary>
    /// Summary description for EventListenerMapEntry.
    /// </summary>
    public class EventListenerMapEntry
    {
        private bool _locked;
        private string _namespaceUri;
        private string _type;
        private object _group;
        private EventListener _listener;

        public EventListenerMapEntry()
        {   
        }

        public EventListenerMapEntry(string namespaceUri, string type, object group,
            EventListener listener, bool locked)
        {
            _namespaceUri = namespaceUri;
            _type         = type;
            _group        = group;
            _listener     = listener;
            _locked       = locked;
        }

        public bool Locked
        {
            get {
                return _locked;
            }
            set {
                _locked = value;
            }
        }

        public string NamespaceUri
        {
            get {
                return _namespaceUri;
            }
            set {
                _namespaceUri = value;
            }
        }

        public string Type
        {
            get {
                return _type;
            }
            set {
                _type = value;
            }
        }

        public object Group
        {
            get {
                return _group;
            }
            set {
                _group = value;
            }
        }

        public EventListener Listener
        {
            get {
                return _listener;
            }
            set {
                _listener = value;
            }
        }
    }
}
