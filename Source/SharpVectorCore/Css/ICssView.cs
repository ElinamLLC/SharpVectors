using System.Xml;

using SharpVectors.Dom.Views;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// This interface represents a CSS view. The <see cref="GetComputedStyle"/> method provides a read only access 
    /// to the computed values of an element. 
    /// </summary>
    public interface ICssView : IAbstractView
    {
		/// <summary>
		/// This method is used to get the computed style as it is defined in [CSS2].
		/// </summary>
		/// <param name="elt">The element whose style is to be computed. This parameter cannot be null.</param>
		/// <param name="pseudoElt">The pseudo-element or null if none</param>
		ICssStyleDeclaration GetComputedStyle(XmlElement elt, string pseudoElt);
	}
}
