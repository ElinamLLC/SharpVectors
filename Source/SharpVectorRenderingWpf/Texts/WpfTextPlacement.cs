using System;
using System.Collections.Generic;

using System.Windows;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Texts
{
    public sealed class WpfTextPlacement
    {
        #region Private Fields

        private bool _isRotateOnly;
        private Point _location;
        private double _rotation;
        private IList<WpfTextPosition> _positions;

        #endregion

        #region Constructors and Destructor

        public WpfTextPlacement()
        {
        }

        public WpfTextPlacement(Point location, double rotation)
        {
            if (double.IsNaN(rotation) || double.IsInfinity(rotation))
            {
                rotation = 0;
            }

            _location = location;
            _rotation = rotation;
        }

        public WpfTextPlacement(Point location, double rotation,
            IList<WpfTextPosition> positions, bool isRotateOnly)
        {
            if (double.IsNaN(rotation) || double.IsInfinity(rotation))
            {
                rotation = 0;
            }

            _location = location;
            _rotation = rotation;
            _positions = positions;
            _isRotateOnly = isRotateOnly;
        }

        #endregion

        #region Public Properties

        public bool HasPositions
        {
            get {
                return (_positions != null && _positions.Count != 0);
            }
        }

        public Point Location
        {
            get {
                return _location;
            }
        }

        public double Rotation
        {
            get {
                return _rotation;
            }
        }

        public bool IsRotateOnly
        {
            get {
                return _isRotateOnly;
            }
        }

        public IList<WpfTextPosition> Positions
        {
            get {
                return _positions;
            }
        }

        #endregion

        #region Public Methods

        public static WpfTextPlacement Create(SvgTextPositioningElement posElement, Point p, bool isTextPath = false)
        {
            ISvgLengthList xValues  = posElement.X.AnimVal;
            ISvgLengthList yValues  = posElement.Y.AnimVal;
            ISvgLengthList dxValues = posElement.Dx.AnimVal;
            ISvgLengthList dyValues = posElement.Dy.AnimVal;
            ISvgNumberList rValues  = posElement.Rotate.AnimVal;

            bool requiresGlyphPositioning = false;
            bool isXYGlyphPositioning     = false;
            bool isDxyGlyphPositioning    = false;
            bool isRotateGlyphPositioning = false;

            double xValue  = p.X;
            double yValue  = p.Y;
            double rValue  = 0;
            double dxValue = 0;
            double dyValue = 0;

            WpfTextPlacement textPlacement = null;

            if (xValues.NumberOfItems > 0 && isTextPath == false)
            {
                if (xValues.NumberOfItems > 1)
                {
                    isXYGlyphPositioning     = true;
                    requiresGlyphPositioning = true;
                }

                xValue = xValues.GetItem(xValues.NumberOfItems - 1).Value;
                p.X = xValue;
            }
            if (yValues.NumberOfItems > 0)
            {
                if (yValues.NumberOfItems > 1)
                {
                    isXYGlyphPositioning     = true;
                    requiresGlyphPositioning = true;
                }

                yValue = yValues.GetItem(yValues.NumberOfItems - 1).Value;
                p.Y = yValue;
            }
            if (dxValues.NumberOfItems > 0 && isTextPath == false)
            {
                if (dxValues.NumberOfItems > 1)
                {
                    isDxyGlyphPositioning    = true;
                    requiresGlyphPositioning = true;
                }

                dxValue = dxValues.GetItem(dxValues.NumberOfItems - 1).Value;
                p.X += dxValue;
            }
            if (dyValues.NumberOfItems > 0)
            {
                if (dyValues.NumberOfItems > 1)
                {
                    isDxyGlyphPositioning    = true;
                    requiresGlyphPositioning = true;
                }

                dyValue = dyValues.GetItem(dyValues.NumberOfItems - 1).Value;
                p.Y += dyValue;
            }
            if (rValues.NumberOfItems > 0)
            {
                if (rValues.NumberOfItems > 1)
                {
                    isRotateGlyphPositioning = true;
                    requiresGlyphPositioning = true;
                }

                // If the element contains more characters than the number of values specified 
                // in the "rotate" attribute. In this case the last value specified in the "rotate" 
                // attribute of the "tspan" must be applied to the remaining characters in the string.
                rValue = rValues.GetItem(rValues.NumberOfItems - 1).Value;
            }

            if (requiresGlyphPositioning)
            {
                uint xCount  = xValues.NumberOfItems;
                uint yCount  = yValues.NumberOfItems;
                uint dxCount = dxValues.NumberOfItems;
                uint dyCount = dyValues.NumberOfItems;
                uint rCount  = rValues.NumberOfItems;

                List<WpfTextPosition> textPositions = null;

                bool isRotateOnly = false;

                if (isXYGlyphPositioning)
                {
                    uint itemCount = Math.Max(Math.Max(xCount, yCount), Math.Max(dxCount, dyCount));
                    itemCount      = Math.Max(itemCount, rCount);
                    textPositions  = new List<WpfTextPosition>((int)itemCount);

                    double xLast = 0;
                    double yLast = 0;

                    for (uint i = 0; i < itemCount; i++)
                    {
                        double xNext  = i < xCount ? xValues.GetItem(i).Value : xValue;
                        double yNext  = i < yCount ? yValues.GetItem(i).Value : yValue;
                        double rNext  = i < rCount ? rValues.GetItem(i).Value : rValue;
                        double dxNext = i < dxCount ? dxValues.GetItem(i).Value : dxValue;
                        double dyNext = i < dyCount ? dyValues.GetItem(i).Value : dyValue;

                        if (i < xCount)
                        {
                            xLast = xNext;
                        }
                        else
                        {
                            xNext = xLast;
                        }
                        if (i < yCount)
                        {
                            yLast = yNext;
                        }
                        else
                        {
                            yNext = yLast;
                        }

                        WpfTextPosition textPosition = new WpfTextPosition(
                            new Point(xNext + dxNext, yNext + dyNext), rNext);

                        textPositions.Add(textPosition);
                    }
                }
                else if (isDxyGlyphPositioning)
                {
                }
                else if (isRotateGlyphPositioning)
                {
                    isRotateOnly   = true;
                    uint itemCount = Math.Max(Math.Max(xCount, yCount), Math.Max(dxCount, dyCount));
                    itemCount      = Math.Max(itemCount, rCount);
                    textPositions  = new List<WpfTextPosition>((int)itemCount);

                    for (uint i = 0; i < itemCount; i++)
                    {
                        double rNext = i < rCount ? rValues.GetItem(i).Value : rValue;

                        WpfTextPosition textPosition = new WpfTextPosition(p, rNext);

                        textPositions.Add(textPosition);
                    }
                }

                if (textPositions != null && textPositions.Count != 0)
                {
                    textPlacement = new WpfTextPlacement(p, rValue, textPositions, isRotateOnly);
                }
                else
                {
                    textPlacement = new WpfTextPlacement(p, rValue);
                }
            }
            else
            {
                textPlacement = new WpfTextPlacement(p, rValue);
            }

            return textPlacement;
        }

        public void UpdatePositions(string targetText)
        {
            //if (string.IsNullOrWhiteSpace(targetText) || (_positions == null || _positions.Count == 0))
            //{
            //    return;
            //}

            //int posCount  = _positions.Count;
            //int textCount = targetText.Length;

            //if (textCount <= posCount)
            //{
            //    return;
            //}
            //WpfTextPosition textPos = new WpfTextPosition(_location, _rotation);
            //for (int i = posCount; i < textCount; i++)
            //{
            //    _positions.Add(textPos);
            //}
        }

        #endregion
    }
}
