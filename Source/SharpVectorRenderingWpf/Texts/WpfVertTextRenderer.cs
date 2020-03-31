using System;
using System.ComponentModel;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;  
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Renderers.Texts
{
    public sealed class WpfVertTextRenderer : WpfTextRenderer
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public WpfVertTextRenderer(SvgTextBaseElement textElement, WpfTextRendering textRendering)
            : base(textElement, textRendering)
        {
        }

        #endregion

        #region Public Methods

        #region RenderSingleLineText Method

        public override void RenderText(SvgTextContentElement element,
            ref Point ctp, string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            int vertOrientation    = -1;
            int horzOrientation    = -1;
            string orientationText = element.GetPropertyValue("glyph-orientation-vertical");
            if (!string.IsNullOrWhiteSpace(orientationText))
            {
                double orientationValue = 0;
                if (double.TryParse(orientationText, out orientationValue))
                {
                    vertOrientation = (int)orientationValue;
                }
            }
            orientationText = element.GetPropertyValue("glyph-orientation-horizontal");
            if (!string.IsNullOrWhiteSpace(orientationText))
            {
                double orientationValue = 0;
                if (double.TryParse(orientationText, out orientationValue))
                {
                    horzOrientation = (int)orientationValue;
                }
            }

            Point startPoint = ctp;
            IList<WpfTextRun> textRunList = WpfTextRun.BreakWords(text, 
                vertOrientation, horzOrientation);

            for (int tr = 0; tr < textRunList.Count; tr++)
            {
                // For unknown reasons, FormattedText will split a text like "-70%" into two parts "-"
                // and "70%". We provide a shift to account for the split...
                double baselineShiftX = 0;
                double baselineShiftY = 0;

                WpfTextRun textRun = textRunList[tr];

                DrawingGroup verticalGroup = new DrawingGroup();

                DrawingContext verticalContext = verticalGroup.Open();
                DrawingContext currentContext  = _drawContext;

                _drawContext = verticalContext;

                this.DrawSingleLineText(element, ref ctp, textRun, rotate, placement);

                verticalContext.Close();

                _drawContext = currentContext;

                if (verticalGroup.Children.Count == 1)
                {
                    DrawingGroup textGroup = verticalGroup.Children[0] as DrawingGroup;
                    if (textGroup != null)
                    {
                        verticalGroup = textGroup;
                    }
                }

                string runText = textRun.Text;
                int charCount = runText.Length;

                double totalHeight = 0;
                DrawingCollection drawings = verticalGroup.Children;
                int itemCount = drawings != null ? drawings.Count : 0;
                for (int i = 0; i < itemCount; i++)
                {
                    Drawing textDrawing = drawings[i];
                    DrawingGroup textGroup = textDrawing as DrawingGroup;

                    if (vertOrientation == -1)
                    {
                        if (textGroup != null)
                        {
                            for (int j = 0; j < textGroup.Children.Count; j++)
                            {
                                GlyphRunDrawing glyphDrawing = textGroup.Children[j] as GlyphRunDrawing;
                                if (glyphDrawing != null)
                                {
                                    if (textRun.IsLatin)
                                    {
                                        GlyphRun glyphRun = glyphDrawing.GlyphRun;

                                        IList<ushort> glyphIndices = glyphRun.GlyphIndices;
                                        IDictionary<ushort, double> allGlyphWeights = glyphRun.GlyphTypeface.AdvanceWidths;
                                        double lastAdvanceWeight =
                                            allGlyphWeights[glyphIndices[glyphIndices.Count - 1]] * glyphRun.FontRenderingEmSize;

                                        totalHeight += glyphRun.ComputeAlignmentBox().Width + lastAdvanceWeight / 2d;
                                    }
                                    else
                                    {
                                        totalHeight += ChangeGlyphOrientation(glyphDrawing,
                                            baselineShiftX, baselineShiftY, false);
                                    }
                                }
                            }
                        }
                        else
                        {
                            GlyphRunDrawing glyphDrawing = textDrawing as GlyphRunDrawing;
                            if (glyphDrawing != null)
                            {
                                if (textRun.IsLatin)
                                {
                                    GlyphRun glyphRun = glyphDrawing.GlyphRun;

                                    IList<UInt16> glyphIndices = glyphRun.GlyphIndices;
                                    IDictionary<ushort, double> allGlyphWeights = glyphRun.GlyphTypeface.AdvanceWidths;
                                    double lastAdvanceWeight =
                                        allGlyphWeights[glyphIndices[glyphIndices.Count - 1]] * glyphRun.FontRenderingEmSize;

                                    totalHeight += glyphRun.ComputeAlignmentBox().Width + lastAdvanceWeight / 2d;
                                }
                                else
                                {
                                    totalHeight += ChangeGlyphOrientation(glyphDrawing,
                                        baselineShiftX, baselineShiftY, false);
                                }
                            }
                        }
                    }
                    else if (vertOrientation == 0)
                    {
                        if (textGroup != null)
                        {
                            for (int j = 0; j < textGroup.Children.Count; j++)
                            {
                                GlyphRunDrawing glyphDrawing = textGroup.Children[j] as GlyphRunDrawing;
                                if (glyphDrawing != null)
                                {
                                    baselineShiftX = ChangeGlyphOrientation(glyphDrawing,
                                        baselineShiftX, baselineShiftY, textRun.IsLatin);
                                    totalHeight += baselineShiftX;
                                }
                            }
                        }
                        else
                        {
                            GlyphRunDrawing glyphDrawing = textDrawing as GlyphRunDrawing;
                            if (textDrawing != null)
                            {
                                totalHeight += ChangeGlyphOrientation(glyphDrawing,
                                    baselineShiftX, baselineShiftY, textRun.IsLatin);
                            }
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                if (!this.IsMeasuring)
                {
                    _drawContext.DrawDrawing(verticalGroup);
                }

                if (tr < textRunList.Count)
                {
                    ctp.X = startPoint.X;
                    ctp.Y = startPoint.Y + totalHeight;
                    startPoint.Y += totalHeight;
                }
            }
        }

        #endregion

        #region RenderTextRun Method

        public override void RenderTextRun(SvgTextContentElement element,
            ref Point ctp, string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            int vertOrientation = -1;
            int horzOrientation = -1;

            string orientationText = element.GetPropertyValue("glyph-orientation-vertical");
            if (!string.IsNullOrWhiteSpace(orientationText))
            {
                double orientationValue = 0;
                if (double.TryParse(orientationText, out orientationValue))
                {
                    vertOrientation = (int)orientationValue;
                }
            }
            orientationText = element.GetPropertyValue("glyph-orientation-horizontal");
            if (!string.IsNullOrWhiteSpace(orientationText))
            {
                double orientationValue = 0;
                if (double.TryParse(orientationText, out orientationValue))
                {
                    horzOrientation = (int)orientationValue;
                }
            }

            Point startPoint      = ctp;

            IList<WpfTextRun> textRunList = WpfTextRun.BreakWords(text,
                vertOrientation, horzOrientation);

            for (int tr = 0; tr < textRunList.Count; tr++)
            {
                // For unknown reasons, FormattedText will split a text like "-70%" into two parts "-"
                // and "70%". We provide a shift to account for the split...
                double baselineShiftX = 0;
                double baselineShiftY = 0;
                WpfTextRun textRun = textRunList[tr];

                if (!textRun.IsLatin)
                {
                    textRun = textRunList[tr];
                }

                DrawingGroup verticalGroup = new DrawingGroup();

                DrawingContext verticalContext = verticalGroup.Open();
                DrawingContext currentContext  = _drawContext;

                _drawContext = verticalContext;

                this.DrawTextRun(element, ref ctp, textRun, rotate, placement);

                verticalContext.Close();

                _drawContext = currentContext;

                if (verticalGroup.Children.Count == 1)
                {
                    DrawingGroup textGroup = verticalGroup.Children[0] as DrawingGroup;
                    if (textGroup != null)
                    {
                        verticalGroup = textGroup;
                    }
                }

                double totalHeight = 0;
                DrawingCollection drawings = verticalGroup.Children;
                int itemCount = drawings != null ? drawings.Count : 0;
                for (int i = 0; i < itemCount; i++)
                {
                    Drawing textDrawing = drawings[i];
                    DrawingGroup textGroup = textDrawing as DrawingGroup;

                    if (vertOrientation == -1)
                    {
                        if (textGroup != null)
                        {
                            for (int j = 0; j < textGroup.Children.Count; j++)
                            {
                                GlyphRunDrawing glyphDrawing = textGroup.Children[j] as GlyphRunDrawing;
                                if (glyphDrawing != null)
                                {
                                    if (textRun.IsLatin)
                                    {
                                        GlyphRun glyphRun = glyphDrawing.GlyphRun;

                                        IList<ushort> glyphIndices = glyphRun.GlyphIndices;
                                        IDictionary<ushort, double> allGlyphWeights = glyphRun.GlyphTypeface.AdvanceWidths;
                                        double lastAdvanceWeight =
                                            allGlyphWeights[glyphIndices[glyphIndices.Count - 1]] * glyphRun.FontRenderingEmSize;

                                        totalHeight += glyphRun.ComputeAlignmentBox().Width + lastAdvanceWeight / 2d;
                                    }
                                    else
                                    {
                                        totalHeight += ChangeGlyphOrientation(glyphDrawing,
                                            baselineShiftX, baselineShiftY, false);
                                    }
                                }
                            }
                        }
                        else
                        {
                            GlyphRunDrawing glyphDrawing = textDrawing as GlyphRunDrawing;
                            if (glyphDrawing != null)
                            {
                                if (textRun.IsLatin)
                                {
                                    GlyphRun glyphRun = glyphDrawing.GlyphRun;

                                    IList<ushort> glyphIndices = glyphRun.GlyphIndices;
                                    IDictionary<ushort, double> allGlyphWeights = glyphRun.GlyphTypeface.AdvanceWidths;
                                    double lastAdvanceWeight =
                                        allGlyphWeights[glyphIndices[glyphIndices.Count - 1]] * glyphRun.FontRenderingEmSize;

                                    totalHeight += glyphRun.ComputeAlignmentBox().Width + lastAdvanceWeight / 2d;
                                }
                                else
                                {
                                    totalHeight += ChangeGlyphOrientation(glyphDrawing,
                                        baselineShiftX, baselineShiftY, false);
                                }
                            }
                        }
                    }
                    else if (vertOrientation == 0)
                    {
                        if (textGroup != null)
                        {
                            for (int j = 0; j < textGroup.Children.Count; j++)
                            {
                                GlyphRunDrawing glyphDrawing = textGroup.Children[j] as GlyphRunDrawing;
                                if (glyphDrawing != null)
                                {
                                    baselineShiftX = ChangeGlyphOrientation(glyphDrawing,
                                        baselineShiftX, baselineShiftY, textRun.IsLatin);
                                    totalHeight += baselineShiftX;
                                }
                            }
                        }
                        else
                        {
                            GlyphRunDrawing glyphDrawing = textDrawing as GlyphRunDrawing;
                            if (glyphDrawing != null)
                            {
                                totalHeight += ChangeGlyphOrientation(glyphDrawing,
                                    baselineShiftX, baselineShiftY, textRun.IsLatin);
                            }
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                if (!this.IsMeasuring)
                {
                    _drawContext.DrawDrawing(verticalGroup);
                }

                if (tr < textRunList.Count)
                {
                    ctp.X = startPoint.X;
                    ctp.Y = startPoint.Y + totalHeight;
                    startPoint.Y += totalHeight;
                }
            }
        }

        #endregion

        #endregion

        #region Private Methods

        #region DrawSingleLineText Method

        private Size MeasureString(string text, double fontSize, Typeface typeFace)
        {
            var verticalText = this.GetVerticalText(text);

            FormattedText formattedText = new FormattedText(verticalText, System.Globalization.CultureInfo.CurrentCulture,
                 FlowDirection.LeftToRight, typeFace, fontSize, Brushes.Black);

            //formattedText.LineHeight = this.CharSpacing;

            return new Size(formattedText.Width, formattedText.Height);
        }

        private string GetVerticalText(string text)
        {
            string result = string.Empty;

            for (int i = 0; i < text.Length - 1; i++)
            {
                result += new string(text[i], 1) + "\n";
            }

            return result;
        }

        private void DrawSingleLineText(SvgTextContentElement element, ref Point ctp,
            WpfTextRun textRun, double rotate, WpfTextPlacement placement)
        {
            if (textRun == null || textRun.IsEmpty)
                return;

            string text        = textRun.Text;
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
            if (textBrush == null)
            {
                // If here, then the pen is not null, and so the fill cannot be null.
                // We set this to transparent for stroke only text path...
                textBrush = Brushes.Transparent;
            }

            TextDecorationCollection textDecors = GetTextDecoration(element);
            TextAlignment alignment = stringFormat.Alignment;

            var typeface = new Typeface(fontFamily, fontStyle, fontWeight, fontStretch);

            string letterSpacing = element.GetAttribute("letter-spacing");
            if (string.IsNullOrWhiteSpace(letterSpacing))
            {
                FormattedText formattedText = new FormattedText(text, 
                    textRun.IsLatin ? _context.EnglishCultureInfo : _context.CultureInfo,
                    stringFormat.Direction, typeface, emSize, textBrush);

                if (this.IsMeasuring)
                {
                    this.AddTextWidth(ctp, formattedText.WidthIncludingTrailingWhitespace);
                    return;
                }

//                formattedText.TextAlignment = stringFormat.Alignment;
                formattedText.TextAlignment = TextAlignment.Left;
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
                //float yCorrection = 0;

                if (textRun.IsLatin && textRun.VerticalOrientation == -1)
                {
                    yCorrection = yCorrection + formattedText.OverhangAfter * 1.5;
                }

                Point textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                var textSize = this.MeasureString(text, emSize, typeface);

                //float bboxWidth1 = (float)formattedText.Width;
                double bboxWidth = formattedText.WidthIncludingTrailingWhitespace;

                TranslateTransform translateAt = null;

                RotateTransform rotateAt = new RotateTransform(90, ctp.X, ctp.Y);

                if (alignment == TextAlignment.Center)
                {
//                    translateAt = new TranslateTransform(0, -(textSize.Height - textSize.Width/2) / 2);
                    translateAt = new TranslateTransform(0, -(textSize.Height - yCorrection/2) / 2);
                }
                else if (alignment == TextAlignment.Right)
                {
//                    translateAt = new TranslateTransform(0, -(textSize.Height + textSize.Width / 2));
                    translateAt = new TranslateTransform(0, -(textSize.Height + yCorrection / 2));
                }
                if (translateAt != null)
                {
                    TransformGroup group = new TransformGroup();
                    group.Children.Add(rotateAt);
                    group.Children.Add(translateAt);
                    _drawContext.PushTransform(group);
                }
                else
                {
                    _drawContext.PushTransform(rotateAt);
                }

                _drawContext.DrawText(formattedText, textPoint);

                if (alignment == TextAlignment.Center)
                    bboxWidth /= 2f;
                else if (alignment == TextAlignment.Right)
                    bboxWidth = 0;

                //ctp.X += bboxWidth + emSize / 4;
                ctp.X += bboxWidth;

                //if (translateAt != null)
                //{
                //    _textContext.Pop();
                //}
                if (rotateAt != null)
                {
                    _drawContext.Pop();
                }
            }
            else
            {
                RotateTransform rotateAt = new RotateTransform(90, ctp.X, ctp.Y);
                _drawContext.PushTransform(rotateAt);

                double spacing = Convert.ToDouble(letterSpacing);
                for (int i = 0; i < text.Length; i++)
                {
                    FormattedText formattedText = new FormattedText(new string(text[i], 1),
                        textRun.IsLatin ? _context.EnglishCultureInfo : _context.CultureInfo, 
                        stringFormat.Direction, typeface, emSize, textBrush);

                    if (this.IsMeasuring)
                    {
                        this.AddTextWidth(ctp, formattedText.WidthIncludingTrailingWhitespace);
                        continue;
                    }

                    formattedText.TextAlignment = stringFormat.Alignment;
                    formattedText.Trimming = stringFormat.Trimming;

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

                    Point textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                    _drawContext.DrawText(formattedText, textPoint);

                    //float bboxWidth = (float)formattedText.Width;
                    double bboxWidth = formattedText.WidthIncludingTrailingWhitespace;
                    if (alignment == TextAlignment.Center)
                        bboxWidth /= 2f;
                    else if (alignment == TextAlignment.Right)
                        bboxWidth = 0;

                    //ctp.X += bboxWidth + emSize / 4 + spacing;
                    ctp.X += bboxWidth + spacing;
                }

                if (rotateAt != null)
                {
                    _drawContext.Pop();
                }
            }
        }

#endregion

        #region DrawTextRun Method

        private void DrawTextRun(SvgTextContentElement element, ref Point ctp,
            WpfTextRun textRun, double rotate, WpfTextPlacement placement)
        {
            if (textRun == null || textRun.IsEmpty)
                return;

            string text           = textRun.Text; 
            double emSize         = GetComputedFontSize(element);
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
            Brush textBrush       = fillPaint.GetBrush();

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
            TextAlignment alignment = stringFormat.Alignment;

            string letterSpacing = element.GetAttribute("letter-spacing");
            if (string.IsNullOrWhiteSpace(letterSpacing))
            {   
                FormattedText formattedText = new FormattedText(text,
                    textRun.IsLatin ? _context.EnglishCultureInfo : _context.CultureInfo,
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

                Point textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                RotateTransform rotateAt = new RotateTransform(90, ctp.X, ctp.Y);
                _drawContext.PushTransform(rotateAt);

                _drawContext.DrawText(formattedText, textPoint);

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
            else
            {
                RotateTransform rotateAt = new RotateTransform(90, ctp.X, ctp.Y);
                _drawContext.PushTransform(rotateAt);

                float spacing = Convert.ToSingle(letterSpacing);
                for (int i = 0; i < text.Length; i++)
                {
                    FormattedText formattedText = new FormattedText(new string(text[i], 1),
                        textRun.IsLatin ? _context.EnglishCultureInfo : _context.CultureInfo, 
                        stringFormat.Direction, 
                        new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                        emSize, textBrush);

                    formattedText.Trimming      = stringFormat.Trimming;
                    formattedText.TextAlignment = stringFormat.Alignment;

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

                    Point textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                    _drawContext.DrawText(formattedText, textPoint);

                    //float bboxWidth = (float)formattedText.Width;
                    double bboxWidth = formattedText.WidthIncludingTrailingWhitespace;
                    if (alignment == TextAlignment.Center)
                        bboxWidth /= 2f;
                    else if (alignment == TextAlignment.Right)
                        bboxWidth = 0;

                    //ctp.X += bboxWidth + emSize / 4 + spacing;
                    ctp.X += bboxWidth + spacing;
                }

                if (rotateAt != null)
                {
                    _drawContext.Pop();
                }
            }
        }

        #endregion

        #region ChangeGlyphOrientation Method

        private double ChangeGlyphOrientation(GlyphRunDrawing glyphDrawing, 
            double baselineShiftX, double baselineShiftY, bool isLatin)
        {
            if (glyphDrawing == null)
            {
                return 0;
            }
            GlyphRun glyphRun = glyphDrawing.GlyphRun;

            GlyphRun verticalRun = new GlyphRun();
            ISupportInitialize glyphInit = verticalRun;
            glyphInit.BeginInit();

            verticalRun.IsSideways = true;

            List<double> advancedHeights = null;

            double totalHeight = 0;

            IList<ushort> glyphIndices = glyphRun.GlyphIndices;
            int advancedCount = glyphIndices.Count;
            //{
            //    double textHeight = glyphRun.ComputeInkBoundingBox().Height + glyphRun.ComputeAlignmentBox().Height;
            //    textHeight = textHeight / 2.0d;

            //    totalHeight = advancedCount * textHeight;
            //    advancedHeights = new List<double>(advancedCount);
            //    for (int k = 0; k < advancedCount; k++)
            //    {
            //        advancedHeights.Add(textHeight);
            //    }
            //}
            advancedHeights = new List<double>(advancedCount);
            IDictionary<ushort, double> allGlyphHeights = glyphRun.GlyphTypeface.AdvanceHeights;
            double fontSize = glyphRun.FontRenderingEmSize;
            for (int k = 0; k < advancedCount; k++)
            {
                double tempValue = allGlyphHeights[glyphIndices[k]] * fontSize;
                advancedHeights.Add(tempValue);
                totalHeight += tempValue;
            }

            Point baselineOrigin = glyphRun.BaselineOrigin;
            if (isLatin)
            {
                baselineOrigin.X += baselineShiftX;
                baselineOrigin.Y += baselineShiftY;
            }

            //verticalRun.AdvanceWidths     = glyphRun.AdvanceWidths;
            verticalRun.AdvanceWidths       = advancedHeights;
            verticalRun.BaselineOrigin      = baselineOrigin;
            verticalRun.BidiLevel           = glyphRun.BidiLevel;
            verticalRun.CaretStops          = glyphRun.CaretStops;
            verticalRun.Characters          = glyphRun.Characters;
            verticalRun.ClusterMap          = glyphRun.ClusterMap;
            verticalRun.DeviceFontName      = glyphRun.DeviceFontName;
            verticalRun.FontRenderingEmSize = glyphRun.FontRenderingEmSize;
            verticalRun.GlyphIndices        = glyphRun.GlyphIndices;
            verticalRun.GlyphOffsets        = glyphRun.GlyphOffsets;
            verticalRun.GlyphTypeface       = glyphRun.GlyphTypeface;
            verticalRun.Language            = glyphRun.Language;

            glyphInit.EndInit();

            glyphDrawing.GlyphRun = verticalRun;

            return totalHeight;
        }

        #endregion

        #endregion
    }
}
