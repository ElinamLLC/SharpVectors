using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SvgFESpotLightElement : SvgElement, ISvgFESpotLightElement
    {
        #region Private Fields

        private ISvgAnimatedNumber _x;
        private ISvgAnimatedNumber _y;
        private ISvgAnimatedNumber _z;

        private ISvgAnimatedNumber _pointsAtX;
        private ISvgAnimatedNumber _pointsAtY;
        private ISvgAnimatedNumber _pointsAtZ;

        private ISvgAnimatedNumber _limitingConeAngle;
        private ISvgAnimatedNumber _specularExponent;

        #endregion

        #region Constructors

        public SvgFESpotLightElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFESpotLightElement Members

        /// <summary>
        /// X location for the light source in the coordinate system established by attribute 'primitiveUnits' on the 'filter' element.
        /// </summary>
        /// <value>
        /// <para>x = "number"</para>
        /// </value>
        /// If the attribute is not specified, then the effect is as if a value of 0 were specified.
        /// <remarks>
        /// Corresponds to attribute 'x' on the given 'feSpotLight' element. 
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
        /// If the attribute is not specified, then the effect is as if a value of 0 were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'y' on the given 'feSpotLight' element.
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
        /// assuming that, in the initial coordinate system, the positive Z-axis comes out towards the person viewing the content and 
        /// assuming that one unit along the Z-axis equals one unit in X and Y.
        /// </summary>
        /// <value>
        /// <para>z = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of 0 were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'z' on the given 'feSpotLight' element. 
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

        /// <summary>
        /// X location in the coordinate system established by attribute 'primitiveUnits' on the 'filter' element of the point 
        /// at which the light source is pointing.
        /// </summary>
        /// <value>
        /// <para>pointsAtX = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of 0 were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'pointsAtX' on the given 'feSpotLight' element.
        /// </remarks>
        public ISvgAnimatedNumber PointsAtX
        {
            get {
                if (_pointsAtX == null)
                {
                    _pointsAtX = new SvgAnimatedNumber(this.GetAttribute("pointsAtX", 0.0));
                }
                return _pointsAtX;
            }
        }

        /// <summary>
        /// Y location in the coordinate system established by attribute 'primitiveUnits' on the 'filter' element of the point 
        /// at which the light source is pointing.
        /// </summary>
        /// <value>
        /// <para>pointsAtY = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of 0 were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'pointsAtY' on the given 'feSpotLight' element. 
        /// </remarks>
        public ISvgAnimatedNumber PointsAtY
        {
            get {
                if (_pointsAtY == null)
                {
                    _pointsAtY = new SvgAnimatedNumber(this.GetAttribute("pointsAtY", 0.0));
                }
                return _pointsAtY;
            }
        }

        /// <summary>
        /// Z location in the coordinate system established by attribute 'primitiveUnits' on the 'filter' element of the point 
        /// at which the light source is pointing, assuming that, in the initial coordinate system, the positive Z-axis comes 
        /// out towards the person viewing the content and assuming that one unit along the Z-axis equals one unit in X and Y. 
        /// </summary>
        /// <value>
        /// <para>pointsAtZ = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of 0 were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'pointsAtZ' on the given 'feSpotLight' element.
        /// </remarks>
        public ISvgAnimatedNumber PointsAtZ
        {
            get {
                if (_pointsAtZ == null)
                {
                    _pointsAtZ = new SvgAnimatedNumber(this.GetAttribute("pointsAtZ", 0.0));
                }
                return _pointsAtZ;
            }
        }

        /// <summary>
        /// Exponent value controlling the focus for the light source.
        /// </summary>
        /// <value>
        /// <para>specularExponent = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of 1 were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'specularExponent' on the given 'feSpotLight' element. 
        /// </remarks>
        public ISvgAnimatedNumber SpecularExponent
        {
            get {
                if (_specularExponent == null)
                {
                    _specularExponent = new SvgAnimatedNumber(this.GetAttribute("specularExponent", 1.0));
                }
                return _specularExponent;
            }
        }

        /// <summary>
        /// A limiting cone which restricts the region where the light is projected. No light is projected outside the cone. 
        /// 'limitingConeAngle' represents the angle in degrees between the spot light axis (i.e. the axis between the light 
        /// source and the point to which it is pointing at) and the spot light cone. User agents should apply a smoothing 
        /// technique such as anti-aliasing at the boundary of the cone.
        /// </summary>
        /// <value>
        /// <para>limitingConeAngle = "number"</para>
        /// If no value is specified, then no limiting cone will be applied.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'limitingConeAngle' on the given 'feSpotLight' element. 
        /// </remarks>
        public ISvgAnimatedNumber LimitingConeAngle
        {
            get {
                if (_limitingConeAngle == null)
                {
                    if (this.HasAttribute("limitingConeAngle"))
                    {
                        _limitingConeAngle = new SvgAnimatedNumber(this.GetAttribute("limitingConeAngle", 0.0));
                    }
                }
                return _limitingConeAngle;
            }
        }

        #endregion
    }
}
