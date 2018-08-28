using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgAnimatedEnumeration.
	/// </summary>
    public sealed class SvgAnimatedEnumeration : ISvgAnimatedEnumeration
	{
		#region Private Fields

		private ushort _baseVal;
		private ushort _animVal;
		
        #endregion

		#region Constructor

		public SvgAnimatedEnumeration(ushort val)
		{
			_baseVal = _animVal = val;
		}

		#endregion

        #region ISvgAnimatedEnumeration Interface

		public ushort BaseVal
		{
			get
			{
				return _baseVal;
			}
			set
			{
				_baseVal = value;
			}
		}

		public ushort AnimVal
		{
			get
			{
				return _animVal;
			}
		}

		#endregion
	}
}
