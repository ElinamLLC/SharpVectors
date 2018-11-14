using System;
using System.IO;
using System.Reflection;
using System.IO.Compression;
using System.Windows;

namespace SvgImageBindingSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // ICons credit: https://github.com/icons8/flat-color-icons
            string workingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string iconsPath = Path.Combine(workingDir, PageMultiple.IconZipFile);
            if (!File.Exists(iconsPath))
            {
                return;
            }

            var iconsDir = new DirectoryInfo(Path.Combine(workingDir, PageMultiple.IconFolder));
            if (!iconsDir.Exists)
            {
                iconsDir.Create();

                ZipFile.ExtractToDirectory(iconsPath, workingDir);
            }
        }
    }

}
