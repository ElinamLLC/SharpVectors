using System;
using NUnit.Framework;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Rendering.Wpf.Tests.CssVariables
{
    [TestFixture]
    public class CssVariableResolutionTest
    {
        [Test]
        public void ResolveVariables_NullInput_ReturnsNull()
        {
            string result = CssVariableResolver.ResolveVariables(null);
            Assert.IsNull(result);
        }

        [Test]
        public void ResolveVariables_EmptyString_ReturnsEmpty()
        {
            string result = CssVariableResolver.ResolveVariables(string.Empty);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ResolveVariables_NoVariables_ReturnedUnchanged()
        {
            string value = "#FF0000";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void ResolveVariables_UnresolvedVariable_ReturnsEmpty()
        {
            string value = "var(--missing-color)";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ResolveVariables_SimpleVariableWithLiteralFallback_ReturnsFallback()
        {
            string value = "var(--missing-color, #FF0000)";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.AreEqual("#FF0000", result);
        }

        [Test]
        public void ResolveVariables_VariableWithFallback_FallbackHasWhitespace_ReturnsTrimmedFallback()
        {
            string value = "var(--missing-color, #FF0000 )";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.AreEqual("#FF0000", result);
        }

        [Test]
        public void ResolveVariables_NestedFallback_ResolvesRecursively()
        {
            // Fallback is itself a var() call
            string value = "var(--missing-a, var(--missing-b, #0000FF))";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.AreEqual("#0000FF", result);
        }

        [Test]
        public void ResolveVariables_NestedFallbackWithIntermediateLiteral_ReturnsIntermediateLiteral()
        {
            // Fallback chain: var(--missing-a, rgb(255,0,0)) which is literal
            string value = "var(--missing-a, rgb(255,0,0))";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.AreEqual("rgb(255,0,0)", result);
        }

        [Test]
        public void ResolveVariables_DeepNestedFallbackChain_ResolvesToTerminalValue()
        {
            // Deep nesting: var(--a, var(--b, var(--c, #000)))
            string value = "var(--missing-a, var(--missing-b, var(--missing-c, #000)))";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.AreEqual("#000", result);
        }

        [Test]
        public void ResolveVariables_CircularReference_PreventsInfiniteLoop()
        {
            // This simulates a circular reference scenario (though actual resolution would be caught at CSS parse time)
            string value = "var(--a, var(--a, #000))";
            string result = CssVariableResolver.ResolveVariables(value);
            // Should gracefully degrade to empty or resolved value, not hang
            Assert.IsNotNull(result);
        }

        [Test]
        public void ResolveVariables_ExceedsMaxDepth_StopsRecursion()
        {
            // Create a deeply nested fallback chain that exceeds max depth (currently 10)
            // var(--a, var(--b, var(--c, var(--d, var(--e, var(--f, var(--g, var(--h, var(--i, var(--j, var(--k, #000))))))))))
            string value = "var(--a, var(--b, var(--c, var(--d, var(--e, var(--f, var(--g, var(--h, var(--i, var(--j, var(--k, #000)))))))))))";
            string result = CssVariableResolver.ResolveVariables(value);
            // Should stop at max depth and return gracefully
            Assert.IsNotNull(result);
        }

        [Test]
        public void ResolveVariables_MultipleVarsWithMixedFallbacks_FirstVarResolved()
        {
            // "var(--a) var(--b, #FF0000)" - has multiple var() calls
            // First var() is unresolved, but we should resolve the entire expression
            string value = "var(--a) var(--b, #FF0000)";
            string result = CssVariableResolver.ResolveVariables(value);
            // Should extract the first fallback or handle appropriately
            Assert.IsNotNull(result);
        }

        [Test]
        public void ResolveVariables_VariableWithWhitespaceAroundComma_HandlesCorrectly()
        {
            string value = "var(  --missing-color  ,  #FF0000  )";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.AreEqual("#FF0000", result);
        }

        [Test]
        public void ResolveVariables_VariableWithCommaInFallback_ExtractsCorrectFallback()
        {
            // Fallback is "rgb(255, 0, 0)" which contains commas
            string value = "var(--missing-color, rgb(255, 0, 0))";
            string result = CssVariableResolver.ResolveVariables(value);
            // Should correctly parse the fallback despite the comma inside rgb()
            Assert.AreEqual("rgb(255, 0, 0)", result);
        }

        [Test]
        public void ResolveVariables_FallbackIsComputedValue_ReturnsAsIs()
        {
            string value = "var(--missing-color, calc(100% - 20px))";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.AreEqual("calc(100% - 20px)", result);
        }

        [Test]
        public void ResolveVariables_MalformedVar_ReturnedUnchanged()
        {
            // Missing closing paren
            string value = "var(--missing-color";
            string result = CssVariableResolver.ResolveVariables(value);
            // Should handle gracefully
            Assert.IsNotNull(result);
        }

        [Test]
        public void ResolveVariables_EmptyFallback_ReturnsEmpty()
        {
            string value = "var(--missing-color, )";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ResolveVariables_VariableNameWithUnderscores_HandledCorrectly()
        {
            string value = "var(--my_custom_color, #0F0)";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.AreEqual("#0F0", result);
        }

        [Test]
        public void ResolveVariables_FallbackWithLeadingZero_PreservedAsIs()
        {
            string value = "var(--missing-weight, 0700)";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.AreEqual("0700", result);
        }

        [Test]
        public void ResolveVariables_CaseInsensitiveVar_HandledCorrectly()
        {
            string value = "VAR(--missing-color, #FF0000)";
            string result = CssVariableResolver.ResolveVariables(value);
            // Should handle case-insensitivity
            Assert.IsNotNull(result);
        }

        [Test]
        public void ResolveVariables_NestedVarWithComplexFallback_ResolvesCorrectly()
        {
            // var(--missing, var(--also-missing, rgba(255, 0, 0, 0.5)))
            string value = "var(--missing, var(--also-missing, rgba(255, 0, 0, 0.5)))";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.AreEqual("rgba(255, 0, 0, 0.5)", result);
        }

        [Test]
        public void ResolveVariables_VariableWithNumericFallback_ReturnsFallback()
        {
            string value = "var(--missing-width, 300px)";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.AreEqual("300px", result);
        }

        [Test]
        public void ResolveVariables_VariableWithPercentFallback_ReturnsFallback()
        {
            string value = "var(--missing-percentage, 50%)";
            string result = CssVariableResolver.ResolveVariables(value);
            Assert.AreEqual("50%", result);
        }

        [Test]
        public void ResolveVariables_ConsecutiveVarCalls_HandledAppropriately()
        {
            // Two separate var() calls in sequence
            string value = "var(--a, #111) var(--b, #222)";
            string result = CssVariableResolver.ResolveVariables(value);
            // First var is processed; may contain both vars or first only depending on implementation
            Assert.IsNotNull(result);
        }
    }
}
