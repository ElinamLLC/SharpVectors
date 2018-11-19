using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public static class SvgElementFactory
    {                    
        public static XmlElement Create(string prefix, string localName, string ns, SvgDocument doc)
        {
            // This factory assumes the requested element is defined in the 
            // http://www.w3.org/2000/svg namespace.
            if (string.IsNullOrWhiteSpace(ns) || !string.Equals(ns, 
                SvgDocument.SvgNamespace, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            switch (localName) 
            {        
                case "a":
                    return new SvgAElement(prefix, localName, ns, doc);
                case "circle": 
                    return new SvgCircleElement(prefix, localName, ns, doc);
                case "clipPath": 
                    return new SvgClipPathElement(prefix, localName, ns, doc);
                case "defs": 
                    return new SvgDefsElement(prefix, localName, ns, doc);
                case "desc": 
                    return new SvgDescElement(prefix, localName, ns, doc);
                case "ellipse": 
                    return new SvgEllipseElement(prefix, localName, ns, doc);
                case "g": 
                    return new SvgGElement(prefix, localName, ns, doc);
                case "image": 
                    return new SvgImageElement(prefix, localName, ns, doc);
                case "line": 
                    return new SvgLineElement(prefix, localName, ns, doc);
                case "linearGradient": 
                    return new SvgLinearGradientElement(prefix, localName, ns, doc);
                case "marker": 
                    return new SvgMarkerElement(prefix, localName, ns, doc);
                case "mask": 
                    return new SvgMaskElement(prefix, localName, ns, doc);
                case "metadata": 
                    return new SvgMetadataElement(prefix, localName, ns, doc);
                case "rect": 
                    return new SvgRectElement(prefix, localName, ns, doc);
                case "path": 
                    return new SvgPathElement(prefix, localName, ns, doc);
                case "pattern": 
                    return new SvgPatternElement(prefix, localName, ns, doc);
                case "polyline": 
                    return new SvgPolylineElement(prefix, localName, ns, doc);
                case "polygon": 
                    return new SvgPolygonElement(prefix, localName, ns, doc);
                case "radialGradient": 
                    return new SvgRadialGradientElement(prefix, localName, ns, doc);
                case "script": 
                    return new SvgScriptElement(prefix, localName, ns, doc);
                case "stop": 
                    return new SvgStopElement(prefix, localName, ns, doc);
                case "svg": 
                    return new SvgSvgElement(prefix, localName, ns, doc);
                case "switch": 
                    return new SvgSwitchElement(prefix, localName, ns, doc);
                case "symbol": 
                    return new SvgSymbolElement(prefix, localName, ns, doc);
                case "text": 
                    return new SvgTextElement(prefix, localName, ns, doc);
                case "textPath":
                    return new SvgTextPathElement(prefix, localName, ns, doc);
                case "title": 
                    return new SvgTitleElement(prefix, localName, ns, doc);
                case "tref": 
                    return new SvgTRefElement(prefix, localName, ns, doc);
                case "tspan": 
                    return new SvgTSpanElement(prefix, localName, ns, doc);
                case "use": 
                    return new SvgUseElement(prefix, localName, ns, doc);
                case "color-profile":
                    return new SvgColorProfileElement(prefix, localName, ns, doc);

                // SVG font support
                case "font":
                    return new SvgFontElement(prefix, localName, ns, doc);
                case "font-face":
                    return new SvgFontFaceElement(prefix, localName, ns, doc);
                case "font-face-format":
                    return new SvgFontFaceFormatElement(prefix, localName, ns, doc);
                case "font-face-name":
                    return new SvgFontFaceNameElement(prefix, localName, ns, doc);
                case "font-face-src":
                    return new SvgFontFaceSrcElement(prefix, localName, ns, doc);
                case "font-face-uri":
                    return new SvgFontFaceUriElement(prefix, localName, ns, doc);
                case "glyph":
                    return new SvgGlyphElement(prefix, localName, ns, doc);
                case "glyphRef":
                    return new SvgGlyphRefElement(prefix, localName, ns, doc);
                case "hkern":
                    return new SvgHKernElement(prefix, localName, ns, doc);
                case "missing-glyph":
                    return new SvgMissingGlyphElement(prefix, localName, ns, doc);
                case "vkern":
                    return new SvgVKernElement(prefix, localName, ns, doc);

                // Animation Support
                case "animate":
                    return new SvgAnimateElement(prefix, localName, ns, doc);
                case "set":
                    return new SvgAnimateSetElement(prefix, localName, ns, doc);
                case "animateMotion":
                    return new SvgAnimateMotionElement(prefix, localName, ns, doc);
                case "animateColor":
                    return new SvgAnimateColorElement(prefix, localName, ns, doc);
                case "animateTransform":
                    return new SvgAnimateTransformElement(prefix, localName, ns, doc);
                case "mpath":
                    return new SvgAnimateMPathElement(prefix, localName, ns, doc);
            }

            return null;
        }
    }
}
