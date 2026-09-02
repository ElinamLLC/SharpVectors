# Implementation Checklist: Text Rendering Improvements

## Overview

Three comprehensive design documents have been created to guide the implementation of advanced SVG text rendering features in SharpVectors:

1. **text_rendering_architecture.md** - Strategic roadmap with phased improvements
2. **text_rendering_quickstart.md** - Tactical guide with specific file locations and code patterns
3. **text_rendering_analysis.md** - Summary findings and success metrics

---

## Current State Assessment

### What Already Exists ✅

**Solid Foundation:**
- `WpfGlyphTextBuilder` - Glyph creation via GlyphRun
- Per-glyph offset arrays (`_glyphOffsets[]`) - Ready for positioning
- `WpfTextContext` - SVG attribute parsing and tracking
- Modular renderer architecture (horizontal, vertical, path)
- Typeface/font management infrastructure

**Architecture:**
```
SVG Text Element → WpfTextContext (parse attributes)
	↓
WpfTextRendering (dispatcher)
	↓
WpfHorzTextRenderer / WpfVertTextRenderer / WpfPathTextRenderer
	↓
WpfGlyphTextBuilder (CreateGlyphRun, ComputeMeasurement)
	↓
DrawingContext.DrawGlyphRun() (WPF rendering)
```

### Critical Gaps ❌

1. **Per-Character Positioning**
   - SVG `x`, `y`, `dx`, `dy` attributes parsed but not used
   - Location: Parsed in WpfTextContext, needs application in glyph offsets

2. **Character Rotation**
   - `rotate` attribute only applies to entire element
   - Per-character rotation not implemented

3. **Path Text Rotation**
   - Glyphs positioned along curve but don't rotate with tangent
   - Missing: Tangent angle calculation and per-glyph rotation

4. **Bidirectional Text**
   - Basic RTL/LTR via `bidiLevel` (implemented)
   - No character-level direction override

---

## Phase 1: Character Positioning (Recommended Starting Point)

### Goal
Enable SVG text positioning attributes: `x`, `y`, `dx`, `dy`

**Example SVG:**
```xml
<text font-size="20">
  <tspan x="10 40 70" y="20 20 20">ABC</tspan>
</text>
```

**Current Behavior:** Text rendered at default position  
**Desired Behavior:** Characters at (10,20), (40,20), (70,20)

---

### Implementation Steps

#### Step 1: Extract Positioning Attributes (2-3 hours)

**File:** `Source/SharpVectorRenderingWpf/Texts/WpfTextContext.cs`

**Task:** Ensure SVG element attributes are parsed and stored

```csharp
// Add or verify these properties exist:
public IList<double> GetXPositions(SvgTextContentElement element)
// Returns: [10, 40, 70, ...]

public IList<double> GetYPositions(SvgTextContentElement element)
// Returns: [20, 20, 20, ...]

public IList<double> GetDXOffsets(SvgTextContentElement element)
public IList<double> GetDYOffsets(SvgTextContentElement element)
```

**Validation:** Check if these already parse from SVG or if new logic needed

---

#### Step 2: Update WpfGlyphTextBuilder (3-4 hours)

**File:** `Source/SharpVectorRenderingWpf/Texts/WpfGlyphTextBuilder.cs`

**Key Method:** `ComputeMeasurement(string text, double OriginX, double OriginY)`

**Current Code (around line 560):**
```csharp
private void ComputeMeasurement(string text, double OriginX, double OriginY)
{
	_unicodeString = text;
	_glyphRun = null;
	ParseGlyphRunProperties();

	// ... alignment calculation ...

	// Applies origin (OriginX, OriginY) to entire text
	_glyphRunOrigin = new Point(OriginX, OriginY);
}
```

**Enhancement Needed:**
```csharp
public void SetCharacterPositioning(
	IList<double> charX,   // Per-character absolute X
	IList<double> charY,   // Per-character absolute Y
	IList<double> charDX,  // Per-character relative DX
	IList<double> charDY)  // Per-character relative DY
{
	// Store these for use in ComputeMeasurement()
	_characterPositioningData = new CharacterPositioningData 
	{ 
		AbsoluteX = charX,
		AbsoluteY = charY,
		RelativeX = charDX,
		RelativeY = charDY
	};
}

private Point[] ComputeCharacterOffsets(string text, Point basePosition)
{
	var offsets = new Point[text.Length];
	double x = basePosition.X;
	double y = basePosition.Y;

	for (int i = 0; i < text.Length && i < _characterPositioningData.AbsoluteX.Count; i++)
	{
		// Apply absolute positioning
		if (_characterPositioningData.AbsoluteX[i] != 0)
			x = _characterPositioningData.AbsoluteX[i];
		else
			x += _characterPositioningData.RelativeX[i];

		if (_characterPositioningData.AbsoluteY[i] != 0)
			y = _characterPositioningData.AbsoluteY[i];
		else
			y += _characterPositioningData.RelativeY[i];

		offsets[i] = new Point(x, y);
	}

	return offsets;
}
```

**Test Point:** Verify `_glyphOffsets[]` now contains character-specific positions

---

#### Step 3: Wire Up in Renderers (2-3 hours)

**File:** `Source/SharpVectorRenderingWpf/Texts/WpfHorzTextRenderer.cs`

**Key Method:** `RenderText()` (around line 963) or `RenderTextRun()` (around line 1182)

**Change:**
```csharp
// Before building glyph:
var textContext = this._textRendering.TextContext;
var charX = textContext.GetXPositions(element);
var charY = textContext.GetYPositions(element);
var charDX = textContext.GetDXOffsets(element);
var charDY = textContext.GetDYOffsets(element);

// Configure builder
glyphBuilder.SetCharacterPositioning(charX, charY, charDX, charDY);

// Then proceed with standard GlyphRun creation
_glyphRun = glyphBuilder.CreateGlyphRun(...);
```

---

#### Step 4: Testing (2-3 hours)

**Unit Tests:**
- [ ] Single character with explicit x/y
- [ ] Multiple characters with array of positions
- [ ] Mixed absolute/relative positioning
- [ ] Comparison to SVG renderer baseline

**Visual Tests:**
```xml
<!-- Test 1: Spread characters -->
<text font-size="20">
  <tspan x="10 50 90" y="30">ABC</tspan>
</text>

<!-- Test 2: Relative offsets -->
<text font-size="20">
  <tspan dx="10 10 10">ABC</tspan>
</text>

<!-- Test 3: Mixed -->
<text font-size="20">
  <tspan x="10" dx="40 40">ABC</tspan>
</text>
```

---

### Estimate: 9-13 Hours Total

| Task | Hours | Priority |
|------|-------|----------|
| Attribute parsing verification | 1 | Critical |
| WpfGlyphTextBuilder enhancement | 4 | Critical |
| Renderer wiring | 3 | Critical |
| Testing & validation | 3-4 | Important |
| Documentation | 1 | Nice-to-have |

---

## Phase 2: Character Rotation (After Phase 1)

### Goal
Support SVG `rotate` attribute per character

**Example SVG:**
```xml
<text font-size="20">
  <tspan rotate="0 45 90 135">ABCD</tspan>
</text>
```

### High-Level Strategy

1. Detect per-character rotation in `WpfHorzTextRenderer`
2. If rotation array varies by character:
   - Split rendering into per-glyph or per-cluster loops
   - Apply RotateTransform for each glyph
3. If uniform rotation:
   - Apply single transform to entire GlyphRun (current behavior)

### Pseudo-code
```csharp
private void RenderTextWithRotation(DrawingContext drawContext, 
	GlyphRun glyphRun, IList<double> rotations, Point[] positions)
{
	bool hasVariedRotation = rotations.Distinct().Count() > 1;

	if (!hasVariedRotation)
	{
		// Simple case: single rotation for all glyphs
		drawContext.PushTransform(new RotateTransform(rotations[0]));
		drawContext.DrawGlyphRun(brush, glyphRun);
		drawContext.Pop();
	}
	else
	{
		// Complex case: per-glyph rotation
		for (int i = 0; i < glyphRun.GlyphCount; i++)
		{
			var singleGlyph = SplitGlyphRunByGlyph(glyphRun, i);
			var transform = new RotateTransform(rotations[i], positions[i].X, positions[i].Y);

			drawContext.PushTransform(transform);
			drawContext.DrawGlyphRun(brush, singleGlyph);
			drawContext.Pop();
		}
	}
}
```

### Estimate: 6-8 Hours
- Implementation: 4-5 hours
- Testing: 2-3 hours

---

## Phase 3: Path Text Rotation (After Phase 1-2)

### Goal
Auto-rotate glyphs to follow curve tangent

### Files Involved
- `Source/SharpVectorRenderingWpf/Texts/WpfPathTextRenderer.cs`
- `WpfGlyphTextBuilder.cs`

### Strategy
```csharp
// In RenderTextPath():
double currentOffset = 0;
double[] glyphRotations = new double[glyphCount];

for (int i = 0; i < glyphCount; i++)
{
	// Get point and tangent on path
	if (pathGeometry.GetPointAtFractionLength(
		currentOffset / pathLength, 
		out Point pt, 
		out Point tangent))
	{
		// Calculate angle from tangent
		double angle = Math.Atan2(tangent.Y, tangent.X) * 180 / Math.PI;
		glyphRotations[i] = angle;
		glyphPositions[i] = pt;
	}

	currentOffset += glyphAdvanceWidths[i];
}

// Apply rotations during rendering
RenderTextWithRotation(drawContext, glyphRun, glyphRotations, glyphPositions);
```

### Estimate: 5-7 Hours
- Tangent calculation: 2 hours
- Apply rotations: 2 hours
- Testing: 1-3 hours

---

## Phase 4: Bidirectional Text (Lower Priority)

### Goal
Character-level direction handling for Arabic, Hebrew, etc.

### Strategy
- Extend `WpfTextContext` to track character-level direction
- Use Uniscribe API via P/Invoke for complex text shaping
- Handle character mirroring for RTL text

### Estimate: 8-10 Hours
- Uniscribe integration: 4-5 hours
- Character mirroring: 2-3 hours
- Testing with RTL samples: 2-3 hours

---

## Development Workflow

### Before Starting Code

1. **Read the Documents:**
   - `text_rendering_architecture.md` - Strategic context
   - `text_rendering_quickstart.md` - Tactical reference
   - This checklist - Implementation roadmap

2. **Verify Assumptions:**
   - [ ] Check `WpfTextContext` has attribute parsing for x/y/dx/dy
   - [ ] Search codebase for existing character position handling
   - [ ] Identify if GlyphRun can be efficiently split

3. **Set Up Testing:**
   - [ ] Create SVG test samples in `Samples/` directory
   - [ ] Add unit tests to testing project
   - [ ] Compare with W3C reference SVG tests

### During Implementation

1. **Keep Changes Minimal:**
   - Extend, don't rewrite
   - Maintain backward compatibility
   - Add features as optional parameters

2. **Build Incrementally:**
   - Get Phase 1 working first
   - Add tests as you go
   - Don't move to Phase 2 until Phase 1 is solid

3. **Performance Awareness:**
   - Profile before optimizing
   - Track rendering time for large text
   - Monitor memory with character arrays

### After Implementation

1. **Validate:**
   - Run W3C SVG text test suite
   - Test with real-world SVG files
   - Compare to other SVG renderers (Firefox, Chrome)

2. **Document:**
   - Add comments explaining new code
   - Update user documentation if needed
   - Record any breaking changes (should be none)

3. **Optimize:**
   - Identify hot paths
   - Cache computed values if needed
   - Consider lazy evaluation

---

## Risk Mitigation

### Risk: GlyphRun Splitting Performance

**Impact:** High - Per-character rotation requires split GlyphRun

**Mitigation:**
1. Profile before implementing
2. Consider caching rotated glyphs as Geometry
3. Only split when necessary (detect variant rotation)
4. Batch similar rotations together

---

### Risk: SVG Spec Compliance

**Impact:** Medium - Different handling of positioning edge cases

**Mitigation:**
1. Reference W3C SVG 2 spec (https://www.w3.org/TR/SVG2/text.html)
2. Test against W3C test suite
3. Document any simplifications/limitations

---

### Risk: Breaking Existing Functionality

**Impact:** High - Text rendering affects many applications

**Mitigation:**
1. Make all changes backward compatible
2. New features are opt-in (via method calls)
3. Run full regression test suite before committing
4. Keep old code path as fallback

---

## Success Criteria

### Phase 1 Complete
- [ ] Characters positioned per SVG x/y/dx/dy attributes
- [ ] Tests pass for positioning edge cases
- [ ] Backward compatible (no breaking changes)
- [ ] Performance regression < 5%
- [ ] Documentation updated

### Phase 2 Complete
- [ ] Per-character rotation working
- [ ] Performance acceptable (cache if needed)
- [ ] Tests for rotation edge cases
- [ ] Interaction with positioning verified

### Phase 3 Complete
- [ ] Path text glyphs rotate with curve
- [ ] Tangent calculation accurate
- [ ] Visual quality matches expectations

### Phase 4 Complete
- [ ] Arabic/Hebrew text renders correctly
- [ ] Proper glyph mirroring
- [ ] Character-level direction handled

---

## File Reference Quick Guide

| File | Purpose | Phase | Changes |
|------|---------|-------|---------|
| `WpfTextContext.cs` | SVG parsing | 1 | Verify x/y/dx/dy extraction |
| `WpfGlyphTextBuilder.cs` | Glyph creation | 1,2,3 | Add positioning, rotation logic |
| `WpfHorzTextRenderer.cs` | Horizontal flow | 1,2 | Wire positioning/rotation |
| `WpfVertTextRenderer.cs` | Vertical flow | 1,2 | Adapt for vertical |
| `WpfPathTextRenderer.cs` | Path text | 3 | Add tangent calculation |
| `WpfTextRenderer.cs` | Base class | All | May need helper methods |

---

## Next Steps

1. **This Week:**
   - [ ] Read all three text rendering documents
   - [ ] Create SVG test samples
   - [ ] Explore `WpfTextContext` and verify attribute parsing

2. **Next Week:**
   - [ ] Start Phase 1 implementation
   - [ ] Add unit tests
   - [ ] Get first visual validation

3. **Following Weeks:**
   - [ ] Complete Phase 1
   - [ ] Plan Phase 2 based on Phase 1 learnings
   - [ ] Begin Phase 2 if resources available

---

## Resources

**Documentation (in this repo):**
- `Docs/articles/text_rendering_architecture.md` - Detailed roadmap
- `Docs/articles/text_rendering_quickstart.md` - Code location guide
- `Docs/articles/text_rendering_analysis.md` - Current state analysis

**External References:**
- W3C SVG 2 Text: https://www.w3.org/TR/SVG2/text.html
- WPF GlyphRun: https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.glyphrun
- PathGeometry: https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.pathgeometry

---

## Questions & Clarifications

**Q: Should Phase 1 support inherited x/y from parent elements?**  
A: Yes, but start simple. Parent inheritance can be added in Phase 1b.

**Q: What if SVG has more x values than characters?**  
A: Use available values, ignore extras. This matches SVG spec behavior.

**Q: Can we use FormattedText instead of GlyphRun splitting?**  
A: No - FormattedText doesn't provide per-character transform control. Stay with GlyphRun.

**Q: Should we optimize for mono-spaced fonts first?**  
A: Consider it optimization, not first pass. Handle general case first.

---

## Sign-Off

Implementation roadmap complete and documented. Ready to begin Phase 1: Character Positioning.

Started: [Date]  
Documentation Complete: [Date]  
Phase 1 Target: [Date]  
Phase 1-3 Target: [Date]

