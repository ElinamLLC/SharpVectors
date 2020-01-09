using System;

using SharpVectors.Dom.Events;

namespace SharpVectors.Scripting
{     
    ///// <summary>
    ///// Implementation wrapper for IJsElementTimeControl
    ///// </summary>
    //public class JsElementTimeControl : JsObject<IElementTimeControl>, IJsElementTimeControl
    //{
    //    public JsElementTimeControl(object baseObject, ISvgScriptEngine engine) : base(baseObject) { }

    //    #region Methods - IJsElementTimeControl
    //    public void beginElement()
    //    {
    //        throw new NotImplementedException(); //((IElementTimeControl)baseObject).BeginElement();
    //    }

    //    public void beginElementAt(float offset)
    //    {
    //        throw new NotImplementedException(); //((IElementTimeControl)baseObject).BeginElementAt(offset);
    //    }

    //    public void endElement()
    //    {
    //        throw new NotImplementedException(); //((IElementTimeControl)baseObject).EndElement();
    //    }

    //    public void endElementAt(float offset)
    //    {
    //        throw new NotImplementedException(); //((IElementTimeControl)baseObject).EndElementAt(offset);
    //    }
    //    #endregion
    //}

    /// <summary>
    /// Implementation wrapper for IJsTimeEvent
    /// </summary>
    public class JsTimeEvent : JsEvent, IJsTimeEvent
    {
        public JsTimeEvent(ITimeEvent baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        #region Methods - IJsTimeEvent
        public void initTimeEvent(string typeArg, IJsAbstractView viewArg, long detailArg)
        {
            throw new NotImplementedException(); //((ITimeEvent)baseObject).InitTimeEvent(typeArg, ((IAbstractView)((JsAbstractView)viewArg).baseObject), detailArg);
        }
        #endregion

        #region Properties - IJsTimeEvent
        public IJsAbstractView view
        {
            get { throw new NotImplementedException(); }//object result = ((ITimeEvent)baseObject).View; return (result != null) ? (IJsAbstractView)JsObject.CreateWrapper(result) : null; }
        }

        public long detail
        {
            get { throw new NotImplementedException(); }//return ((ITimeEvent)baseObject).Detail;  }
        }

        #endregion
    }   
}

