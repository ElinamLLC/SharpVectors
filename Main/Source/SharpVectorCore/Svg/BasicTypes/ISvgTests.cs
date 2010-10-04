namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Interface SvgTests defines an interface which applies to all elements which have attributes requiredFeatures, requiredExtensions and systemLanguage. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>80</completed>
	public interface ISvgTests
	{
		ISvgStringList RequiredFeatures{get;}
  		ISvgStringList RequiredExtensions{get;}
  		ISvgStringList SystemLanguage{get;}

  		bool HasExtension ( string extension );
	}
}
