using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfSvgTestBox
{
    public sealed class SvgResourceColor : INotifyPropertyChanged
    {
        private string _name;
        private Color _color;
        private Color _selectedColor;

        public SvgResourceColor()
        {
            _name          = string.Empty;
            _color         = Colors.Transparent;
            _selectedColor = Colors.Transparent;
        }

        public SvgResourceColor(string name, Color color)
        {
            _name          = name;
            _color         = color;
            _selectedColor = Colors.Transparent;
        }

        public bool IsModified
        {
            get {
                return _selectedColor != Colors.Transparent;
            }
        }

        public Color OriginalColor
        {
            get { return _color; }
            set { _color = value; }
        }

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set { 
                _selectedColor = value;
                OnPropertyChanged("SelectedBrush");
            }
        }

        public SolidColorBrush OriginalBrush
        {
            get {
                return new SolidColorBrush(_color);
            }
        }

        public SolidColorBrush SelectedBrush
        {
            get {
                return new SolidColorBrush(_selectedColor);
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// Interaction logic for SvgResourceColors.xaml
    /// </summary>
    public partial class SvgResourceColors : Window
    {
        private enum UpdateValueType
        {
            None,
            Rgb,
            Hsv,
            Hex,
            Alpha,
            All
        }

        private bool _isUpdating;
        private Color _brdrOrgColor;

        private ObservableCollection<SvgResourceColor> _resourceColors;

        #region Constructors
        public SvgResourceColors()
            : this(new List<SvgResourceColor>())
        {

        }

        public SvgResourceColors(IList<SvgResourceColor> resourceList)
        {
            _isUpdating = false;
            _resourceColors = new ObservableCollection<SvgResourceColor>(resourceList);

            InitializeComponent();

            this.OriginalColor = Colors.Transparent;

            resourceColors.ItemsSource = _resourceColors;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the color selected in this dialog.
        /// </summary>
        public Color SelectedColor { get; private set; } = Colors.Transparent;
        /// <summary>
        /// Gets the dialog result of this dialog, based upon whether the user accepted the changes.
        /// </summary>
        public new bool DialogResult { get; private set; } = false;

        public Color OriginalColor
        {
            get {
                return _brdrOrgColor;
            }
            set {
                _brdrOrgColor = value;

                brdrOrgColor.Background = new SolidColorBrush(value);
                btnChange.IsEnabled = (this.OriginalColor != this.SelectedColor);
            }
        }

        public IList<SvgResourceColor> ResourceColors
        {
            get {
                return _resourceColors;
            }
            set {
                if (value == null)
                {
                    value = new List<SvgResourceColor>();
                }
                _resourceColors = new ObservableCollection<SvgResourceColor>(value);
                if (resourceColors != null)
                {
                    resourceColors.ItemsSource = _resourceColors;
                }
            }
        }

        #endregion

        private void UpdateSelectedColor(Color color, bool includeSliders = true)
        {
            this.SelectedColor = color;
            brdrSelColor.Background = new SolidColorBrush(color);

            if (_brdrOrgColor == Colors.Transparent)
            {
                this.OriginalColor = color;
            }

            if (includeSliders)
            {
                UpdateValues(color, UpdateValueType.None);
            }

            btnChange.IsEnabled = (this.OriginalColor != this.SelectedColor);
        }

        #region Private Event Handlers

        private void OnColorClicked(object sender, RoutedEventArgs e)
        {
            UpdateSelectedColor(((sender as Button).Background as System.Windows.Media.SolidColorBrush).Color);
        }

        private void OnRValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isUpdating)
            {
                nudR.Value = (int)e.NewValue;
            }
        }

        private void OnGValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isUpdating)
            {
                nudG.Value = (int)e.NewValue;
            }
        }

        private void OnBValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isUpdating)
            {
                nudB.Value = (int)e.NewValue;
            }
        }

        private void OnHexTextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded == false)
            {
                return;
            }
            try
            {
                Color col = (Color)ColorConverter.ConvertFromString(txtHex.Text);
                lblQ.Visibility = Visibility.Hidden;

                UpdateValues(col, UpdateValueType.Hex);
            }
            catch (Exception)
            {
                lblQ.Visibility = Visibility.Visible;
            }
        }

        private void OnAlphaChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            if (this.IsLoaded == false || _isUpdating)
            {
                return;
            }

            Color col = Color.FromArgb((byte)nudAlpha.IntegerValue, (byte)nudR.IntegerValue, (byte)nudG.IntegerValue, (byte)nudB.IntegerValue);
            UpdateValues(col, UpdateValueType.Alpha);
        }

        private void OnRgbChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            if (this.IsLoaded == false || _isUpdating)
            {
                return;
            }

            Color col = Color.FromArgb((byte)nudAlpha.IntegerValue, (byte)nudR.IntegerValue, (byte)nudG.IntegerValue, (byte)nudB.IntegerValue);
            UpdateValues(col, UpdateValueType.Rgb);
        }

        private void OnChangeClicked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded == false || resourceColors.SelectedIndex < 0)
            {
                return;
            }

            int selectedIndex = resourceColors.SelectedIndex;
            var selectedItem = _resourceColors[selectedIndex];

            selectedItem.SelectedColor = this.SelectedColor;
        }

        private void OnApplyClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion

        #region Color Conversion Functions

        private void UpdateValues(Color color, UpdateValueType except)
        {
            if (except == UpdateValueType.All)
            {
                return;
            }

            _isUpdating = true;

            if (except != UpdateValueType.Rgb)
            {
                nudR.Value = color.R;
                nudG.Value = color.G;
                nudB.Value = color.B;
            }

            if (except != UpdateValueType.Hex)
            {
                txtHex.Text = color.ToString();
            }

            if (except != UpdateValueType.Alpha)
            {
                nudAlpha.IntegerValue = color.A;
            }

            UpdateSelectedColor(color, false);

            _isUpdating = false;
        }

        #endregion

        private void OnResourceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded == false || resourceColors.SelectedIndex < 0)
            {
                return;
            }

            int selectedIndex = resourceColors.SelectedIndex;
            var selectedItem = _resourceColors[selectedIndex];

            this.OriginalColor = selectedItem.OriginalColor;

            if (selectedItem.SelectedColor == Colors.Transparent)
            {
                this.UpdateSelectedColor(selectedItem.OriginalColor, true);
            }
            else
            {
                this.UpdateSelectedColor(selectedItem.SelectedColor, true);
            }
        }
    }

}
