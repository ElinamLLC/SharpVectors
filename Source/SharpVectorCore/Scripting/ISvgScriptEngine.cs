using System;

namespace SharpVectors.Scripting
{
    public interface ISvgScriptEngine : IDisposable
    {
        string Name { get; }

        string this[string propertyName]
        {
            get;
        }

        bool IsDisposed { get; }
        bool IsInitialised { get; }

        IJsSvgWindow ScriptWindow { get; }

        ISvgScriptReferenceCache ReferenceCache { get; set; }

        ISvgScriptEventHandler ScriptEvents { get; }
        ISvgScriptClosureHandler ScriptClosures { get; }
        ISvgScriptTimerHandler ScriptTimers { get; }

        void Execute(string code);
        void Initialise();
        void Close();

    }
}
