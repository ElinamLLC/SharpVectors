namespace GdiW3cSvgTestSuite
{
    partial class PopupMessage
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
            this.components = new System.ComponentModel.Container();
            this.txtMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            this.txtMessage.BackColor = System.Drawing.SystemColors.Window;
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.txtMessage.Location = new System.Drawing.Point(1, 1);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(528, 74);
            this.txtMessage.TabIndex = 0;
            // 
            // PopupMessage
            // 
            this.Controls.Add(this.txtMessage);
            this.Name = "PopupMessage";
            this.Padding = new System.Windows.Forms.Padding(0);
            this.Size = new System.Drawing.Size(530, 76);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label txtMessage;
    }
}
