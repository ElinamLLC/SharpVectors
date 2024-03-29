﻿using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit.Indentation;

using SharpVectors.Runtime;
using SharpVectors.Converters;
using SharpVectors.Renderers;
using SharpVectors.Renderers.Wpf;

using FolderBrowserDialog = ShellFileDialogs.FolderBrowserDialog;

using Microsoft.Win32;
using DpiScale = SharpVectors.Runtime.DpiScale;
using DpiUtilities = SharpVectors.Runtime.DpiUtilities;

using SharpVectors.Test.Sample;

namespace WpfSvgTestBox
{
    public sealed class ImageData
    {
        private int _index;
        private string _fileName;
        private DrawingGroup _drawing;

        private string _resourceName;

        public ImageData()
        {
        }

        public ImageData(int index, DrawingGroup drawing, string fileName)
        {
            _index = index;
            _drawing = drawing;
            _fileName = fileName;
        }

        public int Index
        {
            get {
                return _index;
            }
        }

        public string Name
        {
            get {
                return string.Format("{0:D3}: {1}", _index + 1, _fileName);
            }
        }

        public string FileName
        {
            get {
                return _fileName;
            }
        }

        public DrawingGroup Drawing
        {
            get {
                return _drawing;
            }
        }

        public DrawingImage Image
        {
            get {
                return new DrawingImage(_drawing);
            }
        }

        public string ResourceName
        {
            get {
                if (string.IsNullOrWhiteSpace(_resourceName))
                {
                    return _fileName;
                }
                return _resourceName;
            }
            set {
                _resourceName = value;
            }
        }
    }

    public sealed class ResourceData
    {
        private ResourceModeType _resourceMode;
        private string _fileName;

        private DpiScale _dpiScale;

        public ResourceData()
        {
            _resourceMode = ResourceModeType.None;
        }

        public ResourceData(string fileName, ResourceModeType resourceMode, DpiScale dpiScale)
        {
            _fileName     = fileName;
            _resourceMode = resourceMode;
            _dpiScale     = dpiScale;
        }

        public string Name
        {
            get {
                return _fileName;
            }
        }

        public DrawingImage Image
        {
            get {
                if (!string.IsNullOrWhiteSpace(_fileName))
                {
                    if (_resourceMode == ResourceModeType.Drawing)
                    {
                        var drawing = Application.Current.TryFindResource(_fileName) as DrawingGroup;
                        if (drawing != null)
                        {
                            return new DrawingImage(drawing);
                        }
                    }
                    else if (_resourceMode == ResourceModeType.Image)
                    {
                        var image = Application.Current.TryFindResource(_fileName) as DrawingImage;
                        if (image != null)
                        {
                            return image;
                        }
                    }
                    return new DrawingImage(this.CreateMessageText(_fileName));
                }
                return new DrawingImage();
            }
        }

        // Convert the text string to a geometry and draw it to the control's DrawingContext.
        private DrawingGroup CreateMessageText(string messageText)
        {
            double opacity = 1.0;

            var fontFamily = SystemFonts.MessageFontFamily;
            var fontSize = 24.0f;
            Brush fillBrush = Brushes.Gold;
            Brush strokeBrush = Brushes.Maroon;
            if ((fillBrush == null && strokeBrush == null) || opacity <= 0
                || fontFamily == null || fontSize <= 3)
            {
                return null;
            }
            if (strokeBrush == null)
            {
                strokeBrush = Brushes.Transparent;
            }

            // Create a new DrawingGroup of the control.
            DrawingGroup drawingGroup = new DrawingGroup();

            drawingGroup.Opacity = opacity;

            // Open the DrawingGroup in order to access the DrawingContext.
            using (DrawingContext drawingContext = drawingGroup.Open())
            {
                // Create the formatted text based on the properties set.
                FormattedText formattedText = null;
#if DOTNET40 || DOTNET45 || DOTNET46
                formattedText = new FormattedText(messageText,
                    System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                    fontSize, Brushes.Black);
#else
                formattedText = new FormattedText(messageText,
                    System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                    fontSize, Brushes.Black, _dpiScale.PixelsPerDip);
#endif

                // Build the geometry object that represents the text.
                Geometry textGeometry = formattedText.BuildGeometry(new Point(20, 0));

                // Draw a rounded rectangle under the text that is slightly larger than the text.
                var backgroundBrush = Brushes.PapayaWhip;
                if (backgroundBrush != null)
                {
                    drawingContext.DrawRoundedRectangle(backgroundBrush, null,
                        new Rect(new Size(formattedText.Width + 50, formattedText.Height + 5)), 5.0, 5.0);
                }

                // Draw the outline based on the properties that are set.
                drawingContext.DrawGeometry(fillBrush, new Pen(strokeBrush, 1.5), textGeometry);

                // Return the updated DrawingGroup content to be used by the control.
                return drawingGroup;
            }
        }
    }

    /// <summary>
    /// Interaction logic for SvgResourcePage.xaml
    /// </summary>
    public partial class SvgResourcePage : Page
    {
        private const string AppTitle      = "Svg Test Box";
        private const string AppErrorTitle = "Svg Test Box - Error";

        private delegate void AppendImageDelegate(DrawingGroup drawing, string fileName);

        private FoldingManager _foldingManager;
        private XmlFoldingStrategy _foldingStrategy;

        private readonly SearchPanel _searchPanel;

        private int _imageCount;

        private string _xamlText;
        private ResourceDictionary _resourceDictionary;
        private ResourceDictionary _mergedResourceDictionary;

        private ObservableCollection<ImageData> _imageList;
        private ObservableCollection<ResourceData> _resourceList;

        private FileSvgReader _fileReader;

        private DpiScale _dpiScale;

        private WpfDrawingSettings _conversionSettings;
        private WpfResourceSettings _resourceSettings;
        private WpfDrawingResources _drawingResources;

        private MainWindow _mainWindow;
        private bool _resourcesHasColors;
        private SvgResourceColors _colorsDlg;
        private IList<SvgResourceColor> _resourceColors;

        public SvgResourcePage()
        {
            InitializeComponent();

            _imageList    = new ObservableCollection<ImageData>();
            _resourceList = new ObservableCollection<ResourceData>();
            this.DataContext = this;

            _conversionSettings = new WpfDrawingSettings();
            _conversionSettings.CultureInfo     = _conversionSettings.NeutralCultureInfo;
            _conversionSettings.IncludeRuntime  = false;
            _conversionSettings.TextAsGeometry  = true;
            _conversionSettings.InteractiveMode = SvgInteractiveModes.None;

            _conversionSettings[WpfDrawingSettings.PropertyIsResources] = true;

            _fileReader = new FileSvgReader(_conversionSettings);
            _fileReader.SaveXaml = false;
            _fileReader.SaveZaml = false;

            _resourceSettings = new WpfResourceSettings();

            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
            TextEditorOptions options = textEditor.Options;
            if (options != null)
            {
                //options.AllowScrollBelowDocument = true;
                options.EnableHyperlinks           = true;
                options.EnableEmailHyperlinks      = true;
                options.EnableVirtualSpace         = false;
                options.HighlightCurrentLine       = true;
                //options.ShowSpaces               = true;
                //options.ShowTabs                 = true;
                //options.ShowEndOfLine            = true;              
            }

            textEditor.ShowLineNumbers = true;

            _foldingManager = FoldingManager.Install(textEditor.TextArea);
            _foldingStrategy = new XmlFoldingStrategy();

            _searchPanel = SearchPanel.Install(textEditor);

            textEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();

            this.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);

            this.Loaded += OnPageLoaded;
            this.SizeChanged += OnPageSizeChanged;
        }

        public WpfDrawingSettings ConversionSettings
        {
            get {
                return _conversionSettings;
            }
            set {
                if (value != null)
                {
                    _conversionSettings = value;

                    _conversionSettings.CultureInfo     = _conversionSettings.NeutralCultureInfo;
                    _conversionSettings.InteractiveMode = SvgInteractiveModes.None;

                    _conversionSettings[WpfDrawingSettings.PropertyIsResources] = true;

                    // Recreated the conveter
                    _fileReader = new FileSvgReader(_conversionSettings);
                    _fileReader.SaveXaml = true;
                    _fileReader.SaveZaml = false;

                    string svgPath = txtSvgSource.Text;
                    this.OnClearAllClicked(null, null);

                    txtSvgSource.Text = svgPath;
                }
            }
        }

        public WpfResourceSettings ResourceSettings
        {
            get {
                return _resourceSettings;
            }
            set {
                if (value != null)
                {
                    _resourceSettings = value;

                    string svgPath = txtSvgSource.Text;
                    this.OnClearAllClicked(null, null);

                    var svgDir = value.Sources.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(svgDir) && Directory.Exists(svgDir))
                    {
                        txtSvgSource.Text = svgDir;
                    }
                    else
                    {
                        txtSvgSource.Text = svgPath;
                    }
                }
            }
        }

        public ObservableCollection<ImageData> ImageList
        {
            get {
                return _imageList;
            }
        }

        public ObservableCollection<ResourceData> ResourceList
        {
            get {
                return _resourceList;
            }
        }

        public MainWindow Window
        {
            get {
                return _mainWindow;
            }
            set {
                _mainWindow = value;
            }
        }

        private void ReportInfo(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            Trace.TraceInformation(message);

            MessageBox.Show(message, AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AppendInfo(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            Trace.TraceInformation(message);
        }

        private void ReportError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            Trace.TraceError(message);

            MessageBox.Show(message, AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ReportError(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            Trace.TraceError(ex.ToString());

            MessageBox.Show(ex.ToString(), AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnLoadClicked(object sender, RoutedEventArgs e)
        {
            string svgDir = txtSvgSource.Text;
            if (string.IsNullOrWhiteSpace(svgDir) || Directory.Exists(svgDir) == false)
            {
                string selectedDirectory = FolderBrowserDialog.ShowDialog(IntPtr.Zero,
                    "Select the location of the SVG files", null);
                if (selectedDirectory != null)
                {
                    txtSvgSource.Text = selectedDirectory;
                    svgDir = selectedDirectory;
                }
                else
                {
                    return;
                }
            }

            _imageList.Clear();
            _resourceList.Clear();
            _xamlText = string.Empty;
            textEditor.Text = string.Empty;
            _resourceColors = null;

            if (_mergedResourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(_mergedResourceDictionary);
                _mergedResourceDictionary = null;
            }

            btnSaveXaml.IsEnabled = false;
            btnCopyXaml.IsEnabled = false;
            btnChangeColors.IsEnabled = false;
            btnConvertResources.IsEnabled = false;

            _resourceSettings.ClearSources();
            _resourceSettings.AddSource(svgDir);

            _drawingResources = new WpfDrawingResources();
            _conversionSettings.DrawingResources = _drawingResources;

            foreach (var svgFilePath in Directory.EnumerateFiles(svgDir, "*.svg"))
            {
                // Eliminate any compressed file, not supported in this test...
                if (svgFilePath.EndsWith(".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                UpdateDrawing(svgFilePath);
            }

            btnConvertResources.IsEnabled = _imageList.Count != 0;
            btnTestXaml.IsEnabled = _imageList.Count != 0;
        }

        private void UpdateDrawing(string svgFilePath)
        {
            using (var textReader = new StreamReader(svgFilePath))
            {
                _imageCount++;
                try
                {
                    var drawing = _fileReader.Read(textReader);

                    AppendImage(drawing, Path.GetFileNameWithoutExtension(svgFilePath));
                }
                catch (Exception ex)
                {
                    ReportError("File: " + svgFilePath);
                    ReportError(ex);
                }
            }
        }

        private void AppendImage(DrawingGroup drawing, string fileName)
        {
            if (Dispatcher.CheckAccess())
            {
                _imageList.Add(new ImageData(_imageList.Count, drawing, fileName));
            }
            else
            {
                Dispatcher.Invoke(new AppendImageDelegate(AppendImage), drawing, fileName);
            }
        }

        private void OnConvertClicked(object sender, RoutedEventArgs e)
        {
            _xamlText = string.Empty;

            textEditor.Clear();
            _resourceList.Clear();

            if (_imageList == null || _imageList.Count == 0)
            {
                return;
            }

            _resourceDictionary = new ResourceDictionary();

            if (_drawingResources == null)
            {
                _drawingResources = _conversionSettings.DrawingResources;
            }

            var colorPalette = new Dictionary<Color, string>(WpfDrawingResources.ColorComparer);
            colorPalette.Add((Color)ColorConverter.ConvertFromString("#FF008000"), "SvgColor01");
            colorPalette.Add((Color)ColorConverter.ConvertFromString("#FF000000"), "SvgColor02");
            colorPalette.Add((Color)ColorConverter.ConvertFromString("#FFFFFF00"), "SvgColor03");
            colorPalette.Add((Color)ColorConverter.ConvertFromString("#FF0000FF"), "SvgColor04");
            colorPalette.Add((Color)ColorConverter.ConvertFromString("#FF00FF00"), "SvgColor05");
            colorPalette.Add((Color)ColorConverter.ConvertFromString("#FF339966"), "SvgColor06");
            colorPalette.Add((Color)ColorConverter.ConvertFromString("#FFFF00FF"), "SvgColor07");
            colorPalette.Add((Color)ColorConverter.ConvertFromString("#FFFFA500"), "SvgColor08");
            colorPalette.Add((Color)ColorConverter.ConvertFromString("#FF007700"), "SvgColor09");
            colorPalette.Add((Color)ColorConverter.ConvertFromString("#FF33CC66"), "SvgColor10");
            _resourceSettings.ColorPalette = colorPalette;

            _resourceSettings.CopyTo(_drawingResources);
            _drawingResources.InitialiseKeys();
            var resourceFreeze = _resourceSettings.ResourceFreeze;

            var keyResolver = _resourceSettings.RetrieveResolver();
            if (keyResolver == null || keyResolver.IsValid == false)
            {
                keyResolver = new ResourceKeyResolver();
            }

            keyResolver.BeginResolve();
            int itemCount = _resourceSettings.UseResourceIndex ? 0 : 1;
            foreach (var imageData in _imageList)
            {
                var drawGroup = imageData.Drawing;
                if (drawGroup.Children.Count == 1)
                {
                    drawGroup = (DrawingGroup)drawGroup.Children[0];
                    string drawingName = drawGroup.GetValue(FrameworkElement.NameProperty) as string;
                    if (!string.IsNullOrWhiteSpace(drawingName) && string.Equals(drawingName, SvgObject.DrawLayer))
                    {
                        drawGroup.SetValue(FrameworkElement.NameProperty, null);
                    }
                }

                string resourceName = string.Empty;
                if (_drawingResources.ResourceMode == ResourceModeType.Image)
                {
                    var drawImage = new DrawingImage(drawGroup);
                    resourceName = keyResolver.Resolve(drawImage, itemCount, imageData.FileName, imageData.FileName);
                    if (resourceFreeze)
                    {
                        drawImage.Freeze();
                    }
                    _resourceDictionary.Add(resourceName, drawImage);
                }
                else
                {
                    resourceName = keyResolver.Resolve(drawGroup, itemCount, imageData.FileName, imageData.FileName);
                    if (resourceFreeze)
                    {
                        drawGroup.Freeze();
                    }
                    _resourceDictionary.Add(resourceName, drawGroup);
                }
                imageData.ResourceName = resourceName;

                itemCount++;
            }

            XmlXamlWriter writer = new XmlXamlWriter(_conversionSettings);
            writer.NumberDecimalDigits = _resourceSettings.NumericPrecision;
            writer.IndentSpaces = _resourceSettings.IndentSpaces;

            _xamlText = writer.Save(_resourceDictionary);

            textEditor.Text = _xamlText;

            try
            {
                _mergedResourceDictionary = (ResourceDictionary)XamlReader.Parse(_xamlText);
                if (_mergedResourceDictionary == null)
                {
                    return;
                }

                Application.Current.Resources.MergedDictionaries.Add(_mergedResourceDictionary);

                foreach (var imageData in _imageList)
                {
                    _resourceList.Add(new ResourceData(imageData.ResourceName, _drawingResources.ResourceMode, _dpiScale));
                }

                btnSaveXaml.IsEnabled = _resourceList.Count != 0;
                btnCopyXaml.IsEnabled = _resourceList.Count != 0;
                btnChangeColors.IsEnabled = _resourceList.Count != 0;

                btnTestXaml.IsEnabled = _resourceList.Count != 0;
            }
            catch (Exception ex)
            {
                _mergedResourceDictionary = null;

                ReportError(ex);
            }
        }

        private void OnClearAllClicked(object sender, RoutedEventArgs e)
        {
            _imageList.Clear();
            _resourceList.Clear();

            _resourceColors = null;

            _xamlText         = string.Empty;
            textEditor.Text   = string.Empty;
            txtSvgSource.Text = string.Empty;

            btnSaveXaml.IsEnabled = false;
            btnCopyXaml.IsEnabled = false;
            btnChangeColors.IsEnabled = false;
            btnConvertResources.IsEnabled = false;

            btnTestXaml.IsEnabled = false;

            if (_mergedResourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(_mergedResourceDictionary);
                _mergedResourceDictionary = null;
            }
        }

        private void OnChangeColorsClicked(object sender, RoutedEventArgs e)
        {
            if (_drawingResources == null || _drawingResources.IsReady == false)
            {
                return;
            }
            if (_mergedResourceDictionary == null || _mergedResourceDictionary.Count == 0)
            {
                return;
            }

            if (_resourceColors == null || _resourceColors.Count == 0)
            {
                _resourceColors = new List<SvgResourceColor>();

                ICollection<string> colorKeys = _drawingResources.ColorKeys;
                ICollection<string> brushKeys = _drawingResources.BrushKeys;
                if (colorKeys != null && colorKeys.Count != 0)
                {
                    _resourcesHasColors = true;
                    foreach (var colorKey in colorKeys)
                    {
                        if (_mergedResourceDictionary.Contains(colorKey))
                        {
                            object colorValue = _mergedResourceDictionary[colorKey];
                            if (colorValue != null)
                            {
                                _resourceColors.Add(new SvgResourceColor(colorKey, (Color)colorValue));
                            }
                        }
                    }
                }
                else if (brushKeys != null && brushKeys.Count != 0)
                {
                    _resourcesHasColors = false;
                    foreach (var brushKey in brushKeys)
                    {
                        if (_mergedResourceDictionary.Contains(brushKey))
                        {
                            SolidColorBrush brush = (SolidColorBrush)_mergedResourceDictionary[brushKey];
                            if (brush != null)
                            {
                                _resourceColors.Add(new SvgResourceColor(brushKey, brush.Color));
                            }
                        }
                    }
                }
            }

            if (_resourceColors == null || _resourceColors.Count == 0)
            {
                return;
            }
            _colorsDlg = new SvgResourceColors(_resourceColors);
            _colorsDlg.Owner = this.Window;

            _colorsDlg.Closed += OnResourceColorClosed;

            _colorsDlg.Show();
        }

        private void OnResourceColorClosed(object sender, EventArgs e)
        {
            if (_colorsDlg == null || string.IsNullOrWhiteSpace(_xamlText))
            {
                return;
            }
            _colorsDlg.Closed -= OnResourceColorClosed;

            if (_colorsDlg.DialogResult == false)
            {
                return;
            }
            _colorsDlg = null;

            if (_resourceColors == null || _resourceColors.Count == 0)
            {
                return;
            }

            bool isModified = false;
            var appResources = Application.Current.Resources;
            foreach (var resourceColor in _resourceColors)
            {
                if (resourceColor.IsModified)
                {
                    isModified = true;
                    if (_resourcesHasColors)
                    {
                        appResources[resourceColor.Name] = resourceColor.SelectedColor;
                    }
                    else
                    {
                        var brush = new SolidColorBrush(resourceColor.SelectedColor);
                        appResources[resourceColor.Name] = brush;
                    }
                }
            }

            if (isModified)
            {
                try
                {
                    _mergedResourceDictionary = (ResourceDictionary)XamlReader.Parse(_xamlText);
                    if (_mergedResourceDictionary == null)
                    {
                        return;
                    }

                    Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(_mergedResourceDictionary);

                    _resourceList.Clear();
                    foreach (var imageData in _imageList)
                    {
                        _resourceList.Add(new ResourceData(imageData.ResourceName, _drawingResources.ResourceMode, _dpiScale));
                    }
                }
                catch (Exception ex)
                {
                    _mergedResourceDictionary = null;

                    ReportError(ex);
                }
            }
        }

        private void OnSessionClicked(object sender, RoutedEventArgs e)
        {
            if (_resourceSettings == null)
            {
                return;
            }
            var converter = new ResourceSvgConverterSample();
            XmlViewerDialog dlg = new XmlViewerDialog();
            dlg.Owner = _mainWindow;
            dlg.XmlType = XmlViewerType.Xml;
            //dlg.XmlText = _resourceSettings.Save();
            dlg.XmlText = converter.Save();

            dlg.ShowDialog();

            //var wpfSettings = new WpfDrawingSettings();
            //wpfSettings.IncludeRuntime = false;
            //wpfSettings.TextAsGeometry = false;

            //wpfSettings.PixelWidth = 120;
            //wpfSettings.PixelHeight = 120;

            //var converter = new ImageSvgConverter(wpfSettings);
            //converter.Convert(@"D:\Visual Studio\Workspaces\SharpVectors\DocFx\images\about.svg");
            //converter.Convert(@"D:\Visual Studio\Workspaces\SharpVectors\DocFx\images\area_chart.svg");
            //converter.Convert(@"D:\Visual Studio\Workspaces\SharpVectors\DocFx\images\crystal_oscillator.svg");

            //Clipboard.SetText(converter.Convert(@"C:\Users\Paul Selormey\Desktop\icons2"), TextDataFormat.UnicodeText);
        }

        private void OnCopyXamlClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_xamlText))
            {
                return;
            }

            Clipboard.SetText(_xamlText, TextDataFormat.UnicodeText);
        }

        private void OnSaveXamlClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_xamlText))
            {
                return;
            }

            string selectedFile = null;
            SaveFileDialog dlg = new SaveFileDialog();
            string svgDir = txtSvgSource.Text;
            if (!string.IsNullOrWhiteSpace(svgDir) && Directory.Exists(svgDir))
            {
                dlg.InitialDirectory = svgDir;
            }
            dlg.Title      = "Save Resource Dictionary As";
            dlg.Filter     = "XAML Files|*.xaml";
            dlg.FileName   = "TestDictionary.xaml";
            dlg.DefaultExt = ".xaml";
            if (dlg.ShowDialog() ?? false)
            {
                selectedFile = dlg.FileName;
            }
            else
            {
                return;
            }

            using (var textWriter = File.CreateText(selectedFile))
            {
                textWriter.WriteLine(_xamlText);
            }
        }

        private void OnTestXamlClicked(object sender, RoutedEventArgs e)
        {
            var resourceConverter = new ResourceSvgConverter(_conversionSettings, _resourceSettings);

            var xamlText = resourceConverter.Convert();
            if (string.IsNullOrWhiteSpace(xamlText))
            {
                return;
            }

            XmlViewerDialog dlg = new XmlViewerDialog();
            dlg.Owner = _mainWindow;
            dlg.XmlType = XmlViewerType.Xaml;
            dlg.XmlText = xamlText;

            dlg.ShowDialog();

            //Clipboard.SetText(xamlText, TextDataFormat.UnicodeText);
        }

        private void UpdateFoldings()
        {
            if (_foldingManager == null || _foldingStrategy == null)
            {
                _foldingManager = FoldingManager.Install(textEditor.TextArea);
                _foldingStrategy = new XmlFoldingStrategy();
            }
            _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            RowDefinition rowTab = rightGrid.RowDefinitions[2];
            rowTab.Height = new GridLength((this.ActualHeight - 8) / 2, GridUnitType.Pixel);

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
            foldingUpdateTimer.Start();

            textEditor.Focus();

            if (_dpiScale == null)
            {
                _dpiScale = DpiUtilities.GetWindowScale(this);
            }
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RowDefinition rowTab = rightGrid.RowDefinitions[2];
            rowTab.Height = new GridLength((this.ActualHeight - 8) / 2, GridUnitType.Pixel);
        }
    }
}
