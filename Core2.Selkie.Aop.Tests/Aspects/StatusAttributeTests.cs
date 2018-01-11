using System.Diagnostics.CodeAnalysis;
using Core2.Selkie.Aop.Aspects;
using NUnit.Framework;

namespace Core2.Selkie.Aop.Tests.Aspects
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class StatusAttributeTests
    {
        [Test]
        public void Constructor_SetsText_WhenCalled()
        {
            // Arrange
            var sut = new StatusAttribute("Test");

            // Act
            // Assert
            Assert.AreEqual("Test",
                            sut.Text);
        }
    }
}