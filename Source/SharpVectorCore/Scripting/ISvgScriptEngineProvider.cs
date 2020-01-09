using System;

namespace SharpVectors.Scripting
{
    public interface ISvgScriptEngineProvider : IDisposable
    {
        bool IsDisposed { get; }
        ISvgScriptEngine Create();
    }
}
