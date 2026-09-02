using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfTestResourceSvg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public class KnownColor
        {
            public Color Color { get; set; }
            public string Name { get; set; }
        }

        private Brush _fillBrush;

        private IList<KnownColor> _knownColors;

        private string _bitmapIcon1;
        private string _bitmapIcon2;
        private string _bitmapIcon3;
        private string _bitmapIcon4;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            _fillBrush = Brushes.Green;
            _knownColors = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(i => new KnownColor()
                {
                    Color = (Color)i.GetValue(null, null),
                    Name = i.Name
                }).ToList();

            // Initialize bitmap icon paths for direct binding demo
            _bitmapIcon1 = "/Resources/autum_leaf.svg";
            _bitmapIcon2 = "/Resources/basket.svg";
            _bitmapIcon3 = "/Resources/kite.svg";
            _bitmapIcon4 = "/Resources/beach_ball.svg";

            this.DataContext = this;
        }

        public Brush FillBrush
        {
            get {
                return _fillBrush;
            }
        }

        public IList<KnownColor> KnownColors
        {
            get {
                return _knownColors;
            }
        }

        public string BitmapIcon1
        {
            get { return _bitmapIcon1; }
            set { SetProperty(ref _bitmapIcon1, value, nameof(BitmapIcon1)); }
        }

        public string BitmapIcon2
        {
            get { return _bitmapIcon2; }
            set { SetProperty(ref _bitmapIcon2, value, nameof(BitmapIcon2)); }
        }

        public string BitmapIcon3
        {
            get { return _bitmapIcon3; }
            set { SetProperty(ref _bitmapIcon3, value, nameof(BitmapIcon3)); }
        }

        public string BitmapIcon4
        {
            get { return _bitmapIcon4; }
            set { SetProperty(ref _bitmapIcon4, value, nameof(BitmapIcon4)); }
        }

        private void OnColorChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboColors.SelectedIndex < 0)
            {
                return;
            }

            _fillBrush = new SolidColorBrush(_knownColors[cboColors.SelectedIndex].Color);
            this.NotifyPropertyChanged("FillBrush");
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetProperty<T>(ref T field, T value, string propertyName)
        {
            if (!Equals(field, value))
            {
                field = value;
                NotifyPropertyChanged(propertyName);
            }
        }
    }
}
