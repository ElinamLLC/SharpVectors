// Credit: Gil Klod, https://www.codeproject.com/Articles/4022/Scrollable-and-RatioStretch-PictureBox

using System;
using System.Drawing;
using System.Windows.Forms;

namespace GdiW3cSvgTestSuite
{
    public enum SizeMode
    {
        Scrollable,
        RatioStretch
    }

    public partial class ImageViewer : UserControl
    {
        private SizeMode _sizeMode;

        public ImageViewer()
        {
            InitializeComponent();

            _sizeMode = SizeMode.RatioStretch;
            this.AutoScroll = (_sizeMode == SizeMode.Scrollable);
        }

        public Image Image
        {
            get { return this.pictureBox.Image; }
            set
            {
                this.pictureBox.Image = value;
                this.SetLayout();
                //this.ChangeSize();
            }
        }

        public SizeMode ImageSizeMode
        {
            get { return this._sizeMode; }
            set
            {
                this._sizeMode = value;
                this.AutoScroll = (_sizeMode == SizeMode.Scrollable);
                this.SetLayout();
            }
        }

        private void RatioStretch()
        {
            float pRatio  = (float)this.Width / this.Height;
            float imRatio = (float)this.pictureBox.Image.Width / this.pictureBox.Image.Height;

            if (this.Width >= this.pictureBox.Image.Width && this.Height >= this.pictureBox.Image.Height)
            {
                this.pictureBox.Width = this.pictureBox.Image.Width;
                this.pictureBox.Height = this.pictureBox.Image.Height;
            }
            else if (this.Width > this.pictureBox.Image.Width && this.Height < this.pictureBox.Image.Height)
            {
                this.pictureBox.Height = this.Height;
                this.pictureBox.Width = (int)(this.Height * imRatio);
            }
            else if (this.Width < this.pictureBox.Image.Width && this.Height > this.pictureBox.Image.Height)
            {
                this.pictureBox.Width = this.Width;
                this.pictureBox.Height = (int)(this.Width / imRatio);
            }
            else if (this.Width < this.pictureBox.Image.Width && this.Height < this.pictureBox.Image.Height)
            {
                if (this.Width >= this.Height)
                {
                    //width image
                    if (this.pictureBox.Image.Width >= this.pictureBox.Image.Height && imRatio >= pRatio)
                    {
                        this.pictureBox.Width = this.Width;
                        this.pictureBox.Height = (int)(this.Width / imRatio);
                    }
                    else
                    {
                        this.pictureBox.Height = this.Height;
                        this.pictureBox.Width = (int)(this.Height * imRatio);
                    }
                }
                else
                {
                    //width image
                    if (this.pictureBox.Image.Width < this.pictureBox.Image.Height && imRatio < pRatio)
                    {
                        this.pictureBox.Height = this.Height;
                        this.pictureBox.Width = (int)(this.Height * imRatio);
                    }
                    else // height image
                    {
                        this.pictureBox.Width = this.Width;
                        this.pictureBox.Height = (int)(this.Width / imRatio);
                    }
                }
            }
            this.CenterImage();
        }

        private void Scrollable()
        {
            this.pictureBox.Width = this.pictureBox.Image.Width;
            this.pictureBox.Height = this.pictureBox.Image.Height;
            this.CenterImage();
        }

        private void SetLayout()
        {
            if (this.pictureBox.Image == null)
                return;
            if (_sizeMode == SizeMode.RatioStretch)
                this.RatioStretch();
            else
            {
                this.AutoScroll = false;
                this.Scrollable();
                this.AutoScroll = true;

            }
        }
        private void CenterImage()
        {
            int top  = (int)((this.Height - this.pictureBox.Height) / 2.0);
            int left = (int)((this.Width - this.pictureBox.Width) / 2.0);
            if (top < 0)
                top = 0;
            if (left < 0)
                left = 0;
            this.pictureBox.Top = top;
            this.pictureBox.Left = left;
        }

        private void OnViewerLoad(object sender, EventArgs e)
        {
            this.pictureBox.Width = 0;
            this.pictureBox.Height = 0;
            this.SetLayout();
        }

        private void OnViewerResize(object sender, EventArgs e)
        {
            this.SetLayout();
        }

    }
}
