// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

using System;
using System.Text;
using System.Text.RegularExpressions;

using SharpVectors.Dom;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgNumberList.
	/// </summary>
    public sealed class SvgNumberList : SvgList<ISvgNumber>, ISvgNumberList
    {
        #region Private Fields

        private static Regex delim = new Regex(@"\s+,?\s*|,\s*", RegexOptions.Compiled);

        #endregion

        #region Constructors

        public SvgNumberList()
        {
        }

        public SvgNumberList(string listString)
        {
            this.FromString(listString);
        }

        #endregion

        #region Public Methods

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
                    foreach ( string item in delim.Split(listString) )
                    {
                        // the following test is needed to catch consecutive commas
                        // for example, "one,two,,three"
                        if (item.Length == 0)
                            throw new DomException(DomExceptionType.SyntaxErr);

                        AppendItem(new SvgNumber(item));
                    }
                }
            }
        }

        #endregion
    }
}
