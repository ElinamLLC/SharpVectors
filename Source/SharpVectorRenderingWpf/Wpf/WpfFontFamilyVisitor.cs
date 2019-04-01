using System;

namespace SharpVectors.Renderers.Wpf
{
    public abstract class WpfFontFamilyVisitor : WpfVisitor
    {
        protected WpfFontFamilyVisitor()
        {   
        }

        public abstract WpfFontFamilyInfo Visit(string fontName, WpfFontFamilyInfo familyInfo,
            WpfDrawingContext context);
    }
}
