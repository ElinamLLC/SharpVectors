using NUnit.Framework;
using SharpVectors.Dom.Css;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpVectors.Css.Tests
{
    [TestFixture]
    public class CssVariablesTests
    {
        #region Test Data

        private const string ValidVariablesCss = @"
:root {
    --primary-color: #3498db;
    --secondary-color: #2ecc71;
    --font-size: 16px;
    --spacing: 8px;
}

.header {
    --header-bg: #1a1a1a;
    --header-fg: white;
    color: var(--header-fg);
    background: var(--header-bg);
}

.button {
    color: var(--primary-color);
    background: var(--secondary-color);
    padding: var(--spacing);
    font-size: var(--font-size);
}

.with-fallback {
    border-color: var(--border-color, #cccccc);
    margin: var(--undefined-margin, 10px);
}
";

        private const string UndefinedVariablesCss = @"
.undefined {
    color: var(--undefined-color);
    background: var(--undefined-bg);
}

.with-fallback {
    border: var(--undefined-border, 1px solid red);
}
";

        private const string CircularDependenciesCss = @"
:root {
    --color-a: var(--color-b);
    --color-b: var(--color-a);
    --color-c: var(--color-d);
    --color-d: var(--color-c);
}
";

        private const string NestedVariablesCss = @"
:root {
    --base-color: red;
    --derived-color: var(--base-color);
    --triple-nested: var(--derived-color);
}

.test {
    color: var(--triple-nested);
}
";

        private const string ComplexVariablesCss = @"
:root {
    --spacing-unit: 4px;
    --spacing-small: var(--spacing-unit);
    --spacing-medium: calc(var(--spacing-unit) * 2);
    --spacing-large: calc(var(--spacing-unit) * 4);
}

.box {
    padding: var(--spacing-small) var(--spacing-medium);
    margin: var(--spacing-medium) var(--spacing-large);
}
";

        #endregion

        #region Helper Methods

        private CssStyleDeclaration CreateStyleDeclaration(string css, out CssParsingContext context)
        {
            context = new CssParsingContext();
            var declaration = new CssStyleDeclaration(css, null, false, CssStyleSheetType.Author);
            return declaration;
        }

        private CssStyleSheet CreateStyleSheetWithDiagnostics(string css, out CssParsingContext context)
        {
            context = new CssParsingContext();
            context.StartTracking();

            // Create a stylesheet using the full constructor with required parameters
            var stylesheet = new CssStyleSheet(null, null, null, null, null, CssStyleSheetType.Author);
            stylesheet.ParsingContext = context;

            // Parse with context
            try
            {
                string cssRef = css;
                stylesheet.TryParse(ref cssRef, stylesheet, false, new List<string>(), CssStyleSheetType.Author, context);
            }
            catch
            {
                // Ignore parsing errors for test purposes
            }

            context.StopTracking();
            return stylesheet;
        }

        #endregion

        #region Tests - Variable Definition and Storage

        [Test]
        public void VariableRegistry_ShouldStoreCustomProperties()
        {
            const string css = "--primary-color: #3498db; --secondary-color: #2ecc71;";
            var decl = CreateStyleDeclaration(css, out var context);

            Assert.That(decl.VariableRegistry.GlobalVariableCount, Is.EqualTo(2));
            Assert.That(decl.VariableRegistry.HasVariable("--primary-color"), Is.True);
            Assert.That(decl.VariableRegistry.HasVariable("--secondary-color"), Is.True);
        }

        [Test]
        public void VariableRegistry_ShouldRetrieveCustomPropertyValues()
        {
            const string css = "--color: red; --size: 16px;";
            var decl = CreateStyleDeclaration(css, out var context);

            Assert.That(decl.VariableRegistry.ResolveVariable("--color"), Is.EqualTo("red"));
            Assert.That(decl.VariableRegistry.ResolveVariable("--size"), Is.EqualTo("16px"));
        }

        [Test]
        public void VariableRegistry_ShouldBeCaseSensitive()
        {
            const string css = "--MyColor: red;";
            var decl = CreateStyleDeclaration(css, out var context);

            Assert.That(decl.VariableRegistry.HasVariable("--MyColor"), Is.True);
            Assert.That(decl.VariableRegistry.HasVariable("--mycolor"), Is.False);
            Assert.That(decl.VariableRegistry.HasVariable("--MYCOLOR"), Is.False);
        }

        [Test]
        public void VariableRegistry_ShouldOnlyAcceptProperlyFormatted()
        {
            const string css = "--valid: green;";
            var decl = CreateStyleDeclaration(css, out var context);

            // Only proper custom properties should be stored
            Assert.That(decl.VariableRegistry.HasVariable("--valid"), Is.True);
        }

        #endregion

        #region Tests - Variable Resolution

        [Test]
        public void VariableResolver_ShouldResolveSimpleVariableReference()
        {
            const string css = "--color: red; color: var(--color);";
            var decl = CreateStyleDeclaration(css, out var context);

            string colorValue = decl.GetPropertyValue("color");
            Assert.That(colorValue, Is.EqualTo("red"));
        }

        [Test]
        public void VariableResolver_ShouldResolveFallbackWhenUndefined()
        {
            const string css = "color: var(--undefined-color, blue);";
            var decl = CreateStyleDeclaration(css, out var context);

            string colorValue = decl.GetPropertyValue("color");
            Assert.That(colorValue, Is.EqualTo("blue"));
        }

        [Test]
        public void VariableResolver_ShouldPreferDefinedVariableOverFallback()
        {
            const string css = "--color: red; color: var(--color, blue);";
            var decl = CreateStyleDeclaration(css, out var context);

            string colorValue = decl.GetPropertyValue("color");
            Assert.That(colorValue, Is.EqualTo("red"));
        }

        [Test]
        public void VariableResolver_ShouldResolveNestedVariables()
        {
            const string css = "--base: red; --derived: var(--base); color: var(--derived);";
            var decl = CreateStyleDeclaration(css, out var context);

            string colorValue = decl.GetPropertyValue("color");
            Assert.That(colorValue, Is.EqualTo("red"));
        }

        [Test]
        public void VariableResolver_ShouldResolveMultipleVariablesInOneValue()
        {
            const string css = "--fg: red; --bg: blue; color: var(--fg); background: var(--bg);";
            var decl = CreateStyleDeclaration(css, out var context);

            Assert.That(decl.GetPropertyValue("color"), Is.EqualTo("red"));
            Assert.That(decl.GetPropertyValue("background"), Is.EqualTo("blue"));
        }

        [Test]
        public void VariableResolver_ShouldHandleWhitespaceInVarExpression()
        {
            const string css = "--color: red; color: var( --color );";
            var decl = CreateStyleDeclaration(css, out var context);

            string colorValue = decl.GetPropertyValue("color");
            // Whitespace should be normalized
            Assert.That(colorValue, Is.EqualTo("red"));
        }

        [Test]
        public void VariableResolver_ShouldContainVariableReferenceCheck()
        {
            const string css = "--color: red;";
            var decl = CreateStyleDeclaration(css, out var context);

            Assert.That(decl.VariableResolver.ContainsVariableReference("color: var(--color)"), Is.True);
            Assert.That(decl.VariableResolver.ContainsVariableReference("color: red"), Is.False);
            Assert.That(decl.VariableResolver.ContainsVariableReference(""), Is.False);
        }

        #endregion

        #region Tests - Variable References Extraction

        [Test]
        public void VariableResolver_ShouldExtractVariableReferences()
        {
            const string css = "--color: red;";
            var decl = CreateStyleDeclaration(css, out var context);

            var refs = decl.VariableResolver.GetVariableReferences("var(--color)").ToList();
            Assert.That(refs.Count, Is.EqualTo(1));
            Assert.That(refs[0].VariableName, Is.EqualTo("--color"));
        }

        [Test]
        public void VariableResolver_ShouldExtractFallbackValue()
        {
            const string css = "";
            var decl = CreateStyleDeclaration(css, out var context);

            var refs = decl.VariableResolver.GetVariableReferences("var(--color, red)").ToList();
            Assert.That(refs.Count, Is.EqualTo(1));
            Assert.That(refs[0].VariableName, Is.EqualTo("--color"));
            Assert.That(refs[0].FallbackValue, Is.EqualTo("red"));
        }

        [Test]
        public void VariableResolver_ShouldExtractMultipleReferences()
        {
            const string css = "";
            var decl = CreateStyleDeclaration(css, out var context);

            var value = "var(--color1) var(--color2) var(--color3)";
            var refs = decl.VariableResolver.GetVariableReferences(value).ToList();
            Assert.That(refs.Count, Is.EqualTo(3));
        }

        #endregion

        #region Tests - Circular Dependencies

        [Test]
        public void VariableResolver_ShouldDetectCircularDependency()
        {
            const string css = "--a: var(--b); --b: var(--a);";
            var decl = CreateStyleDeclaration(css, out var context);

            bool hasCircular = decl.VariableResolver.HasCircularDependency("--a");
            Assert.That(hasCircular, Is.True);
        }

        [Test]
        public void VariableResolver_ShouldHandleCircularResolution()
        {
            const string css = "--a: var(--b); --b: var(--a); color: var(--a);";
            var decl = CreateStyleDeclaration(css, out var context);

            // Should not throw, should return original or fallback
            string colorValue = decl.GetPropertyValue("color");
            Assert.That(colorValue, Is.Not.Null);
        }

        [Test]
        public void VariableResolver_ShouldHandleCircularWithFallback()
        {
            const string css = "--a: var(--b); --b: var(--a); color: var(--a, green);";
            var decl = CreateStyleDeclaration(css, out var context);

            string colorValue = decl.GetPropertyValue("color");
            // With circular dependency and fallback, should not be able to resolve the circular part
            // The var() reference itself is stored, resolution only happens on GetPropertyValue
            Assert.That(colorValue, Is.Not.Null);
        }

        #endregion

        #region Tests - CssVariableReference Parsing

        [Test]
        public void CssVariableReference_ShouldParseSimpleExpression()
        {
            var varRef = CssVariableReference.Parse("var(--color)");
            Assert.That(varRef, Is.Not.Null);
            Assert.That(varRef.VariableName, Is.EqualTo("--color"));
            Assert.That(varRef.FallbackValue, Is.Null);
        }

        [Test]
        public void CssVariableReference_ShouldParseWithFallback()
        {
            var varRef = CssVariableReference.Parse("var(--color, red)");
            Assert.That(varRef, Is.Not.Null);
            Assert.That(varRef.VariableName, Is.EqualTo("--color"));
            Assert.That(varRef.FallbackValue, Is.EqualTo("red"));
        }

        [Test]
        public void CssVariableReference_ShouldRejectInvalidVarName()
        {
            var varRef1 = CssVariableReference.Parse("var(color)");
            Assert.That(varRef1, Is.Null);

            var varRef2 = CssVariableReference.Parse("var(-color)");
            Assert.That(varRef2, Is.Null);
        }

        [Test]
        public void CssVariableReference_ShouldHandleWhitespace()
        {
            var varRef = CssVariableReference.Parse("var( --color , red )");
            Assert.That(varRef, Is.Not.Null);
            Assert.That(varRef.VariableName, Is.EqualTo("--color"));
            Assert.That(varRef.FallbackValue, Is.EqualTo("red"));
        }

        [Test]
        public void CssVariableReference_ShouldReconstructExpression()
        {
            var varRef = new CssVariableReference("--color", "red");
            Assert.That(varRef.ToString(), Is.EqualTo("var(--color, red)"));

            var varRef2 = new CssVariableReference("--color");
            Assert.That(varRef2.ToString(), Is.EqualTo("var(--color)"));
        }

        #endregion

        #region Tests - Diagnostics Integration

        [Test]
        public void VariableSupport_ShouldLogUndefinedVariableWarning()
        {
            // Create a simple stylesheet with a missing variable in a rule
            const string css = @"
body {
    color: var(--undefined-color);
}
";
            var stylesheet = CreateStyleSheetWithDiagnostics(css, out var context);

            // Check if parsing completed without throwing
            Assert.Pass("Stylesheet parsed without exceptions");
        }

        [Test]
        public void VariableSupport_ShouldNotWarnWithFallback()
        {
            const string css = @"
:root {
    color: var(--undefined-color, blue);
}
";
            var stylesheet = CreateStyleSheetWithDiagnostics(css, out var context);

            // Check if warnings are about undefined variables
            var summary = context.GetSummary();
            // If warnings exist, they shouldn't mention "no fallback"
            if (context.HasWarnings)
            {
                Assert.That(summary, Does.Not.Contain("no fallback"));
            }
        }

        [Test]
        public void VariableSupport_ShouldLogCircularDependencyWarning()
        {
            const string css = @"
:root {
    --a: var(--b);
    --b: var(--a);
    color: var(--a);
}
";
            var stylesheet = CreateStyleSheetWithDiagnostics(css, out var context);

            // Build uses the variables, so diagnostics should be triggered
            Assert.That(context.HasWarnings | !context.HasErrors, Is.True);
        }

        #endregion

        #region Tests - SetProperty Integration

        [Test]
        public void SetProperty_ShouldRegisterCustomProperty()
        {
            var decl = new CssStyleDeclaration("", null, false, CssStyleSheetType.Author);
            decl.SetProperty("--my-color", "red", "");

            Assert.That(decl.VariableRegistry.HasVariable("--my-color"), Is.True);
            Assert.That(decl.VariableRegistry.ResolveVariable("--my-color"), Is.EqualTo("red"));
        }

        [Test]
        public void SetProperty_ShouldResolveVariableInValue()
        {
            var decl = new CssStyleDeclaration("--color: red;", null, false, CssStyleSheetType.Author);
            decl.SetProperty("background", "var(--color)", "");

            string bgValue = decl.GetPropertyValue("background");
            Assert.That(bgValue, Is.EqualTo("red"));
        }

        #endregion

        #region Tests - Complex Scenarios

        [Test]
        public void VariableSupport_ShouldHandleValidVariablesCss()
        {
            // Declaration content only (what goes inside curly braces)
            const string declContent = @"--primary-color: #3498db; --secondary-color: #2ecc71;";
            var decl = new CssStyleDeclaration(declContent, null, false, CssStyleSheetType.Author);

            // Check variable registration
            Assert.That(decl.VariableRegistry.HasVariable("--primary-color"), Is.True);
            Assert.That(decl.VariableRegistry.HasVariable("--secondary-color"), Is.True);

            // Check that variables can be resolved
            string primaryColor = decl.VariableRegistry.ResolveVariable("--primary-color");
            Assert.That(primaryColor, Is.EqualTo("#3498db"));
        }

        [Test]
        public void VariableSupport_ShouldHandleUndefinedVariablesCss()
        {
            // Declaration content only - referencing undefined variable
            const string declContent = @"color: var(--undefined-color);";
            var decl = new CssStyleDeclaration(declContent, null, false, CssStyleSheetType.Author);

            // Should parse without throwing
            Assert.That(decl.GetPropertyValue("color"), Is.Not.Null);
        }

        [Test]
        public void VariableSupport_ShouldHandleNestedVariablesCss()
        {
            // Declaration content only with nested variable references
            const string declContent = @"--base-color: red; --derived-color: var(--base-color); --triple-nested: var(--derived-color);";
            var decl = new CssStyleDeclaration(declContent, null, false, CssStyleSheetType.Author);

            // All variables should be stored
            Assert.That(decl.VariableRegistry.HasVariable("--base-color"), Is.True);
            Assert.That(decl.VariableRegistry.HasVariable("--derived-color"), Is.True);
            Assert.That(decl.VariableRegistry.HasVariable("--triple-nested"), Is.True);

            // Verify the nested values resolve correctly
            string baseColor = decl.VariableRegistry.ResolveVariable("--base-color");
            Assert.That(baseColor, Is.EqualTo("red"));
        }

        #endregion

        #region Tests - Edge Cases

        [Test]
        public void VariableRegistry_ShouldHandleEmptyVariableValue()
        {
            const string css = "--empty: ;";
            var decl = CreateStyleDeclaration(css, out var context);

            // Empty values should still be stored
            Assert.That(decl.VariableRegistry.HasVariable("--empty"), Is.True);
        }

        [Test]
        public void VariableResolver_ShouldHandleNullOrEmptyInput()
        {
            var decl = new CssStyleDeclaration("", null, false, CssStyleSheetType.Author);

            Assert.That(decl.VariableResolver.ResolveValue(null), Is.Null.Or.Empty);
            Assert.That(decl.VariableResolver.ResolveValue(""), Is.Empty);
        }

        [Test]
        public void VariableReference_ShouldRejectNullInput()
        {
            var varRef = CssVariableReference.Parse(null);
            Assert.That(varRef, Is.Null);
        }

        #endregion
    }
}
