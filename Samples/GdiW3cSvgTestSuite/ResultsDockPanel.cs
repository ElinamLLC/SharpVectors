using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace GdiW3cSvgTestSuite
{
    public partial class ResultsDockPanel : DockPanelContent, ITestPagePanel
    {
        #region Private Fields

        private bool _wasDeselected;
        private IList<string> _categoryLabels;
        private IList<SvgTestResult> _testResults;

        #endregion

        #region Constructors and Destructor

        public ResultsDockPanel()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.DockAreas     = DockAreas.Document | DockAreas.Float;

            this.CloseButton   = false;

            labelTitle.Font    = new Font(PanelDefaultFont, 20F, FontStyle.Bold, GraphicsUnit.World);
        }

        #endregion

        #region Public Properties

        public IList<SvgTestResult> Results
        {
            get {
                return _testResults;
            }
            set {
                _testResults = value;
            }
        }

        #endregion

        #region Public Methods

        public override void OnPageDeselected(EventArgs e)
        {
            base.OnPageDeselected(e);


            _wasDeselected = true;
        }

        public override void OnPageSelected(EventArgs e)
        {
            base.OnPageSelected(e);

            if (_wasDeselected || _testResults == null)
            {
                if (_mainForm != null)
                {
                    _testResults = _mainForm.TestResults;
                }

                this.CreateDocument();
            }

            _wasDeselected = false;
        }

        #endregion

        #region ITestPagePanel Members

        public bool LoadDocument(string documentFilePath, SvgTestInfo testInfo, object extraInfo)
        {
            return true;
        }

        public void UnloadDocument()
        {
        }

        #endregion

        #region Private Event Handlers

        private void OnFormLoad(object sender, EventArgs e)
        {
            if (_mainForm != null)
            {
                _testResults = _mainForm.TestResults;
            }

            testDetailsDoc.MinimumSize = new Size(800, testDetailsDoc.Height);

            this.CreateDocument();
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void OnFormShown(object sender, EventArgs e)
        {
        }

        #endregion

        #region Private Methods

        private string BeginDocument()
        {
            StringBuilder textBuilder = new StringBuilder();

            textBuilder.AppendLine("<html>");
            textBuilder.AppendLine("<head>");
            textBuilder.AppendLine("<title>Test Results</title>");
            textBuilder.AppendLine("</head>");
            textBuilder.AppendLine("<body style=\"min-width:800px;\">");

            textBuilder.AppendLine("<div style=\"padding:0px;margin:0px 0px 15px 0px;\">");
            textBuilder.AppendLine("<h3 style=\"padding:0px;margin:0px;\">Introduction</h3>");

            textBuilder.AppendLine(string.Format("<p>{0}</p>",
                @"The document attempts to present a summary of the test results of SharpVectors library, 
and the comparison of the results of released versions to track the progress of the development."));
            textBuilder.AppendLine(string.Format("<p>{0}</p>",
                @"These results are not based on pixel comparisons of the generated images and the expected PNG images 
provided with the Test Suite, but are manual or human-eye comparisons. This is mainly due to the fact that 
the rendered image quality vary and the Test Suite fonts are mostly not available on testing enviroments."));

            textBuilder.AppendLine("</div>");

            return textBuilder.ToString();
        }

        private string EndDocument()
        {
            StringBuilder textBuilder = new StringBuilder();

            textBuilder.AppendLine("</body>");
            textBuilder.AppendLine("</html>");

            return textBuilder.ToString();
        }

        private void CreateDocument()
        {
            testDetailsDoc.Text = string.Empty;
            if (_testResults == null || _testResults.Count == 0)
            {
                return;
            }

            var documentBuilder = new StringBuilder(BeginDocument());

            var noteSection = new StringBuilder();
            noteSection.AppendLine(CreateAlert("NOTE: Test Suite",
                "These tests are based on SVG 1.1 First Edition Test Suite: 13 December 2006 (Full)."));

            noteSection.AppendLine(CreateHorzLine( false));

            documentBuilder.AppendLine(noteSection.ToString());

            _categoryLabels = new List<string>();

            int resultCount = _testResults.Count;
            for (int i = 0; i < resultCount; i++)
            {
                SvgTestResult testResult = _testResults[i];
                if (!testResult.IsValid)
                {
                    continue;
                }
                IList<SvgTestCategory> testCategories = testResult.Categories;
                if (testCategories == null || testCategories.Count == 0)
                {
                    continue;
                }

                if (i == 0)
                {
                    for (int j = 0; j < testCategories.Count; j++)
                    {
                        _categoryLabels.Add(testCategories[j].Label);
                    }
                }

                var titleSection = new StringBuilder();
                titleSection.AppendLine("<div style=\"padding:0px;margin:0px;\">");
                string headingText = string.Format("{0}. Test Results: SharpVectors Version {1}", (i + 1), testResult.Version);
                var titlePara = new StringBuilder();
                titlePara.AppendLine(string.Format("<h3 style=\"padding:0px;margin:0px;\">{0}</h3>", headingText));
                titleSection.AppendLine(titlePara.ToString());

                var datePara = new StringBuilder();
                var tableStyle = new StringBuilder();
                tableStyle.Append("font-weight:bold;");
                tableStyle.Append("font-size:16px;");
                tableStyle.Append("border-width:1px;");
                tableStyle.Append("text-align:center;");
                tableStyle.Append("padding:3px;");
                datePara.AppendLine(string.Format("<p style=\"{0}\">{1}</p>",
                    tableStyle, "Test Date: " + testResult.Date.ToString()));

                titleSection.AppendLine(datePara.ToString());

                titleSection.AppendLine("</div>");

                documentBuilder.AppendLine(titleSection.ToString());

                var resultSection = new StringBuilder();
                var resultTable = CreateResultTable(testCategories);

                resultSection.AppendLine(resultTable);

                if (resultCount > 1)
                {
                    resultSection.AppendLine(this.CreateHorzLine((i == (resultCount - 1))));
                }
                else
                {
                    resultSection.AppendLine(this.CreateHorzLine());
                }

                documentBuilder.AppendLine(resultSection.ToString());
            }

            if (resultCount > 1)
            {
                var summarySection = new StringBuilder();
                var summaryPara = new StringBuilder();
                summaryPara.AppendLine("<h3>Test Results: Summary</h3>");

                summarySection.AppendLine(summaryPara.ToString());

                var summaryTable = CreateSummaryTable();

                summarySection.AppendLine(summaryTable);

                summarySection.AppendLine(CreateAlert("NOTE: Percentage",
                    "The percentage calculations do not include partial success cases."));

                summarySection.AppendLine(this.CreateHorzLine(true));

                documentBuilder.AppendLine(summarySection.ToString());
            }

            documentBuilder.AppendLine(this.EndDocument());

            testDetailsDoc.Text = documentBuilder.ToString();
        }

        private string CreateHorzLine(bool thicker = false)
        {
            var linePara  = new StringBuilder();
            var lineStyle = new StringBuilder();
            lineStyle.AppendFormat("margin:{0}px {1}px {2}px {3}px;", 10, 3, 10, 3);

            int factor = thicker ? 2 : 1;

            linePara.AppendFormat("<p style=\"{0}\"><hr style=\"border:{1}px solid gray;\" size=\"{2}\"/></p>", 
                lineStyle, 2 * factor, 2 * factor);

            return linePara.ToString();
        }

        private string CreateResultTable(IList<SvgTestCategory> testCategories)
        {
            var resultTable = new StringBuilder();
            var tableStyle = new StringBuilder();
            tableStyle.Append("border:1px solid gray;");
            tableStyle.AppendFormat("margin:{0}px {1}px {2}px {3}px;", 0, 16, 16, 16);
            tableStyle.Append("width:95%;");
            resultTable.AppendLine(string.Format(
                "<table style=\"{0}\" border=\"1\" cellpadding=\"3\" cellspacing=\"0\">", tableStyle));

            tableStyle.Length = 0;
            var headerGroup = new StringBuilder();
            tableStyle.Append("border-color:lightgray;");
            headerGroup.AppendLine(string.Format("<tr style=\"{0}\">", tableStyle));

            headerGroup.AppendLine(CreateHeaderCell("Category", false, false));
            headerGroup.AppendLine(CreateHeaderCell("Failure", false, false));
            headerGroup.AppendLine(CreateHeaderCell("Success", false, false));
            headerGroup.AppendLine(CreateHeaderCell("Partial", false, false));
            headerGroup.AppendLine(CreateHeaderCell("Unknown", false, false));
            headerGroup.AppendLine(CreateHeaderCell("Total", true, false));

            headerGroup.AppendLine("</tr>");
            resultTable.AppendLine(headerGroup.ToString());

            for (int i = 0; i < testCategories.Count; i++)
            {
                SvgTestCategory testCategory = testCategories[i];
                if (!testCategory.IsValid)
                {
                    continue;
                }

                var resultGroup = new StringBuilder();
                resultGroup.AppendLine("<tr>");

                bool lastBottom = (i == (testCategories.Count - 1));

                resultGroup.AppendLine(CreateCell(testCategory.Label, false, lastBottom));
                resultGroup.AppendLine(CreateCell(testCategory.Failures, false, lastBottom));
                resultGroup.AppendLine(CreateCell(testCategory.Successes, false, lastBottom));
                resultGroup.AppendLine(CreateCell(testCategory.Partials, false, lastBottom));
                resultGroup.AppendLine(CreateCell(testCategory.Unknowns, false, lastBottom));
                resultGroup.AppendLine(CreateCell(testCategory.Total, true, lastBottom));

                resultGroup.AppendLine("</tr>");
                resultTable.AppendLine(resultGroup.ToString());
            }

            resultTable.AppendLine("</table>");
            return resultTable.ToString();
        }

        private string CreateSummaryTable()
        {
            int resultCount = _testResults.Count;

            var summaryTable = new StringBuilder();
            var tableStyle   = new StringBuilder();
            tableStyle.Append("border:1px solid gray;");
            tableStyle.AppendFormat("margin:{0}px {1}px {2}px {3}px;", 0, 16, 16, 16);
            tableStyle.Append("width:95%;");
            summaryTable.AppendLine(string.Format(
                "<table style=\"{0}\" border=\"1\" cellpadding=\"3\" cellspacing=\"0\">", tableStyle));

            tableStyle.Length = 0;
            var headerGroup = new StringBuilder();
            tableStyle.Append("border-color:lightgray;");
            headerGroup.AppendLine(string.Format("<tr style=\"{0}\">", tableStyle));

            headerGroup.AppendLine(CreateHeaderCell("Category", false, false, true));
            headerGroup.AppendLine(CreateHeaderCell("Total", false, false, true));

            for (int i = 0; i < resultCount; i++)
            {
                SvgTestResult testResult = _testResults[i];
                headerGroup.AppendLine(CreateHeaderCellEx(testResult.Version,
                    (i == (resultCount - 1)), false, true));
            }
            headerGroup.AppendLine("</tr>");

            summaryTable.AppendLine(headerGroup.ToString());

            int[] successValues = new int[resultCount];
            int totalValue = 0;

            // Start with color of newer vesions
            string[] changedBrushes = {
                "lightsalmon",    // Brushes.LightSalmon
                "lightseagreen",  // Brushes.LightSeaGreen  - Version 1.2
                "lightskyblue",   // Brushes.LightSkyBlue   - Version 1.1
                "lightpink",      // Brushes.LightPink
                "lightsteelblue", // Brushes.LightSteelBlue
                "lightskyblue",   // Brushes.LightSkyBlue
                "lightskyblue"    // Brushes.LightSkyBlue
            };

            for (int k = 0; k < _categoryLabels.Count; k++)
            {
                var resultCells = new List<string>();

                bool lastBottom = (k == (_categoryLabels.Count - 1));

                resultCells.Add(CreateCell(_categoryLabels[k], false, lastBottom));

                for (int i = 0; i < resultCount; i++)
                {
                    SvgTestResult testResult = _testResults[i];

                    IList<SvgTestCategory> testCategories = testResult.Categories;

                    SvgTestCategory testCategory = testCategories[k];
                    if (!testCategory.IsValid)
                    {
                        continue;
                    }

                    int total = testCategory.Total;

                    if (i == 0)
                    {
                        resultCells.Add(CreateCell(total, false, lastBottom));
                    }

                    successValues[i] = testCategory.Successes;
                    totalValue       = total;
                    bool lastRight = (i == (resultCount - 1));

                    double percentValue = Math.Round(testCategory.Successes * 100.0d / total, 2);

                    resultCells.Add(CreateCellSummary(percentValue.ToString("00.00"), lastRight, lastBottom));
                }

                int cellCount = resultCells.Count;
                var cellBackgrounds = new string[cellCount];

                if (IsAllZero(successValues))
                {
                    for (int i = 1; i < cellCount; i++)
                    {
                        cellBackgrounds[i] = "background-color:palevioletred;"; // Brushes.PaleVioletRed;
                    }
                }
                else if (IsAllDone(successValues, totalValue))
                {
                    for (int i = 1; i < cellCount; i++)
                    {
                        cellBackgrounds[i] = "background-color:silver;"; // Brushes.Silver
                    }
                }
                else
                {
                    if (IsNowDone(successValues, totalValue))
                    {
                        for (int i = 1; i < cellCount; i++)
                        {
                            cellBackgrounds[i] = "background-color:silver;"; // Brushes.Silver
                        }
                    }

                    if (resultCount > 1 )
                    {
                        int i = 0;
                        for (int j = (resultCount - 1); j >= 1; j--)
                        {
                            var selectedBrush = string.Format("background-color:{0};", changedBrushes[i]);

                            i++;
                            if (IsBetterResult(successValues, j))
                            {
                                cellBackgrounds[cellCount - i] = selectedBrush; // selectedBrush
                            }
                        }
                    }
                }

                var resultGroup = new StringBuilder();
                resultGroup.AppendLine("<tr>");
                for (int i = 0; i < cellCount; i++)
                {
                    string resultRow = resultCells[i];
                    var cellBackground = cellBackgrounds[i];
                    if (string.IsNullOrWhiteSpace(cellBackground))
                    {
                        resultRow = resultRow.Replace("background-color:transparent;", string.Empty);
                    }
                    else
                    {
                        resultRow = resultRow.Replace("background-color:transparent;", cellBackground);
                    }
                    resultGroup.AppendLine(resultRow);
                }
                resultGroup.AppendLine("</tr>");

                summaryTable.AppendLine(resultGroup.ToString());
            }

            summaryTable.AppendLine("</table>");
            return summaryTable.ToString();
        }

        private static bool IsAllZero(int[] successValues)
        {
            if (successValues == null || successValues.Length == 0)
            {
                return true;
            }
            foreach (int successValue in successValues)
            {
                if (successValue != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsNowDone(int[] successValues, int totalValue)
        {
            if (successValues == null || successValues.Length == 0)
            {
                return false;
            }
            
            if (successValues[successValues.Length - 1] != totalValue)
            {
                return false;
            }
            return true;
        }

        private static bool IsAllDone(int[] successValues, int totalValue)
        {
            if (successValues == null || successValues.Length == 0)
            {
                return false;
            }
            foreach (int successValue in successValues)
            {
                if (successValue != totalValue)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsBetterResult(int[] successValues, int index)
        {
            if (index <= 0 || successValues == null || successValues.Length == 0 || index >= successValues.Length)
            {
                return false;
            }
            int successPrev = successValues[index - 1];
            int successNext = successValues[index];

            return successNext > successPrev;
        }

        private string CreateAlert(string title, string message)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "NOTE";
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                return null;
            }

            var tableStyle = new StringBuilder();
            tableStyle.Append("border-color:dimgray;");
            tableStyle.Append("border-width:1px;");
            tableStyle.AppendFormat("margin:{0}px {1}px {2}px {3}px;width:90%;", 0, 24, 16, 24);

            var alertTable = new StringBuilder();
            alertTable.AppendLine(string.Format(
                "<table style=\"{0}\" border=\"1\" cellpadding=\"3\" cellspacing=\"0\">", tableStyle));

            tableStyle.Length = 0;

            tableStyle.Append("background-color:dimgray;");
            tableStyle.Append("color:white;");

            var headerGroup = new StringBuilder();
            headerGroup.AppendLine(string.Format("<tr style=\"{0}\">", tableStyle));
            headerGroup.AppendLine(CreateCell(title, 0, 0, true, false, true, true, 16));
            headerGroup.AppendLine("</tr>");

            alertTable.Append(headerGroup.ToString());

            var alertGroup = new StringBuilder();
            alertGroup.AppendLine("<tr>");
            alertGroup.AppendLine(CreateCell(message, 0, 0, true, true, false, false));
            alertGroup.AppendLine("</tr>");

            alertTable.Append(alertGroup.ToString());
            alertTable.AppendLine("</table>");

            return alertTable.ToString();
        }

        private string CreateHeaderCellEx(string text, bool lastRight, bool lastBottom, bool boldText = true, bool filled = true)
        {
            var tableStyle = new StringBuilder();
            tableStyle.Append("border-color:gray;");
            tableStyle.AppendFormat("border-width:{0}px {1}px {2}px {3}px;", 0, lastRight ? 0 : 1, lastBottom ? 0 : 1, 0);
            if (filled)
            {
                tableStyle.Append("background-color:lightgray;");
            }
            if (boldText)
            {
                tableStyle.Append("font-weight:bold;");
            }
            tableStyle.Append("text-align:center;");

            var tableCell = new StringBuilder();
            tableCell.AppendFormat("<td style=\"{0}\">{1}<br/>(%)</td>", tableStyle, text);

            return tableCell.ToString();
        }

        private string CreateHeaderCell(string text, bool lastRight, bool lastBottom, bool boldText = true, bool filled = true)
        {
            var tableStyle = new StringBuilder();
            tableStyle.Append("border-color:gray;");
            tableStyle.AppendFormat("border-width:{0}px {1}px {2}px {3}px;", 0, lastRight ? 0 : 1, lastBottom ? 0 : 1, 0);
            if (filled)
            {
                tableStyle.Append("background-color:lightgray;");
            }
            if (boldText)
            {
                tableStyle.Append("font-weight:bold;");
            }

            var tableCell = new StringBuilder();
            tableCell.AppendFormat("<td style=\"{0}\">{1}</td>", tableStyle, text);

            return tableCell.ToString();
        }

        private string CreateCell(string text, int colSpan, int rowSpan,
            bool lastRight, bool lastBottom, bool filled, bool boldText, int fontSize = 0)
        {
            var tableStyle = new StringBuilder();
            if (filled)
            {
                tableStyle.Append("background-color:dimgray;");
            }
            tableStyle.Append("border-color:dimgray;");
            tableStyle.AppendFormat("border-width:{0}px {1}px {2}px {3}px;", 0, lastRight ? 0 : 1, lastBottom ? 0 : 1, 0);
            if (boldText)
            {
                tableStyle.Append("font-weight:bold;");
            }
            if (fontSize > 1)
            {
                tableStyle.AppendFormat("font-size:{0}px;", fontSize);
            }
            tableStyle.Append("white-space:nowrap;");

            var tableCell = new StringBuilder();
            tableCell.AppendFormat("<td style=\"{0}\"", tableStyle);
            if (colSpan > 0)
            {
                tableCell.AppendFormat(" colspan={0}", colSpan);
            }
            if (rowSpan > 0)
            {
                tableCell.AppendFormat(" rowspan={0}", rowSpan);
            }
            tableCell.AppendFormat(">{0}</td>", text);

            return tableCell.ToString();
        }

        private string CreateCellSummary(string text, bool lastRight, bool lastBottom)
        {
            var tableStyle = new StringBuilder();
            tableStyle.Append("background-color:transparent;");
            tableStyle.Append("border-color:gray;");
            tableStyle.AppendFormat("border-width:{0}px {1}px {2}px {3}px;", 0, lastRight ? 0 : 1, lastBottom ? 0 : 1, 0);
            tableStyle.Append("white-space:nowrap;");

            var tableCell = new StringBuilder();
            tableCell.AppendFormat("<td style=\"{0}\">{1}</td>", tableStyle, text);

            return tableCell.ToString();
        }

        private string CreateCell(string text, bool lastRight, bool lastBottom,
            bool boldText = false, bool filled = true)
        {
            var tableStyle = new StringBuilder();
            if (filled)
            {
                tableStyle.Append("background-color:lightgray;");
            }
            tableStyle.Append("border-color:gray;");
            tableStyle.AppendFormat("border-width:{0}px {1}px {2}px {3}px;", 0, lastRight ? 0 : 1, lastBottom ? 0 : 1, 0);
            tableStyle.Append("white-space:nowrap;");
            if (boldText)
            {
                tableStyle.Append("font-weight:bold;");
            }

            var tableCell = new StringBuilder();
            tableCell.AppendFormat("<td style=\"{0}\">{1}</td>", tableStyle, text);

            return tableCell.ToString();
        }

        private string CreateCell(int number, bool lastRight, bool lastBottom, bool boldText = false)
        {
            var tableStyle = new StringBuilder();
            tableStyle.Append("border-color:gray;");
            tableStyle.AppendFormat("border-width:{0}px {1}px {2}px {3}px;", 0, lastRight ? 0 : 1, lastBottom ? 0 : 1, 0);
            if (boldText)
            {
                tableStyle.Append("font-weight:bold;");
            }

            var tableCell = new StringBuilder();
            tableCell.AppendFormat("<td style=\"{0}\">{1}</td>", tableStyle, number);

            return tableCell.ToString();
        }

        #endregion
    }
}
