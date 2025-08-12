using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Renderers.Texts
{
    public sealed class WpfHorzTextRenderer : WpfTextRenderer
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public WpfHorzTextRenderer(SvgTextBaseElement textElement, WpfTextRendering textRendering)
            : base(textElement, textRendering)
        {
        }

        #endregion

        #region Public Methods

        public override void RenderText(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            double emSize      = GetComputedFontSize(element);
            var fontFamilyInfo = GetTextFontFamilyInfo(element);

            WpfTextStringFormat stringFormat = GetTextStringFormat(element);

            if (fontFamilyInfo.FontFamilyType == WpfFontFamilyType.Svg ||
                fontFamilyInfo.FontFamilyType == WpfFontFamilyType.Private)
            {
                WpfTextTuple textInfo = new WpfTextTuple(fontFamilyInfo, emSize, stringFormat, element);
                this.RenderText(textInfo, ref ctp, text, rotate, placement);
                return;
            }

            FontFamily fontFamily   = fontFamilyInfo.Family;
            FontStyle fontStyle     = fontFamilyInfo.Style;
            FontWeight fontWeight   = fontFamilyInfo.Weight;
            FontStretch fontStretch = fontFamilyInfo.Stretch;

            // Fix the use of Postscript fonts...
            WpfFontFamilyVisitor fontFamilyVisitor = _context.FontFamilyVisitor;
            if (!string.IsNullOrWhiteSpace(_actualFontName) && fontFamilyVisitor != null)
            {
                WpfFontFamilyInfo familyInfo = fontFamilyVisitor.Visit(_actualFontName,
                    fontFamilyInfo, _context);
                if (familyInfo != null && !familyInfo.IsEmpty)
                {
                    fontFamily  = familyInfo.Family;
                    fontWeight  = familyInfo.Weight;
                    fontStyle   = familyInfo.Style;
                    fontStretch = familyInfo.Stretch;
                }
            }

            WpfSvgPaint fillPaint   = new WpfSvgPaint(_context, element, "fill");
            Brush textBrush = fillPaint.GetBrush();

            WpfSvgPaint strokePaint = new WpfSvgPaint(_context, element, "stroke");
            Pen textPen = strokePaint.GetPen();

            if (textBrush == null && textPen == null)
            {
                return;
            }
            bool isForcedPathMode = false; 
            if (textBrush == null)
            {
                // If here, then the pen is not null, and so the fill cannot be null.
                // We set this to transparent for stroke only text path...
                textBrush = Brushes.Transparent;
            }
            else
            {
                // WPF gradient fill does not work well on text, use geometry to render it
                isForcedPathMode = (fillPaint.FillType == WpfFillType.Gradient);
            }
            if (textPen != null)
            {
                textPen.LineJoin = PenLineJoin.Round; // Better for text rendering
                isForcedPathMode = true;
            }

            TextDecorationCollection textDecors = GetTextDecoration(element);
            TextAlignment alignment = stringFormat.Alignment;

            bool hasWordSpacing  = false;
            string wordSpaceText = element.GetAttribute("word-spacing");
            double wordSpacing   = 0;
            if (!string.IsNullOrWhiteSpace(wordSpaceText) &&
                double.TryParse(wordSpaceText, out wordSpacing) && !wordSpacing.Equals(0))
            {
                hasWordSpacing = true;
            }

            bool hasLetterSpacing = false;
            string letterSpaceText = element.GetAttribute("letter-spacing");
            double letterSpacing = 0;
            if (!string.IsNullOrWhiteSpace(letterSpaceText) && 
                double.TryParse(letterSpaceText, out letterSpacing) && !letterSpacing.Equals(0))
            {
                hasLetterSpacing = true;
            }

            bool isRotatePosOnly = false;

            IList<WpfTextPosition> textPositions = null;
            int textPosCount = 0;
            if ((placement != null && placement.HasPositions))
            {
                textPositions   = placement.Positions;
                textPosCount    = textPositions.Count;
                isRotatePosOnly = placement.IsRotateOnly;
            }

            var typeFace = new Typeface(fontFamily, fontStyle, fontWeight, fontStretch);

            //bool isRightToLeft = false;
            //var xmlLang = _textElement.XmlLang;
            //if (!string.IsNullOrWhiteSpace(xmlLang))
            //{
            //    if (string.Equals(xmlLang, "ar", StringComparison.OrdinalIgnoreCase)      // Arabic language
            //        || string.Equals(xmlLang, "he", StringComparison.OrdinalIgnoreCase))  // Hebrew language
            //    {
            //        isRightToLeft = true;
            //    }
            //}

            if (hasLetterSpacing || hasWordSpacing || textPositions != null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    var nextText = new string(text[i], 1);

#if DOTNET40 || DOTNET45 || DOTNET46
                    FormattedText formattedText = new FormattedText(nextText, 
                        _context.CultureInfo, stringFormat.Direction, typeFace, emSize, textBrush);
#else
                    FormattedText formattedText = new FormattedText(nextText, _context.CultureInfo,
                        stringFormat.Direction, typeFace, emSize, textBrush, _context.PixelsPerDip);
#endif

//                    formattedText.FlowDirection = isRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                    formattedText.TextAlignment = stringFormat.Alignment;
                    formattedText.Trimming      = stringFormat.Trimming;

                    if (textDecors != null && textDecors.Count != 0)
                    {
                        formattedText.SetTextDecorations(textDecors);
                    }

                    WpfTextPosition? textPosition = null;
                    if (textPositions != null && i < textPosCount)
                    {
                        textPosition = textPositions[i];
                    }

                    //float xCorrection = 0;
                    //if (alignment == TextAlignment.Left)
                    //    xCorrection = emSize * 1f / 6f;
                    //else if (alignment == TextAlignment.Right)
                    //    xCorrection = -emSize * 1f / 6f;

                    double yCorrection = formattedText.Baseline;

                    float rotateAngle = (float)rotate;
                    if (textPosition != null)
                    {
                        if (!isRotatePosOnly)
                        {
                            Point pt = textPosition.Value.Location;
                            ctp.X = pt.X;
                            ctp.Y = pt.Y;
                        }
                        rotateAngle = (float)textPosition.Value.Rotation;
                    }
                    Point textStart = ctp;

                    RotateTransform rotateAt = null;
                    if (!rotateAngle.Equals(0))
                    {
                        rotateAt = new RotateTransform(rotateAngle, textStart.X, textStart.Y);
                        _drawContext.PushTransform(rotateAt);
                    }

                    Point textPoint = new Point(textStart.X, textStart.Y - yCorrection);

                    if (_context.TextAsGeometry || isForcedPathMode)
                    {
                        //Geometry textGeometry = formattedText.BuildGeometry(textPoint);
                        Geometry textGeometry = null;
                        if (stringFormat.Dominant != DominantBaseline.Auto
                            && stringFormat.Dominant != DominantBaseline.Alphabetic)
                        {
                            textGeometry = this.BuildGeometryWithDominant(formattedText, textPoint, typeFace, emSize, stringFormat.Dominant);
                        }
                        else
                        {
                            textGeometry = formattedText.BuildGeometry(textPoint);
                        }

                        if (textGeometry != null && !textGeometry.IsEmpty())
                        {
                            _drawContext.DrawGeometry(textBrush, textPen, ExtractTextPathGeometry(textGeometry));

                            this.IsTextPath = true;
                        }
                        else
                        {
                            if (stringFormat.Dominant != DominantBaseline.Auto
                                && stringFormat.Dominant != DominantBaseline.Alphabetic)
                            {
                                this.RenderTextWithBaseline(_drawContext, formattedText, textPoint,
                                    typeFace, emSize, stringFormat.Dominant);
                            }
                            else
                            {
                                _drawContext.DrawText(formattedText, textPoint);
                            }
                            //_drawContext.DrawText(formattedText, textPoint);
                        }
                    }
                    else
                    {
                        if (stringFormat.Dominant != DominantBaseline.Auto
                            && stringFormat.Dominant != DominantBaseline.Alphabetic)
                        {
                            this.RenderTextWithBaseline(_drawContext, formattedText, textPoint,
                                typeFace, emSize, stringFormat.Dominant);
                        }
                        else
                        {
                            _drawContext.DrawText(formattedText, textPoint);
                        }
                        //_drawContext.DrawText(formattedText, textPoint);
                    }

                    //float bboxWidth = (float)formattedText.Width;
                    double bboxWidth = formattedText.WidthIncludingTrailingWhitespace;
                    if (alignment == TextAlignment.Center)
                        bboxWidth /= 2f;
                    else if (alignment == TextAlignment.Right)
                        bboxWidth = 0;

                    //ctp.X += bboxWidth + emSize / 4 + spacing;
                    if (hasLetterSpacing)
                    {
                        ctp.X += bboxWidth + letterSpacing;
                    }
                    if (hasWordSpacing && char.IsWhiteSpace(text[i]))
                    {
                        if (hasLetterSpacing)
                        {
                            ctp.X += wordSpacing;
                        }
                        else
                        {
                            ctp.X += bboxWidth + wordSpacing;
                        }
                    }
                    else
                    {
                        if (!hasLetterSpacing)
                        {
                            ctp.X += bboxWidth;
                        }
                    }

                    if (rotateAt != null)
                    {
                        _drawContext.Pop();
                    }
                }
            }
            else
            {
#if DOTNET40 || DOTNET45 || DOTNET46
                FormattedText formattedText = new FormattedText(text, _context.CultureInfo,
                    stringFormat.Direction, typeFace, emSize, textBrush);
#else
                FormattedText formattedText = new FormattedText(text, _context.CultureInfo,
                    stringFormat.Direction, typeFace, emSize, textBrush, _context.PixelsPerDip);
#endif

                formattedText.TextAlignment = stringFormat.Alignment;
                formattedText.Trimming      = stringFormat.Trimming;

//                formattedText.FlowDirection = isRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

                if (textDecors != null && textDecors.Count != 0)
                {
                    formattedText.SetTextDecorations(textDecors);
                }

                //float xCorrection = 0;
                //if (alignment == TextAlignment.Left)
                //    xCorrection = emSize * 1f / 6f;
                //else if (alignment == TextAlignment.Right)
                //    xCorrection = -emSize * 1f / 6f;

                double yCorrection = formattedText.Baseline;

                float rotateAngle = (float)rotate;
                Point textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                RotateTransform rotateAt = null;
                if (!rotateAngle.Equals(0))
                {
                    rotateAt = new RotateTransform(rotateAngle, ctp.X, ctp.Y);
                    _drawContext.PushTransform(rotateAt);
                }

                if (_context.TextAsGeometry || isForcedPathMode)
                {
                    // Geometry textGeometry = formattedText.BuildGeometry(textPoint);
                    Geometry textGeometry = null;
                    if (stringFormat.Dominant != DominantBaseline.Auto
                        && stringFormat.Dominant != DominantBaseline.Alphabetic)
                    {
                        textGeometry = this.BuildGeometryWithDominant(formattedText, textPoint, typeFace, emSize, stringFormat.Dominant);
                    }
                    else
                    {
                        textGeometry = formattedText.BuildGeometry(textPoint);
                    }

                    if (textGeometry != null && !textGeometry.IsEmpty())
                    {
                        _drawContext.DrawGeometry(textBrush, textPen, 
                            ExtractTextPathGeometry(textGeometry));

                        this.IsTextPath = true;
                    }
                    else
                    {
                        if (stringFormat.Dominant != DominantBaseline.Auto
                            && stringFormat.Dominant != DominantBaseline.Alphabetic)
                        {
                            this.RenderTextWithBaseline(_drawContext, formattedText, textPoint,
                                typeFace, emSize, stringFormat.Dominant);
                        }
                        else
                        {
                            _drawContext.DrawText(formattedText, textPoint);
                        }
                        //_drawContext.DrawText(formattedText, textPoint);
                    }
                }
                else
                {
                    if (stringFormat.Dominant != DominantBaseline.Auto
                        && stringFormat.Dominant != DominantBaseline.Alphabetic)
                    {
                        this.RenderTextWithBaseline(_drawContext, formattedText, textPoint,
                            typeFace, emSize, stringFormat.Dominant);
                    }
                    else
                    {
                        _drawContext.DrawText(formattedText, textPoint);
                    }
                    //_drawContext.DrawText(formattedText, textPoint);
                }

                //float bboxWidth = (float)formattedText.Width;
                double bboxWidth = formattedText.WidthIncludingTrailingWhitespace;
                if (alignment == TextAlignment.Center)
                {
                    bboxWidth /= 2f;
                }
                else if (alignment == TextAlignment.Right)
                {
                    bboxWidth = 0;
                }

                //ctp.X += bboxWidth + emSize / 4;
                ctp.X += bboxWidth;

                if (rotateAt != null)
                {
                    _drawContext.Pop();
                }
            }
        }

        public override void RenderTextRun(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            double emSize      = GetComputedFontSize(element);
            var fontFamilyInfo = GetTextFontFamilyInfo(element);

            WpfTextStringFormat stringFormat = GetTextStringFormat(element);

            if (fontFamilyInfo.FontFamilyType == WpfFontFamilyType.Svg)
            {
                WpfTextTuple textInfo = new WpfTextTuple(fontFamilyInfo, emSize, stringFormat, element);
                this.RenderTextRun(textInfo, ref ctp, text, rotate, placement);
                return;
            }

            FontFamily fontFamily   = fontFamilyInfo.Family;
            FontStyle fontStyle     = fontFamilyInfo.Style;
            FontWeight fontWeight   = fontFamilyInfo.Weight;
            FontStretch fontStretch = fontFamilyInfo.Stretch;

            // Fix the use of Postscript fonts...
            WpfFontFamilyVisitor fontFamilyVisitor = _context.FontFamilyVisitor;
            if (!string.IsNullOrWhiteSpace(_actualFontName) && fontFamilyVisitor != null)
            {
                WpfFontFamilyInfo familyInfo = fontFamilyVisitor.Visit(_actualFontName,
                    fontFamilyInfo, _context);
                if (familyInfo != null && !familyInfo.IsEmpty)
                {
                    fontFamily  = familyInfo.Family;
                    fontWeight  = familyInfo.Weight;
                    fontStyle   = familyInfo.Style;
                    fontStretch = familyInfo.Stretch;
                }
            }

            WpfSvgPaint fillPaint = new WpfSvgPaint(_context, element, "fill");
            Brush textBrush = fillPaint.GetBrush();

            WpfSvgPaint strokePaint = new WpfSvgPaint(_context, element, "stroke");
            Pen textPen = strokePaint.GetPen();

            if (textBrush == null && textPen == null)
            {
                return;
            }
            bool isForcedPathMode = false;
            if (textBrush == null)
            {
                // If here, then the pen is not null, and so the fill cannot be null.
                // We set this to transparent for stroke only text path...
                textBrush = Brushes.Transparent;
            }
            else
            {
                // WPF gradient fill does not work well on text, use geometry to render it
                isForcedPathMode = (fillPaint.FillType == WpfFillType.Gradient);
            }
            if (textPen != null)
            {
                textPen.LineJoin = PenLineJoin.Round; // Better for text rendering
                isForcedPathMode = true;
            }

            TextDecorationCollection textDecors = GetTextDecoration(element);
            if (textDecors == null)
            {
                SvgTextBaseElement textElement = element.ParentNode as SvgTextBaseElement;

                if (textElement != null)
                {
                    textDecors = GetTextDecoration(textElement);
                }
            }

            TextAlignment alignment = stringFormat.Alignment;

            bool hasWordSpacing  = false;
            string wordSpaceText = element.GetAttribute("word-spacing");
            double wordSpacing   = 0;
            if (!string.IsNullOrWhiteSpace(wordSpaceText) &&
                double.TryParse(wordSpaceText, out wordSpacing) && !wordSpacing.Equals(0))
            {
                hasWordSpacing = true;
            }

            bool hasLetterSpacing  = false;
            string letterSpaceText = element.GetAttribute("letter-spacing");
            double letterSpacing   = 0;
            if (!string.IsNullOrWhiteSpace(letterSpaceText) &&
                double.TryParse(letterSpaceText, out letterSpacing) && !letterSpacing.Equals(0))
            {
                hasLetterSpacing = true;
            }

            bool isRotatePosOnly = false;

            IList<WpfTextPosition> textPositions = null;
            int textPosCount = 0;
            if ((placement != null && placement.HasPositions))
            {
                textPositions   = placement.Positions;
                textPosCount    = textPositions.Count;
                isRotatePosOnly = placement.IsRotateOnly;
            }

            var typeFace = new Typeface(fontFamily, fontStyle, fontWeight, fontStretch);

            if (textPositions != null && textPositions.Count != 0)
            {
                if (textPositions.Count == text.Trim().Length) //TODO: Best way to handle this...
                {
                    text = text.Trim();
                }
            }

            if (hasLetterSpacing || hasWordSpacing || textPositions != null)
            {
                double spacing = Convert.ToDouble(letterSpacing);

                int startSpaces = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (char.IsWhiteSpace(text[i]))
                    {
                        startSpaces++;
                    }
                    else
                    {
                        break;
                    }
                }

                int j = 0;

                string inputText = string.Empty;
                for (int i = 0; i < text.Length; i++)
                {
                    // Avoid rendering only spaces at the start of text run...
                    if (i == 0 && startSpaces != 0)
                    {
                        inputText = text.Substring(0, startSpaces + 1);
                        i += startSpaces;
                    }
                    else
                    {
                        inputText = new string(text[i], 1);
                    }

#if DOTNET40 || DOTNET45 || DOTNET46
                    FormattedText formattedText = new FormattedText(inputText,  _context.CultureInfo, 
                        stringFormat.Direction, typeFace, emSize, textBrush);
#else
                    FormattedText formattedText = new FormattedText(inputText, _context.CultureInfo,
                        stringFormat.Direction, typeFace, emSize, textBrush, _context.PixelsPerDip);
#endif

                    if (this.IsMeasuring)
                    {
                        this.AddTextWidth(ctp, formattedText.WidthIncludingTrailingWhitespace); 
                        continue;
                    }

                    formattedText.Trimming      = stringFormat.Trimming;
                    formattedText.TextAlignment = stringFormat.Alignment;

                    if (textDecors != null && textDecors.Count != 0)
                    {
                        formattedText.SetTextDecorations(textDecors);
                    }

                    WpfTextPosition? textPosition = null;
                    if (textPositions != null && j < textPosCount)
                    {
                        textPosition = textPositions[j];
                    }

                    //float xCorrection = 0;
                    //if (alignment == TextAlignment.Left)
                    //    xCorrection = emSize * 1f / 6f;
                    //else if (alignment == TextAlignment.Right)
                    //    xCorrection = -emSize * 1f / 6f;

                    double yCorrection = formattedText.Baseline;

                    float rotateAngle = (float)rotate;
                    if (textPosition != null)
                    {
                        if (!isRotatePosOnly)
                        {
                            Point pt = textPosition.Value.Location;
                            ctp.X = pt.X;
                            ctp.Y = pt.Y;
                        }
                        rotateAngle = (float)textPosition.Value.Rotation;
                    }
                    Point textStart = ctp;

                    RotateTransform rotateAt = null;
                    if (!rotateAngle.Equals(0))
                    {
                        rotateAt = new RotateTransform(rotateAngle, textStart.X, textStart.Y);
                        _drawContext.PushTransform(rotateAt);
                    }

                    Point textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                    if (isForcedPathMode || _context.TextAsGeometry)
                    {
                        //Geometry textGeometry = formattedText.BuildGeometry(textPoint);
                        Geometry textGeometry = null;
                        if (stringFormat.Dominant != DominantBaseline.Auto
                            && stringFormat.Dominant != DominantBaseline.Alphabetic)
                        {
                            textGeometry = this.BuildGeometryWithDominant(formattedText, textPoint, typeFace, emSize, stringFormat.Dominant);
                        }
                        else
                        {
                            textGeometry = formattedText.BuildGeometry(textPoint);
                        }

                        if (textGeometry != null && !textGeometry.IsEmpty())
                        {
                            _drawContext.DrawGeometry(textBrush, textPen,
                                ExtractTextPathGeometry(textGeometry));

                            this.IsTextPath = true;
                        }
                        else
                        {
                            if (stringFormat.Dominant != DominantBaseline.Auto
                                && stringFormat.Dominant != DominantBaseline.Alphabetic)
                            {
                                this.RenderTextWithBaseline(_drawContext, formattedText, textPoint,
                                    typeFace, emSize, stringFormat.Dominant);
                            }
                            else
                            {
                                _drawContext.DrawText(formattedText, textPoint);
                            }
                            //_drawContext.DrawText(formattedText, textPoint);
                        }
                    }
                    else
                    {
                        if (stringFormat.Dominant != DominantBaseline.Auto
                            && stringFormat.Dominant != DominantBaseline.Alphabetic)
                        {
                            this.RenderTextWithBaseline(_drawContext, formattedText, textPoint,
                                typeFace, emSize, stringFormat.Dominant);
                        }
                        else
                        {
                            _drawContext.DrawText(formattedText, textPoint);
                        }
                        //_drawContext.DrawText(formattedText, textPoint);
                    }

                    //float bboxWidth = (float)formattedText.Width;
                    double bboxWidth = formattedText.WidthIncludingTrailingWhitespace;
                    if (alignment == TextAlignment.Center)
                        bboxWidth /= 2f;
                    else if (alignment == TextAlignment.Right)
                        bboxWidth = 0;

                    //ctp.X += bboxWidth + emSize / 4 + spacing;
                    if (hasLetterSpacing)
                    {
                        ctp.X += bboxWidth + letterSpacing;
                    }
                    if (hasWordSpacing && char.IsWhiteSpace(text[i]))
                    {
                        if (hasLetterSpacing)
                        {
                            ctp.X += wordSpacing;
                        }
                        else
                        {
                            ctp.X += bboxWidth + wordSpacing;
                        }
                    }
                    else
                    {
                        if (!hasLetterSpacing)
                        {
                            ctp.X += bboxWidth;
                        }
                    }

                    if (rotateAt != null)
                    {
                        _drawContext.Pop();
                    }
                    j++;
                }
            }
            else
            {
#if DOTNET40 || DOTNET45 || DOTNET46
                FormattedText formattedText = new FormattedText(text, _context.CultureInfo,
                    stringFormat.Direction, typeFace,  emSize, textBrush);
#else
                FormattedText formattedText = new FormattedText(text, _context.CultureInfo,
                    stringFormat.Direction, typeFace, emSize, textBrush, _context.PixelsPerDip);
#endif

                if (this.IsMeasuring)
                {
                    this.AddTextWidth(ctp, formattedText.WidthIncludingTrailingWhitespace);  
                    return;
                }

                var textContext = this.TextContext;
                if (textContext != null && textContext.IsPositionChanged(element) == false)
                {
                    if (alignment == TextAlignment.Center && this.TextWidth > 0)
                    {
                        alignment = TextAlignment.Left;
                    }
                }

                formattedText.TextAlignment = alignment;
                formattedText.Trimming      = stringFormat.Trimming;

                if (textDecors != null && textDecors.Count != 0)
                {
                    formattedText.SetTextDecorations(textDecors);
                }

                //float xCorrection = 0;
                //if (alignment == TextAlignment.Left)
                //    xCorrection = emSize * 1f / 6f;
                //else if (alignment == TextAlignment.Right)
                //    xCorrection = -emSize * 1f / 6f;

                double yCorrection = formattedText.Baseline;

                float rotateAngle  = (float)rotate;
                Point textPoint    = new Point(ctp.X, ctp.Y - yCorrection);

                RotateTransform rotateAt = null;
                if (!rotateAngle.Equals(0))
                {
                    rotateAt = new RotateTransform(rotateAngle, ctp.X, ctp.Y);
                    _drawContext.PushTransform(rotateAt);
                }

                if (isForcedPathMode || _context.TextAsGeometry)
                {
                    // Geometry textGeometry = formattedText.BuildGeometry(textPoint);
                    Geometry textGeometry = null;
                    if (stringFormat.Dominant != DominantBaseline.Auto
                        && stringFormat.Dominant != DominantBaseline.Alphabetic)
                    {
                        textGeometry = this.BuildGeometryWithDominant(formattedText, textPoint, typeFace, emSize, stringFormat.Dominant);
                    }
                    else
                    {
                        textGeometry = formattedText.BuildGeometry(textPoint);
                    }

                    if (textGeometry != null && !textGeometry.IsEmpty())
                    {
                        _drawContext.DrawGeometry(textBrush, textPen,
                            ExtractTextPathGeometry(textGeometry));

                        this.IsTextPath = true;
                    }
                    else
                    {
                        if (stringFormat.Dominant != DominantBaseline.Auto
                            && stringFormat.Dominant != DominantBaseline.Alphabetic)
                        {
                            this.RenderTextWithBaseline(_drawContext, formattedText, textPoint,
                                typeFace, emSize, stringFormat.Dominant);
                        }
                        else
                        {
                            _drawContext.DrawText(formattedText, textPoint);
                        }
                        //_drawContext.DrawText(formattedText, textPoint);
                    }
                }
                else
                {
                    if (stringFormat.Dominant != DominantBaseline.Auto
                        && stringFormat.Dominant != DominantBaseline.Alphabetic)
                    {
                        this.RenderTextWithBaseline(_drawContext, formattedText, textPoint,
                            typeFace, emSize, stringFormat.Dominant);
                    }
                    else
                    {
                        _drawContext.DrawText(formattedText, textPoint);
                    }
                    //_drawContext.DrawText(formattedText, textPoint);
                }

                //float bboxWidth = (float)formattedText.Width;
                double bboxWidth = formattedText.WidthIncludingTrailingWhitespace;
                if (alignment == TextAlignment.Center)
                    bboxWidth /= 2f;
                else if (alignment == TextAlignment.Right)
                    bboxWidth = 0;

                //ctp.X += bboxWidth + emSize / 4;
                ctp.X += bboxWidth;

                if (rotateAt != null)
                {
                    _drawContext.Pop();
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Draws a FormattedText object at a specified point, with a calculated
        /// vertical offset to simulate CSS dominant-baseline behavior.
        /// </summary>
        private void RenderTextWithBaseline(DrawingContext drawingContext, FormattedText formattedText,
            Point origin, Typeface typeface, double fontSize, DominantBaseline baseline)
        {
            double yOffset = 0;

            // CORRECTED: Access metrics from the Typeface.FontFamily properties.
            // These are ratios, so we multiply by font size.
            double baselineRatio = typeface.FontFamily.Baseline;
            double lineSpacingRatio = typeface.FontFamily.LineSpacing;

            double ascent = baselineRatio * fontSize;
            double descent = (lineSpacingRatio - baselineRatio) * fontSize;
            double lineSpacing = lineSpacingRatio * fontSize;

            switch (baseline)
            {
                case DominantBaseline.Auto:
                case DominantBaseline.Alphabetic:
                    yOffset = 0;
                    break;
                case DominantBaseline.Middle:
                    yOffset = ascent - (lineSpacing / 2.0);
                    break;
                case DominantBaseline.Central:
                    yOffset = (ascent - descent) / 2.0;
                    break;
                case DominantBaseline.Hanging:
                    yOffset = ascent;
                    break;
                case DominantBaseline.Ideographic:
                    yOffset = 0;
                    break;
                case DominantBaseline.Mathematical:
                    yOffset = (ascent - descent) / 2.0;
                    break;
                case DominantBaseline.TextBeforeEdge:
                    yOffset = ascent;
                    break;
                case DominantBaseline.TextAfterEdge:
                    yOffset = -descent;
                    break;
                default:
                    yOffset = 0;
                    break;
            }

            drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y + yOffset));
            drawingContext.DrawText(formattedText, new Point(0, 0));
            drawingContext.Pop();
        }

        /// <summary>
        /// Builds the Geometry for a FormattedText object and transforms it to its
        /// final position, including dominant-baseline adjustments.
        /// </summary>
        /// <param name="formattedText">The FormattedText object to build the geometry for.</param>
        /// <param name="origin">The desired origin (x,y) of the text.</param>
        /// <param name="typeface">The Typeface of the formatted text.</param>
        /// <param name="fontSize">The font size of the formatted text.</param>
        /// <param name="baseline">The dominant-baseline for the text alignment.</param>
        /// <returns>A Geometry object representing the text, correctly positioned.</returns>
        private Geometry BuildGeometryWithDominant(FormattedText formattedText, Point origin,
            Typeface typeface, double fontSize, DominantBaseline baseline)
        {
            // First, get the y-offset for the dominant-baseline.
            double yOffset = CalculateBaselineOffset(typeface, fontSize, baseline);

            // Now, build the geometry using the text's natural origin (0,0).
            // This is the geometry of the un-positioned text block.
            Geometry textGeometry = formattedText.BuildGeometry(new Point(0, 0));

            // A FormattedText object's geometry is initially un-transformed.
            // Any existing transform would come from parent elements in an SVG renderer.
            // This is how you would handle that scenario.
            if (textGeometry.Transform != null && !textGeometry.Transform.Value.IsIdentity)
            {
                // Create a transform group to manage the final transformation.
                TransformGroup transformGroup = new TransformGroup();

                // If a transform already exists, add it to the group first.
                // The order of operations in a TransformGroup is crucial.
                transformGroup.Children.Add(textGeometry.Transform);

                // The Geometry.Transform property is how we apply transformations to the shape.
                // We'll create a TransformGroup to combine the initial positioning
                // and the dominant-baseline offset into a single transformation.
                transformGroup.Children.Add(new TranslateTransform(origin.X, origin.Y + yOffset));

                // The textGeometry is now ready to be transformed.
                textGeometry.Transform = transformGroup;
            }
            else
            {
                // If no existing transform, we can directly apply the translation.
                // Create a TranslateTransform to apply the y-offset.
                // The textGeometry is now ready to be transformed.
                textGeometry.Transform = new TranslateTransform(origin.X, origin.Y + yOffset);
            }   

            return textGeometry;
        }

        /// <summary>
        /// Helper to calculate the y-offset for a given dominant-baseline.
        /// </summary>
        private static double CalculateBaselineOffset(Typeface typeface, double fontSize, DominantBaseline baseline)
        {
            double yOffset = 0;

            double baselineRatio = typeface.FontFamily.Baseline;
            double lineSpacingRatio = typeface.FontFamily.LineSpacing;

            double ascent = baselineRatio * fontSize;
            double descent = (lineSpacingRatio - baselineRatio) * fontSize;
            double lineSpacing = lineSpacingRatio * fontSize;

            switch (baseline)
            {
                case DominantBaseline.Auto:
                case DominantBaseline.Alphabetic:
                    yOffset = 0;
                    break;
                case DominantBaseline.Middle:
                    yOffset = ascent - (lineSpacing / 2.0);
                    break;
                case DominantBaseline.Central:
                    yOffset = (ascent - descent) / 2.0;
                    break;
                case DominantBaseline.Hanging:
                    yOffset = ascent;
                    break;
                case DominantBaseline.Ideographic:
                    yOffset = 0;
                    break;
                case DominantBaseline.Mathematical:
                    yOffset = (ascent - descent) / 2.0;
                    break;
                case DominantBaseline.TextBeforeEdge:
                    yOffset = ascent;
                    break;
                case DominantBaseline.TextAfterEdge:
                    yOffset = -descent;
                    break;
                default:
                    yOffset = 0;
                    break;
            }

            return yOffset;
        }


        private void RenderText(WpfTextTuple textInfo, ref Point ctp, string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            WpfFontFamilyInfo familyInfo     = textInfo.Item1;
            double emSize                    = textInfo.Item2;
            WpfTextStringFormat stringFormat = textInfo.Item3;
            SvgTextContentElement element    = textInfo.Item4;

            FontFamily fontFamily  = familyInfo.Family;
            FontWeight   fontWeight = familyInfo.Weight;
            FontStyle fontStyle     = familyInfo.Style;
            FontStretch fontStretch = familyInfo.Stretch;

            WpfSvgPaint fillPaint   = new WpfSvgPaint(_context, element, "fill");
            Brush textBrush = fillPaint.GetBrush();

            WpfSvgPaint strokePaint = new WpfSvgPaint(_context, element, "stroke");
            Pen textPen = strokePaint.GetPen();

            if (textBrush == null && textPen == null)
            {
                return;
            }
            if (textBrush == null)
            {
                // If here, then the pen is not null, and so the fill cannot be null.
                // We set this to transparent for stroke only text path...
                textBrush = Brushes.Transparent;
            }
            if (textPen != null)
            {
                textPen.LineJoin   = PenLineJoin.Miter; // Better for text rendering
                textPen.MiterLimit = 1;
            }

            TextDecorationCollection textDecors = GetTextDecoration(element);
            TextAlignment alignment = stringFormat.Alignment;

            bool hasWordSpacing  = false;
            string wordSpaceText = element.GetAttribute("word-spacing");
            double wordSpacing   = 0;
            if (!string.IsNullOrWhiteSpace(wordSpaceText) &&
                double.TryParse(wordSpaceText, out wordSpacing) && !wordSpacing.Equals(0))
            {
                hasWordSpacing = true;
            }

            bool hasLetterSpacing = false;
            string letterSpaceText = element.GetAttribute("letter-spacing");
            double letterSpacing = 0;
            if (!string.IsNullOrWhiteSpace(letterSpaceText) && 
                double.TryParse(letterSpaceText, out letterSpacing) && !letterSpacing.Equals(0))
            {
                hasLetterSpacing = true;
            }

            bool isRotatePosOnly = false;

            IList<WpfTextPosition> textPositions = null;
            int textPosCount = 0;
            if ((placement != null && placement.HasPositions))
            {
                textPositions   = placement.Positions;
                textPosCount    = textPositions.Count;
                isRotatePosOnly = placement.IsRotateOnly;
            }

            WpfTextBuilder textBuilder = WpfTextBuilder.Create(familyInfo, this.TextCulture, emSize);
            this.IsTextPath = true;

            if (hasLetterSpacing || hasWordSpacing || textPositions != null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    var nextText = new string(text[i], 1);

                    textBuilder.TextAlignment = stringFormat.Alignment;
                    textBuilder.Trimming      = stringFormat.Trimming;

                    if (textDecors != null && textDecors.Count != 0)
                    {
                        textBuilder.TextDecorations = textDecors;
                    }

                    WpfTextPosition? textPosition = null;
                    if (textPositions != null && i < textPosCount)
                    {
                        textPosition = textPositions[i];
                    }

                    //float xCorrection = 0;
                    //if (alignment == TextAlignment.Left)
                    //    xCorrection = emSize * 1f / 6f;
                    //else if (alignment == TextAlignment.Right)
                    //    xCorrection = -emSize * 1f / 6f;

                    double yCorrection = textBuilder.Baseline;

                    float rotateAngle = (float)rotate;
                    if (textPosition != null)
                    {
                        if (!isRotatePosOnly)
                        {
                            Point pt = textPosition.Value.Location;
                            ctp.X = pt.X;
                            ctp.Y = pt.Y;
                        }
                        rotateAngle = (float)textPosition.Value.Rotation;
                    }
                    Point textStart = ctp;

                    RotateTransform rotateAt = null;
                    if (!rotateAngle.Equals(0))
                    {
                        rotateAt = new RotateTransform(rotateAngle, textStart.X, textStart.Y);
                        _drawContext.PushTransform(rotateAt);
                    }

                    Point textPoint = new Point(textStart.X, textStart.Y - yCorrection);

                    Geometry textGeometry = textBuilder.Build(element, nextText, textPoint.X, textPoint.Y);
                    if (textGeometry != null && !textGeometry.IsEmpty())
                    {
                        //_drawContext.DrawGeometry(textBrush, textPen, ExtractTextPathGeometry(textGeometry));
                        _drawContext.DrawGeometry(textBrush, textPen, textGeometry);
                    }

                    //float bboxWidth = (float)formattedText.Width;
                    double bboxWidth = textGeometry.Bounds.Width;
                    if (alignment == TextAlignment.Center)
                        bboxWidth /= 2f;
                    else if (alignment == TextAlignment.Right)
                        bboxWidth = 0;

                    //ctp.X += bboxWidth + emSize / 4 + spacing;
                    if (hasLetterSpacing)
                    {
                        ctp.X += bboxWidth + letterSpacing;
                    }
                    if (hasWordSpacing && char.IsWhiteSpace(text[i]))
                    {
                        if (hasLetterSpacing)
                        {
                            ctp.X += wordSpacing;
                        }
                        else
                        {
                            ctp.X += bboxWidth + wordSpacing;
                        }
                    }
                    else
                    {
                        if (!hasLetterSpacing)
                        {
                            ctp.X += bboxWidth;
                        }
                    }

                    if (rotateAt != null)
                    {
                        _drawContext.Pop();
                    }
                }
            }
            else
            {   
                textBuilder.TextAlignment = stringFormat.Alignment;
                textBuilder.Trimming      = stringFormat.Trimming;

                if (textDecors != null && textDecors.Count != 0)
                {
                    textBuilder.TextDecorations = textDecors;
                }

                //float xCorrection = 0;
                //if (alignment == TextAlignment.Left)
                //    xCorrection = emSize * 1f / 6f;
                //else if (alignment == TextAlignment.Right)
                //    xCorrection = -emSize * 1f / 6f;

                double yCorrection = textBuilder.Baseline;

                float rotateAngle = (float)rotate;
                Point textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                RotateTransform rotateAt = null;
                if (!rotateAngle.Equals(0))
                {
                    rotateAt = new RotateTransform(rotateAngle, ctp.X, ctp.Y);
                    _drawContext.PushTransform(rotateAt);
                }

                Geometry textGeometry = textBuilder.Build(element, text, textPoint.X, textPoint.Y);
                if (textGeometry != null && !textGeometry.IsEmpty())
                {
//                    _drawContext.DrawGeometry(textBrush, textPen, ExtractTextPathGeometry(textGeometry));
                    _drawContext.DrawGeometry(textBrush, textPen, textGeometry);
                }

                //float bboxWidth = (float)formattedText.Width;
//                double bboxWidth = textGeometry.Bounds.Width;
                double bboxWidth = textBuilder.Width;
                if (alignment == TextAlignment.Center)
                    bboxWidth /= 2f;
                else if (alignment == TextAlignment.Right)
                    bboxWidth = 0;

                //ctp.X += bboxWidth + emSize / 4;
                ctp.X += bboxWidth;

                if (rotateAt != null)
                {
                    _drawContext.Pop();
                }
            }
        }

        private void RenderTextRun(WpfTextTuple textInfo, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            WpfFontFamilyInfo familyInfo     = textInfo.Item1;
            double emSize                    = textInfo.Item2;
            WpfTextStringFormat stringFormat = textInfo.Item3;
            SvgTextContentElement element    = textInfo.Item4;

            FontFamily  fontFamily  = familyInfo.Family;
            FontWeight   fontWeight = familyInfo.Weight;
            FontStyle fontStyle     = familyInfo.Style;
            FontStretch fontStretch = familyInfo.Stretch;

            WpfSvgPaint fillPaint = new WpfSvgPaint(_context, element, "fill");
            Brush textBrush = fillPaint.GetBrush();

            WpfSvgPaint strokePaint = new WpfSvgPaint(_context, element, "stroke");
            Pen textPen = strokePaint.GetPen();

            if (textBrush == null && textPen == null)
            {
                return;
            }
            if (textBrush == null)
            {
                // If here, then the pen is not null, and so the fill cannot be null.
                // We set this to transparent for stroke only text path...
                textBrush = Brushes.Transparent;
            }
            if (textPen != null)
            {
                textPen.LineJoin = PenLineJoin.Miter; // Better for text rendering
                textPen.MiterLimit = 1;
            }

            TextDecorationCollection textDecors = GetTextDecoration(element);
            if (textDecors == null)
            {
                SvgTextBaseElement textElement = element.ParentNode as SvgTextBaseElement;

                if (textElement != null)
                {
                    textDecors = GetTextDecoration(textElement);
                }
            }

            TextAlignment alignment = stringFormat.Alignment;

            bool hasWordSpacing  = false;
            string wordSpaceText = element.GetAttribute("word-spacing");
            double wordSpacing   = 0;
            if (!string.IsNullOrWhiteSpace(wordSpaceText) &&
                double.TryParse(wordSpaceText, out wordSpacing) && !wordSpacing.Equals(0))
            {
                hasWordSpacing = true;
            }

            bool hasLetterSpacing  = false;
            string letterSpaceText = element.GetAttribute("letter-spacing");
            double letterSpacing   = 0;
            if (!string.IsNullOrWhiteSpace(letterSpaceText) &&
                double.TryParse(letterSpaceText, out letterSpacing) && !letterSpacing.Equals(0))
            {
                hasLetterSpacing = true;
            }

            bool isRotatePosOnly = false;

            IList<WpfTextPosition> textPositions = null;
            int textPosCount = 0;
            if ((placement != null && placement.HasPositions))
            {
                textPositions   = placement.Positions;
                textPosCount    = textPositions.Count;
                isRotatePosOnly = placement.IsRotateOnly;
            }

            WpfTextBuilder textBuilder = WpfTextBuilder.Create(familyInfo, this.TextCulture, emSize);
            this.IsTextPath = true;

            if (textPositions != null && textPositions.Count != 0)
            {
                if (textPositions.Count == text.Trim().Length) //TODO: Best way to handle this...
                {
                    text = text.Trim();
                }
            }

            if (hasLetterSpacing || hasWordSpacing || textPositions != null)
            {
                double spacing = Convert.ToDouble(letterSpacing);

                int startSpaces = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (char.IsWhiteSpace(text[i]))
                    {
                        startSpaces++;
                    }
                    else
                    {
                        break;
                    }
                }

                int j = 0;

                string inputText = string.Empty;
                for (int i = 0; i < text.Length; i++)
                {
                    // Avoid rendering only spaces at the start of text run...
                    if (i == 0 && startSpaces != 0)
                    {
                        inputText = text.Substring(0, startSpaces + 1);
                        i += startSpaces;
                    }
                    else
                    {
                        inputText = new string(text[i], 1);
                    }

                    if (this.IsMeasuring)
                    {
                        var textSize = textBuilder.MeasureText(element, inputText);
                        this.AddTextWidth(ctp, textSize.Width); 
                        continue;
                    }

                    textBuilder.Trimming      = stringFormat.Trimming;
                    textBuilder.TextAlignment = stringFormat.Alignment;

                    if (textDecors != null && textDecors.Count != 0)
                    {
                        textBuilder.TextDecorations = textDecors;
                    }

                    WpfTextPosition? textPosition = null;
                    if (textPositions != null && j < textPosCount)
                    {
                        textPosition = textPositions[j];
                    }

                    //float xCorrection = 0;
                    //if (alignment == TextAlignment.Left)
                    //    xCorrection = emSize * 1f / 6f;
                    //else if (alignment == TextAlignment.Right)
                    //    xCorrection = -emSize * 1f / 6f;

                    double yCorrection = textBuilder.Baseline;

                    float rotateAngle = (float)rotate;
                    if (textPosition != null)
                    {
                        if (!isRotatePosOnly)
                        {
                            Point pt = textPosition.Value.Location;
                            ctp.X = pt.X;
                            ctp.Y = pt.Y;
                        }
                        rotateAngle = (float)textPosition.Value.Rotation;
                    }
                    Point textStart = ctp;

                    RotateTransform rotateAt = null;
                    if (!rotateAngle.Equals(0))
                    {
                        rotateAt = new RotateTransform(rotateAngle, textStart.X, textStart.Y);
                        _drawContext.PushTransform(rotateAt);
                    }

                    Point textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                    Geometry textGeometry = textBuilder.Build(element, inputText, textPoint.X, textPoint.Y);
                    if (textGeometry != null && !textGeometry.IsEmpty())
                    {
//                        _drawContext.DrawGeometry(textBrush, textPen, ExtractTextPathGeometry(textGeometry));
                        _drawContext.DrawGeometry(textBrush, textPen, textGeometry);
                    }

                    //float bboxWidth = (float)formattedText.Width;
                    double bboxWidth = textGeometry.Bounds.Width;
                    if (alignment == TextAlignment.Center)
                        bboxWidth /= 2f;
                    else if (alignment == TextAlignment.Right)
                        bboxWidth = 0;

                    //ctp.X += bboxWidth + emSize / 4 + spacing;
                    if (hasLetterSpacing)
                    {
                        ctp.X += bboxWidth + letterSpacing;
                    }
                    if (hasWordSpacing && char.IsWhiteSpace(text[i]))
                    {
                        if (hasLetterSpacing)
                        {
                            ctp.X += wordSpacing;
                        }
                        else
                        {
                            ctp.X += bboxWidth + wordSpacing;
                        }
                    }
                    else
                    {
                        if (!hasLetterSpacing)
                        {
                            ctp.X += bboxWidth;
                        }
                    }

                    if (rotateAt != null)
                    {
                        _drawContext.Pop();
                    }
                    j++;
                }
            }
            else
            {   
                if (this.IsMeasuring)
                {
                    var textSize = textBuilder.MeasureText(element, text);
                    this.AddTextWidth(ctp, textSize.Width);
                    return;
                }

                var textContext = this.TextContext;
                if (textContext != null && textContext.IsPositionChanged(element) == false)
                {
                    if (alignment == TextAlignment.Center && this.TextWidth > 0)
                    {
                        alignment = TextAlignment.Left;
                    }
                }

                textBuilder.TextAlignment = alignment;
                textBuilder.Trimming      = stringFormat.Trimming;

                if (textDecors != null && textDecors.Count != 0)
                {
                    textBuilder.TextDecorations = textDecors;
                }

                //float xCorrection = 0;
                //if (alignment == TextAlignment.Left)
                //    xCorrection = emSize * 1f / 6f;
                //else if (alignment == TextAlignment.Right)
                //    xCorrection = -emSize * 1f / 6f;

                double yCorrection = textBuilder.Baseline;

                float rotateAngle  = (float)rotate;
                Point textPoint    = new Point(ctp.X, ctp.Y - yCorrection);

                RotateTransform rotateAt = null;
                if (!rotateAngle.Equals(0))
                {
                    rotateAt = new RotateTransform(rotateAngle, ctp.X, ctp.Y);
                    _drawContext.PushTransform(rotateAt);
                }

                Geometry textGeometry = textBuilder.Build(element, text, textPoint.X, textPoint.Y);
                if (textGeometry != null && !textGeometry.IsEmpty())
                {
//                    _drawContext.DrawGeometry(textBrush, textPen, ExtractTextPathGeometry(textGeometry));
                    _drawContext.DrawGeometry(textBrush, textPen, textGeometry);
                }

                //float bboxWidth = (float)formattedText.Width;
                //                double bboxWidth = textGeometry.Bounds.Width;
                double bboxWidth = textBuilder.Width;

                if (alignment == TextAlignment.Center)
                    bboxWidth /= 2f;
                else if (alignment == TextAlignment.Right)
                    bboxWidth = 0;

                //ctp.X += bboxWidth + emSize / 4;
                ctp.X += bboxWidth;

                if (rotateAt != null)
                {
                    _drawContext.Pop();
                }
            }
        }

        #endregion

        #region Private Classes

        private sealed class WpfTextTuple : Tuple<WpfFontFamilyInfo, double, WpfTextStringFormat, SvgTextContentElement>
        {
            public WpfTextTuple(WpfFontFamilyInfo familyInfo, double emSize,
                WpfTextStringFormat stringFormat, SvgTextContentElement element)
                : base(familyInfo, emSize, stringFormat, element)
            {
            }
        }

        #endregion
    }
}
