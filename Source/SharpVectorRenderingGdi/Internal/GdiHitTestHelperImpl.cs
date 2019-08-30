using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    internal sealed class GdiHitTestHelperImpl : GdiHitTestHelper
    {
        #region Private Fields

        /// <summary>
        /// A counter that tracks the next hit color.
        /// </summary>
        private int _colorCounter;

        /// <summary>
        /// Maps a 'hit color' to a graphics node.
        /// </summary>
        /// <remarks>
        /// The 'hit color' is an integer identifier that identifies the graphics node that drew it.  
        /// When 'hit colors' are drawn onto a bitmap (ie. <see cref="_idMapRaster">id-mapppe raster</see> 
        /// the 'hit color' of each pixel with the help of <see cref="_colorMap">color map</see> 
        /// can identify for a given x, y coordinate the relevant graphics node a mouse event should be dispatched to.
        /// </remarks>
        private IDictionary<Color, SvgElement> _colorMap;

        /// <summary>
        /// A bitmap image that consists of 'hit color' instead of visual color.  A 'hit color' is an 
        /// integer identifier that identifies the graphics node that drew it.  A 'hit color' can 
        /// therefore identify the graphics node that corresponds an x-y coordinates.
        /// </summary>
        private Bitmap _idMapRaster;

        #endregion

        #region Constructors and Destructor

        public GdiHitTestHelperImpl(int imageWidth, int imageHeight, PixelFormat format)
        {
            _colorCounter = 0;
            _colorMap = new Dictionary<Color, SvgElement>();
            _idMapRaster = new Bitmap(imageWidth, imageHeight, format);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the image map of the rendered Svg document. This
        /// is a picture of how the renderer will map the (x,y) positions
        /// of mouse events to objects. You can display this raster
        /// to help in debugging of hit testing.
        /// </summary>
        public Bitmap IdMapRaster
        {
            get {
                return _idMapRaster;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the topmost object of a hit test by specifying a Point.
        /// </summary>
        /// <param name="pointX">The X-value of the point to hit test against.</param>
        /// <param name="pointY">The Y-value of the point to hit test against.</param>
        /// <returns>The hit test result of the renderer, returned as a <see cref="GdiHitTestResult"/> type.</returns>
        public override GdiHitTestResult HitTest(int pointX, int pointY)
        {
            if (_idMapRaster == null || _idMapRaster.Width <= pointX || _idMapRaster.Height <= pointY)
            {
                return GdiHitTestResult.Empty;
            }

            Color pixel = _idMapRaster.GetPixel(pointX, pointY);
            SvgElement svgElement = GetElementFromColor(pixel);

            return new GdiHitTestResult(pointX, pointY, svgElement);
        }

        public void ClearMap()
        {
            _colorMap = null;
            _colorMap = new Dictionary<Color, SvgElement>();
        }

        /// <summary>
        /// Allocate a hit color for the specified graphics node.
        /// </summary>
        /// <param name="element">
        /// The <see cref="SvgElement">SvgElement</see> object for which to
        /// allocate a new hit color.
        /// </param>
        /// <returns>
        /// The hit color for the <see cref="SvgElement">SvgElement</see>
        /// object.
        /// </returns>
        public override Color GetNextHitColor(SvgElement element)
        {
            //	TODO: [newhoggy] It looks like there is a potential memory leak here.
            //	We only ever add to the graphicsNodes map, never remove
            //	from it, so it will grow every time this function is called.

            // The counter is used to generate IDs in the range [0,2^24-1]
            // The 24 bits of the counter are interpreted as follows:
            // [red 7 bits | green 7 bits | blue 7 bits |shuffle term 3 bits]
            // The shuffle term is used to define how the remaining high
            // bit is set on each color. The colors are generated in the
            // range [0,127] (7 bits) instead of [0,255]. Then the shuffle term
            // is used to adjust them into the range [0,255].
            // This algorithm has the feature that consecutive ids generate
            // visually distinct colors.
            int id = _colorCounter++; // Zero should be the first color.
            int shuffleTerm = id & 7;
            int r = 0x7f & (id >> 17);
            int g = 0x7f & (id >> 10);
            int b = 0x7f & (id >> 3);

            switch (shuffleTerm)
            {
                case 0: break;
                case 1: b |= 0x80; break;
                case 2: g |= 0x80; break;
                case 3: g |= 0x80; b |= 0x80; break;
                case 4: r |= 0x80; break;
                case 5: r |= 0x80; b |= 0x80; break;
                case 6: r |= 0x80; g |= 0x80; break;
                case 7: r |= 0x80; g |= 0x80; b |= 0x80; break;
            }

            Color color = Color.FromArgb(r, g, b);

            _colorMap.Add(color, element);

            return color;
        }

        public void RemoveHitColor(Color color)
        {
            if (!color.IsEmpty)
            {
                _colorMap[color] = null;
                _colorMap.Remove(color);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the <see cref="SvgElement">SvgElement</see> object that
        /// corresponds to the given hit color.
        /// </summary>
        /// <param name="color">
        /// The hit color for which to get the corresponding
        /// <see cref="SvgElement">SvgElement</see> object.
        /// </param>
        /// <remarks>
        /// Returns <c>null</c> if a corresponding
        /// <see cref="SvgElement">SvgElement</see> object cannot be
        /// found for the given hit color.
        /// </remarks>
        /// <returns>
        /// The <see cref="SvgElement">SvgElement</see> object that
        /// corresponds to the given hit color
        /// </returns>
        private SvgElement GetElementFromColor(Color color)
        {
            if (color.A == 0)
            {
                return null;
            }
            if (_colorMap.ContainsKey(color))
            {
                return _colorMap[color];
            }
            return null;
        }

        /// <summary>
        /// TODO: This method is not used.
        /// </summary>
        /// <param name="color">
        /// </param>
        /// <returns>
        /// </returns>
        private static int ColorToId(Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;
            int shuffleTerm = 0;

            if (0 != (r & 0x80))
            {
                shuffleTerm |= 4;
                r &= 0x7f;
            }

            if (0 != (g & 0x80))
            {
                shuffleTerm |= 2;
                g &= 0x7f;
            }

            if (0 != (b & 0x80))
            {
                shuffleTerm |= 1;
                b &= 0x7f;
            }

            return (r << 17) + (g << 10) + (b << 3) + shuffleTerm;
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_idMapRaster != null)
            {
                _idMapRaster.Dispose();
                _idMapRaster = null;
            }
        }

        #endregion
    }
}
