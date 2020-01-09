using System;

using SharpVectors.Dom;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Events;

namespace SharpVectors.Scripting
{
    #region Implementation - IJsSvgAnimationElement

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimationElement
    /// </summary>
    public class JsSvgAnimationElement : JsSvgElement, IJsSvgAnimationElement
    {
        public JsSvgAnimationElement(ISvgAnimationElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public float getStartTime()
        {
            return ((ISvgAnimationElement)_baseObject).StartTime;
        }

        public float getCurrentTime()
        {
            return ((ISvgAnimationElement)_baseObject).CurrentTime;
        }

        public float getSimpleDuration()
        {
            return ((ISvgAnimationElement)_baseObject).SimpleDuration;
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public void beginElement()
        {
            ((IElementTimeControl)_baseObject).BeginElement();
        }

        public void beginElementAt(float offset)
        {
            ((IElementTimeControl)_baseObject).BeginElementAt(offset);
        }

        public void endElement()
        {
            ((IElementTimeControl)_baseObject).EndElement();
        }

        public void endElementAt(float offset)
        {
            ((IElementTimeControl)_baseObject).EndElementAt(offset);
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
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgElement targetElement
        {
            get {
                var wrappedValue = ((ISvgAnimationElement)_baseObject).TargetElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get {
                return (ISvgTests)this.BaseObject;
            }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get {
                return (ISvgExternalResourcesRequired)this.BaseObject;
            }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get {
                return (IEventTarget)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimateElement

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimateElement
    /// </summary>
    public sealed class JsSvgAnimateElement : JsSvgAnimationElement, IJsSvgAnimateElement
    {
        public JsSvgAnimateElement(ISvgAnimateElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgSetElement

    /// <summary>
    /// Implementation wrapper for IJsSvgSetElement
    /// </summary>
    public sealed class JsSvgSetElement : JsSvgAnimationElement, IJsSvgSetElement
    {
        public JsSvgSetElement(ISvgAnimateSetElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimateMotionElement

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimateMotionElement
    /// </summary>
    public sealed class JsSvgAnimateMotionElement : JsSvgAnimationElement, IJsSvgAnimateMotionElement
    {
        public JsSvgAnimateMotionElement(ISvgAnimateMotionElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgMPathElement

    /// <summary>
    /// Implementation wrapper for IJsSvgMPathElement
    /// </summary>
    public sealed class JsSvgMPathElement : JsSvgElement, IJsSvgMPathElement
    {
        public JsSvgMPathElement(ISvgAnimateMPathElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgAnimatedString href
        {
            get {
                var wrappedValue = ((ISvgUriReference)_baseObject).Href;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        ISvgUriReference IScriptableObject<ISvgUriReference>.BaseObject
        {
            get {
                return (ISvgUriReference)this.BaseObject;
            }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get {
                return (ISvgExternalResourcesRequired)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimateColorElement

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimateColorElement
    /// </summary>
    public sealed class JsSvgAnimateColorElement : JsSvgAnimationElement, IJsSvgAnimateColorElement
    {
        public JsSvgAnimateColorElement(ISvgAnimateColorElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimateTransformElement

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimateTransformElement
    /// </summary>
    public sealed class JsSvgAnimateTransformElement : JsSvgAnimationElement, IJsSvgAnimateTransformElement
    {
        public JsSvgAnimateTransformElement(ISvgAnimateTransformElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion
}
