using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This filter blends two objects together using commonly used imaging software blending modes. 
    /// It performs a pixel-wise combination of two input images.
    /// </summary>
    public sealed class SvgFEBlendElement : SvgFilterPrimitiveStandardAttributes, ISvgFEBlendElement
    {
        #region Private Fields

        private ISvgAnimatedEnumeration _mode;
        private ISvgAnimatedString _in1;
        private ISvgAnimatedString _in2;

        #endregion

        #region Constructors

        public SvgFEBlendElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFEBlendElement Members

        /// <summary>
        /// One of the blend modes defined by 'Compositing and Blending Level 1' with the input in representing the source 
        /// <c>Cs</c> and the second input <c>in2</c> representing the backdrop <c>Cb</c>. The output of this filter 
        /// primitive <c>Cm</c> is the result of blending <c>Cs</c> with <c>Cb</c>.
        /// </summary>
        /// <value>
        /// <para>mode = "blend-mode" </para>
        /// An enumeration of the type <see cref="SvgFilterBlendMode"/>. The default value is <see cref="SvgFilterBlendMode.Normal"/>.
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedEnumeration Mode
        {
            get {
                if (_mode == null)
                {
                    SvgFilterBlendMode modeType = SvgFilterBlendMode.Normal;
                    if (this.HasAttribute("mode"))
                    {
                        modeType = (SvgFilterBlendMode)Enum.Parse(typeof(SvgFilterBlendMode),
                            this.GetAttribute("mode").Replace("-", ""), true);
                    }
                    _mode = new SvgAnimatedEnumeration((ushort)modeType);
                }
                return _mode;
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

        #endregion
    }
}
