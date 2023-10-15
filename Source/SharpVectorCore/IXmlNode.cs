using System.Xml;

namespace SharpVectors.Dom
{
	/// <summary>
	/// The <see cref="IXmlNode"/> interface is the primary datatype for the entire Document Object Model. 
	/// It represents a single node in the document tree. <see cref="XmlNode"/>
	/// </summary>
	/// <remarks>
	/// While all objects implementing the <see cref="INode"/> interface expose methods for dealing with children, 
	/// not all objects implementing the <see cref="INode"/> interface may have children. For example, <see cref="IText"/> 
	/// nodes may not have children, and adding children to such nodes results in a <see cref="DomException"/> being raised.
	/// <para>The attributes <see cref="Name"/>, <see cref="Value"/> and <see cref="Attributes"/> are included 
	/// as a mechanism to get at node information without casting down to the specific derived interface. 
	/// In cases where there is no obvious mapping of these attributes for a specific <see cref="NodeType"/> 
	/// (e.g., <see cref="Value"/> for an <see cref="IElement"/> or <see cref="Attributes"/> for a <see cref="IComment"/>), 
	/// this returns <see langword="null"/>. Note that the specialized interfaces may contain additional and more 
	/// convenient mechanisms to get and set the relevant information.
	/// </para>
	/// </remarks>
	/// <seealso href="https://www.w3.org/TR/2000/REC-DOM-Level-2-Core-20001113">
	/// Document Object Model (DOM) Level 2 Core Specification</seealso>
	public interface IXmlNode
	{
		/// <summary>
		/// Gets the qualified name of the node.
		/// </summary>
		/// <value>
		/// The qualified name of the node. The name returned is dependent on the <see cref="NodeType"/> of the node.
		/// </value>
		string Name
		{
			get;
		}

		/// <summary>
		/// Gets or sets the value of the node.
		/// </summary>
		/// <value>The value returned depends on the <see cref="NodeType"/> of the node</value>
		string Value
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the type of the current node.
		/// </summary>
		/// <value>An enumeration of the type <see cref="XmlNodeType"/> specifying the type of this node.</value>
		XmlNodeType NodeType
		{
			get;
		}

		/// <summary>
		/// Gets the parent of this node (for nodes that can have parents).
		/// </summary>
		/// <value>
		/// The <see cref="XmlNode"/> that is the parent of the current node. If a node has just been created
		/// and not yet added to the tree, or if it has been removed from the tree, the parent
		/// is <see langword="null"/>. For all other nodes, the value returned depends on the <see cref="NodeType"/>
		/// of the node. 
		/// </value>
		/// <remarks>
		/// Gets the parent of this node. All nodes, except <see cref="IAttribute"/>, <see cref="IDocument"/>, 
		/// <see cref="IDocumentFragment"/>, <see cref="IEntity"/>, and <see cref="INotation"/> may have a parent. 
		/// However, if a node has just been created and not yet added to the tree, or if it has been 
		/// removed from the tree, this is <see langword="null"/>.
		/// </remarks>
		XmlNode ParentNode
		{
			get;
		}

		/// <summary>
		/// Gets all the child nodes of the node.
		/// </summary>
		/// <value>
		/// An System.Xml.XmlNodeList that contains all the child nodes of the node. If there
		/// are no child nodes, this property returns an empty System.Xml.XmlNodeList not <see langword="null"/>.
		/// </value>
		XmlNodeList ChildNodes
		{
			get;
		}

		/// <summary>
		/// Gets the first child of this node. If there is no such node, this returns <see langword="null"/>.
		/// </summary>
		/// <value>The first child of the node. If there is no such node, <see langword="null"/> is returned.</value>
		XmlNode FirstChild
		{
			get;
		}

		/// <summary>
		/// Gets the last child of this node. If there is no such node, this returns <see langword="null"/>.
		/// </summary>
		/// <value>The last child of the node. If there is no such node, <see langword="null"/> is returned.</value>
		XmlNode LastChild
		{
			get;
		}

		/// <summary>
		/// Gets the node immediately preceding this node. If there is no such node, this returns <see langword="null"/>.
		/// </summary>
		/// <value>The preceding XmlNode. If there is no preceding node, null is returned.</value>
		XmlNode PreviousSibling
		{
			get;
		}

		/// <summary>
		/// Gets the node immediately following this node. If there is no such node, this returns <see langword="null"/>.
		/// </summary>
		/// <value>The next XmlNode. If there is no next node, null is returned.</value>
		XmlNode NextSibling
		{
			get;
		}

		/// <summary>
		/// Gets a <see cref="INamedNodeMap"/> containing the attributes of this node (if 
		/// it is an <see cref="IElement"/>) or <see langword="null"/> otherwise. 
		/// <para>This implementation returns an System.Xml.XmlAttributeCollection containing the attributes of this node.</para>
		/// </summary>
		/// <value>
		/// An XmlAttributeCollection containing the attributes of the node. If the node is
		/// of type XmlNodeType.Element, the attributes of the node are returned. Otherwise,
		/// this property returns null.
		/// </value>
		/// <seealso cref="INamedNodeMap"/>
		XmlAttributeCollection Attributes
		{
			get;
		}

		/// <summary>
		/// The <see cref="IDocument"/> object associated with this node. This is also the <see cref="IDocument"/> 
		/// object used to create new nodes. When this node is a <see cref="IDocument"/> or a <see cref="IDocumentType"/> 
		/// which is not used with any <see cref="IDocument"/> yet, this is <see langword="null"/>.
		/// <para>This implementation returns the System.Xml.XmlDocument to which this node belongs.</para>
		/// </summary>
		/// <value>
		/// The System.Xml.XmlDocument to which this node belongs.If the node is an System.Xml.XmlDocument
		/// (NodeType equals XmlNodeType.Document), this property returns null.
		/// </value>
		XmlDocument OwnerDocument
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether this node has any children.
		/// </summary>
		/// <value><see langword="true"/> if this node has any children, <see langword="false"/> otherwise. </value>
		bool HasChildNodes
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <summary> 
		/// Gets the namespace URI of this node, or <see langword="null"/> if it is unspecified.
		/// </summary>
		/// <value>
		/// The namespace URI of this node. If there is no namespace URI, this property returns <see cref="string.Empty"/>.
		/// </value>
		/// <remarks>
		/// <para>
		/// This is not a computed value that is the result of a namespace lookup based on an examination of the 
		/// namespace declarations in scope. It is merely the namespace URI given at creation time.
		/// </para>
		/// <para>
		/// For nodes of any type other than <see cref="XmlNodeType.Element"/> and <see cref="XmlNodeType.Attribute"/> 
		/// and nodes created with a DOM Level 1 method, such as <see cref="IDocument.CreateElement"/> from the 
		/// <see cref="IDocument"/> interface, this is always <see langword="null"/>. Per the Namespaces in XML Specification 
		/// an attribute does not inherit its namespace from the element it is attached to. If an attribute is 
		/// not explicitly given a namespace, it simply has no namespace.
		/// </para>
		/// </remarks>
		string NamespaceURI
		{
			get;
		}

        /// <summary>
        /// Gets or sets the namespace prefix of this node, or <see langword="null"/> if it is unspecified.
        /// </summary>
        /// <value>
        /// The namespace prefix of this node. For example, Prefix is <c>inkscape</c> for the element
        /// <c>&lt; inkscape:label &gt;</c>. If there is no prefix, this property returns String.Empty.
        /// </value>
        /// <remarks>
        /// <para>
        /// Note that setting this attribute, when permitted, changes the <see cref="Name"/> attribute, which holds 
        /// the qualified name.
        /// </para>
        /// <para>
        /// Note also that changing the prefix of an attribute that is known to have a default value, does not make a new 
        /// attribute with the default value and the original prefix appear, since the <see cref="NamespaceURI"/> and 
        /// <see cref="LocalName"/> do not change.
        /// </para>
        /// <para>
        /// For nodes of any type other than <see cref="XmlNodeType.Element"/> and <see cref="XmlNodeType.Attribute"/> and 
        /// nodes created with a DOM Level 1 method, such as <see cref="IDocument.CreateElement"/> from the 
        /// <see cref="IDocument"/> interface, this is always <see langword="null"/>. 
        /// </para>
        /// </remarks>
        /// <exception cref="DomException">
        /// <para>
        /// INVALID_CHARACTER_ERR: Raised if the specified prefix contains an illegal character.
        /// </para>
        /// <para>
        /// NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly.
        /// </para>
        /// <para>
        /// NAMESPACE_ERR: Raised if the specified <see cref="Prefix"/> is malformed, if the <see cref="NamespaceURI"/> of 
        /// this node is <see langword="null"/>, if the specified prefix is "xml" and the <see cref="NamespaceURI"/> of this 
        /// node is different from <see href="https://www.w3.org/XML/1998/namespace"/>, if this node is an attribute and the 
        /// specified prefix is <c>xmlns</c> and the <see cref="NamespaceURI"/> of this node is different from 
        /// <see href="https://www.w3.org/2000/xmlns/"/>, or if this node is an attribute and the <c>QualifiedName</c> of 
        /// this node is <c>xmlns</c>.
        /// </para>
        /// </exception>
        string Prefix
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the local part of the qualified name of this node.
		/// </summary>
		/// <value>
		/// The name of the node with the prefix removed. For example, <see cref="LocalName"/> is <c>label</c>
		/// for the element <c>&lt; inkscape:label &gt;</c>.The name returned is dependent on the System.Xml.XmlNode.NodeType
		/// of the node.
		/// </value>
		/// <remarks>
		/// For nodes of any type other than <see cref="XmlNodeType.Element"/> and <see cref="XmlNodeType.Attribute"/> and 
		/// nodes created with a DOM Level 1 method, such as <see cref="IDocument.CreateElement"/> from the 
		/// <see cref="IDocument"/> interface, this is always <see langword="null"/>.
		/// </remarks>
		string LocalName
		{
			get;
		}

		/// <summary>
		/// Inserts the node <paramref name="newChild"/> before the existing child node <paramref name="refChild"/>. 
		/// </summary>
		/// <remarks>
		/// If <paramref name="refChild"/> is <see langword="null"/>, insert <paramref name="newChild"/> at the end 
		/// of the list of children.
		/// <para>
		/// If <paramref name="newChild"/> is a <see cref="IDocumentFragment"/> object, 
		/// all of its children are inserted, in the same order, before <paramref name="refChild"/>. 
		/// </para>
		/// If the <paramref name="newChild"/> is already in the tree, it is first removed. 
		/// </remarks>
		/// <param name="newChild"> node to insert.</param>
		/// <param name="refChild"> reference node, i.e., the node before which the new node must be inserted.</param>
		/// <returns> The node being inserted. </returns>
		/// <exception cref="DomException">
		/// <para>
		/// HIERARCHY_REQUEST_ERR: Raised if this node is of a type that does not allow children of the type 
		/// of the <paramref name="newChild"/> node, or if the node to insert is one of this node's ancestors.
		/// </para>
		/// WRONG_DOCUMENT_ERR: Raised if <paramref name="newChild"/> was created from a different document 
		/// than the one that created this node.
		/// <para>
		/// NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly or if the parent of 
		/// the node being inserted is readonly.
		/// </para>
		/// <para>
		/// NOT_FOUND_ERR: Raised if <paramref name="refChild"/> is not a child of this node. 
		/// </para>
		/// </exception>
		XmlNode InsertBefore(XmlNode newChild, XmlNode refChild);

		/// <summary>
		/// Replaces the child node <paramref name="oldChild"/> with <paramref name="newChild"/>
		/// in the list of children, and returns the <paramref name="oldChild"/> node.
		/// </summary>
		/// <remarks>
		/// If <paramref name="newChild"/> is a <see cref="IDocumentFragment"/> object, <paramref name="oldChild"/> 
		/// is replaced by all of the <see cref="IDocumentFragment"/> children, which are inserted in the 
		/// same order. If the <paramref name="newChild"/> is already in the tree, it is first removed. 
		/// </remarks>
		/// <param name="newChild"> new node to put in the child list. </param>
		/// <param name="oldChild"> node being replaced in the list. </param>
		/// <returns> The node replaced. </returns>
		/// <exception cref="DomException">
		/// <para>
		/// HIERARCHY_REQUEST_ERR: Raised if this node is of a type that does not allow children of 
		/// the type of the <paramref name="newChild"/> node, or if the node to put in is one of this node's ancestors.
		/// </para>
		/// <para>
		/// WRONG_DOCUMENT_ERR: Raised if <paramref name="newChild"/> was created from a different 
		/// document than the one that created this node.
		/// </para>
		/// <para>
		/// NO_MODIFICATION_ALLOWED_ERR: Raised if this node or the parent of the new node is readonly.
		/// </para>
		/// <para>
		/// NOT_FOUND_ERR: Raised if <paramref name="oldChild"/> is not a child of this node. 
		/// </para>
		/// </exception>
		XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild);

		/// <summary>
		/// Removes the child node indicated by <paramref name="oldChild"/> from the list of children, and returns it. 
		/// </summary>
		/// <param name="oldChild"> node being removed. </param>
		/// <returns> The node removed. </returns>
		/// <exception cref="DomException">
		/// NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly.
		/// <para>
		/// NOT_FOUND_ERR: Raised if <paramref name="oldChild"/> is not a child of this node. 
		/// </para>
		/// </exception>
		XmlNode RemoveChild(XmlNode oldChild);

		/// <summary>
		/// Adds the node <paramref name="newChild"/> to the end of the list of children of this node. 
		/// If the <paramref name="newChild"/> is already in the tree, it is first removed. 
		/// </summary>
		/// <param name="newChild"> The node to add. If it is a <see cref="IDocumentFragment"/> object, the entire 
		/// contents of the document fragment are moved into the child list of this node </param>
		/// <returns> The node added. </returns>
		/// <exception cref="DomException">
		/// <para>
		/// HIERARCHY_REQUEST_ERR: Raised if this node is of a type that does not allow children of the 
		/// type of the <paramref name="newChild"/> node, or if the node to append is one of this node's ancestors.
		/// </para>
		/// <para>
		/// WRONG_DOCUMENT_ERR: Raised if <paramref name="newChild"/> was created from a different document 
		/// than the one that created this node.
		/// </para>
		/// <para>
		/// NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly. 
		/// </para>
		/// </exception>
		XmlNode AppendChild(XmlNode newChild);

		/// <summary>
		/// Returns a duplicate of this node, i.e., serves as a generic copy constructor for nodes. 
		/// The duplicate node has no parent; (<see cref="ParentNode"/> is <see langword="null"/>.).
		/// </summary>
		/// <remarks>
		/// <para>
		/// Cloning an <see cref="IElement"/> copies all attributes and their values, including those generated 
		/// by the XML processor to represent defaulted attributes, but this method does not copy any text it 
		/// contains unless it is a deep clone, since the text is contained in a child <see cref="IText"/> node. 
		/// </para>
		/// <para>
		/// Cloning an <see cref="IAttribute"/> directly, as opposed to be cloned as part of an <see cref="IElement"/> 
		/// cloning operation, returns a specified attribute (<see cref="IAttribute.Specified"/> is <see langword="true"/>). 
		/// </para>
		/// <para>
		/// Cloning any other type of node simply returns a copy of this node.
		/// Note that cloning an immutable subtree results in a mutable copy, but the children of an 
		/// <see cref="IEntityReference"/> clone are readonly. In addition, clones of unspecified <see cref="IAttribute"/> 
		/// nodes are specified. And, cloning <see cref="IDocument"/>, <see cref="IDocumentType"/>, <see cref="IEntity"/>, 
		/// and <see cref="INotation"/> nodes is implementation dependent. 
		/// </para>
		/// </remarks>
		/// <param name="deep"> <see langword="true"/>, recursively clone the subtree under the specified node; 
		/// if <see langword="false"/>, clone only the node itself (and its attributes, if it is an <see cref="IElement"/>). 
		/// </param>
		/// <returns> The duplicate node. </returns>
		XmlNode CloneNode(bool deep);

		/// <summary>
		/// Puts all <see cref="IText"/> nodes in the full depth of the sub-tree underneath this <see cref="INode"/>, 
		/// including attribute nodes, into a "normal" form where only structure (e.g., elements, comments, 
		/// processing instructions, CDATA sections, and entity references) separates <see cref="IText"/> nodes, 
		/// i.e., there are neither adjacent <see cref="IText"/> nodes nor empty <see cref="IText"/> nodes. 
		/// </summary>
		/// <remarks>
		/// This can be used to ensure that the DOM view of a document is the same as if it were saved and re-loaded, 
		/// and is useful when operations (such as <c>XPointer</c> lookups) that depend on a particular document tree 
		/// structure are to be used. 
		/// <para>
		/// In cases where the document contains <see cref="ICDataSection"/>, the normalize operation alone may not be 
		/// sufficient, since XPointers do not differentiate between <see cref="IText"/> nodes and <see cref="ICDataSection"/> nodes.
		/// </para>
		/// </remarks>
		void Normalize();

		/// <summary>
		/// Tests whether the DOM implementation implements a specific feature and that feature is supported by this node. 
		/// </summary>
		/// <param name="feature"> name of the feature to test. This is the same name which can be passed to the 
		/// method <see cref="IDomImplementation.HasFeature"/> on <see cref="IDomImplementation"/>. </param>
		/// <param name="version"> This is the version number of the feature to test. In Level 2, version 1, 
		/// this is the string <c>"2.0"</c>. If the version is not specified, supporting any version of the feature will cause the
		/// method to return <see langword="true"/>.</param>
		/// <returns> Returns <see langword="true"/> if the specified feature is supported on this node, 
		/// <see langword="false"/> otherwise.</returns>
		bool Supports(string feature, string version);
	}
}
