---
uid: topic_svg_to_bitmap_converter
title: SvgToBitmapValueConverter
---

# SVG to Bitmap Value Converter - SvgToBitmapValueConverter

The **[SvgToBitmapValueConverter](xref:SharpVectors.Converters.SvgToBitmapValueConverter)** is a WPF value converter that rasterizes SVG files into bitmap images suitable for binding to image controls. This converter transforms vector SVG content into static raster images at 96 DPI, making it ideal for icons, thumbnails, and other UI elements that require bitmap representations.

## Overview

Unlike the [SvgImageExtension](xref:SharpVectors.Converters.SvgImageExtension) and [SvgImageConverterExtension](xref:SharpVectors.Converters.SvgImageConverterExtension) markup extensions that produce [DrawingImage](xref:System.Windows.Media.DrawingImage) objects (vector-based), the `SvgToBitmapValueConverter` produces [BitmapImage](xref:System.Windows.Media.Imaging.BitmapImage) objects (raster-based).

> [!NOTE]
> The `SvgToBitmapValueConverter` is designed for scenarios where:
> - You need rasterized bitmap images instead of vector drawings
> - You're binding SVG paths from view models or code-behind
> - You require static, fixed-resolution images
> - You need compatibility with legacy image processing pipelines

## Key Features

- **Direct Value Conversion**: Implements [IValueConverter](xref:System.Windows.Data.IValueConverter) for WPF data binding
- **Multiple Source Types**: Supports string paths, URIs, and file paths
- **Smart URI Resolution**: Handles relative paths, pack URIs, and web URLs
- **Automatic Sizing**: Determines bitmap dimensions from SVG viewBox or uses defaults
- **Design-Time Support**: Works reliably in the Visual Studio WPF designer
- **Configurable Rendering**: Inherited settings for text rendering, path optimization, and more

## Supported SVG Source Types

The converter accepts SVG files from multiple sources:

| Source Type | Example | Note |
|-------------|---------|------|
| **Web URLs** | `http://example.com/icon.svg` | HTTP/HTTPS protocol |
| **Local Paths** | `Images\icon.svg` | Relative or absolute file paths |
| **Embedded Resources** | `pack://application:,,,/Images/icon.svg` | Microsoft Pack URI format |
| **Compressed SVG** | `Images\icon.svgz` | Gzip-compressed SVG files |

## Basic Usage

### Simple XAML Binding

```xml
<Window x:Class="MyApp.MainWindow"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:converters="clr-namespace:SharpVectors.Converters;assembly=SharpVectors.Converters.Wpf">

	<Window.Resources>
		<!-- Declare the converter -->
		<converters:SvgToBitmapValueConverter x:Key="SvgToBitmapConverter"/>
	</Window.Resources>

	<StackPanel>
		<!-- Bind SVG path to Image control -->
		<Image Source="{Binding IconPath, Converter={StaticResource SvgToBitmapConverter}}" 
			   Width="64" Height="64"/>
	</StackPanel>
</Window>
```

### View Model

```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyApp
{
	public class IconViewModel : INotifyPropertyChanged
	{
		private string _iconPath;

		public string IconPath
		{
			get => _iconPath;
			set => SetProperty(ref _iconPath, value);
		}

		public IconViewModel()
		{
			// Set the icon path (can be web URL, local path, or pack URI)
			IconPath = "pack://application:,,,/Images/icon.svg";
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void SetProperty<T>(ref T field, T value, 
			[CallerMemberName] string name = null)
		{
			if (!Equals(field, value))
			{
				field = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
```

## Advanced Usage

### URL Binding

Bind to SVG files hosted on web servers:

```xml
<Image Source="{Binding RemoteIconUrl, Converter={StaticResource SvgToBitmapConverter}}" 
	   Width="48" Height="48"/>
```

### Using Converter Parameters

The converter accepts an optional parameter that takes precedence over the binding value:

```xml
<!-- Use parameter path instead of binding value -->
<Image Source="{Binding SomePath, Converter={StaticResource SvgToBitmapConverter}, 
				ConverterParameter=pack://application:,,,/FallbackIcon.svg}" 
	   Width="32" Height="32"/>
```

### Dynamic Icon Selection

```csharp
public class StatusIconViewModel : INotifyPropertyChanged
{
	private int _status;

	public int Status
	{
		get => _status;
		set
		{
			if (_status != value)
			{
				_status = value;
				IconPath = GetIconForStatus(value);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconPath)));
			}
		}
	}

	public string IconPath { get; private set; }

	private string GetIconForStatus(int status)
	{
		return status switch
		{
			0 => "pack://application:,,,/Images/status-idle.svg",
			1 => "pack://application:,,,/Images/status-active.svg",
			2 => "pack://application:,,,/Images/status-error.svg",
			_ => "pack://application:,,,/Images/status-unknown.svg"
		};
	}

	public event PropertyChangedEventHandler PropertyChanged;
}
```

## Rendering Configuration

The converter inherits rendering options from [SvgImageBase](xref:SharpVectors.Converters.SvgImageBase). Configure these properties on the converter instance:

```xml
<Window.Resources>
	<converters:SvgToBitmapValueConverter x:Key="ConfiguredConverter"
										  TextAsGeometry="True"
										  OptimizePath="True"
										  IncludeRuntime="False"/>
</Window.Resources>
```

| Property | Default | Purpose |
|----------|---------|---------|
| **TextAsGeometry** | `false` | Render text as geometry paths instead of text |
| **OptimizePath** | `true` | Optimize path geometries for better performance |
| **IncludeRuntime** | `true` | Include SharpVectors.Runtime assembly for advanced features |
| **AppName** | Auto-detected | Explicitly specify the source assembly name for resource resolution |

## Design-Time Support

The converter works reliably in the Visual Studio WPF designer:

- ✅ Displays preview bitmaps at design time
- ✅ Supports relative paths to project resources
- ✅ Handles compilation artifacts automatically
- ✅ Shows fallback placeholders on load failures (improved with recent designer enhancements)

## Bitmap Sizing

The converter determines bitmap dimensions intelligently:

1. **From SVG Viewbox**: Uses the SVG's viewBox dimensions if available
2. **From Drawing Bounds**: Measures the actual drawing boundaries
3. **Default Fallback**: Uses 96×96 pixels if dimensions cannot be determined

The sizing is DPI-aware at 96 DPI (standard for WPF):

```csharp
// Converter automatically calculates size
Rect bounds = drawingGroup.Bounds;
int width = Math.Max(1, (int)Math.Ceiling(bounds.Width));
int height = Math.Max(1, (int)Math.Ceiling(bounds.Height));
```

## Error Handling

The converter handles errors gracefully:

- **Invalid URIs**: Returns `null` (binding shows no image)
- **File Not Found**: Returns `null` with internal logging
- **Rendering Errors**: Returns `null` with exception suppression
- **Design-Mode Failures**: Shows placeholder in designer (with recent improvements)

Configure a fallback image for user feedback:

```xml
<Grid>
	<Image Source="{Binding IconPath, Converter={StaticResource SvgToBitmapConverter}}" 
		   Width="64" Height="64"/>
	<!-- Fallback placeholder -->
	<Image Source="/Images/placeholder.png" Width="64" Height="64"
		   Visibility="{Binding IconPath, Converter={StaticResource NullToVisibilityConverter}}"/>
</Grid>
```

## Performance Considerations

### Caching Behavior

- SVG-to-Bitmap conversions are **not cached** by default
- Consider implementing application-level caching for frequently-used icons:

```csharp
public class CachedIconProvider
{
	private static readonly Dictionary<string, BitmapImage> _cache = new();
	private readonly SvgToBitmapValueConverter _converter;

	public CachedIconProvider()
	{
		_converter = new SvgToBitmapValueConverter();
	}

	public BitmapImage GetIcon(string svgPath)
	{
		if (_cache.TryGetValue(svgPath, out var cached))
			return cached;

		var bitmap = _converter.Convert(svgPath, typeof(BitmapImage), null, null) 
					as BitmapImage;
		if (bitmap != null)
			_cache[svgPath] = bitmap;

		return bitmap;
	}
}
```

### Memory Usage

- Each bitmap consumes memory proportional to: **width × height × 4 bytes** (32-bit RGBA)
- 64×64 icon ≈ 16 KB
- 256×256 icon ≈ 256 KB
- Consider using [DrawingImage](xref:System.Windows.Media.DrawingImage) (vector-based) for scalable icons

## Comparison with Alternatives

| Feature | SvgToBitmapValueConverter | SvgImageExtension | SvgCanvas |
|---------|--------------------------|------------------|-----------|
| **Output Type** | BitmapImage (raster) | DrawingImage (vector) | Drawing (vector) |
| **Data Binding** | ✅ Yes (IValueConverter) | ❌ No | ❌ No |
| **Scalability** | ❌ Fixed resolution | ✅ Scales infinitely | ✅ Scales infinitely |
| **Memory** | Moderate | Low | Low |
| **Performance** | Fast (rasterized) | Medium (vector) | Medium (vector) |
| **Designer Preview** | ✅ Yes | ✅ Yes | ✅ Yes |
| **Use Case** | Icons, thumbnails | UI graphics, logos | Full documents |

## Sample Code

### C# Converter Implementation

# [C# Sample](#tab/csharp)

```csharp
using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using SharpVectors.Converters;

namespace MyApp.Converters
{
	public class IconViewModel
	{
		public string ActiveIcon { get; set; } = "pack://application:,,,/Icons/active.svg";
		public string InactiveIcon { get; set; } = "pack://application:,,,/Icons/inactive.svg";
	}

	public class IconBinding
	{
		private readonly SvgToBitmapValueConverter _converter = 
			new SvgToBitmapValueConverter();

		public void Demo()
		{
			// Direct conversion
			var viewModel = new IconViewModel();

			var activeIcon = _converter.Convert(
				viewModel.ActiveIcon, 
				typeof(BitmapImage), 
				null, 
				null) as BitmapImage;

			var inactiveIcon = _converter.Convert(
				viewModel.InactiveIcon, 
				typeof(BitmapImage), 
				null, 
				null) as BitmapImage;

			// Use in UI...
		}
	}
}
```

# [XAML Sample](#tab/xaml)

```xml
<Window x:Class="MyApp.IconWindow"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:converters="clr-namespace:SharpVectors.Converters;assembly=SharpVectors.Converters.Wpf"
		Title="SVG to Bitmap Conversion" Width="400" Height="300">

	<Window.Resources>
		<converters:SvgToBitmapValueConverter x:Key="SvgToBitmap"/>
	</Window.Resources>

	<StackPanel Margin="20" Spacing="10">
		<TextBlock Text="SVG Icon Examples" FontSize="16" FontWeight="Bold"/>

		<StackPanel Orientation="Horizontal" Spacing="20">
			<StackPanel>
				<TextBlock Text="Active Icon" FontSize="12" Foreground="Gray"/>
				<Image Source="{Binding ActiveIcon, Converter={StaticResource SvgToBitmap}}" 
					   Width="64" Height="64"/>
			</StackPanel>

			<StackPanel>
				<TextBlock Text="Inactive Icon" FontSize="12" Foreground="Gray"/>
				<Image Source="{Binding InactiveIcon, Converter={StaticResource SvgToBitmap}}" 
					   Width="64" Height="64"/>
			</StackPanel>
		</StackPanel>

		<Button Content="Click Me" Padding="10" Click="OnButtonClick">
			<Button.Content>
				<StackPanel Orientation="Horizontal" Spacing="5">
					<Image Source="{Binding ButtonIcon, Converter={StaticResource SvgToBitmap}}" 
						   Width="24" Height="24"/>
					<TextBlock Text="Click Me" VerticalAlignment="Center"/>
				</StackPanel>
			</Button.Content>
		</Button>
	</StackPanel>
</Window>
```

---

## Common Issues and Solutions

### Issue: Image not displaying

**Causes:**
- Path is incorrect or file not found
- URI format is invalid
- SVG file is corrupted or unreadable

**Solution:**
```xml
<!-- Add debug output -->
<Image Source="{Binding IconPath, Converter={StaticResource SvgToBitmap}}" 
	   Width="64" Height="64"/>

<!-- Check in code-behind -->
var converter = (SvgToBitmapValueConverter)this.Resources["SvgToBitmap"];
var result = converter.Convert(iconPath, typeof(BitmapImage), null, null);
if (result == null)
	MessageBox.Show("Conversion failed!");
```

### Issue: Design-time preview shows blank

**Cause:** Resource path is relative and incorrect in designer context

**Solution:** Use absolute pack URI:
```xml
<!-- Instead of -->
Source="{Binding IconPath}"

<!-- Use -->
Source="{Binding IconPath, Converter={StaticResource SvgToBitmap}, 
				ConverterParameter=pack://application:,,,/Icons/default.svg}"
```

### Issue: Performance is slow

**Causes:**
- Large SVG files being converted repeatedly
- Network URLs with slow connections
- No caching implemented

**Solution:** Implement application-level caching (see Performance Considerations section)

## See Also

- [SvgImageExtension](xref:SharpVectors.Converters.SvgImageExtension) - Vector-based markup extension
- [SvgImageConverterExtension](xref:SharpVectors.Converters.SvgImageConverterExtension) - Vector-based value converter
- [SvgCanvas](xref:SharpVectors.Converters.SvgCanvas) - Full SVG control
- [SvgViewbox](xref:SharpVectors.Converters.SvgViewbox) - Animated SVG control
- [SVG Markup Extensions](xref:topic_markup_extensions)
- [SVG Image Controls](xref:topic_image_controls)
