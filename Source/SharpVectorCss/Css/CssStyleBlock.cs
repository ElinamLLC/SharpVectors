using System;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// Internal class that stores a style in a declaration block
    /// </summary>
    internal sealed class CssStyleBlock
    {
        #region Private Fields

        /// <summary>
        /// The type of the owner stylesheet
        /// </summary>
        private CssStyleSheetType _origin;
        /// <summary>
        /// The property name
        /// </summary>
        private string _name;
        /// <summary>
        /// The value of the style
        /// </summary>
        private string _value;
        /// <summary>
        /// The prioroty of the style, e.g. "important"
        /// </summary>
        private string _priority;
        /// <summary>
        /// The calculated specificity of the owner selector
        /// </summary>
        private int _specificity = -1;

        private CssValue _cssValue;

        #endregion

        #region Constructors

        public CssStyleBlock(string name, string val, string priority, CssStyleSheetType origin)
        {
            _name     = name.Trim();
            _value    = val.Trim();
            _priority = priority.Trim();
            _origin   = origin;
        }

        public CssStyleBlock(string name, string val, string priority, int specificity,
            CssStyleSheetType origin) : this(name, val, priority, origin)
        {
            _specificity = specificity;
        }

        public CssStyleBlock(CssStyleBlock style, int specificity, CssStyleSheetType origin)
            : this(style.Name, style.Value, style.Priority, origin)
        {
            _specificity = specificity;
        }

        #endregion

        #region Public Properties

        public string CssText
        {
            get {
                string ret = Name + ":" + Value;
                if (Priority != null && Priority.Length > 0)
                {
                    ret += " !" + Priority;
                }
                return ret;
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

        public string Name
        {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }

        public string Value
        {
            get {
                return _value;
            }
            set {
                _value = value;
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

        public int Specificity
        {
            get {
                return _specificity;
            }
            set {
                _specificity = value;
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

        #endregion
    }
}
