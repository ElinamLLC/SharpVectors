// TextOnPathBase.cs by Charles Petzold, September 2008
using System;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SharpVectors.Renderers.Texts
{
    public abstract class WpfTextOnPathBase : DependencyObject
    {
        // Dependency properties
        public static readonly DependencyProperty FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner(typeof(WpfTextOnPathBase),
                new FrameworkPropertyMetadata(OnFontPropertyChanged));

        public static readonly DependencyProperty FontStyleProperty =
            TextElement.FontStyleProperty.AddOwner(typeof(WpfTextOnPathBase),
                new FrameworkPropertyMetadata(OnFontPropertyChanged));

        public static readonly DependencyProperty FontWeightProperty =
            TextElement.FontWeightProperty.AddOwner(typeof(WpfTextOnPathBase),
                new FrameworkPropertyMetadata(OnFontPropertyChanged));

        public static readonly DependencyProperty FontStretchProperty =
            TextElement.FontStretchProperty.AddOwner(typeof(WpfTextOnPathBase),
                new FrameworkPropertyMetadata(OnFontPropertyChanged));

        public static readonly DependencyProperty ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner(typeof(WpfTextOnPathBase),
                new FrameworkPropertyMetadata(OnForegroundPropertyChanged));

        //public static readonly DependencyProperty TextProperty =
        //    TextBlock.TextProperty.AddOwner(typeof(TextOnPathBase),
        //        new FrameworkPropertyMetadata(OnTextPropertyChanged));

        //public static readonly DependencyProperty PathFigureProperty =
        //    DependencyProperty.Register("PathFigure",
        //        typeof(PathFigure),
        //        typeof(TextOnPathBase),
        //        new FrameworkPropertyMetadata(OnPathPropertyChanged));

        // Properties
        public FontFamily FontFamily
        {
            set { SetValue(FontFamilyProperty, value); }
            get { return (FontFamily)GetValue(FontFamilyProperty); }
        }

        public FontStyle FontStyle
        {
            set { SetValue(FontStyleProperty, value); }
            get { return (FontStyle)GetValue(FontStyleProperty); }
        }

        public FontWeight FontWeight
        {
            set { SetValue(FontWeightProperty, value); }
            get { return (FontWeight)GetValue(FontWeightProperty); }
        }

        public FontStretch FontStretch
        {
            set { SetValue(FontStretchProperty, value); }
            get { return (FontStretch)GetValue(FontStretchProperty); }
        }

        public Brush Foreground
        {
            set { SetValue(ForegroundProperty, value); }
            get { return (Brush)GetValue(ForegroundProperty); }
        }

        //public string Text
        //{
        //    set { SetValue(TextProperty, value); }
        //    get { return (string)GetValue(TextProperty); }
        //}

        //public PathFigure PathFigure
        //{
        //    set { SetValue(PathFigureProperty, value); }
        //    get { return (PathFigure)GetValue(PathFigureProperty); }
        //}

        // Property changed handlers
        static void OnFontPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as WpfTextOnPathBase).OnFontPropertyChanged(args);
        }

        protected abstract void OnFontPropertyChanged(DependencyPropertyChangedEventArgs args);

        static void OnForegroundPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as WpfTextOnPathBase).OnForegroundPropertyChanged(args);
        }

        protected abstract void OnForegroundPropertyChanged(DependencyPropertyChangedEventArgs args);

        //static void OnTextPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        //{
        //    (obj as TextOnPathBase).OnTextPropertyChanged(args);
        //}

        //protected abstract void OnTextPropertyChanged(DependencyPropertyChangedEventArgs args);

        //static void OnPathPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        //{
        //    (obj as TextOnPathBase).OnPathPropertyChanged(args);
        //}

        //protected abstract void OnPathPropertyChanged(DependencyPropertyChangedEventArgs args);

        // Utility method
        public static double GetPathFigureLength(PathFigure pathFigure)
        {
            if (pathFigure == null)
                return 0;

            bool isAlreadyFlattened = true;

            foreach (PathSegment pathSegment in pathFigure.Segments)
            {
                if (!(pathSegment is PolyLineSegment) && !(pathSegment is LineSegment))
                {
                    isAlreadyFlattened = false;
                    break;
                }
            }

            PathFigure pathFigureFlattened = isAlreadyFlattened ? 
                pathFigure : pathFigure.GetFlattenedPathFigure();

            double length = 0;
            Point pt1     = pathFigureFlattened.StartPoint;

            foreach (PathSegment pathSegment in pathFigureFlattened.Segments)
            {
                if (pathSegment is LineSegment)
                {
                    Point pt2 = (pathSegment as LineSegment).Point;
                    length += (pt2 - pt1).Length;
                    pt1 = pt2;
                }
                else if (pathSegment is PolyLineSegment)
                {
                    PointCollection pointCollection = (pathSegment as PolyLineSegment).Points;
                    foreach (Point pt2 in pointCollection)
                    {
                        length += (pt2 - pt1).Length;
                        pt1 = pt2;
                    }
                }
            }

            return length;
        }
    }
}
