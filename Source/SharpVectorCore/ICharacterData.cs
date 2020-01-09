namespace SharpVectors.Dom
{
	/// <summary>
	/// Summary description for ICharacterData.
	/// </summary>
	public interface ICharacterData : INode
	{
        //
        // Summary:
        //     Contains the data of the node.
        //
        // Returns:
        //     The data of the node.
        string Data { get; set; }
        //
        // Summary:
        //     Gets the length of the data, in characters.
        //
        // Returns:
        //     The length, in characters, of the string in the System.Xml.XmlCharacterData.Data
        //     property. The length may be zero; that is, CharacterData nodes can be empty.
        int Length { get; }

        //
        // Summary:
        //     Appends the specified string to the end of the character data of the node.
        //
        // Parameters:
        //   strData:
        //     The string to insert into the existing string.
        void AppendData(string strData);
        //
        // Summary:
        //     Removes a range of characters from the node.
        //
        // Parameters:
        //   offset:
        //     The position within the string to start deleting.
        //
        //   count:
        //     The number of characters to delete.
        void DeleteData(int offset, int count);
        //
        // Summary:
        //     Inserts the specified string at the specified character offset.
        //
        // Parameters:
        //   offset:
        //     The position within the string to insert the supplied string data.
        //
        //   strData:
        //     The string data that is to be inserted into the existing string.
        void InsertData(int offset, string strData);
        //
        // Summary:
        //     Replaces the specified number of characters starting at the specified offset
        //     with the specified string.
        //
        // Parameters:
        //   offset:
        //     The position within the string to start replacing.
        //
        //   count:
        //     The number of characters to replace.
        //
        //   strData:
        //     The new data that replaces the old string data.
        void ReplaceData(int offset, int count, string strData);
        //
        // Summary:
        //     Retrieves a substring of the full string from the specified range.
        //
        // Parameters:
        //   offset:
        //     The position within the string to start retrieving. An offset of zero indicates
        //     the starting point is at the start of the data.
        //
        //   count:
        //     The number of characters to retrieve.
        //
        // Returns:
        //     The substring corresponding to the specified range.
        string Substring(int offset, int count);
    }
}
