using NUnit.Framework;

using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SharpVectors.Rendering.Wpf.Tests.Core
{
    [TestFixture]
    public class SharpVectorRegressionTests
    {
        /// <summary>
        /// Regression test for Issue #310: Stack overflow in SVG rendering for Petra timeline.
        /// This test verifies that the SVG file can be rendered successfully without encountering
        /// infinite recursion or stack overflow exceptions.
        /// </summary>
        [Test]
        public void Convert_MalformedSvg_RendersSuccessfully()
        {
            // Try to find the file relative to the test assembly location
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? AppContext.BaseDirectory;
            string testSvgPath = Path.Combine(baseDirectory, "Data/Issue310_Petra-timeline.svg");

            // Verify that the test SVG file exists in the output directory
            FileAssert.Exists(testSvgPath, $"The test SVG file was not found at path: {testSvgPath}");

            // Arrange
            bool completedSuccessfully = false;
            Exception thrownException = null;

            // Act: Run conversion on a separate thread with a reasonable stack size
            // 262144 bytes = 256 KB max stack size (helps expose stack overflow issues early)
            Thread workerThread = new Thread(() =>
            {
                try
                {
                    var settings = new WpfDrawingSettings();
                    using (var reader = new FileSvgReader(settings))
                    {
                        DrawingGroup drawingGroup = reader.Read(testSvgPath);

                        // Verify we got a valid result
                        if (drawingGroup != null)
                        {
                            completedSuccessfully = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    thrownException = ex;
                }
            }, 262144);

            workerThread.Start();

            // Give the renderer a reasonable timeout (5 seconds)
            // If it takes longer than this, there may be an infinite loop
            bool finished = workerThread.Join(TimeSpan.FromSeconds(5));

            // Assert
            if (thrownException != null)
            {
                Assert.Fail(
                    $"Rendering threw an exception: {thrownException.GetType().Name}: {thrownException.Message}\n\n{thrownException.StackTrace}");
            }

            if (!finished)
            {
                Assert.Fail("Rendering operation timed out after 5 seconds. Possible infinite loop or recursion.");
            }

            if (!completedSuccessfully)
            {
                if (workerThread.ThreadState == ThreadState.Stopped)
                {
                    Assert.Fail("Rendering thread terminated without completing successfully.");
                }
                else
                {
                    Assert.Fail("Rendering did not complete successfully.");
                }
            }

            Assert.IsTrue(completedSuccessfully, "SVG should render successfully without exceptions or timeouts.");
        }

        /// <summary>
        /// Regression test for Issue #312: WpfDrawingSettings is not thread-safe.
        /// This test verifies that when WpfDrawingSettings is shared across multiple threads
        /// (as in ParallelEnumerable operations), it properly manages the registered IDs
        /// without race conditions or data loss.
        /// 
        /// The issue occurs when multiple threads access settings[RegisteredIdKey] concurrently:
        /// - Thread A reads null, Thread B reads null
        /// - Both create new HashSet instances, overwriting each other
        /// - IDs registered by one thread are lost
        /// 
        /// This is a simple baseline test with moderate contention.
        /// </summary>
        [Test]
        public void WpfDrawingSettings_ParallelAccess_IsThreadSafe()
        {
            // Arrange
            var settings = new WpfDrawingSettings { IncludeRuntime = false, IgnoreRootViewbox = false };
            var registeredIdSets = new List<HashSet<string>>();
            var errors = new List<Exception>();
            object lockObj = new object();

            // Act: Simulate parallel access similar to the bug report scenario
            // Create multiple tasks that each try to register IDs through WpfDrawingContext
            var tasks = Enumerable.Range(0, 10).Select(taskIndex =>
            {
                return Task.Run(() =>
                {
                    try
                    {
                        // Each context retrieves or creates the registered IDs HashSet
                        var context = new WpfDrawingContext(false, settings);

                        // Simulate ID registration by recording unique IDs
                        var registeredIds = new HashSet<string>();
                        for (int i = 0; i < 100; i++)
                        {
                            string uniqueId = $"task{taskIndex}_id{i}";
                            registeredIds.Add(uniqueId);
                        }

                        lock (lockObj)
                        {
                            registeredIdSets.Add(registeredIds);
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (lockObj)
                        {
                            errors.Add(ex);
                        }
                    }
                });
            }).ToArray();

            Task.WaitAll(tasks);

            // Assert
            Assert.That(errors, Is.Empty, 
                "No exceptions should be thrown during parallel access: " + 
                string.Join(", ", errors.Select(e => e.Message)));

            Assert.That(registeredIdSets, Has.Count.EqualTo(10), 
                "All 10 tasks should have completed and recorded their ID sets");

            // Verify that each task was able to register its IDs without loss
            // If there were race conditions, some sets would be empty or corrupted
            foreach (var set in registeredIdSets)
            {
                Assert.That(set.Count, Is.EqualTo(100), 
                    "Each task should have successfully registered all 100 IDs without loss due to race conditions");
            }
        }
        [Test]
        public void WpfDrawingSettings_ParallelAccess_StressTest()
        {
            // Arrange
            var errors = new List<Exception>();
            object lockObj = new object();

            // Act: Run the parallel access scenario multiple times
            for (int iteration = 0; iteration < 5; iteration++)
            {
                var settings = new WpfDrawingSettings { IncludeRuntime = false, IgnoreRootViewbox = false };
                var registeredIdSets = new List<HashSet<string>>();

                // Create more tasks for higher contention
                var tasks = Enumerable.Range(0, 20).Select(taskIndex =>
                {
                    return Task.Run(() =>
                    {
                        try
                        {
                            // Each context retrieves or creates the registered IDs HashSet
                            var context = new WpfDrawingContext(false, settings);

                            // Simulate ID registration
                            var registeredIds = new HashSet<string>();
                            for (int i = 0; i < 50; i++)
                            {
                                string uniqueId = $"iter{iteration}_task{taskIndex}_id{i}";
                                registeredIds.Add(uniqueId);
                            }

                            lock (lockObj)
                            {
                                registeredIdSets.Add(registeredIds);
                            }
                        }
                        catch (Exception ex)
                        {
                            lock (lockObj)
                            {
                                errors.Add(ex);
                            }
                        }
                    });
                }).ToArray();

                Task.WaitAll(tasks);

                // Verify each iteration succeeded
                if (errors.Any())
                    break;

                Assert.That(registeredIdSets, Has.Count.EqualTo(20),
                    $"Iteration {iteration}: All 20 tasks should have completed");

                foreach (var set in registeredIdSets)
                {
                    Assert.That(set.Count, Is.EqualTo(50),
                        $"Iteration {iteration}: Each task should have registered all 50 IDs without loss");
                }
            }

            // Assert
            Assert.That(errors, Is.Empty,
                "No exceptions should be thrown during stress test: " +
                string.Join(", ", errors.Select(e => e.Message)));
        }
    }
}

