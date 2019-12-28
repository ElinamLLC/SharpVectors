using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This filter primitive creates an image using the Perlin turbulence function. It allows the synthesis of 
    /// artificial textures like clouds or marble.
    /// </summary>
    public sealed class SvgFETurbulenceElement : SvgFilterPrimitiveStandardAttributes, ISvgFETurbulenceElement
    {
        #region Private Fields

        private ISvgAnimatedNumber      _baseFrequencyX;
        private ISvgAnimatedNumber      _baseFrequencyY;
        private ISvgAnimatedInteger     _numOctaves;
        private ISvgAnimatedNumber      _seed;
        private ISvgAnimatedEnumeration _stitchTiles;
        private ISvgAnimatedEnumeration _type;

        #endregion

        #region Constructors

        public SvgFETurbulenceElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFETurbulenceElement Members

        /// <summary>
        /// Corresponds to attribute 'baseFrequency' on the given 'feTurbulence' element. 
        /// Contains the X component of the 'baseFrequency' attribute.
        /// </summary>
        /// <value>
        /// <para>baseFrequency = "number-optional-number"</para>
        /// If the attribute is not specified, then the effect is as if a value of 0 were specified.
        /// </value>
        /// <remarks>
        /// The base frequency (frequencies) parameter(s) for the noise function. If two <c>number</c>s are provided, 
        /// the first number represents a base frequency in the X direction and the second value represents a base 
        /// frequency in the Y direction. If one number is provided, then that value is used for both X and Y.
        /// A negative value for base frequency is an error (see Error processing).
        /// </remarks>
        public ISvgAnimatedNumber BaseFrequencyX
        {
            get {
                if (_baseFrequencyX == null || _baseFrequencyY == null)
                {
                    this.ParseBaseFrequency();
                }
                return _baseFrequencyX;
            }
        }

        /// <summary>
        /// Corresponds to attribute 'baseFrequency' on the given 'feTurbulence' element. 
        /// Contains the Y component of the (possibly computed automatically) 'baseFrequency' attribute. 
        /// </summary>
        /// <value>
        /// <para>baseFrequency = "number-optional-number"</para>
        /// If the attribute is not specified, then the effect is as if a value of 0 were specified.
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedNumber BaseFrequencyY
        {
            get {
                if (_baseFrequencyX == null || _baseFrequencyY == null)
                {
                    this.ParseBaseFrequency();
                }
                return _baseFrequencyY;
            }
        }

        /// <summary>
        /// The numOctaves parameter for the noise function.
        /// </summary>
        /// <value>
        /// <para>numOctaves = "integer"</para>
        /// If the attribute is not specified, then the effect is as if a value of 1 were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'numOctaves' on the given 'feTurbulence' element.
        /// </remarks>
        public ISvgAnimatedInteger NumOctaves
        {
            get {
                if (_numOctaves == null)
                {
                    _numOctaves = new SvgAnimatedInteger(this.GetAttribute("numOctaves", 1));
                }
                return _numOctaves;
            }
        }

        /// <summary>
        /// The starting number for the pseudo random number generator.
        /// </summary>
        /// <value>
        /// <para>seed = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of 0 were specified. 
        /// When the seed number is handed over to the algorithm above it must first be truncated, i.e. 
        /// rounded to the closest integer value towards zero.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'seed' on the given 'feTurbulence' element. 
        /// </remarks>
        public ISvgAnimatedNumber Seed
        {
            get {
                if (_seed == null)
                {
                    _seed = new SvgAnimatedNumber(this.GetAttribute("seed", 0.0));
                }
                return _seed;
            }
        }

        /// <summary>
        /// Corresponds to attribute 'stitchTiles' on the given 'feTurbulence' element. 
        /// </summary>
        /// <value>
        /// <para>stitchTiles = "stitch | noStitch"</para>
        /// An enumeration of the type <see cref="SvgFilterStitchOption"/>. 
        /// The default value is <see cref="SvgFilterStitchOption.NoStitch"/>.
        /// </value>
        /// <remarks>
        /// <para>
        /// If stitchTiles="noStitch", no attempt it made to achieve smooth transitions at the border of tiles which contain a 
        /// turbulence function. Sometimes the result will show clear discontinuities at the tile borders.
        /// If stitchTiles = "stitch", then the user agent will automatically adjust baseFrequency-x and baseFrequency-y values 
        /// such that the feTurbulence node's width and height (i.e., the width and height of the current subregion) contains 
        /// an integral number of the Perlin tile width and height for the first octave. The baseFrequency will be adjusted up 
        /// or down depending on which way has the smallest relative (not absolute) change as follows: Given the frequency, 
        /// calculate lowFreq=floor(width*frequency)/width and hiFreq=ceil(width*frequency)/width. 
        /// If <c><![CDATA[frequency/lowFreq < hiFreq/frequency]]></c> then use lowFreq, else use hiFreq. 
        /// While generating turbulence values, generate lattice vectors as normal for Perlin Noise, except for those 
        /// lattice points that lie on the right or bottom edges of the active area (the size of the resulting tile). 
        /// In those cases, copy the lattice vector from the opposite edge of the active area. 
        /// </para>
        /// <para>
        /// If attribute 'stitchTiles' is not specified, then the effect is as if a value of noStitch were specified. 
        /// </para>
        /// </remarks>
        public ISvgAnimatedEnumeration StitchTiles
        {
            get {
                if (_stitchTiles == null)
                {
                    SvgFilterStitchOption stitchTilesType = SvgFilterStitchOption.NoStitch;
                    if (this.HasAttribute("stitchTiles"))
                    {
                        stitchTilesType = (SvgFilterStitchOption)Enum.Parse(typeof(SvgFilterStitchOption),
                            this.GetAttribute("stitchTiles"), true);
                    }
                    _stitchTiles = new SvgAnimatedEnumeration((ushort)stitchTilesType);
                }
                return _stitchTiles;
            }
        }

        /// <summary>
        /// Indicates whether the filter primitive should perform a noise or turbulence function. 
        /// </summary>
        /// <value>
        /// <para>type = "fractalNoise | turbulence"</para>
        /// An enumeration of the type <see cref="SvgFilterTurbulenceType"/>. 
        /// The default value is <see cref="SvgFilterTurbulenceType.Turbulence"/>.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'type' on the given 'feTurbulence' element.
        /// If attribute 'type' is not specified, then the effect is as if a value of turbulence were specified.
        /// </remarks>
        public ISvgAnimatedEnumeration Type
        {
            get {
                if (_type == null)
                {
                    SvgFilterTurbulenceType turbulenceType = SvgFilterTurbulenceType.Turbulence;
                    if (this.HasAttribute("type"))
                    {
                        turbulenceType = (SvgFilterTurbulenceType)Enum.Parse(typeof(SvgFilterTurbulenceType),
                            this.GetAttribute("type"), true);
                    }
                    _type = new SvgAnimatedEnumeration((ushort)turbulenceType);
                }
                return _type;
            }
        }

        #endregion

        #region Private Methods

        private void ParseBaseFrequency()
        {
            if (!this.HasAttribute("baseFrequency"))
            {
                _baseFrequencyX = new SvgAnimatedNumber(0.0);
                _baseFrequencyY = new SvgAnimatedNumber(0.0);
            }
            var baseFrequency = this.GetAttribute("baseFrequency");
            if (string.IsNullOrWhiteSpace(baseFrequency))
            {
                _baseFrequencyX = new SvgAnimatedNumber(0.0);
                _baseFrequencyY = new SvgAnimatedNumber(0.0);
            }

            var baseFrequencyParts = _reSeparators.Split(baseFrequency);
            if (baseFrequencyParts == null || baseFrequencyParts.Length == 0)
            {
                _baseFrequencyX = new SvgAnimatedNumber(0.0);
                _baseFrequencyY = new SvgAnimatedNumber(0.0);
            }
            if (baseFrequencyParts.Length == 1)
            {
                var baseFrequencyValue = SvgNumber.ParseNumber(baseFrequencyParts[0]);
                _baseFrequencyX = new SvgAnimatedNumber(baseFrequencyValue);
                _baseFrequencyY = new SvgAnimatedNumber(baseFrequencyValue);
            }
            else if (baseFrequencyParts.Length == 2)
            {
                _baseFrequencyX = new SvgAnimatedNumber(SvgNumber.ParseNumber(baseFrequencyParts[0]));
                _baseFrequencyY = new SvgAnimatedNumber(SvgNumber.ParseNumber(baseFrequencyParts[1]));
            }
        }

        #endregion
    }
}
