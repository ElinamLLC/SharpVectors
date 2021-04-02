using System;
using System.Xml.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace SharpVectors.Renderers.Forms
{
    internal sealed class SvgFilesUITypeEditor : UITypeEditor
    {
        private OpenFileDialog ofd;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService editorService =
                (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    if (this.ofd == null)
                    {
                        this.ofd = new OpenFileDialog();
                    }
                    ofd.Multiselect = false;
                    ofd.Filter      = "Svg Files (*.svg;*.svgz;*.xml)|*.svg;*.svgz;*.xml|All files (*.*)|*.*";
                    ofd.FileName    = "";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        value = ofd.FileName;
                    }
                }
            }
            return value;
        }
    }

    internal sealed class SvgUrlUITypeEditor : UITypeEditor
    {
        private SvgUrlDialog uriDlg;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService editorService =
                (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    if (uriDlg == null)
                    {
                        uriDlg = new SvgUrlDialog();
                    }
                    uriDlg.SvgUrl = value as Uri;
                    if (uriDlg.ShowDialog() == DialogResult.OK)
                    {
                        value = uriDlg.SvgUrl;
                    }
                }
            }
            return value;
        }
    }

    internal sealed class SvgTextUITypeEditor : UITypeEditor
    {
        private SvgTextDialog textDlg;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService editorService =
                (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    if (textDlg == null)
                    {
                        textDlg = new SvgTextDialog();
                    }
                    textDlg.SvgText = value as string;
                    if (textDlg.ShowDialog() == DialogResult.OK)
                    {
                        value = textDlg.SvgText;
                    }
                }
            }
            return value;
        }
    }

    internal sealed class SvgPictureBoxActionList : DesignerActionList
    {
        private SvgPictureBoxDesigner _designer;

        public SvgPictureBoxActionList(SvgPictureBoxDesigner designer) : base(designer.Component)
        {
            this._designer = designer;
        }

        public void SelectSource()
        {
            try
            {
                EditorServiceContext.EditValue(_designer, base.Component, "Source");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void SelectUrlSource()
        {
            try
            {
                EditorServiceContext.EditValue(_designer, base.Component, "UriSource");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void EnterXmlSource()
        {
            try
            {
                EditorServiceContext.EditValue(_designer, base.Component, "XmlSource");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            items.Add(new DesignerActionMethodItem(this, "SelectSource",
                "Select SVG File", "Data", "Select the SVG File", true));
            items.Add(new DesignerActionMethodItem(this, "SelectUrlSource",
                "Select SVG Url", "Data", "Select the Url of SVG File", true));
            items.Add(new DesignerActionMethodItem(this, "EnterXmlSource",
                "Enter SVG Content", "Data", "Enter the XML Content", true));

            items.Add(new DesignerActionPropertyItem("SizeMode", 
                "Size Mode", "Behavior", "Controls the placement of the Drawing in the control"));
            return items;
        }

        public PictureBoxSizeMode SizeMode
        {
            get {
                return ((SvgPictureBox)base.Component).SizeMode;
            }
            set {
                TypeDescriptor.GetProperties(base.Component)["SizeMode"].SetValue(base.Component, value);
            }
        }
    }

    internal sealed class SvgPictureBoxDesigner : ControlDesigner
    {
        private DesignerActionListCollection _actionLists;

        public SvgPictureBoxDesigner()
        {
            base.AutoResizeHandles = true;
        }

        private void DrawBorder(Graphics graphics)
        {
            Color color;
            Control control = this.Control;
            Rectangle clientRectangle = control.ClientRectangle;
            if (control.BackColor.GetBrightness() < 0.5)
            {
                color = ControlPaint.Light(control.BackColor);
            }
            else
            {
                color = ControlPaint.Dark(control.BackColor);
            }
            Pen pen = new Pen(color)
            {
                DashStyle = DashStyle.Dash
            };
            clientRectangle.Width--;
            clientRectangle.Height--;
            graphics.DrawRectangle(pen, clientRectangle);
            pen.Dispose();
        }

        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            SvgPictureBox component = (SvgPictureBox)base.Component;
            if (component.BorderStyle == BorderStyle.None)
            {
                this.DrawBorder(pe.Graphics);
            }
            base.OnPaintAdornments(pe);
        }

        public override DesignerActionListCollection ActionLists
        {
            get {
                if (this._actionLists == null)
                {
                    this._actionLists = new DesignerActionListCollection();
                    this._actionLists.Add(new SvgPictureBoxActionList(this));
                }
                return this._actionLists;
            }
        }

        public override System.Windows.Forms.Design.SelectionRules SelectionRules
        {
            get {
                System.Windows.Forms.Design.SelectionRules selectionRules = base.SelectionRules;
                object component = base.Component;
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(base.Component)["SizeMode"];
                if (descriptor != null)
                {
                    PictureBoxSizeMode mode = (PictureBoxSizeMode)descriptor.GetValue(component);
                    if (mode == PictureBoxSizeMode.AutoSize)
                    {
                        selectionRules &= ~System.Windows.Forms.Design.SelectionRules.AllSizeable;
                    }
                }
                return selectionRules;
            }
        }
    }

    internal sealed class EditorServiceContext : IWindowsFormsEditorService, ITypeDescriptorContext, IServiceProvider
    {
        // Fields
        private IComponentChangeService _componentChangeSvc;
        private ComponentDesigner _designer;
        private PropertyDescriptor _targetProperty;

        // Methods
        internal EditorServiceContext(ComponentDesigner designer)
        {
            this._designer = designer;
        }

        internal EditorServiceContext(ComponentDesigner designer, PropertyDescriptor prop)
        {
            _designer = designer;
            _targetProperty = prop;
            if (prop == null)
            {
                prop = TypeDescriptor.GetDefaultProperty(designer.Component);
                if ((prop != null) && typeof(ICollection).IsAssignableFrom(prop.PropertyType))
                {
                    this._targetProperty = prop;
                }
            }
        }

        internal EditorServiceContext(ComponentDesigner designer, PropertyDescriptor prop, string newVerbText) : this(designer, prop)
        {
            _designer.Verbs.Add(new DesignerVerb(newVerbText, new EventHandler(this.OnEditItems)));
        }

        public static object EditValue(ComponentDesigner designer, object objectToChange, string propName)
        {
            PropertyDescriptor prop = TypeDescriptor.GetProperties(objectToChange)[propName];
            EditorServiceContext context = new EditorServiceContext(designer, prop);
            if (prop == null)
            {
                throw new Exception("propName: " + propName);
            }
            object obj2 = prop.GetValue(objectToChange);
            object obj3 = (prop.GetEditor(typeof(UITypeEditor)) as UITypeEditor).EditValue(context, context, obj2);
            if (obj3 != obj2)
            {
                try
                {
                    prop.SetValue(objectToChange, obj3);
                }
                catch (CheckoutException)
                {
                }
            }
            return obj3;
        }

        private void OnEditItems(object sender, EventArgs e)
        {
            object component = this._targetProperty.GetValue(this._designer.Component);
            if (component != null)
            {
                CollectionEditor editor = TypeDescriptor.GetEditor(component, typeof(UITypeEditor)) as CollectionEditor;
                if (editor != null)
                {
                    editor.EditValue(this, this, component);
                }
            }
        }

        void ITypeDescriptorContext.OnComponentChanged()
        {
            this.ChangeService.OnComponentChanged(this._designer.Component, this._targetProperty, null, null);
        }

        bool ITypeDescriptorContext.OnComponentChanging()
        {
            try
            {
                this.ChangeService.OnComponentChanging(this._designer.Component, this._targetProperty);
            }
            catch (CheckoutException exception1)
            {
                if (exception1 != CheckoutException.Canceled)
                {
                    throw;
                }
                return false;
            }
            return true;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            if ((serviceType == typeof(ITypeDescriptorContext)) || (serviceType == typeof(IWindowsFormsEditorService)))
            {
                return this;
            }
            if ((this._designer.Component != null) && (this._designer.Component.Site != null))
            {
                return this._designer.Component.Site.GetService(serviceType);
            }
            return null;
        }

        void IWindowsFormsEditorService.CloseDropDown()
        {
        }

        void IWindowsFormsEditorService.DropDownControl(Control control)
        {
        }

        DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog)
        {
            IUIService service = (IUIService)((IServiceProvider)this).GetService(typeof(IUIService));
            if (service != null)
            {
                return service.ShowDialog(dialog);
            }
            return dialog.ShowDialog(this._designer.Component as IWin32Window);
        }

        // Properties
        private IComponentChangeService ChangeService
        {
            get {
                if (this._componentChangeSvc == null)
                {
                    this._componentChangeSvc = (IComponentChangeService)((IServiceProvider)this).GetService(
                        typeof(IComponentChangeService));
                }
                return this._componentChangeSvc;
            }
        }

        IContainer ITypeDescriptorContext.Container
        {
            get {
                if (this._designer.Component.Site != null)
                {
                    return this._designer.Component.Site.Container;
                }
                return null;
            }
        }

        object ITypeDescriptorContext.Instance
        {
            get {
                return this._designer.Component;
            }
        }

        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
        {
            get {
                return this._targetProperty;
            }
        }
    }

    #region SvgUrlDialog class

    internal sealed class SvgUrlDialog : Form
    {
        private Button btnCancel;
        private Label labelPrompt;
        private Label labelUrl;
        private TextBox txtUrl;
        private Button btnUrl;
        private Button btnOK;

        private Uri _svgUri;

        public SvgUrlDialog()
        {
            btnCancel = new Button();
            labelPrompt = new Label();
            labelUrl = new Label();
            txtUrl = new TextBox();
            btnUrl = new Button();
            btnOK = new Button();

            this.SuspendLayout();

            btnCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            btnCancel.Location = new Point(549, 79);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(78, 23);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += new EventHandler(OnClickedCancel);

            labelPrompt.AutoSize = true;
            labelPrompt.Location = new Point(12, 16);
            labelPrompt.Name = "labelPrompt";
            labelPrompt.Size = new Size(459, 12);
            labelPrompt.TabIndex = 2;
            labelPrompt.Text = "Enter the World Wide Web location (URL) or specify the local SVG file you want to open.";

            labelUrl.AutoSize = true;
            labelUrl.Location = new Point(25, 42);
            labelUrl.Name = "labelUrl";
            labelUrl.Size = new Size(50, 12);
            labelUrl.TabIndex = 3;
            labelUrl.Text = "Location:";
            labelUrl.TextAlign = ContentAlignment.MiddleLeft;

            txtUrl.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            txtUrl.Location = new Point(81, 37);
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new Size(510, 19);
            txtUrl.TabIndex = 4;

            btnUrl.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            btnUrl.Location = new Point(597, 34);
            btnUrl.Name = "btnUrl";
            btnUrl.Size = new Size(30, 24);
            btnUrl.TabIndex = 5;
            btnUrl.Text = "...";
            btnUrl.UseVisualStyleBackColor = true;
            btnUrl.Click += new EventHandler(OnClickedUrl);

            btnOK.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            btnOK.Location = new Point(454, 79);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(78, 23);
            btnOK.TabIndex = 6;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += new EventHandler(OnClickedOK);

            AutoScaleDimensions = new SizeF(6F, 12F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(644, 116);
            Controls.Add(btnOK);
            Controls.Add(btnUrl);
            Controls.Add(txtUrl);
            Controls.Add(labelUrl);
            Controls.Add(labelPrompt);
            Controls.Add(btnCancel);
            MaximizeBox = false;
            MaximumSize = new Size(800, 160);
            MinimizeBox = false;
            MinimumSize = new Size(650, 150);
            Name = "SvgUrlUIEditor";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "SVG Url";
            Load += new EventHandler(OnFormLoad);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public Uri SvgUrl
        {
            get {
                return _svgUri;
            }
            set {
                _svgUri = value;
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            try
            {
                if (_svgUri != null && _svgUri.IsAbsoluteUri)
                {
                    txtUrl.Text = _svgUri.AbsoluteUri;
                }
            }
            catch
            {
            }
        }

        private void OnClickedUrl(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Svg Files (*.svg;*.svgz;*.xml)|*.svg;*.svgz;*.xml|All files (*.*)|*.*";
            ofd.FileName = "";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string strUrl = ofd.FileName;
                Uri svgUri;
                if (!Uri.TryCreate(strUrl, UriKind.Absolute, out svgUri))
                {
                    MessageBox.Show("The location URL is invalid", "SharpVectors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                txtUrl.Text = svgUri.LocalPath;
                _svgUri = svgUri;
            }
        }

        private void OnClickedOK(object sender, EventArgs e)
        {
            string strUrl = txtUrl.Text.Trim();
            if (string.IsNullOrWhiteSpace(strUrl))
            {
                _svgUri = null;
                this.Close();
            }
            else
            {
                Uri svgUri;
                if (!Uri.TryCreate(strUrl, UriKind.Absolute, out svgUri))
                {
                    MessageBox.Show("The location URL is invalid", "SharpVectors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _svgUri = null;
                    return;
                }
                else
                {
                    _svgUri = svgUri;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void OnClickedCancel(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    #endregion

    #region SvgTextDialog class

    internal sealed class SvgTextDialog : Form
    {
        private Label labelPrompt;
        private RichTextBox txtSvg;
        private Button btnOK;
        private Button btnCancel;

        private string _svgText;

        public SvgTextDialog()
        {
            _svgText = string.Empty;

            this.labelPrompt = new Label();
            this.txtSvg = new RichTextBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            this.labelPrompt.AutoSize = true;
            this.labelPrompt.Location = new Point(13, 13);
            this.labelPrompt.Name = "labelPrompt";
            this.labelPrompt.Size = new Size(273, 12);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "Enter the SVG content to be rendered in the control.";

            this.txtSvg.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.txtSvg.Location = new Point(34, 33);
            this.txtSvg.Name = "txtSvg";
            this.txtSvg.Size = new Size(496, 129);
            this.txtSvg.TabIndex = 1;
            this.txtSvg.Text = "";

            this.btnOK.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            this.btnOK.Location = new Point(362, 176);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.OnClickedOK);

            this.btnCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            this.btnCancel.Location = new Point(455, 176);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.OnClickedCancel);

            this.AutoScaleDimensions = new SizeF(6F, 12F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(564, 211);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtSvg);
            this.Controls.Add(this.labelPrompt);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(360, 210);
            this.Name = "SvgTextDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Svg Text";
            this.Load += new EventHandler(this.OnFormLoad);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public string SvgText
        {
            get {
                return _svgText;
            }
            set {
                if (value == null)
                {
                    value = string.Empty;
                }
                _svgText = value;
            }
        }

        private void OnClickedOK(object sender, EventArgs e)
        {
            string svgText = txtSvg.Text.Trim();
            _svgText = svgText;
            if (string.IsNullOrWhiteSpace(svgText))
            {
                this.DialogResult = DialogResult.OK;
                return;
            }
            else
            {
                try
                {
                    var xmlDoc = XDocument.Parse(svgText);

                    this.DialogResult = DialogResult.OK;
                    return;
                }
                catch
                {
                    MessageBox.Show("The SVG Content is invalid", "SharpVectors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void OnClickedCancel(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_svgText))
            {
                txtSvg.Text = _svgText;
            }

        }
    }

    #endregion
}
