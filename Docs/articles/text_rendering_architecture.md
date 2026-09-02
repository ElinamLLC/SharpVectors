# Advanced SVG Text Rendering Architecture

## Current Implementation Overview

### Existing Glyph-Based Foundation

SharpVectors already leverages **GlyphRun** for text rendering, which is the correct approach for SVG compliance. The current architecture:

**Key Components:**

1. **WpfGlyphTextBuilder** (`Source/SharpVectorRenderingWpf/Texts/WpfGlyphTextBuilder.cs`)
   - Core glyph extraction and management
   - Uses `GlyphTypeface` to access font glyphs
   - Creates `GlyphRun` objects with:
	 - Glyph indices
	 - Advance widths
	 - Glyph offsets (x, y positioning)
	 - Cluster mapping
	 - Bidi level support

2. **Text Renderers (Horizontal/Vertical/Path)**
   - `WpfHorzTextRenderer` - Standard horizontal text flow
   - `WpfVertTextRenderer` - Vertical writing systems
   - `WpfPathTextRenderer` - Text following paths
   - Each handles orientation, rotation, and positioning

3. **WpfTextContext**
   - Manages text layout state
   - Tracks positioning attributes
   - Handles SVG text semantics

### Current Capabilities

✅ **Implemented:**
- Basic character rendering via glyphs
- Font family, size, weight, and style selection
- Horizontal and vertical text direction
- Text on paths (`<textPath>`)
- Basic transformations
- Multiple writing systems (LTR/RTL via bidi level)
- Ligature handling (via `GlyphTypeface`)

### Identified Gaps for Full SVG Compliance

❌ **Missing/Limited:**
1. **Character-level positioning**
   - `x`, `y`, `dx`, `dy` attributes on individual characters
   - Per-character rotation
   - Character-level spacing adjustments

2. **Text Path Features**
   - Proper curve following with character rotation
   - Text anchor point positioning on paths
   - Curve offset parameters

3. **Complex Bidirectional Text**
   - Character-level direction overrides
   - Mixed LTR/RTL within same element
   - Proper `bidi-override` handling

4. **Advanced Transformations**
   - Glyph-level matrix transformations
   - Per-character skew/rotation
   - Proper interaction with SVG transforms

5. **Text Effects**
   - Baseline alignment variants
   - Glyph substitution (`<altGlyph>`)
   - Custom font support within SVG

## Improvement Roadmap

### Phase 1: Character Positioning (High Priority)

**Objective:** Enable per-character `x`, `y`, `dx`, `dy` attributes

**Implementation:**
```csharp
// Proposed enhancement to WpfGlyphTextBuilder
public class CharacterPositioningData
{
	public double? AbsoluteX { get; set; }      // x attribute
	public double? AbsoluteY { get; set; }      // y attribute
	public double RelativeX { get; set; }       // dx attribute
	public double RelativeY { get; set; }       // dy attribute
	public double? Rotation { get; set; }       // rotate attribute
}

// Modified glyph offset calculation
private Point[] ComputeCharacterPositions(
	string text,
	IList<CharacterPositioningData> positions,
	Point currentPosition)
{
	var glyphOffsets = new Point[text.Length];
	double x = currentPosition.X;
	double y = currentPosition.Y;

	for (int i = 0; i < text.Length; i++)
	{
		var charPos = positions[i];

		// Apply absolute positioning
		if (charPos.AbsoluteX.HasValue)
			x = charPos.AbsoluteX.Value;
		else
			x += charPos.RelativeX;

		if (charPos.AbsoluteY.HasValue)
			y = charPos.AbsoluteY.Value;
		else
			y += charPos.RelativeY;

		glyphOffsets[i] = new Point(x, y);
	}

	return glyphOffsets;
}
```

**Files to Modify:**
- `WpfGlyphTextBuilder.cs` - Core glyph positioning logic
- `WpfHorzTextRenderer.cs` - Character iteration and position collection
- `WpfVertTextRenderer.cs` - Vertical orientation support

**Testing:**
- SVG strings with `x/y` attributes
- Mixed absolute/relative positioning
- Baseline adjustments

---

### Phase 2: Glyph Rotation (Medium Priority)

**Objective:** Support per-character rotation while maintaining proper layout

**Implementation:**
```csharp
// Extend CharacterPositioningData
public double? Rotation { get; set; }  // rotate attribute in degrees

// Apply rotation to individual glyphs
private void ApplyCharacterRotation(
	DrawingContext drawContext,
	GlyphRun glyphRun,
	IList<double?> rotations,
	Point[] positions)
{
	// Split GlyphRun by character and apply transforms
	for (int i = 0; i < rotations.Count; i++)
	{
		if (rotations[i].HasValue && rotations[i] != 0)
		{
			// Create single-glyph GlyphRun
			var singleGlyph = SplitGlyphRun(glyphRun, i);

			// Apply rotation transform
			var transform = new RotateTransform(
				rotations[i].Value,
				positions[i].X,
				positions[i].Y);

			drawContext.PushTransform(transform);
			drawContext.DrawGlyphRun(Brushes.Black, singleGlyph);
			drawContext.Pop();
		}
		else
		{
			// Draw unrotated glyph
		}
	}
}
```

**Challenge:** Current GlyphRun architecture groups glyphs. Character-level rotation requires:
- Breaking GlyphRun into per-character segments
- Managing transform context per glyph
- Maintaining proper text flow and hit-testing

**Alternatives:**
- Use `FormattedText` for individual characters (simpler but less control)
- Cache transformed glyphs as `Geometry` (better performance)

---

### Phase 3: Text Path Enhancements (Medium Priority)

**Objective:** Proper text rotation along curves with anchor positioning

**Enhancement to WpfPathTextRenderer:**
```csharp
public class TextPathPositioning
{
	public double StartOffset { get; set; }           // startOffset attribute
	public TextPathMethod Method { get; set; }         // align attribute
	public TextPathSpacing Spacing { get; set; }       // spacing attribute
}

private void AdjustGlyphsAlongPath(
	PathGeometry path,
	Point[] glyphOffsets,
	double[] glyphWidths,
	TextPathPositioning positioning)
{
	double pathLength = CalculatePathLength(path);
	double currentOffset = positioning.StartOffset;

	for (int i = 0; i < glyphOffsets.Length; i++)
	{
		// Get point on path at current offset
		if (path.GetPointAtFractionLength(
			currentOffset / pathLength, out Point point, out Point tangent))
		{
			// Rotate glyph according to path tangent
			double angle = Math.Atan2(tangent.Y, tangent.X) * 180 / Math.PI;

			glyphOffsets[i] = new Point(point.X, point.Y);
			glyphRotations[i] = angle;
		}

		currentOffset += glyphWidths[i];
	}
}
```

**Related Files:**
- `WpfPathTextRenderer.cs` - Path curve following logic
- `WpfGlyphTextBuilder.cs` - Glyph offset application

---

### Phase 4: Bidirectional Text Improvements (Low-Medium Priority)

**Current State:** `GlyphRun` supports `bidiLevel` for basic RTL/LTR

**Enhancement:**  detailed character-level direction handling:

```csharp
public class BidiCharacterData
{
	public BidiDirection Direction { get; set; }      // LTR, RTL, or Override
	public bool IsHidden { get; set; }                // bidi-control chars
	public bool IsMirrorableChar { get; set; }        // () [] {} <> etc
}

private void ApplyBidiOverrides(
	string text,
	IList<BidiCharacterData> bidiData)
{
	// Handle mixed directionality within same text run
	// Apply character mirroring for RTL text
	// Respect bidi-override hints
}
```

**Windows API Resource:**
- `Uniscribe` (via P/Invoke) - Advanced text shaping
- Unicode Bidirectional Algorithm implementation

---

### Phase 5: Text Effects & Styling (Lower Priority)

**Baseline Variants:**
```csharp
public enum TextBaseline
{
	Auto,           // Dominant baseline
	Alphabetic,     // Latin baseline
	Ideographic,    // CJK baseline
	Hanging,        // Devanagari baseline
	Mathematical    // Mathematical baseline
}

private Point AdjustForBaseline(
	TextBaseline baseline,
	GlyphRun glyphRun,
	Point currentPosition)
{
	// Adjust Y position based on baseline
	var metrics = glyphRun.GlyphRunTypeface.FamilyMapInfo(??);
	// Apply appropriate vertical offset
}
```

**Glyph Substitution (`<altGlyph>`):**
```csharp
private GlyphRun SubstituteGlyph(
	ushort originalGlyph,
	string fontName,
	string glyphName)
{
	// Look up alternate glyph from specified font
	// Return modified GlyphRun with substituted indices
}
```

---

## Implementation Strategy

### Step-by-Step Approach

1. **Create TextCharacterData Structure**
   - Consolidate positioning, rotation, and styling per character
   - Integrate with `WpfTextContext`

2. **Enhance WpfGlyphTextBuilder**
   - Modify `ComputeMeasurement()` to accept per-character data
   - Update `CreateGlyphRun()` signatures
   - Add character splitting/rotation helpers

3. **Update Renderers**
   - `WpfHorzTextRenderer::RenderHorzTextRun()` - Collect positioning attributes
   - `WpfVertTextRenderer::RenderVertTextRun()` - Adapt for vertical
   - `WpfPathTextRenderer::RenderTextPath()` - Integrate curve positioning

4. **Drawing Context Changes**
   - May require splitting GlyphRun for per-character rotation
   - Manage transform stack for rotated glyphs
   - Maintain proper hit-testing (DrawingVisual or GeometryGroup)

5. **Testing & Validation**
   - W3C SVG text test suite coverage
   - Arabic/Hebrew text samples
   - Path-following text
   - Performance benchmarking (glyph splitting overhead)

---

## Backward Compatibility

✅ **All changes should be additive:**
- Existing `GlyphRun` path remains default
- Optional per-character data structures
- Graceful degradation if positioning data unavailable
- No breaking changes to public APIs

---

## Performance Considerations

**Current:** Single `GlyphRun` per text element  
**With Rotation:** Multiple `GlyphRun` objects or per-glyph transforms

**Optimization Strategies:**
1. **Lazy evaluation** - Only split/rotate if necessary
2. **Geometry caching** - Pre-compute rotated glyphs
3. **Batch transforms** - Group similarly-rotated glyphs
4. **DrawingVisual pooling** - Reuse visual objects

---

## Windows API Integration Points

| Task | API | Usage |
|------|-----|-------|
| Glyph metrics | `GlyphTypeface` metrics | Baseline, advance width calculations |
| Path length | `PathGeometry.GetPointAtFractionLength()` | Text path positioning |
| Bidirectional text | `Uniscribe` (via P/Invoke) | Complex shaping for Arabic/Hebrew |
| Text shaping | Windows Text Services Framework | Advanced ligatures and diacritics |

---

## References

- **SVG 2 Text Spec:** https://www.w3.org/TR/SVG2/text.html
- **WPF GlyphRun:** https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.glyphrun
- **Uniscribe:** https://en.wikipedia.org/wiki/Uniscribe
- **Unicode Bidirectional Algorithm:** https://www.unicode.org/reports/tr9/

---

## Next Steps

1. **Prioritize improvements** based on real-world SVG test failures
2. **Prototype Phase 1** (character positioning) as quickest win
3. **Gather performance baseline** before major changes
4. **Engage W3C test suite** to measure compliance improvements
