using System;
using System.Xml;
using System.Collections;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// Used internally to store collected properties.
	/// </summary>
	public class CssCollectedProperty
	{
		#region Constructors
		internal CssCollectedProperty(string name, int specificity, CssValue cssValue, CssStyleSheetType origin, string priority)
		{
			Name = name;
			Specificity = specificity;
			Origin = origin;
			CssValue = cssValue;
			Priority = priority;
		}
		#endregion

		#region Public properties
		/// <summary>
		/// The name of the property
		/// </summary>
		public string Name;
		/// <summary>
		/// The calculated specificity
		/// </summary>
		public int Specificity;
		/// <summary>
		/// The origin of the collected property.
		/// </summary>
		public CssStyleSheetType Origin;
		/// <summary>
		/// The value of the property
		/// </summary>
        public CssValue CssValue;
		/// <summary>
		/// The priority of the property, e.g. "important"
		/// </summary>
		public string Priority;
		#endregion

		#region Internal methods
		internal bool IsBetterThen(CssCollectedProperty existing)
		{
			bool yes = false;
			#region sorting according to the rules at http://www.w3.org/TR/CSS21/cascade.html#cascading-order
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
					if(Origin == CssStyleSheetType.Author && 
						Priority == existing.Priority &&
						gotHigherSpecificity)
					{
						// author rules of the same priority
						yes = true;
					}
					else if(Origin == CssStyleSheetType.Inline)
					{
						// inline rules override author rules
						yes = true;
					}
					else if(Origin == CssStyleSheetType.User && Priority == "important")
					{
						// !important user rules overrides author rules
						yes = true;
					}
					else if(Origin == CssStyleSheetType.Author && 
						existing.Priority != "important" &&
						Priority == "important"
						)
					{
						// !important author rules override non-!important author rules
						yes = true;
					}
					break;
				case CssStyleSheetType.User:
					if(Origin == CssStyleSheetType.User && 
						existing.Priority == Priority && 
						gotHigherSpecificity)
					{
						yes = true;
					}
					else if((Origin == CssStyleSheetType.Author || Origin == CssStyleSheetType.Inline) &&
						existing.Priority != "important")
					{
						// author rules overrides not !important user rules
						yes = true;
					}
					else if(Origin == CssStyleSheetType.User && 
						Priority == "important")
					{
						// !important user rules override non-!important user rules
						yes = true;
					}
					break;
			}
			#endregion

			return yes;
		}
		#endregion
	}

}
