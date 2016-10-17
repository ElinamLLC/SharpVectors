using System.Windows;

namespace SharpVectors.Renderers.Wpf
{
    public abstract class WpfFontFamilyVisitor : DependencyObject
    {
        protected WpfFontFamilyVisitor()
        {   
        }

        public abstract WpfFontFamilyInfo Visit(string fontName, WpfFontFamilyInfo familyInfo,
            WpfDrawingContext context);
    }
}
