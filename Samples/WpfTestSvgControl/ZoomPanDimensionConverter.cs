using System;
using System.Globalization;

using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using SharpVectors.Runtime;

namespace WpfTestSvgControl
{
    [MarkupExtensionReturnType(typeof(double))]
    public sealed class ZoomPanDimensionConverter : MarkupExtension, IMultiValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //NOTE: Cannot pass ExtentWidth or ExtentHeight as one of the values because it does not seem to update
            var zoomPanControl = values[3] as ZoomPanControl;
            if (values[0] == null || zoomPanControl == null)
                return DependencyProperty.UnsetValue;
            var size   = (double)values[0];
            var offset = (double)values[1];
            var zoom   = (double)values[2];

            var isWidth = string.Equals(parameter?.ToString(), "width", StringComparison.OrdinalIgnoreCase);
            return Math.Max(isWidth ? Math.Min(zoomPanControl.ExtentWidth / zoom - offset, size)
                 : Math.Min(zoomPanControl.ExtentHeight / zoom - offset, size), 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
