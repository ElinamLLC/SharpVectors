using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgFECompositeElement : SvgFilterPrimitiveStandardAttributes, ISvgFECompositeElement
    {
        #region Private Fields

        private ISvgAnimatedString _in1;
        private ISvgAnimatedString _in2;
        private ISvgAnimatedNumber _k1;
        private ISvgAnimatedNumber _k2;
        private ISvgAnimatedNumber _k3;
        private ISvgAnimatedNumber _k4;
        private ISvgAnimatedEnumeration _operator;

        #endregion

        #region Constructors

        public SvgFECompositeElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFECompositeElement Members

        /// <summary>
        /// </summary>
        /// <value>
        /// <para></para>
        /// An enumeration of the type <see cref="SvgFilterCompositeOperator"/>. 
        /// The default value is <see cref="SvgFilterCompositeOperator.Over"/>.
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedEnumeration Operator
        {
            get {
                if (_operator == null)
                {
                    SvgFilterCompositeOperator compositeOperator = SvgFilterCompositeOperator.Over;
                    if (this.HasAttribute("operator"))
                    {
                        compositeOperator = (SvgFilterCompositeOperator)Enum.Parse(typeof(SvgFilterCompositeOperator),
                            this.GetAttribute("operator"), true);
                    }
                    _operator = new SvgAnimatedEnumeration((ushort)compositeOperator);
                }
                return _operator;
            }
        }

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

        public ISvgAnimatedNumber K1
        {
            get {
                if (_k1 == null)
                {
                    _k1 = new SvgAnimatedNumber(this.GetAttribute("k1", 0.0));
                }
                return _k1;
            }
        }

        public ISvgAnimatedNumber K2
        {
            get {
                if (_k2 == null)
                {
                    _k2 = new SvgAnimatedNumber(this.GetAttribute("k2", 0.0));
                }
                return _k2;
            }
        }

        public ISvgAnimatedNumber K3
        {
            get {
                if (_k3 == null)
                {
                    _k3 = new SvgAnimatedNumber(this.GetAttribute("k3", 0.0));
                }
                return _k3;
            }
        }

        public ISvgAnimatedNumber K4
        {
            get {
                if (_k4 == null)
                {
                    _k4 = new SvgAnimatedNumber(this.GetAttribute("k4", 0.0));
                }
                return _k4;
            }
        }

        #endregion
    }
}
