using System;

using SharpVectors.Dom.Events;

namespace SharpVectors.Scripting
{
    #region Implementation - IJsEventTarget

    /// <summary>
    /// Implementation wrapper for IJsEventTarget
    /// </summary>
    public class JsEventTarget : JsObject<IEventTarget>, IJsEventTarget
    {
        public JsEventTarget(IEventTarget baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return _baseObject.DispatchEvent(evt.BaseObject);
        }
    }

    #endregion

    #region Implementation - IJsEventListener

    /// <summary>
    /// Implementation wrapper for IJsEventListener
    /// </summary>
    public sealed class JsEventListener : JsObject<IEventListener>, IJsEventListener
    {
        public JsEventListener(IEventListener baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public void handleEvent(IJsEvent evt)
        {
            _baseObject.HandleEvent(evt.BaseObject);
        }
    }

    #endregion

    #region Implementation - IJsEvent

    /// <summary>
    /// Implementation wrapper for IJsEvent
    /// </summary>
    public class JsEvent : JsObject<IEvent>, IJsEvent
    {
        public JsEvent(IEvent baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public void stopPropagation()
        {
            _baseObject.StopPropagation();
        }

        public void preventDefault()
        {
            _baseObject.PreventDefault();
        }

        public void initEvent(string eventTypeArg, bool canBubbleArg, bool cancelableArg)
        {
            _baseObject.InitEvent(eventTypeArg, canBubbleArg, cancelableArg);
        }

        public string type
        {
            get { return _baseObject.Type; }
        }

        public IJsEventTarget target
        {
            get {
                var result = _baseObject.Target;
                return (result != null) ? CreateWrapper<IJsEventTarget>(result, _engine) : null;
            }
        }

        public IJsEventTarget currentTarget
        {
            get {
                var result = _baseObject.CurrentTarget;
                return (result != null) ? CreateWrapper<IJsEventTarget>(result, _engine) : null;
            }
        }

        public ushort eventPhase
        {
            get { return (ushort)_baseObject.EventPhase; }
        }

        public bool bubbles
        {
            get { return _baseObject.Bubbles; }
        }

        public bool cancelable
        {
            get { return _baseObject.Cancelable; }
        }

        public IJsDomTimeStamp timeStamp
        {
            get {
                var result = _baseObject.TimeStamp;
                return CreateWrapper<IJsDomTimeStamp>(result, _engine);
            }
        }
    }

    #endregion

    #region Implementation - IJsDocumentEvent

    /// <summary>
    /// Implementation wrapper for IJsDocumentEvent
    /// </summary>
    public sealed class JsDocumentEvent : JsObject<IDocumentEvent>, IJsDocumentEvent
    {
        public JsDocumentEvent(IDocumentEvent baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsEvent createEvent(string eventType)
        {
            var result = _baseObject.CreateEvent(eventType);
            return (result != null) ? CreateWrapper<IJsEvent>(result, _engine) : null;
        }
    }

    #endregion

    #region Implementation - IJsUiEvent

    /// <summary>
    /// Implementation wrapper for IJsUiEvent
    /// </summary>
    public class JsUiEvent : JsEvent, IJsUiEvent
    {
        public JsUiEvent(IUiEvent baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public void initUIEvent(string typeArg, bool canBubbleArg, bool cancelableArg, IJsAbstractView viewArg, long detailArg)
        {
            ((IUiEvent)_baseObject).InitUiEvent(typeArg, canBubbleArg, cancelableArg, viewArg.BaseObject, detailArg);
        }

        public IJsAbstractView view
        {
            get {
                var result = ((IUiEvent)_baseObject).View;
                return (result != null) ? CreateWrapper<IJsAbstractView>(result, _engine) : null;
            }
        }

        public long detail
        {
            get { return ((IUiEvent)_baseObject).Detail; }
        }
    }

    #endregion

    #region Implementation - IJsMouseEvent

    /// <summary>
    /// Implementation wrapper for IJsMouseEvent
    /// </summary>
    public sealed class JsMouseEvent : JsUiEvent, IJsMouseEvent
    {
        public JsMouseEvent(IMouseEvent baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public void initMouseEvent(string typeArg, bool canBubbleArg, bool cancelableArg, 
            IJsAbstractView viewArg, long detailArg, long screenXArg, long screenYArg, 
            long clientXArg, long clientYArg, bool ctrlKeyArg, bool altKeyArg, bool shiftKeyArg, 
            bool metaKeyArg, ushort buttonArg, IJsEventTarget relatedTargetArg)
        {
            var mouseEvent = (IMouseEvent)_baseObject;
            mouseEvent.InitMouseEvent(typeArg, canBubbleArg, cancelableArg, viewArg.BaseObject, detailArg, 
                screenXArg, screenYArg, clientXArg, clientYArg, ctrlKeyArg, altKeyArg, shiftKeyArg, metaKeyArg, 
                buttonArg, relatedTargetArg.BaseObject);
        }

        public long screenX
        {
            get { return ((IMouseEvent)_baseObject).ScreenX; }
        }

        public long screenY
        {
            get { return ((IMouseEvent)_baseObject).ScreenY; }
        }

        public long clientX
        {
            get { return ((IMouseEvent)_baseObject).ClientX; }
        }

        public long clientY
        {
            get { return ((IMouseEvent)_baseObject).ClientY; }
        }

        public bool ctrlKey
        {
            get { return ((IMouseEvent)_baseObject).CtrlKey; }
        }

        public bool shiftKey
        {
            get { return ((IMouseEvent)_baseObject).ShiftKey; }
        }

        public bool altKey
        {
            get { return ((IMouseEvent)_baseObject).AltKey; }
        }

        public bool metaKey
        {
            get { return ((IMouseEvent)_baseObject).MetaKey; }
        }

        public ushort button
        {
            get { return ((IMouseEvent)_baseObject).Button; }
        }

        public IJsEventTarget relatedTarget
        {
            get {
                var result = ((IMouseEvent)_baseObject).RelatedTarget;
                return (result != null) ? CreateWrapper<IJsEventTarget>(result, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsMutationEvent

    /// <summary>
    /// Implementation wrapper for IJsMutationEvent
    /// </summary>
    public sealed class JsMutationEvent : JsEvent, IJsMutationEvent
    {
        public JsMutationEvent(IMutationEvent baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public void initMutationEvent(string typeArg, bool canBubbleArg, bool cancelableArg, IJsNode relatedNodeArg, 
            string prevValueArg, string newValueArg, string attrNameArg, ushort attrChangeArg)
        {
            var mutationEvent = (IMutationEvent)_baseObject;
            mutationEvent.InitMutationEvent(typeArg, canBubbleArg, cancelableArg, relatedNodeArg.BaseObject, 
                prevValueArg, newValueArg, attrNameArg, (AttrChangeType)attrChangeArg);
        }

        public IJsNode relatedNode
        {
            get {
                var result = ((IMutationEvent)_baseObject).RelatedNode;
                return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
            }
        }

        public string prevValue
        {
            get {
                return ((IMutationEvent)_baseObject).PrevValue;
            }
        }

        public string newValue
        {
            get {
                return ((IMutationEvent)_baseObject).NewValue;
            }
        }

        public string attrName
        {
            get {
                return ((IMutationEvent)_baseObject).AttrName;
            }
        }

        public ushort attrChange
        {
            get {
                return (ushort)((IMutationEvent)_baseObject).AttrChange;
            }
        }
    }

    #endregion
}

