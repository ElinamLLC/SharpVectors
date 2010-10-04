using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgMarkerElement : SvgStyleableElement, ISvgMarkerElement
	{
		public SvgMarkerElement(string prefix, string localname, string ns, SvgDocument doc) 
            : base(prefix, localname, ns, doc) 
		{
			svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
			svgFitToViewBox = new SvgFitToViewBox(this);
		}

        #region ISvgElement Members

        /// <summary>
        /// Gets a value indicating whether this SVG element is renderable.
        /// </summary>
        /// <value>
        /// This is <see langword="'true"/> if the element is renderable; otherwise,
        /// it is <see langword="false"/>.
        /// </value>
        public override bool IsRenderable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        /// An enumeration of the <see cref="SvgRenderingHint"/> specifying the rendering hint.
        /// This will always return <see cref="SvgRenderingHint.Containment"/>
        /// </value>
        public override SvgRenderingHint RenderingHint
        {
            get
            {
                return SvgRenderingHint.Containment;
            }
        }

        #endregion

		#region ISvgMarkerElement Members

		/// <summary>
		///  Sets the value of attribute orient to 'auto'.
		/// </summary>
		public void SetOrientToAuto()
		{
			orientType = null;
			SetAttribute("orient", "auto");
		}

		/// <summary>
		///  Sets the value of attribute orient to the given angle.
		/// </summary>
		/// <param name="angle"> The angle value to use for attribute orient.</param>
		public void SetOrientToAngle(ISvgAngle angle)
		{
			orientType = null;
			SetAttribute("orient", angle.ValueAsString);
			orientAngle = new SvgAnimatedAngle(angle);
		}

		private ISvgAnimatedLength refX;
		/// <summary>
		/// Corresponds to attribute refX on the given 'marker' element.
		/// </summary>
		public ISvgAnimatedLength RefX
		{
			get
			{
				if(refX == null)
				{
					refX = new SvgAnimatedLength(this, "refX", SvgLengthDirection.Horizontal, "0");
				}
				return refX;
			}
		}

		private ISvgAnimatedLength refY;
		/// <summary>
		/// Corresponds to attribute refY on the given 'marker' element.
		/// </summary>
		public ISvgAnimatedLength RefY
		{
			get
			{
				if(refY == null)
				{
					refY = new SvgAnimatedLength(this, "refY", SvgLengthDirection.Vertical, "0");
				}
				return refY;
			}
		}

		private ISvgAnimatedEnumeration markerUnits;
		/// <summary>
		/// Corresponds to attribute markerUnits on the given 'marker' element.
		/// </summary>
		public ISvgAnimatedEnumeration MarkerUnits
		{
			get
			{
				if(markerUnits == null)
				{
					SvgMarkerUnit type = SvgMarkerUnit.Unknown;
					switch(GetAttribute("markerUnits"))
					{
						case "userSpaceOnUse":
							type = SvgMarkerUnit.UserSpaceOnUse;
							break;
						case "":
						case "strokeWidth":
							type = SvgMarkerUnit.StrokeWidth;
							break;
					}
					markerUnits = new SvgAnimatedEnumeration((ushort)type);
				}
				return markerUnits;
			}
		}

		private ISvgAnimatedLength markerWidth;
		/// <summary>
		/// Corresponds to attribute markerWidth on the given 'marker' element
		/// </summary>
		public ISvgAnimatedLength MarkerWidth
		{
			get
			{
				if(markerWidth == null)
				{
					markerWidth = new SvgAnimatedLength(this, "markerWidth", SvgLengthDirection.Horizontal, "3");
				}
				return markerWidth;
			}
		}

		private ISvgAnimatedLength markerHeight;
		/// <summary>
		/// Corresponds to attribute markerHeight on the given 'marker' element.
		/// </summary>
		public ISvgAnimatedLength MarkerHeight
		{
			get
			{
				if(markerHeight == null)
				{
					markerHeight = new SvgAnimatedLength(this, "markerHeight", SvgLengthDirection.Vertical, "3");
				}
				return markerHeight;
			}
		}

		private ISvgAnimatedEnumeration orientType;
		/// <summary>
		/// Corresponds to attribute orient on the given 'marker' element. One of the Marker Orientation Types defined above.
		/// </summary>
		public ISvgAnimatedEnumeration OrientType
		{
			get
			{
				if(orientType == null)
				{
					if(GetAttribute("orient") == "auto")
					{
						orientType = new SvgAnimatedEnumeration((ushort)SvgMarkerOrient.Auto);
					}
					else
					{
						orientType = new SvgAnimatedEnumeration((ushort)SvgMarkerOrient.Angle);
					}
				}
				return orientType;
			}
		}

		public ISvgAnimatedAngle orientAngle;
		/// <summary>
		/// Corresponds to attribute orient on the given 'marker' element. If markerUnits is SVG_MARKER_ORIENT_ANGLE, the angle value for attribute orient; otherwise, it will be set to zero.
		/// </summary>
		public ISvgAnimatedAngle OrientAngle
		{
			get
			{
				if(orientAngle == null)
				{
					if(OrientType.AnimVal.Equals(SvgMarkerOrient.Angle))
					{
						orientAngle = new SvgAnimatedAngle(GetAttribute("orient"), "0");
					}
					else
					{
						orientAngle = new SvgAnimatedAngle("0", "0");
					}
				}
				return orientAngle;
			}
		}
		#endregion

		#region ISvgFitToViewBox Members

		private SvgFitToViewBox svgFitToViewBox;
		public ISvgAnimatedRect ViewBox
		{
			get
			{
				return svgFitToViewBox.ViewBox;
			}
		}

		public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio
		{
			get
			{
				return svgFitToViewBox.PreserveAspectRatio;
			}
		}

		#endregion

		#region ISvgExternalResourcesRequired Members

		private SvgExternalResourcesRequired svgExternalResourcesRequired;
		public ISvgAnimatedBoolean ExternalResourcesRequired
		{
			get
			{
				return svgExternalResourcesRequired.ExternalResourcesRequired;
			}
		}

		#endregion
	}
}
