using System;

using SharpVectors.Dom.Views;     

namespace SharpVectors.Scripting
{               
    /// <summary>
    /// Implementation wrapper for IJsAbstractView
    /// </summary>
    public class JsAbstractView : JsObject<IAbstractView>, IJsAbstractView
    {
        public JsAbstractView(IAbstractView baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsDocumentView document
        {
            get {
                var result = _baseObject.Document;
                return (result != null) ? CreateWrapper<IJsDocumentView>(result, _engine) : null;
            }
        }
    }

    /// <summary>
    /// Implementation wrapper for IJsDocumentView
    /// </summary>
    public class JsDocumentView : JsObject<IDocumentView>, IJsDocumentView
    {
        public JsDocumentView(IDocumentView baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsAbstractView defaultView
        {
            get {
                var result = _baseObject.DefaultView;
                return (result != null) ? CreateWrapper<IJsAbstractView>(result, _engine) : null;
            }
        }
    }   
}

