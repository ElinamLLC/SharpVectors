using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    public interface ISvgControl
    {
        int Width { get; }
        int Height { get; }
        bool DesignMode { get; }

        void HandleAlert(string message);

        void HandleError(string message);
        void HandleError(Exception exception);
        void HandleError(string message, Exception exception);
    }
}
