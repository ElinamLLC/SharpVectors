// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Interface for matrix operations used within the SVG DOM
	/// </summary>
	public interface ISvgMatrix
	{
        double A { get; set; }
        double B { get; set; }
        double C { get; set; }
        double D { get; set; }
        double E { get; set; }
        double F { get; set; }

        bool IsIdentity
        {
            get;
        }
		
		ISvgMatrix Multiply(ISvgMatrix secondMatrix);
		ISvgMatrix Inverse();
        ISvgMatrix Translate(double x, double y);
        ISvgMatrix Scale(double scaleFactor);
        ISvgMatrix ScaleNonUniform(double scaleFactorX, double scaleFactorY);
        ISvgMatrix Rotate(double angle);
        ISvgMatrix RotateFromVector(double x, double y);
		ISvgMatrix FlipX();
		ISvgMatrix FlipY();
        ISvgMatrix SkewX(double angle);
        ISvgMatrix SkewY(double angle);
	}	
}
