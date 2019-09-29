using System;
using System.IO;
using System.Xml;
using System.Diagnostics;

using SharpVectors.Net;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The ISvgScriptElement interface corresponds to the 'script' element. 
    /// </summary>
    /// <remarks>
    /// <para>A 'script' element is equivalent to the 'script' element in HTML and thus is the place for scripts.</para>
    /// <para>Any functions defined within any 'script' element have a 'global' scope across the entire current document.</para>
    /// </remarks>
    public sealed class SvgScriptElement : SvgElement, ISvgScriptElement
    {
        #region Public Static Fields

        public const string ECMAScript = "application/ecmascript";

        #endregion

        #region Private Fields

        private string _content;
        private SvgUriReference _uriReference;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors

        public SvgScriptElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _content                   = string.Empty;
            _uriReference              = new SvgUriReference(this);
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
        }

        #endregion

        #region ISvgScriptElement Members

        /// <summary>
        /// Gets or sets a value corresponding to the attribute 'type' on the given 'script' element.
        /// </summary>
        /// <value>A string specifying the script type.</value>
        public string Type
        {
            get {
                string mimeType = null;
                if (this.HasAttribute("type"))
                {
                    mimeType = this.GetAttribute("type");
                }
                else
                {
                    mimeType = this.OwnerDocument.RootElement.GetAttribute("type");
                }
                if (string.IsNullOrWhiteSpace(mimeType) || mimeType == "text/ecmascript" 
                    || mimeType == "text/javascript" || mimeType == "application/javascript")
                {
                    mimeType = ECMAScript;
                }
                return mimeType;
            }
            set {
                this.SetAttribute("type", value);
            }
        }

        /// <summary>
        /// Gets or sets a value corresponding to the attribute 'crossorigin' on the given 'script' element.
        /// </summary>
        /// <value>An enumeration specifying the CORS (Cross-Origin Resource Sharing) setting attributes. 
        /// Possible values are <c>anonymous</c>, <c>use-credentials</c> and empty string.</value>
        /// <remarks>This is introduced in <c>SVG 2</c>.</remarks>
        /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/CORS_settings_attributes"/>
        public string CrossOrigin
        {
            get {
                return this.GetAttribute("crossorigin");
            }
            set {
                this.SetAttribute("crossorigin", value);
            }
        }

        public string Content
        {
            get {
                if (string.IsNullOrWhiteSpace(_content))
                {
                    // We will only support the ECMAScript or JavaScript...
                    var mimeType = this.Type;
                    if (!string.Equals(mimeType, ECMAScript))
                    {
                        return string.Empty;
                    }

                    if (_uriReference == null || _uriReference.IsEmpty)
                    {
                        if (this.HasChildNodes)
                        {
                            foreach (XmlNode xmlNode in this.ChildNodes)
                            {
                                if (xmlNode.NodeType == XmlNodeType.Text || xmlNode.NodeType == XmlNodeType.CDATA)
                                {
                                    _content = xmlNode.InnerText;
                                    break;
                                }                            
                            }
                        }
                    }
                    else
                    {
                        Uri scriptUri = new Uri(_uriReference.AbsoluteUri);
                        if (scriptUri.IsFile)
                        {
                            var localPath = scriptUri.LocalPath;
                            if (File.Exists(localPath))
                            {
                                _content = File.ReadAllText(localPath);
                            }
                        }
                        else
                        {
                            try
                            {
                                var creator = new ExtendedHttpWebRequestCreator();
                                var request = creator.Create(scriptUri);
                                var response = request.GetResponse();
                                using (Stream stream = response.GetResponseStream())
                                {
                                    StreamReader reader = new StreamReader(stream);
                                    _content = reader.ReadToEnd();
                                }
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteLine(ex.Message, "SvgScriptElement");
                            }
                        }
                    }
                }

                return _content;
            }
        }

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href
        {
            get {
                return _uriReference.Href;
            }
        }

        public SvgUriReference UriReference
        {
            get {
                return _uriReference;
            }
        }

        #endregion

        #region Implementation of ISvgExternalResourcesRequired

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get {
                return _externalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion
    }
}
