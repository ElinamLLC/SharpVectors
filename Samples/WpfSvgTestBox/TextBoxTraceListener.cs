using System;
using System.Diagnostics;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

using Notification.Wpf;

namespace WpfSvgTestBox
{
    public sealed class TextBoxTraceListener : TraceListener
    {
        private delegate void AppendTextDelegate(string msg);

        private const string AppName = "WpfSvgTestBox.exe";

        private TextEditor _textBox;
        private SelectedSegmentBackgroundRenderer _backgroundRenderer;

        private NotificationManager _notifyIcon;

        public TextBoxTraceListener(TextEditor textBox)
        {
            _textBox = textBox;

            _notifyIcon = new NotificationManager();

            _backgroundRenderer = new SelectedSegmentBackgroundRenderer();
            textBox.TextArea.TextView.BackgroundRenderers.Add(_backgroundRenderer);
        }

        public override void Write(string message)
        {
            SetText(message);
        }

        public override void WriteLine(string message)
        {
            if (message == null)
            {
                return;
            }
            SetText(message + Environment.NewLine);
        }

        protected override void Dispose(bool disposing)
        {
            if (_notifyIcon != null)
            {
                //_notifyIcon.Visible = false;
                _notifyIcon.Close();
            }

            _textBox = null;
            _notifyIcon = null;

            base.Dispose(disposing);
        }

        private void SetText(string message)
        {
            if (_textBox == null || message == null)
            {
                return;
            }

            if (!_textBox.Dispatcher.CheckAccess())
            {
                _textBox.Dispatcher.Invoke(new AppendTextDelegate(SetText), message);
                return;
            }

            if (message.StartsWith(AppName, StringComparison.OrdinalIgnoreCase))
            {
                message = message.Remove(0, AppName.Length + 1);
            }

            var selectionStart = _textBox.Document.TextLength;

            if (message.StartsWith("Error", StringComparison.OrdinalIgnoreCase))
            {
                if (_notifyIcon != null)
                {
                    _notifyIcon.Show(MainWindow.AppTitle, "There is a reported error or exception. See the Debug Window.",
                        NotificationType.Error, "", TimeSpan.FromSeconds(5));
                }

                _textBox.SelectionStart = _textBox.Document.TextLength;
                _textBox.SelectionLength = 0;

                _textBox.AppendText(message);
                _textBox.Select(selectionStart, message.TrimEnd().Length);

                IEnumerable<SelectionSegment> selectionSegments = _textBox.TextArea.Selection.Segments;
                TextDocument document = _textBox.TextArea.Document;
                foreach (SelectionSegment segment in selectionSegments)
                {
                    _backgroundRenderer.SelectedSegments.Add(new SelectedSegment(segment, SelectedType.Error));
                }
            }
            else if (message.StartsWith("Warn", StringComparison.OrdinalIgnoreCase))
            {
                if (_notifyIcon != null)
                {
                    _notifyIcon.Show(MainWindow.AppTitle, "There is a reported warning. See the Debug Window.",
                        NotificationType.Warning, "", TimeSpan.FromSeconds(5));
                }
                _textBox.SelectionStart = _textBox.Document.TextLength;
                _textBox.SelectionLength = 0;

                _textBox.AppendText(message);
                _textBox.Select(selectionStart, message.TrimEnd().Length);

                IEnumerable<SelectionSegment> selectionSegments = _textBox.TextArea.Selection.Segments;
                TextDocument document = _textBox.TextArea.Document;
                foreach (SelectionSegment segment in selectionSegments)
                {
                    _backgroundRenderer.SelectedSegments.Add(new SelectedSegment(segment, SelectedType.Warning));
                }
            }
            else
            {
                _textBox.AppendText(message);
            }

            _textBox.Focus();
            _textBox.Select(_textBox.Document.TextLength, 0);
            _textBox.SelectionStart = _textBox.Document.TextLength;
            _textBox.ScrollToEnd();
        }

        private enum SelectedType
        {
            None,
            Error,
            Warning,
            Information
        }

        private sealed class SelectedSegment
        {
            private SelectedType _selectedType;
            private ISegment _textSegment;

            public SelectedSegment(ISegment textSegment, SelectedType selectedType)
            {
                _textSegment  = textSegment;
                _selectedType = selectedType;
            }

            public ISegment Segment
            {
                get {
                    return _textSegment;
                }
            }

            public SelectedType SelectedType
            {
                get {
                    return _selectedType;
                }
            }
        }

        private sealed class SelectedSegmentBackgroundRenderer : IBackgroundRenderer
        {
            private IList<SelectedSegment> _selectedSegments;

            private Brush _errorBrush;
            private Pen _errorPen;

            private Brush _warnBrush;
            private Pen _warnPen;

            public SelectedSegmentBackgroundRenderer()
            {
                _selectedSegments = new List<SelectedSegment>();

                _errorBrush       = Brushes.LightPink;
                _errorPen         = new Pen(_errorBrush, 1);

                _warnBrush        = Brushes.LightGoldenrodYellow;
                _warnPen          = new Pen(_warnBrush, 1);
            }

            public IList<SelectedSegment> SelectedSegments
            {
                get { return _selectedSegments; }
            }

            public KnownLayer Layer
            {
                get {
                    // draw behind selection
                    return KnownLayer.Selection;
                }
            }

            public Brush ErrorBrush
            {
                get { return _errorBrush; }
                set {
                    if (value != null)
                    {
                        _errorBrush = value;
                        _errorPen   = new Pen(_errorBrush, 1);
                    }
                }
            }

            public Brush WarningBrush
            {
                get { return _warnBrush; }
                set {
                    if (value != null)
                    {
                        _warnBrush = value;
                        _warnPen   = new Pen(_warnBrush, 1);
                    }
                }
            }

            public void Draw(TextView textView, DrawingContext drawingContext)
            {
                if (textView == null)
                    throw new ArgumentNullException("textView");
                if (drawingContext == null)
                    throw new ArgumentNullException("drawingContext");

                if (_selectedSegments == null || !textView.VisualLinesValid)
                    return;

                var visualLines = textView.VisualLines;
                if (visualLines.Count == 0 || _selectedSegments.Count == 0)
                    return;

                foreach (SelectedSegment result in _selectedSegments)
                {
                    var selectedType = result.SelectedType;
                    if (selectedType == SelectedType.None || selectedType == SelectedType.Information)
                    {
                        continue;
                    }

                    var geoBuilder = new BackgroundGeometryBuilder();
                    geoBuilder.AlignToWholePixels = true;
                    geoBuilder.BorderThickness    = _errorPen != null ? _errorPen.Thickness : 0;
                    geoBuilder.CornerRadius       = 3;
                    geoBuilder.AddSegment(textView, result.Segment);

                    var geometry = geoBuilder.CreateGeometry();
                    if (geometry != null && !geometry.IsEmpty())
                    {
                        if (selectedType == SelectedType.Error)
                        {
                            drawingContext.DrawGeometry(_errorBrush, _errorPen, geometry);
                        }
                        else if (selectedType == SelectedType.Warning)
                        {
                            drawingContext.DrawGeometry(_warnBrush, _warnPen, geometry);
                        }
                    }
                }
            }
        }
    }
}
