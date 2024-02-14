# Introduction
The Scalable Vector Graphics (SVG) is a well-known graphics format developed and maintained by the W3C SVG Working Group. The SVG is designed to be used on the web, but the format is flexible enough for applications outside the web. Moreover, the format is exported and imported by most image editing applications, making it the ideal format for image file exchanges.

SVG is a markup language (an XML format) for describing two-dimensional graphics applications and images, and a set of related graphics script interfaces. With the script interfaces, it can be created and manipulated through JavaScript scripts.

The **SharpVectors** project aims to provide libraries and tools to parse, convert and view the SVG on Windows, especially on the Windows Presentation Foundation (WPF) platform.

## Installation
The SharpVectors library currently targets the following .NET frameworks
* .NET Framework, Version 4.0
* .NET Framework, Version 4.5
* .NET Framework, Version 4.6
* .NET Framework, Version 4.7
* .NET Framework, Version 4.8
* .NET Standard, Version 2.1
* .NET Core, Version 3.1
* .NET 6.0 ~ .NET 8.0

The library consists of a number of shared or common components and rendering implementations for Windows Presentation Foundation (WPF) and Windows Forms applications.

### For the Libraries
The library can be downloaded from the following sources
* **NuGet (Full Package - WPF/GDI+)**, [Version 1.8.4 - SharpVectors](https://www.nuget.org/packages/SharpVectors/).
* **NuGet (Full Package - WPF/GDI+)**, [Version 1.8.4 - SharpVectors.Reloaded](https://www.nuget.org/packages/SharpVectors.Reloaded/).
* **NuGet (WPF Only)**, [Version 1.8.4 - SharpVectors.Wpf](https://www.nuget.org/packages/SharpVectors.Wpf/).
* **GitHub Releases Page**, [Version 1.8.4](https://github.com/ElinamLLC/SharpVectors/releases).

> [!NOTE]
> * The **SharpVectors.Reloaded** package is the same as the **SharpVectors**, which is the recommended package if you need the full package.
> * The **SharpVectors.Reloaded** name was used for the Nuget package at the time the **SharpVectors** package name was not available.
> * The **SharpVectors.Reloaded** package name will be retired in Version 2.0.
> * The **SharpVectors.Wpf** is the recommended package, for `WPF` only application.
> * As outlined in the [roadmap](https://github.com/ElinamLLC/SharpVectors/issues/147), other packages such as the **SharpVectors.Gdi** for the `GDI+`, will be available as the renderers mature.

### For the Applications
The following are related SharpVectors application repositories
* [SvgXaml](https://github.com/ElinamLLC/SvgXaml) : SharpVectors based SVG to XAML converter application.
* [SvgViewer](https://github.com/ElinamLLC/SvgViewer) : SharpVectors based SVG viewer application.

## Getting Started
This section provides [Getting Start](xref:topic_getting_started) information to help you get started.

## Credits
SharpVectors uses source codes from articles and other open source projects. We wish to acknowledge and thank 
the authors of these great articles and projects
* [SharpVectorGraphics (aka SVG#)](https://sourceforge.net/projects/svgdomcsharp/) by SVG# Team of Developers (SourceForge)
* [WPF Zooming and Panning Control](https://www.codeproject.com/KB/WPF/zoomandpancontrol.aspx) by Ashley Davis (CodeProject)
* [Render Text On A Path With WPF](https://msdn.microsoft.com/en-us/magazine/dd263097.aspx) by Charles Petzold (MSDN Magazine - December 2008)
* [MinIoC](https://github.com/microsoft/MinIoC) by Microsoft (Single-file minimal C# IoC container)
* [.NET ZLib Implementation](https://www.codeproject.com/Tips/830793/NET-ZLib-Implementation) by Alberto M (CodeProject)
* [Brotli compression format](https://github.com/google/brotli) by Google (C# Decoder)

## Links to Resources
* **Overview**
	* [About SVG](https://www.w3.org/Graphics/SVG/About.html)

* **Scalable Vector Graphics (SVG) Specifications**
	* [SVG 1.1 (First Edition)](https://www.w3.org/TR/2003/REC-SVG11-20030114/) - This version is outdated!
	* [SVG 1.1 (Second Edition)](https://www.w3.org/TR/SVG11/)

* **Libraries and Frameworks**
	* [Batik SVG Toolkit](https://xmlgraphics.apache.org/batik/), the most complete SVG toolkit available (Java).
