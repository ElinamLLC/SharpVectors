using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
    public interface ISvgZoomEvent : IUiEvent
    {
        double PreviousScale { get; }
        ISvgPoint PreviousTranslate { get; }
        double NewScale { get; }
        ISvgPoint NewTranslate { get; }

        ISvgRect ZoomRectScreen { get; }
    }
}
