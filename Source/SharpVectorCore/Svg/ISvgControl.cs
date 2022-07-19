using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This defines the interface for the control object that is controlling the rendering process and 
    /// displaying the rendered drawings.
    /// </summary>
    public interface ISvgControl
    {
        /// <summary>
        /// Gets the width of the <c>SVG</c> viewer control.
        /// </summary>
        /// <value>A specifying specifying the width of the control in pixels.</value>
        int Width { get; }

        /// <summary>
        /// Gets the height of the <c>SVG</c> viewer control.
        /// </summary>
        /// <value>A specifying specifying the height of the control in pixels.</value>
        int Height { get; }

        /// <summary>
        /// Gets a value specifying whether the viewer control is in design-mode.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the viewer control is in design-mode, otherwise; it is <see langword="false"/>.
        /// </value>
        bool DesignMode { get; }

        /// <summary>
        /// This signals the viewer control to display an alert message to the user.
        /// </summary>
        /// <param name="message">A <see cref="string"/> containing the alert message to be displayed.</param>
        void HandleAlert(string message);

        /// <summary>
        /// This signals the viewer control to display an error message to the user.
        /// </summary>
        /// <param name="message">A <see cref="string"/> containing the error message to be displayed.</param>
        void HandleError(string message);

        /// <summary>
        /// This signals the viewer control to display an error message due to an exception to the user.
        /// </summary>
        /// <param name="Exception">An <see cref="Exception"/> specifying the error to be displayed.</param>
        void HandleError(Exception exception);

        /// <summary>
        /// This signals the viewer control to display an error message due to an exception with additional message to the user.
        /// </summary>
        /// <param name="message">A <see cref="string"/> containing the additional message to be displayed.</param>
        /// <param name="Exception">An <see cref="Exception"/> specifying the error to be displayed.</param>
        void HandleError(string message, Exception exception);
    }
}
