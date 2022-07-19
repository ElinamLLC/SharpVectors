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
    }
}
