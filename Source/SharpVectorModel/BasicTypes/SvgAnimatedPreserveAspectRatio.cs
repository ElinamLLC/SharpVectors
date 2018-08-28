using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgAnimatedPreserveAspectRatio.
	/// </summary>
    public sealed class SvgAnimatedPreserveAspectRatio : ISvgAnimatedPreserveAspectRatio
	{
		#region Private Fields

		private SvgPreserveAspectRatio _baseVal;
		private SvgPreserveAspectRatio _animVal;
 		
        #endregion

		#region Constructor

		public SvgAnimatedPreserveAspectRatio(string attr, SvgElement ownerElement)
		{
			_baseVal = new SvgPreserveAspectRatio(attr, ownerElement);
			_animVal = _baseVal;
		}

		#endregion

        #region ISvgAnimatedPreserveAspectRatio Interface

		public ISvgPreserveAspectRatio BaseVal
		{
			get
			{
				return _baseVal;
			}
		}

		public ISvgPreserveAspectRatio AnimVal
		{
			get
			{
				return _animVal;
			}
		}

		#endregion
	}
}