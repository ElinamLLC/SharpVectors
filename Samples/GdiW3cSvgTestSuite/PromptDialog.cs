using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.NetworkInformation;
#if DOTNET40
using Ionic.Zip;
#else
using System.Net.Http;
#endif

using FolderBrowserDialog = ShellFileDialogs.FolderBrowserDialog;

namespace GdiW3cSvgTestSuite
{
    public partial class PromptDialog : Form
    {
        #region Private Fields 

        /// <summary>
        /// Default font used for the generic 'serif' family
        /// </summary>
        private const string PanelDefaultFont = "Segoe UI";

        private string _downloadeFilePath;
        private OptionSettings _optionSettings;

        #endregion

        #region Constructors and Destructor

        public PromptDialog()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.Dpi;

            this.Font = new Font(PanelDefaultFont, 14F, FontStyle.Regular, GraphicsUnit.World);

            labelLocal.Font = new Font(labelLocal.Font, FontStyle.Bold);
            labelWeb.Font   = new Font(labelWeb.Font, FontStyle.Bold);

            labelTitle.Font = new Font(labelTitle.Font.FontFamily, labelTitle.Font.Size + 2, FontStyle.Regular, labelTitle.Font.Unit);
        }

        #endregion

        #region Public Properties

        public OptionSettings OptionSettings
        {
            get {
                return _optionSettings;
            }
            set {
                if (value != null)
                {
                    _optionSettings = value;
                }
            }
        }

        #endregion

        #region Private Methods and Event Handlers 

        private void OnFormLoad(object sender, EventArgs e)
        {
            if (_optionSettings != null)
            {
                txtSvgSuitePath.Text    = _optionSettings.LocalSuitePath;
                txtSvgSuitePathWeb.Text = _optionSettings.WebSuitePath;

                btnOK.Enabled       = OptionSettings.IsTestSuiteAvailable(_optionSettings.LocalSuitePath);
                btnDownload.Enabled = NetworkInterface.GetIsNetworkAvailable();
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_downloadeFilePath) &&
                File.Exists(_downloadeFilePath))
            {
                File.Delete(_downloadeFilePath);
            }
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            btnCancel.Focus();
        }

#if DOTNET40
        private void OnDownloadClicked(object sender, EventArgs e)
        {
            var dlg = new LoadingAdorner();
            dlg.Owner = this;
            dlg.StartPosition = FormStartPosition.Manual;
            dlg.Location = new Point(this.Location.X + (this.Width - dlg.Width) / 2, 
                this.Location.Y + (this.Height - dlg.Height) / 2);
            dlg.Show(this);

            string url = _optionSettings.WebSuitePath;

            _downloadeFilePath = Path.Combine(_optionSettings.LocalSuitePath, "FullTestSuite.zip");
            if (File.Exists(_downloadeFilePath))
            {
                File.Delete(_downloadeFilePath);
            }

            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)768; //TLS 1.1
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            using (WebClient client = new WebClient())
            {
                client.DownloadFileCompleted += delegate(object other, AsyncCompletedEventArgs args) {
                    bool result = !args.Cancelled;
                    if (!result)
                    {
                        return;
                    }
                    using (ZipFile zip = ZipFile.Read(_downloadeFilePath))
                    {
                        zip.ExtractAll(_optionSettings.LocalSuitePath);
                    }

                    dlg.Close();

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };
                client.DownloadFileAsync(new Uri(url), _downloadeFilePath);
            }
        }
#else
        private async void OnDownloadClicked(object sender, EventArgs e)
        {
            var dlg = new LoadingAdorner();
            dlg.Owner = this;
            dlg.StartPosition = FormStartPosition.Manual;
            dlg.Location = new Point(this.Location.X + (this.Width - dlg.Width) / 2, 
                this.Location.Y + (this.Height - dlg.Height) / 2);
            dlg.Show(this);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            string url = _optionSettings.WebSuitePath;

            _downloadeFilePath = Path.Combine(_optionSettings.LocalSuitePath, "FullTestSuite.zip");
            if (File.Exists(_downloadeFilePath))
            {
                File.Delete(_downloadeFilePath);
            }

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                {
                    using (Stream streamToWriteTo = File.Open(_downloadeFilePath, FileMode.Create))
                    {
                        await streamToReadFrom.CopyToAsync(streamToWriteTo);
                    }

                    ZipFile.ExtractToDirectory(_downloadeFilePath, _optionSettings.LocalSuitePath);

                    dlg.Close();

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
#endif

        private void OnSvgSuitePathTextChanged(object sender, EventArgs e)
        {
            UpdateStates();
        }

        private void OnBrowseForSvgSuitePath(object sender, EventArgs e)
        {
            string selectedDirectory = FolderBrowserDialog.ShowDialog(IntPtr.Zero,
                "Select the location of the W3C SVG 1.1 Full Test Suite", null);
            if (!string.IsNullOrWhiteSpace(selectedDirectory))
            {
                txtSvgSuitePath.Text = selectedDirectory;

                UpdateStates();
            }
        }

        private void OnOpenSvgSuitePath(object sender, EventArgs e)
        {
            var filePath = txtSvgSuitePath.Text;
            if (string.IsNullOrWhiteSpace(filePath) || Directory.Exists(filePath) == false)
            {
                return;
            }

            OptionSettings.OpenFolderAndSelectItem(filePath, null);
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OnOKClicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void UpdateStates()
        {
            string selectePath = txtSvgSuitePath.Text;
            if (selectePath != null)
            {
                selectePath = selectePath.Trim();
            }
            if (string.IsNullOrWhiteSpace(selectePath) || !Directory.Exists(selectePath))
            {
                btnOK.Enabled = false;
                btnDownload.Enabled = false;
                btnPathLocate.Enabled = false;

                return;
            }

            _optionSettings.LocalSuitePath = selectePath;

            btnPathLocate.Enabled = true;
            btnDownload.Enabled = NetworkInterface.GetIsNetworkAvailable();

            btnOK.Enabled = OptionSettings.IsTestSuiteAvailable(_optionSettings.LocalSuitePath);
        }

        #endregion
    }
}
