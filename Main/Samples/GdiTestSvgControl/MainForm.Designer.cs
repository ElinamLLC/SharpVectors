namespace TestSvgControl
{
    partial class MainForm
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
            this.svgPictureBox = new SharpVectors.Renderers.Forms.SvgPictureBox();
            this.SuspendLayout();
            // 
            // svgPictureBox
            // 
            this.svgPictureBox.BackColor = System.Drawing.SystemColors.Window;
            this.svgPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.svgPictureBox.Location = new System.Drawing.Point(0, 0);
            this.svgPictureBox.Name = "svgPictureBox";
            this.svgPictureBox.Size = new System.Drawing.Size(892, 659);
            this.svgPictureBox.Surface = null;
            this.svgPictureBox.TabIndex = 0;
            this.svgPictureBox.Text = "svgPictureBox1";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 659);
            this.Controls.Add(this.svgPictureBox);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Testing Svg";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
            this.ResumeLayout(false);

        }

        #endregion

        private SharpVectors.Renderers.Forms.SvgPictureBox svgPictureBox;
    }
}

