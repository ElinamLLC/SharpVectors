using System;

namespace SharpVectors.Dom.Events
{
    /// <summary>
    /// This interface represents an object that can handle an event dispatched by an <see cref="IEventTarget"/> object.
    /// </summary>
    public interface IEventListener
    {
        /// <summary>
        /// A function that is called whenever an event of the specified type occurs.
        /// </summary>
        void HandleEvent(IEvent evt);
    }
}
