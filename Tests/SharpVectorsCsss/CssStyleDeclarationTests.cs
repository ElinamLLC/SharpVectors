using System;
using NUnit.Framework;

using SharpVectors.Dom;
using SharpVectors.Dom.Css;

namespace SharpVectors.Csss.Tests
{
    [TestFixture]
    public class CssStyleDeclarationTests
    {
        [Test]
        public void SetProperty_StoresTheValueAndPriority()
        {
            var declaration = CssHelper.CreateDeclaration();

            declaration.SetProperty("color", "red", "important");

            Assert.That(declaration.GetPropertyValue("color"), Is.EqualTo("red"));
            Assert.That(declaration.GetPropertyPriority("color"), Is.EqualTo("important"));
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
        public void ReadOnlyDeclaration_ThrowsOnModification()
        {
            var declaration = CssHelper.CreateReadOnlyDeclaration();

            // Attempting to modify should throw or fail gracefully
            Assert.Throws<DomException>(() =>
                declaration.SetProperty("color", "red", "")
            );
        }

        [Test]
        public void InlineStyleDeclaration_PropertiesAreAccessible()
        {
            var declaration = CssHelper.CreateInlineDeclaration("display: flex; justify-content: center;");

            Assert.That(declaration.GetPropertyValue("display"), Is.EqualTo("flex"));
            Assert.That(declaration.GetPropertyValue("justify-content"), Is.EqualTo("center"));
        }
    }
}