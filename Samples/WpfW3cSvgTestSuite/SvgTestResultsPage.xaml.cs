using System;
using System.Collections.Generic;

using System.Windows;
using System.Printing;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace WpfW3cSvgTestSuite
{
    /// <summary>
    /// Interaction logic for SvgTestResultsPage.xaml
    /// </summary>
    public partial class SvgTestResultsPage : Page
    {
        #region Private Fields

        private MainWindow _mainWindow;

        private IList<string> _categoryLabels;
        private IList<SvgTestResult> _testResults;

        #endregion

        #region Constructors and Destructor

        public SvgTestResultsPage()
        {
            InitializeComponent();

            this.Loaded += OnWindowLoaded;
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

        public MainWindow MainWindow
        {
            get {
                return _mainWindow;
            }
            set {
                _mainWindow = value;
            }
        }

        #endregion

        #region Private Methods

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            var pageSize = new PageMediaSize(PageMediaSizeName.ISOA4);

            if (pageSize.Width != null)
            {
                testDetailsDoc.ColumnWidth = pageSize.Width.Value;
            }

            if (_mainWindow != null)
            {
                _testResults = _mainWindow.TestResults;
            }

            this.CreateDocument();
        }

        private void CreateDocument()
        {
            if (testDetailsDoc.Blocks.Count != 0)
            {
                var introBlock = testDetailsDoc.Blocks.FirstBlock;
                if (testDetailsDoc.Blocks.Count > 1)
                {
                    testDetailsDoc.Blocks.Clear();

                    testDetailsDoc.Blocks.Add(introBlock);
                }
            }            

            if (_testResults == null || _testResults.Count == 0)
            {
                testDetailsDoc.Blocks.Clear();
                return;
            }

            Section noteSection = new Section();
            //noteSection.Blocks.Add(CreateAlert("Note: Test Suite",
            //    "These tests are based on SVG 1.1 First Edition Test Suite: 13 December 2006 (Full)."));

            this.CreateHorzLine(noteSection, false);

            testDetailsDoc.Blocks.Add(noteSection);

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

                Section titleSection = new Section();
                string headingText = string.Format("{0}. Test Results: SharpVectors Version {1}", (i + 1), testResult.Version);
                Paragraph titlePara = new Paragraph();
                titlePara.FontWeight = FontWeights.Bold;
                titlePara.FontSize = 18;
                titlePara.Inlines.Add(new Run(headingText));
                titleSection.Blocks.Add(titlePara);

                Paragraph datePara = new Paragraph();
                datePara.FontWeight = FontWeights.Bold;
                datePara.FontSize = 16;
                datePara.TextAlignment = TextAlignment.Center;
                datePara.Padding = new Thickness(3);
                datePara.Inlines.Add(new Run("Test Date: " + testResult.Date.ToString()));
                titleSection.Blocks.Add(datePara);

                testDetailsDoc.Blocks.Add(titleSection);

                Section resultSection = new Section();
                Table resultTable = CreateResultTable(testCategories);

                resultSection.Blocks.Add(resultTable);

                if (resultCount > 1)
                {
                    this.CreateHorzLine(resultSection, (i == (resultCount - 1)));
                }
                else
                {
                    this.CreateHorzLine(resultSection);
                }

                testDetailsDoc.Blocks.Add(resultSection);
            }

            if (resultCount > 1)
            {
                Section summarySection = new Section();
                Paragraph summaryPara = new Paragraph();
                summaryPara.FontWeight = FontWeights.Bold;
                summaryPara.Margin = new Thickness(3, 3, 3, 10);
                summaryPara.FontSize = 18;
                summaryPara.Inlines.Add(new Run("Test Results: Summary"));
                summarySection.Blocks.Add(summaryPara);

                Table summaryTable = CreateSummaryTable();

                summarySection.Blocks.Add(summaryTable);

                summarySection.Blocks.Add(CreateAlert("Note: Percentage",
                    "The percentage calculations do not include partial success cases."));
                this.CreateHorzLine(summarySection, true);

                testDetailsDoc.Blocks.Add(summarySection);
            }

            Section endSection = new Section();
            Paragraph endPara = new Paragraph();
            endPara.Inlines.Add(new LineBreak());
            endSection.Blocks.Add(endPara);

            testDetailsDoc.Blocks.Add(endSection);
        }

        private void CreateHorzLine(Section section, bool thicker = false)
        {
            Paragraph linePara = new Paragraph();
            linePara.Margin = new Thickness(3, 10, 3, 10);

            int factor = thicker ? 2 : 1;

            var horzLine = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = 1000,
                Y2 = 0,
                Stroke = Brushes.DimGray,
                StrokeThickness = 2 * factor
            };

            linePara.Inlines.Add(horzLine);

            section.Blocks.Add(linePara);
        }

        private Table CreateResultTable(IList<SvgTestCategory> testCategories)
        {
            Table resultTable = new Table();
            resultTable.CellSpacing = 0;
            resultTable.BorderBrush = Brushes.Gray;
            resultTable.BorderThickness = new Thickness(1);
            resultTable.Margin = new Thickness(16, 0, 16, 16);

            TableColumn categoryCol = new TableColumn();
            TableColumn failureCol = new TableColumn();
            TableColumn successCol = new TableColumn();
            TableColumn partialCol = new TableColumn();
            TableColumn unknownCol = new TableColumn();
            TableColumn totalCol = new TableColumn();

            categoryCol.Width = new GridLength(2, GridUnitType.Star);
            failureCol.Width = new GridLength(1, GridUnitType.Star);
            successCol.Width = new GridLength(1, GridUnitType.Star);
            partialCol.Width = new GridLength(1, GridUnitType.Star);
            unknownCol.Width = new GridLength(1, GridUnitType.Star);
            totalCol.Width = new GridLength(1, GridUnitType.Star);

            resultTable.Columns.Add(categoryCol);
            resultTable.Columns.Add(failureCol);
            resultTable.Columns.Add(successCol);
            resultTable.Columns.Add(partialCol);
            resultTable.Columns.Add(unknownCol);
            resultTable.Columns.Add(totalCol);

            TableRowGroup headerGroup = new TableRowGroup();
            headerGroup.Background = Brushes.LightGray;
            TableRow headerRow = new TableRow();

            headerRow.Cells.Add(CreateHeaderCell("Category", false, false));
            headerRow.Cells.Add(CreateHeaderCell("Failure", false, false));
            headerRow.Cells.Add(CreateHeaderCell("Success", false, false));
            headerRow.Cells.Add(CreateHeaderCell("Partial", false, false));
            headerRow.Cells.Add(CreateHeaderCell("Unknown", false, false));
            headerRow.Cells.Add(CreateHeaderCell("Total", true, false));

            headerGroup.Rows.Add(headerRow);
            resultTable.RowGroups.Add(headerGroup);

            for (int i = 0; i < testCategories.Count; i++)
            {
                SvgTestCategory testCategory = testCategories[i];
                if (!testCategory.IsValid)
                {
                    continue;
                }

                TableRowGroup resultGroup = new TableRowGroup();
                TableRow resultRow = new TableRow();

                bool lastBottom = (i == (testCategories.Count - 1));

                resultRow.Cells.Add(CreateCell(testCategory.Label, false, lastBottom));
                resultRow.Cells.Add(CreateCell(testCategory.Failures, false, lastBottom));
                resultRow.Cells.Add(CreateCell(testCategory.Successes, false, lastBottom));
                resultRow.Cells.Add(CreateCell(testCategory.Partials, false, lastBottom));
                resultRow.Cells.Add(CreateCell(testCategory.Unknowns, false, lastBottom));
                resultRow.Cells.Add(CreateCell(testCategory.Total, true, lastBottom));

                resultGroup.Rows.Add(resultRow);
                resultTable.RowGroups.Add(resultGroup);
            }

            return resultTable;
        }

        private Table CreateSummaryTable()
        {
            int resultCount = _testResults.Count;

            Table summaryTable = new Table();
            summaryTable.CellSpacing = 0;
            summaryTable.BorderBrush = Brushes.Gray;
            summaryTable.BorderThickness = new Thickness(1);
            summaryTable.Margin = new Thickness(16, 0, 16, 16);

            TableColumn categoryCol = new TableColumn();
            categoryCol.Width = new GridLength(2, GridUnitType.Star);
            summaryTable.Columns.Add(categoryCol);

            TableColumn totalCol = new TableColumn();
            totalCol.Width = new GridLength(1, GridUnitType.Star);
            summaryTable.Columns.Add(totalCol);

            for (int i = 0; i < resultCount; i++)
            {
                TableColumn successCol = new TableColumn();
                successCol.Width = new GridLength(1, GridUnitType.Star);
                summaryTable.Columns.Add(successCol);
            }

            TableRowGroup headerGroup = new TableRowGroup();
            headerGroup.Background = Brushes.LightGray;
            TableRow headerRow = new TableRow();

            headerRow.Cells.Add(CreateHeaderCell("Category", false, false));
            headerRow.Cells.Add(CreateHeaderCell("Total", false, false));

            for (int i = 0; i < resultCount; i++)
            {
                SvgTestResult testResult = _testResults[i];
                headerRow.Cells.Add(CreateHeaderCellEx(testResult.Version,
                    (i == (resultCount - 1)), false));
            }

            headerGroup.Rows.Add(headerRow);
            summaryTable.RowGroups.Add(headerGroup);

            int[] successValues = new int[resultCount];
            int totalValue = 0;

            // Start with color of newer vesions
            Brush[] changedBrushes = {
                Brushes.LightCyan,
                Brushes.LightGoldenrodYellow,
                Brushes.LightGreen,
                Brushes.LightSalmon,
                Brushes.LightSeaGreen,  // Version 1.2
                Brushes.LightSkyBlue,   // Version 1.1
                Brushes.LightPink,
                Brushes.LightSteelBlue,
                Brushes.LightSkyBlue,
                Brushes.LightSkyBlue
            };

            for (int k = 0; k < _categoryLabels.Count; k++)
            {
                TableRowGroup resultGroup = new TableRowGroup();
                TableRow resultRow = new TableRow();

                bool lastBottom = (k == (_categoryLabels.Count - 1));

                resultRow.Cells.Add(CreateCell(_categoryLabels[k], false, lastBottom));

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
                        resultRow.Cells.Add(CreateCell(total, false, lastBottom));
                    }

                    successValues[i] = testCategory.Successes;
                    totalValue       = total;
                    bool lastRight = (i == (resultCount - 1));

                    double percentValue = Math.Round(testCategory.Successes * 100.0d / total, 2);

                    resultRow.Cells.Add(CreateCell(percentValue.ToString("00.00"),
                        lastRight, lastBottom, false, false));
                }

                int cellCount = resultRow.Cells.Count;
                if (IsAllZero(successValues))
                {
                    for (int i = 1; i < cellCount; i++)
                    {
                        resultRow.Cells[i].Background = Brushes.PaleVioletRed;
                    }
                }
                else if (IsAllDone(successValues, totalValue))
                {
                    for (int i = 1; i < cellCount; i++)
                    {
                        resultRow.Cells[i].Background = Brushes.Silver;
                    }
                }
                else
                {
                    if (IsNowDone(successValues, totalValue))
                    {
                        for (int i = 1; i < cellCount; i++)
                        {
                            resultRow.Cells[i].Background = Brushes.Silver;
                        }
                    }

                    if (resultCount > 1 )
                    {
                        int i = 0;
                        for (int j = (resultCount - 1); j >= 1; j--)
                        {
                            var selectedBrush = changedBrushes[i];

                            i++;
                            if (IsBetterResult(successValues, j))
                            {
                                resultRow.Cells[cellCount - i].Background = selectedBrush;
                            }
                        }
                    }
                }

                resultGroup.Rows.Add(resultRow);
                summaryTable.RowGroups.Add(resultGroup);
            }

            return summaryTable;
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

        private Table CreateAlert(string title, string message)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "NOTE";
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                return null;
            }

            Table alertTable = new Table();
            alertTable.CellSpacing = 0;
            alertTable.BorderBrush = Brushes.DimGray;
            alertTable.BorderThickness = new Thickness(1);
            alertTable.Margin = new Thickness(48, 0, 48, 16);

            TableRowGroup headerGroup = new TableRowGroup();
            headerGroup.Background = Brushes.DimGray;
            headerGroup.Foreground = Brushes.White;
            TableRow headerRow = new TableRow();

            headerRow.Cells.Add(CreateCell(title, 0, 0, true, false, false, true, 16));
            headerGroup.Rows.Add(headerRow);
            alertTable.RowGroups.Add(headerGroup);

            TableRowGroup alertGroup = new TableRowGroup();
            TableRow alertRow = new TableRow();

            alertRow.Cells.Add(CreateCell(message, 0, 0, true, true, false, false));

            alertGroup.Rows.Add(alertRow);
            alertTable.RowGroups.Add(alertGroup);

            return alertTable;
        }

        private TableCell CreateHeaderCellEx(string text, bool lastRight, bool lastBottom, bool boldText = true)
        {
            TableCell tableCell = new TableCell();
            tableCell.BorderBrush = Brushes.Gray;
            tableCell.BorderThickness = new Thickness(0, 0, lastRight ? 0 : 1, lastBottom ? 0 : 1);

            Paragraph cellPara = new Paragraph();
            if (boldText)
            {
                cellPara.FontWeight = FontWeights.Bold;
            }
            cellPara.Inlines.Add(new Run(text));
            cellPara.Inlines.Add(new LineBreak());
            cellPara.Inlines.Add(new Run("(%)"));

            cellPara.TextAlignment = TextAlignment.Center;

            tableCell.Blocks.Add(cellPara);

            return tableCell;
        }

        private TableCell CreateHeaderCell(string text, bool lastRight, bool lastBottom, bool boldText = true)
        {
            TableCell tableCell = new TableCell();
            tableCell.BorderBrush = Brushes.Gray;
            tableCell.BorderThickness = new Thickness(0, 0, lastRight ? 0 : 1, lastBottom ? 0 : 1);

            Paragraph cellPara = new Paragraph();
            if (boldText)
            {
                cellPara.FontWeight = FontWeights.Bold;
            }
            cellPara.Inlines.Add(new Run(text));

            tableCell.Blocks.Add(cellPara);

            return tableCell;
        }

        private TableCell CreateCell(string text, int colSpan, int rowSpan,
            bool lastRight, bool lastBottom, bool filled, bool boldText, int fontSize = 0)
        {
            TableCell tableCell = new TableCell();
            if (filled)
            {
                tableCell.Background = Brushes.DimGray;
            }
            tableCell.BorderBrush = Brushes.DimGray;
            tableCell.BorderThickness = new Thickness(0, 0, lastRight ? 0 : 1, lastBottom ? 0 : 1);

            if (colSpan > 0)
            {
                tableCell.ColumnSpan = colSpan;
            }
            if (rowSpan > 0)
            {
                tableCell.RowSpan = rowSpan;
            }

            Paragraph cellPara = new Paragraph();
            cellPara.KeepTogether = true;
            if (boldText)
            {
                cellPara.FontWeight = FontWeights.Bold;
            }
            if (fontSize > 1)
            {
                cellPara.FontSize = fontSize;
            }
            cellPara.Inlines.Add(new Run(text));

            tableCell.Blocks.Add(cellPara);

            return tableCell;
        }

        private TableCell CreateCell(string text, bool lastRight, bool lastBottom,
            bool boldText = false, bool filled = true)
        {
            TableCell tableCell = new TableCell();
            if (filled)
            {
                tableCell.Background = Brushes.LightGray;
            }
            tableCell.BorderBrush = Brushes.Gray;
            tableCell.BorderThickness = new Thickness(0, 0, lastRight ? 0 : 1, lastBottom ? 0 : 1);

            Paragraph cellPara = new Paragraph();
            cellPara.KeepTogether = true;
            if (boldText)
            {
                cellPara.FontWeight = FontWeights.Bold;
            }
            cellPara.Inlines.Add(new Run(text));

            tableCell.Blocks.Add(cellPara);

            return tableCell;
        }

        private TableCell CreateCell(int number, bool lastRight, bool lastBottom, bool boldText = false)
        {
            TableCell tableCell = new TableCell();
            tableCell.BorderBrush = Brushes.Gray;
            tableCell.BorderThickness = new Thickness(0, 0, lastRight ? 0 : 1, lastBottom ? 0 : 1);

            Paragraph cellPara = new Paragraph();
            cellPara.Inlines.Add(new Run(number.ToString()));
            if (boldText)
            {
                cellPara.FontWeight = FontWeights.Bold;
            }

            tableCell.Blocks.Add(cellPara);

            return tableCell;
        }

        #endregion
    }
}
