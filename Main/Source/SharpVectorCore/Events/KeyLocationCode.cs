using System;

namespace SharpVectors.Dom.Events
{
	/// <remarks>
	/// Note: In case a DOM implementation wishes to provide new location
	/// information, all values above the value of this constant can be
	/// used for that effect and generic DOM applications should consider
	/// values above the value of this constant as being equivalent to
	/// <see cref="KeyLocationCode.Unknown">Unknown</see>.
	/// </remarks>
	public enum KeyLocationCode
		: ulong
	{
		/// <summary>
		/// The key activation is not distinguished as the left or right
		/// version of the key, and did not originate from the numeric
		/// keypad (or did not originate with a virtual key corresponding to
		/// the numeric keypad). Example: the 'Q' key on a PC 101 Key US.
		/// </summary>
		Standard = 0x00,
		/// <summary>
		/// The key activated is in the left key location (there is more than
		/// one possible location for this key). Example: the left Shift key
		/// on a PC 101 Key US.
		/// </summary>
		Left = 0x01,
		/// <summary>
		/// The key activation is in the right key location (there is more
		/// than one possible location for this key). Example: the right
		/// Shift key on a PC 101 Key US.
		/// </summary>
		Right = 0x02,
		/// <summary>
		/// The key activation originated on the numeric keypad or with a
		/// virtual key corresponding to the numeric keypad. Example: the
		/// '1' key on a PC 101 Key US.
		/// </summary>
		Numpad = 0x03,
		/// <summary>
		/// Implementations can use this constant to indicate that the
		/// location of the key cannot be determined.
		/// </summary>
		Unknown = 0x04,
	}
}
