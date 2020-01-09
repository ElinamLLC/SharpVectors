using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgStyleElement interface corresponds to the 'style' element. 
    /// </summary>
    public sealed class SvgStyleElement : SvgElement, ISvgStyleElement
    {
        public SvgStyleElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        public string Type
        {
            get {
                return this.GetAttribute("type");
            }
            set {
                this.SetAttribute("type", value);
            }
        }

        public string Media
        {
            get {
                return this.GetAttribute("media");
            }
            set {
                this.SetAttribute("media", value);
            }
        }

        public string Title
        {
            get {
                return this.GetAttribute("title");
            }
            set {
                this.SetAttribute("title", value);
            }
        }
    }
}
