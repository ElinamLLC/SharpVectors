using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

using SharpVectors.Net;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Events;

namespace TestSvgControl
{
    public partial class MainForm : Form
    {
        private string _titleBase;
        private QuickHelpForm _quickHelpForm;

        public MainForm()
        {
            InitializeComponent();

            _titleBase = this.Text;

            Rectangle rectScreen = Screen.PrimaryScreen.WorkingArea;

            this.Width  = (int)(rectScreen.Width * 0.80f);
            this.Height = (int)(rectScreen.Height * 0.90f);
        }

        private void OnDragDrop(object sender, DragEventArgs de)
        {
            string fileName = string.Empty;
            if (de.Data.GetDataPresent(DataFormats.Text))
            {
                fileName = (string)de.Data.GetData(DataFormats.Text);
            }
            else if (de.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileNames;
                fileNames = (string[])de.Data.GetData(DataFormats.FileDrop);
                fileName = fileNames[0];
            }

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                this.OpenSvgFile(fileName);
            }
        }

        private void OnDragEnter(object sender, DragEventArgs de)
        {
            if (de.Data.GetDataPresent(DataFormats.Text) ||
               de.Data.GetDataPresent(DataFormats.FileDrop))
            {
                de.Effect = DragDropEffects.Copy;
            }
            else
            {
                de.Effect = DragDropEffects.None;
            }
        }

        private static Uri TryGetUri(string svgSource)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(svgSource))
                {
                    return null;
                }
                return new Uri(svgSource);
            }
            catch (UriFormatException)
            {
                return null;
            }
        }

        private async Task OpenSvgSource(string svgSource)
        {
            if (string.IsNullOrWhiteSpace(svgSource))
            {
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                await svgPictureBox.LoadAsync(svgSource);

                // JR Test event
                ISvgDocument doc = svgPictureBox.Window.Document;    
                if (doc != null && doc.RootElement != null)
                {
                    doc.RootElement.AddEventListener("click", new EventListener(OnSvgElementClicked), false);
                }

                if (File.Exists(svgSource))
                {
                    this.Text = _titleBase + " - " + Path.GetFileName(svgSource);
                }
                else
                {
                    var uriSource = TryGetUri(svgSource);
                    if (uriSource != null)
                    {
                        this.Text = _titleBase + " - " + uriSource.AbsoluteUri;
                    }
                    else
                    {
                        this.Text = _titleBase + " - " + svgSource;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void OpenSvgFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
            {
                return;
            }
            string fileExt = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(fileExt))
            {
                return;
            }
            if (!string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                svgPictureBox.Load(fileName);

                // JR Test event
                ISvgDocument doc = svgPictureBox.Window.Document;
                if (doc != null && doc.RootElement != null)
                {
                    doc.RootElement.AddEventListener("click", new EventListener(OnSvgElementClicked), false);
                }

                this.Text = _titleBase + " - " + Path.GetFileName(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        public void OnSvgElementClicked(IEvent e)
        {
            SvgElement el = ((SvgElement)e.Target);

            HitTestForm _hitTestForm = new HitTestForm();

            var title = string.Format("Clicked - LocalName = {0}, ID = {1}", el.LocalName, el.Id);
            _hitTestForm.SetTexts(title, el.OuterXml);

            _hitTestForm.ShowDialog(this);
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            DataSecurityProtocols.Initialize();
        }

        private async void OnFormShown(object sender, EventArgs e)
        {
            using (SvgSourceForm dlg = new SvgSourceForm())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    await this.OpenSvgSource(dlg.SvgSource);
                }
            }
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private async void OnSelectClicked(object sender, EventArgs e)
        {
            using (SvgSourceForm dlg = new SvgSourceForm())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    await this.OpenSvgSource(dlg.SvgSource);
                }
            }
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnSizeModeChanged(object sender, EventArgs e)
        {
            var selectedMenuItem = sender as ToolStripMenuItem;
            if (selectedMenuItem == null)
            {
                return;
            }
            
            PictureBoxSizeMode selectedMode = svgPictureBox.SizeMode;
            if (selectedMenuItem.Checked == false)
            {
                selectedMode = PictureBoxSizeMode.Normal;
                menuItemNormal.Checked = true;
            }
            else
            {
                var menuItems = new ToolStripMenuItem[] {
                    menuItemNormal,
                    menuItemStretchImage,
                    menuItemAutoSize,
                    menuItemCenterImage,
                    menuItemZoom
                };

                var selectedIndex = -1;

                for (int i = 0; i < menuItems.Length; i++)
                {
                    var menuItem = menuItems[i];
                    if (menuItem != selectedMenuItem)
                    {
                        menuItem.Checked = false;
                    }
                    else
                    {
                        selectedIndex = i;
                    }
                }

                if (selectedIndex != -1)
                {
                    selectedMode = (PictureBoxSizeMode)selectedIndex;
                }
            }

            if (selectedMode != svgPictureBox.SizeMode)
            {
                svgPictureBox.SizeMode = selectedMode;
            }

        }

        private void OnQuickHelpClick(object sender, EventArgs e)
        {
            if (_quickHelpForm == null || _quickHelpForm.IsDisposed)
            {
                _quickHelpForm = new QuickHelpForm();
            }

            _quickHelpForm.Owner = this;
            _quickHelpForm.Show();
        }
    }
}
