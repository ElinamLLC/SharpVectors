using System;

namespace SharpVectors.Scripting
{
    public interface ISvgScriptEventHandler : IDisposable
    {
        bool IsDisposed { get; }
        ISvgScriptEngine ScriptEngine { get; }
    }
}
