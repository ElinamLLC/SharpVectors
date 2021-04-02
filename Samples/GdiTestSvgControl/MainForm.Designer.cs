namespace TestSvgControl
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.svgPictureBox = new SharpVectors.Renderers.Forms.SvgPictureBox();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemNormal = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemStretchImage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAutoSize = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCenterImage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemQuickHelp = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.svgPictureBox)).BeginInit();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // svgPictureBox
            // 
            this.svgPictureBox.BackColor = System.Drawing.SystemColors.Window;
            this.svgPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.svgPictureBox.Location = new System.Drawing.Point(6, 6);
            this.svgPictureBox.Name = "svgPictureBox";
            this.svgPictureBox.Padding = new System.Windows.Forms.Padding(3);
            this.svgPictureBox.Size = new System.Drawing.Size(1134, 625);
            this.svgPictureBox.Source = null;
            this.svgPictureBox.TabIndex = 0;
            this.svgPictureBox.Text = "svgPictureBox1";
            this.svgPictureBox.UriSource = null;
            this.svgPictureBox.XmlSource = null;
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.BackColor = System.Drawing.SystemColors.Window;
            this.toolStripContainer.ContentPanel.Controls.Add(this.svgPictureBox);
            this.toolStripContainer.ContentPanel.Padding = new System.Windows.Forms.Padding(6);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(1146, 637);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.LeftToolStripPanelVisible = false;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.RightToolStripPanelVisible = false;
            this.toolStripContainer.Size = new System.Drawing.Size(1146, 661);
            this.toolStripContainer.TabIndex = 1;
            this.toolStripContainer.Text = "toolStripContainer1";
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.menuStrip);
            // 
            // menuStrip
            // 
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1146, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemSelect,
            this.menuItemExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // menuItemSelect
            // 
            this.menuItemSelect.Name = "menuItemSelect";
            this.menuItemSelect.Size = new System.Drawing.Size(177, 22);
            this.menuItemSelect.Text = "Select SVG Source...";
            this.menuItemSelect.Click += new System.EventHandler(this.OnSelectClicked);
            // 
            // menuItemExit
            // 
            this.menuItemExit.Name = "menuItemExit";
            this.menuItemExit.Size = new System.Drawing.Size(177, 22);
            this.menuItemExit.Text = "Exit";
            this.menuItemExit.Click += new System.EventHandler(this.OnExitClicked);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemNormal,
            this.menuItemStretchImage,
            this.menuItemAutoSize,
            this.menuItemCenterImage,
            this.menuItemZoom});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.viewToolStripMenuItem.Text = "Size Mode";
            // 
            // menuItemNormal
            // 
            this.menuItemNormal.Checked = true;
            this.menuItemNormal.CheckOnClick = true;
            this.menuItemNormal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuItemNormal.Name = "menuItemNormal";
            this.menuItemNormal.Size = new System.Drawing.Size(146, 22);
            this.menuItemNormal.Tag = "Normal";
            this.menuItemNormal.Text = "Normal";
            this.menuItemNormal.Click += new System.EventHandler(this.OnSizeModeChanged);
            // 
            // menuItemStretchImage
            // 
            this.menuItemStretchImage.CheckOnClick = true;
            this.menuItemStretchImage.Name = "menuItemStretchImage";
            this.menuItemStretchImage.Size = new System.Drawing.Size(146, 22);
            this.menuItemStretchImage.Tag = "StretchImage";
            this.menuItemStretchImage.Text = "Stretch Image";
            this.menuItemStretchImage.Click += new System.EventHandler(this.OnSizeModeChanged);
            // 
            // menuItemAutoSize
            // 
            this.menuItemAutoSize.CheckOnClick = true;
            this.menuItemAutoSize.Name = "menuItemAutoSize";
            this.menuItemAutoSize.Size = new System.Drawing.Size(146, 22);
            this.menuItemAutoSize.Tag = "AutoSize";
            this.menuItemAutoSize.Text = "Auto Size";
            this.menuItemAutoSize.Click += new System.EventHandler(this.OnSizeModeChanged);
            // 
            // menuItemCenterImage
            // 
            this.menuItemCenterImage.CheckOnClick = true;
            this.menuItemCenterImage.Name = "menuItemCenterImage";
            this.menuItemCenterImage.Size = new System.Drawing.Size(146, 22);
            this.menuItemCenterImage.Tag = "CenterImage";
            this.menuItemCenterImage.Text = "Center Image";
            this.menuItemCenterImage.Click += new System.EventHandler(this.OnSizeModeChanged);
            // 
            // menuItemZoom
            // 
            this.menuItemZoom.CheckOnClick = true;
            this.menuItemZoom.Name = "menuItemZoom";
            this.menuItemZoom.Size = new System.Drawing.Size(146, 22);
            this.menuItemZoom.Tag = "Zoom";
            this.menuItemZoom.Text = "Zoom";
            this.menuItemZoom.Click += new System.EventHandler(this.OnSizeModeChanged);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemQuickHelp});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // menuItemQuickHelp
            // 
            this.menuItemQuickHelp.Name = "menuItemQuickHelp";
            this.menuItemQuickHelp.Size = new System.Drawing.Size(139, 22);
            this.menuItemQuickHelp.Text = "QuickHelp...";
            this.menuItemQuickHelp.Click += new System.EventHandler(this.OnQuickHelpClick);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1146, 661);
            this.Controls.Add(this.toolStripContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Testing Svg";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.svgPictureBox)).EndInit();
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SharpVectors.Renderers.Forms.SvgPictureBox svgPictureBox;
        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemSelect;
        private System.Windows.Forms.ToolStripMenuItem menuItemExit;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemQuickHelp;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemAutoSize;
        private System.Windows.Forms.ToolStripMenuItem menuItemCenterImage;
        private System.Windows.Forms.ToolStripMenuItem menuItemNormal;
        private System.Windows.Forms.ToolStripMenuItem menuItemStretchImage;
        private System.Windows.Forms.ToolStripMenuItem menuItemZoom;
    }
}

