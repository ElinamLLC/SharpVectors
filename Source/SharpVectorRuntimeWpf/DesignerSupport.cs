using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;

namespace SharpVectors.Renderers.Wpf
{
    /// <summary>
    /// Provides utilities for WPF designer-time support and detection.
    /// This class handles design-time vs runtime detection, URI resolution,
    /// assembly context management, and diagnostic information.
    /// </summary>
    public static class DesignerSupport
    {
        #region Private Fields

        private static readonly object _syncLock = new object();
        private static bool? _cachedIsDesignMode;
        private static Assembly _cachedDesignTimeAssembly;
        private static DateTime _lastAssemblyCacheTime = DateTime.MinValue;
        private const int ASSEMBLY_CACHE_DURATION_MS = 5000; // Cache for 5 seconds

        #endregion

        #region Designer Detection

        /// <summary>
        /// Determines if code is running in the WPF designer with multi-level fallback detection.
        /// Uses process name, environment variables, and DesignerProperties as fallback.
        /// </summary>
        public static bool IsInDesignMode()
        {
            // Use cached result if available (per-session cache)
            if (_cachedIsDesignMode.HasValue)
            {
                return _cachedIsDesignMode.Value;
            }

            lock (_syncLock)
            {
                // Double-check after lock
                if (_cachedIsDesignMode.HasValue)
                {
                    return _cachedIsDesignMode.Value;
                }

                bool result = DetectDesignMode();
                _cachedIsDesignMode = result;
                return result;
            }
        }

        /// <summary>
        /// Clears the cached design mode detection result.
        /// Useful for testing or when design context changes.
        /// </summary>
        public static void ClearCache()
        {
            lock (_syncLock)
            {
                _cachedIsDesignMode = null;
                _cachedDesignTimeAssembly = null;
                _lastAssemblyCacheTime = DateTime.MinValue;
            }
        }

        private static bool DetectDesignMode()
        {
            try
            {
                // Use the standard WPF method: DesignerProperties.GetIsInDesignMode()
                // This is the most reliable way to detect if code is running in XAML designer
                // It checks the visual tree and design-time context, not just environment/process names
                bool isInDesigner = DesignerProperties.GetIsInDesignMode(new DependencyObject());
                if (isInDesigner)
                {
                    LogDiagnostic("Design mode detected via DesignerProperties");
                }
                return isInDesigner;
            }
            catch (Exception ex)
            {
                // Safe fallback on any exception
                LogDiagnostic($"Exception during design mode detection: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Assembly Resolution

        /// <summary>
        /// Gets the appropriate assembly to use for resource resolution in design mode.
        /// Implements intelligent fallback to find the actual application assembly,
        /// not the designer process assembly.
        /// </summary>
        public static Assembly GetDesignTimeResourceAssembly()
        {
            lock (_syncLock)
            {
                // Return cached result if still valid
                if (_cachedDesignTimeAssembly != null &&
                    (DateTime.UtcNow - _lastAssemblyCacheTime).TotalMilliseconds < ASSEMBLY_CACHE_DURATION_MS)
                {
                    return _cachedDesignTimeAssembly;
                }

                _cachedDesignTimeAssembly = ResolveResourceAssembly();
                _lastAssemblyCacheTime = DateTime.UtcNow;
                return _cachedDesignTimeAssembly;
            }
        }

        private static Assembly ResolveResourceAssembly()
        {
            try
            {
                // List of assembly names to exclude (designer/framework assemblies)
                var excludedPrefixes = new[]
                {
                    "SharpVectors",
                    "XDesProc",
                    "DevEnv",
                    "WpfSurface",
                    "System.",
                    "Microsoft.",
                    "PresentationCore",
                    "PresentationFramework",
                    "WindowsBase"
                };

                var appAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .Where(a => !excludedPrefixes.Any(prefix => 
                        a.GetName().Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                // Prefer executable assemblies (typically the user's app)
                var exeAssemblies = appAssemblies
                    .Where(a => GetAssemblyFile(a).EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (exeAssemblies.Any())
                    return exeAssemblies.First();

                // Fallback to first remaining assembly
                if (appAssemblies.Any())
                    return appAssemblies.First();

                // Ultimate fallback
                var resourceAssembly = Application.ResourceAssembly;
                if (resourceAssembly != null)
                    return resourceAssembly;

                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly != null)
                    return entryAssembly;

                return Assembly.GetExecutingAssembly();
            }
            catch (Exception ex)
            {
                LogDiagnostic($"Error resolving resource assembly: {ex.Message}");
                return Assembly.GetExecutingAssembly();
            }
        }

        private static string GetAssemblyFile(Assembly assembly)
        {
            try
            {
#if NETCOREAPP
                return Path.GetFileName(assembly.Location);
#else
                var codeBase = assembly.CodeBase;
                if (!string.IsNullOrEmpty(codeBase))
                {
                    var uri = new Uri(codeBase);
                    return Path.GetFileName(uri.LocalPath);
                }
                return Path.GetFileName(assembly.Location);
#endif
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion

        #region URI Resolution

        /// <summary>
        /// Resolves an SVG URI in design-time context, handling pack URIs, relative paths, and remote sources.
        /// </summary>
        public static Uri ResolveSvgUri(Uri sourceUri, Assembly resourceAssembly = null)
        {
            if (sourceUri == null)
                return null;

            try
            {
                if (sourceUri.IsAbsoluteUri)
                {
                    return ResolvieAbsoluteUri(sourceUri, resourceAssembly);
                }
                else
                {
                    return ResolveRelativeUri(sourceUri, resourceAssembly);
                }
            }
            catch (Exception ex)
            {
                LogDiagnostic($"Error resolving URI '{sourceUri}': {ex.Message}");
                return null;
            }
        }

        private static Uri ResolvieAbsoluteUri(Uri sourceUri, Assembly resourceAssembly)
        {
            // Handle pack URIs specially
            if (sourceUri.Scheme == "pack")
            {
                return HandlePackUri(sourceUri, resourceAssembly);
            }

            // Remote URIs (http, https, ftp, etc.)
            if (sourceUri.Scheme == "http" || sourceUri.Scheme == "https" || sourceUri.Scheme == "ftp")
            {
                // These work as-is, but should have timeout protection
                return sourceUri;
            }

            // File URIs
            if (sourceUri.Scheme == "file")
            {
                return sourceUri;
            }

            return sourceUri;
        }

        private static Uri ResolveRelativeUri(Uri sourceUri, Assembly resourceAssembly)
        {
            resourceAssembly = resourceAssembly ?? GetDesignTimeResourceAssembly();

            try
            {
                var assemblyLocation = resourceAssembly.Location;
                if (string.IsNullOrEmpty(assemblyLocation))
                    return null;

                var assemblyDir = Path.GetDirectoryName(assemblyLocation);
                var fullPath = Path.Combine(assemblyDir, sourceUri.OriginalString.Replace('/', '\\'));

                if (File.Exists(fullPath))
                {
                    return new Uri(fullPath, UriKind.Absolute);
                }

                LogDiagnostic($"Relative SVG file not found: {fullPath}");
                return null;
            }
            catch (Exception ex)
            {
                LogDiagnostic($"Error resolving relative URI: {ex.Message}");
                return null;
            }
        }

        private static Uri HandlePackUri(Uri packUri, Assembly resourceAssembly)
        {
            try
            {
                resourceAssembly = resourceAssembly ?? GetDesignTimeResourceAssembly();

                var uriString = packUri.ToString();

                // Convert pack URI to assembly-qualified path if needed
                if (!uriString.Contains("component") && !uriString.Contains("application"))
                {
                    return packUri;
                }

                // Try to load the resource directly using the pack URI
                // This is the most reliable approach for embedded resources
                return packUri;
            }
            catch (Exception ex)
            {
                LogDiagnostic($"Error handling pack URI '{packUri}': {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Diagnostics

        /// <summary>
        /// Provides comprehensive diagnostic information about the current design-time context.
        /// </summary>
        public static string GetDiagnosticInfo()
        {
            var sb = new StringBuilder();

            try
            {
                sb.AppendLine("=== SharpVectors Designer Support Diagnostics ===");
                sb.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                sb.AppendLine();

                // Process Information
                sb.AppendLine("--- Process Information ---");
                var process = Process.GetCurrentProcess();
                sb.AppendLine($"Process Name: {process.ProcessName}");
                sb.AppendLine($"Process ID: {process.Id}");
                sb.AppendLine();

                // Design Mode Detection
                sb.AppendLine("--- Design Mode Detection ---");
                sb.AppendLine($"IsInDesignMode: {IsInDesignMode()}");
                sb.AppendLine();

                // Assembly Information
                sb.AppendLine("--- Assembly Information ---");
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                sb.AppendLine($"Loaded Assemblies: {assemblies.Length}");
                sb.AppendLine($"Entry Assembly: {Assembly.GetEntryAssembly()?.GetName().Name ?? "null"}");
                sb.AppendLine($"Executing Assembly: {Assembly.GetExecutingAssembly().GetName().Name}");
                sb.AppendLine($"Resource Assembly: {Application.ResourceAssembly?.GetName().Name ?? "null"}");
                sb.AppendLine($"Design-Time Resource Assembly: {GetDesignTimeResourceAssembly().GetName().Name}");
                sb.AppendLine();

                // Environment Variables
                sb.AppendLine("--- Environment Variables ---");
                sb.AppendLine($"VisualStudioVersion: {Environment.GetEnvironmentVariable("VisualStudioVersion") ?? "not set"}");
                sb.AppendLine($"SHARPVECTORS_DESIGNER_MODE: {Environment.GetEnvironmentVariable("SHARPVECTORS_DESIGNER_MODE") ?? "not set"}");
                sb.AppendLine();

                // Framework Information
                sb.AppendLine("--- Framework Information ---");
#if NETCOREAPP
                sb.AppendLine($"Runtime: .NET Core/5+");
#else
                sb.AppendLine($"Runtime: .NET Framework");
#endif
                sb.AppendLine($"CLR Version: {Environment.Version}");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Error gathering diagnostics: {ex}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Outputs diagnostic information to the debug output.
        /// </summary>
        public static void OutputDiagnostics()
        {
            Debug.WriteLine(GetDiagnosticInfo());
        }

        private static void LogDiagnostic(string message)
        {
            Debug.WriteLine($"[DesignerSupport] {message}");
        }

        #endregion

        #region Fallback Rendering

        /// <summary>
        /// Creates a placeholder DrawingImage for display when actual SVG cannot be loaded in designer.
        /// </summary>
        public static System.Windows.Media.DrawingImage CreatePlaceholder(string message = null)
        {
            try
            {
                var drawingGroup = new System.Windows.Media.DrawingGroup();

                // Background rectangle
                var bgGeometry = new System.Windows.Media.RectangleGeometry(new Rect(0, 0, 64, 64));
                var bgBrush = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromArgb(255, 240, 240, 240));
                var bgPen = new System.Windows.Media.Pen(
                    new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.DarkGray), 1);
                var bgDrawing = new System.Windows.Media.GeometryDrawing(bgBrush, bgPen, bgGeometry);
                drawingGroup.Children.Add(bgDrawing);

                // SVG icon representation (simplified SVG shape)
                var iconGeometry = System.Windows.Media.Geometry.Parse(
                    "M 10 10 L 54 10 L 54 54 L 10 54 Z M 20 25 L 45 25 M 20 35 L 45 35 M 20 45 L 30 45");
                var iconBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
                var iconDrawing = new System.Windows.Media.GeometryDrawing(iconBrush, null, iconGeometry);
                drawingGroup.Children.Add(iconDrawing);

                return new System.Windows.Media.DrawingImage(drawingGroup);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating placeholder: {ex}");
                return null;
            }
        }

        #endregion

        #region Timeout Protection

        /// <summary>
        /// Executes an action with timeout protection, useful for preventing designer hangs.
        /// </summary>
        public static bool ExecuteWithTimeout(Action action, int timeoutMs = 3000)
        {
            if (action == null)
                return false;

            if (!IsInDesignMode())
            {
                // No timeout needed at runtime
                try
                {
                    action();
                    return true;
                }
                catch (Exception ex)
                {
                    LogDiagnostic($"Error executing action: {ex.Message}");
                    return false;
                }
            }

            // At design time, use timeout to prevent hangs
            var resetEvent = new ManualResetEvent(false);
            bool success = false;
            Exception caughtException = null;

            var thread = new Thread(() =>
            {
                try
                {
                    action();
                    success = true;
                }
                catch (Exception ex)
                {
                    caughtException = ex;
                }
                finally
                {
                    resetEvent.Set();
                }
            })
            {
                IsBackground = true
            };

            thread.Start();

            if (!resetEvent.WaitOne(timeoutMs))
            {
                LogDiagnostic($"Operation timeout after {timeoutMs}ms in design mode");
                return false;
            }

            if (caughtException != null)
            {
                LogDiagnostic($"Timeout action error: {caughtException.Message}");
            }

            return success;
        }

        /// <summary>
        /// Executes a function with timeout protection, useful for preventing designer hangs.
        /// </summary>
        public static T ExecuteWithTimeout<T>(Func<T> func, T defaultValue = default, int timeoutMs = 3000)
        {
            if (func == null)
                return defaultValue;

            if (!IsInDesignMode())
            {
                // No timeout needed at runtime
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    LogDiagnostic($"Error executing function: {ex.Message}");
                    return defaultValue;
                }
            }

            // At design time, use timeout to prevent hangs
            var resetEvent = new ManualResetEvent(false);
            T result = defaultValue;
            bool succeeded = false;
            Exception caughtException = null;

            var thread = new Thread(() =>
            {
                try
                {
                    result = func();
                    succeeded = true;
                }
                catch (Exception ex)
                {
                    caughtException = ex;
                }
                finally
                {
                    resetEvent.Set();
                }
            })
            {
                IsBackground = true
            };

            thread.Start();

            if (!resetEvent.WaitOne(timeoutMs))
            {
                LogDiagnostic($"Operation timeout after {timeoutMs}ms in design mode");
                return defaultValue;
            }

            if (caughtException != null)
            {
                LogDiagnostic($"Timeout function error: {caughtException.Message}");
                return defaultValue;
            }

            return succeeded ? result : defaultValue;
        }

        #endregion
    }
}
