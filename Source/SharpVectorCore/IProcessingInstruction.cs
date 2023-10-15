namespace SharpVectors.Dom
{
    /// <summary>
    /// Represents a processing instruction, which XML defines to keep processor-specific
    /// information in the text of the document.
    /// </summary>
    /// <seealso href="http://www.w3.org/TR/2000/REC-DOM-Level-2-Core-20001113">
    /// Document Object Model (DOM) Level 2 Core Specification</seealso>
    public interface IProcessingInstruction : INode
	{
        /// <summary>
        /// Gets the target of the processing instruction.
        /// </summary>
        /// <value>The target of the processing instruction.</value>
        /// <remarks>
        /// The target of this processing instruction. XML defines this as being 
        /// the first token following the markup that begins the processing instruction.
        /// </remarks>
        string Target { get; }

        /// <summary>
        /// Gets or sets the content of the processing instruction, excluding the target.
        /// </summary>
        /// <value>The content of the processing instruction, excluding the target.</value>
        string Data { get; set; }
    }
}
