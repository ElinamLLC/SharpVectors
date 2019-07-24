using System;
using System.Drawing;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    public abstract class GdiHitTestHelper : IDisposable
    {
        #region Private Fields

        private bool _isDisposed; // To detect redundant calls

        #endregion

        #region Constructors and Destructor

        protected GdiHitTestHelper()
        {
        }

        ~GdiHitTestHelper()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public static GdiHitTestHelper NoHit
        {
            get {
                return new GdiHitTestHelperNoHit();
            }
        }

        public bool IsDisposed
        {
            get {
                return _isDisposed;
            }
            protected set {
                _isDisposed = value;
            }
        }

        #endregion

        #region Public Methods

        public virtual Color GetNextHitColor(SvgElement element)
        {
            return Color.White;
        }

        /// <summary>
        /// Returns the topmost object of a hit test by specifying a Point.
        /// </summary>
        /// <param name="pointX">The X-value of the point to hit test against.</param>
        /// <param name="pointY">The Y-value of the point to hit test against.</param>
        /// <returns>The hit test result of the renderer, returned as a <see cref="GdiHitTestResult"/> type.</returns>
        public abstract GdiHitTestResult HitTest(int pointX, int pointY);

        /// <summary>
        /// Returns the topmost object of a hit test by specifying a Point.
        /// </summary>
        /// <param name="point">The point value to hit test against.</param>
        /// <returns>The hit test result of the renderer, returned as a <see cref="GdiHitTestResult"/> type.</returns>
        public GdiHitTestResult HitTest(Point point)
        {
            return this.HitTest(point.X, point.Y);
        }

        #endregion

        #region IDisposable Members

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _isDisposed = true;
        }

        #endregion

        #region Private Inner Class

        private sealed class GdiHitTestHelperNoHit : GdiHitTestHelper
        {
            public GdiHitTestHelperNoHit()
            {
            }

            /// <summary>
            /// Returns the topmost object of a hit test by specifying a Point.
            /// </summary>
            /// <param name="pointX">The X-value of the point to hit test against.</param>
            /// <param name="pointY">The Y-value of the point to hit test against.</param>
            /// <returns>The hit test result of the renderer, returned as a <see cref="GdiHitTestResult"/> type.</returns>
            public override GdiHitTestResult HitTest(int pointX, int pointY)
            {
                return GdiHitTestResult.Empty;
            }
        }

        #endregion
    }
}
