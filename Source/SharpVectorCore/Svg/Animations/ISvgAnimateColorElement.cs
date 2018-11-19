using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SVGAnimateColorElement interface corresponds to the "animateColor" element.
    /// </summary>
    public interface ISvgAnimateColorElement : ISvgAnimationElement, ISvgStylable
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

        //--------------- Attributes that define animation values over time
        // "calcMode", "values", "keyTimes", "keySplines", "from", "to", "by"

        /// <summary>
        /// Gets or sets a value that specifies the interpolation mode for the animation. 
        /// </summary>
        /// <value>
        /// This can take any of the following values. The default mode is 'linear', however if the attribute 
        /// does not support linear interpolation (e.g. for strings), the "calcMode" attribute is ignored 
        /// and discrete interpolation is used.
        /// <list type="bullet">
        /// <item><term>discrete</term>
        /// <description>
        /// This specifies that the animation function will jump from one value to the next without any interpolation.
        /// </description>
        /// </item>
        /// <item><term>linear</term>
        /// <description>
        /// Simple linear interpolation between values is used to calculate the animation function. 
        /// Except for "animateMotion", this is the default "calcMode".
        /// </description>
        /// </item>
        /// <item><term>paced</term>
        /// <description>
        /// Defines interpolation to produce an even pace of change across the animation.
        ///  If 'paced' is specified, any "keyTimes" or "keySplines" will be ignored. 
        ///  For "animateMotion", this is the default "calcMode". 
        /// </description>
        /// </item>
        /// <item><term>spline</term>
        /// <description>
        /// Interpolates from one value in the "values" list to the next according to a time function 
        /// defined by a cubic Bézier spline. The points of the spline are defined in the "keyTimes" attribute, 
        /// and the control points for each interval are defined in the "keySplines" attribute.
        /// </description>
        /// </item>
        /// </list>
        /// </value>
        string CalcMode { get; set; }

        /// <summary>
        /// Gets or sets a semicolon-separated list of one or more values. 
        /// </summary>
        /// <value>
        /// Vector-valued attributes are supported using the vector syntax of the "attributeType" domain. 
        /// </value>
        string Values { get; set; }

        /// <summary>
        /// Gets or sets a semicolon-separated list of time values used to control the pacing of the animation.
        /// </summary>
        /// <value>
        /// Each time in the list corresponds to a value in the "values" attribute list, and defines when the 
        /// value is used in the animation function. Each time value in the "keyTimes" list is specified as 
        /// a floating point value between 0 and 1 (inclusive), representing a proportional offset into the 
        /// simple duration of the animation element.
        /// </value>
        /// <remarks>
        /// <para>
        /// For animations specified with a "values" list, the "keyTimes" attribute if specified must have 
        /// exactly as many values as there are in the "values" attribute. For from/to/by animations, 
        /// the "keyTimes" attribute if specified must have two values.
        /// </para>
        /// <para>
        /// Each successive time value must be greater than or equal to the preceding time value.
        /// </para>
        /// <para>
        /// The "keyTimes" list semantics depends upon the interpolation mode:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// For linear and spline animation, the first time value in the list must be 0, and the last time 
        /// value in the list must be 1. The key time associated with each value defines when the value is set; 
        /// values are interpolated between the key times.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// For discrete animation, the first time value in the list must be 0. The time associated with each 
        /// value defines when the value is set; the animation function uses that value until the next time 
        /// defined in "keyTimes".
        /// </description>
        /// </item>
        /// </list>
        /// <para>
        /// If the interpolation mode is 'paced', the "keyTimes" attribute is ignored.
        /// </para>
        /// <para>
        /// If there are any errors in the "keyTimes" specification (bad values, too many or too few values), 
        /// the document fragment is in error (see error processing).
        /// </para>
        /// <para>
        /// If the simple duration is indefinite, any "keyTimes" specification will be ignored.
        /// </para>
        /// </remarks>
        string KeyTimes { get; set; }

        /// <summary>
        /// Gets or sets a set of Bézier control points associated with the "keyTimes" list, defining a cubic 
        /// Bézier function that controls interval pacing.
        /// </summary>
        /// <value>
        /// <para>
        /// The attribute value is a semicolon-separated list of control point descriptions. Each control 
        /// point description is a set of four values: x1 y1 x2 y2, describing the Bézier control points 
        /// for one time segment.
        /// </para>
        /// <para>
        /// The values must all be in the range 0 to 1.
        /// </para>
        /// <para>
        /// This attribute is ignored unless the "calcMode" is set to 'spline'.
        /// </para>
        /// </value>
        string KeySplines { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the starting value of the animation.
        /// </summary>
        /// <value>
        /// Specifies the starting value of the animation.
        /// </value>
        string From { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the ending value of the animation. 
        /// </summary>
        /// <value>
        /// Specifies the ending value of the animation.
        /// </value>
        string To { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies a relative offset value for the animation.
        /// </summary>
        /// <value>
        /// Specifies a relative offset value for the animation.
        /// </value>
        string By { get; set; }


        //--------------- Attributes that control whether animations are additive
        // "additive", "accumulate"

        /// <summary>
        /// Gets or sets a value that controls whether or not the animation is additive.
        /// </summary>
        /// <value>
        /// Posible values are replace | sum, default is sum.
        /// <list type="bullet">
        /// <item><term>replace</term>
        /// <description>
        /// Specifies that the animation will override the underlying value of the attribute and 
        /// other lower priority animations.
        /// </description>
        /// </item>
        /// <item><term>sum</term>
        /// <description>
        /// Specifies that the animation will add to the underlying value of the attribute and 
        /// other lower priority animations.
        /// </description>
        /// </item>
        /// </list>
        /// </value>
        string Additive { get; set; }

        /// <summary>
        /// Gets or sets a value that controls whether or not the animation is cumulative.
        /// </summary>
        /// <value>
        /// Possible value are none | sum, default is none.
        /// <list type="bullet">
        /// <item><term>sum</term>
        /// <description>
        /// Specifies that each repeat iteration after the first builds upon the last value of 
        /// the previous iteration.
        /// </description>
        /// </item>
        /// <item><term>none</term>
        /// <description>
        /// Specifies that repeat iterations are not cumulative. This is the default.
        /// </description>
        /// </item>
        /// </list>
        /// </value>
        /// <remarks>
        /// <para>
        /// This attribute is ignored if the target attribute value does not support addition, 
        /// or if the animation element does not repeat.
        /// </para>
        /// <para>Cumulative animation is not defined for "to animation".</para>
        /// <para>This attribute will be ignored if the animation function is specified with only the "to" attribute.</para>
        /// </remarks>
        string Accumulate { get; set; }
    }
}
