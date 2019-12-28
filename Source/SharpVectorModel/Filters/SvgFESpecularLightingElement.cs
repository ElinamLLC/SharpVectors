using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This filter primitive lights a source graphic using the alpha channel as a bump map. The resulting image is an 
    /// RGBA image based on the light color. The lighting calculation follows the standard specular component of the 
    /// Phong lighting model. The resulting image depends on the light color, light position and surface geometry of 
    /// the input bump map. The result of the lighting calculation is added. The filter primitive assumes that the 
    /// viewer is at infinity in the z direction (i.e., the unit vector in the eye direction is (0,0,1) everywhere).
    /// </summary>
    public sealed class SvgFESpecularLightingElement : SvgFilterPrimitiveStandardAttributes, ISvgFESpecularLightingElement
    {
        #region Private Fields

        private ISvgAnimatedString _in1;

        private ISvgAnimatedNumber _specularConstant;
        private ISvgAnimatedNumber _specularExponent;
        private ISvgAnimatedNumber _surfaceScale;

        private ISvgAnimatedNumber _kernelUnitLengthX;
        private ISvgAnimatedNumber _kernelUnitLengthY;

        #endregion

        #region Constructors

        public SvgFESpecularLightingElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFESpecularLightingElement Members

        public ISvgAnimatedString In
        {
            get {
                return this.In1;
            }
        }

        /// <summary>
        /// Corresponds to attribute <c>in</c> on the given <c>feBlend</c> element. 
        /// </summary>
        /// <value>
        /// <para> in = "SourceGraphic | SourceAlpha | BackgroundImage | BackgroundAlpha | 
        ///             FillPaint | StrokePaint | filter-primitive-reference"</para>
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'in' on the given 'feSpecularLighting' element. 
        /// </remarks>
        public ISvgAnimatedString In1
        {
            get {
                if (_in1 == null)
                {
                    _in1 = new SvgAnimatedString(this.GetAttribute("in", string.Empty));
                }
                return _in1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// <para>surfaceScale = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of 1 were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'surfaceScale' on the given 'feSpecularLighting' element. 
        /// </remarks>
        public ISvgAnimatedNumber SurfaceScale
        {
            get {
                if (_surfaceScale == null)
                {
                    _surfaceScale = new SvgAnimatedNumber(this.GetAttribute("surfaceScale", 0.0));
                }
                return _surfaceScale;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// <para>specularConstant = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of 1 were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'specularConstant' on the given 'feSpecularLighting' element. 
        /// </remarks>
        public ISvgAnimatedNumber SpecularConstant
        {
            get {
                if (_specularConstant == null)
                {
                    _specularConstant = new SvgAnimatedNumber(this.GetAttribute("specularConstant", 0.0));
                }
                return _specularConstant;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// <para>specularExponent = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of 1 were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'specularExponent' on the given 'feSpecularLighting' element. 
        /// </remarks>
        public ISvgAnimatedNumber SpecularExponent
        {
            get {
                if (_specularExponent == null)
                {
                    _specularExponent = new SvgAnimatedNumber(this.GetAttribute("specularExponent", 0.0));
                }
                return _specularExponent;
            }
        }

        /// <summary>
        /// 
        /// Corresponds to attribute 'kernelUnitLength' on the given 'feSpecularLighting' element.
        /// </summary>
        /// <value>
        /// <para></para>
        /// </value>
        /// <remarks>
        /// The first number is the <c>dx</c> value. The second number is the <c>dy</c> value. If the <c>dy</c> value is not specified, 
        /// it defaults to the same value as <c>dx</c>. Indicates the intended distance in current filter units (i.e., units as 
        /// determined by the value of attribute 'primitiveUnits') for dx and dy, respectively, in the surface normal calculation 
        /// formulas. By specifying value(s) for 'kernelUnitLength', the kernel becomes defined in a scalable, abstract coordinate 
        /// system. If 'kernelUnitLength' is not specified, the dx and dy values should represent very small deltas relative to a 
        /// given (x,y) position, which might be implemented in some cases as one pixel in the intermediate image offscreen bitmap, 
        /// which is a pixel-based coordinate system, and thus potentially not scalable. For some level of consistency across 
        /// display media and user agents, it is necessary that a value be provided for at least one of 'filterRes' and 
        /// 'kernelUnitLength'. 
        /// </remarks>
        public ISvgAnimatedNumber KernelUnitLengthX
        {
            get {
                if (_kernelUnitLengthX == null || _kernelUnitLengthY == null)
                {
                    this.ParseKernelUnitLength();
                }
                return _kernelUnitLengthX;
            }
        }

        /// <summary>
        /// 
        /// Corresponds to attribute 'kernelUnitLength' on the given 'feSpecularLighting' element. 
        /// </summary>
        /// <value>
        /// <para></para>
        /// </value>
        /// <remarks>
        /// The first number is the <c>dx</c> value. The second number is the <c>dy</c> value. If the <c>dy</c> value is not specified, 
        /// it defaults to the same value as <c>dx</c>. Indicates the intended distance in current filter units (i.e., units as 
        /// determined by the value of attribute 'primitiveUnits') for dx and dy, respectively, in the surface normal calculation 
        /// formulas. By specifying value(s) for 'kernelUnitLength', the kernel becomes defined in a scalable, abstract coordinate 
        /// system. If 'kernelUnitLength' is not specified, the dx and dy values should represent very small deltas relative to a 
        /// given (x,y) position, which might be implemented in some cases as one pixel in the intermediate image offscreen bitmap, 
        /// which is a pixel-based coordinate system, and thus potentially not scalable. For some level of consistency across 
        /// display media and user agents, it is necessary that a value be provided for at least one of 'filterRes' and 
        /// 'kernelUnitLength'. 
        /// </remarks>
        public ISvgAnimatedNumber KernelUnitLengthY
        {
            get {
                if (_kernelUnitLengthX == null || _kernelUnitLengthY == null)
                {
                    this.ParseKernelUnitLength();
                }
                return _kernelUnitLengthY;
            }
        }

        #endregion

        #region Private Methods

        private void ParseKernelUnitLength()
        {
            if (!this.HasAttribute("kernelUnitLength"))
            {
                return;
            }
            var kernelUnitLength = this.GetAttribute("kernelUnitLength");
            if (string.IsNullOrWhiteSpace(kernelUnitLength))
            {
                return;
            }

            var kernelUnitLengthParts = _reSeparators.Split(kernelUnitLength);
            if (kernelUnitLengthParts == null || kernelUnitLengthParts.Length == 0)
            {
                return;
            }
            if (kernelUnitLengthParts.Length == 1)
            {
                var kernelUnitLengthValue = SvgNumber.ParseNumber(kernelUnitLengthParts[0]);
                _kernelUnitLengthX = new SvgAnimatedNumber(kernelUnitLengthValue);
                _kernelUnitLengthY = new SvgAnimatedNumber(kernelUnitLengthValue);
            }
            else if (kernelUnitLengthParts.Length == 2)
            {
                _kernelUnitLengthX = new SvgAnimatedNumber(SvgNumber.ParseNumber(kernelUnitLengthParts[0]));
                _kernelUnitLengthY = new SvgAnimatedNumber(SvgNumber.ParseNumber(kernelUnitLengthParts[1]));
            }
        }

        #endregion
    }
}
