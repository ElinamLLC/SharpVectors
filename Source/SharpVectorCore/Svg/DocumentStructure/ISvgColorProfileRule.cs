using System;

using SharpVectors.Scripting;

namespace SharpVectors.Dom.Svg
{
    public interface ISvgColorProfileRule : ISvgCssRule, ISvgRenderingIntent
    {
        string Src { get; set; }
        string Name { get; set; }
        ushort RenderingIntent { get; set; }
    }
}
