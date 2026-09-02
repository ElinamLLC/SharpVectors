# Text Rendering Improvement - Quick Start Guide

## Current Codebase Entry Points

### Core Files to Understand

**1. Glyph Creation: `WpfGlyphTextBuilder.cs` (933 lines)**
- **Key Method:** `CreateGlyphRun()` (line ~520)
  - Builds WPF GlyphRun from parsed glyph data
  - Handles offset (`_glyphOffsets`) and advance width arrays
  - Supports character cluster mapping

- **Key Method:** `ComputeMeasurement()` (line ~560)
  - Calculates text layout and positioning
  - Applies origin (x, y) values
  - Computes alignment boxes

- **Key Method:** `GetGlyphFromCharacter()` (line ~726)
  - Maps Unicode character → glyph index
  - Uses GlyphTypeface lookup

- **Data Structure:** `ParsedGlyphData`
  - Per-glyph rendering info:
	```csharp
	struct ParsedGlyphData {
		ushort glyphIndex;      // Glyph ID
		double advanceWidth;    // Horizontal spacing
		double offsetX, offsetY; // Glyph positioning
	}
	```

**Why This Matters:** Offsets are already computed per-glyph! The `_glyphOffsets` array is ready to be leveraged.

---

**2. Text Flow: `WpfHorzTextRenderer.cs`**
- **Key Method:** `RenderHorzTextRun()` (line ~862)
  - Main horizontal text rendering loop
  - Iterates through text characters
  - Collects positioning attributes

- **Key Method:** `RenderHorzText()` (line ~831)
  - Handles `<text>` and `<tspan>` elements
  - Processes child elements recursively

**Current Limitation:** Doesn't extract per-character `x`, `y`, `dx`, `dy` attributes from SVG

---

**3. Path Text: `WpfPathTextRenderer.cs`**
- **Key Method:** `RenderTextPath()` (line ~931)
  - References path geometry
  - Follows curve for text flow

**Current Limitation:** No character-level rotation based on curve tangent

---

**4. Text Layout Context: `WpfTextContext.cs`**
- Maintains current position, rotation, and text state
- Tracks transformation matrix
- Collects SVG text attributes

---

### Supporting Structure: `WpfTextBuilder.cs`

**Base class for text rendering:**
- Font management
- Measurement helpers
- Text cleanup (whitespace handling)

---

## Quick-Win Improvements

### #1: Character Position Collection (Easiest)

**Goal:** Extract SVG `x`, `y`, `dx`, `dy` attributes per character

**Current Code Location:**  
`WpfHorzTextRenderer.RenderHorzTextRun()` (~900 lines)

**Change Required:**
```csharp
// Add to character iteration loop (current code processes text)
string text = ...; // from element

// NEW: Collect positioning attributes
double[] charX = new double[text.Length];    // x attribute values
double[] charY = new double[text.Length];    // y attribute values
double[] charDX = new double[text.Length];   // dx attribute values
double[] charDY = new double[text.Length];   // dy attribute values

// Extract from SVG element attributes
// (These are already parsed in WpfTextContext)

// Pass to builder:
glyphBuilder.SetCharacterPositioning(charX, charY, charDX, charDY);
```

**Affected Methods:**
1. Extract attribute parsing in `WpfTextContext`
2. Create `SetCharacterPositioning()` in `WpfGlyphTextBuilder`
3. Apply offsets in `ComputeMeasurement()`

**Why It's Easy:**
- No rendering logic changes
- Just better use of existing `_glyphOffsets` array
- SVG attributes already parsed

---

### #2: Character Rotation Handling (Medium Complexity)

**Goal:** Support SVG `rotate` attribute per character

**Current Limitation:**  
Rotation applies to entire text element, not per-character

**Strategy:**
1. Parse `rotate` attribute into per-character array
2. In `WpfHorzTextRenderer.RenderHorzTextRun()`, detect if individual rotation differs
3. If so, split rendering:
   ```csharp
   if (HasPerCharacterRotation(rotate))
   {
	   RenderWithCharacterRotation(glyphBuilder, rotate);
   }
   else
   {
	   RenderNormal(glyphBuilder);  // Current path
   }
   ```

4. `RenderWithCharacterRotation()` pseudo-code:
   ```csharp
   for (int i = 0; i < text.Length; i++)
   {
	   // Create single-char GlyphRun
	   var singleGlyph = BuildSingleCharGlyph(text[i], font, size);

	   // Apply rotation transform
	   drawContext.PushTransform(
		   new RotateTransform(rotate[i], position[i].X, position[i].Y));
	   drawContext.DrawGlyphRun(brush, singleGlyph);
	   drawContext.Pop();
   }
   ```

---

### #3: Text Path Rotation (Medium Complexity)

**Goal:** Auto-rotate glyphs to follow curve direction

**Current Code:**  
`WpfPathTextRenderer.RenderTextPath()` positions characters along path  
BUT doesn't compute per-character rotation angle

**Enhancement:**
```csharp
// In RenderTextPath():
double currentOffset = 0;
for (int i = 0; i < text.Length; i++)
{
	// Get point on path
	path.GetPointAtFractionLength(
		currentOffset / pathLength, 
		out Point pt, 
		out Point tangent);

	// Compute character rotation from tangent
	double angle = Math.Atan2(tangent.Y, tangent.X);

	positions[i] = pt;
	rotations[i] = angle;

	currentOffset += glyphAdvanceWidth[i];
}

// Use rotations[i] during glyph rendering
```

**Files Affected:**
- `WpfPathTextRenderer.cs`
- May need `PathGeometry.GetTangentAtFractionLength()` or similar

---

## Discovery Tasks (Next Session)

### Before Coding:

1. **Verify SVG Attribute Parsing**
   ```
   File: WpfTextContext.cs
   Q: Where are x/y/dx/dy attributes extracted from SVG?
   Q: Are they available during rendering, or only during parsing?
   ```

2. **Understand Existing Text Flow**
   ```
   File: WpfHorzTextRenderer.cs
   Q: How is current position (`ctp`) maintained?
   Q: Where does `rotate` currently get applied?
   Q: How does measurement pass work vs. rendering pass?
   ```

3. **Check GlyphRun Splitting Capability**
   ```
   File: WpfGlyphTextBuilder.cs
   Q: Can we create a single-character GlyphRun?
   Q: What data is needed for `CreateGlyphRun()`?
   Q: Is there existing code that splits GlyphRun by character?
   ```

4. **Path Geometry Capabilities**
   ```
   Q: Does PathGeometry expose tangent/slope info?
   Q: What API exists for "point at distance along path"?
   ```

---

## Testing Strategy

### Phase 1 Test Cases (Character Positioning)

**Input SVG:**
```xml
<text font-size="20">
  <tspan x="10" y="20">A</tspan>
  <tspan x="40" y="20">B</tspan>
  <tspan x="70" y="20">C</tspan>
</text>
```

**Expected:** Characters at (10,20), (40,20), (70,20) instead of baseline flow

---

### Phase 2 Test Cases (Rotation)

```xml
<text>
  <tspan rotate="0 45 90 135">ABCD</tspan>
</text>
```

**Expected:** Each character rotated individually

---

### Phase 3 Test Cases (Text Path)

```xml
<path id="curve" d="M 0 0 Q 50 50 100 0"/>
<text>
  <textPath href="#curve">Text along curve</textPath>
</text>
```

**Expected:** Characters follow curve and rotate with it

---

## Performance Benchmarks to Establish

Before optimizing, measure:
1. Render time for 1000-character text with per-character rotation
2. Memory overhead of character-indexed arrays
3. DrawingContext push/pop transform cost

---

## References in Codebase

**Existing similar work:**
- Look for `ComputeAlignmentBox()` usage → understand baseline handling
- Search `BuildGeometry()` → see how glyphs become Geometry
- Review `WpfDrawingDocument.HitTestDrawing()` → hit-testing logic

**Don't Reinvent:**
- Font metrics already computed in `GlyphTypeface`
- Transformation stack in `DrawingContext` 
- SVG attribute parsing in `WpfTextContext`

---

## High-Impact, Low-Effort Changes

**If time-limited, focus on:**

1. **Better Attribute Propagation** (2-3 hours)
   - Ensure `x/y/dx/dy` parsed from SVG
   - Pass to `WpfGlyphTextBuilder`
   - Test with samples

2. **Individual Glyph Rotation** (4-6 hours)
   - Split rendering path for `rotate` attribute
   - No architecture change needed
   - Immediate visual correctness boost

3. **Path Tangent Calculation** (3-4 hours)
   - Use `PathGeometry` to compute curve angles
   - Apply computed angles to glyphs
   - Improves `<textPath>` appearance significantly

---

## Files Checklist

| File | Purpose | Priority |
|------|---------|----------|
| `WpfGlyphTextBuilder.cs` | Glyph data + offsets | Critical |
| `WpfHorzTextRenderer.cs` | Attribute collection | Critical |
| `WpfTextContext.cs` | SVG parsing | Important |
| `WpfPathTextRenderer.cs` | Curve positioning | Important |
| `WpfVertTextRenderer.cs` | Vertical flow | Medium |
| `WpfTextBuilder.cs` | Base utilities | Reference |

