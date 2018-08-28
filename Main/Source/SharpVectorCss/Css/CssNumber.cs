using System;
using System.Globalization;

namespace SharpVectors.Dom.Css
{
    public static class CssNumber
	{
		private static NumberFormatInfo _format;

		public static NumberFormatInfo Format
		{
			get
			{
				if (_format == null)
				{
					_format = new NumberFormatInfo();

					_format.NumberDecimalSeparator = ".";
				}

				return _format;
			}
		}
	}
}
