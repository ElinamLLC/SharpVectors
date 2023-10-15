using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public delegate void RenderEvent(SvgRectF updatedRect);

    /// <summary>
    /// Defines the interface required by a renderer to render the SVG DOM.
    /// </summary>
    /// <remarks>
    /// The <see cref="ISvgRenderer">ISvgRenderer</see> is used to render
    /// a <see cref="ISvgElement">ISvgElement</see> object onto a bitmap.
    /// </remarks>
    public interface ISvgRenderer
    {
        /// <summary>
        /// The window that is being rendered to.
        /// </summary>
        ISvgWindow Window
        {
            get;
            set;
        }

        /// <summary>
        /// Renders an <see cref="ISvgElement">SvgElement</see> object onto a
        /// bitmap and returns that bitmap.
        /// </summary>
        /// <param name="node">
        /// The SvgElement object to be rendered.
        /// </param>
        /// <returns>
        /// A bitmap with <c>node</c> rendered onto it.
        /// </returns>
        void Render(ISvgElement node);

        /// <summary>
        /// Renders an <see cref="ISvgDocument">SvgDocument</see> object onto
        /// a bitmap and returns that bitmap.
        /// </summary>
        /// <param name="node">
        /// The SvgDocument object to be rendered.
        /// </param>
        /// <returns>
        /// A bitmap with <c>node</c> rendered onto it.
        /// </returns>
        void Render(ISvgDocument node);

        /// <summary>
        /// Controls the rendering of the document.  
        /// </summary>
        SvgRectF InvalidRect
        {
            get;
            set;
        }

        /// <summary>
        /// Allows you to establish or add to the existing invalidation rectangle
        /// </summary>
        /// <param name="rect"></param>
        void InvalidateRect(SvgRectF rect);

        /// <summary>
        /// Event Delegate to report when the SVG renderer does it's work.
        /// </summary>
        RenderEvent OnRender
        {
            get;
            set;
        }

        ISvgRect GetRenderedBounds(ISvgElement element, float margin);
    }
}
