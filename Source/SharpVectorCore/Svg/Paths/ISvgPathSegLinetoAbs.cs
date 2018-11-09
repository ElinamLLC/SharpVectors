namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgPathSegLinetoAbs interface corresponds to an "absolute lineto" (L) path data command. 
    /// </summary>
    public interface ISvgPathSegLinetoAbs : ISvgPathSeg
    {
        double X { get; set; }
        double Y { get; set; }
    }
}