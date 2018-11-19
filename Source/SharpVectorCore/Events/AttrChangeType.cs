namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// An integer indicating in which way the attribute was changed.
	/// </summary>
	public enum AttrChangeType : ushort
	{
		/// <summary>
		/// No attribute was modified, added nor removed.
		/// </summary>
		None         = 1,
		/// <summary>
		/// The attribute was modified in place.
		/// </summary>
		Modification = 1,
		/// <summary>
		/// The attribute was just added.
		/// </summary>
		Addition     = 2,
		/// <summary>
		/// The attribute was just removed.
		/// </summary>
		Removal      = 3
	}
}
