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
            //if (_svgElements.ContainsKey(localName))
            //{
            //    return _svgElements[localName].Invoke(prefix, localName, ns, doc);
            //}

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
                case "style":
                    return new SvgStyleElement(prefix, localName, ns, doc);
                case "stop":
                    return new SvgStopElement(prefix, localName, ns, doc);
                case "svg":
                    return new SvgSvgElement(prefix, localName, ns, doc);
                case "switch":
                    return new SvgSwitchElement(prefix, localName, ns, doc);
                case "symbol":
                    return new SvgSymbolElement(prefix, localName, ns, doc);
                case "solidColor":
                case "solidcolor":
                    return new SvgSolidColorElement(prefix, localName, ns, doc);
                case "text":
                    return new SvgTextElement(prefix, localName, ns, doc);
                case "textArea":
                    return new SvgTextAreaElement(prefix, localName, ns, doc);
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
                case "altGlyph":
                    return new SvgAltGlyphElement(prefix, localName, ns, doc);
                case "altGlyphDef":
                    return new SvgAltGlyphDefElement(prefix, localName, ns, doc);
                case "altGlyphItem":
                    return new SvgAltGlyphItemElement(prefix, localName, ns, doc);

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

                // Filter Effects support
                case "filter":
                    return new SvgFilterElement(prefix, localName, ns, doc);

                case "feDistantLight":
                    return new SvgFEDistantLightElement(prefix, localName, ns, doc);
                case "fePointLight":
                    return new SvgFEPointLightElement(prefix, localName, ns, doc);
                case "feSpotLight":
                    return new SvgFESpotLightElement(prefix, localName, ns, doc);

                case "feBlend":
                    return new SvgFEBlendElement(prefix, localName, ns, doc);
                case "feColorMatrix":
                    return new SvgFEColorMatrixElement(prefix, localName, ns, doc);
                case "feComponentTransfer":
                    return new SvgFEComponentTransferElement(prefix, localName, ns, doc);
                case "feComposite":
                    return new SvgFECompositeElement(prefix, localName, ns, doc);
                case "feConvolveMatrix":
                    return new SvgFEConvolveMatrixElement(prefix, localName, ns, doc);
                case "feDiffuseLighting":
                    return new SvgFEDiffuseLightingElement(prefix, localName, ns, doc);
                case "feDisplacementMap":
                    return new SvgFEDisplacementMapElement(prefix, localName, ns, doc);
                case "feFlood":
                    return new SvgFEFloodElement(prefix, localName, ns, doc);

                case "feFuncR":
                    return new SvgFEFuncRElement(prefix, localName, ns, doc);
                case "feFuncG":
                    return new SvgFEFuncGElement(prefix, localName, ns, doc);
                case "feFuncB":
                    return new SvgFEFuncBElement(prefix, localName, ns, doc);
                case "feFuncA":
                    return new SvgFEFuncAElement(prefix, localName, ns, doc);

                case "feGaussianBlur":
                    return new SvgFEGaussianBlurElement(prefix, localName, ns, doc);
                case "feImage":
                    return new SvgFEImageElement(prefix, localName, ns, doc);
                case "feMerge":
                    return new SvgFEMergeElement(prefix, localName, ns, doc);
                case "feMergeNode":
                    return new SvgFEMergeNodeElement(prefix, localName, ns, doc);
                case "feMorphology":
                    return new SvgFEMorphologyElement(prefix, localName, ns, doc);
                case "feOffset":
                    return new SvgFEOffsetElement(prefix, localName, ns, doc);
                case "feSpecularLighting":
                    return new SvgFESpecularLightingElement(prefix, localName, ns, doc);
                case "feTile":
                    return new SvgFETileElement(prefix, localName, ns, doc);
                case "feTurbulence":
                    return new SvgFETurbulenceElement(prefix, localName, ns, doc);
            }

            return null;
        }

        //private readonly static Dictionary<string, Func<string, string, string, SvgDocument, XmlElement>> _svgElements;

        //static SvgElementFactory()
        //{
        //    _svgElements = new Dictionary<string, Func<string, string, string, SvgDocument, XmlElement>>()
        //    {
        //        { "a",
        //            (prefix, localName, ns, doc) => new SvgAElement(prefix, localName, ns, doc) },
        //        { "circle",
        //            (prefix, localName, ns, doc) => new SvgCircleElement(prefix, localName, ns, doc) },
        //        { "clipPath",
        //            (prefix, localName, ns, doc) => new SvgClipPathElement(prefix, localName, ns, doc) },
        //        { "defs",
        //            (prefix, localName, ns, doc) => new SvgDefsElement(prefix, localName, ns, doc) },
        //        { "desc",
        //            (prefix, localName, ns, doc) => new SvgDescElement(prefix, localName, ns, doc) },
        //        { "ellipse",
        //            (prefix, localName, ns, doc) => new SvgEllipseElement(prefix, localName, ns, doc) },
        //        { "g",
        //            (prefix, localName, ns, doc) => new SvgGElement(prefix, localName, ns, doc) },
        //        { "image",
        //            (prefix, localName, ns, doc) => new SvgImageElement(prefix, localName, ns, doc) },
        //        { "line",
        //            (prefix, localName, ns, doc) => new SvgLineElement(prefix, localName, ns, doc) },
        //        { "linearGradient",
        //            (prefix, localName, ns, doc) => new SvgLinearGradientElement(prefix, localName, ns, doc) },
        //        { "marker",
        //            (prefix, localName, ns, doc) => new SvgMarkerElement(prefix, localName, ns, doc) },
        //        { "mask",
        //            (prefix, localName, ns, doc) => new SvgMaskElement(prefix, localName, ns, doc) },
        //        { "metadata",
        //            (prefix, localName, ns, doc) => new SvgMetadataElement(prefix, localName, ns, doc) },
        //        { "rect",
        //            (prefix, localName, ns, doc) => new SvgRectElement(prefix, localName, ns, doc) },
        //        { "path",
        //            (prefix, localName, ns, doc) => new SvgPathElement(prefix, localName, ns, doc) },
        //        { "pattern",
        //            (prefix, localName, ns, doc) => new SvgPatternElement(prefix, localName, ns, doc) },
        //        { "polyline",
        //            (prefix, localName, ns, doc) => new SvgPolylineElement(prefix, localName, ns, doc) },
        //        { "polygon",
        //            (prefix, localName, ns, doc) => new SvgPolygonElement(prefix, localName, ns, doc) },
        //        { "radialGradient",
        //            (prefix, localName, ns, doc) => new SvgRadialGradientElement(prefix, localName, ns, doc) },
        //        { "script",
        //            (prefix, localName, ns, doc) => new SvgScriptElement(prefix, localName, ns, doc) },
        //        { "stop",
        //            (prefix, localName, ns, doc) => new SvgStopElement(prefix, localName, ns, doc) },
        //        { "svg",
        //            (prefix, localName, ns, doc) => new SvgSvgElement(prefix, localName, ns, doc) },
        //        { "switch",
        //            (prefix, localName, ns, doc) => new SvgSwitchElement(prefix, localName, ns, doc) },
        //        { "symbol",
        //            (prefix, localName, ns, doc) => new SvgSymbolElement(prefix, localName, ns, doc) },
        //        { "solidColor",
        //            (prefix, localName, ns, doc) => new SvgSolidColorElement(prefix, localName, ns, doc) },
        //        { "solidcolor",
        //            (prefix, localName, ns, doc) => new SvgSolidColorElement(prefix, localName, ns, doc) },
        //        { "text",
        //            (prefix, localName, ns, doc) => new SvgTextElement(prefix, localName, ns, doc) },
        //        { "textArea",
        //            (prefix, localName, ns, doc) => new SvgTextAreaElement(prefix, localName, ns, doc) },
        //        { "textPath",
        //            (prefix, localName, ns, doc) => new SvgTextPathElement(prefix, localName, ns, doc) },
        //        { "title",
        //            (prefix, localName, ns, doc) => new SvgTitleElement(prefix, localName, ns, doc) },
        //        { "tref",
        //            (prefix, localName, ns, doc) => new SvgTRefElement(prefix, localName, ns, doc) },
        //        { "tspan",
        //            (prefix, localName, ns, doc) => new SvgTSpanElement(prefix, localName, ns, doc) },
        //        { "use",
        //            (prefix, localName, ns, doc) => new SvgUseElement(prefix, localName, ns, doc) },
        //        { "color-profile",
        //            (prefix, localName, ns, doc) => new SvgColorProfileElement(prefix, localName, ns, doc) },

        //        // SVG font support
        //        { "font",
        //            (prefix, localName, ns, doc) => new SvgFontElement(prefix, localName, ns, doc) },
        //        { "font-face",
        //            (prefix, localName, ns, doc) => new SvgFontFaceElement(prefix, localName, ns, doc) },
        //        { "font-face-format",
        //            (prefix, localName, ns, doc) => new SvgFontFaceFormatElement(prefix, localName, ns, doc) },
        //        { "font-face-name",
        //            (prefix, localName, ns, doc) => new SvgFontFaceNameElement(prefix, localName, ns, doc) },
        //        { "font-face-src",
        //            (prefix, localName, ns, doc) => new SvgFontFaceSrcElement(prefix, localName, ns, doc) },
        //        { "font-face-uri",
        //            (prefix, localName, ns, doc) => new SvgFontFaceUriElement(prefix, localName, ns, doc) },
        //        { "glyph",
        //            (prefix, localName, ns, doc) => new SvgGlyphElement(prefix, localName, ns, doc) },
        //        { "glyphRef",
        //            (prefix, localName, ns, doc) => new SvgGlyphRefElement(prefix, localName, ns, doc) },
        //        { "hkern",
        //            (prefix, localName, ns, doc) => new SvgHKernElement(prefix, localName, ns, doc) },
        //        { "missing-glyph",
        //            (prefix, localName, ns, doc) => new SvgMissingGlyphElement(prefix, localName, ns, doc) },
        //        { "vkern",
        //            (prefix, localName, ns, doc) => new SvgVKernElement(prefix, localName, ns, doc) },
        //        { "altGlyph",
        //            (prefix, localName, ns, doc) => new SvgAltGlyphElement(prefix, localName, ns, doc) },
        //        { "altGlyphDef",
        //            (prefix, localName, ns, doc) => new SvgAltGlyphDefElement(prefix, localName, ns, doc) },
        //        { "altGlyphItem",
        //            (prefix, localName, ns, doc) => new SvgAltGlyphItemElement(prefix, localName, ns, doc) },

        //        // Animation Support
        //        { "animate",
        //            (prefix, localName, ns, doc) => new SvgAnimateElement(prefix, localName, ns, doc) },
        //        { "set",
        //            (prefix, localName, ns, doc) => new SvgAnimateSetElement(prefix, localName, ns, doc) },
        //        { "animateMotion",
        //            (prefix, localName, ns, doc) => new SvgAnimateMotionElement(prefix, localName, ns, doc) },
        //        { "animateColor",
        //            (prefix, localName, ns, doc) => new SvgAnimateColorElement(prefix, localName, ns, doc) },
        //        { "animateTransform",
        //            (prefix, localName, ns, doc) => new SvgAnimateTransformElement(prefix, localName, ns, doc) },
        //        { "mpath",
        //            (prefix, localName, ns, doc) => new SvgAnimateMPathElement(prefix, localName, ns, doc) },

        //        // Filter Effects support
        //        { "filter",
        //            (prefix, localName, ns, doc) => new SvgFilterElement(prefix, localName, ns, doc) },

        //        { "feDistantLight",
        //            (prefix, localName, ns, doc) => new SvgFEDistantLightElement(prefix, localName, ns, doc) },
        //        { "fePointLight",
        //            (prefix, localName, ns, doc) => new SvgFEPointLightElement(prefix, localName, ns, doc) },
        //        { "feSpotLight",
        //            (prefix, localName, ns, doc) => new SvgFESpotLightElement(prefix, localName, ns, doc) },

        //        { "feBlend",
        //            (prefix, localName, ns, doc) => new SvgFEBlendElement(prefix, localName, ns, doc) },
        //        { "feColorMatrix",
        //            (prefix, localName, ns, doc) => new SvgFEColorMatrixElement(prefix, localName, ns, doc) },
        //        { "feComponentTransfer",
        //            (prefix, localName, ns, doc) => new SvgFEComponentTransferElement(prefix, localName, ns, doc) },
        //        { "feComposite",
        //            (prefix, localName, ns, doc) => new SvgFECompositeElement(prefix, localName, ns, doc) },
        //        { "feConvolveMatrix",
        //            (prefix, localName, ns, doc) => new SvgFEConvolveMatrixElement(prefix, localName, ns, doc) },
        //        { "feDiffuseLighting",
        //            (prefix, localName, ns, doc) => new SvgFEDiffuseLightingElement(prefix, localName, ns, doc) },
        //        { "feDisplacementMap",
        //            (prefix, localName, ns, doc) => new SvgFEDisplacementMapElement(prefix, localName, ns, doc) },
        //        { "feFlood",
        //            (prefix, localName, ns, doc) => new SvgFEFloodElement(prefix, localName, ns, doc) },

        //        { "feFuncR",
        //            (prefix, localName, ns, doc) => new SvgFEFuncRElement(prefix, localName, ns, doc) },
        //        { "feFuncG",
        //            (prefix, localName, ns, doc) => new SvgFEFuncGElement(prefix, localName, ns, doc) },
        //        { "feFuncB",
        //            (prefix, localName, ns, doc) => new SvgFEFuncBElement(prefix, localName, ns, doc) },
        //        { "feFuncA",
        //            (prefix, localName, ns, doc) => new SvgFEFuncAElement(prefix, localName, ns, doc) },

        //        { "feGaussianBlur",
        //            (prefix, localName, ns, doc) => new SvgFEGaussianBlurElement(prefix, localName, ns, doc) },
        //        { "feImage",
        //            (prefix, localName, ns, doc) => new SvgFEImageElement(prefix, localName, ns, doc) },
        //        { "feMerge",
        //            (prefix, localName, ns, doc) => new SvgFEMergeElement(prefix, localName, ns, doc) },
        //        { "feMergeNode",
        //            (prefix, localName, ns, doc) => new SvgFEMergeNodeElement(prefix, localName, ns, doc) },
        //        { "feMorphology",
        //            (prefix, localName, ns, doc) => new SvgFEMorphologyElement(prefix, localName, ns, doc) },
        //        { "feOffset",
        //            (prefix, localName, ns, doc) => new SvgFEOffsetElement(prefix, localName, ns, doc) },
        //        { "feSpecularLighting",
        //            (prefix, localName, ns, doc) => new SvgFESpecularLightingElement(prefix, localName, ns, doc) },
        //        { "feTile",
        //            (prefix, localName, ns, doc) => new SvgFETileElement(prefix, localName, ns, doc) },
        //        { "feTurbulence",
        //            (prefix, localName, ns, doc) => new SvgFETurbulenceElement(prefix, localName, ns, doc) }
        //    };
        //}
    }
}
