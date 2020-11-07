using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgAnimateMotionElement : SvgAnimationElement, ISvgAnimateMotionElement
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructors

        public SvgAnimateMotionElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgAnimateMotionElement Members

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

        public string To
        {
            get {
                return this.GetAttribute("to");
            }
            set {
                this.SetAttribute("to", value);
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
        /// Gets or set the attribute specifying the motion path, expressed in the same format and interpreted the same 
        /// way as the "d" attribute on the "path" element. 
        /// </summary>
        /// <value>
        /// The effect of a motion path animation is to add a supplemental transformation matrix onto the CTM for the 
        /// referenced object which causes a translation along the x- and y-axes of the current user coordinate system 
        /// by the computed X and Y values computed over time.
        /// </value>
        public string Path
        {
            get {
                return this.GetAttribute("path");
            }
            set {
                this.SetAttribute("path", value);
            }
        }

        /// <summary>
        /// Gets or sets a value that takes a semicolon-separated list of floating point values between 0 and 1 and indicates 
        /// how far along the motion path the object shall move at the moment in time specified by corresponding "keyTimes" value. 
        /// </summary>
        /// <value>
        /// <para>
        /// Distance calculations use the user agent's distance along the path algorithm. Each progress value in the list 
        /// corresponds to a value in the "keyTimes" attribute list.
        /// </para>
        /// <para>
        /// If a list of "keyPoints" is specified, there must be exactly as many values in the "keyPoints" list 
        /// as in the "keyTimes" list.
        /// </para>
        /// </value>
        public string KeyPoints
        {
            get {
                return this.GetAttribute("keyPoints");
            }
            set {
                this.SetAttribute("keyPoints", value);
            }
        }

        /// <summary>
        /// Gets or sets an attribute post-multiplies a supplemental transformation matrix onto the CTM of the target element 
        /// to apply a rotation transformation about the origin of the current user coordinate system. 
        /// </summary>
        /// <value>
        /// The rotation transformation is applied after the supplemental translation transformation that is computed 
        /// due to the "path" attribute.
        /// <list type="bullet">
        /// <item><term>auto</term>
        /// <description>Indicates that the object is rotated over time by the angle of the direction (i.e., 
        /// directional tangent vector) of the motion path.</description>
        /// </item>
        /// <item><term>auto-reverse</term>
        /// <description>Indicates that the object is rotated over time by the angle of the direction (i.e., 
        /// directional tangent vector) of the motion path plus 180 degrees.</description>
        /// </item>
        /// <item><term>number</term>
        /// <description>Indicates that the target element has a constant rotation transformation applied to it, 
        /// where the rotation angle is the specified number of degrees.</description>
        /// </item>
        /// </list>
        /// The default value is '0'.
        /// </value>
        public string Rotate
        {
            get {
                return this.GetAttribute("rotate");
            }
            set {
                this.SetAttribute("rotate", value);
            }
        }

        /// <summary>
        /// The "origin" attribute is defined in the SMIL Animation specification. It has no effect in SVG.
        /// </summary>
        /// <value>
        /// The value is "default".
        /// </value>
        public string Origin
        {
            get {
                return this.GetAttribute("origin");
            }
            set {
                this.SetAttribute("origin", value);
            }
        }

        #endregion
    }
}
