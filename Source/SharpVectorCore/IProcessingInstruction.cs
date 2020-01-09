namespace SharpVectors.Dom
{
    /// <summary>
    ///   Represents a processing instruction, which XML defines to keep processor-specific
    ///   information in the text of the document.
    /// </summary>
    public interface IProcessingInstruction : INode
	{
        //
        // Summary:
        //     Gets the target of the processing instruction.
        //
        // Returns:
        //     The target of the processing instruction.
        string Target { get; }
        //
        // Summary:
        //     Gets or sets the content of the processing instruction, excluding the target.
        //
        // Returns:
        //     The content of the processing instruction, excluding the target.
        string Data { get; set; }
    }
}
