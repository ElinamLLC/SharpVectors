namespace SharpVectors.Dom.Svg
{
    public enum SvgTransformType : short
    {
        Unknown,
        Matrix,
        Translate,
        Scale,
        Rotate,
        SkewX,
        SkewY
    }

    /// <summary>
    /// SvgTransform is the interface for one of the component transformations within a SvgTransformList; 
    /// thus, a SvgTransform object corresponds to a single component (e.g., "scale(..)" or "matrix(...)") 
    /// within a transform attribute specification. 
    /// </summary>
    public interface ISvgTransform
	{
		short Type { get; }
        SvgTransformType TransformType { get; }
		ISvgMatrix Matrix { get; }
        double Angle { get; }
        double[] InputValues { get; }

		void SetMatrix(ISvgMatrix matrix);
        void SetTranslate(double tx, double ty);
        void SetScale(double sx, double sy);
        void SetRotate(double angle, double cx, double cy);
        void SetSkewX(double angle);
        void SetSkewY(double angle);
	}
}
