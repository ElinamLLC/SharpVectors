// <developer>niklas@protocol7.com</developer>
// <completed>0</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// For each 'use' element, the Svg DOM maintains a shadow tree (the "instance tree") of objects of type SvgElementInstance
	/// </summary>
	public interface ISvgElementInstance 
	{
		ISvgElement CorrespondingElement{get;}
		ISvgUseElement CorrespondingUseElement{get;}
		ISvgElementInstance ParentNode{get;}
		ISvgElementInstanceList ChildNodes{get;}
		ISvgElementInstance FirstChild{get;}
		ISvgElementInstance LastChild{get;}
		ISvgElementInstance PreviousSibling{get;}
		ISvgElementInstance NextSibling{get;}
	}
}
