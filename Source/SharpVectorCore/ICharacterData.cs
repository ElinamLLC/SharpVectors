using System.Xml;

namespace SharpVectors.Dom
{
    /// <summary>
    /// Provides text manipulation methods that are used by several classes.
    /// </summary>
    public interface ICharacterData : INode
	{
        /// <summary>Contains the data of the node.</summary>
        /// <value>The data of the node.</value>
        string Data { get; set; }

        /// <summary>Gets the length of the data, in characters.</summary>
        /// <value>The length, in characters, of the string in the <see cref="ICharacterData.Data"/>
        /// property. The length may be zero; that is, <see cref="ICharacterData"/> nodes can be empty.</value>
        int Length { get; }

        /// <summary>Appends the specified string to the end of the character data of the node.</summary>
        /// <param name="strData">The string to insert into the existing string.</param>
        void AppendData(string strData);

        /// <summary>Removes a range of characters from the node.</summary>
        /// <param name="offset">The position within the string to start deleting.</param>
        /// <param name="count">The number of characters to delete.</param>
        void DeleteData(int offset, int count);

        /// <summary>Inserts the specified string at the specified character offset.</summary>
        /// <param name="offset">The position within the string to insert the supplied string data.</param>
        /// <param name="strData">The string data that is to be inserted into the existing string.</param>
        void InsertData(int offset, string strData);

        /// <summary>
        /// Replaces the specified number of characters starting at the specified offset
        /// with the specified string.
        /// </summary>
        /// <param name="offset">The position within the string to start replacing.</param>
        /// <param name="count">The number of characters to replace.</param>
        /// <param name="strData">The new data that replaces the old string data.</param>
        void ReplaceData(int offset, int count, string strData);

        /// <summary>Retrieves a substring of the full string from the specified range.</summary>
        /// <param name="offset">
        /// The position within the string to start retrieving. An offset of zero indicates
        /// the starting point is at the start of the data.
        /// </param>
        /// <param name="count">The number of characters to retrieve.</param>
        /// <returns>The substring corresponding to the specified range.</returns>
        string Substring(int offset, int count);
    }
}
