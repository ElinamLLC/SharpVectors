using System;

namespace SharpVectors.Dom.Svg
{
    public interface ISvgColorProfileElement : ISvgElement, ISvgUriReference
    {
        string Local { get; set; }
        new string Name { get; set; }
        ushort RenderingIntent { get; set; }
    }
}
