namespace GdiW3cSvgTestSuite
{
    partial class AboutTestDockPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutTestDockPanel));
            this.testTitleLabel = new System.Windows.Forms.Label();
            this.testTitle = new System.Windows.Forms.TextBox();
            this.testDescritionLabel = new System.Windows.Forms.Label();
            this.testDescrition = new System.Windows.Forms.TextBox();
            this.testFilePathLabel = new System.Windows.Forms.Label();
            this.testFilePath = new System.Windows.Forms.TextBox();
            this.btnFilePath = new System.Windows.Forms.Button();
            this.testDetailsLabel = new System.Windows.Forms.Label();
            this.testDetailsDoc = new TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel();
            this.SuspendLayout();
            // 
            // testTitleLabel
            // 
            this.testTitleLabel.AutoSize = true;
            this.testTitleLabel.Location = new System.Drawing.Point(13, 13);
            this.testTitleLabel.Name = "testTitleLabel";
            this.testTitleLabel.Size = new System.Drawing.Size(55, 12);
            this.testTitleLabel.TabIndex = 0;
            this.testTitleLabel.Text = "Test Title";
            // 
            // testTitle
            // 
            this.testTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testTitle.BackColor = System.Drawing.SystemColors.Window;
            this.testTitle.Location = new System.Drawing.Point(43, 33);
            this.testTitle.Name = "testTitle";
            this.testTitle.ReadOnly = true;
            this.testTitle.Size = new System.Drawing.Size(745, 19);
            this.testTitle.TabIndex = 1;
            // 
            // testDescritionLabel
            // 
            this.testDescritionLabel.AutoSize = true;
            this.testDescritionLabel.Location = new System.Drawing.Point(15, 65);
            this.testDescritionLabel.Name = "testDescritionLabel";
            this.testDescritionLabel.Size = new System.Drawing.Size(90, 12);
            this.testDescritionLabel.TabIndex = 2;
            this.testDescritionLabel.Text = "Test Description";
            // 
            // testDescrition
            // 
            this.testDescrition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testDescrition.BackColor = System.Drawing.SystemColors.Window;
            this.testDescrition.Location = new System.Drawing.Point(43, 86);
            this.testDescrition.Multiline = true;
            this.testDescrition.Name = "testDescrition";
            this.testDescrition.ReadOnly = true;
            this.testDescrition.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.testDescrition.Size = new System.Drawing.Size(745, 54);
            this.testDescrition.TabIndex = 3;
            // 
            // testFilePathLabel
            // 
            this.testFilePathLabel.AutoSize = true;
            this.testFilePathLabel.Location = new System.Drawing.Point(17, 153);
            this.testFilePathLabel.Name = "testFilePathLabel";
            this.testFilePathLabel.Size = new System.Drawing.Size(78, 12);
            this.testFilePathLabel.TabIndex = 4;
            this.testFilePathLabel.Text = "Test File Path";
            // 
            // testFilePath
            // 
            this.testFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testFilePath.BackColor = System.Drawing.SystemColors.Window;
            this.testFilePath.Location = new System.Drawing.Point(43, 175);
            this.testFilePath.Name = "testFilePath";
            this.testFilePath.ReadOnly = true;
            this.testFilePath.Size = new System.Drawing.Size(664, 19);
            this.testFilePath.TabIndex = 5;
            // 
            // btnFilePath
            // 
            this.btnFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilePath.BackColor = System.Drawing.SystemColors.Control;
            this.btnFilePath.Enabled = false;
            this.btnFilePath.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnFilePath.FlatAppearance.BorderSize = 2;
            this.btnFilePath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilePath.Location = new System.Drawing.Point(713, 172);
            this.btnFilePath.Name = "btnFilePath";
            this.btnFilePath.Size = new System.Drawing.Size(75, 30);
            this.btnFilePath.TabIndex = 6;
            this.btnFilePath.Text = "Locate...";
            this.btnFilePath.UseVisualStyleBackColor = false;
            this.btnFilePath.Click += new System.EventHandler(this.OnLocateFile);
            // 
            // testDetailsLabel
            // 
            this.testDetailsLabel.AutoSize = true;
            this.testDetailsLabel.Location = new System.Drawing.Point(19, 211);
            this.testDetailsLabel.Name = "testDetailsLabel";
            this.testDetailsLabel.Size = new System.Drawing.Size(68, 12);
            this.testDetailsLabel.TabIndex = 7;
            this.testDetailsLabel.Text = "Test Details";
            // 
            // testDetailsDoc
            // 
            this.testDetailsDoc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testDetailsDoc.AutoScroll = true;
            this.testDetailsDoc.BackColor = System.Drawing.SystemColors.Window;
            this.testDetailsDoc.BaseStylesheet = null;
            this.testDetailsDoc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.testDetailsDoc.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.testDetailsDoc.Location = new System.Drawing.Point(43, 233);
            this.testDetailsDoc.Name = "testDetailsDoc";
            this.testDetailsDoc.Size = new System.Drawing.Size(745, 541);
            this.testDetailsDoc.TabIndex = 8;
            this.testDetailsDoc.Text = null;
            this.testDetailsDoc.UseGdiPlusTextRendering = true;
            // 
            // AboutTestDockPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(800, 790);
            this.Controls.Add(this.testDetailsDoc);
            this.Controls.Add(this.testDetailsLabel);
            this.Controls.Add(this.btnFilePath);
            this.Controls.Add(this.testFilePath);
            this.Controls.Add(this.testFilePathLabel);
            this.Controls.Add(this.testDescrition);
            this.Controls.Add(this.testDescritionLabel);
            this.Controls.Add(this.testTitle);
            this.Controls.Add(this.testTitleLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AboutTestDockPanel";
            this.Text = "AboutTestDockPanel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label testTitleLabel;
        private System.Windows.Forms.TextBox testTitle;
        private System.Windows.Forms.Label testDescritionLabel;
        private System.Windows.Forms.TextBox testDescrition;
        private System.Windows.Forms.Label testFilePathLabel;
        private System.Windows.Forms.TextBox testFilePath;
        private System.Windows.Forms.Button btnFilePath;
        private System.Windows.Forms.Label testDetailsLabel;
        private TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel testDetailsDoc;
    }
}