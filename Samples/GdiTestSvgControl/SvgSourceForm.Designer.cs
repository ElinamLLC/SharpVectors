namespace TestSvgControl
{
    partial class SvgSourceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SvgSourceForm));
            this.labelDesc = new System.Windows.Forms.Label();
            this.labelWiki = new System.Windows.Forms.Label();
            this.cbxWiki = new System.Windows.Forms.ComboBox();
            this.linkWiki = new System.Windows.Forms.LinkLabel();
            this.labelSource = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.btnSource = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelDesc
            // 
            this.labelDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDesc.Location = new System.Drawing.Point(20, 30);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(764, 47);
            this.labelDesc.TabIndex = 0;
            this.labelDesc.Text = "Select from the prefined sources (Wikipedia files) named in the select box, direc" +
    "tly enter a valid local SVG file path or web uri of an SVG file or browse to sel" +
    "ect a local SVG file path.";
            // 
            // labelWiki
            // 
            this.labelWiki.AutoSize = true;
            this.labelWiki.Location = new System.Drawing.Point(32, 100);
            this.labelWiki.Name = "labelWiki";
            this.labelWiki.Size = new System.Drawing.Size(132, 12);
            this.labelWiki.TabIndex = 1;
            this.labelWiki.Text = "Wikipedia Media Sources";
            // 
            // cbxWiki
            // 
            this.cbxWiki.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxWiki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxWiki.FormattingEnabled = true;
            this.cbxWiki.ItemHeight = 12;
            this.cbxWiki.Location = new System.Drawing.Point(75, 125);
            this.cbxWiki.Name = "cbxWiki";
            this.cbxWiki.Size = new System.Drawing.Size(709, 20);
            this.cbxWiki.TabIndex = 2;
            this.cbxWiki.SelectedIndexChanged += new System.EventHandler(this.OnSelectionChanged);
            // 
            // linkWiki
            // 
            this.linkWiki.AutoSize = true;
            this.linkWiki.Location = new System.Drawing.Point(189, 100);
            this.linkWiki.Name = "linkWiki";
            this.linkWiki.Size = new System.Drawing.Size(363, 12);
            this.linkWiki.TabIndex = 3;
            this.linkWiki.TabStop = true;
            this.linkWiki.Text = "https://commons.wikimedia.org/wiki/Category:Valid_SVG_created_with";
            this.linkWiki.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            // 
            // labelSource
            // 
            this.labelSource.AutoSize = true;
            this.labelSource.Location = new System.Drawing.Point(34, 162);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(115, 12);
            this.labelSource.TabIndex = 4;
            this.labelSource.Text = "Selected SVG Source";
            // 
            // txtSource
            // 
            this.txtSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSource.Location = new System.Drawing.Point(75, 187);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(709, 19);
            this.txtSource.TabIndex = 5;
            // 
            // btnSource
            // 
            this.btnSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSource.Location = new System.Drawing.Point(653, 213);
            this.btnSource.Name = "btnSource";
            this.btnSource.Size = new System.Drawing.Size(131, 27);
            this.btnSource.TabIndex = 6;
            this.btnSource.Text = "Browse...";
            this.btnSource.UseVisualStyleBackColor = true;
            this.btnSource.Click += new System.EventHandler(this.OnSourceClick);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(252, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(113, 32);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Cancel";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.OnCancelClick);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(423, 9);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(113, 32);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnOKClick);
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.Control;
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Controls.Add(this.btnOK);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(8, 254);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(788, 50);
            this.panelBottom.TabIndex = 9;
            // 
            // SvgSourceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(804, 312);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.btnSource);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.labelSource);
            this.Controls.Add(this.linkWiki);
            this.Controls.Add(this.cbxWiki);
            this.Controls.Add(this.labelWiki);
            this.Controls.Add(this.labelDesc);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1200, 350);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(820, 350);
            this.Name = "SvgSourceForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SVG Source";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDesc;
        private System.Windows.Forms.Label labelWiki;
        private System.Windows.Forms.ComboBox cbxWiki;
        private System.Windows.Forms.LinkLabel linkWiki;
        private System.Windows.Forms.Label labelSource;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Button btnSource;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panelBottom;
    }
}