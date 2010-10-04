// <developer>niklas@protocol7.com</developer>
// <completed>20</completed>

using System.Xml;
using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// A key interface definition is the
    /// <see cref="ISvgSvgElement">ISvgSvgElement</see> interface, which is
    /// the interface that corresponds to the 'svg' element.
    /// </summary>
    /// <remarks>
    /// This interface
    /// contains various miscellaneous commonly-used utility methods, such
    /// as matrix operations and the ability to control the time of redraw
    /// on visual rendering devices.
    /// <see cref="ISvgSvgElement">ISvgSvgElement</see> extends ViewCSS and
    /// DocumentCSS to provide access to
    /// the computed values of properties and the override style sheet as
    /// described in DOM2.
    /// </remarks>
    public interface ISvgSvgElement : ISvgElement, ISvgTests, ISvgLangSpace, 
        ISvgExternalResourcesRequired, ISvgStylable, ISvgLocatable, ISvgFitToViewBox, 
        ISvgZoomAndPan, IEventTarget
    {
        /// <summary>
        /// Corresponds to attribute x on the given 'svg' element.
        /// </summary>
        ISvgAnimatedLength X
        {
            get;
        }

        /// <summary>
        /// Corresponds to attribute y on the given 'svg' element.
        /// </summary>
        ISvgAnimatedLength Y
        {
            get;
        }

        /// <summary>
        /// Corresponds to attribute width on the given 'svg' element.
        /// </summary>
        ISvgAnimatedLength Width
        {
            get;
        }

        /// <summary>
        /// Corresponds to attribute height on the given 'svg' element.
        /// </summary>
        ISvgAnimatedLength Height
        {
            get;
        }

        /// <summary>
        /// Corresponds to attribute contentScriptType on the given 'svg'
        /// element.
        /// </summary>
        string ContentScriptType
        {
            get;
            set;
        }

        /// <summary>
        /// Corresponds to attribute contentStyleType on the given 'svg' element.
        /// </summary>
        string ContentStyleType
        {
            get;
            set;
        }

        /// <summary>
        /// The position and size of the viewport (implicit or explicit) that
        /// corresponds to this 'svg' element.
        /// </summary>
        /// <remarks>
        /// <p>
        /// When the user agent is
        /// actually rendering the content, then the position and size values
        /// represent the actual values when rendering. The position and size
        /// values are unitless values in the coordinate system of the parent
        /// element. If no parent element exists (i.e., 'svg' element
        /// represents the root of the document tree), if this SVG document
        /// is embedded as part of another document (e.g., via the HTML
        /// 'object' element), then the position and size are unitless values
        /// in the coordinate system of the parent document. (If the parent
        /// uses CSS or XSL layout, then unitless values represent pixel units
        /// for the current CSS or XSL viewport, as described in the CSS2
        /// specification.) If the parent element does not have a coordinate
        /// system, then the user agent should provide reasonable default
        /// values for this attribute.
        /// </p>
        /// <p>
        /// The object itself and its contents are both readonly.
        /// </p>
        /// </remarks>
        ISvgRect Viewport
        {
            get;
        }

        /// <summary>
        /// Size of a pixel unit (as defined by CSS2) along the x-axis of the
        /// viewport, which represents a unit somewhere in the range of 70dpi
        /// to 120dpi, and, on systems that support this, might actually match
        /// the characteristics of the target medium.
        /// </summary>
        /// <remarks>
        /// On systems where it is impossible to know the size of a pixel, a
        /// suitable default pixel size is provided.
        /// </remarks>
        float PixelUnitToMillimeterX
        {
            get;
        }

        /// <summary>
        /// Corresponding size of a pixel unit along the y-axis of the viewport.
        /// </summary>
        float PixelUnitToMillimeterY
        {
            get;
        }

        /// <summary>
        /// User interface (UI) events in DOM Level 2 indicate the screen
        /// positions at which the given UI event occurred. When the user
        /// agent actually knows the physical size of a "screen unit", this
        /// attribute will express that information; otherwise, user agents
        /// will provide a suitable default value such as .28mm.
        /// </summary>
        float ScreenPixelToMillimeterX
        {
            get;
        }

        /// <summary>
        /// Corresponding size of a screen pixel along the y-axis of the
        /// viewport.
        /// </summary>
        float ScreenPixelToMillimeterY
        {
            get;
        }

        /// <summary>
        /// The initial view (i.e., before magnification and panning) of the
        /// current innermost SVG document fragment can be either the
        /// "standard" view (i.e., based on attributes on the 'svg' element
        /// such as fitBoxToViewport) or to a "custom" view (i.e., a
        /// hyperlink into a particular 'view' or other element - see
        /// <a href="http://www.w3.org/TR/SVG/linking.html#LinksIntoSVG"
        /// >Linking into SVG content: URI fragments and SVG views</a>). If
        /// the initial view is the "standard" view, then this attribute is
        /// false. If the initial view is a "custom" view, then this
        /// attribute is true.
        /// </summary>
        bool UseCurrentView
        {
            get;
            set;
        }

        /// <summary>
        /// The definition of the initial view (i.e., before magnification
        /// and panning) of the current innermost SVG document fragment.
        /// </summary>
        /// <remarks>
        /// The meaning depends on the situation:
        /// <list type="bullet">
        ///  <item><description>
        ///  If the initial view was a "standard" view, then:
        ///   <list type="bullet">
        ///    <item><description>
        ///    the values for viewBox, preserveAspectRatio and zoomAndPan
        ///    within currentView will match the values for the corresponding
        ///    DOM attributes that are on SVGSVGElement directly
        ///    </description></item>
        ///    <item><description>
        ///    the values for transform and viewTarget within currentView will
        ///    be null
        ///    </description></item>
        ///   </list>
        ///  </description></item>
        ///  <item><description>
        ///  If the initial view was a link into a 'view' element, then:
        ///    <list type="bullet">
        ///     <item><description>
        ///     the values for viewBox, preserveAspectRatio and zoomAndPan within
        ///     currentView will correspond to the corresponding attributes for
        ///     the given 'view' element
        ///     </description></item>
        ///     <item><description>
        ///     the values for transform and viewTarget within currentView will
        ///     be null
        ///     </description></item>
        ///    </list>
        ///  </description></item>
        ///  <item><description>
        ///  If the initial view was a link into another element (i.e., other
        ///  than a 'view'), then:
        ///   <list type="bullet">
        ///    <item><description>
        ///    the values for viewBox, preserveAspectRatio and zoomAndPan
        ///    within currentView will match the values for the corresponding
        ///    DOM attributes that are on SVGSVGElement directly for the
        ///    closest ancestor 'svg' element
        ///    </description></item>
        ///    <item><description>
        ///    the values for transform within currentView will be null
        ///    </description></item>
        ///    <item><description>
        ///    the viewTarget within currentView will represent the target of
        ///    the link
        ///    </description></item>
        ///   </list>
        ///  </description></item>
        ///  <item><description>
        ///  If the initial view was a link into the SVG document fragment
        ///  using an SVG view specification fragment identifier (i.e.,
        ///  #svgView(...)), then:
        ///   <list type="bullet">
        ///     <item><description>
        ///     the values for viewBox, preserveAspectRatio, zoomAndPan,
        ///     transform and viewTarget within currentView will correspond
        ///     to the values from the SVG view specification fragment
        ///     identifier
        ///     </description></item>
        ///   </list>
        ///  </description></item>
        /// </list>
        /// The object itself and its contents are both readonly.
        /// </remarks>
        ISvgViewSpec CurrentView
        {
            get;
        }

        /// <summary>
        /// This attribute indicates the current scale factor relative to
        /// the initial view to take into account user magnification and
        /// panning operations, as described under <a
        /// href="http://www.w3.org/TR/SVG/interact.html#ZoomAndPanAttribute"
        /// >Magnification and panning</a>.
        /// </summary>
        /// <remarks>
        /// DOM attributes currentScale and currentTranslate are
        /// equivalent to the 2x3 matrix [a b c d e f] = [currentScale 0
        /// 0 currentScale currentTranslate.x currentTranslate.y]. If
        /// "magnification" is enabled (i.e., zoomAndPan="magnify"), then
        /// the effect is as if an extra transformation were placed at the
        /// outermost level on the SVG document fragment (i.e., outside the
        /// outermost 'svg' element).
        /// </remarks>
        float CurrentScale
        {
            get;
            set;
        }

        /// <summary>
        /// The corresponding translation factor that takes into account
        /// user "magnification".
        /// </summary>
        ISvgPoint CurrentTranslate
        {
            get;
        }

        /// <summary>
        /// Takes a time-out value which indicates that redraw shall not
        /// occur until certain conditions are met.
        /// </summary>
        /// <remarks>
        /// Takes a time-out value which indicates that redraw shall not
        /// occur until: (a) the corresponding unsuspendRedraw(
        /// suspend_handle_id) call has been made, (b) an
        /// unsuspendRedrawAll() call has been made, or (c) its timer
        /// has timed out. In environments that do not support
        /// interactivity (e.g., print media), then redraw shall not be
        /// suspended. suspend_handle_id = suspendRedraw(
        /// max_wait_milliseconds) and unsuspendRedraw(suspend_handle_id)
        /// must be packaged as balanced pairs. When you want to suspend
        /// redraw actions as a collection of SVG DOM changes occur,
        /// then precede the changes to the SVG DOM with a method call
        /// similar to suspend_handle_id = suspendRedraw(
        /// max_wait_milliseconds) and follow the changes with a method
        /// call similar to unsuspendRedraw(suspend_handle_id). Note
        /// that multiple suspendRedraw calls can be used at once and
        /// that each such method call is treated independently of the
        /// other suspendRedraw method calls.
        /// </remarks>
        /// <param name="max_wait_milliseconds">
        /// The amount of time in milliseconds to hold off before redrawing
        /// the device. Values greater than 60 seconds will be truncated down
        /// to 60 seconds.
        /// </param>
        /// <returns>
        /// A number which acts as a unique identifier for the given
        /// suspendRedraw() call. This value must be passed as the parameter
        /// to the corresponding unsuspendRedraw() method call.
        /// </returns>
        int SuspendRedraw(int max_wait_milliseconds);

        /// <summary>
        /// Cancels a specified suspendRedraw() by providing a unique
        /// suspend_handle_id.
        /// </summary>
        /// <param name="suspend_handle_id">
        /// A number which acts as a unique identifier for the desired
        /// suspendRedraw() call. The number supplied must be a value
        /// returned from a previous call to suspendRedraw()
        /// </param>
        void UnsuspendRedraw(int suspend_handle_id);

        /// <summary>
        /// Cancels all currently active suspendRedraw() method calls.
        /// been cancelled.
        /// </summary>
        /// <remarks>
        /// This method is most useful at the very end of a set of SVG
        /// DOM calls to ensure that all pending suspendRedraw() method
        /// calls have been cancelled.
        /// </remarks>
        void UnsuspendRedrawAll();

        /// <summary>
        /// In rendering environments supporting interactivity, forces the
        /// user agent to immediately redraw all regions of the viewport
        /// that require updating.
        /// </summary>
        void ForceRedraw();

        /// <summary>
        /// Suspends (i.e., pauses) all currently running animations that are
        /// defined within the SVG document fragment corresponding to this
        /// 'svg' element, causing the animation clock corresponding to this
        /// document fragment to stand still until it is unpaused.
        /// </summary>
        void PauseAnimations();

        /// <summary>
        /// Unsuspends (i.e., unpauses) currently running animations that are
        /// defined within the SVG document fragment, causing the animation
        /// clock to continue from the time at which it was suspended.
        /// </summary>
        void UnpauseAnimations();

        /// <summary>
        /// Returns true if this SVG document fragment is in a paused state.
        /// </summary>
        /// <returns>
        /// Boolean indicating whether this SVG document fragment is in a
        /// paused state.
        /// </returns>
        bool AnimationsPaused();

        /// <summary>
        /// The current time in seconds relative to the start time
        /// for the current SVG document fragment.
        /// </summary>
        float CurrentTime
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the list of graphics elements whose rendered content
        /// intersects the supplied rectangle, honoring the 'pointer-events'
        /// property value on each candidate graphics element.
        /// </summary>
        /// <param name="rect">
        /// The test rectangle. The values are in the initial coordinate
        /// system for the current 'svg' element.
        /// </param>
        /// <param name="referenceElement">
        /// If not null, then only return elements whose drawing order has
        /// them below the given reference element.
        /// </param>
        /// <returns>
        /// A list of Elements whose content intersects the supplied
        /// rectangle.
        /// </returns>
        XmlNodeList GetIntersectionList(ISvgRect rect, ISvgElement referenceElement);

        /// <summary>
        /// Returns the list of graphics elements whose rendered content is
        /// entirely contained within the supplied rectangle, honoring the
        /// 'pointer-events' property value on each candidate graphics
        /// element.
        /// </summary>
        /// <param name="rect">
        /// The test rectangle. The values are in the initial coordinate
        /// system for the current 'svg' element.
        /// </param>
        /// <param name="referenceElement">
        /// If not null, then only return elements whose drawing order has
        /// them below the given reference element.
        /// </param>
        /// <returns>
        /// A list of Elements whose content is enclosed by the supplied
        /// rectangle.
        /// </returns>
        XmlNodeList GetEnclosureList(ISvgRect rect, ISvgElement referenceElement);

        /// <summary>
        /// Returns true if the rendered content of the given element
        /// intersects the supplied rectangle, honoring the 'pointer-events'
        /// property value on each candidate graphics element.
        /// </summary>
        /// <param name="element">
        /// The element on which to perform the given test.
        /// </param>
        /// <param name="rect">
        /// The test rectangle. The values are in the initial coordinate
        /// system for the current 'svg' element.
        /// </param>
        /// <returns>
        /// True or false, depending on whether the given element intersects
        /// the supplied rectangle.
        /// </returns>
        bool CheckIntersection(ISvgElement element, ISvgRect rect);

        /// <summary>
        /// Returns true if the rendered content of the given element is
        /// entirely contained within the supplied rectangle, honoring the
        /// 'pointer-events' property value on each candidate graphics
        /// element.
        /// </summary>
        /// <param name="element">
        /// The element on which to perform the given test.
        /// </param>
        /// <param name="rect">
        /// The test rectangle. The values are in the initial coordinate
        /// system for the current 'svg' element.
        /// </param>
        /// <returns>
        /// True or false, depending on whether the given element is
        /// enclosed by the supplied rectangle.
        /// </returns>
        bool CheckEnclosure(ISvgElement element, ISvgRect rect);

        /// <summary>
        /// Unselects any selected objects, including any selections of text
        /// strings and type-in bars.
        /// </summary>
        void DeselectAll();

        /// <summary>
        /// Creates an SVGNumber object outside of any document trees. The
        /// object is initialized to a value of zero.
        /// </summary>
        /// <returns>
        /// An SVGNumber object.
        /// </returns>
        ISvgNumber CreateSvgNumber();

        /// <summary>
        /// Creates an SVGLength object outside of any document trees. The
        /// object is initialized to the value of 0 user units.
        /// </summary>
        /// <returns>
        /// An SVGLength object.
        /// </returns>
        ISvgLength CreateSvgLength();

        /// <summary>
        /// Creates an SVGAngle object outside of any document trees. The
        /// object is initialized to the value 0 degrees (unitless).
        /// </summary>
        /// <returns>
        /// An SVGAngle object.
        /// </returns>
        ISvgAngle CreateSvgAngle();

        /// <summary>
        /// Creates an SVGPoint object outside of any document trees. The
        /// object is initialized to the point (0,0) in the user coordinate
        /// system.
        /// </summary>
        /// <returns>
        /// An SVGPoint object.
        /// </returns>
        ISvgPoint CreateSvgPoint();

        /// <summary>
        /// Creates an SVGMatrix object outside of any document trees. The
        /// object is initialized to the identity matrix.
        /// </summary>
        /// <returns>
        /// An SVGMatrix object.
        /// </returns>
        ISvgMatrix CreateSvgMatrix();

        /// <summary>
        /// Creates an SVGRect object outside of any document trees. The
        /// object is initialized such that all values are set to 0 user
        /// units.
        /// </summary>
        /// <returns>
        /// An SVGRect object.
        /// </returns>
        ISvgRect CreateSvgRect();

        /// <summary>
        /// Creates an SVGTransform object outside of any document trees. The
        /// object is initialized to an identity matrix transform
        /// (SVG_TRANSFORM_MATRIX).
        /// </summary>
        /// <returns>
        /// An SVGTransform object.
        /// </returns>
        ISvgTransform CreateSvgTransform();

        /// <summary>
        /// Creates an SVGTransform object outside of any document trees.
        /// The object is initialized to the given matrix transform (i.e.,
        /// SVG_TRANSFORM_MATRIX).
        /// </summary>
        /// <param name="matrix">
        /// The transform matrix.
        /// </param>
        /// <returns>
        /// An SVGTransform object.
        /// </returns>
        ISvgTransform CreateSvgTransformFromMatrix(ISvgMatrix matrix);

        /// <summary>
        /// Searches this SVG document fragment (i.e., the search is
        /// restricted to a subset of the document tree) for an Element whose
        /// id is given by elementId.
        /// </summary>
        /// <remarks>
        /// If an Element is found, that Element is
        /// returned. If no such element exists, returns null. Behavior is
        /// not defined if more than one element has this id.
        /// </remarks>
        /// <param name="elementId">
        /// The unique id value for an element.
        /// </param>
        /// <returns>
        /// The matching element.
        /// </returns>
        XmlElement GetElementById(string elementId);
    }
}
