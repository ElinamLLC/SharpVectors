using System.Collections.Generic;
using System.Windows.Media;

using SharpVectors.Renderers;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Test.Sample
{
    public class ResourceSvgConverterSample
    {
        private WpfResourceSettings _resourceSettings;

        public ResourceSvgConverterSample()
        {
            // Create the resource settings or options
            _resourceSettings = new WpfResourceSettings();
            _resourceSettings.ResourceFreeze = false; // Do not freeze

            // Initialize the default key resolver and register it
            var resolver = new ResourceKeyResolver("icon_${name}");
            _resourceSettings.RegisterResolver(resolver);

            // Add predefined color palette
            _resourceSettings.ColorPalette = new Dictionary<Color, string>(WpfDrawingResources.ColorComparer)
            {
                {(Color)ColorConverter.ConvertFromString("#FF008000"), "SvgColor01"},
                {(Color)ColorConverter.ConvertFromString("#FF000000"), "SvgColor02"},
                {(Color)ColorConverter.ConvertFromString("#FFFFFF00"), "SvgColor03"},
                {(Color)ColorConverter.ConvertFromString("#FF0000FF"), "SvgColor04"},
                {(Color)ColorConverter.ConvertFromString("#FF00FF00"), "SvgColor05"},
                {(Color)ColorConverter.ConvertFromString("#FF339966"), "SvgColor06"},
                {(Color)ColorConverter.ConvertFromString("#FFFF00FF"), "SvgColor07"},
                {(Color)ColorConverter.ConvertFromString("#FFFFA500"), "SvgColor08"},
                {(Color)ColorConverter.ConvertFromString("#FF007700"), "SvgColor09"},
                {(Color)ColorConverter.ConvertFromString("#FF33CC66"), "SvgColor10"}
            };

            // Add directories as SVG source
            _resourceSettings.AddSource(@"C:\Abc-Project\Icons1");
            _resourceSettings.AddSource(@"C:\Abc-Project\Icons2");
        }

        public string Save()
        {
            // Serialize the resource settings to XML
            return _resourceSettings.Save();
        }
    }
}

    //public class ResourceSvgConverterSample
    //{
    //    private WpfResourceSettings _resourceSettings;

    //    public ResourceSvgConverterSample()
    //    {
    //        // Create the resource settings or options
    //        _resourceSettings = new WpfResourceSettings();
    //        _resourceSettings.ResourceFreeze = false; // Do not freeze

    //        // Initialize the custom key resolver and register it
    //        var resolver = new CustomResourceKeyResolver();
    //        _resourceSettings.RegisterResolver(resolver);
    //    }

    //    public string Convert(string svgDir)
    //    {
    //        // Add a directory as SVG source
    //        _resourceSettings.AddSource(svgDir);

    //        // Create the resource converter
    //        var converter = new ResourceSvgConverter(_resourceSettings);

    //        // Perform the conversion to ResourceDictionary XAML
    //        return converter.Convert();
    //    }
    //}
