namespace GdiSvgTestBox
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
            this.groupBoxConversion = new System.Windows.Forms.GroupBox();
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
            this.groupBoxGeneral.Size = new System.Drawing.Size(776, 297);
            this.groupBoxGeneral.TabIndex = 0;
            this.groupBoxGeneral.TabStop = false;
            this.groupBoxGeneral.Text = "General Settings";
            // 
            // panelGeneral
            // 
            this.panelGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelGeneral.Controls.Add(this.labelThemeNote);
            this.panelGeneral.Controls.Add(this.cmbTheme);
            this.panelGeneral.Controls.Add(this.labelTheme);
            this.panelGeneral.Controls.Add(this.labelHide);
            this.panelGeneral.Controls.Add(this.chkHidePathsRoot);
            this.panelGeneral.Location = new System.Drawing.Point(11, 25);
            this.panelGeneral.Name = "panelGeneral";
            this.panelGeneral.Size = new System.Drawing.Size(759, 252);
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
            this.labelTheme.Location = new System.Drawing.Point(187, 21);
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
            // groupBoxConversion
            // 
            this.groupBoxConversion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxConversion.Location = new System.Drawing.Point(12, 344);
            this.groupBoxConversion.Name = "groupBoxConversion";
            this.groupBoxConversion.Size = new System.Drawing.Size(776, 140);
            this.groupBoxConversion.TabIndex = 1;
            this.groupBoxConversion.TabStop = false;
            this.groupBoxConversion.Text = "Conversion Settings";
            // 
            // SettingsDockPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(800, 772);
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
        private System.Windows.Forms.Label labelHide;
        private System.Windows.Forms.CheckBox chkHidePathsRoot;
        private System.Windows.Forms.GroupBox groupBoxConversion;
        private System.Windows.Forms.Panel panelGeneral;
        private System.Windows.Forms.ComboBox cmbTheme;
        private System.Windows.Forms.Label labelTheme;
        private System.Windows.Forms.Label labelThemeNote;
    }
}