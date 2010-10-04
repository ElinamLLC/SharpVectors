using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgTextPositioningElement interface is inherited by text-related interfaces: 
    /// SvgTextElement, SvgTSpanElement, SvgTRefElement and SvgAltGlyphElement. 
	/// </summary>
	public abstract class SvgTextPositioningElement : SvgTextContentElement, ISvgTextPositioningElement
    {
        #region Constructors and Destructor

        public SvgTextPositioningElement(string prefix, string localname, string ns, 
            SvgDocument doc) : base(prefix, localname, ns, doc) 
		{
        }

        #endregion

        #region Public Properties

        public ISvgAnimatedLengthList X
		{
			get
			{
				return new SvgAnimatedLengthList("x", GetAttribute("x"), this, 
                    SvgLengthDirection.Horizontal);
			}
		}

		public ISvgAnimatedLengthList Y
		{
			get
			{
				return new SvgAnimatedLengthList("y", GetAttribute("y"), this, 
                    SvgLengthDirection.Vertical);
			}
		}

		public ISvgAnimatedLengthList Dx
		{
			get
			{
				return new SvgAnimatedLengthList("dx", GetAttribute("dx"), this, 
                    SvgLengthDirection.Horizontal);
			}
		}

		public ISvgAnimatedLengthList Dy
		{
			get
			{
				return new SvgAnimatedLengthList("dy", GetAttribute("dy"), this, 
                    SvgLengthDirection.Vertical);
			}
		}

		public ISvgAnimatedNumberList Rotate
		{
			get
			{
				return new SvgAnimatedNumberList(GetAttribute("rotate"));
			}
        }

        #endregion
    }
}
