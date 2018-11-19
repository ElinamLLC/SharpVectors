using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgAnimateSetElement : SvgAnimationElement, ISvgAnimateSetElement
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructors

        public SvgAnimateSetElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgAnimateSetElement Members

        public string AttributeName
        {
            get {
                return this.GetAttribute("attributeName");
            }
            set {
                this.SetAttribute("attributeName", value);
            }
        }

        public string AttributeType
        {
            get {
                return this.GetAttribute("attributeType");
            }
            set {
                this.SetAttribute("attributeType", value);
            }
        }

        public string To
        {
            get {
                return this.GetAttribute("to");
            }
            set {
                this.SetAttribute("to", value);
            }
        }

        #endregion
    }
}
