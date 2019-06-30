namespace GdiW3cSvgTestSuite
{
    partial class ResultsDockPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultsDockPanel));
            this.panelTitle = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelContent = new System.Windows.Forms.Panel();
            this.testDetailsDoc = new TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel();
            this.panelTitle.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTitle
            // 
            this.panelTitle.Controls.Add(this.labelTitle);
            this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitle.Location = new System.Drawing.Point(0, 0);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(800, 32);
            this.panelTitle.TabIndex = 0;
            // 
            // labelTitle
            // 
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(800, 32);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "SVG Test Results";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelContent
            // 
            this.panelContent.Controls.Add(this.testDetailsDoc);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 32);
            this.panelContent.Name = "panelContent";
            this.panelContent.Padding = new System.Windows.Forms.Padding(15, 5, 15, 5);
            this.panelContent.Size = new System.Drawing.Size(800, 418);
            this.panelContent.TabIndex = 1;
            // 
            // testDetailsDoc
            // 
            this.testDetailsDoc.AutoScroll = true;
            this.testDetailsDoc.BackColor = System.Drawing.SystemColors.Window;
            this.testDetailsDoc.BaseStylesheet = null;
            this.testDetailsDoc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.testDetailsDoc.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.testDetailsDoc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testDetailsDoc.Location = new System.Drawing.Point(15, 5);
            this.testDetailsDoc.Name = "testDetailsDoc";
            this.testDetailsDoc.Size = new System.Drawing.Size(770, 408);
            this.testDetailsDoc.TabIndex = 0;
            this.testDetailsDoc.Text = null;
            this.testDetailsDoc.UseGdiPlusTextRendering = true;
            // 
            // ResultsDockPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelTitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ResultsDockPanel";
            this.Text = "ResultsDockPanel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.panelTitle.ResumeLayout(false);
            this.panelContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Panel panelContent;
        private TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel testDetailsDoc;
        private System.Windows.Forms.Label labelTitle;
    }
}