using System;
using System.Xml;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace GdiW3cSvgTestSuite
{
    public partial class AboutTestDockPanel : DockPanelContent, ITestPagePanel
    {
        #region Private Fields

        private const string NamespaceText = " xmlns=\"http://www.w3.org/1999/xhtml\"";

        private string _svgFilePath;
        private SvgTestCase _testCase;

        #endregion

        #region Constructors and Destructor

        public AboutTestDockPanel()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.DockAreas     = DockAreas.Document | DockAreas.Float;

            this.Font = new Font(PanelDefaultFont, 14F, FontStyle.Regular, GraphicsUnit.World);

            testTitleLabel.Font      = new Font(PanelDefaultFont, 16F, FontStyle.Bold, GraphicsUnit.World);
            testDescritionLabel.Font = new Font(PanelDefaultFont, 16F, FontStyle.Bold, GraphicsUnit.World);
            testFilePathLabel.Font   = new Font(PanelDefaultFont, 16F, FontStyle.Bold, GraphicsUnit.World);
            testDetailsLabel.Font    = new Font(PanelDefaultFont, 16F, FontStyle.Bold, GraphicsUnit.World);

            testTitle.BorderStyle      = BorderStyle.None;
            testDescrition.BorderStyle = BorderStyle.None;
        }

        #endregion

        #region Public Properties

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

        #endregion

        #region ITestPagePanel Members

        public bool LoadDocument(string documentFilePath, SvgTestInfo testInfo, object extraInfo)
        {
            this.UnloadDocument();

            if (string.IsNullOrWhiteSpace(documentFilePath) || testInfo == null)
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

            btnFilePath.Enabled = isLoaded;

            testDetailsDoc.Focus();

            return isLoaded;
        }

        public void UnloadDocument()
        {
            _svgFilePath        = "";

            testTitle.Text      = "";
            testDescrition.Text = "";
            testFilePath.Text   = "";

            _testCase           = null;

            btnFilePath.Enabled = false;

            testDetailsDoc.Text = string.Empty;
        }

        #endregion

        #region Private Event Handlers

        private void OnFormLoad(object sender, EventArgs e)
        {

        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void OnFormShown(object sender, EventArgs e)
        {
            testDetailsDoc.Focus();
        }

        private void OnLocateFile(object sender, EventArgs e)
        {
            var filePath = testFilePath.Text;
            if (string.IsNullOrWhiteSpace(filePath) || File.Exists(filePath) == false)
            {
                return;
            }

            System.Diagnostics.Process.Start("explorer.exe", @"/select, " + filePath);
        }

        private bool LoadFile(Stream stream, SvgTestInfo testInfo)
        {
            Regex rgx = new Regex("\\s+");

            testTitle.Text = testInfo.Title;
            testDescrition.Text = rgx.Replace(testInfo.Description, " ").Trim();
            testFilePath.Text = _svgFilePath;

            btnFilePath.Enabled = true;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace             = false;
            settings.IgnoreComments               = true;
            settings.IgnoreProcessingInstructions = true;
            settings.DtdProcessing                = DtdProcessing.Parse;

            StringBuilder textBuilder = new StringBuilder();

            textBuilder.AppendLine("<html>");
            textBuilder.AppendLine("<head>");
            textBuilder.AppendLine("<title>About Test</title>");
            textBuilder.AppendLine("</head>");
            textBuilder.AppendLine("<body>");
            textBuilder.AppendLine("<div style=\"padding:0px;margin:0px 0px 15px 0px;\">");

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

            var paraStyle = new StringBuilder();
            paraStyle.Append("padding:3px;");
            paraStyle.Append("margin:0px;");

            var paraTag = string.Format("<p style=\"{0}\">", paraStyle);

            var tableStyle = new StringBuilder();
            tableStyle.Append("border:1px solid gray;");
            tableStyle.AppendFormat("margin:{0}px {1}px {2}px {3}px;", 16, 16, 16, 16);
//            tableStyle.Append("width:75%;");
            var tableTag = string.Format("<table style=\"{0}\" border=\"1\" cellpadding=\"3\" cellspacing=\"0\">", tableStyle);

            tableStyle.Length = 0;
            //            tableStyle.Append("border:1px solid gray;");
            tableStyle.Append("font-weight:bold;");
            tableStyle.Append("font-size:16px;");
            tableStyle.Append("text-align:center;");
            var cellTag = string.Format("<td style=\"{0}\">", tableStyle);

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                if (majorVersion == 1 && minorVersion == 1)
                {
                    var titleStyle = new StringBuilder();
                    titleStyle.Append("font-weight:bold;");
                    titleStyle.Append("font-size:14px;");
                    titleStyle.Append("padding:10px 0px 10px 0px;");
                    titleStyle.Append("margin:0px;");

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
                                    string inputText = reader.ReadInnerXml().Replace(NamespaceText, string.Empty);
                                    inputText = inputText.Replace("<p>", paraTag);
                                    inputText = inputText.Replace("<table>", tableTag);
                                    inputText = inputText.Replace("<th>", cellTag);
                                    inputText = inputText.Replace("</th>", "</td>");
                                    textBuilder.AppendLine(string.Format("<p style=\"{0}\">Operator Script</p>", titleStyle));
                                    textBuilder.AppendLine(inputText);
                                }
                                else if (string.Equals(nodeName, "d:passCriteria", StringComparison.OrdinalIgnoreCase))
                                {
                                    string inputText = reader.ReadInnerXml().Replace(NamespaceText, string.Empty);
                                    inputText = inputText.Replace("<p>", paraTag);
                                    inputText = inputText.Replace("<table>", tableTag);
                                    inputText = inputText.Replace("<th>", cellTag);
                                    inputText = inputText.Replace("</th>", "</td>");
                                    textBuilder.AppendLine(string.Format("<p style=\"{0}\">Pass Criteria</p>", titleStyle));
                                    textBuilder.AppendLine(inputText);
                                }
                                else if (string.Equals(nodeName, "d:testDescription", StringComparison.OrdinalIgnoreCase))
                                {
                                    string inputText = reader.ReadInnerXml().Replace(NamespaceText, string.Empty);
                                    inputText = inputText.Replace("<p>", paraTag);
                                    inputText = inputText.Replace("<table>", tableTag);
                                    inputText = inputText.Replace("<th>", cellTag);
                                    inputText = inputText.Replace("</th>", "</td>");
                                    textBuilder.AppendLine(string.Format("<p style=\"{0}\">Test Description</p>", titleStyle));
                                    textBuilder.AppendLine(inputText);

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
                                    string inputText = reader.ReadInnerXml().Replace(NamespaceText, string.Empty);
                                    inputText = inputText.Replace("<p>", paraTag);
                                    inputText = inputText.Replace("<table>", tableTag);
                                    inputText = inputText.Replace("<th>", cellTag);
                                    inputText = inputText.Replace("</th>", "</td>");
                                    textBuilder.AppendLine(inputText);
                                }
                                else if (string.Equals(nodeName, "d:PassCriteria", StringComparison.OrdinalIgnoreCase))
                                {
                                    string inputText = reader.ReadInnerXml().Replace(NamespaceText, string.Empty);
                                    inputText = inputText.Replace("<p>", paraTag);
                                    inputText = inputText.Replace("<table>", tableTag);
                                    inputText = inputText.Replace("<th>", cellTag);
                                    inputText = inputText.Replace("</th>", "</td>");
                                    textBuilder.AppendLine(inputText);
                                }
                                else if (string.Equals(nodeName, "d:TestDescription", StringComparison.OrdinalIgnoreCase))
                                {
                                    string inputText = reader.ReadInnerXml().Replace(NamespaceText, string.Empty);
                                    inputText = inputText.Replace("<p>", paraTag);
                                    inputText = inputText.Replace("<table>", tableTag);
                                    inputText = inputText.Replace("<th>", cellTag);
                                    inputText = inputText.Replace("</th>", "</td>");
                                    textBuilder.AppendLine(inputText);

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
                            string nodeName = reader.Name;
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
                                    string inputText = reader.ReadInnerXml().Replace(NamespaceText, string.Empty);

                                    string paraText = rgx.Replace(inputText, " ").Trim();

                                    textBuilder.AppendLine("<p>" + paraText + "</p>");
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

            textBuilder.AppendLine("</div>");
            textBuilder.AppendLine("</body>");
            textBuilder.AppendLine("</html>");
            testDetailsDoc.Text = textBuilder.ToString();

            return true;
        }

        #endregion
    }
}
