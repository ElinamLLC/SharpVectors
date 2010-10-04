using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Arguments when namespace is trying to be resolved
    /// </summary>
    public sealed class SvgResolveNamespaceEventArgs : EventArgs
    {
        private string _uri;
        private string _prefix;

        public SvgResolveNamespaceEventArgs()
        {   
        }

        public SvgResolveNamespaceEventArgs(string prefix)
        {
            _prefix = prefix;
        }

        /// <summary>
        /// Gets or sets the prefix (for example: 'rdf')
        /// </summary>
        /// <value>The prefix.</value>
        public string Prefix 
        { 
            get
            {
                return _prefix;
            }
            set
            {
                _prefix = value;
            }
        }
        /// <summary>
        /// Gets or sets the URI (for example: 'http://www.w3.org/1999/02/22-rdf-syntax-ns#').
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
