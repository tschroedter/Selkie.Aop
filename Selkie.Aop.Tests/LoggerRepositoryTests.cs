using System.Diagnostics.CodeAnalysis;
using Castle.Core.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Selkie.Aop.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class LoggerRepositoryTests
    {
        [SetUp]
        public void Setup()
        {
            m_LoggerOne = Substitute.For <ILogger>();
            m_LoggerTwo = Substitute.For <ILogger>();
            m_Factory = Substitute.For <ILoggerFactory>();

            m_Factory.Create(Arg.Any <string>()).Returns(m_LoggerOne,
                                                         m_LoggerTwo);

            m_Sut = new LoggerRepository(m_Factory);
        }

        private ILoggerFactory m_Factory;
        private ILogger m_LoggerOne;
        private ILogger m_LoggerTwo;
        private LoggerRepository m_Sut;

        [Test]
        public void Get_CreatesNewLogger_ForNewType()
        {
            // Arrange
            // Act
            ILogger actual = m_Sut.Get("One");

            // Assert
            Assert.NotNull(actual);
        }

        [Test]
        public void Get_ReturnsDifferentLogger_ForDifferentTypes()
        {
            // Arrange
            // Act
            ILogger one = m_Sut.Get("One");
            ILogger two = m_Sut.Get("Two");

            // Assert
            Assert.AreNotEqual(two,
                               one);
        }

        [Test]
        public void Get_ReturnsExistingLogger_ForExistingType()
        {
            // Arrange
            ILogger expected = m_Sut.Get("One");

            // Act
            ILogger actual = m_Sut.Get("One");

            // Assert
            Assert.AreEqual(expected,
                            actual);
        }
    }
}