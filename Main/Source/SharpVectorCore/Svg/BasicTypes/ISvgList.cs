// <developer>kevin@kevlidnev.com</developer>
// <completed>100</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// This interface defines a all methods used in a Svg*List interface. 
	/// </summary>
	public interface ISvgList
	{
		ulong NumberOfItems{get;}
		void Clear();
		object Initialize(object newItem);
		object GetItem(ulong index);
		object InsertItemBefore(object newItem, ulong index);
		object ReplaceItem(object newItem, ulong index);
		object RemoveItem(ulong index);
		object AppendItem(object newItem);
	}
}
