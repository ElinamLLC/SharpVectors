/// <developer>niklas@protocol7.com</developer>
/// <completed>100</completed>
/// 
using System;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The Rect interface is used to represent any rect value. This interface reflects the values in the underlying style property. Hence, modifications made to the CSSPrimitiveValue objects modify the style property.
	/// </summary>
	public sealed class CssRect : ICssRect
	{
        #region Private Fields

        private static Regex delim = new Regex(@"\s+,?\s*|,\s*", RegexOptions.Compiled);

        private bool readOnly;
        
        #endregion

		#region Constructors

		/// <summary>
		/// Constructs a new Rect
		/// </summary>
		/// <param name="s">The string to parse that contains the Rect structure</param>
		/// <param name="readOnly">Specifies if the Rect should be read-only</param>
		public CssRect(string rectString, bool readOnly)
		{
			this.readOnly = readOnly;

            if (rectString == null)
            {
                rectString = String.Empty;
            }

            // remove leading and trailing whitespace
            // NOTE: Need to check if .NET whitespace = SVG (XML) whitespace
            rectString = rectString.Trim();

            if (rectString.Length > 0)
            {   
                string[] parts = rectString.Split(' ');
                if (parts.Length != 4)
                {
                    parts = delim.Split(rectString);
                }
			    if (parts.Length == 4)
			    {
				    _top    = new CssPrimitiveLengthValue(parts[0], readOnly);
				    _right  = new CssPrimitiveLengthValue(parts[1], readOnly);
				    _bottom = new CssPrimitiveLengthValue(parts[2], readOnly);
				    _left   = new CssPrimitiveLengthValue(parts[3], readOnly);
			    }
			    else
			    {
                    throw new DomException(DomExceptionType.SyntaxErr);
                }
            }
		}
		#endregion

		#region IRect Members

		private CssPrimitiveValue _left;
		/// <summary>
		/// This attribute is used for the left of the rect.
		/// </summary>
		public ICssPrimitiveValue Left
		{
			get
			{
				return _left;
			}
		}

		private CssPrimitiveValue _bottom;
		/// <summary>
		/// This attribute is used for the bottom of the rect.
		/// </summary>
		public ICssPrimitiveValue Bottom
		{
			get
			{
				return _bottom;
			}
		}

		private CssPrimitiveValue _right;
		/// <summary>
		/// This attribute is used for the right of the rect.
		/// </summary>
		public ICssPrimitiveValue Right
		{
			get
			{
				return _right;
			}
		}

		private CssPrimitiveValue _top;
		/// <summary>
		/// This attribute is used for the top of the rect.
		/// </summary>
		public ICssPrimitiveValue Top
		{
			get
			{
				return _top;
			}
		}

		#endregion
	}
}
