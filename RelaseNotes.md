### Release 1.7.6
    * WPF: Font Family support improvements
    * WPF: Fallback use of AppDomain.Current.BaseDirectory instead of Assembly.Location (by @MihailsKuzminsDG)
    * Several bug fixes and code improvements

### Release 1.7.5
    * Several bug fixes and code improvements

### Release 1.7.1
    * Fixed DPI bug causing the controls to throw exceptions.

### Release 1.7.0
    * Updated requiredFeatures support, fixed nested URI and circular references
    * Add support for CSS font shorthand properties
    * Support for embedded SVG contents in controls
    * Fixed issues related to hit-test
    * Added error and alert handling events
    * Improved DPI support
    * Fixed error when font-family name is a color name (by LucaPassarella)
    * Considered SvgTransformList and not single transform (by LucaPassarella)
    * Adding support for .NET 5.0
    * Several bug fixes and code improvements

### Release 1.6.0
    * Multi-frameworks support: .NET4.0 ~ .NET4.8, .NETCORE3.1
    * More Tiny Specs support: solidColor, viewport-fill
    * Private fonts supports and system font caching
    * Initial support for web fonts (woff and woff2)
    * Replaced the embedded modular DTD files with single DTD to reduce the size
    * Fixed radial gradient issues: pservers-grad-21-b.svg, pservers-grad-20-b.svg
    * WPF Rendering: Reworked pattern paint rendering
    * Added filter effects classes
    * Added Scripting interfaces and classes
    * Added IoC container, from on Microsoft MinIoC
    * Fixed performance degradation issue introduced in previous version.
    * Initial support for CSS Custom Properties
    * Several bug fixes and code improvements

### Release 1.5.0
    * Improved CSS support
    * WPF Rendering: Reworked text-path rendering
        - Support for SVG Fonts
        - Support for letter-spacing, textLength, startOffset etc
        - For simplicity, characters are rendered as path geometry
    * WPF Rendering: Improved SVG Fonts supports
        - Text decorations: Underline, Overline and strike-through features
    * Issue #115: WPF Rendering: Thread safety fixes
    * Several bug fixes and code improvements

### Release 1.4.0
    * Design time update to SvgImage and SvgImageConverter.
    * Adding support for href in addition to the xlink:href (as in SVG 2)
    * Improvements in the WPF runtime library
        - ZoomPanControl is updated to the latest version
        - SvgImage renamed to SvgImageNameScope (not SvgImageExtension in converter library)
    * GDI+ Rendering: Resumed the developments of this renderer
        - Code Refactoring, improvements and bug fixes
        - Improvements in the SvgPictureBox control
    * WPF Rendering: Initial support for SVG fonts
    * New Points and Path Parsers (based on Batik path parsing)
        - Introduced new points and paths parser for better error handling
        - Removed the fallback for the WPF PathGeometry parser
    * Introduced WpfDrawingDocument to manage drawing information
        - This will simply the search for ID and provide hit-testing
        - This preserves the format of the SVG ID, x:Name must comply to variable naming
    * Introduced EmbeddedImageSerializerVisitor to serialize embedded images to file, where required
    * Improved color syntax support
        - Support for all color syntaxes: rgb, rgba, hsl, hsla
        - Fixed the reported issues #99, #112
    * Issue #111: Trim for quotes to ResolveUri methods (fixed by @mmatriccino)
    * Several bug fixes and code improvements

### Release 1.3.0
    * Support for fallback paint server.
    * Issue #81: Mouse Click on DOM Element. 
    * Issue #82: Please add Databinding. Added new image source with binding support.
    * Issue #83: Show no Image when Path is not pointing to an existing file.
    * Issue #84: Transfrom parsing with floating numbers (submitted by @grayed).
    * Issue #87: SvgViewbox doesn't show ToolTip.
    * Issue #88: Make SvgDrawingCanvas Drawings public.
    * Issue #95: Wrong rect displaying.
    * Issue #96: Problem with SVG logo. Rework of the use element WPF rendering.

### Release 1.2.0
    * Improvements in marker support, including marker in curves.
    * Improvements in color, color-profile, coordinates, transforms and aspect ratios. 
    * Support for slice alignment.
    * Support for SVG element visitor interfaces.
    * Removed the default size of 640x480 in WPF rendering.
    * Issue #53. Binding support in controls.
    * Issue #79. Fixed clipping bug.
    * Issue #64. WPF path rendering improvements, gradient stop fix.

### Release 1.1.0
    * Moved from the .NET Client Profile to the full .NET Framework.
    * Cleanup the .NET framework dependencies.
    * Changed the converter assembly name; SharpVectors.Converters.dll to SharpVectors.Converters.Wpf.dll
    * Changed the converter assembly name; SharpVectors.Runtime.dll to SharpVectors.Runtime.Wpf.dll
    * Resolved various reported parsing and number handling issues. 
    * Resolved various reported rendering issues, especially opacity and gradient fills. 
    * Added marker support.
