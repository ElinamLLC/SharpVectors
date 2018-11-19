using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Object-oriented access to the attributes of the "set" element via the SVG DOM is not available.
    /// </summary>
    public interface ISvgAnimateSetElement : ISvgAnimationElement
    {
        //--------------- Attributes to identify the target attribute or property for an animation
        /// <summary>
        /// Gets or sets the name of the target attribute. An XMLNS prefix may be used to indicate the XML 
        /// namespace for the attribute. The prefix will be interpreted in the scope of the 
        /// current (i.e., the referencing) animation element.
        /// </summary>
        string AttributeName { get; }

        /// <summary>
        /// Gets or sets the namespace in which the target attribute and its associated values are defined. 
        /// </summary>
        /// <value>
        /// The attribute value is one of the following (values are case-sensitive): CSS | XML | auto
        /// <list type="bullet">
        /// <item><term>CSS</term>
        /// <description>
        /// This specifies that the value of "attributeName" is the name of a CSS property 
        /// defined as animatable in this specification.
        /// </description>
        /// </item>
        /// <item><term>XML</term>
        /// <description>
        /// This specifies that the value of "attributeName" is the name of an XML attribute defined 
        /// in the default XML namespace for the target element.If the value for "attributeName" has 
        /// an XMLNS prefix, the implementation must use the associated namespace as defined in the 
        /// scope of the target element.The attribute must be defined as animatable in this specification.
        /// </description>
        /// </item>
        /// <item><term>auto</term>
        /// <description>
        /// The implementation should match the "attributeName" to an attribute for the target element.
        /// The implementation must first search through the list of CSS properties for a matching property name, 
        /// and if none is found, search the default XML namespace for the element.
        /// </description>
        /// </item>
        /// </list>
        /// <para>The default value is 'auto'.</para>
        /// </value>
        string AttributeType { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the ending value of the animation. 
        /// </summary>
        /// <value>
        /// Specifies the value for the attribute during the duration of the ‘set’ element. 
        /// The argument value must match the attribute type.
        /// </value>
        string To { get; set; }
    }
}
