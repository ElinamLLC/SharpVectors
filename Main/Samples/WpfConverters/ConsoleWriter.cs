using System;

namespace SharpVectors.Converters
{
    public sealed class ConsoleWriter
    {
        private bool _isQuiet;
        private object _synchObject;
        private ConsoleWriterVerbosity _verbosity;

        public ConsoleWriter()
            : this(false, ConsoleWriterVerbosity.Normal)
        {   
        }

        public ConsoleWriter(bool isQuiet, ConsoleWriterVerbosity verbosity)
        {
            _isQuiet   = isQuiet;
            _verbosity = verbosity;

            _synchObject = new object();
        }

        public bool IsQuiet
        {
            get
            {
                return _isQuiet;
            }
        }

        public object SynchObject
        {
            get
            {
                return _synchObject;
            }
        }

        public ConsoleWriterVerbosity Verbosity
        {
            get
            {
                return _verbosity;
            }
        }

        public void WriteLine()
        {
            if (_isQuiet)
            {
                return;
            }

            lock (_synchObject)
            {
                Console.WriteLine();
            }
        }

        public void Write(string text)
        {
            if (_isQuiet || text == null)
            {
                return;
            }

            lock (_synchObject)
            {
                Console.Write(text);
            }
        }

        public void WriteProgress(string text)
        {
            if (_isQuiet || text == null)
            {
                return;
            }

            lock (_synchObject)
            {
                //Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(text);
                Console.Write("\b");
                //Console.ResetColor();
            }
        }

        public void WriteLine(string text)
        {
            if (_isQuiet || text == null)
            {
                return;
            }

            lock (_synchObject)
            {
                Console.WriteLine(text);
            }
        }

        public void WriteInfoLine(string text)
        {   
            if (_isQuiet || String.IsNullOrEmpty(text))
            {
                return;
            }

            lock (_synchObject)
            {
                Console.WriteLine("Info: " + text);
            }
        }

        public void WriteWarnLine(string text)
        {
            if (_isQuiet || String.IsNullOrEmpty(text))
            {
                return;
            }

            lock (_synchObject)
            {
                Console.WriteLine("Warn: " + text);
            }
        }

        public void WriteErrorLine(string text)
        {
            if (_isQuiet || String.IsNullOrEmpty(text))
            {
                return;
            }

            lock (_synchObject)
            {
                Console.WriteLine("Error: " + text);
            }
        }
    }
}
