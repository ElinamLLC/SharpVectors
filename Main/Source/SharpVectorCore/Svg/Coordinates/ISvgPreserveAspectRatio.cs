// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPreserveAspectRatio interface corresponds to the preserveAspectRatio attribute, which is available for some of Svg's elements. 
	/// </summary>
	public interface ISvgPreserveAspectRatio
	{
		/// <summary>
		/// The type of the alignment value as specified by one of the constants specified above.
		/// </summary>
		SvgPreserveAspectRatioType Align{get;set;}

		/// <summary>
		/// The type of the meet-or-slice value as specified by one of the constants specified above.
		/// </summary>
		SvgMeetOrSlice MeetOrSlice{get;set;}
	}
}