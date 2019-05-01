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
    /// </summary>
    public class SvgControl : Control
    {
        static SvgControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SvgControl), 
                new FrameworkPropertyMetadata(typeof(SvgControl)));
        }
    }
}
