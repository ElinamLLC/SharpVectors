using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO.Compression;
using System.Printing;
using System.Text.RegularExpressions;

using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace WpfW3cSvgTestSuite
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page, ITestPage
    {
        private bool _useConverter;

        private string _svgFilePath;
        private SvgTestCase _testCase;

        private OptionSettings _optionSettings;

        public AboutPage()
        {
            InitializeComponent();

            _useConverter = true;

            var pageSize = new PageMediaSize(PageMediaSizeName.ISOA4);

            if (pageSize.Width != null)
            {
                testDetailsDoc.ColumnWidth = pageSize.Width.Value;
            }

            this.Loaded += OnPageLoaded;
        }

        public string SvgFilePath
        {
            get {
                return _svgFilePath;
            }
            set {
                _svgFilePath = value;
            }
        }

        public SvgTestCase TestCase
        {
            get {
                return _testCase;
            }
            set {
                _testCase = value;
            }
        }

        public bool LoadDocument(string documentFilePath, SvgTestInfo testInfo, object extraInfo = null)
        {
            this.UnloadDocument();

            if (string.IsNullOrWhiteSpace(documentFilePath) || testInfo == null)
            {
                return false;
            }

            if (extraInfo != null)
            {
                _optionSettings = extraInfo as OptionSettings;
            }
            _svgFilePath = documentFilePath;

            bool isLoaded = false;

            string fileExt = Path.GetExtension(documentFilePath);
            if (string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
            {
                using (FileStream fileStream = File.OpenRead(documentFilePath))
                {
                    using (GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        // Text Editor does not work with this stream, so we read the data to memory stream...
                        MemoryStream memoryStream = new MemoryStream();
                        // Use this method is used to read all bytes from a stream.
                        int totalCount = 0;
                        int bufferSize = 512;
                        byte[] buffer = new byte[bufferSize];
                        while (true)
                        {
                            int bytesRead = zipStream.Read(buffer, 0, bufferSize);
                            if (bytesRead == 0)
                            {
                                break;
                            }

                            memoryStream.Write(buffer, 0, bytesRead);
                            totalCount += bytesRead;
                        }

                        if (totalCount > 0)
                        {
                            memoryStream.Position = 0;
                        }

                        isLoaded = this.LoadFile(memoryStream, testInfo);

                        memoryStream.Close();
                    }
                }
            }
            else
            {
                using (FileStream stream = File.OpenRead(documentFilePath))
                {
                    isLoaded = this.LoadFile(stream, testInfo);
                }
            }

            btnFilePath.IsEnabled = isLoaded;

            return isLoaded;
        }

        public void UnloadDocument()
        {
            _svgFilePath        = "";

            testTitle.Text      = "";
            testDescrition.Text = "";
            testFilePath.Text   = "";

            _testCase           = null;

            btnFilePath.IsEnabled = false;

            testDetailsDoc.Blocks.Clear();

            if (this.NavigationService != null && this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
                this.NavigationService.RemoveBackEntry();
            }
        }

        private bool LoadFile(Stream stream, SvgTestInfo testInfo)
        {
            testDetailsDoc.Blocks.Clear();

            Regex rgx = new Regex("\\s+");

            testTitle.Text      = testInfo.Title;
            testDescrition.Text = rgx.Replace(testInfo.Description, " ").Trim();
            testFilePath.Text   = _svgFilePath;

            btnFilePath.IsEnabled = true;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace             = false;
            settings.IgnoreComments               = true;
            settings.IgnoreProcessingInstructions = true;
            settings.DtdProcessing                = DtdProcessing.Parse;

            SvgTestSuite selectedTestSuite = null;
            if (_optionSettings != null)
            {
                selectedTestSuite = _optionSettings.SelectedTestSuite;
            }
            if (selectedTestSuite == null)
            {
                selectedTestSuite = SvgTestSuite.GetDefault(SvgTestSuite.Create());
            }

            int majorVersion = selectedTestSuite.MajorVersion;
            int minorVersion = selectedTestSuite.MinorVersion;

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                if (majorVersion == 1 && minorVersion == 1)
                {
                    if (reader.ReadToFollowing("d:SVGTestCase"))
                    {
                        _testCase = new SvgTestCase();

                        while (reader.Read())
                        {
                            string nodeName = reader.Name;
                            XmlNodeType nodeType = reader.NodeType;
                            if (nodeType == XmlNodeType.Element)
                            {
                                if (string.Equals(nodeName, "d:operatorScript", StringComparison.OrdinalIgnoreCase))
                                {
                                    string inputText = reader.ReadInnerXml();
                                    string xamlText = HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(inputText, false);

                                    Paragraph titlePara = new Paragraph();
                                    titlePara.FontWeight = FontWeights.Bold;
                                    titlePara.FontSize = 16;
                                    titlePara.Inlines.Add(new Run("Operator Script"));

                                    if (_useConverter)
                                    {
                                        StringReader stringReader = new StringReader(xamlText);
                                        XmlReader xmlReader = XmlReader.Create(stringReader);

                                        Section nextSection = (Section)XamlReader.Load(xmlReader);
                                        if (nextSection != null)
                                        {
                                            testDetailsDoc.Blocks.Add(titlePara);
                                            testDetailsDoc.Blocks.Add(nextSection);
                                        }
                                    }
                                    else
                                    {
                                        Section pageSession = new Section();
                                        Paragraph nextPara = new Paragraph();
                                        //nextPara.Padding = new Thickness(3, 5, 3, 5);
                                        string paraText = rgx.Replace(inputText, " ").Trim();
                                        nextPara.Inlines.Add(new Run(paraText));
                                        pageSession.Blocks.Add(titlePara);
                                        pageSession.Blocks.Add(nextPara);

                                        testDetailsDoc.Blocks.Add(pageSession);
                                    }
                                }
                                else if (string.Equals(nodeName, "d:passCriteria", StringComparison.OrdinalIgnoreCase))
                                {
                                    string inputText = reader.ReadInnerXml();
                                    string xamlText = HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(inputText, false);

                                    Paragraph titlePara = new Paragraph();
                                    titlePara.FontWeight = FontWeights.Bold;
                                    titlePara.FontSize = 16;
                                    titlePara.Inlines.Add(new Run("Pass Criteria"));

                                    if (_useConverter)
                                    {
                                        StringReader stringReader = new StringReader(xamlText);
                                        XmlReader xmlReader = XmlReader.Create(stringReader);

                                        Section nextSection = (Section)XamlReader.Load(xmlReader);
                                        if (nextSection != null)
                                        {
                                            testDetailsDoc.Blocks.Add(titlePara);
                                            testDetailsDoc.Blocks.Add(nextSection);
                                        }
                                    }
                                    else
                                    {
                                        Section pageSession = new Section();
                                        Paragraph nextPara = new Paragraph();
                                        //nextPara.Padding = new Thickness(3, 5, 3, 5);
                                        string paraText = rgx.Replace(inputText, " ").Trim();
                                        nextPara.Inlines.Add(new Run(paraText));
                                        pageSession.Blocks.Add(titlePara);
                                        pageSession.Blocks.Add(nextPara);

                                        testDetailsDoc.Blocks.Add(pageSession);
                                    }
                                }
                                else if (string.Equals(nodeName, "d:testDescription", StringComparison.OrdinalIgnoreCase))
                                {
                                    string inputText = reader.ReadInnerXml();
                                    string xamlText = HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(inputText, false);

                                    Paragraph titlePara = new Paragraph();
                                    titlePara.FontWeight = FontWeights.Bold;
                                    titlePara.FontSize = 16;
                                    titlePara.Inlines.Add(new Run("Test Description"));

                                    if (_useConverter)
                                    {
                                        StringReader stringReader = new StringReader(xamlText);
                                        XmlReader xmlReader = XmlReader.Create(stringReader);

                                        Section nextSection = (Section)XamlReader.Load(xmlReader);
                                        if (nextSection != null)
                                        {
                                            testDetailsDoc.Blocks.Add(titlePara);
                                            testDetailsDoc.Blocks.Add(nextSection);
                                        }
                                    }
                                    else
                                    {
                                        Section pageSession = new Section();
                                        Paragraph nextPara = new Paragraph();
                                        //nextPara.Padding = new Thickness(3, 5, 3, 5);
                                        string paraText = rgx.Replace(inputText, " ").Trim();
                                        nextPara.Inlines.Add(new Run(paraText));
                                        pageSession.Blocks.Add(titlePara);
                                        pageSession.Blocks.Add(nextPara);

                                        testDetailsDoc.Blocks.Add(pageSession);
                                    }

                                    _testCase.Paragraphs.Add(inputText);
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement &&
                                string.Equals(nodeName, "SVGTestCase", StringComparison.OrdinalIgnoreCase))
                            {
                                break;
                            }
                        }

                    }
                }
                else if (majorVersion == 1 && minorVersion == 2)
                {
                    if (reader.ReadToFollowing("SVGTestCase"))
                    {
                        _testCase = new SvgTestCase();

                        while (reader.Read())
                        {
                            string nodeName = reader.Name;
                            XmlNodeType nodeType = reader.NodeType;
                            if (nodeType == XmlNodeType.Element)
                            {
                                if (string.Equals(nodeName, "d:OperatorScript", StringComparison.OrdinalIgnoreCase))
                                {
                                    string inputText = reader.ReadInnerXml();
                                    string xamlText = HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(inputText, false);

                                    if (_useConverter)
                                    {
                                        StringReader stringReader = new StringReader(xamlText);
                                        XmlReader xmlReader = XmlReader.Create(stringReader);

                                        Section nextSection = (Section)XamlReader.Load(xmlReader);
                                        if (nextSection != null)
                                        {
                                            testDetailsDoc.Blocks.Add(nextSection);
                                        }
                                    }
                                    else
                                    {
                                        Section pageSession = new Section();
                                        Paragraph nextPara = new Paragraph();
                                        //nextPara.Padding = new Thickness(3, 5, 3, 5);
                                        string paraText = rgx.Replace(inputText, " ").Trim();
                                        nextPara.Inlines.Add(new Run(paraText));
                                        pageSession.Blocks.Add(nextPara);

                                        testDetailsDoc.Blocks.Add(pageSession);
                                    }
                                }
                                else if (string.Equals(nodeName, "d:PassCriteria", StringComparison.OrdinalIgnoreCase))
                                {
                                    string inputText = reader.ReadInnerXml();
                                    string xamlText = HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(inputText, false);

                                    if (_useConverter)
                                    {
                                        StringReader stringReader = new StringReader(xamlText);
                                        XmlReader xmlReader = XmlReader.Create(stringReader);

                                        Section nextSection = (Section)XamlReader.Load(xmlReader);
                                        if (nextSection != null)
                                        {
                                            testDetailsDoc.Blocks.Add(nextSection);
                                        }
                                    }
                                    else
                                    {
                                        Section pageSession = new Section();
                                        Paragraph nextPara = new Paragraph();
                                        //nextPara.Padding = new Thickness(3, 5, 3, 5);
                                        string paraText = rgx.Replace(inputText, " ").Trim();
                                        nextPara.Inlines.Add(new Run(paraText));
                                        pageSession.Blocks.Add(nextPara);

                                        testDetailsDoc.Blocks.Add(pageSession);
                                    }
                                }
                                else if (string.Equals(nodeName, "d:TestDescription", StringComparison.OrdinalIgnoreCase))
                                {
                                    string inputText = reader.ReadInnerXml();
                                    string xamlText = HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(inputText, false);

                                    if (_useConverter)
                                    {
                                        StringReader stringReader = new StringReader(xamlText);
                                        XmlReader xmlReader = XmlReader.Create(stringReader);

                                        Section nextSection = (Section)XamlReader.Load(xmlReader);
                                        if (nextSection != null)
                                        {
                                            testDetailsDoc.Blocks.Add(nextSection);
                                        }
                                    }
                                    else
                                    {
                                        Section pageSession = new Section();
                                        Paragraph nextPara = new Paragraph();
                                        //nextPara.Padding = new Thickness(3, 5, 3, 5);
                                        string paraText = rgx.Replace(inputText, " ").Trim();
                                        nextPara.Inlines.Add(new Run(paraText));
                                        pageSession.Blocks.Add(nextPara);

                                        testDetailsDoc.Blocks.Add(pageSession);
                                    }

                                    _testCase.Paragraphs.Add(inputText);
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement &&
                                string.Equals(nodeName, "SVGTestCase", StringComparison.OrdinalIgnoreCase))
                            {
                                break;
                            }
                        }

                    }
                }
                else
                {
                    if (reader.ReadToFollowing("SVGTestCase"))
                    {
                        _testCase = new SvgTestCase();

                        while (reader.Read())
                        {
                            string nodeName      = reader.Name;
                            XmlNodeType nodeType = reader.NodeType;
                            if (nodeType == XmlNodeType.Element)
                            {
                                if (string.Equals(nodeName, "OperatorScript", StringComparison.OrdinalIgnoreCase))
                                {
                                    string revisionText = reader.GetAttribute("version");
                                    if (!string.IsNullOrWhiteSpace(revisionText))
                                    {
                                        revisionText = revisionText.Replace("$", "");
                                        _testCase.Revision = revisionText.Trim();
                                    }
                                    string nameText = reader.GetAttribute("testname");
                                    if (!string.IsNullOrWhiteSpace(nameText))
                                    {
                                        _testCase.Name = nameText.Trim();
                                    }
                                }
                                else if (string.Equals(nodeName, "Paragraph", StringComparison.OrdinalIgnoreCase))
                                {
                                    string inputText = reader.ReadInnerXml();
                                    string xamlText  = HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(inputText, false);

                                    if (_useConverter)
                                    {
                                        StringReader stringReader = new StringReader(xamlText);
                                        XmlReader xmlReader = XmlReader.Create(stringReader);

                                        Section nextSection = (Section)XamlReader.Load(xmlReader);

                                        testDetailsDoc.Blocks.Add(nextSection);
                                    }
                                    else
                                    {
                                        Section pageSession = new Section();
                                        Paragraph nextPara = new Paragraph();
                                        //nextPara.Padding = new Thickness(3, 5, 3, 5);
                                        string paraText = rgx.Replace(inputText, " ").Trim();
                                        nextPara.Inlines.Add(new Run(paraText));
                                        pageSession.Blocks.Add(nextPara);

                                        testDetailsDoc.Blocks.Add(pageSession);
                                    }

                                    _testCase.Paragraphs.Add(inputText);
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement &&
                                string.Equals(nodeName, "SVGTestCase", StringComparison.OrdinalIgnoreCase))
                            {
                                break;
                            }
                        }

                    }
                }
            }

            SubscribeToAllHyperlinks();

            return true;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnLocateFile(object sender, RoutedEventArgs e)
        {
            var filePath = testFilePath.Text;
            if (string.IsNullOrWhiteSpace(filePath) || File.Exists(filePath) == false)
            {
                return;
            }

            System.Diagnostics.Process.Start("explorer.exe", @"/select, " + filePath);
        }

        // https://stackoverflow.com/questions/2288999/how-can-i-get-a-flowdocument-hyperlink-to-launch-browser-and-go-to-url-in-a-wpf
        private void SubscribeToAllHyperlinks()
        {
            if (testDetailsDoc == null || testDetailsDoc.Blocks.Count == 0)
            {
                return;
            }

            var hyperlinks = GetVisuals(testDetailsDoc).OfType<Hyperlink>();
            foreach (var link in hyperlinks)
            {
                link.RequestNavigate += new RequestNavigateEventHandler(OnRequestNavigate);
            }
        }

        private static IEnumerable<DependencyObject> GetVisuals(DependencyObject root)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
            {
                yield return child;
                foreach (var descendants in GetVisuals(child))
                {
                    yield return descendants;
                }
            }
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var uriText = e.Uri.AbsoluteUri.Replace("http:", "https:");
            uriText = uriText.Replace("&", "^&");

            try
            {
                e.Handled = true;
                Process.Start(new ProcessStartInfo("cmd", $"/c start {uriText}") { CreateNoWindow = true });
            }
            catch (Exception ex)
            {
                var msgText = "Uri: " + uriText + Environment.NewLine + ex.ToString();
                Trace.TraceError(msgText);

                MessageBox.Show(msgText, MainWindow.AppErrorTitle, 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
