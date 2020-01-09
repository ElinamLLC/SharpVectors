using System;

using SharpVectors.Dom.Events;

namespace SharpVectors.Scripting
{         
	/// <summary>
	/// IJsEventTarget
	/// </summary>
	public interface IJsEventTarget : IScriptableObject<IEventTarget>
	{
		void addEventListener(string type, object listener, bool useCapture);
		void removeEventListener(string type, object listener, bool useCapture);
		bool dispatchEvent(IJsEvent evt);
	}

	/// <summary>
	/// IJsEventListener
	/// </summary>
	public interface IJsEventListener : IScriptableObject<IEventListener>
    {
		void handleEvent(IJsEvent evt);
	}

	/// <summary>
	/// IJsEvent
	/// </summary>
	public interface IJsEvent : IScriptableObject<IEvent>
    {
		string type { get; }
		IJsEventTarget target { get; }
		IJsEventTarget currentTarget { get; }
		ushort eventPhase { get; }
		bool bubbles { get; }
		bool cancelable { get; }
		IJsDomTimeStamp timeStamp { get; }

		void stopPropagation();
		void preventDefault();
		void initEvent(string eventTypeArg, bool canBubbleArg, bool cancelableArg);
	}

	/// <summary>
	/// IJsDocumentEvent
	/// </summary>
	public interface IJsDocumentEvent : IScriptableObject<IDocumentEvent>
    {
		IJsEvent createEvent(string eventType);
	}

	/// <summary>
	/// IJsUiEvent
	/// </summary>
	public interface IJsUiEvent : IJsEvent
	{
		IJsAbstractView view { get; }
		long detail { get; }

		void initUIEvent(string typeArg, bool canBubbleArg, bool cancelableArg, 
            IJsAbstractView viewArg, long detailArg);
	}

	/// <summary>
	/// IJsMouseEvent
	/// </summary>
	public interface IJsMouseEvent : IJsUiEvent
	{
		long screenX { get; }
		long screenY { get; }
		long clientX { get; }
		long clientY { get; }
		bool ctrlKey { get; }
		bool shiftKey { get; }
		bool altKey { get; }
		bool metaKey { get; }
		ushort button { get; }

		IJsEventTarget relatedTarget { get; }

		void initMouseEvent(string typeArg, bool canBubbleArg, bool cancelableArg, 
            IJsAbstractView viewArg, long detailArg, long screenXArg, long screenYArg, 
            long clientXArg, long clientYArg, bool ctrlKeyArg, bool altKeyArg, bool shiftKeyArg, 
            bool metaKeyArg, ushort buttonArg, IJsEventTarget relatedTargetArg);
	}

	/// <summary>
	/// IJsMutationEvent
	/// </summary>
	public interface IJsMutationEvent : IJsEvent
	{
		IJsNode relatedNode { get; }
		string prevValue { get; }
		string newValue { get; }
		string attrName { get; }
		ushort attrChange { get; }

		void initMutationEvent(string typeArg, bool canBubbleArg, bool cancelableArg, 
            IJsNode relatedNodeArg, string prevValueArg, string newValueArg, 
            string attrNameArg, ushort attrChangeArg);
	}
}
  