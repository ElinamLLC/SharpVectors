using System;

namespace GdiSvgTestBox
{
    public interface ITestPagePanel
    {
        bool IsDisposed { get; }

        void OnPageSelected(EventArgs e);
        void OnPageDeselected(EventArgs e);
    }
}
