//-------------------------------------------------------------------------------- 
// Original source: https://github.com/mkoertgen/winforms.expander
// Modified for the Windows Forms test suite application
//-------------------------------------------------------------------------------- 

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace WinFormsExpander
{
    [Designer(typeof (ExpanderDesigner))]
    public partial class Expander : UserControl
    {
        private static readonly Bitmap DefaultCollapseImage = GdiW3cSvgTestSuite.Properties.Resources.Collapse;
        private static readonly Bitmap DefaultExpandImage   = GdiW3cSvgTestSuite.Properties.Resources.Expand;

        private Image _collapseImage = DefaultCollapseImage;
        private Image _expandImage = DefaultExpandImage;
        private Size _minimumSize;

        public event EventHandler ExpandedChanged;

        public Expander()
        {
            InitializeComponent();

            expandButton.Click += (sender, args) => IsExpanded = !IsExpanded;
            header.Click += (sender, args) => IsExpanded = !IsExpanded;

            // disable hover/click color effect, cf.: http://stackoverflow.com/a/19654946/2592915
            //expandButton.FlatStyle = FlatStyle.Flat;
            //expandButton.FlatAppearance.BorderSize = 0;
            //expandButton.FlatAppearance.MouseDownBackColor = expandButton.BackColor;
            //expandButton.FlatAppearance.MouseOverBackColor = expandButton.BackColor;
            //expandButton.BackColorChanged += (s, e) =>
            //{
            //    expandButton.FlatAppearance.MouseDownBackColor =
            //        expandButton.FlatAppearance.MouseOverBackColor = expandButton.BackColor;
            //};

            CollapsedHeight = splitContainer.SplitterDistance;
            _minimumSize = GetMinimumSize();
        }

        public override Size MinimumSize => _minimumSize;

        [Category("Appearance"), DefaultValue(typeof(int), "30")]
        // ReSharper disable once MemberInitializerValueIgnored
        public int CollapsedHeight { get; } = 30;

        public int ExpandedHeight { get; private set; }

        [Category("Appearance"), DefaultValue(typeof(int), "30")]
        public int MinimumContentHeight { get; set; } = 30;

        [Category("Appearance"), DefaultValue(typeof (string), "Header")]
        public string Header
        {
            get { return header.Text; }
            set { header.Text = value; }
        }

        public Font HeaderFont
        {
            get {
                return header.Font;
            }
            set {
                if (value != null)
                {
                    header.Font = value;
                }
            }
        }

        [Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel Content => splitContainer.Panel2;

        [Category("Appearance"), DefaultValue(typeof(bool), "true")]
        public bool IsExpanded
        {
            get { return !splitContainer.Panel2Collapsed; }
            set
            {
                if (splitContainer.Panel2Collapsed != value) return;
                if (!CanToggle) return;

                splitContainer.Panel2Collapsed = !value;
                expandButton.Image = value ? CollapseImage : ExpandImage;

                expandButton.ImageAlign = ContentAlignment.MiddleCenter;

                _minimumSize = GetMinimumSize();
                if (value)
                {
                    MaximumSize = new Size(MaximumSize.Width, 0);
                    Size = new Size(Size.Width, ExpandedHeight);
                }
                else
                {
                    ExpandedHeight = ClientRectangle.Height + 6; // Include the ExpanderTablePanel.SplitterSize
                    MaximumSize = new Size(MaximumSize.Width, CollapsedHeight);
                    Size = new Size(Size.Width, header.Height);
                }

                ExpandedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        internal bool CanToggle
        {
            get { return expandButton.Enabled; }
            set
            {
                if (expandButton.Enabled == value) return;
                expandButton.Enabled = value;
            }
        }

        [Category("Appearance")]
        public Image CollapseImage
        {
            get { return _collapseImage; }
            set { _collapseImage = value ?? DefaultCollapseImage; }
        }

        [Category("Appearance")]
        public Image ExpandImage
        {
            get { return _expandImage; }
            set { _expandImage = value ?? DefaultExpandImage; }
        }

        private Size GetMinimumSize()
        {
            var minHeight = CollapsedHeight;
            if (IsExpanded) minHeight += splitContainer.SplitterWidth + MinimumContentHeight;
            return new Size(0, minHeight);
        }
    }
}
