namespace GdiW3cSvgTestSuite
{
    partial class SearchForm
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
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.chkSelection = new System.Windows.Forms.CheckBox();
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
            this.tableLayoutPanel.Controls.Add(this.btnClose, 4, 1);
            this.tableLayoutPanel.Controls.Add(this.chkMatchCase, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.chkMatchWholeWord, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.txtSearch, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.chkSelection, 3, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(1, 8);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(530, 67);
            this.tableLayoutPanel.TabIndex = 0;
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
            this.btnNext.Image = global::GdiW3cSvgTestSuite.Properties.Resources.FindNext;
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
            this.btnPrevious.Image = global::GdiW3cSvgTestSuite.Properties.Resources.FindPrevious;
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
            this.btnClose.Location = new System.Drawing.Point(461, 36);
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
            this.chkMatchCase.Location = new System.Drawing.Point(73, 36);
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
            this.chkMatchWholeWord.Location = new System.Drawing.Point(179, 36);
            this.chkMatchWholeWord.Name = "chkMatchWholeWord";
            this.chkMatchWholeWord.Size = new System.Drawing.Size(150, 28);
            this.chkMatchWholeWord.TabIndex = 5;
            this.chkMatchWholeWord.Text = "Match whole word";
            this.chkMatchWholeWord.UseVisualStyleBackColor = true;
            this.chkMatchWholeWord.CheckedChanged += new System.EventHandler(this.OnMatchWholeWordChanged);
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
            // chkSelection
            // 
            this.chkSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkSelection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkSelection.Location = new System.Drawing.Point(335, 36);
            this.chkSelection.Name = "chkSelection";
            this.chkSelection.Size = new System.Drawing.Size(120, 28);
            this.chkSelection.TabIndex = 6;
            this.chkSelection.Text = "Selection Only";
            this.chkSelection.UseVisualStyleBackColor = true;
            this.chkSelection.CheckedChanged += new System.EventHandler(this.OnSelectionOnlyChanged);
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 76);
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "SearchForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "SearchForm";
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
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox chkSelection;
    }
}