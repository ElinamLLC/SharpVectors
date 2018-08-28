using System;

namespace SharpVectors.Dom.Events
{
	/// <summary>
	/// Summary description for IEventTargetSupports.
	/// </summary>
	public interface IEventTargetSupport : IEventTarget
	{
		void FireEvent(IEvent evt);
	}
}
