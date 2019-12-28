using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This filter primitive uses the pixels values from the image from 'in2' to spatially displace the image from 'in'. 
    /// </summary>
    public sealed class SvgFEDisplacementMapElement : SvgFilterPrimitiveStandardAttributes, ISvgFEDisplacementMapElement
    {
        #region Private Fields

        private ISvgAnimatedString _in1;
        private ISvgAnimatedString _in2;

        private ISvgAnimatedNumber _scale;

        private ISvgAnimatedEnumeration _xChannelSelector;
        private ISvgAnimatedEnumeration _yChannelSelector;

        #endregion

        #region Constructors

        public SvgFEDisplacementMapElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFEDisplacementMapElement Members

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
        /// Corresponds to attribute <c>in2</c> on the given <c>feBlend</c> element. 
        /// </summary>
        /// <value>
        /// <para></para>
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedString In2
        {
            get {
                if (_in2 == null)
                {
                    _in2 = new SvgAnimatedString(this.GetAttribute("in2", string.Empty));
                }
                return _in2;
            }
        }

        /// <summary>
        /// Displacement scale factor. The amount is expressed in the coordinate system established by 
        /// attribute 'primitiveUnits' on the 'filter' element. 
        /// </summary>
        /// <value>
        /// <para>scale = "number"</para>
        /// <para>When the value of this attribute is <c>0</c>, this operation has no effect on the source image.</para>
        /// <para>If the attribute is not specified, then the effect is as if a value of <c>0</c> were specified.</para>
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedNumber Scale
        {
            get {
                if (_scale == null)
                {
                    _scale = new SvgAnimatedNumber(this.GetAttribute("scale", 0.0));
                }
                return _scale;
            }
        }

        /// <summary>
        /// Indicates which channel from 'in2' to use to displace the pixels in 'in' along the x-axis. 
        /// </summary>
        /// <value>
        /// <para>xChannelSelector = "R | G | B | A"</para>
        /// An enumeration of the type <see cref="SvgFilterChannelSelector"/>. The default value is <see cref="SvgFilterChannelSelector.A"/>.
        /// </value>
        /// <remarks>
        /// If attribute 'xChannelSelector' is not specified, then the effect is as if a value of A were specified. 
        /// </remarks>
        public ISvgAnimatedEnumeration XChannelSelector
        {
            get {
                if (_xChannelSelector == null)
                {
                    SvgFilterChannelSelector xChannelSelectorType = SvgFilterChannelSelector.A;
                    if (this.HasAttribute("xChannelSelector"))
                    {
                        xChannelSelectorType = (SvgFilterChannelSelector)Enum.Parse(typeof(SvgFilterChannelSelector),
                            this.GetAttribute("xChannelSelector"), true);
                    }
                    _xChannelSelector = new SvgAnimatedEnumeration((ushort)xChannelSelectorType);
                }
                return _xChannelSelector;
            }
        }

        /// <summary>
        /// Indicates which channel from 'in2' to use to displace the pixels in 'in' along the y-axis. 
        /// </summary>
        /// <value>
        /// <para>yChannelSelector = "R | G | B | A"</para>
        /// An enumeration of the type <see cref="SvgFilterChannelSelector"/>. The default value is <see cref="SvgFilterChannelSelector.A"/>.
        /// </value>
        /// <remarks>
        /// If attribute 'yChannelSelector' is not specified, then the effect is as if a value of A were specified. 
        /// </remarks>
        public ISvgAnimatedEnumeration YChannelSelector
        {
            get {
                if (_yChannelSelector == null)
                {
                    SvgFilterChannelSelector yChannelSelectorType = SvgFilterChannelSelector.A;
                    if (this.HasAttribute("yChannelSelector"))
                    {
                        yChannelSelectorType = (SvgFilterChannelSelector)Enum.Parse(typeof(SvgFilterChannelSelector),
                            this.GetAttribute("yChannelSelector"), true);
                    }
                    _yChannelSelector = new SvgAnimatedEnumeration((ushort)yChannelSelectorType);
                }
                return _yChannelSelector;
            }
        }

        #endregion
    }
}
