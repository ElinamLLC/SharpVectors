using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgFEDiffuseLightingElement : SvgFilterPrimitiveStandardAttributes, ISvgFEDiffuseLightingElement
    {
        #region Private Fields

        private ISvgAnimatedString _in1;

        private ISvgAnimatedNumber _surfaceScale;
        private ISvgAnimatedNumber _diffuseConstant;

        private ISvgAnimatedNumber _kernelUnitLengthX;
        private ISvgAnimatedNumber _kernelUnitLengthY;

        #endregion

        #region Constructors

        public SvgFEDiffuseLightingElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFEDiffuseLightingElement Members

        public ISvgAnimatedString In
        {
            get {
                return this.In1;
            }
        }

        /// <summary>
        /// Corresponds to attribute <c>in</c> on the given <c>feDiffuseLighting</c> element. 
        /// </summary>
        /// <value>
        /// <para> in = "SourceGraphic | SourceAlpha | BackgroundImage | BackgroundAlpha | 
        ///             FillPaint | StrokePaint | filter-primitive-reference"</para>
        /// </value>
        /// <remarks>
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
        /// Corresponds to attribute 'surfaceScale' on the given 'feDiffuseLighting' element. 
        /// </summary>
        /// <value>
        /// <para>surfaceScale = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of 1 were specified.
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedNumber SurfaceScale
        {
            get {
                if (_surfaceScale == null)
                {
                    _surfaceScale = new SvgAnimatedNumber(this.GetAttribute("surfaceScale", 1.0));
                }
                return _surfaceScale;
            }
        }

        /// <summary>
        /// Corresponds to attribute 'diffuseConstant' on the given 'feDiffuseLighting' element. 
        /// </summary>
        /// <value>
        /// <para>diffuseConstant = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of 1 were specified.
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedNumber DiffuseConstant
        {
            get {
                if (_diffuseConstant == null)
                {
                    _diffuseConstant = new SvgAnimatedNumber(this.GetAttribute("diffuseConstant", 1.0));
                }
                return _diffuseConstant;
            }
        }

        /// <summary>
        /// Corresponds to attribute 'kernelUnitLength' on the given 'feDiffuseLighting' element. 
        /// </summary>
        /// <value>
        /// <para></para>
        /// </value>
        /// <remarks>
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
        /// Corresponds to attribute 'kernelUnitLength' on the given 'feDiffuseLighting' element. 
        /// </summary>
        /// <value>
        /// <para></para>
        /// </value>
        /// <remarks>
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
