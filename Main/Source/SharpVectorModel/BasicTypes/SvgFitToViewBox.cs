using System;
using System.Xml;

using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
    public class SvgFitToViewBox : ISvgFitToViewBox
	{
        private ISvgAnimatedRect viewBox;
        private ISvgAnimatedPreserveAspectRatio preserveAspectRatio;

        #region Protected Fields

        protected SvgElement ownerElement;

        #endregion

		public SvgFitToViewBox(SvgElement ownerElement)
		{
			this.ownerElement = ownerElement;
			this.ownerElement.attributeChangeHandler += new NodeChangeHandler(AttributeChange);
		}

		#region Update handling

		private void AttributeChange(Object src, XmlNodeChangedEventArgs args)
		{
			XmlAttribute attribute = src as XmlAttribute;

			if(attribute.NamespaceURI.Length == 0)
			{
				switch(attribute.LocalName)
				{
					case "viewBox":
						viewBox = null;
						break;
					case "preserveAspectRatio":
						preserveAspectRatio = null;
						break;
				}
			}
		}

		#endregion

		public ISvgAnimatedRect ViewBox
		{
			get
			{
				if (viewBox == null)
				{
					string attr = ownerElement.GetAttribute("viewBox").Trim();
					if (String.IsNullOrEmpty(attr))
					{
						double x      = 0;
						double y      = 0;
						double width  = 0;
						double height = 0;
						if (ownerElement is SvgSvgElement)
						{
							SvgSvgElement svgSvgElm = ownerElement as SvgSvgElement;

							x      = svgSvgElm.X.AnimVal.Value;
							y      = svgSvgElm.Y.AnimVal.Value; 
							width  = svgSvgElm.Width.AnimVal.Value;
							height = svgSvgElm.Height.AnimVal.Value;
						}
						viewBox = new SvgAnimatedRect(new SvgRect(x, y, width, height));
					}
					else
					{
						viewBox = new SvgAnimatedRect(attr);
					}
				}

				return viewBox;
			}
		}

		public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio
		{
			get
			{
				if (preserveAspectRatio == null)
				{
					preserveAspectRatio = new SvgAnimatedPreserveAspectRatio(
                        ownerElement.GetAttribute("preserveAspectRatio"), ownerElement);
				}
				return preserveAspectRatio;
			}
		}  
	}
}
