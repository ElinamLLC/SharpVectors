using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpVectors.Renderers
{
    public sealed class SvgImageSource : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private bool _isDisposed = false; // To detect redundant calls

        #endregion

        #region Constructors and Destructor

        public SvgImageSource()
        {
        }

        ~SvgImageSource()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool IsDisposed
        {
            get {
                return _isDisposed;
            }
        }

        #endregion

        #region IDisposable Members

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
            }

            _isDisposed = true;
        }

        #endregion
    }
}
