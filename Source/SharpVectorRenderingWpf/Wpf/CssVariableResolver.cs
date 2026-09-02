using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SharpVectors.Renderers.Wpf
{
    /// <summary>
    /// Resolves CSS custom property (variable) references with support for fallbacks, nested variables, and fallback chains.
    /// </summary>
    /// <remarks>
    /// <para><b>Specification:</b> CSS Cascading Variables Module Level 1 (css-variables-1)</para>
    /// <para><b>Purpose:</b></para>
    /// This resolver robustly handles CSS var() function calls that may be unresolved at render time.
    /// It supports fallback extraction, nested variables, fallback chains, and cycle detection.
    /// <para><b>Supported Patterns:</b></para>
    /// <list type="bullet">
    ///   <item><description>Simple: "var(--my-color)" → empty (unresolved)</description></item>
    ///   <item><description>With fallback: "var(--my-color, #FF0000)" → "#FF0000"</description></item>
    ///   <item><description>Nested fallback: "var(--missing, var(--also-missing, #000))" → "#000" (recursive resolution)</description></item>
    ///   <item><description>Multiple vars: "var(--a) var(--b, #111)" → processed independently</description></item>
    ///   <item><description>Whitespace handling: "var( --name , fallback )" → normalized</description></item>
    /// </list>
    /// <para><b>Edge Cases Handled:</b></para>
    /// <list type="bullet">
    ///   <item><description>Circular references: Tracked via depth limit and visited set to prevent infinite recursion</description></item>
    ///   <item><description>Unresolved chains: Falls back to empty string if no terminal value found</description></item>
    ///   <item><description>Malformed syntax: Treated as non-variable, returned unchanged</description></item>
    /// </list>
    /// </remarks>
    public static class CssVariableResolver
    {
        /// <summary>
        /// Maximum recursion depth for variable fallback chains (e.g., var(--a, var(--b, var(--c, ...)))).
        /// Prevents stack overflow from circular references or pathological nesting.
        /// </summary>
        private const int MaxRecursionDepth = 10;

        /// <summary>
        /// Resolves CSS variable references in a property value with support for fallbacks and nested variables.
        /// </summary>
        /// <remarks>
        /// This is the main entry point for CSS variable resolution. It handles:
        /// - Simple unresolved variables: "var(--missing)" → "" (empty, triggers default)
        /// - Variables with fallback: "var(--missing, #FF0000)" → "#FF0000"
        /// - Nested/chained fallbacks: "var(--a, var(--b, #000))" → "#000"
        /// - Multiple var() in one value: Each is processed independently
        /// 
        /// Non-variable strings (e.g., "red", "#000", "300") are returned unchanged.
        /// </remarks>
        /// <param name="value">A CSS property value that may contain var() function syntax.</param>
        /// <returns>A resolved value, fallback value, or empty string if unresolved.</returns>
        public static string ResolveVariables(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return ResolveVariablesInternal(value, 0, new HashSet<string>());
        }

        /// <summary>
        /// Internal recursive implementation of variable resolution with cycle detection.
        /// </summary>
        /// <param name="value">The value to resolve.</param>
        /// <param name="depth">Current recursion depth (for cycle detection).</param>
        /// <param name="visitedVars">Set of variable names already visited in this chain (prevents circular references).</param>
        /// <returns>Resolved value or empty string if unresolved.</returns>
        private static string ResolveVariablesInternal(string value, int depth, HashSet<string> visitedVars)
        {
            // Depth limit check: prevent stack overflow from malformed circular references
            if (depth >= MaxRecursionDepth)
            {
                Debug.WriteLine($"[CSS Variable Resolver] Max recursion depth ({MaxRecursionDepth}) reached for: '{value}'");
                return string.Empty;
            }

            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            // Trim leading/trailing whitespace
            value = value.Trim();

            // If the value doesn't contain var(, it's a literal value
            if (value.IndexOf("var(", StringComparison.OrdinalIgnoreCase) < 0)
            {
                return value;
            }

            // Try to extract a single var() call and its fallback
            if (TryExtractVariable(value, out string varName, out string fallback, out string prefix, out string suffix))
            {
                // We have a var(--name) or var(--name, fallback)
                // The variable name itself is unresolved (variables are resolved by CSS engine at parse time)
                // so we focus on extracting and resolving the fallback

                if (!string.IsNullOrEmpty(fallback))
                {
                    // We have a fallback to use
                    fallback = fallback.Trim();

                    // Check if the fallback is itself a var() call
                    if (fallback.StartsWith("var(", StringComparison.OrdinalIgnoreCase))
                    {
                        // Recursive case: fallback is another variable
                        // Check for circular reference
                        string trimmedVarName = varName.Trim();
                        if (visitedVars.Contains(trimmedVarName))
                        {
                            Debug.WriteLine($"[CSS Variable Resolver] Circular reference detected: {trimmedVarName}");
                            return string.Empty;
                        }

                        var newVisited = new HashSet<string>(visitedVars) { trimmedVarName };
                        string resolvedFallback = ResolveVariablesInternal(fallback, depth + 1, newVisited);

                        // If the recursive resolution succeeded
                        if (!string.IsNullOrEmpty(resolvedFallback))
                        {
                            Debug.WriteLine($"[CSS Variable Resolver] Resolved via fallback chain: var({varName}, ...) → {resolvedFallback}");
                            // Reconstruct the property value with the resolved fallback
                            return prefix + resolvedFallback + suffix;
                        }
                        // If recursive resolution failed, fall through to return empty
                        Debug.WriteLine($"[CSS Variable Resolver] Unresolved variable with unresolved fallback: var({varName}, {fallback})");
                        return string.Empty;
                    }
                    else
                    {
                        // Fallback is a literal value (not a var() call)
                        Debug.WriteLine($"[CSS Variable Resolver] Using literal fallback: var({varName}, {fallback}) → {fallback}");
                        // Recursively resolve in case there are more var() calls after this VAR's fallback
                        string afterFallback = prefix + fallback + suffix;
                        if (afterFallback.IndexOf("var(", StringComparison.OrdinalIgnoreCase) >= 0 && afterFallback != value)
                        {
                            return ResolveVariablesInternal(afterFallback, depth + 1, visitedVars);
                        }
                        return afterFallback;
                    }
                }
                else
                {
                    // No fallback: the variable is unresolved
                    Debug.WriteLine($"[CSS Variable Resolver] Unresolved variable without fallback: var({varName})");
                    return string.Empty;
                }
            }
            else
            {
                // Couldn't extract a valid var() pattern; return as-is
                Debug.WriteLine($"[CSS Variable Resolver] Could not parse var() syntax in: '{value}'");
                return value;
            }
        }

        /// <summary>
        /// Attempts to extract a single var() call from a value string.
        /// </summary>
        /// <param name="value">The CSS property value containing a var() call.</param>
        /// <param name="varName">Output: The variable name (e.g., "--my-color").</param>
        /// <param name="fallback">Output: The fallback value if present; null otherwise.</param>
        /// <param name="prefix">Output: Any text before the var() call.</param>
        /// <param name="suffix">Output: Any text after the var() call.</param>
        /// <returns>True if a var() pattern was found and extracted; false otherwise.</returns>
        private static bool TryExtractVariable(string value, out string varName, out string fallback, 
            out string prefix, out string suffix)
        {
            varName = null;
            fallback = null;
            prefix = string.Empty;
            suffix = string.Empty;

            // Find the start of var(
            int varStart = value.IndexOf("var(", StringComparison.OrdinalIgnoreCase);
            if (varStart < 0)
            {
                return false;
            }

            prefix = value.Substring(0, varStart);

            // Find the matching closing paren
            int openParenPos = varStart + 3; // Position of (
            int closeParenPos = FindMatchingCloseParen(value, openParenPos);

            if (closeParenPos < 0)
            {
                // Malformed var() - no closing paren
                return false;
            }

            // Extract the content inside var(...) = e.g., "--my-color, fallback"
            string content = value.Substring(openParenPos + 1, closeParenPos - openParenPos - 1);
            suffix = value.Substring(closeParenPos + 1);

            // Split on first comma to separate variable name from fallback
            int commaPos = FindFirstTopLevelComma(content);
            if (commaPos >= 0)
            {
                // Has fallback: var(--name, fallback)
                varName = content.Substring(0, commaPos).Trim();
                fallback = content.Substring(commaPos + 1).Trim();
            }
            else
            {
                // No fallback: var(--name)
                varName = content.Trim();
                fallback = null;
            }

            return true;
        }

        /// <summary>
        /// Finds the position of the first top-level comma (not inside nested parens).
        /// </summary>
        /// <param name="value">The string to search.</param>
        /// <returns>Index of comma, or -1 if not found.</returns>
        private static int FindFirstTopLevelComma(string value)
        {
            int depth = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '(')
                {
                    depth++;
                }
                else if (value[i] == ')')
                {
                    depth--;
                }
                else if (value[i] == ',' && depth == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Finds the position of the closing parenthesis that matches an opening parenthesis.
        /// </summary>
        /// <param name="value">The string containing the opening paren at position startPos.</param>
        /// <param name="startPos">Position of the opening paren.</param>
        /// <returns>Index of matching close paren, or -1 if not found.</returns>
        private static int FindMatchingCloseParen(string value, int startPos)
        {
            if (startPos < 0 || startPos >= value.Length || value[startPos] != '(')
            {
                return -1;
            }

            int depth = 1;
            for (int i = startPos + 1; i < value.Length; i++)
            {
                if (value[i] == '(')
                {
                    depth++;
                }
                else if (value[i] == ')')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
    }
}
