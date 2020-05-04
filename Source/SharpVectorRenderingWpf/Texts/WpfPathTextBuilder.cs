using System;
using System.Diagnostics;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;

using SharpVectors.Renderers.Utils;

namespace SharpVectors.Renderers.Texts
{
    /// <summary>
    /// This is the text-path builder, which handles the rendering using the text-builder objects, 
    /// <see cref="WpfTextBuilder"/>, to convert the text characters to geometric representations.
    /// </summary>
    public sealed class WpfPathTextBuilder
    {
        #region Private Fields

        private bool _endsWhitespace;

        private double _pathLength;
        private double _textLength;

        private IList<WpfPathChar> _pathChars;

        private SvgTextBaseElement _textElement;
        private SvgTextPathElement _textPathElement;

        private IList<WpfPathTextRun> _pathTextRuns;

        #endregion

        #region Constructors and Destructor

        public WpfPathTextBuilder(SvgTextBaseElement textElement)
        {
            _textElement = textElement;
            _pathChars    = new List<WpfPathChar>();
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        public void BeginTextPath(SvgTextPathElement svgElement)
        {
            _textPathElement = svgElement;

            if (_pathChars == null || _pathChars.Count != 0)
            {
                _pathChars = new List<WpfPathChar>();
            }
            if (_pathTextRuns == null || _pathTextRuns.Count != 0)
            {
                _pathTextRuns = new List<WpfPathTextRun>();
            }

            _textLength = 0;
            _pathLength = 0;
        }

        public void AddTextRun(WpfPathTextRun pathTextRun, string text, Point textPos, Brush textBrush, Pen textPen)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            if (_pathChars == null)
            {
                _pathChars = new List<WpfPathChar>();
            }

            if (_endsWhitespace)
            {
                text = text.TrimStart();
                _endsWhitespace = text.EndsWith(" ", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                _endsWhitespace = text.EndsWith(" ", StringComparison.OrdinalIgnoreCase);
            }

            pathTextRun.Initialize(text, textBrush, textPen);
            pathTextRun.SetPosition(textPos, _textPathElement, _textElement);

            WpfTextBuilder contentBuilder = pathTextRun.Builder;
            SvgTextContentElement contentElement = pathTextRun.Element;

            foreach (char ch in text)
            {
                var textChar = ch.ToString();
                var textSize = contentBuilder.MeasureText(contentElement, textChar, true);

                _textLength += textSize.Width;

                _pathChars.Add(new WpfPathChar(textChar, textPos, textSize, _pathTextRuns.Count));
            }

            _pathTextRuns.Add(pathTextRun);
        }

        public void EndTextPath()
        {
            _pathChars     = null;
            _pathTextRuns = null;
        }

        public void RenderTextPath(DrawingContext dc, PathGeometry pathGeometry, TextAlignment pathAlignment)
        {
            if (_pathChars == null)
            {
                throw new InvalidOperationException();
            }
            if (_pathChars.Count == 0)
            {
                return;
            }

            //ISvgAnimatedLength pathOffset  = _svgElement.StartOffset;
            SvgTextPathMethod pathMethod   = (SvgTextPathMethod)_textPathElement.Method.BaseVal;
            SvgTextPathSpacing pathSpacing = (SvgTextPathSpacing)_textPathElement.Spacing.BaseVal;

            ISvgAnimatedLength pathOffset  = this.GetPathOffset();

            if (pathOffset != null && pathOffset.AnimVal.Value < 0)
            {
                this.RenderEndAlignedText(dc, pathGeometry, pathOffset);
            }
            else
            {
                if (pathAlignment == TextAlignment.Right)
                {
                    this.RenderEndAlignedText(dc, pathGeometry, pathOffset);
                }
                else
                {
                    this.RenderStartAlignedText(dc, pathGeometry, pathOffset, pathAlignment);
                }
            }
        }

        #endregion

        #region Private Methods

        private void RenderStartAlignedText(DrawingContext dc, PathGeometry svgPath,
            ISvgAnimatedLength startOffset, TextAlignment alignment)
        {
            if (svgPath == null || svgPath.Figures == null || svgPath.Figures.Count != 1)
            {
                Debug.Assert(false, "Monitor invalid path cases in debug mode!");
                return;
            }

            var pathFigure = svgPath.Figures[0];

            _pathLength = WpfConvert.GetPathFigureLength(pathFigure);
            if (_pathLength.Equals(0) || _textLength.Equals(0))
                return;

            //double scalingFactor = pathLength / textLength;
            double scalingFactor = 1.0; // Not scaling the text to fit...
            double progress = 0;
            if (startOffset != null && startOffset.AnimVal != null)
            {
                ISvgLength offsetLength = startOffset.AnimVal;
                switch (offsetLength.UnitType)
                {
                        // If a percentage is given, then the startOffset represents a 
                        // percentage distance along the entire path.
                    case SvgLengthType.Percentage:
                        if (!offsetLength.ValueInSpecifiedUnits.Equals(0))
                        {
                            progress += offsetLength.ValueInSpecifiedUnits / 100d;
                        }
                        break;
                        // If a length other than a percentage is given, then the startOffset represents a distance along 
                        // the path measured in the current user coordinate system.
                    case SvgLengthType.Number:
                        progress += offsetLength.ValueInSpecifiedUnits / _pathLength;
                        break;
                }
            }
            PathGeometry pathGeometry = new PathGeometry(new PathFigure[] { pathFigure });

            Point ptPrevPos = new Point(0, 0);

            double letterSpacing = 0;
            if (_textElement.LetterSpacing != null && _textElement.LetterSpacing.AnimVal != null)
            {
                letterSpacing = _textElement.LetterSpacing.AnimVal.ValueInSpecifiedUnits;
            }

            _pathLength += letterSpacing;

            var lengthFactor = 1.0;

            var textLength = this.GetTextLength();

            if (textLength != null && textLength.AnimVal != null)
            {
                var textLengthValue = textLength.AnimVal.ValueInSpecifiedUnits;
                if (textLengthValue > 0)
                {
                    lengthFactor = textLengthValue / _pathLength;
//                    lengthFactor = _textLength / textLengthValue;
//                    lengthFactor = textLengthValue / _textLength;
//                    lengthFactor = _pathLength / textLengthValue;
                    if (lengthFactor < 0.5) // Check for abuse!
                    {
                        lengthFactor = 1;
                    }
                }
            }

            int charCount = _pathChars.Count;
            for (int i = 0; i < charCount; i++)
            {
                var pathChar = _pathChars[i];

                var pathTextRun = _pathTextRuns[pathChar.Index];
                var textBuilder = pathTextRun.Builder;

                double width    = (scalingFactor * pathChar.Width * lengthFactor + letterSpacing);
                double baseline = scalingFactor * textBuilder.Baseline;

                progress += width / 2 / _pathLength;
                if (progress > 1)
                {
                    break;
                }
                Point ptNextPos, ptTangent;

                pathGeometry.GetPointAtFractionLength(progress, out ptNextPos, out ptTangent);

                if (i != 0 && AreEqual(ptNextPos, ptPrevPos))
                {
                    break;
                }

                var textPath = textBuilder.Build(pathTextRun.Element, pathChar.Text, pathChar.X, pathChar.Y);

                var textTransform = new TransformGroup();
                textTransform.Children.Add(new RotateTransform(Math.Atan2(ptTangent.Y, ptTangent.X) * 180 / Math.PI, width / 2, baseline));
                textTransform.Children.Add(new TranslateTransform(ptNextPos.X - width / 2, ptNextPos.Y - baseline));

                var curTransform = textPath.Transform;
                if (curTransform != null && curTransform.Value.IsIdentity == false)
                {
                    textTransform.Children.Add(curTransform);
                }

                textPath.Transform = new MatrixTransform(textTransform.Value);

                pathTextRun.AddRun(textPath);

                progress += width / 2 / _pathLength;

                ptPrevPos = ptNextPos;
            }

            foreach (var pathTextRun in _pathTextRuns)
            {
                pathTextRun.RenderRun(dc);
            }
        }

        private void RenderEndAlignedText(DrawingContext dc, PathGeometry svgPath, ISvgAnimatedLength startOffset)
        {
            if (svgPath == null || svgPath.Figures == null || svgPath.Figures.Count != 1)
            {
                Debug.Assert(false, "Monitor invalid path cases in debug mode!");
                return;
            }

            var pathFigure = svgPath.Figures[0];

            _pathLength = WpfConvert.GetPathFigureLength(pathFigure);
            if (_pathLength.Equals(0) || _textLength.Equals(0))
                return;

            double scalingFactor = 1.0; // Not scaling the text to fit...
            double progress = 1.0;
            if (startOffset != null && startOffset.AnimVal != null)
            {
                ISvgLength offsetLength = startOffset.AnimVal;
                if (offsetLength.Value < 0)
                {
                    progress = 0;
                }

                switch (offsetLength.UnitType)
                {
                    // If a percentage is given, then the startOffset represents a 
                    // percentage distance along the entire path.
                    case SvgLengthType.Percentage:
                        if (!offsetLength.ValueInSpecifiedUnits.Equals(0))
                        {
                            progress += offsetLength.ValueInSpecifiedUnits / 100d;
                        }
                        break;
                    // If a length other than a percentage is given, then the startOffset represents a distance along 
                    // the path measured in the current user coordinate system.
                    case SvgLengthType.Number:
                        progress += offsetLength.ValueInSpecifiedUnits / _pathLength;
                        break;
                }
            }
            if (progress < 0)
            {
                progress *= -1;
            }

            if (progress > 1)
            {
                progress = progress - 1;
                while (progress > 1)
                {
                    progress = progress - 1;
                }
            }
            PathGeometry pathGeometry = new PathGeometry(new PathFigure[] { pathFigure });

            Point ptPrevPos = new Point(0, 0);

            int charCount = _pathChars.Count - 1;

            for (int i = charCount; i >= 0; i--)
            {
                var pathChar = _pathChars[i];

                var pathTextRun = _pathTextRuns[pathChar.Index];
                var textBuilder  = pathTextRun.Builder;

                double width = scalingFactor * pathChar.Width;
                double baseline = scalingFactor * textBuilder.Baseline;

                progress -= width / 2 / _pathLength;
                if (progress < 0)
                {
                    break;
                }
                Point ptNext, ptTangent;

                pathGeometry.GetPointAtFractionLength(progress, out ptNext, out ptTangent);

                if (i != charCount && AreEqual(ptNext, ptPrevPos))
                {
                    break;
                }

                var textPath = textBuilder.Build(pathTextRun.Element, pathChar.Text, pathChar.X, pathChar.Y);

                var textTransform = new TransformGroup();
                textTransform.Children.Add(new RotateTransform(Math.Atan2(ptTangent.Y, ptTangent.X) * 180 / Math.PI, width / 2, baseline));
                textTransform.Children.Add(new TranslateTransform(ptNext.X - width / 2, ptNext.Y - baseline));

                var curTransform = textPath.Transform;
                if (curTransform != null && curTransform.Value.IsIdentity == false)
                {
                    //Debug.Assert(false, "TODO: Wish to see the case");
                    textTransform.Children.Add(curTransform);
                }

                textPath.Transform = new MatrixTransform(textTransform.Value);

                pathTextRun.AddRun(textPath);

                progress -= width / 2 / _pathLength;

                ptPrevPos = ptNext;
            }

            foreach (var pathTextRun in _pathTextRuns)
            {
                pathTextRun.RenderRun(dc);
            }
        }

        private ISvgAnimatedLength GetTextLength()
        {
            if (_textPathElement.TextLength != null && _textPathElement.TextLength.AnimVal != null)
            {
                var textLength = _textPathElement.TextLength;
                if (textLength.AnimVal.Value > 0)
                {
                    return textLength;
                }
            }
            if (_textElement.TextLength != null && _textElement.TextLength.AnimVal != null)
            {
                var textLength = _textElement.TextLength;
                if (textLength.AnimVal.Value > 0)
                {
                    return textLength;
                }
            }
            return null;
        }

        private ISvgAnimatedLength GetPathOffset()
        {
            ISvgAnimatedLength pathOffset = _textPathElement.StartOffset;
            if (pathOffset != null && pathOffset.AnimVal != null)
            {
                if (pathOffset.AnimVal.Value.Equals(0))
                {
                    var curOffset = _pathTextRuns[0].StartOffset;
                    if (curOffset != null && curOffset.AnimVal != null)
                    {
                        if (!curOffset.AnimVal.Value.Equals(0))
                        {
                            return curOffset;
                        }
                    }
                }
            }

            return pathOffset;
        }

        private const double EqualPointsComparer = 0.0001;
        private static bool AreEqual(Point pt1, Point pt2)
        {
            if (pt1 == pt2)
            {
                return true;
            }
            return Math.Abs(pt1.X - pt2.X) < EqualPointsComparer && Math.Abs(pt1.Y - pt2.Y) < EqualPointsComparer;
        }

        #endregion

        #region WpfPathChar Class

        private sealed class WpfPathChar
        {
            public WpfPathChar(string text, Point pos, Size size, int index)
            {
                this.Text  = text;
                this.Width = size.Width;
                this.X     = pos.X;
                this.Y     = pos.Y;
                this.Index = index;
            }
            
            public int Index;

            public double X;
            public double Y;    

            public string Text;

            public double Width;
        }

        #endregion
    }
}
