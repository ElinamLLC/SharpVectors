using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgAnimatedLengthList.
    /// </summary>
    public sealed class SvgAnimatedNumberList : ISvgAnimatedNumberList
	{
        #region Private Fields

        private SvgNumberList _baseVal;
        private SvgNumberList _animVal;
        
        #endregion

        #region Constructor

		public SvgAnimatedNumberList(string str)
		{
			_baseVal = new SvgNumberList(str);
			_animVal = _baseVal;
		}

        #endregion
		
        #region ISvgAnimatedNumberList Interface

		public ISvgNumberList BaseVal
		{
			get
			{
				return _baseVal;
			}
		}
		
		public ISvgNumberList AnimVal
		{
			get
			{
				return _animVal;
			}
		}

        #endregion
	}
}