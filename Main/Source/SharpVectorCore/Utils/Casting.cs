using System;
using System.Collections.Generic;

namespace SharpVectors
{
    public static class TryCast
    {
        public static bool Cast<B, D>(B baseObject, out D derivedObject)
            where D : class
        {
            if (baseObject == null)
            {
                derivedObject = null;
                return false;
            }

            derivedObject = baseObject as D;

            return (derivedObject != null);
        }
    }

    public static class DynamicCast
    {
        public static bool Cast<B, D>(B baseObject, out D derivedObject)
            where D : class, B
        {
            if (baseObject == null)
            {
                derivedObject = null;
                return false;
            }

            derivedObject = baseObject as D;

            return (derivedObject != null);
        }
    }
}
