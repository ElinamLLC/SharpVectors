using System;
using System.Text;
using System.Collections.Generic;

using System.Windows;

namespace SharpVectors.Runtime
{
    public enum SvgLinkAction
    {
        LinkNone    = 0,
        LinkPage    = 1,
        LinkHtml    = 2,
        LinkTooltip = 3
    }

    public sealed class SvgLink : DependencyObject
    {
        #region Public Fields

        public static readonly DependencyProperty ColorProperty = 
            DependencyProperty.RegisterAttached("Color", typeof(String), typeof(SvgLink), 
            new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty PartsIdProperty =
            DependencyProperty.RegisterAttached("PartsId", typeof(String), typeof(SvgLink),
            new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.RegisterAttached("Type", typeof(String), typeof(SvgLink),
            new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.RegisterAttached("Number", typeof(String), typeof(SvgLink),
            new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty PinProperty =
            DependencyProperty.RegisterAttached("Pin", typeof(String), typeof(SvgLink),
            new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty LineIdProperty =
            DependencyProperty.RegisterAttached("LineId", typeof(String), typeof(SvgLink),
            new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.None));
        
        //----------------------------------------------------------------------------------------------------

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.RegisterAttached("Key", typeof(String), typeof(SvgLink),
            new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.RegisterAttached("Location", typeof(String), typeof(SvgLink),
            new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.RegisterAttached("Action", typeof(SvgLinkAction), typeof(SvgLink),
            new FrameworkPropertyMetadata(SvgLinkAction.LinkNone, FrameworkPropertyMetadataOptions.None));

        #endregion

        #region Public Methods

        public static void SetColor(DependencyObject element, string value)
        {
            element.SetValue(ColorProperty, value);
        }

        public static string GetColor(DependencyObject element)
        {
            return (string)element.GetValue(ColorProperty);
        }

        public static void SetPartsId(DependencyObject element, string value)
        {
            element.SetValue(PartsIdProperty, value);
        }

        public static string GetPartsId(DependencyObject element)
        {
            return (string)element.GetValue(PartsIdProperty);
        }

        public static void SetType(DependencyObject element, string value)
        {
            element.SetValue(TypeProperty, value);
        }

        public static string GetType(DependencyObject element)
        {
            return (string)element.GetValue(TypeProperty);
        }

        public static void SetNumber(DependencyObject element, string value)
        {
            element.SetValue(NumberProperty, value);
        }

        public static string GetNumber(DependencyObject element)
        {
            return (string)element.GetValue(NumberProperty);
        }

        public static void SetPin(DependencyObject element, string value)
        {
            element.SetValue(PinProperty, value);
        }

        public static string GetPin(DependencyObject element)
        {
            return (string)element.GetValue(PinProperty);
        }

        public static void SetLineId(DependencyObject element, string value)
        {
            element.SetValue(LineIdProperty, value);
        }

        public static string GetLineId(DependencyObject element)
        {
            return (string)element.GetValue(LineIdProperty);
        }

        //----------------------------------------------------------------------------------------------------

        public static void SetKey(DependencyObject element, string value)
        {
            element.SetValue(KeyProperty, value);
        }

        public static string GetKey(DependencyObject element)
        {
            return (string)element.GetValue(KeyProperty);
        }

        public static void SetLocation(DependencyObject element, string value)
        {
            element.SetValue(LocationProperty, value);
        }

        public static string GetLocation(DependencyObject element)
        {
            return (string)element.GetValue(LocationProperty);
        }

        public static void SetAction(DependencyObject element, SvgLinkAction value)
        {
            element.SetValue(ActionProperty, value);
        }

        public static SvgLinkAction GetAction(DependencyObject element)
        {
            return (SvgLinkAction)element.GetValue(ActionProperty);
        }

        #endregion
    }
}
