using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SvgFEPointLightElement : SvgElement, ISvgFEPointLightElement
    {
        #region Private Fields

        private ISvgAnimatedNumber _x;
        private ISvgAnimatedNumber _y;
        private ISvgAnimatedNumber _z;

        #endregion

        #region Constructors

        public SvgFEPointLightElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFEPointLightElement Members

        /// <summary>
        /// X location for the light source in the coordinate system established by attribute 'primitiveUnits' on the 'filter' element.
        /// </summary>
        /// <value>
        /// <para>x = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of <c>0</c> were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'x' on the given 'fePointLight' element.
        /// </remarks>
        public ISvgAnimatedNumber X
        {
            get {
                if (_x == null)
                {
                    _x = new SvgAnimatedNumber(this.GetAttribute("x", 0.0));
                }
                return _x;
            }
        }

        /// <summary>
        /// Y location for the light source in the coordinate system established by attribute 'primitiveUnits' on the 'filter' element.
        /// </summary>
        /// <value>
        /// <para>y = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of <c>0</c> were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'y' on the given 'fePointLight' element.
        /// </remarks>
        public ISvgAnimatedNumber Y
        {
            get {
                if (_y == null)
                {
                    _y = new SvgAnimatedNumber(this.GetAttribute("y", 0.0));
                }
                return _y;
            }
        }

        /// <summary>
        /// Z location for the light source in the coordinate system established by attribute 'primitiveUnits' on the 'filter' element, 
        /// assuming that, in the initial coordinate system, the positive Z-axis comes out towards the person viewing the content 
        /// and assuming that one unit along the Z-axis equals one unit in X and Y.
        /// </summary>
        /// <value>
        /// <para>z = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of <c>0</c> were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'z' on the given 'fePointLight' element. 
        /// </remarks>
        public ISvgAnimatedNumber Z
        {
            get {
                if (_z == null)
                {
                    _z = new SvgAnimatedNumber(this.GetAttribute("z", 0.0));
                }
                return _z;
            }
        }

        #endregion
    }
}
