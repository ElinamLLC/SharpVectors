using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// Manages CSS custom property definitions (CSS variables like --my-color: red;)
    /// and provides variable resolution with scope support.
    /// </summary>
    public class CssVariableRegistry
    {
        #region Private Fields

        private Dictionary<string, string> _globalVariables;
        private Dictionary<string, Dictionary<string, string>> _scopedVariables;
        private Stack<Dictionary<string, string>> _scopeStack;
        private CssParsingContext _context;

        // Performance caching
        private Dictionary<string, string> _resolutionCache;           // Variable lookup cache
        private Dictionary<string, bool> _undefinedCache;              // Cache for undefined variables
        private bool _cacheValid;                                      // Flag to track cache validity

        #endregion

        #region Constructor

        public CssVariableRegistry(CssParsingContext context = null)
        {
            _globalVariables = new Dictionary<string, string>(StringComparer.Ordinal);  // CSS vars are case-sensitive!
            _scopedVariables = new Dictionary<string, Dictionary<string, string>>();
            _scopeStack = new Stack<Dictionary<string, string>>();
            _context = context;

            // Initialize caches
            _resolutionCache = new Dictionary<string, string>(StringComparer.Ordinal);
            _undefinedCache = new Dictionary<string, bool>(StringComparer.Ordinal);
            _cacheValid = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Defines a custom property (CSS variable) in the global scope
        /// </summary>
        /// <param name="variableName">The variable name (e.g., "--my-color")</param>
        /// <param name="value">The variable value</param>
        public void DefineGlobalVariable(string variableName, string value)
        {
            if (!IsValidVariableName(variableName))
            {
                LogWarning($"Invalid variable name: '{variableName}'. Custom properties must start with '--'");
                return;
            }

            _globalVariables[variableName] = value;

            // Invalidate caches when variable is redefined
            InvalidateCache();
        }

        /// <summary>
        /// Defines a custom property in the current scope
        /// </summary>
        /// <param name="scopeName">The scope identifier (e.g., element selector)</param>
        /// <param name="variableName">The variable name</param>
        /// <param name="value">The variable value</param>
        public void DefineScopedVariable(string scopeName, string variableName, string value)
        {
            if (!IsValidVariableName(variableName))
            {
                LogWarning($"Invalid variable name: '{variableName}'");
                return;
            }

            if (!_scopedVariables.ContainsKey(scopeName))
            {
                _scopedVariables[scopeName] = new Dictionary<string, string>(StringComparer.Ordinal);
            }

            _scopedVariables[scopeName][variableName] = value;

            // Invalidate caches when variable is redefined
            InvalidateCache();
        }

        /// <summary>
        /// Resolves a variable reference with optional fallback value
        /// </summary>
        /// <param name="variableName">The variable name</param>
        /// <param name="fallbackValue">Optional fallback value if variable is not defined</param>
        /// <param name="scopeName">Optional scope name for scoped lookup</param>
        /// <returns>Resolved value or fallback, or null if not found</returns>
        public string ResolveVariable(string variableName, string fallbackValue = null, string scopeName = null)
        {
            if (!IsValidVariableName(variableName))
            {
                LogWarning($"Invalid variable reference: '{variableName}'");
                return fallbackValue;
            }

            // Check cache first (only for global scope, scoped lookups depend on scopeName)
            if (string.IsNullOrEmpty(scopeName) && _cacheValid)
            {
                if (_resolutionCache.TryGetValue(variableName, out var cachedValue))
                {
                    return cachedValue;
                }

                if (_undefinedCache.TryGetValue(variableName, out var isUndefined) && isUndefined)
                {
                    return fallbackValue;
                }
            }

            string value = null;

            // Try scope-specific lookup first if scope provided
            if (!string.IsNullOrEmpty(scopeName) && _scopedVariables.ContainsKey(scopeName))
            {
                if (_scopedVariables[scopeName].TryGetValue(variableName, out value))
                {
                    return value;
                }
            }

            // Try global variables
            if (_globalVariables.TryGetValue(variableName, out value))
            {
                // Cache the result
                if (string.IsNullOrEmpty(scopeName) && _cacheValid)
                {
                    _resolutionCache[variableName] = value;
                }
                return value;
            }

            // Variable not found, log warning if context available
            LogWarning($"Undefined variable: '{variableName}'");

            // Cache the undefined result
            if (string.IsNullOrEmpty(scopeName) && _cacheValid)
            {
                _undefinedCache[variableName] = true;
            }

            // Return fallback value if provided
            return fallbackValue;
        }

        /// <summary>
        /// Checks if a variable is defined
        /// </summary>
        public bool HasVariable(string variableName, string scopeName = null)
        {
            if (!IsValidVariableName(variableName))
                return false;

            if (!string.IsNullOrEmpty(scopeName) && _scopedVariables.ContainsKey(scopeName))
            {
                if (_scopedVariables[scopeName].ContainsKey(variableName))
                    return true;
            }

            return _globalVariables.ContainsKey(variableName);
        }

        /// <summary>
        /// Enters a new scope (e.g., when processing a CSS rule)
        /// </summary>
        public void PushScope(string scopeName)
        {
            var scopeVars = new Dictionary<string, string>(StringComparer.Ordinal);
            if (_scopedVariables.ContainsKey(scopeName))
            {
                foreach (var kvp in _scopedVariables[scopeName])
                {
                    scopeVars[kvp.Key] = kvp.Value;
                }
            }
            _scopeStack.Push(scopeVars);
        }

        /// <summary>
        /// Exits the current scope
        /// </summary>
        public void PopScope()
        {
            if (_scopeStack.Count > 0)
            {
                _scopeStack.Pop();
            }
        }

        /// <summary>
        /// Gets all defined variables in global scope
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> GetGlobalVariables()
        {
            return _globalVariables.AsEnumerable();
        }

        /// <summary>
        /// Gets all defined variables in a specific scope
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> GetScopedVariables(string scopeName)
        {
            if (_scopedVariables.ContainsKey(scopeName))
            {
                return _scopedVariables[scopeName].AsEnumerable();
            }
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Clears all variables
        /// </summary>
        public void Clear()
        {
            _globalVariables.Clear();
            _scopedVariables.Clear();
            _scopeStack.Clear();

            // Clear caches as well
            InvalidateCache();
        }

        /// <summary>
        /// Gets count of defined variables
        /// </summary>
        public int GlobalVariableCount => _globalVariables.Count;

        public int TotalVariableCount => _globalVariables.Count + 
            _scopedVariables.Values.Sum(d => d.Count);

        #endregion

        #region Private Methods

        /// <summary>
        /// Invalidates all resolution caches
        /// </summary>
        private void InvalidateCache()
        {
            _resolutionCache.Clear();
            _undefinedCache.Clear();
            _cacheValid = true;
        }

        /// <summary>
        /// Validates CSS variable name (must start with --)
        /// </summary>
        private bool IsValidVariableName(string name)
        {
            return !string.IsNullOrEmpty(name) && name.StartsWith("--");
        }

        /// <summary>
        /// Logs a warning to the parsing context if available
        /// </summary>
        private void LogWarning(string message)
        {
            if (_context != null)
            {
                _context.AddWarning(message, CssWarningLevel.Medium);
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a CSS variable reference (var() expression)
    /// </summary>
    public class CssVariableReference
    {
        #region Properties

        /// <summary>
        /// The variable name (e.g., "--my-color")
        /// </summary>
        public string VariableName { get; set; }

        /// <summary>
        /// Optional fallback value for the variable
        /// </summary>
        public string FallbackValue { get; set; }

        /// <summary>
        /// Optional scope name for scoped variable lookup
        /// </summary>
        public string ScopeName { get; set; }

        /// <summary>
        /// The original var() expression
        /// </summary>
        public string OriginalExpression { get; set; }

        #endregion

        #region Constructor

        public CssVariableReference(string variableName, string fallbackValue = null, string scopeName = null)
        {
            VariableName = variableName;
            FallbackValue = fallbackValue;
            ScopeName = scopeName;
            OriginalExpression = BuildExpression();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses a CSS var() expression
        /// </summary>
        /// <param name="expression">The var() expression (e.g., "var(--color, red)")</param>
        /// <returns>CssVariableReference or null if invalid</returns>
        public static CssVariableReference Parse(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return null;

            var trimmed = expression.Trim();

            // Check for var( prefix and ) suffix
            if (!trimmed.StartsWith("var(", StringComparison.OrdinalIgnoreCase) || !trimmed.EndsWith(")"))
            {
                return null;
            }

            // Extract the content inside var(...)
            var content = trimmed.Substring(4, trimmed.Length - 5).Trim();

            // Split by first comma at the top level (not inside nested var())
            var parts = SplitVariableExpression(content);

            if (parts.Count < 1)
                return null;

            var variableName = parts[0].Trim();

            // Validate variable name
            if (string.IsNullOrEmpty(variableName) || !variableName.StartsWith("--"))
            {
                return null;
            }

            string fallback = null;
            if (parts.Count > 1)
            {
                fallback = parts[1].Trim();
            }

            return new CssVariableReference(variableName, fallback);
        }

        /// <summary>
        /// Gets the textual representation of this variable reference
        /// </summary>
        public override string ToString()
        {
            return OriginalExpression;
        }

        #endregion

        #region Private Methods

        private static List<string> SplitVariableExpression(string content)
        {
            var parts = new List<string>();
            var current = new System.Text.StringBuilder();
            int depth = 0;

            foreach (char c in content)
            {
                if (c == '(' && (current.Length == 0 || current.ToString().TrimEnd().EndsWith("var", StringComparison.OrdinalIgnoreCase)))
                {
                    depth++;
                    current.Append(c);
                }
                else if (c == ')' && depth > 0)
                {
                    depth--;
                    current.Append(c);
                }
                else if (c == ',' && depth == 0)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            if (current.Length > 0)
            {
                parts.Add(current.ToString());
            }

            return parts;
        }

        private string BuildExpression()
        {
            if (string.IsNullOrEmpty(FallbackValue))
            {
                return $"var({VariableName})";
            }
            else
            {
                return $"var({VariableName}, {FallbackValue})";
            }
        }

        #endregion
    }
}
