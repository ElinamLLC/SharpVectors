using System.Threading;
using System;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;

namespace SharpVectors.Converters
{
    /// <summary>
    /// ConsoleWorker is a helper class for running asynchronous tasks. 
    /// </summary>
    public sealed class ConsoleWorker
    {
        #region Private Fields

        private int _count;
        private int _maxCount;
        private bool _cancelationPending;
        private object _countProtector;

        private AsyncCallback _workerCallback;

        private DoWorkEventHandler _eventHandler;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleWorker"/> class.
        /// </summary>
        public ConsoleWorker()
            : this(1)
        {   
        }

        public ConsoleWorker(int maximumCount)
        {
            _countProtector = new Object();

            _maxCount       = maximumCount;
            _workerCallback = new AsyncCallback(this.OnRunWorkerCompleted);
            _eventHandler   = new DoWorkEventHandler(this.OnDoWork);
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when [do work].
        /// </summary>
        public event DoWorkEventHandler DoWork;
        /// <summary>
        /// Occurs when [run worker completed].
        /// </summary>
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;
        /// <summary>
        /// Occurs when [progress changed].
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this instance is busy.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        public bool IsBusy
        {
            get
            {
                lock (_countProtector)
                {
                    if (_count >= _maxCount)
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether [cancellation pending].
        /// </summary>
        /// <value><c>true</c> if [cancellation pending]; otherwise, <c>false</c>.</value>
        public bool CancellationPending
        {
            get
            {
                return _cancelationPending;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs the worker async.
        /// </summary>
        /// <param name="abortIfBusy">if set to <c>true</c> [abort if busy].</param>
        public bool RunWorkerAsync(bool abortIfBusy)
        {
            return this.RunWorkerAsync(abortIfBusy, null);
        }      

        /// <summary>
        /// Runs the worker async.
        /// </summary>
        /// <param name="abortIfBusy">if set to <c>true</c> [abort if busy].</param>
        /// <param name="argument">The argument.</param>
        public bool RunWorkerAsync(object argument)
        {
            if (this.IsBusy)
            {
                return false;
            }
            _count++;

            DoWorkEventArgs args = new DoWorkEventArgs(argument);
            _eventHandler.BeginInvoke(this, args, _workerCallback, args);

            return true;
        }

        public bool RunWorkerAsync()
        {
            if (this.IsBusy)
            {
                return false;
            }
            _count++;

            DoWorkEventArgs args = new DoWorkEventArgs(null);
            _eventHandler.BeginInvoke(this, args, _workerCallback, args);

            return true;
        }

        public bool RunWorkerAsync(bool abortIfBusy, object argument)
        {
            if (abortIfBusy && this.IsBusy)
            {
                return false;
            }
            _count++;

            DoWorkEventArgs args = new DoWorkEventArgs(argument);
            _eventHandler.BeginInvoke(this, args, _workerCallback, args);

            return true;
        }

        /// <summary>
        /// Cancels the async.
        /// </summary>
        public void CancelAsync()
        {
            _cancelationPending = true;
        }

        /// <summary>
        /// Reports the progress.
        /// </summary>
        /// <param name="percentProgress">The percent progress.</param>
        public void ReportProgress(int percentProgress)
        {
            this.OnProgressChanged(new ProgressChangedEventArgs(
                percentProgress, null));
        }

        /// <summary>
        /// Reports the progress.
        /// </summary>
        /// <param name="percentProgress">The percent progress.</param>
        /// <param name="userState">State of the user.</param>
        public void ReportProgress(int percentProgress, object userState)
        {
            this.OnProgressChanged(new ProgressChangedEventArgs(
                percentProgress, userState));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Called when [do work].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }
            if (this.DoWork != null)
            {
                this.DoWork(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ProgressChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void OnProgressChanged(ProgressChangedEventArgs e)
        {
            if (this.ProgressChanged != null)
            {
                this.ProgressChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [run worker completed].
        /// </summary>
        /// <param name="ar">The ar.</param>
        private void OnRunWorkerCompleted(IAsyncResult ar)
        {
            DoWorkEventArgs args = (DoWorkEventArgs)ar.AsyncState;

            try
            {
                DoWorkEventHandler doWorkDelegate =
                    (DoWorkEventHandler)((AsyncResult)ar).AsyncDelegate;

                doWorkDelegate.EndInvoke(ar);

                if (this.RunWorkerCompleted != null)
                {
                    this.RunWorkerCompleted(this,
                        new RunWorkerCompletedEventArgs(args.Result,
                            null, args.Cancel));
                }
            }
            catch (Exception ex)
            {
                if (this.RunWorkerCompleted != null)
                {
                    this.RunWorkerCompleted(this,
                        new RunWorkerCompletedEventArgs(args.Result,
                            ex, args.Cancel));
                }               	
            }

            _count--;
        }

        #endregion
    }
}
