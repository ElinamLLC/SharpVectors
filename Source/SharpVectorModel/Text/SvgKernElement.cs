using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgKernElement is the base interface corresponds to the 'vkern' and 'hkern' elements. 
    /// </summary>
    public abstract class SvgKernElement : SvgElement
    {
        #region Constructors and Destructor

        protected SvgKernElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region Public Properties (ISvgVKernElement and ISvgHKernElement)

        public abstract bool IsHorizontal
        {
            get;
        }

        // attribute name = "g1" <string>
        public string Glyph1
        {
            get { return this.GetAttribute("g1"); }
            set { this.SetAttribute("g1", value); }
        }

        // attribute name = "g2" <string>
        public string Glyph2
        {
            get { return this.GetAttribute("g2"); }
            set { this.SetAttribute("g2", value); }
        }

        // attribute name = "u1" <string>
        public string Unicode1
        {
            get { return this.GetAttribute("u1"); }
            set { this.SetAttribute("u1", value); }
        }

        // attribute name = "u2" <string>
        public string Unicode2
        {
            get { return this.GetAttribute("u2"); }
            set { this.SetAttribute("u2", value); }
        }

        // attribute name = "k" <number>
        public float Kerning
        {
            get { return this.GetAttribute("k", 0.0f); }
            set { this.SetAttribute("k", value); }
        }

        #endregion
    }
}
