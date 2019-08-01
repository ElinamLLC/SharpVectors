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

        public WpfHorzTextRenderer(SvgTextElement textElement, WpfTextRendering textRendering)
            : base(textElement, textRendering)
        {
        }

        #endregion

        #region Public Methods

        public override void RenderSingleLineText(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            double emSize      = GetComputedFontSize(element);
            var fontFamilyInfo = GetTextFontFamilyInfo(element);

            FontFamily fontFamily   = fontFamilyInfo.Family;
            FontStyle fontStyle     = fontFamilyInfo.Style;
            FontWeight fontWeight   = fontFamilyInfo.Weight;
            FontStretch fontStretch = fontFamilyInfo.Stretch;

            WpfTextStringFormat stringFormat = GetTextStringFormat(element);

            // Fix the use of Postscript fonts...
            WpfFontFamilyVisitor fontFamilyVisitor = _context.FontFamilyVisitor;
            if (!string.IsNullOrWhiteSpace(_actualFontName) && fontFamilyVisitor != null)
            {
                //WpfFontFamilyInfo currentFamily = new WpfFontFamilyInfo(fontFamily, fontWeight,
                //    fontStyle, fontStretch);
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
            if (textBrush == null)
            {
                // If here, then the pen is not null, and so the fill cannot be null.
                // We set this to transparent for stroke only text path...
                textBrush = Brushes.Transparent;
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

            if (hasLetterSpacing || hasWordSpacing || textPositions != null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    var nextText = new string(text[i], 1);
                    FormattedText formattedText = new FormattedText(nextText, 
                        _context.CultureInfo, stringFormat.Direction, typeFace, emSize, textBrush);

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

                    if (textPen != null || _context.TextAsGeometry)
                    {
                        Geometry textGeometry = formattedText.BuildGeometry(textPoint);
                        if (textGeometry != null && !textGeometry.IsEmpty())
                        {
                            _drawContext.DrawGeometry(textBrush, textPen, ExtractTextPathGeometry(textGeometry));

                            this.IsTextPath = true;
                        }
                        else
                        {
                            _drawContext.DrawText(formattedText, textPoint);
                        }
                    }
                    else
                    {
                        _drawContext.DrawText(formattedText, textPoint);
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
                FormattedText formattedText = new FormattedText(text, _context.CultureInfo,
                    stringFormat.Direction, typeFace, emSize, textBrush);

                formattedText.TextAlignment = stringFormat.Alignment;
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

                float rotateAngle = (float)rotate;
                Point textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                RotateTransform rotateAt = null;
                if (!rotateAngle.Equals(0))
                {
                    rotateAt = new RotateTransform(rotateAngle, ctp.X, ctp.Y);
                    _drawContext.PushTransform(rotateAt);
                }

                if (textPen != null || _context.TextAsGeometry)
                {
                    Geometry textGeometry = formattedText.BuildGeometry(textPoint);
                    if (textGeometry != null && !textGeometry.IsEmpty())
                    {
                        _drawContext.DrawGeometry(textBrush, textPen, 
                            ExtractTextPathGeometry(textGeometry));

                        this.IsTextPath = true;
                    }
                    else
                    {
                        _drawContext.DrawText(formattedText, textPoint);
                    }
                }
                else
                {
                    _drawContext.DrawText(formattedText, textPoint);
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

        public override void RenderTextRun(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            double emSize      = GetComputedFontSize(element);
            var fontFamilyInfo = GetTextFontFamilyInfo(element);

            FontFamily fontFamily   = fontFamilyInfo.Family;
            FontStyle fontStyle     = fontFamilyInfo.Style;
            FontWeight fontWeight   = fontFamilyInfo.Weight;
            FontStretch fontStretch = fontFamilyInfo.Stretch;

            WpfTextStringFormat stringFormat = GetTextStringFormat(element);

            // Fix the use of Postscript fonts...
            WpfFontFamilyVisitor fontFamilyVisitor = _context.FontFamilyVisitor;
            if (!string.IsNullOrWhiteSpace(_actualFontName) && fontFamilyVisitor != null)
            {
                //WpfFontFamilyInfo currentFamily = new WpfFontFamilyInfo(fontFamily, fontWeight,
                //    fontStyle, fontStretch);
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
            else if (textBrush == null)
            {
                // If here, then the pen is not null, and so the fill cannot be null.
                // We set this to transparent for stroke only text path...
                textBrush = Brushes.Transparent;
            }

            TextDecorationCollection textDecors = GetTextDecoration(element);
            if (textDecors == null)
            {
                SvgTextElement textElement = element.ParentNode as SvgTextElement;

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

                    FormattedText formattedText = new FormattedText(inputText,  _context.CultureInfo, 
                        stringFormat.Direction, typeFace, emSize, textBrush);

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

                    if (textPen != null || _context.TextAsGeometry)
                    {
                        Geometry textGeometry = formattedText.BuildGeometry(textPoint);
                        if (textGeometry != null && !textGeometry.IsEmpty())
                        {
                            _drawContext.DrawGeometry(textBrush, textPen,
                                ExtractTextPathGeometry(textGeometry));

                            this.IsTextPath = true;
                        }
                        else
                        {
                            _drawContext.DrawText(formattedText, textPoint);
                        }
                    }
                    else
                    {
                        _drawContext.DrawText(formattedText, textPoint);
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
                FormattedText formattedText = new FormattedText(text, _context.CultureInfo,
                    stringFormat.Direction, typeFace,  emSize, textBrush);

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

                if (textPen != null || _context.TextAsGeometry)
                {
                    Geometry textGeometry = formattedText.BuildGeometry(textPoint);
                    if (textGeometry != null && !textGeometry.IsEmpty())
                    {
                        _drawContext.DrawGeometry(textBrush, textPen,
                            ExtractTextPathGeometry(textGeometry));

                        this.IsTextPath = true;
                    }
                    else
                    {
                        _drawContext.DrawText(formattedText, textPoint);
                    }
                }
                else
                {
                    _drawContext.DrawText(formattedText, textPoint);
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
    }
}
