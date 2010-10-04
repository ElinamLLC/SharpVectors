using System;
using System.Xml;

using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
    public class SvgTransformableElement : SvgStyleableElement, ISvgTransformable
    {
        #region Constructors and Destructor

        public SvgTransformableElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgTransformable Members

        private ISvgAnimatedTransformList transform;
        public ISvgAnimatedTransformList Transform
        {
            get
            {
                if (transform == null)
                {
                    transform = new SvgAnimatedTransformList(GetAttribute("transform"));
                }
                return transform;
            }
        }

        public ISvgElement NearestViewportElement
        {
            get
            {
                XmlNode parent = this.ParentNode;
                while (parent != null)
                {
                    if (parent is SvgSvgElement)
                    {
                        return (ISvgElement)parent;
                    }
                    parent = parent.ParentNode;
                }
                return null;
            }
        }

        public ISvgElement FarthestViewportElement
        {
            get
            {
                ISvgDocument doc = OwnerDocument;
                if (doc.RootElement == this) return null;
                else return doc.RootElement;
            }
        }

        public ISvgRect GetBBox()
        {
            ISvgWindow svgWnd = this.OwnerDocument.Window;
            if (svgWnd == null || svgWnd.Renderer == null)
            {
                return null;
            }

            return svgWnd.Renderer.GetRenderedBounds(this, 0);
        }

        /// <summary>
        /// For each given element, the accumulation of all transformations that have been defined 
        /// on the given element and all of its ancestors up to and including the element that 
        /// established the current viewport (usually, the 'svg' element which is the most 
        /// immediate ancestor to the given element) is called the current transformation matrix 
        /// or CTM. 
        /// </summary>
        /// <returns>A matrix representing the mapping of current user coordinates to viewport 
        /// coordinates.</returns>
        public ISvgMatrix GetCTM()
        {
            ISvgMatrix matrix = new SvgMatrix();
            ISvgTransformList svgTList;
            ISvgMatrix vCTM;
            if (this is SvgSvgElement)
            {
                vCTM = (this as SvgSvgElement).ViewBoxTransform;
                matrix = vCTM;
            }
            else if (this.Transform != null)
            {
                svgTList = this.Transform.AnimVal;
                matrix = svgTList.Consolidate().Matrix;
            }

            ISvgElement nVE = this.NearestViewportElement;
            if (nVE != null)
            {
                SvgTransformableElement par = ParentNode as SvgTransformableElement;
                while (par != null && par != nVE)
                {
                    svgTList = par.Transform.AnimVal;
                    matrix = svgTList.Consolidate().Matrix.Multiply(matrix);
                    par = par.ParentNode as SvgTransformableElement;
                }

                if (par == nVE && nVE is SvgSvgElement)
                {
                    vCTM = (nVE as SvgSvgElement).ViewBoxTransform;
                    matrix = vCTM.Multiply(matrix);
                }
            }
            return matrix;
        }

        public ISvgMatrix GetScreenCTM()
        {
            ISvgMatrix matrix = new SvgMatrix();
            ISvgTransformList svgTList;
            ISvgMatrix vCTM;
            if (this is SvgSvgElement)
            {
                vCTM = (this as SvgSvgElement).ViewBoxTransform;
                matrix = vCTM;
            }
            else if (this.Transform != null)
            {
                svgTList = this.Transform.AnimVal;
                matrix = svgTList.Consolidate().Matrix;
            }

            SvgTransformableElement par = ParentNode as SvgTransformableElement;
            while (par != null)
            {
                // TODO: other elements can establish viewports, not just <svg>!
                if (par is SvgSvgElement)
                {
                    vCTM = (par as SvgSvgElement).ViewBoxTransform;
                    matrix = vCTM.Multiply(matrix);
                }
                else
                {
                    svgTList = par.Transform.AnimVal;
                    matrix = svgTList.Consolidate().Matrix.Multiply(matrix);
                }
                par = par.ParentNode as SvgTransformableElement;
            }

            // Now scale out the pixels
            //ISvgSvgElement root = OwnerDocument.RootElement;
            //float innerWidth = this.OwnerDocument.Window.InnerWidth;
            //float innerHeight = this.OwnerDocument.Window.InnerHeight;
            //if (innerWidth != 0 && innerHeight != 0) 
            //{
            //  float screenRatW = (float)root.Width.AnimVal.Value / innerWidth;
            //  float screenRatH = (float)root.Height.AnimVal.Value / innerHeight;
            //  matrix.ScaleNonUniform(screenRatW, screenRatH);
            //}

            return matrix;
        }

        public ISvgMatrix GetTransformToElement(ISvgElement element)
        {
            ISvgLocatable loc = element as ISvgLocatable;
            ISvgMatrix ctm = loc.GetCTM();
            ISvgMatrix vctm;
            XmlNode node = element.ParentNode;
            while (node != null && node != OwnerDocument)
            {
                if (node.Name == "svg")
                {
                    vctm = (node as SvgSvgElement).ViewBoxTransform;
                    ctm = vctm.Multiply(ctm);
                }
                else
                {
                    loc = node as ISvgLocatable;
                    ctm = loc.GetCTM().Multiply(ctm);
                }

                node = node.ParentNode;
            }

            return ctm;
        }

        #endregion

        #region Public Methods

        #region Update handling

        public override void CssInvalidate()
        {
            base.CssInvalidate();

            //string strokeWidth = this.GetPropertyValue("stroke-width");
            //if (strokeWidth.Length == 0) strokeWidth = "1px";
            //SvgLength strokeWidthLength = new SvgLength(this, "stroke-width", SvgLengthDirection.Viewport, strokeWidth);

            //if (renderingNode != null)
            //{
            //    // Quick-cache
            //    renderingNode.ScreenRegion = GetBRect((float)strokeWidthLength.Value);
            //    OwnerDocument.Window.Renderer.InvalidateRect(renderingNode.ScreenRegion);
            //}
            //else
            //{
            //    OwnerDocument.Window.Renderer.InvalidateRect(GetBRect((float)strokeWidthLength.Value));
            //}
        }

        public override void HandleAttributeChange(XmlAttribute attribute)
        {
            if (attribute.NamespaceURI.Length == 0)
            {
                switch (attribute.LocalName)
                {
                    case "transform":
                        transform = null;
                        //renderingNode = null;
                        return;
                }

                base.HandleAttributeChange(attribute);
            }
        }

        #endregion

        #endregion
    }
}
