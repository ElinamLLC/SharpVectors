namespace GdiW3cSvgTestSuite
{
    partial class SettingsDockPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDockPanel));
            this.groupBoxGeneral = new System.Windows.Forms.GroupBox();
            this.panelGeneral = new System.Windows.Forms.Panel();
            this.labelThemeNote = new System.Windows.Forms.Label();
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            this.labelTheme = new System.Windows.Forms.Label();
            this.labelHide = new System.Windows.Forms.Label();
            this.chkHidePathsRoot = new System.Windows.Forms.CheckBox();
            this.labelLocal = new System.Windows.Forms.Label();
            this.btnPathBrowse = new System.Windows.Forms.Button();
            this.labelWeb = new System.Windows.Forms.Label();
            this.txtSvgSuitePath = new System.Windows.Forms.TextBox();
            this.txtSvgSuitePathWeb = new System.Windows.Forms.TextBox();
            this.btnPathLocate = new System.Windows.Forms.Button();
            this.groupBoxConversion = new System.Windows.Forms.GroupBox();
            this.htmlLabel = new TheArtOfDev.HtmlRenderer.WinForms.HtmlLabel();
            this.lblTestSuitesTitle = new System.Windows.Forms.Label();
            this.lblTestSuitesSelect = new System.Windows.Forms.Label();
            this.cboTestSuites = new System.Windows.Forms.ComboBox();
            this.groupBoxGeneral.SuspendLayout();
            this.panelGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxGeneral
            // 
            this.groupBoxGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGeneral.Controls.Add(this.panelGeneral);
            this.groupBoxGeneral.Location = new System.Drawing.Point(12, 13);
            this.groupBoxGeneral.Name = "groupBoxGeneral";
            this.groupBoxGeneral.Size = new System.Drawing.Size(776, 358);
            this.groupBoxGeneral.TabIndex = 0;
            this.groupBoxGeneral.TabStop = false;
            this.groupBoxGeneral.Text = "General Settings";
            // 
            // panelGeneral
            // 
            this.panelGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelGeneral.Controls.Add(this.cboTestSuites);
            this.panelGeneral.Controls.Add(this.lblTestSuitesSelect);
            this.panelGeneral.Controls.Add(this.lblTestSuitesTitle);
            this.panelGeneral.Controls.Add(this.labelThemeNote);
            this.panelGeneral.Controls.Add(this.cmbTheme);
            this.panelGeneral.Controls.Add(this.labelTheme);
            this.panelGeneral.Controls.Add(this.labelHide);
            this.panelGeneral.Controls.Add(this.chkHidePathsRoot);
            this.panelGeneral.Controls.Add(this.labelLocal);
            this.panelGeneral.Controls.Add(this.btnPathBrowse);
            this.panelGeneral.Controls.Add(this.labelWeb);
            this.panelGeneral.Controls.Add(this.txtSvgSuitePath);
            this.panelGeneral.Controls.Add(this.txtSvgSuitePathWeb);
            this.panelGeneral.Controls.Add(this.btnPathLocate);
            this.panelGeneral.Location = new System.Drawing.Point(11, 25);
            this.panelGeneral.Name = "panelGeneral";
            this.panelGeneral.Size = new System.Drawing.Size(759, 320);
            this.panelGeneral.TabIndex = 0;
            // 
            // labelThemeNote
            // 
            this.labelThemeNote.AutoSize = true;
            this.labelThemeNote.Location = new System.Drawing.Point(165, 47);
            this.labelThemeNote.Name = "labelThemeNote";
            this.labelThemeNote.Size = new System.Drawing.Size(354, 12);
            this.labelThemeNote.TabIndex = 21;
            this.labelThemeNote.Text = "The selected theme will be applied when you restart the application.";
            this.labelThemeNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbTheme
            // 
            this.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTheme.FormattingEnabled = true;
            this.cmbTheme.Location = new System.Drawing.Point(241, 16);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(204, 20);
            this.cmbTheme.TabIndex = 20;
            this.cmbTheme.SelectedIndexChanged += new System.EventHandler(this.OnSelectedThemeChanged);
            // 
            // labelTheme
            // 
            this.labelTheme.AutoSize = true;
            this.labelTheme.Location = new System.Drawing.Point(193, 21);
            this.labelTheme.Name = "labelTheme";
            this.labelTheme.Size = new System.Drawing.Size(39, 12);
            this.labelTheme.TabIndex = 19;
            this.labelTheme.Text = "Theme";
            // 
            // labelHide
            // 
            this.labelHide.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHide.Location = new System.Drawing.Point(36, 96);
            this.labelHide.Name = "labelHide";
            this.labelHide.Size = new System.Drawing.Size(710, 23);
            this.labelHide.TabIndex = 18;
            this.labelHide.Text = "Security feature, which is only useful in screenshot to be posted on the web.";
            // 
            // chkHidePathsRoot
            // 
            this.chkHidePathsRoot.AutoSize = true;
            this.chkHidePathsRoot.Location = new System.Drawing.Point(12, 73);
            this.chkHidePathsRoot.Name = "chkHidePathsRoot";
            this.chkHidePathsRoot.Size = new System.Drawing.Size(108, 16);
            this.chkHidePathsRoot.TabIndex = 17;
            this.chkHidePathsRoot.Text = "Hide Path Roots";
            this.chkHidePathsRoot.UseVisualStyleBackColor = true;
            this.chkHidePathsRoot.Click += new System.EventHandler(this.OnGeneralSettingsChanged);
            // 
            // labelLocal
            // 
            this.labelLocal.AutoSize = true;
            this.labelLocal.Location = new System.Drawing.Point(10, 254);
            this.labelLocal.Name = "labelLocal";
            this.labelLocal.Size = new System.Drawing.Size(145, 12);
            this.labelLocal.TabIndex = 13;
            this.labelLocal.Text = "Local W3C SVG Suite Path:";
            // 
            // btnPathBrowse
            // 
            this.btnPathBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPathBrowse.BackColor = System.Drawing.SystemColors.Control;
            this.btnPathBrowse.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnPathBrowse.FlatAppearance.BorderSize = 2;
            this.btnPathBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPathBrowse.Location = new System.Drawing.Point(588, 275);
            this.btnPathBrowse.Name = "btnPathBrowse";
            this.btnPathBrowse.Size = new System.Drawing.Size(75, 32);
            this.btnPathBrowse.TabIndex = 15;
            this.btnPathBrowse.Text = "Browse...";
            this.btnPathBrowse.UseVisualStyleBackColor = false;
            this.btnPathBrowse.Click += new System.EventHandler(this.OnBrowseForSvgSuitePath);
            // 
            // labelWeb
            // 
            this.labelWeb.AutoSize = true;
            this.labelWeb.Location = new System.Drawing.Point(8, 198);
            this.labelWeb.Name = "labelWeb";
            this.labelWeb.Size = new System.Drawing.Size(139, 12);
            this.labelWeb.TabIndex = 11;
            this.labelWeb.Text = "Web W3C SVG Suite Path:";
            // 
            // txtSvgSuitePath
            // 
            this.txtSvgSuitePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSvgSuitePath.Location = new System.Drawing.Point(36, 279);
            this.txtSvgSuitePath.Name = "txtSvgSuitePath";
            this.txtSvgSuitePath.Size = new System.Drawing.Size(546, 19);
            this.txtSvgSuitePath.TabIndex = 14;
            this.txtSvgSuitePath.TextChanged += new System.EventHandler(this.OnSvgSuitePathTextChanged);
            // 
            // txtSvgSuitePathWeb
            // 
            this.txtSvgSuitePathWeb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSvgSuitePathWeb.BackColor = System.Drawing.SystemColors.Window;
            this.txtSvgSuitePathWeb.Location = new System.Drawing.Point(36, 221);
            this.txtSvgSuitePathWeb.Name = "txtSvgSuitePathWeb";
            this.txtSvgSuitePathWeb.ReadOnly = true;
            this.txtSvgSuitePathWeb.Size = new System.Drawing.Size(710, 19);
            this.txtSvgSuitePathWeb.TabIndex = 12;
            // 
            // btnPathLocate
            // 
            this.btnPathLocate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPathLocate.BackColor = System.Drawing.SystemColors.Control;
            this.btnPathLocate.Enabled = false;
            this.btnPathLocate.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnPathLocate.FlatAppearance.BorderSize = 2;
            this.btnPathLocate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPathLocate.Location = new System.Drawing.Point(671, 275);
            this.btnPathLocate.Name = "btnPathLocate";
            this.btnPathLocate.Size = new System.Drawing.Size(75, 32);
            this.btnPathLocate.TabIndex = 16;
            this.btnPathLocate.Text = "Open...";
            this.btnPathLocate.UseVisualStyleBackColor = false;
            this.btnPathLocate.Click += new System.EventHandler(this.OnOpenSvgSuitePath);
            // 
            // groupBoxConversion
            // 
            this.groupBoxConversion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxConversion.Location = new System.Drawing.Point(12, 386);
            this.groupBoxConversion.Name = "groupBoxConversion";
            this.groupBoxConversion.Size = new System.Drawing.Size(776, 140);
            this.groupBoxConversion.TabIndex = 1;
            this.groupBoxConversion.TabStop = false;
            this.groupBoxConversion.Text = "Conversion Settings";
            // 
            // htmlLabel
            // 
            this.htmlLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.htmlLabel.AutoSize = false;
            this.htmlLabel.BackColor = System.Drawing.SystemColors.Window;
            this.htmlLabel.BaseStylesheet = null;
            this.htmlLabel.Location = new System.Drawing.Point(45, 533);
            this.htmlLabel.Name = "htmlLabel";
            this.htmlLabel.Size = new System.Drawing.Size(709, 22);
            this.htmlLabel.TabIndex = 2;
            this.htmlLabel.Text = "<b>NOTE:</b> Changes are automatically applied if you switch to any other tab.";
            this.htmlLabel.UseGdiPlusTextRendering = true;
            // 
            // lblTestSuitesTitle
            // 
            this.lblTestSuitesTitle.AutoSize = true;
            this.lblTestSuitesTitle.Location = new System.Drawing.Point(10, 136);
            this.lblTestSuitesTitle.Name = "lblTestSuitesTitle";
            this.lblTestSuitesTitle.Size = new System.Drawing.Size(130, 12);
            this.lblTestSuitesTitle.TabIndex = 22;
            this.lblTestSuitesTitle.Text = "W3C SVG Suite Version:";
            // 
            // lblTestSuitesSelect
            // 
            this.lblTestSuitesSelect.AutoSize = true;
            this.lblTestSuitesSelect.Location = new System.Drawing.Point(91, 163);
            this.lblTestSuitesSelect.Name = "lblTestSuitesSelect";
            this.lblTestSuitesSelect.Size = new System.Drawing.Size(81, 12);
            this.lblTestSuitesSelect.TabIndex = 23;
            this.lblTestSuitesSelect.Text = "Selected Suite:";
            this.lblTestSuitesSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboTestSuites
            // 
            this.cboTestSuites.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTestSuites.FormattingEnabled = true;
            this.cboTestSuites.Location = new System.Drawing.Point(188, 159);
            this.cboTestSuites.Name = "cboTestSuites";
            this.cboTestSuites.Size = new System.Drawing.Size(492, 20);
            this.cboTestSuites.TabIndex = 24;
            this.cboTestSuites.SelectedIndexChanged += new System.EventHandler(this.OnTestSuitesSelectionChanged);
            // 
            // SettingsDockPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(800, 772);
            this.Controls.Add(this.htmlLabel);
            this.Controls.Add(this.groupBoxConversion);
            this.Controls.Add(this.groupBoxGeneral);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsDockPanel";
            this.Text = "SettingsDockPanel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.groupBoxGeneral.ResumeLayout(false);
            this.panelGeneral.ResumeLayout(false);
            this.panelGeneral.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxGeneral;
        private System.Windows.Forms.Button btnPathLocate;
        private System.Windows.Forms.Button btnPathBrowse;
        private System.Windows.Forms.TextBox txtSvgSuitePath;
        private System.Windows.Forms.Label labelLocal;
        private System.Windows.Forms.TextBox txtSvgSuitePathWeb;
        private System.Windows.Forms.Label labelWeb;
        private System.Windows.Forms.Label labelHide;
        private System.Windows.Forms.CheckBox chkHidePathsRoot;
        private System.Windows.Forms.GroupBox groupBoxConversion;
        private TheArtOfDev.HtmlRenderer.WinForms.HtmlLabel htmlLabel;
        private System.Windows.Forms.Panel panelGeneral;
        private System.Windows.Forms.ComboBox cmbTheme;
        private System.Windows.Forms.Label labelTheme;
        private System.Windows.Forms.Label labelThemeNote;
        private System.Windows.Forms.Label lblTestSuitesTitle;
        private System.Windows.Forms.Label lblTestSuitesSelect;
        private System.Windows.Forms.ComboBox cboTestSuites;
    }
}