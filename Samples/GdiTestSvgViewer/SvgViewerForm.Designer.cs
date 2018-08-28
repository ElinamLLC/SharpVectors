namespace Viewer
{
    partial class SvgViewerForm
    {
        private System.ComponentModel.IContainer components;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnGo = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.miFile = new System.Windows.Forms.MenuItem();
            this.miFileOpenLocal = new System.Windows.Forms.MenuItem();
            this.miFileSeparator1 = new System.Windows.Forms.MenuItem();
            this.miFileExit = new System.Windows.Forms.MenuItem();
            this.miEdit = new System.Windows.Forms.MenuItem();
            this.miEditCopy = new System.Windows.Forms.MenuItem();
            this.miTest = new System.Windows.Forms.MenuItem();
            this.miTestShowRefImage = new System.Windows.Forms.MenuItem();
            this.miTestSeparator1 = new System.Windows.Forms.MenuItem();
            this.miTestSuite = new System.Windows.Forms.MenuItem();
            this.miOptions = new System.Windows.Forms.MenuItem();
            this.miOptionsClearCache = new System.Windows.Forms.MenuItem();
            this.miHelp = new System.Windows.Forms.MenuItem();
            this.miHelpAbout = new System.Windows.Forms.MenuItem();
            this.svgUrlCombo = new System.Windows.Forms.ComboBox();
            this.pbRefImage = new System.Windows.Forms.PictureBox();
            this.svgPictureBox = new SharpVectors.Renderers.Forms.SvgPictureBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.labelSvg = new System.Windows.Forms.Label();
            this.labelPng = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbRefImage)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(422, 9);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(58, 28);
            this.btnGo.TabIndex = 0;
            this.btnGo.Text = "&Go";
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "svg";
            this.openFileDialog1.Filter = "SVG Files|*.svg;*.svgz|All Files|*.*";
            this.openFileDialog1.Title = "Select an SVG File";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // toolBar1
            // 
            this.toolBar1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.Location = new System.Drawing.Point(0, 0);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new System.Drawing.Size(913, 43);
            this.toolBar1.TabIndex = 22;
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 651);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(913, 26);
            this.statusBar.TabIndex = 23;
            this.statusBar.Text = "Welcome";
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miFile,
            this.miEdit,
            this.miTest,
            this.miOptions,
            this.miHelp});
            // 
            // miFile
            // 
            this.miFile.Index = 0;
            this.miFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miFileOpenLocal,
            this.miFileSeparator1,
            this.miFileExit});
            this.miFile.Text = "File";
            // 
            // miFileOpenLocal
            // 
            this.miFileOpenLocal.Index = 0;
            this.miFileOpenLocal.Text = "Open local...";
            this.miFileOpenLocal.Click += new System.EventHandler(this.miOpenLocal_Click);
            // 
            // miFileSeparator1
            // 
            this.miFileSeparator1.Index = 1;
            this.miFileSeparator1.Text = "-";
            // 
            // miFileExit
            // 
            this.miFileExit.Index = 2;
            this.miFileExit.Text = "Exit";
            this.miFileExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // miEdit
            // 
            this.miEdit.Index = 1;
            this.miEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miEditCopy});
            this.miEdit.Text = "Edit";
            this.miEdit.Visible = false;
            // 
            // miEditCopy
            // 
            this.miEditCopy.Index = 0;
            this.miEditCopy.Text = "Copy";
            this.miEditCopy.Click += new System.EventHandler(this.miCopy_Click);
            // 
            // miTest
            // 
            this.miTest.Index = 2;
            this.miTest.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miTestShowRefImage,
            this.miTestSeparator1,
            this.miTestSuite});
            this.miTest.Text = "Test";
            // 
            // miTestShowRefImage
            // 
            this.miTestShowRefImage.Checked = true;
            this.miTestShowRefImage.Index = 0;
            this.miTestShowRefImage.Text = "Show reference image";
            this.miTestShowRefImage.Click += new System.EventHandler(this.miShowRefImage_Click);
            // 
            // miTestSeparator1
            // 
            this.miTestSeparator1.Index = 1;
            this.miTestSeparator1.Text = "-";
            // 
            // miTestSuite
            // 
            this.miTestSuite.Index = 2;
            this.miTestSuite.Text = "W3C SVG Test suite";
            // 
            // miOptions
            // 
            this.miOptions.Index = 3;
            this.miOptions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miOptionsClearCache});
            this.miOptions.Text = "Options";
            // 
            // miOptionsClearCache
            // 
            this.miOptionsClearCache.Index = 0;
            this.miOptionsClearCache.Text = "Clear cache";
            this.miOptionsClearCache.Click += new System.EventHandler(this.miClearCache_Click);
            // 
            // miHelp
            // 
            this.miHelp.Index = 4;
            this.miHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miHelpAbout});
            this.miHelp.Text = "Help";
            // 
            // miHelpAbout
            // 
            this.miHelpAbout.Index = 0;
            this.miHelpAbout.Text = "About...";
            this.miHelpAbout.Click += new System.EventHandler(this.miAbout_Click);
            // 
            // svgUrlCombo
            // 
            this.svgUrlCombo.ItemHeight = 16;
            this.svgUrlCombo.Items.AddRange(new object[] {
            "http://www.w3.org/Graphics/SVG/Test/20011026/rendering-orderGr-BE-01.svg",
            "http://www.croczilla.com/svg/butterfly.xml",
            "http://www.croczilla.com/svg/tiger.xml",
            "http://www.croczilla.com/svg/skew01.xml",
            "http://www.protocol7.com/svg.net/people.svg"});
            this.svgUrlCombo.Location = new System.Drawing.Point(10, 9);
            this.svgUrlCombo.Name = "svgUrlCombo";
            this.svgUrlCombo.Size = new System.Drawing.Size(403, 24);
            this.svgUrlCombo.TabIndex = 5;
            // 
            // pbRefImage
            // 
            this.pbRefImage.BackColor = System.Drawing.SystemColors.Window;
            this.pbRefImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbRefImage.Location = new System.Drawing.Point(0, 28);
            this.pbRefImage.Name = "pbRefImage";
            this.pbRefImage.Size = new System.Drawing.Size(442, 580);
            this.pbRefImage.TabIndex = 26;
            this.pbRefImage.TabStop = false;
            this.pbRefImage.Visible = false;
            // 
            // svgPictureBox
            // 
            this.svgPictureBox.BackColor = System.Drawing.SystemColors.Window;
            this.svgPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.svgPictureBox.Location = new System.Drawing.Point(0, 28);
            this.svgPictureBox.Name = "svgPictureBox";
            this.svgPictureBox.Size = new System.Drawing.Size(467, 580);
            this.svgPictureBox.Surface = null;
            this.svgPictureBox.TabIndex = 25;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 43);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.svgPictureBox);
            this.splitContainer.Panel1.Controls.Add(this.labelSvg);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.pbRefImage);
            this.splitContainer.Panel2.Controls.Add(this.labelPng);
            this.splitContainer.Size = new System.Drawing.Size(913, 608);
            this.splitContainer.SplitterDistance = 467;
            this.splitContainer.TabIndex = 27;
            // 
            // labelSvg
            // 
            this.labelSvg.BackColor = System.Drawing.SystemColors.Window;
            this.labelSvg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelSvg.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelSvg.Location = new System.Drawing.Point(0, 0);
            this.labelSvg.Name = "labelSvg";
            this.labelSvg.Size = new System.Drawing.Size(467, 28);
            this.labelSvg.TabIndex = 26;
            this.labelSvg.Text = "SVG Rendered";
            // 
            // labelPng
            // 
            this.labelPng.BackColor = System.Drawing.SystemColors.Window;
            this.labelPng.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelPng.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelPng.Location = new System.Drawing.Point(0, 0);
            this.labelPng.Name = "labelPng";
            this.labelPng.Size = new System.Drawing.Size(442, 28);
            this.labelPng.TabIndex = 27;
            this.labelPng.Text = "Png Rendered";
            // 
            // SvgViewerForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(913, 677);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.svgUrlCombo);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.toolBar1);
            this.Menu = this.mainMenu1;
            this.Name = "SvgViewerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = ".NET SVG Viewer";
            this.Load += new System.EventHandler(this.SvgViewerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbRefImage)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Private Fields

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolBar toolBar1;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.ComboBox svgUrlCombo;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.StatusBar statusBar;

        private System.Windows.Forms.MenuItem miFile;
        private System.Windows.Forms.MenuItem miFileOpenLocal;
        private System.Windows.Forms.MenuItem miFileSeparator1;
        private System.Windows.Forms.MenuItem miFileExit;
        private System.Windows.Forms.MenuItem miEdit;
        private System.Windows.Forms.MenuItem miEditCopy;
        private System.Windows.Forms.MenuItem miTest;
        private System.Windows.Forms.MenuItem miTestShowRefImage;
        private System.Windows.Forms.MenuItem miTestSeparator1;
        private System.Windows.Forms.MenuItem miTestSuite;
        private System.Windows.Forms.MenuItem miOptions;
        private System.Windows.Forms.MenuItem miOptionsClearCache;
        private System.Windows.Forms.MenuItem miHelp;
        private System.Windows.Forms.MenuItem miHelpAbout;
        private System.Windows.Forms.PictureBox pbRefImage;
        private SharpVectors.Renderers.Forms.SvgPictureBox svgPictureBox;

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Label labelSvg;
        private System.Windows.Forms.Label labelPng;
    }
}
