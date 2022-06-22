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

namespace WpfSvgTestBox
{
    /// <summary>
    /// Interaction logic for NumericSpinner.xaml
    /// </summary>
    public partial class NumericSpinner : UserControl
    {
        #region Fields

        public readonly static DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(decimal),
            typeof(NumericSpinner),
            new FrameworkPropertyMetadata(new decimal(0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public readonly static DependencyProperty IncrementProperty = DependencyProperty.Register(
            "Increment",
            typeof(decimal),
            typeof(NumericSpinner),
            new PropertyMetadata(new decimal(0.1)));

        public readonly static DependencyProperty DecimalsProperty = DependencyProperty.Register(
            "Decimals",
            typeof(int),
            typeof(NumericSpinner),
            new PropertyMetadata(2));

        public readonly static DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum",
            typeof(decimal),
            typeof(NumericSpinner),
            new PropertyMetadata(new decimal(int.MinValue)));

        public readonly static DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum",
            typeof(decimal),
            typeof(NumericSpinner),
            new PropertyMetadata(new decimal(int.MaxValue)));

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<decimal>), typeof(NumericSpinner));
        public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
        {
            add {
                base.AddHandler(ValueChangedEvent, value);
            }
            remove {
                base.RemoveHandler(ValueChangedEvent, value);
            }
        }

        public static readonly RoutedEvent DoubleChangedEvent = EventManager.RegisterRoutedEvent(
            "DoubleChanged", RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<double>), typeof(NumericSpinner));
        public event RoutedPropertyChangedEventHandler<double> DoubleChanged
        {
            add {
                base.AddHandler(DoubleChangedEvent, value);
            }
            remove {
                base.RemoveHandler(DoubleChangedEvent, value);
            }
        }

        public static readonly RoutedEvent IntegerChangedEvent = EventManager.RegisterRoutedEvent(
            "IntegerChanged", RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<int>), typeof(NumericSpinner));
        public event RoutedPropertyChangedEventHandler<int> IntegerChanged
        {
            add {
                base.AddHandler(IntegerChangedEvent, value);
            }
            remove {
                base.RemoveHandler(IntegerChangedEvent, value);
            }
        }

        #endregion

        private string oldText = string.Empty;

        public NumericSpinner()
        {
            InitializeComponent();

            spinnerText.SetBinding(TextBox.TextProperty, new Binding("Value")
            {
                ElementName = "ctrlNumericSpinner",
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            spinnerText.Text = this.Value.ToString();
        }

        #region Public Properties

        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set {
                if (value < Minimum)
                    value = Minimum;
                if (value > Maximum)
                    value = Maximum;

                var oldValue = this.Value;

                value = this.CoerceValue(value, false);

                if (oldValue == value)
                {
                    return;
                }

                SetValue(ValueProperty, value);

                var newValue = value;

                this.RaiseEvents(oldValue, newValue);
            }
        }

        public double DoubleValue
        {
            get {
                return Convert.ToDouble(this.Value);
            }
            set {
                this.Value = Convert.ToDecimal(value);
            }
        }

        public int IntegerValue
        {
            get {
                return Convert.ToInt32(this.Value);
            }
            set {
                this.Value = Convert.ToDecimal(value);
            }
        }

        public decimal Increment
        {
            get { return (decimal)GetValue(IncrementProperty); }
            set {
                SetValue(IncrementProperty, value);
            }
        }

        public int Decimals
        {
            get { return (int)GetValue(DecimalsProperty); }
            set {
                SetValue(DecimalsProperty, value);

                this.CoerceValue(this.Value, true);
            }
        }

        public decimal Minimum
        {
            get { return (decimal)GetValue(MinimumProperty); }
            set {
                if (value > Maximum)
                    Maximum = value;
                SetValue(MinimumProperty, value);

                this.CoerceValue(this.Value, true);
            }
        }

        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumProperty); }
            set {
                if (value < Minimum)
                    value = Minimum;
                SetValue(MaximumProperty, value);

                this.CoerceValue(this.Value, true);
            }
        }

        #endregion

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        /// <summary>
        /// Revalidate the object, whenever a value is changed...
        /// </summary>
        private decimal CoerceValue(decimal value, bool applyValue)
        {
            if (Minimum > Maximum) Minimum = Maximum;
            if (Maximum < Minimum) Maximum = Minimum;
            if (value < Minimum) value = Minimum;
            if (value > Maximum) value = Maximum;

            value = decimal.Round(value, Decimals);
            if (applyValue)
            {
                this.Value = value;
            }

            return value;
        }

        private void OnSpinnerUp(object sender, RoutedEventArgs e)
        {
            this.Value += this.Increment;
        }

        private void OnSpinnerDown(object sender, RoutedEventArgs e)
        {
            this.Value -= this.Increment;
        }

        private void OnSpinnerGotFocus(object sender, RoutedEventArgs e)
        {
            oldText = spinnerText.Text.Trim();
        }

        private void OnSpinnerTextChanged(object sender, TextChangedEventArgs e)
        {
            var inputText = spinnerText.Text.Trim();

            if (inputText.Length == 0)
            {
                this.Value = this.Minimum;
            }
            else
            {
                if (decimal.TryParse(inputText, out decimal newValue))
                {
                    newValue = CoerceValue(newValue, false);

                    if (newValue != this.Value)
                    {
                        this.Value = newValue;
                    }
                    else if (string.Equals(oldText, inputText) == false)
                    {
                        decimal oldValue = this.Minimum;
                        if (!string.IsNullOrWhiteSpace(oldText) && decimal.TryParse(oldText, out decimal tryValue))
                        {
                            oldValue = CoerceValue(tryValue, false);
                        }

                        this.RaiseEvents(oldValue, newValue);
                    }
                }
            }
        }

        private void RaiseEvents(decimal oldValue, decimal newValue)
        {
            if (this.IsLoaded == false || oldValue == newValue)
            {
                return;
            }

            this.RaiseEvent(new RoutedPropertyChangedEventArgs<decimal>(oldValue, newValue, ValueChangedEvent));
            this.RaiseEvent(new RoutedPropertyChangedEventArgs<double>(
                Convert.ToDouble(oldValue), Convert.ToDouble(newValue), DoubleChangedEvent));
            this.RaiseEvent(new RoutedPropertyChangedEventArgs<int>(
                Convert.ToInt32(oldValue), Convert.ToInt32(newValue), IntegerChangedEvent));
        }
    }
}
