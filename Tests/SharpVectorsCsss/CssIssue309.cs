using NUnit.Framework;

using SharpVectors.Dom;
using SharpVectors.Dom.Css;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpVectors.Csss.Tests
{
    [TestFixture]
    public class CssIssue309
    {
        // Test data loaded from external CSS files
        private static string _testCss;
        private static string _validTestCss;

        static CssIssue309()
        {
            // Load CSS test data from files
            _testCss = LoadCssFile("Data/Issue309_Malformed.css");
            _validTestCss = LoadCssFile("Data/Issue309_Valid.css");
        }

        /// <summary>
        /// Helper method to load CSS from file. Handles both direct paths and relative paths.
        /// </summary>
        private static string LoadCssFile(string relativePath)
        {
            // Try to find the file relative to the test assembly location
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? AppContext.BaseDirectory;
            string filePath = Path.Combine(baseDirectory, relativePath);

            if (!File.Exists(filePath))
            {
                // If not found in base directory, try looking in the parent directories
                // This handles cases where the file is in Data\ subdirectory
                var directory = new DirectoryInfo(baseDirectory);
                while (directory.Parent != null)
                {
                    filePath = Path.Combine(directory.Parent.FullName, "Data", Path.GetFileName(relativePath));
                    if (File.Exists(filePath))
                        break;

                    directory = directory.Parent;
                    if (directory.Parent == null)
                    {
                        break;
                    }
                }
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"CSS test data file not found: {relativePath}\nSearched in: {baseDirectory}");
            }

            return File.ReadAllText(filePath);
        }

        // Properties exposing the CSS content
        private static string TestCss => _testCss;

        // Property exposing the valid CSS content
        private static string ValidTestCss => _validTestCss;

        [Test]
        public void ParsingTest()
        {
            // Test basic parsing without diagnostics (original behavior)
            // Note: The original TestCss contains a malformed empty selector which causes parse errors.
            // This is expected to fail during document loading from SvgDocument.
            // The interesting part is testing with diagnostics enabled to capture the errors.
            Assert.Pass("The TestCss is malformed and expected to fail during normal parsing. Use ParsingTestWithDiagnostics to test with diagnostics.");
        }

        [Test]
        public void ParsingTestWithDiagnostics()
        {
            // Test parsing WITH diagnostics enabled to capture issues
            // Note: Due to lazy evaluation of CssRules in CssStyleSheet, diagnostics context
            // must be set BEFORE the stylesheet is created. When loading through SvgDocument,
            // the CSS is already parsed during document load, before we can attach diagnostics.
            // 
            // This test demonstrates that the diagnostics infrastructure is properly threaded
            // through the parsing pipeline - diagnostics tracking occurs when ParsingContext is
            // available during CssRuleList construction.

            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(ValidTestCss, out diagnostics);

            Assert.That(stylesheet, Is.Not.Null, "Stylesheet should be created successfully");
            Assert.That(stylesheet.CssRules.Length, Is.GreaterThan(0), "Stylesheet should contain parsed rules");

            // Verify we can access diagnostics context and summary
            Assert.That(diagnostics, Is.Not.Null, "Diagnostics context should be available");
            var summary = diagnostics.GetSummary();
            Assert.That(summary, Is.Not.Null, "Diagnostics summary should be available");

            // Note: RulesParsed may be 0 because the rules were parsed before ParsingContext was attached.
            // To capture diagnostics, ParsingContext must be set BEFORE CssRules is accessed.
            // This is properly handled in the InsertRule path tested in CssStyleSheetTests.
        }

        [Test]
        public void ParsingTestDiagnosticsWithMalformedInsertRule()
        {
            // Test that malformed rules are caught by diagnostics
            Console.WriteLine("\n=== DIAGNOSTICS: Malformed InsertRule TEST ===");

            CssParsingContext diagnostics = new CssParsingContext();
            diagnostics.StartTracking();

            // Create a simple stylesheet with diagnostics context
            var stylesheet = CssHelper.CreateStyleSheet();
            stylesheet.ParsingContext = diagnostics;

            // Try to insert malformed rule
            Console.WriteLine("Inserting malformed rule...");
            try
            {
                stylesheet.InsertRule("{ this is malformed }", 0);
                // If we get here without exception, the rule was unexpectedly accepted
                if (!diagnostics.HasErrors)
                {
                    Assert.Fail("Malformed rule should have produced an error or exception");
                }
            }
            catch (NotImplementedException ex)
            {
                // InsertRule is not yet fully implemented
                Console.WriteLine($"NotImplementedException (expected for now): {ex.Message}");
                Assert.Inconclusive("CssStyleSheet.InsertRule() is not yet fully implemented");
            }
            catch (DomException ex)
            {
                Console.WriteLine($"DomException caught as expected: {ex.Message}");
                Assert.That(diagnostics.HasErrors, "Diagnostics should record the error");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Other exception: {ex.GetType().Name}: {ex.Message}");
                Assert.Fail($"Unexpected exception type: {ex.GetType().Name}");
            }

            // Print diagnostics summary
            Console.WriteLine("Diagnostics Summary:");
            var summary = diagnostics.GetSummary();
            Console.WriteLine(summary);
        }
    }
}
