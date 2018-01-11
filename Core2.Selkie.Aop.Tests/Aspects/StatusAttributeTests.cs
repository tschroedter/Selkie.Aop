using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Core2.Selkie.Aop.Aspects;

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