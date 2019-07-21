using System;
using System.Windows;
using System.Windows.Data;

namespace ZoomPanControlSample
{
    /// <summary>
    /// Interaction logic for ZoomPanOverviewWindow.xaml
    /// </summary>
    public partial class ZoomPanOverviewWindow : Window
    {
        public ZoomPanOverviewWindow()
        {
            InitializeComponent();

            this.Loaded += OnWindowLoaded;
        }

        public ZoomPanPageMode PageMode
        {
            get {
                if (zoomOverview != null)
                {
                    return zoomOverview.PageMode;
                }
                return ZoomPanPageMode.None;
            }
            set {
                if (zoomOverview != null)
                {
                    zoomOverview.PageMode = value;
                }
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // This assumes the target page is stored in the DataContext, 
            // before the page is displayed...
            if (this.DataContext == null || zoomOverview == null)
            {
                return;
            }

            // Pass the required information to the overview control through binding...
            // 1. The target page, so that the ZoomPanControl install can be accessed...
            var pageBinding = new Binding();
            pageBinding.Source = this.DataContext;
            zoomOverview.SetBinding(DataContextProperty, pageBinding);

            // 2. The target page's viewer property, so that the SvgDrawingCanvas instance can be accessed...
            var drawingBinding = new Binding("Viewer");
            drawingBinding.Source = this.DataContext;
            zoomOverview.SetBinding(ZoomPanOverview.VisualProperty, drawingBinding);
        }
    }
}
