using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpVectors.Converters
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();

            // Reset the dimensions...
            this.Width  = Double.NaN;
            this.Height = Double.NaN;

            this.Loaded += new RoutedEventHandler(OnStartPageLoaded);
        }

        private void OnStartPageLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
