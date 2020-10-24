using System;

namespace SharpVectors.Dom
{
    /// <summary>
    /// DOM exception code types.
    /// </summary>
	public enum DomExceptionType
	{
        /// <summary>
        /// If there is no defined error code for this exception or the exception is unknown.
        /// </summary>
        UnknowError              = 0,
        /// <summary>
        /// If index or size is negative, or greater than the allowed value.
        /// </summary>
        IndexSizeErr             = 1,
        /// <summary>
        /// If the specified range of text does not fit into a string.
        /// </summary>
        DomstringSizeErr         = 2,
        /// <summary>
        /// If any node is inserted somewhere it doesn't belong.
        /// </summary>
        HierarchyRequestErr      = 3,
        /// <summary>
        /// If a node is used in a different document than the one that created it (that doesn't support it).
        /// </summary>
        WrongDocumentErr         = 4,
        /// <summary>
        /// If an invalid or illegal character is specified, such as in a name. See 
        /// production 2 in the XML specification for the definition of a legal 
        /// character, and production 5 for the definition of a legal name character.
        /// </summary>
        InvalidCharacterErr      = 5,
        /// <summary>
        /// If data is specified for a node which does not support data.
        /// </summary>
        NoDataAllowedErr         = 6,
        /// <summary>
        /// If an attempt is made to modify an object where modifications are not allowed.
        /// </summary>
        NoModificationAllowedErr = 7,
        /// <summary>
        /// If an attempt is made to reference a node in a context where it does not exist.
        /// </summary>
        NotFoundErr              = 8,
        /// <summary>
        /// If the implementation does not support the requested type of object or operation.
        /// </summary>
        NotSupportedErr          = 9,
        /// <summary>
        /// If an attempt is made to add an attribute that is already in use elsewhere.
        /// </summary>
        InuseAttributeErr        = 10,
        /// <summary>
        /// If an attempt is made to use an object that is not, or is no longer, usable.
        /// </summary>
        InvalidStateErr          = 11,
        /// <summary>
        /// If an invalid or illegal string is specified.
        /// </summary>
        SyntaxErr                = 12,
        /// <summary>
        /// If an attempt is made to modify the type of the underlying object.
        /// </summary>
        InvalidModificationErr   = 13,
        /// <summary>
        /// If an attempt is made to create or change an object in a way which is 
        /// incorrect with regard to namespaces.
        /// </summary>
        NamespaceErr             = 14,
        /// <summary>
        /// If a parameter or an operation is not supported by the underlying object.
        /// </summary>
        InvalidAccessErr         = 15
    }
}
