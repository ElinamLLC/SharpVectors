using System;

using SharpVectors.Dom.Views;

namespace SharpVectors.Dom.Events
{
    /// <summary>
    /// This interface, defined in SMIL Animation: Supported interfaces, provides specific 
    /// contextual information associated with Time events.
    /// </summary>
    /// <remarks>
    /// <para>The different types of events that can occur are:</para>
    /// <para>
    /// </para>
    /// </remarks>
    public interface ITimeEvent : IEvent
    {
        /// <summary>
        /// The view attribute identifies the <see cref="IAbstractView"/> from which the event was generated.
        /// </summary>
        IAbstractView View { get; }

        /// <summary>
        /// Specifies some detail information about the Event, depending on the type of the event. 
        /// For this event type, indicates the repeat number for the animation.
        /// </summary>
        long Detail { get; }

        /// <summary>
        /// This method is used to initialize the value of a <see cref="ITimeEvent"/> created through 
        /// the <see cref="IDocumentEvent"/> interface. This method may only be called before the 
        /// <see cref="ITimeEvent"/> has been dispatched via the dispatchEvent method, though it may 
        /// be called multiple times during that phase if necessary. If called multiple times, the 
        /// final invocation takes precedence.
        /// </summary>
        /// <param name="typeArg">Specifies the event type.</param>
        /// <param name="viewArg">Specifies the event's <see cref="IAbstractView"/>.</param>
        /// <param name="detailArg">Specifies the event's detail.</param>
        void InitTimeEvent(string typeArg, IAbstractView viewArg, long detailArg);
    }
}
