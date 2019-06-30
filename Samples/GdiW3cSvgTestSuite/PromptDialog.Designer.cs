namespace GdiW3cSvgTestSuite
{
    partial class PromptDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PromptDialog));
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelWeb = new System.Windows.Forms.Label();
            this.txtSvgSuitePathWeb = new System.Windows.Forms.TextBox();
            this.btnDownload = new System.Windows.Forms.Button();
            this.labelLocal = new System.Windows.Forms.Label();
            this.txtSvgSuitePath = new System.Windows.Forms.TextBox();
            this.btnPathBrowse = new System.Windows.Forms.Button();
            this.btnPathLocate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Location = new System.Drawing.Point(13, 13);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(759, 61);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "The W3C Test Suite is not found in the specified local directory. Click the Downl" +
    "oad button to download it or Browse... button to select the local directory wher" +
    "e the tests are stored.";
            // 
            // labelWeb
            // 
            this.labelWeb.AutoSize = true;
            this.labelWeb.Location = new System.Drawing.Point(16, 85);
            this.labelWeb.Name = "labelWeb";
            this.labelWeb.Size = new System.Drawing.Size(139, 12);
            this.labelWeb.TabIndex = 1;
            this.labelWeb.Text = "Web W3C SVG Suite Path:";
            // 
            // txtSvgSuitePathWeb
            // 
            this.txtSvgSuitePathWeb.BackColor = System.Drawing.SystemColors.Window;
            this.txtSvgSuitePathWeb.Location = new System.Drawing.Point(44, 108);
            this.txtSvgSuitePathWeb.Name = "txtSvgSuitePathWeb";
            this.txtSvgSuitePathWeb.ReadOnly = true;
            this.txtSvgSuitePathWeb.Size = new System.Drawing.Size(728, 19);
            this.txtSvgSuitePathWeb.TabIndex = 2;
            // 
            // btnDownload
            // 
            this.btnDownload.BackColor = System.Drawing.SystemColors.Control;
            this.btnDownload.Enabled = false;
            this.btnDownload.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnDownload.FlatAppearance.BorderSize = 2;
            this.btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownload.Location = new System.Drawing.Point(283, 140);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(173, 32);
            this.btnDownload.TabIndex = 3;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = false;
            this.btnDownload.Click += new System.EventHandler(this.OnDownloadClicked);
            // 
            // labelLocal
            // 
            this.labelLocal.AutoSize = true;
            this.labelLocal.Location = new System.Drawing.Point(18, 190);
            this.labelLocal.Name = "labelLocal";
            this.labelLocal.Size = new System.Drawing.Size(145, 12);
            this.labelLocal.TabIndex = 4;
            this.labelLocal.Text = "Local W3C SVG Suite Path:";
            // 
            // txtSvgSuitePath
            // 
            this.txtSvgSuitePath.Location = new System.Drawing.Point(44, 215);
            this.txtSvgSuitePath.Name = "txtSvgSuitePath";
            this.txtSvgSuitePath.Size = new System.Drawing.Size(564, 19);
            this.txtSvgSuitePath.TabIndex = 5;
            this.txtSvgSuitePath.TextChanged += new System.EventHandler(this.OnSvgSuitePathTextChanged);
            // 
            // btnPathBrowse
            // 
            this.btnPathBrowse.BackColor = System.Drawing.SystemColors.Control;
            this.btnPathBrowse.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnPathBrowse.FlatAppearance.BorderSize = 2;
            this.btnPathBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPathBrowse.Location = new System.Drawing.Point(614, 211);
            this.btnPathBrowse.Name = "btnPathBrowse";
            this.btnPathBrowse.Size = new System.Drawing.Size(75, 32);
            this.btnPathBrowse.TabIndex = 6;
            this.btnPathBrowse.Text = "Browse...";
            this.btnPathBrowse.UseVisualStyleBackColor = false;
            this.btnPathBrowse.Click += new System.EventHandler(this.OnBrowseForSvgSuitePath);
            // 
            // btnPathLocate
            // 
            this.btnPathLocate.BackColor = System.Drawing.SystemColors.Control;
            this.btnPathLocate.Enabled = false;
            this.btnPathLocate.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnPathLocate.FlatAppearance.BorderSize = 2;
            this.btnPathLocate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPathLocate.Location = new System.Drawing.Point(697, 211);
            this.btnPathLocate.Name = "btnPathLocate";
            this.btnPathLocate.Size = new System.Drawing.Size(75, 32);
            this.btnPathLocate.TabIndex = 7;
            this.btnPathLocate.Text = "Open...";
            this.btnPathLocate.UseVisualStyleBackColor = false;
            this.btnPathLocate.Click += new System.EventHandler(this.OnOpenSvgSuitePath);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnCancel.FlatAppearance.BorderSize = 2;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(223, 255);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(142, 32);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.OnCancelClicked);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.SystemColors.Control;
            this.btnOK.Enabled = false;
            this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnOK.FlatAppearance.BorderSize = 2;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(420, 255);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(142, 32);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.OnOKClicked);
            // 
            // PromptDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(784, 312);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnPathLocate);
            this.Controls.Add(this.btnPathBrowse);
            this.Controls.Add(this.txtSvgSuitePath);
            this.Controls.Add(this.labelLocal);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.txtSvgSuitePathWeb);
            this.Controls.Add(this.labelWeb);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PromptDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "W3C SVG Test Suite";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelWeb;
        private System.Windows.Forms.TextBox txtSvgSuitePathWeb;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Label labelLocal;
        private System.Windows.Forms.TextBox txtSvgSuitePath;
        private System.Windows.Forms.Button btnPathBrowse;
        private System.Windows.Forms.Button btnPathLocate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}