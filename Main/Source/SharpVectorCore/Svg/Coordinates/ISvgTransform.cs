// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// SvgTransform is the interface for one of the component transformations within a SvgTransformList; thus, a SvgTransform object corresponds to a single component (e.g., "scale(..)" or "matrix(...)") within a transform attribute specification. 
	/// </summary>
	public interface ISvgTransform
	{
		short Type { get; }
		ISvgMatrix Matrix { get; }
        double Angle { get; }

		void SetMatrix(ISvgMatrix matrix);
        void SetTranslate(double tx, double ty);
        void SetScale(double sx, double sy);
        void SetRotate(double angle, double cx, double cy);
        void SetSkewX(double angle);
        void SetSkewY(double angle);
	}
}
