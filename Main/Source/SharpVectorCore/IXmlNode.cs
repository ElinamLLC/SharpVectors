using System;
using System.Xml;

namespace SharpVectors.Dom
{
	public interface IXmlNode
	{
		string Name
		{
			get;
		}
		
		string Value
		{
			get;
			set;
		}
		
		XmlNodeType NodeType
		{
			get;
		}
		
		XmlNode ParentNode
		{
			get;
		}
		
		XmlNodeList ChildNodes
		{
			get;
		}
		
		XmlNode FirstChild
		{
			get;
		}
		
		XmlNode LastChild
		{
			get;
		}
		
		XmlNode PreviousSibling
		{
			get;
		}
		
		XmlNode NextSibling
		{
			get;
		}
		
		XmlAttributeCollection Attributes
		{
			get;
		}
		
		XmlDocument OwnerDocument
		{
			get;
		}
		
		bool HasChildNodes
		{
			get;
		}
		
		string NamespaceURI
		{
			get;
		}
		
		string Prefix
		{
			get;
			set;
		}
		
		string LocalName
		{
			get;
		}
		
		XmlNode InsertBefore(XmlNode newChild, XmlNode refChild);
		
		XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild);
		
		XmlNode RemoveChild(XmlNode oldChild);
		
		XmlNode AppendChild(XmlNode newChild);
		
		XmlNode CloneNode(bool deep);
		
		void Normalize();
		
		bool Supports(string feature, string version);
	}
}
