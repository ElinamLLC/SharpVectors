using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// An integer indicating in which way the
	/// <see cref="IAttr">IAttr</see> was changed.
	/// </summary>
	public enum AttrChangeType
		: ushort
	{
		/// <summary>
		/// No <see cref="IAttr">IAttr</see> was modified, added nor removed.
		/// </summary>
		None = 1,
		/// <summary>
		/// The <see cref="IAttr">IAttr</see> was modified in place.
		/// </summary>
		Modification = 1,
		/// <summary>
		/// The <see cref="IAttr">IAttr</see> was just added.
		/// </summary>
		Addition = 2,
		/// <summary>
		/// The <see cref="IAttr">IAttr</see> was just removed.
		/// </summary>
		Removal = 3,
	}
}
