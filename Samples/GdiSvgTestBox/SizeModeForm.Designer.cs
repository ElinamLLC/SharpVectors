namespace GdiSvgTestBox
{
    partial class SizeModeForm
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
            this.components = new System.ComponentModel.Container();
            this.lblSize = new System.Windows.Forms.Label();
            this.cbbSizeMode = new System.Windows.Forms.ComboBox();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.timerMouse = new System.Windows.Forms.Timer(this.components);
            this.panelContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(14, 12);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(57, 12);
            this.lblSize.TabIndex = 0;
            this.lblSize.Text = "Size Mode";
            this.lblSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbbSizeMode
            // 
            this.cbbSizeMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbSizeMode.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbbSizeMode.FormattingEnabled = true;
            this.cbbSizeMode.Location = new System.Drawing.Point(91, 8);
            this.cbbSizeMode.Name = "cbbSizeMode";
            this.cbbSizeMode.Size = new System.Drawing.Size(189, 20);
            this.cbbSizeMode.TabIndex = 1;
            this.cbbSizeMode.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
            // 
            // panelContainer
            // 
            this.panelContainer.BackColor = System.Drawing.SystemColors.Window;
            this.panelContainer.Controls.Add(this.lblSize);
            this.panelContainer.Controls.Add(this.cbbSizeMode);
            this.panelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContainer.Location = new System.Drawing.Point(1, 8);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(301, 42);
            this.panelContainer.TabIndex = 2;
            // 
            // timerMouse
            // 
            this.timerMouse.Interval = 1000;
            this.timerMouse.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // SizeModeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(303, 51);
            this.Controls.Add(this.panelContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SizeModeForm";
            this.Padding = new System.Windows.Forms.Padding(1, 8, 1, 1);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SizeModeForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.panelContainer.ResumeLayout(false);
            this.panelContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.ComboBox cbbSizeMode;
        private System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.Timer timerMouse;
    }
}