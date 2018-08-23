using System;

namespace WpfW3cSvgTestSuite
{
    public interface ITestPage
    {
        bool LoadDocument(string documentFilePath, SvgTestInfo testInfo, object extraInfo);
        void UnloadDocument();
    }
}
