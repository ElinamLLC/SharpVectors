using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// Resolves CSS variable references with support for fallback values,
    /// nested variables, and circular dependency detection.
    /// </summary>
    public class CssVariableResolver
    {
        #region Private Fields

        private CssVariableRegistry _registry;
        private CssParsingContext _context;
        private HashSet<string> _resolutionChain;
        private static readonly Regex _varPattern = new Regex(@"var\s*\(\s*([^,)]+)\s*(?:,\s*([^)]+))?\s*\)", RegexOptions.IgnoreCase);
        private const int MaxResolutionDepth = 100;

        // Performance caching
        private Dictionary<string, List<CssVariableReference>> _referenceCache;      // Cache var() references per value
        private Dictionary<string, bool> _circularCheckCache;                        // Memoize circular dependency checks

        #endregion

        #region Constructor

        public CssVariableResolver(CssVariableRegistry registry, CssParsingContext context = null)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _context = context;
            _resolutionChain = new HashSet<string>(StringComparer.Ordinal);

            // Initialize caches
            _referenceCache = new Dictionary<string, List<CssVariableReference>>(StringComparer.Ordinal);
            _circularCheckCache = new Dictionary<string, bool>(StringComparer.Ordinal);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resolves all var() references in a CSS property value
        /// </summary>
        /// <param name="value">The CSS property value that may contain var() references</param>
        /// <param name="scopeName">Optional scope name for variable lookup</param>
        /// <returns>The resolved value with all var() replaced, or original value if no var()</returns>
        public string ResolveValue(string value, string scopeName = null)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains("var("))
            {
                return value;
            }

            _resolutionChain.Clear();
            return ResolveValueInternal(value, scopeName, 0);
        }

        /// <summary>
        /// Checks if a value contains any CSS variable references
        /// </summary>
        public bool ContainsVariableReference(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Contains("var(");
        }

        /// <summary>
        /// Detects if there are circular variable dependencies
        /// </summary>
        /// <param name="variableName">The variable to check</param>
        /// <returns>True if circular dependency detected</returns>
        public bool HasCircularDependency(string variableName, string scopeName = null)
        {
            _resolutionChain.Clear();
            return CheckCircularDependency(variableName, scopeName, new HashSet<string>(StringComparer.Ordinal));
        }

        /// <summary>
        /// Gets all variable references in a value (without resolving them)
        /// </summary>
        public IEnumerable<CssVariableReference> GetVariableReferences(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                yield break;
            }

            // Check cache first
            if (_referenceCache.TryGetValue(value, out var cachedReferences))
            {
                foreach (var varRef in cachedReferences)
                {
                    yield return varRef;
                }
                yield break;
            }

            // Extract and cache references
            var references = new List<CssVariableReference>();
            var matches = _varPattern.Matches(value);
            foreach (Match match in matches)
            {
                var varName = match.Groups[1].Value.Trim();
                var fallback = match.Groups.Count > 2 ? match.Groups[2].Value.Trim() : null;

                var varRef = new CssVariableReference(varName, fallback);
                references.Add(varRef);
                yield return varRef;
            }

            // Cache the results
            _referenceCache[value] = references;
        }

        #endregion

        #region Private Methods

        private string ResolveValueInternal(string value, string scopeName, int depth)
        {
            if (depth > MaxResolutionDepth)
            {
                LogWarning($"Maximum variable resolution depth exceeded. Possible circular dependency: {value}");
                return value;
            }

            if (string.IsNullOrWhiteSpace(value) || !value.Contains("var("))
            {
                return value;
            }

            return _varPattern.Replace(value, match =>
            {
                var varName = match.Groups[1].Value.Trim();
                var fallback = match.Groups.Count > 2 ? match.Groups[2].Value.Trim() : null;

                // Check for circular dependency
                if (_resolutionChain.Contains(varName))
                {
                    LogWarning($"Circular variable dependency detected: {varName} -> {string.Join(" -> ", _resolutionChain)} -> {varName}");
                    return fallback ?? match.Value; // Use fallback or keep original
                }

                // Track this variable in the resolution chain
                _resolutionChain.Add(varName);

                try
                {
                    // Try to resolve the variable
                    var resolvedValue = _registry.ResolveVariable(varName, null, scopeName);

                    if (resolvedValue != null)
                    {
                        // Recursively resolve any var() in the resolved value
                        var furtherResolved = ResolveValueInternal(resolvedValue, scopeName, depth + 1);
                        return furtherResolved;
                    }
                    else
                    {
                        // Variable not found
                        if (fallback != null)
                        {
                            // Recursively resolve fallback as it might contain var() too
                            var resolvedFallback = ResolveValueInternal(fallback, scopeName, depth + 1);
                            return resolvedFallback;
                        }
                        else
                        {
                            // No fallback, return the original var() expression
                            return match.Value;
                        }
                    }
                }
                finally
                {
                    // Remove from resolution chain when done
                    _resolutionChain.Remove(varName);
                }
            });
        }

        private bool CheckCircularDependency(string variableName, string scopeName, HashSet<string> visited)
        {
            // Check memoization cache first
            if (_circularCheckCache.TryGetValue(variableName, out var cachedResult))
            {
                return cachedResult;
            }

            if (visited.Contains(variableName))
            {
                return true; // Circular dependency detected
            }

            visited.Add(variableName);

            // Get the value of this variable
            var value = _registry.ResolveVariable(variableName, null, scopeName);

            if (string.IsNullOrWhiteSpace(value) || !value.Contains("var("))
            {
                // Memoize: no circular dependency found for this variable
                _circularCheckCache[variableName] = false;
                return false; // No further dependencies
            }

            // Check all var() references in this value
            var references = GetVariableReferences(value);
            foreach (var varRef in references)
            {
                if (CheckCircularDependency(varRef.VariableName, scopeName, new HashSet<string>(visited, StringComparer.Ordinal)))
                {
                    // Memoize: circular dependency found
                    _circularCheckCache[variableName] = true;
                    return true;
                }
            }

            // Memoize: no circular dependency found
            _circularCheckCache[variableName] = false;
            return false;
        }

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
    /// Helper class for analyzing and validating CSS variables in stylesheets
    /// </summary>
    public class CssVariableAnalyzer
    {
        #region Properties

        public CssVariableRegistry Registry { get; private set; }
        public CssVariableResolver Resolver { get; private set; }
        public List<VariableUsage> UsageReport { get; private set; }

        #endregion

        #region Constructor

        public CssVariableAnalyzer(CssParsingContext context = null)
        {
            Registry = new CssVariableRegistry(context);
            Resolver = new CssVariableResolver(Registry, context);
            UsageReport = new List<VariableUsage>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Analyzes a property value for variable usage
        /// </summary>
        public void AnalyzePropertyValue(string propertyName, string propertyValue, string scopeName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyValue) || !propertyValue.Contains("var("))
            {
                return;
            }

            var references = Resolver.GetVariableReferences(propertyValue);
            foreach (var varRef in references)
            {
                var usage = new VariableUsage
                {
                    PropertyName = propertyName,
                    VariableName = varRef.VariableName,
                    Expression = varRef.ToString(),
                    IsDefined = Registry.HasVariable(varRef.VariableName, scopeName),
                    HasFallback = !string.IsNullOrEmpty(varRef.FallbackValue),
                    HasCircularDependency = Resolver.HasCircularDependency(varRef.VariableName, scopeName)
                };

                UsageReport.Add(usage);
            }
        }

        /// <summary>
        /// Gets a summary of variable usage
        /// </summary>
        public string GetAnalysisSummary()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("=== CSS Variable Analysis Summary ===");
            sb.AppendLine($"Total Variables Defined: {Registry.GlobalVariableCount}");
            sb.AppendLine($"Total Variable Usages: {UsageReport.Count}");

            var undefined = UsageReport.Where(u => !u.IsDefined && !u.HasFallback).ToList();
            if (undefined.Count > 0)
            {
                sb.AppendLine($"Undefined Variables (no fallback): {undefined.Count}");
                foreach (var usage in undefined)
                {
                    sb.AppendLine($"  - {usage.VariableName} used in {usage.PropertyName}");
                }
            }

            var circular = UsageReport.Where(u => u.HasCircularDependency).ToList();
            if (circular.Count > 0)
            {
                sb.AppendLine($"Circular Dependencies Detected: {circular.Count}");
                foreach (var usage in circular)
                {
                    sb.AppendLine($"  - {usage.VariableName} used in {usage.PropertyName}");
                }
            }

            return sb.ToString();
        }

        public void Clear()
        {
            Registry.Clear();
            UsageReport.Clear();
        }

        #endregion
    }

    /// <summary>
    /// Represents a usage of a CSS variable in a property
    /// </summary>
    public class VariableUsage
    {
        public string PropertyName { get; set; }
        public string VariableName { get; set; }
        public string Expression { get; set; }
        public bool IsDefined { get; set; }
        public bool HasFallback { get; set; }
        public bool HasCircularDependency { get; set; }

        public string Status
        {
            get
            {
                if (HasCircularDependency)
                    return "CIRCULAR";
                if (!IsDefined && !HasFallback)
                    return "UNDEFINED";
                if (!IsDefined && HasFallback)
                    return "FALLBACK";
                return "OK";
            }
        }
    }
}
