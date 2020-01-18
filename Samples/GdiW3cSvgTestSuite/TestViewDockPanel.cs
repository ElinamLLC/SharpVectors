using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace GdiW3cSvgTestSuite
{
    public partial class TestViewDockPanel : DockPanelContent
    {
        #region Private Fields

        private bool _isTreeModified;
        private bool _isTreeChangedPending;

        private bool _isSuiteAvailable;

        private string _suitePath;
        private string _svgPath;
        private string _pngPath;

        private string _testFilePath;
        private string _testResultsPath;

        private IList<SvgTestResult> _testResults;
        private IList<ITestPagePanel> _testPages;

        #endregion

        #region Constructors and Destructor

        public TestViewDockPanel()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.DockAreas     = DockAreas.DockLeft | DockAreas.Float;

            this.CloseButton        = false;
            this.CloseButtonVisible = false;

            stateExpander.IsExpanded = false;
            stateExpander.HeaderFont = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.World, 0);

            treeImageList.Images.Add(Properties.Resources.FolderClose);
            treeImageList.Images.Add(Properties.Resources.FolderOpen);

            treeImageList.Images.Add(Properties.Resources.Unknown);
            treeImageList.Images.Add(Properties.Resources.Failure);
            treeImageList.Images.Add(Properties.Resources.Success);
            treeImageList.Images.Add(Properties.Resources.Partial);

            // Set the TreeView control's default image and selected image indexes: Closed Folder
            treeView.Font = new Font("Verdana", 12F, FontStyle.Regular, GraphicsUnit.World, 0);
            treeView.ImageIndex         = 0;
            treeView.SelectedImageIndex = 0;

            _testPages = new List<ITestPagePanel>();

            stateComboBox.Items.Add(new ComboBoxItem("Unknown", Properties.Resources.Unknown));
            stateComboBox.Items.Add(new ComboBoxItem("Failure", Properties.Resources.Failure));
            stateComboBox.Items.Add(new ComboBoxItem("Success", Properties.Resources.Success));
            stateComboBox.Items.Add(new ComboBoxItem("Partial", Properties.Resources.Partial));

            stateComboBox.SelectedIndex = 0;
        }

        #endregion

        #region Public Properties

        public string SuitePath
        {
            get {
                return _suitePath;
            }
        }

        public IList<SvgTestResult> TestResults
        {
            get {
                return _testResults;
            }
        }

        public IList<ITestPagePanel> Pages
        {
            get {
                if (_testPages == null)
                {
                    _testPages = new List<ITestPagePanel>();
                }
                return _testPages;
            }
        }

        #endregion

        #region Public Methods

        public void InitializePath(string suitePath)
        {
            if (string.IsNullOrWhiteSpace(suitePath) || !Directory.Exists(suitePath))
            {
                return;
            }

            string svgPath = Path.Combine(suitePath, "svg");
            if (!Directory.Exists(svgPath))
            {
                return;
            }

            string pngPath = Path.Combine(suitePath, "png");
            if (!Directory.Exists(pngPath))
            {
                return;
            }

            var selectedSuite = _optionSettings.SelectedTestSuite;
            if (selectedSuite == null)
            {
                return;
            }

            _svgPath = svgPath;
            _pngPath = pngPath;

            _suitePath = suitePath;
            _isSuiteAvailable = true;

            _testResults = new List<SvgTestResult>();

            _testResultsPath = Path.GetFullPath(Path.Combine("..\\", selectedSuite.ResultFileName));
            if (!string.IsNullOrWhiteSpace(_testResultsPath) && File.Exists(_testResultsPath))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace             = false;
                settings.IgnoreComments               = true;
                settings.IgnoreProcessingInstructions = true;

                using (XmlReader reader = XmlReader.Create(_testResultsPath, settings))
                {
                    LoadTestResults(reader);
                }
            }

            string fullFilePath = Path.GetFullPath(Path.Combine("..\\", selectedSuite.TestFileName));
            if (!string.IsNullOrWhiteSpace(fullFilePath) && File.Exists(fullFilePath))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace             = false;
                settings.IgnoreComments               = true;
                settings.IgnoreProcessingInstructions = true;

                using (XmlReader reader = XmlReader.Create(fullFilePath, settings))
                {
                    LoadTreeView(reader);
                }

                _testFilePath = fullFilePath;
            }
        }

        #endregion

        #region Private Event Handlers

        private void OnFormLoad(object sender, EventArgs e)
        {
            stateExpander.Enabled = false;
            _isTreeChangedPending = false;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isTreeModified || string.IsNullOrWhiteSpace(_testFilePath) ||
                !File.Exists(_testFilePath))
            {
                return;
            }

            string backupFile = Path.ChangeExtension(_testFilePath, ".bak");
            try
            {
                if (File.Exists(backupFile))
                {
                    File.Delete(backupFile);
                }
                File.Move(_testFilePath, backupFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), MainForm.AppErrorTitle, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "    ";
                settings.Encoding = Encoding.UTF8;

                using (XmlWriter writer = XmlWriter.Create(_testFilePath, settings))
                {
                    this.SaveTreeView(writer);
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(backupFile))
                {
                    File.Move(backupFile, _testFilePath);
                }

                MessageBox.Show(ex.ToString(), MainForm.AppErrorTitle, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!string.IsNullOrWhiteSpace(backupFile) && File.Exists(backupFile))
            {
                File.Delete(backupFile);
            }

            if (!string.IsNullOrWhiteSpace(_testResultsPath))
            {
                if (!string.IsNullOrWhiteSpace(_testResultsPath))
                {
                    if (File.Exists(_testResultsPath))
                    {
                        backupFile = Path.ChangeExtension(_testResultsPath, ".bak");
                        try
                        {
                            if (File.Exists(backupFile))
                            {
                                File.Delete(backupFile);
                            }
                            File.Move(_testResultsPath, backupFile);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), MainForm.AppErrorTitle, 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    try
                    {
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent      = true;
                        settings.IndentChars = "    ";
                        settings.Encoding    = Encoding.UTF8;

                        using (XmlWriter writer = XmlWriter.Create(_testResultsPath, settings))
                        {
                            this.SaveTestResults(writer);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!string.IsNullOrWhiteSpace(backupFile) && File.Exists(backupFile))
                        {
                            File.Move(backupFile, _testResultsPath);
                        }

                        MessageBox.Show(ex.ToString(), MainForm.AppErrorTitle, 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (!string.IsNullOrWhiteSpace(backupFile) && File.Exists(backupFile))
                    {
                        File.Delete(backupFile);
                    }
                }
            }
        }

        private void OnFormShown(object sender, EventArgs e)
        {

        }

        private void OnTreeAfterCollapse(object sender, TreeViewEventArgs e)
        {
            TreeNode treeItem = e.Node;
            if (treeItem == null)
            {
                return;
            }

            treeItem.ImageIndex = 0;
            treeItem.SelectedImageIndex = 0;
        }

        private void OnTreeBeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void OnTreeAfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeNode treeItem = e.Node;
            if (treeItem == null)
            {
                return;
            }

            treeItem.ImageIndex = 1;
            treeItem.SelectedImageIndex = 1;
        }

        private void OnTreeBeforeExpand(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void OnTreeAfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selItem = treeView.SelectedNode as TreeNode;
            if (selItem == null || selItem.Tag == null)
            {
                EnableTestPanel(false);
                return;
            }

            SvgTestInfo testItem = selItem.Tag as SvgTestInfo;
            if (testItem == null)
            {
                EnableTestPanel(false);
                return;
            }

            if (!_isSuiteAvailable)
            {
                EnableTestPanel(false);
                return;
            }

            string svgFilePath = Path.Combine(_suitePath, "svg\\" + testItem.FileName);
            if (!File.Exists(svgFilePath))
            {
                EnableTestPanel(false);
                return;
            }
            string pngFilePath = Path.Combine(_suitePath,
                "png\\" + Path.ChangeExtension(testItem.FileName, ".png"));
            if (!File.Exists(pngFilePath))
            {
                EnableTestPanel(false);
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (_testPages != null && _testPages.Count != 0)
                {
                    foreach (var page in _testPages)
                    {
                        if (page != null && page.IsDisposed == false)
                        {
                            page.LoadDocument(svgFilePath, testItem, pngFilePath);
                        }
                    }
                }

                stateComboBox.SelectedIndex = (int)testItem.State;
                testComment.Text = testItem.Comment;

                _isTreeChangedPending   = false;
                testApplyButton.Enabled = false;
                EnableTestPanel(true);

                if (_optionSettings != null)
                {
                    _optionSettings.SelectedValuePath = testItem.Path;
                }
            }
            catch (Exception ex)
            {
                _isTreeChangedPending = false;
                this.Cursor = Cursors.Default;

                MessageBox.Show(ex.ToString(), MainForm.AppErrorTitle, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isTreeChangedPending = false;
                this.Cursor = Cursors.Default;

                treeView.Focus();
            }
        }

        private void OnTreeBeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (_optionSettings != null)
            {
                _optionSettings.SelectedValuePath = "";
            }
            // Prompt for any un-applied modifications to avoid lost.
            if (_isTreeChangedPending)
            {
                TreeNode treeItem = e.Node as TreeNode;
                if (treeItem == null)
                {
                    return;
                }

                if (treeItem != null && treeItem.Tag != null)
                {
                    SvgTestInfo testInfo = treeItem.Tag as SvgTestInfo;
                    if (testInfo != null)
                    {
                        DialogResult boxResult = MessageBox.Show(
                            "The previously selected test item was modified.\nDo you want to apply the current modifications?",
                            "Svg Test Suite",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (boxResult == DialogResult.Yes || boxResult == DialogResult.OK)
                        {
                            this.OnApplyTestState(treeItem);
                        }
                    }
                }

                _isTreeChangedPending = false;
            }
        }

        private void OnApplyTestState(object sender, EventArgs e)
        {
            TreeNode treeItem = treeView.SelectedNode as TreeNode;
            if (treeView == null || treeItem.Tag == null)
            {
                return;
            }

            this.OnApplyTestState(treeItem);
        }

        private void OnStateSelectionChanged(object sender, EventArgs e)
        {
            _isTreeChangedPending   = true;
            testApplyButton.Enabled = true;
        }

        private void OnCommentTextChanged(object sender, EventArgs e)
        {
            _isTreeChangedPending   = true;
            testApplyButton.Enabled = true;
        }

        #endregion

        #region Private Fields

        private void EnableTestPanel(bool isEnabled)
        {
            if (_optionSettings != null && isEnabled == false)
            {
                _optionSettings.SelectedValuePath = "";
            }

            stateExpander.Enabled = isEnabled;
            if (!isEnabled)
            {
                stateComboBox.SelectedIndex = -1;
                testComment.Text = string.Empty;

                _isTreeChangedPending = false;

                if (_testPages != null && _testPages.Count != 0)
                {
                    foreach (var page in _testPages)
                    {
                        if (page != null && page.IsDisposed == false)
                        {
                            page.UnloadDocument();
                        }
                    }
                }
            }

            treeView.Focus();
        }

        private void OnApplyTestState(TreeNode treeItem)
        {
            SvgTestInfo testInfo = treeItem.Tag as SvgTestInfo;
            if (testInfo == null)
            {
                return;
            }

            int selIndex = stateComboBox.SelectedIndex;
            if (selIndex < 0)
            {
                return;
            }

            testInfo.State = (SvgTestState)selIndex;
            testInfo.Comment = testComment.Text;

            treeItem.ImageIndex = testInfo.ImageIndex;
            treeItem.SelectedImageIndex = testInfo.ImageIndex;

            if (!_isTreeModified)
            {
                this.Text = this.Text + " *";

                _isTreeModified = true;
            }

            _isTreeChangedPending   = false;
            testApplyButton.Enabled = false;
        }

        private void LoadTestResults(XmlReader reader)
        {
            if (_testResults == null || _testResults.Count != 0)
            {
                _testResults = new List<SvgTestResult>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element &&
                    string.Equals(reader.Name, "result", StringComparison.OrdinalIgnoreCase))
                {
                    SvgTestResult testResult = new SvgTestResult(reader);
                    if (testResult.IsValid)
                    {
                        _testResults.Add(testResult);
                    }
                }
            }
        }

        private void SaveTestResults(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            if (_testResults == null || _testResults.Count == 0)
            {
                return;
            }

            writer.WriteStartDocument();
            writer.WriteStartElement("results");

            for (int i = 0; i < _testResults.Count; i++)
            {
                SvgTestResult testResult = _testResults[i];
                if (testResult != null && testResult.IsValid)
                {
                    testResult.WriteXml(writer);
                }
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        private void LoadTreeView(XmlReader reader)
        {
            SvgTestResult testResult = new SvgTestResult();

            // Suppress repainting the TreeView until all the objects have been created.
            treeView.BeginUpdate();
            // Clear the TreeView each time the method is called.
            treeView.Nodes.Clear();

            string selectedCategory = "";
            string selectedTest     = "";

            if (_optionSettings != null &&
                !string.IsNullOrWhiteSpace(_optionSettings.SelectedValuePath))
            {
                var selectedPaths = _optionSettings.SelectedValuePath.Split('/');
                if (selectedPaths != null && selectedPaths.Length == 2)
                {
                    selectedCategory = selectedPaths[0];
                    selectedTest = selectedPaths[1];
                }
            }
            TreeNode selectedCategoryItem = null;

            var treeFont = this.treeView.Font;
            this.treeView.ItemHeight = 22;
            var categoryFont = new Font(treeFont.FontFamily, treeFont.Size + 1, FontStyle.Bold, GraphicsUnit.Pixel);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element &&
                    string.Equals(reader.Name, "category", StringComparison.OrdinalIgnoreCase))
                {
                    // <category label="Animations">
                    string categoryLabel = reader.GetAttribute("label");
                    if (!string.IsNullOrWhiteSpace(categoryLabel))
                    {
                        SvgTestCategory testCategory = new SvgTestCategory(categoryLabel);

                        TreeNode categoryItem = new TreeNode(categoryLabel);
                        categoryItem.NodeFont           = categoryFont;
                        categoryItem.ImageIndex         = 0;
                        categoryItem.SelectedImageIndex = 0;
                        categoryItem.Tag                = testCategory;

                        treeView.Nodes.Add(categoryItem);

                        bool categorySelected = false;
                        if (!string.IsNullOrWhiteSpace(selectedCategory)
                            && selectedCategory.Equals(categoryLabel, StringComparison.OrdinalIgnoreCase))
                        {
                            selectedCategoryItem = categoryItem;
                            categorySelected = true;
                        }
                        LoadTreeViewCategory(reader, categoryItem, testCategory, categorySelected, selectedTest);

                        if (testCategory.IsValid)
                        {
                            testResult.Categories.Add(testCategory);

                        }
                    }
                }
            }

            if (_testResults == null)
            {
                _testResults = new List<SvgTestResult>();
            }

            bool saveResults = false;
            if (testResult.IsValid)
            {
                if (_testResults.Count == 0)
                {
                    _testResults.Add(testResult);

                    saveResults = true;
                }
                else
                {
                    int foundAt = -1;
                    for (int i = 0; i < _testResults.Count; i++)
                    {
                        SvgTestResult nextResult = _testResults[i];
                        if (nextResult != null && nextResult.IsValid)
                        {
                            if (string.Equals(nextResult.Version, testResult.Version))
                            {
                                foundAt = i;
                                break;
                            }
                        }
                    }

                    if (foundAt >= 0)
                    {
                        SvgTestResult nextResult = _testResults[foundAt];

                        if (!SvgTestResult.AreEqual(nextResult, testResult))
                        {
                            _testResults[foundAt] = testResult;
                            saveResults = true;
                        }
                    }
                    else
                    {
                        _testResults.Add(testResult);

                        saveResults = true;
                    }
                }
            }
            if (saveResults)
            {
                if (!string.IsNullOrWhiteSpace(_testResultsPath))
                {
                    string backupFile = null;
                    if (File.Exists(_testResultsPath))
                    {
                        backupFile = Path.ChangeExtension(_testResultsPath, ".bak");
                        try
                        {
                            if (File.Exists(backupFile))
                            {
                                File.Delete(backupFile);
                            }
                            File.Move(_testResultsPath, backupFile);
                        }
                        catch (Exception ex)
                        {
                            if (!string.IsNullOrWhiteSpace(backupFile) && File.Exists(backupFile))
                            {
                                File.Delete(backupFile);
                            }
                            MessageBox.Show(ex.ToString(), MainForm.AppErrorTitle, 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    try
                    {
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent      = true;
                        settings.IndentChars = "    ";
                        settings.Encoding    = Encoding.UTF8;

                        using (XmlWriter writer = XmlWriter.Create(_testResultsPath, settings))
                        {
                            this.SaveTestResults(writer);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!string.IsNullOrWhiteSpace(backupFile) && File.Exists(backupFile))
                        {
                            File.Move(backupFile, _testResultsPath);
                        }

                        MessageBox.Show(ex.ToString(), MainForm.AppErrorTitle, 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (!string.IsNullOrWhiteSpace(backupFile) && File.Exists(backupFile))
                    {
                        File.Delete(backupFile);
                    }
                }
            }

            if (selectedCategoryItem != null)
            {
                selectedCategoryItem.ExpandAll();
            }

            // Begin repainting the TreeView.
            treeView.EndUpdate();
            treeView.Focus();
        }

        private void LoadTreeViewCategory(XmlReader reader, TreeNode categoryItem,
            SvgTestCategory testCategory, bool categorySelected, string selectedTest)
        {
            int total = 0, unknowns = 0, failures = 0, successes = 0, partials = 0;

            int itemCount = 0;

            var comparer = StringComparison.OrdinalIgnoreCase;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (string.Equals(reader.Name, "test", comparer))
                    {
                        SvgTestInfo testInfo = new SvgTestInfo(reader);
                        if (!testInfo.IsEmpty)
                        {
                            testInfo.Category = testCategory.Label;

                            var itemLabel = string.Format("({0:D3}) - {1}", itemCount, testInfo.Title);
                            TreeNode treeItem = new TreeNode(itemLabel);
                            treeItem.ImageIndex         = testInfo.ImageIndex;
                            treeItem.SelectedImageIndex = testInfo.ImageIndex;
                            treeItem.Tag                = testInfo;

                            categoryItem.Nodes.Add(treeItem);

                            if (categorySelected &&
                                string.Equals(testInfo.Title, selectedTest, comparer))
                            {
                                treeItem.Expand();

                                treeView.SelectedNode = treeItem;
                            }

                            itemCount++;

                            total++;
                            SvgTestState testState = testInfo.State;

                            switch (testState)
                            {
                                case SvgTestState.Unknown:
                                    unknowns++;
                                    break;
                                case SvgTestState.Failure:
                                    failures++;
                                    break;
                                case SvgTestState.Success:
                                    successes++;
                                    break;
                                case SvgTestState.Partial:
                                    partials++;
                                    break;
                            }
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (string.Equals(reader.Name, "category", comparer))
                    {
                        break;
                    }
                }
            }

            testCategory.SetValues(total, unknowns, failures, successes, partials);
        }

        private void SaveTreeView(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            TreeNodeCollection treeNodes = treeView.Nodes;
            if (treeNodes == null || treeNodes.Count == 0)
            {
                return;
            }

            SvgTestResult testResult = new SvgTestResult();

            writer.WriteStartDocument();
            writer.WriteStartElement("tests");

            var selectedSuite = _optionSettings.SelectedTestSuite;
            if (selectedSuite != null)
            {
                writer.WriteAttributeString("version", selectedSuite.Version);
                writer.WriteAttributeString("description", selectedSuite.Description);
            }

            for (int i = 0; i < treeNodes.Count; i++)
            {
                TreeNode categoryItem = treeNodes[i] as TreeNode;
                if (categoryItem != null)
                {
                    SvgTestCategory testCategory = new SvgTestCategory(categoryItem.Text);

                    this.SaveTreeViewCategory(writer, categoryItem, testCategory);

                    if (testCategory.IsValid)
                    {
                        testResult.Categories.Add(testCategory);
                    }
                }
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();

            if (_testResults == null)
            {
                _testResults = new List<SvgTestResult>();
            }

            if (testResult.IsValid)
            {
                if (_testResults.Count == 0)
                {
                    _testResults.Add(testResult);
                }
            }
            else
            {
                int foundAt = -1;
                for (int i = 0; i < _testResults.Count; i++)
                {
                    SvgTestResult nextResult = _testResults[i];
                    if (nextResult != null && nextResult.IsValid)
                    {
                        if (string.Equals(nextResult.Version, testResult.Version))
                        {
                            foundAt = i;
                            break;
                        }
                    }
                }

                if (foundAt >= 0)
                {
                    SvgTestResult nextResult = _testResults[foundAt];

                    if (!SvgTestResult.AreEqual(nextResult, testResult))
                    {
                        _testResults[foundAt] = testResult;
                    }
                }
                else
                {
                    _testResults.Add(testResult);
                }
            }
        }

        private void SaveTreeViewCategory(XmlWriter writer, TreeNode categoryItem, SvgTestCategory testCategory)
        {
            int total = 0, unknowns = 0, failures = 0, successes = 0, partials = 0;

            writer.WriteStartElement("category");

            writer.WriteAttributeString("label", testCategory.Label);

            TreeNodeCollection treeItems = categoryItem.Nodes;
            for (int j = 0; j < treeItems.Count; j++)
            {
                TreeNode treeItem = treeItems[j] as TreeNode;
                if (treeItem != null)
                {
                    SvgTestInfo testInfo = treeItem.Tag as SvgTestInfo;
                    if (testInfo != null)
                    {
                        testInfo.WriteXml(writer);

                        total++;
                        SvgTestState testState = testInfo.State;

                        switch (testState)
                        {
                            case SvgTestState.Unknown:
                                unknowns++;
                                break;
                            case SvgTestState.Failure:
                                failures++;
                                break;
                            case SvgTestState.Success:
                                successes++;
                                break;
                            case SvgTestState.Partial:
                                partials++;
                                break;
                        }
                    }
                }
            }

            writer.WriteEndElement();

            testCategory.SetValues(total, unknowns, failures, successes, partials);
        }

        #endregion
    }
}
