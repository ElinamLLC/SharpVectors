using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SharpVectors.Dom.Svg;

using static System.Net.Mime.MediaTypeNames;

namespace SharpVectors.Csss.Tests
{
    public sealed class TestSvgWindow : SvgWindow
    {
        public TestSvgWindow(long innerWidth, long innerHeight)
            : base(innerWidth, innerHeight, new TestSvgRenderer())
        {
            if (this.BaseUrls == null)
            {
                this.BaseUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public TestSvgWindow(long innerWidth, long innerHeight, ISvgRenderer renderer)
            : base(innerWidth, innerHeight, renderer)
        {
            if (this.BaseUrls == null)
            {
                this.BaseUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public TestSvgWindow(SvgWindow parentWindow, long innerWidth, long innerHeight)
            : base(parentWindow, innerWidth, innerHeight)
        {
        }

        public static TestSvgWindow Create()
        {
            return new TestSvgWindow(800, 600);
        }

        public override long InnerWidth
        {
            get
            {
                return base.InnerWidth;
            }
            set
            {
                base.InnerWidth = value;
            }
        }

        public override long InnerHeight
        {
            get
            {
                return base.InnerHeight;
            }
            set
            {
                base.InnerHeight = value;
            }
        }

        public override string Source
        {
            get
            {
                SvgDocument document = (SvgDocument)this.Document;
                return (document != null) ? document.Url : string.Empty;
            }
            set
            {
                Uri uri = new Uri(new Uri(Environment.CurrentDirectory), value);

                SvgDocument document = new SvgDocument(this);
                document.Load(uri.AbsoluteUri);

                this.Document = document;
            }
        }

        public override DirectoryInfo WorkingDir
        {
            get
            {
                return new DirectoryInfo(Environment.CurrentDirectory);
            }
        }

        public override void Alert(string message)
        {
        }

        public override ISvgRenderer CreateSvgRenderer()
        {
            return new TestSvgRenderer();
        }

        public override SvgWindow CreateOwnedWindow(long innerWidth, long innerHeight)
        {
            if (innerWidth == 0 || innerHeight == 0)
            {
                return new TestSvgWindow(this, this.InnerWidth, this.InnerHeight);
            }
            return new TestSvgWindow(this, innerWidth, innerHeight);
        }
    }

    public sealed class TestSvgRenderer : ISvgRenderer
    {
        private SvgRectF _invalidRect;
        private SvgWindow _window;

        public ISvgWindow Window { get => _window; set => _window = (SvgWindow)value; }
        public SvgRectF InvalidRect { get => _invalidRect; set => _invalidRect = value; }
        public RenderEvent OnRender { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TestSvgRenderer()
        {
        }


        public void DrawImage(object image, float x, float y, float width, float height)
        {
        }
        public void DrawImage(object image, float x, float y)
        {
        }
        public void DrawImage(object image, float x, float y, float width, float height, object attributes)
        {
        }
        public void DrawImage(object image, float x, float y, object attributes)
        {
        }
        public void Dispose()
        {
        }

        public void Render(ISvgElement node)
        {
            throw new NotImplementedException();
        }

        public void Render(ISvgDocument node)
        {
            throw new NotImplementedException();
        }

        public void InvalidateRect(SvgRectF rect)
        {
            throw new NotImplementedException();
        }

        public ISvgRect GetRenderedBounds(ISvgElement element, float margin)
        {
            throw new NotImplementedException();
        }
    }

}
