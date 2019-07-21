using System;
using System.Windows;
using System.Windows.Documents;

namespace ZoomPanControlSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScrollableZoomPanPage _scrollablePage;
        private InfiniteZoomPanPage _infinitePage;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            helpViewer.Document = Application.LoadComponent(
                new Uri("QuickHelpPage.xaml", UriKind.Relative)) as FlowDocument;

            _scrollablePage = frameScrollable.Content as ScrollableZoomPanPage;
            _infinitePage   = frameInfinite.Content as InfiniteZoomPanPage;

            if (_scrollablePage != null)
            {
                _scrollablePage.MainWindow = this;
            }
            if (_infinitePage != null)
            {
                _infinitePage.MainWindow = this;
            }
        }
    }
}
