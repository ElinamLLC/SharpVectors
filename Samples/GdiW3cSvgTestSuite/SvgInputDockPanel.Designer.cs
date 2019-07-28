namespace GdiW3cSvgTestSuite
{
    partial class SvgInputDockPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SvgInputDockPanel));
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.textEditor = new ICSharpCode.TextEditor.TextEditorControl();
            this.toolBar = new System.Windows.Forms.ToolStrip();
            this.tbbFileNew = new System.Windows.Forms.ToolStripButton();
            this.tbbFileOpen = new System.Windows.Forms.ToolStripButton();
            this.tbbSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbbCut = new System.Windows.Forms.ToolStripButton();
            this.tbbCopy = new System.Windows.Forms.ToolStripButton();
            this.tbbPaste = new System.Windows.Forms.ToolStripButton();
            this.tbbDelete = new System.Windows.Forms.ToolStripButton();
            this.tbbSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbbSave = new System.Windows.Forms.ToolStripButton();
            this.tbbSaveAs = new System.Windows.Forms.ToolStripButton();
            this.tbbSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tbbUndo = new System.Windows.Forms.ToolStripButton();
            this.tbbRedo = new System.Windows.Forms.ToolStripButton();
            this.tbbSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tbbWordWrap = new System.Windows.Forms.ToolStripButton();
            this.tbbShowLineNumber = new System.Windows.Forms.ToolStripButton();
            this.tbbShowWhitespace = new System.Windows.Forms.ToolStripButton();
            this.tbbSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tbbComboBoxSyntax = new System.Windows.Forms.ToolStripComboBox();
            this.tbbSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tbbTextBoxFind = new System.Windows.Forms.ToolStripTextBox();
            this.tbbFind = new System.Windows.Forms.ToolStripButton();
            this.tbbSearchReplace = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.toolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.textEditor);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(800, 418);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.LeftToolStripPanelVisible = false;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.RightToolStripPanelVisible = false;
            this.toolStripContainer.Size = new System.Drawing.Size(800, 450);
            this.toolStripContainer.TabIndex = 0;
            this.toolStripContainer.Text = "toolStripContainer";
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.toolBar);
            // 
            // textEditor
            // 
            this.textEditor.AutoScroll = true;
            this.textEditor.AutoScrollMinSize = new System.Drawing.Size(41, 30);
            this.textEditor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.textEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditor.IsReadOnly = false;
            this.textEditor.Location = new System.Drawing.Point(0, 0);
            this.textEditor.Margin = new System.Windows.Forms.Padding(6);
            this.textEditor.Name = "textEditor";
            this.textEditor.Size = new System.Drawing.Size(800, 418);
            this.textEditor.TabIndex = 0;
            // 
            // toolBar
            // 
            this.toolBar.Dock = System.Windows.Forms.DockStyle.None;
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbbFileNew,
            this.tbbFileOpen,
            this.tbbSeparator1,
            this.tbbCut,
            this.tbbCopy,
            this.tbbPaste,
            this.tbbDelete,
            this.tbbSeparator2,
            this.tbbSave,
            this.tbbSaveAs,
            this.tbbSeparator6,
            this.tbbUndo,
            this.tbbRedo,
            this.tbbSeparator3,
            this.tbbWordWrap,
            this.tbbShowLineNumber,
            this.tbbShowWhitespace,
            this.tbbSeparator4,
            this.tbbComboBoxSyntax,
            this.tbbSeparator5,
            this.tbbTextBoxFind,
            this.tbbFind,
            this.tbbSearchReplace});
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.Padding = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.toolBar.Size = new System.Drawing.Size(800, 32);
            this.toolBar.Stretch = true;
            this.toolBar.TabIndex = 0;
            // 
            // tbbFileNew
            // 
            this.tbbFileNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbFileNew.Image = global::GdiW3cSvgTestSuite.Properties.Resources.NewFile;
            this.tbbFileNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbFileNew.Name = "tbbFileNew";
            this.tbbFileNew.Size = new System.Drawing.Size(23, 23);
            this.tbbFileNew.Text = "New File";
            this.tbbFileNew.Click += new System.EventHandler(this.OnClickFileNew);
            // 
            // tbbFileOpen
            // 
            this.tbbFileOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbFileOpen.Image = global::GdiW3cSvgTestSuite.Properties.Resources.Open;
            this.tbbFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbFileOpen.Name = "tbbFileOpen";
            this.tbbFileOpen.Size = new System.Drawing.Size(23, 23);
            this.tbbFileOpen.Text = "Open SVG File";
            this.tbbFileOpen.Click += new System.EventHandler(this.OnClickFileOpen);
            // 
            // tbbSeparator1
            // 
            this.tbbSeparator1.Name = "tbbSeparator1";
            this.tbbSeparator1.Size = new System.Drawing.Size(6, 26);
            // 
            // tbbCut
            // 
            this.tbbCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbCut.Image = global::GdiW3cSvgTestSuite.Properties.Resources.Cut;
            this.tbbCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbCut.Name = "tbbCut";
            this.tbbCut.Size = new System.Drawing.Size(23, 23);
            this.tbbCut.Text = "Cut To Clipboard";
            this.tbbCut.Click += new System.EventHandler(this.OnClickCut);
            // 
            // tbbCopy
            // 
            this.tbbCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbCopy.Image = global::GdiW3cSvgTestSuite.Properties.Resources.Copy;
            this.tbbCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbCopy.Name = "tbbCopy";
            this.tbbCopy.Size = new System.Drawing.Size(23, 23);
            this.tbbCopy.Text = "Copy To Clipboard";
            this.tbbCopy.Click += new System.EventHandler(this.OnClickCopy);
            // 
            // tbbPaste
            // 
            this.tbbPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbPaste.Image = global::GdiW3cSvgTestSuite.Properties.Resources.Paste;
            this.tbbPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbPaste.Name = "tbbPaste";
            this.tbbPaste.Size = new System.Drawing.Size(23, 23);
            this.tbbPaste.Text = "Paste From Clipboard";
            this.tbbPaste.Click += new System.EventHandler(this.OnClickPaste);
            // 
            // tbbDelete
            // 
            this.tbbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbDelete.Image = global::GdiW3cSvgTestSuite.Properties.Resources.Delete;
            this.tbbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbDelete.Name = "tbbDelete";
            this.tbbDelete.Size = new System.Drawing.Size(23, 23);
            this.tbbDelete.Text = "Delete Selected Text";
            this.tbbDelete.Click += new System.EventHandler(this.OnClickDelete);
            // 
            // tbbSeparator2
            // 
            this.tbbSeparator2.Name = "tbbSeparator2";
            this.tbbSeparator2.Size = new System.Drawing.Size(6, 26);
            // 
            // tbbSave
            // 
            this.tbbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbSave.Image = global::GdiW3cSvgTestSuite.Properties.Resources.Save;
            this.tbbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbSave.Name = "tbbSave";
            this.tbbSave.Size = new System.Drawing.Size(23, 23);
            this.tbbSave.Text = "Save";
            this.tbbSave.Click += new System.EventHandler(this.OnClickSave);
            // 
            // tbbSaveAs
            // 
            this.tbbSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbSaveAs.Image = global::GdiW3cSvgTestSuite.Properties.Resources.SaveAs;
            this.tbbSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbSaveAs.Name = "tbbSaveAs";
            this.tbbSaveAs.Size = new System.Drawing.Size(23, 23);
            this.tbbSaveAs.Text = "Save As";
            this.tbbSaveAs.Click += new System.EventHandler(this.OnClickSaveAs);
            // 
            // tbbSeparator6
            // 
            this.tbbSeparator6.Name = "tbbSeparator6";
            this.tbbSeparator6.Size = new System.Drawing.Size(6, 26);
            // 
            // tbbUndo
            // 
            this.tbbUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbUndo.Image = global::GdiW3cSvgTestSuite.Properties.Resources.Undo;
            this.tbbUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbUndo.Name = "tbbUndo";
            this.tbbUndo.Size = new System.Drawing.Size(23, 23);
            this.tbbUndo.Text = "Undo Last Editing";
            this.tbbUndo.Click += new System.EventHandler(this.OnClickUndo);
            // 
            // tbbRedo
            // 
            this.tbbRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbRedo.Image = global::GdiW3cSvgTestSuite.Properties.Resources.Redo;
            this.tbbRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbRedo.Name = "tbbRedo";
            this.tbbRedo.Size = new System.Drawing.Size(23, 23);
            this.tbbRedo.Text = "Redo Last Editing";
            this.tbbRedo.Click += new System.EventHandler(this.OnClickRedo);
            // 
            // tbbSeparator3
            // 
            this.tbbSeparator3.Name = "tbbSeparator3";
            this.tbbSeparator3.Size = new System.Drawing.Size(6, 26);
            // 
            // tbbWordWrap
            // 
            this.tbbWordWrap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbWordWrap.Image = global::GdiW3cSvgTestSuite.Properties.Resources.WordWrap;
            this.tbbWordWrap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbWordWrap.Name = "tbbWordWrap";
            this.tbbWordWrap.Size = new System.Drawing.Size(23, 23);
            this.tbbWordWrap.Text = "Toggle Word Wrap";
            this.tbbWordWrap.Visible = false;
            this.tbbWordWrap.Click += new System.EventHandler(this.OnClickWordWrap);
            // 
            // tbbShowLineNumber
            // 
            this.tbbShowLineNumber.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbShowLineNumber.Image = global::GdiW3cSvgTestSuite.Properties.Resources.PageNumber;
            this.tbbShowLineNumber.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbShowLineNumber.Name = "tbbShowLineNumber";
            this.tbbShowLineNumber.Size = new System.Drawing.Size(23, 23);
            this.tbbShowLineNumber.Text = "Toggle Line Number";
            this.tbbShowLineNumber.Click += new System.EventHandler(this.OnClickLineNumber);
            // 
            // tbbShowWhitespace
            // 
            this.tbbShowWhitespace.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbShowWhitespace.Image = global::GdiW3cSvgTestSuite.Properties.Resources.Space;
            this.tbbShowWhitespace.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbShowWhitespace.Name = "tbbShowWhitespace";
            this.tbbShowWhitespace.Size = new System.Drawing.Size(23, 23);
            this.tbbShowWhitespace.Text = "Toggle Whitespace";
            this.tbbShowWhitespace.Click += new System.EventHandler(this.OnClickWhitespace);
            // 
            // tbbSeparator4
            // 
            this.tbbSeparator4.Name = "tbbSeparator4";
            this.tbbSeparator4.Size = new System.Drawing.Size(6, 26);
            // 
            // tbbComboBoxSyntax
            // 
            this.tbbComboBoxSyntax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tbbComboBoxSyntax.Items.AddRange(new object[] {
            "C#",
            "VB.NET",
            "HTML",
            "XML",
            "SQL",
            "PHP",
            "JavaScript",
            "Lua"});
            this.tbbComboBoxSyntax.Name = "tbbComboBoxSyntax";
            this.tbbComboBoxSyntax.Size = new System.Drawing.Size(100, 26);
            this.tbbComboBoxSyntax.ToolTipText = "Syntax Language";
            // 
            // tbbSeparator5
            // 
            this.tbbSeparator5.Name = "tbbSeparator5";
            this.tbbSeparator5.Size = new System.Drawing.Size(6, 26);
            // 
            // tbbTextBoxFind
            // 
            this.tbbTextBoxFind.Name = "tbbTextBoxFind";
            this.tbbTextBoxFind.Size = new System.Drawing.Size(200, 26);
            this.tbbTextBoxFind.ToolTipText = "Search Box";
            this.tbbTextBoxFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPressSearch);
            // 
            // tbbFind
            // 
            this.tbbFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbFind.Image = global::GdiW3cSvgTestSuite.Properties.Resources.Find;
            this.tbbFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbFind.Name = "tbbFind";
            this.tbbFind.Size = new System.Drawing.Size(23, 23);
            this.tbbFind.Text = "Search";
            this.tbbFind.Click += new System.EventHandler(this.OnClickFind);
            // 
            // tbbSearchReplace
            // 
            this.tbbSearchReplace.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbSearchReplace.Image = global::GdiW3cSvgTestSuite.Properties.Resources.Replace;
            this.tbbSearchReplace.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbSearchReplace.Name = "tbbSearchReplace";
            this.tbbSearchReplace.Size = new System.Drawing.Size(23, 23);
            this.tbbSearchReplace.Text = "Search And Replace";
            this.tbbSearchReplace.Click += new System.EventHandler(this.OnClickSearchReplace);
            // 
            // SvgInputDockPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.toolStripContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SvgInputDockPanel";
            this.Text = "SvgInputDockPanel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.ToolStrip toolBar;
        private System.Windows.Forms.ToolStripButton tbbFileNew;
        private ICSharpCode.TextEditor.TextEditorControl textEditor;
        private System.Windows.Forms.ToolStripButton tbbFileOpen;
        private System.Windows.Forms.ToolStripSeparator tbbSeparator1;
        private System.Windows.Forms.ToolStripButton tbbCut;
        private System.Windows.Forms.ToolStripButton tbbCopy;
        private System.Windows.Forms.ToolStripButton tbbPaste;
        private System.Windows.Forms.ToolStripButton tbbDelete;
        private System.Windows.Forms.ToolStripSeparator tbbSeparator2;
        private System.Windows.Forms.ToolStripButton tbbUndo;
        private System.Windows.Forms.ToolStripButton tbbRedo;
        private System.Windows.Forms.ToolStripSeparator tbbSeparator3;
        private System.Windows.Forms.ToolStripButton tbbWordWrap;
        private System.Windows.Forms.ToolStripButton tbbShowLineNumber;
        private System.Windows.Forms.ToolStripButton tbbShowWhitespace;
        private System.Windows.Forms.ToolStripSeparator tbbSeparator4;
        private System.Windows.Forms.ToolStripComboBox tbbComboBoxSyntax;
        private System.Windows.Forms.ToolStripSeparator tbbSeparator5;
        private System.Windows.Forms.ToolStripButton tbbFind;
        private System.Windows.Forms.ToolStripButton tbbSearchReplace;
        private System.Windows.Forms.ToolStripTextBox tbbTextBoxFind;
        private System.Windows.Forms.ToolStripButton tbbSave;
        private System.Windows.Forms.ToolStripButton tbbSaveAs;
        private System.Windows.Forms.ToolStripSeparator tbbSeparator6;
    }
}