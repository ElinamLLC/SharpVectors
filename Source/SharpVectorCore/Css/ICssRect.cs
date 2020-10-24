namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The <c>ICssRect</c> interface is used to represent any rect value. This interface reflects the values 
	/// in the underlying style property. Hence, modifications made to the <see cref="ICssPrimitiveValue"/>
	/// objects modify the style property. 
	/// </summary>
	public interface ICssRect
	{
		/// <summary>
		/// This attribute is used for the left of the rect.
		/// </summary>
		ICssPrimitiveValue Left
		{
			get;
		}
	
		/// <summary>
		/// This attribute is used for the bottom of the rect.
		/// </summary>
		ICssPrimitiveValue Bottom
		{
			get;
		}
	
		/// <summary>
		/// This attribute is used for the right of the rect.
		/// </summary>
		ICssPrimitiveValue Right
		{
			get;
		}
	
		/// <summary>
		/// This attribute is used for the top of the rect.
		/// </summary>
		ICssPrimitiveValue Top
		{
			get;
		}
	}
}
