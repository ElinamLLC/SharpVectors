using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// Validates CSS property values against CSS specifications.
    /// Provides validation rules for common CSS properties and returns detailed feedback.
    /// </summary>
    public class CssPropertyValidator
    {
        #region Static Members

        // Regex patterns for common value types
        private static readonly Regex _hexColorPattern = new Regex(@"^#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6}|[0-9A-Fa-f]{8})$");
        private static readonly Regex _rgbColorPattern = new Regex(@"^rgb\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)$", RegexOptions.IgnoreCase);
        private static readonly Regex _rgbaColorPattern = new Regex(@"^rgba\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*([\d.]+)\s*\)$", RegexOptions.IgnoreCase);
        private static readonly Regex _hslColorPattern = new Regex(@"^hsl\s*\(\s*(\d+)\s*,\s*(\d+)%\s*,\s*(\d+)%\s*\)$", RegexOptions.IgnoreCase);
        private static readonly Regex _lengthPattern = new Regex(@"^(-?\d+(?:\.\d+)?)(px|em|rem|cm|mm|in|pt|pc|ex|ch|vw|vh|vmin|vmax|%)$", RegexOptions.IgnoreCase);
        private static readonly Regex _numberPattern = new Regex(@"^-?\d+(?:\.\d+)?$");
        private static readonly Regex _urlPattern = new Regex(@"^url\s*\(\s*['\x22]?([^'\x22]+)['\x22]?\s*\)$", RegexOptions.IgnoreCase);

        // Common keyword values
        private static readonly HashSet<string> _commonKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "inherit", "initial", "unset", "auto", "none", "normal", "block", "inline", "inline-block",
            "flex", "grid", "absolute", "relative", "fixed", "sticky", "static", "visible", "hidden", "scroll"
        };

        private static readonly HashSet<string> _colorKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "black", "white", "red", "green", "blue", "yellow", "cyan", "magenta", "gray", "grey",
            "silver", "maroon", "navy", "teal", "olive", "lime", "aqua", "fuchsia", "purple", "orange",
            "pink", "brown", "beige", "tan", "coral", "gold", "khaki", "lavender", "salmon", "wheat",
            "transparent", "currentColor", "currentcolor"
        };

        private static readonly HashSet<string> _displayKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "inline", "block", "inline-block", "flex", "grid", "table", "table-row", "table-cell",
            "none", "contents", "flow", "flow-root", "inline-flex", "inline-grid"
        };

        private static readonly HashSet<string> _positionKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "static", "relative", "absolute", "fixed", "sticky"
        };

        private static readonly HashSet<string> _fontWeightKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "normal", "bold", "bolder", "lighter"
        };

        private static readonly HashSet<string> _fontStyleKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "normal", "italic", "oblique"
        };

        private static readonly HashSet<string> _textAlignKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "left", "right", "center", "justify", "start", "end"
        };

        private static readonly HashSet<string> _borderStyleKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "none", "solid", "dashed", "dotted", "double", "groove", "ridge", "inset", "outset"
        };

        #endregion

        #region Property Information

        /// <summary>
        /// Defines validation rules for a CSS property
        /// </summary>
        public class PropertyRuleSet
        {
            public string PropertyName { get; set; }
            public string Description { get; set; }
            public Func<string, ValidationResult> Validator { get; set; }
        }

        /// <summary>
        /// Result of validating a property value
        /// </summary>
        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }
            public string RecommendedValue { get; set; }

            public ValidationResult(bool isValid, string message = "", string recommendedValue = "")
            {
                IsValid = isValid;
                Message = message;
                RecommendedValue = recommendedValue;
            }
        }

        #endregion

        #region Private Fields

        private Dictionary<string, PropertyRuleSet> _propertyRules;

        #endregion

        #region Constructor

        public CssPropertyValidator()
        {
            _propertyRules = InitializePropertyRules();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates a CSS property value
        /// </summary>
        /// <param name="propertyName">The CSS property name</param>
        /// <param name="propertyValue">The CSS property value</param>
        /// <returns>Validation result with details</returns>
        public ValidationResult Validate(string propertyName, string propertyValue)
        {
            if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(propertyValue))
            {
                return new ValidationResult(false, "Property name or value is empty");
            }

            // Trim whitespace and normalize
            propertyValue = propertyValue.Trim();

            // Check for !important flag
            if (propertyValue.EndsWith("!important", StringComparison.OrdinalIgnoreCase))
            {
                propertyValue = propertyValue.Substring(0, propertyValue.Length - 10).Trim();
            }

            // Check if property has validation rules
            if (_propertyRules.TryGetValue(propertyName.ToLower(), out var ruleSet))
            {
                return ruleSet.Validator(propertyValue);
            }

            // Unknown properties are allowed (for compatibility)
            return new ValidationResult(true, $"Property '{propertyName}' has no validation rules (unknown property)");
        }

        /// <summary>
        /// Gets all supported properties
        /// </summary>
        public IEnumerable<string> GetSupportedProperties()
        {
            return _propertyRules.Keys.OrderBy(k => k);
        }

        #endregion

        #region Private Methods

        private Dictionary<string, PropertyRuleSet> InitializePropertyRules()
        {
            var rules = new Dictionary<string, PropertyRuleSet>(StringComparer.OrdinalIgnoreCase);

            // Color properties
            rules.Add("color", new PropertyRuleSet
            {
                PropertyName = "color",
                Description = "Text color",
                Validator = ValidateColor
            });

            rules.Add("background-color", new PropertyRuleSet
            {
                PropertyName = "background-color",
                Description = "Background color",
                Validator = ValidateColor
            });

            rules.Add("border-color", new PropertyRuleSet
            {
                PropertyName = "border-color",
                Description = "Border color",
                Validator = ValidateColor
            });

            rules.Add("background", new PropertyRuleSet
            {
                PropertyName = "background",
                Description = "Shorthand for background properties",
                Validator = ValidateBackground
            });

            // Size properties
            rules.Add("width", new PropertyRuleSet
            {
                PropertyName = "width",
                Description = "Element width",
                Validator = ValidateSizeProperty
            });

            rules.Add("height", new PropertyRuleSet
            {
                PropertyName = "height",
                Description = "Element height",
                Validator = ValidateSizeProperty
            });

            rules.Add("min-width", new PropertyRuleSet
            {
                PropertyName = "min-width",
                Description = "Minimum width",
                Validator = ValidateSizeProperty
            });

            rules.Add("max-width", new PropertyRuleSet
            {
                PropertyName = "max-width",
                Description = "Maximum width",
                Validator = ValidateSizeProperty
            });

            rules.Add("min-height", new PropertyRuleSet
            {
                PropertyName = "min-height",
                Description = "Minimum height",
                Validator = ValidateSizeProperty
            });

            rules.Add("max-height", new PropertyRuleSet
            {
                PropertyName = "max-height",
                Description = "Maximum height",
                Validator = ValidateSizeProperty
            });

            // Spacing properties
            rules.Add("margin", new PropertyRuleSet
            {
                PropertyName = "margin",
                Description = "Element margin (shorthand)",
                Validator = ValidateSpacingProperty
            });

            rules.Add("padding", new PropertyRuleSet
            {
                PropertyName = "padding",
                Description = "Element padding (shorthand)",
                Validator = ValidateSpacingProperty
            });

            rules.Add("margin-top", new PropertyRuleSet
            {
                PropertyName = "margin-top",
                Description = "Top margin",
                Validator = ValidateSpacingValue
            });

            rules.Add("margin-right", new PropertyRuleSet
            {
                PropertyName = "margin-right",
                Description = "Right margin",
                Validator = ValidateSpacingValue
            });

            rules.Add("margin-bottom", new PropertyRuleSet
            {
                PropertyName = "margin-bottom",
                Description = "Bottom margin",
                Validator = ValidateSpacingValue
            });

            rules.Add("margin-left", new PropertyRuleSet
            {
                PropertyName = "margin-left",
                Description = "Left margin",
                Validator = ValidateSpacingValue
            });

            rules.Add("padding-top", new PropertyRuleSet
            {
                PropertyName = "padding-top",
                Description = "Top padding",
                Validator = ValidatePaddingValue
            });

            rules.Add("padding-right", new PropertyRuleSet
            {
                PropertyName = "padding-right",
                Description = "Right padding",
                Validator = ValidatePaddingValue
            });

            rules.Add("padding-bottom", new PropertyRuleSet
            {
                PropertyName = "padding-bottom",
                Description = "Bottom padding",
                Validator = ValidatePaddingValue
            });

            rules.Add("padding-left", new PropertyRuleSet
            {
                PropertyName = "padding-left",
                Description = "Left padding",
                Validator = ValidatePaddingValue
            });

            // Border properties
            rules.Add("border", new PropertyRuleSet
            {
                PropertyName = "border",
                Description = "Border (shorthand)",
                Validator = ValidateBorder
            });

            rules.Add("border-width", new PropertyRuleSet
            {
                PropertyName = "border-width",
                Description = "Border width",
                Validator = ValidateBorderWidth
            });

            rules.Add("border-style", new PropertyRuleSet
            {
                PropertyName = "border-style",
                Description = "Border style",
                Validator = ValidateBorderStyle
            });

            rules.Add("border-top", new PropertyRuleSet
            {
                PropertyName = "border-top",
                Description = "Top border",
                Validator = ValidateBorder
            });

            rules.Add("border-right", new PropertyRuleSet
            {
                PropertyName = "border-right",
                Description = "Right border",
                Validator = ValidateBorder
            });

            rules.Add("border-bottom", new PropertyRuleSet
            {
                PropertyName = "border-bottom",
                Description = "Bottom border",
                Validator = ValidateBorder
            });

            rules.Add("border-left", new PropertyRuleSet
            {
                PropertyName = "border-left",
                Description = "Left border",
                Validator = ValidateBorder
            });

            // Font properties
            rules.Add("font-size", new PropertyRuleSet
            {
                PropertyName = "font-size",
                Description = "Font size",
                Validator = ValidateFontSize
            });

            rules.Add("font-weight", new PropertyRuleSet
            {
                PropertyName = "font-weight",
                Description = "Font weight",
                Validator = ValidateFontWeight
            });

            rules.Add("font-style", new PropertyRuleSet
            {
                PropertyName = "font-style",
                Description = "Font style",
                Validator = ValidateFontStyle
            });

            rules.Add("font-family", new PropertyRuleSet
            {
                PropertyName = "font-family",
                Description = "Font family",
                Validator = ValidateFontFamily
            });

            rules.Add("line-height", new PropertyRuleSet
            {
                PropertyName = "line-height",
                Description = "Line height",
                Validator = ValidateLineHeight
            });

            // Display and layout properties
            rules.Add("display", new PropertyRuleSet
            {
                PropertyName = "display",
                Description = "Display type",
                Validator = ValidateDisplay
            });

            rules.Add("position", new PropertyRuleSet
            {
                PropertyName = "position",
                Description = "Position type",
                Validator = ValidatePosition
            });

            rules.Add("text-align", new PropertyRuleSet
            {
                PropertyName = "text-align",
                Description = "Text alignment",
                Validator = ValidateTextAlign
            });

            rules.Add("float", new PropertyRuleSet
            {
                PropertyName = "float",
                Description = "Float property",
                Validator = ValidateFloat
            });

            rules.Add("clear", new PropertyRuleSet
            {
                PropertyName = "clear",
                Description = "Clear property",
                Validator = ValidateClear
            });

            // Opacity
            rules.Add("opacity", new PropertyRuleSet
            {
                PropertyName = "opacity",
                Description = "Element opacity (0-1)",
                Validator = ValidateOpacity
            });

            // Visibility
            rules.Add("visibility", new PropertyRuleSet
            {
                PropertyName = "visibility",
                Description = "Visibility",
                Validator = ValidateVisibility
            });

            return rules;
        }

        #region Validation Methods

        private ValidationResult ValidateColor(string value)
        {
            if (IsCommonKeyword(value) || _colorKeywords.Contains(value))
                return new ValidationResult(true);

            if (_hexColorPattern.IsMatch(value))
                return new ValidationResult(true);

            if (_rgbColorPattern.IsMatch(value))
            {
                var match = _rgbColorPattern.Match(value);
                var r = int.Parse(match.Groups[1].Value);
                var g = int.Parse(match.Groups[2].Value);
                var b = int.Parse(match.Groups[3].Value);

                if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
                    return new ValidationResult(false, "RGB values must be 0-255");
                return new ValidationResult(true);
            }

            if (_rgbaColorPattern.IsMatch(value))
            {
                var match = _rgbaColorPattern.Match(value);
                var r = int.Parse(match.Groups[1].Value);
                var g = int.Parse(match.Groups[2].Value);
                var b = int.Parse(match.Groups[3].Value);
                var a = double.Parse(match.Groups[4].Value);

                if ((r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255) || (a < 0 || a > 1))
                    return new ValidationResult(false, "RGB values must be 0-255, alpha must be 0-1");
                return new ValidationResult(true);
            }

            if (_hslColorPattern.IsMatch(value))
            {
                var match = _hslColorPattern.Match(value);
                var h = int.Parse(match.Groups[1].Value);
                var s = int.Parse(match.Groups[2].Value);
                var l = int.Parse(match.Groups[3].Value);

                if (h < 0 || h > 360 || s < 0 || s > 100 || l < 0 || l > 100)
                    return new ValidationResult(false, "HSL values: H 0-360, S 0-100%, L 0-100%");
                return new ValidationResult(true);
            }

            return new ValidationResult(false, "Invalid color format. Expected hex, rgb(), rgba(), hsl(), or color keyword");
        }

        private ValidationResult ValidateBackground(string value)
        {
            // Background is complex, allow URLs and colors
            if (_urlPattern.IsMatch(value))
                return new ValidationResult(true);

            return ValidateColor(value);
        }

        private ValidationResult ValidateSizeProperty(string value)
        {
            if (IsCommonKeyword(value) || value == "auto")
                return new ValidationResult(true);

            if (_lengthPattern.IsMatch(value))
                return new ValidationResult(true);

            if (_numberPattern.IsMatch(value) && value == "0")
                return new ValidationResult(true);

            return new ValidationResult(false, "Invalid size. Expected length unit (px, em, rem, %, etc.) or keyword");
        }

        private ValidationResult ValidateSpacingProperty(string value)
        {
            // Handle shorthand: 1-4 values
            var parts = value.Split(' ');
            if (parts.Length > 4)
                return new ValidationResult(false, "Margin shorthand accepts 1-4 values");

            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                if (IsCommonKeyword(trimmed) || trimmed == "auto")
                    continue;

                if (!_lengthPattern.IsMatch(trimmed) && trimmed != "0")
                    return new ValidationResult(false, $"Invalid margin value: {trimmed}. Expected length or 'auto'");
            }

            return new ValidationResult(true);
        }

        private ValidationResult ValidateSpacingValue(string value)
        {
            if (IsCommonKeyword(value) || value == "auto")
                return new ValidationResult(true);

            if (_lengthPattern.IsMatch(value) || value == "0")
                return new ValidationResult(true);

            return new ValidationResult(false, "Invalid margin value. Expected length unit or 'auto'");
        }

        private ValidationResult ValidatePaddingValue(string value)
        {
            // Padding does not accept negative values or 'auto'
            if (_lengthPattern.IsMatch(value))
            {
                var match = _lengthPattern.Match(value);
                var numValue = double.Parse(match.Groups[1].Value);
                if (numValue < 0)
                    return new ValidationResult(false, "Padding cannot be negative");
                return new ValidationResult(true);
            }

            if (value == "0")
                return new ValidationResult(true);

            return new ValidationResult(false, "Invalid padding value. Expected non-negative length unit");
        }

        private ValidationResult ValidateBorder(string value)
        {
            // Border is: width style color
            var parts = value.Split(' ');
            if (parts.Length < 1 || parts.Length > 3)
                return new ValidationResult(false, "Border expects 1-3 values (width, style, color)");

            // Validate each potential part
            bool hasWidth = false, hasStyle = false, hasColor = false;

            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                // Check if it's a width
                if (!hasWidth && (_lengthPattern.IsMatch(trimmed) || trimmed == "0" || _borderWidthKeywords.Contains(trimmed)))
                {
                    hasWidth = true;
                    continue;
                }

                // Check if it's a style
                if (!hasStyle && _borderStyleKeywords.Contains(trimmed))
                {
                    hasStyle = true;
                    continue;
                }

                // Check if it's a color
                if (!hasColor && ValidateColor(trimmed).IsValid)
                {
                    hasColor = true;
                    continue;
                }

                return new ValidationResult(false, $"Invalid border component: {trimmed}");
            }

            return new ValidationResult(true);
        }

        private static readonly HashSet<string> _borderWidthKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "thin", "medium", "thick"
        };

        private ValidationResult ValidateBorderWidth(string value)
        {
            if (_borderWidthKeywords.Contains(value) || value == "0")
                return new ValidationResult(true);

            if (_lengthPattern.IsMatch(value))
                return new ValidationResult(true);

            return new ValidationResult(false, "Invalid border width. Expected length or 'thin', 'medium', 'thick'");
        }

        private ValidationResult ValidateBorderStyle(string value)
        {
            var parts = value.Split(' ');
            foreach (var part in parts)
            {
                if (!string.IsNullOrWhiteSpace(part) && !_borderStyleKeywords.Contains(part))
                    return new ValidationResult(false, $"Invalid border style: {part}");
            }
            return new ValidationResult(true);
        }

        private ValidationResult ValidateFontSize(string value)
        {
            if (IsCommonKeyword(value))
                return new ValidationResult(true);

            var sizeKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "xx-small", "x-small", "small", "medium", "large", "x-large", "xx-large",
                "smaller", "larger"
            };

            if (sizeKeywords.Contains(value))
                return new ValidationResult(true);

            if (_lengthPattern.IsMatch(value))
                return new ValidationResult(true);

            return new ValidationResult(false, "Invalid font size. Expected length unit, percentage, or size keyword");
        }

        private ValidationResult ValidateFontWeight(string value)
        {
            if (_fontWeightKeywords.Contains(value))
                return new ValidationResult(true);

            if (_numberPattern.IsMatch(value))
            {
                var num = int.Parse(value);
                if (num >= 100 && num <= 900 && num % 100 == 0)
                    return new ValidationResult(true);
                return new ValidationResult(false, "Font weight must be 100-900 in increments of 100");
            }

            return new ValidationResult(false, "Invalid font weight. Expected keyword or 100-900");
        }

        private ValidationResult ValidateFontStyle(string value)
        {
            if (_fontStyleKeywords.Contains(value))
                return new ValidationResult(true);

            return new ValidationResult(false, "Invalid font style. Expected 'normal', 'italic', or 'oblique'");
        }

        private ValidationResult ValidateFontFamily(string value)
        {
            // Font family is generally very permissive
            if (string.IsNullOrWhiteSpace(value))
                return new ValidationResult(false, "Font family cannot be empty");

            return new ValidationResult(true);
        }

        private ValidationResult ValidateLineHeight(string value)
        {
            if (IsCommonKeyword(value))
                return new ValidationResult(true);

            if (_numberPattern.IsMatch(value))
                return new ValidationResult(true);

            if (_lengthPattern.IsMatch(value))
                return new ValidationResult(true);

            return new ValidationResult(false, "Invalid line height. Expected number, length, or keyword");
        }

        private ValidationResult ValidateDisplay(string value)
        {
            if (_displayKeywords.Contains(value))
                return new ValidationResult(true);

            return new ValidationResult(false, "Invalid display value. Expected 'block', 'inline', 'flex', 'grid', etc.");
        }

        private ValidationResult ValidatePosition(string value)
        {
            if (_positionKeywords.Contains(value))
                return new ValidationResult(true);

            return new ValidationResult(false, "Invalid position value. Expected 'static', 'relative', 'absolute', 'fixed', or 'sticky'");
        }

        private ValidationResult ValidateTextAlign(string value)
        {
            if (_textAlignKeywords.Contains(value))
                return new ValidationResult(true);

            return new ValidationResult(false, "Invalid text alignment. Expected 'left', 'right', 'center', 'justify', etc.");
        }

        private ValidationResult ValidateFloat(string value)
        {
            var floatKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "left", "right", "none" };
            if (floatKeywords.Contains(value))
                return new ValidationResult(true);

            return new ValidationResult(false, "Invalid float value. Expected 'left', 'right', or 'none'");
        }

        private ValidationResult ValidateClear(string value)
        {
            var clearKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "left", "right", "both", "none" };
            if (clearKeywords.Contains(value))
                return new ValidationResult(true);

            return new ValidationResult(false, "Invalid clear value. Expected 'left', 'right', 'both', or 'none'");
        }

        private ValidationResult ValidateOpacity(string value)
        {
            if (_numberPattern.IsMatch(value))
            {
                var opacity = double.Parse(value);
                if (opacity >= 0 && opacity <= 1)
                    return new ValidationResult(true);
                return new ValidationResult(false, "Opacity must be between 0 and 1");
            }

            return new ValidationResult(false, "Invalid opacity. Expected decimal value between 0 and 1");
        }

        private ValidationResult ValidateVisibility(string value)
        {
            var visibilityKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "visible", "hidden", "collapse" };
            if (visibilityKeywords.Contains(value))
                return new ValidationResult(true);

            return new ValidationResult(false, "Invalid visibility value. Expected 'visible', 'hidden', or 'collapse'");
        }

        private bool IsCommonKeyword(string value)
        {
            return _commonKeywords.Contains(value);
        }

        #endregion

        #endregion
    }
}
