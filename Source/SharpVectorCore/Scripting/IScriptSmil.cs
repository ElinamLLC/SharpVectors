using System;

namespace SharpVectors.Scripting
{      
    /// <summary>
    /// IJsElementTimeControl
    /// </summary>
    public interface IJsElementTimeControl
    {
        void beginElement();
        void beginElementAt(float offset);
        void endElement();
        void endElementAt(float offset);
    }

    /// <summary>
    /// IJsTimeEvent
    /// </summary>
    public interface IJsTimeEvent : IJsEvent
    {
        IJsAbstractView view { get; }  
        long detail { get; }
        
        void initTimeEvent(string typeArg, IJsAbstractView viewArg, long detailArg);
    }   
}
