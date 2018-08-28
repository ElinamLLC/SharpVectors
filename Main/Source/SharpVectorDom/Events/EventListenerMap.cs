using System;

namespace SharpVectors.Dom.Events
{
    /// <summary>
    /// Summary description for EventListenerMap.
    /// </summary>
    public class EventListenerMap
    {
        #region Private Fields

        private const int GrowthBuffer = 8;
        private const int GrowthFactor = 2;

        private int _count;
        private bool _locked;
        private EventListenerMapEntry[] _entries;

        #endregion

        #region Constructors

        public EventListenerMap()
        {
        }

        #endregion

        #region Private Helpers

        private EventListenerMapEntry[] GrowBy(int growth)
        {
            if (_entries == null)
            {
                _entries = new EventListenerMapEntry[growth * GrowthFactor + GrowthBuffer];

                this._count = 0;
                this._locked = false;

                return _entries;
            }

            int newCount = _count + growth;

            if (newCount > _entries.Length)
            {
                newCount = newCount * GrowthFactor + GrowthBuffer;

                EventListenerMapEntry[] newEntries = new EventListenerMapEntry[newCount];

                Array.Copy(_entries, 0, newEntries, 0, _entries.Length);

                _entries = newEntries;
            }

            return _entries;
        }

        #endregion

        #region Public Methods

        public void AddEventListener(string namespaceUri, string eventType, object eventGroup, EventListener listener)
        {
            EventListenerMapEntry[] entries = GrowBy(1);

            for (int i = 0; i < _count; i++)
            {
                if (namespaceUri != entries[i].NamespaceUri)
                {
                    continue;
                }

                if (eventType != entries[i].Type)
                {
                    continue;
                }

                if (listener == entries[i].Listener)
                {
                    return;
                }
            }

            entries[_count] = new EventListenerMapEntry(namespaceUri, eventType, eventGroup, listener, _locked);

            _count++;
        }

        public void RemoveEventListener(string namespaceUri, string eventType, EventListener listener)
        {
            if (_entries == null)
            {
                return;
            }

            for (int i = 0; i < _count; i++)
            {
                if (namespaceUri != _entries[i].NamespaceUri)
                {
                    continue;
                }
                if (eventType != _entries[i].Type)
                {
                    continue;
                }

                if (listener == _entries[i].Listener)
                {
                    _count--;
                    _entries[i]      = _entries[_count];
                    _entries[_count] = new EventListenerMapEntry();

                    return;
                }
            }
        }

        public void FireEvent(IEvent eventArg)
        {
            string namespaceUri = eventArg.NamespaceUri;
            string eventType    = eventArg.Type;
            for (int i = 0; i < _count; i++)
            {
                // Check if the entry was added during this phase
                if (_entries[i].Locked)
                    continue;

                string entryNamespaceUri = _entries[i].NamespaceUri;
                string entryEventType    = _entries[i].Type;

                if (entryNamespaceUri != null && namespaceUri != null)
                {
                    if (entryNamespaceUri != namespaceUri)
                    {
                        continue;
                    }
                }
                if (entryEventType != eventType)
                {
                    continue;
                }

                _entries[i].Listener(eventArg);
            }
        }

        public bool HasEventListenerNs(string namespaceUri, string eventType)
        {
            if (_entries == null)
            {
                return false;
            }

            for (int i = 0; i < _count; i++)
            {
                if (namespaceUri != _entries[i].NamespaceUri)
                {
                    continue;
                }
                if (eventType != _entries[i].Type)
                {
                    continue;
                }
                return true;
            }
            return false;
        }

        public void Lock()
        {
            _locked = true;
        }

        public void Unlock()
        {
            // Unlock the map
            _locked = false;

            // Unlock pending entries
            for (int i = 0; i < _count; i++)
            {
                _entries[i].Locked = false;
            }
        }

        #endregion
    }
}
