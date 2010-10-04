using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Gdi;
using SharpVectors.Net;
using SharpVectors.Dom.Events;

namespace TestSvgControl
{
    public partial class MainForm : Form
    {
        private string _titleBase;

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
            string fileName = "";
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

            if (!String.IsNullOrEmpty(fileName))
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

        private void OpenSvgFile(string fileName)
        {
            if (String.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return;
            }
            string fileExt = Path.GetExtension(fileName);
            if (String.IsNullOrEmpty(fileExt))
            {
                return;
            }
            if (!String.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase) &&
                !String.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                svgPictureBox.SourceURL = fileName;

                // JR Test event
                ISvgDocument doc = svgPictureBox.Window.Document;                
                doc.RootElement.AddEventListener("click", new EventListener(OnSvgElementClicked), false);

                this.Text = _titleBase + " - " + Path.GetFileName(fileName);
            }
            catch (System.Exception ex)
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
            
            //MessageBox.Show(String.Format("Clicked - LocalName = {0}, ID = {1}", el.LocalName, el.Id));

            MessageBox.Show(el.OuterXml);
        }
    }
}
