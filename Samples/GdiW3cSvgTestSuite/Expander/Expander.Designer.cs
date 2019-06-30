namespace WinFormsExpander
{
    partial class Expander
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.header = new System.Windows.Forms.Label();
            this.expandButton = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.header);
            this.splitContainer.Panel1.Controls.Add(this.expandButton);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.AutoScroll = true;
            this.splitContainer.Panel2.AutoScrollMinSize = new System.Drawing.Size(150, 50);
            this.splitContainer.Size = new System.Drawing.Size(150, 93);
            this.splitContainer.SplitterDistance = 27;
            this.splitContainer.SplitterWidth = 6;
            this.splitContainer.TabIndex = 0;
            // 
            // header
            // 
            this.header.BackColor = System.Drawing.SystemColors.Control;
            this.header.Dock = System.Windows.Forms.DockStyle.Fill;
            this.header.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.header.Location = new System.Drawing.Point(24, 0);
            this.header.MaximumSize = new System.Drawing.Size(0, 25);
            this.header.MinimumSize = new System.Drawing.Size(75, 25);
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(126, 25);
            this.header.TabIndex = 0;
            this.header.Text = "Header";
            this.header.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // expandButton
            // 
            this.expandButton.AutoSize = true;
            this.expandButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.expandButton.Image = global::GdiW3cSvgTestSuite.Properties.Resources.Collapse;
            this.expandButton.Location = new System.Drawing.Point(0, 0);
            this.expandButton.MinimumSize = new System.Drawing.Size(24, 24);
            this.expandButton.Name = "expandButton";
            this.expandButton.Size = new System.Drawing.Size(24, 24);
            this.expandButton.TabIndex = 1;
            this.expandButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Expander
            // 
            this.AutoScroll = true;
            this.Controls.Add(this.splitContainer);
            this.MinimumSize = new System.Drawing.Size(150, 25);
            this.Name = "Expander";
            this.Size = new System.Drawing.Size(150, 93);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Label header;
        private System.Windows.Forms.Label expandButton;
    }
}
