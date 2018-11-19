using System;

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This is the base interface for all of the animation element interfaces: 
    /// <see cref="ISvgAnimateElement"/>, <see cref="ISvgAnimateSetElement"/>, <see cref="ISvgAnimateColorElement"/>, 
    /// <see cref="ISvgAnimateMotionElement"/> and <see cref="ISvgAnimateTransformElement"/>.
    /// </summary>
    /// <remarks>
    /// Unlike other SVG DOM interfaces, the SVG DOM does not specify convenience DOM properties corresponding to 
    /// the various language attributes on SVG's animation elements. 
    /// </remarks>
    public interface ISvgAnimationElement : ISvgElement, ISvgTests, ISvgUriReference, 
        ISvgExternalResourcesRequired, IElementTimeControl, IEventTarget
    {
        /// <summary>
        /// Gets the element which is being animated.
        /// </summary>
        ISvgElement TargetElement { get; }

        /// <summary>
        /// Gets the begin time, in seconds, for this animation element's current interval, if it exists, 
        /// regardless of whether the interval has begun yet. If there is no current interval, then a 
        /// <see cref="DomException"/> with code INVALID_STATE_ERR is thrown.
        /// </summary>
        /// <value>
        /// The start time, in seconds, of this animation element's current interval.
        /// </value>
        /// <exception cref="DomException">The animation element does not have a current interval.</exception>
        float StartTime { get; }

        /// <summary>
        /// Gets the current time in seconds relative to time zero for the given time container.
        /// </summary>
        /// <value>
        /// The current time in seconds relative to time zero for the given time container.
        /// </value>
        float CurrentTime { get; }

        /// <summary>
        /// Gets the number of seconds for the simple duration for this animation. If the simple duration 
        /// is undefined (e.g., the end time is indefinite), then an exception is raised.
        /// </summary>
        /// <value>
        /// The number of seconds for the simple duration for this animation.
        /// </value>
        /// <exception cref="DomException">The simple duration is not determined on the given element.</exception>
        float SimpleDuration { get; }

        //--------------- Attributes to control the timing of the animation
        //  "begin", "dur", "end", "min", "max", "restart", "repeatCount", "repeatDur", "fill"

        /// <summary>
        /// Gets or sets values of values that defines when the element should begin (i.e. become active).
        /// </summary>
        /// <value>
        /// The attribute value is a semicolon separated list of values.
        /// <para>
        /// If set to "indefinite", the begin of the animation will be determined by a 
        /// <see cref="IElementTimeControl.BeginElement()"/> method call or a hyperlink targeted to the element.
        /// </para>
        /// </value>
        string Begin { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the simple duration.
        /// </summary>
        /// <value>
        /// <para>The attribute value can be one of the following:</para>
        /// <list type="bullet">
        /// <item><term>Clock-value (time duration)</term>
        /// <description>
        /// Specifies the length of the simple duration in presentation time. Value must be greater than 0.
        /// </description>
        /// </item>
        /// <item><term>media</term>
        /// <description>
        /// Specifies the simple duration as the intrinsic media duration. This is only valid for elements that define media.
        /// <para>(For SVG's animation elements, if 'media' is specified, the attribute will be ignored.)</para>
        /// </description>
        /// </item>
        /// <item><term>indefinite</term>
        /// <description>Specifies the simple duration as indefinite.</description>
        /// </item>
        /// </list>
        /// <para>If the animation does not have a "dur" attribute, the simple duration is indefinite.</para>
        /// </value>
        string Duration { get; set; }

        /// <summary>
        /// Gets or sets the attribute that defines an end value for the animation that can 
        /// constrain the active duration. 
        /// </summary>
        /// <value>
        /// <para>
        /// The attribute value is a semicolon separated list of values.
        /// </para>
        /// <para>
        /// A value of "indefinite" specifies that the end of the animation will be determined by an 
        /// <see cref="IElementTimeControl.EndElement()"/> method call (the animation DOM methods are 
        /// described in DOM interfaces).
        /// </para>
        /// </value>
        string End { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the minimum value of the active duration.
        /// </summary>
        /// <value>
        /// <para>The attribute value can be either of the following:</para>
        /// <list type="bullet">
        /// <item><term>Clock-value</term>
        /// <description>
        /// Specifies the length of the minimum value of the active duration, measured in local time.
        /// Value must be greater than 0.
        /// </description>
        /// </item>
        /// <item><term>media</term>
        /// <description>
        /// Specifies the minimum value of the active duration as the intrinsic media duration. 
        /// This is only valid for elements that define media. 
        /// (For SVG's animation elements, if 'media' is specified, the attribute will be ignored.)
        /// </description>
        /// </item>
        /// </list>
        /// </value>
        string Minimum { get; set; }

        /// <summary>
        /// Gets or sets the value that specifies the maximum value of the active duration.
        /// </summary>
        /// <value>
        /// <para>The attribute value can be either of the following:</para>
        /// <list type="bullet">
        /// <item><term>Clock-value</term>
        /// <description>
        /// Specifies the length of the maximum value of the active duration, measured in local time.
        /// <para>Value must be greater than 0.</para>
        /// </description>
        /// </item>
        /// <item><term>media</term>
        /// <description>
        /// Specifies the maximum value of the active duration as the intrinsic media duration. 
        /// This is only valid for elements that define media. (For SVG's animation elements, if 
        /// 'media' is specified, the attribute will be ignored.)
        /// </description>
        /// </item>
        /// </list>
        /// <para>There is no default value for "max". This does not constrain the active duration at all.</para>
        /// </value>
        string Maximum { get; set; }

        /// <summary>
        /// Gets or sets the attribute that controls the circumstances under which an animation is restarted.
        /// </summary>
        /// <value>
        /// <list type="bullet">
        /// <item><term>always</term>
        /// <description>The animation can be restarted at any time. This is the default value.</description>
        /// </item>
        /// <item><term>whenNotActive</term>
        /// <description>
        /// The animation can only be restarted when it is not active (i.e. after the active end). 
        /// Attempts to restart the animation during its active duration are ignored.
        /// </description>
        /// </item>
        /// <item><term>never</term>
        /// <description>
        /// The element cannot be restarted for the remainder of the current simple duration of the parent 
        /// time container. (In the case of SVG, since the parent time container is the SVG document fragment, 
        /// then the animation cannot be restarted for the remainder of the document duration.)
        /// </description>
        /// </item>
        /// </list>
        /// </value>
        string Restart { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the number of iterations of the animation function. 
        /// </summary>
        /// <value>
        /// It can have the following attribute values:
        /// <list type="bullet">
        /// <item><term>numeric value</term>
        /// <description>
        /// This is a (base 10) "floating point" numeric value that specifies the number of iterations. 
        /// It can include partial iterations expressed as fraction values. 
        /// A fractional value describes a portion of the simple duration. Values must be greater than 0.
        /// </description>
        /// </item>
        /// <item><term>indefinite</term>
        /// <description>
        /// The animation is defined to repeat indefinitely (i.e. until the document ends).
        /// </description>
        /// </item>
        /// </list>
        /// </value>
        string RepeatCount { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the total duration for repeat. 
        /// </summary>
        /// <value>
        /// It can have the following attribute values: 
        /// <list type="bullet">
        /// <item><term>Clock-value</term>
        /// <description>Specifies the duration in presentation time to repeat the animation function f(t).</description>
        /// </item>
        /// <item><term>indefinite</term>
        /// <description>The animation is defined to repeat indefinitely (i.e. until the document ends).</description>
        /// </item>
        /// </list>
        /// </value>
        string RepeatDuration { get; set; }

        /// <summary>
        /// Gets or sets the attribute can be used to maintain the value of the animation after 
        /// the active duration of the animation element ends.
        /// </summary>
        /// <value>
        /// This attribute can have the following values:
        /// <list type="bullet">
        /// <item><term>freeze</term>
        /// <description>
        /// The animation effect F(t) is defined to freeze the effect value at the last value of the active duration. 
        /// The animation effect is "frozen" for the remainder of the document duration.
        /// </description>
        /// </item>
        /// <item><term>remove</term>
        /// <description>
        /// The animation effect is removed (no longer applied) when the active duration of the animation is over. 
        /// After the active end of the animation, the animation no longer affects the target. This is the default value.
        /// </description>
        /// </item>
        /// </list>
        /// </value>
        string Fill { get; set; }
    }
}
