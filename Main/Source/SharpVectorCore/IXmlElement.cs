using System;
using System.Xml;

namespace SharpVectors.Dom
{
	public interface IXmlElement : INode
	{
		bool HasAttributes
		{
			get;
		}
		
		string GetAttribute(string name);
		
		void SetAttribute(string name, string value);
		
		void RemoveAttribute(string name);
		
		XmlAttribute GetAttributeNode(string name);
		
		XmlAttribute SetAttributeNode(XmlAttribute newAttr);
		
		XmlAttribute RemoveAttributeNode(XmlAttribute oldAttr);
		
		XmlNodeList GetElementsByTagName(string name);
		
		// level 2 calls
		string GetAttribute(string localName, string namespaceURI);
		
		string SetAttribute(string qualifiedName, string namespaceURI, string value);
		
		void RemoveAttribute(string localName, string namespaceURI);
		
		XmlAttribute GetAttributeNode(string localName,	string namespaceURI);
		
		XmlNodeList GetElementsByTagName(string localName, string namespaceURI);
		
		bool HasAttribute(string name);
		
		bool HasAttribute(string localName, string namespaceURI);
	}
}
