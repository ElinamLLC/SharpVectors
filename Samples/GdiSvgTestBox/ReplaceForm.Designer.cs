namespace GdiSvgTestBox
{
    partial class ReplaceForm
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.chkMatchCase = new System.Windows.Forms.CheckBox();
            this.chkMatchWholeWord = new System.Windows.Forms.CheckBox();
            this.chkSelection = new System.Windows.Forms.CheckBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.lblReplace = new System.Windows.Forms.Label();
            this.btnReplace = new System.Windows.Forms.Button();
            this.btnReplaceAll = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.BackColor = System.Drawing.SystemColors.Window;
            this.tableLayoutPanel.ColumnCount = 6;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel.Controls.Add(this.lblSearch, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.btnNext, 4, 0);
            this.tableLayoutPanel.Controls.Add(this.btnPrevious, 5, 0);
            this.tableLayoutPanel.Controls.Add(this.btnClose, 4, 2);
            this.tableLayoutPanel.Controls.Add(this.chkMatchCase, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.chkMatchWholeWord, 2, 2);
            this.tableLayoutPanel.Controls.Add(this.chkSelection, 3, 2);
            this.tableLayoutPanel.Controls.Add(this.txtSearch, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.txtReplace, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.lblReplace, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.btnReplace, 4, 1);
            this.tableLayoutPanel.Controls.Add(this.btnReplaceAll, 5, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(1, 8);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(530, 100);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSearch.Location = new System.Drawing.Point(3, 0);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(64, 33);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search";
            this.lblSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnNext
            // 
            this.btnNext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNext.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Image = global::GdiSvgTestBox.Properties.Resources.FindNext;
            this.btnNext.Location = new System.Drawing.Point(461, 3);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(30, 27);
            this.btnNext.TabIndex = 1;
            this.toolTip.SetToolTip(this.btnNext, "Find Next");
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.OnClickNext);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPrevious.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.btnPrevious.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrevious.Image = global::GdiSvgTestBox.Properties.Resources.FindPrevious;
            this.btnPrevious.Location = new System.Drawing.Point(497, 3);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(31, 27);
            this.btnPrevious.TabIndex = 2;
            this.toolTip.SetToolTip(this.btnPrevious, "Find Previous");
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.OnClickPrevious);
            // 
            // btnClose
            // 
            this.tableLayoutPanel.SetColumnSpan(this.btnClose, 2);
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(461, 69);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(67, 28);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.OnClickClose);
            // 
            // chkMatchCase
            // 
            this.chkMatchCase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkMatchCase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkMatchCase.Location = new System.Drawing.Point(73, 69);
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.Size = new System.Drawing.Size(100, 28);
            this.chkMatchCase.TabIndex = 4;
            this.chkMatchCase.Text = "Match case";
            this.chkMatchCase.UseVisualStyleBackColor = true;
            this.chkMatchCase.CheckedChanged += new System.EventHandler(this.OnMatchCaseChanged);
            // 
            // chkMatchWholeWord
            // 
            this.chkMatchWholeWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkMatchWholeWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkMatchWholeWord.Location = new System.Drawing.Point(179, 69);
            this.chkMatchWholeWord.Name = "chkMatchWholeWord";
            this.chkMatchWholeWord.Size = new System.Drawing.Size(150, 28);
            this.chkMatchWholeWord.TabIndex = 5;
            this.chkMatchWholeWord.Text = "Match whole word";
            this.chkMatchWholeWord.UseVisualStyleBackColor = true;
            this.chkMatchWholeWord.CheckedChanged += new System.EventHandler(this.OnMatchWholeWordChanged);
            // 
            // chkSelection
            // 
            this.chkSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkSelection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkSelection.Location = new System.Drawing.Point(335, 69);
            this.chkSelection.Name = "chkSelection";
            this.chkSelection.Size = new System.Drawing.Size(120, 28);
            this.chkSelection.TabIndex = 6;
            this.chkSelection.Text = "Selection Only";
            this.chkSelection.UseVisualStyleBackColor = true;
            this.chkSelection.CheckedChanged += new System.EventHandler(this.OnSelectionOnlyChanged);
            // 
            // txtSearch
            // 
            this.tableLayoutPanel.SetColumnSpan(this.txtSearch, 3);
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(73, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(382, 26);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.WordWrap = false;
            this.txtSearch.TextChanged += new System.EventHandler(this.OnSearchTextChanged);
            // 
            // txtReplace
            // 
            this.tableLayoutPanel.SetColumnSpan(this.txtReplace, 3);
            this.txtReplace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReplace.Location = new System.Drawing.Point(73, 36);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(382, 26);
            this.txtReplace.TabIndex = 6;
            // 
            // lblReplace
            // 
            this.lblReplace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReplace.Location = new System.Drawing.Point(3, 33);
            this.lblReplace.Name = "lblReplace";
            this.lblReplace.Size = new System.Drawing.Size(64, 33);
            this.lblReplace.TabIndex = 7;
            this.lblReplace.Text = "Replace";
            this.lblReplace.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnReplace
            // 
            this.btnReplace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReplace.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.btnReplace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReplace.Image = global::GdiSvgTestBox.Properties.Resources.Replace;
            this.btnReplace.Location = new System.Drawing.Point(461, 36);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(30, 27);
            this.btnReplace.TabIndex = 8;
            this.toolTip.SetToolTip(this.btnReplace, "Replace Next");
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.OnClickReplace);
            // 
            // btnReplaceAll
            // 
            this.btnReplaceAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReplaceAll.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.btnReplaceAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReplaceAll.Image = global::GdiSvgTestBox.Properties.Resources.ReplaceAll;
            this.btnReplaceAll.Location = new System.Drawing.Point(497, 36);
            this.btnReplaceAll.Name = "btnReplaceAll";
            this.btnReplaceAll.Size = new System.Drawing.Size(31, 27);
            this.btnReplaceAll.TabIndex = 9;
            this.toolTip.SetToolTip(this.btnReplaceAll, "Replace All");
            this.btnReplaceAll.UseVisualStyleBackColor = true;
            this.btnReplaceAll.Click += new System.EventHandler(this.OnClickReplaceAll);
            // 
            // ReplaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 109);
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "ReplaceForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "ReplaceForm";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox chkMatchCase;
        private System.Windows.Forms.CheckBox chkMatchWholeWord;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.Label lblReplace;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnReplaceAll;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox chkSelection;
    }
}