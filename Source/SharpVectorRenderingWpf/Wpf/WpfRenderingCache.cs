using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.Collections.Concurrent;

using SharpVectors.Dom;
using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public static class WpfRenderingCache
    {
        private static ConcurrentDictionary<string, WpfRendering> _cache;

        private static Timer _cleanupTimer;

        private static TimeSpan _inactivityPeriod;
        private static TimeSpan _cleanupInterval;

        private static DateTime _lastCacheAccessTime;
        
        private static Dispatcher _uiDispatcher;

        private static bool _isTimerEnabled = false; // A flag to track timer state
        private static bool _isDisposed = false;
        private static bool _isInitialized = false;

        private static readonly object _initLock = new object();
        private static readonly object _accessTimeLock = new object();

        public static bool IsDispose => _isDisposed;

        public static bool IsInitialized => _isInitialized;

        public static bool IsTimerEnabled => _isTimerEnabled;

        public static void Initialize(TimeSpan inactivityPeriod, TimeSpan cleanupInterval, Dispatcher uiDispatcher)
        {
            lock (_initLock)
            {
                if (_isInitialized)
                {
                    LogTrace("Warning: WpfRenderingCache already initialized. Skipping re-initialization.");
                    return;
                }
                if (uiDispatcher == null)
                {
                    throw new ArgumentNullException(nameof(uiDispatcher), "UI Dispatcher cannot be null for WPF cache manager.");
                }

                _inactivityPeriod = inactivityPeriod;
                _cleanupInterval = cleanupInterval;
                _uiDispatcher = uiDispatcher;

                _cache = new ConcurrentDictionary<string, WpfRendering>();
                _lastCacheAccessTime = DateTime.UtcNow;

                // Create the timer but initialize it as disabled (stopped)
                _cleanupTimer = new Timer(InternalCleanupCache, null, Timeout.Infinite, Timeout.Infinite);
                _isTimerEnabled = false;

                _isInitialized = true;
                _isDisposed = false;

                LogTrace("WpfRenderingCache initialized.");
            }
        }

        public static WpfRendering Create(ISvgElement element)
        {
            if (element == null)
            {
                return null;
            }
            // --- New Step 0: Derive the key from the element ---
            // Assuming ISvgElement can be safely cast to SvgElement to access LocalName
            // Or, if LocalName is part of ISvgElement itself, even better.
            if (!(element is SvgElement svgElement))
            {
                throw new ArgumentException(
                    "Provided element must be an SvgElement instance to derive a key.", nameof(element));
            }
            string localName = svgElement.LocalName; // This is the cache key

            EnsureInitialized();

            // 1. Check if the timer needs to be started
            if (!_isTimerEnabled)
            {
                lock (_initLock) // Use the main lock for thread-safe timer state change
                {
                    if (!_isTimerEnabled)
                    {
                        LogTrace("Cache received first item. Starting cleanup timer.");
                        _cleanupTimer.Change(_cleanupInterval, _cleanupInterval);
                        _isTimerEnabled = true;
                    }
                }
            }

            // 2. Update last access time (cache-wide)
            lock (_accessTimeLock)
            {
                _lastCacheAccessTime = DateTime.UtcNow;
            }

            // ... rest of the Create method (TryGetValue, GetOrAdd, etc.) ...
            if (!string.IsNullOrWhiteSpace(localName))
            {
                if (_cache.TryGetValue(localName, out WpfRendering existingValue))
                {
                    if (existingValue.IsReady)
                    {
                        existingValue.InternalInitialize(svgElement);

                        existingValue.IsReady = false;
                        return existingValue;
                    }
                    //return existingValue;
                }
                else
                {
                    WpfRendering newValue = WpfRendering.CreateRendering(svgElement);
                    WpfRendering addedOrExistingValue = _cache.GetOrAdd(localName, newValue);
                    addedOrExistingValue.IsReady = false;

                    if (ReferenceEquals(addedOrExistingValue, newValue) == false)
                    {
                        newValue?.Dispose();
                    }

                    return addedOrExistingValue;
                }
            }

            return WpfRendering.CreateRendering(svgElement);
        }

        public static void ClearCache()
        {
            if (!_isInitialized || _isDisposed) return;
            if (_cache == null || _cache.IsEmpty)
            {
                return;
            }

            //EnsureInitialized();
            LogTrace("Manual cache clear initiated.");
            PerformFullCacheClear();
        }

        public static void Shutdown()
        {
            lock (_initLock)
            {
                if (_isDisposed)
                {
                    return;
                }

                Dispose();
            }
        }

        private static void Dispose()
        {
            lock (_initLock)
            {
                if (_isDisposed) return;
                if (!_isInitialized)
                {
                    _isDisposed = true;
                    return;
                }

                _cleanupTimer?.Dispose();

                if (_cache == null || _cache.IsEmpty)
                {
                    LogTrace("Cache already empty or not initialized during dispose.");
                }
                else
                {
                    LogTrace("Disposing static cache: clearing all remaining items during shutdown.");

                    var cachedRenderers = _cache.Values.ToList();
                    _cache.Clear();

                    if (_uiDispatcher != null && _uiDispatcher.Thread.IsAlive && !_uiDispatcher.HasShutdownStarted)
                    {
                        _uiDispatcher.Invoke(new Action(() =>
                        {
                            foreach (var cachedRenderer in cachedRenderers)
                            {
                                try
                                {
                                    // If in used, do not dispose...
                                    if (cachedRenderer.IsReady)
                                    {
                                        cachedRenderer.Dispose();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogTrace($"Error during shutdown disposing cached item: {ex.Message}");
                                }
                            }
                        }));
                    }
                    else
                    {
                        LogTrace("UI Dispatcher not available or shutting down during dispose. Disposing items directly on current thread.");
                        foreach (var cachedRenderer in cachedRenderers)
                        {
                            try
                            {
                                // If in used, do not dispose...
                                if (cachedRenderer.IsReady)
                                {
                                    cachedRenderer.Dispose();
                                }
                            }
                            catch (Exception ex)
                            {
                                LogTrace($"Error during shutdown (no dispatcher) disposing cached item: {ex.Message}");
                            }
                        }
                    }
                }

                _uiDispatcher = null;
                _cache = null;

                _isDisposed = true;
                _isInitialized = false;
                LogTrace("WpfRenderingCache fully disposed.");
            }
        }

        private static void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException(
                    "WpfRenderingCache is not initialized. Call WpfRenderingCache.Initialize() from your UI thread (e.g., App.xaml.cs OnStartup) first.");
            }
        }

        private static void StopTimer()
        {
            lock (_initLock) // Use the lock for thread-safe state change
            {
                if (_isTimerEnabled)
                {
                    LogTrace("Cache is empty. Stopping cleanup timer.");
                    _cleanupTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    _isTimerEnabled = false;
                }
            }
        }

        private static void InternalCleanupCache(object state)
        {
            if (!_isInitialized || _isDisposed) return;
            LogTrace("InternalCleanupCache: " + _cache.Count);
            if (_cache == null || _cache.IsEmpty)
            {
                return;
            }

            DateTime cutoff;
            lock (_accessTimeLock)
            {
                cutoff = DateTime.UtcNow - _inactivityPeriod;
                if (_lastCacheAccessTime >= cutoff)
                {
                    return;
                }
            }

            LogTrace($"Cache inactive for {_inactivityPeriod.TotalMinutes} minutes. Initiating full clear...");
            PerformFullCacheClear();
        }

        private static void PerformFullCacheClear()
        {
            if (_cache == null || _cache.IsEmpty)
            {
                LogTrace("Cache already empty or not initialized, skipping full clear.");
                // Stop the timer if it's already running and the cache is empty
                if (_isTimerEnabled)
                {
                    StopTimer();
                }
                return;
            }

            var cachedRenderers = _cache.Values.ToList();
            // Clear the cache
            _cache.Clear();

            if (_uiDispatcher != null && _uiDispatcher.Thread.IsAlive && !_uiDispatcher.HasShutdownStarted)
            {
                _uiDispatcher.Invoke(new Action(() =>
                {
                    foreach (var cachedRenderer in cachedRenderers)
                    {
                        try
                        {
                            // If in used, do not dispose...
                            if (cachedRenderer.IsReady)
                            {
                                cachedRenderer.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogTrace($"Error disposing cached item during full clear: {ex.Message}");
                        }
                    }
                    LogTrace("Full cache clear and renderer disposal complete.");
                }));
            }
            else
            {
                LogTrace("UI Dispatcher not available or shutting down. Disposing items directly on current thread.");
                foreach (var cachedRenderer in cachedRenderers)
                {
                    try
                    {
                        // If in used, do not dispose...
                        if (cachedRenderer.IsReady)
                        {
                            cachedRenderer.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogTrace($"Error during full clear (no dispatcher) disposing cached item: {ex.Message}");
                    }
                }
                LogTrace("Full cache clear and renderer disposal complete (without UI Dispatcher).");
            }

            // Stop the timer because the cache is now empty
            StopTimer();
        }

        // --- A helper method for safe Trace.WriteLine ---
        [Conditional("DEBUG")]
        private static void LogTrace(string message)
        {
            try
            {
                Trace.WriteLine(message);
            }
            catch (ObjectDisposedException)
            {
                // This is common during application shutdown when TraceListeners are being disposed.
                // Just suppress the error, as we're trying to log during a tearing-down process.
            }
            catch (Exception ex)
            {
                // Catch other unexpected errors during tracing.
                // For critical errors, you might consider logging to a very basic,
                // always-available fallback (like a simple file append if possible,
                // or even just Debug.WriteLine which is simpler).
                // For now, we'll just suppress.
            }
        }
    }
}
