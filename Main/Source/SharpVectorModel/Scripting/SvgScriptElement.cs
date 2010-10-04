using System;
using System.Text;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SVGScriptElement interface corresponds to the 'script' element.
    /// </summary>
    public sealed class SvgScriptElement : SvgElement, ISvgScriptElement
    {
        private SvgUriReference svgURIReference;
        private SvgExternalResourcesRequired svgExternalResourcesRequired;

        #region Constructors

        public SvgScriptElement(string prefix, string localname, string ns, SvgDocument doc) 
            : base(prefix, localname, ns, doc)
        {
            svgURIReference = new SvgUriReference(this);
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
        }

        #endregion

        public string Type
        {
            get 
            { 
                return GetAttribute("type"); 
            }
            set 
            { 
                SetAttribute("type", value); 
            }
        }

        #region ISvgURIReference Members

        public ISvgAnimatedString Href
        {
            get
            {
                return svgURIReference.Href;
            }
        }

        #endregion

        #region Implementation of ISvgExternalResourcesRequired

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get
            {
                return svgExternalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion
    }
}
