using System;
using System.Diagnostics;

namespace WpfTestSvgControl
{
    interface ITraceTextSink
    {
        void Fail(string msg);
        void Event(string msg, TraceEventType eventType);
    }

    class TraceTextSource : TraceListener
    {
        public ITraceTextSink Sink { get; private set; }
        private bool _fail;
        private TraceEventType _eventType = TraceEventType.Information;

        public TraceTextSource(ITraceTextSink sink)
        {
            Debug.Assert(sink != null);
            Sink = sink;
        }

        public override void Fail(string message)
        {
            _fail = true;
            base.Fail(message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            _eventType = eventType;
            base.TraceEvent(eventCache, source, eventType, id, message);
        }

        public override void Write(string message)
        {
            if (IndentLevel > 0)
                message = message.PadLeft(IndentLevel + message.Length, '\t');

            if (_fail)
                Sink.Fail(message);

            else
                Sink.Event(message, _eventType);

            _fail = false;
            _eventType = TraceEventType.Information;
        }

        public override void WriteLine(string message)
        {
            Write(message + "\n");
        }
    }
}
