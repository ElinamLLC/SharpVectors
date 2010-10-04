using System;

namespace SharpVectors.Dom.Svg
{
    public interface ISvgColorProfileElement : ISvgElement, ISvgUriReference
    {
        string Local
        {
            get;
        }
    }
}
