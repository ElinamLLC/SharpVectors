## About

This project provides a C# library for parsing, converting and viewing the SVG files in WPF applications.

More documentation is available at the [ShapeVectors Github Pages](https://elinamllc.github.io/SharpVectors/).

## Key Features

* Parsing of the SVG files to create the SVG DOM
* SVG to XAML conversion
* A small runtime library to handle font URI, embedded images and others when using the XAML files directly from disk.
* An optimized XAML output.
* Interaction with the conversion process (by a visitor pattern) to allow for custom hyper-link implementations, font substitutions etc.
* Supports .NET Framework 4+, .NET Core, and .NET 6.0

## Main Components

The library consists of several components as listed below and there is no dependency on external components:

* `SharpVectors.Core.dll` (SVG interfaces and basic types)
* `SharpVectors.Css.dll` (CSS interfaces and types)
* `SharpVectors.Dom.dll` (SVG DOM, integrates CSS DOM)
* `SharpVectors.Model.dll` (SVG model, parsing and utility classes)
* `SharpVectors.Rendering.Gdi.dll` (GDI+ rendering of the SVG Model - requires more work)
* `SharpVectors.Runtime.Wpf.dll` (An independent component with scroling/zooming panels for viewing rendered XAML)
* `SharpVectors.Rendering.Wpf.dll` (WPF rendering of the SVG Model)
* `SharpVectors.Converters.Wpf.dll` (Utitlity classes providing access to the WPF rendering features)

## Credits

SharpVectors uses source codes from articles and other open source projects. We wish to acknowledge and thank 
the authors of these great articles and projects
* [SharpVectorGraphics (aka SVG#)](https://sourceforge.net/projects/svgdomcsharp/) by SVG# Team of Developers (SourceForge)
* [WPF Zooming and Panning Control](https://www.codeproject.com/KB/WPF/zoomandpancontrol.aspx) by Ashley Davis (CodeProject)
* [Render Text On A Path With WPF](https://msdn.microsoft.com/en-us/magazine/dd263097.aspx) by Charles Petzold (MSDN Magazine - December 2008)
* [MinIoC](https://github.com/microsoft/MinIoC) by Microsoft (Single-file minimal C# IoC container)
* [.NET ZLib Implementation](https://www.codeproject.com/Tips/830793/NET-ZLib-Implementation) by Alberto M (CodeProject)
* [Brotli compression format](https://github.com/google/brotli) by Google (C# Decoder)

## Related Packages

* Full Package (WPF/GDI+) - Retiring (Old Name): [ SharpVectors.Reloaded](https://www.nuget.org/packages/SharpVectors.Reloaded/)
* WPF Package (WPF Only)  - Recommended for WPF: [ SharpVectors.Wpf](https://www.nuget.org/packages/SharpVectors.Wpf/)

## Feedback

SharpVectors is released as open source under the [BSD-3-Clause license](https://github.com/ElinamLLC/SharpVectors/blob/master/License.md),
and the [Third-Party licences](https://github.com/ElinamLLC/SharpVectors/blob/master/License.txt)

* Bug reports and contributions are welcome at [SharpVectors Issues](https://github.com/ElinamLLC/SharpVectors/issues/).
* Discussions and Q/A are welcome at [SharpVectors Discussions](https://github.com/ElinamLLC/SharpVectors/discussions/).
