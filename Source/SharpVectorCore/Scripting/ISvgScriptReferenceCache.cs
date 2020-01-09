using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpVectors.Scripting
{
    public interface ISvgScriptReferenceCache
    {
        void Add(object key, IScriptableObject value);
        bool Remove(object key);
        bool TryGetValue(object key, out IScriptableObject value);
    }
}
