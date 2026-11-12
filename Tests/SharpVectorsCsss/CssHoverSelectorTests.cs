using System.Xml;
using NUnit.Framework;
using SharpVectors.Dom.Css;

namespace SharpVectors.Csss.Tests
{
    [TestFixture]
    public class CssHoverSelectorTests
    {
        [Test]
        public void Hover_IsInactiveInStaticDocument()
        {
            var document = new XmlDocument();
            document.LoadXml("<g class='shaper'><g class='liner'/></g>");
            var navigator = document.DocumentElement.FirstChild.CreateNavigator();
            var hover = new CssXPathSelector(".shaper:hover .liner");
            var notHover = new CssXPathSelector(".shaper:not(:hover) .liner");
            var normal = new CssXPathSelector(".shaper .liner");

            Assert.That(hover.Matches(navigator), Is.False);
            Assert.That(notHover.Matches(navigator), Is.True);
            Assert.That(normal.Matches(navigator), Is.True);
            Assert.That(hover.Specificity, Is.EqualTo(30));
            Assert.That(notHover.Specificity, Is.EqualTo(30));
        }
    }
}
