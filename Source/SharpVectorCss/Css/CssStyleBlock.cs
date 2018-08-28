using System;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// Internal class that stores a style in a declaration block
	/// </summary>
	internal sealed class CssStyleBlock
	{
		#region Constructors

		internal CssStyleBlock(string name, string val, string priority, CssStyleSheetType origin)
		{
			Name = name.Trim();
			Value = val.Trim();
			Priority = priority.Trim();
			Origin = origin;
		}

		internal CssStyleBlock(string name, string val, string priority, int specificity, 
            CssStyleSheetType origin) : this(name, val, priority, origin)
		{
			Specificity = specificity;
		}

		internal CssStyleBlock(CssStyleBlock style, int specificity, CssStyleSheetType origin) 
            : this(style.Name, style.Value, style.Priority, origin)
		{
			Specificity = specificity;
		}

        #endregion

        #region Public properties

        public string CssText
		{
			get
			{
				string ret = Name + ":" + Value;
				if(Priority != null && Priority.Length > 0)
				{
                    ret += " !" + Priority;
				}
                return ret;
			}
		}

        /// <summary>
        /// The type of the owner stylesheet
        /// </summary>
        public CssStyleSheetType Origin;
        /// <summary>
        /// The property name
        /// </summary>
        public string Name;
        /// <summary>
        /// The value of the style
        /// </summary>
        public string Value;
        /// <summary>
        /// The prioroty of the style, e.g. "important"
        /// </summary>
        public string Priority;
        /// <summary>
        /// The calculated specificity of the owner selector
        /// </summary>
        public int Specificity = -1;

		public CssValue CssValue;

		#endregion
	}
}
