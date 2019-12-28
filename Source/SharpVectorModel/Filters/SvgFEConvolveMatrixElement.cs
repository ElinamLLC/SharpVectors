using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This applies a matrix convolution filter effect. A convolution combines pixels in the input image 
    /// with neighboring pixels to produce a resulting image. A wide variety of imaging operations can be 
    /// achieved through convolutions, including blurring, edge detection, sharpening, embossing and beveling.
    /// </summary>
    public sealed class SvgFEConvolveMatrixElement : SvgFilterPrimitiveStandardAttributes, ISvgFEConvolveMatrixElement
    {
        #region Private Fields

        private ISvgAnimatedEnumeration _edgeMode;
        private ISvgAnimatedInteger     _orderX;
        private ISvgAnimatedInteger     _orderY;
        private ISvgAnimatedNumberList  _kernelMatrix;
        private ISvgAnimatedNumber      _divisor;
        private ISvgAnimatedNumber      _bias;
        private ISvgAnimatedInteger     _targetX;
        private ISvgAnimatedInteger     _targetY;
        private ISvgAnimatedNumber      _kernelUnitLengthX;
        private ISvgAnimatedNumber      _kernelUnitLengthY;
        private ISvgAnimatedBoolean     _preserveAlpha;

        #endregion

        #region Constructors

        public SvgFEConvolveMatrixElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFEConvolveMatrixElement Members

        /// <summary>
        /// Determines how to extend the input image as necessary with color values so that the matrix operations 
        /// can be applied when the kernel is positioned at or near the edge of the input image.
        /// </summary>
        /// <value>
        /// <para>edgeMode = "duplicate | wrap | none"</para>
        /// An enumeration of the type <see cref="SvgFilterEdgeMode"/>. The default value is <see cref="SvgFilterEdgeMode.Duplicate"/>.
        /// </value>
        /// <remarks>
        /// <para>
        /// "duplicate" indicates that the input image is extended along each of its borders as necessary by duplicating 
        /// the color values at the given edge of the input image.
        /// </para>
        /// <para>
        /// "wrap" indicates that the input image is extended by taking the color values from the opposite edge of the image.
        /// </para>
        /// <para>
        /// "none" indicates that the input image is extended with pixel values of zero for R, G, B and A.
        /// </para>
        /// If attribute 'edgeMode' is not specified, then the effect is as if a value of duplicate were specified. 
        /// </remarks>
        public ISvgAnimatedEnumeration EdgeMode
        {
            get {
                if (_edgeMode == null)
                {
                    SvgFilterEdgeMode edgeMode = SvgFilterEdgeMode.Duplicate;
                    if (this.HasAttribute("edgeMode"))
                    {
                        edgeMode = (SvgFilterEdgeMode)Enum.Parse(typeof(SvgFilterEdgeMode),
                            this.GetAttribute("edgeMode"), true);
                    }
                    _edgeMode = new SvgAnimatedEnumeration((ushort)edgeMode);
                }
                return _edgeMode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// <para>bias = "number"</para>
        /// </value>
        /// <remarks>
        /// <para>
        /// After applying the 'kernelMatrix' to the input image to yield a number and applying the 'divisor', the 
        /// 'bias' attribute is added to each component.
        /// </para>
        /// <para>
        /// One application of 'bias' is when it is desirable to have .5 gray value be the zero response of the filter. 
        /// The bias property shifts the range of the filter. This allows representation of values that would otherwise 
        /// be clamped to 0 or 1.
        /// </para>
        /// If 'bias' is not specified, then the effect is as if a value of 0 were specified.
        /// </remarks>
        public ISvgAnimatedNumber Bias
        {
            get {
                if (_bias == null)
                {
                    _bias = new SvgAnimatedNumber(this.GetAttribute("bias", 0.0));
                }
                return _bias;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// <para>divisor = number</para>
        /// </value>
        /// <remarks>
        /// <para>
        /// After applying the <see cref="KernelMatrix"/> to the input image to yield a number, that number is divided by 'divisor' 
        /// to yield the final destination color value. A divisor that is the sum of all the matrix values tends to have 
        /// an evening effect on the overall color intensity of the result. It is an error to specify a divisor of zero.
        /// </para>
        /// <para>
        /// The default value is the sum of all values in <see cref="KernelMatrix"/>, with the exception that if the sum is zero, 
        /// then the divisor is set to 1.
        /// </para>
        /// </remarks>
        public ISvgAnimatedNumber Divisor
        {
            get {
                if (_divisor == null)
                {
                    if (this.HasAttribute("divisor"))
                    {
                        _divisor = new SvgAnimatedNumber(this.GetAttribute("divisor", 1.0));
                    }
                    else
                    {
                        ISvgAnimatedNumberList numberList = this.KernelMatrix;
                        if (numberList == null || numberList.Count == 0)
                        {
                            _divisor = new SvgAnimatedNumber(this.GetAttribute("divisor", 1.0));
                        }
                        else
                        {
                            double numberSum = 0.0;
                            var numberValues = numberList.AnimVal;
                            for (int i = 0; i < numberValues.Count; i++)
                            {
                                numberSum += numberValues[i].Value;
                            }
                            _divisor = new SvgAnimatedNumber(numberSum);
                        }
                    }
                }
                return _divisor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// <para>kernelMatrix = "list of numbers"</para>
        /// </value>
        /// <remarks>
        /// The list of <c>number</c>s that make up the kernel matrix for the convolution. Values are separated by space 
        /// characters and/or a comma. The number of entries in the list must equal <see cref="OrderX"/> times <see cref="OrderY"/>.
        /// </remarks>
        public ISvgAnimatedNumberList KernelMatrix
        {
            get {
                if (_kernelMatrix == null)
                {
                    _kernelMatrix = SvgAnimatedNumberList.Empty;
                    if (this.HasAttribute("kernelMatrix"))
                    {
                        _kernelMatrix = new SvgAnimatedNumberList(this.GetAttribute("kernelMatrix"));
                    }
                }
                return _kernelMatrix;
            }
        }

        /// <summary>
        /// Corresponds to attribute 'kernelUnitLength' on the given 'feConvolveMatrix' element. 
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
        /// Corresponds to attribute 'kernelUnitLength' on the given 'feConvolveMatrix' element. 
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

        /// <summary>
        /// Indicates the number of cells in each dimension for 'kernelMatrix'. 
        /// </summary>
        /// <value>
        /// <para>order = "number-optional-number"</para>
        /// <para>The values provided must be <c>integer</c>s greater than zero.</para>
        /// <para>The first number, <see cref="OrderX"/>, indicates the number of columns in the matrix. </para>
        /// <para>
        /// The second number, <see cref="OrderY"/>, indicates the number of rows in the matrix. 
        /// If <see cref="OrderY"/>is not provided, it defaults to <see cref="OrderX"/>.
        /// </para>
        /// </value>
        /// <remarks>
        /// <para>
        /// A typical value is order="3". It is recommended that only small values(e.g., 3) be used; 
        /// higher values may result in very high CPU overhead and usually do not produce results that 
        /// justify the impact on performance.
        /// </para>
        /// <para>
        /// If the attribute is not specified, the effect is as if a value of <c>3</c> were specified.
        /// </para>
        /// </remarks>
        public ISvgAnimatedInteger OrderX
        {
            get {
                if (_orderX == null || _orderY == null)
                {
                    this.ParseOrder();
                }
                return _orderX;
            }
        }

        /// <summary>
        /// Indicates the number of cells in each dimension for 'kernelMatrix'. 
        /// </summary>
        /// <value>
        /// <para>order = "number-optional-number"</para>
        /// <para>The values provided must be <c>integer</c>s greater than zero.</para>
        /// <para>The first number, <see cref="OrderX"/>, indicates the number of columns in the matrix. </para>
        /// <para>
        /// The second number, <see cref="OrderY"/>, indicates the number of rows in the matrix. 
        /// If <see cref="OrderY"/>is not provided, it defaults to <see cref="OrderX"/>.
        /// </para>
        /// </value>
        /// <remarks>
        /// <para>
        /// A typical value is order="3". It is recommended that only small values(e.g., 3) be used; 
        /// higher values may result in very high CPU overhead and usually do not produce results that 
        /// justify the impact on performance.
        /// </para>
        /// <para>
        /// If the attribute is not specified, the effect is as if a value of <c>3</c> were specified.
        /// </para>
        /// </remarks>
        public ISvgAnimatedInteger OrderY
        {
            get {
                if (_orderX == null || _orderY == null)
                {
                    this.ParseOrder();
                }
                return _orderY;
            }
        }

        /// <summary>
        /// Determines the positioning in X of the convolution matrix relative to a given target pixel in the input image. 
        /// </summary>
        /// <value>
        /// <para>targetX = "integer"</para>
        /// The leftmost column of the matrix is column number zero. The value must be such that: 
        /// <c><![CDATA[ 0 <= targetX < orderX. ]]></c>
        /// By default, the convolution matrix is centered in X over each pixel of the input image 
        /// (i.e., <c>targetX = floor ( orderX / 2 )</c>).
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedInteger TargetX
        {
            get {
                if (_targetX == null)
                {
                    if (this.HasAttribute("targetX"))
                    {
                        _targetX = new SvgAnimatedInteger(this.GetAttribute("targetX", 0L));
                    }
                    else
                    {
                        var orderX = this.OrderX;
                        _targetX = new SvgAnimatedInteger((long)Math.Floor(orderX.AnimVal / 2.0));
                    }
                }
                return _targetX;
            }
        }

        /// <summary>
        /// Determines the positioning in Y of the convolution matrix relative to a given target pixel in the input image. 
        /// </summary>
        /// <value>
        /// <para>targetY = "integer"</para>
        /// The leftmost column of the matrix is column number zero. The value must be such that: 
        /// <c><![CDATA[ 0 <= targetY < orderY. ]]></c>
        /// By default, the convolution matrix is centered in Y over each pixel of the input image 
        /// (i.e., <c>targetY = floor ( orderY / 2 )</c>).
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedInteger TargetY
        {
            get {
                if (_targetY == null)
                {
                    if (this.HasAttribute("targetY"))
                    {
                        _targetY = new SvgAnimatedInteger(this.GetAttribute("targetY", 0L));
                    }
                    else
                    {
                        var orderY = this.OrderY;
                        _targetY = new SvgAnimatedInteger((long)Math.Floor(orderY.AnimVal / 2.0));
                    }
                }
                return _targetY;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// <para>preserveAlpha = "false | true"</para>
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedBoolean PreserveAlpha
        {
            get {
                if (_preserveAlpha == null)
                {
                    _preserveAlpha = new SvgAnimatedBoolean(this.GetAttribute("preserveAlpha"), false);
                }
                return _preserveAlpha;
            }
        }

        #endregion

        #region Private Methods

        private void ParseOrder()
        {
            if (!this.HasAttribute("order"))
            {
                return;
            }
            var order = this.GetAttribute("order");
            if (string.IsNullOrWhiteSpace(order))
            {
                _orderX = new SvgAnimatedInteger(3.0);
                _orderY = new SvgAnimatedInteger(3.0);
                return;
            }

            var orderParts = _reSeparators.Split(order);
            if (orderParts == null || orderParts.Length == 0)
            {
                _orderX = new SvgAnimatedInteger(3.0);
                _orderY = new SvgAnimatedInteger(3.0);
                return;
            }
            if (orderParts.Length == 1)
            {
                var orderValue = SvgNumber.ParseNumber(orderParts[0]);
                _orderX = new SvgAnimatedInteger(orderValue);
                _orderY = new SvgAnimatedInteger(orderValue);
            }
            else if (orderParts.Length == 2)
            {
                _orderX = new SvgAnimatedInteger(SvgNumber.ParseNumber(orderParts[0]));
                _orderY = new SvgAnimatedInteger(SvgNumber.ParseNumber(orderParts[1]));
            }
        }

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
