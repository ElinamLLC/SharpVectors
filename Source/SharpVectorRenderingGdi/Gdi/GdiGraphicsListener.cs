using System;

namespace SharpVectors.Renderers.Gdi
{
    public abstract class GdiGraphicsListener : GdiGraphics
    {
        #region Private Fields

        private GdiHitTestHelper _hitTestHelper;

        #endregion

        #region Constructors and Destructor

        protected GdiGraphicsListener(bool isStatic)
            : base(isStatic)
        {
        }

        #endregion

        #region Public Properties

        public override GdiHitTestHelper HitTestHelper
        {
            get {
                return _hitTestHelper;
            }
            set {
                _hitTestHelper = value;
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_hitTestHelper != null)
            {
                _hitTestHelper.Dispose();
            }
            _hitTestHelper = null;

            base.Dispose(disposing);
        }

        #endregion
    }
}
