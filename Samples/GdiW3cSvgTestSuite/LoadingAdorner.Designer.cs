namespace GdiW3cSvgTestSuite
{
    partial class LoadingAdorner
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
            this.progressIndicator = new ProgressControls.ProgressIndicator();
            this.SuspendLayout();
            // 
            // progressIndicator
            // 
            this.progressIndicator.BackColor = System.Drawing.Color.Transparent;
            this.progressIndicator.CircleColor = System.Drawing.Color.DodgerBlue;
            this.progressIndicator.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.progressIndicator.Location = new System.Drawing.Point(269, 44);
            this.progressIndicator.Name = "progressIndicator";
            this.progressIndicator.Percentage = 0F;
            this.progressIndicator.ShowText = true;
            this.progressIndicator.Size = new System.Drawing.Size(263, 263);
            this.progressIndicator.TabIndex = 0;
            this.progressIndicator.Text = "Download...";
            this.progressIndicator.TextDisplay = ProgressControls.TextDisplayModes.Text;
            // 
            // LoadingAdorner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(800, 350);
            this.Controls.Add(this.progressIndicator);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadingAdorner";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LoadingAdorner";
            this.UseWaitCursor = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private ProgressControls.ProgressIndicator progressIndicator;
    }
}