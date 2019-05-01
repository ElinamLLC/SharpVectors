using System;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// Used internally to store collected properties.
	/// </summary>
	public sealed class CssCollectedProperty
    {
        #region Private Fields

        /// <summary>
        /// The name of the property
        /// </summary>
        private string _name;

        /// <summary>
        /// The calculated specificity
        /// </summary>
        private int _specificity;

        /// <summary>
        /// The origin of the collected property.
        /// </summary>
        private CssStyleSheetType _origin;

        /// <summary>
        /// The value of the property
        /// </summary>
        private CssValue _cssValue;

        /// <summary>
        /// The priority of the property, e.g. "important"
        /// </summary>
        private string _priority;

        #endregion

        #region Constructors

        public CssCollectedProperty(string name, int specificity, 
            CssValue cssValue, CssStyleSheetType origin, string priority)
		{
			_name        = name;
			_specificity = specificity;
			_origin      = origin;
			_cssValue    = cssValue;
			_priority    = priority;
		}

        #endregion

        #region Public Properties

        public string Name
        {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }

        public int Specificity
        {
            get {
                return _specificity;
            }
            set {
                _specificity = value;
            }
        }

        public CssStyleSheetType Origin
        {
            get {
                return _origin;
            }
            set {
                _origin = value;
            }
        }

        public CssValue CssValue
        {
            get {
                return _cssValue;
            }
            set {
                _cssValue = value;
            }
        }

        public string Priority
        {
            get {
                return _priority;
            }
            set {
                _priority = value;
            }
        }

        #endregion

        #region Internal methods

        internal bool IsBetterThen(CssCollectedProperty existing)
		{
			bool yes = false;
			
            // Sorting according to the rules at http://www.w3.org/TR/CSS21/cascade.html#cascading-order

			bool gotHigherSpecificity = (Specificity>=existing.Specificity);
			
			switch(existing.Origin)
			{
				case CssStyleSheetType.UserAgent:
					yes = (Origin == CssStyleSheetType.UserAgent) ? gotHigherSpecificity : true;
					break;
				case CssStyleSheetType.NonCssPresentationalHints:
					yes = (Origin != CssStyleSheetType.UserAgent);
					break;
				case CssStyleSheetType.Inline:
					yes = (Origin != CssStyleSheetType.UserAgent && 
                        Origin != CssStyleSheetType.NonCssPresentationalHints);
					break;
				case CssStyleSheetType.Author:
					if (Origin == CssStyleSheetType.Author && Priority == existing.Priority &&
						gotHigherSpecificity)
					{
						// author rules of the same priority
						yes = true;
					}
					else if (Origin == CssStyleSheetType.Inline)
					{
						// inline rules override author rules
						yes = true;
					}
					else if (Origin == CssStyleSheetType.User && Priority == "important")
					{
						// !important user rules overrides author rules
						yes = true;
					}
					else if(Origin == CssStyleSheetType.Author && existing.Priority != "important" &&
						Priority == "important")
					{
						// !important author rules override non-!important author rules
						yes = true;
					}
					break;
				case CssStyleSheetType.User:
					if (Origin == CssStyleSheetType.User && existing.Priority == Priority && gotHigherSpecificity)
					{
						yes = true;
					}
					else if ((Origin == CssStyleSheetType.Author || Origin == CssStyleSheetType.Inline) &&
						existing.Priority != "important")
					{
						// author rules overrides not !important user rules
						yes = true;
					}
					else if (Origin == CssStyleSheetType.User && Priority == "important")
					{
						// !important user rules override non-!important user rules
						yes = true;
					}
					break;
			}

			return yes;
		}

		#endregion
	}
}
