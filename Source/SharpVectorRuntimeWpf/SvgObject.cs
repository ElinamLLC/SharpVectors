using System;
using System.Text;

using System.Windows;

namespace SharpVectors.Runtime
{
    public static class SvgObject
    {
        #region Public Fields

        public const string LinksLayer = "AnimationLayer";
        public const string DrawLayer  = "DrawingLayer";

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.RegisterAttached("Id", typeof(string), typeof(SvgObject),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty ClassProperty =
            DependencyProperty.RegisterAttached("Class", typeof(string), typeof(SvgObject),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.RegisterAttached("Type", typeof(SvgObjectType), typeof(SvgObject),
            new FrameworkPropertyMetadata(SvgObjectType.None, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.RegisterAttached("Title", typeof(String), typeof(SvgObject),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None));

        #endregion

        #region Public Methods

        public static void SetName(DependencyObject element, string name)
        {
            if (!string.IsNullOrEmpty(name) && char.IsDigit(name[0]))
            {
                name = "_" + name;
            }
            element.SetValue(FrameworkElement.NameProperty, name);
        }

        public static string GetName(DependencyObject element)
        {
            string name = element.GetValue(FrameworkElement.NameProperty) as string;
            if (!string.IsNullOrEmpty(name) && name.StartsWith("_"))
            {
                name = name.Remove(0, 1);
            }
            return name;
        }

        public static void SetId(DependencyObject element, string value)
        {
            element.SetValue(IdProperty, value);
        }

        public static string GetId(DependencyObject element)
        {
            return (string)element.GetValue(IdProperty);
        }

        public static void SetClass(DependencyObject element, string value)
        {
            element.SetValue(ClassProperty, value);
        }

        public static string GetClass(DependencyObject element)
        {
            return (string)element.GetValue(ClassProperty);
        }

        public static void SetType(DependencyObject element, SvgObjectType value)
        {
            element.SetValue(TypeProperty, value);
        }

        public static SvgObjectType GetType(DependencyObject element)
        {
            return (SvgObjectType)element.GetValue(TypeProperty);
        }

        public static void SetTitle(DependencyObject element, string value)
        {
            element.SetValue(TitleProperty, value);
        }

        public static string GetTitle(DependencyObject element)
        {
            return (string)element.GetValue(TitleProperty);
        }

        public static bool IsValid(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return false;
            }
            return true;
        }

        public static bool IsValid(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                return false;
            }
            return true;
        }
             
        #endregion
    }
}
