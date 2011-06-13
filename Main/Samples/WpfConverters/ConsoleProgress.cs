using System;
using System.Threading;

namespace SharpVectors.Converters
{
    public sealed class ConsoleProgress : IObserver
    {
        #region Private Fields

        private bool   _isQuiet;
        private volatile bool _isStarted;
        private Thread _thread;
        private ConsoleWriter _writer;

        #endregion

        #region Constructors and Destructor

        public ConsoleProgress(bool isQuiet, ConsoleWriter writer)
        {
            _isQuiet = isQuiet;
            _writer  = writer;
        }

        #endregion

        #region Public Properties

        #endregion

        #region Private Methods

        private void ThreadProc()
        {
            int counter    = 0;
            string[] frame = new string[] 
            { 
                "|", "/", "-", "\\", "|", "/", "-", "\\" 
            };

            while (true)
            {
                counter++;

                int index = counter % frame.Length;
                _writer.WriteProgress(frame[index]);

                if (counter >= frame.Length)
                {
                    counter = 0;
                }

                if (!_isStarted || 
                    (_thread != null && _thread.ThreadState == ThreadState.AbortRequested))
                {
                    _writer.Write(" ");
                    break;
                }
            }
        }

        #endregion

        #region IObserver Members

        public void OnStarted(IObservable sender)
        {
            _isStarted = true;
            if (_isQuiet)
            {
                return;
            }

            _thread = new Thread(ThreadProc);
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void OnCompleted(IObservable sender, bool isSuccessful)
        {
            _isStarted = false;

            if (_isQuiet || _thread == null)
            {
                return;
            }
            if (_thread != null && _thread.IsAlive)
            {
                try
                {
                    if (_thread.IsAlive)
                    {
                        _thread.Abort();
                    }
                    if (_thread.IsAlive)
                    {
                        _thread.Join();
                    }
                }
                catch
                {
                }
                _thread = null;
            }
        }

        #endregion
    }
}
