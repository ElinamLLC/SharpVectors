using System;
using System.Xml;

namespace SharpVectors.Dom
{
    /// <summary>
    /// Summary description for IAttribute.
    /// </summary>
    public interface IAttribute : INode
    {
        XmlElement OwnerElement
        {
            get;
        }

        bool Specified
        {
            get;
        }
    }
}
