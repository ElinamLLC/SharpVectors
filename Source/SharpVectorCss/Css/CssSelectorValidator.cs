using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// Validates and analyzes CSS selectors to provide diagnostics feedback
    /// and detect modern selectors that may not be fully supported.
    /// </summary>
    public sealed class CssSelectorValidator
    {
        #region Static Fields

        // Pattern for modern CSS selectors not yet supported
        private static readonly Regex _modernSelectorPattern = new Regex(
            @":(is|where|has|has-slotted)\s*\(",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Pattern for pseudo-classes
        private static readonly Regex _pseudoClassPattern = new Regex(
            @":(link|visited|active|hover|focus|target|enabled|disabled|checked|" +
            @"default|valid|invalid|in-range|out-of-range|required|optional|" +
            @"read-only|read-write|placeholder-shown|blank|user-valid|" +
            @"first-child|last-child|only-child|nth-child|nth-last-child|" +
            @"first-of-type|last-of-type|only-of-type|nth-of-type|nth-last-of-type|" +
            @"empty|root|scope|current|past|future|playing|paused|" +
            @"seeking|stalled|buffering|stalled|muted|volume-locked|" +
            @"fullscreen|modal|local-link|target-within|focus-visible|focus-within|" +
            @"autofill|not|is|where|has|dir|lang)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Pattern for pseudo-elements
        private static readonly Regex _pseudoElementPattern = new Regex(
            @"::(before|after|first-line|first-letter|selection|backdrop|" +
            @"placeholder|marker|cue|cue-region|grammar-error|spelling-error|" +
            @"slotted|part|view-transition|view-transition-image-pair|" +
            @"view-transition-old|view-transition-new)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Pattern for combinators
        private static readonly Regex _combinatorPattern = new Regex(
            @"\s*([>+~]|\s+)\s*",
            RegexOptions.Compiled);

        // Pattern for attribute selectors
        private static readonly Regex _attributePattern = new Regex(
            @"\[\s*[^\]]+\s*\]",
            RegexOptions.Compiled);

        // Pattern for class selectors
        private static readonly Regex _classPattern = new Regex(
            @"\.[A-Za-z_\-][A-Za-z0-9_\-]*",
            RegexOptions.Compiled);

        // Pattern for ID selectors
        private static readonly Regex _idPattern = new Regex(
            @"#[A-Za-z_\-][A-Za-z0-9_\-]*",
            RegexOptions.Compiled);

        // Pattern for type selectors
        private static readonly Regex _typePattern = new Regex(
            @"^([A-Za-z_\-]|[A-Za-z_\-][A-Za-z0-9_\-]*|\*)",
            RegexOptions.Compiled);

        #endregion

        #region Public Classes

        /// <summary>
        /// Represents the analysis results of a CSS selector.
        /// </summary>
        public sealed class SelectorAnalysis
        {
            public string Selector { get; set; }
            public bool IsValid { get; set; }
            public int Complexity { get; set; }
            public List<string> PseudoClasses { get; set; }
            public List<string> PseudoElements { get; set; }
            public List<string> ModernFeatures { get; set; }
            public List<string> Issues { get; set; }
            public SelectorSupportLevel SupportLevel { get; set; }

            public SelectorAnalysis()
            {
                PseudoClasses = new List<string>();
                PseudoElements = new List<string>();
                ModernFeatures = new List<string>();
                Issues = new List<string>();
                SupportLevel = SelectorSupportLevel.Supported;
            }
        }

        #endregion

        #region Public Enums

        /// <summary>
        /// Indicates the level of selector support.
        /// </summary>
        public enum SelectorSupportLevel
        {
            /// <summary>Selector is fully supported</summary>
            Supported,

            /// <summary>Selector uses advanced but supported features</summary>
            Advanced,

            /// <summary>Selector uses modern CSS features with limited support</summary>
            Modern,

            /// <summary>Selector uses unsupported features or has syntax errors</summary>
            Unsupported,

            /// <summary>Selector has parsing errors</summary>
            Error
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Analyzes a single CSS selector for features and issues.
        /// </summary>
        public static SelectorAnalysis Analyze(string selector)
        {
            if (string.IsNullOrWhiteSpace(selector))
            {
                return new SelectorAnalysis
                {
                    Selector = selector,
                    IsValid = false,
                    Issues = new List<string> { "Selector is empty" },
                    SupportLevel = SelectorSupportLevel.Error
                };
            }

            var analysis = new SelectorAnalysis
            {
                Selector = selector.Trim(),
                IsValid = true
            };

            try
            {
                // Extract modern selector features
                var modernMatches = _modernSelectorPattern.Matches(analysis.Selector);
                foreach (Match match in modernMatches)
                {
                    var featureName = match.Groups[1].Value.ToLowerInvariant();
                    analysis.ModernFeatures.Add(featureName);
                    analysis.Issues.Add($"Modern selector :{featureName}() has limited support");
                }

                // Extract pseudo-classes
                var pseudoClassMatches = _pseudoClassPattern.Matches(analysis.Selector);
                foreach (Match match in pseudoClassMatches)
                {
                    analysis.PseudoClasses.Add(match.Value.ToLowerInvariant());
                }

                // Extract pseudo-elements
                var pseudoElementMatches = _pseudoElementPattern.Matches(analysis.Selector);
                foreach (Match match in pseudoElementMatches)
                {
                    analysis.PseudoElements.Add(match.Value.ToLowerInvariant());
                }

                // Calculate complexity
                analysis.Complexity = CalculateComplexity(analysis.Selector);

                // Determine support level
                if (analysis.ModernFeatures.Count > 0)
                {
                    analysis.SupportLevel = SelectorSupportLevel.Modern;
                }
                else if (analysis.PseudoClasses.Count > 3 || analysis.Complexity > 20)
                {
                    analysis.SupportLevel = SelectorSupportLevel.Advanced;
                }
                else
                {
                    analysis.SupportLevel = SelectorSupportLevel.Supported;
                }

                // Validate syntax
                if (!ValidateSelectorSyntax(analysis.Selector))
                {
                    analysis.IsValid = false;
                    analysis.SupportLevel = SelectorSupportLevel.Error;
                    analysis.Issues.Add("Selector has invalid syntax");
                }
            }
            catch (Exception ex)
            {
                analysis.IsValid = false;
                analysis.SupportLevel = SelectorSupportLevel.Error;
                analysis.Issues.Add($"Error analyzing selector: {ex.Message}");
            }

            return analysis;
        }

        /// <summary>
        /// Analyzes multiple comma-separated selectors.
        /// </summary>
        public static List<SelectorAnalysis> AnalyzeSelectors(string selectorText)
        {
            var results = new List<SelectorAnalysis>();

            if (string.IsNullOrWhiteSpace(selectorText))
            {
                return results;
            }

            // Split by comma but be careful of commas inside :not() etc.
            var selectors = SplitSelectors(selectorText);

            foreach (var selector in selectors)
            {
                results.Add(Analyze(selector));
            }

            return results;
        }

        /// <summary>
        /// Gets diagnostic information for a selector to report via CssParsingContext.
        /// </summary>
        public static List<string> GetDiagnostics(string selector, CssWarningLevel minLevel = CssWarningLevel.Info)
        {
            var diagnostics = new List<string>();
            var analysis = Analyze(selector);

            if (!analysis.IsValid)
            {
                diagnostics.Add($"Invalid selector: {string.Join("; ", analysis.Issues)}");
                return diagnostics;
            }

            if (analysis.SupportLevel == SelectorSupportLevel.Modern)
            {
                foreach (var issue in analysis.Issues)
                {
                    diagnostics.Add(issue);
                }
            }

            if (analysis.Complexity > 15 && minLevel <= CssWarningLevel.Low)
            {
                diagnostics.Add($"Selector complexity is high ({analysis.Complexity}), may impact performance");
            }

            return diagnostics;
        }

        /// <summary>
        /// Gets a human-readable representation of a selector's features.
        /// </summary>
        public static string GetSelectorDescription(string selector)
        {
            var analysis = Analyze(selector);
            var parts = new List<string>();

            if (analysis.PseudoClasses.Count > 0)
                parts.Add($"pseudo-classes: {string.Join(", ", analysis.PseudoClasses)}");

            if (analysis.PseudoElements.Count > 0)
                parts.Add($"pseudo-elements: {string.Join(", ", analysis.PseudoElements)}");

            if (analysis.ModernFeatures.Count > 0)
                parts.Add($"modern features: {string.Join(", ", analysis.ModernFeatures)}");

            if (parts.Count == 0)
                return "Basic selector";

            return string.Join("; ", parts);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Splits comma-separated selectors, respecting nesting in pseudo-class functions.
        /// </summary>
        private static List<string> SplitSelectors(string selectorText)
        {
            var selectors = new List<string>();
            var current = new System.Text.StringBuilder();
            int parenDepth = 0;

            foreach (char ch in selectorText)
            {
                if (ch == '(')
                {
                    parenDepth++;
                    current.Append(ch);
                }
                else if (ch == ')')
                {
                    parenDepth--;
                    current.Append(ch);
                }
                else if (ch == ',' && parenDepth == 0)
                {
                    var selector = current.ToString().Trim();
                    if (selector.Length > 0)
                    {
                        selectors.Add(selector);
                    }
                    current.Clear();
                }
                else
                {
                    current.Append(ch);
                }
            }

            var lastSelector = current.ToString().Trim();
            if (lastSelector.Length > 0)
            {
                selectors.Add(lastSelector);
            }

            return selectors;
        }

        /// <summary>
        /// Calculates selector complexity based on various factors.
        /// Score: Simple = 1-5, Moderate = 6-15, Complex = 16+
        /// </summary>
        private static int CalculateComplexity(string selector)
        {
            int score = 1;

            // Count combinators
            var combinators = _combinatorPattern.Matches(selector);
            score += combinators.Count;

            // Count selectors (roughly)
            var typeMatches = Regex.Matches(selector, @"[A-Za-z_][A-Za-z0-9_-]*");
            score += Math.Max(0, typeMatches.Count - 1);

            // Count pseudo-classes and pseudo-elements
            var pseudoMatches = Regex.Matches(selector, @":(:[a-z-]+|[a-z-]+)");
            score += pseudoMatches.Count * 2;

            // Count attribute selectors
            var attrMatches = _attributePattern.Matches(selector);
            score += attrMatches.Count * 2;

            // Count class and ID selectors
            var classMatches = _classPattern.Matches(selector);
            score += classMatches.Count;

            var idMatches = _idPattern.Matches(selector);
            score += idMatches.Count * 2;

            return score;
        }

        /// <summary>
        /// Basic validation of selector syntax.
        /// </summary>
        private static bool ValidateSelectorSyntax(string selector)
        {
            if (string.IsNullOrWhiteSpace(selector))
                return false;

            // Check for obvious issues
            if (selector.IndexOf('{') >= 0 || selector.IndexOf('}') >= 0)
                return false;

            if (selector.IndexOf(';') >= 0)
                return false;

            // Check balanced brackets
            if (!HasBalancedBrackets(selector))
                return false;

            // Check balanced parentheses
            if (!HasBalancedParentheses(selector))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if square brackets are balanced.
        /// </summary>
        private static bool HasBalancedBrackets(string text)
        {
            int count = 0;
            foreach (char ch in text)
            {
                if (ch == '[') count++;
                else if (ch == ']') count--;

                if (count < 0) return false;
            }
            return count == 0;
        }

        /// <summary>
        /// Checks if parentheses are balanced.
        /// </summary>
        private static bool HasBalancedParentheses(string text)
        {
            int count = 0;
            foreach (char ch in text)
            {
                if (ch == '(') count++;
                else if (ch == ')') count--;

                if (count < 0) return false;
            }
            return count == 0;
        }

        #endregion
    }
}
