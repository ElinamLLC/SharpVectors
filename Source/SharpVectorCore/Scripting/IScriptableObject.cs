using System;

namespace SharpVectors.Scripting
{
    public interface IScriptableObject
    {
    }

    public interface IScriptableObject<T> : IScriptableObject where T : class
    {
        T BaseObject { get; }
    }
}
