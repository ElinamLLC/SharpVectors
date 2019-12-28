using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgFEDistantLightElement : SvgElement, ISvgFEDistantLightElement
    {
        #region Private Fields

        private ISvgAnimatedNumber _azimuth;
        private ISvgAnimatedNumber _elevation;

        #endregion

        #region Constructors

        public SvgFEDistantLightElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFEDistantLightElement Members

        /// <summary>
        /// Direction angle for the light source on the XY plane (clockwise), in degrees from the x axis.
        /// </summary>
        /// <value>
        /// Corresponds to attribute 'azimuth' on the given 'feDistantLight' element. 
        /// <para>azimuth = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of <c>0</c> were specified.
        /// </value>
        public ISvgAnimatedNumber Azimuth
        {
            get {
                if (_azimuth == null)
                {
                    _azimuth = new SvgAnimatedNumber(this.GetAttribute("azimuth", 0.0));
                }
                return _azimuth;
            }
        }

        /// <summary>
        /// Direction angle for the light source from the XY plane towards the z axis, in degrees. 
        /// Note the positive Z-axis points towards the viewer of the content.
        /// </summary>
        /// <value>
        /// Corresponds to attribute 'elevation' on the given 'feDistantLight' element.
        /// <para>elevation = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of <c>0</c> were specified.
        /// </value>
        public ISvgAnimatedNumber Elevation
        {
            get {
                if (_elevation == null)
                {
                    _elevation = new SvgAnimatedNumber(this.GetAttribute("elevation", 0.0));
                }
                return _elevation;
            }
        }

        #endregion
    }
}
