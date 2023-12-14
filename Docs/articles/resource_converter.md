---
uid: topic_resource_converter
title: ResourceSvgConverter
---

# Resource SVG Converter - ResourceSvgConverter
The **[](xref:SharpVectors.Converters.ResourceSvgConverter)** converter class takes multiple files and directories as input and outputs a
[ResourceDictionary](xref:System.Windows.ResourceDictionary)  in XAML format.
This is useful for vector icons, but required supports in the rendering process to track brushes and pens, and an extension to the 
**[](xref:SharpVectors.Converters.XmlXamlWriter)** class to format a compact XAML output.
It is the most recent SharpVector SVG converter and adds extension features that will be supported in the other converters.

> [!NOTE] 
> Currently, the input is restricted to local or network files and directories.
> 
> The output is @System.String or local or network file or one of the following
> * @System.IO.Stream: A stream object created by the user.
> * @System.IO.TextWriter: A text writer object created by the user.
> * @System.Xml.XmlWriter: An XML writer object created by the user

## Resource Options
This converter offers extra settings or options in the form of **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings)** class.

![resource_settings](../images/resource_settings.png)

The following are the properties exposed by the resource options to customize the output resource dictionary XAML:
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.ResourceMode)**: An enumeration specifying the type of the resource object; 
@System.Windows.Media.DrawingGroup or @System.Windows.Media.DrawingImage.
    # [DrawingGroup Resource](#tab/xaml1)
    ```xml
    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:po="http://schemas.microsoft.com/winfx/2006/xaml/presentation/options" 
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" mc:Ignorable="po">
      <DrawingGroup x:Key="shapes-rect-01-t" po:Freeze="True" ClipGeometry="F0M0,0L480,0L480,360L0,360z">
      </DrawingGroup>
    </ResourceDictionary>
    ```
    # [DrawingImage Resource](#tab/xaml2)
    ```xml
    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:po="http://schemas.microsoft.com/winfx/2006/xaml/presentation/options" 
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" mc:Ignorable="po">
      <DrawingImage x:Key="shapes-rect-01-t" po:Freeze="True">
        <DrawingImage.Drawing>
          <DrawingGroup ClipGeometry="F0M0,0L480,0L480,360L0,360z">
            </DrawingGroup>
        </DrawingImage.Drawing>
      </DrawingImage>
    </ResourceDictionary>
    ```
    ***
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.ResourceFreeze)**: Makes the freezable resource object unmodifiable and sets its @System.Windows.Freezable.IsFrozen property to `true`. 
To freeze a resource object declared in markup, the `PresentationOptions:Freeze` attribute is used, as shown above as `po.Freeze`, and the `xmlns:po="http://schemas.microsoft.com/winfx/2006/xaml/presentation/options"` namespace is added.
    > [!NOTE] 
    > If this property is set to `true`, it is admisable to set the `ResourceAccessType` type below to `Static`.
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.IndentSpaces)**: This specifies the number of spaces used for the indentation of the XAML output. It is honored with the @System.Xml.XmlReaderSettings. The default is `2 spaces`.
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.NumericPrecision)**: This specifies numeric precision or number of decimal places for the floating number. The default is `4`, in complaince to the SVG specifications. Setting this to `-1` will disable this property.
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.ColorNameFormat)**: This determines the key name of color objects, if both **BindToResources**  and **BindToColors** properties are enabled. The default is `Color{0}`.
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.BrushNameFormat)**: This determines the key name of brush objects, if the **BindToResources** property is enabled. The default is `Brush{0}`.
    > [!NOTE] 
    > Only solid color brushes are supported and extracted from the drawings.
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.PenNameFormat)**:  This determines the key name of pen or stroke objects, if the **BindToResources** property is enabled. The default is `Pen{0}`
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.BindToResources)**: This determines whether the media basic objects; color, brush and stroke/pen are extracted and the drawing objects bind to them. The default is `true`.
    # [BindToResources == true](#tab/xaml1)
    ```xml
    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
      <Color x:Key="Color1">#FF000000</Color>
      <Color x:Key="Color2">#FF008000</Color>
      <SolidColorBrush x:Key="Brush1" Color="{DynamicResource Color1}" />
      <SolidColorBrush x:Key="Brush2" Color="{DynamicResource Color2}" />
      <Pen x:Key="Pen1" Brush="{DynamicResource Brush2}" Thickness="1" StartLineCap="Flat" EndLineCap="Flat" LineJoin="Miter" />
      <DrawingImage x:Key="simple-path">
        <DrawingImage.Drawing>
          <DrawingGroup ClipGeometry="F0M0,0L110,0L110,110L0,110z">
            <GeometryDrawing Brush="{DynamicResource Brush1}" Pen="{DynamicResource Pen1}" 
            Geometry="F1M10,30A20,20,0,0,1,50,30A20,20,0,0,1,90,30Q90,60 50,90 10,60 10,30z" />
          </DrawingGroup>
        </DrawingImage.Drawing>
      </DrawingImage>
    </ResourceDictionary>
    ```
    # [BindToResources == false](#tab/xaml2)
    ```xml
    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
      <DrawingImage x:Key="simple-path">
        <DrawingImage.Drawing>
          <DrawingGroup ClipGeometry="F0M0,0L110,0L110,110L0,110z">
            <GeometryDrawing Brush="#FF000000">
              <GeometryDrawing.Pen>
                <Pen Brush="#FF008000" Thickness="1" StartLineCap="Flat" EndLineCap="Flat" LineJoin="Miter" />
              </GeometryDrawing.Pen>
              <GeometryDrawing.Geometry>
                <PathGeometry FillRule="Nonzero" Figures="M10,30A20,20,0,0,1,50,30A20,20,0,0,1,90,30Q90,60 50,90 10,60 10,30z" />
              </GeometryDrawing.Geometry>
            </GeometryDrawing>
          </DrawingGroup>
        </DrawingImage.Drawing>
      </DrawingImage>
    </ResourceDictionary>
    ```
    ***
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.BindToColors)**: This determines if the color object of brushes is extracted. The default is `true`.
    # [BindToColors == true](#tab/xaml1)
    ```xml
    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
      <Color x:Key="Color1">#FF000000</Color>
      <Color x:Key="Color2">#FF008000</Color>
      <SolidColorBrush x:Key="Brush1" Color="{DynamicResource Color1}" />
      <SolidColorBrush x:Key="Brush2" Color="{DynamicResource Color2}" />
      <Pen x:Key="Pen1" Brush="{DynamicResource Brush2}" Thickness="1" StartLineCap="Flat" EndLineCap="Flat" LineJoin="Miter" />
      <DrawingImage x:Key="simple-path">
        <DrawingImage.Drawing>
          <DrawingGroup ClipGeometry="F0M0,0L110,0L110,110L0,110z">
            <GeometryDrawing Brush="{DynamicResource Brush1}" Pen="{DynamicResource Pen1}" 
            Geometry="F1M10,30A20,20,0,0,1,50,30A20,20,0,0,1,90,30Q90,60 50,90 10,60 10,30z" />
          </DrawingGroup>
        </DrawingImage.Drawing>
      </DrawingImage>
    </ResourceDictionary>
    ```
    # [BindToColors == false](#tab/xaml2)
    ```xml
    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
      <SolidColorBrush x:Key="Brush1" Color="#FF000000" />
      <SolidColorBrush x:Key="Brush2" Color="#FF008000" />
      <Pen x:Key="Pen1" Brush="{DynamicResource Brush2}" Thickness="1" StartLineCap="Flat" EndLineCap="Flat" LineJoin="Miter" />
      <DrawingImage x:Key="simple-path">
        <DrawingImage.Drawing>
          <DrawingGroup ClipGeometry="F0M0,0L110,0L110,110L0,110z">
            <GeometryDrawing Brush="{DynamicResource Brush1}" Pen="{DynamicResource Pen1}" 
            Geometry="F1M10,30A20,20,0,0,1,50,30A20,20,0,0,1,90,30Q90,60 50,90 10,60 10,30z" />
          </DrawingGroup>
        </DrawingImage.Drawing>
      </DrawingImage>
    </ResourceDictionary>
    ```
    ***
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.BindPenToBrushes)**: This determines if solid color brushes of pen/stroke is extracted. The default is `true`.
    # [BindPenToBrushes == true](#tab/xaml1)
    ```xml
    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
      <Color x:Key="Color1">#FF000000</Color>
      <Color x:Key="Color2">#FF008000</Color>
      <SolidColorBrush x:Key="Brush1" Color="{DynamicResource Color1}" />
      <SolidColorBrush x:Key="Brush2" Color="{DynamicResource Color2}" />
      <Pen x:Key="Pen1" Brush="{DynamicResource Brush2}" Thickness="1" StartLineCap="Flat" EndLineCap="Flat" LineJoin="Miter" />
      <DrawingImage x:Key="simple-path">
        <DrawingImage.Drawing>
          <DrawingGroup ClipGeometry="F0M0,0L110,0L110,110L0,110z">
            <GeometryDrawing Brush="{DynamicResource Brush1}" Pen="{DynamicResource Pen1}" 
            Geometry="F1M10,30A20,20,0,0,1,50,30A20,20,0,0,1,90,30Q90,60 50,90 10,60 10,30z" />
          </DrawingGroup>
        </DrawingImage.Drawing>
      </DrawingImage>
    </ResourceDictionary>
    ```
    # [BindPenToBrushes == false](#tab/xaml2)
    ```xml
    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
      <Color x:Key="Color1">#FF000000</Color>
      <SolidColorBrush x:Key="Brush1" Color="{DynamicResource Color1}" />
      <Pen x:Key="Pen1" Brush="#FF008000" Thickness="1" StartLineCap="Flat" EndLineCap="Flat" LineJoin="Miter" />
      <DrawingImage x:Key="simple-path">
        <DrawingImage.Drawing>
          <DrawingGroup ClipGeometry="F0M0,0L110,0L110,110L0,110z">
            <GeometryDrawing Brush="{DynamicResource Brush1}" Pen="{DynamicResource Pen1}" 
            Geometry="F1M10,30A20,20,0,0,1,50,30A20,20,0,0,1,90,30Q90,60 50,90 10,60 10,30z" />
          </DrawingGroup>
        </DrawingImage.Drawing>
      </DrawingImage>
    </ResourceDictionary>
    ```
    ***
    > [!NOTE] 
    > If **BindToResources** property is set to `false`, then **BindToColors** and **BindPenToBrushes** values have no effect.
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.UseResourceIndex)**: This specifies whether a zero-based numbering is applied to the key names. The default is `false`.
    # [UseResourceIndex == true](#tab/xaml1)
    ```xml
    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
      <Color x:Key="Color0">#FF000000</Color>
      <Color x:Key="Color1">#FF008000</Color>
      <SolidColorBrush x:Key="Brush0" Color="{DynamicResource Color0}" />
      <SolidColorBrush x:Key="Brush1" Color="{DynamicResource Color1}" />
      <Pen x:Key="Pen0" Brush="{DynamicResource Brush1}" Thickness="1" StartLineCap="Flat" EndLineCap="Flat" LineJoin="Miter" />
      <DrawingImage x:Key="simple-path">
        <DrawingImage.Drawing>
          <DrawingGroup ClipGeometry="F0M0,0L110,0L110,110L0,110z">
            <GeometryDrawing Brush="{DynamicResource Brush0}" Pen="{DynamicResource Pen0}" 
            Geometry="F1M10,30A20,20,0,0,1,50,30A20,20,0,0,1,90,30Q90,60 50,90 10,60 10,30z" />
          </DrawingGroup>
        </DrawingImage.Drawing>
      </DrawingImage>
    </ResourceDictionary>
    ```
    # [UseResourceIndex == false](#tab/xaml2)
    ```xml
    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
       xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
      <Color x:Key="Color1">#FF000000</Color>
      <Color x:Key="Color2">#FF008000</Color>
      <SolidColorBrush x:Key="Brush1" Color="{DynamicResource Color1}" />
      <SolidColorBrush x:Key="Brush2" Color="{DynamicResource Color2}" />
      <Pen x:Key="Pen1" Brush="{DynamicResource Brush2}" Thickness="1" StartLineCap="Flat" EndLineCap="Flat" LineJoin="Miter" />
      <DrawingImage x:Key="simple-path">
        <DrawingImage.Drawing>
          <DrawingGroup ClipGeometry="F0M0,0L110,0L110,110L0,110z">
            <GeometryDrawing Brush="{DynamicResource Brush1}" Pen="{DynamicResource Pen1}" 
            Geometry="F1M10,30A20,20,0,0,1,50,30A20,20,0,0,1,90,30Q90,60 50,90 10,60 10,30z" />
          </DrawingGroup>
        </DrawingImage.Drawing>
      </DrawingImage>
    </ResourceDictionary>
    ```
    ***
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.ColorPalette)**: This is a dictionary of color to resource key, specifying a predefined resource keys for the colors.
    # [Color Palette C#](#tab/csharp)
    ```csharp
    // Create the resource settings or options
    var resourceSettings = new WpfResourceSettings() {
        ColorPalette = new Dictionary<Color, string>(WpfDrawingResources.ColorComparer) {
            { (Color)ColorConverter.ConvertFromString("#FF000000"), "IconFill" },
            { (Color)ColorConverter.ConvertFromString("#FF008000"), "IconBorder" },
        }
    };
    // Add a directory as SVG source
    resourceSettings.AddSource(svgDir);

    // Create the resource converter
    var converter = new ResourceSvgConverter(resourceSettings);

    // Perform the conversion to ResourceDictionary XAML
    var xamlText = converter.Convert();
    ```
    # [Color Palette VB.NET](#tab/vb)
    ```vb
    ' Create the resource settings or options
    Dim resourceSettings = New WpfResourceSettings() With {
        .ColorPalette = New Dictionary(Of Color, String)(WpfDrawingResources.ColorComparer) From {
            {CType(ColorConverter.ConvertFromString("#FF000000"), Color), "IconFill"},
            {CType(ColorConverter.ConvertFromString("#FF008000"), Color), "IconBorder"}
        }
    }
    ' Add a directory as SVG source
    resourceSettings.AddSource(svgDir)

    ' Create the resource converter
    Dim converter = New ResourceSvgConverter(resourceSettings)

    ' Perform the conversion to ResourceDictionary XAML
    Dim xamlText = converter.Convert()
    ```
    # [XAML Output](#tab/xaml)
    ```xml
    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
      <Color x:Key="IconFill">#FF000000</Color>
      <Color x:Key="IconBorder">#FF008000</Color>
      <SolidColorBrush x:Key="Brush1" Color="{DynamicResource IconFill}" />
      <SolidColorBrush x:Key="Brush2" Color="{DynamicResource IconBorder}" />
      <Pen x:Key="Pen1" Brush="{DynamicResource Brush2}" Thickness="1" StartLineCap="Flat" EndLineCap="Flat" LineJoin="Miter" />
      <DrawingImage x:Key="simple-path">
        <DrawingImage.Drawing>
          <DrawingGroup ClipGeometry="F0M0,0L110,0L110,110L0,110z">
            <GeometryDrawing Brush="{DynamicResource Brush1}" Pen="{DynamicResource Pen1}" 
            Geometry="F1M10,30A20,20,0,0,1,50,30A20,20,0,0,1,90,30Q90,60 50,90 10,60 10,30z" />
          </DrawingGroup>
        </DrawingImage.Drawing>
      </DrawingImage>
    </ResourceDictionary>
    ```
    ***
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.Sources)**: Gets an enumeration of all the SVG sources to be converted to the resource dictionary.
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.SourceCount)**: Gets the count of the SVG sources to be converted to the resource dictionary.
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.ResourceAccess)**: This specifies the type of the resource access, **DynamicResource** or **StaticResource**, to be applied to the resource objects.
    # [ResourceAccessType.Dynamic](#tab/xaml1)
    ```xml
    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
      <Color x:Key="Color1">#FF000000</Color>
      <Color x:Key="Color2">#FF008000</Color>
      <SolidColorBrush x:Key="Brush1" Color="{DynamicResource Color1}" />
      <SolidColorBrush x:Key="Brush2" Color="{DynamicResource Color2}" />
      <Pen x:Key="Pen1" Brush="{DynamicResource Brush2}" Thickness="1" StartLineCap="Flat" EndLineCap="Flat" LineJoin="Miter" />
      <DrawingImage x:Key="simple-path">
        <DrawingImage.Drawing>
          <DrawingGroup ClipGeometry="F0M0,0L110,0L110,110L0,110z">
            <GeometryDrawing Brush="{DynamicResource Brush1}" Pen="{DynamicResource Pen1}" 
            Geometry="F1M10,30A20,20,0,0,1,50,30A20,20,0,0,1,90,30Q90,60 50,90 10,60 10,30z" />
          </DrawingGroup>
        </DrawingImage.Drawing>
      </DrawingImage>
    </ResourceDictionary>
    ```
    # [ResourceAccessType.Static](#tab/xaml2)
    ```xml
    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
      <Color x:Key="Color1">#FF000000</Color>
      <Color x:Key="Color2">#FF008000</Color>
      <SolidColorBrush x:Key="Brush1" Color="{StaticResource Color1}" />
      <SolidColorBrush x:Key="Brush2" Color="{StaticResource Color2}" />
      <Pen x:Key="Pen1" Brush="{StaticResource Brush2}" Thickness="1" StartLineCap="Flat" EndLineCap="Flat" LineJoin="Miter" />
      <DrawingImage x:Key="simple-path">
        <DrawingImage.Drawing>
          <DrawingGroup ClipGeometry="F0M0,0L110,0L110,110L0,110z">
            <GeometryDrawing Brush="{StaticResource Brush1}" Pen="{StaticResource Pen1}" 
            Geometry="F1M10,30A20,20,0,0,1,50,30A20,20,0,0,1,90,30Q90,60 50,90 10,60 10,30z" />
          </DrawingGroup>
        </DrawingImage.Drawing>
      </DrawingImage>
    </ResourceDictionary>
    ```
    ***
* **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings.ResourceResolverType)**: Get a value specifying the current type of the resource object key resolver, 
which is implemented through the @SharpVectors.Renderers.IResourceKeyResolver interface.
This applies to the main resource objects; @System.Windows.Media.DrawingGroup or @System.Windows.Media.DrawingImage (and not the color, brush or pen objects). 
It returns @SharpVectors.Renderers.ResourceKeyResolverType.None, if no resource key resolver is attached, in which case the default resolver will be used.

### ResourceKeyResolverType - Default
For the illustration of the resource key resolvers, we will assume a directory containing the following three SVG files

| ![](../images/about.png) | ![](../images/area_chart.png) | ![](../images/crystal_oscillator.png) |
|:-------------------------:|:-------------------------------:|:-------------------------------------:|
|          about.svg           |            area_chart.svg         |           crystal_oscillator.svg          |

The following sample code uses the default resource key resolver, or @SharpVectors.Renderers.ResourceKeyResolver, to resolve the keys. If not specified, the keys are
simply the SVG file names without the file extension.
# [C#](#tab/csharp)
```csharp
using System.Windows;
using System.Windows.Media;

using SharpVectors.Renderers;
using SharpVectors.Converters;
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
        }

        public string Convert(string svgDir)
        {
            // Add a directory as SVG source
            _resourceSettings.AddSource(svgDir);

            // Create the resource converter
            var converter = new ResourceSvgConverter(_resourceSettings);

            // Perform the conversion to ResourceDictionary XAML
            return converter.Convert();
        }
    }
}
```
# [VB.NET](#tab/vb)
```vb
Imports System.Windows.Media

Imports SharpVectors.Renderers
Imports SharpVectors.Converters
Imports SharpVectors.Renderers.Wpf

Namespace SharpVectors.Test.Sample
    Public Class ResourceSvgConverterSample
        Private _resourceSettings As WpfResourceSettings

        Public Sub New()
            ' Create the resource settings or options
            _resourceSettings = New WpfResourceSettings()
            _resourceSettings.ResourceFreeze = False ' Do not freeze
        End Sub

        Public Function Convert(ByVal svgDir As String) As String
            ' Add a directory as SVG source
            _resourceSettings.AddSource(svgDir)

            ' Create the resource converter
            Dim converter = New ResourceSvgConverter(_resourceSettings)

            ' Perform the conversion to ResourceDictionary XAML
            Return converter.Convert()
        End Function
    End Class
End Namespace
```
# [XAML](#tab/xaml)
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Color x:Key="Color1">#FF2196F3</Color>
  <Color x:Key="Color2">#FFFFFFFF</Color>
  <Color x:Key="Color3">#FF3F51B5</Color>
  <Color x:Key="Color4">#FF00BCD4</Color>
  <Color x:Key="Color5">#FFFF9800</Color>
  <Color x:Key="Color6">#FF64B5F6</Color>
  <SolidColorBrush x:Key="Brush1" Color="{DynamicResource Color1}" />
  <SolidColorBrush x:Key="Brush2" Color="{DynamicResource Color2}" />
  <SolidColorBrush x:Key="Brush3" Color="{DynamicResource Color3}" />
  <SolidColorBrush x:Key="Brush4" Color="{DynamicResource Color4}" />
  <SolidColorBrush x:Key="Brush5" Color="{DynamicResource Color5}" />
  <SolidColorBrush x:Key="Brush6" Color="{DynamicResource Color6}" />
  <DrawingGroup x:Key="area_chart" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <GeometryDrawing Brush="{DynamicResource Brush3}" Geometry="F1M42,37L42,37 6,37 6,25 16,10 30,17 42,6z" />
    <GeometryDrawing Brush="{DynamicResource Brush4}" Geometry="F1M42,42L42,42 6,42 6,32 16,24 30,26 42,17z" />
  </DrawingGroup>
  <DrawingGroup x:Key="about" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <GeometryDrawing Brush="{DynamicResource Brush1}" Geometry="F1M37,40L11,40 5,46 5,12C5,8.7,7.7,6,11,6L37,
         6C40.3,6,43,8.7,43,12L43,34C43,37.3,40.3,40,37,40z" />
    <DrawingGroup>
      <GeometryDrawing Brush="{DynamicResource Brush2}" Geometry="F0M22,20L26,20 26,31 22,31z" />
      <GeometryDrawing Brush="{DynamicResource Brush2}" Geometry="F0M26,15C26,16.1046,25.1046,17,24,17C22.8954,
         17,22,16.1046,22,15C22,13.8954,22.8954,13,24,13C25.1046,13,26,13.8954,26,15z" />
    </DrawingGroup>
  </DrawingGroup>
  <DrawingGroup x:Key="crystal_oscillator" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <DrawingGroup>
      <GeometryDrawing Brush="{DynamicResource Brush5}" Geometry="F0M3,28L29,28 29,32 3,32z" />
      <GeometryDrawing Brush="{DynamicResource Brush5}" Geometry="F0M3,16L29,16 29,20 3,20z" />
    </DrawingGroup>
    <GeometryDrawing Brush="{DynamicResource Brush1}" Geometry="F1M43,11L20,11 20,37 43,37C44.1,37,45,36.1,
        45,35L45,13C45,11.9,44.1,11,43,11z" />
    <GeometryDrawing Brush="{DynamicResource Brush6}" Geometry="F1M20,9L18,9 18,39 20,39C21.1,39,22,38.1,
        22,37L22,11C22,9.9,21.1,9,20,9z" />
  </DrawingGroup>
</ResourceDictionary>
```
***

The default resource key resolver supports simple string substitution template to customize the resource keys with the following two tags
* **${name}**: Representing the SVG file name without the extension.
* **${number}**: Representing the resource number or index, depending on the value of the @SharpVectors.Renderers.Wpf.WpfResourceSettings.UseResourceIndex property.

For instance, using a key format of **icon_${name}**, an SVG file with the name `about.svg` will be have a resource key: **icon_about**.

The following sample code illustrates how the customized resource key format is applied with the default resolver, and the resulting XAML output:
# [C#](#tab/csharp)
```csharp
using System.Windows;
using System.Windows.Media;

using SharpVectors.Renderers;
using SharpVectors.Converters;
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
        }

        public string Convert(string svgDir)
        {
            // Add a directory as SVG source
            _resourceSettings.AddSource(svgDir);

            // Create the resource converter
            var converter = new ResourceSvgConverter(_resourceSettings);

            // Perform the conversion to ResourceDictionary XAML
            return converter.Convert();
        }
    }
}
```
# [VB.NET](#tab/vb)
```vb
Imports System.Windows.Media

Imports SharpVectors.Renderers
Imports SharpVectors.Converters
Imports SharpVectors.Renderers.Wpf

Namespace SharpVectors.Test.Sample
    Public Class ResourceSvgConverterSample
        Private _resourceSettings As WpfResourceSettings

        Public Sub New()
            ' Create the resource settings or options
            _resourceSettings = New WpfResourceSettings()
            _resourceSettings.ResourceFreeze = False ' Do not freeze

            ' Initialize the default key resolver and register it
            Dim resolver = New ResourceKeyResolver("icon_${name}")
            _resourceSettings.RegisterResolver(resolver)
        End Sub

        Public Function Convert(ByVal svgDir As String) As String
            ' Add a directory as SVG source
            _resourceSettings.AddSource(svgDir)

            ' Create the resource converter
            Dim converter = New ResourceSvgConverter(_resourceSettings)

            ' Perform the conversion to ResourceDictionary XAML
            Return converter.Convert()
        End Function
    End Class
End Namespace
```
# [XAML](#tab/xaml)
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Color x:Key="Color1">#FF2196F3</Color>
  <Color x:Key="Color2">#FFFFFFFF</Color>
  <Color x:Key="Color3">#FF3F51B5</Color>
  <Color x:Key="Color4">#FF00BCD4</Color>
  <Color x:Key="Color5">#FFFF9800</Color>
  <Color x:Key="Color6">#FF64B5F6</Color>
  <SolidColorBrush x:Key="Brush1" Color="{DynamicResource Color1}" />
  <SolidColorBrush x:Key="Brush2" Color="{DynamicResource Color2}" />
  <SolidColorBrush x:Key="Brush3" Color="{DynamicResource Color3}" />
  <SolidColorBrush x:Key="Brush4" Color="{DynamicResource Color4}" />
  <SolidColorBrush x:Key="Brush5" Color="{DynamicResource Color5}" />
  <SolidColorBrush x:Key="Brush6" Color="{DynamicResource Color6}" />
  <DrawingGroup x:Key="icon_area_chart" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <GeometryDrawing Brush="{DynamicResource Brush3}" Geometry="F1M42,37L42,37 6,37 6,25 16,10 30,17 42,6z" />
    <GeometryDrawing Brush="{DynamicResource Brush4}" Geometry="F1M42,42L42,42 6,42 6,32 16,24 30,26 42,17z" />
  </DrawingGroup>
  <DrawingGroup x:Key="icon_crystal_oscillator" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <DrawingGroup>
      <GeometryDrawing Brush="{DynamicResource Brush5}" Geometry="F0M3,28L29,28 29,32 3,32z" />
      <GeometryDrawing Brush="{DynamicResource Brush5}" Geometry="F0M3,16L29,16 29,20 3,20z" />
    </DrawingGroup>
    <GeometryDrawing Brush="{DynamicResource Brush1}" Geometry="F1M43,11L20,11 20,37 43,37C44.1,37,45,36.1,
       45,35L45,13C45,11.9,44.1,11,43,11z" />
    <GeometryDrawing Brush="{DynamicResource Brush6}" Geometry="F1M20,9L18,9 18,39 20,39C21.1,39,22,38.1,22,
       37L22,11C22,9.9,21.1,9,20,9z" />
  </DrawingGroup>
  <DrawingGroup x:Key="icon_about" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <GeometryDrawing Brush="{DynamicResource Brush1}" Geometry="F1M37,40L11,40 5,46 5,12C5,8.7,7.7,6,11,6L37,
       6C40.3,6,43,8.7,43,12L43,34C43,37.3,40.3,40,37,40z" />
    <DrawingGroup>
      <GeometryDrawing Brush="{DynamicResource Brush2}" Geometry="F0M22,20L26,20 26,31 22,31z" />
      <GeometryDrawing Brush="{DynamicResource Brush2}" Geometry="F0M26,15C26,16.1046,25.1046,17,24,17C22.8954,17,22,
       16.1046,22,15C22,13.8954,22.8954,13,24,13C25.1046,13,26,13.8954,26,15z" />
    </DrawingGroup>
  </DrawingGroup>
</ResourceDictionary>
```
***

### ResourceKeyResolverType - Dictionary
For the cases where the resource keys are already defined, you can use the dictionary resolver, or @SharpVectors.Renderers.DictionaryKeyResolver, which maps the
SVG file names without the extension to the predefined keys.

The following sample code uses the dictionary resource key resolver, or @SharpVectors.Renderers.DictionaryKeyResolver, to resolve the keys of the previous SVG sample files.
# [C#](#tab/csharp)
```csharp
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using SharpVectors.Renderers;
using SharpVectors.Converters;
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

            // Create a dictionary of SVG file name to predefined names
            var dictionaryKeys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "about",              "About" },
                { "area_chart",         "AreaChart" },
                { "crystal_oscillator", "CrystalOscillator" }
            };
            // Initialize the dictionary key resolver and register it
            var resolver = new DictionaryKeyResolver(dictionaryKeys);
            _resourceSettings.RegisterResolver(resolver);
        }

        public string Convert(string svgDir)
        {
            // Add a directory as SVG source
            _resourceSettings.AddSource(svgDir);

            // Create the resource converter
            var converter = new ResourceSvgConverter(_resourceSettings);

            // Perform the conversion to ResourceDictionary XAML
            return converter.Convert();
        }
    }
}
```
# [VB.NET](#tab/vb)
```vb
Imports System
Imports System.Collections.Generic
Imports System.Windows.Media

Imports SharpVectors.Renderers
Imports SharpVectors.Converters
Imports SharpVectors.Renderers.Wpf

Namespace SharpVectors.Test.Sample
    Public Class ResourceSvgConverterSample
        Private _resourceSettings As WpfResourceSettings

        Public Sub New()
            ' Create the resource settings or options
            _resourceSettings = New WpfResourceSettings()
            _resourceSettings.ResourceFreeze = False ' Do not freeze

            ' Create a dictionary of SVG file name to predefined names
            Dim dictionaryKeys = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
                {"about", "About"},
                {"area_chart", "AreaChart"},
                {"crystal_oscillator", "CrystalOscillator"}
            }
            ' Initialize the dictionary key resolver and register it
            Dim resolver = New DictionaryKeyResolver(dictionaryKeys)
            _resourceSettings.RegisterResolver(resolver)
        End Sub

        Public Function Convert(ByVal svgDir As String) As String
            ' Add a directory as SVG source
            _resourceSettings.AddSource(svgDir)

            ' Create the resource converter
            Dim converter = New ResourceSvgConverter(_resourceSettings)

            ' Perform the conversion to ResourceDictionary XAML
            Return converter.Convert()
        End Function
    End Class
End Namespace
```
# [XAML](#tab/xaml)
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Color x:Key="Color1">#FF2196F3</Color>
  <Color x:Key="Color2">#FFFFFFFF</Color>
  <Color x:Key="Color3">#FF3F51B5</Color>
  <Color x:Key="Color4">#FF00BCD4</Color>
  <Color x:Key="Color5">#FFFF9800</Color>
  <Color x:Key="Color6">#FF64B5F6</Color>
  <SolidColorBrush x:Key="Brush1" Color="{DynamicResource Color1}" />
  <SolidColorBrush x:Key="Brush2" Color="{DynamicResource Color2}" />
  <SolidColorBrush x:Key="Brush3" Color="{DynamicResource Color3}" />
  <SolidColorBrush x:Key="Brush4" Color="{DynamicResource Color4}" />
  <SolidColorBrush x:Key="Brush5" Color="{DynamicResource Color5}" />
  <SolidColorBrush x:Key="Brush6" Color="{DynamicResource Color6}" />
  <DrawingGroup x:Key="CrystalOscillator" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <DrawingGroup>
      <GeometryDrawing Brush="{DynamicResource Brush5}" Geometry="F0M3,28L29,28 29,32 3,32z" />
      <GeometryDrawing Brush="{DynamicResource Brush5}" Geometry="F0M3,16L29,16 29,20 3,20z" />
    </DrawingGroup>
    <GeometryDrawing Brush="{DynamicResource Brush1}" Geometry="F1M43,11L20,11 20,37 43,37C44.1,37,
        45,36.1,45,35L45,13C45,11.9,44.1,11,43,11z" />
    <GeometryDrawing Brush="{DynamicResource Brush6}" Geometry="F1M20,9L18,9 18,39 20,39C21.1,39,22,
        38.1,22,37L22,11C22,9.9,21.1,9,20,9z" />
  </DrawingGroup>
  <DrawingGroup x:Key="About" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <GeometryDrawing Brush="{DynamicResource Brush1}" Geometry="F1M37,40L11,40 5,46 5,12C5,8.7,7.7,6,
        11,6L37,6C40.3,6,43,8.7,43,12L43,34C43,37.3,40.3,40,37,40z" />
    <DrawingGroup>
      <GeometryDrawing Brush="{DynamicResource Brush2}" Geometry="F0M22,20L26,20 26,31 22,31z" />
      <GeometryDrawing Brush="{DynamicResource Brush2}" Geometry="F0M26,15C26,16.1046,25.1046,17,24,17C22.8954,
	      17,22,16.1046,22,15C22,13.8954,22.8954,13,24,13C25.1046,13,26,13.8954,26,15z" />
    </DrawingGroup>
  </DrawingGroup>
  <DrawingGroup x:Key="AreaChart" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <GeometryDrawing Brush="{DynamicResource Brush3}" Geometry="F1M42,37L42,37 6,37 6,25 16,10 30,17 42,6z" />
    <GeometryDrawing Brush="{DynamicResource Brush4}" Geometry="F1M42,42L42,42 6,42 6,32 16,24 30,26 42,17z" />
  </DrawingGroup>
</ResourceDictionary>
```
***

### ResourceKeyResolverType - CodeSnippet
In applications you need to use a code snippet to implement the @SharpVectors.Renderers.IResourceKeyResolver interface, we provider the code snippet resolver,
or @SharpVectors.Renderers.CodeSnippetKeyResolver, which uses @System.CodeDom.Compiler.CodeDomProvider to compile the implementation in memory
and use it to resolve the keys. This is used to reduce dependencies but limits the lanagauge feature supports to .NET 4.0.

For the code snippet, the following conditions are required
* The namespace must be `SharpVectors.Renderers`.
* The class name must be `SnippetKeyResolver`.

The following sample code uses the snippet resource key resolver, or @SharpVectors.Renderers.CodeSnippetKeyResolver, to resolve the keys. To reduce the code length
for the screen, we will assume the snippet is stored in a file, and load it from the file.
```csharp
using System;
using System.Xml;
using System.Windows;

namespace SharpVectors.Renderers {
    public sealed class SnippetKeyResolver : WpfSettings<SnippetKeyResolver>, IResourceKeyResolver {
        public SnippetKeyResolver() {
        }

        public ResourceKeyResolverType ResolverType {
            get {
                return ResourceKeyResolverType.Custom;
            }
        }

        public bool IsValid {
            get {
                return true;
            }
        }

        public void BeginResolve() {
        }

        public void EndResolve() {
        }

        public override SnippetKeyResolver Clone() {
            return new SnippetKeyResolver();
        }

        public override void ReadXml(XmlReader reader) {
        }

        public override void WriteXml(XmlWriter writer) {
        }

        public string Resolve(DependencyObject resource, int index, string fileName, string fileSource) {
            if (index < 0) {
                throw new ArgumentException("The specified index is invalid", "index");
            }
            NotNullNotEmpty(fileName, "fileName");

            var keyValue = ToLowerCamelCase(fileName.ToUpper());
            if (!string.IsNullOrWhiteSpace(keyValue) && keyValue.Length >= 3 && keyValue.Length < 255) {
                return keyValue;
            }
            return fileName;
        }

        private static string ToLowerCamelCase(string fileName) {
            if (string.IsNullOrWhiteSpace(fileName)) {
                return string.Empty;
            }

            string camelCaseStr = char.ToLower(fileName[0]).ToString();

            if (fileName.Length > 1) {
                bool isStartOfWord = false;
                for (int i = 1; i < fileName.Length; i++) {
                    char currChar = fileName[i];
                    if (currChar == '_' || currChar == '-') {
                        isStartOfWord = true;
                    } else if (char.IsUpper(currChar)) {
                        if (isStartOfWord) {
                            camelCaseStr += currChar;
                        } else {
                            camelCaseStr += char.ToLower(currChar);
                        }
                        isStartOfWord = false;
                    } else {
                        camelCaseStr += currChar;
                        isStartOfWord = false;
                    }
                }
            }
            return camelCaseStr;
        }
    }
}
```

The test code snippet (C# code) will convert the specified SVG file name to the lower camel case naming format as the resource key.
# [C#](#tab/csharp)
```csharp
using System.Windows;
using System.Windows.Media;

using SharpVectors.Renderers;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Test.Sample
{
    public class ResourceSvgConverterSample
    {
        private WpfResourceSettings _resourceSettings;

        public ResourceSvgConverterSample(string snippetFile)
        {
            // Create the resource settings or options
            _resourceSettings = new WpfResourceSettings();
            _resourceSettings.ResourceFreeze = false; // Do not freeze

            // Get the code snippet, here assumed from a file
            string codeSnippet = string.Empty;
            using (var reader = new System.IO.StreamReader(snippetFile))
            {
                codeSnippet = reader.ReadToEnd();
            }

            // Initialize the code snippet key resolver and register it
            var resolver = new CodeSnippetKeyResolver(codeSnippet, "cs");
            _resourceSettings.RegisterResolver(resolver);
        }

        public string Convert(string svgDir)
        {
            // Add a directory as SVG source
            _resourceSettings.AddSource(svgDir);

            // Create the resource converter
            var converter = new ResourceSvgConverter(_resourceSettings);

            // Perform the conversion to ResourceDictionary XAML
            return converter.Convert();
        }
    }
}
```
# [VB.NET](#tab/vb)
```vb
Imports System.Windows.Media

Imports SharpVectors.Renderers
Imports SharpVectors.Converters
Imports SharpVectors.Renderers.Wpf

Namespace SharpVectors.Test.Sample
    Public Class ResourceSvgConverterSample
        Private _resourceSettings As WpfResourceSettings

        Public Sub New(ByVal snippetFile As String)
            ' Create the resource settings or options
            _resourceSettings = New WpfResourceSettings()
            _resourceSettings.ResourceFreeze = False ' Do not freeze

            ' Get the code snippet, here assumed from a file
            Dim codeSnippet = String.Empty
            Using reader = New StreamReader(snippetFile)
                codeSnippet = reader.ReadToEnd()
            End Using

            ' Initialize the code snippet key resolver and register it
            Dim resolver = New CodeSnippetKeyResolver(codeSnippet, "cs")
            _resourceSettings.RegisterResolver(resolver)
        End Sub

        Public Function Convert(ByVal svgDir As String) As String
            ' Add a directory as SVG source
            _resourceSettings.AddSource(svgDir)

            ' Create the resource converter
            Dim converter = New ResourceSvgConverter(_resourceSettings)

            ' Perform the conversion to ResourceDictionary XAML
            Return converter.Convert()
        End Function
    End Class
End Namespace
```
# [XAML](#tab/xaml)
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Color x:Key="Color1">#FF2196F3</Color>
  <Color x:Key="Color2">#FFFFFFFF</Color>
  <Color x:Key="Color3">#FF3F51B5</Color>
  <Color x:Key="Color4">#FF00BCD4</Color>
  <Color x:Key="Color5">#FFFF9800</Color>
  <Color x:Key="Color6">#FF64B5F6</Color>
  <SolidColorBrush x:Key="Brush1" Color="{DynamicResource Color1}" />
  <SolidColorBrush x:Key="Brush2" Color="{DynamicResource Color2}" />
  <SolidColorBrush x:Key="Brush3" Color="{DynamicResource Color3}" />
  <SolidColorBrush x:Key="Brush4" Color="{DynamicResource Color4}" />
  <SolidColorBrush x:Key="Brush5" Color="{DynamicResource Color5}" />
  <SolidColorBrush x:Key="Brush6" Color="{DynamicResource Color6}" />
  <DrawingGroup x:Key="crystalOscillator" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <DrawingGroup>
      <GeometryDrawing Brush="{DynamicResource Brush5}" Geometry="F0M3,28L29,28 29,32 3,32z" />
      <GeometryDrawing Brush="{DynamicResource Brush5}" Geometry="F0M3,16L29,16 29,20 3,20z" />
    </DrawingGroup>
    <GeometryDrawing Brush="{DynamicResource Brush1}" Geometry="F1M43,11L20,11 20,37 43,37C44.1,37,
        45,36.1,45,35L45,13C45,11.9,44.1,11,43,11z" />
    <GeometryDrawing Brush="{DynamicResource Brush6}" Geometry="F1M20,9L18,9 18,39 20,39C21.1,39,22,
        38.1,22,37L22,11C22,9.9,21.1,9,20,9z" />
  </DrawingGroup>
  <DrawingGroup x:Key="about" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <GeometryDrawing Brush="{DynamicResource Brush1}" Geometry="F1M37,40L11,40 5,46 5,12C5,8.7,7.7,6,11,
        6L37,6C40.3,6,43,8.7,43,12L43,34C43,37.3,40.3,40,37,40z" />
    <DrawingGroup>
      <GeometryDrawing Brush="{DynamicResource Brush2}" Geometry="F0M22,20L26,20 26,31 22,31z" />
      <GeometryDrawing Brush="{DynamicResource Brush2}" Geometry="F0M26,15C26,16.1046,25.1046,17,24,
	17C22.8954,17,22,16.1046,22,15C22,13.8954,22.8954,13,24,13C25.1046,13,26,13.8954,26,15z" />
    </DrawingGroup>
  </DrawingGroup>
  <DrawingGroup x:Key="areaChart" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <GeometryDrawing Brush="{DynamicResource Brush3}" Geometry="F1M42,37L42,37 6,37 6,25 16,10 30,17 42,6z" />
    <GeometryDrawing Brush="{DynamicResource Brush4}" Geometry="F1M42,42L42,42 6,42 6,32 16,24 30,26 42,17z" />
  </DrawingGroup>
</ResourceDictionary>
```
***

### ResourceKeyResolverType - Custom
You can create a custom resource key resolver by implementing the @SharpVectors.Renderers.IResourceKeyResolver interface.

The following sample code implements a resource key resolver that will convert the specified SVG file name to the upper camel case naming format as the resource key.
```cs
using System;
using System.Xml;

using System.Windows;

using SharpVectors.Renderers;

namespace SharpVectors.Test.Sample {
    internal sealed class CustomResourceKeyResolver : WpfSettings<CustomResourceKeyResolver>, IResourceKeyResolver {
        public CustomResourceKeyResolver() {
        }

        public override CustomResourceKeyResolver Clone() {
            return new CustomResourceKeyResolver();
        }

        public override void ReadXml(XmlReader reader) {
            NotNull(reader, nameof(reader));
        }

        public override void WriteXml(XmlWriter writer) {
            NotNull(writer, nameof(writer));
        }

        public ResourceKeyResolverType ResolverType {
            get {
                return ResourceKeyResolverType.Custom;
            }
        }

        public bool IsValid {
            get {
                return true;
            }
        }

        public void BeginResolve() {
        }

        public void EndResolve() {
        }

        public string Resolve(DependencyObject resource, int index, string fileName, string fileSource) {
            if (index < 0) {
                throw new ArgumentException("The specified index is invalid", "index");
            }
            NotNullNotEmpty(fileName, "fileName");

            var keyValue = ToUpperCamelCase(fileName.ToUpper());
            if (!string.IsNullOrWhiteSpace(keyValue) && keyValue.Length >= 3 && keyValue.Length < 255) {
                return keyValue;
            }
            return fileName;
        }

        internal static string ToUpperCamelCase(string fileName) {
            if (string.IsNullOrWhiteSpace(fileName)) {
                return string.Empty;
            }

            string camelCaseStr = fileName[0].ToString();

            if (fileName.Length > 1) {
                bool isStartOfWord = false;
                for (int i = 1; i < fileName.Length; i++) {
                    char currChar = fileName[i];
                    if (currChar == '_' || currChar == '-') {
                        isStartOfWord = true;
                    } else if (char.IsUpper(currChar)) {
                        if (isStartOfWord) {
                            camelCaseStr += currChar;
                        } else {
                            camelCaseStr += char.ToLower(currChar);
                        }
                        isStartOfWord = false;
                    } else {
                        camelCaseStr += currChar;
                        isStartOfWord = false;
                    }
                }
            }
            return camelCaseStr;
        }
    }
}
```

The following illustrate how to use the custom resource key resolver.

# [C#](#tab/csharp)
```csharp
using System.Windows;
using System.Windows.Media;

using SharpVectors.Renderers;
using SharpVectors.Converters;
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

            // Initialize the custom key resolver and register it
            var resolver = new CustomResourceKeyResolver();
            _resourceSettings.RegisterResolver(resolver);
        }

        public string Convert(string svgDir)
        {
            // Add a directory as SVG source
            _resourceSettings.AddSource(svgDir);

            // Create the resource converter
            var converter = new ResourceSvgConverter(_resourceSettings);

            // Perform the conversion to ResourceDictionary XAML
            return converter.Convert();
        }
    }
}
```
# [VB.NET](#tab/vb)
```vb
Imports System.Windows.Media

Imports SharpVectors.Renderers
Imports SharpVectors.Converters
Imports SharpVectors.Renderers.Wpf

Namespace SharpVectors.Test.Sample
    Public Class ResourceSvgConverterSample
        Private _resourceSettings As WpfResourceSettings

        Public Sub New()
            ' Create the resource settings or options
            _resourceSettings = New WpfResourceSettings()
            _resourceSettings.ResourceFreeze = False ' Do not freeze

            ' Initialize the custom key resolver and register it
            Dim resolver = New CustomResourceKeyResolver()
            _resourceSettings.RegisterResolver(resolver)
        End Sub

        Public Function Convert(ByVal svgDir As String) As String
            ' Add a directory as SVG source
            _resourceSettings.AddSource(svgDir)

            ' Create the resource converter
            Dim converter = New ResourceSvgConverter(_resourceSettings)

            ' Perform the conversion to ResourceDictionary XAML
            Return converter.Convert()
        End Function
    End Class
End Namespace
```
# [XAML](#tab/xaml)
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Color x:Key="Color1">#FF2196F3</Color>
  <Color x:Key="Color2">#FFFFFFFF</Color>
  <Color x:Key="Color3">#FF3F51B5</Color>
  <Color x:Key="Color4">#FF00BCD4</Color>
  <Color x:Key="Color5">#FFFF9800</Color>
  <Color x:Key="Color6">#FF64B5F6</Color>
  <SolidColorBrush x:Key="Brush1" Color="{DynamicResource Color1}" />
  <SolidColorBrush x:Key="Brush2" Color="{DynamicResource Color2}" />
  <SolidColorBrush x:Key="Brush3" Color="{DynamicResource Color3}" />
  <SolidColorBrush x:Key="Brush4" Color="{DynamicResource Color4}" />
  <SolidColorBrush x:Key="Brush5" Color="{DynamicResource Color5}" />
  <SolidColorBrush x:Key="Brush6" Color="{DynamicResource Color6}" />
  <DrawingGroup x:Key="CrystalOscillator" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <DrawingGroup>
      <GeometryDrawing Brush="{DynamicResource Brush5}" Geometry="F0M3,28L29,28 29,32 3,32z" />
      <GeometryDrawing Brush="{DynamicResource Brush5}" Geometry="F0M3,16L29,16 29,20 3,20z" />
    </DrawingGroup>
    <GeometryDrawing Brush="{DynamicResource Brush1}" Geometry="F1M43,11L20,11 20,37 43,
      37C44.1,37,45,36.1,45,35L45,13C45,11.9,44.1,11,43,11z" />
    <GeometryDrawing Brush="{DynamicResource Brush6}" Geometry="F1M20,9L18,9 18,39 20,39C21.1,
      39,22,38.1,22,37L22,11C22,9.9,21.1,9,20,9z" />
  </DrawingGroup>
  <DrawingGroup x:Key="About" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <GeometryDrawing Brush="{DynamicResource Brush1}" Geometry="F1M37,40L11,40 5,46 5,12C5,8.7,
      7.7,6,11,6L37,6C40.3,6,43,8.7,43,12L43,34C43,37.3,40.3,40,37,40z" />
    <DrawingGroup>
      <GeometryDrawing Brush="{DynamicResource Brush2}" Geometry="F0M22,20L26,20 26,31 22,31z" />
      <GeometryDrawing Brush="{DynamicResource Brush2}" Geometry="F0M26,15C26,16.1046,25.1046,17,24,
       17C22.8954,17,22,16.1046,22,15C22,13.8954,22.8954,13,24,13C25.1046,13,26,13.8954,26,15z" />
    </DrawingGroup>
  </DrawingGroup>
  <DrawingGroup x:Key="AreaChart" ClipGeometry="F0M0,0L48,0L48,48L0,48z">
    <GeometryDrawing Brush="{DynamicResource Brush3}" Geometry="F1M42,37L42,37 6,37 6,25 16,10 30,17 42,6z" />
    <GeometryDrawing Brush="{DynamicResource Brush4}" Geometry="F1M42,42L42,42 6,42 6,32 16,24 30,26 42,17z" />
  </DrawingGroup>
</ResourceDictionary>
```
***

## Options XML Serialization
The resource settings,  **[](xref:SharpVectors.Renderers.Wpf.WpfResourceSettings)** object can be saved to file in an XML format, or initialized from an XML format file.

You can, therefore, create a resource dictionary converter options for a project and share with other developers or projects. The following code defines a sample resource dictionary converter
options and serializes @SharpVectors.Renderers.Wpf.WpfResourceSettings object to XML.

# [C#](#tab/csharp)
```csharp
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
```
# [VB.NET](#tab/vb)
```vb
Imports System.Collections.Generic
Imports System.Windows.Media

Imports SharpVectors.Renderers
Imports SharpVectors.Renderers.Wpf

Namespace SharpVectors.Test.Sample
    Public Class ResourceSvgConverterSample
        Private _resourceSettings As WpfResourceSettings

        Public Sub New()
            ' Create the resource settings or options
            _resourceSettings = New WpfResourceSettings()
            _resourceSettings.ResourceFreeze = False ' Do not freeze

            ' Initialize the default key resolver and register it
            Dim resolver = New ResourceKeyResolver("icon_${name}")
            _resourceSettings.RegisterResolver(resolver)

            ' Add predefined color palette
            _resourceSettings.ColorPalette = New Dictionary(Of Color, String)(WpfDrawingResources.ColorComparer) From {
		    {CType(ColorConverter.ConvertFromString("#FF008000"), Color), "SvgColor01"},
		    {CType(ColorConverter.ConvertFromString("#FF000000"), Color), "SvgColor02"},
		    {CType(ColorConverter.ConvertFromString("#FFFFFF00"), Color), "SvgColor03"},
		    {CType(ColorConverter.ConvertFromString("#FF0000FF"), Color), "SvgColor04"},
		    {CType(ColorConverter.ConvertFromString("#FF00FF00"), Color), "SvgColor05"},
		    {CType(ColorConverter.ConvertFromString("#FF339966"), Color), "SvgColor06"},
		    {CType(ColorConverter.ConvertFromString("#FFFF00FF"), Color), "SvgColor07"},
		    {CType(ColorConverter.ConvertFromString("#FFFFA500"), Color), "SvgColor08"},
		    {CType(ColorConverter.ConvertFromString("#FF007700"), Color), "SvgColor09"},
		    {CType(ColorConverter.ConvertFromString("#FF33CC66"), Color), "SvgColor10"}
		}

            ' Add directories as SVG source
            _resourceSettings.AddSource("C:\Abc-Project\Icons1")
            _resourceSettings.AddSource("C:\Abc-Project\Icons2")
        End Sub

        Public Function Save() As String
            ' Serialize the resource settings to XML
            Return _resourceSettings.Save()
        End Function
    End Class
End Namespace
```
# [XAML](#tab/xaml)
```xml
<?xml version="1.0" encoding="utf-16"?>
<resourceSettings version="1.0.0">
    <properties>
        <property name="ResourceFreeze" type="bool" value="false" />
        <property name="UseResourceIndex" type="bool" value="false" />
        <property name="ResourceMode" type="enum" value="Drawing" />
        <property name="ResourceAccess" type="enum" value="Dynamic" />
        <property name="IndentSpaces" type="int" value="2" />
        <property name="NumericPrecision" type="int" value="4" />
    </properties>
    <naming>
        <property name="PenNameFormat" type="string" value="Pen{0}" />
        <property name="ColorNameFormat" type="string" value="Color{0}" />
        <property name="BrushNameFormat" type="string" value="Brush{0}" />
        <resolver type="Default">
            <property name="ResourceNameFormat" type="string" value="icon_${name}" />
        </resolver>
    </naming>
    <binding>
        <property name="BindToResources" type="bool" value="true" />
        <property name="BindPenToBrushes" type="bool" value="true" />
        <property name="BindToColors" type="bool" value="true" />
    </binding>
    <palettes>
        <palette color="#FF008000" name="SvgColor01" />
        <palette color="#FF000000" name="SvgColor02" />
        <palette color="#FFFFFF00" name="SvgColor03" />
        <palette color="#FF0000FF" name="SvgColor04" />
        <palette color="#FF00FF00" name="SvgColor05" />
        <palette color="#FF339966" name="SvgColor06" />
        <palette color="#FFFF00FF" name="SvgColor07" />
        <palette color="#FFFFA500" name="SvgColor08" />
        <palette color="#FF007700" name="SvgColor09" />
        <palette color="#FF33CC66" name="SvgColor10" />
    </palettes>
    <sources>
        <source>C:/Abc-Project/Icons1</source>
        <source>C:/Abc-Project/Icons2</source>
    </sources>
</resourceSettings>
```
***