using System;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Collections.Generic;

namespace SharpVectors.Runtime
{
    public sealed class SvgFontUriExtension : MarkupExtension
    {
        private string _inputUri;

        public SvgFontUriExtension(string inputUri)
        {
            _inputUri = Environment.ExpandEnvironmentVariables(inputUri);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new Uri(_inputUri);
        }
    }
}
