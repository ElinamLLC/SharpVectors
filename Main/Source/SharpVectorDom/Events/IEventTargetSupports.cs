using System;

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// Summary description for IEventTargetSupports.
	/// </summary>
	public interface IEventTargetSupport : IEventTarget
	{
		#region NON-DOM
		
		void FireEvent(
			IEvent evt);
		
		#endregion
	}
}
