using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SharpVectors.Runtime
{
    /// <summary>
    /// This provides a wrapper for the Scoreboard, which is used for opacity animation.
    /// </summary>
    public sealed class SvgAnimator : FrameworkElement
    {
        #region Private Fields

        private bool            _isStarted;
        private string          _targetName;

        private Storyboard      _storyboard;
        private DoubleAnimation _opacityAnimation;

        #endregion

        #region Constructors and Destructor

        public SvgAnimator()
        {
            NameScope.SetNameScope(this, new NameScope());

            _opacityAnimation = new DoubleAnimation();
            _opacityAnimation.From           = 0.9;
            _opacityAnimation.To             = 0.0;
            _opacityAnimation.Duration       = new Duration(TimeSpan.FromSeconds(1));
            _opacityAnimation.AutoReverse    = false;
            _opacityAnimation.RepeatBehavior = RepeatBehavior.Forever;

            _storyboard = new Storyboard();

            _storyboard.Children.Add(_opacityAnimation);

            Storyboard.SetTargetProperty(_opacityAnimation, new PropertyPath(
                Brush.OpacityProperty));
        }

        #endregion

        #region Public Properties

        public bool IsAnimating
        {
            get 
            { 
                return _isStarted; 
            }
        }

        public string TargetName
        {
            get
            {
                return _targetName;
            }
        }

        #endregion

        #region Public Methods

        public void Start(string targetName, object scopedElement)
        {
            if (String.IsNullOrEmpty(targetName))
            {
                return;
            }

            if (_isStarted)
            {
                this.Stop();
            }
            this.RegisterName(targetName, scopedElement);

            Storyboard.SetTargetName(_opacityAnimation, targetName);

            _storyboard.Begin(this, true);

            _targetName = targetName;
            _isStarted  = true;
        }

        public void Stop()
        {
            _storyboard.Stop(this);

            if (!String.IsNullOrEmpty(_targetName))
            {
                this.UnregisterName(_targetName);
            }

            _isStarted  = false;
            _targetName = null;
        }

        #endregion
    }
}
