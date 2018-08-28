using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgAnimatedNumber.
	/// </summary>
    public sealed class SvgAnimatedBoolean : ISvgAnimatedBoolean
	{
		#region Private Fields

		private bool _baseVal;
		private bool _animVal;
		
        #endregion

		#region Constructor

		public SvgAnimatedBoolean(string str, bool defaultValue)
		{
            switch (str)
            {
                case "true":
                    _baseVal = true;
                    break;
                case "false":
                    _baseVal = false;
                    break;
                default:
                    _baseVal = defaultValue;
                    break;
            }
            _animVal = _baseVal;
		}

		#endregion

        #region ISvgAnimatedBoolean Interface

		public bool BaseVal
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

		public bool AnimVal
		{
			get
			{
				return _animVal;
			}
		}

		#endregion
	}
}
