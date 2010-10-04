// <developer>niklas@protocol7.com</developer>
// <completed>0</completed>
 
using System;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The Counter interface is used to represent any counter or 
	/// counters function value. This interface reflects the values 
	/// in the underlying style property.
	/// </summary>
	public interface ICssCounter
	{
		/// <summary>
		/// This attribute is used for the separator of the nested counters.
		/// </summary>
		string Separator
		{
			get;
		}
	
		/// <summary>
		/// This attribute is used for the style of the list.
		/// </summary>
		string ListStyle
		{
			get;
		}
	
		/// <summary>
		/// This attribute is used for the identifier of the counter.
		/// </summary>
		string Identifier
		{
			get;
		}
	}
}
