using System;

namespace SharpVectors.Scripting
{
    public interface ISvgScriptClosureHandler : IDisposable
    {
        bool IsDisposed { get; }
        ISvgScriptEngine ScriptEngine { get; }

        void AddListener(string type, object listener, object useCapture);
        void RemoveListener(string type, object listener, object useCapture);
    }
}
