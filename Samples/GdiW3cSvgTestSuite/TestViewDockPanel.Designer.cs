namespace GdiW3cSvgTestSuite
{
    partial class TestViewDockPanel
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestViewDockPanel));
            this.expanderTablePanel = new WinFormsExpander.ExpanderTablePanel();
            this.stateExpander = new WinFormsExpander.Expander();
            this.testApplyButton = new System.Windows.Forms.Button();
            this.testComment = new System.Windows.Forms.TextBox();
            this.commentLabel = new System.Windows.Forms.Label();
            this.stateComboBox = new GdiW3cSvgTestSuite.ImagedComboBox();
            this.stateLabel = new System.Windows.Forms.Label();
            this.treeView = new System.Windows.Forms.TreeView();
            this.treeImageList = new System.Windows.Forms.ImageList(this.components);
            this.expanderTablePanel.SuspendLayout();
            this.stateExpander.Content.SuspendLayout();
            this.SuspendLayout();
            // 
            // expanderTablePanel
            // 
            this.expanderTablePanel.ColumnCount = 1;
            this.expanderTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.expanderTablePanel.Controls.Add(this.stateExpander, 0, 1);
            this.expanderTablePanel.Controls.Add(this.treeView, 0, 0);
            this.expanderTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expanderTablePanel.Location = new System.Drawing.Point(0, 0);
            this.expanderTablePanel.Name = "expanderTablePanel";
            this.expanderTablePanel.RowCount = 2;
            this.expanderTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.expanderTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 231F));
            this.expanderTablePanel.Size = new System.Drawing.Size(345, 789);
            this.expanderTablePanel.TabIndex = 0;
            // 
            // stateExpander
            // 
            this.stateExpander.AutoScroll = true;
            this.stateExpander.CollapseImage = ((System.Drawing.Image)(resources.GetObject("stateExpander.CollapseImage")));
            // 
            // stateExpander.Content
            // 
            this.stateExpander.Content.AutoScroll = true;
            this.stateExpander.Content.AutoScrollMinSize = new System.Drawing.Size(150, 50);
            this.stateExpander.Content.BackColor = System.Drawing.SystemColors.Window;
            this.stateExpander.Content.Controls.Add(this.testApplyButton);
            this.stateExpander.Content.Controls.Add(this.testComment);
            this.stateExpander.Content.Controls.Add(this.commentLabel);
            this.stateExpander.Content.Controls.Add(this.stateComboBox);
            this.stateExpander.Content.Controls.Add(this.stateLabel);
            this.stateExpander.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateExpander.ExpandImage = ((System.Drawing.Image)(resources.GetObject("stateExpander.ExpandImage")));
            this.stateExpander.Header = "Selected Test State";
            this.stateExpander.HeaderFont = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stateExpander.Location = new System.Drawing.Point(3, 561);
            this.stateExpander.MinimumSize = new System.Drawing.Size(0, 63);
            this.stateExpander.Name = "stateExpander";
            this.stateExpander.Size = new System.Drawing.Size(339, 225);
            this.stateExpander.TabIndex = 0;
            // 
            // testApplyButton
            // 
            this.testApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.testApplyButton.BackColor = System.Drawing.SystemColors.Control;
            this.testApplyButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.testApplyButton.FlatAppearance.BorderSize = 2;
            this.testApplyButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.testApplyButton.Location = new System.Drawing.Point(243, 155);
            this.testApplyButton.Name = "testApplyButton";
            this.testApplyButton.Size = new System.Drawing.Size(80, 29);
            this.testApplyButton.TabIndex = 4;
            this.testApplyButton.Text = "Apply";
            this.testApplyButton.UseVisualStyleBackColor = false;
            this.testApplyButton.Click += new System.EventHandler(this.OnApplyTestState);
            // 
            // testComment
            // 
            this.testComment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testComment.Location = new System.Drawing.Point(26, 87);
            this.testComment.Multiline = true;
            this.testComment.Name = "testComment";
            this.testComment.Size = new System.Drawing.Size(298, 61);
            this.testComment.TabIndex = 3;
            this.testComment.TextChanged += new System.EventHandler(this.OnCommentTextChanged);
            // 
            // commentLabel
            // 
            this.commentLabel.AutoSize = true;
            this.commentLabel.Location = new System.Drawing.Point(12, 69);
            this.commentLabel.Name = "commentLabel";
            this.commentLabel.Size = new System.Drawing.Size(80, 12);
            this.commentLabel.TabIndex = 2;
            this.commentLabel.Text = "Test Comment";
            // 
            // stateComboBox
            // 
            this.stateComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stateComboBox.BackColor = System.Drawing.SystemColors.Control;
            this.stateComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.stateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stateComboBox.FormattingEnabled = true;
            this.stateComboBox.ItemHeight = 24;
            this.stateComboBox.Location = new System.Drawing.Point(26, 29);
            this.stateComboBox.Name = "stateComboBox";
            this.stateComboBox.Size = new System.Drawing.Size(298, 30);
            this.stateComboBox.TabIndex = 1;
            this.stateComboBox.SelectedIndexChanged += new System.EventHandler(this.OnStateSelectionChanged);
            // 
            // stateLabel
            // 
            this.stateLabel.AutoSize = true;
            this.stateLabel.Location = new System.Drawing.Point(10, 9);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(59, 12);
            this.stateLabel.TabIndex = 0;
            this.stateLabel.Text = "Test State";
            // 
            // treeView
            // 
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.HideSelection = false;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.treeImageList;
            this.treeView.ItemHeight = 24;
            this.treeView.Location = new System.Drawing.Point(3, 3);
            this.treeView.Name = "treeView";
            this.treeView.PathSeparator = "/";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(339, 552);
            this.treeView.TabIndex = 1;
            this.treeView.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.OnTreeBeforeCollapse);
            this.treeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeAfterCollapse);
            this.treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.OnTreeBeforeExpand);
            this.treeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeAfterExpand);
            this.treeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.OnTreeBeforeSelect);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeAfterSelect);
            // 
            // treeImageList
            // 
            this.treeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.treeImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.treeImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // TestViewDockPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 789);
            this.Controls.Add(this.expanderTablePanel);
            this.Name = "TestViewDockPanel";
            this.Text = "TestViewDockPanel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.expanderTablePanel.ResumeLayout(false);
            this.stateExpander.Content.ResumeLayout(false);
            this.stateExpander.Content.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private WinFormsExpander.ExpanderTablePanel expanderTablePanel;
        private WinFormsExpander.Expander stateExpander;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList treeImageList;
        private ImagedComboBox stateComboBox;
        private System.Windows.Forms.Label stateLabel;
        private System.Windows.Forms.Label commentLabel;
        private System.Windows.Forms.TextBox testComment;
        private System.Windows.Forms.Button testApplyButton;
    }
}