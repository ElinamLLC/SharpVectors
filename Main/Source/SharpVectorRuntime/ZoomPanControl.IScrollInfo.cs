// <copyright>
// This control is created by Ashley Davis and copyrighted under CPOL, and available as part of
// a CodeProject article at
//    http://www.codeproject.com/KB/WPF/zoomandpancontrol.aspx
// <date>This code is based on the article dated: 29 Jun 2010</date>
// </copyright>

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace SharpVectors.Runtime
{
    /// <summary>
    /// This is an extension to the ZoomPanControl class that implements
    /// the IScrollInfo interface properties and functions.
    /// 
    /// IScrollInfo is implemented to allow ZoomPanControl to be wrapped (in XAML)
    /// in a ScrollViewer. IScrollInfo allows the ScrollViewer and ZoomPanControl to 
    /// communicate important information such as the horizontal and vertical scrollbar offsets.
    /// 
    /// There is a good series of articles showing how to implement IScrollInfo starting here:
    ///     http://blogs.msdn.com/bencon/archive/2006/01/05/509991.aspx     
    /// </summary>
    public partial class ZoomPanControl
    {
        #region Public Properties

        /// <summary>
        /// Set to 'true' when the vertical scrollbar is enabled.
        /// </summary>
        public bool CanVerticallyScroll
        {
            get
            {
                return canVerticallyScroll;
            }
            set
            {
                canVerticallyScroll = value;
            }
        }

        /// <summary>
        /// Set to 'true' when the vertical scrollbar is enabled.
        /// </summary>
        public bool CanHorizontallyScroll
        {
            get
            {
                return canHorizontallyScroll;
            }
            set
            {
                canHorizontallyScroll = value;
            }
        }

        /// <summary>
        /// The width of the content (with 'ContentScale' applied).
        /// </summary>
        public double ExtentWidth
        {
            get
            {
                return unScaledExtent.Width * ContentScale;
            }
        }

        /// <summary>
        /// The height of the content (with 'ContentScale' applied).
        /// </summary>
        public double ExtentHeight
        {
            get
            {
                return unScaledExtent.Height * ContentScale;
            }
        }

        /// <summary>
        /// Get the width of the viewport onto the content.
        /// </summary>
        public double ViewportWidth
        {
            get
            {
                return viewport.Width;
            }
        }

        /// <summary>
        /// Get the height of the viewport onto the content.
        /// </summary>
        public double ViewportHeight
        {
            get
            {
                return viewport.Height;
            }
        }

        /// <summary>
        /// Reference to the ScrollViewer that is wrapped (in XAML) around the ZoomPanControl.
        /// Or set to null if there is no ScrollViewer.
        /// </summary>
        public ScrollViewer ScrollOwner
        {
            get
            {
                return scrollOwner;
            }
            set
            {
                scrollOwner = value;
            }
        }

        /// <summary>
        /// The offset of the horizontal scrollbar.
        /// </summary>
        public double HorizontalOffset
        {
            get
            {
                return ContentOffsetX * ContentScale;
            }
        }

        /// <summary>
        /// The offset of the vertical scrollbar.
        /// </summary>
        public double VerticalOffset
        {
            get
            {
                return ContentOffsetY * ContentScale;
            }
        }

        /// <summary>
        /// Called when the offset of the horizontal scrollbar has been set.
        /// </summary>
        public void SetHorizontalOffset(double offset)
        {
            if (disableScrollOffsetSync)
            {
                return;
            }

            try
            {
                disableScrollOffsetSync = true;

                ContentOffsetX = offset / ContentScale;
            }
            finally
            {
                disableScrollOffsetSync = false;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when the offset of the vertical scrollbar has been set.
        /// </summary>
        public void SetVerticalOffset(double offset)
        {
            if (disableScrollOffsetSync)
            {
                return;
            }

            try
            {
                disableScrollOffsetSync = true;

                ContentOffsetY = offset / ContentScale;
            }
            finally
            {
                disableScrollOffsetSync = false;
            }
        }

        /// <summary>
        /// Shift the content offset one line up.
        /// </summary>
        public void LineUp()
        {
            ContentOffsetY -= (ContentViewportHeight / 10);
        }

        /// <summary>
        /// Shift the content offset one line down.
        /// </summary>
        public void LineDown()
        {
            ContentOffsetY += (ContentViewportHeight / 10);
        }

        /// <summary>
        /// Shift the content offset one line left.
        /// </summary>
        public void LineLeft()
        {
            ContentOffsetX -= (ContentViewportWidth / 10);
        }

        /// <summary>
        /// Shift the content offset one line right.
        /// </summary>
        public void LineRight()
        {
            ContentOffsetX += (ContentViewportWidth / 10);
        }

        /// <summary>
        /// Shift the content offset one page up.
        /// </summary>
        public void PageUp()
        {
            ContentOffsetY -= ContentViewportHeight;
        }

        /// <summary>
        /// Shift the content offset one page down.
        /// </summary>
        public void PageDown()
        {
            ContentOffsetY += ContentViewportHeight;
        }

        /// <summary>
        /// Shift the content offset one page left.
        /// </summary>
        public void PageLeft()
        {
            ContentOffsetX -= ContentViewportWidth;
        }

        /// <summary>
        /// Shift the content offset one page right.
        /// </summary>
        public void PageRight()
        {
            ContentOffsetX += ContentViewportWidth;
        }

        /// <summary>
        /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
        /// used for zooming in and out, not for manipulating the scrollbars.
        /// </summary>
        public void MouseWheelDown()
        {
            if (IsMouseWheelScrollingEnabled)
            {
                LineDown();
            }
        }

        /// <summary>
        /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
        /// used for zooming in and out, not for manipulating the scrollbars.
        /// </summary>
        public void MouseWheelLeft()
        {
            if (IsMouseWheelScrollingEnabled)
            {
                LineLeft();
            }
        }

        /// <summary>
        /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
        /// used for zooming in and out, not for manipulating the scrollbars.
        /// </summary>
        public void MouseWheelRight()
        {
            if (IsMouseWheelScrollingEnabled)
            {
                LineRight();
            }
        }

        /// <summary>
        /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
        /// used for zooming in and out, not for manipulating the scrollbars.
        /// </summary>
        public void MouseWheelUp()
        {
            if (IsMouseWheelScrollingEnabled)
            {
                LineUp();
            }
        }

        /// <summary>
        /// Bring the specified rectangle to view.
        /// </summary>
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (content.IsAncestorOf(visual))
            {
                Rect transformedRect = visual.TransformToAncestor(content).TransformBounds(rectangle);
                if (!transformedRect.IntersectsWith(new Rect(ContentOffsetX, ContentOffsetY, ContentViewportWidth, ContentViewportHeight)))
                {
                    AnimatedSnapTo(new Point(transformedRect.X + (transformedRect.Width / 2), 
                        transformedRect.Y + (transformedRect.Height / 2)));
                }
            }
            return rectangle;
        }

        #endregion
    }
}
