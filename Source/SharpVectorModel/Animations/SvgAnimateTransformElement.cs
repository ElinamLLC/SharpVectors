using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgAnimateTransformElement: SvgAnimationElement, ISvgAnimateTransformElement
    {
        #region Private Fields

        private SvgTransformType _type;

        #endregion

        #region Constructors and Destructors

        public SvgAnimateTransformElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgAnimateTransformElement Members

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

        public string By
        {
            get {
                return this.GetAttribute("by");
            }
            set {
                this.SetAttribute("by", value);
            }
        }

        public string CalcMode
        {
            get {
                return this.GetAttribute("calcMode");
            }
            set {
                this.SetAttribute("calcMode", value);
            }
        }

        public string From
        {
            get {
                return this.GetAttribute("from");
            }
            set {
                this.SetAttribute("from", value);
            }
        }

        public string KeySplines
        {
            get {
                return this.GetAttribute("keySplines");
            }
            set {
                this.SetAttribute("keySplines", value);
            }
        }

        public string KeyTimes
        {
            get {
                return this.GetAttribute("keyTimes");
            }
            set {
                this.SetAttribute("keyTimes", value);
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

        public string Values
        {
            get {
                return this.GetAttribute("values");
            }
            set {
                this.SetAttribute("values", value);
            }
        }

        public string Accumulate
        {
            get {
                return this.GetAttribute(SvgConstants.AttrAccumulate);
            }
            set {
                this.SetAttribute(SvgConstants.AttrAccumulate, value);
            }
        }

        public string Additive
        {
            get {
                return this.GetAttribute("additive");
            }
            set {
                this.SetAttribute(string.Empty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value to indicates the type of transformation which is to have its values change over time. 
        /// </summary>
        /// <value>
        /// <para>
        /// The values are translate | scale | rotate | skewX | skewY.
        /// </para>
        /// <para>
        /// If the attribute is not specified, then the effect is as if a value of 'translate' were specified.
        /// </para>
        /// <para>
        /// The ‘from’, ‘by’ and ‘to’ attributes take a value expressed using the same syntax that is available for the given transformation type:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <description>For a type='translate", each individual value is expressed as {tx} [,{ty}].</description>
        /// </item>
        /// <item>
        /// <description>For a type="scale", each individual value is expressed as {sx} [,{sy}].</description>
        /// </item>
        /// <item>
        /// <description>For a type="rotate", each individual value is expressed as {rotate-angle} [{cx} {cy}].</description>
        /// </item>
        /// <item>
        /// <description>For a type="skewX" and type="skewY", each individual value is expressed as {skew-angle}.</description>
        /// </item>
        /// </list>
        /// </value>
        public SvgTransformType Type
        {
            get {
                return _type;
            }
            set {
                _type = value;
            }
        }

        #endregion
    }
}
