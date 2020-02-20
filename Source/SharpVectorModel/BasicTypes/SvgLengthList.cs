using System;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This interface defines a list of SvgLength objects
    /// </summary>
    public sealed class SvgLengthList : SvgList<ISvgLength>, ISvgLengthList
	{
        #region Private Fields

        private static readonly Regex _reDelim = new Regex(@"\s+,?\s*|,\s*");

		private string _propertyName;
        private SvgElement _ownerElement;
        private SvgLengthDirection _direction;

        #endregion

        #region Constructors
        public SvgLengthList()
        {
        }

        public SvgLengthList(string listString)
        {
            this.FromString(listString);
        }

        public SvgLengthList(string propertyName, string listString, SvgElement ownerElement, SvgLengthDirection direction)
        {
			this._propertyName = propertyName;
			this._ownerElement = ownerElement;
			this._direction = direction;

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

                        AppendItem(new SvgLength(_ownerElement, _propertyName, _direction, item, string.Empty));
                    }
                }
            }
        }

        #endregion
    }
}
