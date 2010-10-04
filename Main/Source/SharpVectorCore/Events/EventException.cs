using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// Event operations may throw an
	/// <see cref="EventException">EventException</see> as specified in their
	/// method descriptions.
	/// </summary>
	public class EventException
		: Exception
	{
		#region Private Fields
		
		private EventExceptionCode _code;
		
		#endregion
		
		#region Constructors
		
		public EventException(
			EventExceptionCode code)
		{
			_code = code;
		}
		
		#endregion
		
		#region Properties
		
		#region DOM Level 2
		
		/// <summary>
		/// An integer indicating the type of error generated.
		/// </summary>
		public EventExceptionCode Code
		{
			get
			{
				return _code;
			}
		}
		
		#endregion
		
		#endregion
	}
}
