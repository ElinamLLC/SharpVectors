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
            if (String.IsNullOrEmpty(text))
                return;

            double emSize         = GetComputedFontSize(element);
            FontFamily fontFamily = GetTextFontFamily(element, emSize);

            FontStyle fontStyle   = GetTextFontStyle(element);
            FontWeight fontWeight = GetTextFontWeight(element);

            FontStretch fontStretch = GetTextFontStretch(element);

            WpfTextStringFormat stringFormat = GetTextStringFormat(element);

            // Fix the use of Postscript fonts...
            WpfFontFamilyVisitor fontFamilyVisitor = _drawContext.FontFamilyVisitor;
            if (!String.IsNullOrEmpty(_actualFontName) && fontFamilyVisitor != null)
            {
                WpfFontFamilyInfo currentFamily = new WpfFontFamilyInfo(fontFamily, fontWeight,
                    fontStyle, fontStretch);
                WpfFontFamilyInfo familyInfo = fontFamilyVisitor.Visit(_actualFontName, 
                    currentFamily, _drawContext);
                if (familyInfo != null && !familyInfo.IsEmpty)
                {
                    fontFamily  = familyInfo.Family;
                    fontWeight  = familyInfo.Weight;
                    fontStyle   = familyInfo.Style;
                    fontStretch = familyInfo.Stretch;
                }
            }

            WpfSvgPaint fillPaint   = new WpfSvgPaint(_drawContext, element, "fill");
            Brush textBrush = fillPaint.GetBrush();

            WpfSvgPaint strokePaint = new WpfSvgPaint(_drawContext, element, "stroke");
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
            TextAlignment alignment = stringFormat.Alignment;

            bool hasWordSpacing  = false;
            string wordSpaceText = element.GetAttribute("word-spacing");
            double wordSpacing   = 0;
            if (!String.IsNullOrEmpty(wordSpaceText) &&
                Double.TryParse(wordSpaceText, out wordSpacing) && (float)wordSpacing != 0)
            {
                hasWordSpacing = true;
            }

            bool hasLetterSpacing = false;
            string letterSpaceText = element.GetAttribute("letter-spacing");
            double letterSpacing = 0;
            if (!String.IsNullOrEmpty(letterSpaceText) && 
                Double.TryParse(letterSpaceText, out letterSpacing) && (float)letterSpacing != 0)
            {
                hasLetterSpacing = true;
            }

            bool isRotatePosOnly = false;

            IList<WpfTextPosition> textPositions = null;
            int textPosCount = 0;
            if ((placement != null && placement.HasPositions))
            {
                textPositions = placement.Positions;
                textPosCount = textPositions.Count;
                isRotatePosOnly = placement.IsRotateOnly;
            }

            if (hasLetterSpacing || hasWordSpacing || textPositions != null)
            {   
                for (int i = 0; i < text.Length; i++)
                {
                    FormattedText formattedText = new FormattedText(new string(text[i], 1), 
                        _drawContext.CultureInfo, stringFormat.Direction, 
                        new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                        emSize, textBrush);

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
                    if (rotateAngle != 0)
                    {
                        rotateAt = new RotateTransform(rotateAngle, textStart.X, textStart.Y);
                        _textContext.PushTransform(rotateAt);
                    }

                    Point textPoint = new Point(textStart.X, textStart.Y - yCorrection);

                    if (textPen != null || _drawContext.TextAsGeometry)
                    {
                        Geometry textGeometry = formattedText.BuildGeometry(textPoint);
                        if (textGeometry != null && !textGeometry.IsEmpty())
                        {
                            _textContext.DrawGeometry(textBrush, textPen, 
                                ExtractTextPathGeometry(textGeometry));

                            this.IsTextPath = true;
                        }
                        else
                        {
                            _textContext.DrawText(formattedText, textPoint);
                        }
                    }
                    else
                    {
                        _textContext.DrawText(formattedText, textPoint);
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
                    if (hasWordSpacing && Char.IsWhiteSpace(text[i]))
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
                        _textContext.Pop();
                    }
                }
            }
            else
            {   
                FormattedText formattedText = new FormattedText(text, _drawContext.CultureInfo,
                    stringFormat.Direction, new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), 
                    emSize, textBrush);

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
                if (rotateAngle != 0)
                {
                    rotateAt = new RotateTransform(rotateAngle, ctp.X, ctp.Y);
                    _textContext.PushTransform(rotateAt);
                }

                if (textPen != null || _drawContext.TextAsGeometry)
                {
                    Geometry textGeometry = formattedText.BuildGeometry(textPoint);
                    if (textGeometry != null && !textGeometry.IsEmpty())
                    {
                        _textContext.DrawGeometry(textBrush, textPen, 
                            ExtractTextPathGeometry(textGeometry));

                        this.IsTextPath = true;
                    }
                    else
                    {
                        _textContext.DrawText(formattedText, textPoint);
                    }
                }
                else
                {
                    _textContext.DrawText(formattedText, textPoint);
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
                    _textContext.Pop();
                }
            }
        }

        public override void RenderTextRun(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement)
        {
            if (String.IsNullOrEmpty(text))
                return;

            double emSize         = GetComputedFontSize(element);
            FontFamily fontFamily = GetTextFontFamily(element, emSize);

            FontStyle fontStyle   = GetTextFontStyle(element);
            FontWeight fontWeight = GetTextFontWeight(element);

            FontStretch fontStretch = GetTextFontStretch(element);

            WpfTextStringFormat stringFormat = GetTextStringFormat(element);

            // Fix the use of Postscript fonts...
            WpfFontFamilyVisitor fontFamilyVisitor = _drawContext.FontFamilyVisitor;
            if (!String.IsNullOrEmpty(_actualFontName) && fontFamilyVisitor != null)
            {
                WpfFontFamilyInfo currentFamily = new WpfFontFamilyInfo(fontFamily, fontWeight,
                    fontStyle, fontStretch);
                WpfFontFamilyInfo familyInfo = fontFamilyVisitor.Visit(_actualFontName, 
                    currentFamily,_drawContext);
                if (familyInfo != null && !familyInfo.IsEmpty)
                {
                    fontFamily  = familyInfo.Family;
                    fontWeight  = familyInfo.Weight;
                    fontStyle   = familyInfo.Style;
                    fontStretch = familyInfo.Stretch;
                }
            }

            WpfSvgPaint fillPaint = new WpfSvgPaint(_drawContext, element, "fill");
            Brush textBrush = fillPaint.GetBrush();

            WpfSvgPaint strokePaint = new WpfSvgPaint(_drawContext, element, "stroke");
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
            double wordSpacing = 0;
            if (!String.IsNullOrEmpty(wordSpaceText) &&
                Double.TryParse(wordSpaceText, out wordSpacing) && (float)wordSpacing != 0)
            {
                hasWordSpacing = true;
            }

            bool hasLetterSpacing = false;
            string letterSpaceText = element.GetAttribute("letter-spacing");
            double letterSpacing = 0;
            if (!String.IsNullOrEmpty(letterSpaceText) &&
                Double.TryParse(letterSpaceText, out letterSpacing) && (float)letterSpacing != 0)
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

            if (hasLetterSpacing || hasWordSpacing || textPositions != null)
            {
                double spacing = Convert.ToDouble(letterSpacing);
                for (int i = 0; i < text.Length; i++)
                {
                    FormattedText formattedText = new FormattedText(new string(text[i], 1), 
                        _drawContext.CultureInfo, stringFormat.Direction, 
                        new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                        emSize, textBrush);

                    if (this.IsMeasuring)
                    {
                        this.AddTextWidth(formattedText.WidthIncludingTrailingWhitespace); 
                        continue;
                    }

                    formattedText.Trimming      = stringFormat.Trimming;
                    formattedText.TextAlignment = stringFormat.Alignment;

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
                    if (rotateAngle != 0)
                    {
                        rotateAt = new RotateTransform(rotateAngle, textStart.X, textStart.Y);
                        _textContext.PushTransform(rotateAt);
                    }

                    Point textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                    if (textPen != null || _drawContext.TextAsGeometry)
                    {
                        Geometry textGeometry = formattedText.BuildGeometry(textPoint);
                        if (textGeometry != null && !textGeometry.IsEmpty())
                        {
                            _textContext.DrawGeometry(textBrush, textPen,
                                ExtractTextPathGeometry(textGeometry));

                            this.IsTextPath = true;
                        }
                        else
                        {
                            _textContext.DrawText(formattedText, textPoint);
                        }
                    }
                    else
                    {
                        _textContext.DrawText(formattedText, textPoint);
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
                    if (hasWordSpacing && Char.IsWhiteSpace(text[i]))
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
                        _textContext.Pop();
                    }
                }
            }
            else
            {   
                FormattedText formattedText = new FormattedText(text, _drawContext.CultureInfo,
                    stringFormat.Direction, new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), 
                    emSize, textBrush);

                if (this.IsMeasuring)
                {
                    this.AddTextWidth(formattedText.WidthIncludingTrailingWhitespace);  
                    return;
                }

                if (alignment == TextAlignment.Center && this.TextWidth > 0)
                {
                    alignment = TextAlignment.Left;
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
                if (rotateAngle != 0)
                {
                    rotateAt = new RotateTransform(rotateAngle, ctp.X, ctp.Y);
                    _textContext.PushTransform(rotateAt);
                }

                if (textPen != null || _drawContext.TextAsGeometry)
                {
                    Geometry textGeometry = formattedText.BuildGeometry(textPoint);
                    if (textGeometry != null && !textGeometry.IsEmpty())
                    {
                        _textContext.DrawGeometry(textBrush, textPen,
                            ExtractTextPathGeometry(textGeometry));

                        this.IsTextPath = true;
                    }
                    else
                    {
                        _textContext.DrawText(formattedText, textPoint);
                    }
                }
                else
                {
                    _textContext.DrawText(formattedText, textPoint);
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
                    _textContext.Pop();
                }
            }
        }

        #endregion
    }
}
