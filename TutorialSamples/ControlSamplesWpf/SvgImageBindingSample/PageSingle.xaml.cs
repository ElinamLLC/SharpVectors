using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace SvgImageBindingSample
{
    /// <summary>
    /// Interaction logic for PageSingle.xaml
    /// </summary>
    public partial class PageSingle : Page
    {
        private const string SvgFileName = "Asian_Openbill.svg";

        public PageSingle()
        {
            InitializeComponent();

            this.Loaded      += OnPageLoaded;
            this.SizeChanged += OnPageSizeChanged;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            string workingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string svgFilePath = Path.Combine(workingDir, SvgFileName);
            if (File.Exists(svgFilePath))
            {
                inputBox.Text = svgFilePath;
            }

            RowDefinition rowTab = rightGrid.RowDefinitions[2];
            rowTab.Height = new GridLength((this.ActualHeight - 8) / 2, GridUnitType.Pixel);
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RowDefinition rowTab = rightGrid.RowDefinitions[2];
            rowTab.Height = new GridLength((this.ActualHeight - 8) / 2, GridUnitType.Pixel);
        }
    }
}
