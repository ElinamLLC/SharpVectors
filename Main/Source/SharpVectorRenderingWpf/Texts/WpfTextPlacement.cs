using System;
using System.Collections.Generic;

using System.Windows;

namespace SharpVectors.Renderers.Texts
{
    public sealed class WpfTextPlacement
    {
        #region Private Fields

        private bool   _isRotateOnly;
        private Point  _location;
        private double _rotation;
        private IList<WpfTextPosition> _positions;

        #endregion

        #region Constructors and Destructor

        public WpfTextPlacement()
        {
        }

        public WpfTextPlacement(Point location, double rotation)
        {
            if (Double.IsNaN(rotation) || Double.IsInfinity(rotation))
            {
                rotation = 0;
            }

            _location = location;
            _rotation = rotation;
        }

        public WpfTextPlacement(Point location, double rotation, 
            IList<WpfTextPosition> positions, bool isRotateOnly)
        {
            if (Double.IsNaN(rotation) || Double.IsInfinity(rotation))
            {
                rotation = 0;
            }

            _location     = location;
            _rotation     = rotation;
            _positions    = positions;
            _isRotateOnly = isRotateOnly;
        }

        #endregion

        #region Public Properties

        public bool HasPositions
        {
            get
            {
                return (_positions != null && _positions.Count != 0);
            }
        }

        public Point Location
        {
            get
            {
                return _location;
            }
        }

        public double Rotation
        {
            get
            {
                return _rotation;
            }
        }

        public bool IsRotateOnly
        {
            get
            {
                return _isRotateOnly;
            }
        }

        public IList<WpfTextPosition> Positions
        {
            get
            {
                return _positions;
            }
        }

        #endregion

        #region Public Methods

        public void UpdatePositions(string targetText)
        {
            //if (String.IsNullOrEmpty(targetText) || (_positions == null || _positions.Count == 0))
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
