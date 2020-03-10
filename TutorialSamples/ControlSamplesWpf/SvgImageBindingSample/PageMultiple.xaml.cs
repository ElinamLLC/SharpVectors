using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace SvgImageBindingSample
{
    public class IconData
    {
        private Uri _uri;
        private string _title;

        public IconData()
        {
        }

        public IconData(FileInfo source)
        {
            if (source != null)
            {
                _title = Path.GetFileNameWithoutExtension(source.Name);
                _uri = new Uri(source.FullName);
            }
        }

        public string ImageTitle
        {
            get { return this._title; }
            set { this._title = value; }
        }

        public Uri ImageUri
        {
            get { return this._uri; }
            set { this._uri = value; }
        }
    }

    /// <summary>
    /// Interaction logic for PageMultiple.xaml
    /// </summary>
    public partial class PageMultiple : Page
    {
        public const string IconZipFile = @"..\svg-icons.zip";
        public const string IconFolder  = @"..\Svg-Icons";

        public PageMultiple()
        {
            InitializeComponent();

            this.Loaded += OnPageLoaded;
            this.SizeChanged += OnPageSizeChanged;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            string workingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string iconsPath = Path.Combine(workingDir, PageMultiple.IconZipFile);
            if (!File.Exists(iconsPath))
            {
                return;
            }

            var iconsDir = new DirectoryInfo(Path.Combine(workingDir, PageMultiple.IconFolder));
            if (!iconsDir.Exists)
            {
                return;
            }

            FileInfo[] iconFiles = iconsDir.GetFiles("*.svg", SearchOption.TopDirectoryOnly);
            if (iconFiles == null || iconFiles.Length == 0)
            {
                return;
            }

            List<IconData> sourceData = new List<IconData>(iconFiles.Length);
            foreach (var iconFile in iconFiles)
            {
                sourceData.Add(new IconData(iconFile));
            }
            sourceData.Add(new IconData()); //Test-Empty

            this.DataContext = sourceData;
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
        }
    }
}
