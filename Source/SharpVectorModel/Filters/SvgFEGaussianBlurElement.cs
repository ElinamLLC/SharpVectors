using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This filter primitive performs a Gaussian blur on the input image.
    /// </summary>
    public sealed class SvgFEGaussianBlurElement : SvgFilterPrimitiveStandardAttributes, ISvgFEGaussianBlurElement
    {
        #region Private Fields

        private ISvgAnimatedString _in1;

        private ISvgAnimatedNumber _stdDeviationX;
        private ISvgAnimatedNumber _stdDeviationY;

        #endregion

        #region Constructors

        public SvgFEGaussianBlurElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFEGaussianBlurElement Members

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
        /// <para></para>
        /// </value>
        /// <remarks>
        /// <para>
        /// The standard deviation for the blur operation. If two <c>number</c>s are provided, the first number represents 
        /// a standard deviation value along the x-axis of the coordinate system established by attribute 'primitiveUnits' 
        /// on the 'filter' element. The second value represents a standard deviation in Y. If one number is provided, 
        /// then that value is used for both X and Y.
        /// </para>
        /// <para>
        /// A negative value is an error(see Error processing). A value of zero disables the effect of the given filter 
        /// primitive(i.e., the result is the filter input image). 
        /// </para>
        /// <para>
        /// If 'stdDeviation' is 0 in only one of X or Y, then the effect is that the blur is only applied in the direction 
        /// that has a non-zero value.
        /// </para>
        /// </remarks>
        public ISvgAnimatedNumber StdDeviationX
        {
            get {
                if (_stdDeviationX == null || _stdDeviationY == null)
                {
                    this.ParseStdDeviation();
                }
                return _stdDeviationX;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// <para></para>
        /// </value>
        /// <remarks>
        /// <para>
        /// The standard deviation for the blur operation. If two <c>number</c>s are provided, the first number represents 
        /// a standard deviation value along the x-axis of the coordinate system established by attribute 'primitiveUnits' 
        /// on the 'filter' element. The second value represents a standard deviation in Y. If one number is provided, 
        /// then that value is used for both X and Y.
        /// </para>
        /// <para>
        /// A negative value is an error(see Error processing). A value of zero disables the effect of the given filter 
        /// primitive(i.e., the result is the filter input image). 
        /// </para>
        /// <para>
        /// If 'stdDeviation' is 0 in only one of X or Y, then the effect is that the blur is only applied in the direction 
        /// that has a non-zero value.
        /// </para>
        /// </remarks>
        public ISvgAnimatedNumber StdDeviationY
        {
            get {
                if (_stdDeviationX == null || _stdDeviationY == null)
                {
                    this.ParseStdDeviation();
                }
                return _stdDeviationY;
            }
        }

        public void SetStdDeviation(float stdDeviationX, float stdDeviationY)
        {
            _stdDeviationX = new SvgAnimatedNumber(stdDeviationX);
            _stdDeviationY = new SvgAnimatedNumber(stdDeviationY);
        }

        #endregion

        #region Private Methods

        private void ParseStdDeviation()
        {
            if (!this.HasAttribute("stdDeviation"))
            {
                _stdDeviationX = new SvgAnimatedNumber(0.0);
                _stdDeviationY = new SvgAnimatedNumber(0.0);
            }
            var stdDeviation = this.GetAttribute("stdDeviation");
            if (string.IsNullOrWhiteSpace(stdDeviation))
            {
                _stdDeviationX = new SvgAnimatedNumber(0.0);
                _stdDeviationY = new SvgAnimatedNumber(0.0);
            }

            var stdDeviationParts = _reSeparators.Split(stdDeviation);
            if (stdDeviationParts == null || stdDeviationParts.Length == 0)
            {
                _stdDeviationX = new SvgAnimatedNumber(0.0);
                _stdDeviationY = new SvgAnimatedNumber(0.0);
            }
            if (stdDeviationParts.Length == 1)
            {
                var stdDeviationValue = SvgNumber.ParseNumber(stdDeviationParts[0]);
                _stdDeviationX = new SvgAnimatedNumber(stdDeviationValue);
                _stdDeviationY = new SvgAnimatedNumber(stdDeviationValue);
            }
            else if (stdDeviationParts.Length == 2)
            {
                _stdDeviationX = new SvgAnimatedNumber(SvgNumber.ParseNumber(stdDeviationParts[0]));
                _stdDeviationY = new SvgAnimatedNumber(SvgNumber.ParseNumber(stdDeviationParts[1]));
            }
        }

        #endregion
    }
}
