using System;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// This interface defines a list of String objects
	/// </summary>
    public sealed class SvgStringList : SvgList<string>, ISvgStringList
	{
        #region Private Fields

        private static readonly Regex _reDelim = new Regex(@"\s+,?\s*|,\s*");

        #endregion

        #region Constructors

        public SvgStringList()
        {
        }

        public SvgStringList(string listString)
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
                    foreach (string item in _reDelim.Split(listString))
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
                    AppendItem(string.Empty);
                }
            }
        }

        #endregion
    }
}
