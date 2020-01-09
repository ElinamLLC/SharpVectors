using System;

namespace SharpVectors.Scripting
{
    public interface ISvgScriptTimerHandler : IDisposable
    {
        bool IsDisposed { get; }
        ISvgScriptEngine ScriptEngine { get; }
    }
}
