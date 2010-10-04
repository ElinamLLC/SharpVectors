using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Arguments when URI is trying to be resolved while loading schema
    /// </summary>
    public sealed class SvgResolveUriEventArgs : EventArgs
    {
        private string _uri;

        public SvgResolveUriEventArgs()
        {   
        }

        public SvgResolveUriEventArgs(string uri)
        {   
        }

        /// <summary>
        /// Gets or sets the URI (for example: 'http://www.w3.org/2000/svg').
        /// This value may have already been initialized, it's up to the application to check if it wants to override the resolution
        /// </summary>
        /// <value>The URI.</value>
        public string Uri 
        { 
            get
            {
                return _uri;
            }
            set
            {
                _uri = value;
            }
        }
    }
}
