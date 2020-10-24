namespace SharpVectors.Dom
{
	/// <summary>
	/// The <see cref="INodeList"/> interface provides the abstraction of an ordered 
	/// collection of nodes, without defining or constraining how this collection 
	/// is implemented. <see cref="INodeList"/> objects in the DOM are live.
	/// <para>The items in the <see cref="INodeList"/> are accessible via an integral 
	/// index, starting from <c>0</c>.
	/// </para>
	/// </summary>
	/// <seealso href="http://www.w3.org/TR/2000/REC-DOM-Level-2-Core-20001113">
	/// Document Object Model (DOM) Level 2 Core Specification</seealso>
	public interface INodeList
	{
		/// <summary>
		/// Gets the <paramref name="index"/>th item in the collection. If <paramref name="index"/> is greater 
		/// than or equal to the number of nodes in the list, this returns <see langword="null"/>. 
		/// </summary>
		/// <param name="index"> into the collection. </param>
		/// <returns> The node at the <paramref name="index"/>th position in the 
		/// <see cref="INodeList"/>, or <see langword="null"/> if that is not a valid index. 
		/// </returns>
		INode this[ulong index]
		{
			get;
		}

		/// <summary>
		/// Gets the number of nodes in the list. The range of valid child node indices 
		/// is <c>0</c> to <c><see cref="Count"/>-1</c> inclusive. 
		/// </summary>
		ulong Count
		{
			get;
		}
	}
}
