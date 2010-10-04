using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public enum SvgLengthAdjust
    {
        Unknown          = 0,
        Spacing          = 1,
        SpacingAndGlyphs = 2
    }

    /// <summary>
    /// Summary description for SvgTextContentElement.
    /// </summary>
    public abstract class SvgTextContentElement : SvgTransformableElement, ISvgTextContentElement
    {
        #region Private Fields

        private SvgTests svgTests;
        private SvgExternalResourcesRequired svgExternalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        protected SvgTextContentElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgTests = new SvgTests(this);
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
        }

        #endregion

        #region ISvgElement Members

        /// <summary>
        /// Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        /// An enumeration of the <see cref="SvgRenderingHint"/> specifying the rendering hint.
        /// This will always return <see cref="SvgRenderingHint.Text"/>
        /// </value>
        public override SvgRenderingHint RenderingHint
        {
            get
            {
                return SvgRenderingHint.Text;
            }
        }

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get
            {
                return svgExternalResourcesRequired.ExternalResourcesRequired;
            }
        }
        #endregion

        #region ISvgTests Members
        
        public ISvgStringList RequiredFeatures
        {
            get { return svgTests.RequiredFeatures; }
        }

        public ISvgStringList RequiredExtensions
        {
            get { return svgTests.RequiredExtensions; }
        }

        public ISvgStringList SystemLanguage
        {
            get { return svgTests.SystemLanguage; }
        }

        public bool HasExtension(string extension)
        {
            return svgTests.HasExtension(extension);
        }

        #endregion

        public ISvgAnimatedLength TextLength
        {
            get { throw new NotImplementedException(); }
        }

        public ISvgAnimatedEnumeration LengthAdjust
        {
            get { throw new NotImplementedException(); }
        }

        protected SvgTextElement OwnerTextElement
        {
            get
            {
                XmlNode node = this;
                while (node != null)
                {                     
                    SvgTextElement text = node as SvgTextElement;
                    if (text != null)
                    {
                        return text;
                    }

                    node = node.ParentNode;
                }

                return null;
            }
        }

        #region Update handling

        public void Invalidate()
        {
        }

        public override void HandleAttributeChange(XmlAttribute attribute)
        {
            if (attribute.NamespaceURI.Length == 0)
            {
                // This list may be too long to be useful...
                switch (attribute.LocalName)
                {
                    // Additional attributes
                    case "x":
                    case "y":
                    case "dx":
                    case "dy":
                    case "rotate":
                    case "textLength":
                    case "lengthAdjust":
                    // Text.attrib
                    case "writing-mode":
                    // TextContent.attrib
                    case "alignment-baseline":
                    case "baseline-shift":
                    case "direction":
                    case "dominant-baseline":
                    case "glyph-orientation-horizontal":
                    case "glyph-orientation-vertical":
                    case "kerning":
                    case "letter-spacing":
                    case "text-anchor":
                    case "text-decoration":
                    case "unicode-bidi":
                    case "word-spacing":
                    // Font.attrib
                    case "font-family":
                    case "font-size":
                    case "font-size-adjust":
                    case "font-stretch":
                    case "font-style":
                    case "font-variant":
                    case "font-weight":
                    // textPath
                    case "startOffset":
                    case "method":
                    case "spacing":
                    // Color.attrib, Paint.attrib 
                    case "color":
                    case "fill":
                    case "fill-rule":
                    case "stroke":
                    case "stroke-dasharray":
                    case "stroke-dashoffset":
                    case "stroke-linecap":
                    case "stroke-linejoin":
                    case "stroke-miterlimit":
                    case "stroke-width":
                    // Opacity.attrib
                    case "opacity":
                    case "stroke-opacity":
                    case "fill-opacity":
                    // Graphics.attrib
                    case "display":
                    case "image-rendering":
                    case "shape-rendering":
                    case "text-rendering":
                    case "visibility":
                        Invalidate();
                        return;
                    case "transform":
                        Invalidate();
                        break;
                }

                base.HandleAttributeChange(attribute);
            }
            else if (attribute.Name == "xml:preserve" || attribute.Name == "xlink:href")
            {
                // xml:preserve and xlink:href changes may affect the actual text content
                Invalidate();
            }
        }

        public override void ElementChange(Object src, XmlNodeChangedEventArgs args)
        {
            Invalidate();

            base.ElementChange(src, args);
        }

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        public long GetNumberOfChars()
        {
            return this.InnerText.Length;
        }

        #region UnImplemented Methods

        public float GetComputedTextLength()
        {
            throw new NotImplementedException();
        }

        public float GetSubStringLength(long charnum, long nchars)
        {
            throw new NotImplementedException();
        }

        public ISvgPoint GetStartPositionOfChar(long charnum)
        {
            throw new NotImplementedException();
        }

        public ISvgPoint GetEndPositionOfChar(long charnum)
        {
            throw new NotImplementedException();
        }

        public ISvgRect GetExtentOfChar(long charnum)
        {
            throw new NotImplementedException();
        }

        public float GetRotationOfChar(long charnum)
        {
            throw new NotImplementedException();
        }

        public long GetCharNumAtPosition(ISvgPoint point)
        {
            throw new NotImplementedException();
        }

        public void SelectSubString(long charnum, long nchars)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
