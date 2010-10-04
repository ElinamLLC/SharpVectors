using System;

using System.Windows;

namespace SharpVectors.Renderers.Texts
{
    public struct WpfTextPosition
    {
        private Point  _location;
        private double _rotation;
        
        public WpfTextPosition(Point location, double rotation)
        {
            if (Double.IsNaN(rotation) || Double.IsInfinity(rotation))
            {
                rotation = 0;
            }
            _location = location;
            _rotation = rotation;
        }

        public Point Location
        {
            get 
            { 
                return _location; 
            }
            set 
            { 
                _location = value; 
            }
        }

        public double Rotation
        {
            get 
            { 
                return _rotation; 
            }
            set 
            {
                if (Double.IsNaN(value) || Double.IsInfinity(value))
                {
                    _rotation = 0; 
                }
                else
                {
                    _rotation = value; 
                }
            }
        }
    }
}
