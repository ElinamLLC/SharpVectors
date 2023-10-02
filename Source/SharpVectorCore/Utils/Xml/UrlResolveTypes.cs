using System;

namespace SharpVectors.Xml
{
    [Flags]
    public enum UrlResolveTypes
    {
        None     = 0x0,
        Local    = 0x1,
        Remote   = 0x2,
        Resource = 0x4
    }
}