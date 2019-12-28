using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgFEColorMatrixElement : SvgFilterPrimitiveStandardAttributes, ISvgFEColorMatrixElement
    {
        #region Private Fields

        private ISvgAnimatedString _in1;
        private ISvgAnimatedEnumeration _type;
        private ISvgAnimatedNumberList _values;

        #endregion

        #region Constructors

        public SvgFEColorMatrixElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFEColorMatrixElement Members

        /// <summary>
        /// </summary>
        /// <value>
        /// <para></para>
        /// An enumeration of the type <see cref="SvgFilterColorMatrix"/>. 
        /// The default value is <see cref="SvgFilterColorMatrix.Matrix"/>.
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedEnumeration Type
        {
            get {
                if (_type == null)
                {
                    SvgFilterColorMatrix modeType = SvgFilterColorMatrix.Matrix;
                    if (this.HasAttribute("type"))
                    {
                        modeType = (SvgFilterColorMatrix)Enum.Parse(typeof(SvgFilterColorMatrix),
                            this.GetAttribute("type"), true);
                    }
                    _type = new SvgAnimatedEnumeration((ushort)modeType);
                }
                return _type;
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

        public ISvgAnimatedNumberList Values
        {
            get {
                if (_values == null)
                {
                    _values = SvgAnimatedNumberList.Empty;
                    if (this.HasAttribute("values"))
                    {
                        _values = new SvgAnimatedNumberList(this.GetAttribute("values"));
                    }
                }
                return _values;
            }
        }

        #endregion
    }
}
