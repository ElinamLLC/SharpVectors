// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com.com</developer>
// <completed>100</completed>

using System;
using System.Text;
using System.Text.RegularExpressions;

using SharpVectors.Dom;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// This interface defines a list of String objects
	/// </summary>
    public sealed class SvgStringList : SvgList<string>, ISvgStringList
	{
        #region Constructors

        public SvgStringList()
        {
        }

        public SvgStringList(string listString)
        {
            this.FromString(listString);
        }
        #endregion

		public void FromString(string listString)
		{
            // remove existing list items
            Clear();

            if (listString != null)
            {
                // remove leading and trailing whitespace
                // NOTE: Need to check if .NET whitespace = SVG (XML) whitespace
                listString = listString.Trim();

                if (listString.Length > 0)
                {
                    Regex delim = new Regex(@"\s+,?\s*|,\s*");
                    foreach (string item in delim.Split(listString))
                    {
                        // the following test is needed to catch consecutive commas
                        // for example, "one,two,,three"
                        if (item.Length == 0)
                            throw new DomException(DomExceptionType.SyntaxErr);

                        AppendItem(item);
                    }
                }
                else
                {
                    AppendItem(String.Empty);
                }
            }
		}
	}
}
