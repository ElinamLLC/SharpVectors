using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// An integer indicating which phase of the event flow is being processed
	/// as defined in
	/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/events.html#Events-flow">DOM event flow</see>.
	/// </summary>
	public enum EventPhase
	{
		/// <summary>
		/// (DOM Level 2)
		/// The current event phase is the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-capture-phase">capture phase</see>.
		/// </summary>
		CapturingPhase = 1,
		
		/// <summary>
		/// (DOM Level 2)
		/// The current event is in the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-capture-phase">target phase</see>,
		/// i.e. it is being evaluated at the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-event-target">event target</see>.
		/// </summary>
		AtTarget = 2,
		
		/// <summary>
		/// (DOM Level 2)
		/// The current event phase is the
		/// <see href="http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/glossary.html#dt-bubbling-phase">bubbling phase</see>.
		/// </summary>
		BubblingPhase = 3
	}
}
