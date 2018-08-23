using System;
using System.Xml;
using System.IO;
using System.IO.Compression;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System.Windows.Markup;

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

        public string SvgFilePath { get => _svgFilePath; set => _svgFilePath = value; }
        public SvgTestCase TestCase { get => _testCase; set => _testCase = value; }

        public bool LoadDocument(string documentFilePath, SvgTestInfo testInfo, object extraInfo = null)
        {
            this.UnloadDocument();

            if (string.IsNullOrEmpty(documentFilePath))
            {
                return false;
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

            return isLoaded;
        }

        public void UnloadDocument()
        {
            _svgFilePath        = "";

            testTitle.Text      = "";
            testDescrition.Text = "";
            testFilePath.Text   = "";

            _testCase           = null;

            testDetailsDoc.Blocks.Clear();

            if (this.NavigationService != null && this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
                this.NavigationService.RemoveBackEntry();
            }
        }

        private bool LoadFile(Stream stream, SvgTestInfo testInfo)
        {
            testTitle.Text      = testInfo.Title;
            testDescrition.Text = testInfo.Description;
            testFilePath.Text   = _svgFilePath;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace             = false;
            settings.IgnoreComments               = true;
            settings.IgnoreProcessingInstructions = true;
            settings.DtdProcessing                = DtdProcessing.Ignore;

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                if (reader.ReadToFollowing("SVGTestCase"))
                {
                    _testCase = new SvgTestCase();

                    Regex rgx = new Regex("\\s+");

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

            return true;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
