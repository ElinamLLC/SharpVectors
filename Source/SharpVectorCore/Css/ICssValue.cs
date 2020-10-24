namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// The <c>ICssValue</c> interface represents the current computed value of a CSS property.
    /// </summary>
    public interface ICssValue
    {
        /// <summary>
        /// A string representation of the current value.
        /// </summary>
        /// <exception cref="DomException">
        /// <c>SYNTAX_ERR:</c> Raised if the specified CSS string value has a syntax error 
        /// (according to the attached property) or is unparsable.
        /// </exception>
        /// <exception cref="DomException">
        /// <c>INVALID_MODIFICATION_ERR:</c> Raised if the specified CSS string value represents a 
        /// different type of values than the values allowed by the CSS property.</exception>
        /// <exception cref="DomException">
        /// <c>NO_MODIFICATION_ALLOWED_ERR:</c> Raised if this value is readonly.</exception>
        string CssText
        {
            get;
            set;
        }

        /// <summary>
        /// An enumeration code defining the type of the value.
        /// </summary>
        CssValueType CssValueType
        {
            get;
        }

        /// <summary>
        /// Gets a value specifiying whether this CSS value an absolute or variable.
        /// <para>This is an implementation extension, not part of the CSS specification.</para>
        /// </summary>
        bool IsAbsolute
        {
            get;
        }
    }
}
