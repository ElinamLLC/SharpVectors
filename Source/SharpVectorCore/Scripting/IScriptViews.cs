using System;

using SharpVectors.Dom.Views;

namespace SharpVectors.Scripting
{  
	/// <summary>
	/// IJsAbstractView
	/// </summary>
	public interface IJsAbstractView : IScriptableObject<IAbstractView>
    {
		IJsDocumentView document { get; }
	}

	/// <summary>
	/// IJsDocumentView
	/// </summary>
	public interface IJsDocumentView : IScriptableObject<IDocumentView>
    {
		IJsAbstractView defaultView { get; }
	}     
}
  