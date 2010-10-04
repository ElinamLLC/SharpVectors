using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

namespace SharpVectors.Renderers.Texts
{
    public struct WpfTextStringFormat
    {
        public FlowDirection Direction;
        public TextTrimming Trimming;
        //public TextAlignment Alignment;
        public WpfTextAnchor Anchor;

        public WpfTextStringFormat(FlowDirection direction, TextTrimming trimming,
            WpfTextAnchor anchor)
        {
            this.Direction = direction;
            this.Trimming = trimming;
            this.Anchor = anchor;
        }

        public TextAlignment Alignment
        {
            get
            {
                if (Anchor == WpfTextAnchor.Middle)
                {
                    return TextAlignment.Center;
                }
                if (Anchor == WpfTextAnchor.End)
                {
                    return TextAlignment.Right;
                }

                return TextAlignment.Left;
            }
        }

        public static WpfTextStringFormat Default
        {
            get
            {
                return new WpfTextStringFormat(FlowDirection.LeftToRight,
                    TextTrimming.None, WpfTextAnchor.Start);
            }
        }
    }
}
