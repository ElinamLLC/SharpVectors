using System;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSRule interface is the abstract base interface for any
	/// type of CSS statement. This includes both rule sets and 
	/// at-rules. An implementation is expected to preserve all rules 
	/// specified in a CSS style sheet, even if the rule is not recognized 
	/// by the parser. Unrecognized rules are represented using the 
	/// CSSUnknownRule interface. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>80</completed>	
	public interface ICssRule
	{
		/// <summary>
		/// The type of the rule, as defined above. The expectation is that binding-specific casting methods can be used to cast down from an instance of the CSSRule interface to the specific derived interface implied by the type.
		/// </summary>
		SharpVectors.Dom.Css.ICssStyleSheet ParentStyleSheet{get;}

		/// <summary>
		/// The style sheet that contains this rule.
		/// </summary>
		SharpVectors.Dom.Css.ICssRule ParentRule{get;}
	
		/// <summary>
		/// If this rule is contained inside another rule (e.g. a style rule inside an @media block), this is the containing rule. If this rule is not nested inside any other rules, this returns null
		/// </summary>
		SharpVectors.Dom.Css.CssRuleType Type{get;}
	
		/// <summary>
		/// The parsable textual representation of the rule. This reflects the current state of the rule and not its initial value
		/// </summary>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified CSS string value has a syntax error and is unparsable.</exception>
		/// <exception cref="DomException">INVALID_MODIFICATION_ERR: Raised if the specified CSS string value represents a different type of rule than the current one.</exception>
		/// <exception cref="DomException">HIERARCHY_REQUEST_ERR: Raised if the rule cannot be inserted at this point in the style sheet.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if the rule is readonly.</exception>
		string CssText{get;set;}
	}
}
