using System;

namespace GdiW3cSvgTestSuite
{
    public interface ITestPagePanel
    {
        bool LoadDocument(string documentFilePath, SvgTestInfo testInfo, object extraInfo);
        void UnloadDocument();

        bool IsDisposed { get; }

        void OnPageSelected(EventArgs e);
        void OnPageDeselected(EventArgs e);
    }
}
