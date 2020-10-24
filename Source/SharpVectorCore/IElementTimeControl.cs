using System;

namespace SharpVectors.Dom
{
    /// <summary>
    /// An <c>SMIL</c> animation interface to supports several methods for controlling the behavior of animation.
    /// </summary>
    public interface IElementTimeControl
    {
        /// <summary>
        /// Creates a begin instance time for the current time. The new instance time is added 
        /// to the begin instance times list. 
        /// <para>
        /// The behavior of this method is equivalent to BeginElementAt(0).
        /// </para>
        /// </summary>
        void BeginElement();

        /// <summary>
        /// Creates a begin instance time for the current time plus the specified offset. 
        /// The new instance time is added to the begin instance times list.
        /// </summary>
        /// <param name="offset">
        /// The offset from the current document time, in seconds, at which to begin the element.
        /// </param>
        void BeginElementAt(float offset);

        /// <summary>
        /// Creates an end instance time for the current time. The new instance time is added 
        /// to the end instance times list. 
        /// <para>
        /// The behavior of this method is equivalent to EndElementAt(0).
        /// </para>
        /// </summary>
        void EndElement();

        /// <summary>
        /// Creates a end instance time for the current time plus the specified offset. 
        /// The new instance time is added to the end instance times list.
        /// </summary>
        /// <param name="offset">
        /// The offset from the current document time, in seconds, at which to end the element.
        /// </param>
        void EndElementAt(float offset);
    }
}
