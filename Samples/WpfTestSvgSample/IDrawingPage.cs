namespace WpfTestSvgSample
{
    interface IDrawingPage
    {
        bool SaveXaml { get; set; }
        string XamlDrawingDir { get; set; }

        bool LoadDocument(string svgFilePath);
        void UnloadDocument();
        bool SaveDocument(string fileName);
        void PageSelected(bool isSelected);
    }
}
