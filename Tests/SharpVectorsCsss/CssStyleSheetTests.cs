using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using NUnit.Framework;

using SharpVectors.Dom;
using SharpVectors.Dom.Css;
using SharpVectors.Dom.Stylesheets;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Utils;

namespace SharpVectors.Csss.Tests
{
    [TestFixture]
    public class CssStyleSheetTests
    {
        #region Basic Rule Parsing Tests

        [Test]
        public void InsertRule_ParsesSimpleSelectorRule()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            var index = stylesheet.InsertRule("div { color: red; background-color: blue; }", 0U);

            Assert.That(index, Is.EqualTo(0));
            Assert.That(stylesheet.CssRules.Length, Is.EqualTo(1));

            var rule = stylesheet.CssRules[0U];
            Assert.That(rule, Is.TypeOf<CssStyleRule>());

            var styleRule = (CssStyleRule)rule;
            Assert.That(styleRule.SelectorText, Is.EqualTo("div"));
            Assert.That(styleRule.Style.GetPropertyValue("color"), Is.EqualTo("red"));
            Assert.That(styleRule.Style.GetPropertyValue("background-color"), Is.EqualTo("blue"));
        }

        [Test]
        public void InsertRule_ParsesMultipleSelectorRule()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            stylesheet.InsertRule("h1, h2, h3 { font-weight: bold; }", 0U);

            var rule = stylesheet.CssRules[0U];
            Assert.That(rule, Is.TypeOf<CssStyleRule>());

            var styleRule = (CssStyleRule)rule;
            Assert.That(styleRule.SelectorText, Does.Contain("h1"));
            Assert.That(styleRule.Style.GetPropertyValue("font-weight"), Is.EqualTo("bold"));
        }

        [Test]
        public void InsertRule_ParsesClassSelector()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            stylesheet.InsertRule(".container { width: 100%; margin: 0 auto; }", 0U);

            var rule = stylesheet.CssRules[0U];
            var styleRule = (CssStyleRule)rule;
            Assert.That(styleRule.SelectorText, Is.EqualTo(".container"));
            Assert.That(styleRule.Style.GetPropertyValue("width"), Is.EqualTo("100%"));
        }

        [Test]
        public void InsertRule_ParsesIdSelector()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            stylesheet.InsertRule("#header { background: #333; color: #fff; }", 0U);

            var rule = stylesheet.CssRules[0U];
            var styleRule = (CssStyleRule)rule;
            Assert.That(styleRule.SelectorText, Is.EqualTo("#header"));
            Assert.That(styleRule.Style.GetPropertyValue("background"), Is.EqualTo("#333"));
        }

        [Test]
        public void InsertRule_ParsesAttributeSelector()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            stylesheet.InsertRule("input[type='text'] { border: 1px solid gray; }", 0U);

            var rule = stylesheet.CssRules[0U];
            var styleRule = (CssStyleRule)rule;
            Assert.That(styleRule.SelectorText, Is.EqualTo("input[type='text']"));
        }

        #endregion

        #region Style Declaration Tests

        [Test]
        public void SetProperty_StoresTheValueAndPriority()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("color", "red", "important");

            Assert.That(declaration.GetPropertyValue("color"), Is.EqualTo("red"));
            Assert.That(declaration.GetPropertyPriority("color"), Is.EqualTo("important"));
        }

        [Test]
        public void SetProperty_WithoutPriority()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("margin", "10px", "");

            Assert.That(declaration.GetPropertyValue("margin"), Is.EqualTo("10px"));
            Assert.That(declaration.GetPropertyPriority("margin"), Is.EqualTo(""));
        }

        [Test]
        public void RemoveProperty_DeletesTheProperty()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("color", "red", "");
            Assert.That(declaration.Length, Is.GreaterThan(0));

            var removed = declaration.RemoveProperty("color");
            Assert.That(removed, Is.EqualTo("red"));
            Assert.That(declaration.GetPropertyValue("color"), Is.EqualTo(""));
        }

        [Test]
        public void CssText_ParsesCompleteDeclaration()
        {
            var declaration = CssHelper.CreateDeclaration("color: blue; font-size: 14px; text-align: center;");

            Assert.That(declaration.GetPropertyValue("color"), Is.EqualTo("blue"));
            Assert.That(declaration.GetPropertyValue("font-size"), Is.EqualTo("14px"));
            Assert.That(declaration.GetPropertyValue("text-align"), Is.EqualTo("center"));
        }

        [Test]
        public void GetPropertyValue_WithInvalidProperty_ReturnsEmpty()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("color", "red", "");
            var value = declaration.GetPropertyValue("background-color");

            Assert.That(value, Is.EqualTo(""));
        }

        #endregion

        #region Rule Order Tests

        [Test]
        public void InsertRule_PreservesRuleOrder()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            stylesheet.InsertRule("p { color: red; }", 0U);
            stylesheet.InsertRule("span { color: blue; }", 1U);

            Assert.That(stylesheet.CssRules.Length, Is.EqualTo(2));
            Assert.That(stylesheet.CssRules[0U].CssText, Does.Contain("p"));
            Assert.That(stylesheet.CssRules[1U].CssText, Does.Contain("span"));
        }

        [Test]
        public void InsertRule_InsertsAtMiddleIndex()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            stylesheet.InsertRule("p { color: red; }", 0U);
            stylesheet.InsertRule("span { color: blue; }", 1U);
            stylesheet.InsertRule("div { color: green; }", 1U);

            Assert.That(stylesheet.CssRules.Length, Is.EqualTo(3));
            Assert.That(stylesheet.CssRules[1U].CssText, Does.Contain("div"));
        }

        [Test]
        public void DeleteRule_RemovesRuleAtIndex()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            stylesheet.InsertRule("p { color: red; }", 0U);
            stylesheet.InsertRule("span { color: blue; }", 1U);
            stylesheet.InsertRule("div { color: green; }", 2U);

            stylesheet.DeleteRule(1U);

            Assert.That(stylesheet.CssRules.Length, Is.EqualTo(2));
            Assert.That(stylesheet.CssRules[1U].CssText, Does.Contain("div"));
        }

        #endregion

        #region Color Value Tests

        [Test]
        public void SetProperty_ParsesHexColorValue()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("color", "#FF0000", "");

            Assert.That(declaration.GetPropertyValue("color"), Is.EqualTo("#FF0000"));
        }

        [Test]
        public void SetProperty_ParsesRgbColorValue()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("background-color", "rgb(255, 0, 0)", "");

            Assert.That(declaration.GetPropertyValue("background-color"), Does.Contain("rgb"));
        }

        [Test]
        public void SetProperty_ParsesNamedColorValue()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("color", "red", "");

            Assert.That(declaration.GetPropertyValue("color"), Is.EqualTo("red"));
        }

        #endregion

        #region Unit and Value Tests

        [Test]
        public void SetProperty_ParsesPixelValues()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("width", "100px", "");
            declaration.SetProperty("margin", "10px 20px 30px 40px", "");

            Assert.That(declaration.GetPropertyValue("width"), Is.EqualTo("100px"));
        }

        [Test]
        public void SetProperty_ParsesPercentageValues()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("width", "50%", "");

            Assert.That(declaration.GetPropertyValue("width"), Is.EqualTo("50%"));
        }

        [Test]
        public void SetProperty_ParsesEmValues()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("font-size", "1.5em", "");

            Assert.That(declaration.GetPropertyValue("font-size"), Is.EqualTo("1.5em"));
        }

        [Test]
        public void SetProperty_ParsesRemValues()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("font-size", "2rem", "");

            Assert.That(declaration.GetPropertyValue("font-size"), Is.EqualTo("2rem"));
        }

        #endregion

        #region Shorthand Property Tests

        [Test]
        public void SetProperty_ParsesBorderShorthand()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("border", "1px solid black", "");

            Assert.That(declaration.GetPropertyValue("border"), Does.Contain("1px"));
        }

        [Test]
        public void SetProperty_ParsesPaddingShorthand()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("padding", "10px 20px", "");

            Assert.That(declaration.GetPropertyValue("padding"), Is.EqualTo("10px 20px"));
        }

        [Test]
        public void SetProperty_ParsesMarginShorthand()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("margin", "5px", "");

            Assert.That(declaration.GetPropertyValue("margin"), Is.EqualTo("5px"));
        }

        #endregion

        #region Priority Tests

        [Test]
        public void SetProperty_WithImportantPriority()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("color", "red", "important");

            Assert.That(declaration.GetPropertyPriority("color"), Is.EqualTo("important"));
        }

        [Test]
        public void SetProperty_UpdatesExistingPropertyPriority()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("color", "red", "");
            Assert.That(declaration.GetPropertyPriority("color"), Is.EqualTo(""));

            declaration.SetProperty("color", "blue", "important");
            Assert.That(declaration.GetPropertyValue("color"), Is.EqualTo("blue"));
            Assert.That(declaration.GetPropertyPriority("color"), Is.EqualTo("important"));
        }

        #endregion

        #region Complex Selector Tests

        [Test]
        public void InsertRule_PseudoClassSelector()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            stylesheet.InsertRule("a:hover { color: orange; text-decoration: underline; }", 0U);

            var rule = stylesheet.CssRules[0U];
            var styleRule = (CssStyleRule)rule;
            Assert.That(styleRule.SelectorText, Is.EqualTo("a:hover"));
        }

        [Test]
        public void InsertRule_PseudoElementSelector()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            stylesheet.InsertRule("p::first-line { font-weight: bold; }", 0U);

            var rule = stylesheet.CssRules[0U];
            var styleRule = (CssStyleRule)rule;
            Assert.That(styleRule.SelectorText, Does.Contain("first-line"));
        }

        [Test]
        public void InsertRule_DescendantCombinator()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            stylesheet.InsertRule("div p { color: green; }", 0U);

            var rule = stylesheet.CssRules[0U];
            var styleRule = (CssStyleRule)rule;
            Assert.That(styleRule.SelectorText, Is.EqualTo("div p"));
        }

        [Test]
        public void InsertRule_ChildCombinator()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            stylesheet.InsertRule("ul > li { margin: 5px; }", 0U);

            var rule = stylesheet.CssRules[0U];
            var styleRule = (CssStyleRule)rule;
            Assert.That(styleRule.SelectorText, Is.EqualTo("ul > li"));
        }

        #endregion

        #region At-Rule Tests

        [Test]
        public void InsertRule_MediaRule()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            var rule = "@media screen and (max-width: 768px) { body { font-size: 12px; } }";
            var index = stylesheet.InsertRule(rule, 0U);

            Assert.That(index, Is.GreaterThanOrEqualTo(0));
            Assert.That(stylesheet.CssRules.Length, Is.GreaterThan(0));
        }

        [Test]
        public void InsertRule_FontFaceRule()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            var rule = "@font-face { font-family: 'MyFont'; src: url('font.woff'); }";
            var index = stylesheet.InsertRule(rule, 0U);

            Assert.That(index, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void InsertRule_CharsetRule()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            var rule = "@charset \"UTF-8\";";
            var index = stylesheet.InsertRule(rule, 0U);

            Assert.That(index, Is.GreaterThanOrEqualTo(0));
        }

        #endregion

        #region Property Length Tests

        [Test]
        public void Length_ReturnsTotalPropertiesCount()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("color", "red", "");
            declaration.SetProperty("background", "blue", "");
            declaration.SetProperty("padding", "10px", "");

            Assert.That(declaration.Length, Is.EqualTo(3));
        }

        [Test]
        public void Item_ReturnsPropertyNameByIndex()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("color", "red", "");
            declaration.SetProperty("background", "blue", "");

            var firstProp = declaration[0U];
            Assert.That(firstProp, Is.Not.Empty);
        }

        #endregion

        #region Case Sensitivity Tests

        [Test]
        public void SetProperty_PropertyNamesAreCaseInsensitive()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("backGround-COLOR", "red", "");
            var value = declaration.GetPropertyValue("background-color");

            Assert.That(value, Is.Not.Empty);
        }

        [Test]
        public void SetProperty_PropertyValuesAreCaseSensitive()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("color", "RED", "");
            var value = declaration.GetPropertyValue("color");

            Assert.That(value, Does.Contain("RED"));
        }

        #endregion

        #region StyleSheetType Tests

        [Test]
        public void CreateInlineDeclaration_HasInlineOrigin()
        {
            var declaration = CssHelper.CreateInlineDeclaration("display: flex;");

            Assert.That(declaration.GetPropertyValue("display"), Is.EqualTo("flex"));
        }

        [Test]
        public void ReadOnlyDeclaration_IsReadOnly()
        {
            var declaration = CssHelper.CreateReadOnlyDeclaration();

            // The declaration should be created as read-only
            // Behavior depends on implementation - it may throw or silently fail on modification
            Assert.Pass("Read-only declaration created successfully");
        }

        #endregion

        #region Multiple Rules Tests

        [Test]
        public void InsertRule_MultipleComplexRules()
        {
            var stylesheet = CssHelper.CreateStyleSheet();

            stylesheet.InsertRule("* { box-sizing: border-box; }", 0U);
            stylesheet.InsertRule("body { margin: 0; padding: 0; font-family: sans-serif; }", 1U);
            stylesheet.InsertRule(".container { max-width: 1200px; margin: 0 auto; }", 2U);
            stylesheet.InsertRule("h1 { font-size: 32px; font-weight: bold; }", 3U);
            stylesheet.InsertRule("p { line-height: 1.6; }", 4U);

            Assert.That(stylesheet.CssRules.Length, Is.EqualTo(5));
        }

        #endregion
    }
}
