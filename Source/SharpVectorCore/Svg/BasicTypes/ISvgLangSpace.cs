namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Interface SvgLangSpace defines an interface which applies to all elements which have attributes xml:lang and xml:space. 
	/// </summary>
	public interface ISvgLangSpace
	{
		string XmlLang{get;set;}
		string XmlSpace{get;set;}
	}
}
