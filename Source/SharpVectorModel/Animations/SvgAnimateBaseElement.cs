using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
    public abstract class SvgAnimateBaseElement : SvgAnimationElement
    {
        #region Private Fields

        private static readonly Regex _isImportant = new Regex(@"!\s*important$");

        private ISvgAnimatedString _className;
        private IDictionary<string, ICssValue> _presentationAttributes = new Dictionary<string, ICssValue>();

        #endregion

        #region Constructors and Destructors

        protected SvgAnimateBaseElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region SvgAnimateBaseElement Members

        public string AttributeName
        {
            get {
                return this.GetAttribute("attributeName");
            }
            set {
                this.SetAttribute("attributeName", value);
            }
        }

        public string AttributeType
        {
            get {
                return this.GetAttribute("attributeType");
            }
            set {
                this.SetAttribute("attributeType", value);
            }
        }

        public string By
        {
            get {
                return this.GetAttribute("by");
            }
            set {
                this.SetAttribute("by", value);
            }
        }

        public string CalcMode
        {
            get {
                return this.GetAttribute("calcMode");
            }
            set {
                this.SetAttribute("calcMode", value);
            }
        }

        public string From
        {
            get {
                return this.GetAttribute("from");
            }
            set {
                this.SetAttribute("from", value);
            }
        }

        public string KeySplines
        {
            get {
                return this.GetAttribute("keySplines");
            }
            set {
                this.SetAttribute("keySplines", value);
            }
        }

        public string KeyTimes
        {
            get {
                return this.GetAttribute("keyTimes");
            }
            set {
                this.SetAttribute("keyTimes", value);
            }
        }

        public string To
        {
            get {
                return this.GetAttribute("to");
            }
            set {
                this.SetAttribute("to", value);
            }
        }

        public string Values
        {
            get {
                return this.GetAttribute("values");
            }
            set {
                this.SetAttribute("values", value);
            }
        }

        public string Accumulate
        {
            get {
                return this.GetAttribute("accumulate");
            }
            set {
                this.SetAttribute("accumulate", value);
            }
        }

        public string Additive
        {
            get {
                return this.GetAttribute("additive");
            }
            set {
                this.SetAttribute("", value);
            }
        }

        #endregion

        #region ISvgStylable Members

        public ISvgAnimatedString ClassName
        {
            get {
                if (_className == null)
                {
                    _className = new SvgAnimatedString(GetAttribute("class", string.Empty));
                }
                return _className;
            }
        }

        public ICssValue GetPresentationAttribute(string name)
        {
            if (!_presentationAttributes.ContainsKey(name))
            {
                ICssValue result;
                string attValue = GetAttribute(name, string.Empty).Trim();
                if (attValue != null && attValue.Length > 0)
                {
                    if (_isImportant.IsMatch(attValue))
                    {
                        result = null;
                    }
                    else
                    {
                        result = CssValue.GetCssValue(attValue, false);
                    }
                }
                else
                {
                    result = null;
                }
                _presentationAttributes[name] = result;
            }
            return _presentationAttributes[name];
        }

        #endregion
    }
}
