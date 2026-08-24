using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// Provides context information and diagnostics for CSS parsing operations.
    /// Helps track parsing state, errors, and performance metrics.
    /// </summary>
    public class CssParsingContext
    {
        private readonly List<CssParsingError> _errors = new List<CssParsingError>();
        private readonly List<CssParsingWarning> _warnings = new List<CssParsingWarning>();
        private int _rulesParsed;
        private DateTime _startTime;

        /// <summary>
        /// Gets the collection of parsing errors encountered.
        /// </summary>
        public ReadOnlyCollection<CssParsingError> Errors
        {
            get { return _errors.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the collection of parsing warnings encountered.
        /// </summary>
        public ReadOnlyCollection<CssParsingWarning> Warnings
        {
            get { return _warnings.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the number of successfully parsed rules.
        /// </summary>
        public int RulesParsed => _rulesParsed;

        /// <summary>
        /// Gets the elapsed time for the parsing operation.
        /// </summary>
        public TimeSpan ElapsedTime { get; private set; }

        /// <summary>
        /// Gets a value indicating whether any errors occurred during parsing.
        /// </summary>
        public bool HasErrors => _errors.Count > 0;

        /// <summary>
        /// Gets a value indicating whether any warnings occurred during parsing.
        /// </summary>
        public bool HasWarnings => _warnings.Count > 0;

        /// <summary>
        /// Starts tracking the parsing operation.
        /// </summary>
        public void StartTracking()
        {
            _startTime = DateTime.UtcNow;
            _rulesParsed = 0;
            _errors.Clear();
            _warnings.Clear();
        }

        /// <summary>
        /// Completes tracking and calculates elapsed time.
        /// </summary>
        public void StopTracking()
        {
            ElapsedTime = DateTime.UtcNow - _startTime;
        }

        /// <summary>
        /// Records a successfully parsed rule.
        /// </summary>
        public void RecordRuleParsed()
        {
            _rulesParsed++;
        }

        /// <summary>
        /// Records a parsing error.
        /// </summary>
        /// <param name="message">Description of the error</param>
        /// <param name="position">Position in the CSS where the error occurred</param>
        /// <param name="context">Context snippet of the CSS around the error</param>
        public void AddError(string message, int position = -1, string context = null)
        {
            _errors.Add(new CssParsingError 
            { 
                Message = message, 
                Position = position, 
                Context = context,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Records a parsing warning.
        /// </summary>
        /// <param name="message">Description of the warning</param>
        /// <param name="severity">Severity level of the warning</param>
        public void AddWarning(string message, CssWarningLevel severity = CssWarningLevel.Info)
        {
            _warnings.Add(new CssParsingWarning 
            { 
                Message = message, 
                Severity = severity,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Gets a formatted summary of parsing results.
        /// </summary>
        public string GetSummary()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== CSS Parsing Summary ===");
            sb.AppendLine($"Rules Parsed: {_rulesParsed}");
            sb.AppendLine($"Errors: {_errors.Count}");
            sb.AppendLine($"Warnings: {_warnings.Count}");
            sb.AppendLine($"Elapsed Time: {ElapsedTime.TotalMilliseconds:F2}ms");

            if (_errors.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Errors:");
                foreach (var error in _errors)
                {
                    sb.AppendLine($"  [{error.Position}] {error.Message}");
                    if (!string.IsNullOrEmpty(error.Context))
                    {
                        sb.AppendLine($"    Context: {error.Context}");
                    }
                }
            }

            if (_warnings.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Warnings:");
                foreach (var warning in _warnings)
                {
                    sb.AppendLine($"  [{warning.Severity}] {warning.Message}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Clears all errors and warnings.
        /// </summary>
        public void Clear()
        {
            _errors.Clear();
            _warnings.Clear();
            _rulesParsed = 0;
            ElapsedTime = TimeSpan.Zero;
        }
    }

    /// <summary>
    /// Represents a CSS parsing error.
    /// </summary>
    public class CssParsingError
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the position in the CSS where the error occurred.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the context snippet around the error.
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the error was recorded.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Returns a string representation of this error.
        /// </summary>
        public override string ToString()
        {
            if (Position >= 0)
            {
                return $"[{Position}] {Message}";
            }
            return Message;
        }
    }

    /// <summary>
    /// Represents a CSS parsing warning.
    /// </summary>
    public class CssParsingWarning
    {
        /// <summary>
        /// Gets or sets the warning message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the severity level of the warning.
        /// </summary>
        public CssWarningLevel Severity { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the warning was recorded.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Returns a string representation of this warning.
        /// </summary>
        public override string ToString()
        {
            return $"[{Severity}] {Message}";
        }
    }

    /// <summary>
    /// Severity levels for CSS parsing warnings.
    /// </summary>
    public enum CssWarningLevel
    {
        /// <summary>
        /// Informational message.
        /// </summary>
        Info = 0,

        /// <summary>
        /// Minor issue that doesn't affect parsing.
        /// </summary>
        Low = 1,

        /// <summary>
        /// Potentially problematic but still valid.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// Significant issue that may cause unexpected behavior.
        /// </summary>
        High = 3
    }
}
