namespace GdiW3cSvgTestSuite
{
    partial class TestDockPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestDockPanel));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.panelConverted = new System.Windows.Forms.Panel();
            this.panelExpected = new System.Windows.Forms.Panel();
            this.labelConverted = new System.Windows.Forms.Label();
            this.labelExpected = new System.Windows.Forms.Label();
            this.viewerConverted = new GdiW3cSvgTestSuite.ImageViewer();
            this.viewerExpected = new GdiW3cSvgTestSuite.ImageViewer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panelConverted.SuspendLayout();
            this.panelExpected.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.viewerConverted);
            this.splitContainer.Panel1.Controls.Add(this.panelConverted);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.viewerExpected);
            this.splitContainer.Panel2.Controls.Add(this.panelExpected);
            this.splitContainer.Size = new System.Drawing.Size(747, 850);
            this.splitContainer.SplitterDistance = 426;
            this.splitContainer.TabIndex = 0;
            // 
            // panelConverted
            // 
            this.panelConverted.Controls.Add(this.labelConverted);
            this.panelConverted.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelConverted.Location = new System.Drawing.Point(0, 0);
            this.panelConverted.Name = "panelConverted";
            this.panelConverted.Size = new System.Drawing.Size(747, 32);
            this.panelConverted.TabIndex = 0;
            // 
            // panelExpected
            // 
            this.panelExpected.Controls.Add(this.labelExpected);
            this.panelExpected.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelExpected.Location = new System.Drawing.Point(0, 0);
            this.panelExpected.Name = "panelExpected";
            this.panelExpected.Size = new System.Drawing.Size(747, 32);
            this.panelExpected.TabIndex = 0;
            // 
            // labelConverted
            // 
            this.labelConverted.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelConverted.Location = new System.Drawing.Point(0, 0);
            this.labelConverted.Margin = new System.Windows.Forms.Padding(3);
            this.labelConverted.Name = "labelConverted";
            this.labelConverted.Size = new System.Drawing.Size(747, 32);
            this.labelConverted.TabIndex = 0;
            this.labelConverted.Text = "Converted Image";
            this.labelConverted.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelExpected
            // 
            this.labelExpected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelExpected.Location = new System.Drawing.Point(0, 0);
            this.labelExpected.Margin = new System.Windows.Forms.Padding(3);
            this.labelExpected.Name = "labelExpected";
            this.labelExpected.Size = new System.Drawing.Size(747, 32);
            this.labelExpected.TabIndex = 0;
            this.labelExpected.Text = "Expected Image";
            this.labelExpected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // viewerConverted
            // 
            this.viewerConverted.AutoScroll = true;
            this.viewerConverted.BackColor = System.Drawing.SystemColors.Window;
            this.viewerConverted.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewerConverted.Image = null;
            this.viewerConverted.ImageSizeMode = GdiW3cSvgTestSuite.SizeMode.Scrollable;
            this.viewerConverted.Location = new System.Drawing.Point(0, 32);
            this.viewerConverted.Name = "viewerConverted";
            this.viewerConverted.Size = new System.Drawing.Size(747, 394);
            this.viewerConverted.TabIndex = 1;
            // 
            // viewerExpected
            // 
            this.viewerExpected.AutoScroll = true;
            this.viewerExpected.BackColor = System.Drawing.SystemColors.Window;
            this.viewerExpected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewerExpected.Image = null;
            this.viewerExpected.ImageSizeMode = GdiW3cSvgTestSuite.SizeMode.Scrollable;
            this.viewerExpected.Location = new System.Drawing.Point(0, 32);
            this.viewerExpected.Name = "viewerExpected";
            this.viewerExpected.Size = new System.Drawing.Size(747, 388);
            this.viewerExpected.TabIndex = 1;
            // 
            // TestDockPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 850);
            this.Controls.Add(this.splitContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TestDockPanel";
            this.Text = "TestDockPanel";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panelConverted.ResumeLayout(false);
            this.panelExpected.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel panelConverted;
        private ImageViewer viewerConverted;
        private System.Windows.Forms.Panel panelExpected;
        private ImageViewer viewerExpected;
        private System.Windows.Forms.Label labelConverted;
        private System.Windows.Forms.Label labelExpected;
    }
}