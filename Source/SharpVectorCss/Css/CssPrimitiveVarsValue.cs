using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
    public sealed class CssPrimitiveVarsValue : CssPrimitiveValue
    {
		#region Static Fields
		
		// valid variables must start with -- and must be a valid css indentifier
		// this regex doesn't deal with escape characters
		private static readonly Regex _reParser = new Regex(@"var\(\s*--([_a-zA-Z][_a-zA-Z0-9\-]*)(?:,\s*(.+?)\s*)?\)");

		#endregion

		#region Private Fields

		private string _varName;
		private string _varValue;

		#endregion

		#region Constructors

		public CssPrimitiveVarsValue(string cssText, bool readOnly)
			: base(cssText, readOnly)
		{
            this.PrimitiveType = CssPrimitiveType.Vars;
			this.OnSetCssText(cssText);

            this.ParseString(cssText);
		}

        #endregion

        #region Public Properties

        public string VarName
        {
            get {
                return _varName;
            }
            set {
                _varName = value;
            }
        }

        public string VarValue
        {
            get {
                return _varValue;
            }
            set {
                _varValue = value;
            }
        }

        #endregion

        #region Private Methods

        private void ParseString(string cssText)
        {
            if (string.IsNullOrWhiteSpace(cssText))
            {
                return;
            }
            var match = _reParser.Match(cssText);
            if (match.Success)
            {
                var varName = match.Groups.Count > 0 ? match.Groups[1].ToString() : "";
                if (!string.IsNullOrWhiteSpace(varName))
                {
                    _varName = "--" + varName.Trim();
                }
                // fallback 
                _varValue = match.Groups.Count > 2 ? match.Groups[match.Groups.Count - 1].ToString() : "";
            }
        }

        #endregion
    }
}
