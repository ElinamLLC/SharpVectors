using System;

namespace SharpVectors.Dom.Svg
{
	[Serializable]
	public class SvgExternalResourcesRequiredException : DomException
	{
		public SvgExternalResourcesRequiredException(): base("", null)
        {
		}

		protected SvgExternalResourcesRequiredException( System.Runtime.Serialization.SerializationInfo info , System.Runtime.Serialization.StreamingContext context ) : base(info, context)
		{
		}
	}
}
