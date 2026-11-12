---
uid: topic_faq
title: FAQ
---

# FAQ - Q&A
In this article, we will provide answers to known questions, how to work around known issues, etc

## WPF Component

### Loading fonts from SVG documents

`WpfDrawingSettings` provides separate controls for document-referenced fonts. They are independent of `ExternalResourcesAccessMode`:

- `LoadFonts` (default `true`) controls referenced fonts, including embedded `@font-face` data and `font-face-uri` files. Inline SVG fonts and system fonts are not disabled by this setting.
- `AllowLocalFontFiles` (default `true`) allows document-referenced font files to be read. Set it to `false` to skip them.
- `AllowFontFileWrites` (default `true`) allows embedded fonts and converted WOFF fonts to be cached on disk. Set it to `false` to skip these fonts; ordinary local TTF and OTF files can still be read when local font files are allowed.
- `AllowUncFontPaths` (default `false`) allows explicit UNC font paths and an explicit UNC font-cache directory when set to `true`. This check does not detect mapped network drives or filesystem links to network locations.

For SVGs from untrusted sources, set `LoadFonts = false` if document-referenced fonts are not required. These settings govern font loading only; they are not a general filesystem or network sandbox. With font caching enabled, distinct embedded fonts can consume disk space, so applications should also apply input and resource limits appropriate for their environment.

The same properties are available on `SvgWindow` for applications that use the SVG model directly. Configure them before loading a document.

### Loading images from SVG documents

`WpfDrawingSettings` also provides independent image controls:

- `LoadImages` (default `true`) controls SVG `<image>` elements, including embedded data and referenced SVG images. `CanUseBitmap` remains a separate, bitmap-only option.
- `AllowLocalImageFiles` (default `true`) permits file references used by image elements and their color profiles.
- `AllowUncImagePaths` (default `false`) permits explicit UNC image and color-profile paths when set to `true`. Mapped network drives and filesystem links are not detected.
- `AllowImageFileWrites` (default `true`) permits the built-in `EmbeddedImageSerializerVisitor` to write images. Set it to `false` to keep its decoded images in memory. Constructing that visitor with a directory can itself create the directory; the setting does not restrict custom visitors or application-controlled output files.

The first three properties are also available on `SvgWindow` and inherited by nested SVG windows. These settings do not replace `ExternalResourcesAccessMode` or the URL resolver policy: existing external-resource checks still apply, and some existing bitmap checks also treat local file URIs as external. They are not a general filesystem or network sandbox, and do not limit image size or decoder resource use. Apply application-level input limits and isolation as appropriate for untrusted SVGs.

### Threading Issues

## GDI+ Component

