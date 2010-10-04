using System;
using System.Xml;
using System.Timers;
using System.Collections.Generic;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// A key interface definition is the SVGSVGElement interface, which is the interface that corresponds to the 'svg' element. This interface contains various miscellaneous commonly-used utility methods, such as matrix operations and the ability to control the time of redraw on visual rendering devices.
    /// SVGSVGElement extends ViewCSS and DocumentCSS to provide access to the computed values of properties and the override style sheet as described in DOM2. 
    /// </summary>
    public sealed class SvgSvgElement : SvgTransformableElement, ISvgSvgElement
    {
        #region Private Fields

        private SvgTests svgTests;
        private ISvgAnimatedLength x;
        private ISvgAnimatedLength height;

        #endregion

        #region Constructors

        internal SvgSvgElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
            svgFitToViewBox = new SvgFitToViewBox(this);
            svgTests = new SvgTests(this);
            currentTranslate = new SvgPoint(0, 0);
        }

        #endregion

        #region Public Properties

        public bool IsOuterMost
        {
            get
            {
                return (this.ParentNode is SvgDocument);
            }
        }

        #endregion

        #region Public Methods

        public void Resize()
        {
            // TODO: Invalidate! Fire SVGResize
            x = null;
            y = null;
            width = null;
            height = null;
            currentView = null;
            cachedViewBoxTransform = null;
            viewport = null;
            svgFitToViewBox = null;
            svgFitToViewBox = new SvgFitToViewBox(this);
            if (this == OwnerDocument.RootElement)
            {
                // TODO
            }
            else
            {
                (OwnerDocument.RootElement as SvgSvgElement).Resize();
            }
        }

        #endregion

        #region ISvgElement Members

        /// <summary>
        /// Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        /// An enumeration of the <see cref="SvgRenderingHint"/> specifying the rendering hint.
        /// This will always return <see cref="SvgRenderingHint.Containment"/>
        /// </value>
        public override SvgRenderingHint RenderingHint
        {
            get
            {
                return SvgRenderingHint.Containment;
            }
        }

        #endregion

        #region ISvgZoomAndPan Members

        public SvgZoomAndPanType ZoomAndPan
        {
            get
            {
                return CurrentView.ZoomAndPan;
            }
            set
            {
            }
        }

        #endregion

        #region ISvgSvgElement Members

        /// <summary>
        /// Corresponds to attribute x on the given 'svg' element.
        /// </summary>
        public ISvgAnimatedLength X
        {
            get
            {
                if (x == null)
                {
                    x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "0px");
                }
                return x;
            }
        }

        private ISvgAnimatedLength y;
        /// <summary>
        /// Corresponds to attribute y on the given 'svg' element.
        /// </summary>
        public ISvgAnimatedLength Y
        {
            get
            {
                if (y == null)
                {
                    y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "0px");
                }
                return y;
            }
        }  

        private string widthAsString
        {
            get
            {
                SvgWindow ownerWindow = (SvgWindow)((SvgDocument)OwnerDocument).Window;
                if (ownerWindow.ParentWindow == null)
                {
                    return GetAttribute("width").Trim();
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        private ISvgAnimatedLength width;
        /// <summary>
        /// Corresponds to attribute width on the given 'svg' element.
        /// </summary>
        public ISvgAnimatedLength Width
        {
            get
            {
                if (width == null)
                {
                    width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Horizontal, widthAsString, "100%");
                }
                return width;
            }
        }     

        private string heightAsString
        {
            get
            {
                SvgWindow ownerWindow = (SvgWindow)((SvgDocument)OwnerDocument).Window;
                if (ownerWindow.ParentWindow == null)
                {
                    return GetAttribute("height").Trim();
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Corresponds to attribute height on the given 'svg' element.
        /// </summary>
        public ISvgAnimatedLength Height
        {
            get
            {
                if (height == null)
                {
                    height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Vertical, heightAsString, "100%");
                }
                return height;
            }
        }

        /// <summary>
        /// Corresponds to attribute contentScriptType on the given 'svg' element
        /// </summary>
        /// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the value of a readonly attribute.</exception>
        public string ContentScriptType
        {
            get
            {
                return GetAttribute("contentScriptType");
            }
            set
            {
                SetAttribute("contentScriptType", value);
            }
        }

        /// <summary>
        /// Corresponds to attribute contentStyleType on the given 'svg' element.
        /// </summary>
        /// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the value of a readonly attribute.</exception>
        public string ContentStyleType
        {
            get
            {
                return GetAttribute("contentStyleType");
            }
            set
            {
                SetAttribute("contentStyleType", value);
            }
        }    


        private double getViewportProp(string propertyName, string inValue, double calcParentVP, 
            double defaultValue, SvgLengthDirection dir)
        {
            double ret;
            inValue = inValue.Trim();

            if (inValue.Length > 0)
            {
                if (inValue.EndsWith("%"))
                {
                    double perc = SvgNumber.ParseNumber(inValue.Substring(0, inValue.Length - 1)) / 100;
                    ret = calcParentVP * perc;
                }
                else
                {
                    ret = new SvgLength(this, propertyName, dir, inValue, String.Empty).Value;
                }
            }
            else ret = defaultValue;

            return ret;
        }

        private ISvgRect viewport;
        /// <summary>
        /// The position and size of the viewport (implicit or explicit) that corresponds to this 'svg' element. When the user agent is actually rendering the content, then the position and size values represent the actual values when rendering. The position and size values are unitless values in the coordinate system of the parent element. If no parent element exists (i.e., 'svg' element represents the root of the document tree), if this SVG document is embedded as part of another document (e.g., via the HTML 'object' element), then the position and size are unitless values in the coordinate system of the parent document. (If the parent uses CSS or XSL layout, then unitless values represent pixel units for the current CSS or XSL viewport, as described in the CSS2 specification.) If the parent element does not have a coordinate system, then the user agent should provide reasonable default values for this attribute.
        /// The object itself and its contents are both readonly.
        /// </summary>
        public ISvgRect Viewport
        {
            get
            {
                if (viewport == null)
                {
                    double calcParentVPWidth = (ViewportElement == null) ?
                      OwnerDocument.Window.InnerWidth : ((ISvgFitToViewBox)ViewportElement).ViewBox.AnimVal.Width;

                    double calcParentVPHeight = (ViewportElement == null) ?
                      OwnerDocument.Window.InnerHeight : ((ISvgFitToViewBox)ViewportElement).ViewBox.AnimVal.Height;

                    double x = getViewportProp("x", GetAttribute("x"), calcParentVPWidth, 0, SvgLengthDirection.Horizontal);
                    double y = getViewportProp("y", GetAttribute("y"), calcParentVPHeight, 0, SvgLengthDirection.Vertical);
                    double width = getViewportProp("width", widthAsString, calcParentVPWidth, OwnerDocument.Window.InnerWidth, SvgLengthDirection.Horizontal);
                    double height = getViewportProp("height", heightAsString, calcParentVPHeight, OwnerDocument.Window.InnerHeight, SvgLengthDirection.Vertical);

                    viewport = new SvgRect(x, y, width, height);
                }
                return viewport;
            }
        }

        /// <summary>
        /// Size of a pixel units (as defined by CSS2) along the x-axis of the viewport, which represents a unit somewhere in the range of 70dpi to 120dpi, and, on systems that support this, might actually match the characteristics of the target medium. On systems where it is impossible to know the size of a pixel, a suitable default pixel size is provided.
        /// </summary>
        public float PixelUnitToMillimeterX
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Corresponding size of a pixel unit along the y-axis of the viewport.
        /// </summary>
        public float PixelUnitToMillimeterY
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// User interface (UI) events in DOM Level 2 indicate the screen positions at which the given UI event occurred. When the user agent actually knows the physical size of a "screen unit", this attribute will express that information; otherwise, user agents will provide a suitable default value such as .28mm.
        /// </summary>
        public float ScreenPixelToMillimeterX
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Corresponding size of a screen pixel along the y-axis of the viewport.
        /// </summary>
        public float ScreenPixelToMillimeterY
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The initial view (i.e., before magnification and panning) of the current innermost SVG 
        /// document fragment can be either the "standard" view (i.e., based on attributes on 
        /// the 'svg' element such as fitBoxToViewport) or to a "custom" view (i.e., a hyperlink 
        /// into a particular 'view' or other element - see Linking into SVG content: URI 
        /// fragments and SVG views). If the initial view is the "standard" view, then this 
        /// attribute is false. If the initial view is a "custom" view, then this attribute is 
        /// true.
        /// </summary>
        /// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the value of a readonly attribute.</exception>
        public bool UseCurrentView
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The definition of the initial view (i.e., before magnification and panning) of the current innermost SVG document fragment. The meaning depends on the situation:
        /// * If the initial view was a "standard" view, then:
        ///  o the values for viewBox, preserveAspectRatio and zoomAndPan within currentView will match the values for the corresponding DOM attributes that are on SVGSVGElement directly
        ///  o the values for transform and viewTarget within currentView will be null
        /// * If the initial view was a link into a 'view' element, then:
        ///  o the values for viewBox, preserveAspectRatio and zoomAndPan within currentView will correspond to the corresponding attributes for the given 'view' element
        ///  o the values for transform and viewTarget within currentView will be null
        /// * If the initial view was a link into another element (i.e., other than a 'view'), then:
        ///  o the values for viewBox, preserveAspectRatio and zoomAndPan within currentView will match the values for the corresponding DOM attributes that are on SVGSVGElement directly for the closest ancestor 'svg' element
        ///  o the values for transform within currentView will be null
        ///  o the viewTarget within currentView will represent the target of the link
        /// * If the initial view was a link into the SVG document fragment using an SVG view specification fragment identifier (i.e., #svgView(...)), then:
        ///  o the values for viewBox, preserveAspectRatio, zoomAndPan, transform and viewTarget within currentView will correspond to the values from the SVG view specification fragment identifier
        /// The object itself and its contents are both readonly. 
        /// </summary>
        private ISvgViewSpec currentView = null;
        public ISvgViewSpec CurrentView
        {
            get
            {
                if (currentView == null)
                    currentView = new SvgViewSpec(this) as ISvgViewSpec;
                // For now, we only return the "standard" view.
                return currentView;
            }
        }

        private float currentScale = 1;
        /// <summary>
        /// This attribute indicates the current scale factor relative to the initial view to take into account user magnification and panning operations, as described under Magnification and panning. DOM attributes currentScale and currentTranslate are equivalent to the 2x3 matrix [a b c d e f] = [currentScale 0 0 currentScale currentTranslate.x currentTranslate.y]. If "magnification" is enabled (i.e., zoomAndPan="magnify"), then the effect is as if an extra transformation were placed at the outermost level on the SVG document fragment (i.e., outside the outermost 'svg' element).
        /// </summary>
        /// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the value of a readonly attribute</exception>
        public float CurrentScale
        {
            get
            {
                if (this == OwnerDocument.RootElement)
                    return currentScale;
                else
                    return OwnerDocument.RootElement.CurrentScale;
            }
            set
            {
                if (this == OwnerDocument.RootElement)
                {
                    // TODO: Invalidate! Fire OnZoom
                    currentView = null;
                    currentScale = value;
                    cachedViewBoxTransform = null;
                    viewport = null;
                    width = null;
                    height = null;
                    x = null;
                    y = null;
                    svgFitToViewBox = new SvgFitToViewBox(this);
                }
                else
                    OwnerDocument.RootElement.CurrentScale = value;
            }
        }


        private ISvgMatrix cachedViewBoxTransform = null;
        /// <summary>
        /// This function is super useful, calculates out the transformation matrix 
        /// (i.e., scale and translate) of the viewport to user space.
        /// </summary>
        /// <returns>A Matrix which has the translate and scale portions set.</returns>
        public ISvgMatrix ViewBoxTransform
        {
            // TODO: This needs to be cached... need to handle changes to
            //   parent width or height or viewbox changes (in the case of percents)
            //   x,y,width,height,viewBox,preserveAspectRatio changes
            get
            {
                if (cachedViewBoxTransform == null)
                {
                    ISvgMatrix matrix = CreateSvgMatrix();

                    SvgDocument doc = (SvgDocument)OwnerDocument;
                    double x = 0;
                    double y = 0;
                    double w = 0;
                    double h = 0;

                    double attrWidth = Width.AnimVal.Value;
                    double attrHeight = Height.AnimVal.Value;
                    if (this != doc.RootElement)
                    {
                        // X and Y on the root <svg> have no meaning
                        matrix = matrix.Translate(X.AnimVal.Value, Y.AnimVal.Value);
                    }

                    // Apply the viewBox viewport
                    if (HasAttribute("viewBox"))
                    {
                        ISvgRect r = CurrentView.ViewBox.AnimVal;
                        x += -r.X;
                        y += -r.Y;
                        w = r.Width;
                        h = r.Height;
                        if (w < 0 || h < 0)
                            throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "Negative values are not permitted for viewbox width or height");
                    }
                    else
                    {
                        // This will result in a 1/1 scale.
                        w = attrWidth;
                        h = attrHeight;
                    }

                    double x_ratio = attrWidth / w;
                    double y_ratio = attrHeight / h;

                    ISvgPreserveAspectRatio par = CurrentView.PreserveAspectRatio.AnimVal;
                    if (par.Align == SvgPreserveAspectRatioType.None)
                    {
                        matrix = matrix.ScaleNonUniform(x_ratio, y_ratio);
                    }
                    else
                    {

                        // uniform scaling
                        if (par.MeetOrSlice == SvgMeetOrSlice.Meet)
                            x_ratio = Math.Min(x_ratio, y_ratio);
                        else
                            x_ratio = Math.Max(x_ratio, y_ratio);

                        double x_trans = 0;
                        double x_diff = attrWidth - (x_ratio * w);
                        double y_trans = 0;
                        double y_diff = attrHeight - (x_ratio * h);

                        if (par.Align == SvgPreserveAspectRatioType.XMidYMax ||
                          par.Align == SvgPreserveAspectRatioType.XMidYMid ||
                          par.Align == SvgPreserveAspectRatioType.XMidYMin)
                        {
                            // align to the Middle X
                            x_trans = x_diff / 2;
                        }
                        else if (par.Align == SvgPreserveAspectRatioType.XMaxYMax ||
                          par.Align == SvgPreserveAspectRatioType.XMaxYMid ||
                          par.Align == SvgPreserveAspectRatioType.XMaxYMin)
                        {
                            // align to the right X
                            x_trans = x_diff;
                        }

                        if (par.Align == SvgPreserveAspectRatioType.XMaxYMid ||
                          par.Align == SvgPreserveAspectRatioType.XMidYMid ||
                          par.Align == SvgPreserveAspectRatioType.XMinYMid)
                        {
                            // align to the middle Y
                            y_trans = y_diff / 2;
                        }
                        else if (par.Align == SvgPreserveAspectRatioType.XMaxYMax ||
                          par.Align == SvgPreserveAspectRatioType.XMidYMax ||
                          par.Align == SvgPreserveAspectRatioType.XMinYMax)
                        {
                            // align to the bottom Y
                            y_trans = y_diff;
                        }

                        matrix = matrix.Translate(x_trans, y_trans);
                        matrix = matrix.Scale(x_ratio);
                    }
                    // Translate for min-x and min-y
                    matrix = matrix.Translate(x, y);

                    // Handle currentSranslate and currentScale
                    if (this == OwnerDocument.RootElement)
                    {
                        matrix = matrix.Translate(this.currentTranslate.X, this.currentTranslate.Y);
                        matrix = matrix.Scale(this.currentScale);
                    }

                    // Set the cache
                    cachedViewBoxTransform = matrix;
                }
                return cachedViewBoxTransform;
            }
        }


        private ISvgPoint currentTranslate;
        /// <summary>
        /// The corresponding translation factor that takes into account user "magnification".
        /// </summary>
        public ISvgPoint CurrentTranslate
        {
            get
            {
                if (this == OwnerDocument.RootElement)
                {
                    if (currentTranslate == null)
                    {
                        currentTranslate = CreateSvgPoint();
                    }
                    return currentTranslate;
                }
                else
                    return OwnerDocument.RootElement.CurrentTranslate;
            }
        }

        private List<Timer> redrawTimers = new List<Timer>();

        public void RedrawTimerElapsed(object source, ElapsedEventArgs args)
        {
            UnsuspendRedraw(((Timer)source).GetHashCode());
        }

        /// <summary>
        /// Takes a time-out value which indicates that redraw shall not occur until: (a) the 
        /// corresponding unsuspendRedraw(suspend_handle_id) call has been made, (b) an 
        /// unsuspendRedrawAll() call has been made, or (c) its timer has timed out. In 
        /// environments that do not support interactivity (e.g., print media), then redraw shall 
        /// not be suspended. suspend_handle_id = suspendRedraw(max_wait_milliseconds) and 
        /// unsuspendRedraw(suspend_handle_id) must be packaged as balanced pairs. When you 
        /// want to suspend redraw actions as a collection of SVG DOM changes occur, then 
        /// precede the changes to the SVG DOM with a method call similar to 
        /// suspend_handle_id = suspendRedraw(max_wait_milliseconds) and follow the changes with 
        /// a method call similar to unsuspendRedraw(suspend_handle_id). Note that multiple 
        /// suspendRedraw calls can be used at once and that each such method call is treated
        /// independently of the other suspendRedraw method calls.
        /// </summary>
        /// <param name="max_wait_milliseconds">The amount of time in milliseconds to hold off 
        /// before redrawing the device. Values greater than 60 seconds will be truncated 
        /// down to 60 seconds.</param>
        /// <returns>A number which acts as a unique identifier for the given suspendRedraw() call. This value must be passed as the parameter to the corresponding unsuspendRedraw() method call.</returns>
        public int SuspendRedraw(int maxWaitMilliseconds)
        {
            if (maxWaitMilliseconds > 60000)
                maxWaitMilliseconds = 60000;
            Timer t = new Timer(maxWaitMilliseconds);
            t.AutoReset = false;
            t.Elapsed += new ElapsedEventHandler(this.RedrawTimerElapsed);
            t.Enabled = true;
            redrawTimers.Add(t);
            return t.GetHashCode();
        }

        /// <summary>
        /// Cancels a specified suspendRedraw() by providing a unique suspend_handle_id.
        /// </summary>
        /// <param name="suspend_handle_id">A number which acts as a unique identifier for the desired suspendRedraw() call. The number supplied must be a value returned from a previous call to suspendRedraw()</param>
        /// <exception cref="DomException">This method will raise a DOMException with value NOT_FOUND_ERR if an invalid value (i.e., no such suspend_handle_id is active) for suspend_handle_id is provided.</exception>
        public void UnsuspendRedraw(int suspendHandleId)
        {
            Timer timer = null;
            foreach (Timer t in redrawTimers)
            {
                if (t.GetHashCode() == suspendHandleId)
                {
                    timer = t;
                    break;
                }
            }
            if (timer == null)
                throw new DomException(DomExceptionType.NotFoundErr, "Invalid handle submitted to unsuspendRedraw");

            timer.Enabled = false;
            redrawTimers.Remove(timer);
            if (OwnerDocument.Window.Renderer.InvalidRect != SvgRectF.Empty)
                OwnerDocument.Window.Renderer.Render((ISvgDocument)OwnerDocument);

        }

        /// <summary>
        /// Cancels all currently active suspendRedraw() method calls. This method is most 
        /// useful 
        /// at the very end of a set of SVG DOM calls to ensure that all pending suspendRedraw() 
        /// method calls have been cancelled.
        /// </summary>
        public void UnsuspendRedrawAll()
        {
            foreach (Timer t in redrawTimers)
            {
                t.Enabled = false;
            }
            redrawTimers.Clear();
            if (OwnerDocument.Window.Renderer.InvalidRect != SvgRectF.Empty)
                OwnerDocument.Window.Renderer.Render((ISvgDocument)OwnerDocument);
        }

        /// <summary>
        /// In rendering environments supporting interactivity, forces the user agent to 
        /// immediately redraw all regions of the viewport that require updating.
        /// </summary>
        public void ForceRedraw()
        {
            OwnerDocument.Window.Renderer.InvalidRect = SvgRectF.Empty;
            OwnerDocument.Window.Renderer.Render((ISvgDocument)OwnerDocument);
        }

        /// <summary>
        /// Suspends (i.e., pauses) all currently running animations that are defined within the 
        /// SVG document fragment corresponding to this 'svg' element, causing the animation clock 
        /// corresponding to this document fragment to stand still until it is unpaused.
        /// </summary>
        public void PauseAnimations()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unsuspends (i.e., unpauses) currently running animations that are defined within the 
        /// SVG document fragment, causing the animation clock to continue from the time at which 
        /// it was suspended.
        /// </summary>
        public void UnpauseAnimations()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if this SVG document fragment is in a paused state
        /// </summary>
        /// <returns>Boolean indicating whether this SVG document fragment is in a paused 
        /// state.</returns>
        public bool AnimationsPaused()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// The current time in seconds relative to the start time for the current SVG document 
        /// fragment.
        /// </summary>
        public float CurrentTime
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns the list of graphics elements whose rendered content intersects the supplied 
        /// rectangle, honoring the 'pointer-events' property value on each candidate graphics 
        /// element.
        /// </summary>
        /// <param name="rect">The test rectangle. The values are in the initial coordinate 
        /// system for the current 'svg' element.</param>
        /// <param name="referenceElement">If not null, then only return elements whose drawing 
        /// order has them below the given reference element.</param>
        /// <returns>A list of Elements whose content intersects the supplied rectangle.</returns>
        public XmlNodeList GetIntersectionList(ISvgRect rect, ISvgElement referenceElement)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the list of graphics elements whose rendered content is entirely contained 
        /// within the supplied rectangle, honoring the 'pointer-events' property value on each 
        /// candidate graphics element.
        /// </summary>
        /// <param name="rect">The test rectangle. The values are in the initial coordinate system 
        /// for the current 'svg' element.</param>
        /// <param name="referenceElement">If not null, then only return elements whose drawing 
        /// order has them below the given reference element.</param>
        /// <returns>A list of Elements whose content is enclosed by the supplied 
        /// rectangle.</returns>
        public XmlNodeList GetEnclosureList(ISvgRect rect, ISvgElement referenceElement)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the rendered content of the given element intersects the supplied 
        /// rectangle, honoring the 'pointer-events' property value on each candidate graphics 
        /// element.
        /// </summary>
        /// <param name="element">The element on which to perform the given test.</param>
        /// <param name="rect">The test rectangle. The values are in the initial coordinate system 
        /// for the current 'svg' element.</param>
        /// <returns>True or false, depending on whether the given element intersects the supplied 
        /// rectangle.</returns>
        public bool CheckIntersection(ISvgElement element, ISvgRect rect)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the rendered content of the given element is entirely contained within 
        /// the supplied rectangle, honoring the 'pointer-events' property value on each candidate 
        /// graphics element.
        /// </summary>
        /// <param name="element">The element on which to perform the given test</param>
        /// <param name="rect">The test rectangle. The values are in the initial coordinate system 
        /// for the current 'svg' element.</param>
        /// <returns>True or false, depending on whether the given element is enclosed by the 
        /// supplied rectangle.</returns>
        public bool CheckEnclosure(ISvgElement element, ISvgRect rect)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unselects any selected objects, including any selections of text strings and type-in 
        /// bars.
        /// </summary>
        public void DeselectAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an SVGNumber object outside of any document trees. The object is initialized 
        /// to a value of zero.
        /// </summary>
        /// <returns>An SVGNumber object.</returns>
        public ISvgNumber CreateSvgNumber()
        {
            return new SvgNumber(0F);
        }

        /// <summary>
        /// Creates an SVGLength object outside of any document trees. The object is initialized 
        /// to the value of 0 user units.
        /// </summary>
        /// <returns>An SVGLength object.</returns>
        public ISvgLength CreateSvgLength()
        {
            return new SvgLength(null, string.Empty, SvgLengthSource.String, SvgLengthDirection.Horizontal, "0");
        }

        /// <summary>
        /// Creates an SVGAngle object outside of any document trees. The object is initialized to 
        /// the value 0 degrees (unitless).
        /// </summary>
        /// <returns>An SVGAngle object.</returns>
        public ISvgAngle CreateSvgAngle()
        {
            return new SvgAngle("0", "0", false);
        }

        /// <summary>
        /// Creates an SVGPoint object outside of any document trees. The object is initialized to 
        /// the point (0,0) in the user coordinate system.
        /// </summary>
        /// <returns>An SVGPoint object.</returns>
        public ISvgPoint CreateSvgPoint()
        {
            return new SvgPoint(0, 0);
        }

        /// <summary>
        /// Creates an SVGMatrix object outside of any document trees. The object is initialized 
        /// to the identity matrix.
        /// </summary>
        /// <returns>An SVGMatrix object.</returns>
        public ISvgMatrix CreateSvgMatrix()
        {
            return new SvgMatrix();
        }

        /// <summary>
        /// Creates an SVGRect object outside of any document trees. The object is initialized 
        /// such that all values are set to 0 user units.
        /// </summary>
        /// <returns>An SVGRect object.</returns>
        public ISvgRect CreateSvgRect()
        {
            return new SvgRect(0, 0, 0, 0);
        }

        /// <summary>
        /// Creates an SVGTransform object outside of any document trees. The object is initialized
        /// to an identity matrix transform (SVG_TRANSFORM_MATRIX).
        /// </summary>
        /// <returns>An SVGTransform object.</returns>
        public ISvgTransform CreateSvgTransform()
        {
            return new SvgTransform();
        }

        /// <summary>
        /// Creates an SVGTransform object outside of any document trees. The object is 
        /// initialized to the given matrix transform (i.e., SVG_TRANSFORM_MATRIX).
        /// </summary>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>An SVGTransform object.</returns>
        public ISvgTransform CreateSvgTransformFromMatrix(ISvgMatrix matrix)
        {
            return new SvgTransform(matrix);
        }

        /// <summary>
        /// Searches this SVG document fragment (i.e., the search is restricted to a subset of the 
        /// document tree) for an Element whose id is given by elementId. If an Element is found, 
        /// that Element is returned. If no such element exists, returns null. Behavior is not 
        /// defined if more than one element has this id.
        /// </summary>
        /// <param name="elementId">The unique id value for an element.</param>
        /// <returns>The matching element.</returns>
        public XmlElement GetElementById(string elementId)
        {
            return this.GetElementById(elementId);
        }

        #endregion

        #region ISvgFitToViewBox Members
        private SvgFitToViewBox svgFitToViewBox;
        public ISvgAnimatedRect ViewBox
        {
            get
            {
                return svgFitToViewBox.ViewBox;
            }
        }

        public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio
        {
            get
            {
                return svgFitToViewBox.PreserveAspectRatio;
            }
        }
        #endregion

        #region ISvgExternalResourcesRequired Members

        private SvgExternalResourcesRequired svgExternalResourcesRequired;
        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get
            {
                return svgExternalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion

        #region ISvgTests Members

        public ISvgStringList RequiredFeatures
        {
            get { return svgTests.RequiredFeatures; }
        }

        public ISvgStringList RequiredExtensions
        {
            get { return svgTests.RequiredExtensions; }
        }

        public ISvgStringList SystemLanguage
        {
            get { return svgTests.SystemLanguage; }
        }

        public bool HasExtension(string extension)
        {
            return svgTests.HasExtension(extension);
        }

        #endregion

        #region Update handling

        public override void HandleAttributeChange(XmlAttribute attribute)
        {
            if (attribute.NamespaceURI.Length == 0)
            {
                switch (attribute.LocalName)
                {
                    case "x":
                    case "y":
                    case "width":
                    case "height":
                    case "viewBox":
                    case "preserveAspectRatio":
                        Resize();
                        break;
                }

                base.HandleAttributeChange(attribute);
            }
        }

        #endregion
    }
}
