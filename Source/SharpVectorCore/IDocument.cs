using System.Xml;

namespace SharpVectors.Dom
{
	/// <summary>
	/// 
	/// </summary>
	/// <see cref="XmlDocument"/>
	public interface IDocument : INode
	{
		IDocumentType Doctype
		{
			get;
		}
		
		IDomImplementation Implementation
		{
			get;
		}
		
		IElement DocumentElement
		{
			get;
		}

		bool CanAccessExternalResources(string resourcesUri);

        IElement CreateElement(string tagName);
		
		IDocumentFragment CreateDocumentFragment();
		
		IText CreateTextNode(string data);
		
		IComment CreateComment(string data);
		
		ICDataSection CreateCDataSection(string data);
		
		IProcessingInstruction CreateProcessingInstruction(string target, string data);
		
		IAttribute CreateAttribute(string name);
		
		IEntityReference CreateEntityReference(string name);
		
		INodeList GetElementsByTagName(string tagname);
		
		INode ImportNode(INode importedNode, bool deep);
		
		IElement CreateElementNs(string namespaceUri, string qualifiedName);
		
		IAttribute CreateAttributeNs(string namespaceUri, string qualifiedName);
		
		INodeList GetElementsByTagNameNs(string namespaceURI, string localName);
		
		IElement GetElementById(string elementId);
	}
}
