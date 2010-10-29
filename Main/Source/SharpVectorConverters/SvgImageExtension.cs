using System;
using System.Resources;

using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;

namespace SharpVectors.Converters
{
    [MarkupExtensionReturnType(typeof(DrawingImage))]
    public sealed class SvgImageExtension : MarkupExtension
    {
        public override object ProvideValue(
            IServiceProvider serviceProvider)
        {
            return null;
        }
    }
}
