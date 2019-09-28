using System;
using System.Windows;

namespace SharpVectors.Runtime
{
    public static class SvgObject
    {
        #region Public Fields

        public const string GZipSignature = "H4sI";

        public const string LinksLayer    = "AnimationLayer";
        public const string DrawLayer     = "DrawingLayer";

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.RegisterAttached("Id", typeof(string), typeof(SvgObject),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty UniqueIdProperty =
            DependencyProperty.RegisterAttached("UniqueId", typeof(string), typeof(SvgObject),
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
            if (!string.IsNullOrEmpty(name) && name.StartsWith("_", StringComparison.Ordinal))
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

        public static void SetUniqueId(DependencyObject element, string value)
        {
            element.SetValue(UniqueIdProperty, value);
        }

        public static string GetUniqueId(DependencyObject element)
        {
            return (string)element.GetValue(UniqueIdProperty);
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

        public static bool IsEqual(double value1, double value2)
        {
            return value1.Equals(value2);
        }

        public static string RemoveWhitespace(string str)
        {
            if (str == null || str.Length == 0)
            {
                return string.Empty;
            }
            var len = str.Length;
            var src = str.ToCharArray();
            int dstIdx = 0;

            for (int i = 0; i < len; i++)
            {
                var ch = src[i];

                switch (ch)
                {
                    case '\u0020':
                    case '\u00A0':
                    case '\u1680':
                    case '\u2000':
                    case '\u2001':
                    case '\u2002':
                    case '\u2003':
                    case '\u2004':
                    case '\u2005':
                    case '\u2006':
                    case '\u2007':
                    case '\u2008':
                    case '\u2009':
                    case '\u200A':
                    case '\u202F':
                    case '\u205F':
                    case '\u3000':
                    case '\u2028':
                    case '\u2029':
                    case '\u0009':
                    case '\u000A':
                    case '\u000B':
                    case '\u000C':
                    case '\u000D':
                    case '\u0085':
                        continue;
                    default:
                        src[dstIdx++] = ch;
                        break;
                }
            }
            return new string(src, 0, dstIdx);
        }

        #endregion
    }
}
