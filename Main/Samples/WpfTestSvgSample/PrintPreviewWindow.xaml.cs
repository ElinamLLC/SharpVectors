using System;
using System.IO;
using System.IO.Packaging;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace WpfTestSvgSample
{
    /// <summary>
    /// Interaction logic for PrintPreviewWindow.xaml
    /// </summary>
    public partial class PrintPreviewWindow : Window
    {
        #region Private Fields

        private string      _fileName;
        private Package     _xpsDocPackage;
        private XpsDocument _xpsDocument;

        #endregion

        #region Constructors and Destructor

        public PrintPreviewWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(OnWindowLoaded);
            this.Unloaded += new RoutedEventHandler(OnWindowUnloaded);

            this.Closed += new EventHandler(OnWindowClosed);
            this.Closing += new CancelEventHandler(OnWindowClosing);
        }

        #endregion

        #region Public Methods

        public void LoadDocument(XpsDocument document, Package package, string sourceFileName)
        {
            if (document == null)
            {
                return;
            }

            try
            {
                if (_xpsDocument != null)
                {
                    _xpsDocument.Close();
                    _xpsDocument = null;
                }

                if (_xpsDocPackage != null)
                {
                    _xpsDocPackage.Close();
                    _xpsDocPackage = null;
                }

                if (!String.IsNullOrEmpty(_fileName))
                {
                    PackageStore.RemovePackage(new Uri(_fileName));
                }
            }
            catch
            {
            }

            _xpsDocument   = document;
            _xpsDocPackage = package;
            _fileName      = sourceFileName;

            docViewer.Document = _xpsDocument.GetFixedDocumentSequence();
        }

        #endregion

        #region Private Methods

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            ContentControl findToolbar = docViewer.Template.FindName(
                "PART_FindToolBarHost", docViewer) as ContentControl;
            if (findToolbar != null)
            {
                findToolbar.Visibility = Visibility.Collapsed;
            }
        }

        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                docViewer.Document = null;

                if (_xpsDocument != null)
                {
                    _xpsDocument.Close();
                    _xpsDocument = null;
                }

                if (_xpsDocPackage != null)
                {
                    _xpsDocPackage.Close();
                    _xpsDocPackage = null;
                }

                if (!String.IsNullOrEmpty(_fileName))
                {
                    PackageStore.RemovePackage(new Uri(_fileName));
                }
            }
            catch
            {
            }
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            try
            {
                docViewer.Document = null;

                if (_xpsDocument != null)
                {
                    _xpsDocument.Close();
                    _xpsDocument = null;
                }

                if (_xpsDocPackage != null)
                {
                    _xpsDocPackage.Close();
                    _xpsDocPackage = null;
                }

                if (!String.IsNullOrEmpty(_fileName))
                {
                    PackageStore.RemovePackage(new Uri(_fileName));
                }
            }
            catch
            {
            }
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
        }

        #endregion
    }
}
