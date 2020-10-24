namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The <c>ICssValueList</c> interface provides the abstraction of an ordered collection of CSS values.
	/// </summary>
	public interface ICssValueList : ICssValue
	{
		/// <summary>
		/// The number of CSS Values in the list. The range of valid values of the indices is 0 to length-1 inclusive.
		/// </summary>
		ulong Length
		{
			get;
		}

		/// <summary>
		/// Used to retrieve a <see cref="ICssValue"/> by ordinal index. The order in this collection represents the 
		/// order of the values in the CSS style property. If index is greater than or equal to the number 
		/// of values in the list, this returns null.
		/// </summary>
		ICssValue this[ulong index]
		{
			get;
		}
	}
}
